using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCPickUp : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        SD_SmartDeviceService smartDeviceService;
        private User user;
        private Hu hu;
        private bool isMark;

        private static UCPickUp ucPickUp;
        private static object obj = new object();
        private int keyCodeDiff;

        public UCPickUp(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.btnOrder.Text = "下架";
            this.Reset();
        }

        public static UCPickUp GetUCPickUp(User user)
        {
            if (ucPickUp == null)
            {
                lock (obj)
                {
                    if (ucPickUp == null)
                    {
                        ucPickUp = new UCPickUp(user);
                    }
                }
            }
            ucPickUp.user = user;
            ucPickUp.Reset();
            return ucPickUp;
        }
        //public UCPickUp(User user)
        //{
        //    InitializeComponent();
        //    this.smartDeviceService = new SmartDeviceService();
        //    this.user = user;
        //    this.Reset();
        //}

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
                if (e == null || (e.KeyData & Keys.KeyCode) == Keys.Enter)
                {
                    this.DoPickUp();
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
                this.Reset();
                Utility.ShowMessageBox(ex);
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.Reset();
                Utility.ShowMessageBox(ex.Message);
            }
        }

        private void Reset()
        {
            this.lblMessage.Text = string.Empty;
            this.lbl01.Text = string.Empty;
            this.lbl02.Text = string.Empty;
            this.lbl03.Text = string.Empty;
            this.lbl04.Text = string.Empty;
            this.lbl05.Text = string.Empty;
            this.lbl06.Text = string.Empty;
            this.lbl07.Text = string.Empty;
            this.lbl08.Text = string.Empty;
            this.lblBarCodeInfo.Text = string.Empty;
            this.lblItemDescInfo.Text = string.Empty;
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.hu = new Hu();
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
        }

        private void DoPickUp()
        {
            if (this.tbBarCode.Text.Trim() == string.Empty)
            {
                return;
            }
            this.hu = this.smartDeviceService.GetHu(this.tbBarCode.Text.Trim());
            if (this.hu == null)
            {
                throw new BusinessException("此条码不存在");
            }

            if (!Utility.HasPermission(user.Permissions, null, true, false, this.hu.Region, null))
            {
                throw new BusinessException("没有操作此区域{0}的权限", hu.Region);
            }

            this.smartDeviceService.DoPickUp(this.tbBarCode.Text.Trim(), this.user.Code);

            this.lbl01.Text = this.hu.Item;
            this.lbl02.Text = this.hu.ReferenceItemCode;
            this.lbl03.Text = this.hu.LotNo;
            this.lbl04.Text = this.hu.Qty.ToString("0.###");
            this.lbl05.Text = this.hu.Uom;
            this.lbl06.Text = this.hu.Location;
            this.lbl07.Text = this.hu.ManufactureDate.ToString("yyyy-MM-dd HH:mm");
            this.lbl08.Text = this.hu.ManufactureParty;
            this.lblBarCodeInfo.Text = this.hu.HuId;
            this.lblItemDescInfo.Text = this.hu.ItemDescription;
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.lblMessage.Text = "下架成功";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(null, null);
        }
    }
}
