using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.VIEW;
using com.Sconit.Utility;
using NHibernate;

namespace com.Sconit.Service.MRP.Impl
{
    [Transactional]
    public class MrpMgrImpl : BaseMgr, IMrpMgr
    {
        public IWorkingCalendarMgr workingCalendarMgr { get; set; }
        public IFlowMgr flowMgr { get; set; }
        public IOrderMgr orderMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public ICustomizationMgr customizationMgr { get; set; }

        private static log4net.ILog logRunMrp = log4net.LogManager.GetLogger("Log.MRP.RunMrp");
        private static log4net.ILog logGenMrpSnapShot = log4net.LogManager.GetLogger("Log.MRP.GenMrpSnapShot");

        #region GenMrpSnapShot
        private static object GetMrpSnapShotLock = new object();
        [Transaction(TransactionMode.Requires)]
        public void GenMrpSnapShot(DateTime snapTime, User user, bool isRelease, CodeMaster.SnapType snapType)
        {
            lock (GetMrpSnapShotLock)
            {
                snapTime = DateTime.Parse(snapTime.ToString("yyyy-MM-dd HH:mm:ss"));
                SecurityContextHolder.Set(user);

                BusinessException businessException = new BusinessException();
                try
                {
                    logGenMrpSnapShot.Info("---**------Start GetMrpSnapShot effectivedate:" + snapTime.ToLongDateString() + "------");

                    #region 获取路线
                    logGenMrpSnapShot.Info(string.Format("-{0} - 开始获取路线明细-", snapTime));
                    List<MrpFlowDetail> mrpFlowDetails = GetMrpFlowDetails(snapTime);
                    logGenMrpSnapShot.Info(string.Format("-{0} - 结束获取路线明细-", snapTime));
                    #endregion

                    if (snapType == CodeMaster.SnapType.Mrp)
                    {
                        #region 获取即时库存
                        logGenMrpSnapShot.Info(string.Format("-{0} - 开始获取库存明细-", snapTime));
                        var inventoryBalances = GetInventoryBalance(snapTime, mrpFlowDetails);
                        this.genericMgr.BulkInsert<InventoryBalance>(inventoryBalances);
                        logGenMrpSnapShot.Info(string.Format("-{0} - 结束获取库存明细-", snapTime));
                        #endregion

                        #region 在途(销售在途除外)
                        logGenMrpSnapShot.Info(string.Format("-{0} - 开始获取在途库存明细-", snapTime));
                        var transitOrderList = this.genericMgr.FindAllWithNamedQuery<TransitOrder>
                            (@"USP_Busi_MRP_GetTransitOrder", new object[] { snapTime });

                        this.genericMgr.BulkInsert<TransitOrder>(transitOrderList);
                        logGenMrpSnapShot.Info(string.Format("-{0} - 结束获取在途库存明细-", snapTime));
                        #endregion

                        #region 获取活动订单
                        logGenMrpSnapShot.Info(string.Format("-{0} - 开始获取独立需求明细-", snapTime));
                        var activeOrderList = this.genericMgr.FindAllWithNamedQuery<ActiveOrder>
                            (@"USP_Busi_MRP_GetActiveOrder", new object[] { snapTime });
                        this.genericMgr.BulkInsert<ActiveOrder>(activeOrderList);
                        logGenMrpSnapShot.Info(string.Format("-{0} - 结束获取独立需求明细-", snapTime));
                        #endregion
                    }


                    #region 后加工日历
                    logGenMrpSnapShot.Info(string.Format("-{0} - 开始获取后加工设备日历-", snapTime));
                    GenMachineInstance(snapTime, mrpFlowDetails);
                    logGenMrpSnapShot.Info(string.Format("-{0} - 结束获取后加工设备日历-", snapTime));
                    #endregion

                    #region 挤出资源日历
                    logGenMrpSnapShot.Info(string.Format("-{0} - 开始获取挤出生产线资源日历-", snapTime));
                    GenProdLineExInstance(snapTime);
                    logGenMrpSnapShot.Info(string.Format("-{0} - 结束获取挤出生产线资源日历-", snapTime));
                    #endregion

                    #region 挤出/炼胶/后加工日历
                    GenWorkCalendar(snapTime, mrpFlowDetails);
                    #endregion

                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@SnapTime", snapTime);
                    sqlParameters[1] = new SqlParameter("@SnapType", snapType);
                    this.genericMgr.ExecuteStoredProcedure("USP_Busi_MRP_SnapData", sqlParameters);
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(new Message(CodeMaster.MessageType.Error, ex.StackTrace));
                    //logGenMrpSnapShot.Error(ex);
                }

                #region 记录MrpSnap日志
                List<MrpSnapLog> logs = new List<MrpSnapLog>();
                CodeMaster.MessageType status = CodeMaster.MessageType.Info;
                if (businessException.HasMessage)
                {
                    var messages = businessException.GetMessages().GroupBy(p =>
                        new { Message = p.GetMessageString(), MessageType = p.MessageType },
                        (k, g) => new { k.Message, k.MessageType });
                    foreach (var message in messages)
                    {
                        MrpSnapLog log = new MrpSnapLog();
                        log.ErrorLevel = message.MessageType.ToString();
                        log.Message = message.Message;
                        log.Logger = "GenMrpSnapShot";
                        log.SnapTime = snapTime;
                        logs.Add(log);
                        if (message.MessageType == CodeMaster.MessageType.Warning)
                        {
                            logGenMrpSnapShot.Warn(log.Message);
                        }
                        else if (message.MessageType == CodeMaster.MessageType.Error)
                        {
                            logGenMrpSnapShot.Error(log.Message);
                        }
                        else
                        {
                            logGenMrpSnapShot.Info(log.Message);
                        }
                    }
                    if (messages.Count(f => f.MessageType == CodeMaster.MessageType.Error) > 0)
                    {
                        status = CodeMaster.MessageType.Error;
                    }
                    else if (messages.Count(f => f.MessageType == CodeMaster.MessageType.Warning) > 0)
                    {
                        status = CodeMaster.MessageType.Warning;
                    }
                }

                MrpSnapMaster mrpSnapMaster = new MrpSnapMaster();
                mrpSnapMaster.SnapTime = snapTime;
                mrpSnapMaster.Status = status;
                mrpSnapMaster.CreateDate = DateTime.Now;
                mrpSnapMaster.IsRelease = isRelease;
                mrpSnapMaster.Type = snapType;

                mrpSnapMaster.CreateUserId = user.Id;
                mrpSnapMaster.CreateUserName = user.FullName;
                mrpSnapMaster.CreateDate = DateTime.Now;
                mrpSnapMaster.LastModifyUserId = user.Id;
                mrpSnapMaster.LastModifyUserName = user.FullName;
                mrpSnapMaster.LastModifyDate = DateTime.Now;
                this.genericMgr.Create(mrpSnapMaster);
                #endregion

                double timetick = 1001 - (mrpSnapMaster.CreateDate - snapTime).TotalMilliseconds;
                timetick = timetick > 0 ? timetick : 0;
                Thread.Sleep((int)timetick);
                string infoMessage = string.Format("完成时间:{0},时间总计:{1}秒", DateTime.Now.ToLocalTime(), (DateTime.Now - snapTime).TotalSeconds);
                logGenMrpSnapShot.Info(infoMessage);

                MrpSnapLog infoLog = new MrpSnapLog();
                infoLog.ErrorLevel = CodeMaster.MessageType.Info.ToString();
                infoLog.Message = infoMessage;
                infoLog.Logger = "GenMrpSnapShot";
                infoLog.SnapTime = snapTime;
                logs.Add(infoLog);
                this.genericMgr.BulkInsert<MrpSnapLog>(logs);
            }
        }

        private void GenWorkCalendar(DateTime snapTime, IList<MrpFlowDetail> mrpFlowDetails)
        {
            string startWeekIndex = Utility.DateTimeHelper.GetWeekOfYear(snapTime);

            var oldWorkCalendarList = this.genericMgr.FindAll<WorkCalendar>
              ("from WorkCalendar where (DateType=? and DateIndex>=? ) or (DateType=? and DateIndex>=? ) or (DateType=? and DateIndex>=? ) ",
              new object[] {
                    CodeMaster.TimeUnit.Day, snapTime.ToString("yyyy-MM-dd"),
                    CodeMaster.TimeUnit.Week, startWeekIndex,
                    CodeMaster.TimeUnit.Month, snapTime.ToString("yyyy-MM")
                });

            #region MI,EX,FI
            var flowDetails = mrpFlowDetails.Where(p => p.ResourceGroup == CodeMaster.ResourceGroup.FI ||
                p.ResourceGroup == CodeMaster.ResourceGroup.EX ||
                p.ResourceGroup == CodeMaster.ResourceGroup.MI).ToList();

            List<WorkCalendar> newWorkCalendars = new List<WorkCalendar>();

            var dailyWorkCalendars = GetWorkCalendar(snapTime, CodeMaster.TimeUnit.Day, flowDetails);
            newWorkCalendars.AddRange(dailyWorkCalendars);
            var weeklyWorkCalendars = GetWorkCalendar(snapTime, CodeMaster.TimeUnit.Week, flowDetails);
            newWorkCalendars.AddRange(weeklyWorkCalendars);
            var monthlyWorkCalendars = GetWorkCalendar(snapTime, CodeMaster.TimeUnit.Month, flowDetails);
            newWorkCalendars.AddRange(monthlyWorkCalendars);
            #endregion

            CreateOrUpdateWorkCalendars(newWorkCalendars, oldWorkCalendarList);
        }

        private void CreateOrUpdateWorkCalendars(IList<WorkCalendar> workCalendars, IList<WorkCalendar> oldWorkCalendars)
        {
            var createWorkCalendar = new List<WorkCalendar>();
            foreach (var workCalendar in workCalendars)
            {
                var oldWorkCalendar = oldWorkCalendars.FirstOrDefault(p => p.DateIndex == workCalendar.DateIndex
                    && p.Flow == workCalendar.Flow && p.DateType == workCalendar.DateType);
                if (oldWorkCalendar == null)
                {
                    createWorkCalendar.Add(workCalendar);
                }
                else if (oldWorkCalendar.HaltTime != workCalendar.HaltTime
                        || oldWorkCalendar.Holiday != workCalendar.Holiday
                        || oldWorkCalendar.TrialTime != workCalendar.TrialTime
                        || oldWorkCalendar.UpTime != workCalendar.UpTime
                        || oldWorkCalendar.ResourceGroup != workCalendar.ResourceGroup)
                {
                    if (!oldWorkCalendar.IsLock)
                    {
                        oldWorkCalendar.HaltTime = workCalendar.HaltTime;
                        oldWorkCalendar.Holiday = workCalendar.Holiday;
                        oldWorkCalendar.TrialTime = workCalendar.TrialTime;
                        oldWorkCalendar.UpTime = workCalendar.UpTime;
                        oldWorkCalendar.ResourceGroup = workCalendar.ResourceGroup;
                        this.genericMgr.Update(oldWorkCalendar);
                    }
                }
            }
            this.genericMgr.BulkInsert<WorkCalendar>(createWorkCalendar);
        }

        private List<WorkCalendar> GetWorkCalendar(DateTime snapTime, CodeMaster.TimeUnit dateType, IList<MrpFlowDetail> mrpFlowDetails)
        {
            var workCalendars = new List<WorkCalendar>();

            IList<SpecialTime> specialTimeList = new List<SpecialTime>();
            if (dateType == CodeMaster.TimeUnit.Month)
            {
                specialTimeList = this.genericMgr.FindAll<SpecialTime>(
                   "select s from SpecialTime as s where StartTime <= ? and EndTime >= ?",
                   new object[] { snapTime.AddMonths(31).Date, snapTime.Date });
            }
            else if (dateType == CodeMaster.TimeUnit.Week)
            {
                specialTimeList = this.genericMgr.FindAll<SpecialTime>(
                   "select s from SpecialTime as s where StartTime <= ? and EndTime >= ?",
                   new object[] { snapTime.AddDays(30 * 8).Date, snapTime.Date });
            }
            else if (dateType == CodeMaster.TimeUnit.Day)
            {
                specialTimeList = this.genericMgr.FindAll<SpecialTime>(
                   "select s from SpecialTime as s where StartTime <= ? and EndTime >= ?",
                   new object[] { snapTime.AddDays(31).Date, snapTime.Date });
            }

            var flows = from p in mrpFlowDetails
                        group p by
                        new
                        {
                            p.ResourceGroup,
                            p.Flow,
                            p.PartyFrom,
                        } into g
                        select new
                        {
                            ResourceGroup = g.Key.ResourceGroup,
                            Flow = g.Key.Flow,
                            Region = g.Key.PartyFrom,
                            TrialTime = g.First().TrialTime,
                            HaltTime = g.First().HaltTime
                        };

            for (int i = 1; i < 30; i++)
            {
                string dateIndex = null;
                DateTime startTime = snapTime.Date;
                DateTime endTime = snapTime.Date;
                if (dateType == CodeMaster.TimeUnit.Month)
                {
                    dateIndex = snapTime.AddMonths(i).ToString("yyyy-MM");
                    startTime = DateTime.Parse(dateIndex + "-01");
                    endTime = startTime.AddMonths(1);
                }
                else if (dateType == CodeMaster.TimeUnit.Week)
                {
                    startTime = Utility.DateTimeHelper.GetStartTime(CodeMaster.TimeUnit.Week, snapTime.AddDays(7 * i));
                    dateIndex = Utility.DateTimeHelper.GetWeekOfYear(startTime);
                    endTime = startTime.AddMonths(7 * (i + 1));
                }
                else if (dateType == CodeMaster.TimeUnit.Day)
                {
                    startTime = snapTime.AddDays(i);
                    dateIndex = snapTime.AddDays(i).ToString("yyyy-MM-dd");
                    endTime = snapTime.AddDays(i + 1);
                }
                else
                {
                    throw new BusinessException("不支持此类型的计划");
                }

                var specialTimes_1 = specialTimeList.Where(p => p.StartTime >= startTime && p.EndTime >= endTime);

                foreach (var flow in flows)
                {
                    //后加工排产不看工作日历
                    if (flow.ResourceGroup == CodeMaster.ResourceGroup.FI && dateType == CodeMaster.TimeUnit.Day)
                    {
                        continue;
                    }
                    var workCalendar = new WorkCalendar();
                    workCalendar.DateIndex = dateIndex;
                    workCalendar.DateType = dateType;
                    workCalendar.Flow = flow.Flow;
                    workCalendar.ResourceGroup = flow.ResourceGroup;

                    var specialTimes_common = specialTimes_1.Where(p => string.IsNullOrWhiteSpace(p.Region)
                        && string.IsNullOrWhiteSpace(p.Flow)).ToList();
                    var specialTimes_region = specialTimes_1.Where(p => string.Equals(p.Region, flow.Region, StringComparison.OrdinalIgnoreCase)
                        && string.IsNullOrWhiteSpace(p.Flow)).ToList();
                    var specialTimes_flow = specialTimes_1.Where(p =>
                        string.Equals(p.Flow, flow.Flow, StringComparison.OrdinalIgnoreCase)).ToList();

                    var specialTimes = workingCalendarMgr.GetSpecialTime(specialTimes_common, specialTimes_region, specialTimes_flow);

                    foreach (var specialTime in specialTimes)
                    {
                        specialTime.StartTime = specialTime.StartTime <= startTime ? startTime : specialTime.StartTime;
                        specialTime.EndTime = specialTime.EndTime >= endTime ? endTime : specialTime.EndTime;
                        if (dateType == CodeMaster.TimeUnit.Day)
                        {
                            if (specialTime.HolidayType == CodeMaster.HolidayType.Halt)
                            {
                                workCalendar.HaltTime += (specialTime.EndTime - specialTime.StartTime).TotalHours;
                            }
                            else if (specialTime.HolidayType == CodeMaster.HolidayType.Trial)
                            {
                                workCalendar.TrialTime += (specialTime.EndTime - specialTime.StartTime).TotalHours;
                            }
                            else if (specialTime.HolidayType == CodeMaster.HolidayType.Holiday)
                            {
                                workCalendar.Holiday += (specialTime.EndTime - specialTime.StartTime).TotalHours;
                            }
                        }
                        else
                        {
                            if (specialTime.HolidayType == CodeMaster.HolidayType.Holiday)
                            {
                                workCalendar.Holiday += (specialTime.EndTime - specialTime.StartTime).TotalDays;
                            }
                        }
                    }

                    if (dateType == CodeMaster.TimeUnit.Month)
                    {
                        workCalendar.HaltTime = flow.HaltTime;
                        workCalendar.TrialTime = flow.TrialTime;
                        workCalendar.UpTime = DateTime.DaysInMonth(startTime.Year, startTime.Month)
                            - workCalendar.Holiday - workCalendar.HaltTime - workCalendar.TrialTime;
                    }
                    else if (dateType == CodeMaster.TimeUnit.Week)
                    {
                        //折算,挤出不考虑周停机,试制时间和节假日
                        workCalendar.HaltTime = flow.HaltTime * 7 / DateTime.DaysInMonth(startTime.Year, startTime.Month);
                        workCalendar.TrialTime = flow.TrialTime * 7 / DateTime.DaysInMonth(startTime.Year, startTime.Month);
                        workCalendar.UpTime = 7 - workCalendar.Holiday - workCalendar.HaltTime - workCalendar.TrialTime;
                    }
                    else if (dateType == CodeMaster.TimeUnit.Day)
                    {
                        workCalendar.UpTime = 24 - workCalendar.Holiday - workCalendar.HaltTime - workCalendar.TrialTime;
                    }

                    workCalendars.Add(workCalendar);
                }
            }
            return workCalendars;
        }

        private IList<InventoryBalance> GetInventoryBalance(DateTime snapTime, IList<MrpFlowDetail> mrpFlowDetails)
        {
            IList<LocationDetailView> locationDetailViewList = this.genericMgr.FindEntityWithNativeSql<LocationDetailView>(
                @"select l.* from VIEW_LocationDet as l inner join MD_Location as loc on l.Location = loc.Code where loc.IsMrp = ? ",
                new object[] { true });

            var inventoryBalances = locationDetailViewList
                .GroupBy(p => new { p.Item, p.Location })
                .Select(p => new InventoryBalance
                {
                    SnapTime = snapTime,
                    Item = p.Key.Item,
                    Location = p.Key.Location,
                    Qty = (double)p.Sum(q => q.ATPQty)
                }).ToList();
            var stockDic = mrpFlowDetails.Where(f => f.MrpWeight > 0 && f.EndDate >= DateTime.Now.Date)
                .GroupBy(p => new { Item = p.Item, Location = p.LocationTo })
                .ToDictionary(d => d.Key, d => new { SafeStock = d.Sum(q => q.SafeStock), MaxStock = d.Sum(q => q.MaxStock) });
            foreach (var inventoryBalance in inventoryBalances)
            {
                var stock = stockDic.ValueOrDefault(new { Item = inventoryBalance.Item, Location = inventoryBalance.Location });
                if (stock != null)
                {
                    inventoryBalance.SafeStock = stock.SafeStock;
                    inventoryBalance.MaxStock = stock.MaxStock;
                }
            }

            return inventoryBalances;
        }

        private List<MrpFlowDetail> GetMrpFlowDetails(DateTime snapTime)
        {
            //检查
            string sql = @"Select a.ProductLine,b.Bom,a.productline as flow,a.Item As section into #BomOfFlow from MRP_ProdLineEx a ,PRD_BomDet b where a.Item=b.Item
                        and a.StartDate<=GETDATE() and a.EndDate>GETDATE() and b.StartDate<=GETDATE() and (b.EndDate is null or b.EndDate>GETDATE())
                        Select COUNT(*) from #BomOfFlow a ,MD_Item b where  a.bom=b.code and not exists
                        (select 1 from SCM_FlowDet b join SCM_FlowMstr m on m.Code = b.Flow where a.flow=b.flow  and a.bom =b.item and m.ResourceGroup =20)
                        drop table #BomOfFlow";
            var count = this.genericMgr.FindAllWithNativeSql<int>(sql)[0];
            if (count > 0)
            {
                //throw new BusinessException("挤出资源的断面已维护了断面,挤出生产线上未维护对应的半制品");
            }

            var flowMasters = this.genericMgr.FindAll<FlowMaster>
                    (@"from FlowMaster as m where m.IsMRP = ? and m.IsActive = ? ", new object[] { true, true });

            var flowStrategyDic = this.genericMgr.FindAllIn<com.Sconit.Entity.SCM.FlowStrategy>
                ("from FlowStrategy where Flow in (? ", flowMasters.Select(f => f.Code))
                    .ToDictionary(d => d.Flow, d => d);

            List<MrpFlowDetail> mrpFlowDetails = new List<MrpFlowDetail>();
            //Parallel.ForEach(flowMasters, flow =>
            foreach (var flow in flowMasters)
            {
                logGenMrpSnapShot.Info(string.Format("{1} - 正在获取路线明细:{0}", flow.CodeDescription, snapTime));
                var flowDetailList = this.flowMgr.GetFlowDetailList(flow, true, false);

                if (flowDetailList != null)
                {
                    #region 过滤
                    flowDetailList = (from det in flowDetailList
                                      group det by
                                      new
                                      {
                                          LocationFrom = !string.IsNullOrWhiteSpace(det.LocationFrom) ? det.LocationFrom : det.CurrentFlowMaster.LocationFrom,
                                          LocationTo = !string.IsNullOrWhiteSpace(det.LocationTo) ? det.LocationTo : det.CurrentFlowMaster.LocationTo,
                                          StartDate = det.StartDate,
                                          EndDate = det.EndDate,
                                          Item = det.Item,
                                      } into result
                                      select result.Max<FlowDetail>()).ToList();
                    #endregion

                    #region MrpFlowDetail
                    foreach (var flowDetail in flowDetailList)
                    {
                        string loccationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                        string locationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                        DateTime startDate = flowDetail.StartDate.HasValue ? flowDetail.StartDate.Value : DateTime.MinValue;
                        DateTime endDate = flowDetail.EndDate.HasValue ? flowDetail.EndDate.Value : DateTime.MaxValue;

                        #region 过滤
                        //过滤相同路线明细（来源库位、目的库位、零件,开始时间,结束时间），不考虑单位、包装等因素
                        //可能存在两条相同的路线明细，包装/单位不同，需要过滤只剩一条
                        //可能出现把有MRPWeight的过滤掉。
                        //可能出现把其它需求源过滤掉。
                        //if (mrpFlowDetails.Count(p => p.LocationFrom == loccationFrom && p.LocationTo == locationTo
                        //    && p.Item == flowDetail.Item && p.StartDate == startDate && p.EndDate == endDate) > 0)
                        //{
                        //    continue;
                        //}
                        #endregion 过滤

                        var mrpFlowDetail = new MrpFlowDetail();
                        var flowStrategy = flowStrategyDic.ValueOrDefault(flowDetail.Flow);

                        mrpFlowDetail.DetailId = flowDetail.Id;
                        mrpFlowDetail.Flow = flowDetail.Flow;
                        mrpFlowDetail.Type = flowDetail.CurrentFlowMaster.Type;
                        mrpFlowDetail.Item = flowDetail.Item;
                        mrpFlowDetail.LocationFrom = loccationFrom;
                        mrpFlowDetail.LocationTo = locationTo;

                        mrpFlowDetail.PartyFrom = flowDetail.CurrentFlowMaster.PartyFrom;
                        mrpFlowDetail.PartyTo = flowDetail.CurrentFlowMaster.PartyTo;

                        decimal LeadTime = flowStrategy.RccpLeadTime > flowStrategy.LeadTime ? flowStrategy.RccpLeadTime : flowStrategy.LeadTime;
                        mrpFlowDetail.LeadTime = Utility.DateTimeHelper.TimeTranfer(LeadTime, flowStrategy.TimeUnit, CodeMaster.TimeUnit.Hour);

                        mrpFlowDetail.MrpPriority = flowDetail.MrpPriority;
                        mrpFlowDetail.MrpWeight = (double)flowDetail.MrpWeight;
                        if (mrpFlowDetail.Type == CodeMaster.OrderType.Production || mrpFlowDetail.Type == CodeMaster.OrderType.SubContract)
                        {
                            mrpFlowDetail.Bom = string.IsNullOrWhiteSpace(flowDetail.Bom) ? flowDetail.Item : flowDetail.Bom;
                        }
                        else
                        {
                            mrpFlowDetail.Bom = flowDetail.Bom;
                        }
                        mrpFlowDetail.MaxStock = (double)flowDetail.MaxStock;
                        mrpFlowDetail.SafeStock = (double)flowDetail.SafeStock;

                        mrpFlowDetail.Machine = flowDetail.Machine;
                        mrpFlowDetail.StartDate = startDate;
                        mrpFlowDetail.EndDate = endDate;
                        mrpFlowDetail.ResourceGroup = flowDetail.CurrentFlowMaster.ResourceGroup;

                        mrpFlowDetail.SnapTime = snapTime;
                        mrpFlowDetail.UnitCount = (double)flowDetail.UnitCount;
                        mrpFlowDetail.UnitCount = mrpFlowDetail.UnitCount == 0 ? 1 : mrpFlowDetail.UnitCount;

                        mrpFlowDetail.HaltTime = flowStrategy.HaltTime;
                        mrpFlowDetail.TrialTime = flowStrategy.TrialProduceTime;
                        mrpFlowDetail.Sequence = flowDetail.Sequence;
                        mrpFlowDetail.Uom = flowDetail.Uom;
                        mrpFlowDetail.BatchSize = (double)flowDetail.BatchSize;
                        mrpFlowDetail.BatchSize = mrpFlowDetail.BatchSize == 0 ? 1 : mrpFlowDetail.BatchSize;
                        mrpFlowDetail.ExtraLocationTo = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationTo)
                            ? flowDetail.CurrentFlowMaster.ExtraLocationTo : flowDetail.ExtraLocationTo;
                        mrpFlowDetail.ExtraLocationTo = mrpFlowDetail.ExtraLocationTo == null ?
                            string.Empty : mrpFlowDetail.ExtraLocationTo;

                        mrpFlowDetail.ExtraLocationFrom = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationFrom)
                            ? flowDetail.CurrentFlowMaster.ExtraLocationFrom : flowDetail.ExtraLocationFrom;
                        mrpFlowDetail.ExtraLocationFrom = mrpFlowDetail.ExtraLocationFrom == null ?
                            string.Empty : mrpFlowDetail.ExtraLocationFrom;

                        //this.genericMgr.Create(mrpFlowDetail);
                        mrpFlowDetails.Add(mrpFlowDetail);
                    }
                    #endregion
                }
            }
            //);

            if (mrpFlowDetails.Count() == 0)
            {
                logGenMrpSnapShot.Warn("No flowDetail to run MRP");
            }
            else
            {
                this.genericMgr.BulkInsert<MrpFlowDetail>(mrpFlowDetails);
            }
            return mrpFlowDetails;
        }

        #region GenProdLineExInstance
        [Transaction(TransactionMode.Requires)]
        public void GenProdLineExInstance(DateTime snapTime)
        {
            string startWeekIndex = Utility.DateTimeHelper.GetWeekOfYear(snapTime);

            var oldProdLineExInstanceDic = this.genericMgr.FindAll<ProdLineExInstance>
                ("from ProdLineExInstance where (DateType=? and DateIndex>=? ) or (DateType=? and DateIndex>=? ) ",
                new object[] {
                    //CodeMaster.TimeUnit.Day, DateTime.Now.ToString("yyyy-MM-dd"),
                    CodeMaster.TimeUnit.Week, startWeekIndex,
                    CodeMaster.TimeUnit.Month, DateTime.Now.ToString("yyyy-MM")
                }).GroupBy(p => p.DateType, (k, g) => new { k, g })
                .ToDictionary(d => d.k, d => d.g.ToList());

            var prodLineExList = this.genericMgr.FindAll<ProdLineEx>();

            var newProdLineExInstances = new List<ProdLineExInstance>();
            var monthlyInstances = GenProdLineExInstance(snapTime, oldProdLineExInstanceDic.ValueOrDefault(CodeMaster.TimeUnit.Month), prodLineExList, CodeMaster.TimeUnit.Month);
            newProdLineExInstances.AddRange(monthlyInstances);
            var weeklyInstances = GenProdLineExInstance(snapTime, oldProdLineExInstanceDic.ValueOrDefault(CodeMaster.TimeUnit.Week), prodLineExList, CodeMaster.TimeUnit.Week);
            newProdLineExInstances.AddRange(weeklyInstances);
            //var dailyInstances = GenProdLineExInstance(snapTime, oldProdLineExInstanceDic.ValueOrDefault(CodeMaster.TimeUnit.Day), prodLineExList, CodeMaster.TimeUnit.Day);
            //newProdLineExInstances.AddRange(dailyInstances);
            this.genericMgr.BulkInsert<ProdLineExInstance>(newProdLineExInstances);
        }

        private List<ProdLineExInstance> GenProdLineExInstance(DateTime snapTime, IList<ProdLineExInstance> oldProdLineExInstanceList,
            IList<ProdLineEx> prodLineExList, CodeMaster.TimeUnit dateType)
        {
            var newProdLineExInstances = new List<ProdLineExInstance>();
            oldProdLineExInstanceList = oldProdLineExInstanceList ?? new List<ProdLineExInstance>();
            var oldProdLineExInstanceDic = oldProdLineExInstanceList
                                            .GroupBy(p => p.DateIndex, (k, g) => new { k, g })
                                            .ToDictionary(d => d.k, d => d.g.ToList());

            for (int i = 0; i < 30; i++)
            {
                string dateIndex = null;
                DateTime currentDate = snapTime;
                if (dateType == CodeMaster.TimeUnit.Month)
                {
                    currentDate = snapTime.AddMonths(i);
                    dateIndex = currentDate.ToString("yyyy-MM");
                }
                else if (dateType == CodeMaster.TimeUnit.Week)
                {
                    currentDate = snapTime.AddDays(7 * i);
                    dateIndex = Utility.DateTimeHelper.GetWeekOfYear(currentDate);
                }
                else if (dateType == CodeMaster.TimeUnit.Day)
                {
                    currentDate = snapTime.AddDays(i);
                    dateIndex = snapTime.AddDays(i).ToString("yyyy-MM-dd");
                }
                else
                {
                    throw new BusinessException("不支持此类型的计划");
                }

                var oldProdLineExInstances = (oldProdLineExInstanceDic.ValueOrDefault(dateIndex) ?? new List<ProdLineExInstance>());

                #region 删除多余的
                foreach (var exInstance in oldProdLineExInstances)
                {
                    if (!exInstance.IsRelease && !exInstance.IsManualCreate)
                    {
                        var _proLineEx = prodLineExList
                            .Where(p => p.ProductLine == exInstance.ProductLine && p.Item == exInstance.Item
                            && p.StartDate <= currentDate && p.EndDate > currentDate);
                        if (_proLineEx == null || _proLineEx.Count() == 0)
                        {
                            this.genericMgr.Delete(exInstance);
                        }
                    }
                }
                #endregion

                var _oldProdLineExInstanceDic = oldProdLineExInstances.GroupBy(p => new { p.ProductLine, p.Item }, (k, g) => new { k, g })
                                            .ToDictionary(d => d.k, d => d.g.First());

                #region 已有的更新,没有的新增
                var prodLineExGroupList = prodLineExList.GroupBy(p => new { p.ProductLine, p.Item }, (k, g) => new { k, g });
                //Parallel.ForEach(prodLineExList, prodLineEx =>
                foreach (var prodLineExGroup in prodLineExGroupList)
                {
                    var prodLineEx = prodLineExGroup.g.Where(p => p.StartDate <= currentDate && p.EndDate > currentDate)
                        .OrderBy(p => p.StartDate).LastOrDefault();
                    if (prodLineEx != null)
                    {
                        var oldProdLineExInstance = _oldProdLineExInstanceDic.ValueOrDefault(new { prodLineEx.ProductLine, prodLineEx.Item });
                        if (oldProdLineExInstance == null)
                        {
                            var newProdLineExInstance = new ProdLineExInstance();
                            newProdLineExInstance.Region = prodLineEx.Region;
                            newProdLineExInstance.ApsPriority = prodLineEx.ApsPriority;
                            newProdLineExInstance.Correction = prodLineEx.Correction;
                            newProdLineExInstance.DateType = dateType;
                            newProdLineExInstance.EconomicLotSize = prodLineEx.EconomicLotSize;
                            newProdLineExInstance.IsManualCreate = false;
                            newProdLineExInstance.IsRelease = false;
                            newProdLineExInstance.Item = prodLineEx.Item;
                            newProdLineExInstance.MaxLotSize = prodLineEx.MaxLotSize;
                            newProdLineExInstance.MinLotSize = prodLineEx.MinLotSize;
                            newProdLineExInstance.ProductLine = prodLineEx.ProductLine;
                            newProdLineExInstance.Quota = prodLineEx.Quota;
                            newProdLineExInstance.MrpSpeed = prodLineEx.MrpSpeed;
                            newProdLineExInstance.RccpSpeed = prodLineEx.RccpSpeed;
                            newProdLineExInstance.SpeedTimes = prodLineEx.SpeedTimes;
                            newProdLineExInstance.SwitchTime = prodLineEx.SwitchTime;
                            newProdLineExInstance.TurnQty = prodLineEx.TurnQty;
                            newProdLineExInstance.DateIndex = dateIndex;
                            newProdLineExInstance.ShiftType = prodLineEx.ShiftType;
                            newProdLineExInstance.ProductType = prodLineEx.ProductType;
                            newProdLineExInstances.Add(newProdLineExInstance);
                        }
                        else if (!oldProdLineExInstance.IsRelease && !oldProdLineExInstance.IsManualCreate &&
                                (oldProdLineExInstance.Region != prodLineEx.Region ||
                                oldProdLineExInstance.ApsPriority != prodLineEx.ApsPriority ||
                                oldProdLineExInstance.Correction != prodLineEx.Correction ||
                                oldProdLineExInstance.DateType != dateType ||
                                oldProdLineExInstance.EconomicLotSize != prodLineEx.EconomicLotSize ||
                                oldProdLineExInstance.Item != prodLineEx.Item ||
                                oldProdLineExInstance.MaxLotSize != prodLineEx.MaxLotSize ||
                                oldProdLineExInstance.MinLotSize != prodLineEx.MinLotSize ||
                                oldProdLineExInstance.ProductLine != prodLineEx.ProductLine ||
                                oldProdLineExInstance.Quota != prodLineEx.Quota ||
                                oldProdLineExInstance.MrpSpeed != prodLineEx.MrpSpeed ||
                                oldProdLineExInstance.RccpSpeed != prodLineEx.RccpSpeed ||
                                oldProdLineExInstance.SpeedTimes != prodLineEx.SpeedTimes ||
                                oldProdLineExInstance.SwitchTime != prodLineEx.SwitchTime ||
                                oldProdLineExInstance.TurnQty != prodLineEx.TurnQty ||
                                oldProdLineExInstance.DateIndex != dateIndex ||
                                oldProdLineExInstance.ShiftType != prodLineEx.ShiftType ||
                                oldProdLineExInstance.ProductType != prodLineEx.ProductType))
                        {
                            oldProdLineExInstance.Region = prodLineEx.Region;
                            oldProdLineExInstance.ApsPriority = prodLineEx.ApsPriority;
                            oldProdLineExInstance.Correction = prodLineEx.Correction;
                            oldProdLineExInstance.DateType = dateType;
                            oldProdLineExInstance.EconomicLotSize = prodLineEx.EconomicLotSize;
                            oldProdLineExInstance.Item = prodLineEx.Item;
                            oldProdLineExInstance.MaxLotSize = prodLineEx.MaxLotSize;
                            oldProdLineExInstance.MinLotSize = prodLineEx.MinLotSize;
                            oldProdLineExInstance.ProductLine = prodLineEx.ProductLine;
                            oldProdLineExInstance.Quota = prodLineEx.Quota;
                            oldProdLineExInstance.MrpSpeed = prodLineEx.MrpSpeed;
                            oldProdLineExInstance.RccpSpeed = prodLineEx.RccpSpeed;
                            oldProdLineExInstance.SpeedTimes = prodLineEx.SpeedTimes;
                            oldProdLineExInstance.SwitchTime = prodLineEx.SwitchTime;
                            oldProdLineExInstance.TurnQty = prodLineEx.TurnQty;
                            oldProdLineExInstance.DateIndex = dateIndex;
                            oldProdLineExInstance.ShiftType = prodLineEx.ShiftType;
                            oldProdLineExInstance.ProductType = prodLineEx.ProductType;
                            this.genericMgr.Update(oldProdLineExInstance);
                        }
                    }
                }
                //);
                #endregion
            }
            return newProdLineExInstances;
        }
        #endregion

        #region GenMachineInstance
        [Transaction(TransactionMode.Requires)]
        public void GenMachineInstance(DateTime snapTime, IList<MrpFlowDetail> mrpFlowDetails)
        {
            string startWeekIndex = Utility.DateTimeHelper.GetWeekOfYear(snapTime);

            var oldMachineInstanceDic = this.genericMgr.FindAll<MachineInstance>
                ("from MachineInstance where (DateType=? and DateIndex>=? ) or (DateType=? and DateIndex>=? ) or (DateType=? and DateIndex>=? ) ",
                new object[] {
                    CodeMaster.TimeUnit.Day, DateTime.Now.ToString("yyyy-MM-dd"),
                    CodeMaster.TimeUnit.Week, startWeekIndex,
                    CodeMaster.TimeUnit.Month, DateTime.Now.ToString("yyyy-MM")
                }).GroupBy(p => p.DateType, (k, g) => new { k, g })
                .ToDictionary(d => d.k, d => d.g.ToList());

            var fiFlowDetails = mrpFlowDetails.Where(p => p.ResourceGroup == CodeMaster.ResourceGroup.FI).ToList();

            var machineList = this.genericMgr.FindAll<Machine>();
            List<MachineInstance> newMachineInstances = new List<MachineInstance>();

            var monthlyMachineInstances = GenMachineInstance(snapTime, oldMachineInstanceDic.ValueOrDefault(CodeMaster.TimeUnit.Month), machineList, fiFlowDetails, CodeMaster.TimeUnit.Month);
            newMachineInstances.AddRange(monthlyMachineInstances);
            var weeklyMachineInstances = GenMachineInstance(snapTime, oldMachineInstanceDic.ValueOrDefault(CodeMaster.TimeUnit.Week), machineList, fiFlowDetails, CodeMaster.TimeUnit.Week);
            newMachineInstances.AddRange(weeklyMachineInstances);
            var dailyMachineInstances = GenMachineInstance(snapTime, oldMachineInstanceDic.ValueOrDefault(CodeMaster.TimeUnit.Day), machineList, fiFlowDetails, CodeMaster.TimeUnit.Day);
            newMachineInstances.AddRange(dailyMachineInstances);

            this.genericMgr.BulkInsert<MachineInstance>(newMachineInstances);
        }

        private List<MachineInstance> GenMachineInstance(DateTime snapTime, IList<MachineInstance> oldMachineInstanceList, IList<Machine> machineList, IList<MrpFlowDetail> mrpFlowDetails, CodeMaster.TimeUnit dateType)
        {
            List<MachineInstance> newMachineInstances = new List<MachineInstance>();
            oldMachineInstanceList = oldMachineInstanceList ?? new List<MachineInstance>();
            var oldMachineInstanceDic = oldMachineInstanceList
                                .GroupBy(p => p.DateIndex, (k, g) => new { k, g })
                                .ToDictionary(d => d.k, d => d.g.ToList());
            //查找出最末月份
            var islands = this.genericMgr.FindAll<Island>();

            var holidays = GetWorkCalendar(snapTime, dateType, mrpFlowDetails);

            for (int i = 0; i < 30; i++)
            {
                string dateIndex = null;
                DateTime currentDate = snapTime;
                if (dateType == CodeMaster.TimeUnit.Month)
                {
                    currentDate = snapTime.AddMonths(i);
                    dateIndex = currentDate.ToString("yyyy-MM");
                }
                else if (dateType == CodeMaster.TimeUnit.Week)
                {
                    currentDate = snapTime.AddDays(7 * i);
                    dateIndex = Utility.DateTimeHelper.GetWeekOfYear(currentDate);
                }
                else if (dateType == CodeMaster.TimeUnit.Day)
                {
                    currentDate = snapTime.AddDays(i);
                    dateIndex = snapTime.AddDays(i).ToString("yyyy-MM-dd");
                }
                else
                {
                    throw new BusinessException("不支持此类型的计划");
                }

                var oldMachineInstances = oldMachineInstanceDic.ValueOrDefault(dateIndex) ?? new List<MachineInstance>();

                #region 删除多余的
                foreach (var machineInstance in oldMachineInstances)
                {
                    if (!machineInstance.IsRelease && !machineInstance.IsManualCreate)
                    {
                        var _machines = machineList.Where(p => p.Code == machineInstance.Code
                            && p.StartDate <= currentDate && p.EndDate > currentDate);
                        if (_machines == null || _machines.Count() == 0)
                        {
                            this.genericMgr.Delete(machineInstance);
                        }
                    }
                }
                #endregion

                var dateIndex_Holidays = holidays.Where(p => p.DateIndex == dateIndex);

                var _oldMachineInstanceDic = oldMachineInstances.GroupBy(p => p.Code, (k, g) => new { k, g })
                                     .ToDictionary(d => d.k, d => d.g.First());

                #region 已有的更新,没有的新增
                var machineGroupList = machineList.GroupBy(p => p.Code, (k, g) => new { k, g });
                //Parallel.ForEach(machineList, machine =>
                foreach (var machineGroup in machineGroupList)
                {
                    var machine = machineGroup.g.Where(p => p.StartDate <= currentDate && p.EndDate >= currentDate)
                        .OrderBy(p => p.StartDate).LastOrDefault();
                    if (machine != null)
                    {
                        if ((int)machine.ShiftType == 0)
                        {
                            machine.ShiftType = CodeMaster.ShiftType.ThreeShiftPerDay;
                        }
                        var flowDetail = mrpFlowDetails.FirstOrDefault(p => p.Machine == machine.Code);
                        flowDetail = flowDetail ?? new MrpFlowDetail();
                        var holiday = dateIndex_Holidays.FirstOrDefault(p => p.Flow == flowDetail.Flow);
                        holiday = holiday ?? new WorkCalendar();
                        var island = islands.FirstOrDefault(p => p.Code == machine.Island);
                        island = island ?? new Island();

                        var oldMachineInstance = _oldMachineInstanceDic.ValueOrDefault(machine.Code);
                        var shiftPerDay = machine.ShiftPerDay > (int)machine.ShiftType ? (int)machine.ShiftType : machine.ShiftPerDay;
                        if (oldMachineInstance == null)
                        {
                            var newMachineInstance = new MachineInstance();
                            newMachineInstance.Code = machine.Code;
                            newMachineInstance.Description = machine.Description;
                            newMachineInstance.DateType = dateType;
                            newMachineInstance.Island = machine.Island;
                            newMachineInstance.IsManualCreate = false;
                            newMachineInstance.IsRelease = false;
                            newMachineInstance.MachineType = machine.MachineType;
                            newMachineInstance.MaxWorkDayPerWeek = machine.MaxWorkDayPerWeek;
                            newMachineInstance.NormalWorkDayPerWeek = machine.NormalWorkDayPerWeek;
                            newMachineInstance.Qty = machine.Qty;
                            newMachineInstance.ShiftPerDay = shiftPerDay;
                            newMachineInstance.ShiftQuota = machine.ShiftQuota;
                            newMachineInstance.ShiftType = machine.ShiftType;
                            newMachineInstance.DateIndex = dateIndex;
                            newMachineInstance.IslandQty = island.Qty;
                            newMachineInstance.IslandDescription = island.Description;
                            newMachineInstance.Region = island.Region;
                            newMachineInstance.TrailTime = holiday.TrialTime;
                            newMachineInstance.HaltTime = holiday.HaltTime;
                            newMachineInstance.Holiday = holiday.Holiday;
                            newMachineInstance.Flow = flowDetail.Flow;
                            newMachineInstances.Add(newMachineInstance);
                        }
                        else if (!oldMachineInstance.IsRelease && !oldMachineInstance.IsManualCreate &&
                                (oldMachineInstance.Code != machine.Code ||
                                oldMachineInstance.Description != machine.Description ||
                                oldMachineInstance.DateType != dateType ||
                                oldMachineInstance.Island != machine.Island ||
                                oldMachineInstance.MachineType != machine.MachineType ||
                                oldMachineInstance.MaxWorkDayPerWeek != machine.MaxWorkDayPerWeek ||
                                oldMachineInstance.NormalWorkDayPerWeek != machine.NormalWorkDayPerWeek ||
                                oldMachineInstance.Qty != machine.Qty ||
                                oldMachineInstance.ShiftPerDay != shiftPerDay ||
                                oldMachineInstance.ShiftQuota != machine.ShiftQuota ||
                                oldMachineInstance.ShiftType != machine.ShiftType ||
                                oldMachineInstance.DateIndex != dateIndex ||
                                oldMachineInstance.IslandQty != island.Qty ||
                                oldMachineInstance.IslandDescription != island.Description ||
                                oldMachineInstance.Region != island.Region ||
                                oldMachineInstance.TrailTime != holiday.TrialTime ||
                                oldMachineInstance.HaltTime != holiday.HaltTime ||
                                oldMachineInstance.Holiday != holiday.Holiday ||
                                oldMachineInstance.Flow != flowDetail.Flow))
                        {
                            oldMachineInstance.Code = machine.Code;
                            oldMachineInstance.Description = machine.Description;
                            oldMachineInstance.DateType = dateType;
                            oldMachineInstance.Island = machine.Island;
                            oldMachineInstance.MachineType = machine.MachineType;
                            oldMachineInstance.MaxWorkDayPerWeek = machine.MaxWorkDayPerWeek;
                            oldMachineInstance.NormalWorkDayPerWeek = machine.NormalWorkDayPerWeek;
                            oldMachineInstance.Qty = machine.Qty;
                            oldMachineInstance.ShiftPerDay = shiftPerDay;
                            oldMachineInstance.ShiftQuota = machine.ShiftQuota;
                            oldMachineInstance.ShiftType = machine.ShiftType;
                            oldMachineInstance.DateIndex = dateIndex;
                            oldMachineInstance.IslandQty = island.Qty;
                            oldMachineInstance.IslandDescription = island.Description;
                            oldMachineInstance.Region = island.Region;
                            oldMachineInstance.TrailTime = holiday.TrialTime;
                            oldMachineInstance.HaltTime = holiday.HaltTime;
                            oldMachineInstance.Holiday = holiday.Holiday;
                            oldMachineInstance.Flow = flowDetail.Flow;
                            this.genericMgr.Update(oldMachineInstance);
                        }
                    }
                }
                //);

                #endregion
            }
            return newMachineInstances;
        }
        #endregion

        #endregion

        #region RunMrp
        private static object RunMrpLock = new object();
        [Transaction(TransactionMode.Requires)]
        public void RunMrp(DateTime newPlanVersion, DateTime sourcePlanVersion, CodeMaster.ResourceGroup resourceGroup, string dateIndex, User user)
        {
            lock (RunMrpLock)
            {
                BusinessException businessException = new BusinessException();
                DateTime snapTime = sourcePlanVersion;
                try
                {
                    if (resourceGroup != CodeMaster.ResourceGroup.FI)
                    {
                        snapTime = this.genericMgr.FindAll<MrpSnapMaster>
                            (" from MrpSnapMaster where IsRelease = ? and Type=? Order by SnapTime desc",
                            new object[] { true, CodeMaster.SnapType.Mrp }, 0, 1).First().SnapTime;
                    }

                    newPlanVersion = DateTime.Parse(newPlanVersion.ToString("yyyy-MM-dd HH:mm:ss"));

                    SecurityContextHolder.Set(user);

                    logRunMrp.Info("---**--------Start RunMrp effectivedate:" + snapTime.ToLocalTime() + "-----");

                    #region 获取路线
                    logRunMrp.Info(string.Format("-{0} - 开始获取路线-", newPlanVersion));
                    var mrpFlowDetailList = this.genericMgr.FindAll<MrpFlowDetail>
                        (@"from MrpFlowDetail as m where m.SnapTime = ? ", new object[] { snapTime });
                    var distributionFlowDetails = mrpFlowDetailList.Where(f => f.Type == CodeMaster.OrderType.Distribution).ToList();
                    logRunMrp.Info(string.Format("-{0} - 结束获取路线-", newPlanVersion));
                    #endregion

                    #region 库位
                    var locationDic = this.genericMgr.FindAll<Location>().ToDictionary(d => d.Code, d => d);
                    #endregion

                    #region 获取库存 在途
                    logRunMrp.Info(string.Format("-{0} - 开始获取库存(在途)-", newPlanVersion));
                    var inventoryBalances = this.genericMgr.FindAll<InventoryBalance>
                        (@"from InventoryBalance as m where m.SnapTime = ?", new object[] { snapTime });

                    var transitOrderList = this.genericMgr.FindAll<TransitOrder>
                        ("from TransitOrder as m where m.SnapTime = ?", new object[] { snapTime });
                    foreach (var transitOrder in transitOrderList)
                    {
                        var inventoryBalance = new InventoryBalance();
                        inventoryBalance.Item = transitOrder.Item;
                        if (transitOrder.ShippedQty > transitOrder.ReceivedQty)
                        {
                            inventoryBalance.Qty = transitOrder.ShippedQty - transitOrder.ReceivedQty;
                        }
                        inventoryBalances.Add(inventoryBalance);
                    }

                    inventoryBalances = inventoryBalances.GroupBy(p => p.Item)
                        .Select(p =>
                        {
                            var inventoryBalance = new InventoryBalance();
                            inventoryBalance.Item = p.Key;
                            inventoryBalance.Qty = p.Where(q => q.Qty > 0).Sum(q => q.Qty);
                            inventoryBalance.SafeStock = p.Sum(q => q.SafeStock);
                            inventoryBalance.MaxStock = p.Sum(q => q.MaxStock);
                            return inventoryBalance;
                        }).ToList();

                    var inventoryBalanceDic = inventoryBalances.GroupBy(p => p.Item)
                        .ToDictionary(d => d.Key, d => d.Sum(b => b.Qty));

                    //var receivePlanList = new List<MrpReceivePlan>();
                    //foreach (TransitOrder transitOrder in transitOrderList)
                    //{
                    //    MrpReceivePlan receivePlan = new MrpReceivePlan();
                    //    receivePlan.Item = transitOrder.Item;
                    //    receivePlan.LocationFrom = transitOrder.Location;
                    //    receivePlan.Qty = -transitOrder.TransitQty;
                    //    receivePlan.SourceId = transitOrder.OrderDetailId;
                    //    receivePlan.Flow = transitOrder.Flow;
                    //    receivePlan.OrderType = transitOrder.OrderType;
                    //    receivePlan.ReceiveTime = transitOrder.WindowTime;
                    //    receivePlan.SourceParty = locationDic[transitOrder.Location].Region;
                    //    receivePlan.StartTime = transitOrder.StartTime;

                    //    receivePlanList.Add(receivePlan);
                    //    //logRunMrp.Info(GetTLog(receivePlan, "Create receive plan for TransitOrder"));
                    //}

                    //foreach (var inventory in inventoryBalances)
                    //{
                    //    if (inventory.ActiveQty != 0)
                    //    {
                    //        MrpReceivePlan receivePlan = new MrpReceivePlan();
                    //        receivePlan.Item = inventory.Item;
                    //        receivePlan.LocationFrom = inventory.Location;
                    //        receivePlan.Qty = -inventory.ActiveQty;
                    //        receivePlan.SourceId = inventory.Id;
                    //        receivePlan.ReceiveTime = DateTime.Now.Date;

                    //        //receivePlan.Uom = inventory.Uom;
                    //        receivePlan.SourceParty = locationDic[inventory.Location].Region;

                    //        receivePlanList.Add(receivePlan);
                    //        //logRunMrp.Info(GetTLog(receivePlan, "Create receive plan for safe stock"));
                    //    }
                    //}

                    //var groupReceivePlan = (from p in receivePlanList
                    //                        group p by new
                    //                        {
                    //                            p.Item,
                    //                            Location = p.LocationFrom,
                    //                            p.SourceParty
                    //                        } into g
                    //                        select new MrpReceivePlan
                    //                        {
                    //                            Item = g.Key.Item,
                    //                            LocationFrom = g.Key.Location,
                    //                            SourceParty = g.Key.SourceParty,
                    //                            SourceId = g.First().SourceId,
                    //                            Qty = g.Sum(t => t.Qty)
                    //                        }).ToList();
                    logRunMrp.Info(string.Format("-{0} - 结束获取库存(在途)-", newPlanVersion));
                    #endregion

                    #region MrpContainer
                    MrpInContainer mrpInContainer = new MrpInContainer();
                    mrpInContainer.MrpFlowDetailDic = mrpFlowDetailList.Where(p => p.Type != CodeMaster.OrderType.Distribution)
                        .GroupBy(p => p.Item, (k, g) => new { k, List = g.ToList() }).ToDictionary(d => d.k, d => d.List);
                    mrpInContainer.BusinessException = businessException;
                    mrpInContainer.ResourceGroup = resourceGroup;

                    var shipPlanList = new List<MrpShipPlan>();
                    #endregion MrpContainer

                    #region 获取客户需求 如果已经排班,则按排班的计划计算需求.
                    logRunMrp.Info(string.Format("-{0} - 开始获取需求-", newPlanVersion));
                    List<MrpShipPlan> mrpShipPlanList = new List<MrpShipPlan>();

                    if (resourceGroup == CodeMaster.ResourceGroup.FI)
                    {
                        #region 独立需求
                        var indepentOrderList = this.genericMgr.FindAll<ActiveOrder>
                            (@"from ActiveOrder as m where m.SnapTime = ? and IsIndepentDemand =? ",
                            new object[] { snapTime, true });

                        mrpShipPlanList.AddRange(from p in indepentOrderList
                                                 select new MrpShipPlan
                                                 {
                                                     Flow = p.Flow,
                                                     OrderType = p.OrderType,
                                                     Item = p.Item,
                                                     LocationFrom = p.LocationFrom,
                                                     LocationTo = p.LocationTo,
                                                     StartTime = p.StartTime,
                                                     WindowTime = p.WindowTime,
                                                     SourceType = CodeMaster.MrpSourceType.Order,
                                                     SourceId = p.OrderDetId,
                                                     Qty = p.DemandQty,
                                                     SourceParty = p.PartyTo
                                                 });
                        #endregion

                        #region 发货计划做为后加工的需求来源
                        var mrpPlanList = this.genericMgr.FindAll<MrpPlan>
                            ("from MrpPlan as d where d.PlanDate >= ? ", snapTime);
                        mrpShipPlanList.AddRange(from p in mrpPlanList
                                                 select new MrpShipPlan
                                                 {
                                                     Flow = p.Flow,
                                                     OrderType = p.OrderType,
                                                     Item = p.Item,
                                                     StartTime = p.PlanDate,
                                                     WindowTime = p.PlanDate,
                                                     LocationFrom = p.Location,
                                                     SourceId = p.PlanVersion,
                                                     Qty = p.LeftQty,
                                                     SourceType = CodeMaster.MrpSourceType.Plan,
                                                     SourceParty = p.Party
                                                 });
                        #endregion

                        //#region 处理库存 小于0补充 大于0可用
                        //logRunMrp.Info(string.Format("-{0} - 开始库存MRP迭代计算-", newPlanVersion));
                        //IterateInventory(newPlanVersion, groupReceivePlan, mrpInContainer, shipPlanList);
                        //logRunMrp.Info(string.Format("-{0} - 结束库存MRP迭代计算-", newPlanVersion));
                        //#endregion
                    }
                    else
                    {
                        // 上级的班产计划作为本级的需求来源 订单优先
                        var oldMrpPlanMaster = this.genericMgr.FindById<MrpPlanMaster>(sourcePlanVersion);
                        var shiftPlanList = GetProductPlanInList(oldMrpPlanMaster, snapTime, snapTime.AddDays(30), mrpFlowDetailList);
                        mrpShipPlanList.AddRange(shiftPlanList);
                        var productLines = mrpFlowDetailList.Where(p => p.ResourceGroup == resourceGroup)
                            .Select(p => p.Flow).Distinct();
                        //上级发货计划
                        var parentShipPlanList = this.genericMgr.FindAllIn<MrpShipPlan>
                            ("from MrpShipPlan where PlanVersion=? and Flow in(? ",
                           productLines, new object[] { oldMrpPlanMaster.PlanVersion });
                        mrpShipPlanList.AddRange(parentShipPlanList);
                        if (resourceGroup == CodeMaster.ResourceGroup.MI)
                        {
                            //上上级的发货计划
                            var fiShipPlanList = this.genericMgr.FindAllIn<MrpShipPlan>
                                ("from MrpShipPlan where PlanVersion=? and Flow in(? ",
                               productLines, new object[] { oldMrpPlanMaster.SourcePlanVersion });
                            mrpShipPlanList.AddRange(fiShipPlanList);
                        }
                    }
                    logRunMrp.Info(string.Format("-{0} - 数据准备结束-", newPlanVersion));
                    #endregion

                    #region 循环生成入库计划/发货计划
                    logRunMrp.Info(string.Format("-{0} - 开始发货计划MRP迭代计算-", newPlanVersion));
                    IterateShipPlan(newPlanVersion, mrpShipPlanList, mrpInContainer, distributionFlowDetails, shipPlanList);
                    logRunMrp.Info(string.Format("-{0} - 结束发货计划MRP迭代计算-", newPlanVersion));
                    #endregion

                    //test
                    var test = shipPlanList.Where(p => p.Item == "300024").ToList();

                    #region 汇总ShipPlan 圆整到天
                    logRunMrp.Info(string.Format("-{0} - 汇总ShipPlan-", newPlanVersion));
                    var shipPlanGroupList = (from p in shipPlanList
                                             orderby p.WindowTime
                                             group p by
                                             new
                                             {
                                                 WindowTime = p.WindowTime.Date,
                                                 StartTime = p.StartTime.Date,
                                                 Flow = p.Flow,
                                                 Item = p.Item,
                                                 LocationFrom = p.LocationFrom,
                                                 LocationTo = p.LocationTo,
                                                 OrderType = p.OrderType,
                                             } into g
                                             select new MrpShipPlanGroup
                                             {
                                                 WindowTime = g.Key.WindowTime,
                                                 StartTime = g.Key.StartTime,
                                                 Flow = g.Key.Flow,
                                                 Item = g.Key.Item,
                                                 LocationFrom = g.Key.LocationFrom,
                                                 LocationTo = g.Key.LocationTo,
                                                 OrderType = g.Key.OrderType,
                                                 Qty = g.Sum(t => t.Qty),
                                                 ShipQty = g.Sum(t => t.Qty),
                                                 PlanVersion = newPlanVersion,
                                                 IsStockOver = g.Where(p => p.SourceType == CodeMaster.MrpSourceType.StockOver).Count() > 0,
                                                 MrpShipPlanList = g.OrderBy(q => q.WindowTime).ToList()
                                             }).ToList();
                    #endregion

                    #region 消耗替代物料,移动负需求
                    //ConsumeDiscontinueShipPlanGroup(itemDiscontinueList, shipPlanGroupList);
                    //ConsumeMinusShipPlanGroup(shipPlanGroupList);
                    #endregion

                    #region 写入数据库 shipPlanList groupShipPlanList
                    logRunMrp.Info(string.Format("-{0} - 开始保存GroupShipPlan -", newPlanVersion));
                    CreateShipPlan(shipPlanGroupList);
                    logRunMrp.Info(string.Format("-{0} - 结束保存GroupShipPlan -", newPlanVersion));
                    #endregion 写入数据库

                    //todo
                    if (true || businessException.GetMessages().Where(p => p.MessageType == CodeMaster.MessageType.Error).Count() == 0)
                    {
                        #region 排产
                        //后加工
                        if (resourceGroup == CodeMaster.ResourceGroup.FI)
                        {
                            logRunMrp.Info(string.Format("-{0} - 开始后加工排产 -", newPlanVersion));
                            this.ScheduleFi(shipPlanList, mrpFlowDetailList, dateIndex, inventoryBalances, snapTime, newPlanVersion, businessException);
                            logRunMrp.Info(string.Format("-{0} - 结束后加工排产 -", newPlanVersion));
                        }
                        //挤出
                        if (resourceGroup == CodeMaster.ResourceGroup.EX)
                        {
                            this.ScheduleEx(shipPlanGroupList, mrpFlowDetailList, dateIndex, inventoryBalances, snapTime, newPlanVersion, businessException);
                        }
                        // 炼胶
                        if (resourceGroup == CodeMaster.ResourceGroup.MI)
                        {
                            //this.ScheduleMi(shipPlanList, mrpFlowDetailList, inventoryBalanceDic, itemDiscontinueList, snapTime, newPlanVersion, businessException);
                        }
                        #endregion

                        #region 采购
                        if (resourceGroup == CodeMaster.ResourceGroup.Other)
                        {
                            ScheduleTransferPlan(shipPlanGroupList, newPlanVersion, snapTime, sourcePlanVersion, user);
                            SchedulePurchasePlan(shipPlanGroupList, newPlanVersion, snapTime, sourcePlanVersion, user);
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    logRunMrp.Fatal(ex);
                    businessException.AddMessage(new Message(CodeMaster.MessageType.Error, ex.StackTrace));
                }

                #region 记录log
                List<MrpLog> logs = new List<MrpLog>();
                CodeMaster.MessageType messageStatus = CodeMaster.MessageType.Info;
                if (businessException.HasMessage)
                {
                    var messages = businessException.GetMessages().GroupBy(p =>
                     new { Message = p.GetMessageString(), MessageType = p.MessageType },
                     (k, g) => new { k.Message, k.MessageType });
                    foreach (var message in messages)
                    {
                        MrpLog log = new MrpLog();
                        log.ErrorLevel = message.MessageType.ToString();
                        log.Message = message.Message;
                        log.Logger = "RunMrp";
                        log.PlanVersion = newPlanVersion;
                        logs.Add(log);
                        if (message.MessageType == CodeMaster.MessageType.Warning)
                        {
                            logRunMrp.Warn(log.Message);
                        }
                        else if (message.MessageType == CodeMaster.MessageType.Error)
                        {
                            logRunMrp.Error(log.Message);
                        }
                        else
                        {
                            logRunMrp.Info(log.Message);
                        }
                    }
                    if (messages.Count(f => f.MessageType == CodeMaster.MessageType.Error) > 0)
                    {
                        messageStatus = CodeMaster.MessageType.Error;
                    }
                    else if (messages.Count(f => f.MessageType == CodeMaster.MessageType.Warning) > 0)
                    {
                        messageStatus = CodeMaster.MessageType.Warning;
                    }
                }

                #region 记录MRP Run日志
                MrpPlanMaster mrpPlanMaster = new MrpPlanMaster();
                mrpPlanMaster.SnapTime = snapTime;
                mrpPlanMaster.PlanVersion = newPlanVersion;
                mrpPlanMaster.DateIndex = dateIndex;
                mrpPlanMaster.SourcePlanVersion = sourcePlanVersion;
                mrpPlanMaster.ResourceGroup = resourceGroup;
                mrpPlanMaster.Status = messageStatus;

                mrpPlanMaster.CreateUserId = user.Id;
                mrpPlanMaster.CreateUserName = user.FullName;
                mrpPlanMaster.CreateDate = DateTime.Now;
                mrpPlanMaster.LastModifyUserId = user.Id;
                mrpPlanMaster.LastModifyUserName = user.FullName;
                mrpPlanMaster.LastModifyDate = DateTime.Now;

                this.genericMgr.Create(mrpPlanMaster);
                #endregion

                double timetick = 1001 - (mrpPlanMaster.CreateDate - newPlanVersion).TotalMilliseconds;
                timetick = timetick > 0 ? timetick : 0;
                Thread.Sleep((int)timetick);

                string infoLog = string.Format("完成时间:{0},时间总计:{1}秒", DateTime.Now.ToLocalTime(), (DateTime.Now - newPlanVersion).TotalSeconds);
                logRunMrp.Info(infoLog);
                MrpLog mrplog = new MrpLog();
                mrplog.ErrorLevel = CodeMaster.MessageType.Info.ToString();
                mrplog.Message = infoLog;
                mrplog.Logger = "RunMrp";
                mrplog.PlanVersion = newPlanVersion;
                logs.Add(mrplog);
                this.genericMgr.BulkInsert<MrpLog>(logs);
                #endregion
            }
        }

        private static object RunMrpExLock = new object();
        [Transaction(TransactionMode.Requires)]
        public void RunMrpEx(DateTime newPlanVersion, DateTime sourcePlanVersion, string dateIndex, User user)
        {
            lock (RunMrpExLock)
            {
                SecurityContextHolder.Set(user);
                BusinessException businessException = new BusinessException();
                var snapTime = this.genericMgr.FindAll<MrpSnapMaster>
                  (" from MrpSnapMaster where IsRelease = ? and Type=? Order by SnapTime desc",
                  new object[] { true, CodeMaster.SnapType.Mrp }, 0, 1).First().SnapTime;
                newPlanVersion = DateTime.Parse(newPlanVersion.ToString("yyyy-MM-dd HH:mm:ss"));

                logRunMrp.Info("---**--------Start RunMrp effectivedate:" + snapTime.ToLocalTime() + "-----");

                #region 获取路线
                logRunMrp.Info(string.Format("-{0} - 开始获取路线-", newPlanVersion));
                var mrpFlowDetailList = this.genericMgr.FindAll<MrpFlowDetail>
                    (@"from MrpFlowDetail as m where m.SnapTime = ? ", new object[] { snapTime });
                logRunMrp.Info(string.Format("-{0} - 结束获取路线-", newPlanVersion));
                #endregion

                #region 获取库存 在途
                logRunMrp.Info(string.Format("-{0} - 开始获取库存(在途)-", newPlanVersion));
                var inventoryBalances = this.genericMgr.FindAll<InventoryBalance>
                    (@"from InventoryBalance as m where m.SnapTime = ?", new object[] { snapTime });

                var transitOrderList = this.genericMgr.FindAll<TransitOrder>
                    ("from TransitOrder as m where m.SnapTime = ?", new object[] { snapTime });
                foreach (var transitOrder in transitOrderList)
                {
                    var inventoryBalance = new InventoryBalance();
                    inventoryBalance.Item = transitOrder.Item;
                    if (transitOrder.ShippedQty > transitOrder.ReceivedQty)
                    {
                        inventoryBalance.Qty = transitOrder.ShippedQty - transitOrder.ReceivedQty;
                    }
                    inventoryBalances.Add(inventoryBalance);
                }

                inventoryBalances = inventoryBalances.GroupBy(p => p.Item)
                    .Select(p =>
                    {
                        var inventoryBalance = new InventoryBalance();
                        inventoryBalance.Item = p.Key;
                        inventoryBalance.Qty = p.Where(q => q.Qty > 0).Sum(q => q.Qty);
                        inventoryBalance.SafeStock = p.Sum(q => q.SafeStock);
                        inventoryBalance.MaxStock = p.Sum(q => q.MaxStock);
                        return inventoryBalance;
                    }).ToList();

                var inventoryBalanceDic = inventoryBalances.GroupBy(p => p.Item)
                    .ToDictionary(d => d.Key, d => d.Sum(b => b.Qty));

                logRunMrp.Info(string.Format("-{0} - 结束获取库存(在途)-", newPlanVersion));
                #endregion

                //后加工的需求 温祥永
                var shipPlanGroupList = new List<MrpShipPlanGroup>();
                //todo
                this.ScheduleEx(shipPlanGroupList, mrpFlowDetailList, dateIndex, inventoryBalances, snapTime, newPlanVersion, businessException);
            }
        }


        private void ConsumeMinusShipPlanGroup(List<MrpShipPlanGroup> shipPlanGroupList)
        {
            var shipPlanGroupByFlows = from p in shipPlanGroupList
                                       where !p.IsDiscontinueItem
                                       group p by new
                                       {
                                           p.Flow,
                                           p.Item
                                       } into g
                                       select new { g.Key, List = g };
            foreach (var shipPlanGroupByFlow in shipPlanGroupByFlows)
            {
                double qty = shipPlanGroupByFlow.List.Where(p => p.IsStockOver).Sum(p => p.Qty);
                if (qty < 0)
                {
                    foreach (var shipPlanGroup in shipPlanGroupByFlow.List)
                    {
                        if (shipPlanGroup.Qty >= -qty)
                        {
                            shipPlanGroup.Qty += qty;
                            break;
                        }
                        else
                        {
                            shipPlanGroup.Qty = 0;
                            qty += shipPlanGroup.Qty;
                        }
                    }
                }
            }
        }

        private void ConsumeDiscontinueShipPlanGroup(IList<ItemDiscontinue> itemDiscontinueList, List<MrpShipPlanGroup> shipPlanGroupList)
        {
            var groupDisShipPlans = shipPlanGroupList
                .Where(p => itemDiscontinueList.Select(q => q.DiscontinueItem).Distinct().Contains(p.Item));
            foreach (var groupDisShipPlan in groupDisShipPlans)
            {
                var qty = groupDisShipPlan.Qty;
                groupDisShipPlan.IsDiscontinueItem = true;
                var _shipPlanGroups = shipPlanGroupList.Where(p => itemDiscontinueList
                    .Where(q => q.DiscontinueItem == groupDisShipPlan.Item)
                    .Select(q => q.Item)
                    .Distinct().Contains(p.Item));
                foreach (var shipPlanGroup in _shipPlanGroups)
                {
                    if (shipPlanGroup.Qty >= -qty)
                    {
                        shipPlanGroup.Qty += qty;
                        break;
                    }
                    else
                    {
                        shipPlanGroup.Qty = 0;
                        qty += shipPlanGroup.Qty;
                    }
                }
            }
        }
        #endregion

        #region CreateShipPlan
        private void CreateShipPlan(IList<MrpShipPlanGroup> groupShipPlanList)
        {
            //因为BulkInsert取不到自增Id,Id手动赋值
            int id = 0;
            var lastMrpShipPlanGroups = this.genericMgr.FindAll<MrpShipPlanGroup>(" from MrpShipPlanGroup Order by Id desc ", 0, 1);
            if (lastMrpShipPlanGroups != null && lastMrpShipPlanGroups.Count > 0)
            {
                id = lastMrpShipPlanGroups.First().Id;
            }
            var mrpShipPlans = new List<MrpShipPlan>();
            foreach (var groupShipPlan in groupShipPlanList)
            {
                id++;
                groupShipPlan.Id = id;
                foreach (var shipPlan in groupShipPlan.MrpShipPlanList)
                {
                    shipPlan.GroupId = groupShipPlan.Id;
                    shipPlan.PlanVersion = groupShipPlan.PlanVersion;
                }
                mrpShipPlans.AddRange(groupShipPlan.MrpShipPlanList);
            }
            return;//debug
            this.genericMgr.BulkInsert<MrpShipPlanGroup>(groupShipPlanList);
            this.genericMgr.BulkInsert<MrpShipPlan>(mrpShipPlans);
        }

        private void IterateShipPlan(DateTime newPlanVersion, IList<MrpShipPlan> mrpShipPlanList,
            MrpInContainer mrpInContainer, IList<MrpFlowDetail> distributionFlowDetails, List<MrpShipPlan> shipPlanList)
        {
            //var test = mrpShipPlanList.Where(p => p.Item == "300956").ToList();

            var sortedMrpShipPlanList = mrpShipPlanList.OrderBy(p => p.StartTime);
            //Parallel.ForEach(sortedMrpShipPlanList, shipPlan =>
            foreach (var shipPlan in sortedMrpShipPlanList)
            {
                shipPlan.MrpFlowDetail = distributionFlowDetails
                    .FirstOrDefault(f => f.Flow == shipPlan.Flow && f.Item == shipPlan.Item
                        && f.StartDate <= shipPlan.StartTime && f.EndDate > shipPlan.StartTime);

                shipPlanList.Add(shipPlan);
                CalNextReceivePlan(mrpInContainer, shipPlanList, shipPlan, 0);
            }
            //);
        }

        private void IterateInventory(DateTime newPlanVersion, IList<MrpReceivePlan> receivePlanList, MrpInContainer mrpInContainer, List<MrpShipPlan> shipPlanList)
        {
            //var test = receivePlanList.Where(p => p.Item == "300956").ToList();

            //Parallel.ForEach(groupReceivePlan, receivePlan =>
            foreach (var receivePlan in receivePlanList)
            {
                if (receivePlan.Qty != 0)
                {
                    receivePlan.ReceiveTime = newPlanVersion;
                    receivePlan.SourceType = receivePlan.Qty < 0 ? CodeMaster.MrpSourceType.StockOver : CodeMaster.MrpSourceType.StockLack;
                    receivePlan.PlanVersion = newPlanVersion;
                    receivePlan.StartTime = newPlanVersion;
                    //receivePlan.SourceDateType = CodeMaster.TimeUnit.Day;
                    //this.genericMgr.Create(receivePlan);

                    CalNextShipPlan(mrpInContainer, shipPlanList, receivePlan, 0);
                }
            }
            //);
        }

        private void CalNextReceivePlan(MrpInContainer mrpInContainer, List<MrpShipPlan> mrpShipPlanBag, MrpShipPlan shipPlan, int iterateCount)
        {
            if (shipPlan.Qty == 0)
            {
                return;
            }

            iterateCount++;
            if (iterateCount > 50)
            {
                logRunMrp.Fatal(string.Format("物料{0}已经迭代了50级物流路线仍未结束,可能存在循环的物流路线", shipPlan.Item));
                return;//防止死循环,最多迭代50级物流路线
            }
            var flowDetail = shipPlan.MrpFlowDetail;
            if (flowDetail == null)
            {
                var flowDetails = mrpInContainer.MrpFlowDetailDic.ValueOrDefault(shipPlan.Item) ?? new List<MrpFlowDetail>();
                flowDetail = flowDetails.FirstOrDefault(f => f.Flow == shipPlan.Flow && f.Item == shipPlan.Item
                        && f.StartDate <= shipPlan.StartTime && f.EndDate > shipPlan.StartTime);
                shipPlan.MrpFlowDetail = flowDetail;
            }
            if (flowDetail == null)
            {
                var defaultFlowDetail = mrpInContainer.MrpFlowDetailDic.SelectMany(p => p.Value).Where(p => p.Flow == shipPlan.Flow).FirstOrDefault();
                if (defaultFlowDetail == null)
                {
                    mrpInContainer.BusinessException.AddMessage(
                        new Message(CodeMaster.MessageType.Info, "没有找到对应的物流路线{0}的物料{1}明细", shipPlan.Flow, shipPlan.Item));
                    return;
                }
                else
                {
                    mrpInContainer.BusinessException.AddMessage(
                        new Message(CodeMaster.MessageType.Warning, "没有找到对应的物流路线{0}的物料{1}明细,系统将自动补充明细", shipPlan.Flow, shipPlan.Item));
                }
                flowDetail = new MrpFlowDetail();
                flowDetail.Flow = shipPlan.Flow;
                flowDetail.Item = shipPlan.Item;
                flowDetail.Bom = string.IsNullOrWhiteSpace(shipPlan.Bom) ? shipPlan.Item : shipPlan.Bom;
                flowDetail.LocationFrom = shipPlan.LocationFrom;
                flowDetail.LocationTo = shipPlan.LocationTo;
                flowDetail.PartyFrom = defaultFlowDetail.PartyFrom;
                flowDetail.PartyTo = defaultFlowDetail.PartyTo;
                flowDetail.ResourceGroup = defaultFlowDetail.ResourceGroup;
                flowDetail.StartDate = DateTime.MinValue;
                flowDetail.EndDate = DateTime.MinValue;
                flowDetail.Type = defaultFlowDetail.Type;
                var item = this.itemMgr.GetCacheItem(shipPlan.Item);
                flowDetail.UnitCount = (double)item.UnitCount;
                flowDetail.Uom = item.Uom;
                flowDetail.Sequence = 10000;
            }

            #region 赋默认值
            if (string.IsNullOrWhiteSpace(shipPlan.ParentItem))
            {
                shipPlan.ParentItem = flowDetail.Item;
            }
            if (string.IsNullOrWhiteSpace(shipPlan.SourceParty))
            {
                shipPlan.SourceParty = flowDetail.PartyTo;
            }
            if (string.IsNullOrWhiteSpace(shipPlan.SourceFlow))
            {
                shipPlan.SourceFlow = flowDetail.Flow;
            }
            #endregion

            #region 生成入库计划
            var receivePlanList = new List<MrpReceivePlan>();

            if (shipPlan.OrderType == CodeMaster.OrderType.Production || shipPlan.OrderType == CodeMaster.OrderType.SubContract)
            {
                if (shipPlan.Qty < 0)
                {
                    return;
                }
                if (flowDetail != null)
                {
                    #region 是否继续往下算
                    bool needCalNext = false;

                    if (mrpInContainer.ResourceGroup == CodeMaster.ResourceGroup.EX
                        && flowDetail.ResourceGroup == CodeMaster.ResourceGroup.FI)
                    {
                        needCalNext = true;
                    }
                    else if (mrpInContainer.ResourceGroup == CodeMaster.ResourceGroup.MI
                        && flowDetail.ResourceGroup == CodeMaster.ResourceGroup.EX)
                    {
                        needCalNext = true;
                    }
                    else if (mrpInContainer.ResourceGroup == CodeMaster.ResourceGroup.Other
                        && flowDetail.ResourceGroup == CodeMaster.ResourceGroup.MI)
                    {
                        needCalNext = true;
                    }
                    else if (shipPlan.OrderType == CodeMaster.OrderType.SubContract)
                    {
                        needCalNext = true;
                    }
                    //注意:溢出的库存只留在当前节点,需求不往下传递
                    if (shipPlan.SourceType == CodeMaster.MrpSourceType.StockOver)
                    {
                        needCalNext = false;
                    }
                    #endregion 是否继续往下算

                    if (needCalNext)
                    {
                        #region 生产，需要分解Bom
                        var mrpBoms = GetMrpBomList(flowDetail.Item, flowDetail.Bom, shipPlan.StartTime, mrpInContainer.BusinessException);

                        foreach (var mrpBom in mrpBoms)
                        {
                            #region 创建ReceivePlan
                            MrpReceivePlan receivePlan = new MrpReceivePlan();
                            receivePlan.Item = mrpBom.Item;
                            receivePlan.ReceiveTime = shipPlan.StartTime;
                            receivePlan.SourceId = shipPlan.SourceId;
                            receivePlan.SourceType = shipPlan.SourceType;
                            // 取库位
                            receivePlan.LocationFrom = shipPlan.LocationFrom;  //默认库位
                            receivePlan.ExtraLocationFrom = flowDetail.ExtraLocationFrom;  //默认库位
                            if (mrpBom.Location != null)
                            {
                                receivePlan.LocationFrom = mrpBom.Location;
                            }
                            receivePlan.Qty = shipPlan.Qty * mrpBom.RateQty;
                            receivePlan.StartTime = shipPlan.StartTime;
                            receivePlan.Flow = shipPlan.Flow;
                            receivePlan.OrderType = shipPlan.OrderType;
                            receivePlan.Bom = flowDetail.Bom;

                            receivePlan.SourceParty = flowDetail.PartyFrom;
                            receivePlan.SourceFlow = flowDetail.Flow;
                            receivePlan.ParentItem = flowDetail.Item;
                            receivePlanList.Add(receivePlan);
                            #endregion
                        }
                        #endregion
                    }
                }
            }
            else
            {
                #region 非生产直接从发运计划变为入库计划
                MrpReceivePlan receivePlan = new MrpReceivePlan();
                receivePlan.Item = shipPlan.Item;
                receivePlan.LocationFrom = shipPlan.LocationFrom;
                receivePlan.ExtraLocationFrom = flowDetail.ExtraLocationFrom;
                receivePlan.Qty = shipPlan.Qty;  //转换为库存单位
                receivePlan.ReceiveTime = shipPlan.StartTime;
                receivePlan.SourceId = shipPlan.SourceId;
                //receivePlan.SourceDateType = shipPlan.SourceDateType;
                receivePlan.SourceType = shipPlan.SourceType;
                receivePlan.StartTime = shipPlan.StartTime;
                receivePlan.Flow = shipPlan.Flow;
                receivePlan.OrderType = shipPlan.OrderType;

                receivePlan.SourceParty = string.IsNullOrWhiteSpace(shipPlan.SourceParty) ? flowDetail.PartyTo : shipPlan.SourceParty;
                receivePlan.SourceFlow = string.IsNullOrWhiteSpace(shipPlan.SourceFlow) ? flowDetail.Flow : shipPlan.SourceFlow;
                receivePlan.ParentItem = string.IsNullOrWhiteSpace(shipPlan.ParentItem) ? flowDetail.Item : shipPlan.ParentItem;

                //this.genericMgr.Create(receivePlan);

                receivePlanList.Add(receivePlan);
                //logRunMrp.Debug(GetTLog(receivePlan, "Transfer ship plan to receive plan"));
                #endregion

                if (shipPlan.OrderType == CodeMaster.OrderType.Procurement ||
                    shipPlan.OrderType == CodeMaster.OrderType.CustomerGoods ||
                    shipPlan.OrderType == CodeMaster.OrderType.ScheduleLine)
                {
                    //注意:溢出的库存只留在当前节点,需求不往下传递
                    //细运算暂不考虑采购BOM
                    if (shipPlan.SourceType != CodeMaster.MrpSourceType.StockOver && false)
                    {
                        if (!string.IsNullOrWhiteSpace(flowDetail.Bom))
                        {
                            #region 采购Bom，需要分解Bom
                            var mrpBoms = GetMrpBomList(flowDetail.Item, flowDetail.Bom, shipPlan.StartTime, mrpInContainer.BusinessException);
                            foreach (var mrpBom in mrpBoms)
                            {
                                #region 创建ReceivePlan
                                MrpReceivePlan receivePlanbom = new MrpReceivePlan();
                                receivePlanbom.Item = mrpBom.Item;
                                receivePlanbom.ReceiveTime = shipPlan.StartTime;
                                receivePlanbom.SourceId = shipPlan.SourceId;
                                receivePlanbom.SourceType = shipPlan.SourceType;
                                // 取库位
                                receivePlanbom.LocationFrom = shipPlan.LocationFrom;  //默认库位
                                receivePlanbom.ExtraLocationFrom = flowDetail.ExtraLocationFrom;  //默认库位
                                if (mrpBom.Location != null)
                                {
                                    receivePlanbom.LocationFrom = mrpBom.Location;
                                }
                                receivePlanbom.Qty = shipPlan.Qty * mrpBom.RateQty;
                                receivePlanbom.StartTime = shipPlan.StartTime;
                                receivePlanbom.Flow = shipPlan.Flow;
                                receivePlanbom.OrderType = shipPlan.OrderType;
                                receivePlanbom.Bom = flowDetail.Bom;

                                receivePlanbom.SourceParty = flowDetail.PartyTo;
                                receivePlan.SourceFlow = flowDetail.Flow;
                                receivePlan.ParentItem = flowDetail.Item;

                                receivePlanList.Add(receivePlanbom);
                                #endregion
                            }
                            #endregion
                        }
                    }
                }
            }
            #endregion

            #region 计算下游发运计划
            foreach (MrpReceivePlan receivePlan in receivePlanList)
            {
                //logRunMrp.Debug(GetTLog(shipPlan, "Transfer ship plan flow"));
                CalNextShipPlan(mrpInContainer, mrpShipPlanBag, receivePlan, iterateCount);
            }
            #endregion
        }

        private void CalNextShipPlan(MrpInContainer mrpInContainer, List<MrpShipPlan> mrpShipPlanBag, MrpReceivePlan receivePlan, int iterateCount)
        {
            if (receivePlan.OrderType == CodeMaster.OrderType.Procurement || receivePlan.OrderType == CodeMaster.OrderType.CustomerGoods)
            {
                //采购,客供MRP运行结束
                return;
            }
            var flowDetailList = mrpInContainer.MrpFlowDetailDic.ValueOrDefault(receivePlan.Item) ?? new List<MrpFlowDetail>();

            var nextFlowDetailList = flowDetailList.Where(f => f.LocationTo == receivePlan.LocationFrom
                     && f.StartDate <= receivePlan.ReceiveTime && f.EndDate >= receivePlan.ReceiveTime && f.MrpWeight > 0)
                     .ToList();

            #region 如果没有找到，考虑其他来源库位
            if (nextFlowDetailList.Count() == 0 && !string.IsNullOrWhiteSpace(receivePlan.ExtraLocationFrom))
            {
                var locations = receivePlan.ExtraLocationFrom.Split('|').Distinct();
                foreach (var location in locations)
                {
                    nextFlowDetailList = flowDetailList.Where(f => f.LocationTo == location
                       && f.StartDate <= receivePlan.ReceiveTime && f.EndDate >= receivePlan.ReceiveTime && f.MrpWeight > 0)
                       .ToList();
                    if (nextFlowDetailList.Count() > 0)
                    {
                        break;
                    }
                }
            }
            #endregion

            #region 如果没有找到，考虑其他目的库位
            if (nextFlowDetailList.Count() == 0)
            {
                var locations = flowDetailList.Where(p => !string.IsNullOrWhiteSpace(p.ExtraLocationTo))
                    .SelectMany(p => p.ExtraLocationTo.Split('|')).Distinct();
                foreach (var location in locations)
                {
                    nextFlowDetailList = flowDetailList.Where(f => location == receivePlan.LocationFrom
                        && f.StartDate <= receivePlan.ReceiveTime && f.EndDate >= receivePlan.ReceiveTime && f.MrpWeight > 0)
                        .ToList();
                    if (nextFlowDetailList.Count() > 0)
                    {
                        break;
                    }
                }
            }
            #endregion

            #region 暂不考虑其他来源库位和其他目的库位之间的需求关联

            #endregion

            nextFlowDetailList = nextFlowDetailList.GroupBy(p => new { p.LocationFrom, p.LocationTo })
                .Select(p => p.First()).ToList();

            if (nextFlowDetailList.Count() > 0)
            {
                double mrpTotal = nextFlowDetailList.Sum(p => p.MrpWeight);
                var count = nextFlowDetailList.Count();
                if (count > 1)
                {
                    //test
                }

                for (int i = 0; i < nextFlowDetailList.Count(); i++)
                {
                    var flowDetail = nextFlowDetailList.ElementAt(i);

                    MrpShipPlan shipPlan = new MrpShipPlan();

                    shipPlan.Flow = flowDetail.Flow;
                    shipPlan.OrderType = flowDetail.Type;
                    shipPlan.Item = flowDetail.Item;
                    //if (receivePlan.SourceType == CodeMaster.MrpSourceType.StockLack
                    //    || receivePlan.SourceType == CodeMaster.MrpSourceType.StockOver)
                    //{
                    //    shipPlan.StartTime = receivePlan.ReceiveTime;
                    //}
                    //else
                    //{
                    //    shipPlan.StartTime = receivePlan.ReceiveTime.AddHours(-flowDetail.LeadTime);
                    //}

                    //只是采购类型的才考虑提前期,其他的人工考虑
                    if (receivePlan.OrderType == CodeMaster.OrderType.Procurement ||
                        receivePlan.OrderType == CodeMaster.OrderType.SubContract ||
                        receivePlan.OrderType == CodeMaster.OrderType.CustomerGoods ||
                        receivePlan.OrderType == CodeMaster.OrderType.ScheduleLine)
                    {
                        shipPlan.StartTime = receivePlan.ReceiveTime.AddHours(-flowDetail.LeadTime);
                    }
                    else
                    {
                        shipPlan.StartTime = receivePlan.ReceiveTime;
                    }

                    shipPlan.WindowTime = receivePlan.ReceiveTime;
                    shipPlan.LocationFrom = flowDetail.LocationFrom;
                    shipPlan.LocationTo = flowDetail.LocationTo;
                    shipPlan.SourceType = receivePlan.SourceType;
                    //shipPlan.SourceDateType = receivePlan.SourceDateType;
                    shipPlan.SourceId = receivePlan.SourceId;
                    //shipPlan.SnapTime = snapTime;

                    shipPlan.Qty = receivePlan.Qty * flowDetail.MrpWeight / mrpTotal;
                    shipPlan.Bom = receivePlan.Bom;
                    shipPlan.SourceParty = receivePlan.SourceParty;
                    shipPlan.SourceFlow = receivePlan.SourceFlow;
                    shipPlan.ParentItem = receivePlan.ParentItem;

                    //this.genericMgr.Create(shipPlan);
                    //logRunMrp.Debug(GetTLog(shipPlan, "Transfer receive plan to ship plan"));
                    shipPlan.MrpFlowDetail = flowDetail;
                    mrpShipPlanBag.Add(shipPlan);

                    CalNextReceivePlan(mrpInContainer, mrpShipPlanBag, shipPlan, iterateCount);
                }
            }
            else
            {
                if (receivePlan.OrderType != CodeMaster.OrderType.Procurement &&
                    receivePlan.OrderType != CodeMaster.OrderType.CustomerGoods)
                {
                    MrpShipPlan shipPlan = new MrpShipPlan();

                    shipPlan.Flow = null;
                    shipPlan.OrderType = CodeMaster.OrderType.Procurement;
                    shipPlan.Item = receivePlan.Item;
                    shipPlan.StartTime = receivePlan.ReceiveTime;
                    shipPlan.WindowTime = receivePlan.ReceiveTime;
                    shipPlan.LocationFrom = receivePlan.LocationFrom;
                    //shipPlan.LocationTo = flowDetail.LocationTo;
                    shipPlan.SourceType = receivePlan.SourceType;
                    //shipPlan.SourceDateType = receivePlan.SourceDateType;
                    shipPlan.SourceId = receivePlan.SourceId;
                    //shipPlan.SnapTime = snapTime;

                    shipPlan.Qty = receivePlan.Qty;
                    shipPlan.Bom = null;
                    shipPlan.SourceParty = receivePlan.SourceParty;
                    shipPlan.SourceFlow = receivePlan.SourceFlow;
                    shipPlan.ParentItem = receivePlan.ParentItem;

                    mrpInContainer.BusinessException.AddMessage
                        (new Message(CodeMaster.MessageType.Warning, string.Format("没有找到物料{0}在库位{1}的后继路线,需求类型{2}",
                            receivePlan.Item, receivePlan.LocationFrom, receivePlan.SourceType.ToString())));
                    mrpShipPlanBag.Add(shipPlan);
                }
                else
                {
                    //nothing to do 
                }
            }
        }
        #endregion

        #region ScheduleFi
        private void ScheduleFi(IList<MrpShipPlan> shipPlanList, IList<MrpFlowDetail> mrpFlowDetailList, string dateIndex,
            IList<InventoryBalance> inventoryBalances, DateTime snapTime, DateTime newPlanVersion, BusinessException businessException)
        {
            #region 获得MrpProdFiPlan
            logRunMrp.Info(string.Format("-{0} - 准备后加工排产数据 -", newPlanVersion));
            var mrpFiPlanList = GetMrpFiPlanList(shipPlanList, mrpFlowDetailList, dateIndex, inventoryBalances, newPlanVersion, businessException);
            logRunMrp.Info(string.Format("-{0} - 准备后加工排产数据结束 -", newPlanVersion));
            #endregion

            #region group by Island
            var groupFiPlanListByIsland = (from p in mrpFiPlanList
                                           group p by new
                                           {
                                               Flow = p.ProductLine,
                                               Island = p.Island,
                                           } into g
                                           select new
                                           {
                                               Flow = g.Key.Flow,
                                               Island = g.Key.Island,
                                               List = g.ToList()
                                           }).ToList();
            #endregion

            #region 排产
            logRunMrp.Info(string.Format("-{0} - 开始按岛区排产 -", newPlanVersion));
            var shiftPlanList = new List<MrpFiShiftPlan>();
            var shiftDetailList = this.genericMgr.FindAll<ShiftDetail>("from ShiftDetail where Shift like ? ", "FI" + "%");
            //Parallel.ForEach(groupMrpProdFiPlanListByIsland, groupByIsland =>
            foreach (var groupByIsland in groupFiPlanListByIsland)
            {
                if (groupByIsland.Island == "650005")
                {
                    //debug
                }

                #region 按 PlanDate 汇总
                var groupFiPlanListByPlanDate = (from p in groupByIsland.List
                                                 group p by new
                                                 {
                                                     PlanDate = p.PlanDate,
                                                 } into g
                                                 select new
                                                 {
                                                     PlanDate = g.Key.PlanDate,
                                                     List = g.ToList()
                                                 }).ToList();
                #endregion 按 PlanDate 汇总

                var fiPlanListByItemDic = groupByIsland.List.GroupBy(p => p.Item, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g);

                //缓存当前正在生产的模具
                Dictionary<string, bool> currentMachineDic = groupByIsland.List.GroupBy(p => p.Machine)
                    .ToDictionary(d => d.Key, d => false);
                //一天一天的排
                foreach (var groupByPlanDate in groupFiPlanListByPlanDate)
                {
                    //推算当前库存
                    foreach (var plan in groupByPlanDate.List)
                    {
                        var mrpFiPlansItem = fiPlanListByItemDic[plan.Item];
                        var adjQty = mrpFiPlansItem.Where(p => p.PlanDate < groupByPlanDate.PlanDate).Sum(p => p.AdjustQty);
                        plan.CurrentInvQty = plan.InvQty + adjQty - plan.Qty;
                        if (plan.CurrentInvQty < 0)
                        {
                            plan.Sequence = -1;
                        }
                        else if (plan.CurrentInvQty <= plan.SafeStock)
                        {
                            plan.Sequence = 0;
                        }
                        else if (plan.CurrentInvQty <= plan.MaxStock)
                        {
                            plan.Sequence = 1;
                        }
                        else
                        {
                            plan.Sequence = 2;
                        }
                    }

                    #region group by Machine
                    //混岛生产,模具少/库存少的先做 
                    var groupFiPlanListByMachinePlanDate = (from p in groupByPlanDate.List
                                                            group p by new
                                                            {
                                                                PlanDate = p.PlanDate,
                                                                Machine = p.Machine,
                                                                MachineQty = p.MachineQty,
                                                                Island = p.Island,
                                                                MaxQty = p.MaxQtyPerDay,
                                                                MachineType = p.MachineType,
                                                                ShiftQuota = p.ShiftQuota,
                                                                ShiftType = p.ShiftType,
                                                                WorkDayPerWeek = p.WorkDayPerWeek,
                                                                ShiftPerDay = p.ShiftPerDay,
                                                            } into g
                                                            select new
                                                            {
                                                                PlanDate = g.Key.PlanDate,
                                                                Machine = g.Key.Machine,
                                                                MachineQty = g.Key.MachineQty,
                                                                Island = g.Key.Island,
                                                                MachineType = g.Key.MachineType,
                                                                ShiftQuota = g.Key.ShiftQuota,
                                                                ShiftType = g.Key.ShiftType,
                                                                WorkDayPerWeek = g.Key.WorkDayPerWeek,
                                                                ShiftPerDay = g.Key.ShiftPerDay,
                                                                TotalQty = g.Sum(s => s.TotalQty),
                                                                //MinInvQty = g.Min(m => m.InvQty),
                                                                List = g.OrderBy(q => q.PlanDate).ToList()
                                                            })//.OrderBy(n => n.MachineQty).ThenBy(n => n.MinInvQty)
                                                            .ToList();

                    groupFiPlanListByMachinePlanDate = groupFiPlanListByMachinePlanDate
                        .OrderBy(p => p.List.Min(q => q.Sequence)).ThenBy(p => p.MachineQty).ToList();
                    #endregion

                    #region 排班
                    //启动条件:模具下任一物料库存小于最低
                    //终止条件:模具下所有物料库存都大于最高
                    int monitorInv = MonitorInv(groupByPlanDate.List, groupFiPlanListByMachinePlanDate.First().List.First(), businessException);
                    foreach (var groupPlan in groupFiPlanListByMachinePlanDate)
                    {
                        var groupPlanList = groupPlan.List.OrderBy(p => p.Sequence).ThenBy(p => p.CurrentInvQty).ToList();
                        if (groupPlan.Machine == "600128")
                        {
                            //debug
                        }

                        int dayOfWeek = (int)groupPlan.PlanDate.DayOfWeek;
                        dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
                        if (dayOfWeek <= groupPlan.WorkDayPerWeek || monitorInv < 0)
                        {
                            if (groupPlan.MachineType == CodeMaster.MachineType.Kit)
                            {
                                #region Kit Max
                                var shifts = shiftDetailList
                                    .Where(p => p.Shift.Substring(2, 1) == ((int)groupPlan.ShiftType).ToString())
                                    .Take(groupPlan.ShiftPerDay);

                                var plan = groupPlanList.First();
                                double machineQty = GetMachineQty(groupByPlanDate.List, plan);
                                if (machineQty > 0)
                                {
                                    monitorInv = MonitorInv(groupByPlanDate.List, plan, businessException);
                                    //实施监控库存
                                    if ((monitorInv == 0 && currentMachineDic[groupPlan.Machine]) || monitorInv == -1)
                                    {
                                        //成套生产
                                        foreach (var kitPlan in groupPlanList)
                                        {
                                            foreach (var shift in shifts)
                                            {
                                                shiftPlanList.Add(GetFiShiftPlan(kitPlan, shift, kitPlan.ShiftQuota * machineQty));
                                            }
                                            kitPlan.AdjustQty = kitPlan.ShiftQuota * machineQty * kitPlan.ShiftPerDay - kitPlan.Qty;
                                            kitPlan.OccupyIslandQty = machineQty;
                                        }
                                        currentMachineDic[groupPlan.Machine] = true;
                                    }
                                    else
                                    {
                                        foreach (var kitPlan in groupPlanList)
                                        {
                                            kitPlan.AdjustQty = -kitPlan.Qty;
                                        }
                                        currentMachineDic[groupPlan.Machine] = false;
                                    }
                                }
                                else
                                {
                                    businessException.AddMessage(new Message(CodeMaster.MessageType.Info, "此模具{0}或岛区在日期{1}产能不足",
                                        groupPlan.Machine + "/" + groupPlanList[0].MachineDescription,
                                        groupPlan.PlanDate.ToString("yyyy-MM-dd")));
                                    foreach (var kitPlan in groupPlan.List)
                                    {
                                        kitPlan.AdjustQty = -kitPlan.Qty;
                                    }
                                    currentMachineDic[groupPlan.Machine] = false;
                                }
                                #endregion
                            }
                            else
                            {
                                #region Single ∑ 满负荷差:按单包装拆分/ 按物料周总量从大到小排列,按单包装分摊到各个物料上
                                double machineQty = GetMachineQty(groupByPlanDate.List, groupPlanList[0]);
                                if (machineQty > 0)
                                {
                                    //实施监控库存,换模
                                    //换模条件:
                                    //1.非当前模具小于最低库存
                                    //2.当前模具大于最高库存
                                    //3.非当前模具,在最低域最高之间
                                    monitorInv = MonitorInv(groupByPlanDate.List, groupPlanList[0], businessException);
                                    if (monitorInv == -2 || monitorInv == 1
                                        || (!currentMachineDic[groupPlan.Machine] && monitorInv == 0))
                                    {
                                        //需要换模
                                        foreach (var plan in groupPlanList)
                                        {
                                            plan.AdjustQty = -plan.Qty;
                                        }
                                        currentMachineDic[groupPlan.Machine] = false;
                                    }
                                    else
                                    {
                                        foreach (var plan in groupPlanList)
                                        {
                                            plan.OccupyIslandQty = machineQty;

                                            #region // 获取当前期末库存
                                            var mrpFiPlansItem = fiPlanListByItemDic[plan.Item];//groupByMachine.List.Where(p => p.Item == plan.Item);

                                            //当前库存 = 期初 + 调整 - 当天发
                                            var adjQty = mrpFiPlansItem.Where(p => p.PlanDate <= plan.PlanDate).Sum(p => p.AdjustQty);

                                            //按单包装圆整 125+125-240=10
                                            double currentQty = plan.TotalQty - adjQty;
                                            currentQty = Math.Ceiling(Math.Round(currentQty / plan.UnitCount, 2)) * plan.UnitCount;
                                            plan.AdjustQty = currentQty - plan.Qty;
                                            if (plan.TotalQty < 0)
                                            {
                                                plan.AdjustQty = -plan.Qty;
                                            }
                                            #endregion
                                        }

                                        //按模具汇总的差异数 班产定额*班产数每天*可用设备数量-需求数 1800-1320=480
                                        double diffQty = groupPlan.ShiftQuota * groupPlan.ShiftPerDay * machineQty
                                            - groupPlanList.Sum(p => p.TotalQty);

                                        if (diffQty > 0)
                                        {
                                            int k = 0;
                                            double accQty = 0;
                                            //调整满班生产
                                            while (true)
                                            {
                                                var plan = groupPlanList[k % groupPlanList.Count()];
                                                //正调整:取小
                                                if (diffQty - accQty > plan.UnitCount)
                                                {
                                                    plan.AdjustQty += plan.UnitCount;
                                                    accQty += plan.UnitCount;
                                                    plan.Logs += string.Format("Shit+{0};", plan.UnitCount);
                                                }
                                                else
                                                {
                                                    plan.AdjustQty += (diffQty - accQty);
                                                    plan.Logs += string.Format("Shit+{0};", (diffQty - accQty));
                                                    break;
                                                }
                                                k++;
                                            }
                                        }
                                        else if (diffQty < 0)
                                        {
                                            //按库存排序:小于最小库存者优先,与最大库存之间的差额,越大者优先
                                            groupPlanList = groupPlanList.OrderByDescending(p => p.Sequence)
                                                .ThenByDescending(p => p.CurrentInvQty).ToList();
                                            //先消耗大于0的
                                            var _groupPlanList = groupPlanList.Where(p => p.TotalQty > 0).ToList();
                                            int k = 0;
                                            double accQty = 0;
                                            if (_groupPlanList.Count > 0)
                                            {
                                                while (true)
                                                {
                                                    var plan = _groupPlanList[k % _groupPlanList.Count];
                                                    if (_groupPlanList.All(p => p.TotalQty <= 0))
                                                    {
                                                        break;
                                                    }
                                                    if (plan.TotalQty > 0)
                                                    {
                                                        if (diffQty - accQty < -plan.UnitCount)
                                                        {
                                                            plan.AdjustQty -= plan.UnitCount;
                                                            accQty -= plan.UnitCount;
                                                            plan.Logs += string.Format("Shit-{0};", -plan.UnitCount);
                                                        }
                                                        else
                                                        {
                                                            plan.AdjustQty += (diffQty - accQty);
                                                            accQty = diffQty;
                                                            plan.Logs += string.Format("Shit-{0};", -(diffQty - accQty));
                                                            break;
                                                        }
                                                    }
                                                    k++;
                                                }
                                            }
                                            //有剩余再消耗
                                            k = 0;
                                            if (diffQty < accQty)
                                            {
                                                while (true)
                                                {
                                                    var plan = groupPlanList[k % groupPlanList.Count()];
                                                    if (diffQty - accQty < -plan.UnitCount)
                                                    {
                                                        plan.AdjustQty -= plan.UnitCount;
                                                        accQty -= plan.UnitCount;
                                                        plan.Logs += string.Format("Shit-{0};", -plan.UnitCount);
                                                    }
                                                    else
                                                    {
                                                        plan.AdjustQty += (diffQty - accQty);
                                                        plan.Logs += string.Format("Shit-{0};", -(diffQty - accQty));
                                                        break;
                                                    }
                                                    k++;
                                                }
                                            }
                                        }
                                        var seq = 10;
                                        foreach (var plan in groupPlanList)
                                        {
                                            plan.Sequence = seq;
                                            seq += 10;
                                        }
                                        currentMachineDic[groupPlan.Machine] = true;
                                    }
                                }
                                else
                                {
                                    currentMachineDic[groupPlan.Machine] = false;
                                    businessException.AddMessage(new Message(CodeMaster.MessageType.Warning, "模具{0}或岛区在日期{1}已被占用",
                                        groupPlan.Machine + "/" + groupPlan.List[0].MachineDescription,
                                        groupPlan.PlanDate.ToString("yyyy-MM-dd")));
                                    foreach (MrpFiPlan plan in groupPlanList)
                                    {
                                        plan.AdjustQty = -plan.Qty;
                                    }
                                }
                                // 班产计划
                                shiftPlanList.AddRange(GetFiShiftPlanList(shiftDetailList, groupPlan.List, snapTime));
                                #endregion Single 满负荷差:按单包装拆分/ 按物料周总量从大到小排列,按单包装分摊到各个物料上
                            }
                        }
                        else
                        {
                            //周末不生产
                            foreach (MrpFiPlan plan in groupPlanList)
                            {
                                plan.AdjustQty = -plan.Qty;
                                plan.Logs += "暂停;";
                            }
                            currentMachineDic[groupPlan.Machine] = false;
                        }
                    }
                    #endregion 排班
                }
                logRunMrp.Info(string.Format("-{0} - 完成岛区:{1}[{2}] 的排产 -", newPlanVersion, groupByIsland.Island, groupByIsland.List.First().IslandDescription));
            }
            //);
            logRunMrp.Info(string.Format("-{0} - 完成所有岛区排产 -", newPlanVersion));
            #endregion

            #region Create
            logRunMrp.Info(string.Format("-{0} - 开始保存FiPlan -", newPlanVersion));
            foreach (var mrpFiPlan in mrpFiPlanList)
            {
                if (mrpFiPlan.Logs != null)
                {
                    int length = mrpFiPlan.Logs.Length > 2000 ? 2000 : mrpFiPlan.Logs.Length;
                    mrpFiPlan.Logs = mrpFiPlan.Logs.Substring(0, length);
                }
            }
            this.genericMgr.BulkInsert<MrpFiPlan>(mrpFiPlanList);
            this.SequenceFiShiftPlan(shiftPlanList, mrpFlowDetailList);
            this.genericMgr.BulkInsert<MrpFiShiftPlan>(shiftPlanList);
            var machinePlanList = shiftPlanList.GroupBy(p => new
                {
                    p.PlanVersion,
                    p.PlanDate,
                    p.ProductLine,
                    p.Machine,
                }).Select(p =>
                {
                    var fiMachinePlan = new MrpFiMachinePlan();
                    var firstFiPlan = p.First();
                    fiMachinePlan.PlanVersion = p.Key.PlanVersion;
                    fiMachinePlan.PlanDate = p.Key.PlanDate;
                    fiMachinePlan.ProductLine = p.Key.ProductLine;
                    fiMachinePlan.Island = firstFiPlan.Island;
                    fiMachinePlan.IslandDescription = firstFiPlan.IslandDescription;
                    fiMachinePlan.Machine = p.Key.Machine;
                    fiMachinePlan.MachineDescription = firstFiPlan.MachineDescription;
                    fiMachinePlan.MachineQty = firstFiPlan.MachineQty;
                    fiMachinePlan.MachineType = firstFiPlan.MachineType;
                    fiMachinePlan.ShiftQuota = firstFiPlan.ShiftQuota;
                    fiMachinePlan.ShiftType = firstFiPlan.ShiftType;
                    fiMachinePlan.WorkDayPerWeek = firstFiPlan.WorkDayPerWeek;
                    fiMachinePlan.ShiftPerDay = firstFiPlan.ShiftPerDay;
                    fiMachinePlan.UnitCount = firstFiPlan.UnitCount;
                    if (firstFiPlan.ShiftQuota > 0)
                    {
                        if (p.Sum(q => q.Qty) > 0)
                        {
                            if (fiMachinePlan.MachineType == CodeMaster.MachineType.Kit)
                            {
                                fiMachinePlan.ShiftQty = p.GroupBy(q => q.Shift).Select(q => q.First()).Sum(q => q.Qty) / firstFiPlan.ShiftQuota;
                                fiMachinePlan.ShiftSplit = string.Join("+", p.OrderBy(q => q.Shift).GroupBy(q => q.Shift).Select(q => Math.Round(q.First().Qty / firstFiPlan.ShiftQuota, 1)));
                            }
                            else
                            {
                                fiMachinePlan.ShiftQty = p.Sum(q => q.Qty) / firstFiPlan.ShiftQuota;
                                fiMachinePlan.ShiftSplit = string.Join("+", p.OrderBy(q => q.Shift).GroupBy(q => q.Shift).Select(q => Math.Round(q.Sum(r => r.Qty) / firstFiPlan.ShiftQuota, 1)));
                            }
                        }
                        else
                        {
                            fiMachinePlan.ShiftSplit = "0";
                        }
                    }
                    return fiMachinePlan;
                }).ToList();
            this.genericMgr.BulkInsert<MrpFiMachinePlan>(machinePlanList);
            logRunMrp.Info(string.Format("-{0} - 结束保存FiPlan -", newPlanVersion));
            #endregion

            #region 使用存储过程进行算法修正
            this.genericMgr.FlushSession();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@SnapTime", snapTime);
            sqlParameters[1] = new SqlParameter("@PlanVersion", newPlanVersion);
            this.genericMgr.ExecuteStoredProcedure("USP_Busi_MRP_ScheduleFi", sqlParameters);
            #endregion
        }

        private List<MrpFiPlan> GetMrpFiPlanList(IList<MrpShipPlan> shipPlanList, IList<MrpFlowDetail> mrpFlowDetailList, string dateIndex,
            IList<InventoryBalance> inventoryBalances, DateTime newPlanVersion, BusinessException businessException)
        {
            var fiFlowDetailDic = mrpFlowDetailList.Where(f => f.ResourceGroup == CodeMaster.ResourceGroup.FI && f.MrpWeight > 0)
                .GroupBy(p => p.Flow, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g);
            var stockDic = inventoryBalances.GroupBy(p => p.Item)
                .ToDictionary(d => d.Key, d => new { Qty = d.Sum(p => p.Qty), SafeStock = d.Sum(p => p.SafeStock), MaxStock = d.Sum(p => p.MaxStock) });

            var dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
            var currentDateIndex = Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);

            var fiPlanVersions = this.genericMgr.FindAll<MrpPlanMaster>
                (" from MrpPlanMaster where ResourceGroup = ? and IsRelease=? and DateIndex>=? and DateIndex<? order by PlanVersion desc",
                new object[] { CodeMaster.ResourceGroup.FI, true, currentDateIndex, dateIndex })
                .GroupBy(p => p.DateIndex).Select(p => p.First()).ToList();

            //待收
            List<MrpFiShiftPlan> planInList = new List<MrpFiShiftPlan>();
            foreach (var fiPlanVersion in fiPlanVersions)
            {
                if (!string.IsNullOrWhiteSpace(fiPlanVersion.DateIndex))
                {
                    var _dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(fiPlanVersion.DateIndex).Date;
                    _dateFrom = _dateFrom > DateTime.Now.Date ? _dateFrom : DateTime.Now.Date;//取大
                    var _dateTo = _dateFrom.AddDays(7);
                    _dateTo = _dateTo < dateFrom ? _dateTo : dateFrom;//取小

                    var _planInList = this.genericMgr.FindAll<MrpFiShiftPlan>
                         (" from MrpFiShiftPlan where PlanDate>=? and PlanDate<? and PlanVersion =?",
                          new object[] { _dateFrom, _dateTo, fiPlanVersion.PlanVersion });
                    planInList.AddRange(_planInList);
                }
            }
            var planInDic = planInList.GroupBy(p => p.Item).ToDictionary(d => d.Key, d => d.Sum(q => q.Qty));

            //待发
            var planOutDic = this.genericMgr.FindAll<MrpPlan>
                (" from MrpPlan where PlanDate>=? and PlanDate<? ", new object[] { DateTime.Now.Date, dateFrom })
                .GroupBy(p => p.Item).ToDictionary(d => d.Key, d => d.Sum(q => q.LeftQty));

            #region 对象转换 按物料,天汇总数量
            logRunMrp.Info(string.Format("-{0} - 后加工排产数据准备 - 对象转换 按物料,天汇总数量", newPlanVersion));
            var shipPlanGroups = from p in shipPlanList
                                 orderby p.Flow, p.StartTime
                                 where !string.IsNullOrWhiteSpace(p.Flow) && fiFlowDetailDic.ContainsKey(p.Flow)
                                 && !fiFlowDetailDic.ContainsKey(p.SourceFlow) && p.StartTime.Date >= dateFrom
                                 group p by new
                                 {
                                     Flow = p.Flow
                                 } into g
                                 select new
                                 {
                                     Flow = g.Key.Flow,
                                     List = from q in g
                                            group q by new
                                            {
                                                Item = q.Item,
                                                PlanDate = q.StartTime.Date,
                                            } into g1
                                            select new
                                            {
                                                Item = g1.Key.Item,
                                                PlanDate = g1.Key.PlanDate,
                                                Qty = g1.Sum(s => s.Qty),
                                            }
                                 };
            #endregion

            #region  查询 MachineInstance
            logRunMrp.Info(string.Format("-{0} - 后加工排产数据准备 - 查询MachineInstance", newPlanVersion));
            var machineInstanceDic = (this.genericMgr.FindAll<MachineInstance>
                    ("from MachineInstance where DateType = ? and DateIndex between ? and ? ",
                    new object[] { CodeMaster.TimeUnit.Day,
                    DateTime.Now.Date.ToString("yyyy-MM-dd"),
                    shipPlanGroups.Max(p => p.List.Max(q => q.PlanDate)).ToString("yyyy-MM-dd") }))
                    .GroupBy(p => p.DateIndex, (k, g) => new { k, g })
                    .ToDictionary(d => d.k, d => d.g.GroupBy(p => p.Code, (k, g) => new { k, g }).ToDictionary(d1 => d1.k, d1 => d1.g.First()));
            #endregion

            #region  MrpProdFiPlan
            logRunMrp.Info(string.Format("-{0} - 后加工排产数据准备 - 补齐 MrpProdFiPlan", newPlanVersion));
            List<MrpFiPlan> mrpFiPlanList = new List<MrpFiPlan>();
            var noMachineFlowDetailList = new List<MrpFlowDetail>();
            foreach (var shipPlanGroup in shipPlanGroups)
            {
                var flowDetails = fiFlowDetailDic[shipPlanGroup.Flow];
                var shipPlanDic = shipPlanGroup.List.ToDictionary(d => new { d.PlanDate, d.Item }, d => d.Qty);
                var maxPlanDate = shipPlanGroup.List.Max(p => p.PlanDate);
                maxPlanDate = maxPlanDate < DateTime.Today.AddDays(15) ? DateTime.Today.AddDays(15) : maxPlanDate;
                maxPlanDate = maxPlanDate > DateTime.Today.AddDays(30) ? DateTime.Today.AddDays(30) : maxPlanDate;
                for (var planDate = dateFrom; planDate < maxPlanDate; planDate = planDate.AddDays(1))
                {
                    var machineInstances = machineInstanceDic.ValueOrDefault(planDate.ToString("yyyy-MM-dd"));
                    if (machineInstances != null)
                    {
                        foreach (var flowDetail in flowDetails)
                        {
                            if (flowDetail.StartDate <= planDate && flowDetail.EndDate > planDate)
                            {
                                if (!string.IsNullOrWhiteSpace(flowDetail.Machine))
                                {
                                    var machineInstance = machineInstances.ValueOrDefault(flowDetail.Machine);
                                    if (machineInstance != null)
                                    {
                                        MrpFiPlan mrpFiPlan = new MrpFiPlan();
                                        mrpFiPlan.ProductLine = flowDetail.Flow;
                                        mrpFiPlan.Item = flowDetail.Item;
                                        mrpFiPlan.LocationTo = flowDetail.LocationTo;
                                        mrpFiPlan.LocationFrom = flowDetail.LocationFrom;
                                        mrpFiPlan.Bom = flowDetail.Bom;
                                        mrpFiPlan.PlanDate = planDate;
                                        mrpFiPlan.Machine = machineInstance.Code;
                                        mrpFiPlan.MachineDescription = machineInstance.Description;
                                        mrpFiPlan.Island = machineInstance.Island;
                                        mrpFiPlan.IslandDescription = machineInstance.IslandDescription;
                                        mrpFiPlan.MachineType = machineInstance.MachineType;
                                        //DateIndex = machineInstance.DateIndex;
                                        mrpFiPlan.ShiftQuota = machineInstance.ShiftQuota;
                                        mrpFiPlan.ShiftType = machineInstance.ShiftType;
                                        mrpFiPlan.WorkDayPerWeek = machineInstance.NormalWorkDayPerWeek;
                                        mrpFiPlan.ShiftPerDay = machineInstance.ShiftPerDay;
                                        mrpFiPlan.UnitCount = flowDetail.UnitCount;
                                        mrpFiPlan.MachineQty = machineInstance.Qty;
                                        mrpFiPlan.IslandQty = machineInstance.IslandQty;
                                        mrpFiPlan.IsRelease = machineInstance.IsRelease;
                                        mrpFiPlan.PlanVersion = newPlanVersion;
                                        mrpFiPlan.Qty = shipPlanDic.ValueOrDefault(new { PlanDate = planDate, Item = flowDetail.Item });

                                        mrpFiPlan.SafeStock = flowDetail.SafeStock;
                                        mrpFiPlan.MaxStock = flowDetail.MaxStock;
                                        var stock = stockDic.ValueOrDefault(mrpFiPlan.Item);
                                        if (stock != null)
                                        {
                                            mrpFiPlan.InvQty = stock.Qty + planInDic.ValueOrDefault(mrpFiPlan.Item) - planOutDic.ValueOrDefault(mrpFiPlan.Item);
                                            mrpFiPlan.SafeStock = stock.SafeStock;
                                            mrpFiPlan.MaxStock = stock.MaxStock;
                                        }
                                        mrpFiPlanList.Add(mrpFiPlan);
                                    }
                                    else
                                    {
                                        businessException.AddMessage(new Message(CodeMaster.MessageType.Warning, "没有找到物料{0}的模具{1}",
                                            flowDetail.Item, flowDetail.Machine));
                                    }
                                }
                                else
                                {
                                    noMachineFlowDetailList.Add(flowDetail);
                                }
                            }
                        }
                    }
                    else
                    {
                        businessException.AddMessage(new Message(CodeMaster.MessageType.Warning, "没有找到此日期{0}的模具", planDate.ToString("yyyy-MM-dd")));
                    }
                }
            }
            #endregion
            foreach (var noMachineFlowDetial in noMachineFlowDetailList.Distinct())
            {
                logRunMrp.Info(string.Format("物料{0}在生产线{1}上没有维护模具", noMachineFlowDetial.Item, noMachineFlowDetial.Flow));
            }
            return mrpFiPlanList;
        }

        private List<MrpFiShiftPlan> GetFiShiftPlanList(IList<ShiftDetail> shiftDetailList, IList<MrpFiPlan> mrpFiPlanList, DateTime snapTime)
        {
            var shiftPlanList = new List<MrpFiShiftPlan>();
            if (mrpFiPlanList != null && mrpFiPlanList.Count() > 0)
            {
                var shiftPerDay = mrpFiPlanList.First().ShiftPerDay;
                var shifts = shiftDetailList
                    .Where(p => p.Shift.Substring(2, 1) == ((int)mrpFiPlanList.First().ShiftType).ToString())
                    .Take(shiftPerDay);
                #region 平均分配 天/模具
                int i = 0;
                int seq = 10;
                foreach (var plan in mrpFiPlanList.OrderBy(p => p.Sequence))
                {
                    var _shitPlanList = new List<MrpFiShiftPlan>();
                    foreach (var shift in shifts)
                    {
                        var shiftPlan = GetFiShiftPlan(plan, shift, 0);
                        shiftPlan.Sequence = seq;
                        seq += 10;
                        _shitPlanList.Add(shiftPlan);
                    }
                    for (double accQty = 0; accQty < plan.TotalQty; accQty += plan.UnitCount)
                    {
                        if (plan.TotalQty - accQty >= plan.UnitCount)
                        {
                            _shitPlanList[i % shiftPerDay].Qty += plan.UnitCount;
                        }
                        else
                        {
                            _shitPlanList[i % shiftPerDay].Qty += (plan.TotalQty - accQty);
                        }
                        i++;
                    }
                    shiftPlanList.AddRange(_shitPlanList);
                }
                #endregion
            }
            return shiftPlanList;
        }

        private MrpFiShiftPlan GetFiShiftPlan(MrpFiPlan mrpProdFiPlan, ShiftDetail shiftDetail, double qty)
        {
            var shiftPlan = new MrpFiShiftPlan();

            shiftPlan.PlanDate = mrpProdFiPlan.PlanDate;
            shiftPlan.Item = mrpProdFiPlan.Item;
            shiftPlan.ProductLine = mrpProdFiPlan.ProductLine;
            //shiftPlan.Location = mrpProdFiPlan.LocationTo;
            shiftPlan.Shift = shiftDetail.Shift;

            shiftPlan.Qty = Math.Round(qty);
            shiftPlan.Machine = mrpProdFiPlan.Machine;
            shiftPlan.MachineDescription = mrpProdFiPlan.MachineDescription;
            shiftPlan.MachineQty = mrpProdFiPlan.MachineQty;
            shiftPlan.MachineType = mrpProdFiPlan.MachineType;
            shiftPlan.Island = mrpProdFiPlan.Island;
            shiftPlan.IslandDescription = mrpProdFiPlan.IslandDescription;
            //shiftPlan.DateIndex = mrpProdFiPlan.DateIndex;
            shiftPlan.ShiftQuota = mrpProdFiPlan.ShiftQuota;
            shiftPlan.ShiftType = mrpProdFiPlan.ShiftType;
            shiftPlan.WorkDayPerWeek = mrpProdFiPlan.WorkDayPerWeek;
            shiftPlan.ShiftPerDay = mrpProdFiPlan.ShiftPerDay;
            //shiftPlan.ItemPriority = mrpProdFiPlan.ItemPriority;
            shiftPlan.UnitCount = mrpProdFiPlan.UnitCount;
            shiftPlan.PlanVersion = mrpProdFiPlan.PlanVersion;

            DateTime startTime = shiftPlan.PlanDate;
            DateTime windowTime = shiftPlan.PlanDate;
            workingCalendarMgr.GetStartTimeAndWindowTime(shiftPlan.Shift, shiftPlan.PlanDate, out startTime, out windowTime);
            shiftPlan.StartTime = startTime;
            shiftPlan.WindowTime = windowTime;
            shiftPlan.LocationFrom = mrpProdFiPlan.LocationFrom;
            shiftPlan.LocationTo = mrpProdFiPlan.LocationTo;
            shiftPlan.Bom = mrpProdFiPlan.Bom;
            return shiftPlan;
        }

        /// <summary>
        /// 监控生产中的物料是否大于最大库存,并同时监控同岛区下的其他物料是否小于最小库存.
        /// 全部高于最大:1,任意低于最小:-1(其中,非当前模具-2),正常:0
        /// </summary>
        private int MonitorInv(IList<MrpFiPlan> mrpProdFiPlanList, MrpFiPlan mrpProdFiPlan, BusinessException businessException)
        {
            //小于最低库存
            var minPlans = mrpProdFiPlanList.Where(p => p.CurrentInvQty <= p.SafeStock);

            if (minPlans.Count() > 0)
            {
                var minPlan = minPlans.FirstOrDefault(p => p.Machine == mrpProdFiPlan.Machine);
                if (minPlan == null)
                {
                    minPlan = minPlans.First();
                    mrpProdFiPlan.Logs += string.Format("模具{0},物料{1}低于最小库存,需要换模", minPlan.Machine, minPlan.Item);
                    businessException.AddMessage(new Message(CodeMaster.MessageType.Info, "模具{0},物料{1}低于最小库存,需要换模", minPlan.Machine, minPlan.Item));
                    return -2;
                }
                else
                {
                    mrpProdFiPlan.Logs += string.Format("模具{0},物料{1}低于最小库存,继续生产", minPlan.Machine, minPlan.Item);
                    businessException.AddMessage(new Message(CodeMaster.MessageType.Info, "模具{0},物料{1}低于最小库存,继续生产", minPlan.Machine, minPlan.Item));
                    return -1;
                }
            }

            //此模具下的最高库存
            if ((mrpProdFiPlan.MachineType == CodeMaster.MachineType.Kit && mrpProdFiPlanList.Where(p => p.Machine == mrpProdFiPlan.Machine).Any(p => p.CurrentInvQty >= p.MaxStock))
                || (mrpProdFiPlan.MachineType == CodeMaster.MachineType.Single && mrpProdFiPlan.CurrentInvQty >= mrpProdFiPlan.MaxStock))
            {
                mrpProdFiPlan.Logs += string.Format("超过最大库存,需要换模");
                businessException.AddMessage(new Message(CodeMaster.MessageType.Info,
                    "此模具{0}/{1}在日期{2}的有物料超过最大库存,需要换模",
                    mrpProdFiPlan.Machine, mrpProdFiPlan.MachineDescription, mrpProdFiPlan.PlanDate.ToString("yyyy-MM-dd")));
                return 1;
            }
            return 0;
        }

        private double GetMachineQty(IList<MrpFiPlan> mrpProdFiPlanList, MrpFiPlan mrpProdFiPlan)
        {
            //kit件占用数
            var occupyIslandQty = (from p in mrpProdFiPlanList
                                   where p.PlanDate == mrpProdFiPlan.PlanDate //&& p.Machine != mrpProdFiPlan.Machine
                                   group p by p.Machine into g
                                   select new
                                   {
                                       Machine = g.Key,
                                       OccupyIslandQty = g.Max(p => p.OccupyIslandQty)
                                   }).Sum(m => m.OccupyIslandQty);
            var availableIslandQty = mrpProdFiPlan.IslandQty - occupyIslandQty;
            availableIslandQty = availableIslandQty < mrpProdFiPlan.MachineQty ? availableIslandQty : mrpProdFiPlan.MachineQty;
            return availableIslandQty;
        }

        private void SequenceFiShiftPlan(IList<MrpFiShiftPlan> mrpFiShiftPlanList, IList<MrpFlowDetail> mrpFlowDetailList)
        {
            var flowDetailDic = mrpFlowDetailList.Where(f => f.ResourceGroup == CodeMaster.ResourceGroup.FI && f.MrpWeight > 0)
                    .GroupBy(p => p.Item).ToDictionary(d => d.Key, d => d.First().Sequence);
            foreach (var mrpFiShiftPlan in mrpFiShiftPlanList)
            {
                mrpFiShiftPlan.Sequence = flowDetailDic.ValueOrDefault(mrpFiShiftPlan.Item);
            }

            var groupPlan = mrpFiShiftPlanList.GroupBy(p => new { p.PlanDate, p.Shift });
            foreach (var plan in groupPlan)
            {
                int seq = 10;
                foreach (var shiftPlan in plan.OrderBy(p => p.Island).ThenBy(p => p.Machine).ThenBy(p => p.Sequence))
                {
                    shiftPlan.Sequence = seq;
                    seq += 10;
                }
            }
        }
        #endregion

        #region ScheduleEx
        private void ScheduleEx(IList<MrpShipPlanGroup> groupShipPlans, IList<MrpFlowDetail> mrpFlowDetailList, string weekIndex,
           IList<InventoryBalance> inventoryBalanceList, DateTime snapTime, DateTime newPlanVersion, BusinessException businessException)
        {
            #region  查询
            mrpFlowDetailList = mrpFlowDetailList.Where(q => q.ResourceGroup == CodeMaster.ResourceGroup.EX && q.MrpWeight > 0).ToList();

            var dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(weekIndex);
            var preWeekIndex = Utility.DateTimeHelper.GetWeekOfYear(dateFrom.AddDays(-7));

            var mrpPlanMaster = this.genericMgr.FindAll<MrpPlanMaster>
                      ("from MrpPlanMaster where IsRelease = ? and ResourceGroup=? and DateIndex=? order by PlanVersion desc",
                      new object[] { true, CodeMaster.ResourceGroup.EX, preWeekIndex }, 0, 1).First();
            #endregion

            #region exPlanList sectionExPlanList
            //按周汇总挤出件的需求 按照配额把挤出件分配到生产线上.考虑常用和备用配额 并转换成断面
            var exPlanList = GetMrpExPlan(mrpPlanMaster, groupShipPlans, mrpFlowDetailList, inventoryBalanceList, newPlanVersion, weekIndex, businessException);
            var sectionExPlanList = GetMrpExSectionPlan(newPlanVersion, exPlanList, businessException);
            #endregion

            #region 汇总断面总的需求
            var groupExPlanList = (from p in sectionExPlanList
                                   group p by new
                                   {
                                       Section = p.Section,
                                       DateIndex = p.DateIndex,
                                       TotalAps = p.TotalAps,
                                       TotalQuota = p.TotalQuota,
                                   } into g1
                                   select new
                                   {
                                       Section = g1.Key.Section,
                                       DateIndex = g1.Key.DateIndex,
                                       TotalAps = g1.Key.TotalAps,
                                       TotalQuota = g1.Key.TotalQuota,
                                       Speed = g1.Sum(q => q.Speed * q.Quota * q.SpeedTimes / q.TotalQuota),//加权
                                       SwitchTime = g1.Sum(q => q.SwitchTime * q.Quota / q.TotalQuota),
                                       ShiftType = g1.First().ShiftType,
                                       MinLotTime = g1.Min(q => q.MinLotSize) * (24 / (int)(g1.First().ShiftType)) * 60,
                                       Qty = g1.Sum(q => q.Qty),
                                       CorrectionQty = g1.Sum(q => q.Qty * (q.Correction - 1)),
                                       //此断面在哪些生产线下生产
                                       MrpExPlanList = g1.OrderBy(p => p.ProductLine)
                                                       .ThenBy(p => p.TotalAps)
                                                       .ThenByDescending(p => (int)p.ApsPriority).ToList()
                                       //总权数小的先排，常用的先排
                                   }).ToList();
            #endregion

            var exPlanTimes = GetMrpPlanTime(snapTime, sectionExPlanList);

            //断面的需求总长度进行分配.
            #region 挤出线周产能平衡
            foreach (var groupPlan in groupExPlanList)
            {
                double currentQty = groupPlan.Qty + groupPlan.CorrectionQty;
                //减去前几周多生产的调整量
                //double preDateIndexAdjustQty = sectionExPlanList.Where(p => string.Compare(groupPlan.DateIndex, p.DateIndex) == 1
                //    && p.Section == groupPlan.Section).Sum(p => p.AdjustQty);
                //currentQty -= preDateIndexAdjustQty;

                //常用/备用生产线
                var normalExPlans = groupPlan.MrpExPlanList.Where(p => p.ApsPriority == CodeMaster.ApsPriorityType.Normal);
                var backupExPlans = groupPlan.MrpExPlanList.Where(p => p.ApsPriority == CodeMaster.ApsPriorityType.Backup);
                if (currentQty > 0)
                {
                    //分配到常用
                    foreach (var plan in normalExPlans)
                    {
                        currentQty = ScheduleMrpExSectionPlan(businessException, exPlanTimes, currentQty, normalExPlans, plan);
                    }
                    //分配到备用
                    if (currentQty > 0)
                    {
                        double backupQuota = backupExPlans.Sum(p => p.Quota);
                        backupQuota = backupQuota == 0 ? 1 : backupQuota;
                        foreach (var plan in backupExPlans)
                        {
                            plan.Qty = (plan.Quota / backupQuota) * currentQty / plan.Correction;
                            plan.CorrectionQty = currentQty - plan.Qty;
                        }
                        foreach (var plan in backupExPlans)
                        {
                            currentQty = ScheduleMrpExSectionPlan(businessException, exPlanTimes, currentQty, backupExPlans, plan);
                        }
                    }

                    //如果常用备用都分配后,还有溢出的,分配到权重最大的常用生产线的上面,需后续处理
                    if (currentQty > 0)
                    {
                        var plan = normalExPlans.OrderByDescending(p => (int)p.ApsPriority).First();
                        var _totalQty = plan.TotalQty + currentQty;
                        //_totalQty = MrpExPlanCeilingLotSize(plan, plan.MinLotSize, _totalQty);
                        plan.AdjustQty = _totalQty - (plan.Qty + plan.CorrectionQty);
                        plan.Logs += " 常备都溢出调整" + (_totalQty - plan.Qty - plan.CorrectionQty);
                    }
                }
                else
                {
                    foreach (var plan in normalExPlans)
                    {
                        plan.AdjustQty = -(plan.Qty + plan.CorrectionQty);
                    }
                    foreach (var plan in backupExPlans)
                    {
                        plan.AdjustQty = -(plan.Qty + plan.CorrectionQty);
                    }
                }
            }
            #endregion 挤出线周产能平衡

            #region 最大批量,经济批量,排序

            //最大批量
            SplitMaxMrpExSectionPlan(sectionExPlanList);

            ProcessMrpExSectionPlan(ref sectionExPlanList, mrpPlanMaster, weekIndex);
            #endregion

            #region Save
            //为了获取sectionExPlan.Id,不用BulkInsert
            this.genericMgr.BatchInsert<MrpExSectionPlan>(sectionExPlanList);
            var mrpExPlanItemList = GetMrpExItemPlanList(sectionExPlanList, exPlanList);
            this.genericMgr.BulkInsert<MrpExItemPlan>(mrpExPlanItemList);
            #endregion

            #region 使用存储过程进行修正
            this.genericMgr.FlushSession();
            SqlParameter[] sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("@DateIndex", weekIndex);
            sqlParameters[1] = new SqlParameter("@SnapTime", snapTime);
            sqlParameters[2] = new SqlParameter("@PlanVersion", newPlanVersion);
            this.genericMgr.ExecuteStoredProcedure("USP_Busi_MRP_ScheduleEx", sqlParameters);
            #endregion
        }

        private double ScheduleMrpExSectionPlan(BusinessException businessException, List<MrpExPlanTime> exPlanTimes,
            double currentQty, IEnumerable<MrpExSectionPlan> mrpExSectionPlans, MrpExSectionPlan plan)
        {
            //生产线总的产能(工时)
            var planTime = exPlanTimes.FirstOrDefault(p => p.ProductLine == plan.ProductLine && p.DateIndex == plan.DateIndex);
            if (planTime != null)
            {
                //给明细赋值,总可用时间(min)
                plan.UpTime = planTime.UpTime;
                if (planTime.UsedTime < planTime.UpTime)
                {
                    //本生产线可用工时还能生产多少米 剩余时间*速度*腔口数
                    double availableQty = (planTime.UpTime - planTime.UsedTime) * plan.Speed * plan.SpeedTimes;
                    //如果是最后一条生产线,全部分配给他
                    double planQty = mrpExSectionPlans.Last().ProductLine == plan.ProductLine ? currentQty : plan.TotalQty;
                    //取小
                    availableQty = availableQty > planQty ? planQty : availableQty;
                    //最小批量,并圆整
                    //availableQty = MrpExPlanCeilingLotSize(plan, plan.MinLotSize, availableQty);
                    plan.AdjustQty += (availableQty - plan.Qty - plan.CorrectionQty);
                    currentQty -= availableQty;
                    planTime.UsedTime += plan.TotalQty / plan.Speed / plan.SpeedTimes + plan.SwitchTime;
                    plan.Logs += " 第一次分配" + plan.ApsPriority.ToString() + (availableQty - plan.Qty - plan.CorrectionQty);
                }
                else//没有足够的工时,此生产线不做此断面
                {
                    //plan.Qty = 0;
                    //plan.CorrectionQty = 0;
                    plan.AdjustQty += -(plan.Qty + plan.CorrectionQty);
                    plan.Logs += " 没有工时" + (-(plan.Qty + plan.CorrectionQty));
                }
            }
            else
            {
                businessException.AddMessage("没有找到此断面{0}的工作日历", plan.Section);
            }
            return currentQty;
        }

        private double MrpExPlanCeilingLotSize(MrpExSectionPlan plan, double lotSize, double qty)
        {
            if (qty <= 0)
            {
                return 0;
            }
            if (plan.IsEconomic)
            {
                plan.CurrentSwitchTime = 0;
            }
            else
            {
                plan.CurrentSwitchTime = plan.SwitchTime;
            }
            //一个班有多少分钟
            int _shiftMinite = (24 / (int)(plan.ShiftType)) * 60;
            //最小要生产多少分钟(最小批量)
            double minLotSize = lotSize * _shiftMinite;
            //目前需要生产多少分钟
            double _requireTime = qty / (plan.Speed * plan.SpeedTimes) + plan.CurrentSwitchTime;
            //取大
            _requireTime = _requireTime > minLotSize ? _requireTime : minLotSize;
            //圆整到0.25个班
            _requireTime = Math.Ceiling(Math.Round(_requireTime / (_shiftMinite * 0.25), 2)) * (_shiftMinite * 0.25);
            //得到需要生产的数量
            return (_requireTime - plan.CurrentSwitchTime) * plan.Speed * plan.SpeedTimes;
        }

        private void SplitMaxMrpExSectionPlan(List<MrpExSectionPlan> sectionExPlanList)
        {
            var maxMrpExPlanList = new List<MrpExSectionPlan>();
            foreach (MrpExSectionPlan plan in sectionExPlanList)
            {
                if (plan.Sequence < 1000)
                {
                    double maxQty = plan.ShiftQuota * plan.MaxLotSize - plan.SwitchTime * plan.Speed * plan.SpeedTimes;

                    if (plan.TotalQty > 0 && maxQty > 0 && plan.TotalQty > maxQty)
                    {
                        plan.Sequence = 1;
                        plan.PlanNo = GetExPlanNo(plan, 1);

                        double totalQty = plan.TotalQty;
                        double qtyRate = plan.Qty / totalQty;
                        double correctionQtyRate = plan.CorrectionQty / totalQty;
                        double adjustQtyRate = plan.AdjustQty / totalQty;
                        plan.Qty = maxQty * qtyRate;
                        plan.CorrectionQty = maxQty * correctionQtyRate;
                        plan.AdjustQty = maxQty * adjustQtyRate;
                        plan.Logs += " 最大批量(1)" + maxQty * adjustQtyRate;

                        //新增一条记录,放在最后
                        var newMrpExPlan = Mapper.Map<MrpExSectionPlan, MrpExSectionPlan>(plan);
                        newMrpExPlan.Sequence = 1000;
                        //圆整到0.25个班
                        double newQty = totalQty - maxQty;
                        newMrpExPlan.Qty = newQty * qtyRate;
                        newMrpExPlan.CorrectionQty = newQty * correctionQtyRate;
                        newMrpExPlan.AdjustQty = newQty * adjustQtyRate;
                        newMrpExPlan.PlanNo = GetExPlanNo(plan, 2);
                        newMrpExPlan.Logs += " 最大批量(2)" + newQty * adjustQtyRate;

                        maxMrpExPlanList.Add(newMrpExPlan);
                    }
                }
            }
            sectionExPlanList.AddRange(maxMrpExPlanList);
        }

        public string GetExPlanNo(MrpExSectionPlan exSectionPlan, object sequence)
        {
            return exSectionPlan.DateIndex.Substring(2, 2) + exSectionPlan.DateIndex.Substring(5, 2) + sequence + exSectionPlan.ProductLine + exSectionPlan.Section;
        }

        private List<MrpExSectionPlan> GetMrpExSectionPlan(DateTime newPlanVersion, IList<MrpExPlan> exPlanList, BusinessException businessException)
        {
            var exInstanceDic = this.genericMgr.FindAll<ProdLineExInstance>
                ("from ProdLineExInstance where DateType = ? and DateIndex between ? and ? ",
                new object[] { CodeMaster.TimeUnit.Week, 
                   exPlanList.Min(p => p.DateIndex), 
                   exPlanList.Max(p => p.DateIndex)
                }).GroupBy(p => new { p.DateIndex, p.Item }, (k, g) => new { k, g })
                .ToDictionary(d => d.k, d => d.g);

            var totalSectionExPlanList = exPlanList.GroupBy(p => new { p.DateIndex, p.Section },
                  (k, g) => new
                  {
                      DateIndex = k.DateIndex,
                      Section = k.Section,
                      LatestStartTime = g.OrderBy(p => p.LatestStartTime).First().LatestStartTime,
                      Qty = g.Sum(q => q.SectionQty)
                  }).ToList();

            List<MrpExSectionPlan> sectionExPlanList = new List<MrpExSectionPlan>();
            foreach (var sectionExPlan in totalSectionExPlanList)
            {
                var exInstances = exInstanceDic.ValueOrDefault(new { sectionExPlan.DateIndex, Item = sectionExPlan.Section });
                if (exInstances != null)
                {
                    int totalAps = exInstances.Sum(p => (int)p.ApsPriority);
                    double totalNormalQuota = exInstances.Where(p => p.ApsPriority == CodeMaster.ApsPriorityType.Normal).Sum(p => p.Quota);
                    double totalQuota = exInstances.Sum(p => p.Quota);
                    foreach (var exInstance in exInstances)
                    {
                        MrpExSectionPlan mrpSectionExPlan = Mapper.Map<ProdLineExInstance, MrpExSectionPlan>(exInstance);
                        mrpSectionExPlan.Section = sectionExPlan.Section;
                        if (exInstance.ApsPriority == CodeMaster.ApsPriorityType.Normal)
                        {
                            mrpSectionExPlan.Qty = sectionExPlan.Qty * exInstance.Quota / totalNormalQuota; //断面长度
                            mrpSectionExPlan.CorrectionQty = mrpSectionExPlan.Qty * (mrpSectionExPlan.Correction - 1);
                        }
                        mrpSectionExPlan.DateIndex = sectionExPlan.DateIndex;
                        mrpSectionExPlan.PlanVersion = newPlanVersion;
                        mrpSectionExPlan.TotalAps = totalAps;
                        mrpSectionExPlan.TotalQuota = totalQuota;
                        mrpSectionExPlan.LatestStartTime = sectionExPlan.LatestStartTime;
                        mrpSectionExPlan.PlanNo = GetExPlanNo(mrpSectionExPlan, 1);
                        mrpSectionExPlan.Sequence = 1;
                        sectionExPlanList.Add(mrpSectionExPlan);
                    }
                }
                else
                {
                    businessException.AddMessage(new Message(CodeMaster.MessageType.Warning,
                        "没有找到断面:{0} 对应的挤出资源.", itemMgr.GetCacheItem(sectionExPlan.Section).CodeDescription));
                }
            }
            return sectionExPlanList;
        }

        private List<MrpExPlan> GetMrpExPlan(MrpPlanMaster mrpPlanMaster, IList<MrpShipPlanGroup> groupShipPlans, IList<MrpFlowDetail> mrpFlowDetailList,
            IList<InventoryBalance> inventoryBalanceList, DateTime newPlanVersion, string weekIndex, BusinessException businessException)
        {
            var inventoryBalanceDic = inventoryBalanceList.GroupBy(p => p.Item)
                 .ToDictionary(d => d.Key, d => new { InvQty = d.Sum(b => b.Qty), SafeStock = d.First().SafeStock });

            //要运行周的起始日期
            DateTime planDateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(weekIndex);

            groupShipPlans = groupShipPlans
               .Where(p => mrpFlowDetailList.Select(q => q.Flow).Distinct().Contains(p.Flow) && !p.IsDiscontinueItem)
               .ToList();

            //发货计划按周需求汇总
            var exPlanList = (from p in groupShipPlans
                              orderby p.Flow, p.StartTime
                              group p by new
                              {
                                  Item = p.Item,
                                  DateIndex = DateTimeHelper.GetWeekOfYear(p.StartTime.Date)
                              } into g
                              select new MrpExPlan
                              {
                                  Item = g.Key.Item,
                                  DateIndex = g.Key.DateIndex,
                                  ItemQty = g.Sum(s => s.Qty),
                                  StartTime = g.Max(s => s.StartTime.Date),
                                  UnitCount = Convert.ToDouble(itemMgr.GetCacheItem(g.Key.Item).UnitCount)
                              }).Where(p => p.DateIndex.CompareTo(weekIndex) >= 0)
                              .ToList();

            //从今天起的待收
            var planInList = GetProductPlanInList(mrpPlanMaster, DateTime.Today, planDateFrom, mrpFlowDetailList);
            //从今天到运行周起始日之间的待收
            var sumPlanInDic = planInList.Where(p => p.WindowTime < planDateFrom && p.WindowTime >= DateTime.Now.Date)
                .GroupBy(p => p.Item).ToDictionary(d => d.Key, d => d.Sum(q => q.Qty));
            //从今天到运行周起始日之间的待发
            var sumPlanOutDic = groupShipPlans.Where(p => p.WindowTime < planDateFrom)
                .GroupBy(p => p.Item).ToDictionary(d => d.Key, d => d.Sum(q => q.Qty));
            //周起始日的待发
            var planOutDic = groupShipPlans.Where(p => p.WindowTime >= planDateFrom)
                .GroupBy(p => p.Item).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var exPlan in exPlanList)
            {
                var mrpBoms = GetMrpBomList(exPlan.Item, exPlan.Item, exPlan.StartTime, businessException, true);
                var sectionBom = mrpBoms.FirstOrDefault(p => p.IsSection);
                if (sectionBom != null)
                {
                    var inv = inventoryBalanceDic.ValueOrDefault(exPlan.Item);
                    if (inv != null)
                    {
                        exPlan.InvQty = inv.InvQty;
                        exPlan.SafeStock = inv.SafeStock;
                    }
                   
                    exPlan.PlanInQty = sumPlanInDic.ValueOrDefault(exPlan.Item);
                    exPlan.PlanOutQty = sumPlanOutDic.ValueOrDefault(exPlan.Item) * (1 + sectionBom.ScrapPercentage);
                    exPlan.ItemQty = exPlan.ItemQty * (1 + sectionBom.ScrapPercentage);

                    exPlan.Section = sectionBom.Item;
                    exPlan.RateQty = sectionBom.RateQty;
                    if (exPlan.DateIndex == weekIndex)
                    {
                        //净需求=毛需求-库存-待收+待发+安全库存
                        exPlan.NetQty = exPlan.ItemQty - (exPlan.InvQty + exPlan.PlanInQty - exPlan.PlanOutQty) + exPlan.SafeStock;
                    }
                    else
                    {
                        exPlan.NetQty = exPlan.ItemQty;
                    }

                    if (exPlan.NetQty <= 0)
                    {
                        exPlan.SectionQty = 0;
                    }
                    else
                    {
                        exPlan.SectionQty = exPlan.RateQty * exPlan.NetQty;
                    }
                    exPlan.PlanVersion = newPlanVersion;
                    exPlan.LatestStartTime = DateTime.MaxValue;

                    if (exPlan.DateIndex == weekIndex)
                    {
                        var _planOutDic = (planOutDic.ValueOrDefault(exPlan.Item) ?? new List<MrpShipPlanGroup>())
                            .GroupBy(p => p.WindowTime.Date).ToDictionary(d => d.Key, d => d.Sum(q => q.Qty));

                        exPlan.LatestStartTime = planDateFrom;
                        DateTime endDate = planDateFrom.AddDays(14);
                        //期末库存=期初库存+待收-待发
                        exPlan.CurrentQty = exPlan.InvQty + exPlan.PlanInQty - exPlan.PlanOutQty;
                        while (exPlan.LatestStartTime < endDate)
                        {
                            if (exPlan.CurrentQty <= exPlan.SafeStock)
                            {
                                break;
                            }
                            exPlan.CurrentQty -= _planOutDic.ValueOrDefault(exPlan.LatestStartTime);
                            exPlan.LatestStartTime = exPlan.LatestStartTime.AddDays(1);
                        }

                        var item = itemMgr.GetCacheItem(exPlan.Item);
                        if (item.ItemOption == CodeMaster.ItemOption.NeedAging)
                        {
                            exPlan.LatestStartTime = exPlan.LatestStartTime.AddDays(-2);
                        }
                        else
                        {
                            exPlan.LatestStartTime = exPlan.LatestStartTime.AddDays(-1);
                        }
                    }
                }
                else
                {
                    exPlan.PlanVersion = newPlanVersion;
                    exPlan.LatestStartTime = DateTime.MaxValue;
                    businessException.AddMessage(new Message(CodeMaster.MessageType.Warning, "没有找到物料:{0}对应的断面.", exPlan.Item));
                }
            }

            this.genericMgr.BulkInsert<MrpExPlan>(exPlanList);
            return exPlanList.Where(p => !string.IsNullOrWhiteSpace(p.Section)).ToList();
        }

        private IList<MrpExItemPlan> GetMrpExItemPlanList(IList<MrpExSectionPlan> sectionExPlanList, IList<MrpExPlan> exPlanList)
        {
            List<MrpExItemPlan> mrpExItemPlanList = new List<MrpExItemPlan>();
            var oldSectionPlans = sectionExPlanList.Where(p => p.IsOld).ToList();
            foreach (var oldSectionPlan in oldSectionPlans)
            {
                if (oldSectionPlan.MrpExItemPlanList != null)
                {
                    foreach (var mrpExItemPlan in oldSectionPlan.MrpExItemPlanList)
                    {
                        mrpExItemPlan.SectionId = oldSectionPlan.Id;
                        this.genericMgr.Create(mrpExItemPlan);
                    }
                }
            }

            //断面按照DateIndex分开
            var sectionExPlanGroupByDateIndexs = sectionExPlanList.Where(p => !p.IsOld).ToList()
                .GroupBy(p => p.DateIndex, (k, g) => new { k, g }).ToList();
            //物料计划按照DateIndex分开
            var exPlanGroupByDateIndexs = exPlanList.GroupBy(p => p.DateIndex, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g);
            foreach (var sectionExPlanGroupByDateIndex in sectionExPlanGroupByDateIndexs)
            {
                //按照DateIndex分开好的物料计划
                var exPlanGroupByDateIndex = exPlanGroupByDateIndexs[sectionExPlanGroupByDateIndex.k];
                //按照断面号分开的断面
                var sectionExPlanGroupBySections = sectionExPlanGroupByDateIndex.g.GroupBy(p => p.Section, (k, g) => new { k, g }).ToList();
                //按照断面号分开的物料计划
                var exPlanGroupBySections = exPlanGroupByDateIndex.GroupBy(p => p.Section, (k, g) => new { k, g })
                    .ToDictionary(d => d.k, d => d.g);

                foreach (var sectionExPlanGroupBySection in sectionExPlanGroupBySections)
                {
                    var totalQty = sectionExPlanGroupBySection.g.Sum(p => p.Qty);
                    //var totalAdjustQty = sectionExPlanGroupBySection.g.Sum(p => p.AdjustQty);
                    //var totalCorrectionQty = sectionExPlanGroupBySection.g.Sum(p => p.CorrectionQty);
                    //查找相同断面的物料计划
                    if (exPlanGroupBySections.ContainsKey(sectionExPlanGroupBySection.k))
                    {
                        var exPlanGroupBySection = exPlanGroupBySections[sectionExPlanGroupBySection.k]
                            .OrderBy(p => p.LatestStartTime);
                        foreach (var sectionExPlan in sectionExPlanGroupBySection.g)
                        {
                            //var qtyRate = totalQty == 0 ? 0 : (sectionExPlan.Qty / totalQty);
                            //var adjustRate = sectionExPlan.Qty == 0 ? 0 : (sectionExPlan.AdjustQty / sectionExPlan.Qty);
                            //var correctionQtyRate = sectionExPlan.Qty == 0 ? 0 : (sectionExPlan.CorrectionQty / sectionExPlan.Qty);
                            int sequence = 10;
                            foreach (var exPlan in exPlanGroupBySection)
                            {
                                //var rate = totalQty == 0 ? 0 : (exPlan.SectionQty / totalQty);
                                var mrpExItemPlan = new MrpExItemPlan();
                                if (totalQty > 0)
                                {
                                    //mrpExItemPlan.Qty = exPlan.ItemQty * qtyRate;
                                    mrpExItemPlan.Qty = exPlan.NetQty * sectionExPlan.Qty / totalQty;
                                    mrpExItemPlan.AdjustQty = exPlan.NetQty * sectionExPlan.AdjustQty / totalQty;
                                    //mrpExItemPlan.CorrectionQty = mrpExItemPlan.Qty * correctionQtyRate;
                                    mrpExItemPlan.CorrectionQty = exPlan.NetQty * sectionExPlan.CorrectionQty / totalQty;
                                }

                                mrpExItemPlan.DateIndex = exPlan.DateIndex;
                                mrpExItemPlan.Item = exPlan.Item;
                                mrpExItemPlan.ItemDescription = this.itemMgr.GetCacheItem(exPlan.Item).Description;
                                mrpExItemPlan.PlanQty = exPlan.NetQty;
                                mrpExItemPlan.PlanVersion = exPlan.PlanVersion;
                                mrpExItemPlan.ProductLine = sectionExPlan.ProductLine;
                                mrpExItemPlan.RateQty = exPlan.RateQty;
                                mrpExItemPlan.Section = exPlan.Section;
                                mrpExItemPlan.SectionId = sectionExPlan.Id;//为了获取此值,不用BulkInsert
                                mrpExItemPlan.Sequence = sequence;
                                //mrpExItemPlan.ShiftQty = sectionExPlan.ShiftQty;
                                mrpExItemPlan.UnitCount = exPlan.UnitCount;
                                mrpExItemPlan.PlanDate = sectionExPlan.PlanDate;
                                mrpExItemPlan.IsOld = sectionExPlan.IsOld;
                                //mrpExItemPlan.Shift = sectionExPlan.Shift;
                                sequence += 10;
                                mrpExItemPlanList.Add(mrpExItemPlan);
                            }
                        }
                    }
                    else
                    {
                        //没有找到
                        logRunMrp.Info(string.Format("在周{0}没有找到对应的断面{1}", sectionExPlanGroupByDateIndex.k, sectionExPlanGroupBySection.k));
                    }
                }
            }
            return mrpExItemPlanList;
        }

        private List<MrpExPlanTime> GetMrpPlanTime(DateTime snapTime, IList<MrpExSectionPlan> mrpExSectionPlanList)
        {
            #region 每条生产线的需求总时间
            var exPlanListTime = (from p in mrpExSectionPlanList
                                  group p by new { p.ProductLine, p.DateIndex } into g
                                  select new MrpExPlanTime
                                  {
                                      ProductLine = g.Key.ProductLine,
                                      DateIndex = g.Key.DateIndex,
                                      SwitchTime = g.Sum(p => Math.Floor(p.Qty / (p.ShiftQuota * p.MaxLotSize)) * p.SwitchTime)
                                  }).ToList();

            foreach (var planTime in exPlanListTime)
            {
                IList<WorkingCalendarView> workingCalendarViews = workingCalendarMgr.GetWorkingCalendarViewList(
                       null,
                       planTime.ProductLine,
                       snapTime.Date,
                       DateTimeHelper.GetWeekIndexDateTo(planTime.DateIndex));

                foreach (var workingCalendarView in workingCalendarViews)
                {
                    if (workingCalendarView.Type == CodeMaster.WorkingCalendarType.Work)
                    {
                        if (workingCalendarView.DateFrom > snapTime)
                        {
                            TimeSpan timeSpan = workingCalendarView.DateTo - workingCalendarView.DateFrom;
                            planTime.UpTime += timeSpan.TotalMinutes;
                        }
                        else if (workingCalendarView.DateFrom < snapTime && workingCalendarView.DateTo > snapTime)
                        {
                            TimeSpan timeSpan = workingCalendarView.DateTo - snapTime;
                            planTime.UpTime += timeSpan.TotalMinutes;
                        }
                    }
                }
                planTime.UsedTime = planTime.SwitchTime;
            }
            #endregion

            return exPlanListTime;
        }

        private MrpExSectionPlan SplitMrpExPlanByLot(MrpExSectionPlan plan, double lotQty)
        {
            double totalQty = plan.TotalQty;
            double qtyRate = plan.Qty / totalQty;
            double correctionQtyRate = plan.CorrectionQty / totalQty;
            double adjustQtyRate = plan.AdjustQty / totalQty;
            plan.Qty = lotQty * qtyRate;
            plan.CorrectionQty = lotQty * correctionQtyRate;
            plan.AdjustQty = lotQty * adjustQtyRate;
            plan.Logs += " 批量圆整" + lotQty * adjustQtyRate;

            //新增一条记录,放在最后
            var newMrpExPlan = Mapper.Map<MrpExSectionPlan, MrpExSectionPlan>(plan);
            newMrpExPlan.Sequence = 1000;
            //圆整到0.25个班
            double newQty = MrpExPlanCeilingLotSize(newMrpExPlan, 0.0, totalQty - lotQty);
            newMrpExPlan.Qty = newQty * qtyRate;
            newMrpExPlan.CorrectionQty = newQty * correctionQtyRate;
            newMrpExPlan.AdjustQty = newQty * adjustQtyRate;
            newMrpExPlan.PlanNo = GetExPlanNo(plan, 2);
            newMrpExPlan.Logs += " 批量圆整" + newQty * adjustQtyRate;
            return newMrpExPlan;
        }

        private void ProcessMrpExSectionPlan(ref List<MrpExSectionPlan> mrpExSectionPlanList, MrpPlanMaster mrpPlanMaster, string weekIndex)
        {
            DateTime oldPlanVersion = mrpPlanMaster.PlanVersion;
            DateTime newPlanVersion = mrpExSectionPlanList.First().PlanVersion;
            string prevDateIndex = weekIndex;
            //排产周的开始时间
            DateTime dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(weekIndex);
            string currentWeekIndex = Utility.DateTimeHelper.GetWeekOfYear(newPlanVersion);
            #region 首尾相接
            int compare = string.Compare(currentWeekIndex, weekIndex);
            if (compare == 0)//当期计划重排
            {
                //当前周,开始时间取当前
                dateFrom = DateTime.Now;
            }
            else if (compare < 0)//非当期未来计划
            {
                //上期时间索引
                prevDateIndex = Utility.DateTimeHelper.GetWeekOfYear(dateFrom.AddDays(-7));
                //开始时间取本周的开始时间
            }
            else
            {
                throw new TechnicalException("不能排过期的计划");
            }
            //
            var oldExSectionPlans = this.genericMgr.FindAllWithNativeSql<object[]>
               (@"SELECT T.ProductLine,T.Section FROM
                (select distinct ProductLine from MRP_MrpExSectionPlan where PlanVersion =? and DateIndex =? and WindowTime<=?) AS E
                CROSS APPLY (SELECT TOP(1)* FROM MRP_MrpExSectionPlan AS T1 WHERE E.ProductLine = T1.ProductLine ORDER BY T1.WindowTime DESC) AS T ",
               new object[] { mrpPlanMaster.PlanVersion, prevDateIndex, dateFrom });

            foreach (var oldPlan in oldExSectionPlans)
            {
                var plan = mrpExSectionPlanList
                           .FirstOrDefault(p => p.Section == (string)oldPlan[1] && p.DateIndex == weekIndex && p.ProductLine == (string)oldPlan[0]);

                if (plan != null)
                {
                    plan.Sequence = -1;
                    plan.IsEconomic = true;
                    plan.CurrentSwitchTime = 0;
                    plan.PlanNo = GetExPlanNo(plan, 0);
                }
            }
            #endregion

            #region 圆整
            foreach (var sectionExPlan in mrpExSectionPlanList)
            {
                sectionExPlan.Remark = this.genericMgr.FindById<ProductType>(sectionExPlan.ProductType).Description;
                double qty = MrpExPlanCeilingLotSize(sectionExPlan, 0.0, sectionExPlan.Qty + sectionExPlan.CorrectionQty);
                sectionExPlan.AdjustQty = qty - sectionExPlan.Qty - sectionExPlan.CorrectionQty;
            }
            #endregion

            mrpExSectionPlanList = mrpExSectionPlanList.OrderBy(p => p.DateIndex)
                .ThenBy(p => p.ProductLine)
                .ThenBy(p => p.Sequence)
                .ThenBy(p => p.LatestStartTime)
                .ToList();
            var groupMrpExSectionPlans = mrpExSectionPlanList
                .GroupBy(p => new { p.DateIndex, p.ProductLine }, (k, List) => new { k, List });

            var newPlanList = new List<MrpExSectionPlan>();
            foreach (var groupMrpExSectionPlan in groupMrpExSectionPlans)
            {
                #region //group by Section 圆整
                var groupMrpExSectionPlanBySections = groupMrpExSectionPlan.List
                .GroupBy(p => p.Section, (Section, List) => new { Section, List = List.ToList() });
                foreach (var groupPlan in groupMrpExSectionPlanBySections)
                {
                    //所有断面总需求
                    var sumTotalQty = groupPlan.List.Sum(p => p.TotalQty);
                    if (sumTotalQty > 0)
                    {
                        //断面原始需求
                        var sumQty = groupPlan.List.Sum(p => p.Qty);
                        foreach (var sectionExPlan in groupPlan.List)
                        {
                            var totalQty = sectionExPlan.TotalQty;
                            sectionExPlan.Qty = sumQty * (totalQty / sumTotalQty);
                            sectionExPlan.CorrectionQty = sectionExPlan.Qty * (sectionExPlan.Correction - 1);
                            //生产总量-原先总量=调整量
                            sectionExPlan.AdjustQty = totalQty - sectionExPlan.Qty - sectionExPlan.CorrectionQty;
                            sectionExPlan.Logs += " 按比例调整" + sectionExPlan.AdjustQty;
                        }
                    }
                }
                #endregion

                #region 排班 //工作日历
                int seq = 10;
                dateFrom = newPlanVersion;
                if (!(groupMrpExSectionPlan.k.DateIndex == currentWeekIndex))
                {
                    dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(groupMrpExSectionPlan.k.DateIndex).AddHours(7.75);
                }

                var regionCode = this.genericMgr.FindById<FlowMaster>(groupMrpExSectionPlan.k.ProductLine).PartyFrom;
                var workingCalendarViews = this.workingCalendarMgr.GetWorkingCalendarViewList(regionCode, groupMrpExSectionPlan.k.ProductLine, dateFrom.AddDays(-4), dateFrom.AddDays(30));
                dateFrom = this.workingCalendarMgr.GetStartTimeAtWorkingDate(dateFrom, regionCode, groupMrpExSectionPlan.k.ProductLine, workingCalendarViews);
                var currentCalender = workingCalendarViews.FirstOrDefault(p => p.DateFrom <= dateFrom && p.DateTo > dateFrom);
                var workingCalendarViewArray = workingCalendarViews.Where(p => p.Type == CodeMaster.WorkingCalendarType.Work).ToArray();
                int workIndex = Array.IndexOf<WorkingCalendarView>(workingCalendarViewArray, currentCalender);

                foreach (var plan in groupMrpExSectionPlan.List)
                {
                    plan.Sequence = seq;
                    plan.StartTime = dateFrom;
                    if (plan.TotalQty > 50)//精度损失,所以暂定大于50
                    {
                        double switchTime = plan.SwitchTime;
                        if (plan.IsEconomic)
                        {
                            plan.CurrentSwitchTime = 0;
                            switchTime = 0;
                        }
                        else
                        {
                            plan.CurrentSwitchTime = plan.SwitchTime;
                        }
                        //需要工作的分钟数
                        double workMinites = (plan.TotalQty / plan.Speed / plan.SpeedTimes) + plan.CurrentSwitchTime;

                        if (workIndex < workingCalendarViewArray.Length)
                        {
                            currentCalender = workingCalendarViewArray[workIndex];

                            plan.WindowTime = currentCalender.DateTo;
                            //plan.Shift = currentCalender.ShiftCode;
                            //当前工作日历的可用的分钟数
                            double calenderMinites = (currentCalender.DateTo - dateFrom).TotalMinutes;
                            plan.CurrentSwitchTime = plan.CurrentSwitchTime < calenderMinites ? plan.CurrentSwitchTime : calenderMinites;//取小

                            if (calenderMinites >= workMinites)
                            {
                                dateFrom = dateFrom.AddMinutes(workMinites);
                                plan.WindowTime = dateFrom;
                            }
                            else
                            {
                                //换班
                                //数量
                                double currentQty = ((plan.WindowTime - plan.StartTime).TotalMinutes - plan.CurrentSwitchTime) * plan.Speed * plan.SpeedTimes;
                                double normalQty = plan.Qty + plan.AdjustQty;
                                if (currentQty >= normalQty)
                                {
                                    //用到了修正量
                                    plan.LeftQty = 0;
                                    plan.LeftAdjustQty = 0;

                                    double correctionQty = plan.CorrectionQty;
                                    plan.CorrectionQty = currentQty - normalQty;
                                    plan.LeftCorrectionQty = correctionQty - plan.CorrectionQty;
                                }
                                else
                                {
                                    //没有用到修正量
                                    //qty:100 adj:-20 current:30 
                                    double qty = plan.Qty;
                                    double adjustQty = plan.AdjustQty;
                                    double correctionQty = plan.CorrectionQty;

                                    if (plan.AdjustQty > 0)
                                    {
                                        if (qty > currentQty)
                                        {
                                            plan.Qty = currentQty;
                                            plan.AdjustQty = 0;
                                        }
                                        else
                                        {
                                            //用到了调整
                                            //plan.Qty = currentQty;
                                            plan.AdjustQty = currentQty - qty;
                                        }
                                    }
                                    else//调整数小于0
                                    {
                                        plan.Qty = currentQty;
                                        plan.AdjustQty = 0;
                                    }

                                    plan.CorrectionQty = 0;

                                    plan.LeftQty = qty - plan.Qty;
                                    plan.LeftAdjustQty = adjustQty - plan.AdjustQty;
                                    plan.LeftCorrectionQty = correctionQty;
                                }

                                workMinites = workMinites - calenderMinites;
                                dateFrom = currentCalender.DateTo;
                                double leftSwitchTime = switchTime - plan.CurrentSwitchTime;
                                GetNewExPlanList(plan, workingCalendarViewArray, workMinites, ref dateFrom, ref workIndex, ref newPlanList, ref leftSwitchTime);
                            }
                            plan.ShiftQty = (plan.WindowTime - plan.StartTime).TotalHours / (24 / (int)plan.ShiftType);
                        }
                        else
                        {
                            //没有排上班
                            dateFrom = dateFrom.AddMinutes(workMinites);
                            plan.WindowTime = dateFrom;
                            plan.ShiftQty = Math.Round((plan.TotalQty / plan.ShiftQuota) + plan.SwitchTime / ((24 / (int)plan.ShiftType) * 60), 2);
                        }
                    }
                    else
                    {
                        plan.WindowTime = plan.StartTime;
                    }
                    seq += 10;
                }
                #endregion
            }
            mrpExSectionPlanList.AddRange(newPlanList);

            #region //补齐
            if (weekIndex == currentWeekIndex)
            {
                var groupPlans = mrpExSectionPlanList
                    .Where(p => p.DateIndex == weekIndex)
                    .OrderBy(p => p.StartTime)
                    .GroupBy(p => new
                    {
                        PlanVersion = p.PlanVersion,
                        DateIndex = p.DateIndex,
                        ProductLine = p.ProductLine
                    },
                    (k, g) => new
                    {
                        k,
                        List = g.ToList()
                    });

                var oldMrpExSectionPlanList = this.genericMgr.FindAll<MrpExSectionPlan>
                    ("from MrpExSectionPlan where PlanVersion =? and DateIndex =? ", new object[] { oldPlanVersion, weekIndex });
                var oldExItemPlanDic = this.genericMgr.FindAll<MrpExItemPlan>
                    ("from MrpExItemPlan where PlanVersion =? and DateIndex =? ", new object[] { oldPlanVersion, weekIndex })
                    .GroupBy(p => p.SectionId, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.ToList());

                var oldGroupPlanDic = oldMrpExSectionPlanList
                    .OrderBy(p => p.StartTime)
                    .GroupBy(p => new
                    {
                        ProductLine = p.ProductLine
                    },
                    (k, g) => new
                    {
                        k,
                        List = g.ToList()
                    }).ToDictionary(d => d.k.ProductLine, d => d.List);

                var oldPlanList = new List<MrpExSectionPlan>();
                var planVersion = mrpExSectionPlanList.First().PlanVersion;
                foreach (var groupPlan in groupPlans)
                {
                    var minStartTime = groupPlan.List.Min(q => q.StartTime);
                    //需要补齐
                    var oldPlans = (oldGroupPlanDic.ValueOrDefault(groupPlan.k.ProductLine) ?? new List<MrpExSectionPlan>())
                        .Where(p => p.WindowTime.Date <= minStartTime.Date);
                    foreach (var oldPlan in oldPlans)
                    {
                        var newPlan = Mapper.Map<MrpExSectionPlan, MrpExSectionPlan>(oldPlan);
                        newPlan.PlanVersion = planVersion;
                        newPlan.IsOld = true;
                        var oldItemPlans = oldExItemPlanDic.ValueOrDefault(oldPlan.Id);
                        if (oldItemPlans != null)
                        {
                            foreach (var oldItemPlan in oldItemPlans)
                            {
                                var newItemPlan = Mapper.Map<MrpExItemPlan, MrpExItemPlan>(oldItemPlan);
                                newPlan.AddMrpExPlanItem(newItemPlan);
                            }
                        }

                        if (minStartTime.Date == oldPlan.PlanDate.Date)
                        {
                            if (oldPlan.StartTime > minStartTime)
                            {
                                continue;
                            }
                            else if (oldPlan.WindowTime > minStartTime && oldPlan.StartTime < minStartTime)
                            {
                                var _rate = (minStartTime - oldPlan.StartTime).TotalHours / (oldPlan.WindowTime - oldPlan.StartTime).TotalHours;

                                newPlan.WindowTime = minStartTime;
                                newPlan.AdjustQty = oldPlan.AdjustQty * _rate;
                                newPlan.CorrectionQty = oldPlan.CorrectionQty * _rate;
                                newPlan.CurrentSwitchTime = oldPlan.CurrentSwitchTime * _rate;
                                newPlan.Qty = oldPlan.Qty * _rate;
                                newPlan.ShiftQty = oldPlan.ShiftQty * _rate;
                                if (newPlan.MrpExItemPlanList != null)
                                {
                                    foreach (var exItemPlan in newPlan.MrpExItemPlanList)
                                    {
                                        exItemPlan.CorrectionQty *= _rate;
                                        exItemPlan.Qty *= _rate;
                                        exItemPlan.AdjustQty *= _rate;
                                    }
                                }
                            }
                        }
                        oldPlanList.Add(newPlan);
                    }
                }
                mrpExSectionPlanList.AddRange(oldPlanList);
            }
            #endregion

            #region //汇总到天
            mrpExSectionPlanList = mrpExSectionPlanList.GroupBy(p =>
                new
                {
                    PlanVersion = p.PlanVersion,
                    DateIndex = p.DateIndex,
                    ProductLine = p.ProductLine,
                    Section = p.Section,
                    PlanDate = p.StartTime.AddHours(-7.75).Date,
                    IsOld = p.IsOld
                },
                (k, g) =>
                {
                    var plan = g.First();
                    plan.PlanDate = k.PlanDate;
                    plan.Qty = g.Sum(q => q.Qty);
                    plan.CorrectionQty = g.Sum(q => q.CorrectionQty);
                    plan.AdjustQty = g.Sum(q => q.AdjustQty);
                    plan.ShiftQty = g.Sum(q => q.ShiftQty);
                    plan.CurrentSwitchTime = g.Sum(q => q.CurrentSwitchTime);
                    plan.StartTime = g.Min(p => p.StartTime);
                    plan.WindowTime = g.Max(p => p.WindowTime);
                    plan.IsOld = k.IsOld;
                    return plan;
                })
                .OrderBy(p => p.DateIndex)
                .ThenBy(p => p.ProductLine)
                .ThenBy(p => p.StartTime)
                .ToList();
            #endregion

            #region 按Sequence排序
            var groupPlan1 = mrpExSectionPlanList.OrderBy(p => p.StartTime).GroupBy(p =>
                  new
                  {
                      PlanVersion = p.PlanVersion,
                      DateIndex = p.DateIndex,
                      ProductLine = p.ProductLine,
                      PlanDate = p.PlanDate
                  },
                  (k, g) => new
                  {
                      k,
                      List = g.ToList()
                  });
            foreach (var groupPlan in groupPlan1)
            {
                int seq = 10;
                foreach (var plan in groupPlan.List)
                {
                    plan.Sequence = seq;
                    plan.SectionDescription = itemMgr.GetCacheItem(plan.Section).Description;
                    plan.LastModifyDate = DateTime.Now;
                    //plan.PlanDate = plan.StartTime.AddHours(-7.75).Date;
                    seq += 10;
                }
            }
            #endregion
        }

        private void GetNewExPlanList(MrpExSectionPlan plan, WorkingCalendarView[] workingCalendarViewArray, double workMinites,
            ref DateTime dateFrom, ref int workIndex, ref List<MrpExSectionPlan> newMrpExSectionPlanList, ref double leftSwitchTime)
        {
            workIndex++;
            if (workIndex < workingCalendarViewArray.Length)
            {
                var currentCalender = workingCalendarViewArray[workIndex];

                var newPlan = Mapper.Map<MrpExSectionPlan, MrpExSectionPlan>(plan);
                //newPlan.Shift = currentCalender.ShiftCode;
                newPlan.StartTime = currentCalender.DateFrom;
                newPlan.WindowTime = currentCalender.DateTo;
                newMrpExSectionPlanList.Add(newPlan);

                double calenderMinites = (currentCalender.DateTo - currentCalender.DateFrom).TotalMinutes;
                newPlan.CurrentSwitchTime = newPlan.SwitchTime < calenderMinites ? leftSwitchTime : calenderMinites;//取小
                if (calenderMinites >= workMinites)
                {
                    dateFrom = dateFrom.AddMinutes(workMinites);
                    newPlan.WindowTime = dateFrom;
                    newPlan.CorrectionQty = plan.LeftCorrectionQty;
                    newPlan.Qty = plan.LeftQty;
                    newPlan.AdjustQty = plan.LeftAdjustQty;

                    newPlan.LeftQty = 0;
                    newPlan.LeftAdjustQty = 0;
                    newPlan.LeftCorrectionQty = 0;
                }
                else
                {
                    //数量
                    double currentQty = ((newPlan.WindowTime - newPlan.StartTime).TotalMinutes - newPlan.CurrentSwitchTime) * newPlan.Speed * newPlan.SpeedTimes;
                    double normalQty = newPlan.LeftQty + newPlan.LeftAdjustQty;
                    if (currentQty > normalQty)
                    {
                        //用到了修正量
                        newPlan.Qty = plan.LeftQty;
                        newPlan.AdjustQty = plan.LeftAdjustQty;
                        newPlan.CorrectionQty = currentQty - normalQty;

                        newPlan.LeftQty = 0;
                        newPlan.LeftAdjustQty = 0;
                        newPlan.LeftCorrectionQty = plan.LeftCorrectionQty - newPlan.CorrectionQty;
                    }
                    else
                    {
                        //没有用到修正量
                        double qty = plan.LeftQty;
                        double adjustQty = plan.LeftAdjustQty;
                        double correctionQty = plan.LeftCorrectionQty;

                        //newPlan.Qty = currentQty;
                        //newPlan.AdjustQty = currentQty - normalQty;

                        if (newPlan.AdjustQty > 0)
                        {
                            if (qty > currentQty)
                            {
                                newPlan.Qty = currentQty;
                                newPlan.AdjustQty = 0;
                            }
                            else
                            {
                                //用到了调整
                                //plan.Qty = currentQty;
                                newPlan.AdjustQty = currentQty - qty;
                            }
                        }
                        else//调整数小于0
                        {
                            newPlan.Qty = currentQty;
                            newPlan.AdjustQty = 0;
                        }

                        newPlan.CorrectionQty = 0;

                        newPlan.LeftQty = qty - newPlan.Qty;
                        newPlan.LeftAdjustQty = adjustQty - newPlan.AdjustQty;
                        newPlan.LeftCorrectionQty = correctionQty;
                    }
                    //继续换班
                    workMinites = workMinites - calenderMinites;
                    dateFrom = currentCalender.DateTo;
                    leftSwitchTime = leftSwitchTime - newPlan.CurrentSwitchTime;
                    GetNewExPlanList(newPlan, workingCalendarViewArray, workMinites, ref dateFrom, ref workIndex, ref newMrpExSectionPlanList, ref leftSwitchTime);
                }
                newPlan.ShiftQty = (newPlan.WindowTime - newPlan.StartTime).TotalHours / (24 / (int)newPlan.ShiftType);
            }
            else
            {
                dateFrom = dateFrom.AddMinutes(workMinites);
                plan.WindowTime = dateFrom;
                plan.ShiftQty = Math.Round((plan.TotalQty / plan.ShiftQuota) + plan.SwitchTime / ((24 / (int)plan.ShiftType) * 60), 2);
            }
        }

        #region 挤出手工调整
        [Transaction(TransactionMode.Requires)]
        public void AdjustMrpExSectionPlanList(IList<MrpExSectionPlan> mrpExSectionPlanList)
        {
            if (mrpExSectionPlanList.Select(p => p.ProductLine).Distinct().Count() > 1)
            {
                throw new BusinessException("不支持多条生产线同时调整");
            }
            if (mrpExSectionPlanList.Select(p => p.DateIndex).Distinct().Count() > 1)
            {
                throw new BusinessException("不支持多周同时调整");
            }

            #region 校验时间
            var firstPlan = mrpExSectionPlanList.First();
            int _shift = (int)firstPlan.ShiftType;
            _shift = _shift == 0 ? 3 : _shift;
            double shiftMinutes = (24 / _shift) * 60;//分钟
            mrpExSectionPlanList = mrpExSectionPlanList.OrderBy(p => p.Sequence).ToList();
            DateTime dateFrom = mrpExSectionPlanList.Min(p => p.StartTime);
            //dateFrom = dateFrom<DateTime.Now?
            DateTime dateTo = Utility.DateTimeHelper.GetWeekIndexDateTo(firstPlan.DateIndex);

            var totalMinutes = mrpExSectionPlanList.Sum(p => p.ShiftQty * shiftMinutes);
            if (totalMinutes > (dateTo - dateFrom).TotalMinutes)
            {
                //throw new BusinessException("当前工时{0},超出可用工时{1}", totalMinutes / 60, (dateTo - dateFrom).TotalHours);
            }
            var mrpExDateIndex = genericMgr.FindAll<MrpExDateIndex>
                (" from MrpExDateIndex where DateIndex =? and PlanVersion = ? and IsActive=? ",
                new object[] { firstPlan.DateIndex, firstPlan.PlanVersion, true });
            if (mrpExDateIndex.Count > 0)
            {
                throw new BusinessException("当前计划版本{0}已经释放,此计划版本不能调整.", firstPlan.PlanVersion);
            }
            #endregion

            //new
            foreach (var newMrpExSectionPlan in mrpExSectionPlanList.Where(p => p.Id == 0 && p.Section != BusinessConstants.VIRTUALSECTION))
            {
                var oldMrpExSectionPlans = mrpExSectionPlanList
                    .Where(p => p.DateIndex == newMrpExSectionPlan.DateIndex
                        && p.Section == newMrpExSectionPlan.Section
                        && p.Id > 0);
                if (oldMrpExSectionPlans == null || oldMrpExSectionPlans.Count() == 0)
                {
                    throw new BusinessException("没有需求的断面{0}不能手动新增需求", newMrpExSectionPlan.Section);
                }
            }

            var distinctSectionPlanList = new List<MrpExSectionPlan>();
            var deleteSectionPlanList = new List<MrpExSectionPlan>();

            var distinctSectionSequences = mrpExSectionPlanList.GroupBy(p => new { p.Section, p.Sequence }, (k, g) =>
                new { k.Section, k.Sequence, List = g });
            //Sequence Section相同就被合并
            foreach (var distinctSectionSequence in distinctSectionSequences)
            {
                var firstSectionPlan = distinctSectionSequence.List.First();
                firstSectionPlan.ShiftQty = distinctSectionSequence.List.Sum(p => p.ShiftQty);
                firstSectionPlan.Qty = distinctSectionSequence.List.Sum(p => p.Qty);
                firstSectionPlan.CorrectionQty = distinctSectionSequence.List.Sum(p => p.CorrectionQty);
                distinctSectionPlanList.Add(firstSectionPlan);
                var mergedSectionPlans = distinctSectionSequence.List.Skip(1);
                deleteSectionPlanList.AddRange(mergedSectionPlans);
            }

            var groupMrpExSectionPlanBySections = distinctSectionPlanList
                .GroupBy(p => p.Section, (Section, List) => new { Section, List = List.ToList() });

            var mrpExItemPlanList = this.genericMgr.FindAll<MrpExItemPlan>
                ("from MrpExItemPlan where DateIndex = ? and PlanVersion=? and ProductLine=? ",
                new object[] { firstPlan.DateIndex, firstPlan.PlanVersion, firstPlan.ProductLine });

            #region 调整
            foreach (var groupPlan in groupMrpExSectionPlanBySections)
            {
                var _firstPlan = groupPlan.List.First();
                //断面调整后的所有断面总需求
                var totalCurrentQty = groupPlan.List
                    .Sum(p => (p.ShiftQty * shiftMinutes - _firstPlan.SwitchTime) < 0 ? 0 :
                        ((p.ShiftQty * shiftMinutes - _firstPlan.SwitchTime) * _firstPlan.Speed * _firstPlan.SpeedTimes));
                //挤出件
                var totalMrpExItemPlanDic = mrpExItemPlanList.Where(p => p.Section == groupPlan.Section)
                                          .GroupBy(p => p.Item, (Item, List) => new { Item, Qty = List.Sum(p => p.Qty) })
                                          .ToDictionary(d => d.Item, d => d.Qty);

                //断面原始需求
                int seqIndex = 0;
                var totalQty = groupPlan.List.Sum(p => p.Qty);
                foreach (var sectionExPlan in groupPlan.List)
                {
                    if (sectionExPlan.Id == distinctSectionPlanList.First().Id
                        && sectionExPlan.IsEconomic)
                    {
                        seqIndex = 0;
                    }
                    else
                    {
                        seqIndex++;
                    }
                    sectionExPlan.PlanNo = GetExPlanNo(sectionExPlan, seqIndex);
                    var currentQty = (shiftMinutes * sectionExPlan.ShiftQty - sectionExPlan.SwitchTime)
                                     * sectionExPlan.Speed * sectionExPlan.SpeedTimes;
                    currentQty = currentQty < 0 ? 0 : currentQty;
                    sectionExPlan.Qty = totalCurrentQty == 0 ? 0 : (totalQty * (currentQty / totalCurrentQty));
                    sectionExPlan.CorrectionQty = sectionExPlan.Qty * (sectionExPlan.Correction - 1);
                    sectionExPlan.AdjustQty = currentQty - sectionExPlan.Qty - sectionExPlan.CorrectionQty;//生产总量-原先总量=调整量

                    if (sectionExPlan.Id == 0)
                    {
                        this.genericMgr.Create(sectionExPlan);
                        if (sectionExPlan.Section != BusinessConstants.VIRTUALSECTION)
                        {
                            var mrpExItemPlans = mrpExItemPlanList.Where(p => p.SectionId == groupPlan.List.First().Id);
                            foreach (var mrpExItemPlan in mrpExItemPlans)
                            {
                                var newMrpExItemPlan = Mapper.Map<MrpExItemPlan, MrpExItemPlan>(mrpExItemPlan);
                                newMrpExItemPlan.Qty = totalCurrentQty == 0 ? 0 : (totalMrpExItemPlanDic.ValueOrDefault(newMrpExItemPlan.Item) * (currentQty / totalCurrentQty));
                                newMrpExItemPlan.CorrectionQty = newMrpExItemPlan.Qty * (sectionExPlan.Correction - 1);
                                newMrpExItemPlan.AdjustQty = sectionExPlan.Qty == 0 ? 0 : (newMrpExItemPlan.Qty * (sectionExPlan.AdjustQty / sectionExPlan.Qty));
                                this.genericMgr.Create(newMrpExItemPlan);
                                sectionExPlan.AddMrpExPlanItem(newMrpExItemPlan);
                            }
                        }
                        else
                        {
                            var planItem = new MrpExItemPlan();
                            planItem.AdjustQty = sectionExPlan.TotalQty;
                            planItem.CorrectionQty = 0;
                            planItem.DateIndex = sectionExPlan.DateIndex;
                            planItem.Item = sectionExPlan.Section;
                            planItem.PlanQty = sectionExPlan.TotalQty;
                            planItem.PlanQtyRate = 1;
                            planItem.PlanVersion = sectionExPlan.PlanVersion;
                            planItem.ProductLine = sectionExPlan.ProductLine;
                            planItem.Qty = 0;
                            planItem.RateQty = 1;
                            planItem.Section = sectionExPlan.Section;
                            planItem.SectionId = sectionExPlan.Id;
                            //planItem.Sequence = sequence;
                            planItem.UnitCount = 1;
                            this.genericMgr.Create(planItem);
                            sectionExPlan.AddMrpExPlanItem(planItem);
                        }
                    }
                    else
                    {
                        var mrpExItemPlans = mrpExItemPlanList.Where(p => p.SectionId == sectionExPlan.Id);
                        foreach (var mrpExItemPlan in mrpExItemPlans)
                        {
                            mrpExItemPlan.Qty = totalCurrentQty == 0 ? 0 : (totalMrpExItemPlanDic.ValueOrDefault(mrpExItemPlan.Item) * (currentQty / totalCurrentQty));
                            mrpExItemPlan.CorrectionQty = mrpExItemPlan.Qty * (sectionExPlan.Correction - 1);
                            mrpExItemPlan.AdjustQty = sectionExPlan.Qty == 0 ? 0 : (mrpExItemPlan.Qty * (sectionExPlan.AdjustQty / sectionExPlan.Qty));
                            this.genericMgr.Update(mrpExItemPlan);
                            sectionExPlan.AddMrpExPlanItem(mrpExItemPlan);
                        }
                    }
                }
            }
            #endregion

            #region 排序
            var regionCode = this.genericMgr.FindById<FlowMaster>(firstPlan.ProductLine).PartyFrom;
            var workingCalendarViews = this.workingCalendarMgr.GetWorkingCalendarViewList(regionCode, firstPlan.ProductLine, dateFrom.AddDays(-4), dateTo.AddDays(14));
            dateFrom = this.workingCalendarMgr.GetStartTimeAtWorkingDate(dateFrom, regionCode, firstPlan.ProductLine, workingCalendarViews);
            var currentCalender = workingCalendarViews.FirstOrDefault(p => p.DateFrom <= dateFrom && p.DateTo > dateFrom);
            var workingCalendarViewArray = workingCalendarViews.Where(p => p.Type == CodeMaster.WorkingCalendarType.Work).ToArray();
            int workIndex = Array.IndexOf<WorkingCalendarView>(workingCalendarViewArray, currentCalender);

            int seq = 10;
            int detailSeq = 10;
            foreach (var mrpExSectionPlan in distinctSectionPlanList)
            {
                mrpExSectionPlan.Sequence = seq;
                mrpExSectionPlan.StartTime = dateFrom;

                //需要工作的分钟数
                double workMinites = mrpExSectionPlan.ShiftQty * shiftMinutes;
                if (workIndex < workingCalendarViewArray.Length)
                {
                    currentCalender = workingCalendarViewArray[workIndex];
                    //当前工作日历的可用的分钟数
                    double calenderMinites = (currentCalender.DateTo - dateFrom).TotalMinutes;
                    if (calenderMinites >= workMinites)
                    {
                        dateFrom = dateFrom.AddMinutes(workMinites);
                    }
                    else
                    {
                        workMinites = workMinites - calenderMinites;
                        dateFrom = currentCalender.DateTo;
                        GetExPlanWindowTime(workingCalendarViewArray, workMinites, ref dateFrom, ref workIndex);
                    }
                    mrpExSectionPlan.WindowTime = dateFrom;
                }
                else
                {
                    mrpExSectionPlan.WindowTime = dateFrom.AddMinutes(workMinites);
                    dateFrom = mrpExSectionPlan.WindowTime;
                }

                if (mrpExSectionPlan.MrpExItemPlanList != null)
                {
                    foreach (var mrpExItemPlan in mrpExSectionPlan.MrpExItemPlanList)
                    {
                        mrpExItemPlan.Sequence = detailSeq;
                        detailSeq += 10;
                        this.genericMgr.Update(mrpExItemPlan);
                    }
                }
                this.genericMgr.Update(mrpExSectionPlan);
                seq += 10;
            }
            #endregion

            foreach (var deleteSectionPlan in deleteSectionPlanList)
            {
                this.genericMgr.Delete(deleteSectionPlan);
                var mrpExItemPlans = mrpExItemPlanList.Where(p => p.SectionId == deleteSectionPlan.Id);
                foreach (var mrpExItemPlan in mrpExItemPlans)
                {
                    this.genericMgr.Delete(mrpExItemPlan);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void AdjustMrpExItemPlanList(IList<MrpExItemPlan> mrpExItemPlanList)
        {
            //判断是否超出断面总长度
            if (true)
            {

            }
            foreach (var mrpExItemPlan in mrpExItemPlanList)
            {
                mrpExItemPlan.AdjustQty = mrpExItemPlan.CurrentQty - mrpExItemPlan.Qty - mrpExItemPlan.CorrectionQty;
                this.genericMgr.Update(mrpExItemPlan);
            }
        }

        private static object ReleaseExPlanLock = new object();
        [Transaction(TransactionMode.Requires)]
        public void ReleaseExPlan(string flow, DateTime planVersion, DateTime planDate)
        {
            lock (ReleaseExPlanLock)
            {
                var dateIndex = Utility.DateTimeHelper.GetWeekOfYear(planDate);
                var paramList = new List<object> { planVersion, dateIndex, planDate };
                string hql = " from MrpExSectionPlan p where p.PlanVersion=? and p.DateIndex=? and p.PlanDate=? ";
                if (!string.IsNullOrEmpty(flow))
                {
                    hql += " and p.ProductLine=? ";
                    paramList.Add(flow);
                }
                hql += " order by p.ProductLine,p.PlanDate,p.Sequence";
                var mrpExSectionPlanList = this.genericMgr.FindAll<MrpExSectionPlan>(hql, paramList.ToArray());

                paramList = new List<object> { planDate, true };
                hql = "from MrpExPlanMaster where PlanDate =? and IsActive=? ";
                if (!string.IsNullOrEmpty(flow))
                {
                    hql += " and ProductLine=? ";
                    paramList.Add(flow);
                }
                var oldMrpExPlanMasterList = this.genericMgr.FindAll<MrpExPlanMaster>(hql, paramList.ToArray());
                if (mrpExSectionPlanList == null || mrpExSectionPlanList.Count == 0)
                {
                    if (oldMrpExPlanMasterList != null && oldMrpExPlanMasterList.Count > 0)
                    {
                        foreach (var oldMrpExPlanMaster in oldMrpExPlanMasterList)
                        {
                            oldMrpExPlanMaster.IsActive = false;
                            this.genericMgr.Update(oldMrpExPlanMaster);
                        }
                    }
                    /// 退出
                    return;
                }

                #region 获取上个班生产的物料号
                string shiftCode = "EX3-3";
                DateTime prePlanDate = planDate.AddDays(-1);
                GetExShift(mrpExSectionPlanList[0].ProductLine, planDate, out shiftCode, out prePlanDate);
                var preMrpExPlanMasterList = oldMrpExPlanMasterList;
                if (prePlanDate != planDate)
                {
                    paramList = new List<object> { prePlanDate, true, shiftCode };
                    hql = "from MrpExPlanMaster where PlanDate =? and IsActive=? and Shift =? ";
                    if (!string.IsNullOrEmpty(flow))
                    {
                        hql += " and ProductLine=? ";
                        paramList.Add(flow);
                    }
                    preMrpExPlanMasterList = this.genericMgr.FindAll<MrpExPlanMaster>(hql, paramList.ToArray());
                }
                var preMrpExShiftPlans = new List<MrpExShiftPlan>();
                if (preMrpExPlanMasterList != null)
                {
                    foreach (var preMrpExPlanMaster in preMrpExPlanMasterList)
                    {
                        paramList = new List<object> { flow, preMrpExPlanMaster.ReleaseVersion, shiftCode };
                        hql = @"from MrpExShiftPlan s where s.ProductLine = ? and s.ReleaseVersion =? and s.Shift =? and s.Qty>0
                                order by s.Shift desc,s.Sequence desc ";
                        var preShiftPlans = genericMgr.FindAll<MrpExShiftPlan>(hql, paramList.ToArray(), 0, 1);
                        preMrpExShiftPlans.AddRange(preShiftPlans);
                    }
                }
                #endregion

                DateTime releaseVersion = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                var productTypeList = this.genericMgr.FindAll<ProductType>();
                var needTurns = productTypeList.Where(p => p.NeedTurn).Select(p => p.Code).ToList();
                var needFreezes = productTypeList.Where(p => p.NeedFreeze).Select(p => p.Code).ToList();

                var snapTime = this.genericMgr.FindAll<MrpSnapMaster>
                     (@"from MrpSnapMaster m where m.IsRelease =? and m.Type=? order by m.SnapTime desc",
                     new object[] { true, CodeMaster.SnapType.Mrp }, 0, 1)
                     .First().SnapTime;

                //var mrpExPlanDic = GetMrpExPlanDic(dateIndex, planVersion);
                var mrpExItemPlanList = this.genericMgr.FindAllIn<MrpExItemPlan>(
                    " from MrpExItemPlan where SectionId in(? ", mrpExSectionPlanList.Select(p => (object)p.Id).Distinct());

                var mrpExItemPlanDic = mrpExItemPlanList.GroupBy(p => p.SectionId).ToDictionary(d => d.Key, d => d.ToList());

                #region 库存对半制品分配的影响
                //var locationDetailViewList = this.genericMgr.FindEntityWithNativeSql<LocationDetailView>
                //   (@"select l.* from VIEW_LocationDet as l inner join MD_Location as loc on l.Location = loc.Code where loc.IsMrp = ? ",
                //   new object[] { true });
                //var invDic = locationDetailViewList.Where(p => p.ATPQty > 0).GroupBy(p => p.Item)
                //    .ToDictionary(p => p.Key, p => (double)p.Sum(q => q.ATPQty));

                //foreach (var exSection in mrpExSectionPlanList)
                //{
                //    //和前一个班不同断面且数量前面没有考虑过库存
                //    if (exSection.SwitchTime > 0 && exSection.TotalQty > 1 && exSection.TotalQty == exSection.Qty)
                //    {
                //        var mrpExItemPlans = mrpExItemPlanDic.ValueOrDefault(exSection.Id);
                //        foreach (var exItemPlan in mrpExItemPlans)
                //        {
                //            exItemPlan.InvQty = invDic.ValueOrDefault(exItemPlan.Item);//实时库存
                //            exItemPlan.CurrentQty = exItemPlan.InvQty;
                //        }
                //        double totalLength = 0;
                //        while (totalLength < exSection.TotalQty)
                //        {
                //            var minmrpExItemPlan = mrpExItemPlans.OrderBy(p => p.CurrentQty).First();
                //            minmrpExItemPlan.CurrentQty += 50;
                //            totalLength += 50 * minmrpExItemPlan.RateQty;
                //        }
                //        foreach (var exItemPlan in mrpExItemPlans)
                //        {
                //            exItemPlan.AdjustQty = exItemPlan.CurrentQty - exItemPlan.Qty - exItemPlan.InvQty;
                //        }
                //    }
                //    //和前一个班同断面不考虑库存因素
                //}
                #endregion

                var mrpExItemPlanGroupbySection = mrpExItemPlanList.GroupBy(p => p.Section);
                foreach (var mrpExItemPlanGroup in mrpExItemPlanGroupbySection)
                {
                    double maxPlanQty = mrpExItemPlanGroup.Max(p => p.PlanQty);
                    foreach (var plan in mrpExItemPlanGroup)
                    {
                        plan.CurrentQty = Math.Round(plan.TotalQty, 2);
                        if (maxPlanQty == 0)
                        {
                            plan.PlanQtyRate = 0;
                        }
                        else
                        {
                            plan.PlanQtyRate = Math.Round(plan.PlanQty / maxPlanQty, 2);
                        }
                    }
                }

                var newPlanList = SplitMrpExSectionPlanByShift(mrpExSectionPlanList, planDate);

                var exFlowDetailDic = this.genericMgr.FindAll<MrpFlowDetail>
                                  (" from MrpFlowDetail where SnapTime = ? and ResourceGroup=? ",
                                  new object[] { snapTime, CodeMaster.ResourceGroup.EX })
                                  .GroupBy(p => p.Flow, (Flow, List) => new { Flow, List })
                                  .ToDictionary(d => d.Flow, d => d.List.ToList());

                var groupExSectionPlanByProductLine = newPlanList.GroupBy(p => p.ProductLine)
                    .Select(p => new
                    {
                        ProductLine = p.Key,
                        List = p.GroupBy(q => new
                        {
                            q.ProductLine,
                            q.PlanVersion,
                            q.PlanDate,
                            q.Shift,
                            q.ShiftDateFrom,
                            q.DateIndex,
                        }).Select(r => new
                        {
                            ProductLine = r.Key.ProductLine,
                            PlanVersion = r.Key.PlanVersion,
                            PlanDate = r.Key.PlanDate,
                            Shift = r.Key.Shift,
                            ShiftDateFrom = r.Key.ShiftDateFrom,
                            DateIndex = r.Key.DateIndex,
                            List = r.ToList()
                        }).ToList()
                    }).ToList();

                var mrpExShiftPlanList = new List<MrpExShiftPlan>();
                List<string> excludeShiftList = new List<string>();
                foreach (var list in groupExSectionPlanByProductLine)
                {
                    string turnQtyItem = null;
                    string turnQtySection = null;
                    //获取此生产线前一天最后生产的物料号
                    var preShiftPlan = preMrpExShiftPlans.FirstOrDefault(p => p.ProductLine == list.ProductLine);

                    var groupExSectionPlans = list.List;
                    foreach (var groupExSectionPlan in groupExSectionPlans)
                    {
                        if (groupExSectionPlan.ShiftDateFrom < DateTime.Now)
                        {
                            //此班次已经开始做了
                            //不做处理
                            excludeShiftList.Add(groupExSectionPlan.Shift);
                        }
                        else
                        {
                            #region 记录 MrpExPlanMaster
                            MrpExPlanMaster mrpExPlanMaster = new MrpExPlanMaster();
                            mrpExPlanMaster.Shift = groupExSectionPlan.Shift;
                            mrpExPlanMaster.ProductLine = groupExSectionPlan.ProductLine;
                            mrpExPlanMaster.PlanDate = groupExSectionPlan.PlanDate;
                            mrpExPlanMaster.DateIndex = groupExSectionPlan.DateIndex;
                            mrpExPlanMaster.PlanVersion = groupExSectionPlan.PlanVersion;
                            mrpExPlanMaster.ReleaseVersion = releaseVersion;
                            mrpExPlanMaster.IsActive = true;
                            this.genericMgr.Create(mrpExPlanMaster);
                            #endregion

                            //生产线明细
                            var flowDetails = exFlowDetailDic.ValueOrDefault(groupExSectionPlan.ProductLine) ?? new List<MrpFlowDetail>();

                            var flowDetailDic = flowDetails.GroupBy(p => p.Item, (k, g) => new { k, g })
                                .ToDictionary(d => d.k, d => d.g.First());
                            foreach (var exSectionPlan in groupExSectionPlan.List)
                            {
                                if (turnQtySection != exSectionPlan.Section)
                                {
                                    //上个断面和此断面不一样,取初始轮番倍数
                                    turnQtyItem = null;
                                }
                                turnQtySection = exSectionPlan.Section;
                                bool isFreeze = needFreezes.Contains(exSectionPlan.ProductType);
                                var mrpExPlans = mrpExItemPlanDic.ValueOrDefault(exSectionPlan.NewId);
                                //var mrpExPlans = exSectionPlan.MrpExItemPlanList;

                                if (mrpExPlans == null)
                                {
                                    #region VIRTUALSECTION
                                    if (exSectionPlan.Section == BusinessConstants.VIRTUALSECTION)
                                    {
                                        exSectionPlan.PlanNo = this.GetExPlanNo(exSectionPlan, 1);

                                        var mrpExShiftPlan = new MrpExShiftPlan();
                                        mrpExShiftPlan.PlanVersion = exSectionPlan.PlanVersion;
                                        mrpExShiftPlan.PlanNo = exSectionPlan.PlanNo;
                                        mrpExShiftPlan.ItemId = exSectionPlan.NewId;
                                        mrpExShiftPlan.ProductLine = exSectionPlan.ProductLine;
                                        mrpExShiftPlan.DateIndex = exSectionPlan.DateIndex;
                                        //mrpExShiftPlan.Sequence = sequence;
                                        mrpExShiftPlan.Item = exSectionPlan.Section;
                                        mrpExShiftPlan.ItemDescription = exSectionPlan.SectionDescription;

                                        var item = this.itemMgr.GetCacheItem(exSectionPlan.Section);
                                        mrpExShiftPlan.TurnQty = exSectionPlan.TurnQty;
                                        mrpExShiftPlan.Qty = 1;
                                        mrpExShiftPlan.UnitCount = (double)item.UnitCount;
                                        mrpExShiftPlan.Uom = item.Uom;
                                        mrpExShiftPlan.RateQty = 1;
                                        mrpExShiftPlan.StartTime = exSectionPlan.StartTime;
                                        mrpExShiftPlan.WindowTime = exSectionPlan.WindowTime;
                                        mrpExShiftPlan.Section = exSectionPlan.Section;
                                        mrpExShiftPlan.IsCorrection = false;
                                        mrpExShiftPlan.IsNew = true;
                                        mrpExShiftPlan.Remark = exSectionPlan.Remark;
                                        mrpExShiftPlan.PlanStartTime = exSectionPlan.StartTime;
                                        mrpExShiftPlan.PlanWindowTime = exSectionPlan.WindowTime;
                                        mrpExShiftPlan.Speed = exSectionPlan.Speed * exSectionPlan.SpeedTimes;
                                        mrpExShiftPlan.SwitchTime = 0;
                                        mrpExShiftPlan.Bom = exSectionPlan.Section;

                                        var flowDetail = flowDetails.First();
                                        mrpExShiftPlan.LocationFrom = flowDetail.LocationFrom;
                                        mrpExShiftPlan.LocationTo = flowDetail.LocationTo;
                                        mrpExShiftPlan.Shift = exSectionPlan.Shift;
                                        mrpExShiftPlan.ShiftType = (int)exSectionPlan.ShiftType;
                                        mrpExShiftPlan.ShiftQty = (mrpExShiftPlan.WindowTime - mrpExShiftPlan.StartTime).TotalHours / (24 / mrpExShiftPlan.ShiftType);
                                        mrpExShiftPlan.PlanDate = exSectionPlan.PlanDate;
                                        mrpExShiftPlan.ReleaseVersion = releaseVersion;
                                        mrpExShiftPlan.ProductType = exSectionPlan.ProductType;
                                        mrpExShiftPlanList.Add(mrpExShiftPlan);
                                    }
                                    else
                                    {
                                        //todo
                                        logRunMrp.Warn(string.Format("没有找到此断面{0}对应的半制品", exSectionPlan.Section));
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region
                                    //Sequence	PlanQty	Qty	TrunQty
                                    //   1	    900	    900	300
                                    //   2	    900	    900	300
                                    //   3	    900	    700	300
                                    //   4	    900 	600	300
                                    var startTime = exSectionPlan.StartTime;
                                    //PlanQty最大的Item,需求量最大的
                                    //var maxPlanQtyMrpExPlan = mrpExPlans.OrderByDescending(p => p.PlanQty).First();
                                    int c1 = 0;
                                    bool isFirst = true;
                                    while (true)
                                    {
                                        //算法:按照需求比排序,先补充大的.\1
                                        var maxExPlan = mrpExPlans
                                            .OrderByDescending(p => p.RequirePlanRate)
                                            .ThenByDescending(p => p.Sequence)
                                            .ThenByDescending(p => p.Id).FirstOrDefault();
                                        //\4
                                        var minPlanItem = mrpExPlans
                                            .OrderBy(p => p.RequirePlanRate)
                                            .ThenBy(p => p.Sequence)
                                            .ThenBy(p => p.Id).FirstOrDefault();
                                        if (maxExPlan.RequirePlanRate < 0)
                                        {
                                            //排满了
                                            break;
                                        }

                                        MrpExItemPlan exPlan = null;
                                        double turnQty = 0;
                                        bool refPreshiftTurnQty = false;
                                        //找出相同配比的,相差在半个包装以内的视同平衡
                                        if (preShiftPlan != null)
                                        {
                                            //没有切换时间
                                            if (mrpExPlans.Count > 0)
                                            {
                                                if (preShiftPlan.Section == mrpExPlans.FirstOrDefault().Section)
                                                {
                                                    isFirst = false;
                                                }
                                            }
                                            //接前一天轮番
                                            exPlan = mrpExPlans.FirstOrDefault(p => p.Item == preShiftPlan.Item);
                                            if (exPlan != null)
                                            {
                                                turnQty = exPlan.UnitCount * exSectionPlan.TurnQty - preShiftPlan.Qty;
                                                if (turnQty < 0)
                                                {
                                                    turnQty = 0;
                                                }
                                                refPreshiftTurnQty = turnQty >= 0 ? true : false;
                                                turnQtyItem = exPlan.Item;
                                            }

                                            if (turnQty <= 0 && !refPreshiftTurnQty) //前一天轮番已满,取下一个物料
                                            {
                                                exPlan = GetMrpExItemPlanAccordingToFlowDetSeq(mrpExPlans, mrpExShiftPlanList);
                                                if (exPlan == null)
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                exPlan.CurrentQty -= turnQty;
                                            }
                                            preShiftPlan = null;
                                        }
                                        else
                                        {
                                            exPlan = GetMrpExItemPlanAccordingToFlowDetSeq(mrpExPlans, mrpExShiftPlanList);
                                            if (exPlan == null)
                                            {
                                                break;
                                            }
                                        }
                                        //没有找到相同配比的
                                        if (exPlan == null)
                                        {
                                            exPlan = maxExPlan;
                                            //minPlanItem = new MrpExItemPlan();
                                        }

                                        //没有接上前一班的物料,重新计算轮番倍数
                                        if (turnQty <= 0 && !refPreshiftTurnQty)
                                        {
                                            //轮番量 第一个物料使用轮番倍数,其他的根据量计算出来.
                                            if (turnQtyItem == exPlan.Item || turnQtyItem == null)
                                            {
                                                //使用轮番倍数 不超过剩余数
                                                turnQty = exSectionPlan.TurnQty * exPlan.UnitCount;
                                                //turnQty = exPlan.CurrentQty < turnQty ? Math.Ceiling(Math.Round(exPlan.CurrentQty / exPlan.UnitCount, 2)) * exPlan.UnitCount : turnQty;
                                                if (turnQty <= 0)
                                                {
                                                    break;
                                                }
                                                exPlan.CurrentQty -= turnQty;
                                                turnQtyItem = exPlan.Item;
                                            }
                                            else
                                            {
                                                //计算出来
                                                int c2 = 1;
                                                while (true)
                                                {
                                                    c2++;
                                                    exPlan.CurrentQty -= exPlan.UnitCount * exSectionPlan.TurnQty;
                                                    turnQty += exPlan.UnitCount * exSectionPlan.TurnQty;
                                                    //2014/12/29 轮番倍数不根据需求比例变化Begin
                                                    break;
                                                    //2014/12/29 轮番倍数不根据需求比例变化End
                                                    var requirePlanRate = minPlanItem.RequirePlanRate < 0 ? 0 : minPlanItem.RequirePlanRate;
                                                    if (c2 > exSectionPlan.TurnQty || exPlan.RequirePlanRate <= requirePlanRate)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        refPreshiftTurnQty = false;
                                        //算出窗口时间
                                        double minites = turnQty * exPlan.RateQty / exSectionPlan.Speed / exSectionPlan.SpeedTimes;

                                        double currentSwitchTime = isFirst ? exSectionPlan.CurrentSwitchTime : 0.0;
                                        exSectionPlan.CurrentSwitchTime -= currentSwitchTime;

                                        var mrpExShiftPlans = GetExShiftPlan(exSectionPlan, exPlan, turnQty, startTime, minites, flowDetailDic, isFreeze, currentSwitchTime);

                                        minites += currentSwitchTime;

                                        var lastShiftPlan = mrpExShiftPlanList.LastOrDefault();

                                        if (startTime > DateTime.Now)
                                        {
                                            foreach (var mrpExShiftPlan in mrpExShiftPlans)
                                            {
                                                if (lastShiftPlan != null && lastShiftPlan.Item == mrpExShiftPlan.Item
                                                    && lastShiftPlan.Shift == mrpExShiftPlan.Shift
                                                    && lastShiftPlan.IsCorrection == mrpExShiftPlan.IsCorrection
                                                    && lastShiftPlan.ProductType == mrpExShiftPlan.ProductType)
                                                {
                                                    //lastShiftPlan.Qty += mrpExShiftPlan.Qty;
                                                    lastShiftPlan.WindowTime = mrpExShiftPlan.WindowTime;
                                                    if (lastShiftPlan.WindowTime > exSectionPlan.WindowTime)
                                                    {
                                                        lastShiftPlan.WindowTime = exSectionPlan.WindowTime;
                                                        lastShiftPlan.ShiftQty = (lastShiftPlan.WindowTime - lastShiftPlan.StartTime).TotalHours / (24 / lastShiftPlan.ShiftType);

                                                        var currentLeftTime = (exSectionPlan.WindowTime - mrpExShiftPlan.StartTime).TotalMinutes;
                                                        //重新计算舍弃了<0.01的班别数，所以这里轮番也舍弃<0.01的班别数
                                                        if (mrpExShiftPlan.ShiftQty < 0.01)
                                                        {
                                                            currentLeftTime = 0;
                                                        }
                                                        var leftSwitchTime = currentLeftTime > mrpExShiftPlan.SwitchTime ? 0 : mrpExShiftPlan.SwitchTime - currentLeftTime;
                                                        var avialTime = currentLeftTime > mrpExShiftPlan.SwitchTime ? currentLeftTime - mrpExShiftPlan.SwitchTime : 0;

                                                        //剩余的时间还能做多少个
                                                        var leftQty = avialTime * mrpExShiftPlan.Speed / mrpExShiftPlan.RateQty;

                                                        //圆整
                                                        leftQty = Math.Ceiling(Math.Round(leftQty / mrpExShiftPlan.UnitCount, 2)) * mrpExShiftPlan.UnitCount;

                                                        lastShiftPlan.Qty += leftQty;

                                                        //exPlan.CurrentQty = exPlan.CurrentQty + turnQty - leftQty;
                                                        exSectionPlan.CurrentSwitchTime += leftSwitchTime;

                                                        int planIndex = groupExSectionPlans.IndexOf(groupExSectionPlan);
                                                        if (planIndex < groupExSectionPlans.Count - 1)
                                                        {
                                                            var nextGroupPlan = groupExSectionPlans[planIndex + 1];
                                                            var nextSectionPlan = nextGroupPlan.List.Where(p => p.Section == mrpExShiftPlan.Section).FirstOrDefault();
                                                            if (nextSectionPlan != null)
                                                            {
                                                                var newPlan = Mapper.Map<MrpExShiftPlan, MrpExShiftPlan>(mrpExShiftPlan);
                                                                //跨班被分割的计划换模时间算在前半部分，后半部分没有换模时间
                                                                newPlan.SwitchTime = 0;
                                                                newPlan.ReleaseVersion = releaseVersion;
                                                                newPlan.Shift = nextGroupPlan.Shift;
                                                                newPlan.Qty = mrpExShiftPlan.Qty - leftQty;
                                                                newPlan.StartTime = exSectionPlan.WindowTime;
                                                                newPlan.WindowTime = newPlan.StartTime.AddMinutes(newPlan.Qty * mrpExShiftPlan.RateQty / mrpExShiftPlan.Speed + leftSwitchTime);
                                                                newPlan.ShiftQty = (newPlan.WindowTime - newPlan.StartTime).TotalHours / (24 / lastShiftPlan.ShiftType);
                                                                nextSectionPlan.StartTime = newPlan.WindowTime;
                                                                mrpExShiftPlanList.Add(newPlan);
                                                            }
                                                        }
                                                        //break;
                                                    }
                                                    else
                                                    {
                                                        lastShiftPlan.Qty += mrpExShiftPlan.Qty;
                                                        lastShiftPlan.ShiftQty = (lastShiftPlan.WindowTime - lastShiftPlan.StartTime).TotalHours / (24 / lastShiftPlan.ShiftType);
                                                    }
                                                }
                                                else
                                                {
                                                    mrpExShiftPlan.ReleaseVersion = releaseVersion;
                                                    mrpExShiftPlanList.Add(mrpExShiftPlan);
                                                    //sequence += 10;
                                                    if (mrpExShiftPlan.WindowTime > exSectionPlan.WindowTime)
                                                    {
                                                        mrpExShiftPlan.WindowTime = exSectionPlan.WindowTime;
                                                        mrpExShiftPlan.ShiftQty = (mrpExShiftPlan.WindowTime - mrpExShiftPlan.StartTime).TotalHours / (24 / mrpExShiftPlan.ShiftType);

                                                        var currentLeftTime = (mrpExShiftPlan.WindowTime - mrpExShiftPlan.StartTime).TotalMinutes;
                                                        //重新计算舍弃了<0.01的班别数，所以这里轮番也舍弃<0.01的班别数
                                                        if (mrpExShiftPlan.ShiftQty < 0.01)
                                                        {
                                                            currentLeftTime = 0;
                                                        }
                                                        var leftSwitchTime = currentLeftTime > mrpExShiftPlan.SwitchTime ? 0 : mrpExShiftPlan.SwitchTime - currentLeftTime;
                                                        var avialTime = currentLeftTime > mrpExShiftPlan.SwitchTime ? currentLeftTime - mrpExShiftPlan.SwitchTime : 0;

                                                        //剩余的时间还能做多少个
                                                        var leftQty = avialTime * mrpExShiftPlan.Speed / mrpExShiftPlan.RateQty;
                                                        //圆整
                                                        leftQty = Math.Ceiling(Math.Round(leftQty / mrpExShiftPlan.UnitCount, 2)) * mrpExShiftPlan.UnitCount;

                                                        int planIndex = groupExSectionPlans.IndexOf(groupExSectionPlan);
                                                        if (planIndex < groupExSectionPlans.Count - 1)
                                                        {
                                                            var nextGroupPlan = groupExSectionPlans[planIndex + 1];
                                                            var nextSectionPlan = nextGroupPlan.List.Where(p => p.Section == mrpExShiftPlan.Section).FirstOrDefault();
                                                            if (nextSectionPlan != null)
                                                            {
                                                                var newPlan = Mapper.Map<MrpExShiftPlan, MrpExShiftPlan>(mrpExShiftPlan);
                                                                //跨班被分割的计划换模时间算在前半部分，后半部分没有换模时间
                                                                newPlan.SwitchTime = 0;
                                                                newPlan.Shift = nextGroupPlan.Shift;
                                                                newPlan.Qty = mrpExShiftPlan.Qty - leftQty;
                                                                newPlan.StartTime = mrpExShiftPlan.WindowTime;
                                                                newPlan.WindowTime = newPlan.StartTime.AddMinutes(newPlan.Qty * mrpExShiftPlan.RateQty / mrpExShiftPlan.Speed + leftSwitchTime);
                                                                newPlan.ShiftQty = (newPlan.WindowTime - newPlan.StartTime).TotalHours / (24 / newPlan.ShiftType);
                                                                nextSectionPlan.StartTime = newPlan.WindowTime;
                                                                mrpExShiftPlanList.Add(newPlan);
                                                            }
                                                        }
                                                        exSectionPlan.CurrentSwitchTime += leftSwitchTime;
                                                        mrpExShiftPlan.Qty = leftQty;
                                                    }
                                                }
                                            }
                                        }

                                        startTime = startTime.AddMinutes(minites);
                                        if (startTime >= exSectionPlan.WindowTime)
                                        {
                                            break;
                                        }

                                        c1++;
                                        if (c1 > 10000)
                                        {
                                            logRunMrp.Error("循环了10000次还没有完成排班,结束循环");
                                            break;
                                        }
                                        isFirst = false;
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }

                #region 更新 MrpExPlanMaster 无效
                if (oldMrpExPlanMasterList != null && oldMrpExPlanMasterList.Count > 0)
                {
                    foreach (var oldMrpExPlanMaster in oldMrpExPlanMasterList)
                    {
                        if (!excludeShiftList.Contains(oldMrpExPlanMaster.Shift))
                        {
                            oldMrpExPlanMaster.IsActive = false;
                            this.genericMgr.Update(oldMrpExPlanMaster);
                        }
                    }
                }
                #endregion

                SequenceMrpExShiftPlanList(ref mrpExShiftPlanList, needTurns);

                this.genericMgr.BulkInsert<MrpExShiftPlan>(mrpExShiftPlanList);

                #region 使用存储过程进行修正
                this.genericMgr.FlushSession();
                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@SnapTime", snapTime);
                sqlParameters[1] = new SqlParameter("@PlanVersion", planVersion);
                sqlParameters[2] = new SqlParameter("@DateIndex", dateIndex);
                sqlParameters[3] = new SqlParameter("@PlanDate", planDate);
                this.genericMgr.ExecuteStoredProcedure("USP_Busi_MRP_ReleaseExPlan", sqlParameters);
                #endregion
            }
        }

        private MrpExItemPlan GetMrpExItemPlan(List<MrpExItemPlan> mrpExItemPlanList, MrpExItemPlan maxMrpExItemPlan)
        {
            //找出和需求大的具有相同配比的,相差在半个包装以内的视同平衡
            //var maxSequence = maxMrpExItemPlan.Sequence;
            var mrpExItemPlans = mrpExItemPlanList.Where(p => p.PlanQtyRate == maxMrpExItemPlan.PlanQtyRate
                    && p.CurrentQty <= maxMrpExItemPlan.CurrentQty + p.UnitCount / 2
                    && p.CurrentQty > maxMrpExItemPlan.CurrentQty - p.UnitCount / 2)
                    .OrderBy(p => p.Sequence);

            var exPlan = mrpExItemPlans.FirstOrDefault(p => p.Item != mrpExItemPlanList[0].Item);
            //如果是最后一个,将找不到更大的序号,就找第一个.
            if (exPlan == null)
            {
                exPlan = mrpExItemPlans.FirstOrDefault();
            }
            return exPlan;
        }
        //2014/12/29 增加函数 GetMrpExItemPlanAccordingToFlowDetSeq,轮番的优先级根据FlowDet下面的Seq来取，ItemPlan下的Seq即为FlowDet的Seq.
        private MrpExItemPlan GetMrpExItemPlanAccordingToFlowDetSeq(List<MrpExItemPlan> mrpExItemPlanList, List<MrpExShiftPlan> mrpExShiftPlanList)
        {
            var needTurnItems = mrpExItemPlanList.Where(p => p.CurrentQty > 0);
            if (needTurnItems.ToList().Count == 0)
            {
                return null;
            }
            string section = needTurnItems.FirstOrDefault().Section;
            var minSequence = needTurnItems.OrderBy(p => p.Sequence).FirstOrDefault().Sequence;
            var maxSequence = needTurnItems.OrderByDescending(p => p.Sequence).FirstOrDefault();
            var preMrpExShiftPlanList = mrpExShiftPlanList.Where(p => p.Section == section).LastOrDefault();
            var preSeq = minSequence - 1;
            if (preMrpExShiftPlanList != null)
            {
                //有可能会更改路线下的半制品明细，所以加一个 p.Item == preMrpExShiftPlanList.Item 判断
                if (needTurnItems.Where(p => p.Section == section/* && p.Item == preMrpExShiftPlanList.Item*/).FirstOrDefault() != null)
                {
                    //preSeq = needTurnItems.Where(p => p.Section == section && p.Item == preMrpExShiftPlanList.Item).FirstOrDefault().Sequence;
                    preSeq = mrpExItemPlanList.Where(p => p.Section == preMrpExShiftPlanList.Section && p.Item == preMrpExShiftPlanList.Item).FirstOrDefault().Sequence;
                }
            }
            var mrpExItemPlans = needTurnItems
                    .OrderBy(p => p.Sequence);

            var exPlan = mrpExItemPlans.Where(p => p.Sequence > preSeq).OrderBy(p => p.Sequence).FirstOrDefault();
            //如果是最后一个,将找不到更大的序号,就找第一个.
            if (exPlan == null)
            {
                exPlan = mrpExItemPlans.FirstOrDefault();
            }
            return exPlan;
        }

        private void GetExShift(string flow, DateTime planDate, out string shfitCode, out DateTime prePlanDate)
        {
            shfitCode = "EX3-3";
            prePlanDate = planDate.AddDays(-1);
            if (planDate == DateTime.Now.AddHours(-7.75).Date)
            {
                var workingCalendarViews = workingCalendarMgr.GetWorkingCalendarViewList(null, flow, planDate.AddDays(-1), planDate);
                var workingCalendarView = workingCalendarViews.FirstOrDefault(p => p.DateFrom <= DateTime.Now && p.DateTo > DateTime.Now);
                if (workingCalendarView != null)
                {
                    shfitCode = workingCalendarView.ShiftCode;
                    prePlanDate = workingCalendarView.Date;
                }
            }
        }

        private void SequenceMrpExShiftPlanList(ref List<MrpExShiftPlan> mrpExShiftPlanList, List<string> needTurns)
        {
            var newMrpExShiftPlanList = new List<MrpExShiftPlan>();
            newMrpExShiftPlanList.AddRange(mrpExShiftPlanList.Where(p => needTurns.Contains(p.ProductType)));
            newMrpExShiftPlanList.AddRange(
                mrpExShiftPlanList.Where(p => !needTurns.Contains(p.ProductType))
                .GroupBy(p => new { p.Shift, p.Item, p.ItemDescription })
                .Select(p =>
                {
                    var shiftPlan = p.First();
                    shiftPlan.Qty = p.Sum(q => q.Qty);
                    shiftPlan.StartTime = p.Select(q => q.StartTime).Min();
                    shiftPlan.WindowTime = p.Select(q => q.WindowTime).Max();
                    shiftPlan.ShiftQty = p.Sum(q => q.ShiftQty);
                    shiftPlan.IsCorrection = false;
                    return shiftPlan;
                })
                );

            var groupShiftPlans = newMrpExShiftPlanList.OrderBy(p => p.StartTime)
                .GroupBy(p => new { p.Shift, p.ProductLine });

            foreach (var groupShiftPlan in groupShiftPlans)
            {
                int seq = 10;
                foreach (var shiftPlan in groupShiftPlan)
                {
                    if (shiftPlan.Qty > 0)
                    {
                        shiftPlan.Sequence = seq;
                        seq += 10;
                    }
                }
            }
            mrpExShiftPlanList = newMrpExShiftPlanList.Where(p => p.Qty > 0).ToList();
        }

        private List<MrpExSectionPlan> SplitMrpExSectionPlanByShift(IList<MrpExSectionPlan> mrpExSectionPlanList, DateTime planDate)
        {
            var groupExSectionPlans1 = from p in mrpExSectionPlanList
                                       group p by new
                                       {
                                           ProductLine = p.ProductLine,
                                           PlanVersion = p.PlanVersion,
                                           PlanDate = p.PlanDate,
                                           DateIndex = p.DateIndex,
                                       } into g
                                       select new
                                       {
                                           ProductLine = g.Key.ProductLine,
                                           PlanVersion = g.Key.PlanVersion,
                                           PlanDate = g.Key.PlanDate,
                                           DateIndex = g.Key.DateIndex,
                                           List = g.ToList()
                                       };
            #region 拆分到班
            var shiftMasterList = this.genericMgr.FindAll<ShiftMaster>
                ("from ShiftMaster where Code like ? order by Code", "EX%");
            var newPlanList = new List<MrpExSectionPlan>();
            foreach (var groupPlan in groupExSectionPlans1)
            {
                foreach (var plan in groupPlan.List)
                {
                    plan.LeftShiftQty = plan.ShiftQty;
                    plan.LeftSwitchTime = plan.CurrentSwitchTime;
                }
                for (int i = 0; i < shiftMasterList.Count; i++)
                {
                    var shift = shiftMasterList[i];
                    DateTime startTime = planDate;
                    DateTime windowTime = planDate;
                    workingCalendarMgr.GetStartTimeAndWindowTime(shift.Code, planDate, out startTime, out windowTime);

                    double shiftQty = 0;
                    foreach (var plan in groupPlan.List)
                    {
                        if (plan.LeftShiftQty > 0)
                        {
                            var newPlan = Mapper.Map<MrpExSectionPlan, MrpExSectionPlan>(plan);
                            newPlan.NewId = plan.Id;
                            int shiftType = (int)plan.ShiftType;
                            shiftType = shiftType == 0 ? 3 : shiftType;
                            newPlan.ShiftType = (CodeMaster.ShiftType)shiftType;
                            newPlan.StartTime = startTime.AddHours(shiftQty * (24 / shiftType));

                            if (plan.LeftShiftQty + shiftQty > 1)
                            {
                                newPlan.ShiftQty = 1 - shiftQty;
                                shiftQty = 1;
                                plan.LeftShiftQty -= newPlan.ShiftQty;
                            }
                            else
                            {
                                newPlan.ShiftQty = plan.LeftShiftQty;
                                shiftQty += newPlan.ShiftQty;
                                plan.LeftShiftQty = 0;
                            }

                            if (plan.LeftSwitchTime < newPlan.ShiftQty * (24 / shiftType) * 60)
                            {
                                newPlan.CurrentSwitchTime = plan.LeftSwitchTime;
                                plan.LeftSwitchTime = 0;
                            }
                            else
                            {
                                newPlan.CurrentSwitchTime = newPlan.ShiftQty * (24 / shiftType) * 60;
                                plan.LeftSwitchTime -= plan.CurrentSwitchTime;
                            }

                            newPlan.WindowTime = startTime.AddHours(shiftQty * (24 / shiftType));
                            plan.Shift = shift.Code;
                            plan.ShiftDateFrom = startTime;
                            plan.ShiftDateTo = windowTime;

                            newPlan.Shift = shift.Code;
                            newPlan.ShiftDateFrom = startTime;
                            newPlan.ShiftDateTo = windowTime;
                            newPlanList.Add(newPlan);
                            //排满了
                            if (shiftQty == 1)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            return newPlanList;
        }


        //废弃
        private Dictionary<string, List<MrpExPlan>> GetMrpExPlanDic(string dateIndex, DateTime planVersion)
        {
            //只重排尚未开始的班次
            var dateTimeNow = DateTime.Now;
            var planMasterList = this.genericMgr.FindAll<MrpExPlanMaster>
                   ("from MrpExPlanMaster where DateIndex =? and IsActive=? and PlanDate<=? ",
                   new object[] { dateIndex, true, dateTimeNow.Date });

            var paramList = new List<object>();
            string hql = @"from MrpExShiftPlan where 1=1  ";
            string inprocessShift = string.Empty;
            foreach (var planMaster in planMasterList)
            {
                if (planMaster.PlanDate == dateTimeNow.Date)
                {
                    DateTime startTime = planMaster.PlanDate;
                    DateTime windowTime = planMaster.PlanDate;
                    workingCalendarMgr.GetStartTimeAndWindowTime(planMaster.Shift, planMaster.PlanDate, out startTime, out windowTime);
                    if (startTime > dateTimeNow)
                    {
                        //尚未开始
                        continue;
                    }
                    else if (startTime <= dateTimeNow && windowTime > dateTimeNow)
                    {
                        inprocessShift = planMaster.Shift;
                    }
                }
                hql += " and  ProductLine = ? and ReleaseVersion =? and Shift=? ";
                paramList.Add(planMaster.ProductLine);
                paramList.Add(planMaster.ReleaseVersion);
                paramList.Add(planMaster.Shift);
            }
            //正在执行中的班次取Qty,其他的取ReceivedQty
            var shiftQtyDic = genericMgr.FindAll<MrpExShiftPlan>(hql, paramList.ToArray())
                .GroupBy(p => p.Item, (k, g) => new
                {
                    k,
                    Qty = g.Sum(p => (p.Shift == inprocessShift && p.StartTime.Date == dateTimeNow.Date ? p.Qty : p.ReceivedQty))
                }).ToDictionary(d => d.k, d => d.Qty);

            var mrpExPlanList = this.genericMgr.FindAll<MrpExPlan>(
                " from MrpExPlan where PlanVersion=? and DateIndex =? ", new object[] { planVersion, dateIndex });
            foreach (var mrpExPlan in mrpExPlanList)
            {
                mrpExPlan.UsedQty = shiftQtyDic.ValueOrDefault(mrpExPlan.Item);
            }

            var mrpExPlanDic = mrpExPlanList.Where(p => !string.IsNullOrWhiteSpace(p.Section))
                .GroupBy(p => p.Section, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.ToList());
            return mrpExPlanDic;
        }

        //废弃
        [Transaction(TransactionMode.Requires)]
        public void AdjustMrpExShiftPlan(IList<MrpExShiftPlan> mrpExShiftPlanList)
        {
            if (mrpExShiftPlanList.Select(p => p.ProductLine).Distinct().Count() > 1)
            {
                throw new BusinessException("不支持多条生产线同时调整");
            }
            if (mrpExShiftPlanList.Select(p => p.DateIndex).Distinct().Count() > 1)
            {
                throw new BusinessException("不支持多周同时调整");
            }
            //一个断面一个断面的排
            var groupMrpExShiftPlanListBySection = mrpExShiftPlanList.GroupBy(p => p.Section, (k, g) => new { Section = k, List = g });
            foreach (var groupMrpExShiftPlan in groupMrpExShiftPlanListBySection)
            {
                foreach (var shiftPlan in groupMrpExShiftPlan.List)
                {
                    if (shiftPlan.Id == 0)
                    {
                        shiftPlan.PlanNo = shiftPlan.DateIndex.Substring(2, 2) + shiftPlan.DateIndex.Substring(5, 2) + "N" + shiftPlan.ProductLine + shiftPlan.Section;

                        var oldShiftPlan = mrpExShiftPlanList.OrderBy(p => p.Sequence)
                            .Where(p => p.Id > 0 && p.Sequence < shiftPlan.Sequence).LastOrDefault();

                        var prodLineEx = this.genericMgr.FindAll<ProdLineExInstance>
                            (" from ProdLineExInstance where ProductLine =? and Item =? and DateIndex=? and DateType=? ",
                            new object[] { shiftPlan.ProductLine, shiftPlan.Section, shiftPlan.DateIndex, CodeMaster.TimeUnit.Week })
                            .FirstOrDefault();
                        if (shiftPlan.Item == BusinessConstants.VIRTUALSECTION)
                        {
                            prodLineEx = LoadVirtualProdLineExInstance();
                        }
                        else if (prodLineEx == null)
                        {
                            throw new BusinessException("没有找到断面{0}对应的挤出资源", shiftPlan.Section);
                        }

                        if (oldShiftPlan == null)
                        {
                            oldShiftPlan = mrpExShiftPlanList.OrderBy(p => p.Sequence)
                                .Where(p => p.Id > 0 && p.Sequence > shiftPlan.Sequence).FirstOrDefault();

                            if (oldShiftPlan != null)
                            {
                                shiftPlan.StartTime = oldShiftPlan.StartTime;
                                shiftPlan.WindowTime = shiftPlan.StartTime.AddMinutes
                                    (shiftPlan.Qty * shiftPlan.RateQty / prodLineEx.SpeedTimes / prodLineEx.MrpSpeed);
                                shiftPlan.PlanStartTime = shiftPlan.StartTime;
                                shiftPlan.PlanWindowTime = shiftPlan.WindowTime;
                            }
                        }
                        else
                        {
                            shiftPlan.StartTime = oldShiftPlan.WindowTime;
                            shiftPlan.WindowTime = shiftPlan.StartTime.AddMinutes
                                (shiftPlan.Qty * shiftPlan.RateQty / prodLineEx.SpeedTimes / prodLineEx.MrpSpeed);
                            shiftPlan.PlanStartTime = shiftPlan.StartTime;
                            shiftPlan.PlanWindowTime = shiftPlan.WindowTime;
                        }
                        oldShiftPlan.Speed = prodLineEx.MrpSpeed * prodLineEx.SpeedTimes;
                        oldShiftPlan.SwitchTime = prodLineEx.SwitchTime;
                        this.genericMgr.Create(shiftPlan);

                        var mrpExOrders = this.genericMgr.FindAll<MrpExOrder>(" from MrpExOrder where PlanNo=? ", shiftPlan.PlanNo);
                        if (mrpExOrders == null || mrpExOrders.Count == 0)
                        {
                            var mrpExOrder = new MrpExOrder();
                            mrpExOrder.DateIndex = shiftPlan.DateIndex;
                            mrpExOrder.PlanNo = shiftPlan.PlanNo;
                            mrpExOrder.PlanVersion = shiftPlan.PlanVersion;
                            mrpExOrder.ProductLine = shiftPlan.ProductLine;
                            mrpExOrder.Section = shiftPlan.Section;
                            //mrpExOrder.StartDate = shiftPlan.StartDate
                            mrpExOrder.StartTime = shiftPlan.StartTime;
                            mrpExOrder.Status = CodeMaster.OrderStatus.Create;
                            mrpExOrder.WindowTime = shiftPlan.WindowTime;
                            this.genericMgr.Create(mrpExOrder);
                        }
                    }
                    else
                    {
                        this.genericMgr.Update(shiftPlan);
                    }
                }
            }
        }


        public ProdLineExInstance LoadVirtualProdLineExInstance(double qty = 1.0)
        {
            var prodLineExInstance = new ProdLineExInstance();
            prodLineExInstance.ApsPriority = CodeMaster.ApsPriorityType.Backup;
            prodLineExInstance.Correction = 1.2;
            prodLineExInstance.EconomicLotSize = qty;
            prodLineExInstance.IsManualCreate = true;
            prodLineExInstance.IsRelease = true;
            prodLineExInstance.Item = BusinessConstants.VIRTUALSECTION;
            prodLineExInstance.MaxLotSize = qty;
            prodLineExInstance.MinLotSize = qty;
            prodLineExInstance.MrpSpeed = 1;
            //prodLineExInstance.ProductLine =
            //CodeMaster.ProductType.C
            prodLineExInstance.ProductType = "C";
            prodLineExInstance.Quota = 100;
            prodLineExInstance.RccpSpeed = 1;
            //prodLineExInstance.Region
            prodLineExInstance.ShiftType = CodeMaster.ShiftType.ThreeShiftPerDay;
            prodLineExInstance.SpeedTimes = 1;
            prodLineExInstance.SwitchTime = 0;
            prodLineExInstance.TurnQty = 1;
            return prodLineExInstance;
        }

        [Transaction(TransactionMode.Requires)]
        public void AdjustMrpExShiftPlanWorkingCalendar(DateTime planDate, string flow)
        {
            if (string.IsNullOrEmpty(flow))
            {
                throw new BusinessException("生产线不能为空");
            }

            var shiftPlanList = GetMrpExShiftPlanList(planDate, flow);
            var groupPlanList = shiftPlanList.GroupBy(p => p.Shift, (k, g) => new { k, List = g.ToList() }).ToList();
            foreach (var groupPlan in groupPlanList)
            {
                AdjustMrpExShiftPlanWorkingCalendar(groupPlan.List);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void AdjustMrpExShiftPlanWorkingCalendar(List<MrpExShiftPlan> mrpExShiftPlanList)
        {
            var count = mrpExShiftPlanList.GroupBy(p => new { p.ProductLine, p.Shift, p.PlanDate }, (k, g) => new { k }).Count();
            if (count > 1)
            {
                throw new BusinessException("不能跨班调整");
            }

            #region //更新换模时间
            var groupMrpExShiftPlanList = mrpExShiftPlanList.GroupBy(p => new { p.Shift, p.PlanDate, p.Section });
            foreach (var plans in groupMrpExShiftPlanList)
            {
                var minSeqPlan = plans.Where(p => p.Qty > 0).OrderBy(p => p.Sequence).FirstOrDefault();
                if (minSeqPlan == null)
                {
                    minSeqPlan = plans.OrderBy(p => p.Sequence).FirstOrDefault();
                }
                var switchTime = plans.Sum(p => p.SwitchTime);
                foreach (var plan in plans)
                {
                    plan.SwitchTime = 0;
                }
                minSeqPlan.SwitchTime = switchTime;
            }
            #endregion
            //更新班产计划的开始和结束时间
            var lastShiftPlan = mrpExShiftPlanList.Last();
            DateTime planDate = lastShiftPlan.PlanDate;
            int sequence = 10;
            DateTime startTime = planDate;
            DateTime windowTime = planDate;
            workingCalendarMgr.GetStartTimeAndWindowTime(lastShiftPlan.Shift, planDate, out startTime, out windowTime);
            DateTime dateFrom = startTime;

            double shiftQty = 0;
            //正在执行的班次
            foreach (var shiftPlan in mrpExShiftPlanList.OrderBy(p => p.Sequence))
            {
                if (shiftPlan.ReceivedQty < shiftPlan.Qty || shiftPlan.Qty == 0)
                {
                    shiftQty += shiftPlan.ShiftQty;
                    if (shiftQty > 1)
                    {
                        shiftPlan.ShiftQty = shiftPlan.ShiftQty - (shiftQty - 1);
                        shiftQty = 1;
                    }
                    shiftPlan.Sequence = sequence;
                    shiftPlan.StartTime = dateFrom;
                    //需要工作的分钟数
                    dateFrom = dateFrom.AddHours(shiftPlan.ShiftQty * (24 / shiftPlan.ShiftType));
                    shiftPlan.WindowTime = dateFrom;
                    if (Math.Round(shiftPlan.ShiftQty, 2) == 0)
                    {
                        shiftPlan.Qty = 0;
                        shiftPlan.ShiftQty = 0;
                    }
                }
                shiftPlan.Sequence = sequence;
                sequence += 10;
                shiftPlan.CalShiftQty = (shiftPlan.Qty * shiftPlan.RateQty / shiftPlan.Speed + shiftPlan.SwitchTime) / 60 / (24 / shiftPlan.ShiftType);

                this.genericMgr.Update(shiftPlan);
            }
            //一个班排不满则,用当前班别的最后一个班产计划的半制品物料进行插单
            if (dateFrom < windowTime)
            {
                var totalHours = (windowTime - dateFrom).TotalHours;
                var lastPlan = mrpExShiftPlanList.Where(p => p.Qty > 0).Last();
                var lastHours = (lastPlan.CalShiftQty - lastPlan.ShiftQty) * (24 / lastPlan.ShiftType);
                if (lastHours > 0)
                {
                    lastHours = totalHours < lastHours ? totalHours : lastHours;
                    dateFrom = dateFrom.AddHours(lastHours);
                    lastPlan.WindowTime = dateFrom;
                    lastPlan.ShiftQty = (dateFrom - lastPlan.StartTime).TotalHours / (24 / lastPlan.ShiftType);
                    //lastPlan.Qty += Math.Round(lastHours * 60 * lastPlan.Speed / lastPlan.RateQty);
                }
                else
                {
                    lastHours = 0;
                }
                var newHours = totalHours - lastHours;
                if (newHours > 0)
                {
                    var newPlan = Mapper.Map<MrpExShiftPlan, MrpExShiftPlan>(lastPlan);
                    newPlan.StartTime = dateFrom;
                    newPlan.WindowTime = windowTime;
                    newPlan.ShiftQty = newHours / (24 / newPlan.ShiftType);
                    newPlan.Qty = Math.Round(newHours * 60 * newPlan.Speed / newPlan.RateQty / newPlan.UnitCount) * newPlan.UnitCount;
                    newPlan.IsNew = true;
                    newPlan.SwitchTime = 0;
                    newPlan.Sequence += 10;
                    if (newPlan.Qty > 0)
                    {
                        this.genericMgr.Create(newPlan);
                    }
                }
            }
            return;
            ////以下废弃
            //if (dateFrom < windowTime)
            //{
            //    var nextShift = string.Empty;
            //    var nextPlanDate = planDate;
            //    var shiftMasterList = this.genericMgr.FindAll<ShiftMaster>("from ShiftMaster where Code like ? order by Code", "EX%");
            //    for (int i = 0; i < shiftMasterList.Count; i++)
            //    {
            //        if (shiftMasterList[i].Code == lastShiftPlan.Shift)
            //        {
            //            if (i == shiftMasterList.Count - 1)
            //            {
            //                nextShift = shiftMasterList[0].Code;
            //                nextPlanDate = planDate.AddDays(1);
            //            }
            //            else
            //            {
            //                nextShift = shiftMasterList[i + 1].Code;
            //            }
            //        }
            //    }
            //    var nextPlanList = GetMrpExShiftPlanList(nextPlanDate, lastShiftPlan.ProductLine, nextShift);
            //    if (nextPlanList != null && nextPlanList.First().Section == lastShiftPlan.Section)
            //    {
            //        //如果下个班接着做,移过来
            //        foreach (var shiftPlan in nextPlanList)
            //        {
            //            if (dateFrom > windowTime)
            //            {
            //                break;
            //            }
            //            if (shiftPlan.Section != lastShiftPlan.Section)
            //            {
            //                break;
            //            }
            //            var newPlan = Mapper.Map<MrpExShiftPlan, MrpExShiftPlan>(shiftPlan);
            //            double workMinites = newPlan.Qty * newPlan.RateQty / newPlan.Speed;
            //            newPlan.Sequence = sequence;
            //            newPlan.StartTime = dateFrom;
            //            //需要工作的分钟数
            //            dateFrom = dateFrom.AddMinutes(workMinites);
            //            newPlan.WindowTime = dateFrom;
            //            newPlan.PlanVersion = lastShiftPlan.PlanVersion;
            //            newPlan.PlanNo = lastShiftPlan.PlanNo;
            //            newPlan.DateIndex = lastShiftPlan.DateIndex;
            //            newPlan.ReceivedQty = 0;
            //            newPlan.Shift = lastShiftPlan.Shift;
            //            newPlan.ShiftQty = (workMinites / 60) / (24 / newPlan.ShiftType);
            //            this.genericMgr.Create(newPlan);
            //            sequence += 10;
            //        }
            //    }
            //    else
            //    {
            //        var newPlanList = new List<MrpExShiftPlan>();
            //        //不接着做,重复本班的
            //        DateTime _dateFrom = dateFrom;
            //        foreach (var shiftPlan in mrpExShiftPlanList.Reverse<MrpExShiftPlan>())
            //        {
            //            if (_dateFrom > windowTime)
            //            {
            //                break;
            //            }
            //            if (shiftPlan.Section != lastShiftPlan.Section)
            //            {
            //                break;
            //            }
            //            double workHours = shiftPlan.ShiftQty * (24 / shiftPlan.ShiftType);
            //            var newPlan = Mapper.Map<MrpExShiftPlan, MrpExShiftPlan>(shiftPlan);
            //            newPlanList.Add(newPlan);
            //            _dateFrom = dateFrom.AddHours(workHours);
            //        }

            //        foreach (var shiftPlan in newPlanList.Reverse<MrpExShiftPlan>())
            //        {
            //            //double workMinites = shiftPlan.Qty * shiftPlan.RateQty / shiftPlan.Speed;
            //            shiftPlan.StartTime = dateFrom;
            //            dateFrom = dateFrom.AddHours(shiftPlan.ShiftQty * (24 / shiftPlan.ShiftType));
            //            shiftPlan.WindowTime = dateFrom;
            //            shiftPlan.Sequence = sequence;
            //            shiftPlan.IsNew = true;
            //            sequence += 10;
            //            this.genericMgr.Create(shiftPlan);
            //        }
            //        var newPlanListFirst = newPlanList.First();
            //        if (newPlanListFirst.WindowTime < windowTime)
            //        {
            //            newPlanListFirst.WindowTime = windowTime;
            //            newPlanListFirst.ShiftQty = (newPlanListFirst.WindowTime - newPlanListFirst.StartTime).TotalHours / (24 / newPlanListFirst.ShiftType);
            //            if (newPlanListFirst.Item != BusinessConstants.VIRTUALSECTION)
            //            {
            //                newPlanListFirst.Qty = Math.Ceiling(newPlanListFirst.ShiftQty * (24 / newPlanListFirst.ShiftType) * 60 * newPlanListFirst.Speed / newPlanListFirst.RateQty);
            //            }
            //        }
            //    }
            //}
        }

        [Transaction(TransactionMode.Requires)]
        public List<MrpExShiftPlan> GetMrpExShiftPlanList(DateTime planDate, string flow, string shift = null)
        {
            var oldMrpExPlanMasterList = this.genericMgr.FindAll<MrpExPlanMaster>
                 ("from MrpExPlanMaster where PlanDate =? and ProductLine=? and IsActive=? ",
                 new object[] { planDate, flow, true });
            if (oldMrpExPlanMasterList == null || oldMrpExPlanMasterList.Count == 0)
            {
                return null;
                //throw new BusinessException("没有找到释放的生产单");
            }

            var shiftPlanList = new List<MrpExShiftPlan>();
            foreach (var planMaster in oldMrpExPlanMasterList)
            {
                if (string.IsNullOrWhiteSpace(shift) || shift == planMaster.Shift)
                {
                    string hql = @"from MrpExShiftPlan s where s.ProductLine = ? and s.ReleaseVersion =? and Shift=? order by s.Shift,s.Sequence ";
                    var paramList = new List<object> { flow, planMaster.ReleaseVersion, planMaster.Shift };
                    var shiftPlans = genericMgr.FindAll<MrpExShiftPlan>(hql, paramList.ToArray());
                    shiftPlanList.AddRange(shiftPlans);
                }
            }

            foreach (var shiftPlan in shiftPlanList)
            {
                if (shiftPlan.Item != BusinessConstants.VIRTUALSECTION)
                {
                    shiftPlan.CalShiftQty = (shiftPlan.Qty * shiftPlan.RateQty / shiftPlan.Speed + shiftPlan.SwitchTime) / 60 / (24 / shiftPlan.ShiftType);
                    shiftPlan.CalShiftQty = Math.Round(shiftPlan.CalShiftQty, 2);
                }
                else
                {
                    shiftPlan.CalShiftQty = shiftPlan.ShiftQty;
                }
            }
            return shiftPlanList.ToList();
        }

        //废弃 按生产线分配工作日历,调整开始/窗口时间
        [Transaction(TransactionMode.Requires)]
        public void AdjustMrpExShiftPlanWorkingCalendar(string dateIndex, string flow)
        {
            var oldMrpExDateIndexList = this.genericMgr.FindAll<MrpExDateIndex>
                ("from MrpExDateIndex where DateIndex =? and IsActive=? ", new object[] { dateIndex, true });
            if (oldMrpExDateIndexList == null || oldMrpExDateIndexList.Count == 0)
            {
                throw new BusinessException("没有找到释放的生产单");
            }
            else
            {
                if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(dateIndex))
                {
                    DateTime dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                    dateFrom = dateFrom < DateTime.Now ? DateTime.Now : dateFrom;
                    var planVersion = oldMrpExDateIndexList.First().PlanVersion;
                    string hql = "from MrpExShiftPlan where ProductLine = ? and DateIndex =? and PlanVersion = ? order by Sequence";
                    List<object> param = new List<object> { flow, dateIndex, planVersion };
                    var shiftPlanList = genericMgr.FindAll<MrpExShiftPlan>(hql, param.ToArray());

                    var exOrderDic = this.genericMgr.FindAllIn<MrpExOrder>
                        (" from MrpExOrder where PlanNo in(?", shiftPlanList.Select(p => p.PlanNo).Distinct())
                        .ToDictionary(d => d.PlanNo, d => d);
                    foreach (var shiftPlan in shiftPlanList)
                    {
                        shiftPlan.MrpExOrder = exOrderDic[shiftPlan.PlanNo];
                    }

                    //只有是创建状态的订单明细需要重新计算时间
                    var createShiftPlanList = shiftPlanList.Where(p => p.MrpExOrder.Status == CodeMaster.OrderStatus.Create).ToList();
                    var groupShiftPlansByProductLine = createShiftPlanList.GroupBy(p => p.ProductLine, (k, g) => new { Flow = k, List = g });
                    foreach (var planByProductLine in groupShiftPlansByProductLine)
                    {
                        int sequence = 10;
                        var regionCode = this.genericMgr.FindById<FlowMaster>(planByProductLine.Flow).PartyFrom;
                        #region 推算出DateFrom
                        //找出当前正在生产的断面
                        var inProcessMrpExOrder = exOrderDic.Select(p => p.Value).FirstOrDefault(p => p.Status == CodeMaster.OrderStatus.InProcess);
                        if (inProcessMrpExOrder != null)
                        {
                            //当前断面还需要这么久才能完成
                            var totalMinutes = shiftPlanList.Where(p => p.PlanNo == inProcessMrpExOrder.PlanNo
                                && (p.Qty - p.ReceivedQty) > 0).Select(p => (p.WindowTime - p.StartTime).TotalMinutes).Sum();
                            dateFrom = DateTime.Now.AddMinutes(totalMinutes);
                            var lastInProcessPlan = shiftPlanList.Where(p => p.PlanNo == inProcessMrpExOrder.PlanNo
                                 && (p.Qty - p.ReceivedQty) > 0).OrderBy(p => p.Sequence).LastOrDefault();
                            if (lastInProcessPlan != null)
                            {
                                sequence = lastInProcessPlan.Sequence + 10;
                            }
                        }
                        #endregion
                        var workingCalendarViews = this.workingCalendarMgr.GetWorkingCalendarViewList(regionCode, planByProductLine.Flow, dateFrom.AddDays(-4), dateFrom.AddDays(14));
                        dateFrom = this.workingCalendarMgr.GetStartTimeAtWorkingDate(dateFrom, regionCode, planByProductLine.Flow, workingCalendarViews);
                        var currentCalender = workingCalendarViews.FirstOrDefault(p => p.DateFrom <= dateFrom && p.DateTo > dateFrom);
                        var workingCalendarViewArray = workingCalendarViews.Where(p => p.Type == CodeMaster.WorkingCalendarType.Work).ToArray();
                        int workIndex = Array.IndexOf<WorkingCalendarView>(workingCalendarViewArray, currentCalender);

                        var currentSection = string.Empty;
                        foreach (var shiftPlan in planByProductLine.List)
                        {
                            shiftPlan.Sequence = sequence;

                            if (workIndex < workingCalendarViewArray.Length)
                            {
                                currentCalender = workingCalendarViewArray[workIndex];
                                shiftPlan.StartTime = dateFrom;
                                shiftPlan.Shift = currentCalender.ShiftCode;
                                //需要工作的分钟数
                                double workMinites = shiftPlan.Qty * shiftPlan.RateQty / shiftPlan.Speed;
                                if (currentSection != shiftPlan.Section)
                                {
                                    workMinites += shiftPlan.SwitchTime;
                                    currentSection = shiftPlan.Section;
                                }
                                //当前工作日历的可用的分钟数
                                double calenderMinites = (currentCalender.DateTo - dateFrom).TotalMinutes;
                                if (calenderMinites >= workMinites)
                                {
                                    dateFrom = dateFrom.AddMinutes(workMinites);
                                }
                                else
                                {
                                    workMinites = workMinites - calenderMinites;
                                    dateFrom = currentCalender.DateTo;
                                    GetExPlanWindowTime(workingCalendarViewArray, workMinites, ref dateFrom, ref workIndex);
                                }
                                shiftPlan.WindowTime = dateFrom;
                            }
                            sequence += 10;
                        }
                    }
                }
                else
                {
                    throw new BusinessException("生产线和断面都不能为空");
                }
            }
        }

        private void GetExPlanWindowTime(WorkingCalendarView[] workingCalendarViewArray, double workMinites, ref DateTime dateFrom, ref int workIndex)
        {
            workIndex++;
            if (workIndex < workingCalendarViewArray.Length)
            {
                var currentCalender = workingCalendarViewArray[workIndex];
                double calenderMinites = (currentCalender.DateTo - currentCalender.DateFrom).TotalMinutes;
                if (calenderMinites >= workMinites)
                {
                    dateFrom = dateFrom.AddMinutes(workMinites);
                }
                else
                {
                    //继续换班
                    workMinites = workMinites - calenderMinites;
                    dateFrom = currentCalender.DateTo;
                    GetExPlanWindowTime(workingCalendarViewArray, workMinites, ref dateFrom, ref workIndex);
                }
            }
        }

        private List<MrpExShiftPlan> GetExShiftPlan(MrpExSectionPlan mrpExSectionPlan, MrpExItemPlan mrpExPlan, double qty, DateTime startTime,
            double requireTime, Dictionary<string, MrpFlowDetail> mrpFlowDetailDic, bool isFreeze, double switchTime = 0)
        {
            var mrpExShiftPlanList = new List<MrpExShiftPlan>();

            var totalRequireTime = requireTime + switchTime;
            var item = itemMgr.GetCacheItem(mrpExPlan.Item);
            var mrpExShiftPlan = new MrpExShiftPlan();
            mrpExShiftPlan.PlanVersion = mrpExPlan.PlanVersion;
            mrpExShiftPlan.PlanNo = mrpExSectionPlan.PlanNo;
            mrpExShiftPlan.ItemId = mrpExPlan.Id;
            mrpExShiftPlan.ProductLine = mrpExSectionPlan.ProductLine;
            mrpExShiftPlan.DateIndex = mrpExPlan.DateIndex;
            //mrpExShiftPlan.Sequence = sequence;
            mrpExShiftPlan.Item = mrpExPlan.Item;
            mrpExShiftPlan.ItemDescription = item.Description;

            mrpExShiftPlan.TurnQty = mrpExSectionPlan.TurnQty;
            mrpExShiftPlan.Qty = qty;
            mrpExShiftPlan.UnitCount = mrpExPlan.UnitCount;
            mrpExShiftPlan.Uom = item.Uom;
            mrpExShiftPlan.RateQty = mrpExPlan.RateQty;
            mrpExShiftPlan.StartTime = startTime;
            mrpExShiftPlan.WindowTime = startTime.AddMinutes(totalRequireTime);
            mrpExShiftPlan.Section = mrpExPlan.Section;
            //mrpExShiftPlan.IsCorrection = mrpExPlan.CurrentQty < (mrpExPlan.AdjustQty + mrpExPlan.CorrectionQty);
            mrpExShiftPlan.IsNew = false;
            mrpExShiftPlan.Remark = mrpExSectionPlan.Remark;
            mrpExShiftPlan.PlanStartTime = mrpExShiftPlan.StartTime;
            mrpExShiftPlan.PlanWindowTime = mrpExShiftPlan.WindowTime;
            mrpExShiftPlan.Speed = mrpExSectionPlan.Speed * mrpExSectionPlan.SpeedTimes;
            mrpExShiftPlan.SwitchTime = switchTime;
            var flowDetial = mrpFlowDetailDic.ValueOrDefault(mrpExPlan.Item);
            if (flowDetial == null)
            {
                logRunMrp.Warn(string.Format("在物流路线中{0}没有找到物料{1},CreaterExShiftPlanDetail", mrpExSectionPlan.ProductLine, mrpExPlan.Item));
                flowDetial = mrpFlowDetailDic.First().Value;
                mrpExShiftPlan.Bom = mrpExPlan.Item;
            }
            else
            {
                mrpExShiftPlan.Bom = flowDetial.Bom;
                //mrpExShiftPlan.UnitCount = flowDetial.UnitCount;
            }
            if (flowDetial != null)
            {
                mrpExShiftPlan.LocationFrom = flowDetial.LocationFrom;
                mrpExShiftPlan.LocationTo = flowDetial.LocationTo;
            }
            else
            {
                var flowMaster = this.genericMgr.FindById<FlowMaster>(mrpExShiftPlan.ProductLine);
                mrpExShiftPlan.LocationFrom = flowMaster.LocationFrom;
                mrpExShiftPlan.LocationTo = flowMaster.LocationTo;
            }
            mrpExShiftPlan.Shift = mrpExSectionPlan.Shift;
            mrpExShiftPlan.ShiftType = (int)mrpExSectionPlan.ShiftType;
            mrpExShiftPlan.ShiftQty = (totalRequireTime / 60) / (24 / mrpExShiftPlan.ShiftType);
            mrpExShiftPlan.PlanDate = mrpExSectionPlan.PlanDate;
            mrpExShiftPlan.IsFreeze = isFreeze;
            mrpExShiftPlan.ProductType = mrpExSectionPlan.ProductType;

            var correctQty = mrpExPlan.AdjustQty + mrpExPlan.CorrectionQty - mrpExPlan.CurrentQty;
            if (correctQty > 0 && false)
            {
                if (mrpExShiftPlan.Qty > correctQty)
                {
                    var windowTime = mrpExShiftPlan.WindowTime;
                    var _qty = mrpExShiftPlan.Qty - correctQty;
                    _qty = Math.Ceiling(Math.Round(_qty / mrpExShiftPlan.UnitCount, 2)) * mrpExShiftPlan.UnitCount;
                    mrpExShiftPlan.Qty = _qty;
                    var min = mrpExShiftPlan.Qty * mrpExShiftPlan.RateQty / mrpExShiftPlan.Speed + mrpExShiftPlan.SwitchTime;
                    mrpExShiftPlan.IsCorrection = false;
                    mrpExShiftPlan.WindowTime = startTime.AddMinutes(min);
                    mrpExShiftPlan.ShiftQty = (min / 60) / (24 / mrpExShiftPlan.ShiftType);
                    correctQty = qty - _qty;
                    if (correctQty > 0)
                    {
                        var _newPlan = Mapper.Map<MrpExShiftPlan, MrpExShiftPlan>(mrpExShiftPlan);
                        _newPlan.IsCorrection = true;
                        _newPlan.StartTime = mrpExShiftPlan.WindowTime;
                        _newPlan.WindowTime = windowTime;
                        _newPlan.Qty = correctQty;
                        _newPlan.ShiftQty = ((_newPlan.WindowTime - _newPlan.StartTime).TotalMinutes / 60) / (24 / mrpExShiftPlan.ShiftType);
                        _newPlan.SwitchTime = 0;
                        mrpExShiftPlanList.Add(_newPlan);
                        //mrpExShiftPlanList.Add(mrpExShiftPlan);
                    }
                }
                mrpExShiftPlanList.Add(mrpExShiftPlan);
            }
            else
            {
                mrpExShiftPlanList.Add(mrpExShiftPlan);
            }

            return mrpExShiftPlanList;
        }
        #endregion

        #endregion

        #region ScheduleMi
        private void ScheduleMi(List<MrpShipPlan> mrpShipPlanList, IList<MrpFlowDetail> mrpFlowDetailList, Dictionary<string, double> inventoryBalanceDic,
            IList<ItemDiscontinue> itemDiscontinueList, DateTime snapTime, DateTime newPlanVersion, BusinessException businessException)
        {
            var test0 = mrpShipPlanList.Where(p => p.Item == "270018").ToList();
            var miFlowDetials = mrpFlowDetailList.Where(f => f.ResourceGroup == CodeMaster.ResourceGroup.MI && f.MrpWeight > 0);
            var miFlowCodes = miFlowDetials.Select(t => t.Flow).Distinct();

            mrpShipPlanList = mrpShipPlanList
                .Where(p => miFlowCodes.Contains(p.Flow) && p.Qty != 0 && p.StartTime >= newPlanVersion.Date
                && p.SourceType != CodeMaster.MrpSourceType.Order && !miFlowCodes.Contains(p.SourceFlow))//订单需求最后满足
                .ToList();
            var newShipPlanList = Mapper.Map<List<MrpShipPlan>, List<MrpShipPlan>>(mrpShipPlanList);

            //根据工时排班

            #region 汇总ShipPlan 圆整到天
            logRunMrp.Info(string.Format("-{0} - Mi汇总ShipPlan-", newPlanVersion));
            var shipPlanGroupList = (from p in newShipPlanList
                                     orderby p.WindowTime
                                     group p by new
                                     {
                                         WindowTime = p.WindowTime.Date,
                                         StartTime = p.StartTime.Date,
                                         Flow = p.Flow,
                                         Item = p.Item,
                                         LocationFrom = p.LocationFrom,
                                         LocationTo = p.LocationTo,
                                         OrderType = p.OrderType,
                                         ParentItem = p.ParentItem,
                                         SourceFlow = p.SourceFlow,
                                         SourceParty = p.SourceParty
                                     } into g
                                     select new MrpShipPlanGroup
                                     {
                                         WindowTime = g.Key.WindowTime,
                                         StartTime = g.Key.StartTime,
                                         Flow = g.Key.Flow,
                                         Item = g.Key.Item,
                                         LocationFrom = g.Key.LocationFrom,
                                         LocationTo = g.Key.LocationTo,
                                         OrderType = g.Key.OrderType,
                                         ParentItem = g.Key.ParentItem,
                                         SourceFlow = g.Key.SourceFlow,
                                         SourceParty = g.Key.SourceParty,
                                         Qty = g.Sum(t => t.Qty),
                                         PlanVersion = newPlanVersion,
                                     }).ToList();
            #endregion

            #region 替代物料
            var groupDisShipPlans = shipPlanGroupList.Where(p => itemDiscontinueList.Select(q => q.DiscontinueItem).Distinct().Contains(p.Item));
            foreach (var groupDisShipPlan in groupDisShipPlans)
            {
                var qty = groupDisShipPlan.Qty;
                groupDisShipPlan.IsDiscontinueItem = true;
                var disItem = itemDiscontinueList.Where(q => q.DiscontinueItem == groupDisShipPlan.Item).First();
                var newShipPlanGroup = new MrpShipPlanGroup();
                newShipPlanGroup.WindowTime = groupDisShipPlan.WindowTime;
                newShipPlanGroup.StartTime = groupDisShipPlan.StartTime;
                newShipPlanGroup.Flow = groupDisShipPlan.Flow;
                newShipPlanGroup.Item = disItem.Item;
                newShipPlanGroup.LocationFrom = groupDisShipPlan.LocationFrom;
                newShipPlanGroup.LocationTo = groupDisShipPlan.LocationTo;
                newShipPlanGroup.OrderType = groupDisShipPlan.OrderType;
                newShipPlanGroup.ParentItem = groupDisShipPlan.ParentItem;
                newShipPlanGroup.SourceFlow = groupDisShipPlan.SourceFlow;
                newShipPlanGroup.SourceParty = groupDisShipPlan.SourceParty;
                newShipPlanGroup.Qty = groupDisShipPlan.Qty;
                newShipPlanGroup.PlanVersion = newPlanVersion;
                newShipPlanGroup.IsDiscontinueItem = false;
                shipPlanGroupList.Add(newShipPlanGroup);
            }
            #endregion

            var test = shipPlanGroupList.Where(p => p.Item == "270018").ToList();

            #region 对象转换
            var huToMappings = this.genericMgr.FindAll<HuToMapping>();

            var mrpMiPlanList = (from q in shipPlanGroupList
                                     .Where(p => p.IsDiscontinueItem == false)
                                     .OrderBy(p => p.Flow).ThenBy(p => p.StartTime)
                                 join f in miFlowDetials on new { Flow = q.Flow, q.Item } equals new { f.Flow, f.Item } into gj
                                 from r in gj.DefaultIfEmpty()
                                 where r != null
                                 select new MrpMiPlan
                                 {
                                     ProductLine = q.Flow,
                                     Item = q.Item,
                                     PlanDate = q.StartTime.Date,
                                     Qty = q.Qty,
                                     Sequence = r.Sequence,
                                     UnitCount = r.UnitCount,
                                     MrpPriority = r.MrpPriority,
                                     MaxStock = r.MaxStock,
                                     SafeStock = r.SafeStock,
                                     LocationFrom = r.LocationFrom,
                                     LocationTo = r.LocationTo,
                                     Bom = r.Bom,
                                     BatchSize = r.BatchSize,
                                     ParentItem = q.ParentItem,
                                     SourceFlow = q.SourceFlow,
                                     SourceParty = q.SourceParty
                                 }).Select(p =>
                                 {
                                     customizationMgr.SetHuTo(huToMappings, p);
                                     return p;
                                 }).ToList();

            mrpMiPlanList = mrpMiPlanList.GroupBy(p => new
                            {
                                p.ProductLine,
                                p.Item,
                                p.HuTo,
                                p.PlanDate,
                                p.Sequence,
                                p.UnitCount,
                                p.MrpPriority,
                                p.BatchSize,
                                p.MaxStock,
                                p.SafeStock,
                                p.LocationFrom,
                                p.LocationTo,
                                p.Bom
                            }, (k, g) => new MrpMiPlan
                            {
                                ProductLine = k.ProductLine,
                                Item = k.Item,
                                HuTo = k.HuTo,
                                PlanDate = k.PlanDate,
                                Sequence = k.Sequence,
                                UnitCount = k.UnitCount,
                                MrpPriority = k.MrpPriority,
                                BatchSize = k.BatchSize,
                                MaxStock = k.MaxStock,
                                SafeStock = k.SafeStock,
                                LocationFrom = k.LocationFrom,
                                LocationTo = k.LocationTo,
                                Bom = k.Bom,
                                PlanVersion = newPlanVersion,
                                InvQty = inventoryBalanceDic.ValueOrDefault(k.Item),
                                Qty = g.Sum(q => q.Qty),
                                MrpMiPlanDetailList = g.Select(q => new MrpMiPlanDetail
                                {
                                    ParentItem = q.ParentItem,
                                    Qty = q.Qty,
                                    SourceFlow = q.SourceFlow,
                                    SourceParty = q.SourceParty
                                }).ToList()
                            }).OrderBy(p => p.PlanDate).ThenBy(p => p.ProductLine).ThenBy(p => p.Sequence).ToList();
            var groupbyItems = mrpMiPlanList.GroupBy(p => p.Item).ToList();
            foreach (var groupbyItem in groupbyItems)
            {
                //var currentQty = groupbyItem.Sum(p => p.Qty);
                foreach (var plan in groupbyItem)
                {
                    var item = itemMgr.GetCacheItem(plan.Item);
                    //plan.SumQty = currentQty;
                    plan.WorkHour = item.WorkHour;
                    plan.Uom = item.Uom;
                }
            }
            #endregion

            #region 排程
            double upTime = (8 * 60 - double.Parse(systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.MiCleanTime))) * 3;
            //按天/生产线汇总
            var groupMiPlanByProductLine = (from p in mrpMiPlanList
                                            group p by new
                                            {
                                                p.ProductLine,
                                            } into g
                                            select new
                                            {
                                                ProductLine = g.Key.ProductLine,
                                                List = g.ToList()
                                            }).ToList();

            foreach (var miPlanByProductLine in groupMiPlanByProductLine)
            {
                var groupMiPlanByPlanDate = (from p in miPlanByProductLine.List
                                             group p by new
                                             {
                                                 p.PlanDate
                                             } into g
                                             select new
                                             {
                                                 PlanDate = g.Key.PlanDate,
                                                 List = g.OrderByDescending(q => q.MrpPriority).ToList()
                                             }).OrderBy(r => r.PlanDate).ToList();

                var adjQtyDic = new Dictionary<string, double>();
                foreach (var miPlanByPlanDate in groupMiPlanByPlanDate)
                {
                    #region
                    //double totalWorkHours = miPlanByPlanDate.List.Sum(p => p.CheQty * p.WorkHour);
                    double currentTime = 0;

                    //满足基本需求,0库存
                    foreach (var plan in miPlanByPlanDate.List)
                    {
                        if (plan.Item == "270018")
                        {

                        }
                        if (!adjQtyDic.ContainsKey(plan.Item))
                        {
                            adjQtyDic.Add(plan.Item, 0);
                        }
                        double startQty = plan.TotalQty;
                        //先前总调整的数量
                        double totalAdj = adjQtyDic[plan.Item];
                        //当前库存
                        double currentInv = totalAdj - plan.Qty;
                        //补充到最小库存
                        double requiredQty = 0 - (totalAdj - plan.Qty);
                        //剩余工时可以做的数量
                        double leftQty = ((upTime - currentTime) / plan.WorkHour) * plan.UnitCount;
                        //取小
                        requiredQty = requiredQty < leftQty ? requiredQty : leftQty;
                        //本次调整数
                        plan.AdjustQty = requiredQty - plan.Qty;
                        if (plan.TotalQty < 0)
                        {
                            plan.AdjustQty = -plan.Qty;
                        }

                        //按车圆整
                        plan.Logs = string.Format(" 期初{0},待收:{1},待发:{2},期末:{3}", adjQtyDic[plan.Item], plan.TotalQty, plan.Qty, adjQtyDic[plan.Item] + plan.AdjustQty);

                        double adjQty = Math.Ceiling(Math.Round(plan.TotalQty / (plan.BatchSize * plan.UnitCount), 2))
                                        * plan.BatchSize * plan.UnitCount - plan.Qty;
                        plan.Logs += string.Format(" Round:前{0},后:{1}", adjQtyDic[plan.Item] + plan.AdjustQty, adjQtyDic[plan.Item] + adjQty);
                        plan.AdjustQty = adjQty;

                        adjQtyDic[plan.Item] += plan.AdjustQty;
                        double requiredTime = (plan.TotalQty / plan.UnitCount) * plan.WorkHour;
                        currentTime += requiredTime;
                    }

                    if (currentTime < upTime && false)
                    {
                        var planMaxStocks = miPlanByPlanDate.List.Where(p => p.MaxStock > 0).OrderBy(p => p.MaxStock)
                                         .GroupBy(p => p.Item).ToList();
                        foreach (var planList in planMaxStocks)
                        {
                            if (currentTime >= upTime)
                            {
                                break;
                            }
                            var totalAdj = groupMiPlanByPlanDate.SelectMany(p => p.List)
                              .Where(p => p.PlanDate <= miPlanByPlanDate.PlanDate && p.Item == planList.Key).Sum(p => p.AdjustQty);
                            if (totalAdj < planList.First().MaxStock)
                            {
                                int c = 0;
                                int length = planList.Count();
                                //分配
                                while (true)
                                {
                                    var plan = planList.ElementAt(c % length);
                                    double requiredQty = plan.MaxStock - totalAdj;
                                    double leftQty = ((upTime - currentTime) / plan.WorkHour) * plan.UnitCount;
                                    double startQty = plan.TotalQty;
                                    //取小
                                    requiredQty = requiredQty < leftQty ? requiredQty : leftQty;
                                    plan.AdjustQty += requiredQty;
                                    plan.AdjustQty = Math.Ceiling(Math.Round(plan.TotalQty / (plan.BatchSize * plan.UnitCount), 2))
                                               * plan.BatchSize * plan.UnitCount - plan.Qty;


                                    double requiredTime = ((plan.TotalQty - startQty) / plan.UnitCount) * plan.WorkHour;
                                    currentTime += requiredTime;

                                    if (currentTime >= upTime)
                                    {
                                        break;
                                    }
                                    plan.Logs += string.Format(" RoundMaxStock:前{0},后:{1}", adjQtyDic[plan.Item], adjQtyDic[plan.Item] + (plan.TotalQty - startQty));

                                    if (!adjQtyDic.ContainsKey(plan.Item))
                                    {
                                        adjQtyDic.Add(plan.Item, 0);
                                    }
                                    adjQtyDic[plan.Item] += (plan.TotalQty - startQty);
                                    c++;
                                }
                            }
                        }

                        /*
                        foreach (var plan in planMaxStocks)
                        {
                            if (currentTime >= upTime)
                            {
                                break;
                            }
                            var totalAdj = groupMiPlanByPlanDate.SelectMany(p => p.List)
                                .Where(p => p.PlanDate <= miPlanByPlanDate.PlanDate && p.Item == plan.Item).Sum(p => p.AdjustQty);
                            if (totalAdj < plan.MaxStock)
                            {
                                double requiredQty = plan.MaxStock - totalAdj;
                                double leftQty = ((upTime - currentTime) / plan.WorkHour) * plan.UnitCount;
                                //取小
                                requiredQty = requiredQty < leftQty ? requiredQty : leftQty;

                                plan.AdjustQty += requiredQty;
                                double requiredTime = (requiredQty / plan.UnitCount) * plan.WorkHour;
                                currentTime += requiredTime;
                                plan.Logs += string.Format("Max:期初{0},待收:{1},期末:{2}", totalAdj, requiredQty, totalAdj + requiredQty);
                                if (!adjQtyDic.ContainsKey(plan.Item))
                                {
                                    adjQtyDic.Add(plan.Item, 0);
                                }

                                var roundQty = Math.Ceiling(plan.TotalQty / (plan.BatchSize * plan.UnitCount))
                                                * plan.BatchSize * plan.UnitCount - plan.Qty;
                                plan.Logs += string.Format("Round:前{0},后:{1}", plan.TotalQty, roundQty + plan.Qty);
                                plan.AdjustQty = roundQty;


                            }
                        }*/
                    }
                    else
                    {
                        businessException.AddMessage(new Message(CodeMaster.MessageType.Warning, "生产线{0}在时间{1}产能不足:缺口:{2}分钟",
                            miPlanByProductLine.ProductLine, miPlanByPlanDate.PlanDate.ToString("yyyy-MM-dd"), (currentTime - upTime).ToString("0")));
                    }
                    //如果工时还没有用完,提前补充量大的.超过安全库存也无所谓
                    if (currentTime < upTime)
                    {
                        //todo
                        businessException.AddMessage(new Message(CodeMaster.MessageType.Warning, "生产线{0}在时间{1}产能剩余:{2}分钟",
                           miPlanByProductLine.ProductLine, miPlanByPlanDate.PlanDate.ToString("yyyy-MM-dd"), (upTime - currentTime).ToString("0")));
                    }
                    #endregion
                    continue;
                    #region 废弃
                    /*
                    if (currentTime > 0)
                    {
                        //时间有剩余
                        foreach (var plan in miPlanByPlanDate.List)
                        {
                            if (plan.Item == "270478")
                            {
                                //test
                            }
                            //推算出当前的需要的数量 最大库存-期初库存-sum(调整)
                            double requiredQty = Math.Round(plan.Qty + plan.MaxStock
                                - miPlanByProductLine.List.Where(p => p.PlanDate < plan.PlanDate && p.Item == plan.Item).Sum(p => p.AdjustQty));
                            if (requiredQty > 0)
                            {
                                if (plan.WorkHour > 0)
                                {
                                    double requiredTime = (requiredQty / plan.UnitCount) * plan.WorkHour;
                                    if (requiredTime >= currentTime)
                                    {
                                        plan.AdjustQty += (currentTime / plan.WorkHour) * plan.UnitCount;
                                        currentTime = 0;
                                        break;
                                    }
                                    else
                                    {
                                        plan.AdjustQty += (requiredTime / plan.WorkHour) * plan.UnitCount;
                                        currentTime -= requiredTime;
                                    }
                                }
                                else
                                {
                                    //plan.AdjustQty = -plan.Qty;
                                    if (plan.ProductLine != "MI03")
                                    {
                                        businessException.AddMessage(new Message(CodeMaster.MessageType.Error, "物料{0}的工时为0", plan.Item));
                                    }
                                }
                            }
                            else
                            {
                                plan.AdjustQty = -plan.Qty;
                            }
                        }
                    }
                    else
                    {
                        //时间不足 currentTime 负数
                        foreach (var plan in miPlanByPlanDate.List)
                        {
                            //推算出当前的需要的数量 最大库存-期初库存-sum(调整)
                            double requiredQty = Math.Round(plan.Qty + plan.SafeStock
                                - miPlanByProductLine.List.Where(p => p.PlanDate < plan.PlanDate && p.Item == plan.Item).Sum(p => p.AdjustQty));

                            if (requiredQty < 0)
                            {
                                if (plan.WorkHour > 0)
                                {
                                    requiredQty = (-requiredQty) > plan.TotalQty ? (-plan.TotalQty) : requiredQty;
                                    double requiredTime = (requiredQty / plan.UnitCount) * plan.WorkHour;
                                    if (requiredTime <= currentTime)
                                    {
                                        plan.AdjustQty += (currentTime / plan.WorkHour) * plan.UnitCount;
                                        currentTime = 0;
                                        break;
                                    }
                                    else
                                    {
                                        plan.AdjustQty += (requiredTime / plan.WorkHour) * plan.UnitCount;
                                        currentTime -= requiredTime;
                                    }
                                }
                                else
                                {
                                    //plan.AdjustQty = -plan.Qty;
                                    if (plan.ProductLine != "MI03")
                                    {
                                        businessException.AddMessage(new Message(CodeMaster.MessageType.Error, "物料{0}的工时为0", plan.Item));
                                    }
                                }
                            }
                            else
                            {
                                plan.AdjustQty = -plan.Qty;
                            }
                        }
                    }
                    */
                    #endregion
                }
            }
            //圆整到车
            //foreach (var mrpMiPlan in mrpMiPlanList)
            //{
            //    var adjQty = Math.Ceiling(mrpMiPlan.TotalQty / (mrpMiPlan.BatchSize * mrpMiPlan.UnitCount))
            //        * mrpMiPlan.BatchSize * mrpMiPlan.UnitCount - mrpMiPlan.Qty;

            //    mrpMiPlan.Logs += string.Format("Round:前{0},后:{1}", mrpMiPlan.TotalQty, adjQty + mrpMiPlan.Qty);
            //    mrpMiPlan.AdjustQty = adjQty;
            //}
            #endregion

            #region Save
            //因为BulkInsert取不到自增Id,Id手动赋值
            int id = 0;
            var lastPlans = this.genericMgr.FindAll<MrpMiPlan>(" from MrpMiPlan Order by Id desc ", 0, 1);
            if (lastPlans != null && lastPlans.Count > 0)
            {
                id = lastPlans.First().Id;
            }
            var mrpMiPlanDetailList = new List<MrpMiPlanDetail>();
            foreach (var mrpMiPlan in mrpMiPlanList)
            {
                id++;
                mrpMiPlan.Id = id;
                foreach (var mrpMiPlanDetail in mrpMiPlan.MrpMiPlanDetailList)
                {
                    mrpMiPlanDetail.PlanId = mrpMiPlan.Id;
                }
                mrpMiPlanDetailList.AddRange(mrpMiPlan.MrpMiPlanDetailList);
            }
            this.genericMgr.BulkInsert<MrpMiPlan>(mrpMiPlanList);
            this.genericMgr.BulkInsert<MrpMiPlanDetail>(mrpMiPlanDetailList);
            #endregion

            #region 使用存储过程进行算法修正
            this.genericMgr.FlushSession();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@SnapTime", snapTime);
            sqlParameters[1] = new SqlParameter("@PlanVersion", newPlanVersion);
            this.genericMgr.ExecuteStoredProcedure("USP_Busi_MRP_ScheduleMi", sqlParameters);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public double AdjustMrpMiPlan(IList<MrpMiPlan> mrpMiPlanList)
        {
            if (mrpMiPlanList.Select(p => p.PlanDate).Distinct().Count() > 1)
            {
                throw new BusinessException("天计划调整只支持一天一天的调");
            }
            if (mrpMiPlanList.Select(p => p.PlanVersion).Distinct().Count() > 1)
            {
                throw new BusinessException("天计划调整只支持同一个版本同时调");
            }

            //var shiftDetailList = this.genericMgr.FindAll<ShiftDetail>("from ShiftDetail where Shift like ? ", "MI%");
            var entityPre = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.MiCleanTime, false);
            var totalDailyMinute = 24 * 60 - double.Parse(entityPre) * 3;
            double requiredMinute = 0;

            int id = 0;
            var lastPlans = this.genericMgr.FindAll<MrpMiPlan>(" from MrpMiPlan Order by Id desc ", 0, 1);
            if (lastPlans != null && lastPlans.Count > 0)
            {
                id = lastPlans.First().Id;
            }
            foreach (var mrpMiPlan in mrpMiPlanList)
            {
                mrpMiPlan.AdjustQty = (mrpMiPlan.CurrentCheQty * mrpMiPlan.UnitCount) - mrpMiPlan.Qty;
                if (mrpMiPlan.Id > 0)
                {
                    this.genericMgr.Update(mrpMiPlan);
                }
                else
                {
                    id++;
                    mrpMiPlan.Id = id;
                    this.genericMgr.Create(mrpMiPlan);
                }
                requiredMinute += ((mrpMiPlan.TotalQty / mrpMiPlan.UnitCount) * mrpMiPlan.WorkHour);
            }
            return totalDailyMinute - requiredMinute;
        }

        [Transaction(TransactionMode.Requires)]
        public double ReleaseMiPlan(IList<MrpMiPlan> mrpMiPlanList)
        {
            double leftMinutes = AdjustMrpMiPlan(mrpMiPlanList);
            var mrpMiDateIndexs = this.genericMgr.FindAll<MrpMiDateIndex>
                (" from MrpMiDateIndex where PlanDate=? and ProductLine=? and IsActive=? ",
                new object[] { mrpMiPlanList.First().PlanDate, mrpMiPlanList.First().ProductLine, true });
            //if (mrpMiDateIndexs.Where(p => p.PlanVersion == mrpMiPlanList.First().PlanVersion).Count() > 0)
            //{
            //    this.genericMgr.Delete(@"from MrpMiShiftPlan where ProductLine= ? and PlanVersion=? and PlanDate =? ",
            //      new object[] { mrpMiPlanList.First().ProductLine, mrpMiPlanList.First().PlanVersion, mrpMiPlanList.First().PlanDate },
            //      new NHibernate.Type.IType[] { NHibernateUtil.String, NHibernateUtil.DateTime, NHibernateUtil.DateTime });
            //    //throw new BusinessException("已释放计划的不能再次释放");
            //}
            foreach (var mrpMiDateIndex in mrpMiDateIndexs)
            {
                mrpMiDateIndex.IsActive = false;
                this.genericMgr.Update(mrpMiDateIndex);
            }
            MrpMiDateIndex newMrpMiDateIndex = new MrpMiDateIndex();
            newMrpMiDateIndex.IsActive = true;
            newMrpMiDateIndex.PlanDate = mrpMiPlanList.First().PlanDate;
            newMrpMiDateIndex.PlanVersion = mrpMiPlanList.First().PlanVersion;
            newMrpMiDateIndex.ProductLine = mrpMiPlanList.First().ProductLine;
            this.genericMgr.Create(newMrpMiDateIndex);
            newMrpMiDateIndex.CreateDate = DateTime.Parse(newMrpMiDateIndex.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));
            var mrpMiShiftPlanList = this.MrpMiPlanToShiftPlan(mrpMiPlanList, newMrpMiDateIndex);
            this.genericMgr.BulkInsert<MrpMiShiftPlan>(mrpMiShiftPlanList);

            #region 使用存储过程进行修正
            this.genericMgr.FlushSession();
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@MrpMiDateIndexId", newMrpMiDateIndex.Id);
            this.genericMgr.ExecuteStoredProcedure("USP_Busi_MRP_ReleaseMiPlan", sqlParameters);
            #endregion

            return leftMinutes;
        }

        private List<MrpMiShiftPlan> MrpMiPlanToShiftPlan(IList<MrpMiPlan> mrpMiPlanList, MrpMiDateIndex mrpMiDateIndex)
        {
            if (mrpMiPlanList.Select(p => new { p.PlanDate, p.ProductLine }).Distinct().Count() > 1)
            {
                throw new BusinessException("天计划转班产计划只支持同一生产线一天一天的转");
            }
            mrpMiPlanList = mrpMiPlanList.OrderBy(p => p.Sequence).ToList();
            var shiftDetailList = this.genericMgr.FindAll<ShiftDetail>("from ShiftDetail where Shift like ? ", "MI%");

            var shiftPlanList = new List<MrpMiShiftPlan>();

            var entityPre = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.MiCleanTime, false);

            double shiftMinutes = 8 * 60 - double.Parse(entityPre);//分钟
            int i = 0;
            foreach (var mrpMiPlan in mrpMiPlanList)
            {
                MrpMiShiftPlan mrpMiShiftPlan = new MrpMiShiftPlan();
                mrpMiShiftPlan.CreateDate = mrpMiDateIndex.CreateDate;
                mrpMiShiftPlan.Bom = mrpMiPlan.Bom;
                mrpMiShiftPlan.UnitCount = mrpMiPlan.UnitCount;
                mrpMiShiftPlan.HuTo = mrpMiPlan.HuTo;
                mrpMiShiftPlan.Item = mrpMiPlan.Item;
                mrpMiShiftPlan.LocationFrom = mrpMiPlan.LocationFrom;
                mrpMiShiftPlan.LocationTo = mrpMiPlan.LocationTo;
                //mrpMiShiftPlan.ParentItem = mrpMiPlan.ParentItem;
                mrpMiShiftPlan.PlanDate = mrpMiPlan.PlanDate;
                mrpMiShiftPlan.PlanVersion = mrpMiPlan.PlanVersion;
                mrpMiShiftPlan.ProductLine = mrpMiPlan.ProductLine;
                mrpMiShiftPlan.Sequence = mrpMiPlan.Sequence;
                //mrpMiShiftPlan.SourceFlow = mrpMiPlan.SourceFlow;
                //mrpMiShiftPlan.SourceParty = mrpMiPlan.SourceParty;
                mrpMiShiftPlan.Uom = mrpMiPlan.Uom;
                mrpMiShiftPlan.WorkHour = mrpMiPlan.WorkHour;
                mrpMiShiftPlan.Shift = shiftDetailList[i].Shift;

                if (mrpMiPlan.WorkHour <= 0)
                {
                    throw new BusinessException("物料{0}工时为0", mrpMiPlan.Item);
                }

                DateTime startTime = mrpMiShiftPlan.PlanDate;
                DateTime windowTime = mrpMiShiftPlan.PlanDate;
                workingCalendarMgr.GetStartTimeAndWindowTime(mrpMiShiftPlan.Shift, mrpMiShiftPlan.PlanDate, out startTime, out windowTime);
                mrpMiShiftPlan.StartTime = startTime;
                mrpMiShiftPlan.WindowTime = windowTime;

                double requiredTime = (mrpMiPlan.TotalQty / mrpMiPlan.UnitCount) * mrpMiPlan.WorkHour;
                if (requiredTime > shiftMinutes)
                {
                    mrpMiShiftPlan.Qty = shiftMinutes / mrpMiPlan.WorkHour * mrpMiPlan.UnitCount;
                    GetNewMiShiftPlan(shiftDetailList, shiftPlanList, entityPre, ref shiftMinutes, ref i, mrpMiPlan, mrpMiShiftPlan, requiredTime);
                }
                else
                {
                    mrpMiShiftPlan.Qty = mrpMiPlan.TotalQty;
                    shiftMinutes -= requiredTime;
                }
                shiftPlanList.Add(mrpMiShiftPlan);
            }
            return shiftPlanList;
        }

        private void GetNewMiShiftPlan(IList<ShiftDetail> shiftDetailList, List<MrpMiShiftPlan> shiftPlanList,
            string entityPre, ref double shiftMinutes, ref int i, MrpMiPlan mrpMiPlan, MrpMiShiftPlan mrpMiShiftPlan,
            double requiredTime)
        {
            var newShiftMinutes = (24 / shiftDetailList.Count()) * 60 - double.Parse(entityPre);
            requiredTime = requiredTime - shiftMinutes;

            i++;
            if (i > 2)
            {
                throw new BusinessException("工时超出,天计划释放失败");
                //i = 2;
                //newShiftMinutes = shiftMinutes;
            }

            var newMrpMiShiftPlan = Mapper.Map<MrpMiShiftPlan, MrpMiShiftPlan>(mrpMiShiftPlan);
            //string[] newSplitedShiftTime = shiftDetailList[i].ShiftTime.Split(ShiftDetail.ShiftTimeSplitSymbol);
            //newMrpMiShiftPlan.StartTime = Convert.ToDateTime(mrpMiShiftPlan.PlanDate.ToString("yyyy-MM-dd") + " " + newSplitedShiftTime[0]);
            //newMrpMiShiftPlan.WindowTime = Convert.ToDateTime(mrpMiShiftPlan.PlanDate.ToString("yyyy-MM-dd") + " " + newSplitedShiftTime[1]);

            newMrpMiShiftPlan.Shift = shiftDetailList[i].Shift;
            DateTime startTime = mrpMiShiftPlan.PlanDate;
            DateTime windowTime = mrpMiShiftPlan.PlanDate;
            workingCalendarMgr.GetStartTimeAndWindowTime(newMrpMiShiftPlan.Shift, mrpMiShiftPlan.PlanDate, out startTime, out windowTime);
            newMrpMiShiftPlan.StartTime = startTime;
            newMrpMiShiftPlan.WindowTime = windowTime;

            if (mrpMiPlan.WorkHour <= 0)
            {
                throw new BusinessException("工时不能为0");
            }
            if (requiredTime > newShiftMinutes && newShiftMinutes > 0)
            {
                newMrpMiShiftPlan.Qty = newShiftMinutes / mrpMiPlan.WorkHour * mrpMiPlan.UnitCount;
                shiftMinutes = newShiftMinutes;
                GetNewMiShiftPlan(shiftDetailList, shiftPlanList, entityPre, ref shiftMinutes, ref i, mrpMiPlan, newMrpMiShiftPlan, requiredTime);
            }
            else
            {
                newMrpMiShiftPlan.Qty = requiredTime / mrpMiPlan.WorkHour * mrpMiPlan.UnitCount;
                shiftMinutes = newShiftMinutes - requiredTime;
            }
            shiftPlanList.Add(newMrpMiShiftPlan);
        }

        [Transaction(TransactionMode.Requires)]
        public double AdjustMrpMiShiftPlan(IList<MrpMiShiftPlan> mrpMiShiftPlanList)
        {
            var oldplan = mrpMiShiftPlanList.Where(p => p.Id > 0).First();
            var newplans = mrpMiShiftPlanList.Where(p => p.Id == 0);
            foreach (var newplan in newplans)
            {
                newplan.Shift = oldplan.Shift;
                newplan.StartTime = oldplan.StartTime;
                newplan.WindowTime = oldplan.WindowTime;
                newplan.CreateDate = oldplan.CreateDate;
            }

            if (mrpMiShiftPlanList.Select(p => p.Shift).Distinct().Count() > 1)
            {
                throw new BusinessException("天计划调整只支持一班一班的调");
            }
            if (mrpMiShiftPlanList.Select(p => p.PlanVersion).Distinct().Count() > 1)
            {
                throw new BusinessException("天计划调整只支持同一个版本同时调");
            }

            //var shiftDetailList = this.genericMgr.FindAll<ShiftDetail>("from ShiftDetail where Shift like ? ", "MI%");
            var entityPre = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.MiCleanTime, false);
            var totalDailyMinute = 8 * 60 - double.Parse(entityPre);

            double requiredMinute = 0;
            foreach (var mrpMiShiftPlan in mrpMiShiftPlanList)
            {

                mrpMiShiftPlan.AdjustQty = (mrpMiShiftPlan.CurrentCheQty * mrpMiShiftPlan.UnitCount) - mrpMiShiftPlan.Qty;
                if (mrpMiShiftPlan.Id > 0)
                {
                    this.genericMgr.Update(mrpMiShiftPlan);
                }
                else
                {
                    this.genericMgr.Create(mrpMiShiftPlan);
                }

                requiredMinute += ((mrpMiShiftPlan.TotalQty / mrpMiShiftPlan.UnitCount) * mrpMiShiftPlan.WorkHour);
            }
            return totalDailyMinute - requiredMinute;
        }

        #endregion

        #region SchedulePurchasePlan
        public void RunMrpPurchasePlan(User user)
        {
            var mrpPlanMaster = this.genericMgr.FindAll<MrpPlanMaster>
                ("from MrpPlanMaster where IsRelease = ? and ResourceGroup=? order by PlanVersion desc",
                new object[] { true, CodeMaster.ResourceGroup.MI }, 0, 1).First();
            var newPlanVersion = DateTime.Now;
            this.RunMrp(newPlanVersion, mrpPlanMaster.PlanVersion, CodeMaster.ResourceGroup.Other, null, user);
        }

        private void SchedulePurchasePlan(IList<MrpShipPlanGroup> groupShipPlanList, DateTime newPlanVersion, DateTime snapTime, DateTime sourcePlanVersion, User user)
        {
            var purchasePlans = from p in groupShipPlanList
                                orderby p.Flow, p.StartTime
                                where !string.IsNullOrWhiteSpace(p.Flow)
                                   && (p.OrderType == CodeMaster.OrderType.CustomerGoods
                                   || p.OrderType == CodeMaster.OrderType.Procurement
                                   || p.OrderType == CodeMaster.OrderType.ScheduleLine
                                   || p.OrderType == CodeMaster.OrderType.SubContract)
                                group p by new
                                {
                                    Flow = p.Flow,
                                    Item = p.Item,
                                    OrderType = p.OrderType,
                                    LocationTo = p.LocationTo,
                                    StartTime = p.StartTime.Date,
                                    WindowTime = p.WindowTime.Date,
                                } into g
                                select new PurchasePlan
                                {
                                    DateType = CodeMaster.TimeUnit.Day,
                                    Flow = g.Key.Flow,
                                    Item = g.Key.Item,
                                    OrderType = g.Key.OrderType,
                                    LocationTo = g.Key.LocationTo,
                                    StartTime = g.Key.StartTime,
                                    WindowTime = g.Key.WindowTime,
                                    Qty = g.Sum(q => q.Qty),
                                    PlanQty = g.Sum(s => s.Qty),
                                    PlanVersion = newPlanVersion,
                                };
            var flows = purchasePlans.Select(p => p.Flow).Distinct();
            var flowStategys = this.genericMgr.FindAllIn<FlowStrategy>
                ("from FlowStrategy where IsCheckMrpDailyPlan =? and Flow in(?", flows, new object[] { true });
            var flowMasterDic = this.genericMgr.FindAllIn<FlowMaster>
                ("from FlowMaster where Code in(?", flows)
                .GroupBy(p => p.Code, (k, g) => new { k, g.First().Description })
                .ToDictionary(d => d.k, d => d.Description);
            foreach (var flowStategy in flowStategys)
            {
                PurchasePlanMaster purchasePlanMaster = new PurchasePlanMaster();
                purchasePlanMaster.DateType = CodeMaster.TimeUnit.Day;
                purchasePlanMaster.Flow = flowStategy.Flow;
                purchasePlanMaster.FlowDescription = flowMasterDic[flowStategy.Flow];
                purchasePlanMaster.PlanVersion = newPlanVersion;
                purchasePlanMaster.SnapTime = snapTime;
                purchasePlanMaster.SourcePlanVersion = sourcePlanVersion;
                purchasePlanMaster.IsRelease = true;//天计划自动释放

                purchasePlanMaster.CreateUserId = user.Id;
                purchasePlanMaster.CreateUserName = user.FullName;
                purchasePlanMaster.CreateDate = DateTime.Now;
                purchasePlanMaster.LastModifyUserId = user.Id;
                purchasePlanMaster.LastModifyUserName = user.FullName;
                purchasePlanMaster.LastModifyDate = DateTime.Now;

                this.genericMgr.Create(purchasePlanMaster);
            }
            purchasePlans = purchasePlans.Where(p => flowStategys.Select(q => q.Flow).Contains(p.Flow));

            this.genericMgr.BulkInsert<PurchasePlan>(purchasePlans.ToList());
        }
        #endregion

        private void ScheduleTransferPlan(List<MrpShipPlanGroup> shipPlanGroupList, DateTime newPlanVersion, DateTime snapTime, DateTime sourcePlanVersion, User user)
        {
            var transferShipPlanGroup = shipPlanGroupList.Where(p => !string.IsNullOrWhiteSpace(p.Flow) &&
               (p.OrderType == CodeMaster.OrderType.SubContractTransfer || p.OrderType == CodeMaster.OrderType.Transfer)).ToList();

            GetLastReleaseTransferShipPlanGroup(CodeMaster.ResourceGroup.MI, transferShipPlanGroup);
            GetLastReleaseTransferShipPlanGroup(CodeMaster.ResourceGroup.EX, transferShipPlanGroup);

            var transferPlans = from p in transferShipPlanGroup
                                orderby p.Flow, p.StartTime
                                group p by new
                                {
                                    Flow = p.Flow,
                                    Item = p.Item,
                                    StartTime = p.StartTime.Date,
                                    WindowTime = p.WindowTime.Date,
                                } into g
                                select new TransferPlan
                                {
                                    Flow = g.Key.Flow,
                                    Item = g.Key.Item,
                                    StartTime = g.Key.StartTime,
                                    WindowTime = g.Key.WindowTime,
                                    Qty = g.Sum(q => q.Qty),
                                    PlanVersion = newPlanVersion,
                                };

            var flows = transferShipPlanGroup.Select(p => p.Flow).Distinct();
            foreach (var flow in flows)
            {
                var transferPlanMaster = new TransferPlanMaster();
                transferPlanMaster.Flow = flow;
                transferPlanMaster.PlanVersion = newPlanVersion;
                transferPlanMaster.SnapTime = snapTime;
                transferPlanMaster.SourcePlanVersion = sourcePlanVersion;
                transferPlanMaster.CreateDate = DateTime.Now;
                transferPlanMaster.CreateUserId = user.Id;
                transferPlanMaster.CreateUserName = user.FullName;
                this.genericMgr.Create(transferPlanMaster);
            }
            this.genericMgr.BulkInsert<TransferPlan>(transferPlans.ToList());
        }

        private void GetLastReleaseTransferShipPlanGroup(CodeMaster.ResourceGroup resourceGroup, List<MrpShipPlanGroup> shipPlanGroupList)
        {
            var planVersions = this.genericMgr.FindAll<MrpPlanMaster>
                ("from MrpPlanMaster where IsRelease = ? and ResourceGroup=? order by PlanVersion desc",
                new object[] { true, resourceGroup }, 0, 1);
            if (planVersions != null && planVersions.Count() > 0)
            {
                var planVersion = planVersions.First();
                var lastShipPlanGroupList = this.genericMgr.FindAll<MrpShipPlanGroup>
                      ("from MrpShipPlanGroup where PlanVersion = ? and (OrderType = ? or OrderType =?)",
                      new object[] { planVersion.PlanVersion, CodeMaster.OrderType.Transfer, CodeMaster.OrderType.SubContractTransfer });
                var c = from p in lastShipPlanGroupList.Where(p => !string.IsNullOrWhiteSpace(p.Flow))
                        join q in shipPlanGroupList on new
                        {
                            WindowTime = p.WindowTime.Date,
                            Flow = p.Flow,
                            Item = p.Item,
                        } equals new
                        {
                            WindowTime = q.WindowTime.Date,
                            Flow = q.Flow,
                            Item = q.Item,
                        } into result
                        from r in result.DefaultIfEmpty()
                        where r == null
                        select new MrpShipPlanGroup
                        {
                            WindowTime = p.WindowTime,
                            StartTime = p.StartTime,
                            Flow = p.Flow,
                            Item = p.Item,
                            LocationFrom = p.LocationFrom,
                            LocationTo = p.LocationTo,
                            OrderType = p.OrderType,
                            Qty = p.Qty,
                            PlanVersion = p.PlanVersion,
                        };
                shipPlanGroupList.AddRange(c);
            }
        }

        #region GetTLog
        private string GetTLog<T>(T item, string message)
        {
            string logMessage = string.Empty;
            if (!string.IsNullOrWhiteSpace(message))
            {
                logMessage = message + @"   ";
            }
            if (item != null)
            {
                PropertyInfo[] scheduleBodyPropertyInfo = typeof(T).GetProperties();
                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                {
                    logMessage += pi.Name + ":";
                    logMessage += pi.GetValue(item, null) + @"  ";
                }
                int length = logMessage.Length > 4000 ? 4000 : logMessage.Length;
                logMessage.Substring(0, length);
                return logMessage.Substring(0, length);
            }
            return logMessage;
        }
        #endregion

        #region check flow
        public void CheckFlow()
        {
            BusinessException businessException = new BusinessException();
            var flowMasters = this.genericMgr.FindAll<FlowMaster>
             (@"from FlowMaster as m where m.IsMRP = ? and m.IsActive = ? ", new object[] { true, true });
            var flowDetails = new List<FlowDetail>();
            foreach (var flow in flowMasters)
            {
                foreach (var flowDetail in flowMgr.GetFlowDetailList(flow, false, false))
                {
                    if (flowDetail.MrpWeight > 0)
                    {
                        //flowDetail.DefaultLocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom)
                        //    ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                        //flowDetail.DefaultLocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo)
                        //    ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                        //flowDetail.DefaultExtraLocationFrom = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationFrom)
                        //    ? flowDetail.CurrentFlowMaster.ExtraLocationFrom : flowDetail.ExtraLocationFrom;
                        //flowDetail.DefaultExtraLocationTo = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationTo)
                        //    ? flowDetail.CurrentFlowMaster.ExtraLocationTo : flowDetail.ExtraLocationTo;
                        flowDetails.Add(flowDetail);
                    }
                }
            }

            //var flowDetailDic = flowDetails.GroupBy(p => p.Item, (k, g) => new { k, g })
            //    .ToDictionary(d => d.k, d => d.g);
            var salesFlowDetails = flowDetails.Where(p => p.CurrentFlowMaster.Type == CodeMaster.OrderType.Distribution);
            var otherFlowDetails = flowDetails.Where(p => p.CurrentFlowMaster.Type != CodeMaster.OrderType.Distribution)
                .GroupBy(p => p.Item, (k, g) => new { k, g })
                .ToDictionary(d => d.k, d => d.g.ToList());
            //Parallel.ForEach(salesFlowDetails, flowDetail =>
            foreach (var flowDetail in salesFlowDetails)
            {
                CalNextFlowDetail(flowDetail, otherFlowDetails, businessException, 0);
            }
            //);

            if (businessException.HasMessage)
            {
                throw businessException;
            }
        }

        private void CalNextFlowDetail(FlowDetail flowDetail, Dictionary<string, List<FlowDetail>> flowDetailDic, BusinessException businessException, int count)
        {
            if (count > 50)
            {
                businessException.AddMessage(string.Format("物料{0}已经迭代了50级物流路线仍未结束,可能存在循环的物流路线", flowDetail.Item));
            }
            else
            {
                count++;
                var sourceFlowItems = new List<FlowItem>();
                if (flowDetail.CurrentFlowMaster.Type == CodeMaster.OrderType.Production
                    || flowDetail.CurrentFlowMaster.Type == CodeMaster.OrderType.SubContract)
                {
                    string bom = string.IsNullOrWhiteSpace(flowDetail.Bom) ? flowDetail.Item : flowDetail.Bom;
                    try
                    {
                        var bomDetails = bomMgr.GetFlatBomDetail(bom, DateTime.Now, true);
                        sourceFlowItems.AddRange(bomDetails.Select(p => new FlowItem
                        {
                            Item = p.Item,
                            DefaultLocationFrom = string.IsNullOrWhiteSpace(p.Location) ? flowDetail.DefaultLocationFrom : p.Location,
                            DefaultExtraLocationFrom = flowDetail.DefaultExtraLocationFrom,
                        }));
                    }
                    catch (Exception ex)
                    {
                        businessException.AddMessage(string.Format("分解BOM{0}出错,{1}.StackTrace:{2}", bom, ex.Message, ex.StackTrace));
                    }
                }
                else
                {
                    sourceFlowItems.Add(new FlowItem
                    {
                        Item = flowDetail.Item,
                        DefaultLocationFrom = flowDetail.DefaultLocationFrom,
                        DefaultExtraLocationFrom = flowDetail.DefaultExtraLocationFrom,
                    });
                    if (!string.IsNullOrWhiteSpace(flowDetail.Bom)
                        && flowDetail.CurrentFlowMaster.Type == CodeMaster.OrderType.Procurement)
                    {
                        try
                        {
                            var bomDetails = bomMgr.GetFlatBomDetail(flowDetail.Bom, DateTime.Now, true);
                            sourceFlowItems.AddRange(bomDetails.Select(p => new FlowItem
                            {
                                Item = p.Item,
                                DefaultLocationFrom = string.IsNullOrWhiteSpace(p.Location) ? flowDetail.DefaultLocationFrom : p.Location,
                                DefaultExtraLocationFrom = flowDetail.DefaultExtraLocationFrom,
                            }));
                        }
                        catch (Exception ex)
                        {
                            businessException.AddMessage(string.Format("分解BOM{0}出错,可能没有对应的Bom或Bom明细.StackTrace:{1}", flowDetail.Bom, ex.StackTrace));
                        }
                    }
                }

                foreach (var flowItem in sourceFlowItems)
                {
                    var flowDetailList = flowDetailDic.ValueOrDefault(flowItem.Item) ?? new List<FlowDetail>();
                    var nextFlowDetails = flowDetailList.Where(p => flowItem.DefaultLocationFrom == p.DefaultLocationTo);

                    #region 如果没有找到，考虑其他来源库位
                    if (nextFlowDetails.Count() == 0 && !string.IsNullOrWhiteSpace(flowItem.DefaultExtraLocationFrom))
                    {
                        var locations = flowItem.DefaultExtraLocationFrom.Split('|').Distinct();
                        foreach (var location in locations)
                        {
                            nextFlowDetails = flowDetailList.Where(f => f.DefaultLocationTo == location);
                            if (nextFlowDetails.Count() > 0)
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    #region 如果没有找到，考虑其他目的库位
                    if (nextFlowDetails.Count() == 0)
                    {
                        var locations = flowDetailList.Where(p => !string.IsNullOrWhiteSpace(p.DefaultExtraLocationTo))
                            .SelectMany(p => p.DefaultExtraLocationTo.Split('|')).Distinct();
                        foreach (var location in locations)
                        {
                            nextFlowDetails = flowDetailList.Where(f => location == flowItem.DefaultLocationFrom);
                            if (nextFlowDetails.Count() > 0)
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    if (nextFlowDetails != null && nextFlowDetails.Count() > 0)
                    {
                        foreach (var nextFlowDetail in nextFlowDetails)
                        {
                            CalNextFlowDetail(nextFlowDetail, flowDetailDic, businessException, count);
                        }
                    }
                    else
                    {
                        if (flowDetail.CurrentFlowMaster.Type != CodeMaster.OrderType.CustomerGoods
                            && flowDetail.CurrentFlowMaster.Type != CodeMaster.OrderType.Procurement)
                        {
                            string item = flowDetail.Item;
                            if (flowItem.Item != item)
                            {
                                item += "/" + flowItem.Item;
                            }
                            businessException.AddMessage("{0}|{1}|{2}|{3}|{4}|{5}",
                                                        flowDetail.CurrentFlowMaster.Code,
                                                        flowDetail.CurrentFlowMaster.Description,
                                                        flowDetail.CurrentFlowMaster.Type.ToString(),
                                                        item,
                                                        flowDetail.DefaultLocationFrom,
                                                        flowDetail.DefaultLocationTo);
                        }
                    }
                }
            }
        }
        #endregion

        #region class
        class FlowItem
        {
            public string Item { get; set; }
            public string DefaultLocationFrom { get; set; }
            public string DefaultExtraLocationFrom { get; set; }
        }

        class MrpInContainer
        {
            public Dictionary<string, List<MrpFlowDetail>> MrpFlowDetailDic { get; set; }
            public CodeMaster.ResourceGroup ResourceGroup { get; set; }
            public BusinessException BusinessException { get; set; }
        }

        class MrpExPlanTime
        {
            public string ProductLine { get; set; }
            public string DateIndex { get; set; }
            public double UsedTime { get; set; }
            public double UpTime { get; set; }
            public double SwitchTime { get; set; }
        }
        #endregion
    }

}
