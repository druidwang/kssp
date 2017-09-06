using System;
using System.Windows.Forms;
using System.Drawing;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCWMSDeliveryBarCode : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private SD_SmartDeviceService smartDeviceService;
        private User user;
        private Hu hu;
        private DeliverBarCode dc;
        private bool isMark;
        private int btnIndex;

        public UCWMSDeliveryBarCode(User user)
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
                    if (e == null || (e.KeyData & Keys.KeyCode) == Keys.Enter)
                    {
                        this.ScanBarCode();
                    }
                    else if (((e.KeyData & Keys.KeyCode) == Keys.Escape))
                    {
                        if (this.hu != null)
                        {
                            this.Reset();
                        }
                        else
                        {
                            this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                        }
                    }
                    else if ((e.KeyData & Keys.KeyCode) == Keys.F1)
                    {
                        //todo Help
                    }
                    else if ((e.KeyData & Keys.KeyCode) == Keys.F4)
                    {
                        this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                    }
                }
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.Reset();
                Utility.ShowMessageBox(ex.Message);
            }

        }

        private void ScanBarCode()
        {
            string barCode = this.tbBarCode.Text.Trim();
            this.tbBarCode.Focus();
            this.tbBarCode.Text = string.Empty;
            string op = Utility.GetBarCodeType(this.user.BarCodeTypes, barCode);

            if (op == CodeMaster.BarCodeType.HU.ToString())
            {
                if (this.hu == null)
                {
                    if (barCode.Length < 3)
                    {
                        throw new BusinessException("条码格式不合法");
                    }

                    try
                    {
                        Hu hu = smartDeviceService.GetDeliverMatchHu(barCode, this.user.Code);
                        if (this.dc != null)
                        {
                            if (hu.Item != this.dc.Item)
                            {
                                throw new BusinessException("配送标签的零件号与配送标签不匹配。");
                            }
                            if (hu.Uom != this.dc.Uom)
                            {
                                throw new BusinessException("配送标签的单位与配送标签不匹配。");
                            }
                            if( hu.UnitCount != this.dc.UnitCount)
                            {
                                throw new BusinessException("配送标签的包装与配送标签不匹配。");
                            }
                            if (hu.Qty != this.dc.Qty)
                            {
                                throw new BusinessException("配送标签的数量与配送标签不匹配。");
                            }
                        }
                        this.hu = hu;
                        this.tbHuBin.Text = hu.Bin;
                        this.tbHuId.Text = hu.HuId;
                        this.tbHuItem.Text = hu.Item;
                        this.tbHuItemDescription.Text = hu.ItemDescription;
                        this.tbHuQty.Text = hu.Qty.ToString();
                        this.tbHuUom.Text = hu.Uom;
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
                    if (this.dc == null)
                    {
                        Utility.ShowMessageBox("已扫入条码，请扫描配送标签");
                        this.tbBarCode.Text = string.Empty;
                        this.tbBarCode.Focus();
                    }
                    else
                    {
                        Utility.ShowMessageBox("已扫入条码和配送标签，请提交！");
                        this.tbBarCode.Text = string.Empty;
                        this.tbBarCode.Focus();
                    }
                }
            }
            else if (op == CodeMaster.BarCodeType.DC.ToString())
            {
                if (this.dc == null)
                {
                    if (barCode.Length < 3)
                    {
                        throw new BusinessException("条码格式不合法");
                    }

                    try
                    {
                        DeliverBarCode dc = smartDeviceService.GetDeliverBarCode(barCode, this.user.Code);
                        if (this.hu != null)
                        {
                            if (this.hu.Item != dc.Item || this.hu.Uom != dc.Uom || this.hu.UnitCount != dc.UnitCount)
                            {
                                throw new BusinessException("配送标签的零件号/单位/包装与配送标签不匹配。");
                            }
                        }
                        this.dc = dc;
                        this.tbDCDock.Text = dc.Dock;
                        this.tbDCId.Text = dc.BarCode;
                        this.tbDCWindowTime.Text = dc.WindowTime.ToString();
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
                    if (this.hu == null)
                    {
                        Utility.ShowMessageBox("已扫入配送标签，请扫描条码");
                        this.tbBarCode.Text = string.Empty;
                        this.tbBarCode.Focus();
                    }
                    else
                    {
                        Utility.ShowMessageBox("已扫入条码和配送标签，请提交！");
                        this.tbBarCode.Text = string.Empty;
                        this.tbBarCode.Focus();
                    }
                }
            }
        }

        private void DoSubmit()
        {
            try
            {
                if (this.dc == null)
                {
                    Utility.ShowMessageBox("请扫描配送标签");
                }
                if (this.dc.IsPickHu == false && this.hu == null)
                {
                    Utility.ShowMessageBox("请扫描条码");
                }
                this.smartDeviceService.MatchDCToHU(this.hu != null ? this.hu.HuId:string.Empty, this.dc.BarCode ,this.user.Code);
                this.Reset();
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                this.Reset();
                Utility.ShowMessageBox(ex.Message);
                
            }
        }

        private void Reset()
        {
            this.hu = null;
            this.dc = null;

            this.tbDCDock.Text = string.Empty;
            this.tbDCId.Text = string.Empty;
            this.tbDCWindowTime.Text = string.Empty;
            this.tbHuBin.Text = string.Empty;
            this.tbHuId.Text = string.Empty;
            this.tbHuItem.Text = string.Empty;
            this.tbHuItemDescription.Text = string.Empty;
            this.tbHuQty.Text = string.Empty;
            this.tbHuUom.Text = string.Empty;
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(sender, null);
        }

    }
}
