using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCJudgeInspect : UCBase
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private InspectMaster inspectMaster;
        private DateTime? effDate;
        //private JudgeResult judgeResult;

        public UCJudgeInspect(User user)
            : base(user)
        {
            InitializeComponent();
            base.btnOrder.Text = "判定";
        }

        protected override void ScanBarCode()
        {
            base.ScanBarCode();

            if (base.op == CodeMaster.BarCodeType.INS.ToString())
            {
                this.Reset();
                this.inspectMaster = base.smartDeviceService.GetInspect(base.barCode, true);

                //检查订单类型
                if (this.inspectMaster.Type == InspectType.Quantity)
                {
                    throw new BusinessException("不能执行数量检验单");
                }
                //检查订单状态
                if (this.inspectMaster.Status == InspectStatus.Close)
                {
                    throw new BusinessException("已关闭状态的检验单不能判定");
                }

                if (this.inspectMaster.InspectDetails == null || this.inspectMaster.InspectDetails.Length == 0)
                {
                    throw new BusinessException("没有检验单明细");
                }
                foreach (var inspectDetail in this.inspectMaster.InspectDetails)
                {
                    inspectDetail.CurrentQty = inspectDetail.InspectQty;
                }
                this.gvListDataBind();
                this.lblMessage.Text = "请输入失效代码";
            }
            else if (base.op == CodeMaster.BarCodeType.HU.ToString())
            {
                if (this.inspectMaster == null)
                {
                    throw new BusinessException("请先扫描检验单");
                }

                Hu hu = smartDeviceService.GetHu(barCode);
                //
                this.MatchHu(hu);
            }
            else if (base.op == CodeMaster.BarCodeType.DATE.ToString())
            {
                base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                this.effDate = base.smartDeviceService.GetEffDate(base.barCode);

                this.lblMessage.Text = "生效时间:" + this.effDate.Value.ToString("yyyy-MM-dd HH:mm");
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
            }
            else if (base.barCode.Length == 1)
            {
                if (this.inspectMaster == null)
                {
                    throw new BusinessException("请先扫描检验单");
                }
                this.lblMessage.Text = "请扫描物料条码";
                this.inspectMaster.FailCode = base.barCode;
                if (base.barCode == "1")
                {
                    base.btnOrder.Text = "合格";
                }
                else if (base.barCode == "2")
                {
                    base.btnOrder.Text = "报废";
                }
                else if (base.barCode == "3")
                {
                    base.btnOrder.Text = "退货";
                }
                else if (base.barCode == "4")
                {
                    base.btnOrder.Text = "返工";
                }
                else if (base.barCode == "5")
                {
                    base.btnOrder.Text = "让步";
                }
                else
                {
                    this.inspectMaster.FailCode = null;
                    throw new BusinessException("输入不合法");
                }
            }
            else
            {
                throw new BusinessException("条码格式不合法");
            }
        }

        protected override void gvListDataBind()
        {
            base.gvListDataBind();
            List<InspectDetail> inspectDetailList = new List<InspectDetail>();
            if (this.inspectMaster != null && this.inspectMaster.InspectDetails != null)
            {
                inspectDetailList = this.inspectMaster.InspectDetails.Where(i => i.CurrentQty > 0).ToList();
            }
            base.dgList.DataSource = inspectDetailList;
            base.ts.MappingName = inspectDetailList.GetType().Name;
        }

        protected override void Reset()
        {
            this.inspectMaster = null;
            base.Reset();
            this.lblMessage.Text = "请扫描检验单";
            this.effDate = null;
        }

        protected override void DoSubmit()
        {
            //this.inspectMaster.JudgeResult = judgeResult;
            if (this.inspectMaster == null || this.inspectMaster.InspectDetails == null || this.inspectMaster.InspectDetails.Count() == 0)
            {
                throw new BusinessException("请扫描检验单");
            }
            if (string.IsNullOrEmpty(this.inspectMaster.FailCode))
            {
                throw new BusinessException("请输入失效代码:1合格,2报废,3退货,4返工,5让步");
            }

            base.smartDeviceService.DoJudgeInspect(this.inspectMaster, base.hus.Select(h => h.HuId).ToArray(), this.effDate, this.user.Code);
            this.Reset();
            this.lblMessage.Text = "判定成功";
            this.btnOrder.Text = "判断";
            this.isMark = true;
        }

        private void MatchHu(Hu hu)
        {
            if (hu == null)
            {
                throw new BusinessException("条码不存在");
            }
            hu.CurrentQty = hu.Qty;

            var matchHu = this.hus.Where(h => h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));

            if (this.isCancel)
            {
                if (matchHu == null || matchHu.Count() == 0)
                {
                    throw new BusinessException("没有需要取消匹配条码:{0}", hu.HuId);
                }
                else if (matchHu.Count() == 1)
                {
                    //var _hu = _hus.Single();
                    //this.hus.Remove(_hu);
                }
                else
                {
                    throw new Exception("匹配了多个条码");
                }
            }
            else
            {
                if (matchHu != null && matchHu.Count() > 0)
                {
                    throw new BusinessException("条码重复扫描!");
                }
            }

            if (this.hus == null)
            {
                this.hus = new List<Hu>();
            }
            if (hu.IsFreeze)
            {
                throw new BusinessException("条码被冻结!");
            }
            if (hu.OccupyReferenceNo != this.inspectMaster.InspectNo)
            {
                throw new BusinessException("条码被{0}占用!", hu.OccupyReferenceNo);
            }

            if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
            {
                throw new BusinessException("没有此条码的权限");
            }

            if (!base.isCancel)
            {
                if (hu.Status != HuStatus.Location)
                {
                    throw new BusinessException("条码不在库位中不能检验判定");
                }
                if (hu.QualityType != QualityType.Inspect)
                {
                    throw new BusinessException("以判定过的条码不能再检验判定");
                }

                var matchedOrderDetails = this.inspectMaster.InspectDetails.Where(i => hu.HuId.Equals(i.HuId, StringComparison.OrdinalIgnoreCase));
                if (matchedOrderDetails == null || matchedOrderDetails.Count() == 0)
                {
                    throw new BusinessException("没有匹配的条码");
                }
                var matchedOrderDetail = matchedOrderDetails.First();

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

        protected override Hu DoCancel()
        {
            Hu firstHu = base.DoCancel();
            this.CancelHu(firstHu);
            return firstHu;
        }

        private void CancelHu(Hu hu)
        {
            if (this.hus == null)
            {
                //this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                this.Reset();
                return;
            }

            if (hu != null)
            {
                var matchedOrderDetails = this.inspectMaster.InspectDetails.Where(i => hu.HuId.Equals(i.HuId, StringComparison.OrdinalIgnoreCase));
                if (matchedOrderDetails == null || matchedOrderDetails.Count() == 0)
                {
                    throw new BusinessException("没有需要取消的匹配条码");
                }
                else
                {
                    var matchedOrderDetail = matchedOrderDetails.First();
                    matchedOrderDetail.CurrentQty += hu.Qty;
                    matchedOrderDetail.Carton--;
                }
                base.hus = base.hus.Where(h => !h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase)).ToList();
                this.gvHuListDataBind();
            }
        }
    }
}
