using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.VIEW;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Utility;
using com.Sconit.Entity.SYS;
using com.Sconit.Entity.INV;
using com.Sconit.Entity;
using AutoMapper;
using com.Sconit.Entity.PRD;

namespace com.Sconit.Service.MRP.Impl
{
    [Transactional]
    public class MrpOrderMgrImpl : BaseMgr, IMrpOrderMgr
    {
        public IOrderMgr orderMgr { get; set; }
        public IFlowMgr flowMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public ILocationDetailMgr locationDetialMgr { get; set; }
        public IWorkingCalendarMgr workingCalendarMgr { get; set; }
        public IReceiptMgr receiptMgr { get; set; }
        public IHuMgr huMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public OrderMaster CreateExScrapOrder(MrpExScrap mrpExScrap)
        {
            var newOrder = new OrderMaster();
            Item item = this.itemMgr.GetCacheItem(mrpExScrap.Item);
            mrpExScrap.ItemDescription = item.Description;
            this.genericMgr.Create(mrpExScrap);
            if(mrpExScrap.Item == BusinessConstants.VIRTUALSECTION)
            {
                //nothing todo
            }
            else if(mrpExScrap.ScrapType == CodeMaster.ScheduleType.MES24 || mrpExScrap.ScrapType == CodeMaster.ScheduleType.MES25)
            {
                //只记录废品数,无材料消耗
                DateTime startTime = mrpExScrap.EffectDate;
                DateTime windowTime = mrpExScrap.EffectDate;
                workingCalendarMgr.GetStartTimeAndWindowTime(mrpExScrap.Shift, mrpExScrap.EffectDate, out startTime, out windowTime);

                FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(mrpExScrap.Flow);
                newOrder = orderMgr.TransferFlow2Order(flowMaster, false);
                newOrder.Shift = mrpExScrap.Shift;
                newOrder.StartTime = startTime;
                newOrder.WindowTime = windowTime;
                newOrder.EffectiveDate = mrpExScrap.EffectDate;
                newOrder.ReferenceOrderNo = mrpExScrap.Id.ToString();
                newOrder.Priority = CodeMaster.OrderPriority.Normal;

                OrderDetail newOrderDetail = new OrderDetail();
                newOrderDetail.Item = item.Code;
                newOrderDetail.UnitCount = (decimal)item.UnitCount;
                newOrderDetail.Uom = "KG";
                newOrderDetail.BaseUom = item.Uom;
                newOrderDetail.ItemDescription = item.Description;
                newOrderDetail.Sequence = 10;
                newOrderDetail.MinUnitCount = item.UnitCount;
                newOrderDetail.OrderedQty = (decimal)mrpExScrap.ScrapQty;
                newOrderDetail.LocationFrom = flowMaster.LocationFrom;
                newOrderDetail.LocationTo = flowMaster.LocationTo;
                newOrderDetail.CurrentScrapQty = newOrderDetail.OrderedQty;
                newOrderDetail.ScheduleType = mrpExScrap.ScrapType;

                newOrder.ExternalOrderNo = newOrderDetail.ScheduleType.ToString();
                newOrder.AddOrderDetail(newOrderDetail);
                newOrder.SubType = CodeMaster.OrderSubType.Other;
                newOrder.IsQuick = true;
                newOrder.IsShipScanHu = false;
                newOrder.IsReceiveScanHu = false;
                newOrder.CreateHuOption = CodeMaster.CreateHuOption.None;
                orderMgr.CreateOrder(newOrder, true);
                mrpExScrap.OrderNo = newOrder.OrderNo;
            }
            return newOrder;
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelExScrapOrder(MrpExScrap mrpExScrap)
        {
            if(!string.IsNullOrWhiteSpace(mrpExScrap.OrderNo))
            {
                var receiptDetails = this.genericMgr.FindAll<ReceiptDetail>(" from ReceiptDetail where OrderNo =?", mrpExScrap.OrderNo);
                receiptMgr.CancelReceipt(receiptDetails.First().ReceiptNo);
                orderMgr.ManualCloseOrder(mrpExScrap.OrderNo);
            }
            mrpExScrap.IsVoid = true;
            this.genericMgr.Update(mrpExScrap);
        }

        [Transaction(TransactionMode.Requires)]
        public OrderMaster CreateFiOrder(string flow, DateTime planVersion, DateTime planDate, string shift)
        {
            string sql = @"select OrderNo from ORD_OrderMstr_4 as a where a.Type = ? and a.Flow = ? and a.Shift = ?
                            and a.EffDate=? and a.Status in ( ?,?,? ) ";
            IList<object> orderNos = this.genericMgr.FindAllWithNativeSql<object>(sql,
                new Object[] { CodeMaster.OrderType.Production, flow, shift, planDate.Date,
                    CodeMaster.OrderStatus.Create, CodeMaster.OrderStatus.Submit, CodeMaster.OrderStatus.InProcess });
            if(orderNos != null && orderNos.Count() > 0)
            {
                throw new BusinessException(string.Format("此路线{0}班次{1}已经有生产单{2}。请先取消对应的生产单。", flow, shift, orderNos[0]));
            }

            string hql = "from MrpFiShiftPlan where PlanVersion =? and ProductLine = ? and PlanDate =? and Shift = ? ";
            var planList = genericMgr.FindAll<MrpFiShiftPlan>(hql, new object[] { planVersion, flow, planDate, shift });
            if(planList != null && planList.Count > 0)
            {
                DateTime startTime = DateTime.Now;
                DateTime windowTime = DateTime.Now;
                workingCalendarMgr.GetStartTimeAndWindowTime(shift, planDate, out startTime, out windowTime);

                FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(flow);
                OrderMaster newOrder = orderMgr.TransferFlow2Order(flowMaster, false);
                newOrder.IsQuick = false;
                newOrder.Shift = shift;
                newOrder.StartTime = startTime;
                newOrder.WindowTime = windowTime;
                newOrder.EffectiveDate = planDate.Date;
                newOrder.ReferenceOrderNo = planVersion.ToString("yyyy-MM-ddTHH:mm:ss");
                newOrder.Priority = CodeMaster.OrderPriority.Normal;
                newOrder.SubType = CodeMaster.OrderSubType.Normal;

                var locationCodes = planList.Select(p => p.LocationTo).Distinct();
                var locationTos = this.genericMgr.FindAllIn<Location>(" from Location where Code in(?", locationCodes);
                var locationFrom = this.genericMgr.FindById<Location>(flowMaster.LocationFrom);
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                foreach(var plan in planList.OrderBy(p => p.Sequence))
                {
                    OrderDetail newOrderDetail = new OrderDetail();
                    Item item = itemMgr.GetCacheItem(plan.Item);
                    newOrderDetail.Item = item.Code;
                    newOrderDetail.UnitCount = (decimal)plan.UnitCount;
                    newOrderDetail.Uom = item.Uom;
                    newOrderDetail.ItemDescription = item.Description;
                    newOrderDetail.Sequence = plan.Sequence;
                    newOrderDetail.MinUnitCount = item.UnitCount;
                    newOrderDetail.OrderedQty = (decimal)plan.Qty;
                    newOrderDetail.LocationFrom = flowMaster.LocationFrom;
                    newOrderDetail.LocationFromName = locationFrom.Name;
                    newOrderDetail.LocationTo = plan.LocationTo;
                    newOrderDetail.LocationToName = locationTos.FirstOrDefault(p => p.Code == plan.LocationTo).Name;
                    newOrderDetail.ScheduleType = CodeMaster.ScheduleType.SY01;
                    plan.OrderDetail = newOrderDetail;
                    orderDetailList.Add(newOrderDetail);
                }
                newOrder.OrderDetails = orderDetailList;
                orderMgr.CreateOrder(newOrder);

                foreach(var plan in planList)
                {
                    plan.OrderDetailId = plan.OrderDetail.Id;
                    this.genericMgr.Update(plan);
                }

                return newOrder;
            }
            else
            {
                throw new BusinessException(string.Format("没有此路线{0}班次{1}的计划明细，创建生产单失败。", flow, shift));
            }
        }

        public IList<string> GetActiveOrder(string flow, DateTime effDate, string shift)
        {
            string sql = @"select OrderNo from ORD_OrderMstr_4 as a where a.Type = ? and a.Flow = ? and a.Shift = ?
                           and a.EffDate=? and a.Status in ( ?,?,? ) ";
            IList<object> orderNos = this.genericMgr.FindAllWithNativeSql<object>(sql,
                new Object[] { CodeMaster.OrderType.Production, flow, shift, effDate.Date,
                                CodeMaster.OrderStatus.Create, 
                                CodeMaster.OrderStatus.Submit, 
                                CodeMaster.OrderStatus.InProcess });
            return orderNos.Select(p => p.ToString()).ToList();
        }

        [Transaction(TransactionMode.Requires)]
        public OrderMaster CreateMiOrder(IList<MrpMiShiftPlan> mrpMiShiftPlanList)
        {
            OrderMaster newOrder = new OrderMaster();
            if(mrpMiShiftPlanList != null && mrpMiShiftPlanList.Count > 0)
            {
                var miShiftPlan = mrpMiShiftPlanList.First();
                DateTime startTime = miShiftPlan.StartTime;
                DateTime windowTime = miShiftPlan.WindowTime;

                FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(miShiftPlan.ProductLine);
                var flowDetails = this.flowMgr.GetFlowDetailList(flowMaster);
                newOrder = orderMgr.TransferFlow2Order(flowMaster, false);
                newOrder.IsQuick = false;
                newOrder.Shift = miShiftPlan.Shift;
                newOrder.StartTime = startTime;
                newOrder.WindowTime = windowTime;
                //newOrder.EffectiveDate = planDate;
                newOrder.ReferenceOrderNo = miShiftPlan.PlanVersion.ToString("yyyy-MM-ddTHH:mm:ss");
                newOrder.Priority = CodeMaster.OrderPriority.Normal;
                newOrder.EffectiveDate = miShiftPlan.PlanDate.Date;
                newOrder.SubType = CodeMaster.OrderSubType.Normal;

                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                foreach(var mrpMiShiftPlan in mrpMiShiftPlanList)
                {
                    var flowDetail = flowDetails.FirstOrDefault(p => p.DefaultLocationFrom == mrpMiShiftPlan.LocationFrom
                        && p.DefaultLocationTo == mrpMiShiftPlan.LocationTo && p.Item == mrpMiShiftPlan.Item);
                    if(flowDetail == null)
                    {
                        throw new BusinessException("此物料{0}不在生产线{1}上", mrpMiShiftPlan.Item, flowMaster.Code);
                    }

                    OrderDetail newOrderDetail = new OrderDetail();
                    Item item = itemMgr.GetCacheItem(mrpMiShiftPlan.Item);
                    newOrderDetail.Item = item.Code;
                    newOrderDetail.UnitCount = flowDetail.UnitCount;
                    newOrderDetail.Uom = flowDetail.Uom;
                    newOrderDetail.BaseUom = item.Uom;
                    newOrderDetail.ItemDescription = item.Description;
                    newOrderDetail.Sequence = mrpMiShiftPlan.Sequence;
                    newOrderDetail.MinUnitCount = flowDetail.MinUnitCount;
                    newOrderDetail.OrderedQty = (decimal)mrpMiShiftPlan.TotalQty;
                    newOrderDetail.LocationFrom = mrpMiShiftPlan.LocationFrom;
                    //newOrderDetail.LocationFromName = locations.Single(p => p.Code == newOrderDetail.LocationFrom).Name;
                    newOrderDetail.LocationTo = mrpMiShiftPlan.LocationTo;
                    //newOrderDetail.LocationToName = locations.Single(p => p.Code == newOrderDetail.LocationTo).Name;
                    newOrderDetail.Bom = mrpMiShiftPlan.Bom;
                    newOrderDetail.Direction = mrpMiShiftPlan.HuTo;
                    newOrderDetail.ScheduleType = CodeMaster.ScheduleType.SY01;
                    orderDetailList.Add(newOrderDetail);
                }
                newOrder.OrderDetails = orderDetailList;
                orderMgr.CreateOrder(newOrder);
            }
            return newOrder;
        }

        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster ReceiveExOrder(MrpExShiftPlan mrpExShiftPlan)
        {
            OrderMaster orderMaster = new OrderMaster();
            if(!string.IsNullOrWhiteSpace(mrpExShiftPlan.OrderNo))
            {
                try
                {
                    orderMaster = orderMgr.LoadOrderMaster(mrpExShiftPlan.OrderNo, true, false, true);
                    if(orderMaster.Status != CodeMaster.OrderStatus.InProcess)
                    {
                        orderMaster = CreateExOrder(mrpExShiftPlan);
                    }
                }
                catch(Exception)
                {
                    orderMaster = CreateExOrder(mrpExShiftPlan);
                }
            }
            else
            {
                orderMaster = CreateExOrder(mrpExShiftPlan);
            }
            var receiptMaster = new ReceiptMaster();
            if(mrpExShiftPlan.CurrentQty <= 0)//只是打印废品条码
            {
                //打印废品条码
                var item = new Item();
                if(!string.IsNullOrWhiteSpace(mrpExShiftPlan.Section) && mrpExShiftPlan.Section != "299999")
                {
                    item = itemMgr.GetCacheItem(mrpExShiftPlan.Section);
                }
                else
                {
                    this.itemMgr.GetCacheItem(mrpExShiftPlan.Item);
                }
                Hu hu = new Hu();
                hu.Qty = 0;
                hu.Item = item.Code;
                hu.ItemDescription = item.Description;
                hu.BaseUom = item.Uom;
                hu.Uom = item.Uom;
                hu.UnitCount = item.UnitCount;
                hu.UnitQty = 1;
                hu.Qty = 0;
                hu.HuTemplate = "BarCodeEXScrap.xls";
                hu.ManufactureDate = mrpExShiftPlan.PlanDate;
                hu.LotNo = Utility.LotNoHelper.GenerateLotNo(hu.ManufactureDate);
                hu.PrintCount = 0;
                hu.ConcessionCount = 0;
                hu.ReferenceItemCode = item.ReferenceCode;
                hu.LocationTo = mrpExShiftPlan.LocationTo;
                hu.OrderNo = orderMaster.OrderNo;
                hu.Shift = mrpExShiftPlan.Shift;
                hu.Flow = mrpExShiftPlan.ProductLine;
                hu.ManufactureParty = orderMaster.PartyFrom;
                //hu.HuId = Guid.NewGuid().ToString();
                //hu.IsOdd = hu.Qty < hu.UnitCount;
                //hu.IsChangeUnitCount = false;
                //hu.UnitCountDescription = null;
                //hu.SupplierLotNo = null;
                //hu.ContainerDesc = null;
                receiptMaster.HuList = new List<Hu>();
                receiptMaster.HuList.Add(hu);
                receiptMaster.CreateHuOption = orderMaster.CreateHuOption;
                receiptMaster.HuTemplate = orderMaster.HuTemplate;
            }
            else
            {
                OrderDetailInput orderDetailInput = new OrderDetailInput();
                orderDetailInput.ReceiveQty = (decimal)mrpExShiftPlan.CurrentQty;

                orderMaster.OrderDetails[0].AddOrderDetailInput(orderDetailInput);
                orderMaster.OrderDetails[0].IsInspect = mrpExShiftPlan.IsFreeze;
                orderMaster.IsInspect = true;

                receiptMaster = orderMgr.ReceiveOrder(orderMaster.OrderDetails);
                mrpExShiftPlan.ReceivedQty += mrpExShiftPlan.CurrentQty;
                this.genericMgr.Update(mrpExShiftPlan);

                receiptMaster.HuList = receiptMaster.ReceiptDetails.SelectMany(p => p.ReceiptLocationDetails.Select(q => q.HuId))
                    .Select(p => { return this.genericMgr.FindById<Hu>(p); }).ToList();
                foreach(var hu in receiptMaster.HuList)
                {
                    hu.ItemVersion = mrpExShiftPlan.ProductType;
                    this.genericMgr.Update(hu);
                }
            }
            return receiptMaster;
        }

        private OrderMaster CreateExOrder(MrpExShiftPlan mrpExShiftPlan)
        {
            var flowMaster = genericMgr.FindById<FlowMaster>(mrpExShiftPlan.ProductLine);
            var orderMaster = orderMgr.TransferFlow2Order(flowMaster, false);

            OrderDetail orderDetail = new OrderDetail();
            var item = this.itemMgr.GetCacheItem(mrpExShiftPlan.Item);
            orderDetail.Item = item.Code;
            orderDetail.UnitCount = (decimal)mrpExShiftPlan.UnitCount;
            orderDetail.Uom = item.Uom;
            orderDetail.ItemDescription = item.Description;
            orderDetail.Sequence = 10;
            orderDetail.MinUnitCount = item.UnitCount;
            orderDetail.OrderedQty = (decimal)mrpExShiftPlan.Qty;
            orderDetail.IsInspect = mrpExShiftPlan.IsFreeze;

            orderDetail.Remark = mrpExShiftPlan.Remark;
            if(item.ItemOption == com.Sconit.CodeMaster.ItemOption.NeedAging)
            {
                orderDetail.OldOption = Sconit.CodeMaster.HuOption.UnAging;
            }
            else
            {
                orderDetail.OldOption = Sconit.CodeMaster.HuOption.NoNeed;
            }

            var productType = this.genericMgr.FindById<ProductType>(mrpExShiftPlan.ProductType);
            orderDetail.ScheduleType = productType.SubType;
            orderMaster.AddOrderDetail(orderDetail);

            var flowDetailItems = this.flowMgr.GetFlowDetailList("EXV").Select(p => p.Item).Distinct().ToList();
            if(flowDetailItems.Contains(orderDetail.Item))
            {
                orderMaster.ProductLineFacility = "EXV";
            }

            orderMaster.IsAutoRelease = true;
            orderMaster.IsAutoStart = true;
            orderMaster.IsAutoReceive = false;
            orderMaster.IsOpenOrder = true;
            orderMaster.IsInspect = true;
            orderMaster.Shift = mrpExShiftPlan.Shift;
            orderMaster.StartTime = mrpExShiftPlan.StartTime;
            orderMaster.WindowTime = mrpExShiftPlan.WindowTime;
            //orderMaster.EffectiveDate = mrpExShiftPlan.PlanVersion;
            orderMaster.ReferenceOrderNo = mrpExShiftPlan.Section;
            orderMaster.ExternalOrderNo = mrpExShiftPlan.Id.ToString();
            orderMaster.Priority = CodeMaster.OrderPriority.Normal;
            orderMaster.CreateHuOption = Sconit.CodeMaster.CreateHuOption.Receive;
            orderMaster.EffectiveDate = DateTime.Now.AddHours(-7.75).Date;
            orderMaster.SubType = CodeMaster.OrderSubType.Normal;
            orderMgr.CreateOrder(orderMaster);

            mrpExShiftPlan.OrderNo = orderMaster.OrderNo;
            return orderMaster;
        }

        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster ReceiveUrgentExOrder(FlowDetail flowDetail)
        {
            var flowMaster = flowDetail.CurrentFlowMaster;
            if(flowMaster == null)
            {
                flowMaster = genericMgr.FindById<FlowMaster>(flowDetail.Flow);
            }

            var orderMaster = orderMgr.TransferFlow2Order(flowMaster, false);

            OrderDetail orderDetail = new OrderDetail();
            var item = this.itemMgr.GetCacheItem(flowDetail.Item);
            orderDetail.Item = item.Code;
            orderDetail.UnitCount = flowDetail.UnitCount;
            orderDetail.Uom = flowDetail.Uom;
            orderDetail.ItemDescription = item.Description;
            orderDetail.BaseUom = item.Uom;
            orderDetail.Sequence = 10;
            orderDetail.MinUnitCount = item.UnitCount;
            orderDetail.OrderedQty = (decimal)flowDetail.CurrentQty;
            orderDetail.IsInspect = flowDetail.IsFreeze;
            orderDetail.ReferenceItemCode = item.ReferenceCode;
            orderDetail.UnitCountDescription = flowDetail.UnitCountDescription;
            orderDetail.ContainerDescription = flowDetail.ContainerDescription;

            orderDetail.Remark = flowDetail.Remark;
            if(item.ItemOption == com.Sconit.CodeMaster.ItemOption.NeedAging)
            {
                orderDetail.OldOption = Sconit.CodeMaster.HuOption.UnAging;
            }
            else
            {
                orderDetail.OldOption = Sconit.CodeMaster.HuOption.NoNeed;
            }
            var productType = this.genericMgr.FindById<ProductType>(flowDetail.ProductType);
            orderDetail.ScheduleType = productType.SubType;
            orderMaster.AddOrderDetail(orderDetail);

            if(flowMaster.Code == "EXV")
            {
                orderMaster.ProductLineFacility = "EXV";
            }

            orderMaster.IsAutoRelease = true;
            orderMaster.IsAutoStart = true;
            orderMaster.IsAutoReceive = false;
            orderMaster.IsOpenOrder = false;
            orderMaster.Shift = flowDetail.Shift;
            DateTime startTime = DateTime.Now;
            DateTime windowTime = DateTime.Now;
            workingCalendarMgr.GetStartTimeAndWindowTime(orderMaster.Shift, DateTime.Now.AddHours(-7.75).Date, out startTime, out windowTime);
            orderMaster.StartTime = startTime;
            orderMaster.WindowTime = windowTime;
            orderMaster.Priority = CodeMaster.OrderPriority.Urgent;
            orderMaster.CreateHuOption = Sconit.CodeMaster.CreateHuOption.Receive;
            orderMaster.EffectiveDate = startTime.Date;

            var bomDetail = this.bomMgr.GetOnlyNextLevelBomDetail(flowDetail.Item, orderMaster.StartTime)
                    .Where(p => p.Item.StartsWith("29")).FirstOrDefault();
            if(bomDetail != null)
            {
                item = itemMgr.GetCacheItem(bomDetail.Item);
                orderMaster.ReferenceOrderNo = bomDetail.Item;
            }
            orderMaster.SubType = CodeMaster.OrderSubType.Normal;
            orderMgr.CreateOrder(orderMaster);

            var receiptMaster = new ReceiptMaster();
            if(flowDetail.CurrentQty <= 0)
            {
                Hu hu = new Hu();
                hu.Qty = 0;
                hu.Item = item.Code;
                hu.ItemDescription = item.Description;
                hu.BaseUom = item.Uom;
                hu.Uom = item.Uom;
                hu.UnitCount = item.UnitCount;
                hu.UnitQty = 1;
                hu.Qty = 0;
                hu.HuTemplate = "BarCodeEXScrap.xls";
                hu.ManufactureDate = DateTime.Now.AddHours(-7.75).Date;
                hu.LotNo = Utility.LotNoHelper.GenerateLotNo(hu.ManufactureDate);
                hu.PrintCount = 0;
                hu.ConcessionCount = 0;
                hu.ReferenceItemCode = item.ReferenceCode;
                hu.LocationTo = flowDetail.LocationTo;
                if(string.IsNullOrWhiteSpace(hu.LocationTo))
                {
                    hu.LocationTo = flowMaster.LocationTo;
                }
                hu.Shift = flowDetail.Shift;
                hu.Flow = flowDetail.Flow;
                hu.OrderNo = orderMaster.OrderNo;

                receiptMaster.HuList = new List<Hu>();
                receiptMaster.HuList.Add(hu);
                receiptMaster.CreateHuOption = orderMaster.CreateHuOption;
                receiptMaster.HuTemplate = orderMaster.HuTemplate;
                orderMgr.ManualCloseOrder(orderMaster);
            }
            else
            {
                OrderDetailInput orderDetailInput = new OrderDetailInput();
                orderDetailInput.ReceiveQty = (decimal)flowDetail.CurrentQty;
                orderMaster.OrderDetails[0].AddOrderDetailInput(orderDetailInput);
                receiptMaster = orderMgr.ReceiveOrder(orderMaster.OrderDetails);
                receiptMaster.HuList = receiptMaster.ReceiptDetails.SelectMany(p => p.ReceiptLocationDetails.Select(q => q.HuId))
                    .Select(p => { return this.genericMgr.FindById<Hu>(p); }).ToList();
                foreach(var hu in receiptMaster.HuList)
                {
                    hu.ItemVersion = flowDetail.ProductType;
                    this.genericMgr.Update(hu);
                }
            }
            return receiptMaster;
        }
    }
}
