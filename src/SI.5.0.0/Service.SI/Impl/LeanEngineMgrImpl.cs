using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.SI;
using com.Sconit.Entity.VIEW;
using com.Sconit.Service.MRP;
using com.Sconit.Service.SI;
using com.Sconit.Utility;
using LeanEngine;
using LeanEngine.Entity;
using LeanEngine.Utility;

namespace com.Sconit.Service.SI.Impl
{
    [Transactional]
    public class LeanEngineMgrImpl : BaseMgr, ILeanEngineMgr
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.LeanEngine");
        private static Object Lock = new Object();
        //public IGenericMgr siGenericMgr { get; set; }
        public IMrpMgr mrpMgr { get; set; }
        public ICustomizationMgr customizationMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public void RunLeanEngine()
        {
            lock (Lock)
            {
                log.Info("----------------------------------Invincible's dividing line---------------------------------------");
                log.Debug("Lean Engine start working.");

                #region 获取数据（路线、库存、订单、工作日历）
                log.Debug("Begin to load data.");
                DateTime dateTimeNow = DateTime.Now;

                #region 获取所有库位代码
                IList<string> locationCodeList = this.genericMgr.FindAll<string>("select Code from Location");
                #endregion

                #region 获取路线
                IList<com.Sconit.Entity.SCM.FlowStrategy> flowStrategyList = this.genericMgr.FindAll<com.Sconit.Entity.SCM.FlowStrategy>
                    ("from FlowStrategy where Strategy in (?,?,?)",
                   new object[] { CodeMaster.FlowStrategy.KB, CodeMaster.FlowStrategy.JIT, CodeMaster.FlowStrategy.KB2 });  //精益引擎计算只考虑看板和JIT

                IList<FlowMaster> flowMasterList = this.genericMgr.FindEntityWithNativeSql<FlowMaster>
                    (NativeSqlStatement.SELECT_FLOW_MASTER_STATEMENT,
                    new object[] { CodeMaster.FlowStrategy.KB, CodeMaster.FlowStrategy.JIT, CodeMaster.FlowStrategy.KB2, true });

                IList<FlowDetail> flowDetailList = this.genericMgr.FindEntityWithNativeSql<FlowDetail>
                    (NativeSqlStatement.SELECT_FLOW_DETAIL_STATEMENT,
                    new object[] { CodeMaster.FlowStrategy.KB, CodeMaster.FlowStrategy.JIT, CodeMaster.FlowStrategy.KB2, true, dateTimeNow, dateTimeNow, true });

                if (flowDetailList != null && flowDetailList.Count > 0)
                {
                    #region 路线头和路线策略赋值
                    Parallel.ForEach(flowDetailList, (flowDetail) =>
                    {
                        flowDetail.CurrentFlowMaster = flowMasterList.Where(mstr => mstr.Code == flowDetail.Flow).Single();
                        flowDetail.CurrentFlowStrategy = flowStrategyList.Where(str => str.Flow == flowDetail.Flow).Single();
                    });
                    #endregion

                    #region 查找引用路线
                    int maxFlowDetaiId = flowDetailList.Max(det => det.Id);
                    IList<FlowDetail> refFlowDetailList = new List<FlowDetail>();
                    foreach (FlowMaster flowMaster in flowMasterList.Where(f => !string.IsNullOrWhiteSpace(f.ReferenceFlow)))
                    {
                        com.Sconit.Entity.SCM.FlowStrategy flowStrategy = flowStrategyList.Where(str => str.Flow == flowMaster.Code).Single();
                        foreach (FlowDetail orgFlowDetail in flowDetailList.Where(det => det.Flow == flowMaster.Code))
                        {
                            FlowDetail refFlowDetail = Mapper.Map<FlowDetail, FlowDetail>(orgFlowDetail);
                            refFlowDetail.LocationFrom = null;   //来源库位制空，取头上的库位
                            refFlowDetail.LocationTo = null;    //目的库位制空，取头上的库位
                            refFlowDetail.Id = maxFlowDetaiId++; //为了区分不同的FlowDetail，要给flowdetailId赋不同的值。
                            refFlowDetail.CurrentFlowMaster = flowMaster;
                            refFlowDetail.CurrentFlowStrategy = flowStrategy;
                            refFlowDetailList.Add(refFlowDetail);
                        }
                    }

                    ((List<FlowDetail>)flowDetailList).AddRange(refFlowDetailList);
                    #endregion

                    #region 过滤相同路线明细（来源库位、目的库位、零件），不考虑单位、包装等因素
                    //#region 汇总其它需求源
                    //var groupedExtraDemandSource = from det in flowDetailList
                    //                               group det by
                    //                               new
                    //                               {
                    //                                   LocationFrom = !string.IsNullOrWhiteSpace(det.LocationFrom) ? det.LocationFrom : det.CurrentFlowMaster.LocationFrom,
                    //                                   LocationTo = !string.IsNullOrWhiteSpace(det.LocationTo) ? det.LocationTo : det.CurrentFlowMaster.LocationTo,
                    //                                   Item = det.Item,
                    //                               } into result
                    //                               select new
                    //                               {
                    //                                   LocationFrom = result.Key.LocationFrom,
                    //                                   LocationTo = result.Key.LocationTo,
                    //                                   Item = result.Key.Item,
                    //                                   ExtraDemandSource = result.Select(r => r.ExtraDemandSource).ToList()
                    //                               };
                    //#endregion

                    #region 可能存在两条相同的路线明细，包装/单位不同，需要过滤只剩一条
                    //可能出现把有MRPWeight的过滤掉。
                    //可能出现把其它需求源过滤掉。
                    flowDetailList = (from det in flowDetailList
                                      group det by
                                      new
                                      {
                                          LocationFrom = !string.IsNullOrWhiteSpace(det.LocationFrom) ? det.LocationFrom : det.CurrentFlowMaster.LocationFrom,
                                          LocationTo = !string.IsNullOrWhiteSpace(det.LocationTo) ? det.LocationTo : det.CurrentFlowMaster.LocationTo,
                                          Item = det.Item,
                                      } into result
                                      select result.Max<FlowDetail>()).ToList();
                    #endregion
                    #endregion
                }
                #endregion

                #region 获取订单，包含在途
                IList<object[]> orderPlanList = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_Busi_GetOrderPlan4LeanEngine");
                #endregion

                #region 获取库存
                IList<LocationDetailView> locationDetailViewList = this.genericMgr.FindAll<LocationDetailView>();
                #endregion

                #region 获取工作日历，不考虑不同区域的不同工作日历
                IList<WorkingCalendarView> workingCalendarViewList = null;
                if (orderPlanList != null && orderPlanList.Count > 0)
                {
                    //DateTime maxWindowTime = orderPlanList.Select(plan => (DateTime)plan[4]).Max();
                    //workingCalendarViewList = workingCalendarMgr.GetWorkingCalendarView(null, dateTimeNow, maxWindowTime);

                    //如果最大窗口时间落在休息日中，则在往后面取，直到取到工作日
                    //workingCalendarViewList.OrderByDescending(c => c.DateTo).Take(1).Single();

                    //简单处理，给一个月的工作日历
                    workingCalendarViewList = workingCalendarMgr.GetWorkingCalendarViewList(null, null, dateTimeNow.AddDays(-7), dateTimeNow.AddMonths(1));
                }
                #endregion

                log.Debug("Load data success.");
                #endregion

                #region 计算指定供应商的需求
                List<Orders> specifiedOrders = new List<Orders>();
                //指定供应商不为空
                //IList<object[]> specifiedManufacturePlan = (orderPlanList.Where(plan => !string.IsNullOrWhiteSpace((string)plan[3]))).ToList();
                //if (specifiedManufacturePlan != null && specifiedManufacturePlan.Count() > 0)
                //{
                //    specifiedOrders = CalSpecifiedManufactureOrders(specifiedManufacturePlan, flowDetailList, locationDetailViewList, workingCalendarViewList, locationCodeList);
                //}
                #endregion

                List<Orders> orders = CalOrders(orderPlanList, specifiedOrders, flowDetailList, locationDetailViewList, workingCalendarViewList, locationCodeList);

                #region 指定供应商的需求
                if (orders != null && orders.Count > 0)
                {
                    foreach (Orders order in orders)
                    {
                        if (specifiedOrders != null && specifiedOrders.Count > 0)
                        {
                            foreach (Orders specifiedOrder in specifiedOrders)
                            {
                                if (order.Flow.Equals(specifiedOrder.Flow))
                                {
                                    order.ItemFlows.AddRange(specifiedOrder.ItemFlows);
                                }
                            }
                        }
                    }
                }

                if (specifiedOrders != null && specifiedOrders.Count > 0)
                {
                    List<Orders> addOrders = new List<Orders>();
                    foreach (Orders specifiedOrder in specifiedOrders)
                    {
                        if (orders != null && orders.Count > 0)
                        {
                            if (!orders.Select(o => o.Flow).Contains(specifiedOrder.Flow))
                            {
                                addOrders.Add(specifiedOrder);
                            }
                        }
                        else
                        {
                            addOrders.Add(specifiedOrder);
                        }
                    }

                    if (addOrders.Count > 0)
                    {
                        orders.AddRange(addOrders);
                    }
                }
                #endregion

                #region 生成订单
                IList<object[]> erorrList = new List<object[]>();
                if (orders != null && orders.Count > 0)
                {
                    foreach (var order in orders)
                    {
                        FlowMaster flowMaster = flowMasterList.Where(mstr => mstr.Code == order.Flow.Code).Single();
                        flowMaster.FlowDetails = flowDetailList.Where(det => det.Flow == order.Flow.Code).ToList();
                        com.Sconit.Entity.SCM.FlowStrategy flowStrategy = flowStrategyList.Where(str => str.Flow == order.Flow.Code).Single();
                        try
                        {
                            this.ProcessNewOrder(order, flowMaster, flowStrategy);
                        }
                        catch (Exception ex)
                        {
                            this.genericMgr.CleanSession();
                            erorrList.Add(new object[] { order, ex });
                            log.Error("Create order failed.", ex);
                        }
                    }
                }

                SendErrorEmail(erorrList);
                #endregion
                //挤出JIT
                RunJIT_EX();
                log.Debug("Lean Engine finished the job.");
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void RunJIT_EX()
        {
            try
            {
                #region 挤出JIT
                var flowStrategyList = genericMgr.FindAll<Entity.SCM.FlowStrategy>
                    (" from FlowStrategy where Strategy =? ", CodeMaster.FlowStrategy.JIT_EX);

                var flowList = this.genericMgr.FindAll<string>
                  ("select Code from FlowMaster where ResourceGroup = ?", com.Sconit.CodeMaster.ResourceGroup.EX);

                var huToMappings = this.genericMgr.FindAll<HuToMapping>();

                var shiftPlanList = new List<MrpExShiftPlan>();
                DateTime currentDate = DateTime.Now.Date;
                while (currentDate <= DateTime.Now.Date.AddDays(1))
                {
                    foreach (var flow in flowList)
                    {
                        var shiftPlans = mrpMgr.GetMrpExShiftPlanList(currentDate, flow) ?? new List<MrpExShiftPlan>(); ;
                        shiftPlanList.AddRange(shiftPlans);
                    }
                    currentDate = currentDate.AddDays(1);
                }
                foreach (var shiftPlan in shiftPlanList)
                {
                    double _minutes = (shiftPlan.CalShiftQty - shiftPlan.ShiftQty) * (24 / shiftPlan.ShiftType) * 60;
                    _minutes = _minutes > 0 ? _minutes : 0;
                    double qty = _minutes * shiftPlan.Speed / shiftPlan.RateQty;
                    shiftPlan.CurrentQty = shiftPlan.Qty - qty;
                    shiftPlan.CurrentQty = shiftPlan.CurrentQty > 0 ? shiftPlan.CurrentQty : 0.0;
                }

                foreach (var flowStrategy in flowStrategyList)
                {
                    if (!flowStrategy.NextOrderTime.HasValue || flowStrategy.NextOrderTime.Value < DateTime.Now)
                    {
                        try
                        {
                            double leadTime = Utility.DateTimeHelper.TimeTranfer(flowStrategy.LeadTime, flowStrategy.TimeUnit, CodeMaster.TimeUnit.Hour);
                            double emLeadTime = Utility.DateTimeHelper.TimeTranfer(flowStrategy.EmergencyLeadTime, flowStrategy.TimeUnit, CodeMaster.TimeUnit.Hour);
                            string[][] windowTimes = GetWindowTimes(flowStrategy);
                            DateTime winTime1 = flowStrategy.NextWindowTime.HasValue ?
                                flowStrategy.NextWindowTime.Value : this.GetNextWindowTime(DateTime.Now, windowTimes, flowStrategy.WeekInterval);
                            DateTime winTime2 = this.GetNextWindowTime(winTime1, windowTimes, flowStrategy.WeekInterval);
                            flowStrategy.CurrentNextWindowTime = winTime2;
                            flowStrategy.CurrentNextOrderTime = this.GetNextOrderTime(winTime2, leadTime);

                            var flowMaster = this.genericMgr.FindById<FlowMaster>(flowStrategy.Flow);
                            if (!flowMaster.IsAutoCreate)
                            {
                                continue;
                            }
                            var flowDetails = flowMgr.GetFlowDetailList(flowMaster, false, true)
                                .Where(p => p.IsAutoCreate).ToList();

                            var flowDetailDic = flowDetails
                                .GroupBy(p => p.Item, (k, g) => new { Item = k, FlowDetail = g.First() })
                                .ToDictionary(d => d.Item, d => d.FlowDetail);

                            #region 计算winTime1和winTime2之间的需求
                            var orderDetails = shiftPlanList.Where(p => p.StartTime < winTime2 && p.WindowTime > winTime1)
                                .Where(p => p.WindowTime > p.StartTime)
                                .Where(p => p.Bom != com.Sconit.Entity.BusinessConstants.VIRTUALSECTION && !string.IsNullOrWhiteSpace(p.Bom))
                                .Select(p => new
                                {
                                    Tracer = p,
                                    Flow = p.ProductLine,
                                    Item = p.Item,
                                    Bom = p.Bom,
                                    Qty = p.CurrentQty * ((p.WindowTime < winTime2 ? p.WindowTime : winTime2) - (p.StartTime > winTime1 ? p.StartTime : winTime1)).TotalMinutes / (p.WindowTime - p.StartTime).TotalMinutes
                                })
                                .GroupBy(p => new { p.Flow, p.Item, p.Bom }, (k, g) => new
                                {
                                    Flow = k.Flow,
                                    ParentItem = k.Item,
                                    UnitQty = itemMgr.ConvertItemUomQty(k.Item, itemMgr.GetCacheItem(k.Item).Uom, 1, bomMgr.GetCacheBomMaster(k.Bom).Uom),//Bom单位转成物料单位
                                    Qty = (decimal)(g.Sum(p => p.Qty)),
                                    Details = bomMgr.GetFlatBomDetail(k.Bom, winTime1),
                                    Tracers = g
                                })
                                .SelectMany(p => p.Details.Where(q => flowDetailDic.ContainsKey(q.Item)).Select(q => new
                                {
                                    Flow = p.Flow,
                                    Direction = customizationMgr.GetHuTo(huToMappings, p.Flow, p.ParentItem),
                                    Item = q.Item,
                                    Qty = itemMgr.ConvertItemUomQty(q.Item, q.Uom, q.CalculatedQty * p.UnitQty * p.Qty, flowDetailDic.ValueOrDefault(q.Item).Uom),//转订单单位
                                    Tracers = p.Tracers
                                }))
                                .GroupBy(p => new { p.Flow, p.Direction, p.Item }, (k, g) => new
                                {
                                    Flow = k.Flow,
                                    Direction = k.Direction,
                                    Item = itemMgr.GetCacheItem(k.Item),
                                    FlowDetail = flowDetailDic.ValueOrDefault(k.Item),
                                    Qty = g.Sum(q => q.Qty),
                                    Tracers = g.SelectMany(q => q.Tracers)
                                })
                                .Select(p => new Entity.ORD.OrderDetail
                                {
                                    Item = p.Item.Code,
                                    UnitCount = p.FlowDetail.UnitCount,
                                    Uom = p.FlowDetail.Uom,
                                    ItemDescription = p.Item.Description,
                                    Sequence = p.FlowDetail.Sequence,
                                    RequiredQty = p.Qty,
                                    OrderedQty = GetRoundOrderQty(p.FlowDetail, p.Qty),
                                    LocationFrom = p.FlowDetail.LocationFrom,
                                    LocationTo = p.FlowDetail.LocationTo,
                                    Direction = p.Direction,
                                    Remark = string.Format("{0}{1}", string.IsNullOrWhiteSpace(p.Direction) ? p.Flow + " " : null, GetOrderDetailRemark(p.Tracers.Select(q => string.IsNullOrWhiteSpace(q.Tracer.Section) ? q.Tracer.Item : q.Tracer.Section).Distinct())),
                                    OrderTracerList = p.Tracers.Select(q => new Entity.ORD.OrderTracer
                                    {
                                        Code = q.Tracer.Section,
                                        FinishedQty = 0,
                                        Item = q.Item,
                                        //OrderDetailId = ,
                                        OrderedQty = (decimal)q.Tracer.CurrentQty,
                                        Qty = (decimal)q.Qty,
                                        RefId = q.Tracer.Id,
                                        ReqTime = q.Tracer.StartTime
                                    }).ToList()
                                })
                                .Where(p => p.OrderedQty > 0)
                                .OrderBy(p => p.Direction)
                                .ThenBy(p => p.Sequence)
                                .ToList();
                            #endregion

                            if (orderDetails.Count() > 0)
                            {
                                OrderMaster orderMaster = this.orderMgr.TransferFlow2Order(flowMaster, false);
                                //orderMaster.IsAutoRelease = false;
                                orderMaster.WindowTime = winTime1;
                                orderMaster.StartTime = winTime1.AddHours(-leadTime);
                                orderMaster.Priority = DateTime.Now.AddHours(emLeadTime) >= winTime1 ? CodeMaster.OrderPriority.Urgent : CodeMaster.OrderPriority.Normal;
                                orderMaster.OrderDetails = orderDetails.ToList();
                                //orderMaster.Dock = groupShiftPlan.Flow;
                                //orderMaster.ReferenceOrderNo = groupShiftPlan.Flow;
                                orderMgr.CreateOrder(orderMaster);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            IList<object[]> erorrList = new List<object[]>();
                            erorrList.Add(new object[] { null, ex });
                            SendErrorEmail(erorrList);
                        }
                    }
                }
                foreach (var flowStrategy in flowStrategyList)
                {
                    if (flowStrategy.CurrentNextWindowTime > DateTime.MinValue)
                    {
                        flowStrategy.NextWindowTime = flowStrategy.CurrentNextWindowTime;
                        flowStrategy.NextOrderTime = flowStrategy.CurrentNextOrderTime;
                        this.genericMgr.Update(flowStrategy);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                log.Error(ex);
                IList<object[]> erorrList = new List<object[]>();
                erorrList.Add(new object[] { null, ex });
                SendErrorEmail(erorrList);
            }
        }

        private string GetOrderDetailRemark(IEnumerable<string> sections)
        {
            string remark = string.Empty;
            foreach (var section in sections)
            {
                var itemDesc = itemMgr.GetCacheItem(section).Description;
                if (remark == string.Empty)
                {
                    remark = itemDesc;
                }
                else
                {
                    remark += "," + itemDesc;
                }
            }
            return remark;
        }

        private decimal GetRoundOrderQty(FlowDetail flowDetail, decimal orderQty)
        {
            if (orderQty < 0)
            {
                return 0;
            }
            if (flowDetail.MinLotSize > 0 && orderQty < flowDetail.MinLotSize)
            {
                orderQty = flowDetail.MinLotSize;
            }
            if (flowDetail.UnitCount > 0)
            {
                if (flowDetail.RoundUpOption == CodeMaster.RoundUpOption.ToUp)
                {
                    orderQty = Math.Ceiling(orderQty / flowDetail.UnitCount) * flowDetail.UnitCount;
                }
                else if (flowDetail.RoundUpOption == CodeMaster.RoundUpOption.ToDown)
                {
                    orderQty = Math.Floor(orderQty / flowDetail.UnitCount) * flowDetail.UnitCount;
                }
            }
            return orderQty;
        }

        public void ProcessNewOrder(LeanEngine.Entity.Orders order, FlowMaster flowMaster, com.Sconit.Entity.SCM.FlowStrategy flowStrategy)
        {
            log.Debug("-------------------------------Current flow code:" + order.Flow.Code + "-------------------------------------");
            #region 更新窗口时间
            if (order.Flow.IsUpdateWindowTime)
            {
                if (flowStrategy.NextOrderTime != order.Flow.NextOrderTime || flowStrategy.NextWindowTime != order.Flow.NextWindowTime)
                {
                    flowStrategy.NextOrderTime = order.Flow.NextOrderTime;
                    flowStrategy.NextWindowTime = order.Flow.NextWindowTime;
                    this.genericMgr.Update(flowStrategy);
                    this.genericMgr.FlushSession();

                    log.Debug("Update Window time to :" + flowStrategy.NextWindowTime);
                }
            }
            #endregion

            if (order.ItemFlows == null || order.ItemFlows.Count == 0)
                return;

            #region 更新配额
            foreach (ItemFlow itemFlow in order.ItemFlows)
            {
                FlowDetail flowDetail = flowMaster.FlowDetails.Where(det => det.Id == itemFlow.FlowDetailId).Single();

                //只有权重大于零，并且不是引用路线才更新累计数量
                if (flowDetail.MrpWeight > 0 && flowDetail.Flow == flowDetail.CurrentFlowMaster.Code)
                {
                    log.Debug("Update flowdetail Mrp total, flowDetail Id[" + flowDetail.Id + "], Mrp total[" + flowDetail.MrpTotal + "], Add Qty[" + itemFlow.OrderQty + "]");
                    flowDetail.MrpTotal += itemFlow.OrderQty;
                    //this.genericMgr.Update(flowDetail);
                }
            }
            #endregion

            #region 订单头
            flowMaster.FlowDetails = null;
            OrderMaster orderMaster = this.orderMgr.TransferFlow2Order(flowMaster, order.ItemFlows.Select(i => i.Item.ItemCode).Distinct().ToList());
            orderMaster.IsAutoRelease = true;
            //申雅客户化,看板2不自动释放
            if (flowStrategy.Strategy == CodeMaster.FlowStrategy.KB2)
            {
                orderMaster.IsAutoRelease = false;
            }
            orderMaster.WindowTime = order.WindowTime;
            orderMaster.StartTime = order.StartTime;
            orderMaster.Priority = order.IsEmergency ? CodeMaster.OrderPriority.Urgent : CodeMaster.OrderPriority.Normal;
            #endregion

            #region 订单明细
            IList<OrderDetail> addOrderDetailList = new List<OrderDetail>();
            foreach (ItemFlow itemFlow in order.ItemFlows)
            {
                #region 查找订单明细
                //根据来源目的库位+零件号查找
                //来源目的库位明细上没有取头上的
                OrderDetail orderDetail = orderMaster.OrderDetails.Where(det => det.Item == itemFlow.Item.ItemCode
                    && (!string.IsNullOrWhiteSpace(det.LocationFrom) ? det.LocationFrom == itemFlow.LocFrom : orderMaster.LocationFrom == itemFlow.LocFrom)
                    && (!string.IsNullOrWhiteSpace(det.LocationTo) ? det.LocationTo == itemFlow.LocTo : orderMaster.LocationTo == itemFlow.LocTo)).First();
                #endregion

                OrderDetail addOrderDetail = Mapper.Map<OrderDetail, OrderDetail>(orderDetail);

                addOrderDetail.RequiredQty = itemFlow.ReqQty;
                addOrderDetail.OrderedQty = itemFlow.OrderQty;
                addOrderDetail.ManufactureParty = itemFlow.Item.FreeValue1;
                if (itemFlow.Flow.FlowStrategy.Strategy == Enumerators.Strategy.JIT2)
                {
                    //添加线边工位
                    addOrderDetail.BinTo = itemFlow.OrderTracers.Select(ot => ot.Location).Distinct().Single();
                }

                IList<com.Sconit.Entity.ORD.OrderTracer> orderTracerList = new List<com.Sconit.Entity.ORD.OrderTracer>();
                foreach (var leanOrderTracer in itemFlow.OrderTracers)
                {
                    com.Sconit.Entity.ORD.OrderTracer orderTracer = new com.Sconit.Entity.ORD.OrderTracer();
                    orderTracer.Code = leanOrderTracer.Code;
                    orderTracer.ReqTime = leanOrderTracer.ReqTime;
                    orderTracer.Item = leanOrderTracer.Item.ItemCode;
                    orderTracer.OrderedQty = leanOrderTracer.OrderedQty;
                    orderTracer.FinishedQty = leanOrderTracer.FinishedQty;
                    orderTracer.Qty = leanOrderTracer.Qty;
                    orderTracer.RefId = leanOrderTracer.RefOrderLocTransId;

                    orderTracerList.Add(orderTracer);
                }
                addOrderDetail.OrderTracerList = orderTracerList;

                addOrderDetailList.Add(addOrderDetail);
            }
            orderMaster.OrderDetails = addOrderDetailList;
            #endregion

            #region 创建订单
            if (orderMaster.OrderDetails.Count > 0)
            {
                log.Debug("Detail count:" + orderMaster.OrderDetails.Count);

                this.orderMgr.CreateOrder(orderMaster);
                log.Info("Create order:" + orderMaster.OrderNo + " finished. Detail count:" + orderMaster.OrderDetails.Count + ",IsEmergency:" + order.IsEmergency);

                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    foreach (com.Sconit.Entity.ORD.OrderTracer orderTracer in orderDetail.OrderTracerList)
                    {
                        orderTracer.OrderDetailId = orderDetail.Id;
                        this.genericMgr.Save(orderTracer);
                    }
                }
            }
            #endregion

            this.genericMgr.FlushSession();
        }

        private void SendErrorEmail(IList<object[]> erorrList)
        {
            try
            {
                if (erorrList != null && erorrList.Count > 0)
                {
                    var logToUser = genericMgr.FindById<LogToUser>
                        ((int)NVelocityTemplateRepository.TemplateEnum.LeanEngine_CreateOrder);
                    var emailReceiveUsers = logToUser.Emails;
                    if (!string.IsNullOrWhiteSpace(emailReceiveUsers))
                    {
                        IDictionary<string, object> data = new Dictionary<string, object>();
                        List<ErrorMessage> errorMassageList = new List<ErrorMessage>();
                        foreach (var error in erorrList)
                        {
                            var errorMessage = new ErrorMessage();
                            if (error[0] != null)
                            {
                                errorMessage.ItemFlows = new List<string>();
                                var orders = (Orders)error[0];
                                errorMessage.Flow = GetTLog(orders, "Flow");
                                if (orders.ItemFlows != null)
                                {
                                    foreach (var itemFlow in orders.ItemFlows)
                                    {
                                        errorMessage.ItemFlows.Add(GetTLog(itemFlow, "ItemFlow"));
                                    }
                                }
                            }
                            if (error[1] != null)
                            {
                                Exception ex = (Exception)error[1];
                                if (ex is Entity.Exception.BusinessException)
                                {
                                    errorMessage.Message = string.Empty;
                                    foreach (var ms in ((Entity.Exception.BusinessException)(ex)).GetMessages())
                                    {
                                        errorMessage.Message += ms.GetMessageString() + @"
";
                                    }
                                }
                                else
                                {
                                    errorMessage.Message = ex.Message;
                                    errorMessage.StackTrace = ExceptionHelper.GetExceptionMessage(ex);
                                }
                            }
                            errorMassageList.Add(errorMessage);
                        }
                        data.Add("Title", "精益引擎创建订单失败");
                        data.Add("ErrorMessages", errorMassageList);
                        string content = vmReporsitory.RenderTemplate(logToUser.Template, data);
                        emailMgr.AsyncSendEmail("精益引擎创建订单失败", content, emailReceiveUsers, MailPriority.High);
                    }
                }
            }
            catch (Exception exception)
            {
                log.Fatal(exception);
            }
        }

        private List<Orders> CalSpecifiedManufactureOrders(IList<object[]> orderPlanList, IList<FlowDetail> flowDetailList, IList<LocationDetailView> locationDetailViewList, IList<WorkingCalendarView> workingCalendarViewList, IList<string> locationCodeList)
        {
            #region 计算指定供应商的需求
            log.Debug("Start calculate manufacture party specified orders.");
            IEngine engine = new Engine();
            EngineContainer container = new EngineContainer();

            #region 加载订单
            container.Plans = (from plan in orderPlanList
                               select new Plans
                               {
                                   Loc = (string)plan[0],
                                   Item = new Item
                                   {
                                       ItemCode = (string)plan[1],
                                       //ItemDescription = det[0],
                                       Uom = (string)plan[2],
                                       //UC = det.UnitCount.ToString(),
                                       FreeValue1 = (string)plan[3],  //指定供应商
                                       //FreeValue2 = det[0],
                                       //FreeValue3 = det[0],
                                       //FreeValue4 = det[0],
                                       //FreeValue5 = det[0],
                                   },
                                   ReqTime = (DateTime)plan[4],
                                   OrderNo = (string)plan[5],
                                   FlowCode = (string)plan[6],
                                   IRType = (Enumerators.IRType)Enum.Parse(typeof(Enumerators.IRType), (string)plan[7]),
                                   PlanType = (Enumerators.PlanType)Enum.Parse(typeof(Enumerators.PlanType), (string)plan[8]),
                                   FlowType = (Enumerators.FlowType)Enum.Parse(typeof(Enumerators.FlowType), (string)plan[9]),
                                   //TimeUnit = (string)plan[0],
                                   OrderedQty = (decimal)plan[10],
                                   FinishedQty = (decimal)plan[11],
                                   //AllocatedQty = (string)plan[0],
                               }).ToList();

            log.Debug("OrderLocTrans count:" + container.Plans.Count);
            #endregion

            #region 汇总零件及供应商，得出需要指定供应商的零件和供应商列表
            var itemManufacturePartyList = from plan in container.Plans
                                           group plan by new { Item = plan.Item.ItemCode, ManufactureParty = plan.Item.FreeValue1 } into g
                                           select new
                                           {
                                               Item = g.Key.Item,
                                               ManufactureParty = g.Key.ManufactureParty
                                           };
            #endregion

            #region 加载可用路线
            #region 和路线明细结合，为每个相关的路线明细加上指定供应商
            int id = 1;
            container.ItemFlows = (from det in flowDetailList
                                   join item in itemManufacturePartyList
                                   on det.Item equals item.Item
                                   select new ItemFlow
                                   {
                                       ID = id++,
                                       FlowDetailId = det.Id,
                                       Flow = new Flow
                                       {
                                           Code = det.CurrentFlowMaster.Code,
                                           PartyFrom = det.CurrentFlowMaster.PartyFrom,
                                           PartyTo = det.CurrentFlowMaster.PartyTo,
                                           FlowStrategy = new LeanEngine.Entity.FlowStrategy
                                           {
                                               Code = det.CurrentFlowMaster.Code,
                                               Strategy = Enumerators.Strategy.JIT,   //指定供应商全部按JIT来拉动
                                               LeadTime = Utility.DateTimeHelper.TimeTranfer(det.CurrentFlowStrategy.LeadTime, det.CurrentFlowStrategy.TimeUnit, CodeMaster.TimeUnit.Hour),
                                               EmLeadTime = Utility.DateTimeHelper.TimeTranfer(det.CurrentFlowStrategy.EmergencyLeadTime, det.CurrentFlowStrategy.TimeUnit, CodeMaster.TimeUnit.Hour),
                                               WindowTimeType = Enumerators.WindowTimeType.Fix,
                                               WinTimeDiff = Utility.DateTimeHelper.TimeTranfer(det.CurrentFlowStrategy.WinTimeDiff, det.CurrentFlowStrategy.TimeUnit, CodeMaster.TimeUnit.Hour),
                                               //WeekInterval = det.CurrentFlowStrategy.WeekInterval,
                                               //MonWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime1),
                                               //TueWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime2),
                                               //WedWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime3),
                                               //ThuWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime4),
                                               //FriWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime5),
                                               //SatWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime6),
                                               //SunWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime7),
                                           },
                                           FlowType = FlowTypeMapping(det.CurrentFlowMaster.Type),
                                           //TimeUnit = det[0],
                                           OrderTime = det.CurrentFlowStrategy.NextOrderTime,
                                           WindowTime = det.CurrentFlowStrategy.NextWindowTime,
                                       },
                                       Item = new Item
                                       {
                                           ItemCode = det.Item,
                                           //ItemDescription = det[0],
                                           Uom = det.BaseUom,  //BaseUom
                                           UC = det.UnitCount.ToString(),
                                           FreeValue1 = item.ManufactureParty,
                                           //FreeValue2 = det[0],
                                           //FreeValue3 = det[0],
                                           //FreeValue4 = det[0],
                                           //FreeValue5 = det[0],
                                       },
                                       LocFrom = !string.IsNullOrWhiteSpace(det.LocationFrom) ? det.LocationFrom : det.CurrentFlowMaster.LocationFrom,
                                       LocTo = !string.IsNullOrWhiteSpace(det.LocationTo) ? det.LocationTo : det.CurrentFlowMaster.LocationTo,
                                       MaxInv = 0,    //指定供应商不设置最大库存
                                       SafeInv = 0,   //指定供应商不设置安全库存
                                       UC = det.UnitCount,
                                       MinLotSize = det.MinLotSize,
                                       OrderLotSize = det.OrderLotSize,
                                       RoundUp = RoundUpMapping(det.RoundUpOption),
                                       SupplyingRate = det.MrpWeight,
                                       AcmlOrderedQty = det.MrpTotal + det.MrpTotalAdjust,  //累计值+累计调整值
                                       DemandSources = GetExtraDemandSources(det.ExtraDemandSource, det.CurrentFlowMaster.ExtraDemandSource, locationCodeList)
                                   }).ToList();
            #endregion

            log.Debug("FlowDetail count:" + container.ItemFlows.Count);
            #endregion

            #region 加载库存
            container.InvBalances = (from det in locationDetailViewList
                                     join item in itemManufacturePartyList
                                     on new { Item = det.Item, ManufactureParty = det.ManufactureParty }
                                     equals new { Item = item.Item, ManufactureParty = item.ManufactureParty }
                                     select new InvBalance
                                     {
                                         Item = new Item
                                         {
                                             ItemCode = det.Item,
                                             //ItemDescription = det[0],
                                             //Uom = det.BaseUom,  //BaseUom
                                             //UC = det.UnitCount.ToString(),
                                             FreeValue1 = det.ManufactureParty,
                                             //FreeValue2 = det[0],
                                             //FreeValue3 = det[0],
                                             //FreeValue4 = det[0],
                                             //FreeValue5 = det[0],
                                         },
                                         Loc = det.Location,
                                         Qty = det.QualifyQty,
                                         InvType = Enumerators.InvType.Normal
                                     }).Union
                                    (from det in locationDetailViewList
                                     join item in itemManufacturePartyList
                                     on new { Item = det.Item, ManufactureParty = det.ManufactureParty }
                                     equals new { Item = item.Item, ManufactureParty = item.ManufactureParty }
                                     select new InvBalance
                                     {
                                         Item = new Item
                                         {
                                             ItemCode = det.Item,
                                             //ItemDescription = det[0],
                                             //Uom = det.BaseUom,  //BaseUom
                                             //UC = det.UnitCount.ToString(),
                                             FreeValue1 = det.ManufactureParty,
                                             //FreeValue2 = det[0],
                                             //FreeValue3 = det[0],
                                             //FreeValue4 = det[0],
                                             //FreeValue5 = det[0],
                                         },
                                         Loc = det.Location,
                                         //待验+不合格 - 不可用库存 = 待验可用库存
                                         Qty = det.InspectQty + det.RejectQty - (det.Qty - det.ATPQty),
                                         InvType = Enumerators.InvType.Inspect
                                     }).ToList();

            log.Debug("LocationDetail count:" + container.InvBalances.Count);
            #endregion

            #region 加载工作日历
            if (workingCalendarViewList != null && workingCalendarViewList.Count > 0)
            {
                container.RestTimes = ProcessRestTimes(workingCalendarViewList);
            }
            #endregion

            #region 精益引擎计算
            log.Debug("Begin to calculate:");
            engine.TellMeDemands(container);
            log.Debug("Calculating finished.");

            List<LeanEngine.Entity.Orders> orders = engine.DemandToOrders(container.ItemFlows);
            if (orders == null || orders.Count == 0)
            {
                log.Debug("No orders to create.");
            }
            #endregion

            log.Debug("Finish calculate manufacture party specified orders.");
            return orders;
            #endregion
        }

        private List<Orders> CalOrders(IList<object[]> orderPlanList, List<Orders> calculatedSpecifiedManufactureOrders, IList<FlowDetail> flowDetailList, IList<LocationDetailView> locationDetailViewList, IList<WorkingCalendarView> workingCalendarViewList, IList<string> locationCodeList)
        {
            #region 计算总的需求
            log.Debug("Start calculate orders.");
            IEngine engine = new Engine();
            EngineContainer container = new EngineContainer();

            #region 加载订单
            container.Plans = (from plan in orderPlanList
                               select new Plans
                               {
                                   Loc = (string)plan[0],
                                   Item = new Item
                                   {
                                       ItemCode = (string)plan[1],
                                       //ItemDescription = det[0],
                                       Uom = (string)plan[2],
                                       //UC = det.UnitCount.ToString(),
                                       //FreeValue1 = (string)plan[3],  //指定供应商
                                       //FreeValue2 = det[0],
                                       //FreeValue3 = det[0],
                                       //FreeValue4 = det[0],
                                       //FreeValue5 = det[0],
                                   },
                                   ReqTime = (DateTime)plan[4],
                                   OrderNo = (string)plan[5],
                                   FlowCode = (string)plan[6],
                                   IRType = (Enumerators.IRType)Enum.Parse(typeof(Enumerators.IRType), (string)plan[7]),
                                   PlanType = (Enumerators.PlanType)Enum.Parse(typeof(Enumerators.PlanType), (string)plan[8]),
                                   FlowType = (Enumerators.FlowType)Enum.Parse(typeof(Enumerators.FlowType), (string)plan[9]),
                                   //TimeUnit = (string)plan[0],
                                   OrderedQty = (decimal)plan[10],
                                   FinishedQty = (decimal)plan[11],
                                   Status = (int)plan[12],
                                   //AllocatedQty = (string)plan[0],
                               }).ToList();

            #region 加载指定供应商计算出来的订单，把指定供应商的订单转为无供应商的订单
            if (calculatedSpecifiedManufactureOrders != null && calculatedSpecifiedManufactureOrders.Count > 0)
            {
                foreach (Orders order in calculatedSpecifiedManufactureOrders)
                {
                    #region 出库
                    container.Plans.AddRange((from itemFlow in order.ItemFlows
                                              where itemFlow.OrderQty > 0
                                              && !string.IsNullOrWhiteSpace(itemFlow.LocFrom)  //出库库位不为空
                                              && order.Flow.FlowType != Enumerators.FlowType.Production  //如果是生产，出库库位指定的是原材料不是成品，所以要忽略成品的出库
                                              select new Plans
                                              {
                                                  Loc = itemFlow.LocFrom,
                                                  Item = new Item
                                                  {
                                                      ItemCode = itemFlow.Item.ItemCode,
                                                      //ItemDescription = det[0],
                                                      Uom = itemFlow.Item.Uom,
                                                      //UC = det.UnitCount.ToString(),
                                                      //FreeValue1 = (string)plan[3],
                                                      //FreeValue2 = det[0],
                                                      //FreeValue3 = det[0],
                                                      //FreeValue4 = det[0],
                                                      //FreeValue5 = det[0],
                                                  },
                                                  ReqTime = order.StartTime,
                                                  OrderNo = itemFlow.Item.FreeValue1,  //因为还没有生成订单号，所以这里用指定供应商代替
                                                  FlowCode = order.Flow.Code,
                                                  IRType = Enumerators.IRType.ISS,
                                                  PlanType = Enumerators.PlanType.Orders,
                                                  FlowType = order.Flow.FlowType,
                                                  //TimeUnit = (string)plan[0],
                                                  OrderedQty = itemFlow.OrderQty,
                                                  FinishedQty = 0,
                                                  //AllocatedQty = (string)plan[0],
                                              }).ToList());
                    #endregion

                    #region 入库
                    container.Plans.AddRange((from itemFlow in order.ItemFlows
                                              where itemFlow.OrderQty > 0
                                              && !string.IsNullOrWhiteSpace(itemFlow.LocTo)  //入库库位不为空
                                              select new Plans
                                              {
                                                  Loc = itemFlow.LocTo,
                                                  Item = new Item
                                                  {
                                                      ItemCode = itemFlow.Item.ItemCode,
                                                      //ItemDescription = det[0],
                                                      Uom = itemFlow.Item.Uom,
                                                      //UC = det.UnitCount.ToString(),
                                                      //FreeValue1 = (string)plan[3],
                                                      //FreeValue2 = det[0],
                                                      //FreeValue3 = det[0],
                                                      //FreeValue4 = det[0],
                                                      //FreeValue5 = det[0],
                                                  },
                                                  ReqTime = order.WindowTime,
                                                  OrderNo = itemFlow.Item.FreeValue1,  //因为还没有生成订单号，所以这里用指定供应商代替
                                                  FlowCode = order.Flow.Code,
                                                  IRType = Enumerators.IRType.RCT,
                                                  PlanType = Enumerators.PlanType.Orders,
                                                  FlowType = order.Flow.FlowType,
                                                  //TimeUnit = (string)plan[0],
                                                  OrderedQty = itemFlow.OrderQty,
                                                  FinishedQty = 0,
                                                  //AllocatedQty = (string)plan[0],
                                              }).ToList());
                    #endregion
                }
            }
            #endregion

            log.Debug("OrderLocTrans count:" + container.Plans.Count);
            #endregion

            #region 加载可用路线
            #region 和路线明细结合，为每个相关的路线明细加上指定供应商
            int id = 1;
            container.ItemFlows = (from det in flowDetailList
                                   select new ItemFlow
                                   {
                                       ID = id++,
                                       FlowDetailId = det.Id,
                                       Flow = new Flow
                                       {
                                           Code = det.CurrentFlowMaster.Code,
                                           PartyFrom = det.CurrentFlowMaster.PartyFrom,
                                           PartyTo = det.CurrentFlowMaster.PartyTo,
                                           FlowStrategy = new LeanEngine.Entity.FlowStrategy
                                           {
                                               Code = det.CurrentFlowMaster.Code,
                                               Strategy = FlowStrategyMapping(det.CurrentFlowStrategy.Strategy),
                                               LeadTime = Utility.DateTimeHelper.TimeTranfer(det.CurrentFlowStrategy.LeadTime, det.CurrentFlowStrategy.TimeUnit, CodeMaster.TimeUnit.Hour),
                                               EmLeadTime = Utility.DateTimeHelper.TimeTranfer(det.CurrentFlowStrategy.EmergencyLeadTime, det.CurrentFlowStrategy.TimeUnit, CodeMaster.TimeUnit.Hour),
                                               WeekInterval = det.CurrentFlowStrategy.WeekInterval,
                                               WindowTimeType = WindowTimeTypeMapping(det.CurrentFlowStrategy.WindowTimeType),
                                               WinTimeDiff = Utility.DateTimeHelper.TimeTranfer(det.CurrentFlowStrategy.WinTimeDiff, det.CurrentFlowStrategy.TimeUnit, CodeMaster.TimeUnit.Hour),
                                               MonWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime1),
                                               TueWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime2),
                                               WedWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime3),
                                               ThuWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime4),
                                               FriWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime5),
                                               SatWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime6),
                                               SunWinTime = SplitWindowTime(det.CurrentFlowStrategy.WindowTime7),
                                           },
                                           FlowType = FlowTypeMapping(det.CurrentFlowMaster.Type),
                                           //TimeUnit = det[0],
                                           OrderTime = det.CurrentFlowStrategy.NextOrderTime,
                                           WindowTime = det.CurrentFlowStrategy.NextWindowTime,
                                       },
                                       Item = new Item
                                       {
                                           ItemCode = det.Item,
                                           //ItemDescription = det[0],
                                           Uom = det.BaseUom,  //BaseUom
                                           UC = det.UnitCount.ToString(),
                                           //FreeValue1 = item.ManufactureParty,
                                           //FreeValue2 = det[0],
                                           //FreeValue3 = det[0],
                                           //FreeValue4 = det[0],
                                           //FreeValue5 = det[0],
                                       },
                                       LocFrom = !string.IsNullOrWhiteSpace(det.LocationFrom) ? det.LocationFrom : det.CurrentFlowMaster.LocationFrom,
                                       LocTo = !string.IsNullOrWhiteSpace(det.LocationTo) ? det.LocationTo : det.CurrentFlowMaster.LocationTo,
                                       MaxInv = det.MaxStock,
                                       SafeInv = det.SafeStock,
                                       UC = det.UnitCount,
                                       MinLotSize = det.MinLotSize,
                                       OrderLotSize = det.OrderLotSize,
                                       RoundUp = RoundUpMapping(det.RoundUpOption),
                                       SupplyingRate = det.MrpWeight,
                                       AcmlOrderedQty = det.MrpTotal + det.MrpTotalAdjust,  //累计值+累计调整值
                                       DemandSources = GetExtraDemandSources(det.ExtraDemandSource, det.CurrentFlowMaster.ExtraDemandSource, locationCodeList)
                                   }).ToList();
            #endregion

            log.Debug("FlowDetail count:" + container.ItemFlows.Count);
            #endregion

            #region 加载库存
            container.InvBalances = (from det in locationDetailViewList
                                     select new InvBalance
                                     {
                                         Item = new Item
                                         {
                                             ItemCode = det.Item,
                                             //ItemDescription = det[0],
                                             //Uom = det.BaseUom,  //BaseUom
                                             //UC = det.UnitCount.ToString(),
                                             //FreeValue1 = det.ManufactureParty,
                                             //FreeValue2 = det[0],
                                             //FreeValue3 = det[0],
                                             //FreeValue4 = det[0],
                                             //FreeValue5 = det[0],
                                         },
                                         Loc = det.Location,
                                         Qty = det.QualifyQty,
                                         InvType = Enumerators.InvType.Normal
                                     }).Union
                                    (from det in locationDetailViewList
                                     select new InvBalance
                                     {
                                         Item = new Item
                                         {
                                             ItemCode = det.Item,
                                             //ItemDescription = det[0],
                                             //Uom = det.BaseUom,  //BaseUom
                                             //UC = det.UnitCount.ToString(),
                                             //FreeValue1 = det.ManufactureParty,
                                             //FreeValue2 = det[0],
                                             //FreeValue3 = det[0],
                                             //FreeValue4 = det[0],
                                             //FreeValue5 = det[0],
                                         },
                                         Loc = det.Location,
                                         //待验+不合格 - 不可用库存 = 待验可用库存
                                         Qty = det.InspectQty + det.RejectQty - (det.Qty - det.ATPQty),
                                         InvType = Enumerators.InvType.Inspect
                                     }).ToList();

            log.Debug("LocationDetail count:" + container.InvBalances.Count);
            #endregion

            #region 加载工作日历
            if (workingCalendarViewList != null && workingCalendarViewList.Count > 0)
            {
                container.RestTimes = ProcessRestTimes(workingCalendarViewList);
            }
            #endregion

            #region 精益引擎计算
            log.Debug("Begin to calculate:");
            engine.TellMeDemands(container);
            log.Debug("Calculating finished.");

            List<LeanEngine.Entity.Orders> orders = engine.DemandToOrders(container.ItemFlows);
            if (orders == null || orders.Count == 0)
            {
                log.Debug("No orders to create.");
            }
            #endregion

            log.Debug("Finish calculate manufacture party specified orders.");
            return orders;
            #endregion
        }

        private Enumerators.Strategy FlowStrategyMapping(CodeMaster.FlowStrategy flowStrategy)
        {
            switch (flowStrategy)
            {
                case CodeMaster.FlowStrategy.Manual:
                    return Enumerators.Strategy.Manual;
                case CodeMaster.FlowStrategy.KB:
                    return Enumerators.Strategy.KB;
                case CodeMaster.FlowStrategy.KB2:
                    return Enumerators.Strategy.KB2;
                case CodeMaster.FlowStrategy.JIT:
                    return Enumerators.Strategy.JIT;
                case CodeMaster.FlowStrategy.JIT2:
                    return Enumerators.Strategy.JIT2;
                default:
                    return Enumerators.Strategy.Manual;
            }
        }

        private Enumerators.WindowTimeType WindowTimeTypeMapping(CodeMaster.WindowTimeType windowTimeType)
        {
            switch (windowTimeType)
            {
                case CodeMaster.WindowTimeType.CycledWindowTime:
                    return Enumerators.WindowTimeType.Cycle;
                default:
                    return Enumerators.WindowTimeType.Fix;
            }
        }

        private string[] SplitWindowTime(string windowTime)
        {
            if (!string.IsNullOrWhiteSpace(windowTime))
            {
                return windowTime.Split('|');
            }

            return null;
        }

        private Enumerators.FlowType FlowTypeMapping(CodeMaster.OrderType flowType)
        {
            switch (flowType)
            {
                case CodeMaster.OrderType.Procurement:
                case CodeMaster.OrderType.CustomerGoods:
                case CodeMaster.OrderType.ScheduleLine:
                    return Enumerators.FlowType.Procurement;
                case CodeMaster.OrderType.Transfer:
                case CodeMaster.OrderType.SubContractTransfer:
                    return Enumerators.FlowType.Transfer;
                case CodeMaster.OrderType.Distribution:
                    return Enumerators.FlowType.Distribution;
                case CodeMaster.OrderType.Production:
                case CodeMaster.OrderType.SubContract:
                    return Enumerators.FlowType.Production;
                default:
                    throw new TechnicalException("not supported OrderType.");
            }
        }

        private Enumerators.RoundUp RoundUpMapping(CodeMaster.RoundUpOption roundUpOption)
        {
            switch (roundUpOption)
            {
                case CodeMaster.RoundUpOption.ToDown:
                    return Enumerators.RoundUp.Floor;
                case CodeMaster.RoundUpOption.ToUp:
                    return Enumerators.RoundUp.Ceiling;
                case CodeMaster.RoundUpOption.None:
                    return Enumerators.RoundUp.None;
                default:
                    throw new TechnicalException("not supported RoundUpOption.");
            }
        }

        private List<string> GetExtraDemandSources(string detExtraDemandSource, string mstrExtraDemandSource, IList<string> locationCodeList)
        {
            List<string> extraDemandSources = new List<string>();
            if (!string.IsNullOrWhiteSpace(detExtraDemandSource))
            {
                extraDemandSources = detExtraDemandSource.Split('|').ToList();
            }

            if (!string.IsNullOrWhiteSpace(mstrExtraDemandSource))
            {
                extraDemandSources.AddRange(mstrExtraDemandSource.Split('|').ToList());
            }

            List<string> returnExtraDemandSources = new List<string>();
            foreach (string extraDemandSource in extraDemandSources)
            {
                returnExtraDemandSources.AddRange(GetRangeExtraDemandSources(extraDemandSource, locationCodeList));
            }

            return returnExtraDemandSources;
        }

        private IList<string> GetRangeExtraDemandSources(string extraDemandSource, IList<string> locationCodeList)
        {
            if (extraDemandSource.Contains("-") && locationCodeList != null)
            {
                return locationCodeList.Where(code => code.CompareTo(extraDemandSource.Split('-')[0]) >= 0 && code.CompareTo(extraDemandSource.Split('-')[1]) <= 0).ToList();
                //return this.lesGenericMgr.FindAll<string>("select Code from Location where Code Between ? and ?", new object[] { extraDemandSource.Split('-')[0], extraDemandSource.Split('-')[1] });
            }
            else if (extraDemandSource.Contains("～") && locationCodeList != null)
            {
                return locationCodeList.Where(code => code.CompareTo(extraDemandSource.Split('～')[0]) >= 0 && code.CompareTo(extraDemandSource.Split('～')[1]) <= 0).ToList();
                //return this.lesGenericMgr.FindAll<string>("select Code from Location where Code Between ? and ?", new object[] { extraDemandSource.Split('～')[0], extraDemandSource.Split('～')[1] });
            }
            else
            {
                List<string> extraDemandSourceList = new List<string>();
                extraDemandSourceList.Add(extraDemandSource);
                return extraDemandSourceList;
            }
        }

        private List<RestTime> ProcessRestTimes(IList<WorkingCalendarView> workingCalendarViewList)
        {
            List<RestTime> returnRestTimes = new List<RestTime>();

            List<RestTime> restTimes = (from calendar in workingCalendarViewList
                                        where calendar.Type == CodeMaster.WorkingCalendarType.Rest
                                        select new RestTime
                                        {
                                            Party = null,
                                            StartTime = calendar.DateFrom,
                                            EndTime = calendar.DateTo
                                        }).OrderBy(rt => rt.StartTime).ToList();

            if (restTimes.Count() > 0)
            {
                if (restTimes.Count() == 1)
                {
                    returnRestTimes = restTimes;
                }
                else
                {
                    RestTime thisRestTime = null;
                    RestTime nextRestTime = null;

                    for (int i = 0; i < restTimes.Count() - 1; i++)
                    {
                        if (thisRestTime == null)
                        {
                            thisRestTime = restTimes[i];
                            returnRestTimes.Add(thisRestTime);
                        }
                        nextRestTime = restTimes[i + 1];

                        if (thisRestTime.EndTime == nextRestTime.StartTime)
                        {
                            thisRestTime.EndTime = nextRestTime.EndTime;
                        }
                        else
                        {
                            thisRestTime = nextRestTime;
                            returnRestTimes.Add(thisRestTime);
                        }
                    }
                }
            }

            return returnRestTimes;
        }

        private DateTime GetNextWindowTime(DateTime windowTime, string[][] windowTimes, int weekInterval)
        {
            DateTime nextWindowTime = windowTime;
            int nextWTweekday = (int)windowTime.DayOfWeek;

            bool isGet = false;
            for (int i = 0; i <= 7; i++)
            {
                if (isGet) break;

                int weekday = nextWTweekday + i;
                if (weekday >= 7) weekday = weekday - 7;

                string[] wins = windowTimes[weekday];
                if (wins == null || wins.Length == 0)
                    continue;

                foreach (string s in wins)
                {
                    if (s.Equals(string.Empty)) break;

                    string[] ts = s.Split(":".ToCharArray());
                    TimeSpan tspan = new TimeSpan(Int32.Parse(ts[0]), Int32.Parse(ts[1]), 0);

                    DateTime newTime = windowTime.Date.AddDays(i).Add(tspan);

                    if ((windowTime < DateTime.Now && newTime > DateTime.Now) ||
                        (windowTime >= DateTime.Now && newTime > windowTime))
                    {
                        if (weekInterval > 0)
                        {
                            newTime = newTime.AddDays(weekInterval * 7);
                        }

                        nextWindowTime = newTime;
                        isGet = true;
                        break;
                    }
                }
            }
            return nextWindowTime;
        }

        private DateTime GetNextOrderTime(DateTime windowTime, double leadTime)
        {
            DateTime orderTime = windowTime.AddHours(leadTime);
            if (orderTime < DateTime.Now)
            {
                orderTime = DateTime.Now;
            }
            return orderTime;
        }

        private string[][] GetWindowTimes(Entity.SCM.FlowStrategy flowStrategy)
        {
            if (flowStrategy == null)
            {
                return null;
            }

            string[][] windowTimes = new string[7][];

            windowTimes[0] = flowStrategy.WindowTime6.Split('|');
            windowTimes[1] = flowStrategy.WindowTime1.Split('|');
            windowTimes[2] = flowStrategy.WindowTime2.Split('|');
            windowTimes[3] = flowStrategy.WindowTime3.Split('|');
            windowTimes[4] = flowStrategy.WindowTime4.Split('|');
            windowTimes[5] = flowStrategy.WindowTime5.Split('|');
            windowTimes[6] = flowStrategy.WindowTime6.Split('|');

            for (int i = 0; i < 7; i++)
            {
                if (windowTimes[i] != null && windowTimes[i].Length > 0)
                {
                    return windowTimes;
                }
            }
            return null;
        }

        private string GetTLog<T>(T item, string message)
        {
            string logMessage = string.Empty;
            if (!string.IsNullOrWhiteSpace(message))
            {
                logMessage = message + @"
";
            }
            if (item != null)
            {
                PropertyInfo[] scheduleBodyPropertyInfo = typeof(T).GetProperties();
                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                {
                    var objValue = pi.GetValue(item, null);
                    if (objValue != null && !string.IsNullOrWhiteSpace(objValue.ToString()))
                    {
                        logMessage += pi.Name + ":";
                        logMessage += objValue + @"
";
                    }
                }
                int length = logMessage.Length > 4000 ? 4000 : logMessage.Length;
                logMessage.Substring(0, length);
                return logMessage.Substring(0, length);
            }
            return logMessage;
        }
    }

    class ErrorMessage
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Flow { get; set; }
        public List<string> ItemFlows { get; set; }
    }
}
