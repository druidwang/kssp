using System;
using System.Windows.Forms;
using System.Drawing;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCHuClone : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private SD_SmartDeviceService smartDeviceService;
        private User user;
        private Hu hu;
        private decimal qty;
        private static UCHuClone ucHuClone;
        private static object obj = new object();
        private bool isMark;
        private int keyCodeDiff;

        public UCHuClone(User user)
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
            string barCode = this.tbBarCode.Text.Trim();
            this.tbBarCode.Focus();
            this.tbBarCode.Text = string.Empty;
            string op = Utility.GetBarCodeType(this.user.BarCodeTypes, barCode);

            if (this.hu == null)
            {
                if (barCode.Length < 3)
                {
                    throw new BusinessException("条码格式不合法");
                }

                try
                {
                    Hu hu = smartDeviceService.GetHu(barCode);
                    this.lbl01.Text = hu.Item;
                    this.lbl02.Text = hu.ReferenceItemCode;
                    this.lbl03.Text = hu.Direction;
                    this.lbl04.Text = hu.Qty.ToString("0.########") + " " + hu.Uom;
                    //this.lbl05.Text = hu.Uom;
                    this.lbl06.Text = hu.Location;
                    this.lbl07.Text = hu.Bin;
                    this.lbl08.Text = hu.Status.ToString();
                    this.lbl09.Text = hu.ManufactureDate.ToString("yyyy-MM-dd HH:mm");
                    this.lbl10.Text = hu.ManufactureParty;
                    this.lblBarCodeInfo.Text = hu.HuId;
                    this.lblItemDescInfo.Text = hu.ItemDescription;

                    this.lblMessage.Text = "请输入数量或者点击克隆";
                    this.hu = hu;
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
            else
            {
                try
                {
                    this.qty = decimal.Parse(barCode);
                    this.lbl04.Text = this.qty.ToString("0.########");
                    this.lblMessage.Text = "请确认克隆条码";
                }
                catch (Exception ex)
                {
                    if (ex is System.FormatException)
                    {
                        lblMessage.Text = "请输入数字";
                        lblMessage.ForeColor = Color.Red;
                    }
                    else
                    {
                        Utility.ShowMessageBox(ex.Message);
                    }
                }
            }
        }

        private void DoSubmit()
        {
            try
            {
                if (this.hu == null)
                {
                    Reset();
                    return;
                }
                if (this.qty <= 0)
                {
                    throw new BusinessException("条码数量须大于0");
                }
                var newhu = smartDeviceService.CloneHu(this.hu.HuId, this.qty, this.user.Code);
                this.Reset();
                if (newhu != null)
                {
                    this.lblMessage.Text = string.Format("条码克隆成功,新条码:{0}", newhu.HuId);
                }
                this.isMark = true;
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
                Utility.ShowMessageBox(ex.Message);
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
            }
        }

        private void Reset()
        {
            this.hu = null;
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.label01.Text = "物料号:";
            this.label02.Text = "参考号:";
            this.label03.Text = "方向:";
            this.label04.Text = "数量:";
            //this.label05.Text = "单位:";
            this.label06.Text = "库位:";
            this.label07.Text = "库格:";
            this.label08.Text = "状态:";
            this.label09.Text = "制造时间:";
            this.label10.Text = "制造厂商:";
            this.lbl01.Text = string.Empty;
            this.lbl02.Text = string.Empty;
            this.lbl03.Text = string.Empty;
            this.lbl04.Text = string.Empty;
            //this.lbl05.Text = string.Empty;
            this.lbl06.Text = string.Empty;
            this.lbl07.Text = string.Empty;
            this.lbl08.Text = string.Empty;
            this.lbl09.Text = string.Empty;
            this.lbl10.Text = string.Empty;
            this.lblBarCodeInfo.Text = string.Empty;
            this.lblItemDescInfo.Text = string.Empty;
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
            this.lblMessage.Text = "请扫描条码";
            this.tbBarCode.Focus();
        }

        public static UCHuClone GetUCHuClone(User user)
        {
            if (ucHuClone == null)
            {
                lock (obj)
                {
                    if (ucHuClone == null)
                    {
                        ucHuClone = new UCHuClone(user);
                    }
                }
            }
            ucHuClone.user = user;
            ucHuClone.lblMessage.ForeColor = Color.Black;
            ucHuClone.lblMessage.Text = "请扫描原条码";
            ucHuClone.Reset();
            return ucHuClone;
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
