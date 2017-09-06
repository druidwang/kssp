using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCPickList : UCBase
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        protected PickListMaster pickListMaster;

        public UCPickList(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.btnOrder.Text = "拣货";
        }

        protected override void ScanBarCode()
        {
            base.ScanBarCode();
            if (this.pickListMaster == null)
            {
                if (base.op == CodeMaster.BarCodeType.PIK.ToString())
                {
                    var pickListMaster = smartDeviceService.GetPickList(this.barCode, true);

                    if (pickListMaster.PickListDetails == null || pickListMaster.PickListDetails.Count() == 0)
                    {
                        throw new BusinessException("此拣货单没有明细");
                    }
                    pickListMaster.PickListDetails = pickListMaster.PickListDetails.Where(p => p.IsInventory == true).ToArray();
                    if (pickListMaster.PickListDetails.Count() == 0)
                    {
                        throw new BusinessException("此拣货单没有明细");
                    }
                    if (pickListMaster.Status == PickListStatus.Cancel)
                    {
                        throw new BusinessException("此拣货单已经取消,不能拣货");
                    }
                    if (pickListMaster.Status == PickListStatus.Close)
                    {
                        throw new BusinessException("此拣货单已经关闭,不能拣货");
                    }
                    //检查权限
                    if (!Utility.HasPermission(pickListMaster, this.user))
                    {
                        throw new BusinessException("没有此拣货单的权限");
                    }
                    //foreach (var pickListDetail in pickListMaster.PickListDetails)
                    //{
                    //    pickListDetail.CurrentQty = pickListDetail.Qty;
                    //}
                    this.lblMessage.Text = "请扫描待拣的条码";
                    this.pickListMaster = pickListMaster;
                    this.gvListDataBind();
                }
                else
                {
                    throw new BusinessException("请先扫描拣货单。");
                }
            }
            else
            {
                if (base.op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if (this.pickListMaster == null || this.pickListMaster.PickListDetails == null)
                    {
                        throw new BusinessException("请先扫描拣货单");
                    }
                    Hu hu = smartDeviceService.GetHu(barCode);
                    this.MatchHu(hu);
                }
                else
                {
                    throw new BusinessException("条码格式不合法");
                }
            }
        }

        protected override void gvListDataBind()
        {
            base.gvListDataBind();

            List<PickListDetail> pickListDetails = new List<PickListDetail>();
            if (this.pickListMaster != null && this.pickListMaster.PickListDetails != null)
            {
                pickListDetails = this.pickListMaster.PickListDetails.Where(p => p.CurrentQty > 0).ToList();
            }
            this.dgList.DataSource = pickListDetails;
            this.ts.MappingName = pickListDetails.GetType().Name;
        }

        protected override void Reset()
        {
            this.pickListMaster = null;
            base.Reset();
            this.lblMessage.Text = "请扫描拣货单";
            this.tbBarCode.Focus();
        }

        protected override void DoSubmit()
        {
            try
            {
                if (this.pickListMaster == null)
                {
                    this.Reset();
                    return;
                    throw new BusinessException("请先扫描拣货单。");
                }

                List<PickListDetailInput> pickListDetailInputList = new List<PickListDetailInput>();

                if (this.pickListMaster.PickListDetails != null)
                {
                    foreach (var pickListDetail in this.pickListMaster.PickListDetails)
                    {
                        if (pickListDetail.PickListDetailInputs != null)
                        {
                            pickListDetailInputList.AddRange(pickListDetail.PickListDetailInputs);
                        }
                    }
                }
                if (pickListDetailInputList.Count == 0)
                {
                    throw new BusinessException("没有扫描条码");
                }

                this.smartDeviceService.DoPickList(pickListDetailInputList.Distinct().ToArray(), this.user.Code);

                this.Reset();
                this.lblMessage.Text = "拣货成功";
                this.isMark = true;
            }
            catch (Exception ex)
            {
                Utility.ShowMessageBox(ex.Message);
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                return;
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
                #region 物料匹配
                if (hu.Status != HuStatus.Location)
                {
                    throw new BusinessException("条码{0}不在库存中,不能拣货", hu.HuId);
                }
                var pickListDetails = this.pickListMaster.PickListDetails;

                var pickListDetailList = pickListDetails.Where(o => o.Item == hu.Item || (o.ItemDisconList != null && o.ItemDisconList.Contains(hu.Item)));
                if (pickListDetailList == null || pickListDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的物料号{1}匹配的拣货单明细。", hu.HuId, hu.Item);
                }

                pickListDetailList = pickListDetailList.Where(o => o.Uom.Equals(hu.Uom, StringComparison.OrdinalIgnoreCase));
                if (pickListDetailList == null || pickListDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的单位{1}匹配的拣货单明细。", hu.HuId, hu.Uom);
                }

                //pickListDetailList = pickListDetailList.Where(o => o.UnitCount == hu.UnitCount || o.UnitCount == 1);
                //if (pickListDetailList == null || pickListDetailList.Count() == 0)
                //{
                //    throw new BusinessException("没有找到和条码{0}的包装数{1}匹配的拣货单明细。", hu.HuId, hu.UnitCount.ToString("0.###"));
                //}

                //强制拣货不需要严格限制批次号
                pickListDetailList = pickListDetailList.Where(o => o.LotNo == hu.LotNo);
                if (pickListDetailList == null || pickListDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的批号{1}匹配的拣货单明细。", hu.HuId, hu.LotNo.ToString());
                }

                //pickListDetailList = pickListDetailList.Where(o => o.ManufactureParty == hu.ManufactureParty);
                //if (pickListDetailList == null || pickListDetailList.Count() == 0)
                //{
                //    throw new BusinessException("没有找到和条码{0}的供应商{1}匹配的拣货单明细。", hu.HuId, hu.ManufactureParty);
                //}

                //pickListDetailList = pickListDetailList.Where(o => o.IsOdd == hu.IsOdd);
                //if (pickListDetailList == null || pickListDetailList.Count() == 0)
                //{
                //    throw new BusinessException("没有找到匹配条码{0}{1}零头的拣货单明细。", hu.HuId, hu.IsOdd ? "是" : "不是");
                //}

                pickListDetailList = pickListDetailList.Where(o => o.QualityType == hu.QualityType);
                if (pickListDetailList == null || pickListDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的质量状态{1}匹配的拣货单明细。", hu.HuId, hu.QualityType.ToString());
                }
                pickListDetailList = pickListDetailList.Where(o => !o.IsMatchDirection ||
                    (o.Direction == hu.Direction || (string.IsNullOrEmpty(o.Direction) && string.IsNullOrEmpty(hu.Direction))));
                if (pickListDetailList == null || pickListDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的方向{1}匹配的拣货单明细。", hu.HuId, hu.Direction);
                }
                pickListDetailList = pickListDetailList.Where(o => o.Bin == hu.Bin);
                if (pickListDetailList == null || pickListDetailList.Count() == 0)
                {
                    throw new BusinessException("没有找到和条码{0}的库格{1}匹配的拣货单明细。", hu.HuId, hu.Bin);
                }
                if (hu.HuOption == HuOption.UnAging)
                {
                    pickListDetailList = pickListDetailList.Where(o => (o.LocationTo == hu.AgingLocation));
                    if (pickListDetailList == null || pickListDetailList.Count() == 0)
                    {
                        throw new BusinessException("条码{0}未老化,不能拣货。", hu.HuId);
                    }
                }

                pickListDetailList = pickListDetailList
                    .Where(p => p.CurrentQty > 0)
                    .Where(p => p.UcDeviation < 0 || (p.CurrentQty > p.UnitCount * (p.UcDeviation / 100)))
                    .OrderByDescending(p => p.CurrentQty);
                var matchedpickListDetail = pickListDetailList.Where(p => p.UnitCount == hu.Qty).FirstOrDefault();
                if (matchedpickListDetail == null)
                {
                    matchedpickListDetail = pickListDetailList.FirstOrDefault();
                }

                if (matchedpickListDetail == null)
                {
                    throw new BusinessException("没有找到和条码{0}相匹配的拣货单明细。", hu.HuId);
                }

                #endregion

                PickListDetailInput input = new PickListDetailInput();
                input.HuId = hu.HuId;
                input.Id = matchedpickListDetail.Id;

                List<PickListDetailInput> pickListDetailInputs = new List<PickListDetailInput>();
                if (matchedpickListDetail.PickListDetailInputs != null)
                {
                    pickListDetailInputs = matchedpickListDetail.PickListDetailInputs.ToList();
                }
                pickListDetailInputs.Add(input);
                matchedpickListDetail.PickListDetailInputs = pickListDetailInputs.ToArray();
                matchedpickListDetail.CurrentQty -= hu.Qty;
                matchedpickListDetail.Carton++;
                this.hus.Insert(0, hu);
            }
            else
            {
                DoCancel();
            }
            this.gvListDataBind();
        }

        protected override Hu DoCancel()
        {
            Hu firstHu = base.DoCancel();
            this.CancelHu(firstHu);
            return firstHu;
        }

        protected void CancelHu(Hu hu)
        {
            //if (this.pickListMaster == null || this.pickListMaster.PickListDetails == null || this.pickListMaster.PickListDetails.Count() == 0)
            if (this.hus == null)
            {
                //this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                this.Reset();
                return;
            }
            else
            {
                foreach (var pickListDetail in this.pickListMaster.PickListDetails)
                {
                    if (pickListDetail.PickListDetailInputs != null)
                    {
                        var q_pdi = pickListDetail.PickListDetailInputs.Where(p => p.HuId == hu.HuId);
                        if (q_pdi != null && q_pdi.Count() > 0)
                        {
                            var list = pickListDetail.PickListDetailInputs.ToList();
                            list.Remove(q_pdi.First());
                            pickListDetail.PickListDetailInputs = list.ToArray();
                            //pickListDetail.PickListDetailInputs.ToList().Remove(q_pdi.First());
                            pickListDetail.CurrentQty += hu.Qty;
                            pickListDetail.Carton--;
                            break;
                        }
                    }
                }
                this.hus.Remove(hu);
                this.gvHuListDataBind();
            }
        }
    }
}
