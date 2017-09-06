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
    public partial class UCPutAway : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        SD_SmartDeviceService smartDeviceService;
        private User user;
        private Hu hu;
        private Bin bin;
        private bool isMark;

        private static UCPutAway ucPutAway;
        private static object obj = new object();
        private int keyCodeDiff;

        public UCPutAway(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.btnOrder.Text = "上架";
            this.Reset();
        }

        public static UCPutAway GetUCPutAway(User user)
        {
            if (ucPutAway == null)
            {
                lock (obj)
                {
                    if (ucPutAway == null)
                    {
                        ucPutAway = new UCPutAway(user);
                    }
                }
            }
            ucPutAway.user = user;
            ucPutAway.Reset();
            return ucPutAway;
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
                if (e == null || (e.KeyData & Keys.KeyCode) == Keys.Enter)
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
                //else if ((e.KeyData & Keys.KeyCode) == Keys.F4)
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
            this.lblMessage.Text = "请扫描库格条码";
            this.tbBarCode.Focus();
            this.hu = new Hu();
            this.bin = null;
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
        }

        private void ScanBarCode()
        {
            this.tbBarCode.Focus();
            string barCode = this.tbBarCode.Text.Trim();
            if (barCode.Length < 3)
            {
                return;
                //throw new BusinessException("条码格式不合法");
            }
            string op = Utility.GetBarCodeType(this.user.BarCodeTypes, barCode);
            if (op == CodeMaster.BarCodeType.B.ToString())
            {
                barCode = barCode.Substring(2, barCode.Length - 2);
                this.bin = this.smartDeviceService.GetBin(barCode);
                this.lblMessage.Text = "当前库格: " + this.bin.Code;
                this.lbl06.Text = this.bin.Code;
            }
            else if (op == CodeMaster.BarCodeType.HU.ToString())
            {
                if (this.bin == null)
                {
                    throw new BusinessException("请先扫描库格");
                }

                this.hu = this.smartDeviceService.GetHu(barCode);

                if (this.hu == null)
                {
                    throw new BusinessException("此条码不存在");
                }

                if (!Utility.HasPermission(user.Permissions, null, true, false, this.hu.Region, null))
                {
                    throw new BusinessException("没有操作此区域{0}的权限", hu.Region);
                }

                this.smartDeviceService.DoPutAway(barCode, this.bin.Code, this.user.Code);

                this.lbl01.Text = this.hu.Item;
                this.lbl02.Text = this.hu.ReferenceItemCode;
                this.lbl03.Text = this.hu.LotNo;
                this.lbl04.Text = this.hu.Qty.ToString("0.########");
                this.lbl05.Text = this.hu.Uom;
                //this.lbl06.Text = this.hu.Location;
                this.lbl07.Text = this.hu.ManufactureDate.ToString("yyyy-MM-dd HH:mm");
                this.lbl08.Text = this.hu.ManufactureParty;
                this.lblBarCodeInfo.Text = this.hu.HuId;
                this.lblItemDescInfo.Text = this.hu.ItemDescription;
                this.lblMessage.Text = "上架成功!";
            }
            else
            {
                throw new BusinessException("条码格式不合法");
            }
            this.tbBarCode.Text = string.Empty;
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
