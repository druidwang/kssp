using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCReturn : UCBase
    {
        private FlowMaster flowMaster;
        private DateTime? effDate;

        private static UCReturn ucReturn;
        private static object obj = new object();

        private UCReturn(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.btnOrder.Text = "退货";
        }

        public static UCReturn GetUCReturn(User user)
        {
            if (ucReturn == null)
            {
                lock (obj)
                {
                    if (ucReturn == null)
                    {
                        ucReturn = new UCReturn(user);
                    }
                }
            }
            ucReturn.user = user;
            ucReturn.Reset();
            ucReturn.lblMessage.Text = "请先扫描路线";
            return ucReturn;
        }

        protected override void ScanBarCode()
        {
            base.ScanBarCode();

            if (this.flowMaster == null)
            {
                if (base.op == CodeMaster.BarCodeType.F.ToString())
                {
                    base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                    var flowMaster = smartDeviceService.GetFlowMaster(base.barCode, true);

                    //检查订单类型
                    if (flowMaster.Type != OrderType.Transfer || flowMaster.Type != OrderType.SubContractTransfer)
                    {
                        throw new BusinessException("不是移库路线。");
                    }

                    //是否有效
                    if (!flowMaster.IsActive)
                    {
                        throw new BusinessException("此移库路线无效。");
                    }

                    //检查权限
                    if (!Utility.HasPermission(flowMaster, base.user))
                    {
                        throw new BusinessException("没有此移库路线的权限");
                    }
                    this.flowMaster = flowMaster;
                    base.lblMessage.Text = this.flowMaster.Description;
                    this.gvListDataBind();
                }
                else
                {
                    throw new BusinessException("输入的条码类型不正确。");
                }
            }
            else
            {
                if (base.op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if (this.flowMaster == null)
                    {
                        throw new BusinessException("请先扫描物流路线");
                    }
                    Hu hu = smartDeviceService.GetHu(barCode);
                    this.MatchHu(hu);
                }
                else if (base.op == CodeMaster.BarCodeType.DATE.ToString())
                {
                    //todo 权限校验
                    base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                    this.effDate = base.smartDeviceService.GetEffDate(base.barCode);

                    this.lblMessage.Text = "生效时间:" + this.effDate.Value.ToString("yyyy-MM-dd HH:mm");
                    this.tbBarCode.Text = string.Empty;
                    this.tbBarCode.Focus();
                    this.flowMaster.EffectiveDate = effDate;
                }
                else
                {
                    throw new BusinessException("条码格式不合法");
                }
            }
        }

        protected override void gvListDataBind()
        {
            base.columnIsOdd.Width = 0;
            base.columnLotNo.Width = 0;
            base.gvListDataBind();
            List<FlowDetail> flowDetails = new List<FlowDetail>();
            if (this.flowMaster != null && this.flowMaster.FlowDetails != null)
            {
                flowDetails = this.flowMaster.FlowDetails.Where(f => f.CurrentQty > 0).ToList();
            }
            base.dgList.DataSource = flowDetails;
            base.ts.MappingName = flowDetails.GetType().Name;
        }


        protected override void Reset()
        {
            this.flowMaster = null;
            base.Reset();
            this.lblMessage.Text = "请先扫描物流路线";
            this.effDate = null;
        }

        protected override void DoSubmit()
        {
            try
            {
                if (this.flowMaster == null)
                {
                    throw new BusinessException("请先扫描路线，库位或者库格。");
                }
                //List<OrderDetailInput> orderDetailInputList = new List<OrderDetailInput>();

                //if (this.flowMaster.FlowDetails != null)
                //{
                //    foreach (var flowDetail in this.flowMaster.FlowDetails)
                //    {
                //        if (flowDetail.FlowDetailInputs != null)
                //        {
                //            foreach (var fdi in flowDetail.FlowDetailInputs)
                //            {
                //                OrderDetailInput orderDetailInput = new OrderDetailInput();
                //                orderDetailInput.HuId = fdi.HuId;
                //                orderDetailInput.Qty = fdi.Qty;
                //                orderDetailInput.LotNo = fdi.LotNo;
                //                orderDetailInput.Id = flowDetail.Id;
                //                orderDetailInput.Direction = fdi.Direction;

                //                orderDetailInputList.Add(orderDetailInput);
                //            }
                //        }
                //    }
                //}
                //this.smartDeviceService.DoTransfer(flowMaster, orderDetailInputList.ToArray(), base.user.Code);
                this.smartDeviceService.DoReturnOrder(this.flowMaster.Code, hus.Select(h => h.HuId).ToArray(), this.effDate, this.user.Code);
                this.Reset();
                base.lblMessage.Text = "退货成功";
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
                if (hu.Status != HuStatus.Location)
                {
                    //throw new BusinessException("条码不在库位中");
                }

                if (hu.QualityType == QualityType.Inspect)
                {
                    throw new BusinessException("待验条码不能移库");
                }

                if (this.hus.Count > 0 && hu.QualityType != this.hus[0].QualityType)
                {
                    throw new BusinessException("质量状态不一致的条码不能混合移库");
                }

                FlowDetail matchedFlowDetail = new FlowDetail();
                var flowDetails = this.flowMaster.FlowDetails;

                #region 物料匹配
                if (flowMaster.IsManualCreateDetail)
                {
                    if (flowDetails != null)
                    {
                        var q = flowDetails.Where(f => f.Item.Equals(hu.Item, StringComparison.OrdinalIgnoreCase));
                        if (q.Count() > 0)
                        {
                            matchedFlowDetail = q.First();
                            //matchedFlowDetail.CurrentQty += hu.Qty;
                            //matchedFlowDetail.Carton++;
                        }
                        else
                        {
                            List<FlowDetail> flowDetailList = flowDetails.ToList();
                            if (flowMaster.FlowDetails != null)
                            {
                                flowDetailList = flowMaster.FlowDetails.ToList();
                            }
                            matchedFlowDetail = this.Hu2FlowDetail(flowMaster, hu);
                            flowDetailList.Add(matchedFlowDetail);
                            flowMaster.FlowDetails = flowDetailList.ToArray();
                        }
                    }
                    else
                    {
                        List<FlowDetail> flowDetailList = new List<FlowDetail>();
                        if (flowMaster.FlowDetails != null)
                        {
                            flowDetailList = flowMaster.FlowDetails.ToList();
                        }
                        matchedFlowDetail = this.Hu2FlowDetail(flowMaster, hu);
                        flowDetailList.Add(matchedFlowDetail);
                        flowMaster.FlowDetails = flowDetailList.ToArray();
                    }
                }
                else
                {
                    if (this.flowMaster.FlowDetails == null)
                    {
                        throw new BusinessException("没有找到和条码{0}的物料号{1}匹配的订单明细。", hu.HuId, hu.Item);
                    }

                    var matchedOrderDetailList = flowDetails.Where(o => o.Item == hu.Item);
                    if (matchedOrderDetailList == null || matchedOrderDetailList.Count() == 0)
                    {
                        throw new BusinessException("没有找到和条码{0}的物料号{1}匹配的订单明细。", hu.HuId, hu.Item);
                    }

                    matchedOrderDetailList = matchedOrderDetailList.Where(o => o.Uom.Equals(hu.Uom, StringComparison.OrdinalIgnoreCase));
                    if (matchedOrderDetailList == null || matchedOrderDetailList.Count() == 0)
                    {
                        throw new BusinessException("没有找到和条码{0}的单位{1}匹配的订单明细。", hu.HuId, hu.Uom);
                    }

                    if (this.flowMaster.IsOrderFulfillUC)
                    {
                        matchedOrderDetailList = matchedOrderDetailList.Where(o => o.UnitCount == hu.UnitCount);
                        if (matchedOrderDetailList == null || matchedOrderDetailList.Count() == 0)
                        {
                            throw new BusinessException("没有找到和条码{0}的包装数{1}匹配的订单明细。", hu.HuId, hu.UnitCount.ToString());
                        }
                    }
                    matchedFlowDetail = matchedOrderDetailList.First();
                }
                #endregion

                FlowDetailInput input = new FlowDetailInput();
                input.HuId = hu.HuId;
                input.Qty = hu.Qty;
                input.LotNo = hu.LotNo;
                input.Direction = hu.Direction;

                List<FlowDetailInput> flowDetailInputs = new List<FlowDetailInput>();
                if (matchedFlowDetail.FlowDetailInputs != null)
                {
                    flowDetailInputs = matchedFlowDetail.FlowDetailInputs.ToList();
                }
                flowDetailInputs.Add(input);
                matchedFlowDetail.FlowDetailInputs = flowDetailInputs.ToArray();
                matchedFlowDetail.CurrentQty += hu.Qty;
                matchedFlowDetail.Carton++;
                //
                base.hus.Insert(0, hu);
            }
            else
            {
                this.CancelHu(hu);
            }
            this.gvListDataBind();
        }


        private FlowDetail Hu2FlowDetail(FlowMaster flowMaster, Hu hu)
        {
            //int seq = 10;
            //if (flowMaster.FlowDetails != null)
            //{
            //    seq = flowMaster.FlowDetails.Max(f => f.Sequence) + 10;
            //}

            FlowDetail flowDetail = new FlowDetail();
            flowDetail.Id = this.FindMinId() - 1;
            //flowDetail.CurrentQty = hu.UnitCount;
            flowDetail.Flow = flowMaster.Code;
            flowDetail.Item = hu.Item;
            flowDetail.LocationFrom = flowMaster.LocationFrom;
            flowDetail.LocationTo = flowMaster.LocationTo;
            flowDetail.ReferenceItemCode = hu.ReferenceItemCode;
            //flowDetail.Sequence = seq;
            flowDetail.UnitCount = hu.UnitCount;
            flowDetail.Uom = hu.Uom;

            return flowDetail;
        }

        protected override Hu DoCancel()
        {
            Hu firstHu = base.DoCancel();
            this.CancelHu(firstHu);
            return firstHu;
        }

        protected void CancelHu(Hu hu)
        {
            //if (this.flowMaster == null || this.flowMaster.FlowDetails == null || this.flowMaster.FlowDetails.Count() == 0)
            if (this.hus == null)
            {
                //this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                this.Reset();
                return;
            }

            if (hu != null)
            {
                foreach (var flowDetail in this.flowMaster.FlowDetails)
                {
                    if (flowDetail.FlowDetailInputs != null)
                    {
                        var q_pdi = flowDetail.FlowDetailInputs.Where(p => p.HuId == hu.HuId);
                        if (q_pdi != null && q_pdi.Count() > 0)
                        {
                            var list = flowDetail.FlowDetailInputs.ToList();
                            list.Remove(q_pdi.First());
                            flowDetail.FlowDetailInputs = list.ToArray();
                            //flowDetail.FlowDetailInputs.ToList().Remove(q_pdi.First());
                            flowDetail.CurrentQty -= hu.Qty;
                            flowDetail.Carton--;
                            break;
                        }
                    }
                }
                base.hus = base.hus.Where(h => !h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase)).ToList();
                this.gvHuListDataBind();
            }
        }

        private int FindMinId()
        {
            if (this.flowMaster.FlowDetails != null && this.flowMaster.FlowDetails.Length > 0)
            {
                return this.flowMaster.FlowDetails.Min(f => f.Id);
            }
            return 0;
        }
    }
}
