using System;
using System.Windows.Forms;
using System.Drawing;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCReceiveProdOrder : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private SD_SmartDeviceService smartDeviceService;
        private User user;
        private bool isCancel;
        private static UCReceiveProdOrder ucReceiptProdOrder;
        private static object obj = new object();
        private bool isMark;
        private int keyCodeDiff;

        public UCReceiveProdOrder(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.Reset();
        }

        private void tbBarCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.isMark)
            {
                this.isMark = false;
                this.tbBarCode.Focus();
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
                    else
                    {
                        if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
                        {
                            if (this.tbBarCode.Text.Trim() != string.Empty)
                            {
                                this.ScanBarCode();
                            }
                            else
                            {
                                this.DoSubmit();
                            }
                        }
                    }
                }
                else
                {
                    if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
                    {
                        if (barCode != string.Empty)
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
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F4)
                    else if (e.KeyValue == 115 + this.keyCodeDiff)
                    {
                        this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                    }
                    else if (e.KeyValue == 112 + this.keyCodeDiff)
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F1)
                    {
                        //todo Help
                    }
                    else if (e.KeyValue == 113 + this.keyCodeDiff || e.KeyValue == 116 + this.keyCodeDiff)
                    {
                        if (!this.isCancel)
                        {
                            this.isCancel = true;
                            this.lblBarCode.ForeColor = Color.Red;
                            this.lblMessage.Text = "取消模式.";
                        }
                        else
                        {
                            this.isCancel = false;
                            this.lblBarCode.ForeColor = Color.Black;
                            this.lblMessage.Text = "正常模式.";
                        }
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
                if ((ex is System.Net.WebException) || (ex is SoapException))
                {
                    Utility.ShowMessageBox(ex);
                }
                else if (ex is BusinessException)
                {
                    Utility.ShowMessageBox(ex.Message);
                }
                else
                {
                    this.Reset();
                    Utility.ShowMessageBox(ex.Message);
                }
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
            }
        }

        private void ScanBarCode()
        {
            DoSubmit();
        }

        private void DoSubmit()
        {
            string barCode = this.tbBarCode.Text.Trim();
            this.tbBarCode.Focus();
            this.tbBarCode.Text = string.Empty;
            string op = Utility.GetBarCodeType(this.user.BarCodeTypes, barCode);

            if (barCode.Length < 3)
            {
                throw new BusinessException("条码格式不合法");
            }
            if (op != CodeMaster.BarCodeType.HU.ToString())
            {
                throw new BusinessException("条码格式不合法");
            }

            try
            {
                Hu hu = new Hu();
                if (this.isCancel)
                {
                    hu = smartDeviceService.CancelReceiveProdOrder(barCode, this.user.Code);
                }
                else
                {
                    hu = smartDeviceService.DoReceiveProdOrder(barCode, this.user.Code);
                }
                this.lbl01.Text = hu.Item;
                this.lbl02.Text = hu.ReferenceItemCode;
                this.lbl03.Text = hu.LotNo;
                this.lbl04.Text = hu.Qty.ToString("0.########") + " " + hu.Uom;
                this.lbl05.Text = hu.Memo;
                this.lbl06.Text = hu.Location;
                this.lbl07.Text = hu.Bin;
                this.lbl08.Text = hu.Status.ToString();
                this.lbl09.Text = hu.ManufactureDate.ToString("yyyy-MM-dd HH:mm");
                this.lbl10.Text = hu.ManufactureParty;
                this.lblBarCodeInfo.Text = hu.HuId;
                this.lblItemDescInfo.Text = hu.ItemDescription;

                this.Reset();
                this.lblMessage.Text = "生产下线成功";

            }
            catch (Exception ex)
            {
                if ((ex is System.Net.WebException) || (ex is SoapException))
                {
                    Utility.ShowMessageBox(ex);
                }
                else if (ex is BusinessException)
                {
                    Utility.ShowMessageBox(ex.Message);
                }
                else
                {
                    this.Reset();
                    Utility.ShowMessageBox(ex.Message);
                }
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
            }
        }

        private void Reset()
        {
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.label01.Text = "物料号:";
            this.label02.Text = "参考号:";
            this.label03.Text = "批号:";
            this.label04.Text = "数量:";
            this.label05.Text = "备注:";
            this.label06.Text = "库位:";
            this.label07.Text = "班次:";
            this.label08.Text = "生产单:";
            this.label09.Text = "收货单:";
            this.label10.Text = "生产线:";
            this.lbl01.Text = string.Empty;
            this.lbl02.Text = string.Empty;
            this.lbl03.Text = string.Empty;
            this.lbl04.Text = string.Empty;
            this.lbl05.Text = string.Empty;
            this.lbl06.Text = string.Empty;
            this.lbl07.Text = string.Empty;
            this.lbl08.Text = string.Empty;
            this.lbl09.Text = string.Empty;
            this.lbl10.Text = string.Empty;
            this.lblBarCodeInfo.Text = string.Empty;
            this.lblItemDescInfo.Text = string.Empty;
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
            this.isCancel = false;
            this.lblMessage.Text = "请扫描条码";
        }

        public static UCReceiveProdOrder GetUCReceiptProdOrder(User user)
        {
            if (ucReceiptProdOrder == null)
            {
                lock (obj)
                {
                    if (ucReceiptProdOrder == null)
                    {
                        ucReceiptProdOrder = new UCReceiveProdOrder(user);
                    }
                }
            }
            ucReceiptProdOrder.user = user;
            ucReceiptProdOrder.lblMessage.ForeColor = Color.Black;
            ucReceiptProdOrder.lblMessage.Text = "请扫描条码";
            ucReceiptProdOrder.Reset();
            return ucReceiptProdOrder;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
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
