using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCReceive : UCBase
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        private List<OrderMaster> orderMasters;
        private IpMaster ipMaster;
        private string binCode;
        private DateTime? effDate;
        private bool isScanOne;
        private FlowMaster flowMaster;
        private bool isContinueScanOrder;
        //private List<IpDetailInput> ipDetailProcess;
        private static UCReceive ucReceive;
        private static object obj = new object();

        public UCReceive(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.btnOrder.Text = "收货";
        }

        public static UCReceive GetUCReceive(User user)
        {
            if (ucReceive == null)
            {
                lock (obj)
                {
                    if (ucReceive == null)
                    {
                        ucReceive = new UCReceive(user);
                    }
                }
            }
            ucReceive.user = user;
            ucReceive.Reset();
            ucReceive.lblMessage.Text = "请扫描订单或送货单";
            return ucReceive;
        }

        #region Event

        protected override void ScanBarCode()
        {
            base.ScanBarCode();

            if (this.orderMasters.Count == 0 && this.ipMaster == null)
            {
                if (base.op == CodeMaster.BarCodeType.ORD.ToString() || base.op == null)
                {
                    this.orderMasters = new List<OrderMaster>();
                    var orderMaster = smartDeviceService.GetOrder(base.barCode, true);

                    if (orderMaster == null)
                    {
                        throw new BusinessException("订单不存在");
                    }
                    if (orderMaster.IsPause)
                    {
                        throw new BusinessException("订单已暂停");
                    }
                    this.CheckAndMerge(orderMaster);
                    this.lblMessage.Text = "请继续扫描订单或者物料条码";
                }
                else if (base.op == CodeMaster.BarCodeType.ASN.ToString() || base.op == CodeMaster.BarCodeType.W.ToString() || base.op == CodeMaster.BarCodeType.SP.ToString())
                {
                    var ipMaster = new IpMaster();
                    ipMaster = smartDeviceService.GetIp(base.barCode, true);

                    if (ipMaster.IpDetails == null || ipMaster.IpDetails.Length == 0)
                    {
                        throw new BusinessException("没有送货单明细");
                    }
                    if (!Utility.HasPermission(ipMaster, base.user))
                    {
                        throw new BusinessException("没有送货单的权限。");
                    }
                    VerifyPermission(ipMaster.PartyTo);
                    if (ipMaster.IsReceiveScanHu == false && ipMaster.Type != IpType.KIT)
                    {
                        throw new BusinessException("送货单{0}不需要扫描条码。", ipMaster.IpNo);
                        //DialogResult dr = MessageBox.Show("送货单收货不需要扫描条码，是否继续?", "无需扫描", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        //if (dr == DialogResult.No)
                        //{
                        //    this.isMark = true;
                        //    return;
                        //}
                    }
                    var ipDetailList = ipMaster.IpDetails.Where(o => o.RemainReceivedQty > 0).ToList();
                    if (ipDetailList.Count == 0)
                    {
                        throw new BusinessException("送货单{0}已完成收货。", ipMaster.IpNo);
                    }

                    ipDetailList = new List<IpDetail>();
                    ipDetailList = ipMaster.IpDetails.Where(o => !string.IsNullOrEmpty(o.GapReceiptNo)).ToList();
                    if (ipDetailList.Count > 0)
                    {
                        throw new BusinessException("送货单{0}已完成收货。", ipMaster.IpNo);
                    }
                    this.lblMessage.Text = "请扫描物料条码。";
                    this.ipMaster = ipMaster;
                    this.tbBarCode.Focus();
                    this.gvListDataBind();
                }
                else
                {
                    throw new BusinessException("请扫描订单或送货单。");
                }
            }
            else
            {
                if (base.op == CodeMaster.BarCodeType.ORD.ToString() && this.isContinueScanOrder == true)
                {
                    var orderMaster = smartDeviceService.GetOrder(base.barCode, true);
                    if (orderMaster.IsPause)
                    {
                        throw new BusinessException("订单已暂停。");
                    }
                    this.CheckAndMerge(orderMaster);
                }
                else if (base.op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if ((this.orderMasters == null || this.orderMasters.Count() == 0) && (this.ipMaster == null))
                    {
                        throw new BusinessException("请先扫描订单。");
                    }
                    Hu hu = smartDeviceService.GetHu(barCode);
                    if (!string.IsNullOrEmpty(hu.PalletCode))
                    {
                        throw new BusinessException("条码已与托盘绑定，请扫描托盘。");
                    }

                    if (this.ipMaster == null)
                    {
                        this.MatchOrderMaster(hu);
                    }
                    else
                    {
                        this.MatchIpMaster(hu);
                    }
                    this.isContinueScanOrder = false;

                }
                else if (base.op == CodeMaster.BarCodeType.TP.ToString())
                {
                    if ((this.orderMasters == null || this.orderMasters.Count() == 0) && (this.ipMaster == null))
                    {
                        throw new BusinessException("请先扫描订单。");
                    }
                    Hu[] huArray = smartDeviceService.GetHuListByPallet(barCode);

                    if (this.ipMaster == null)
                    {
                        foreach (Hu hu in huArray)
                        {
                            this.MatchOrderMaster(hu);
                        }
                    }
                    else
                    {
                        foreach (Hu hu in huArray)
                        {
                            this.MatchIpMaster(hu);
                        }
                    }
                    this.isContinueScanOrder = false;

                }
                else if (base.op == CodeMaster.BarCodeType.B.ToString())
                {
                    base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                    Bin bin = smartDeviceService.GetBin(base.barCode);
                    this.binCode = bin.Code;
                    this.lblMessage.Text = "当前库格:" + bin.Code;
                    //检查权限
                    if (!Utility.HasPermission(user.Permissions, null, false, true, null, bin.Region))
                    {
                        throw new BusinessException("没有此库格的权限");
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
        }

        #endregion

        #region DataBind
        protected override void gvListDataBind()
        {
            base.gvListDataBind();

            if (this.ipMaster == null)
            {
                List<OrderDetail> orderDetailList = new List<OrderDetail>();
                if (this.orderMasters != null)
                {
                    foreach (var om in this.orderMasters)
                    {
                        orderDetailList.AddRange(om.OrderDetails.Where(o => o.CurrentQty > 0));
                    }
                }
                base.dgList.DataSource = orderDetailList;
                ts.MappingName = orderDetailList.GetType().Name;
            }
            else
            {
                List<IpDetail> ipDetailList = new List<IpDetail>();
                if (this.ipMaster.IpDetails != null)
                {
                    if (this.ipMaster.Type == IpType.KIT)
                    {
                        foreach (var ipDetail in this.ipMaster.IpDetails)
                        {
                            if (ipDetail.IsScanHu && ipDetail.CurrentQty > 0)
                            {
                                ipDetailList.Add(ipDetail);
                            }
                        }
                    }
                    else
                    {
                        ipDetailList = this.ipMaster.IpDetails.Where(o => o.CurrentQty > 0).ToList();
                    }
                }
                base.ts.MappingName = ipDetailList.GetType().Name;
                base.dgList.DataSource = ipDetailList;
            }
        }

        #endregion

        #region Init Reset
        protected override void Reset()
        {
            //this.ipDetailProcess = new List<IpDetailInput>();
            this.flowMaster = null;
            this.orderMasters = new List<OrderMaster>();
            this.ipMaster = null;
            this.isContinueScanOrder = true;
            base.Reset();
            this.effDate = null;
        }
        #endregion

        protected override void DoSubmit()
        {
            if ((this.orderMasters == null || this.orderMasters.Count == 0)
                && (this.ipMaster == null))
            {
                throw new BusinessException("请先扫描订单或送货单。");
            }

            var receiptNo = string.Empty;

            #region 要货单收货的逻辑
            //ORD收货
            if (this.ipMaster == null)
            {
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
                    DialogResult dr = MessageBox.Show("本次收货有未收完的明细,是否部分收货?", "未全部收货", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.No)
                    {
                        this.isMark = true;
                        this.tbBarCode.Focus();
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

                if (orderDetailInputList.Count == 0)
                {
                    throw new BusinessException("没有扫描条码");
                }
                if (this.orderMasters[0].OrderStrategy == FlowStrategy.KIT && this.orderMasters[0].Status == OrderStatus.Submit
                    && this.orderMasters[0].OrderDetails.Count(o => o.CurrentQty != 0) > 0)
                {
                    throw new BusinessException("必须满足KIT单收货数。");
                }
                receiptNo = this.smartDeviceService.DoReceiveOrder(orderDetailInputList.ToArray(), this.effDate, base.user.Code);
            }
            #endregion

            #region ASN收货逻辑
            //ASN收货
            else
            {
                List<IpDetailInput> ipDetailInputList = new List<IpDetailInput>();
                if (this.ipMaster.IpDetails.Any(od => od.CurrentQty > 0))
                {
                    DialogResult dr = MessageBox.Show("本次收货有未收完的明细,是否继续?", "未全部收货", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.No)
                    {
                        this.isMark = true;
                        this.tbBarCode.Focus();
                        return;
                    }
                }
                foreach (var ipDetailInput in this.ipMaster.IpDetailInputs)
                {
                    if (ipDetailInput.ReceiveQty > 0)
                    {
                        ipDetailInputList.Add(ipDetailInput);
                    }
                }
                if (ipDetailInputList.Count == 0)
                {
                    this.Reset();
                    throw new BusinessException("本次收货出现错误，请重新收货");
                }
                receiptNo = this.smartDeviceService.DoReceiveIp(ipDetailInputList.ToArray(), this.effDate, base.user.Code);
            }
            #endregion

            this.Reset();
            base.lblMessage.Text = string.Format("收货成功,收货单号:{0}", receiptNo);
            this.isMark = true;
        }

        private void MatchOrderMaster(Hu hu)
        {
            if (hu == null)
            {
                throw new BusinessException("条码不存在");
            }

            hu.CurrentQty = hu.Qty;
            var matchHu = this.hus.Where(h => h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));

            #region 取消条码
            if (this.isCancel)
            {
                if (matchHu == null || matchHu.Count() == 0)
                {
                    throw new BusinessException("没有需要取消匹配条码:{0}", hu.HuId);
                }
                else if (matchHu.Count() == 1)
                {
                    this.CancelHu(hu);
                }
                else
                {
                    throw new Exception("匹配了多个条码");
                }
            }
            #endregion
            #region 检查条码并匹配收货条件。
            else
            {
                if (hu.IsFreeze)
                {
                    throw new BusinessException("条码被冻结!");
                }
                if (this.ipMaster == null &&
                    (this.orderMasters[0].Type == OrderType.Transfer || this.orderMasters[0].Type == OrderType.SubContractTransfer))
                {
                    if (hu.Status != HuStatus.Location)
                    {
                        throw new BusinessException("不在库存中的条码不能被收货");
                    }

                    if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
                    {
                        throw new BusinessException("没有此条码的权限");
                    }
                }
                else
                {
                    if (hu.Status == HuStatus.Location)
                    {
                        throw new BusinessException("在库存中的条码不能被收货");
                    }
                }
                if (this.orderMasters[0].Type == OrderType.Procurement)
                {
                    if (hu.Status == HuStatus.Location)
                    {
                        throw new BusinessException("条码已经在库位{0}中", hu.Location);
                    }
                    if (hu.Status == HuStatus.Ip)
                    {
                        throw new BusinessException("条码已经在途{0}");
                    }
                }
                else if (this.orderMasters[0].Type == OrderType.Transfer)
                {
                    if (hu.Status != HuStatus.Location)
                    {
                        throw new BusinessException("条码不在库位中");
                    }
                }

                if (matchHu != null && matchHu.Count() > 0)
                {
                    throw new BusinessException("条码重复扫描!");
                }

                if (this.orderMasters[0].OrderStrategy == FlowStrategy.KIT)
                {
                    if (this.orderMasters.Count > 1)
                    {
                        this.Reset();
                        throw new BusinessException("合并收货包括了KIT单，全部取消，重新扫描");
                    }
                    //接下来应该是统一的逻辑去处理 
                }

                var orderDetailList = new List<OrderDetail>();
                foreach (var om in this.orderMasters)
                {
                    orderDetailList.AddRange(om.OrderDetails);
                }
                #region  标准匹配流程
                List<Hu> inputHus = new List<Hu>();
                List<Hu> needRemoveHus = new List<Hu>();
                inputHus.Add(hu);
                orderDetailList = orderDetailList.OrderByDescending(p => p.RemainReceivedQty).ToList();

                //循环条码做非条码匹配
                foreach (Hu inputHu in inputHus)
                {
                    //var matchIpDetail = new IpDetail();
                    //匹配物料，包装，供应商，质量状态(没有手动修改单包装的)
                    var matchOrderDetailsFirst = new List<OrderDetail>();
                    matchOrderDetailsFirst = orderDetailList.Where(i => i.Item == inputHu.Item &&
                        //i.UnitCount == inputHu.UnitCount &&
                                                i.Uom == inputHu.Uom &&
                                                //i.ManufactureParty == inputHu.ManufactureParty &&
                                                i.RemainReceivedQty >= inputHu.Qty &&
                                                i.QualityType == inputHu.QualityType &&
                                                i.IsChangeUnitCount == false).ToList();

                    if (matchOrderDetailsFirst.Count() > 0)
                    {
                        var matchedOrderDetail = matchOrderDetailsFirst.First();

                        var orderDetailInput = new OrderDetailInput();
                        orderDetailInput.HuId = inputHu.HuId;
                        orderDetailInput.Id = matchedOrderDetail.Id;
                        orderDetailInput.LotNo = inputHu.LotNo;
                        orderDetailInput.ReceiveQty = inputHu.Qty;
                        matchedOrderDetail.RemainReceivedQty = matchedOrderDetail.RemainReceivedQty - inputHu.Qty;
                        matchedOrderDetail.CurrentQty = matchedOrderDetail.CurrentQty - inputHu.Qty;
                        matchedOrderDetail.Carton++;
                        List<OrderDetailInput> orderDetailsProcess = matchedOrderDetail.OrderDetailInputs.ToList();
                        orderDetailsProcess.Add(orderDetailInput);
                        matchedOrderDetail.OrderDetailInputs = orderDetailsProcess.ToArray();
                        needRemoveHus.Add(inputHu);
                    }

                    //匹配物料，供应商，质量状态(手动修改单包装的)
                    matchOrderDetailsFirst = new List<OrderDetail>();
                    matchOrderDetailsFirst = orderDetailList.Where(i => i.Item == inputHu.Item &&
                                                i.Uom == inputHu.Uom &&
                                                //i.ManufactureParty == inputHu.ManufactureParty &&
                                                i.RemainReceivedQty >= inputHu.Qty &&
                                                i.QualityType == inputHu.QualityType &&
                                                i.IsChangeUnitCount == true).ToList();

                    if (matchOrderDetailsFirst.Count() > 0)
                    {
                        var matchedOrderDetail = matchOrderDetailsFirst.First();

                        var orderDetailInput = new OrderDetailInput();
                        orderDetailInput.HuId = inputHu.HuId;
                        orderDetailInput.Id = matchedOrderDetail.Id;
                        orderDetailInput.LotNo = inputHu.LotNo;
                        orderDetailInput.ReceiveQty = inputHu.Qty;
                        matchedOrderDetail.RemainReceivedQty = matchedOrderDetail.RemainReceivedQty - inputHu.Qty;
                        matchedOrderDetail.CurrentQty = matchedOrderDetail.CurrentQty - inputHu.Qty;
                        matchedOrderDetail.Carton++;
                        List<OrderDetailInput> orderDetailsProcess = matchedOrderDetail.OrderDetailInputs.ToList();
                        orderDetailsProcess.Add(orderDetailInput);
                        matchedOrderDetail.OrderDetailInputs = orderDetailsProcess.ToArray();
                        needRemoveHus.Add(inputHu);
                    }
                }

                foreach (var item in needRemoveHus)
                {
                    inputHus.Remove(item);
                }
                needRemoveHus = new List<Hu>();

                //如果inputHus没有全部匹配，接下来匹配物料，包装，质量状态
                if (inputHus.Count > 0)
                {
                    foreach (Hu inputHu in inputHus)
                    {
                        //var matchIpDetail = new IpDetail();
                        //再匹配物料，质量状态
                        var matchOrderDetailsFirst = new List<OrderDetail>();
                        matchOrderDetailsFirst = orderDetailList.Where(i => i.Item.Equals(inputHu.Item, StringComparison.OrdinalIgnoreCase) &&
                            //i.UnitCount == inputHu.UnitCount &&
                                                        i.Uom == inputHu.Uom &&
                            //i.ManufactureParty.Equals(inputHu.ManufactureParty, StringComparison.OrdinalIgnoreCase) &&
                                                        i.RemainReceivedQty >= inputHu.Qty &&
                                                        i.QualityType == inputHu.QualityType &&
                                                        i.IsChangeUnitCount == false).ToList();

                        if (matchOrderDetailsFirst.Count() > 0)
                        {
                            var matchedOrderDetail = matchOrderDetailsFirst.First();

                            var orderDetailInput = new OrderDetailInput();
                            orderDetailInput.HuId = inputHu.HuId;
                            orderDetailInput.Id = matchedOrderDetail.Id;
                            orderDetailInput.LotNo = inputHu.LotNo;
                            orderDetailInput.ReceiveQty = inputHu.Qty;
                            matchedOrderDetail.RemainReceivedQty = matchedOrderDetail.RemainReceivedQty - inputHu.Qty;
                            matchedOrderDetail.CurrentQty = matchedOrderDetail.CurrentQty - inputHu.Qty;
                            matchedOrderDetail.Carton++;
                            List<OrderDetailInput> orderDetailsProcess = matchedOrderDetail.OrderDetailInputs.ToList();
                            orderDetailsProcess.Add(orderDetailInput);
                            matchedOrderDetail.OrderDetailInputs = orderDetailsProcess.ToArray();
                            needRemoveHus.Add(inputHu);
                        }

                        //再匹配物料，包装，质量状态
                        matchOrderDetailsFirst = new List<OrderDetail>();
                        matchOrderDetailsFirst = orderDetailList.Where(i => i.Item.Equals(inputHu.Item, StringComparison.OrdinalIgnoreCase) &&
                            //i.UnitCount == inputHu.UnitCount &&
                                                    i.Uom == inputHu.Uom &&
                            //i.ManufactureParty.Equals(inputHu.ManufactureParty, StringComparison.OrdinalIgnoreCase) &&
                                                    i.RemainReceivedQty >= inputHu.Qty &&
                                                    i.QualityType == inputHu.QualityType &&
                                                    i.IsChangeUnitCount == true).ToList();

                        if (matchOrderDetailsFirst.Count() > 0)
                        {
                            var matchedOrderDetail = matchOrderDetailsFirst.First();

                            var orderDetailInput = new OrderDetailInput();
                            orderDetailInput.HuId = inputHu.HuId;
                            orderDetailInput.Id = matchedOrderDetail.Id;
                            orderDetailInput.LotNo = inputHu.LotNo;
                            orderDetailInput.ReceiveQty = inputHu.Qty;
                            matchedOrderDetail.RemainReceivedQty = matchedOrderDetail.RemainReceivedQty - inputHu.Qty;
                            matchedOrderDetail.CurrentQty = matchedOrderDetail.CurrentQty - inputHu.Qty;
                            matchedOrderDetail.Carton++;
                            List<OrderDetailInput> orderDetailsProcess = matchedOrderDetail.OrderDetailInputs.ToList();
                            orderDetailsProcess.Add(orderDetailInput);
                            matchedOrderDetail.OrderDetailInputs = orderDetailsProcess.ToArray();
                            needRemoveHus.Add(inputHu);
                        }
                    }
                    foreach (var item in needRemoveHus)
                    {
                        inputHus.Remove(item);
                    }
                }

                //如果还有未匹配成功的就报错
                if (inputHus.Count > 0)
                {
                    throw new BusinessException("条码{0}在不满足订单所有收货明细行收货条件", hu.HuId);
                }

                #endregion
                this.gvListDataBind();
                this.lblMessage.Text = "请扫描收货物料条码";
                this.hus.Add(hu);
            #endregion
            }

        }

        /// <summary>
        /// 匹配逻辑:
        /// 1.如果发货时扫描了条码,就按条码匹配,条码匹配上Hu.ShipQty=Hu.Qty;如果条码匹配不上,就按物料匹配,物料匹配上Hu.ShipQty=0;
        ///   如果ipDetail上匹配的条码总数量大于ipDetail.Qty,则把Hu.ShipQty=0的拿出来再重新按物料匹配一次.
        /// 2.如果发货没有扫描条码,就按物料匹配.
        /// </summary>
        /// <param name="hu"></param>
        private void MatchIpMaster(Hu hu)
        {
            if (hu == null)
            {
                throw new BusinessException("条码不存在");
            }

            hu.CurrentQty = hu.Qty;
            var matchHu = this.hus.Where(h => h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));

            #region 取消条码
            if (this.isCancel)
            {
                if (matchHu == null || matchHu.Count() == 0)
                {
                    throw new BusinessException("没有需要取消匹配条码:{0}", hu.HuId);
                }
                else if (matchHu.Count() == 1)
                {
                    this.CancelHu(hu);
                }
                else
                {
                    throw new Exception("匹配了多个条码");
                }
            }
            #endregion
            #region 检查条码并匹配收货条件。
            else
            {
                #region 检查条码
                if (hu.IsFreeze)
                {
                    throw new BusinessException("条码被冻结!");
                }
                //if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
                //{
                //    throw new BusinessException("没有此条码的权限");
                //}
                if (hu.Status == HuStatus.Location)
                {
                    throw new BusinessException("在库存中的条码不能被收货");
                }

                if (matchHu != null && matchHu.Count() > 0)
                {
                    throw new BusinessException("条码重复扫描!");
                }
                #endregion

                var ipDetails = this.ipMaster.IpDetails;

                string huId = hu.HuId;

                if (this.ipMaster.Type == IpType.KIT)
                {
                    if (false)//(this.ipMaster.PartyFrom == this.smartDeviceService.GetEntityPreference(CodeEnum.WMSAnjiRegion) || this.ipMaster.OrderType == OrderType.ScheduleLine || this.ipMaster.OrderType == OrderType.Procurement)
                    {
                        throw new BusinessException("外部KIT单收货不需要扫描条码");
                    }
                    else
                    {
                        this.Reset();
                        throw new BusinessException("内部KIT单不需要扫描送货单。");
                    }
                }

                if (this.ipMaster.Type == IpType.SEQ)
                {
                    //如果在途库存上全部有条码，全部匹配条码，否则全部按物料匹配
                    if (this.ipMaster.IpDetails.All(i => i.RemainReceivedQty == 0))
                    {
                        throw new BusinessException("排序送货单{0}已收满。", this.ipMaster.IpNo);
                    }
                    if (this.ipMaster.IpDetailInputs.Where(i => !string.IsNullOrEmpty(i.HuId) && i.IsOriginal == true).Count() > 0)
                    {
                        //通过在途条码匹配
                        if (this.ipMaster.IpDetailInputs.All(i => i.HuId != hu.HuId))
                        {
                            throw new BusinessException("条码{0}不是排序单所需的物料。", hu.HuId);
                        }
                        else
                        {
                            var matchedIpDetailInput = this.ipMaster.IpDetailInputs.FirstOrDefault(i => i.HuId == hu.HuId);
                            matchedIpDetailInput.ReceiveQty = hu.Qty;
                            var ipDetail = this.ipMaster.IpDetails.FirstOrDefault(i => i.Id == matchedIpDetailInput.Id);
                            ipDetail.RemainReceivedQty = ipDetail.RemainReceivedQty - hu.Qty;
                            ipDetail.CurrentQty = ipDetail.CurrentQty - hu.Qty;
                            ipDetail.Carton++;
                        }
                    }
                    else
                    {
                        if (!this.ipMaster.IsReceiveScanHu)
                        {
                            throw new BusinessException("按数量收货不需要扫描条码，直接按确定收货。");
                        }
                        //通过物料号，单包装，供应商和质量状态匹配
                        int minSeq = this.ipMaster.IpDetails.Where(i => i.RemainReceivedQty > 0).Min(i => i.Sequence);
                        var needMatchIpDetail = this.ipMaster.IpDetails.FirstOrDefault(i => i.Sequence == minSeq);
                        if (needMatchIpDetail.Item == hu.Item
                            //&& needMatchIpDetail.UnitCount == hu.UnitCount
                            //&& needMatchIpDetail.ManufactureParty == hu.ManufactureParty
                            && needMatchIpDetail.QualityType == hu.QualityType)
                        {
                            var ipDetailInput = new IpDetailInput();
                            ipDetailInput.HuId = hu.HuId;
                            ipDetailInput.Id = needMatchIpDetail.Id;
                            ipDetailInput.LotNo = hu.LotNo;
                            ipDetailInput.ReceiveQty = hu.Qty;
                            needMatchIpDetail.RemainReceivedQty = needMatchIpDetail.RemainReceivedQty - hu.Qty;
                            needMatchIpDetail.CurrentQty = needMatchIpDetail.CurrentQty - hu.Qty;
                            needMatchIpDetail.Carton++;
                            List<IpDetailInput> ipDetailProcess = this.ipMaster.IpDetailInputs.ToList();
                            ipDetailProcess.Add(ipDetailInput);
                            this.ipMaster.IpDetailInputs = ipDetailProcess.ToArray();
                        }

                    }
                    this.hus.Add(hu);
                    this.isScanOne = true;
                }
                else
                {
                    //如果发货和收货都扫描条码,严格匹配条码
                    if (this.ipMaster.IsShipScanHu && this.ipMaster.IsReceiveScanHu)
                    {
                        var ipDetailInput = this.ipMaster.IpDetailInputs.FirstOrDefault(id => id.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));
                        if (ipDetailInput == null)
                        {
                            throw new BusinessException("在送货单{0}明细中没有条码{1}。", this.ipMaster.IpNo, hu.HuId);
                        }
                        var ipDetail = this.ipMaster.IpDetails.Single(i => i.Id == ipDetailInput.Id);

                        ipDetailInput.ReceiveQty = hu.Qty;
                        ipDetailInput.LotNo = hu.LotNo;
                        ipDetailInput.IsMatchedHu = true;

                        ipDetail.RemainReceivedQty -= hu.Qty;
                        ipDetail.CurrentQty -= hu.Qty;
                        ipDetail.Carton++;

                        base.hus.Insert(0, hu);
                        this.gvListDataBind();
                        return;
                    }

                    var isNeedRematch = false;
                    var matchedHuDetailInputs = new List<IpDetailInput>();
                    //首先匹配条码，如果条码匹配的话就调整ipDetail的数量RemainReceivedQty
                    //只有在条码完全匹配并且数量不满足收货的情况下产生IpDetailInputs的全部调整
                    var ipDetailInputMatchHuIds = this.ipMaster.IpDetailInputs.Where(id => id.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (ipDetailInputMatchHuIds.Count != 0)
                    {
                        var ipDetailInput = ipDetailInputMatchHuIds.First();
                        //var ipDetailMatchQty = this.ipMaster.IpDetails.Where(i => i.Id == ipDetailInput.Id && i.UnitCount ==hu.UnitCount && i.ManufactureParty==hu.ManufactureParty && i.RemainReceivedQty > hu.Qty).ToList();
                        var ipDetailMatchHu = this.ipMaster.IpDetails.Where(i => i.Id == ipDetailInput.Id).Single();
                        if (ipDetailMatchHu.RemainReceivedQty < hu.Qty)
                        {
                            isNeedRematch = true;
                            //将该物料号未匹配条码的先移开
                            foreach (var ipDetail in this.ipMaster.IpDetails.Where(i => i.Item == hu.Item))
                            {
                                matchedHuDetailInputs.AddRange(this.ipMaster.IpDetailInputs.Where(i => i.Id == ipDetail.Id && i.IsOriginal == true).ToList());
                                var matchTotal = this.ipMaster.IpDetailInputs.Where(i => i.Id == ipDetail.Id && i.IsMatchedHu == true).ToList().Sum(n => n.ReceiveQty);
                                ipDetail.RemainReceivedQty = ipDetail.Qty - ipDetail.ReceivedQty - matchTotal;
                                ipDetail.CurrentQty = ipDetail.Qty - ipDetail.ReceivedQty - matchTotal;
                            }
                            this.ipMaster.IpDetailInputs = matchedHuDetailInputs.ToArray();

                        }
                        //将满足的条码先放进去
                        ipDetailInput.ReceiveQty = hu.Qty;
                        ipDetailInput.LotNo = hu.LotNo;
                        ipDetailInput.IsMatchedHu = true;

                        ipDetailMatchHu.RemainReceivedQty = ipDetailMatchHu.RemainReceivedQty - hu.Qty;
                        ipDetailMatchHu.CurrentQty = ipDetailMatchHu.CurrentQty - hu.Qty;
                        ipDetailMatchHu.Carton++;
                        //this.ipMaster.IpDetails.Where(i => i.Id == ipDetailInput.Id).Single().CurrentQty -= hu.Qty;

                        //如果条码匹配上并且其他的物料也没有溢出那么直接返回
                        if (isNeedRematch == false)
                        {
                            base.hus.Insert(0, hu);
                            this.gvListDataBind();
                            return;
                        }
                    }

                    //以下为非条码匹配的标准流程
                    #region  标准匹配流程
                    List<Hu> inputHus = new List<Hu>();
                    List<Hu> needRemoveHus = new List<Hu>();
                    //如果有条码被从已经匹配的地方挪出来，那么这个物料除了条码已经匹配上的，其他全部重新匹配
                    if (isNeedRematch == true)
                    {
                        var matchedInputHus = (from h in this.hus
                                               from detailInput in this.ipMaster.IpDetailInputs
                                               where h.HuId == detailInput.HuId
                                               && detailInput.IsMatchedHu
                                               select h).ToList();

                        foreach (var item in this.hus)
                        {
                            if (matchedInputHus.All(m => m.HuId != item.HuId))
                            {
                                inputHus.Add(item);
                            }
                        }
                    }
                    else
                    {
                        inputHus.Add(hu);
                    }

                    //循环条码做非条码匹配
                    foreach (Hu inputHu in inputHus)
                    {
                        //var matchIpDetail = new IpDetail();
                        //匹配物料，包装，供应商，质量状态(没有手动修改单包装的)
                        var matchIpDetailsFirst = new List<IpDetail>();
                        matchIpDetailsFirst = this.ipMaster.IpDetails.Where(i => i.Item == inputHu.Item &&
                                                        i.Uom == inputHu.Uom &&
                            //i.UnitCount == inputHu.UnitCount &&
                                                        //i.ManufactureParty == inputHu.ManufactureParty &&
                                                        i.RemainReceivedQty >= inputHu.Qty &&
                                                        i.QualityType == inputHu.QualityType &&
                                                        i.IsChangeUnitCount == false).ToList();

                        if (matchIpDetailsFirst.Count() > 0)
                        {
                            var matchIpDetail = matchIpDetailsFirst.First();

                            var ipDetailInput = new IpDetailInput();
                            ipDetailInput.HuId = inputHu.HuId;
                            ipDetailInput.Id = matchIpDetail.Id;
                            ipDetailInput.LotNo = inputHu.LotNo;
                            ipDetailInput.ReceiveQty = inputHu.Qty;
                            matchIpDetail.RemainReceivedQty = matchIpDetail.RemainReceivedQty - inputHu.Qty;
                            matchIpDetail.CurrentQty = matchIpDetail.CurrentQty - inputHu.Qty;
                            matchIpDetail.Carton++;
                            List<IpDetailInput> ipDetailProcess = this.ipMaster.IpDetailInputs.ToList();
                            ipDetailProcess.Add(ipDetailInput);
                            this.ipMaster.IpDetailInputs = ipDetailProcess.ToArray();
                            needRemoveHus.Add(inputHu);
                        }

                        //匹配物料，供应商，质量状态(手动修改单包装的)
                        matchIpDetailsFirst = new List<IpDetail>();
                        matchIpDetailsFirst = this.ipMaster.IpDetails.Where(i => i.Item == inputHu.Item &&
                                                        i.Uom == inputHu.Uom &&
                                                        //i.ManufactureParty == inputHu.ManufactureParty &&
                                                        i.RemainReceivedQty >= inputHu.Qty &&
                                                        i.QualityType == inputHu.QualityType &&
                                                        i.IsChangeUnitCount == true).ToList();

                        if (matchIpDetailsFirst.Count() > 0)
                        {
                            var matchIpDetail = matchIpDetailsFirst.First();

                            var ipDetailInput = new IpDetailInput();
                            ipDetailInput.HuId = inputHu.HuId;
                            ipDetailInput.Id = matchIpDetail.Id;
                            ipDetailInput.LotNo = inputHu.LotNo;
                            ipDetailInput.ReceiveQty = inputHu.Qty;
                            matchIpDetail.RemainReceivedQty = matchIpDetail.RemainReceivedQty - inputHu.Qty;
                            matchIpDetail.CurrentQty = matchIpDetail.CurrentQty - inputHu.Qty;
                            matchIpDetail.Carton++;
                            List<IpDetailInput> ipDetailProcess = this.ipMaster.IpDetailInputs.ToList();
                            ipDetailProcess.Add(ipDetailInput);
                            this.ipMaster.IpDetailInputs = ipDetailProcess.ToArray();
                            needRemoveHus.Add(inputHu);
                        }
                    }

                    foreach (var item in needRemoveHus)
                    {
                        inputHus.Remove(item);
                    }
                    needRemoveHus = new List<Hu>();

                    //如果inputHus没有全部匹配，接下来匹配物料，包装，质量状态
                    if (inputHus.Count > 0)
                    {
                        foreach (Hu inputHu in inputHus)
                        {
                            //var matchIpDetail = new IpDetail();
                            //再匹配物料，质量状态
                            var matchIpDetailsFirst = new List<IpDetail>();
                            matchIpDetailsFirst = this.ipMaster.IpDetails.Where(i => i.Item.Equals(inputHu.Item, StringComparison.OrdinalIgnoreCase) &&
                                //i.UnitCount == inputHu.UnitCount &&
                                                            i.Uom == inputHu.Uom &&
                                //i.ManufactureParty.Equals(inputHu.ManufactureParty, StringComparison.OrdinalIgnoreCase) &&
                                                            i.RemainReceivedQty >= inputHu.Qty &&
                                                            i.QualityType == inputHu.QualityType &&
                                                            i.IsChangeUnitCount == false).ToList();

                            if (matchIpDetailsFirst.Count() > 0)
                            {
                                var matchIpDetail = matchIpDetailsFirst.First();

                                var ipDetailInput = new IpDetailInput();
                                ipDetailInput.HuId = inputHu.HuId;
                                ipDetailInput.Id = matchIpDetail.Id;
                                ipDetailInput.LotNo = inputHu.LotNo;
                                ipDetailInput.ReceiveQty = inputHu.Qty;
                                matchIpDetail.RemainReceivedQty = matchIpDetail.RemainReceivedQty - inputHu.Qty;
                                matchIpDetail.CurrentQty = matchIpDetail.CurrentQty - inputHu.Qty;
                                matchIpDetail.Carton++;
                                List<IpDetailInput> ipDetailProcess = this.ipMaster.IpDetailInputs.ToList();
                                ipDetailProcess.Add(ipDetailInput);
                                this.ipMaster.IpDetailInputs = ipDetailProcess.ToArray();
                                needRemoveHus.Add(inputHu);
                            }

                            //再匹配物料，包装，质量状态
                            matchIpDetailsFirst = new List<IpDetail>();
                            matchIpDetailsFirst = this.ipMaster.IpDetails.Where(i => i.Item.Equals(inputHu.Item, StringComparison.OrdinalIgnoreCase) &&
                                                            i.Uom == inputHu.Uom &&
                                //i.UnitCount == inputHu.UnitCount &&
                                //i.ManufactureParty.Equals(inputHu.ManufactureParty, StringComparison.OrdinalIgnoreCase) &&
                                                            i.RemainReceivedQty >= inputHu.Qty &&
                                                            i.QualityType == inputHu.QualityType &&
                                                            i.IsChangeUnitCount == true).ToList();

                            if (matchIpDetailsFirst.Count() > 0)
                            {
                                var matchIpDetail = matchIpDetailsFirst.First();

                                var ipDetailInput = new IpDetailInput();
                                ipDetailInput.HuId = inputHu.HuId;
                                ipDetailInput.Id = matchIpDetail.Id;
                                ipDetailInput.LotNo = inputHu.LotNo;
                                ipDetailInput.ReceiveQty = inputHu.Qty;
                                matchIpDetail.RemainReceivedQty = matchIpDetail.RemainReceivedQty - inputHu.Qty;
                                matchIpDetail.CurrentQty = matchIpDetail.CurrentQty - inputHu.Qty;
                                matchIpDetail.Carton++;
                                List<IpDetailInput> ipDetailProcess = this.ipMaster.IpDetailInputs.ToList();
                                ipDetailProcess.Add(ipDetailInput);
                                this.ipMaster.IpDetailInputs = ipDetailProcess.ToArray();
                                needRemoveHus.Add(inputHu);
                            }
                        }
                        foreach (var item in needRemoveHus)
                        {
                            inputHus.Remove(item);
                        }
                    }

                    //如果还有未匹配成功的就报错
                    if (inputHus.Count > 0)
                    {
                        if (isNeedRematch == false)
                        {
                            throw new BusinessException("条码{0}不满足送货单{1}明细行的收货条件", hu.HuId, this.ipMaster.IpNo);
                        }
                        else
                        {
                            throw new BusinessException("物料{0}不满足送货单{1}明细行的收货条件", hu.Item, this.ipMaster.IpNo);
                        }
                    }
                    #endregion
                }
                this.hus.Add(hu);
            }
            #endregion

            this.gvListDataBind();
        }

        private void CheckAndMerge(OrderMaster orderMaster)
        {

            if (orderMaster.OrderStrategy == FlowStrategy.KIT && this.orderMasters.Count > 0)
            {
                throw new BusinessException("KIT单不能合并收货。");
            }

            //检查权限
            if (!Utility.HasPermission(orderMaster, base.user))
            {
                throw new BusinessException("没有此订单的权限。");
            }

            VerifyPermission(orderMaster.PartyTo);

            if (this.orderMasters == null)
            {
                this.orderMasters = new List<OrderMaster>();
            }
            if (orderMasters.Count(o => o.OrderNo == orderMaster.OrderNo) > 0)
            {
                //订单重复扫描检查
                throw new BusinessException("重复扫描订单。");
            }

            //检查订单类型
            if (orderMaster.Type == OrderType.Production)
            {
                throw new BusinessException("扫描的为生产单，不能收货。");
            }
            //else if (orderMaster.Type == OrderType.SubContract)
            //{
            //    throw new BusinessException("扫描的为委外生产单，不能收货。");
            //}

            //检查订单状态
            if (orderMaster.Status != OrderStatus.Submit
                && orderMaster.Status != OrderStatus.InProcess)
            {
                throw new BusinessException("不是Submit或InProcess状态不能收货");
            }

            //收货扫描条码 收数量不能再手持设备上做
            //if (!orderMaster.IsReceiveScanHu)
            //{
            //    throw new BusinessException("收货不用扫描条码,不能再手持终端上操作。");
            //}

            #region IsRecCreateHu
            //var createHuOption = from om in orderMasters
            //                     where om.CreateHuOption == CreateHuOption.Receive
            //                     select om.CreateHuOption;
            //if (createHuOption != null && createHuOption.Count() > 0 && createHuOption.Count() != orderMasters.Count())
            //{
            //    throw new BusinessErrorException("收货创建条码选项不同不能合并发货。");
            //}
            #endregion

            #region 订单类型
            var orderType = orderMasters.Where(o => o.Type != orderMaster.Type);
            if (orderType.Count() > 0)
            {
                throw new BusinessException("订单类型不同不能合并收货。");
            }
            #endregion

            #region 订单质量类型
            var qualityType = orderMasters.Where(o => o.QualityType != orderMaster.QualityType);
            if (qualityType.Count() > 0)
            {
                throw new BusinessException("订单质量状态不同不能合并收货。");
            }
            #endregion

            #region PartyFrom
            var partyFrom = orderMasters.Where(o => o.PartyFrom != orderMaster.PartyFrom);
            if (partyFrom.Count() > 0)
            {
                throw new BusinessException("来源组织不同不能合并收货。");
            }
            #endregion

            #region PartyTo
            var partyTo = orderMasters.Where(o => o.PartyTo != orderMaster.PartyTo);
            if (partyTo.Count() > 0)
            {
                throw new BusinessException("目的组织不同不能合并收货。");
            }
            #endregion

            #region ShipFrom
            var shipFrom = orderMasters.Where(o => o.ShipFrom != orderMaster.ShipFrom);
            if (shipFrom.Count() > 0)
            {
                throw new BusinessException("发货地址不同不能合并收货。");
            }
            #endregion

            #region ShipTo
            var shipTo = orderMasters.Where(o => o.ShipTo != orderMaster.ShipTo);
            if (shipTo.Count() > 0)
            {
                throw new BusinessException("收货地址不同不能合并收货。");
            }
            #endregion

            #region Dock
            var dock = orderMasters.Where(o => o.Dock != orderMaster.Dock);
            if (dock.Count() > 0)
            {
                throw new BusinessException("道口不同不能合并收货。");
            }
            #endregion

            #region IsAutoReceive
            //var isAutoReceive = orderMasters.Where(o => o.IsAutoReceive != orderMaster.IsAutoReceive);
            //if (isAutoReceive.Count() > 1)
            //{
            //    throw new BusinessErrorException("自动收货选项不同不能合并收货。");
            //}
            #endregion

            #region IsShipScanHu
            //var isShipScanHu = orderMasters.Where(o => o.IsShipScanHu != orderMaster.IsShipScanHu);
            //if (isShipScanHu.Count() > 1)
            //{
            //    throw new BusinessErrorException("发货扫描条码选项不同不能合并收货。");
            //}
            #endregion

            #region IsRecScanHu
            var isRecScanHu = orderMasters.Where(o => o.IsReceiveScanHu != orderMaster.IsReceiveScanHu);
            if (isRecScanHu.Count() > 0)
            {
                throw new BusinessException("收货扫描条码选项不同不能合并收货。");
            }
            #endregion

            #region IsRecExceed
            var isRecExceed = orderMasters.Where(o => o.IsReceiveExceed != orderMaster.IsReceiveExceed);
            if (isRecExceed.Count() > 0)
            {
                throw new BusinessException("允许超收选项不同不能合并收货。");
            }
            #endregion

            #region IsRecFulfillUC
            var isRecFulfillUC = orderMasters.Where(o => o.IsReceiveFulfillUC != orderMaster.IsReceiveFulfillUC);
            if (isRecFulfillUC.Count() > 0)
            {
                throw new BusinessException("收货满足包装选项不同不能合并收货。");
            }
            #endregion

            #region IsRecFifo
            var isRecFifo = orderMasters.Where(o => o.IsReceiveFifo != orderMaster.IsReceiveFifo);
            if (isRecFifo.Count() > 0)
            {
                throw new BusinessException("收货先进先出选项不同不能合并收货。");
            }
            #endregion

            #region IsAsnAuotClose
            var isAsnAuotClose = orderMasters.Where(o => o.IsAsnAutoClose != orderMaster.IsAsnAutoClose);
            if (isAsnAuotClose.Count() > 0)
            {
                throw new BusinessException("ASN自动关闭选项不同不能合并收货。");
            }
            #endregion

            #region IsAsnUniqueRec
            //var isAsnUniqueRec = orderMasters.Where(o => o.IsAsnUniqueReceive != orderMaster.IsAsnUniqueReceive);
            //if (isAsnUniqueRec.Count() > 1)
            //{
            //    throw new BusinessErrorException("ASN一次性收货选项不同不能合并收货。");
            //}
            #endregion
            if (this.flowMaster == null)
            {
                this.flowMaster = this.smartDeviceService.GetFlowMaster(orderMaster.Flow, false);
            }
            //分装生产单不能直接收货
            if (orderMaster.OrderStrategy == FlowStrategy.KIT && !string.IsNullOrEmpty(this.flowMaster.Routing))
            {
                throw new BusinessException("分装生产单不可以收货。");
            }
            //KIT单不能合并收货
            if (this.orderMasters.Count > 0)
            {
                if (orderMaster.OrderStrategy == FlowStrategy.KIT)
                {
                    throw new BusinessException("KIT单不能合并收货");
                }
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

        private void VerifyPermission(string partyTo)
        {
            if (!this.user.Permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region)
                .Select(t => t.PermissionCode).Contains(partyTo))
            {
                throw new BusinessException("没有目的区域权限不能收货。");
            }
        }

        private void CancelHu(Hu hu)
        {
            //if (this.ipMaster == null && (this.orderMasters == null || this.orderMasters.Count() == 0))
            if (this.hus == null)
            {
                //this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                this.Reset();
                return;
            }

            if (hu != null)
            {
                if (this.ipMaster == null)
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
                                var list = orderDetail.OrderDetailInputs.ToList();
                                list.Remove(q_pdi.First());
                                orderDetail.OrderDetailInputs = list.ToArray();
                                //orderDetail.OrderDetailInputs.ToList().Remove(q_pdi.First());
                                orderDetail.CurrentQty += hu.Qty;
                                orderDetail.RemainReceivedQty += hu.Qty;
                                orderDetail.Carton--;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (this.ipMaster.IpDetails == null || this.ipMaster.IpDetails.Count() == 0)
                    {
                        this.Reset();
                        throw new BusinessException("此ASN无明细");
                    }

                    //need change
                    foreach (var ipDetailInput in this.ipMaster.IpDetailInputs)
                    {
                        if (ipDetailInput.HuId == hu.HuId)
                        {
                            var ipDetail = this.ipMaster.IpDetails.Where(i => i.Id == ipDetailInput.Id).FirstOrDefault();
                            ipDetail.CurrentQty = ipDetail.CurrentQty + hu.Qty;
                            ipDetail.RemainReceivedQty += hu.Qty;
                            ipDetail.Carton--;
                            ipDetailInput.ReceiveQty -= hu.Qty;
                            if (ipDetailInput.IsOriginal == false)
                            {
                                ipDetailInput.HuId = "";
                            }
                        }
                        //if (ipDetail.IpDetailInputs != null)
                        //{
                        //    var q_pdi = ipDetail.IpDetailInputs.Where(p => p.HuId == hu.HuId);
                        //    if (q_pdi != null && q_pdi.Count() > 0)
                        //    {
                        //        ipDetail.IpDetailInputs.ToList().Remove(q_pdi.First());
                        //        ipDetail.CurrentQty += hu.Qty;
                        //        ipDetail.Carton--;
                        //        break;
                        //    }
                        //}
                    }
                }
                base.hus = base.hus.Where(h => !h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase)).ToList();
                this.gvHuListDataBind();
            }
        }
    }
}
