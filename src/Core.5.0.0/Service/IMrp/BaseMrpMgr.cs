using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Utility;
using com.Sconit.Entity.MRP.MD;

namespace com.Sconit.Service.MRP
{
    public class BaseMgr
    {
        public IItemMgr itemMgr { get; set; }
        public IBomMgr bomMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }

        #region cacheLocation
        private static IDictionary<string, Location> cachedAllLocation;
        private static DateTime cacheDateTime;

        protected IDictionary<string, Location> GetCacheAllLocation()
        {
            if (cachedAllLocation == null || cacheDateTime < DateTime.Now.AddMinutes(-10))
            {
                cacheDateTime = DateTime.Now;
                var allLocation = this.genericMgr.FindAll<Location>();
                cachedAllLocation = allLocation.ToDictionary(d => d.Code, d => d);
            }
            return cachedAllLocation;
        }

        protected Location GetCacheLocation(string locationCode)
        {
            Location location = null;
            if (!GetCacheAllLocation().TryGetValue(locationCode, out location))
            {
                cachedAllLocation = null;
                GetCacheAllLocation().TryGetValue(locationCode, out location);
            };
            return location;
        }
        #endregion

        //得到基本单位用量
        protected List<MrpBom> GetMrpBomList(string fgCode, string bomCode, DateTime effdate, BusinessException businessException, bool isOnlyNext = false)
        {
            var mrpBoms = new List<MrpBom>();
            try
            {
                var bomMaster = this.bomMgr.GetCacheBomMaster(bomCode);
                if (bomMaster != null)
                {
                    var fgItem = this.itemMgr.GetCacheItem(fgCode);
                    IList<BomDetail> bomDetails = new List<BomDetail>();
                    if (isOnlyNext)
                    {
                        bomDetails = bomMgr.GetOnlyNextLevelBomDetail(bomCode, effdate, true);
                    }
                    else
                    {
                        bomDetails = bomMgr.GetFlatBomDetail(bomCode, effdate, true);
                    }
                    foreach (var bomDetail in bomDetails)
                    {
                        var item = this.itemMgr.GetCacheItem(bomDetail.Item);
                        double calculatedQty = 1;
                        //1.将bomMaster的单位转成基本单位 
                        double fgQty = ConvertItemUomQty(fgItem.Code, bomMaster.Uom, 1, fgItem.Uom, businessException);
                        //2.将BomDetail的单位转成基本单位
                        double itemQty = ConvertItemUomQty(item.Code, bomDetail.Uom, (double)bomDetail.CalculatedQty, item.Uom, businessException);
                        //3.单位成品用量
                        calculatedQty = itemQty / fgQty;

                        var mrpBom = new MrpBom();
                        mrpBom.Bom = bomCode;
                        mrpBom.Item = bomDetail.Item;
                        mrpBom.Location = bomDetail.Location;
                        mrpBom.RateQty = calculatedQty;
                        mrpBom.IsSection = item.ItemCategory == "ZHDM";
                        mrpBom.ScrapPercentage = (double)bomDetail.ScrapPercentage;
                        //if (mrpBom.IsSection)
                        //{
                        //    mrpBom.ScrapPercentage = fgItem.ScrapPercent;
                        //}
                        mrpBoms.Add(mrpBom);
                    }
                }
            }
            catch (Exception ex)
            {
                businessException.AddMessage(new Message(CodeMaster.MessageType.Error, string.Format("BomCode {0} Error,{1}", bomCode, ex.Message)));
            }
            return mrpBoms;
        }

        protected double ConvertItemUomQty(string item, string sourceUom, double sourceQty, string targetUom, BusinessException businessException)
        {
            try
            {
                return (double)this.itemMgr.ConvertItemUomQty(item, sourceUom, (decimal)sourceQty, targetUom);
            }
            catch (Exception)
            {
                string message = string.Format("没有找到物料:{0}从单位:{1}到单位:{2}的转换", item, sourceUom, targetUom);
                if (businessException != null)
                {
                    businessException.AddMessage(new Message(CodeMaster.MessageType.Error, message));
                }
                return 1.0;
            }
        }

        protected double ConvertItemUomQty(string item, string sourceUom, double sourceQty, string targetUom)
        {
            return ConvertItemUomQty(item, sourceUom, sourceQty, targetUom, null);
        }


        protected List<MrpShipPlan> GetProductPlanInList(DateTime planVersion, string flow)
        {
            var planInList = this.genericMgr.FindAllWithNativeSql<object[]>
                ("exec USP_Busi_MRP_GetPlanIn ?,?", new object[] { planVersion, flow },
                new NHibernate.Type.IType[] { NHibernate.NHibernateUtil.DateTime, NHibernate.NHibernateUtil.String });

            //[Flow] [varchar](50) NULL,
            //[OrderType] [tinyint] NOT NULL,
            //[Item] [varchar](50) NOT NULL,
            //[StartTime] [datetime] NOT NULL,
            //[WindowTime] [datetime] NOT NULL,
            //[LocationFrom] [varchar](50) NULL,
            //[LocationTo] [varchar](50) NULL,
            //[Qty] [float] NOT NULL,
            //[Bom] [varchar](50) NULL,
            //[SourceType] [tinyint] NOT NULL,
            //[SourceParty] [varchar](50) NULL,

            var shipPlanList = planInList.Select(p =>
                new MrpShipPlan
                {
                    Flow = (string)p[0],
                    OrderType = (CodeMaster.OrderType)(int.Parse(p[1].ToString())),
                    Item = (string)p[2],
                    StartTime = (DateTime)p[3],
                    WindowTime = (DateTime)p[4],
                    LocationFrom = (string)p[5],
                    LocationTo = (string)p[6],
                    Qty = (double)p[7],
                    Bom = (string)p[8],
                    SourceType = (CodeMaster.MrpSourceType)(int.Parse(p[9].ToString())),
                    SourceParty = (string)p[10]
                }).ToList();
            return shipPlanList;
        }

        protected List<MrpShipPlan> GetProductPlanInList(MrpPlanMaster mrpPlanMaster, DateTime startTime, DateTime endTime, IList<MrpFlowDetail> mrpFlowDetailList = null)
        {
            var shipPlanList = new List<MrpShipPlan>();
            //改成存储过程
            shipPlanList = GetProductPlanInList(mrpPlanMaster.PlanVersion, null) ?? new List<MrpShipPlan>();
            shipPlanList = shipPlanList.Where(p => p.StartTime >= startTime && p.StartTime < endTime).ToList();
            return shipPlanList;

            //订单 //todo 状态为Cancel的处理方法,订单类型为Production
            var activeOrderDic = this.genericMgr.FindAll<ActiveOrder>
                (@" from ActiveOrder where SnapTime =? and ResourceGroup =? and IsIndepentDemand=? ",
                new object[] { mrpPlanMaster.SnapTime, mrpPlanMaster.ResourceGroup, false })
                .GroupBy(p => p.Flow, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g);
            shipPlanList.AddRange(from p in activeOrderDic.SelectMany(p => p.Value)
                                  select new MrpShipPlan
                                  {
                                      Flow = p.Flow,
                                      Item = p.Item,
                                      LocationFrom = p.LocationFrom,
                                      LocationTo = p.LocationTo,
                                      StartTime = p.StartTime,
                                      WindowTime = p.WindowTime,
                                      SourceId = p.OrderDetId,
                                      SourceType = CodeMaster.MrpSourceType.Order,
                                      OrderType = p.OrderType,
                                      Qty = p.DemandQty,
                                      SourceFlow = p.Flow,
                                      SourceParty = p.PartyFrom,
                                      ParentItem = p.Item
                                  });

            //计划,订单覆盖计划
            if (mrpPlanMaster.ResourceGroup == CodeMaster.ResourceGroup.FI)
            {
                var shiftPlanList = this.genericMgr.FindAll<MrpFiShiftPlan>
                    (" from MrpFiShiftPlan where PlanVersion = ? and StartTime >=? and StartTime<=? ",
                    new object[] { mrpPlanMaster.PlanVersion, startTime, endTime });
                foreach (var plan in shiftPlanList)
                {
                    var activeOrderCount = (activeOrderDic.ValueOrDefault(plan.ProductLine) ?? new List<ActiveOrder>())
                        .Count(p => p.StartTime.Date == plan.PlanDate.Date && p.Shift == plan.Shift);
                    if (activeOrderCount == 0)
                    {
                        MrpShipPlan mrpShipPlan = new MrpShipPlan();
                        mrpShipPlan.Bom = plan.Bom;
                        mrpShipPlan.Flow = plan.ProductLine;
                        mrpShipPlan.Item = plan.Item;
                        mrpShipPlan.LocationFrom = plan.LocationFrom;
                        mrpShipPlan.LocationTo = plan.LocationTo;
                        mrpShipPlan.Bom = plan.Bom;
                        mrpShipPlan.StartTime = plan.StartTime;
                        mrpShipPlan.WindowTime = plan.WindowTime;
                        mrpShipPlan.SourceId = plan.Id;
                        mrpShipPlan.SourceType = CodeMaster.MrpSourceType.FiShift;
                        mrpShipPlan.OrderType = CodeMaster.OrderType.Production;
                        mrpShipPlan.Qty = plan.Qty;
                        mrpShipPlan.SourceFlow = plan.ProductLine;
                        mrpShipPlan.SourceParty = GetCacheLocation(mrpShipPlan.LocationFrom).Region;
                        mrpShipPlan.ParentItem = plan.Item;
                        shipPlanList.Add(mrpShipPlan);
                    }
                }
            }
            else if (mrpPlanMaster.ResourceGroup == CodeMaster.ResourceGroup.EX)
            {
                var exShiftPlanList = this.genericMgr.FindAll<MrpExItemPlan>
                    (" from MrpExItemPlan where PlanVersion = ? and PlanDate >=? and PlanDate<=? ",
                    new object[] { mrpPlanMaster.PlanVersion, startTime, endTime });
                var shipPlans = (from p in exShiftPlanList
                                 group p by new
                                 {
                                     Flow = p.ProductLine,
                                     Item = p.Item,
                                     PlanDate = p.PlanDate,
                                 } into g
                                 select new MrpShipPlan
                                 {
                                     //Bom = g.Key.Bom,
                                     Flow = g.Key.Flow,
                                     Item = g.Key.Item,
                                     //LocationFrom = g.Key.LocationFrom,
                                     //LocationTo = g.Key.LocationTo,
                                     StartTime = g.Key.PlanDate,
                                     WindowTime = g.Key.PlanDate.AddDays(1),
                                     SourceId = g.Min(q => q.Id),
                                     SourceType = CodeMaster.MrpSourceType.ExDay,
                                     OrderType = CodeMaster.OrderType.Production,
                                     Qty = g.Sum(q => q.Qty),
                                     SourceFlow = g.Key.Flow,
                                     //SourceParty = GetCacheLocation(g.Key.LocationFrom).Region,
                                     ParentItem = g.Key.Item
                                 }).ToList();
                var mrpFlowDetails = new List<MrpFlowDetail>();
                if (mrpFlowDetailList == null)
                {
                    mrpFlowDetails = this.genericMgr.FindAll<MrpFlowDetail>
                        (@" from MrpFlowDetail as m where m.SnapTime = ? and m.ResourceGroup=? ",
                        new object[] { mrpPlanMaster.SnapTime, mrpPlanMaster.ResourceGroup }).ToList();
                }
                else
                {
                    mrpFlowDetails = mrpFlowDetailList.Where(p => p.ResourceGroup == mrpPlanMaster.ResourceGroup).ToList();
                }
                foreach (var shipPlan in shipPlans)
                {
                    var flowDetail = mrpFlowDetails.FirstOrDefault(f => f.Flow == shipPlan.Flow && f.Item == shipPlan.Item
                        && f.StartDate <= shipPlan.StartTime && f.EndDate > shipPlan.StartTime);
                    if (flowDetail == null)
                    {
                        flowDetail = mrpFlowDetails.FirstOrDefault(f => f.Flow == shipPlan.Flow);
                    }
                    shipPlan.Bom = flowDetail.Bom;
                    shipPlan.LocationFrom = flowDetail.LocationFrom;
                    shipPlan.LocationTo = flowDetail.LocationTo;
                    shipPlan.SourceParty = flowDetail.PartyFrom;
                }
                shipPlanList.AddRange(shipPlans);
            }
            else if (mrpPlanMaster.ResourceGroup == CodeMaster.ResourceGroup.MI)
            {
                var shiftPlanDic = this.genericMgr.FindEntityWithNativeSql<MrpMiShiftPlan>
                   (@"select a.* from MRP_MrpMiShiftPlan a join MRP_MrpMiDateIndex b 
                    on a.CreateDate = b.CreateDate and a.ProductLine = b.ProductLine and a.PlanDate = b.PlanDate 
                    where b.PlanDate >=? and b.PlanDate<=? and b.IsActive=? ", new object[] { startTime, endTime, true })
                .GroupBy(p => p.PlanDate, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.ToList());

                var mrpMiPlanDic = this.genericMgr.FindAll<MrpMiPlan>
                (" from MrpMiPlan where PlanVersion = ? and PlanDate >=? and PlanDate<=? ",
                   new object[] { mrpPlanMaster.PlanVersion, startTime, endTime })
                .GroupBy(p => p.PlanDate, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.ToList());

                //优先级:订单>班产计划>日计划
                DateTime currentDate = startTime;
                while (currentDate < endTime)
                {
                    currentDate = currentDate.AddDays(1);
                    var shiftPlans = shiftPlanDic.ValueOrDefault(currentDate);
                    if (shiftPlans != null)
                    {
                        foreach (var plan in shiftPlans)
                        {
                            var activeOrderCount = (activeOrderDic.ValueOrDefault(plan.ProductLine) ?? new List<ActiveOrder>())
                             .Count(p => p.StartTime.Date == plan.PlanDate.Date && p.Shift == plan.Shift);
                            if (activeOrderCount == 0)
                            {
                                MrpShipPlan mrpShipPlan = new MrpShipPlan();
                                mrpShipPlan.Bom = plan.Bom;
                                mrpShipPlan.Flow = plan.ProductLine;
                                mrpShipPlan.Item = plan.Item;
                                mrpShipPlan.LocationFrom = plan.LocationFrom;
                                mrpShipPlan.LocationTo = plan.LocationTo;
                                mrpShipPlan.StartTime = plan.StartTime;
                                mrpShipPlan.WindowTime = plan.WindowTime;
                                mrpShipPlan.SourceId = plan.Id;
                                mrpShipPlan.SourceType = CodeMaster.MrpSourceType.MiShift;
                                mrpShipPlan.OrderType = CodeMaster.OrderType.Production;
                                mrpShipPlan.Qty = plan.TotalQty;
                                mrpShipPlan.SourceFlow = plan.ProductLine;
                                mrpShipPlan.SourceParty = GetCacheLocation(mrpShipPlan.LocationFrom).Region;
                                mrpShipPlan.Item = plan.Item;
                                shipPlanList.Add(mrpShipPlan);
                            }
                        }
                    }
                    else
                    {
                        var miPlans = mrpMiPlanDic.ValueOrDefault(currentDate) ?? new List<MrpMiPlan>();
                        foreach (var plan in miPlans)
                        {
                            MrpShipPlan mrpShipPlan = new MrpShipPlan();
                            mrpShipPlan.Bom = plan.Bom;
                            mrpShipPlan.Flow = plan.ProductLine;
                            mrpShipPlan.Item = plan.Item;
                            mrpShipPlan.LocationFrom = plan.LocationFrom;
                            mrpShipPlan.LocationTo = plan.LocationTo;
                            mrpShipPlan.StartTime = plan.PlanDate;
                            mrpShipPlan.WindowTime = plan.PlanDate.AddDays(1);
                            mrpShipPlan.SourceId = plan.Id;
                            mrpShipPlan.SourceType = CodeMaster.MrpSourceType.MiShift;
                            mrpShipPlan.OrderType = CodeMaster.OrderType.Production;
                            mrpShipPlan.Qty = plan.TotalQty;
                            mrpShipPlan.SourceFlow = plan.ProductLine;
                            mrpShipPlan.SourceParty = GetCacheLocation(mrpShipPlan.LocationFrom).Region;
                            mrpShipPlan.Item = plan.Item;
                            shipPlanList.Add(mrpShipPlan);
                        }
                    }
                }
            }
            return shipPlanList;
        }

        protected class MrpBom
        {
            public string Bom { get; set; }
            public string Item { get; set; }
            public string Location { get; set; }
            /// <summary>
            /// 断面的单位长度不包含废品率,其他的已包含废品率
            /// </summary>
            public Double RateQty { get; set; }
            public bool IsSection { get; set; }
            public Double ScrapPercentage { get; set; }
        }
    }

}
