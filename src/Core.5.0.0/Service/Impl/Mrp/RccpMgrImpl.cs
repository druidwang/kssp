using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Utility;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.PRD;

namespace com.Sconit.Service.MRP.Impl
{
    [Transactional]
    public class RccpMgrImpl : BaseMgr, IRccpMgr
    {
        //public IGenericMgr genericMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }

        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.MRP.RunRccp");

        [Transaction(TransactionMode.Requires)]
        public void RunRccp(CodeMaster.TimeUnit dateType)
        {
            User user = SecurityContextHolder.Get();
            DateTime planVersion = DateTime.Now;
            var snapTime = this.genericMgr.FindAll<MrpSnapMaster>
            (@"from MrpSnapMaster m where m.IsRelease =? and m.Type=? order by m.SnapTime desc",
            new object[] { true, CodeMaster.SnapType.Rccp }, 0, 1).First().SnapTime;
            string dateIndex = string.Empty;
            if(dateType == CodeMaster.TimeUnit.Week)
            {
                dateIndex = Utility.DateTimeHelper.GetWeekOfYear(planVersion);
            }
            else if(dateType == CodeMaster.TimeUnit.Month)
            {
                dateIndex = planVersion.ToString("yyyy-MM");
            }
            RunRccp(planVersion, snapTime, dateType, dateIndex, user);
        }

        private static object RunRccpLock = new object();
        [Transaction(TransactionMode.Requires)]
        public void RunRccp(DateTime planVersion, DateTime snapTime, CodeMaster.TimeUnit dateType, string dateIndex, User user)
        {
            lock(RunRccpLock)
            {
                planVersion = DateTime.Parse(planVersion.ToString("yyyy-MM-dd HH:mm:ss"));
                SecurityContextHolder.Set(user);
                log.Info(string.Format("---**------ SnapTime:{0} PlanVersion:{1} - 开始执行预测计划 ---", snapTime, planVersion));
                BusinessException businessException = new BusinessException();
                int rccpPlanVersion = 0;
                try
                {
                    #region 获取RccpPlan
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 开始获取RccpPlan ---", snapTime, planVersion));
                    var rccpPlans = this.genericMgr.FindAll<RccpPlan>
                        (@"from RccpPlan as m where m.DateIndex >= ? and DateType=? ", new object[] { dateIndex, dateType });
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 结束获取RccpPlan ---", snapTime, planVersion));
                    rccpPlanVersion = rccpPlans.Max(p => p.PlanVersion);
                    #endregion

                    #region 获取路线
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 开始获取路线 ---", snapTime, planVersion));
                    var mrpFlowDetailList = this.genericMgr.FindAll<MrpFlowDetail>
                        (@"from MrpFlowDetail as m where m.SnapTime = ?", new object[] { snapTime });
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 结束获取路线 ---", snapTime, planVersion));
                    #endregion

                    #region 分解BOM
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 开始分解Bom ---", snapTime, planVersion));
                    var rccpTransList = GetRccpTrans(planVersion, rccpPlans, businessException);

                    var rccpTransGroupList = (from r in rccpTransList
                                              group r by new
                                              {
                                                  Item = r.Item,
                                                  DateIndex = r.DateIndex,
                                                  IsLastLevel = r.IsLastLevel,
                                                  DateType = r.DateType
                                              } into g
                                              select new RccpTransGroup
                                              {
                                                  PlanVersion = planVersion,
                                                  DateType = g.Key.DateType,
                                                  Item = g.Key.Item,
                                                  DateIndex = g.Key.DateIndex,
                                                  IsLastLevel = g.Key.IsLastLevel,
                                                  Qty = g.Sum(r => r.Qty),
                                                  ScrapPercentage = g.Sum(s => s.Qty) > 0 ? g.Sum(r => r.ScrapPercentage * (r.Qty / g.Sum(s => s.Qty))) : 0
                                              }).ToList();

                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 结束分解Bom ---", snapTime, planVersion));
                    #endregion

                    #region 后加工
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 开始执行后加工计划 ---", snapTime, planVersion));
                    GenFiRccp(planVersion, dateType, mrpFlowDetailList, rccpTransList, businessException);
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 结束执行后加工计划 ---", snapTime, planVersion));
                    #endregion 后加工

                    #region 炼胶 委外
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 开始执行炼胶计划 ---", snapTime, planVersion));
                    GenMiRccp(planVersion, dateType, dateIndex, mrpFlowDetailList, rccpTransGroupList, businessException);
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 结束执行炼胶计划 ---", snapTime, planVersion));
                    #endregion 炼胶

                    #region 采购/委外等
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 开始执行采购/委外计划 ---", snapTime, planVersion));
                    GenPurchaseRccp(planVersion, dateType, businessException, mrpFlowDetailList, rccpTransGroupList, snapTime, user);
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 结束执行采购/委外计划 ---", snapTime, planVersion));
                    #endregion 采购/委外等

                    #region Create RccpTransGroup
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 开始记录RccpTrans/Group ---", snapTime, planVersion));
                    this.genericMgr.BulkInsert<RccpTransGroup>(rccpTransGroupList);
                    log.Info(string.Format("--- SnapTime:{0} PlanVersion:{1} - 结束记录RccpTrans/Group ---", snapTime, planVersion));
                    #endregion
                }
                catch(Exception ex)
                {
                    businessException.AddMessage(new Message(CodeMaster.MessageType.Error, ex.StackTrace));
                    log.Error(ex);
                }

                List<RccpLog> rccpLogs = new List<RccpLog>();
                CodeMaster.MessageType status = CodeMaster.MessageType.Info;
                if(businessException.HasMessage)
                {
                    var messages = businessException.GetMessages().GroupBy(p =>
                                     new { Message = p.GetMessageString(), MessageType = p.MessageType },
                                     (k, g) => new { k.Message, k.MessageType });
                    foreach(var message in messages)
                    {
                        RccpLog rccpLog = new RccpLog();
                        rccpLog.ErrorLevel = message.MessageType.ToString();
                        rccpLog.Message = message.Message;
                        rccpLog.Logger = "RunRccp";
                        rccpLog.PlanVersion = planVersion;
                        rccpLogs.Add(rccpLog);
                        //this.genericMgr.Create(rccpLog);

                        if(message.MessageType == CodeMaster.MessageType.Warning)
                        {
                            log.Warn(rccpLog.Message);
                        }
                        else if(message.MessageType == CodeMaster.MessageType.Error)
                        {
                            log.Error(rccpLog.Message);
                        }
                        else
                        {
                            log.Info(rccpLog.Message);
                        }
                    }
                    if(messages.Count(f => f.MessageType == CodeMaster.MessageType.Error) > 0)
                    {
                        status = CodeMaster.MessageType.Error;
                    }
                    else if(messages.Count(f => f.MessageType == CodeMaster.MessageType.Warning) > 0)
                    {
                        status = CodeMaster.MessageType.Warning;
                    }
                }

                #region 记录RccpPlanMaster
                RccpPlanMaster rccpPlanMaster = new RccpPlanMaster();
                rccpPlanMaster.DateType = dateType;
                rccpPlanMaster.SnapTime = snapTime;
                rccpPlanMaster.PlanVersion = planVersion;
                rccpPlanMaster.Status = status;
                rccpPlanMaster.RccpPlanVersion = rccpPlanVersion;

                rccpPlanMaster.CreateUserId = user.Id;
                rccpPlanMaster.CreateUserName = user.FullName;
                rccpPlanMaster.CreateDate = DateTime.Now;
                rccpPlanMaster.LastModifyUserId = user.Id;
                rccpPlanMaster.LastModifyUserName = user.FullName;
                rccpPlanMaster.LastModifyDate = DateTime.Now;

                this.genericMgr.Create(rccpPlanMaster);
                #endregion

                double timetick = 1001 - (rccpPlanMaster.CreateDate - planVersion).TotalMilliseconds;
                timetick = timetick > 0 ? timetick : 0;
                Thread.Sleep((int)timetick);

                string infoMessage = string.Format("完成预测计划时间:{0},时间总计:{1}秒", DateTime.Now.ToLocalTime(), (DateTime.Now - planVersion).TotalSeconds);
                log.Info(infoMessage);
                RccpLog infoLog = new RccpLog();
                infoLog.ErrorLevel = CodeMaster.MessageType.Info.ToString();
                infoLog.Message = infoMessage;
                infoLog.Logger = "RunRccp";
                infoLog.PlanVersion = planVersion;
                rccpLogs.Add(infoLog);
                this.genericMgr.BulkInsert<RccpLog>(rccpLogs);
            }
        }

        private void GenMiRccp(DateTime planVersion, CodeMaster.TimeUnit dateType, string dateIndex,
            IList<MrpFlowDetail> mrpFlowDetailList, List<RccpTransGroup> rccpTransGroupList, BusinessException businessException)
        {
            var itemDiscontinueList = this.genericMgr.FindAll<ItemDiscontinue>();
            var rccpMiPlanList = new List<RccpMiPlan>();

            var workCalendars = this.genericMgr.FindAll<WorkCalendar>
                (@" from WorkCalendar as w where w.DateType =? and w.ResourceGroup=? and w.DateIndex between ? and ? ",
                new object[] { dateType, CodeMaster.ResourceGroup.MI, dateIndex, rccpTransGroupList.Max(p => p.DateIndex) });

            double cleanTime = double.Parse(systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.MiCleanTime));

            var miFlowDetailList = mrpFlowDetailList.Where(p => p.ResourceGroup == CodeMaster.ResourceGroup.MI);

            foreach(var groupRccpTrans in rccpTransGroupList)
            {
                DateTime dateFrom = DateTime.Now;
                if(dateType == CodeMaster.TimeUnit.Week)
                {
                    dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(groupRccpTrans.DateIndex);
                }
                else
                {
                    dateFrom = DateTime.Parse(groupRccpTrans.DateIndex + "-01");
                }

                var mrpFlowDetail = miFlowDetailList.FirstOrDefault(p => p.ResourceGroup == CodeMaster.ResourceGroup.MI
                    && p.Item == groupRccpTrans.Item && p.StartDate <= dateFrom && p.EndDate > dateFrom);

                if(mrpFlowDetail != null)
                {
                    var workCalendar = workCalendars.FirstOrDefault(p => p.DateIndex == groupRccpTrans.DateIndex
                              && p.Flow == mrpFlowDetail.Flow);
                    var miPlan = new RccpMiPlan();
                    DateTime planDate = DateTime.Now;

                    if(workCalendar != null)
                    {
                        miPlan.HaltTime = workCalendar.HaltTime * 24 * 60;
                        miPlan.TrialProduceTime = workCalendar.TrialTime * 24 * 60;
                        miPlan.Holiday = workCalendar.Holiday * 24 * 60;
                        miPlan.UpTime = workCalendar.UpTime * 24 * 60;
                        if(dateType == CodeMaster.TimeUnit.Month)
                        {
                            planDate = DateTime.Parse(groupRccpTrans.DateIndex + "-01");
                            //miPlan.UpTime = DateTime.DaysInMonth(planDate.Year, planDate.Month) * 24 * 60 * ((8 * 60 - cleanTime) / (8 * 60));
                            //miPlan.UpTime = workCalendar.UpTime * 24 * 60;
                        }
                        else if(dateType == CodeMaster.TimeUnit.Week)
                        {
                            planDate = Utility.DateTimeHelper.GetWeekIndexDateFrom(groupRccpTrans.DateIndex);
                            //miPlan.HaltTime = (7 - workCalendar.Holiday) * 24 * 60 * (cleanTime / (8 * 60));
                            //miPlan.UpTime = (7 - workCalendar.Holiday) * 24 * 60 * ((8 * 60 - cleanTime) / (8 * 60));
                        }
                    }
                    else
                    {
                        //出错
                        businessException.AddMessage("没有找到炼胶的工作日历");
                    }
                    miPlan.ProductLine = mrpFlowDetail.Flow;
                    miPlan.Item = groupRccpTrans.Item;
                    miPlan.DateIndex = groupRccpTrans.DateIndex;
                    miPlan.DateType = dateType;
                    miPlan.PlanVersion = planVersion;

                    miPlan.Qty = groupRccpTrans.Qty;
                    var miItem = this.itemMgr.GetCacheItem(groupRccpTrans.Item);
                    if(miItem == null)
                    {
                        businessException.AddMessage(new Message(CodeMaster.MessageType.Error, string.Format("没有找到此物料{0}的对应的工时", groupRccpTrans.Item)));
                    }
                    else
                    {
                        miPlan.WorkHour = miItem.WorkHour;
                    }
                    miPlan.CheRateQty = mrpFlowDetail.UnitCount;
                    //替代物料
                    var itemDiscontinues = itemDiscontinueList.Where(p => p.Item == miPlan.Item && p.StartDate <= planDate
                          && (!p.EndDate.HasValue || (p.EndDate.HasValue && p.EndDate.Value > planDate))).OrderBy(p => p.Priority).ToList();

                    var items = new List<string>();
                    items.Add(miPlan.Item);
                    items.AddRange(itemDiscontinues.Select(p => p.DiscontinueItem));
                    //可委外的物料
                    var flowDetail = mrpFlowDetailList.FirstOrDefault(f => f.Type == CodeMaster.OrderType.SubContract && items.Contains(f.Item));
                    if(flowDetail != null)
                    {
                        miPlan.SubFlowDetail = flowDetail;
                    }
                    rccpMiPlanList.Add(miPlan);
                    //this.genericMgr.Create(miPlan);
                }
            }

            var groupMiPlans = (from p in rccpMiPlanList
                                group p by new
                                {
                                    p.ProductLine,
                                    p.DateIndex,
                                    p.UpTime
                                } into g
                                select new
                                {
                                    ProductLine = g.Key.ProductLine,
                                    DateIndex = g.Key.DateIndex,
                                    UpTime = g.Key.UpTime,
                                    List = g
                                }).OrderBy(p => p.DateIndex).ThenBy(p => p.ProductLine);

            var purchasePlanList = new List<PurchasePlan>();
            foreach(var groupMiPlan in groupMiPlans)
            {
                double requireTime = groupMiPlan.List.Sum(p => p.RequireTime);
                double currentTime = requireTime - groupMiPlan.UpTime;

                if(currentTime > 0)
                {
                    DateTime dateFrom = DateTime.Now;
                    if(dateType == CodeMaster.TimeUnit.Week)
                    {
                        dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(groupMiPlan.DateIndex);
                    }
                    else
                    {
                        dateFrom = DateTime.Parse(groupMiPlan.DateIndex + "-01");
                    }

                    foreach(var plan in groupMiPlan.List)
                    {
                        if(plan.SubFlowDetail != null)
                        {
                            double currentQty = (currentTime / plan.WorkHour) * plan.CheRateQty;
                            double subQty = currentQty > plan.Qty ? plan.Qty : currentQty;

                            currentTime = currentTime - (subQty / plan.CheRateQty) * plan.WorkHour;

                            var purchasePlan = new PurchasePlan();

                            purchasePlan.Item = plan.Item;
                            //purchasePlan.Sequence = plan.SubFlowDetail.Sequence;
                            purchasePlan.Flow = plan.SubFlowDetail.Flow;
                            purchasePlan.LocationTo = plan.SubFlowDetail.LocationTo;
                            purchasePlan.OrderType = plan.SubFlowDetail.Type;
                            purchasePlan.WindowTime = dateFrom;
                            purchasePlan.StartTime = dateFrom.AddHours(-plan.SubFlowDetail.LeadTime);
                            purchasePlan.Qty = subQty;
                            purchasePlan.PlanQty = subQty;
                            purchasePlan.DateType = dateType;
                            purchasePlan.PlanVersion = planVersion;
                            purchasePlanList.Add(purchasePlan);

                            plan.SubQty = subQty;
                            plan.Qty -= subQty;
                            //auto update
                        }
                    }
                }
            }

            #region Create
            this.genericMgr.BulkInsert<RccpMiPlan>(rccpMiPlanList);
            this.genericMgr.BulkInsert<PurchasePlan>(purchasePlanList);
            #endregion Create
        }

        #region Private Methods

        private void GenPurchaseRccp(DateTime planVersion, CodeMaster.TimeUnit dateType, BusinessException businessException,
            IList<MrpFlowDetail> mrpFlowDetailList, IEnumerable<RccpTransGroup> rccpTransGroupList, DateTime snapTime, User user)
        {
            if(businessException.GetMessages().Where(p => p.MessageType == CodeMaster.MessageType.Error).Count() > 0)
            {
                //如果有错误,退出,不产生采购物料需求
                return;
            }

            var flowDetails = mrpFlowDetailList
                  .Where(p => p.Type == CodeMaster.OrderType.Procurement || p.Type == CodeMaster.OrderType.CustomerGoods
                   || p.Type == CodeMaster.OrderType.SubContract || p.Type == CodeMaster.OrderType.ScheduleLine);

            var purchasePlanList = new List<PurchasePlan>();
            var rccpTransGroupByIndexList = (from p in rccpTransGroupList
                                             group p by p.DateIndex into g
                                             select new
                                             {
                                                 DateIndex = g.Key,
                                                 List = g
                                             }).OrderBy(p => p.DateIndex).ToList();

            foreach(var rccpTransGroupByIndex in rccpTransGroupByIndexList)
            {
                DateTime windowTime = DateTime.Now;
                if(dateType == CodeMaster.TimeUnit.Week)
                {
                    windowTime = DateTimeHelper.GetWeekIndexDateFrom(rccpTransGroupByIndex.DateIndex);
                }
                else if(dateType == CodeMaster.TimeUnit.Month)
                {
                    windowTime = DateTime.Parse(rccpTransGroupByIndex.DateIndex + "-01");
                }
                var mrpFlowDetailDic = flowDetails.Where(p => p.StartDate <= windowTime && p.EndDate > windowTime)
                    .GroupBy(p => p.Item, (k, g) => new { k, g })
                    .ToDictionary(d => d.k, d => d.g);

                foreach(var groupRccpTrans in rccpTransGroupByIndex.List)
                {
                    var mrpFlowDetails = mrpFlowDetailDic.ValueOrDefault(groupRccpTrans.Item);
                    if(mrpFlowDetails != null)
                    {
                        foreach(var mrpFlowDetail in mrpFlowDetails)
                        {
                            var purchasePlan = new PurchasePlan();
                            purchasePlan.Item = groupRccpTrans.Item;
                            //purchasePlan.Sequence = mrpFlowDetail.Sequence;
                            purchasePlan.Flow = mrpFlowDetail.Flow;
                            purchasePlan.LocationTo = mrpFlowDetail.LocationTo;
                            purchasePlan.OrderType = mrpFlowDetail.Type;
                            purchasePlan.WindowTime = windowTime;
                            var leadDay = Utility.DateTimeHelper.TimeTranfer((decimal)mrpFlowDetail.LeadTime, CodeMaster.TimeUnit.Hour, CodeMaster.TimeUnit.Day);
                            if(dateType == CodeMaster.TimeUnit.Week)
                            {
                                purchasePlan.StartTime = purchasePlan.WindowTime.AddDays(3).AddDays(-leadDay);
                                purchasePlan.StartTime = Utility.DateTimeHelper.GetWeekStart(purchasePlan.StartTime);
                            }
                            else
                            {
                                purchasePlan.StartTime = purchasePlan.WindowTime.AddDays(15).AddDays(-leadDay);
                                purchasePlan.StartTime = Utility.DateTimeHelper.GetStartTime(CodeMaster.TimeUnit.Month, purchasePlan.StartTime);
                            }

                            purchasePlan.Qty = (mrpFlowDetail.MrpWeight / mrpFlowDetails.Sum(p => p.MrpWeight)) * groupRccpTrans.Qty;
                            purchasePlan.PlanQty = purchasePlan.Qty;
                            purchasePlan.DateType = dateType;
                            purchasePlan.PlanVersion = planVersion;
                            purchasePlanList.Add(purchasePlan);
                        }
                    }
                    else
                    {
                        if(groupRccpTrans.IsLastLevel)
                        {
                            businessException.AddMessage(new Message(CodeMaster.MessageType.Warning, "没有找到物料{0}的采购路线", groupRccpTrans.Item));
                        }
                    }
                }
            }

            string hql = string.Empty;
            if(dateType == CodeMaster.TimeUnit.Week)
            {
                hql = "from FlowStrategy where IsCheckMrpWeeklyPlan =? and Flow in(?";
            }
            else if(dateType == CodeMaster.TimeUnit.Month)
            {
                hql = "from FlowStrategy where IsCheckMrpMonthlyPlan =? and Flow in(?";
            }

            var flowStategys = this.genericMgr.FindAllIn<FlowStrategy>
                (hql, purchasePlanList.Select(p => p.Flow).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct(),
                new object[] { true });
            var flowMasterDic = this.genericMgr.FindAllIn<FlowMaster>
             ("from FlowMaster where Code in(?", flowStategys.Select(p => p.Flow).Distinct())
             .GroupBy(p => p.Code, (k, g) => new { k, g.First().Description })
             .ToDictionary(d => d.k, d => d.Description);
            foreach(var flowStategy in flowStategys)
            {
                PurchasePlanMaster purchasePlanMaster = new PurchasePlanMaster();
                purchasePlanMaster.DateType = dateType;
                purchasePlanMaster.Flow = flowStategy.Flow;
                purchasePlanMaster.FlowDescription = flowMasterDic[flowStategy.Flow];
                purchasePlanMaster.PlanVersion = planVersion;
                purchasePlanMaster.SnapTime = snapTime;
                purchasePlanMaster.SourcePlanVersion = snapTime;

                purchasePlanMaster.CreateUserId = user.Id;
                purchasePlanMaster.CreateUserName = user.FullName;
                purchasePlanMaster.CreateDate = DateTime.Now;
                purchasePlanMaster.LastModifyUserId = user.Id;
                purchasePlanMaster.LastModifyUserName = user.FullName;
                purchasePlanMaster.LastModifyDate = DateTime.Now;

                this.genericMgr.Create(purchasePlanMaster);
            }

            purchasePlanList = purchasePlanList.Where(p => flowStategys.Select(q => q.Flow).Contains(p.Flow)).ToList();
            this.genericMgr.BulkInsert<PurchasePlan>(purchasePlanList);
        }

        private void GenFiRccp(DateTime planVersion, CodeMaster.TimeUnit dateType, IList<MrpFlowDetail> mrpFlowDetailList,
            IList<RccpTrans> rccpTransList, BusinessException businessException)
        {
            var fiFlowDetailDic = mrpFlowDetailList.Where(p => p.ResourceGroup == CodeMaster.ResourceGroup.FI)
                .GroupBy(p => p.Item, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g);

            var fiPlanList = new List<RccpFiPlan>();

            var rccpTransGroups = rccpTransList.GroupBy(p => new
            {
                p.DateIndex,
                p.DateType,
                p.Item,
                p.PlanVersion,
                p.Model
            }, (k, g) => new { k, g });

            foreach(var rccpTransGroup in rccpTransGroups)
            {
                DateTime dateFrom = DateTime.Now;
                if(dateType == CodeMaster.TimeUnit.Week)
                {
                    dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(rccpTransGroup.k.DateIndex);
                }
                else
                {
                    dateFrom = DateTime.Parse(rccpTransGroup.k.DateIndex + "-01");
                }

                var mrpFlowDetail = (fiFlowDetailDic.ValueOrDefault(rccpTransGroup.k.Item) ?? new List<MrpFlowDetail>())
                                    .FirstOrDefault(p => p.StartDate <= dateFrom && p.EndDate > dateFrom);
                if(mrpFlowDetail != null)
                {
                    var fiPlan = new RccpFiPlan();
                    fiPlan.ProductLine = mrpFlowDetail.Flow;
                    fiPlan.Machine = mrpFlowDetail.Machine == null ? string.Empty : mrpFlowDetail.Machine;
                    fiPlan.Item = rccpTransGroup.k.Item;
                    fiPlan.DateIndex = rccpTransGroup.k.DateIndex;
                    fiPlan.DateType = dateType;
                    fiPlan.PlanVersion = planVersion;
                    fiPlan.Model = rccpTransGroup.k.Model ?? string.Empty;
                    fiPlan.ModelRate = rccpTransGroup.g.First().ModelRate;
                    fiPlan.Qty = rccpTransGroup.g.Sum(p => p.Qty);
                    fiPlanList.Add(fiPlan);
                }
            }
            this.genericMgr.BulkInsert<RccpFiPlan>(fiPlanList);
        }

        private List<RccpTrans> GetRccpTrans(DateTime planVersion, IList<RccpPlan> rccpPlans, BusinessException businessException)
        {
            var groupRccpPlans = from p in rccpPlans
                                 //where p.Qty > 0
                                 group p by new
                                 {
                                     p.DateIndex,
                                     p.DateType
                                 } into g
                                 select new
                                 {
                                     DateIndex = g.Key.DateIndex,
                                     DateType = g.Key.DateType,
                                     List = g.ToList()
                                 };

            var rccpTransBag = new ConcurrentBag<RccpTrans>();
            DateTime effdate = DateTime.Now;
            var errorItems = new List<string>();
            var itemDic = this.itemMgr.GetCacheAllItem();
            var bomDetDic = bomMgr.GetCacheAllBomDetail();
            foreach(var bomDet in bomDetDic)
            {
                if(!itemDic.ContainsKey(bomDet.Key))
                {
                    errorItems.Add(bomDet.Key);
                }
            }
            if(errorItems.Count > 0)
            {
                string errorMessage = string.Format("{0}", string.Join(",", errorItems));
                businessException.AddMessage(new Message(CodeMaster.MessageType.Warning, "没有发现bom{0}的物料基础数据", errorMessage));
            }
            foreach(var groupRccpPlan in groupRccpPlans)
            {
                log.Info(string.Format("正在分解{0}的计划", groupRccpPlan.DateIndex));
                if(groupRccpPlan.DateType == CodeMaster.TimeUnit.Month)
                {
                    effdate = DateTime.Parse(string.Format("{0}-01", groupRccpPlan.DateIndex));
                }
                else
                {
                    effdate = Utility.DateTimeHelper.GetWeekIndexDateFrom(groupRccpPlan.DateIndex);
                }

                Parallel.ForEach(groupRccpPlan.List, rccpPlan =>
                //foreach (var rccpPlan in rccpPlans)
                {
                    RccpTrans topRccpTrans = new RccpTrans();
                    topRccpTrans.PlanVersion = planVersion;
                    topRccpTrans.Item = rccpPlan.Item;
                    topRccpTrans.Qty = rccpPlan.Qty;
                    topRccpTrans.ScrapPercentage = 0;
                    topRccpTrans.SourcePlanVersion = rccpPlan.PlanVersion;
                    topRccpTrans.DateIndex = rccpPlan.DateIndex;
                    topRccpTrans.DateType = rccpPlan.DateType;
                    rccpTransBag.Add(topRccpTrans);

                    var item = this.itemMgr.GetCacheItem(rccpPlan.Item);
                    BomDetail modelBomDetail = null;
                    if(item.ItemCategory == "FERT")
                    {
                        modelBomDetail = bomDetDic
                            .Where(p => (this.itemMgr.GetCacheItem(p.Key) ?? new Item()).ItemCategory == "MODEL")
                            .SelectMany(p => p.Value)
                            .Where(p => p.Item == item.Code)
                            .FirstOrDefault();
                    }

                    var mrpBoms = GetMrpBomList(rccpPlan.Item, rccpPlan.Item, effdate, businessException, true);
                    if(mrpBoms.Count() > 0)
                    {
                        foreach(var mrpBom in mrpBoms)
                        {
                            RccpTrans rccpTrans = new RccpTrans();
                            rccpTrans.Bom = mrpBom.Bom;
                            rccpTrans.PlanVersion = planVersion;
                            rccpTrans.Item = mrpBom.Item;
                            rccpTrans.Qty = mrpBom.RateQty * rccpPlan.Qty;
                            rccpTrans.ScrapPercentage = mrpBom.ScrapPercentage;
                            rccpTrans.SourcePlanVersion = rccpPlan.PlanVersion;
                            rccpTrans.DateIndex = rccpPlan.DateIndex;
                            rccpTrans.DateType = rccpPlan.DateType;
                            if(item.ItemCategory == "MODEL")
                            {
                                rccpTrans.Model = item.Code;
                                rccpTrans.ModelRate = mrpBom.RateQty;
                            }
                            else if(item.ItemCategory == "FERT")
                            {
                                if(modelBomDetail != null)
                                {
                                    rccpTrans.Model = modelBomDetail.Bom;
                                    rccpTrans.ModelRate = (double)modelBomDetail.UnitBomQty;
                                }
                            }
                            rccpTransBag.Add(rccpTrans);

                            this.GetNextRccpTrans(effdate, rccpTrans, rccpTransBag, businessException);
                        }
                    }
                    else
                    {
                        topRccpTrans.IsLastLevel = true;
                    }
                }
                );
            }
            return rccpTransBag.ToList();
        }

        private void GetNextRccpTrans(DateTime effdate, RccpTrans parentRccpPlan, ConcurrentBag<RccpTrans> rccpTransQueue, BusinessException businessException)
        {
            var pboms = GetMrpBomList(parentRccpPlan.Item, parentRccpPlan.Item, effdate, businessException, true);
            if(pboms.Count > 0)
            {
                foreach(var pbom in pboms)
                {
                    RccpTrans rccpTrans = new RccpTrans();
                    rccpTrans.Bom = pbom.Bom;
                    rccpTrans.PlanVersion = parentRccpPlan.PlanVersion;
                    rccpTrans.Item = pbom.Item;
                    rccpTrans.Qty = pbom.RateQty * parentRccpPlan.Qty;
                    rccpTrans.ScrapPercentage = pbom.ScrapPercentage;
                    rccpTrans.SourcePlanVersion = parentRccpPlan.SourcePlanVersion;
                    rccpTrans.DateIndex = parentRccpPlan.DateIndex;
                    rccpTrans.DateType = parentRccpPlan.DateType;
                    rccpTrans.Model = parentRccpPlan.Model;
                    rccpTrans.ModelRate = parentRccpPlan.ModelRate;
                    rccpTransQueue.Add(rccpTrans);
                    this.GetNextRccpTrans(effdate, rccpTrans, rccpTransQueue, businessException);
                }
            }
            else
            {
                parentRccpPlan.IsLastLevel = true;
            }
        }
        #endregion
    }
}
