using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.MRP.VIEW;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.SYS;
using com.Sconit.Entity.VIEW;
using com.Sconit.Utility;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace com.Sconit.Service.MRP.Impl
{
    [Transactional]
    public class PlanMgrImpl : BaseMgr, IPlanMgr
    {
        public IFlowMgr flowMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }

        #region Customized Methods

        #region 粗 View

        #region 后加工能力输出
        [Transaction(TransactionMode.Requires)]
        public IList<RccpFiView> GetMachineRccpView(IList<RccpFiPlan> rccpFiPlanList)
        {
            ConcurrentBag<RccpFiView> rccpFiViewBag = new ConcurrentBag<RccpFiView>();

            if (rccpFiPlanList != null)
            {
                string startDateIndex = rccpFiPlanList.Min(p => p.DateIndex);
                string endDateIndex = rccpFiPlanList.Max(p => p.DateIndex);
                var dateType = rccpFiPlanList.First().DateType;

                #region 获取数据
                DateTime snapTime = this.genericMgr.FindById<RccpPlanMaster>(rccpFiPlanList.First().PlanVersion).SnapTime;

                var fiFlowDetails = this.genericMgr.FindAll<MrpFlowDetail>
                    (" from MrpFlowDetail where SnapTime = ? and ResourceGroup=? ",
                    new object[] { snapTime, CodeMaster.ResourceGroup.FI });

                var workCalendars = this.genericMgr.FindAll<WorkCalendar>
                    (@" from WorkCalendar as w where w.DateType =? and w.ResourceGroup=? and w.DateIndex between ? and ? ",
                    new object[] { dateType, CodeMaster.ResourceGroup.FI, startDateIndex, endDateIndex });

                var machineInstances = this.genericMgr.FindAll<MachineInstance>
                    ("from MachineInstance where DateType = ? and DateIndex between ? and ? ",
                    new object[] { dateType, startDateIndex, endDateIndex });

                var machines = this.genericMgr.FindAll<Machine>();
                #endregion

                #region 从RccpFiPlan中取值
                var rccpFiViewList = (from p in rccpFiPlanList
                                      group p by new
                                      {
                                          ProductLine = p.ProductLine,
                                          Machine = p.Machine,
                                          DateIndex = p.DateIndex,
                                          DateType = p.DateType,
                                      } into g
                                      select new RccpFiView
                                      {
                                          ProductLine = g.Key.ProductLine,
                                          Machine = g.Key.Machine,
                                          DateIndex = g.Key.DateIndex,
                                          DateType = g.Key.DateType,
                                          Qty = g.Sum(p => p.Qty),
                                          RccpFiPlanList = g.ToList()
                                      }).ToList();
                #endregion

                #region 从MachineInstance中取值
                var groupRccpFiViews = from p in rccpFiViewList
                                       group p by new
                                       {
                                           DateIndex = p.DateIndex,
                                           ProductLine = p.ProductLine,
                                           DateType = p.DateType,
                                       } into g
                                       select new
                                       {
                                           DateIndex = g.Key.DateIndex,
                                           ProductLine = g.Key.ProductLine,
                                           DateType = g.Key.DateType,
                                           List = g
                                       };
                //Parallel.ForEach(groupRccpFiViews, groupRccpFiView =>
                foreach (var groupRccpFiView in groupRccpFiViews)
                {
                    #region 工作日历
                    var workCalendar = workCalendars.FirstOrDefault(p => p.DateIndex == groupRccpFiView.DateIndex
                        && p.Flow == groupRccpFiView.ProductLine && p.DateType == dateType) ?? new WorkCalendar();
                    var daysInMonth = 30;
                    if (dateType == CodeMaster.TimeUnit.Month)
                    {
                        DateTime startDate = DateTime.Parse(groupRccpFiView.DateIndex + "-01");
                        DateTime endDate = Utility.DateTimeHelper.GetEndTime(CodeMaster.TimeUnit.Month, startDate);
                        daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
                    }
                    #endregion

                    var machineInstancesByDateIndex = machineInstances.Where(p => groupRccpFiView.DateIndex == p.DateIndex);
                    foreach (var rccpFiView in groupRccpFiView.List)
                    {
                        var newRccpFiView = Mapper.Map<RccpFiView, RccpFiView>(rccpFiView);
                        var machineInstance = machineInstancesByDateIndex
                            .FirstOrDefault(p => newRccpFiView.Machine == p.Code && newRccpFiView.DateType == p.DateType);
                        var machine = machines.FirstOrDefault(p => newRccpFiView.Machine == p.Code);
                        if (machineInstance != null && machine != null)
                        {
                            newRccpFiView.Seq = machine.Seq;
                            newRccpFiView.Description = machineInstance.Description;
                            newRccpFiView.Island = machineInstance.Island;
                            newRccpFiView.IslandDescription = machineInstance.IslandDescription;
                            newRccpFiView.IslandQty = machineInstance.IslandQty;
                            newRccpFiView.MachineQty = machineInstance.Qty;
                            newRccpFiView.ShiftType = machineInstance.ShiftType;
                            newRccpFiView.ShiftQuota = machineInstance.ShiftQuota;
                            newRccpFiView.ShiftPerDay = machineInstance.ShiftPerDay;
                            newRccpFiView.MachineType = machineInstance.MachineType;
                            if (newRccpFiView.DateType == CodeMaster.TimeUnit.Month)
                            {
                                newRccpFiView.TrialProduceTime = workCalendar.TrialTime;
                                newRccpFiView.HaltTime = workCalendar.HaltTime;
                                newRccpFiView.Holiday = workCalendar.Holiday;
                                newRccpFiView.NormalWorkDay = daysInMonth - newRccpFiView.Holiday - newRccpFiView.TrialProduceTime - newRccpFiView.HaltTime;
                                newRccpFiView.MaxWorkDay = newRccpFiView.NormalWorkDay;
                            }
                            else
                            {
                                newRccpFiView.NormalWorkDay = machineInstance.NormalWorkDayPerWeek - workCalendar.Holiday;
                                newRccpFiView.MaxWorkDay = machineInstance.MaxWorkDayPerWeek - workCalendar.Holiday;
                            }

                            newRccpFiView.CurrentNormalShiftQty = newRccpFiView.ShiftPerDay * newRccpFiView.NormalWorkDay;
                            newRccpFiView.CurrentMaxShiftQty = newRccpFiView.ShiftPerDay * newRccpFiView.MaxWorkDay;


                            newRccpFiView.NormalShiftQty = (int)newRccpFiView.ShiftType * newRccpFiView.NormalWorkDay;
                            newRccpFiView.MaxShiftQty = (int)newRccpFiView.ShiftType * newRccpFiView.MaxWorkDay;

                            newRccpFiView.CurrentMaxQty = newRccpFiView.ShiftQuota * newRccpFiView.ShiftPerDay
                                                    * newRccpFiView.MaxWorkDay
                                                    * newRccpFiView.MachineQty;

                            newRccpFiView.CurrentNormalQty = newRccpFiView.ShiftQuota * newRccpFiView.ShiftPerDay
                                                    * newRccpFiView.NormalWorkDay
                                                    * newRccpFiView.MachineQty;

                            newRccpFiView.MaxQty = newRccpFiView.ShiftQuota * (int)newRccpFiView.ShiftType
                                              * newRccpFiView.MaxWorkDay
                                              * newRccpFiView.MachineQty;

                            newRccpFiView.NormalQty = newRccpFiView.ShiftQuota * (int)newRccpFiView.ShiftType
                                                    * newRccpFiView.NormalWorkDay
                                                    * newRccpFiView.MachineQty;
                            if (newRccpFiView.MachineType == CodeMaster.MachineType.Kit)
                            {
                                var itemCount = fiFlowDetails.Count(p => p.Machine == newRccpFiView.Machine);
                                //取大
                                newRccpFiView.RequiredShiftQty = rccpFiPlanList.Where(p => p.Machine == newRccpFiView.Machine && p.DateIndex == newRccpFiView.DateIndex)
                                                                    .Max(p => p.Qty)
                                                                    / newRccpFiView.ShiftQuota
                                                                    / newRccpFiView.MachineQty;

                                newRccpFiView.ItemCount = itemCount;
                            }
                            else
                            {
                                newRccpFiView.RequiredShiftQty = newRccpFiView.Qty
                                    / newRccpFiView.ShiftQuota //当前班产定额
                                    / newRccpFiView.MachineQty;         //模具
                                newRccpFiView.ItemCount = 1;
                                //套的班产定额
                                //newRccpFiView.KitShiftQuota = newRccpFiView.ShiftQuota / itemCount;
                            }
                            newRccpFiView.MaxQty *= newRccpFiView.ItemCount;
                            newRccpFiView.NormalQty *= newRccpFiView.ItemCount;
                            newRccpFiView.CurrentMaxQty *= newRccpFiView.ItemCount;
                            newRccpFiView.CurrentNormalQty *= newRccpFiView.ItemCount;

                            newRccpFiView.RequiredFactQty = newRccpFiView.Qty / newRccpFiView.ItemCount / newRccpFiView.ShiftQuota
                                / (int)newRccpFiView.ShiftType / newRccpFiView.MaxWorkDay;
                            //需求模具数=需求/套数/班产定额/（班次/天)/工作天数    工作天数=周最大工作天数-节假日
                            newRccpFiView.CurrentRequiredFactQty = newRccpFiView.Qty / newRccpFiView.ItemCount
                                / newRccpFiView.ShiftQuota / newRccpFiView.ShiftPerDay / newRccpFiView.MaxWorkDay;
                            rccpFiViewBag.Add(newRccpFiView);
                        }
                    }
                }
                //);
                #endregion
            }
            foreach (var rccpFiView in rccpFiViewBag)
            {
                rccpFiView.KitQty = rccpFiView.RccpFiPlanList
                    .Where(p => !string.IsNullOrWhiteSpace(p.Model))
                    .GroupBy(p => p.Model, (k, g) => new { k, Qty = g.Sum(q => q.Qty) / g.Sum(q => q.ModelRate) })
                    .Sum(p => p.Qty);
                if (rccpFiView.KitQty == 0)
                {
                    var _list = rccpFiView.RccpFiPlanList
                       .Where(p => !string.IsNullOrWhiteSpace(p.Model))
                       .GroupBy(p => p.Model, (k, g) => new { k, ModelRate = g.Sum(p => p.ModelRate) });
                    rccpFiView.ModelRate = _list.Sum(p => p.ModelRate / _list.Count());
                }
                else
                {
                    rccpFiView.ModelRate = rccpFiView.Qty / rccpFiView.KitQty;
                }
                if (rccpFiView.MachineType == CodeMaster.MachineType.Single)
                {
                    rccpFiView.KitShiftQuota = rccpFiView.ShiftQuota / rccpFiView.ModelRate;
                }
                else
                {
                    rccpFiView.KitShiftQuota = rccpFiView.ShiftQuota;
                }
                rccpFiView.DiffQty = rccpFiView.MaxQty - rccpFiView.Qty;
                rccpFiView.CurrentDiffQty = rccpFiView.CurrentMaxQty - rccpFiView.Qty;
                //班次/天（参考）= 需求班次总数/工作天数
                rccpFiView.RequiredShiftPerDay = rccpFiView.RequiredShiftQty / rccpFiView.MaxWorkDay;
            }
            return rccpFiViewBag.OrderBy(p => p.Seq).ThenBy(p => p.Island).ThenBy(p => p.Machine).ToList();
        }

        [Transaction(TransactionMode.Requires)]
        public IList<RccpFiView> GetIslandRccpView(IList<RccpFiPlan> rccpFiPlanList)
        {
            var rccpMachineFiViewList = GetMachineRccpView(rccpFiPlanList);

            //var bomDetailDic = this.genericMgr.FindAllIn<BomDetail>("from BomDetail where Item in(?", rccpFiPlanList.Select(p => p.Item).Distinct())
            //         .GroupBy(p => p.Item, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.First());

            var islands = this.genericMgr.FindAll<Island>();

            var rccpFiViewList = (from p in rccpMachineFiViewList
                                  group p by new
                                  {
                                      ProductLine = p.ProductLine,
                                      Island = p.Island,
                                      DateIndex = p.DateIndex,
                                      DateType = p.DateType,
                                  } into g
                                  select new RccpFiView
                                  {
                                      Seq = islands.FirstOrDefault(q => q.Code == g.Key.Island).Seq,
                                      ProductLine = g.Key.ProductLine,
                                      Island = g.Key.Island,
                                      IslandDescription = g.First().IslandDescription,
                                      IslandQty = g.First().IslandQty,
                                      DateIndex = g.Key.DateIndex,
                                      DateType = g.Key.DateType,
                                      Qty = g.Sum(p => p.Qty),
                                      RequiredShiftQty = g.Sum(p => p.RequiredShiftQty),
                                      MaxWorkDay = g.Max(p => p.MaxWorkDay),
                                      NormalQty = (g.Sum(p => p.NormalQty / p.MachineQty) / g.Count()) * g.First().IslandQty,
                                      MaxQty = (g.Sum(p => p.MaxQty / p.MachineQty) / g.Count()) * g.First().IslandQty,
                                      CurrentNormalQty = (g.Sum(p => p.CurrentNormalQty / p.MachineQty) / g.Count()) * g.First().IslandQty,
                                      CurrentMaxQty = (g.Sum(p => p.CurrentMaxQty / p.MachineQty) / g.Count()) * g.First().IslandQty,
                                      MaxShiftQty = g.Max(p => p.MaxShiftQty),
                                      NormalShiftQty = g.Max(p => p.NormalShiftQty),
                                      CurrentMaxShiftQty = g.Max(p => p.CurrentMaxShiftQty),
                                      CurrentNormalShiftQty = g.Max(p => p.CurrentNormalShiftQty),
                                      RequiredFactQty = g.Sum(p => p.RequiredFactQty),
                                      CurrentRequiredFactQty = g.Sum(p => p.CurrentRequiredFactQty),
                                      RccpFiPlanList = g.SelectMany(q => q.RccpFiPlanList).ToList(),
                                      RccpFiViewList = g.ToList()
                                  }).ToList();
            foreach (var rccpFiView in rccpFiViewList)
            {
                rccpFiView.KitQty = rccpFiView.RccpFiPlanList
                    .Where(p => !string.IsNullOrWhiteSpace(p.Model))
                    .GroupBy(p => p.Model, (k, g) => new { k, Qty = g.Sum(q => q.Qty) / g.Sum(q => q.ModelRate) })
                    .Sum(p => p.Qty);
                if (rccpFiView.KitQty == 0)
                {
                    var _list = rccpFiView.RccpFiPlanList
                       .Where(p => !string.IsNullOrWhiteSpace(p.Model))
                       .GroupBy(p => p.Model, (k, g) => new { k, ModelRate = g.Sum(p => p.ModelRate) });
                    rccpFiView.ModelRate = _list.Sum(p => p.ModelRate / _list.Count());
                }
                else
                {
                    rccpFiView.ModelRate = rccpFiView.Qty / rccpFiView.KitQty;
                }
            }
            return rccpFiViewList;
        }
        #endregion

        #region 挤出能力输出
        /// <summary> 
        /// 生产线汇总:负荷率 挤出生产线负荷线
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByProdLineView> GetExRccpViewGroupByProdLineLoad(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var rccpExViews = GetExRccpView(rccpTransList);
            int lineCount = rccpExViews.Select(p => p.ProductLine).Distinct().Count();
            var rccpExViewGroups = (from p in rccpExViews
                                    group p by new
                                    {
                                        p.ProductLine,
                                        p.DateIndex,
                                        p.UpTime,
                                    } into g
                                    select new RccpExGroupByProdLineView
                                    {
                                        ProductLine = g.Key.ProductLine,
                                        DateIndex = g.Key.DateIndex,
                                        Qty = g.Sum(q => q.RequireTime) / g.Key.UpTime,
                                    }).ToList();

            var sumPlan = rccpExViewGroups.GroupBy(p => p.DateIndex, (k, g) => new RccpExGroupByProdLineView
                                             {
                                                 ProductLine = "Avg",
                                                 DateIndex = k,
                                                 Qty = g.Sum(q => q.Qty) / g.Count(),
                                             });
            rccpExViewGroups.AddRange(sumPlan);


            List<WarningColor> warningColors = GetWarningColors();
            foreach (var rccp in rccpExViewGroups)
            {
                var warningColor = warningColors.FirstOrDefault(p => rccp.Qty >= p.Start && rccp.Qty < p.End);
                if (warningColor != null)
                {
                    rccp.Css = warningColor.Css;
                }
            }

            return rccpExViewGroups;
        }

        private List<WarningColor> GetWarningColors()
        {
            var entityPre = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.ProdLineWarningColors, false);
            var colors = entityPre.Split('-');
            List<WarningColor> warningColors = new List<WarningColor>();
            for (int i = 0; i < colors.Length; i++)
            {
                WarningColor warningColor = new WarningColor();
                if (i == 0 || i == 6)
                {
                    warningColor.Start = double.Parse(colors[i]) / 100;
                    warningColor.End = double.Parse(colors[i + 1]) / 100;
                    warningColor.Css = "WarningColor_Black";
                }
                else if (i == 1 || i == 5)
                {
                    warningColor.Start = double.Parse(colors[i]) / 100;
                    warningColor.End = double.Parse(colors[i + 1]) / 100;
                    warningColor.Css = "WarningColor_Red";
                }
                else if (i == 2 || i == 4)
                {
                    warningColor.Start = double.Parse(colors[i]) / 100;
                    warningColor.End = double.Parse(colors[i + 1]) / 100;
                    warningColor.Css = "WarningColor_Yellow";
                }
                else if (i == 3)
                {
                    warningColor.Start = double.Parse(colors[i]) / 100;
                    warningColor.End = double.Parse(colors[i + 1]) / 100;
                    warningColor.Css = "WarningColor_Green";
                }
                warningColors.Add(warningColor);
            }
            return warningColors;
        }

        class WarningColor
        {
            public double Start { get; set; }
            public double End { get; set; }
            public string Css { get; set; }
        }

        /// <summary>
        /// 生产线汇总:总米数 挤出生产线总米数
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByProdLineView> GetExRccpViewGroupByProdLineQty(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var rccpExViews = GetExRccpView(rccpTransList);
            var rccpExViewGroups = (from p in rccpExViews
                                    group p by new
                                    {
                                        p.ProductLine,
                                        p.DateIndex,
                                    } into g
                                    select new RccpExGroupByProdLineView
                                    {
                                        ProductLine = g.Key.ProductLine,
                                        DateIndex = g.Key.DateIndex,
                                        Qty = g.Sum(q => q.Qty) / 10000,
                                    }).ToList();
            var sumPlan = rccpExViewGroups.GroupBy(p => p.DateIndex, (k, g) => new RccpExGroupByProdLineView
                                   {
                                       ProductLine = "Total",
                                       DateIndex = k,
                                       Qty = g.Sum(q => q.Qty),
                                   });
            rccpExViewGroups.AddRange(sumPlan);

            return rccpExViewGroups;
        }

        /// <summary>
        /// 生产线汇总:加权速度 挤出生产线加权速度
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByProdLineView> GetExRccpViewGroupByProdLineSpeed(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var rccpExViews = GetExRccpView(rccpTransList);
            var rccpExViewGroups = (from p in rccpExViews
                                    group p by new
                                    {
                                        p.ProductLine,
                                        p.DateIndex,
                                    } into g
                                    select new RccpExGroupByProdLineView
                                    {
                                        ProductLine = g.Key.ProductLine,
                                        DateIndex = g.Key.DateIndex,
                                        //总长度(/腔口数)/总时间
                                        Qty = g.Sum(q => (q.Qty / q.SpeedTimes)) / g.Sum(q => q.RequireTime),
                                    }).ToList();

            var avgPlan = rccpExViewGroups.GroupBy(p => p.DateIndex, (k, g) => new RccpExGroupByProdLineView
            {
                ProductLine = "Total",
                DateIndex = k,
                Qty = g.Average(q => q.Qty),
            });
            rccpExViewGroups.AddRange(avgPlan);

            return rccpExViewGroups;
        }

        /// <summary>
        /// 生产线汇总:加权废品率 挤出生产线加权废品率
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByProdLineView> GetExRccpViewGroupByProdLineScrapPercentage(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var rccpExViews = GetExRccpView(rccpTransList);
            var rccpExViewGroups = (from p in rccpExViews
                                    group p by new
                                    {
                                        p.ProductLine,
                                        p.DateIndex,
                                    } into g
                                    select new RccpExGroupByProdLineView
                                    {
                                        ProductLine = g.Key.ProductLine,
                                        DateIndex = g.Key.DateIndex,
                                        //占线时间比*废品率
                                        Qty = (g.Sum(q => (q.RequireTime / g.Sum(r => r.RequireTime)) * q.ScrapPercentage)),
                                    }).ToList();

            var avgPlan = rccpExViewGroups.GroupBy(p => p.DateIndex, (k, g) => new RccpExGroupByProdLineView
            {
                ProductLine = "Total",
                DateIndex = k,
                Qty = g.Average(q => q.Qty),
            });
            rccpExViewGroups.AddRange(avgPlan);
            return rccpExViewGroups;
        }

        /// <summary>
        /// 生产线断面汇总:占线时间 挤出断面占线时间
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByItemView> GetExRccpViewGroupByItemTime(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var rccpExViews = GetExRccpView(rccpTransList);
            int linecount = rccpExViews.Select(p => p.ProductLine).Distinct().Count();
            var rccpExViewGroups = (from p in rccpExViews
                                    where p.RequireTime > 0
                                    select new RccpExGroupByItemView
                                    {
                                        ProductLine = p.ProductLine,
                                        DateIndex = p.DateIndex,
                                        Item = p.Item,
                                        Qty = p.RequireTime / 60,
                                    }).ToList();
            ;
            return rccpExViewGroups;
        }

        /// <summary>
        /// 生产线断面汇总:占线率 挤出断面占线率
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByItemView> GetExRccpViewGroupByItemLoad(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var rccpExViews = GetExRccpView(rccpTransList);
            int linecount = rccpExViews.Select(p => p.ProductLine).Distinct().Count();
            var rccpExViewGroups = (from p in rccpExViews
                                    where p.RequireTime > 0
                                    select new RccpExGroupByItemView
                                    {
                                        ProductLine = p.ProductLine,
                                        DateIndex = p.DateIndex,
                                        Item = p.Item,
                                        Qty = p.RequireTime / p.UpTime,
                                    }).ToList();
            ;
            return rccpExViewGroups;
        }

        /// <summary>
        /// 生产线断面汇总:米数 挤出断面总米数
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByItemView> GetExRccpViewGroupByItemQty(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var rccpExViews = GetExRccpView(rccpTransList);
            int linecount = rccpExViews.Select(p => p.ProductLine).Distinct().Count();
            var rccpExViewGroups = (from p in rccpExViews
                                    where p.Qty > 0
                                    select new RccpExGroupByItemView
                                    {
                                        ProductLine = p.ProductLine,
                                        DateIndex = p.DateIndex,
                                        Item = p.Item,
                                        Qty = p.Qty / 10000,
                                    }).ToList();
            return rccpExViewGroups;
        }

        /// <summary>
        /// 生产线功能分类汇总:负荷率 生产线功能分类负荷率
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByClassifyView> GetExRccpViewGroupByClassifyLoad(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var warningColors = GetWarningColors();

            var views = GetExClassifyView(rccpTransList);

            foreach (var view in views)
            {
                var gourpViews = from p in view.RccpExViews
                                 group p by
                                 new
                                 {
                                     p.ProductLine,
                                     p.UpTime
                                 } into g
                                 select new
                                 {
                                     ProductLine = g.Key.ProductLine,
                                     UpTime = g.Key.UpTime,
                                     List = g
                                 };

                foreach (var gourpView in gourpViews)
                {
                    if (gourpView.UpTime != 0)
                    {
                        view.Qty += (gourpView.List.Sum(p => p.RequireTime) / gourpView.UpTime);
                    }
                }
                view.Qty = view.Qty / gourpViews.Count();

                var warningColor = warningColors.FirstOrDefault(p => view.Qty >= p.Start && view.Qty < p.End);
                if (warningColor != null)
                {
                    view.Css = warningColor.Css;
                }
            }
            return views;
        }

        private List<RccpExGroupByClassifyView> GetExClassifyView(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var rccpExViews = GetExRccpView(rccpTransList);

            var flowClassifyList = this.genericMgr.FindAll<FlowClassify>();

            var groupRccpExViews = from q in rccpExViews
                                   join r in flowClassifyList
                                   on q.ProductLine equals r.Flow
                                   select new
                                   {
                                       Classify = r != null ? r.Code : null,
                                       DateIndex = q.DateIndex,
                                       RccpExView = q
                                   };
            var views = (from p in groupRccpExViews
                         group p by new
                         {
                             Classify = p.Classify,
                             DateIndex = p.DateIndex
                         } into g
                         select new RccpExGroupByClassifyView
                         {
                             Classify = g.Key.Classify,
                             DateIndex = g.Key.DateIndex,
                             RccpExViews = g.Select(p => p.RccpExView)
                         }).ToList();

            return views;
        }

        /// <summary>
        /// 生产线功能分类汇总:加权速度 生产线功能分类加权速度
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByClassifyView> GetExRccpViewGroupByClassifySpeed(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var views = GetExClassifyView(rccpTransList);

            foreach (var view in views)
            {
                double qty = view.RccpExViews.Sum(p => (p.Qty / p.SpeedTimes));
                double requireTime = view.RccpExViews.Sum(p => p.RequireTime);

                if (requireTime != 0)
                {
                    ///总长度(/腔口数)/总时间
                    view.Qty = qty / requireTime;
                }
            }
            return views;
        }

        /// <summary>
        /// 生产线功能分类汇总:加权废品率 生产线功能分类加权废品率
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByClassifyView> GetExRccpViewGroupByClassifyScrapPercentage(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var views = GetExClassifyView(rccpTransList);

            foreach (var view in views)
            {
                double totalRequireTime = view.RccpExViews.Sum(p => p.RequireTime);

                if (totalRequireTime > 0)
                {
                    view.Qty = (view.RccpExViews.Sum(p => ((p.ScrapPercentage * p.RequireTime) / totalRequireTime)));
                }
            }

            return views;
        }

        /// <summary>
        /// 生产线功能分类汇总:总米数 生产线功能分类总米数
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public IEnumerable<RccpExGroupByClassifyView> GetExRccpViewGroupByClassifyQty(IEnumerable<RccpTransGroup> rccpTransList)
        {
            var views = GetExClassifyView(rccpTransList);

            foreach (var view in views)
            {
                view.Qty = view.RccpExViews.Sum(p => p.Qty) / 10000;
            }

            return views;
        }

        public IEnumerable<RccpExView> GetExRccpView(IEnumerable<RccpTransGroup> rccpTransList)
        {
            #region 获得RccpExPlan
            string startDateIndex = rccpTransList.Min(p => p.DateIndex);
            string endDateIndex = rccpTransList.Max(p => p.DateIndex);
            var exInstanceList = this.genericMgr.FindAll<ProdLineExInstance>
                ("from ProdLineExInstance where DateType =? and DateIndex between ? and ? ",
                new object[] { rccpTransList.First().DateType, startDateIndex, endDateIndex });

            var workCalendars = this.genericMgr.FindAll<WorkCalendar>
                (@" from WorkCalendar as w where w.DateType =? and w.ResourceGroup=? and w.DateIndex between ? and ? ",
                new object[] { rccpTransList.First().DateType, CodeMaster.ResourceGroup.EX, startDateIndex, endDateIndex });

            //DateTime snapTime = this.genericMgr.FindAll<RccpPlanMaster>
            //    (" from RccpPlanMaster where PlanVersion = ? ", rccpTransList.First().PlanVersion, 0, 1).First().SnapTime;

            //IList<MrpFlowDetail> mrpFlowDetailList = this.genericMgr.FindAll<MrpFlowDetail>
            //    ("from MrpFlowDetail where SnapTime =? and ResourceGroup = ? ",
            //    new object[] { snapTime, CodeMaster.ResourceGroup.EX });

            List<RccpExView> rccpExViewList = new List<RccpExView>();

            var groupList1 = (from p in exInstanceList
                              group p by new
                              {
                                  Region = p.Region,
                                  ProductLine = p.ProductLine,
                                  DateType = p.DateType,
                                  DateIndex = p.DateIndex,
                                  Correction = p.Correction,
                              } into g
                              select new
                              {
                                  ProductLine = g.Key.ProductLine,
                                  Region = g.Key.Region,
                                  DateType = g.Key.DateType,
                                  DateIndex = g.Key.DateIndex,
                                  Correction = g.Key.Correction == 0 ? 1 : g.Key.Correction,
                                  RequireTime = 0.0,
                              }).ToList();
            //可用时间
            var groupList2 = (from p in groupList1
                              join q in workCalendars
                              on new
                              {
                                  Flow = p.ProductLine,
                                  DateIndex = p.DateIndex,
                                  DateType = p.DateType
                              }
                              equals new
                              {
                                  Flow = q.Flow,
                                  DateIndex = q.DateIndex,
                                  DateType = q.DateType
                              } into result
                              from r in result.DefaultIfEmpty()
                              select new
                              {
                                  ProductLine = p.ProductLine,
                                  DateType = p.DateType,
                                  DateIndex = p.DateIndex,
                                  UpTime = p.DateType == CodeMaster.TimeUnit.Week ? 7 : (r == null ? 30 : r.UpTime)
                              }).ToList();

            //为了提高性能,按照DateIndex分组
            var exInstanceGroup = (from p in exInstanceList
                                   group p by new
                                   {
                                       p.DateIndex
                                   } into g
                                   select new
                                   {
                                       DateIndex = g.Key.DateIndex,
                                       List = (from q in g
                                               join r in groupList2
                                               on new
                                               {
                                                   q.ProductLine,
                                                   q.DateIndex
                                               } equals
                                               new
                                               {
                                                   r.ProductLine,
                                                   r.DateIndex
                                               }
                                               into result
                                               from t in result.DefaultIfEmpty()
                                               select new RccpExView
                                               {
                                                   Region = q.Region,
                                                   ProductLine = q.ProductLine,
                                                   Item = q.Item,
                                                   DateIndex = q.DateIndex,
                                                   DateType = q.DateType,
                                                   Speed = q.RccpSpeed,
                                                   ApsPriority = q.ApsPriority,
                                                   Quota = q.Quota,
                                                   SpeedTimes = q.SpeedTimes,
                                                   SwitchTime = q.SwitchTime,
                                                   EconomicLotSize = q.EconomicLotSize,
                                                   Correction = q.Correction,
                                                   UpTime = (q.DateType == CodeMaster.TimeUnit.Week ? 7 : (t == null ? 30 : t.UpTime)) * 24 * 60
                                               }).ToList()
                                   }).ToList();

            var rccpTransGroup = (from p in rccpTransList
                                  group p by new
                                  {
                                      p.DateIndex
                                  } into g
                                  select new
                                  {
                                      DateIndex = g.Key.DateIndex,
                                      List = g.ToList()
                                  }).ToList();

            var rccpJoin = (from p in exInstanceGroup
                            join q in rccpTransGroup
                            on p.DateIndex equals q.DateIndex
                            into result
                            from t in result.DefaultIfEmpty()
                            select new
                            {
                                DateIndex = p.DateIndex,
                                RccpExViews = p.List,
                                Trans = t == null ? null : t.List.ToList()
                            }).ToList();

            var notAllotRccpExViews = (from p in rccpTransList
                                       join q in exInstanceList
                                       on new { p.DateIndex, p.Item } equals new { q.DateIndex, q.Item }
                                       into result
                                       from t in result.DefaultIfEmpty()
                                       select new
                                       {
                                           DateIndex = p.DateIndex,
                                           Trans = p,
                                           exInstance = t == null ? null : t
                                       }).Where(p => p.exInstance == null)
                                        .Select(p => new RccpExView
                                        {
                                            Region = "未分配",
                                            ProductLine = "未分配",
                                            Item = p.Trans.Item,
                                            DateIndex = p.Trans.DateIndex,
                                            DateType = p.Trans.DateType,
                                            Speed = 1,
                                            ApsPriority = CodeMaster.ApsPriorityType.Normal,
                                            Quota = 1,
                                            SpeedTimes = 1,
                                            SwitchTime = 1,
                                            EconomicLotSize = 1,
                                            Correction = 1,
                                            Qty = p.Trans.Qty,
                                            ScrapPercentage = p.Trans.ScrapPercentage,
                                            UpTime = (p.Trans.DateType == CodeMaster.TimeUnit.Week ? 7 : 30) * 24 * 60
                                        }).ToList();

            var switchPercent = GetWarningColors()[4].Start;

            //Parallel.ForEach(rccpJoin, rccp =>
            foreach (var rccp in rccpJoin)
            {
                var newRccpExViews = rccp.RccpExViews.ToList();
                if (rccp.Trans != null)
                {
                    //需求 物料总权数小的先排
                    foreach (var rccpTrans in rccp.Trans)
                    {
                        rccpTrans.TotalAps = newRccpExViews.Where(p => p.Item == rccpTrans.Item).Sum(p => (int)p.ApsPriority);
                    }

                    foreach (var rccpTrans in rccp.Trans.OrderBy(p => p.TotalAps))
                    {
                        //需要断面的长度
                        double planQty = rccpTrans.Qty;
                        //找出此断面在哪些生产线上生产 常用生产线先排
                        var rccpExViews = newRccpExViews.Where(p => p.Item == rccpTrans.Item).OrderByDescending(p => p.ApsPriority);

                        if (rccpExViews.Count() == 0)
                        {
                            continue;
                        }

                        //var normalExPlanViews = rccpExViews.Where(p => p.ApsPriority == CodeMaster.ApsPriorityType.Normal);
                        //double totalQuota = normalExPlanViews.Sum(p => p.Quota);

                        //常用/备用生产线
                        var normalExPlans = rccpExViews.Where(p => p.ApsPriority == CodeMaster.ApsPriorityType.Normal);
                        var backupExPlans = rccpExViews.Where(p => p.ApsPriority == CodeMaster.ApsPriorityType.Backup);
                        //总的配额
                        double normalQuota = normalExPlans.Sum(p => p.Quota);
                        double backupQuota = backupExPlans.Sum(p => p.Quota);
                        normalQuota = normalQuota == 0 ? 1 : normalQuota;
                        backupQuota = backupQuota == 0 ? 1 : backupQuota;

                        #region 遍历此断面的生产线,按顺序分配到常用上去
                        double normalQty = planQty;
                        foreach (var rccpExView in normalExPlans)
                        {
                            if (planQty == 0)
                            {
                                break;
                            }
                            double requireTime = newRccpExViews.Where(p => p.ProductLine == rccpExView.ProductLine).Sum(p => p.RequireTime);
                            double upTime = rccpExView.UpTime * switchPercent;// rccpExView.Correction;
                            if (requireTime > upTime)
                            {
                                break;
                            }
                            //切换时间折合到速度上去.
                            rccpExView.ConvSpeed = ((rccpExView.EconomicLotSize * 8 * 60 - rccpExView.SwitchTime) * rccpExView.Speed * rccpExView.SpeedTimes)
                                              / (rccpExView.EconomicLotSize * 8 * 60);

                            //废品率
                            rccpExView.ScrapPercentage = rccpTrans.ScrapPercentage;
                            //可用工时还能生产多少米 剩余时间*速度*腔口数
                            double availableQty = (upTime - requireTime) * rccpExView.ConvSpeed;
                            //配额分配
                            double currentQty = planQty;

                            //如果是此物料的最后一条常用生产线,剩余的全部分配给他
                            if (normalExPlans.Last().ProductLine == rccpExView.ProductLine)
                            {
                                currentQty = planQty;
                            }
                            else
                            {
                                currentQty = (rccpExView.Quota / normalQuota) * normalQty;
                            }

                            //取小
                            availableQty = availableQty > currentQty ? currentQty : availableQty;
                            rccpExView.Qty = availableQty;
                            planQty -= availableQty;
                            rccpExView.RequireTime = rccpExView.Qty / rccpExView.ConvSpeed;
                        }
                        #endregion

                        //遍历此断面的生产线,按顺序分配到常用备用上去
                        if (planQty > 0)
                        {
                            double backupQty = planQty;
                            foreach (var rccpExView in backupExPlans)
                            {
                                if (planQty == 0)
                                {
                                    break;
                                }
                                double requireTime = newRccpExViews.Where(p => p.ProductLine == rccpExView.ProductLine).Sum(p => p.RequireTime);
                                double upTime = rccpExView.UpTime;// rccpExView.Correction;
                                if (requireTime > upTime)
                                {
                                    break;
                                }
                                //切换时间折合到速度上去.
                                rccpExView.ConvSpeed = ((rccpExView.EconomicLotSize * 8 * 60 - rccpExView.SwitchTime) * rccpExView.Speed * rccpExView.SpeedTimes)
                                                  / (rccpExView.EconomicLotSize * 8 * 60);

                                //废品率
                                rccpExView.ScrapPercentage = rccpTrans.ScrapPercentage;
                                //可用工时还能生产多少米 剩余时间*速度*腔口数
                                double availableQty = (upTime - requireTime) * rccpExView.ConvSpeed;
                                //配额分配
                                double currentQty = planQty;

                                //如果是此物料的最后一条生产线,剩余的全部分配给他
                                if (backupExPlans.Last().ProductLine == rccpExView.ProductLine)
                                {
                                    currentQty = planQty;
                                }
                                else
                                {
                                    currentQty = (rccpExView.Quota / backupQuota) * backupQty;
                                }

                                //取小
                                availableQty = availableQty > currentQty ? currentQty : availableQty;
                                rccpExView.Qty = availableQty;
                                planQty -= availableQty;
                                rccpExView.RequireTime = rccpExView.Qty / rccpExView.ConvSpeed;
                            }
                        }

                        //如果重用备用都分配后,还有溢出的,分配到权重最大的常用生产线的上面,需后续处理
                        if (planQty > 0)
                        {
                            var rccpExView = rccpExViews.First();

                            rccpExView.Qty += planQty;
                            rccpExView.ConvSpeed = ((rccpExView.EconomicLotSize * 8 * 60 - rccpExView.SwitchTime) * rccpExView.Speed * rccpExView.SpeedTimes)
                                              / (rccpExView.EconomicLotSize * 8 * 60);
                            //时间 = 数量/速度/腔口数
                            rccpExView.RequireTime = rccpExView.Qty / rccpExView.ConvSpeed;
                        }
                    }
                }
                rccpExViewList.AddRange(newRccpExViews);
            }
            //);
            #endregion
            rccpExViewList.AddRange(notAllotRccpExViews);
            return rccpExViewList;
        }

        #endregion

        #endregion View

        #region  Import
        [Transaction(TransactionMode.Requires)]
        public void ReadDailyMrpPlanFromXls(Stream inputStream, DateTime? startDate, DateTime? endDate, string flowCode, bool isItemRef)
        {
            #region 判断
            if (startDate.HasValue)
            {
                if (startDate.Value.Date < DateTime.Now.Date)
                {
                    throw new BusinessException("开始日期必须大于当期日期");
                }
            }
            else
            {
                startDate = DateTime.Now.Date;
            }

            if (endDate.HasValue)
            {
                if (endDate.Value.Date <= DateTime.Now.Date)
                {
                    throw new BusinessException("结束日期必须大于当期日期");
                }
            }
            else
            {
                endDate = DateTime.MaxValue.Date;
            }

            if (startDate.Value > endDate.Value)
            {
                throw new BusinessException("开始日期必须小于结束日期");
            }

            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }
            #endregion 判断

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);
            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();
            IRow dateRow = sheet.GetRow(5);

            ImportHelper.JumpRows(rows, 6);

            #region 列定义
            int colFlow = 0;
            //int colLocation = 0;//库位
            int colItemCode = 1;//物料代码或参考物料号
            //int colItemDescription = 2;//物料描述
            int colUom = 3;//单位
            #endregion

            List<FlowDetail> flowDetailList = new List<FlowDetail>();
            var uomDic = this.genericMgr.FindAll<Uom>().ToDictionary(d => d.Code, d => d);
            if (string.IsNullOrWhiteSpace(flowCode))
            {
                var flowMasters = this.genericMgr.FindAll<FlowMaster>(" from FlowMaster where Type=?",
                    CodeMaster.OrderType.Distribution).ToList();
                foreach (var flowMaster in flowMasters)
                {
                    flowDetailList.AddRange(GetFlowDetails(flowMaster));
                }
            }
            else
            {
                flowDetailList = GetFlowDetails(flowCode).ToList();
            }
            var flowDetailDic = flowDetailList.GroupBy(p => p.Flow, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g);

            List<MrpPlanLog> mrpPlanLogList = new List<MrpPlanLog>();
            BusinessException businessException = new BusinessException();
            while (rows.MoveNext())
            {
                Item item = null;
                string uomCode = null;
                string itemReference = null;
                string flow = null;

                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 0, 3))
                {
                    break;//边界
                }
                string rowIndex = (row.RowNum + 1).ToString();

                #region 读取路线
                try
                {
                    string flowCell = ImportHelper.GetCellStringValue(row.GetCell(colFlow));
                    if (flowCell != null && flowDetailDic.ContainsKey(flowCell))
                    {
                        flow = flowCell;
                    }
                    else
                    {
                        businessException.AddMessage(string.Format("读取路线出错,第{0}行.", rowIndex));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(string.Format("读取路线出错,第{0}行." + ex.Message, rowIndex));
                    continue;
                }
                var flowDetails = flowDetailDic.ValueOrDefault(flow);
                #endregion

                #region 读取物料代码
                try
                {
                    string itemCode = ImportHelper.GetCellStringValue(row.GetCell(colItemCode));
                    if (itemCode == null)
                    {
                        businessException.AddMessage(string.Format("物料号不能为空,第{0}行", rowIndex));
                        continue;
                    }
                    if (isItemRef)
                    {
                        item = this.genericMgr.FindAll<Item>("from Item as i where i.ReferenceCode = ? ", new object[] { itemCode }, 0, 1).First();
                        itemReference = itemCode;
                    }
                    else
                    {
                        item = this.itemMgr.GetCacheItem(itemCode);
                        itemReference = item.ReferenceCode;
                    }
                    if (item == null)
                    {
                        businessException.AddMessage(string.Format("物料号{0}不存在,第{1}行.", itemCode, rowIndex));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(ex.Message + ". 第" + rowIndex + "行.");
                    continue;
                }
                #endregion

                #region 读取单位
                try
                {
                    string uomCell = ImportHelper.GetCellStringValue(row.GetCell(colUom));
                    uomCode = uomDic.ValueOrDefault(uomCell).Code;
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(ex.Message + ". 第" + rowIndex + "行.");
                    continue;
                }
                #endregion

                #region 使用flowDetails过滤

                var q = flowDetails.Where(f => f.Item == item.Code && f.Uom == uomCode);
                string locationCode = string.Empty;
                if (q.Count() > 0)
                {
                    locationCode = !string.IsNullOrWhiteSpace(q.FirstOrDefault().LocationFrom) ? q.FirstOrDefault().LocationFrom : q.FirstOrDefault().CurrentFlowMaster.LocationFrom;
                }
                else
                {
                    if (flowDetails.First().CurrentFlowMaster.IsManualCreateDetail)
                    {
                        uomCode = uomCode == null ? item.Uom : uomCode;
                        locationCode = flowDetails.First().CurrentFlowMaster.LocationFrom;
                    }
                    else
                    {
                        businessException.AddMessage(string.Format("没有找到匹配的路线明细,第{0}行.物料{1},单位{2}", rowIndex, item.Code, uomCode));
                        continue;
                    }
                }

                #endregion

                #region 读取数量
                try
                {
                    for (int i = 4; ; i++)
                    {
                        ICell dateCell = dateRow.GetCell(i);

                        #region 读取计划日期
                        DateTime currentDate = DateTime.Now;
                        string weekOfYear = string.Empty;

                        if (dateCell != null)
                        {
                            if (dateCell.CellType == CellType.NUMERIC)
                            {
                                currentDate = dateCell.DateCellValue;
                            }
                            else if (dateCell.CellType == CellType.STRING)
                            {
                                currentDate = DateTime.Parse(dateCell.StringCellValue);
                            }
                        }
                        else
                        {
                            break;
                        }

                        if (startDate.HasValue && currentDate.Date < startDate.Value.Date)
                        {
                            continue;
                        }
                        if (endDate.HasValue && currentDate.Date > endDate.Value.Date)
                        {
                            break;
                        }
                        #endregion

                        double qty = 0;
                        if (row.GetCell(i) != null)
                        {
                            if (row.GetCell(i).CellType == CellType.NUMERIC)
                            {
                                qty = row.GetCell(i).NumericCellValue;
                            }
                            else
                            {
                                string qtyValue = ImportHelper.GetCellStringValue(row.GetCell(i));
                                if (qtyValue != null)
                                {
                                    qty = Convert.ToDouble(qtyValue);
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (qty < 0)
                        {
                            businessException.AddMessage(string.Format("数量必须大于0,第{0}行", rowIndex));
                            continue;
                        }
                        else
                        {
                            MrpPlanLog mrpPlanLog = new MrpPlanLog();
                            decimal unitQty = itemMgr.ConvertItemUomQty(item.Code, uomCode, 1, item.Uom);
                            mrpPlanLog.PlanDate = currentDate;
                            mrpPlanLog.Item = item.Code;
                            //dailyPlanLog.PlanVersion = dailyPlan.PlanVersion;
                            mrpPlanLog.Flow = flowDetails.First().CurrentFlowMaster.Code;
                            mrpPlanLog.Location = locationCode;
                            mrpPlanLog.Party = flowDetails.First().CurrentFlowMaster.PartyTo;
                            mrpPlanLog.OrderType = flowDetails.First().CurrentFlowMaster.Type;
                            mrpPlanLog.Uom = uomCode;
                            mrpPlanLog.Qty = qty;
                            mrpPlanLog.UnitQty = unitQty;
                            mrpPlanLog.ItemDescription = item.CodeDescription;
                            mrpPlanLog.ItemReference = item.ReferenceCode;
                            //dailyPlanLog.CreateDate = System.DateTime.Now;
                            //dailyPlanLog.CreateUserId = user.Id;
                            //dailyPlanLog.CreateUserName = user.FullName;
                            mrpPlanLogList.Add(mrpPlanLog);
                        }
                    }
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(string.Format("读取数量出错,第{0}行." + ex.Message, rowIndex));
                }
                #endregion
            }

            var mrpPlanLogGroups = mrpPlanLogList.GroupBy(p => p.Flow, (k, g) => new { k, g });
            foreach (var mrpPlanLogGroup in mrpPlanLogGroups)
            {
                CreateMrpPlan(mrpPlanLogGroup.k, mrpPlanLogGroup.g.ToList());
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void ReadWeeklyMrpPlanFromXls(Stream inputStream, string startWeek, string endWeek, string flowCode, bool isItemRef)
        {
            #region 判断
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(startWeek))
            {
                startDate = DateTimeHelper.GetWeekIndexDateFrom(startWeek);
            }
            if (!string.IsNullOrEmpty(endWeek))
            {
                endDate = DateTimeHelper.GetWeekIndexDateFrom(endWeek);
            }

            if (startDate.HasValue)
            {
                if (startDate.Value.Date < DateTime.Now.Date)
                {
                    throw new BusinessException("开始日期必须大于当期日期");
                }
            }
            else
            {
                startDate = DateTime.Now.Date;
            }

            if (endDate.HasValue)
            {
                if (endDate.Value.Date <= DateTime.Now.Date)
                {
                    throw new BusinessException("结束日期必须大于当期日期");
                }
            }
            else
            {
                endDate = DateTime.MaxValue.Date;
            }

            if (startDate.Value > endDate.Value)
            {
                throw new BusinessException("开始日期必须小于结束日期");
            }

            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }
            #endregion 判断

            //var flowDetails = GetFlowDetails(flowCode);
            List<FlowDetail> flowDetailList = new List<FlowDetail>();
            var uomDic = this.genericMgr.FindAll<Uom>().ToDictionary(d => d.Code, d => d);
            if (string.IsNullOrWhiteSpace(flowCode))
            {
                var flowMasters = this.genericMgr.FindAll<FlowMaster>(" from FlowMaster where Type=?",
                    CodeMaster.OrderType.Distribution).ToList();
                foreach (var flowMaster in flowMasters)
                {
                    flowDetailList.AddRange(GetFlowDetails(flowMaster));
                }
            }
            else
            {
                flowDetailList = GetFlowDetails(flowCode).ToList();
            }
            var flowDetailDic = flowDetailList.GroupBy(p => p.CurrentFlowMaster.Code, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g);


            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);
            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();
            IRow dateRow = sheet.GetRow(5);
            ImportHelper.JumpRows(rows, 6);

            var mrpPlanLogList = new List<MrpPlanLog>();

            int days = 7;
            try
            {
                days = (int)sheet.GetRow(4).GetCell(1).NumericCellValue;
            }
            catch (Exception)
            { }

            #region 列定义
            //int colFlow = 0;//
            int colFlow = 0;//
            int colItemCode = 1;//物料代码或参考物料号
            //int colItemDescription = 2;//物料描述
            int colUom = 3;//单位
            #endregion

            BusinessException businessException = new BusinessException();

            while (rows.MoveNext())
            {
                string flow = null;
                Item item = null;
                string uomCode = null;
                string itemReference = null;

                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 0, 3))
                {
                    break;//边界
                }
                string rowIndex = (row.RowNum + 1).ToString();

                #region 读取路线
                try
                {
                    string flowCell = ImportHelper.GetCellStringValue(row.GetCell(colFlow));
                    if (flowCell != null && flowDetailDic.ContainsKey(flowCell))
                    {
                        flow = flowCell;
                    }
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(string.Format("读取库位出错,第{0}行." + ex.Message, rowIndex));
                    continue;
                }
                var flowDetails = flowDetailDic.ValueOrDefault(flow);
                #endregion

                #region 读取物料代码
                try
                {
                    string itemCode = ImportHelper.GetCellStringValue(row.GetCell(colItemCode));
                    if (itemCode == null)
                    {
                        businessException.AddMessage(string.Format("物料号不存在,第{0}行", rowIndex));
                        continue;
                    }
                    if (isItemRef)
                    {
                        item = this.genericMgr.FindAll<Item>("from Item as i where i.ReferenceCode = ? ", new object[] { itemCode }, 0, 1).First();
                        itemReference = itemCode;
                    }
                    else
                    {
                        item = this.genericMgr.FindById<Item>(itemCode);
                        itemReference = item.ReferenceCode;
                    }
                    if (item == null)
                    {
                        businessException.AddMessage(string.Format("物料号{0}不存在,第{1}行", itemCode, rowIndex));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(string.Format("读取物料号出错,第{0}行" + ex.Message, rowIndex));
                    continue;
                }
                #endregion

                #region 读取单位
                try
                {
                    string uomCell = ImportHelper.GetCellStringValue(row.GetCell(colUom));
                    uomCode = uomDic.ValueOrDefault(uomCell).Code;
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(string.Format("读取单位出错,第{0}行," + ex.Message, rowIndex));
                    continue;
                }
                #endregion

                #region 使用flowDetails过滤
                string locationCode = string.Empty;
                var q = flowDetails.Where(f => f.Item == item.Code && f.Uom == uomCode);
                if (q.Count() > 0)
                {
                    locationCode = !string.IsNullOrWhiteSpace(q.FirstOrDefault().LocationFrom) ? q.FirstOrDefault().LocationFrom : q.FirstOrDefault().CurrentFlowMaster.LocationFrom;
                }
                else
                {
                    if (flowDetails.First().CurrentFlowMaster.IsManualCreateDetail)
                    {
                        uomCode = uomCode == null ? item.Uom : uomCode;
                        locationCode = flowDetails.First().CurrentFlowMaster.LocationFrom;
                    }
                    else
                    {
                        businessException.AddMessage("没有找到匹配的路线明细,第{0}行.物料号{1},单位{2},库位{3}", rowIndex, item.Code, uomCode, flow);
                        continue;
                    }
                }
                #endregion

                #region 读取数量
                try
                {
                    for (int i = 4; ; i++)
                    {
                        ICell dateCell = dateRow.GetCell(i);
                        string weekOfYear = null;

                        #region 读取计划日期
                        if (dateCell != null)
                        {
                            weekOfYear = ImportHelper.GetCellStringValue(dateCell);
                            if (string.IsNullOrWhiteSpace(weekOfYear))
                            {
                                break;
                            }

                            if (weekOfYear.CompareTo(startWeek) < 0)
                            {
                                continue;
                            }
                            if (weekOfYear.CompareTo(endWeek) > 0)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                        #endregion

                        double qty = 0;
                        if (row.GetCell(i) != null)
                        {
                            if (row.GetCell(i).CellType == CellType.NUMERIC)
                            {
                                qty = row.GetCell(i).NumericCellValue;
                            }
                            else
                            {
                                string qtyValue = ImportHelper.GetCellStringValue(row.GetCell(i));
                                if (qtyValue != null)
                                {
                                    qty = Convert.ToDouble(qtyValue);
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (qty < 0)
                        {
                            businessException.AddMessage("导入数量不能小于0,第{0}行", rowIndex);
                            continue;
                        }
                        else
                        {
                            double dayQty = Math.Round(qty / days);
                            DateTime currentDate = DateTimeHelper.GetWeekIndexDateFrom(weekOfYear);
                            for (int j = 0; j < days; j++)
                            {
                                MrpPlanLog mrpPlanLog = new MrpPlanLog();
                                decimal unitQty = itemMgr.ConvertItemUomQty(item.Code, uomCode, 1, item.Uom);
                                mrpPlanLog.UnitQty = unitQty;
                                mrpPlanLog.ItemDescription = item.CodeDescription;
                                mrpPlanLog.ItemReference = item.ReferenceCode;

                                mrpPlanLog.PlanDate = currentDate;
                                mrpPlanLog.Item = item.Code;
                                mrpPlanLog.Party = flowDetails.First().CurrentFlowMaster.PartyTo;
                                mrpPlanLog.OrderType = flowDetails.First().CurrentFlowMaster.Type;
                                //dailyPlanLog.PlanVersion = dailyPlan.PlanVersion;
                                mrpPlanLog.Flow = flowDetails.First().CurrentFlowMaster.Code;
                                mrpPlanLog.Location = locationCode;
                                mrpPlanLog.Uom = uomCode;

                                if (j == days - 1)
                                {
                                    mrpPlanLog.Qty = qty - dayQty * (days - 1);
                                }
                                else
                                {
                                    mrpPlanLog.Qty = dayQty;
                                }
                                currentDate = currentDate.AddDays(1);
                                mrpPlanLogList.Add(mrpPlanLog);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(ex.Message);
                }
                #endregion
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }

            var mrpPlanLogGroups = mrpPlanLogList.GroupBy(p => p.Flow, (k, g) => new { k, g });
            foreach (var mrpPlanLogGroup in mrpPlanLogGroups)
            {
                CreateMrpPlan(mrpPlanLogGroup.k, mrpPlanLogGroup.g.ToList());
            }
        }

        private IList<FlowDetail> GetFlowDetails(FlowMaster flowMaster)
        {
            #region 读取路线
            var flowDetails = this.flowMgr.GetFlowDetailList(flowMaster, false, true);
            if (flowDetails == null || flowDetails.Count() == 0)
            {
                if (flowMaster.IsManualCreateDetail)
                {
                    flowDetails = new List<FlowDetail>();
                    var flowDetail = new FlowDetail();
                    flowDetail.CurrentFlowMaster = flowMaster;
                    flowDetails.Add(flowDetail);
                }
            }
            #endregion
            return flowDetails;
        }

        private IList<FlowDetail> GetFlowDetails(string flowCode)
        {
            if (flowCode == null || flowCode.Trim() == string.Empty)
            {
                throw new BusinessException("MRP.Plan.Import.CustomerPlan.Result.SelectFlow");
            }
            FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(flowCode);
            return GetFlowDetails(flowMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateMrpPlan(string flowCode, List<MrpPlanLog> mrpPlanLogList)
        {
            var flowStategy = this.genericMgr.FindAll<FlowStrategy>(" from FlowStrategy where Flow =? ", flowCode).First();
            var leadTime = DateTimeHelper.TimeTranfer(flowStategy.LeadTime, flowStategy.TimeUnit, CodeMaster.TimeUnit.Day);

            var user = SecurityContextHolder.Get();
            var mrpPlanDic = this.genericMgr.FindAll<MrpPlan>
                ("select d from MrpPlan as d where d.PlanDate>=? and  d.Flow=? ",
                new object[] { mrpPlanLogList.Min(p => p.PlanDate).AddDays(-leadTime), flowCode })
                 .GroupBy(p => p.Item, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.ToList());

            int planVersion = int.Parse(numberControlMgr.GetNextSequence(com.Sconit.Entity.MRP.BusinessConstants.NUMBERCONTROL_DAILYLYPLAN + "_" + flowCode));
            foreach (var mrpPlanLog in mrpPlanLogList)
            {
                mrpPlanLog.WindowTime = mrpPlanLog.PlanDate;
                //提前期
                mrpPlanLog.PlanDate = mrpPlanLog.WindowTime.AddDays(-Math.Round(leadTime));

                var q_mrpPlan = (mrpPlanDic.ValueOrDefault(mrpPlanLog.Item) ?? new List<MrpPlan>())
                    .Where(p => p.Flow == flowCode && p.PlanDate == mrpPlanLog.PlanDate && p.Location == mrpPlanLog.Location);

                MrpPlan mrpPlan = new MrpPlan();
                if (q_mrpPlan.Count() == 0)
                {
                    #region New DailyPlan
                    mrpPlan.Flow = flowCode;
                    mrpPlan.PlanDate = mrpPlanLog.PlanDate;
                    mrpPlan.Item = mrpPlanLog.Item;
                    mrpPlan.Location = mrpPlanLog.Location;
                    mrpPlan.Qty = mrpPlanLog.Qty * (double)mrpPlanLog.UnitQty;
                    mrpPlan.PlanVersion = planVersion;
                    mrpPlan.Party = mrpPlanLog.Party;
                    mrpPlan.OrderType = mrpPlanLog.OrderType;
                    this.genericMgr.Create(mrpPlan);
                    #endregion

                    #region New DailyPlanLog
                    mrpPlanLog.PlanVersion = mrpPlan.PlanVersion;
                    mrpPlanLog.CreateDate = System.DateTime.Now;
                    mrpPlanLog.CreateUserId = user.Id;
                    mrpPlanLog.CreateUserName = user.FullName;
                    this.genericMgr.Create(mrpPlanLog);
                    #endregion
                }
                else if (q_mrpPlan.Count() == 1)
                {
                    if (q_mrpPlan.First().Qty != mrpPlanLog.Qty * (double)mrpPlanLog.UnitQty)
                    {
                        mrpPlan = q_mrpPlan.First();
                        mrpPlan.Qty = mrpPlanLog.Qty * (double)mrpPlanLog.UnitQty;
                        mrpPlan.PlanVersion = planVersion;
                        this.genericMgr.Update(mrpPlan);

                        #region New DailyPlanlog
                        mrpPlanLog.PlanVersion = mrpPlan.PlanVersion;
                        mrpPlanLog.CreateDate = System.DateTime.Now;
                        mrpPlanLog.CreateUserId = user.Id;
                        mrpPlanLog.CreateUserName = user.FullName;
                        this.genericMgr.Create(mrpPlanLog);
                        #endregion
                    }
                }
                else
                {
                    throw new TechnicalException("Error:DateException");
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void ReadRccpPlanFromXls(Stream inputStream, DateTime startDate, DateTime endDate, bool isItemRef, com.Sconit.CodeMaster.TimeUnit dateType)
        {
            #region  判断

            if (startDate > endDate)
            {
                throw new BusinessException("开始日期必须小于结束日期");
            }
            #endregion 判断

            #region Import
            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);

            IEnumerator rows = sheet.GetRowEnumerator();

            IRow dateRow = sheet.GetRow(5);

            ImportHelper.JumpRows(rows, 6);

            var rccpPlanLogList = new List<RccpPlanLog>();

            #region 列定义
            int colFlow = 0;//路线
            int colItemCode = 1;//物料代码或参考物料号
            int colUom = 3;//单位
            #endregion
            BusinessException businessException = new BusinessException();

            while (rows.MoveNext())
            {
                Item item = null;
                string uomCode = null;
                string itemReference = null;
                string flowCode = null;

                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 0, 3))
                {
                    break;//边界
                }
                string rowIndex = (row.RowNum + 1).ToString();

                #region 读取路线代码
                flowCode = ImportHelper.GetCellStringValue(row.GetCell(colFlow));
                #endregion

                #region 读取物料代码
                try
                {
                    string itemCode = ImportHelper.GetCellStringValue(row.GetCell(colItemCode));
                    if (itemCode == null)
                    {
                        businessException.AddMessage(string.Format("物料不能为空,第{0}行", rowIndex));
                        continue;
                    }
                    if (isItemRef)
                    {
                        item = this.genericMgr.FindAll<Item>("from Item as i where i.ReferenceCode = ? ", new object[] { itemCode }, 0, 1).First();
                        itemReference = itemCode;
                    }
                    else
                    {
                        item = this.itemMgr.GetCacheItem(itemCode);
                        itemReference = item.ReferenceCode;
                    }
                    if (item == null)
                    {
                        businessException.AddMessage(string.Format("物料号{0}不存在,第{1}行.", itemCode, rowIndex));
                        continue;
                    }
                    if (item.ItemCategory != "MODEL")
                    {
                        businessException.AddMessage(string.Format("物料号{0}不是车型,第{1}行.", itemCode, rowIndex));
                        continue;
                    }
                }
                catch
                {
                    businessException.AddMessage(string.Format("读取物料时出错,第{0}行.", rowIndex));
                    continue;
                }
                #endregion

                #region 读取单位
                try
                {
                    string uomCell = ImportHelper.GetCellStringValue(row.GetCell(colUom));
                    uomCode = this.genericMgr.FindById<Uom>(uomCell).Code;
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(string.Format("读取单位出错,第{0}行." + ex.Message, rowIndex));
                    continue;
                }

                #endregion

                #region 读取数量
                try
                {
                    for (int i = 4; ; i++)
                    {
                        ICell dateCell = dateRow.GetCell(i);
                        string dateIndex = null;

                        #region 读取计划日期
                        if (dateCell != null)
                        {
                            if (dateCell.CellType == CellType.STRING)
                            {
                                dateIndex = dateCell.StringCellValue;
                            }
                            else
                            {
                                if (dateType == CodeMaster.TimeUnit.Month)
                                {
                                    if (dateCell.CellType == CellType.NUMERIC)
                                    {
                                        dateIndex = dateCell.DateCellValue.ToString("yyyy-MM");
                                    }
                                    else
                                    {
                                        throw new BusinessException("月的时间索引必须为文本或日期格式");
                                    }
                                }
                                else if (dateType == CodeMaster.TimeUnit.Day)
                                {
                                    if (dateCell.CellType == CellType.NUMERIC)
                                    {
                                        dateIndex = dateCell.DateCellValue.ToString("yyyy-MM-dd");
                                    }
                                    else
                                    {
                                        throw new BusinessException("天的时间索引必须为文本或日期格式");
                                    }
                                }
                                else
                                {
                                    throw new BusinessException("周的时间索引必须为文本格式");
                                }
                            }

                            if (string.IsNullOrWhiteSpace(dateIndex))
                            {
                                break;
                            }
                            DateTime currentDateTime = DateTime.Now;
                            if (dateType == CodeMaster.TimeUnit.Week)
                            {
                                currentDateTime = DateTimeHelper.GetWeekIndexDateFrom(dateIndex);
                            }
                            else if (dateType == CodeMaster.TimeUnit.Month)
                            {
                                if (!DateTime.TryParse(dateIndex + "-01", out currentDateTime))
                                {
                                    businessException.AddMessage("日期{0}格式无效", dateIndex);
                                    continue;
                                }
                            }
                            else if (dateType == CodeMaster.TimeUnit.Day)
                            {
                                if (!DateTime.TryParse(dateIndex, out currentDateTime))
                                {
                                    businessException.AddMessage("日期{0}格式无效", dateIndex);
                                    continue;
                                }
                            }

                            if (currentDateTime.CompareTo(startDate) < 0)
                            {
                                continue;
                            }
                            if (currentDateTime.CompareTo(endDate) > 0)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                        #endregion

                        double qty = 0;
                        if (row.GetCell(i) != null)
                        {
                            if (row.GetCell(i).CellType == CellType.NUMERIC)
                            {
                                qty = row.GetCell(i).NumericCellValue;
                            }
                            else
                            {
                                string qtyValue = ImportHelper.GetCellStringValue(row.GetCell(i));
                                if (qtyValue != null)
                                {
                                    qty = Convert.ToDouble(qtyValue);
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (qty < 0)
                        {
                            businessException.AddMessage(string.Format("数量需大于0,第{0}行", rowIndex));
                            continue;
                        }
                        else
                        {
                            decimal unitQty = itemMgr.ConvertItemUomQty(item.Code, uomCode, 1, item.Uom);
                            RccpPlanLog rccpPlanLog = new RccpPlanLog();
                            rccpPlanLog.Flow = flowCode;
                            rccpPlanLog.DateIndexTo = dateIndex;
                            rccpPlanLog.DateIndex = dateIndex;
                            rccpPlanLog.Item = item.Code;
                            rccpPlanLog.Uom = uomCode;
                            rccpPlanLog.DateType = dateType;
                            rccpPlanLog.Qty = qty;
                            rccpPlanLog.UnitQty = unitQty;
                            rccpPlanLog.ItemDescription = item.CodeDescription;
                            rccpPlanLog.ItemReference = item.ReferenceCode;

                            rccpPlanLogList.Add(rccpPlanLog);
                        }
                    }
                }
                catch (Exception ex)
                {
                    businessException.AddMessage(ex.Message);
                }
                #endregion
            }
            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            var flowMasters = this.genericMgr.FindAll<FlowMaster>
              (@"from FlowMaster as m where m.IsActive = ? and Type=? ",
              new object[] { true, CodeMaster.OrderType.Distribution });

            #region Day
            if (dateType == CodeMaster.TimeUnit.Day)
            {
                var flowDetails = new List<FlowDetail>();
                foreach (var flow in flowMasters)
                {
                    foreach (var flowDetail in flowMgr.GetFlowDetailList(flow, false, true))
                    {
                        if (flowDetail.MrpWeight > 0)
                        {
                            //flowDetail.DefaultLocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                            //flowDetail.DefaultLocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                            flowDetails.Add(flowDetail);
                        }
                    }
                }

                var flowDetailDic = flowDetails.GroupBy(p => new { Flow = p.Flow, Item = p.Item }).ToDictionary(d => d.Key, d => d.First());

                var mrpPlanLogs = new List<MrpPlanLog>();
                foreach (var rccpPlanLog in rccpPlanLogList)
                {
                    try
                    {
                        var bomDetails = bomMgr.GetFlatBomDetail(rccpPlanLog.Item, DateTime.Parse(rccpPlanLog.DateIndex), true);
                        var bomMaster = this.bomMgr.GetCacheBomMaster(rccpPlanLog.Item);
                        var qty = itemMgr.ConvertItemUomQty(rccpPlanLog.Item, rccpPlanLog.Uom, (decimal)rccpPlanLog.Qty, bomMaster.Uom);
                        foreach (var bomDetail in bomDetails)
                        {
                            var flowDetail = flowDetailDic.ValueOrDefault(new { Flow = rccpPlanLog.Flow, Item = bomDetail.Item });
                            if (flowDetail == null)
                            {
                                businessException.AddMessage("销售路线{0}中不存在此物料{1}", rccpPlanLog.Flow, bomDetail.Item);
                            }
                            else
                            {
                                var mrpPlanLog = new MrpPlanLog();
                                mrpPlanLog.Flow = rccpPlanLog.Flow;
                                mrpPlanLog.Item = bomDetail.Item;
                                Item bomItem = this.itemMgr.GetCacheItem(bomDetail.Item);
                                mrpPlanLog.ItemDescription = bomItem.Description;
                                mrpPlanLog.ItemReference = bomItem.ReferenceCode;
                                mrpPlanLog.Location = flowDetail.DefaultLocationFrom;
                                mrpPlanLog.OrderType = flowDetail.CurrentFlowMaster.Type;
                                mrpPlanLog.Party = flowDetail.CurrentFlowMaster.PartyTo;
                                mrpPlanLog.PlanDate = DateTime.Parse(rccpPlanLog.DateIndex);
                                var item = this.itemMgr.GetCacheItem(mrpPlanLog.Item);
                                mrpPlanLog.Qty = (double)itemMgr.ConvertItemUomQty(rccpPlanLog.Item, bomDetail.Uom, qty * bomDetail.CalculatedQty, item.Uom);
                                decimal unitQty = itemMgr.ConvertItemUomQty(rccpPlanLog.Item, flowDetail.Uom, 1, item.Uom);
                                mrpPlanLog.UnitQty = unitQty;
                                mrpPlanLog.Uom = flowDetail.Uom;
                                mrpPlanLogs.Add(mrpPlanLog);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        businessException.AddMessage(string.Format("分解bom{0}出错", rccpPlanLog.Item));
                    }
                }

                if (businessException.HasMessage)
                {
                    throw businessException;
                }

                var groupMrpPlanLogs = mrpPlanLogs.GroupBy(p => p.Flow, (k, g) =>
                    new
                    {
                        k,
                        List = from q in g
                               group q by new
                               {
                                   q.PlanDate,
                                   q.Item,
                                   q.PlanVersion,
                                   q.Flow,
                                   q.Location
                               } into result
                               select new MrpPlanLog
                               {
                                   PlanDate = result.Key.PlanDate,
                                   Item = result.Key.Item,
                                   PlanVersion = result.Key.PlanVersion,
                                   Flow = result.Key.Flow,
                                   Location = result.Key.Location,
                                   Qty = result.Sum(r => r.Qty),
                                   Uom = result.First().Uom,
                                   UnitQty = result.First().UnitQty,
                                   ItemDescription = result.First().ItemDescription,
                                   ItemReference = result.First().ItemReference,
                                   Party = result.First().Party,
                                   OrderType = result.First().OrderType
                               }
                    });

                foreach (var groupMrpPlanLog in groupMrpPlanLogs)
                {
                    this.CreateMrpPlan(groupMrpPlanLog.k, groupMrpPlanLog.List.ToList());
                }
            }
            #endregion
            CreateRccpPlan(dateType, rccpPlanLogList);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateRccpPlan(CodeMaster.TimeUnit dateType, List<RccpPlanLog> rccpPlanLogList)
        {
            var flowStategyDic = new Dictionary<string, FlowStrategy>();
            var flowCodes = rccpPlanLogList.Select(p => p.Flow).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct();
            if (flowCodes != null && flowCodes.Count() > 0)
            {
                flowStategyDic = this.genericMgr.FindAllIn<FlowStrategy>
                 (" from FlowStrategy where Flow in(? ", rccpPlanLogList.Select(p => p.Flow).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct())
                 .ToDictionary(d => d.Flow, d => d);
            }

            var user = SecurityContextHolder.Get();
            var oldRccpPlanDic = (this.genericMgr.FindAllIn<RccpPlan>
                    ("from RccpPlan as w where w.DateType = ? and w.DateIndexTo in(?",
                    rccpPlanLogList.Select(p => p.DateIndexTo).Distinct(), new object[] { dateType }) ?? new List<RccpPlan>())
                    .GroupBy(p => p.Item, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.ToList());

            int planVersion = 0;
            if (dateType == CodeMaster.TimeUnit.Month)
            {
                planVersion = int.Parse(numberControlMgr.GetNextSequence(com.Sconit.Entity.MRP.BusinessConstants.NUMBERCONTROL_MONTHLYPLAN));
            }
            else if (dateType == CodeMaster.TimeUnit.Week)
            {
                planVersion = int.Parse(numberControlMgr.GetNextSequence(com.Sconit.Entity.MRP.BusinessConstants.NUMBERCONTROL_WEEKLYPLAN));
            }
            else if (dateType == CodeMaster.TimeUnit.Day)
            {
                planVersion = int.Parse(numberControlMgr.GetNextSequence(com.Sconit.Entity.MRP.BusinessConstants.NUMBERCONTROL_DAILYLYPLAN));
            }

            foreach (var rccpPlanLog in rccpPlanLogList)
            {
                string dateIndexFrom = rccpPlanLog.DateIndexTo;
                if (!string.IsNullOrWhiteSpace(rccpPlanLog.Flow))
                {
                    var flowStategy = flowStategyDic.ValueOrDefault(rccpPlanLog.Flow);
                    if (flowStategy != null)
                    {
                        decimal _leadTime = dateType == CodeMaster.TimeUnit.Day ? flowStategy.LeadTime : flowStategy.RccpLeadTime;
                        double leadTime = Math.Round(DateTimeHelper.TimeTranfer(_leadTime, flowStategy.TimeUnit, CodeMaster.TimeUnit.Day));
                        if (leadTime > 0)
                        {
                            if (dateType == CodeMaster.TimeUnit.Day)
                            {
                                dateIndexFrom = DateTime.Parse(rccpPlanLog.DateIndex).AddDays(-leadTime).ToString("yyyy-MM-dd");
                            }
                            else if (dateType == CodeMaster.TimeUnit.Week)
                            {
                                dateIndexFrom = Utility.DateTimeHelper.GetWeekOfYear(
                                    Utility.DateTimeHelper.GetWeekIndexDateFrom(rccpPlanLog.DateIndex).AddDays(3 - leadTime)
                                    );
                            }
                            else if (dateType == CodeMaster.TimeUnit.Month)
                            {
                                dateIndexFrom = DateTime.Parse(rccpPlanLog.DateIndex + "-15").AddDays(-leadTime).ToString("yyyy-MM");
                            }
                        }
                    }
                }

                var oldRccpPlans = (oldRccpPlanDic.ValueOrDefault(rccpPlanLog.Item) ?? new List<RccpPlan>())
                    .Where(p => p.DateIndex == rccpPlanLog.DateIndex && p.Flow == rccpPlanLog.Flow && p.DateType == rccpPlanLog.DateType);
                if (oldRccpPlans.Count() == 0)
                {
                    #region New RccpPlan
                    var rccpPlan = new RccpPlan();
                    rccpPlan.DateIndex = dateIndexFrom;
                    rccpPlan.DateIndexTo = rccpPlanLog.DateIndexTo;
                    rccpPlan.Item = rccpPlanLog.Item;
                    rccpPlan.Flow = rccpPlanLog.Flow;
                    rccpPlan.Qty = rccpPlanLog.Qty * (double)rccpPlanLog.UnitQty;
                    rccpPlan.PlanVersion = planVersion;
                    rccpPlan.DateType = dateType;

                    this.genericMgr.Create(rccpPlan);
                    #endregion

                    #region New RccpPlanLog
                    rccpPlanLog.PlanId = rccpPlan.Id;
                    rccpPlanLog.PlanVersion = planVersion;
                    rccpPlanLog.DateIndex = dateIndexFrom;
                    rccpPlanLog.DateType = dateType;
                    rccpPlanLog.CreateDate = System.DateTime.Now;
                    rccpPlanLog.CreateUserId = user.Id;
                    rccpPlanLog.CreateUserName = user.FullName;
                    this.genericMgr.Create(rccpPlanLog);
                    #endregion
                }
                else if (oldRccpPlans.Count() == 1)
                {
                    var oldRccpPlan = oldRccpPlans.First();
                    if (oldRccpPlan.Qty != rccpPlanLog.Qty * (double)rccpPlanLog.UnitQty
                        || oldRccpPlan.Flow != rccpPlanLog.Flow)
                    {
                        oldRccpPlan.Qty = rccpPlanLog.Qty * (double)rccpPlanLog.UnitQty;
                        oldRccpPlan.DateIndex = dateIndexFrom;
                        oldRccpPlan.PlanVersion = planVersion;
                        oldRccpPlan.Flow = rccpPlanLog.Flow;
                        this.genericMgr.Update(oldRccpPlan);

                        #region New RccpPlanLog
                        rccpPlanLog.PlanId = oldRccpPlan.Id;
                        rccpPlanLog.DateIndex = dateIndexFrom;
                        rccpPlanLog.PlanVersion = planVersion;
                        rccpPlanLog.DateType = dateType;
                        rccpPlanLog.CreateDate = System.DateTime.Now;
                        rccpPlanLog.CreateUserId = user.Id;
                        rccpPlanLog.CreateUserName = user.FullName;
                        this.genericMgr.Create(rccpPlanLog);
                        #endregion
                    }
                }
                else
                {
                    throw new TechnicalException("Error:DateException");
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void ReadRccpPlanFromXls(Stream inputStream, string startDateIndex, string endDateIndex, bool isItemRef, CodeMaster.TimeUnit dateType)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            #region  判断
            if (!string.IsNullOrEmpty(startDateIndex))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    startDate = DateTimeHelper.GetWeekIndexDateFrom(startDateIndex);
                }
                else if (dateType == CodeMaster.TimeUnit.Month)
                {
                    startDate = DateTime.Parse(startDateIndex + "-01");
                }
                else
                {
                    throw new BusinessException("不能导入此计划类型:" + dateType.ToString());
                }
            }
            if (!string.IsNullOrEmpty(endDateIndex))
            {
                if (dateType == CodeMaster.TimeUnit.Week)
                {
                    endDate = DateTimeHelper.GetWeekIndexDateFrom(endDateIndex);
                }
                else
                {
                    endDate = DateTime.Parse(endDateIndex + "-01");
                }
            }
            #endregion 判断
            ReadRccpPlanFromXls(inputStream, startDate, endDate, isItemRef, dateType);
        }

        [Transaction(TransactionMode.Requires)]
        public List<MrpShiftPlan> ReadShiftPlanFromXls(Stream inputStream, DateTime startDate, DateTime endDate, string flow)
        {
            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);
            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();
            IRow dateRow = sheet.GetRow(2);
            ImportHelper.JumpRows(rows, 3);
            #region 列定义
            int colFlow = 0;// 路线
            int colItemCode = 1;//物料代码或参考物料号
            //int colItemDescription = 2;//物料描述
            int colUom = 3;//单位
            #endregion

            var shiftPlanList = new List<MrpShiftPlan>();
            #region 读取数据
            while (rows.MoveNext())
            {
                string itemCode = null;
                string uomCode = null;
                string flowCode = null;
                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 0, 4))
                {
                    break;//边界
                }
                string rowIndex = (row.RowNum + 1).ToString();

                #region 读取路线
                try
                {
                    flowCode = ImportHelper.GetCellStringValue(row.GetCell(colFlow));
                    if (flowCode == null)
                    {
                        throw new BusinessException("生产线不能为空", rowIndex);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(flow) && flow != flowCode)
                        {
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new BusinessException(ex.Message);
                }
                #endregion

                #region 读取物料代码
                try
                {
                    itemCode = ImportHelper.GetCellStringValue(row.GetCell(colItemCode));
                    if (itemCode == null)
                    {
                        throw new BusinessException("Import.ShipPlan.ItemCode.Empty", rowIndex);
                    }
                }
                catch
                {
                    throw new BusinessException("Import.ShipPlan.ItemCode.Error", rowIndex);
                }
                #endregion

                #region 读取单位
                try
                {
                    uomCode = ImportHelper.GetCellStringValue(row.GetCell(colUom));
                }
                catch (Exception ex)
                {
                    throw new BusinessException(ex.Message);
                }
                #endregion

                #region 读取数量
                try
                {
                    for (int i = 5; ; i++)
                    {
                        ICell dateCell = dateRow.GetCell(i - ((i - 5) % 3));

                        #region 读取计划日期
                        DateTime planDate = DateTime.Now;
                        string weekOfYear = string.Empty;

                        if (dateCell != null && dateCell.CellType == CellType.NUMERIC)
                        {
                            planDate = dateCell.DateCellValue;
                        }
                        else
                        {
                            break;
                        }
                        if (planDate.Date <= DateTime.Now)
                        {
                            continue;
                        }

                        if (planDate.Date < startDate.Date)
                        {
                            continue;
                        }
                        if (planDate.Date > endDate.Date)
                        {
                            break;
                        }
                        #endregion

                        double qty = 0;
                        if (row.GetCell(i) != null)
                        {
                            if (row.GetCell(i).CellType == CellType.NUMERIC)
                            {
                                qty = row.GetCell(i).NumericCellValue;
                            }
                            else
                            {
                                string qtyValue = ImportHelper.GetCellStringValue(row.GetCell(i));
                                if (qtyValue != null)
                                {
                                    qty = Convert.ToDouble(qtyValue);
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (qty < 0)
                        {
                            throw new BusinessException("Import.ShipPlan.Qty.MustGreatThan0", rowIndex);
                        }
                        else if (qty == 0)
                        {
                            continue;
                        }
                        else
                        {
                            var shiftPlan = new MrpShiftPlan();
                            shiftPlan.PlanDate = planDate;
                            shiftPlan.Item = itemCode;
                            shiftPlan.Flow = flowCode;
                            shiftPlan.Uom = uomCode;
                            shiftPlan.Qty = qty;
                            shiftPlanList.Add(shiftPlan);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new BusinessException("Import.ShipPlan.Qty.Error" + ex.Message, rowIndex);
                }
                #endregion
            }
            #endregion

            var flowCodes = shiftPlanList.Select(s => s.Flow).Distinct();
            #region 读取路线
            var flowDetailList = new List<FlowDetail>();
            foreach (var flowCode in flowCodes)
            {
                var flowDetails = this.flowMgr.GetFlowDetailList(flowCode, false, true);
                if (flowDetails == null || flowDetails.Count() == 0)
                {
                    FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(flowCode);
                    if (flowMaster.IsManualCreateDetail)
                    {
                        flowDetails = new List<FlowDetail>();
                        var flowDetail = new FlowDetail();
                        flowDetail.CurrentFlowMaster = flowMaster;
                        flowDetails.Add(flowDetail);
                    }
                }
                flowDetailList.AddRange(flowDetails);
            }
            #endregion

            BusinessException bex = new BusinessException();
            #region 校验数据的有效性
            foreach (var shiftPlan in shiftPlanList)
            {
                var flowDetails = flowDetailList.Where(f => f.CurrentFlowMaster.Code == shiftPlan.Flow);
                if (!flowDetails.First().CurrentFlowMaster.IsManualCreateDetail)
                {
                    if (string.IsNullOrWhiteSpace(shiftPlan.Uom))
                    {
                        flowDetails = flowDetails.Where(f => f.Item == shiftPlan.Item);
                        if (flowDetails == null || flowDetails.Count() == 0)
                        {
                            bex.AddMessage("此物料在路线中不存在");
                        }
                        else
                        {
                            shiftPlan.Uom = flowDetails.First().Uom;
                        }
                    }
                    else
                    {
                        flowDetails = flowDetails.Where(f => f.Item == shiftPlan.Item && f.Uom == shiftPlan.Uom);
                        if (flowDetails == null || flowDetails.Count() == 0)
                        {
                            bex.AddMessage("此物料在路线中不存在");
                        }
                    }
                    //shiftPlan.Machine = flowDetails.First().Machine;
                    //Machine machine = this.genericMgr.FindById<Machine>(shiftPlan.Machine);
                    //shiftPlan.ShiftQuota = machine.ShiftQuota;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(shiftPlan.Uom))
                    {
                        flowDetails = flowDetails.Where(f => f.Item == shiftPlan.Item);
                    }
                    else
                    {
                        flowDetails = flowDetails.Where(f => f.Item == shiftPlan.Item && f.Uom == shiftPlan.Uom);
                    }
                    if (flowDetails != null && flowDetails.Count() > 0)
                    {
                        //shiftPlan.Machine = flowDetails.First().Machine;
                        //shiftPlan.Uom = flowDetails.First().Uom;
                        //Machine machine = this.genericMgr.FindById<Machine>(shiftPlan.Machine);
                        //shiftPlan.ShiftQuota = machine.ShiftQuota;
                    }
                    else
                    {
                        Item item = this.genericMgr.FindById<Item>(shiftPlan.Item);
                        if (!string.IsNullOrWhiteSpace(shiftPlan.Uom))
                        {
                            Uom uom = this.genericMgr.FindById<Uom>(shiftPlan.Uom);
                        }
                        else
                        {
                            shiftPlan.Uom = item.Uom;
                        }
                    }
                }
            }
            #endregion
            if (!bex.HasMessage)
            {
                int planVersion = int.Parse(numberControlMgr.GetNextSequence(com.Sconit.Entity.MRP.BusinessConstants.NUMBERCONTROL_SHIFTPLAN));
                foreach (var shiftPlan in shiftPlanList)
                {
                    var oldShiftPlans = this.genericMgr.FindAll<MrpShiftPlan>
                        ("select d from ShiftPlan as d where d.PlanDate=? and d.Item=? and d.Flow=? and d.Shift=? ",
                        new object[] { shiftPlan.PlanDate, shiftPlan.Item, shiftPlan.Flow, shiftPlan.Shift });
                    if (oldShiftPlans != null && oldShiftPlans.Count() > 0)
                    {
                        //var oldShiftPlan = oldShiftPlans.First();
                        //if (shiftPlan.Machine != oldShiftPlan.Machine
                        //    || shiftPlan.Qty != oldShiftPlan.Qty
                        //    || shiftPlan.ShiftQuota != oldShiftPlan.ShiftQuota
                        //    || shiftPlan.Uom != oldShiftPlan.Uom)
                        //{
                        //    if (oldShiftPlan.OrderQty > 0)
                        //    {
                        //        bex.AddMessage("已转生产单的班产计划不能再进行更改");
                        //    }
                        //    else
                        //    {
                        //        oldShiftPlan.PlanVersion = planVersion;
                        //        oldShiftPlan.Machine = shiftPlan.Machine;
                        //        oldShiftPlan.Qty = shiftPlan.Qty;
                        //        oldShiftPlan.ShiftQuota = shiftPlan.ShiftQuota;
                        //        oldShiftPlan.Uom = shiftPlan.Uom;
                        //        //this.genericMgr.Update(oldShiftPlan);

                        //        //createlog
                        //        var shiftPlanLog = new MrpShiftPlanLog();
                        //        shiftPlanLog.PlanVersion = planVersion;
                        //        shiftPlanLog.PlanId = oldShiftPlan.Id;
                        //        shiftPlanLog.Machine = oldShiftPlan.Machine;
                        //        shiftPlanLog.Qty = oldShiftPlan.Qty;
                        //        shiftPlanLog.ShiftQuota = oldShiftPlan.ShiftQuota;
                        //        shiftPlanLog.Uom = oldShiftPlan.Uom;
                        //        this.genericMgr.Create(shiftPlanLog);
                        //    }
                        //}
                    }
                    else
                    {
                        //shiftPlan.PlanVersion = planVersion;
                        //this.genericMgr.Create(shiftPlan);

                        //var shiftPlanLog = new MrpShiftPlanLog();
                        //shiftPlanLog.PlanVersion = planVersion;
                        //shiftPlanLog.PlanId = shiftPlan.Id;
                        //shiftPlanLog.Machine = shiftPlan.Machine;
                        //shiftPlanLog.Qty = shiftPlan.Qty;
                        //shiftPlanLog.ShiftQuota = shiftPlan.ShiftQuota;
                        //shiftPlanLog.Uom = shiftPlan.Uom;
                        //this.genericMgr.Create(shiftPlanLog);
                    }
                }
            }
            else
            {
                throw bex;
            }
            return shiftPlanList;
        }

        #endregion

        #region update
        [Transaction(TransactionMode.Requires)]
        public void UpdateMrpPlan(MrpPlan mrpPlan)
        {
            mrpPlan.Qty = mrpPlan.CurrentQty;
            this.genericMgr.Update(mrpPlan);
            User user = SecurityContextHolder.Get();
            var mrpPlanLog = this.genericMgr.FindAll<MrpPlanLog>
                (" from MrpPlanLog where PlanDate=? and Item = ? and PlanVersion=? and Flow=? and Location=?",
                new object[] { mrpPlan.PlanDate, mrpPlan.Item, mrpPlan.PlanVersion, mrpPlan.Flow, mrpPlan.Location })
                .First();
            mrpPlanLog.Qty = mrpPlan.Qty;
            mrpPlanLog.UnitQty = 1;
            this.genericMgr.Update(mrpPlanLog);
        }
        #endregion

        #region GetPlanSimulation
        [Transaction(TransactionMode.Requires)]
        public StringBuilder GetPlanSimulation(DateTime planVersion, string flow)
        {
            var planMaster = this.genericMgr.FindById<MrpPlanMaster>(planVersion);

            var mrpFlowDetials = genericMgr.FindAll<MrpFlowDetail>
                ("from MrpFlowDetail where SnapTime =? and StartDate<=? and EndDate>? ",
                new object[] { planMaster.SnapTime, planVersion, planVersion });

            //待收
            var planInDic = GetDailyPlanInDic(planVersion, flow);

            //待发
            var shipPlanDic = GetShipPlanDic(planVersion, flow);

            //库存
            var inventoryBalanceDic = GetInventoryBalanceDic(mrpFlowDetials, flow);

            var flowDetailList = mrpFlowDetials.Where(p => p.Flow == flow)
                .OrderBy(p => p.Machine)
                .ThenBy(p => p.Sequence).ToList();
            var endDate = planVersion.Date;
            if (planInDic.Count > 0)
            {
                endDate = planInDic.SelectMany(p => p.Value).Max(q => q.Key.Date);
            }
            endDate = planVersion.Date.AddDays(15) > endDate ? endDate : planVersion.Date.AddDays(15);

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><tr>");

            #region head
            str.Append("<th rowspan=\"2\" style=\"text-align:center\" >");
            str.Append("物料号");
            str.Append("</th>");

            str.Append("<th rowspan=\"2\" style=\"text-align:center;min-width:150px\" >");
            str.Append("物料描述");
            str.Append("</th>");

            str.Append("<th colspan=\"3\" style=\"text-align:center\" >");
            str.Append("库存");
            str.Append("</th>");

            DateTime currentDate = planVersion.Date;
            for (int i = 0; currentDate.AddDays(i) <= endDate; i++)
            {
                str.Append("<th colspan=\"3\" style=\"text-align:center\" >");
                str.Append(currentDate.AddDays(i).ToString("MM-dd"));
                str.Append("</th>");
            }
            str.Append("</tr>");

            str.Append("<tr>");

            str.Append("<th style=\"text-align:center;min-width:30px\" >");
            str.Append("安全");
            str.Append("</th>");

            str.Append("<th style=\"text-align:center;min-width:30px\" >");
            str.Append("最大");
            str.Append("</th>");

            str.Append("<th style=\"text-align:center;min-width:30px\" >");
            str.Append("期初");
            str.Append("</th>");

            for (int i = 0; currentDate.AddDays(i) <= endDate; i++)
            {
                str.Append("<th style=\"text-align:center\" >");
                str.Append("收");
                str.Append("</th>");

                str.Append("<th style=\"text-align:center\" >");
                str.Append("发");
                str.Append("</th>");

                str.Append("<th style=\"text-align:center\" >");
                str.Append("存");
                str.Append("</th>");
            }
            str.Append("</tr>");
            #endregion

            #region body
            string machine = flowDetailList.First().Machine;
            int l = 0;
            foreach (var flowDetail in flowDetailList)
            {
                var inventoryBalance = inventoryBalanceDic.ValueOrDefault(flowDetail.Item) ?? new InventoryBalance();
                var plans = planInDic.ValueOrDefault(flowDetail.Item) ?? new Dictionary<DateTime, double>();
                var ships = shipPlanDic.ValueOrDefault(flowDetail.Item) ?? new Dictionary<DateTime, double>();

                if (plans.Count == 0 && ships == null && inventoryBalance.Qty >= inventoryBalance.SafeStock)
                {
                    continue;
                }

                var item = itemMgr.GetCacheItem(flowDetail.Item);

                if (machine != flowDetail.Machine)
                {
                    machine = flowDetail.Machine;
                    l++;
                }
                if (l % 2 == 1)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }

                str.Append("<td>");
                str.Append(item.Code);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(item.FullDescription);
                str.Append("</td>");

                var safeStock = planMaster.ResourceGroup == CodeMaster.ResourceGroup.MI ? inventoryBalance.SafeStock / (double)item.UnitCount : inventoryBalance.SafeStock;
                str.Append("<td>");
                str.Append(safeStock.ToString("0.#"));
                str.Append("</td>");

                var maxStock = planMaster.ResourceGroup == CodeMaster.ResourceGroup.MI ? inventoryBalance.MaxStock / (double)item.UnitCount : inventoryBalance.MaxStock;
                str.Append("<td>");
                str.Append(maxStock.ToString("0.#"));
                str.Append("</td>");

                var currentQty = planMaster.ResourceGroup == CodeMaster.ResourceGroup.MI ? inventoryBalance.Qty / (double)item.UnitCount : inventoryBalance.Qty;
                str.Append("<td>");
                str.Append(currentQty.ToString("0.#"));
                str.Append("</td>");

                for (int i = 0; currentDate.AddDays(i) <= endDate; i++)
                {
                    var planQty = plans.ValueOrDefault(currentDate.AddDays(i));
                    var shipQty = ships.ValueOrDefault(currentDate.AddDays(i));
                    planQty = planMaster.ResourceGroup == CodeMaster.ResourceGroup.MI ? planQty / (double)item.UnitCount : planQty;
                    shipQty = planMaster.ResourceGroup == CodeMaster.ResourceGroup.MI ? shipQty / (double)item.UnitCount : shipQty;

                    str.Append("<td class=\"t-alt\">");
                    str.Append(planQty.ToString("0.#"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(shipQty.ToString("0.#"));
                    str.Append("</td>");

                    currentQty = currentQty + planQty - shipQty;
                    if (currentQty < 0)
                    {
                        str.Append("<td class =\"WarningColor_Red\">");
                    }
                    else if (currentQty < inventoryBalance.SafeStock)
                    {
                        str.Append("<td class =\"WarningColor_Orange\">");
                    }
                    else if (currentQty > inventoryBalance.MaxStock && inventoryBalance.MaxStock > 0)
                    {
                        str.Append("<td class =\"WarningColor_Yellow\">");
                    }

                    else
                    {
                        str.Append("<td>");
                    }
                    str.Append(currentQty.ToString("0.#"));
                    str.Append("</td>");
                }
                str.Append("</tr>");
            }
            str.Append("</table>");
            #endregion

            return str;
        }

        private Dictionary<string, Dictionary<DateTime, double>> GetDailyPlanInDic(DateTime planVersion, string flow)
        {
            //PlanDate,Item,Qty
            var planList = base.GetProductPlanInList(planVersion, flow);

            var planDic = (from p in planList
                           group p by p.Item into g
                           select new
                           {
                               Item = (string)g.Key,
                               Dic = g.GroupBy(p => p.StartTime.Date).ToDictionary(d => d.Key, d => d.Sum(q => q.Qty))
                           }).ToDictionary(c => c.Item, c => c.Dic);
            return planDic;
        }

        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        private List<ShiftPlanQty> GetShiftOrderQty(string flow)
        {
            var orderObj = genericMgr.FindAllWithNativeSql<object[]>
                (@"select d.Item, m.EffDate, m.Shift, 
                    sum(case when d.OrderQty-d.RecQty<0 then 0 else d.OrderQty-d.RecQty end) as Qty,d.UC
                    from ORD_OrderDet_4 as d join ORD_OrderMstr_4 as m on d.OrderNo = m.OrderNo 
                    where Flow = ? and Status in (?,?,?) group by d.Item, m.EffDate, m.Shift,d.UC",
                 new object[] { flow,
                                CodeMaster.OrderStatus.Create,
                                CodeMaster.OrderStatus.Submit,
                                CodeMaster.OrderStatus.InProcess
                 });
            //订单
            var orderGroup = (from p in orderObj
                              select new ShiftPlanQty
                              {
                                  Item = (string)p[0],
                                  PlanDate = ((DateTime)p[1]).Date,
                                  Shift = (string)p[2],
                                  Qty = Convert.ToDouble(p[3]),
                                  UnitCount = Convert.ToDouble(p[4])
                              }).ToList();
            return orderGroup;
        }

        private List<ShiftPlanQty> GetMiShiftPlanQty(DateTime planVersion, string flow)
        {
            var planList = genericMgr.FindAll<MrpMiShiftPlan>
               ("from MrpMiShiftPlan where PlanVersion =? and ProductLine = ? ", new object[] { planVersion, flow });

            var shiftPlanQtyList = (from p in planList
                                    group p by new
                                    {
                                        Item = p.Item,
                                        PlanDate = p.PlanDate,
                                        Shift = p.Shift
                                    } into g
                                    select new ShiftPlanQty
                                    {
                                        Item = g.Key.Item,
                                        Sequence = g.First().Sequence,
                                        PlanDate = g.Key.PlanDate,
                                        Shift = g.Key.Shift,
                                        Qty = g.Sum(q => (q.Qty + q.AdjustQty)),
                                        UnitCount = g.First().UnitCount
                                    }).ToList();
            return shiftPlanQtyList;
        }

        private List<ShiftPlanQty> GetFiShiftPlanQty(DateTime planVersion, string flow)
        {
            string hql = @"from MrpFiShiftPlan where PlanVersion =? and ProductLine = ? ";
            List<object> paramList = new List<object> { planVersion, flow };
            var planList = genericMgr.FindAll<MrpFiShiftPlan>(hql, paramList);

            var shiftPlanQtyList = (from p in planList
                                    group p by new
                                    {
                                        Item = p.Item,
                                        PlanDate = p.PlanDate,
                                        Shift = "FI-" + p.Shift.Substring(p.Shift.Length - 1, 1)
                                    } into g
                                    select new ShiftPlanQty
                                    {
                                        Island = g.First().Island,
                                        Machine = g.First().Machine,
                                        Item = g.Key.Item,
                                        Sequence = g.First().Sequence,
                                        PlanDate = g.Key.PlanDate,
                                        Shift = g.Key.Shift,
                                        Qty = g.Sum(q => q.Qty),
                                        UnitCount = g.First().UnitCount
                                    }).ToList();
            var shiftPlanQtyListCopy = shiftPlanQtyList;
            var shiftPlanQtyList1 = (from p in shiftPlanQtyListCopy
                                     select new ShiftPlanQty
                                     {
                                         Island = p.Island,
                                         Machine = p.Machine,
                                         Item = p.Item,
                                         Sequence = p.Sequence,
                                         PlanDate = p.PlanDate,
                                         Shift = "FI-1",
                                         Qty = 0,
                                         UnitCount = p.UnitCount
                                     }).Where(p => !shiftPlanQtyList.Contains(p)).Distinct();
            //shiftPlanQtyList.AddRange(shiftPlanQtyList1);
            var shiftPlanQtyList2 = (from p in shiftPlanQtyListCopy
                                     select new ShiftPlanQty
                                     {
                                         Island = p.Island,
                                         Machine = p.Machine,
                                         Item = p.Item,
                                         Sequence = p.Sequence,
                                         PlanDate = p.PlanDate,
                                         Shift = "FI-2",
                                         Qty = 0,
                                         UnitCount = p.UnitCount
                                     }).Where(p => !shiftPlanQtyList.Contains(p)).Distinct();
            //shiftPlanQtyList.AddRange(shiftPlanQtyList2);
            var shiftPlanQtyList3 = shiftPlanQtyList2;
            if ((int)planList.Select(p => p.ShiftType).ToList().Max() == 3)
            {
                shiftPlanQtyList3 = (from p in shiftPlanQtyListCopy
                                     select new ShiftPlanQty
                                     {
                                         Island = p.Island,
                                         Machine = p.Machine,
                                         Item = p.Item,
                                         Sequence = p.Sequence,
                                         PlanDate = p.PlanDate,
                                         Shift = "FI-3",
                                         Qty = 0,
                                         UnitCount = p.UnitCount
                                     }).Where(p => !shiftPlanQtyList.Contains(p)).Distinct();
                //shiftPlanQtyList.AddRange(shiftPlanQtyList3);
            }

            return (from p in shiftPlanQtyList.Union(shiftPlanQtyList1).Union(shiftPlanQtyList2).Union(shiftPlanQtyList3)
                    group p by new
                    {
                        Item = p.Item,
                        PlanDate = p.PlanDate,
                        Shift = p.Shift,
                    } into g
                    select new ShiftPlanQty
                    {
                        Island = g.First().Island,
                        IslandDescription = this.genericMgr.FindById<Island>(g.First().Island).Description,
                        Machine = g.First().Machine,
                        Item = g.Key.Item,
                        Sequence = g.First().Sequence,
                        PlanDate = g.Key.PlanDate,
                        Shift = g.Key.Shift,
                        Qty = g.Sum(q => q.Qty),
                        UnitCount = g.First().UnitCount
                    }).ToList();
        }

        private Dictionary<string, Dictionary<DateTime, double>> GetShipPlanDic(DateTime planVersion, string flow)
        {
            var planList = genericMgr.FindAllWithNativeSql<object[]>("exec USP_Busi_MRP_GetPlanOut ?,?", new object[] { planVersion, flow });
            var planDic = (from p in planList
                           group p by p[1] into g
                           select new
                           {
                               Item = (string)g.Key,
                               Dic = g.ToDictionary(d => (DateTime)d[0], d => (double)d[2])
                           }).ToDictionary(c => c.Item, c => c.Dic);
            return planDic;
        }

        private Dictionary<string, InventoryBalance> GetInventoryBalanceDic(IList<MrpFlowDetail> mrpFlowDetails, string flow)
        {
            //没有考虑同一物料目的库位不一致的情况
            string sql = @" select l.* from VIEW_LocationDet as l inner join MD_Location as loc on l.Location = loc.Code 
                            where loc.IsMrp = ? and l.ATPQty>0 and  l.Item in(? ";

            IList<LocationDetailView> locationDetailViewList = this.genericMgr.FindEntityWithNativeSqlIn<LocationDetailView>
                (sql, mrpFlowDetails.Where(p => p.Flow == flow).Select(p => p.Item).Distinct(), new object[] { true });

            var groupLocationDetailDic = (from p in locationDetailViewList
                                          group p by p.Item into g
                                          select new
                                          {
                                              Item = g.Key,
                                              Qty = g.Sum(q => q.ATPQty)
                                          }).ToDictionary(d => d.Item, d => (double)d.Qty);

            var stockDic = mrpFlowDetails.Where(f => f.MrpWeight > 0 && f.EndDate >= DateTime.Now.Date)
                .GroupBy(p => p.Item)
                .ToDictionary(d => d.Key, d => new { SafeStock = d.Sum(q => q.SafeStock), MaxStock = d.Sum(q => q.MaxStock) });

            var inventoryBalanceList = (from p in mrpFlowDetails
                                        where p.Flow == flow
                                        group p by p.Item into g
                                        select new InventoryBalance
                                        {
                                            SnapTime = DateTime.Now,
                                            Item = g.Key,
                                            //SafeStock = g.First().SafeStock,
                                            //MaxStock = g.First().MaxStock
                                        }).ToList();

            foreach (var inventoryBalance in inventoryBalanceList)
            {
                inventoryBalance.Qty = groupLocationDetailDic.ValueOrDefault(inventoryBalance.Item);
                var stock = stockDic.ValueOrDefault(inventoryBalance.Item);
                inventoryBalance.SafeStock = stock.SafeStock;
                inventoryBalance.MaxStock = stock.MaxStock;
            }
            var inventoryBalanceDic = inventoryBalanceList.ToDictionary(d => d.Item, d => d);
            return inventoryBalanceDic;
        }
        #endregion

        [Transaction(TransactionMode.Requires)]
        public StringBuilder GetFiShiftPlanView(DateTime planVersion, string flow)
        {
            var shiftPlanQtyList = GetFiShiftPlanQty(planVersion, flow);

            var shiftGroups = shiftPlanQtyList.GroupBy(k => k.Island, (k, g) =>
                new
                {
                    Island = k,
                    IslandDescription = g.First().IslandDescription,
                    List1 = g.GroupBy(k1 => k1.Machine, (k1, g1) =>
                        new
                        {
                            Machine = k1,
                            List2 = g1.GroupBy(k2 => k2.Item, (k2, g2) =>
                                new
                                {
                                    Item = k2,
                                    List3 = g2.ToList()
                                }).ToList()
                        }).ToList()
                }).OrderBy(p => p.IslandDescription);

            var planDateShifts = (from p in shiftPlanQtyList
                                  group p by new
                                  {
                                      PlanDate = p.PlanDate
                                  } into g
                                  select new
                                  {
                                      PlanDate = g.Key.PlanDate,
                                      Shifts = g.Select(q => q.Shift).Distinct().OrderBy(r => r)
                                  }).OrderBy(p => p.PlanDate);

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><tr>");

            #region head
            str.Append("<th rowspan=\"2\" style=\"text-align:center\" >");
            str.Append("岛区");
            str.Append("</th>");

            str.Append("<th rowspan=\"2\" style=\"text-align:center\" >");
            str.Append("模具");
            str.Append("</th>");

            str.Append("<th rowspan=\"2\" style=\"text-align:center\" >");
            str.Append("物料号");
            str.Append("</th>");

            str.Append("<th rowspan=\"2\" style=\"text-align:center;min-width:80px\" >");
            str.Append("物料描述");
            str.Append("</th>");

            //str.Append("<th rowspan=\"2\" style=\"text-align:center;min-width:120px\" >");
            //str.Append("参考零件号");
            //str.Append("</th>");

            DateTime currentDate = planVersion.Date;
            foreach (var planDateShift in planDateShifts)
            {
                str.Append(string.Format("<th colspan=\"{0}\" style=\"text-align:center\" >", planDateShift.Shifts.Count()));
                str.Append(planDateShift.PlanDate.ToString("MM-dd"));
                str.Append("</th>");
            }
            str.Append("</tr>");

            str.Append("<tr>");
            foreach (var planDateShift in planDateShifts)
            {
                foreach (var shift in planDateShift.Shifts)
                {
                    str.Append("<th style=\"text-align:center;padding:4px 0px\" >");
                    str.Append(shift);
                    str.Append("</th>");
                }
            }
            str.Append("</tr>");
            #endregion

            #region body
            int l = 0;
            foreach (var shiftGroup in shiftGroups)
            {
                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }
                str.Append(string.Format("<td rowspan='{0}'>", shiftGroup.List1.Sum(p => p.List2.Count())));
                str.Append(shiftGroup.Island);
                str.Append("<br/>");
                str.Append(shiftGroup.IslandDescription);
                str.Append("</td>");
                foreach (var list1 in shiftGroup.List1)
                {
                    if (shiftGroup.List1.IndexOf(list1) > 0)
                    {
                        if (l % 2 == 0)
                        {
                            str.Append("<tr class=\"t-alt\">");
                        }
                        else
                        {
                            str.Append("<tr>");
                        }
                    }
                    str.Append(string.Format("<td rowspan='{0}'>", list1.List2.Count()));
                    str.Append(list1.Machine);
                    str.Append("</td>");
                    foreach (var shiftPlan in list1.List2)
                    {
                        if (list1.List2.IndexOf(shiftPlan) > 0)
                        {
                            if (l % 2 == 0)
                            {
                                str.Append("<tr class=\"t-alt\">");
                            }
                            else
                            {
                                str.Append("<tr>");
                            }
                        }
                        var item = itemMgr.GetCacheItem(shiftPlan.Item);

                        str.Append("<td>");
                        str.Append(item.Code);
                        str.Append("</td>");

                        str.Append("<td>");
                        str.Append(item.Description);
                        str.Append("</td>");

                        foreach (var planDateShift in planDateShifts)
                        {
                            var _ShiftPlanQtys = shiftPlan.List3.Where(p => p.PlanDate == planDateShift.PlanDate);
                            foreach (var shift in planDateShift.Shifts)
                            {
                                var shiftPlanQty = _ShiftPlanQtys.FirstOrDefault(p => p.Shift == shift) ?? new ShiftPlanQty();
                                str.Append("<td>");
                                str.Append(Math.Round(shiftPlanQty.Qty));
                                str.Append("</td>");
                            }
                        }
                        str.Append("</tr>");
                    }
                }
            }
            str.Append("</table>");
            #endregion
            return str;
        }

        [Transaction(TransactionMode.Requires)]
        public StringBuilder GetMiDailyPlanView(DateTime planVersion, string flow)
        {
            var paramList = new List<object> { planVersion, flow, planVersion.Date };
            string hql = " from MrpMiPlan where PlanVersion =? and ProductLine=? and PlanDate>=? ";
            if (flow == "MI03")
            {
                hql = " from MrpMiPlan where PlanVersion =? and ProductLine in(?,?) and PlanDate>=? ";
                paramList = new List<object> { planVersion, "MI01", "MI02", planVersion.Date };
            }
            var mrpMiPlanList = this.genericMgr.FindAll<MrpMiPlan>(hql, paramList.ToArray());

            var planDates = mrpMiPlanList.Select(p => p.PlanDate).Distinct().OrderBy(p => p);
            var shiftGroups = mrpMiPlanList
                .GroupBy(k => k.Item, (k, g) =>
                  new
                  {
                      Item = k,
                      Sequence = g.First().Sequence,
                      Dic = g.GroupBy(k1 => k1.PlanDate, (k1, g1) =>
                          new
                          {
                              PlanDate = k1,
                              Qty = g1.Sum(p => p.CheQty)
                          }).ToDictionary(d => d.PlanDate, d => d.Qty)
                  }).OrderBy(p => p.Sequence).ToList();

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><tr>");

            #region head
            str.Append("<th style=\"text-align:center\" >");
            str.Append("物料号");
            str.Append("</th>");

            str.Append("<th style=\"text-align:center;min-width:80px\" >");
            str.Append("物料描述");
            str.Append("</th>");

            DateTime currentDate = planVersion.Date;
            foreach (var planDate in planDates)
            {
                str.Append(string.Format("<th style=\"text-align:center\" >"));
                str.Append(planDate.ToString("MM-dd"));
                str.Append("</th>");
            }
            str.Append("</tr>");

            #endregion

            #region body
            int l = 0;
            foreach (var shiftPlan in shiftGroups)
            {
                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }
                var item = itemMgr.GetCacheItem(shiftPlan.Item);
                str.Append("<td>");
                str.Append(item.Code);
                str.Append("</td>");
                str.Append("<td>");
                str.Append(item.Description);
                str.Append("</td>");
                foreach (var planDate in planDates)
                {
                    str.Append("<td>");
                    str.Append(Math.Round(shiftPlan.Dic.ValueOrDefault(planDate), 1));
                    str.Append("</td>");
                }
                str.Append("</tr>");
            }

            var totalCheQtyDic = mrpMiPlanList.GroupBy(p => p.PlanDate)
                .ToDictionary(d => d.Key, d => d.Sum(p => p.CheQty));

            str.Append("<tr><td></td><td>合计(车)</td>");
            foreach (var planDate in planDates)
            {
                str.Append("<td>");
                str.Append(Math.Round(totalCheQtyDic.ValueOrDefault(planDate), 1));
                str.Append("</td>");
            }
            str.Append("</tr>");

            var totalWorkHourDic = mrpMiPlanList.GroupBy(p => p.PlanDate)
                .ToDictionary(d => d.Key, d => d.Sum(p => (p.CheQty * p.WorkHour) / 60));
            str.Append("<tr><td></td><td>工时合计(小时)</td>");
            foreach (var planDate in planDates)
            {
                var workHours = totalWorkHourDic.ValueOrDefault(planDate);
                if (workHours > 24)
                {
                    str.Append("<td class =\"WarningColor_Yellow\">");
                }
                else
                {
                    str.Append("<td>");
                }
                str.Append(Math.Round(workHours, 1));
                str.Append("</td>");
            }
            str.Append("</tr>");

            str.Append("</table>");
            #endregion
            return str;
        }

        [Transaction(TransactionMode.Requires)]
        public IList<MrpPlanTraceView> GetPlanTraceViewList(CodeMaster.ResourceGroup resourceGroup, string flow, string item, bool onlyShowUrgent = true)
        {
            //物料号	物料描述	最大	最小	待收	待发	当前库存
            var items = new List<string>();
            var flows = new List<string>();

            #region //根据路线得到物料明细
            var snapTime = this.genericMgr.FindAll<MrpSnapMaster>
                (@"from MrpSnapMaster m where m.IsRelease =? and m.Type=? order by m.SnapTime desc",
                new object[] { true, CodeMaster.SnapType.Mrp }, 0, 1)
                .First().SnapTime;

            var flowParam = new List<object>();
            flowParam.Add(snapTime);
            var hql = @"from MrpFlowDetail as m where m.SnapTime = ? ";
            if (!string.IsNullOrWhiteSpace(item))
            {
                items.Add(item);
                flowParam.Add(item);
                hql += " and m.Item=? ";
            }
            if (!string.IsNullOrWhiteSpace(flow))
            {
                flows.Add(flow);
            }
            else
            {
                flows = this.genericMgr.FindAll<FlowMaster>("from FlowMaster where ResourceGroup = ? ", resourceGroup)
                    .Select(p => p.Code).ToList();
            }
            hql += " and StartDate<=? and EndDate>? ";
            flowParam.Add(snapTime);
            flowParam.Add(snapTime);
            var mrpFlowDetailList = this.genericMgr.FindAll<MrpFlowDetail>(hql, flowParam);
            items = mrpFlowDetailList.Where(q => flows.Contains(q.Flow)).Select(q => q.Item).Distinct().ToList();

            var flowDetailDic = (from p in mrpFlowDetailList
                                 where items.Contains(p.Item)
                                 group p by new
                                 {
                                     Item = p.Item,
                                 } into g
                                 select new
                                 {
                                     Item = g.Key.Item,
                                     SafeStock = g.Sum(b => b.SafeStock),
                                     MaxStock = g.Sum(b => b.MaxStock),
                                 }).ToDictionary(d => d.Item, d => d);
            #endregion

            #region //期末库存
            var locationDetailViewList = this.genericMgr.FindEntityWithNativeSqlIn<LocationDetailView>(
               @"select l.* from VIEW_LocationDet as l inner join MD_Location as loc on l.Location = loc.Code 
                where loc.IsMrp = ? and l.Item in(? ", items, new object[] { true });
            var locationDetailDic = (from p in locationDetailViewList
                                     group p by new { p.Item } into g
                                     select new
                                     {
                                         Item = g.Key.Item,
                                         Qty = (double)g.Sum(q => q.ATPQty)
                                     }).ToDictionary(d => d.Item, d => d.Qty);
            #endregion

            #region //期末在途//移库
            var endTransitOrderList = (this.genericMgr.FindEntityWithNativeSqlIn<TransitOrder>
                        (@" select 
                            det.Id as Id,
                            det.OrderNo as OrderNo,
                            det.OrderDetId as OrderDetId,
                            det.IpNo as IpNo,
                            det.Flow as Flow,
                            det.OrderType as OrderType,
                            det.LocTo as Location,
                            det.Item as Item,
                            det.Qty * det.UnitQty as ShipQty,
                            det.RecQty * det.UnitQty as RecQty,
                            det.StartTime as StartTime,
                            det.Windowtime as Windowtime,
                            getdate() as SnapTime
                            from ORD_IpDet_2 as det
                            where det.IsClose = ? and det.Item in(?",
                        items, new object[] { false })).ToList();
            endTransitOrderList.AddRange(this.genericMgr.FindEntityWithNativeSqlIn<TransitOrder>
                        (@" select 
                            det.Id as Id,
                            det.OrderNo as OrderNo,
                            det.OrderDetId as OrderDetId,
                            det.IpNo as IpNo,
                            det.Flow as Flow,
                            det.OrderType as OrderType,
                            det.LocTo as Location,
                            det.Item as Item,
                            det.Qty * det.UnitQty as ShipQty,
                            det.RecQty * det.UnitQty as RecQty,
                            det.StartTime as StartTime,
                            det.Windowtime as Windowtime,
                            getdate() as SnapTime
                            from ORD_IpDet_7 as det
                            where det.IsClose = ? and det.Item in(?",
                        items, new object[] { false }));

            var endTransitOrderListDic = (from p in endTransitOrderList
                                          group p by new { p.Item } into g
                                          select new
                                          {
                                              Item = g.Key.Item,
                                              Qty = g.Sum(q => q.TransitQty)
                                          }).ToDictionary(d => d.Item, d => d.Qty);
            #endregion

            #region //得到PlanSimulationView
            var planTraceViews = (from p in mrpFlowDetailList
                                  where p.ResourceGroup == resourceGroup
                                  group p by p.Item into g
                                  select new MrpPlanTraceView
                                  {
                                      Item = g.Key,
                                      MaxStock = flowDetailDic.ValueOrDefault(g.Key) == null ? 0 : flowDetailDic.ValueOrDefault(g.Key).MaxStock,
                                      SafeStock = flowDetailDic.ValueOrDefault(g.Key) == null ? 0 : flowDetailDic.ValueOrDefault(g.Key).SafeStock,
                                      EndQty = locationDetailDic.ValueOrDefault(g.Key),
                                      EndTransQty = endTransitOrderListDic.ValueOrDefault(g.Key),
                                  }).ToList();

            if (onlyShowUrgent && string.IsNullOrWhiteSpace(item))
            {
                planTraceViews = planTraceViews
                    .Where(p => p.EndQty + p.EndTransQty > p.MaxStock || p.EndQty + p.EndTransQty < p.SafeStock)
                    .ToList();
                items = planTraceViews.Select(p => p.Item).Distinct().ToList();
            }
            #endregion

            #region //期初库存
            var inventoryBalanceList = this.genericMgr.FindAllIn<InventoryBalance>
                (@"from InventoryBalance as m where m.SnapTime = ? and m.Item in(? ", items, new object[] { snapTime });
            var inventoryBalanceDic = (from p in (inventoryBalanceList ?? new List<InventoryBalance>())
                                       group p by new { p.Item } into g
                                       select new
                                       {
                                           Item = g.Key.Item,
                                           Qty = g.Sum(q => q.Qty)
                                       }).ToDictionary(d => d.Item, d => d.Qty);
            #endregion

            #region //期初在途
            var startTransitOrderList = this.genericMgr.FindAllIn<TransitOrder>
             (@"from TransitOrder as m where m.SnapTime = ? and (m.OrderType = ? or m.OrderType =? )  and m.Item in(? ",
             items, new object[] { snapTime, CodeMaster.OrderType.Transfer, CodeMaster.OrderType.SubContractTransfer });
            var startTransitOrderDic = (from p in (startTransitOrderList ?? new List<TransitOrder>())
                                        group p by new { p.Item } into g
                                        select new
                                        {
                                            Item = g.Key.Item,
                                            Qty = g.Sum(q => q.TransitQty)
                                        }).ToDictionary(d => d.Item, d => d.Qty);
            #endregion

            #region //库存事务LocTrans,尚未考虑移库到不是MRP库位对库存的影响 //排除库内事务
            //可能原因（质量，生产，采购，销售，委外，检验，调整） 订单号 时间 库位 单位 数量
            //Item OrderNo Qty UnitQty QualityType TransType IOType LocFrom LocTo CreateDate
            var locTransList = this.genericMgr.FindAllWithNativeSqlIn<object[]>(
               @"select Item,OrderNo,Qty*UnitQty,QualityType,TransType,IOType,LocFrom,LocTo,CreateDate 
                from VIEW_LocTrans where CreateDate>=? and TransType not like ? and TransType not in(?,?,?,?) and Item in(? ",
                items,
                new object[]
                {
                    snapTime,
                    "3%",//移库等
                    CodeMaster.TransactionType.ISS_INP,          //报验出库
                    CodeMaster.TransactionType.RCT_INP,          //报验入库
                    CodeMaster.TransactionType.ISS_INP_QDII,     //检验合格出库 
                    CodeMaster.TransactionType.RCT_INP_QDII,     //检验合格入库 
                }
               );

            var planSimulationDetailViewDic = (new List<MrpPlanTraceDetailView>())
                .GroupBy(p => p.Item, (k, g) => new { Item = k, g }).ToDictionary(r => r.Item, r => r.g.ToList());

            if (locTransList != null)
            {
                planSimulationDetailViewDic = (from p in locTransList
                                               group p by p[0] into g
                                               select new
                                               {
                                                   Item = (string)g.Key,
                                                   List = (from q in g
                                                           select new MrpPlanTraceDetailView
                                                           {
                                                               Item = (string)q[0],
                                                               OrderNo = (string)q[1],
                                                               Qty = (double)((decimal)q[2]),
                                                               QualityType = (CodeMaster.QualityType)(int.Parse(q[3].ToString())),
                                                               TransType = (CodeMaster.TransactionType)(int.Parse(q[4].ToString())),
                                                               IOType = (CodeMaster.TransactionIOType)(int.Parse(q[5].ToString())),
                                                               LocationFrom = (string)q[6],
                                                               LocationTo = (string)q[7],
                                                               CreateDate = (DateTime)q[8],
                                                           }).ToList()
                                               }).ToDictionary(r => r.Item, r => r.List);
            }
            #endregion

            #region 计划收发
            var planInDic = new Dictionary<string, double>();
            var planOutDic = new Dictionary<string, double>();
            if (resourceGroup == CodeMaster.ResourceGroup.FI)
            {
                var fiShiftPlan = this.genericMgr.FindAll<MrpFiShiftPlan>
                    (@" from MrpFiShiftPlan where WindowTime between ? and ? ",
                    new object[] { snapTime, DateTime.Now });
                planInDic = (from p in fiShiftPlan
                             group p by p.Item into g
                             select new
                             {
                                 Item = g.Key,
                                 Qty = g.Sum(q => q.Qty)
                             }).ToDictionary(d => d.Item, d => d.Qty);

                var mrpPlan = this.genericMgr.FindAll<MrpPlan>
                    (@" from MrpPlan where PlanDate between ? and ? ",
                    new object[] { snapTime.Date, DateTime.Now });
                planOutDic = (from p in fiShiftPlan
                              group p by p.Item into g
                              select new
                              {
                                  Item = g.Key,
                                  Qty = g.Sum(q => q.Qty)
                              }).ToDictionary(d => d.Item, d => d.Qty);
            }
            else if (resourceGroup == CodeMaster.ResourceGroup.EX)
            {
                var exShiftPlan = this.genericMgr.FindAll<MrpExShiftPlan>
                    (@" from MrpExShiftPlan where WindowTime between ? and ? ",
                    new object[] { snapTime, DateTime.Now });
                planInDic = (from p in exShiftPlan
                             group p by p.Item into g
                             select new
                             {
                                 Item = g.Key,
                                 Qty = g.Sum(q => q.Qty)
                             }).ToDictionary(d => d.Item, d => d.Qty);

                var fiShiftPlan = this.genericMgr.FindAll<MrpFiShiftPlan>
                 (@" from MrpFiShiftPlan where WindowTime between ? and ? ",
                 new object[] { snapTime, DateTime.Now });
                planOutDic = (from p in fiShiftPlan
                              group p by p.Item into g
                              select new
                              {
                                  Item = g.Key,
                                  Qty = g.Sum(q => q.Qty)
                              }).ToDictionary(d => d.Item, d => d.Qty);

            }
            else if (resourceGroup == CodeMaster.ResourceGroup.MI)
            {
                var miShiftPlan = this.genericMgr.FindAll<MrpMiShiftPlan>
              (@" from MrpMiShiftPlan where WindowTime between ? and ? ",
              new object[] { snapTime, DateTime.Now });
                planInDic = (from p in miShiftPlan ?? new List<MrpMiShiftPlan>()
                             group p by p.Item into g
                             select new
                             {
                                 Item = g.Key,
                                 Qty = g.Sum(q => q.Qty)
                             }).ToDictionary(d => d.Item, d => d.Qty);

                var exShiftPlan = this.genericMgr.FindAll<MrpExShiftPlan>
                    (@" from MrpExShiftPlan where WindowTime between ? and ? ",
                    new object[] { snapTime, DateTime.Now });
                planOutDic = (from p in exShiftPlan ?? new List<MrpExShiftPlan>()
                              group p by p.Item into g
                              select new
                              {
                                  Item = g.Key,
                                  Qty = g.Sum(q => q.Qty)
                              }).ToDictionary(d => d.Item, d => d.Qty);
            }
            #endregion

            #region  赋值 期初库存 期初在途 库存事务(收/发）
            foreach (var plan in planTraceViews)
            {
                double startQty = 0;
                inventoryBalanceDic.TryGetValue(plan.Item, out startQty);
                plan.StartQty = startQty;

                double startTransQty = 0;
                startTransitOrderDic.TryGetValue(plan.Item, out startTransQty);
                plan.StartTransQty = startTransQty;

                List<MrpPlanTraceDetailView> detail = null;
                planSimulationDetailViewDic.TryGetValue(plan.Item, out detail);
                plan.MrpPlanTraceDetailViewList = detail;

                plan.InQty = detail != null ? detail.Where(p => p.Qty > 0).Sum(p => p.Qty) : 0;
                plan.OutQty = detail != null ? detail.Where(p => p.Qty < 0).Sum(p => p.Qty) : 0;
                plan.ItemDescription = itemMgr.GetCacheItem(plan.Item).Description;

                double planInQty = 0;
                planInDic.TryGetValue(plan.Item, out planInQty);
                plan.PlanInQty = planInQty;

                double planOutQty = 0;
                planOutDic.TryGetValue(plan.Item, out planOutQty);
                plan.PlanOutQty = planOutQty;
            }
            #endregion

            return planTraceViews;
        }

        [Transaction(TransactionMode.Requires)]
        public IList<ContainerView> GetContainerViewList(DateTime dateTime)
        {
            var locations = systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MiContainerLocations, false).Split('|');
            var containerViewList = new List<ContainerView>();

            var mrpPlanMaster = this.genericMgr.FindAll<MrpPlanMaster>
                  ("from MrpPlanMaster where IsRelease = ? and ResourceGroup=? order by PlanVersion desc",
                  new object[] { true, CodeMaster.ResourceGroup.MI }, 0, 1).First();

            #region 待发 发出数量
            var paramList = new List<object> { mrpPlanMaster.PlanVersion, mrpPlanMaster.SnapTime, dateTime };
            paramList.AddRange(locations);
            var sqlStr = new StringBuilder();
            foreach (var item in locations)
            {
                if (sqlStr.Length == 0)
                {
                    sqlStr.Append(@"select Item,SUM(Qty) from MRP_MrpShipPlan 
                                    where PlanVersion=? and StartTime>? and StartTime<=? and LocationFrom in(?");
                }
                else
                {
                    sqlStr.Append(",?");
                }
            }
            sqlStr.Append(") group by Item");
            var outQtyDic = this.genericMgr.FindAllWithNativeSql<object[]>(sqlStr.ToString(), paramList.ToArray())
                .ToDictionary(d => (string)d[0], d => (double)d[1]);
            #endregion

            #region 待入 生产入的数量
            var inQtyDic = (from p in GetProductPlanInList(mrpPlanMaster, mrpPlanMaster.SnapTime, dateTime)
                            group p by
                            new
                            {
                                p.Item
                            } into g
                            select new
                            {
                                Item = g.Key.Item,
                                Qty = g.Sum(q => q.Qty)
                            }).ToDictionary(d => d.Item, d => d.Qty);
            #endregion

            //期初库存
            var inventoryBalanceList = this.genericMgr.FindAllIn<InventoryBalance>
                (" from InventoryBalance where SnapTime =? and Location in(? ", locations,
                new object[] { mrpPlanMaster.SnapTime });
            var inventoryBalanceDic = (from p in inventoryBalanceList
                                       group p by new
                                       {
                                           Item = p.Item
                                       } into g
                                       select new
                                       {
                                           Item = g.Key.Item,
                                           Qty = g.Sum(q => q.Qty)
                                       }).ToDictionary(d => d.Item, d => d.Qty);
            var items = inQtyDic.Keys.Union(inventoryBalanceDic.Keys).OrderBy(p => p);
            var containers = this.genericMgr.FindAll<Container>();
            foreach (var itemCode in items)
            {
                var item = itemMgr.GetCacheItem(itemCode);
                if (item.ContainerSize > 0)
                {
                    var container = containers.Single(p => p.Code == item.Container);
                    var containerView = new ContainerView();
                    containerView.Item = itemCode;
                    containerView.ItemDescription = item.Description;
                    double qty = inventoryBalanceDic.ValueOrDefault(itemCode)
                                + inQtyDic.ValueOrDefault(containerView.Item)
                                - outQtyDic.ValueOrDefault(containerView.Item);
                    containerView.Qty = qty / (double)item.UnitCount / item.ContainerSize;
                    containerView.ContainerQty = (double)container.Qty;
                    containerView.Container = item.Container;
                    containerView.ContainerDescription = container.Description;
                    if (containerView.Qty > 0)
                    {
                        containerViewList.Add(containerView);
                    }
                }
            }
            return containerViewList;
        }

        private void IterateMrpShipPlan(List<MrpShipPlan> shipPlanList, MrpShipPlan mrpShipPlan, int i)
        {
            if (mrpShipPlan.OrderType == Sconit.CodeMaster.OrderType.Distribution)
            {
                if (mrpShipPlan.SourceType == Sconit.CodeMaster.MrpSourceType.Order)
                {
                    mrpShipPlan.OrderNo = this.genericMgr.FindById<OrderDetail>(mrpShipPlan.SourceId).OrderNo;
                }
                shipPlanList.Add(mrpShipPlan);
            }
            else
            {
                var newShipPlans = this.genericMgr.FindAll<MrpShipPlan>
                    (@"from MrpShipPlan as m where Id=? ", new object[] { mrpShipPlan.SourceId });
                if (newShipPlans == null || newShipPlans.Count() == 0)
                {
                    if (mrpShipPlan.SourceType == Sconit.CodeMaster.MrpSourceType.Order)
                    {
                        mrpShipPlan.OrderNo = this.genericMgr.FindById<OrderDetail>(mrpShipPlan.SourceId).OrderNo;
                    }
                    mrpShipPlan.ItemDescription = itemMgr.GetCacheItem(mrpShipPlan.Item).Description;
                    shipPlanList.Add(mrpShipPlan);
                }
                else
                {
                    foreach (var newShipPlan in newShipPlans)
                    {
                        newShipPlan.ItemDescription = itemMgr.GetCacheItem(newShipPlan.Item).Description;
                        if (newShipPlan.OrderType == Sconit.CodeMaster.OrderType.Distribution
                            || newShipPlan.OrderType == Sconit.CodeMaster.OrderType.Production
                            || newShipPlan.OrderType == Sconit.CodeMaster.OrderType.SubContract)
                        {
                            if (newShipPlan.SourceType == Sconit.CodeMaster.MrpSourceType.Order)
                            {
                                newShipPlan.OrderNo = this.genericMgr.FindById<OrderDetail>(newShipPlan.SourceId).OrderNo;
                            }
                            shipPlanList.Add(newShipPlan);
                        }
                        else
                        {
                            i++;
                            if (i < 10)
                            {
                                IterateMrpShipPlan(shipPlanList, newShipPlan, i);
                            }
                        }
                    }
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public string GetMrpInvIn(DateTime planVersion, CodeMaster.ResourceGroup resourceGroup, string flow, bool isShowDetail)
        {
            StringBuilder str = new StringBuilder(@"<table cellpadding='0' cellspacing='0' border='0' class='display'
                                                    id='datatable' width='100%'><tr>");

            var flowMasters = this.genericMgr.FindAll<FlowMaster>
                (" from FlowMaster where ResourceGroup = ? ", resourceGroup);
            if (!string.IsNullOrWhiteSpace(flow))
            {
                flowMasters = flowMasters.Where(p => p.Code == flow).ToList();
            }

            //var shipGroupList = this.genericMgr.FindAllIn<MrpShipPlanGroup>
            //    (" from MrpShipPlanGroup  where PlanVersion =? and StartTime>=? and Flow in (?",
            //    flowMasters.Select(p => p.Code), new object[] { planVersion, planVersion.Date });

            //var g1 = shipGroupList.GroupBy(p => p.WindowTime, (k, g) => new { k, g }).OrderBy(p => p.k);

            var shipPlanList = this.genericMgr.FindAllIn<MrpShipPlan>
                (" from MrpShipPlan  where PlanVersion =? and Flow in (?",
                flowMasters.Select(p => p.Code), new object[] { planVersion });

            var g1 = shipPlanList.GroupBy(p => p.WindowTime.Date)
                .Where(p => p.Key >= planVersion.Date)
                .OrderBy(p => p.Key);

            #region head
            str.Append("<th style=\"text-align:center\" >");
            str.Append("序号");
            str.Append("</th>");

            str.Append("<th style=\"text-align:center\" >");
            str.Append("物料号");
            str.Append("</th>");

            str.Append("<th style=\"text-align:center;min-width:150px\" >");
            str.Append("物料描述");
            str.Append("</th>");

            str.Append("<th>");
            str.Append("单位");
            str.Append("</th>");

            foreach (var g in g1)
            {
                str.Append("<th style=\"text-align:center;min-width:40px\" >");
                str.Append(g.Key.ToString("MM-dd"));
                str.Append("</th>");
            }
            str.Append("</tr>");
            #endregion

            #region body
            var g2 = shipPlanList.GroupBy(p => p.Item).OrderBy(p => p.Key)
                .Select(p => new
                {
                    Item = p.Key,
                    Dic = p.GroupBy(q => q.WindowTime.Date).ToDictionary(d => d.Key, d => d.ToList())
                }).ToList();

            int l = 0;
            foreach (var gPlan in g2)
            {
                var item = itemMgr.GetCacheItem(gPlan.Item);
                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }
                str.Append("<td>");
                str.Append(l);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(item.Code);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(item.FullDescription);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(item.Uom);
                str.Append("</td>");

                foreach (var g in g1)
                {
                    double qty = 0;
                    StringBuilder title = new StringBuilder();
                    var plans = gPlan.Dic.ValueOrDefault(g.Key);
                    if (plans != null)
                    {
                        qty = plans.Sum(p => p.Qty);
                        if (isShowDetail)
                        {
                            title.Append("cssbody=[obbd] cssheader=[obhd] header=[库位:");
                            title.Append(plans.First().LocationTo);
                            title.Append(" 来源明细:] body=[<table width=100%>");
                            title.Append("<tr><td>区域</td><td>路线</td><td>物料</td><td>数量</td><td>类型</td></tr>");
                            foreach (var plan in plans)
                            {
                                string parentItemDescription = "-";
                                if (!string.IsNullOrWhiteSpace(plan.ParentItem))
                                {
                                    parentItemDescription = this.itemMgr.GetCacheItem(plan.ParentItem).Description;
                                }
                                title.Append("<tr><td>");
                                title.Append(string.IsNullOrWhiteSpace(plan.SourceParty) ? "-" : plan.SourceParty);
                                title.Append("</td><td>");
                                title.Append(string.IsNullOrWhiteSpace(plan.SourceFlow) ? "-" : plan.SourceFlow);
                                title.Append("</td><td>");
                                title.Append(parentItemDescription);
                                title.Append("</td><td>");
                                title.Append(plan.Qty.ToString("0.##"));
                                title.Append("</td><td>");
                                title.Append(systemMgr.GetCodeDetailDescription(CodeMaster.CodeMaster.MrpSourceType, (int)plan.SourceType));
                                title.Append("</td></tr>");
                            }
                            title.Append("</table>]");
                        }
                    }
                    str.Append(string.Format("<td style='text-align:center' title ='{1}'>{0}</td>", qty.ToString("0.##"), title.ToString()));
                }
                str.Append("</tr>");
            }
            str.Append("</table>");
            #endregion
            return str.ToString();
        }

        [Transaction(TransactionMode.Requires)]
        public string GetStringRccpPlanView(IList<RccpPlan> rccpPlanList, int planVersion, string timeType)
        {
            if (rccpPlanList.Count == 0)
            {
                return "没有记录";
            }
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head

            str.Append("<th>");
            str.Append(Resources.MRP.RccpPlan.RccpPlan_Flow);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.MRP.RccpPlan.RccpPlan_Item);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.MRP.RccpPlan.RccpPlan_ItemDescription);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.MRP.RccpPlan.RccpPlan_Uom);
            str.Append("</th>");

            var dateIndexs = new List<string>();
            if (timeType == "WindowTime")
            {
                dateIndexs = rccpPlanList.Select(p => p.DateIndexTo).Distinct().OrderBy(p => p).ToList();
            }
            else
            {
                dateIndexs = rccpPlanList.Select(p => p.DateIndex).Distinct().OrderBy(p => p).ToList();
            }

            foreach (var dateIndex in dateIndexs)
            {
                str.Append("<th style='min-width:50px'>");
                str.Append(dateIndex);
                str.Append("</th>");
            }

            str.Append("</tr></thead><tbody>");
            #endregion

            #region
            IList<object[]> planLogs = new List<object[]>();
            if (planVersion > 0)
            {
                var sql = string.Empty;
                if (timeType == "WindowTime")
                {
                    sql = @"SELECT T.PlanId,T.Qty FROM
                         (select distinct(PlanId) from MRP_RccpPlanLog where PlanVersion <=? and DateType=? and DateIndexTo between ? and ?) AS E
                          CROSS APPLY (SELECT TOP(1)* FROM MRP_RccpPlanLog AS T1 WHERE E.PlanId = T1.PlanId ORDER BY T1.PlanVersion DESC) AS T";
                }
                else
                {
                    sql = @"SELECT T.PlanId,T.Qty FROM
                         (select distinct(PlanId) from MRP_RccpPlanLog where PlanVersion <=? and DateType=? and DateIndex between ? and ?) AS E
                          CROSS APPLY (SELECT TOP(1)* FROM MRP_RccpPlanLog AS T1 WHERE E.PlanId = T1.PlanId ORDER BY T1.PlanVersion DESC) AS T";
                }

                planLogs = this.genericMgr.FindAllWithNativeSql<object[]>(sql, new object[] { planVersion, rccpPlanList[0].DateType, dateIndexs.Min(), dateIndexs.Max() });
            }
            var planLogDic = planLogs.ToDictionary(d => (int)d[0], d => (double)d[1]);

            var rccpPlanListGruopby = from r in rccpPlanList
                                      orderby r.Item
                                      group r by
                                      new
                                      {
                                          Flow = r.Flow,
                                          Item = r.Item,
                                      } into g
                                      select new
                                      {
                                          Flow = g.Key.Flow,
                                          Item = g.Key.Item,
                                          List = g
                                      };
            int l = 0;
            foreach (var groupPlan in rccpPlanListGruopby)
            {
                Item newItem = itemMgr.GetCacheItem(groupPlan.Item);

                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }


                str.Append("<td style=\"text-align:center\">");
                str.Append(groupPlan.Flow);
                str.Append("</td>");

                str.Append("<td style=\"text-align:center\">");
                str.Append(groupPlan.Item);
                str.Append("</td>");
                str.Append("<td>");
                str.Append(newItem.Description);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(newItem.Uom);
                str.Append("</td>");

                #region
                foreach (var dateIndex in dateIndexs)
                {
                    var rccpPlanFirst = new RccpPlan();
                    if (timeType == "WindowTime")
                    {
                        rccpPlanFirst = groupPlan.List.FirstOrDefault(m => m.DateIndexTo == dateIndex);
                    }
                    else
                    {
                        rccpPlanFirst = groupPlan.List.FirstOrDefault(m => m.DateIndex == dateIndex);
                    }

                    if (rccpPlanFirst != null)
                    {
                        if (planVersion > 0)
                        {
                            //var rccpPlanLog = this.genericMgr.FindAll<RccpPlanLog>
                            //    ("from RccpPlanLog where PlanId=?  and PlanVersion <=? order by PlanVersion desc",
                            //    new object[] { 
                            //            rccpPlanFirst.Id,
                            //            planVersion
                            //        }, 0, 1).FirstOrDefault();
                            var qty = planLogDic.ValueOrDefault(rccpPlanFirst.Id);
                            if (qty != rccpPlanFirst.Qty)
                            {
                                str.Append("<td style=\"background:#FFF2F2;\">");
                            }
                            else
                            {
                                str.Append("<td>");
                            }
                            str.Append(qty);
                            str.Append("</td>");
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append(rccpPlanFirst.Qty);
                            str.Append("</td>");
                        }
                    }
                    else
                    {
                        str.Append("<td>");
                        str.Append("0");
                        str.Append("</td>");
                    }
                }

                #endregion

                str.Append("</tr>");
            }
            #endregion

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }

        [Transaction(TransactionMode.Requires)]
        public string GetStringMrpPlanView(IList<MrpPlan> mrpPlanList, DateTime startDate, int planVersion, string reqUrl)
        {
            User user = SecurityContextHolder.Get();
            if (mrpPlanList.Count == 0)
            {
                return "没有记录";
            }
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            str.Append("<th>");
            str.Append(Resources.MRP.MrpPlan.MrpPlan_Flow);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.MRP.MrpPlan.MrpPlan_Location);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.MRP.MrpPlan.MrpPlan_Item);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.MRP.MrpPlan.MrpPlan_ItemDescription);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.MRP.MrpPlan.MrpPlan_Uom);
            str.Append("</th>");

            DateTime dt = startDate;
            for (int i = 0; i < 14; i++)
            {
                str.Append("<th>");
                if (planVersion == 0 && user.UrlPermissions.Contains("Url_OrderMstr_Distribution_New"))
                {
                    ///DistributionOrder/NewFromPlan?flow=O100031&planDate=30207
                    string url = string.Format("<a href='http://{0}DistributionOrder/NewFromPlan?Flow={1}&PlanDate={2}&StartDate={3}&BackUrl={4}'>{5}</a>",
                        reqUrl, mrpPlanList.First().Flow, dt.ToString("yyyy-MM-dd"), startDate.ToString("yyyy-MM-dd"), "~/MrpPlan/MrpPlanView", dt.ToString("MM-dd"));
                    str.Append(url);
                }
                else
                {
                    str.Append(dt.ToString("MM-dd"));
                }
                str.Append("</th>");
                dt = dt.AddDays(1);
            }
            str.Append("</tr></thead><tbody>");

            IList<object[]> planLogs = new List<object[]>();
            if (planVersion > 0)
            {
                var sql = @" SELECT T.PlanDate,T.Flow,T.Location,T.Item,T.Qty FROM
                            (select distinct PlanDate,Flow,Location,Item from MRP_MrpPlanLog where PlanVersion <=? and PlanDate between ? and ? ) AS E
                            CROSS APPLY (SELECT TOP(1)* FROM MRP_MrpPlanLog AS T1 
                            WHERE E.PlanDate=T1.PlanDate and E.Flow=T1.Flow and E.Location=T1.Location and E.Item=T1.Item
                            ORDER BY T1.PlanVersion DESC) AS T ";
                planLogs = this.genericMgr.FindAllWithNativeSql<object[]>(sql, new object[] { planVersion, startDate, dt });
            }
            var planLogDic = planLogs.GroupBy(p => new
                            {
                                PlanDate = (DateTime)p[0],
                                Flow = (string)p[1],
                                Location = (string)p[2],
                                Item = (string)p[3]
                            }, (k, g) => new { k, Qty = (double)g.First()[4] })
                            .ToDictionary(d => d.k, d => d.Qty);

            if (mrpPlanList != null && mrpPlanList.Count > 0)
            {
                var mrpPlanListGruopby = from p in mrpPlanList
                                         group p by
                                         new
                                         {
                                             Flow = p.Flow,
                                             Item = p.Item,
                                             Location = p.Location,
                                         } into g
                                         select new
                                         {
                                             Flow = g.Key.Flow,
                                             Item = g.Key.Item,
                                             Location = g.Key.Location,
                                             List = g
                                         };
                #region
                int l = 0;
                //var items = this.genericMgr.FindAllIn<Item>("from Item where Code in (? ", mrpPlanListGruopby.Select(p => p.Item).Distinct());
                foreach (var mrpPlanGroup in mrpPlanListGruopby)
                {
                    Item newItem = itemMgr.GetCacheItem(mrpPlanGroup.Item);

                    l++;
                    if (l % 2 == 0)
                    {
                        str.Append("<tr class=\"t-alt\">");
                    }
                    else
                    {
                        str.Append("<tr>");
                    }
                    str.Append("<td style=\"text-align:center\">");
                    str.Append(mrpPlanGroup.Flow);
                    str.Append("</td>");
                    str.Append("<td style=\"text-align:center\">");
                    str.Append(mrpPlanGroup.Location);
                    str.Append("</td>");
                    str.Append("<td style=\"text-align:center\">");
                    str.Append(mrpPlanGroup.Item);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(newItem.Description);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(newItem.Uom);
                    str.Append("</td>");

                    #region
                    DateTime time = startDate;
                    for (int j = 0; j < 14; j++)
                    {
                        if (j == 0)
                        {
                            time = startDate;
                        }
                        var mrpPlanFirst = mrpPlanGroup.List.FirstOrDefault(m => m.PlanDate.ToString("yyyy-MM-dd") == time.ToString("yyyy-MM-dd"));

                        if (mrpPlanFirst != null)
                        {
                            if (planVersion > 0)
                            {
                                var qty = planLogDic.ValueOrDefault(new
                                    {
                                        mrpPlanFirst.PlanDate,
                                        mrpPlanFirst.Flow,
                                        mrpPlanFirst.Location,
                                        mrpPlanFirst.Item
                                    });
                                //var mrpPlanLog = this.genericMgr.FindAll<MrpPlanLog>
                                //    ("from MrpPlanLog where PlanDate = ? and Item =? and Flow =? and Location =? and PlanVersion <=? order by PlanVersion desc",
                                //    new object[] { 
                                //        mrpPlanFirst.PlanDate, 
                                //        mrpPlanFirst.Item, 
                                //        mrpPlanFirst.Flow, 
                                //        mrpPlanFirst.Location,
                                //        planVersion
                                //    }, 0, 1).FirstOrDefault();

                                if (qty != mrpPlanFirst.Qty)
                                {
                                    str.Append(string.Format("<td style=\"background:#FFF2F2;\" title='{0}'>", mrpPlanFirst.OrderQty.ToString("0.##")));
                                }
                                else
                                {
                                    str.Append(string.Format("<td title='{0}'>", mrpPlanFirst.OrderQty.ToString("0.##")));
                                }
                                str.Append(qty);

                                str.Append("</td>");
                            }
                            else
                            {
                                str.Append(string.Format("<td title='{0}'>", mrpPlanFirst.OrderQty.ToString("0.##")));
                                str.Append(mrpPlanFirst.Qty.ToString("0.##"));
                                str.Append("</td>");
                            }
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append("0");
                            str.Append("</td>");
                        }
                        time = time.AddDays(1);
                    }
                    #endregion

                    str.Append("</tr>");
                }
                #endregion
            }
            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }

        #endregion Customized Methods

        class ShiftPlanQty
        {
            public string Island { get; set; }
            public string IslandDescription { get; set; }
            public string Machine { get; set; }
            public string Item { get; set; }
            public int Sequence { get; set; }
            public string Shift { get; set; }
            public DateTime PlanDate { get; set; }
            public double Qty { get; set; }
            public double UnitCount { get; set; }
        }

        class DailyPlanQty
        {
            public DateTime PlanDate { get; set; }
            public string Item { get; set; }
            public double Qty { get; set; }
        }
    }
}
