using System;
using System.Windows.Forms;
using System.Drawing;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCHuFilter : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private SD_SmartDeviceService smartDeviceService;
        private User user;
        private Hu hu;
        private decimal qty;
        private static UCHuFilter ucHuFilter;
        private static object obj = new object();
        private bool isMark;
        private int keyCodeDiff;

        public UCHuFilter(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.Reset();
        }

        public static UCHuFilter GetUCHuFilter(User user)
        {
            if (ucHuFilter == null)
            {
                lock (obj)
                {
                    if (ucHuFilter == null)
                    {
                        ucHuFilter = new UCHuFilter(user);
                    }
                }
            }
            ucHuFilter.user = user;
            ucHuFilter.lblMessage.ForeColor = Color.Black;
            ucHuFilter.lblMessage.Text = "请扫描原条码";

            ucHuFilter.Reset();
            return ucHuFilter;
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
                    this.hu = smartDeviceService.GetHu(barCode);
                    if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
                    {
                        throw new BusinessException("没有此条码的权限");
                    }
                    if (this.hu.Status != HuStatus.Location)
                    {
                        throw new BusinessException("不在库存中不能过滤");
                    }
                    if (hu.HuOption != HuOption.UnFilter)
                    {
                        DialogResult dr = MessageBox.Show("此条码无需过滤,是否强制过滤?", "是否强制过滤", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
                        switch (dr)
                        {
                            case DialogResult.No:
                                this.Reset();
                                return;
                            case DialogResult.Yes:
                                this.isMark = true;
                                break;
                            default:
                                return;
                        }
                    }

                    this.lbl01.Text = hu.Item;
                    this.lbl02.Text = hu.ReferenceItemCode;
                    this.lbl03.Text = hu.Direction;
                    this.lbl04.Text = hu.Qty.ToString("0.###") + " " + hu.Uom;
                    this.lbl06.Text = hu.Location;
                    this.lbl07.Text = hu.Bin;
                    //this.lbl08.Text = hu.HuOption.ToString();

                    if (this.hu.HuOption == HuOption.Aged)
                    {
                        this.lbl08.Text = "已老化";
                    }
                    else if (this.hu.HuOption == HuOption.Filtered)
                    {
                        this.lbl08.Text = "已过滤";
                    }
                    else if (this.hu.HuOption == HuOption.NoNeed)
                    {
                        this.lbl08.Text = "无需";
                    }
                    else if (this.hu.HuOption == HuOption.UnAging)
                    {
                        this.lbl08.Text = "未老化";
                    }
                    else if (this.hu.HuOption == HuOption.UnFilter)
                    {
                        this.lbl08.Text = "未过滤";
                    }

                    this.lbl09.Text = hu.ManufactureDate.ToString("yyyy-MM-dd");
                    this.lbl10.Text = hu.ManufactureParty;
                    this.lblBarCodeInfo.Text = hu.HuId;
                    this.lblItemDescInfo.Text = hu.ItemDescription;

                    this.lblMessage.Text = "请输入过滤后的数量";
                    this.qty = 0;
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
                    this.lbl04.Text = this.qty.ToString("0.###") + " " + hu.Uom;
                    this.lblMessage.Text = "请确认过滤";
                    if (this.qty > this.hu.Qty * 1.2m)
                    {
                        throw new BusinessException("过滤后的数量不能超过了过滤前数量的1.2倍");
                    }
                }
                catch (Exception ex)
                {
                    if (ex is System.FormatException)
                    {
                        Utility.ShowMessageBox("请输入过滤后的数量");
                        lblMessage.Text = "请输入过滤后的数量";
                        lblMessage.ForeColor = Color.Red;
                        this.isMark = true;
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
                if (this.qty == 0)
                {
                    return;
                }
                var newHu = smartDeviceService.DoFilter(this.hu.HuId, this.qty, this.user.Code);
                this.Reset();
                this.lblMessage.Text = string.Format("条码过滤成功,新条码:{0}", newHu.HuId);
                this.isMark = true;
            }
            catch (SoapException ex)
            {
                this.Reset();
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex.Message);
            }
            catch (BusinessException ex)
            {
                this.Reset();
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex);
            }
            catch (Exception ex)
            {
                this.Reset();
                Utility.ShowMessageBox(ex.Message);
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
            }
        }

        private void Reset()
        {
            this.isMark = true;
            this.hu = null;
            this.qty = 0;
            this.tbBarCode.Text = string.Empty;
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
