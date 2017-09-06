using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;
using System;
using System.Linq;

namespace com.Sconit.SmartDevice
{
    public partial class UCPickListShip : UserControl
    {
        SD_SmartDeviceService smartDeviceService;
        private User user;
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private bool isMark;
        private PickListMaster pickListMaster;

        private static UCPickListShip ucPickListShip;
        private static object obj = new object();
        private int keyCodeDiff;

        public UCPickListShip(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.btnOrder.Text = "发货";
            this.Reset();
        }

        public static UCPickListShip GetUCPickListShip(User user)
        {
            if (ucPickListShip == null)
            {
                lock (obj)
                {
                    if (ucPickListShip == null)
                    {
                        ucPickListShip = new UCPickListShip(user);
                    }
                }
            }
            ucPickListShip.user = user;
            ucPickListShip.Reset();
            ucPickListShip.lblMessage.Text = "请扫描拣货单号";
            return ucPickListShip;
        }
        //public UCPickListShip(User user)
        //{
        //    InitializeComponent();
        //    this.smartDeviceService = new SmartDeviceService();
        //    this.user = user;
        //    this.btnOrder.Text = "发货";
        //    this.Reset();
        //}

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
                        this.ScanBarCode();
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
                string ipNo = this.smartDeviceService.ShipPickList(this.pickListMaster.PickListNo, this.user.Code);
                this.Reset();
                this.lblMessage.Text = string.Format("拣货单发货成功,送货单:{0}", ipNo);
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
            this.tbBarCode.Text = string.Empty;
            string op = Utility.GetBarCodeType(this.user.BarCodeTypes, barCode);
            if (barCode == string.Empty)
            {
                this.btnOrder.Focus();
            }
            else if (barCode.Length < 3)
            {
                throw new BusinessException("条码格式不合法");
            }
            else if (op == CodeMaster.BarCodeType.PIK.ToString())
            {
                PickListMaster pickListMaster = this.smartDeviceService.GetPickList(barCode, false);

                if (pickListMaster.Status != PickListStatus.InProcess)
                {
                    throw new BusinessException("拣货单不是执行中状态，不可以发货。");
                }
                if (!Utility.HasPermission(pickListMaster, user))
                {
                    throw new BusinessException("没有操作此拣货单{0}的权限", pickListMaster.PickListNo);
                }
                if (!this.user.Permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region)
                        .Select(t => t.PermissionCode).Contains(pickListMaster.PartyFrom))
                {
                    throw new BusinessException("没有此区域权限不能发货。");
                }

                this.pickListMaster = pickListMaster;
                this.lblPartyFrom.Text = pickListMaster.PartyFrom;
                this.lblPartyTo.Text = pickListMaster.PartyTo;
                this.lblStartTime.Text = pickListMaster.StartTime.ToString("yyyy-MM-dd HH:mm");
                this.lblWindowTime.Text = pickListMaster.WindowTime.ToString("yyyy-MM-dd HH:mm");
                this.lblEffectTime.Text = pickListMaster.EffectiveDate.ToString("yyyy-MM-dd HH:mm");
                this.lblMessage.Text = "请点击发货按钮发货";
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
            this.lblMessage.Text = string.Empty;
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
