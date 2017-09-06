using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCShip : UCBase
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        private static UCShip usShip;
        private static object obj = new object();
        private List<OrderMaster> orderMasters;
        private DateTime? effDate;
        private bool isOpPallet;


        public UCShip(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.btnOrder.Text = "发货";
            isOpPallet = false;
        }

        public static UCShip GetUCShip(User user)
        {
            if (usShip == null)
            {
                lock (obj)
                {
                    if (usShip == null)
                    {
                        usShip = new UCShip(user);
                    }
                }
            }
            usShip.user = user;
            usShip.Reset();
            return usShip;
        }


        protected override void ScanBarCode()
        {
            base.ScanBarCode();

            if (base.op == CodeMaster.BarCodeType.ORD.ToString())
            {
                if (orderMasters == null)
                {
                    this.Reset();
                    orderMasters = new List<OrderMaster>();
                }
                var orderMaster = base.smartDeviceService.GetOrder(base.barCode, true);
                if (orderMaster.IsPause)
                {
                    throw new BusinessException("订单已暂停");
                }
                //检查订单类型
                if (orderMaster.Type == OrderType.Production)
                {
                    throw new BusinessException("扫描的为生产单，不能发货。");
                }
                //else if (orderMaster.Type == OrderType.SubContract)
                //{
                //    throw new BusinessException("扫描的为委外生产单，不能发货。");
                //}
                else if (orderMaster.Type == OrderType.CustomerGoods)
                {
                    throw new BusinessException("扫描的为客供品单，不能发货。");
                }
                else if (orderMaster.Type == OrderType.Procurement && orderMaster.SubType == OrderSubType.Normal)
                {
                    throw new BusinessException("扫描的为采购单，不能发货。");
                }
                if (!orderMaster.IsShipByOrder)
                {
                    throw new BusinessException("不允许按订单发货。");
                }

                //检查订单状态
                if (orderMaster.Status != OrderStatus.Submit
                    && orderMaster.Status != OrderStatus.InProcess)
                {
                    throw new BusinessException("不是释放或执行中状态不能发货");
                }

                this.MergeOrderMaster(orderMaster);
            }

            else if (op == CodeMaster.BarCodeType.HU.ToString() || op == CodeMaster.BarCodeType.TP.ToString())
            {
                if (base.op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if (this.orderMasters == null || this.orderMasters.Count() == 0)
                    {
                        throw new BusinessException("请先扫描订单");
                    }
                    Hu hu = smartDeviceService.GetHu(base.barCode);

                    if (hu == null)
                    {
                        throw new BusinessException("此条码不存在");
                    }
                    if (!hu.IsPallet)
                    {
                        foreach (OrderMaster om in orderMasters)
                        {
                            if (hu.ManufactureParty != om.PartyTo)
                            {
                                throw new BusinessException("条码上客户与发货单客户不匹配。");
                            }

                        }

                        this.MatchHu(hu);
                    }
                    else
                    {
                        base.op = CodeMaster.BarCodeType.TP.ToString();
                    }
                }
                if (base.op == CodeMaster.BarCodeType.TP.ToString())
                {
                    if ((this.orderMasters == null || this.orderMasters.Count() == 0))
                    {
                        throw new BusinessException("请先扫描订单。");
                    }
                    Hu[] huArray = smartDeviceService.GetHuListByPallet(barCode);

                    foreach (Hu hu in huArray)
                    {
                        this.MatchHu(hu);
                    }
                    isOpPallet = true;
                }
            }
            else if (base.op == CodeMaster.BarCodeType.DATE.ToString())
            {
                base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                this.effDate = base.smartDeviceService.GetEffDate(base.barCode);

                this.lblMessage.Text = "生效时间:" + this.effDate.Value.ToString("yyyy-MM-dd HH:mm");
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
            }
            else
            {
                throw new BusinessException("条码格式不合法");
            }
        }

        protected override void gvListDataBind()
        {
            base.gvListDataBind();
            List<OrderDetail> orderDetailList = new List<OrderDetail>();
            if (this.orderMasters != null)
            {
                foreach (var om in this.orderMasters)
                {
                    orderDetailList.AddRange(om.OrderDetails.Where(o => o.CurrentQty != 0));
                }
            }
            base.dgList.DataSource = orderDetailList;
            base.ts.MappingName = orderDetailList.GetType().Name;
        }

        protected override void Reset()
        {
            this.orderMasters = new List<OrderMaster>();
            base.Reset();
            this.lblMessage.Text = "请扫描订单";
            this.effDate = null;
        }

        protected override void DoSubmit()
        {
            try
            {
                if (this.orderMasters == null || this.orderMasters.Count == 0)
                {
                    throw new BusinessException("请先扫描订单。");
                }

                List<OrderDetail> orderDetailList = new List<OrderDetail>();
                List<OrderDetailInput> orderDetailInputList = new List<OrderDetailInput>();

                foreach (var om in orderMasters)
                {
                    if (om.OrderDetails != null)
                    {
                        orderDetailList.AddRange(om.OrderDetails);
                    }
                }
                if (orderDetailList.Any(od => od.CurrentQty > 0))
                {
                    DialogResult dr = MessageBox.Show("本次发货有未发完的明细,是否继续?", "未全部发货", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.No)
                    {
                        return;
                    }
                }
                foreach (var orderDetail in orderDetailList)
                {
                    if (orderDetail.OrderDetailInputs != null)
                    {
                        orderDetailInputList.AddRange(orderDetail.OrderDetailInputs);
                    }
                }
                //foreach (var om in this.orderMasters)
                //{
                //    if (om.OrderDetails != null)
                //    {
                //        foreach (var od in om.OrderDetails)
                //        {
                //            if (od.OrderDetailInputs != null)
                //            {
                //                orderDetailInputList.AddRange(od.OrderDetailInputs);
                //            }
                //        }
                //    }
                //}
                if (orderDetailInputList.Count == 0)
                {
                    throw new BusinessException("没有扫描条码");
                }
                if (this.orderMasters.Count > 1)
                {
                    if (this.orderMasters[0].OrderStrategy == FlowStrategy.KIT)
                    {
                        throw new BusinessException("KIT单不能合并发货。");
                    }
                    if (this.orderMasters[0].OrderDetails.Count(o => o.CurrentQty != 0) > 0)
                    {
                        throw new BusinessException("必须满足KIT单发货的发货数。");
                    }
                }

                string ipNo = this.smartDeviceService.DoShipOrder(orderDetailInputList.ToArray(), this.effDate, this.user.Code, isOpPallet);
                this.Reset();
                base.lblMessage.Text = string.Format("发货成功,送货单:{0}", ipNo);
                this.isMark = true;
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex.Message);
            }
        }

        private void MatchHu(Hu hu)
        {
            base.CheckHu(hu);
            if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
            {
                throw new BusinessException("没有此条码的权限");
            }
            if (!base.isCancel)
            {
                #region 条码匹配
                if (hu.Status != HuStatus.Location)
                {
                    throw new BusinessException("条码不在库位中");
                }

                var orderDetails = new List<OrderDetail>();
                var orderMaster = this.orderMasters.First();
                string huId = hu.HuId;

                //先按开始日期排序，在按订单序号排序
                foreach (var om in orderMasters.OrderBy(o => o.StartTime))
                {
                    orderDetails.AddRange(om.OrderDetails.Where(o => o.OrderedQty > o.ShippedQty).OrderBy(o => o.Sequence));
                }
                orderDetails = orderDetails.OrderByDescending(p => p.CurrentQty).ToList();

                var matchedOrderDetailList = orderDetails.Where(o => o.Item == hu.Item);
                if (matchedOrderDetailList == null || matchedOrderDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的物料号{1}匹配的订单明细。", huId, hu.Item);
                }

                matchedOrderDetailList = matchedOrderDetailList.Where(o => o.Uom.Equals(hu.Uom, StringComparison.OrdinalIgnoreCase));
                if (matchedOrderDetailList == null || matchedOrderDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的单位{1}匹配的订单明细。", huId, hu.Uom);
                }

                matchedOrderDetailList = matchedOrderDetailList.Where(o => o.LocationFrom.Equals(hu.Location, StringComparison.OrdinalIgnoreCase));
                if (matchedOrderDetailList == null || matchedOrderDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的库位{1}匹配的订单明细。", huId, hu.Location);
                }

                if (orderMaster.IsShipFulfillUC)
                {
                    matchedOrderDetailList = matchedOrderDetailList.Where(o => o.UnitCount == hu.UnitCount);
                    if (matchedOrderDetailList == null || matchedOrderDetailList.Count() == 0)
                    {
                        throw new BusinessException("没有找到和条码{0}的包装数{1}匹配的订单明细。", huId, hu.UnitCount.ToString());
                    }
                }

                matchedOrderDetailList = matchedOrderDetailList.Where(o => o.QualityType == hu.QualityType);
                if (matchedOrderDetailList == null || matchedOrderDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的质量状态匹配的订单明细。", huId);
                }

                //matchedOrderDetailList = matchedOrderDetailList.Where(o =>
                //    (string.IsNullOrEmpty(o.Direction) && string.IsNullOrEmpty(hu.Direction)) ||
                //    (!string.IsNullOrEmpty(o.Direction) && o.Direction == hu.Direction));
                //if (matchedOrderDetailList == null || matchedOrderDetailList.Count() == 0)
                //{
                //    throw new BusinessException("没有找到和条码{0}的方向匹配的订单明细。", huId);
                //}

                #region 先匹配未满足订单发货数的（未超发的）
                //OrderDetail matchedOrderDetail = MatchOrderDetail(hu, matchedOrderDetailList.Where(o => o.CurrentQty >= hu.Qty).ToList());
                OrderDetail matchedOrderDetail = matchedOrderDetailList.Where(o => o.CurrentQty >= hu.Qty).FirstOrDefault();
                if (matchedOrderDetail == null)
                {
                    if (orderMaster.IsShipExceed)
                    {
                        matchedOrderDetail = matchedOrderDetailList.First();
                    }
                    else
                    {
                        throw new BusinessException("不允许超发物料{0}。", hu.Item);
                    }
                }
                #endregion

                //#region 再匹配允许超发的订单，未发满但是+本次发货超发了
                //if (matchedOrderDetail == null)
                //{
                //    IList<string> orderNoList = orderMasters.Where(o => o.IsShipExceed).Select(o => o.OrderNo).ToList();
                //    matchedOrderDetail = MatchOrderDetail(hu, matchedOrderDetailList.Where(o => (o.CurrentQty > 0)
                //        && (o.CurrentQty < hu.Qty) && orderNoList.Contains(o.OrderNo)).ToList());

                //    #region 再匹配允许超发的订单，已经满了或已经超发了
                //    if (matchedOrderDetail == null)
                //    {
                //        matchedOrderDetail = MatchOrderDetail(hu, matchedOrderDetailList.Where(o => (o.CurrentQty <= 0) && orderNoList.Contains(o.OrderNo)).ToList());
                //    }
                //    #endregion
                //}
                #endregion

                #region 未找到匹配的订单，报错信息
                //if (matchedOrderDetail == null)
                //{
                //    if (string.IsNullOrEmpty(hu.ManufactureParty))
                //    {
                //        //条码未指定制造商
                //        if (matchedOrderDetailList.Where(o => string.IsNullOrEmpty(o.ManufactureParty)).Count() > 0)
                //        {
                //            //有未指定制造商的订货明细
                //            throw new BusinessException("和条码{0}匹配的订单明细的发货数已经全部满足。", huId, hu.Item);
                //        }
                //        else
                //        {
                //            //没有未指定制造商的订货明细
                //            throw new BusinessException("待发货订单明细指定了制造商，而条码{0}没有指定制造商", huId);
                //        }
                //    }
                //    else
                //    {
                //        //条码指定了制造商
                //        if (matchedOrderDetailList.Where(o => o.ManufactureParty == hu.ManufactureParty).Count() > 0)
                //        {
                //            //有未指定制造商的订货明细
                //            throw new BusinessException("和条码{0}匹配的订单明细的发货数已经全部满足。", huId);
                //        }
                //        else
                //        {
                //            //没有未指定制造商的订货明细
                //            throw new BusinessException("待发货订单明细指定的制造商和条码{0}制造商{1}不匹配", huId, hu.ManufactureParty);
                //        }
                //    }
                //}
                #endregion

                OrderDetailInput orderDetailInput = new OrderDetailInput();
                orderDetailInput.HuId = hu.HuId;
                orderDetailInput.ShipQty = hu.Qty;
                orderDetailInput.LotNo = hu.LotNo;
                orderDetailInput.Id = matchedOrderDetail.Id;

                List<OrderDetailInput> orderDetailInputs = new List<OrderDetailInput>();
                if (matchedOrderDetail.OrderDetailInputs != null)
                {
                    orderDetailInputs = matchedOrderDetail.OrderDetailInputs.ToList();
                }

                orderDetailInputs.Add(orderDetailInput);

                matchedOrderDetail.OrderDetailInputs = orderDetailInputs.ToArray();
                matchedOrderDetail.CurrentQty -= hu.Qty;
                matchedOrderDetail.Carton++;
                base.hus.Insert(0, hu);
            }
            else
            {
                #region 取消
                this.CancelHu(hu);
                #endregion
            }

            this.gvListDataBind();
        }

        private OrderDetail MatchOrderDetail(Hu hu, List<OrderDetail> orderDetailList)
        {
            if (orderDetailList != null && orderDetailList.Count > 0)
            {
                //先匹配发货明细的制造商
                OrderDetail matchedOrderDetail = orderDetailList.Where(o => (o.ManufactureParty == null ? string.Empty : o.ManufactureParty.Trim())
                    == (hu.ManufactureParty == null ? string.Empty : hu.ManufactureParty.Trim())).FirstOrDefault();

                //再匹配没有制造上的发货明细
                if (matchedOrderDetail == null)
                {
                    matchedOrderDetail = orderDetailList.Where(o => string.IsNullOrEmpty(o.ManufactureParty)).FirstOrDefault();
                }

                if (matchedOrderDetail == null)
                {
                    matchedOrderDetail = orderDetailList.FirstOrDefault();
                }
                return matchedOrderDetail;
            }
            return null;
        }

        private void MergeOrderMaster(OrderMaster orderMaster)
        {

            if (orderMaster.OrderStrategy == FlowStrategy.KIT)
            {
                if (this.orderMasters != null && this.orderMasters.Count > 0)
                {
                    throw new BusinessException("KIT单不能合并发货。");
                }
            }

            if (this.orderMasters == null)
            {
                this.orderMasters = new List<OrderMaster>();
            }



            if (orderMasters.Count(o => o.OrderNo == orderMaster.OrderNo) > 0)
            {
                //订单重复扫描检查
                throw new BusinessException("重复扫描订单。");
            }
            #region 订单类型
            var orderType = orderMasters.Where(o => o.Type != orderMaster.Type);
            if (orderType.Count() > 0)
            {
                throw new BusinessException("订单类型不同不能合并发货。");
            }
            #endregion

            #region 订单质量类型
            var qualityType = orderMasters.Where(o => o.QualityType != orderMaster.QualityType);
            if (qualityType.Count() > 0)
            {
                throw new BusinessException("订单质量状态不同不能合并发货。");
            }
            #endregion

            #region PartyFrom
            var partyFrom = orderMasters.Where(o => o.PartyFrom != orderMaster.PartyFrom);
            if (partyFrom.Count() > 0)
            {
                throw new BusinessException("来源组织不同不能合并发货。");
            }
            #endregion

            #region PartyTo
            var partyTo = orderMasters.Where(o => o.PartyTo != orderMaster.PartyTo);
            if (partyTo.Count() > 0)
            {
                throw new BusinessException("目的组织不同不能合并发货。");
            }
            #endregion

            #region ShipFrom
            var shipFrom = orderMasters.Where(o => o.ShipFrom != orderMaster.ShipFrom);
            if (shipFrom.Count() > 0)
            {
                throw new BusinessException("发货地址不同不能合并发货。");
            }
            #endregion

            #region ShipTo
            var shipTo = orderMasters.Where(o => o.ShipTo != orderMaster.ShipTo);
            if (shipTo.Count() > 0)
            {
                throw new BusinessException("收货地址不同不能合并发货。");
            }
            #endregion

            #region Dock
            var dock = orderMasters.Where(o => o.Dock != orderMaster.Dock);
            if (dock.Count() > 0)
            {
                throw new BusinessException("道口不同不能合并发货。");
            }
            #endregion

            #region IsAutoReceive
            var isAutoReceive = orderMasters.Where(o => o.IsAutoReceive != orderMaster.IsAutoReceive);
            if (isAutoReceive.Count() > 0)
            {
                throw new BusinessException("自动收货选项不同不能合并发货。");
            }
            #endregion

            #region IsShipScanHu
            var isShipScanHu = orderMasters.Where(o => o.IsShipScanHu != orderMaster.IsShipScanHu);
            if (isShipScanHu.Count() > 0)
            {
                throw new BusinessException("发货扫描条码选项不同不能合并发货。");
            }
            #endregion

            #region IsRecScanHu
            var isRecScanHu = orderMasters.Where(o => o.IsReceiveScanHu != orderMaster.IsReceiveScanHu);
            if (isRecScanHu.Count() > 0)
            {
                throw new BusinessException("收货扫描条码选项不同不能合并发货。");
            }
            #endregion

            #region IsRecExceed
            var isRecExceed = orderMasters.Where(o => o.IsReceiveExceed != orderMaster.IsReceiveExceed);
            if (isRecExceed.Count() > 0)
            {
                throw new BusinessException("允许超收选项不同不能合并发货。");
            }
            #endregion

            #region IsRecFulfillUC
            var isRecFulfillUC = orderMasters.Where(o => o.IsReceiveFulfillUC != orderMaster.IsReceiveFulfillUC);
            if (isRecFulfillUC.Count() > 0)
            {
                throw new BusinessException("收货满足包装选项不同不能合并发货。");
            }
            #endregion

            #region IsRecFifo
            var isRecFifo = orderMasters.Where(o => o.IsReceiveFifo != orderMaster.IsReceiveFifo);
            if (isRecFifo.Count() > 0)
            {
                throw new BusinessException("收货先进先出选项不同不能合并发货。");
            }
            #endregion

            #region IsAsnAuotClose
            var isAsnAuotClose = orderMasters.Where(o => o.IsAsnAutoClose != orderMaster.IsAsnAutoClose);
            if (isAsnAuotClose.Count() > 0)
            {
                throw new BusinessException("送货单自动关闭选项不同不能合并发货。");
            }
            #endregion

            #region IsAsnUniqueRec
            var isAsnUniqueRec = orderMasters.Where(o => o.IsAsnUniqueReceive != orderMaster.IsAsnUniqueReceive);
            if (isAsnUniqueRec.Count() > 0)
            {
                throw new BusinessException("送货单一次性收货选项不同不能合并发货。");
            }
            #endregion

            #region IsRecCreateHu
            var createHuOption = from om in orderMasters
                                 where om.CreateHuOption == CreateHuOption.Receive
                                 select om.CreateHuOption;
            if (createHuOption != null && createHuOption.Count() > 0 && createHuOption.Count() != orderMasters.Count())
            {
                throw new BusinessException("收货创建条码选项不同不能合并发货。");
            }
            #endregion

            #region RecGapTo
            var recGapTo = orderMasters.Where(o => o.ReceiveGapTo != orderMaster.ReceiveGapTo);
            if (recGapTo.Count() > 0)
            {
                throw new BusinessException("收货差异调整选项不同不能合并发货。");
            }
            #endregion

            foreach (var orderDetail in orderMaster.OrderDetails)
            {
                orderDetail.RemainShippedQty = orderDetail.OrderedQty > orderDetail.ShippedQty ? orderDetail.OrderedQty - orderDetail.ShippedQty : 0;
                orderDetail.CurrentQty = orderDetail.RemainShippedQty;
            }
            this.orderMasters.Add(orderMaster);
            this.gvListDataBind();
        }

        protected override Hu DoCancel()
        {
            Hu firstHu = base.DoCancel();
            this.CancelHu(firstHu);
            return firstHu;
        }

        private void CancelHu(Hu hu)
        {
            //if (this.orderMasters == null || this.orderMasters.Count() == 0)
            if (this.hus == null)
            {
                //this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                this.Reset();
                return;
            }

            if (hu != null)
            {
                var orderDetailList = new List<OrderDetail>();
                foreach (var om in this.orderMasters)
                {
                    orderDetailList.AddRange(om.OrderDetails);
                }

                foreach (var orderDetail in orderDetailList)
                {
                    if (orderDetail.OrderDetailInputs != null)
                    {
                        var q_pdi = orderDetail.OrderDetailInputs.Where(p => p.HuId == hu.HuId);
                        if (q_pdi != null && q_pdi.Count() > 0)
                        {
                            var orderDetailInputList = orderDetail.OrderDetailInputs.ToList();
                            orderDetailInputList.Remove(q_pdi.First());
                            orderDetail.OrderDetailInputs = orderDetailInputList.ToArray();
                            orderDetail.CurrentQty += hu.Qty;
                            orderDetail.Carton--;
                            break;
                        }
                    }
                }
                base.hus = base.hus.Where(h => !h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase)).ToList();
                this.gvHuListDataBind();
            }
        }
    }
}
