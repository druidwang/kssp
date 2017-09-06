using System;
using System.Windows.Forms;
using System.Drawing;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCHuAging : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private SD_SmartDeviceService smartDeviceService;
        private User user;
        private int qty;
        private static UCHuAging ucHuAging;
        private static object obj = new object();
        private bool isMark;
        private int keyCodeDiff;
        private bool isStart;

        public UCHuAging(User user, bool isStart)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.isStart = isStart;
            this.Reset();
        }

        public static UCHuAging GetUCHuAging(User user, bool isStart)
        {
            if (ucHuAging == null)
            {
                lock (obj)
                {
                    if (ucHuAging == null)
                    {
                        ucHuAging = new UCHuAging(user, isStart);
                    }
                }
            }
            ucHuAging.user = user;
            ucHuAging.isStart = isStart;
            ucHuAging.lblMessage.ForeColor = Color.Black;

            if (isStart)
            {
                ucHuAging.lblMessage.Text = "老化开始,请扫描条码";

            }
            else
            {
                ucHuAging.lblMessage.Text = "老化结束,请扫描条码";
            }
            ucHuAging.Reset();
            return ucHuAging;
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
                    this.ScanBarCode();
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

            if (barCode.Length < 3)
            {
                throw new BusinessException("条码格式不合法");
            }

            try
            {
                Hu hu = new Hu();
                if (this.isStart)
                {
                    hu = smartDeviceService.StartAging(barCode, this.user.Code);
                    //if (hu.Status != HuStatus.Location)
                    //{
                    //    throw new BusinessException("此条码不在库存中,不能进行老化操作");
                    //}

                    if (hu.IsFreeze)
                    {
                        throw new BusinessException("此条码已经冻结,不能进行老化操作");
                    }
                    if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
                    {
                        throw new BusinessException("没有此条码的权限");
                    }
                    this.lblMessage.Text = string.Format("老化开始成功,时间:{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    if (hu.HuOption == HuOption.NoNeed)
                    {
                        this.lblMessage.Text = string.Format("老化开始成功.注:此条码无需老化");
                    }
                    else
                    {
                        this.lblMessage.Text = string.Format("老化开始成功,时间:{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    }
                }
                else
                {
                    hu = smartDeviceService.DoAging(barCode, this.user.Code);
                    this.lblMessage.Text = string.Format("老化成功,新条码:{0}", hu.HuId);
                }
                this.lbl01.Text = hu.Item;
                this.lbl02.Text = hu.ReferenceItemCode;
                this.lbl03.Text = hu.Direction;
                this.lbl04.Text = hu.Qty.ToString("0.###") + " " + hu.Uom;
                this.lbl06.Text = hu.Location;
                this.lbl07.Text = hu.Bin;
                this.lbl08.Text = hu.HuOption == HuOption.Aged ? "已老化" : "未老化";
                this.lbl09.Text = hu.ManufactureDate.ToString("yyyy-MM-dd");
                this.lbl10.Text = hu.ManufactureParty;
                this.lblBarCodeInfo.Text = hu.HuId;
                this.lblItemDescInfo.Text = hu.ItemDescription;

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
            this.label03.Text = "方向:";
            this.label04.Text = "数量:";
            //this.label05.Text = "单位:";
            this.label06.Text = "库位:";
            this.label07.Text = "库格:";
            this.label08.Text = "条码选项:";
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
            //this.lblMessage.Text = "老化开始,请扫描条码";
            if (this.isStart)
            {
                this.lblMessage.Text = "老化开始,请扫描条码";

            }
            else
            {
                this.lblMessage.Text = "老化结束,请扫描条码";
            }
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
