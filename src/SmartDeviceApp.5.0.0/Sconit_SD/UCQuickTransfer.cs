using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCQuickTransfer : UCBase
    {
        private FlowMaster flowMaster;
        private DateTime? effDate;


        private static UCQuickTransfer ucQuickTransfer;
        private static object obj = new object();

        public UCQuickTransfer(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.btnOrder.Text = "移库";
       
        }

        public static UCQuickTransfer GetUCQuickTransfer(User user)
        {
            if (ucQuickTransfer == null)
            {
                lock (obj)
                {
                    if (ucQuickTransfer == null)
                    {
                        ucQuickTransfer = new UCQuickTransfer(user);
                    }
                }
            }
            ucQuickTransfer.user = user;
            ucQuickTransfer.Reset();
            ucQuickTransfer.lblMessage.Text = "请先扫描路线";
            return ucQuickTransfer;
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

                    //检查路线类型
                    if (flowMaster.Type != OrderType.Transfer && flowMaster.Type != OrderType.SubContractTransfer)
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
                else if (base.op == CodeMaster.BarCodeType.B.ToString())
                {
                    if (!this.user.Permissions.Select(p => p.PermissionCode)
                        .Contains(CodeMaster.TerminalPermission.Client_Location_Bin_Transfer.ToString()))
                    {
                        throw new BusinessException("没按库位/库格的移库权限");
                    }
                    base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                    Bin bin = smartDeviceService.GetBin(base.barCode);
                    //检查权限
                    if (!Utility.HasPermission(user.Permissions, OrderType.Transfer, false, true, null, bin.Region))
                    {
                        throw new BusinessException("没有此移库路线的权限");
                    }
                    this.flowMaster = new FlowMaster();
                    this.flowMaster.PartyTo = bin.Region;
                    this.flowMaster.LocationTo = bin.Location;
                    this.flowMaster.Bin = bin.Code;

                    this.lblMessage.Text = "目的库格:" + bin.Code;
                }
                else if (base.op == CodeMaster.BarCodeType.L.ToString())
                {
                    if (!this.user.Permissions.Select(p => p.PermissionCode)
                        .Contains(CodeMaster.TerminalPermission.Client_Location_Bin_Transfer.ToString()))
                    {
                        throw new BusinessException("没按库位/库格的移库权限");
                    }
                    base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                    Location location = smartDeviceService.GetLocation(base.barCode);

                    //检查权限
                    if (!Utility.HasPermission(user.Permissions, OrderType.Transfer, false, true, null, location.Region))
                    {
                        throw new BusinessException("没有此移库路线的权限");
                    }
                    this.flowMaster = new FlowMaster();
                    this.flowMaster.PartyTo = location.Region;
                    this.flowMaster.LocationTo = location.Code;
                    this.lblMessage.Text = "目的库位:" + location.Code;
                }
                else
                {
                    throw new BusinessException("输入的库位或路线条码不正确。");
                }
            }
            else
            {
                if (base.op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if (this.flowMaster == null)
                    {
                        throw new BusinessException("请先扫描路线或库位或库格");
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
                else if (base.op == CodeMaster.BarCodeType.B.ToString())
                {
                    base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                    Bin bin = smartDeviceService.GetBin(base.barCode);
                    //检查权限
                    if (!Utility.HasPermission(user.Permissions, OrderType.Transfer, false, true, null, bin.Region))
                    {
                        throw new BusinessException("没有此移库路线的权限");
                    }
                    this.flowMaster.Bin = bin.Code;

                    this.lblMessage.Text = "目的库格:" + bin.Code;
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
            this.lblMessage.Text = "请先扫描路线或库位或库格";
            this.effDate = null;
        }

        protected override void DoSubmit()
        {
            try
            {
                if (this.flowMaster == null)
                {
                    this.isMark = true;
                    this.tbBarCode.Focus();
                    return;
                    //throw new BusinessException("请先扫描路线，库位或者库格。");
                }
                List<FlowDetailInput> flowDetailInputList = new List<FlowDetailInput>();

                if (this.flowMaster.FlowDetails != null)
                {
                    foreach (var flowDetail in this.flowMaster.FlowDetails)
                    {
                        if (flowDetail.FlowDetailInputs != null)
                        {
                            flowDetailInputList.AddRange(flowDetail.FlowDetailInputs);
                        }
                    }
                }
                if (flowDetailInputList.Count == 0)
                {
                    this.isMark = true;
                    this.tbBarCode.Focus();
                    return;
                }
                this.smartDeviceService.DoTransfer(flowMaster, flowDetailInputList.ToArray(), base.user.Code,true,false);
                flowMaster.FlowDetails = null;
                this.Reset();
                base.lblMessage.Text = "车间发料成功,共" + flowDetailInputList.Count + "箱";
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

                if (hu.Location == this.flowMaster.LocationTo)
                {
                    throw new BusinessException(string.Format("此条码已在库位{0}中,无需移库", hu.Location));
                }


                FlowDetail matchedFlowDetail = new FlowDetail();
                var flowDetails = this.flowMaster.FlowDetails;

                if (string.IsNullOrEmpty(this.flowMaster.Code))
                {
                    if (flowDetails == null)
                    {
                        flowDetails = new List<FlowDetail>().ToArray();
                        this.flowMaster.LocationFrom = hu.Location;
                        this.flowMaster.PartyFrom = hu.Region;
                    }
                    else
                    {
                        if (!this.flowMaster.PartyFrom.Equals(hu.Region, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new BusinessException("当前条码{0}的来源区域{1}与其他物料的来源区域{2}不一致。", hu.HuId, hu.Region, this.flowMaster.PartyFrom);
                        }
                    }

                    var q = flowDetails
                        .Where(f => f.Item.Equals(hu.Item, StringComparison.OrdinalIgnoreCase)
                            && f.Uom.Equals(hu.Uom, StringComparison.OrdinalIgnoreCase)
                            && f.UnitCount == hu.UnitCount && f.LocationFrom == hu.Location);
                    if (q.Count() > 0)
                    {
                        matchedFlowDetail = q.Single();
                    }
                    else
                    {
                        //检查权限
                        if (!Utility.HasPermission(user.Permissions, OrderType.Transfer, false, true, null, hu.Region))
                        {
                            throw new BusinessException("没有此移库路线的权限");
                        }
                        matchedFlowDetail.Id = FindMinId() - 1;
                        matchedFlowDetail.Item = hu.Item;
                        //matchedFlowDetail.Sequence++;
                        matchedFlowDetail.ReferenceItemCode = hu.ReferenceItemCode;
                        matchedFlowDetail.UnitCount = hu.UnitCount;
                        matchedFlowDetail.Uom = hu.Uom;
                        matchedFlowDetail.LocationFrom = hu.Location;

                        var flowDetailList = new List<FlowDetail>();
                        if (this.flowMaster.FlowDetails != null)
                        {
                            flowDetailList = this.flowMaster.FlowDetails.ToList();
                        }
                        flowDetailList.Add(matchedFlowDetail);
                        this.flowMaster.FlowDetails = flowDetailList.ToArray();
                    }
                }
                else
                {
                    #region 物料匹配

                    if (flowMaster.IsManualCreateDetail)
                    {
                        if (flowDetails != null)
                        {
                            var q = flowDetails.Where(f => f.Item.Equals(hu.Item, StringComparison.OrdinalIgnoreCase)
                                && f.LocationFrom.Equals(hu.Location, StringComparison.OrdinalIgnoreCase));
                            if (q.Count() > 0)
                            {
                                matchedFlowDetail = q.First();
                            }
                            else
                            {
                                if (hu.Location != this.flowMaster.LocationFrom)
                                {
                                    throw new BusinessException(string.Format("此条码不在库位{0}中,不能移库", this.flowMaster.LocationFrom));
                                }

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
                            if (hu.Location != this.flowMaster.LocationFrom)
                            {
                                throw new BusinessException(string.Format("此条码不在库位{0}中,不能移库", this.flowMaster.LocationFrom));
                            }
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
                            throw new BusinessException("没有找到和条码{0}的物料号{1}匹配的路线明细。", hu.HuId, hu.Item);
                        }

                        var matchedFlowDetailList = flowDetails.Where(o => o.Item == hu.Item);
                        if (matchedFlowDetailList == null || matchedFlowDetailList.Count() == 0)
                        {
                            throw new BusinessException("没有找到和条码{0}的物料号{1}匹配的路线明细。", hu.HuId, hu.Item);
                        }

                        matchedFlowDetailList = matchedFlowDetailList.Where(o => o.Uom.Equals(hu.Uom, StringComparison.OrdinalIgnoreCase));
                        if (matchedFlowDetailList == null || matchedFlowDetailList.Count() == 0)
                        {
                            throw new BusinessException("没有找到和条码{0}的单位{1}匹配的路线明细。", hu.HuId, hu.Uom);
                        }
                        matchedFlowDetailList = matchedFlowDetailList.Where(o => o.LocationFrom.Equals(hu.Location, StringComparison.OrdinalIgnoreCase));
                        if (matchedFlowDetailList == null || matchedFlowDetailList.Count() == 0)
                        {
                            throw new BusinessException("没有找到和条码{0}的库位{1}匹配的路线明细。", hu.HuId, hu.Location);
                        }

                        if (this.flowMaster.IsOrderFulfillUC)
                        {
                            matchedFlowDetailList = matchedFlowDetailList.Where(o => o.UnitCount == hu.UnitCount);
                            if (matchedFlowDetailList == null || matchedFlowDetailList.Count() == 0)
                            {
                                throw new BusinessException("没有找到和条码{0}的包装数{1}匹配的路线明细。", hu.HuId, hu.UnitCount.ToString());
                            }
                        }
                        matchedFlowDetail = matchedFlowDetailList.First();
                    }
                    #endregion
                }

                List<FlowDetailInput> flowDetailInputs = new List<FlowDetailInput>();
                if (matchedFlowDetail.FlowDetailInputs != null)
                {
                    flowDetailInputs = matchedFlowDetail.FlowDetailInputs.ToList();
                }

                FlowDetailInput input = new FlowDetailInput();
                input.HuId = hu.HuId;
                input.Qty = hu.Qty;
                input.LotNo = hu.LotNo;
                input.Direction = hu.Direction;
                input.Item = hu.Item;
                input.Uom = hu.Uom;
                input.QualityType = hu.QualityType;
                input.LocFrom = hu.Location;

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
            if (this.hus == null || this.hus.Count() == 0)
            {
                //this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                this.Reset();
                return;
            }
            else
            {
                if (hu == null)
                {
                    hu = base.hus.First();
                }
                foreach (var flowDetail in this.flowMaster.FlowDetails)
                {
                    if (flowDetail.FlowDetailInputs != null)
                    {
                        var q_pdi = flowDetail.FlowDetailInputs
                            .FirstOrDefault(p => p.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));
                        if (q_pdi != null)
                        {
                            flowDetail.FlowDetailInputs = flowDetail.FlowDetailInputs
                                .Where(p => !p.HuId.Equals(q_pdi.HuId, StringComparison.OrdinalIgnoreCase)).ToArray();
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
