using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;
using System;

namespace com.Sconit.SmartDevice
{
    public partial class UCPickListOnline : UserControl
    {
        SD_SmartDeviceService smartDeviceService;
        private User user;
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private bool isMark;
        private PickListMaster pickListMaster;
        private int keyCodeDiff;

        public UCPickListOnline(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.btnOrder.Text = "上线";
            this.Reset();
        }

        private void btnBack_Click(object sender, System.EventArgs e)
        {
            this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
        }

        private void tbBarCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.isMark)
            {
                this.isMark = false;
                return;
            }
            try
            {
                string barCode = this.tbBarCode.Text.Trim();
                if (sender is Button)
                {
                    if (e == null)
                    {
                        this.DoSubmit();
                    }
                }
                else
                {
                    if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
                    {
                        if (!string.IsNullOrEmpty(barCode))
                        {
                            this.ScanBarCode();
                        }
                        else
                        {
                            this.btnOrder.Focus();
                        }
                    }
                    else if (((e.KeyData & Keys.KeyCode) == Keys.Escape))
                    {
                        if (!string.IsNullOrEmpty(barCode))
                        {
                            this.tbBarCode.Text = string.Empty;
                        }
                        else
                        {
                            this.Reset();
                        }
                    }
                    else if (e.KeyValue == 115 + this.keyCodeDiff)
                    {
                        this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                    }
                    else if ((e.KeyData & Keys.KeyCode) == Keys.F1)
                    {
                        //todo Help
                    }
                }
            }
            catch (SoapException ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex.Message);
            }
            catch (BusinessException ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex);
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.Reset();
                Utility.ShowMessageBox(ex.Message);
            }
        }

        private void DoSubmit()
        {
            try
            {
                if (this.pickListMaster == null)
                {
                    throw new BusinessException("请先扫描拣货单。");
                }
                this.smartDeviceService.StartPickList(this.pickListMaster.PickListNo, this.user.Code);
                this.Reset();
                this.lblMessage.Text = "拣货单上线成功";
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

        private void ScanBarCode()
        {
            string barCode = this.tbBarCode.Text.Trim();
            this.tbBarCode.Focus();
            this.tbBarCode.Text = string.Empty;
            string op = Utility.GetBarCodeType(this.user.BarCodeTypes, barCode);
            if (barCode.Length < 3)
            {
                throw new BusinessException("条码格式不合法");
            }
            if (op == CodeMaster.BarCodeType.PIK.ToString())
            {
                PickListMaster pickListMaster = this.smartDeviceService.GetPickList(barCode, false);

                if (!Utility.HasPermission(pickListMaster, user))
                {
                    throw new BusinessException("没有操作此拣货单{0}的权限", pickListMaster.PickListNo);
                }

                this.pickListMaster = pickListMaster;
                this.lblPartyFrom.Text = pickListMaster.PartyFrom;
                this.lblPartyTo.Text = pickListMaster.PartyTo;
                this.lblStartTime.Text = pickListMaster.StartTime.ToString("yyyy-MM-dd HH:mm");
                this.lblWindowTime.Text = pickListMaster.WindowTime.ToString("yyyy-MM-dd HH:mm");
                this.lblEffectTime.Text = pickListMaster.EffectiveDate.ToString("yyyy-MM-dd HH:mm");
                this.lblMessage.Text = "请确定上线";
            }
            else
            {
                throw new BusinessException("条码格式不合法");
            }
        }

        private void Reset()
        {
            this.pickListMaster = null;
            this.lblPartyFrom.Text = string.Empty;
            this.lblPartyTo.Text = string.Empty;
            this.lblStartTime.Text = string.Empty;
            this.lblWindowTime.Text = string.Empty;
            this.lblEffectTime.Text = string.Empty;

            this.tbBarCode.Text = string.Empty;
            this.lblMessage.Text = "请扫描拣货单";
            this.tbBarCode.Focus();
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(sender, null);
        }

        private void btnOrder_KeyUp(object sender, KeyEventArgs e)
        {
            this.tbBarCode_KeyUp(sender, e);
        }
    }
}
