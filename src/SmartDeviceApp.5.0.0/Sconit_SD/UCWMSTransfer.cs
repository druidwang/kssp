using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;
using System.Drawing;
using com.Sconit.SmartDevice.CodeMaster;

namespace com.Sconit.SmartDevice
{
    public partial class UCWMSTransfer : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        protected DataGridTableStyle ts;
        protected DataGridTextBoxColumn columnHuId;
        private DataGridTextBoxColumn columnCurrentQty;
        private DataGridTextBoxColumn columnItemDescription;
        private DataGridTextBoxColumn columnItemCode;
        private DataGridTextBoxColumn columnReferenceItemCode;
        private DataGridTextBoxColumn columnUnitCount;
        private DataGridTextBoxColumn columnUom;
        private DataGridTextBoxColumn columnCarton;
        protected DataGridTextBoxColumn columnIsOdd;
        protected DataGridTextBoxColumn columnLotNo;

        private User user;
        private List<Hu> hus;
        private bool isMark;
        private bool isCancel;
        private string dock;
        private FlowMaster flowMaster;

        private int keyCodeDiff;

        private SD_SmartDeviceService smartDeviceService;

        public UCWMSTransfer(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.InitializeDataGrid();
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
                            this.DoCancel();
                        }
                    }
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F4)
                    else if (e.KeyValue == 115 + this.keyCodeDiff)
                    {
                        this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                    }
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F3)
                    else if (e.KeyValue == 114 + this.keyCodeDiff)
                    {
                        
                    }
                    else if ((e.KeyData & Keys.KeyCode) == Keys.F2 || (e.KeyData & Keys.KeyCode) == Keys.F5)
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

        private void DoCancel()
        {
            this.Reset();
            this.lblMessage.Text = "已全部取消";
        }

        private void DoSubmit()
        {
            try
            {
                if (hus == null || hus.Count == 0)
                {
                    this.tbBarCode.Focus();
                    return;
                }

                
                this.Reset();
                this.lblMessage.Text = "移库成功";
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex.Message);
            }
        }

        private void ScanBarCode()
        {
            string barCode = this.tbBarCode.Text.Trim();
            this.lblMessage.Text = string.Empty;
            this.tbBarCode.Text = string.Empty;

            if (barCode.Length < 3)
            {
                throw new BusinessException("条码格式不合法");
            }
            string op = Utility.GetBarCodeType(this.user.BarCodeTypes, barCode);


            if (string.IsNullOrEmpty(this.dock) == true)
            {
                if (op == CodeMaster.BarCodeType.D.ToString())
                {
                    this.dock = barCode.Substring(2, barCode.Length - 2);
                    
                }
                else
                {
                    throw new BusinessException("请先扫描道口条码");
                }
            }
            else
            {
                #region HU
                if (op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if (this.hus == null)
                    {
                        this.hus = new List<Hu>();
                    }

                    Hu hu = smartDeviceService.GetDeliverMatchHu(barCode, this.user.Code);
                    if (hu == null)
                    {
                        throw new BusinessException("条码不存在");
                    }
                    var matchHu = this.hus.Where(h => h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));
                    if (matchHu != null && matchHu.Count() > 0)
                    {
                        throw new BusinessException("条码重复扫描!");
                    }

                    if (!string.IsNullOrEmpty(hu.AgingLocation) && hu.AgingLocation != this.dock)
                    {
                        throw new BusinessException("该条码{0}已移至其他道口{1}!", hu.HuId, hu.AgingLocation);
                    }
                    this.hus.Insert(0, hu);
                    this.gvHuListDataBind();

                }
                #endregion
                else
                {
                    throw new BusinessException("条码格式不合法1");
                }
            }
        }

        private void gvHuListDataBind()
        {
            List<Hu> hus = new List<Hu>();
            if (this.hus != null)
            {
                hus = this.hus;
            }

            this.ts = new DataGridTableStyle();
            this.ts.MappingName = hus.GetType().Name;

            this.ts.GridColumnStyles.Add(columnHuId);
            this.ts.GridColumnStyles.Add(columnCurrentQty);
            this.ts.GridColumnStyles.Add(columnLotNo);
            this.ts.GridColumnStyles.Add(columnUnitCount);
            this.ts.GridColumnStyles.Add(columnUom);
            this.ts.GridColumnStyles.Add(columnReferenceItemCode);
            this.ts.GridColumnStyles.Add(columnItemDescription);

            this.dgDetail.TableStyles.Clear();
            this.dgDetail.TableStyles.Add(this.ts);

            this.dgDetail.DataSource = hus;
            this.ResumeLayout();

            this.dgDetail.Visible = true;
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
        }

        private void Reset()
        {
            this.hus = new List<Hu>();
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.lblMessage.Text = string.Empty;
            this.dgDetail.Visible = false;
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
            this.lblMessage.Text = "请先扫道口";
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(sender, null);
        }


        private void tbBarCode_LostFocus(object sender, EventArgs e)
        {

        }

        private void btnOrder_KeyUp(object sender, KeyEventArgs e)
        {
            this.tbBarCode_KeyUp(sender, e);
        }

        private void InitializeDataGrid()
        {
            // 
            // columnItemCode
            // 
            this.columnItemCode = new DataGridTextBoxColumn();
            this.columnItemCode.Format = "";
            this.columnItemCode.FormatInfo = null;
            this.columnItemCode.HeaderText = "物料";
            this.columnItemCode.MappingName = "Item";
            this.columnItemCode.Width = 100;
            // 
            // columnLotNo
            // 
            this.columnLotNo = new DataGridTextBoxColumn();
            this.columnLotNo.Format = "";
            this.columnLotNo.FormatInfo = null;
            this.columnLotNo.HeaderText = "批号";
            this.columnLotNo.MappingName = "LotNo";
            this.columnLotNo.Width = 40;
            // 
            // columnCurrentQty
            // 
            this.columnCurrentQty = new DataGridTextBoxColumn();
            this.columnCurrentQty.Format = "0.##";
            this.columnCurrentQty.FormatInfo = null;
            this.columnCurrentQty.HeaderText = "数量";
            this.columnCurrentQty.MappingName = "CurrentQty";
            this.columnCurrentQty.Width = 40;
            // 
            // columnCarton
            // 
            this.columnCarton = new DataGridTextBoxColumn();
            this.columnCarton.Format = "";
            this.columnCarton.FormatInfo = null;
            this.columnCarton.HeaderText = "箱数";
            this.columnCarton.MappingName = "Carton";
            this.columnCarton.Width = 40;
            // 
            // columnUnitCount
            // 
            this.columnUnitCount = new DataGridTextBoxColumn();
            this.columnUnitCount.Format = "0.##";
            this.columnUnitCount.FormatInfo = null;
            this.columnUnitCount.HeaderText = "单包装";
            this.columnUnitCount.MappingName = "UnitCount";
            this.columnUnitCount.Width = 40;
            // 
            // columnUom
            // 
            this.columnUom = new DataGridTextBoxColumn();
            this.columnUom.Format = "";
            this.columnUom.FormatInfo = null;
            this.columnUom.HeaderText = "单位";
            this.columnUom.MappingName = "Uom";
            this.columnUom.Width = 40;
            // 
            // columnIsOdd
            // 
            this.columnIsOdd = new DataGridTextBoxColumn();
            this.columnIsOdd.Format = "";
            this.columnIsOdd.FormatInfo = null;
            this.columnIsOdd.HeaderText = "零头";
            this.columnIsOdd.MappingName = "IsOdd";
            this.columnIsOdd.Width = 40;
            // 
            // columnReferenceItemCode
            // 
            this.columnReferenceItemCode = new DataGridTextBoxColumn();
            this.columnReferenceItemCode.Format = "";
            this.columnReferenceItemCode.FormatInfo = null;
            this.columnReferenceItemCode.HeaderText = "参考物料";
            this.columnReferenceItemCode.MappingName = "ReferenceItemCode";
            this.columnReferenceItemCode.Width = 100;
            // 
            // columnItemDescription
            // 
            this.columnItemDescription = new DataGridTextBoxColumn();
            this.columnItemDescription.Format = "";
            this.columnItemDescription.FormatInfo = null;
            this.columnItemDescription.HeaderText = "描述";
            this.columnItemDescription.MappingName = "ItemDescription";
            this.columnItemDescription.Width = 150;
            // 
            // columnHuId
            // 
            this.columnHuId = new DataGridTextBoxColumn();
            this.columnHuId.Format = "";
            this.columnHuId.FormatInfo = null;
            this.columnHuId.HeaderText = "条码";
            this.columnHuId.MappingName = "HuId";
            this.columnHuId.NullText = "";
            this.columnHuId.Width = 150;
        }
    }
}
