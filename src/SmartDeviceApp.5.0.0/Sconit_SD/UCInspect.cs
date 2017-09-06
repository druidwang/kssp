using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCInspect : UCPack
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        public UCInspect(User user)
            : base(user)
        {
            InitializeComponent();
            base.btnOrder.Text = "检验";
        }

        protected override void ScanBarCode()
        {
            base.barCode = this.tbBarCode.Text.Trim();
            this.lblMessage.Text = string.Empty;
            if (this.barCode.Length < 5)
            {
                throw new BusinessException("条码格式不合法");
            }
            base.op = Utility.GetBarCodeType(base.user.BarCodeTypes, base.barCode);

            if (base.op == CodeMaster.BarCodeType.HU.ToString())
            {
                Hu hu = smartDeviceService.GetHu(barCode);
                if (hu.Status != HuStatus.Location)
                {
                    throw new BusinessException("条码不在库位中不能检验");
                }
                if (base.hus != null & base.hus.Count > 0)
                {
                    if (base.hus.First().Region != hu.Region)
                    {
                        throw new BusinessException("条码不在同一区域中不能检验");
                    }
                }
                if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
                {
                    throw new BusinessException("没有此条码的权限");
                }
                base.MatchHu(hu);
            }
            else
            {
                throw new BusinessException("条码格式不合法");
            }
        }

        protected override void Reset()
        {
            base.Reset();
            this.lblMessage.Text = "请扫描条码";
        }

        protected override void DoSubmit()
        {
            try
            {
                if (base.hus == null || base.hus.Count == 0)
                {
                    throw new BusinessException("没有检验明细");
                }
                base.smartDeviceService.DoInspect(base.hus.Select(h => h.HuId).ToArray(), this.effDate, this.user.Code);
                this.Reset();
                this.lblMessage.Text = "创建检验单成功";
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
    }
}
