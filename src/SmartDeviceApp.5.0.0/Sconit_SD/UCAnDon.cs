using System;
using System.Linq;
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
    public partial class UCAnDon : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private SD_SmartDeviceService smartDeviceService;

        protected DataGridTableStyle ts;

        private DataGridTextBoxColumn columnFlow;
        private DataGridTextBoxColumn columnLocationTo;
        private DataGridTextBoxColumn columnItemCode;
        private DataGridTextBoxColumn columnManufactureParty;
        private DataGridTextBoxColumn columnUnitCount;
        private DataGridTextBoxColumn columnUom;
        private DataGridTextBoxColumn columnSequence;
        private DataGridTextBoxColumn columnNote;

        private User user;
        private List<AnDonInput> anDonInputs;
        private bool isMark;
        private bool isCancel;

        private static UCAnDon ucAnDon;
        private static object obj = new object();
        private int keyCodeDiff;

        public UCAnDon(User user)
        {
            this.InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.btnOrder.Text = "按灯";
            this.InitializeDataGrid();
            this.Reset();
        }

        public static UCAnDon GetUCAnDon(User user)
        {
            if (ucAnDon == null)
            {
                lock (obj)
                {
                    if (ucAnDon == null)
                    {
                        ucAnDon = new UCAnDon(user);
                    }
                }
            }
            ucAnDon.user = user;
            ucAnDon.Reset();
            return ucAnDon;
        }

        private void Reset()
        {
            this.anDonInputs = new List<AnDonInput>();
            //this.cardNos = new List<string>();
            this.lblMessage.Text = "正常模式,请扫描按灯条码";
            this.dgDetailDataBind();
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
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
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F2 || (e.KeyData & Keys.KeyCode) == Keys.F5)
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
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F3)
                    else if (e.KeyValue == 114 + this.keyCodeDiff)
                    {

                    }
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F1)
                    else if (e.KeyValue == 112 + this.keyCodeDiff)
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

            if (barCode.Length < 3)
            {
                throw new BusinessException("条码格式不合法");
            }

            if (this.isCancel == false)
            {
                if (op == CodeMaster.BarCodeType.K.ToString())
                {
                    barCode = barCode.Substring(2, barCode.Length - 2);

                    if (!this.anDonInputs.All(a => a.CardNo != barCode))
                    {
                        throw new BusinessException("请不要重复扫描看板卡");
                    }

                    //AnDonInput anDonInput = smartDeviceService.GetKanBanCard(barCode);

                    //anDonInputs.Add(anDonInput);
                    this.dgDetailDataBind();

                }
                else
                {
                    throw new BusinessException("条码格式不合法");
                }
            }
            else
            {
                if (op == CodeMaster.BarCodeType.K.ToString())
                {
                    barCode = barCode.Substring(2, barCode.Length - 2);

                    if (this.anDonInputs.All(a => a.CardNo != barCode))
                    {
                        throw new BusinessException("请扫描已经扫入的看板卡");
                    }

                    AnDonInput anDonInput = this.anDonInputs.First(a => a.CardNo == barCode);

                    anDonInputs.Remove(anDonInput);
                    this.dgDetailDataBind();

                }
                else
                {
                    throw new BusinessException("条码格式不合法");
                }
            }

        }

        private void DoSubmit()
        {
            try
            {
                this.smartDeviceService.DoAnDon(this.anDonInputs.ToArray(), this.user.Code);
                this.Reset();
                this.lblMessage.Text = "按灯成功";
                this.lblMessage.ForeColor = Color.Green;
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

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(sender, null);
        }


        private void tbBarCode_LostFocus(object sender, EventArgs e)
        {
            if (!this.btnOrder.Focused)
            {
                this.tbBarCode.Focus();
            }
        }

        private void btnOrder_KeyUp(object sender, KeyEventArgs e)
        {
            this.tbBarCode_KeyUp(sender, e);
        }

        private void dgDetailDataBind()
        {

            this.ts = new DataGridTableStyle();
            this.ts.MappingName = anDonInputs.GetType().Name;

            this.ts.GridColumnStyles.Add(columnFlow);
            this.ts.GridColumnStyles.Add(columnLocationTo);
            this.ts.GridColumnStyles.Add(columnItemCode);
            this.ts.GridColumnStyles.Add(columnManufactureParty);
            this.ts.GridColumnStyles.Add(columnUnitCount);
            this.ts.GridColumnStyles.Add(columnUom);
            this.ts.GridColumnStyles.Add(columnSequence);
            this.ts.GridColumnStyles.Add(columnNote);


            this.dgDetail.TableStyles.Clear();
            this.dgDetail.TableStyles.Add(this.ts);
            this.dgDetail.DataSource = anDonInputs;

            this.tbBarCode.Text = string.Empty;
            this.ResumeLayout();
        }

        private void InitializeDataGrid()
        {
            // columnFlow
            this.columnFlow = new DataGridTextBoxColumn();
            this.columnFlow.Format = "";
            this.columnFlow.FormatInfo = null;
            this.columnFlow.HeaderText = "路线";
            this.columnFlow.MappingName = "Flow";
            this.columnFlow.Width = 100;

            // columnLocationTo
            this.columnLocationTo = new DataGridTextBoxColumn();
            this.columnLocationTo.Format = "";
            this.columnLocationTo.FormatInfo = null;
            this.columnLocationTo.HeaderText = "目的库位";
            this.columnLocationTo.MappingName = "LocatoinTo";
            this.columnLocationTo.Width = 100;

            // 
            // columnItemCode
            // 
            this.columnItemCode = new DataGridTextBoxColumn();
            this.columnItemCode.Format = "";
            this.columnItemCode.FormatInfo = null;
            this.columnItemCode.HeaderText = "物料";
            this.columnItemCode.MappingName = "Item";
            this.columnItemCode.Width = 100;

            // columnManufactureParty
            this.columnManufactureParty = new DataGridTextBoxColumn();
            this.columnManufactureParty.Format = "";
            this.columnManufactureParty.FormatInfo = null;
            this.columnManufactureParty.HeaderText = "供应商";
            this.columnManufactureParty.MappingName = "ManufactureParty";
            this.columnManufactureParty.NullText = "";
            this.columnManufactureParty.Width = 150;

            // columnUnitCount
            this.columnUnitCount = new DataGridTextBoxColumn();
            this.columnUnitCount.Format = "";
            this.columnUnitCount.FormatInfo = null;
            this.columnUnitCount.HeaderText = "单包装";
            this.columnUnitCount.MappingName = "UnitCount";
            this.columnUnitCount.Width = 40;

            // columnUom
            this.columnUom = new DataGridTextBoxColumn();
            this.columnUom.Format = "";
            this.columnUom.FormatInfo = null;
            this.columnUom.HeaderText = "单位";
            this.columnUom.MappingName = "Uom";
            this.columnUom.Width = 40;

            // columnSequence
            this.columnSequence = new DataGridTextBoxColumn();
            this.columnSequence.Format = "";
            this.columnSequence.FormatInfo = null;
            this.columnSequence.HeaderText = "顺序号";
            this.columnSequence.MappingName = "Sequence";
            this.columnSequence.Width = 100;

            // columnNote
            this.columnNote = new DataGridTextBoxColumn();
            this.columnNote.Format = "";
            this.columnNote.FormatInfo = null;
            this.columnNote.HeaderText = "备注";
            this.columnNote.MappingName = "Note";
            this.columnNote.Width = 150;

        }
    }
}
