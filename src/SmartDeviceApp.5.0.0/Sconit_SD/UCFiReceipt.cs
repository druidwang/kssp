using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;
using System.Collections;
using System.Drawing;

namespace com.Sconit.SmartDevice
{
    public partial class UCFiReceipt : UCBase
    {
        public new event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        private static UCFiReceipt ucFiReceipt;
        private static object obj = new object();
        private int keyCodeDiff;

        public UCFiReceipt(User user)
            : base(user)
        {
            this.InitializeComponent();
        }

        public static UCFiReceipt GetUCFiReceipt(User user)
        {
            if (ucFiReceipt == null)
            {
                lock (obj)
                {
                    if (ucFiReceipt == null)
                    {
                        ucFiReceipt = new UCFiReceipt(user);
                    }
                }
            }
            ucFiReceipt.user = user;
            ucFiReceipt.Reset();
            ucFiReceipt.lblMessage.Text = "请扫描物料条码";
            return ucFiReceipt;
        }

        protected override void tbBarCode_KeyUp(object sender, KeyEventArgs e)
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
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F2 || (e.KeyData & Keys.KeyCode) == Keys.F5)
                    else if (e.KeyValue == 114 + this.keyCodeDiff)
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
                            this.lblMessage.Text = "正常模式";
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


        protected override void ScanBarCode()
        {
            string barCode = this.tbBarCode.Text.Trim();
            this.tbBarCode.Focus();
            this.tbBarCode.Text = string.Empty;
            string op = Utility.GetBarCodeType(this.user.BarCodeTypes, barCode);

            if (barCode.Length < 3)
            {
                throw new BusinessException("条码格式不合法");
            }

            if (!this.isCancel)
            {
                if (op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if (hus.Where(h => h.HuId == barCode).Count() > 0)
                    {
                        throw new BusinessException("请不要重复扫描条码");
                    }
                    Hu hu = this.smartDeviceService.GetHu(barCode);
                    if (hu == null)
                    {
                        throw new BusinessException("此条码不存在");
                    }
                    else if (hu.Status == HuStatus.Location)
                    {
                        throw new BusinessException("此条码{0}已入库", hu.HuId);
                    }
                    else if (string.IsNullOrEmpty(hu.OrderNo))
                    {
                        throw new BusinessException("此条码{0}没有对应的生产单号", hu.HuId);
                    }
                    //if (!string.IsNullOrEmpty(hu.LocationTo))
                    //{
                    //    hu.Region = smartDeviceService.GetLocation(hu.LocationTo).Region;
                    //    if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
                    //    {
                    //        throw new BusinessException("没有此条码的权限");
                    //    }
                    //}
                    hu.CurrentQty = hu.Qty;
                    this.hus.Insert(0, hu);
                    //detailStringArray.Add(new string[] { hu.HuId, this.binCode, this.locationCode != null ? this.locationCode : string.Empty });
                    base.gvHuListDataBind();

                    this.isCancel = false;
                    this.lblMessage.Text = "请继续扫描物料条码";
                }
                else
                {
                    throw new BusinessException("条码格式不合法");
                }
            }
            else
            {
                if (op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if (hus.Where(h => h.HuId == barCode).ToList().Count > 0)
                    {
                        this.hus = this.hus.Where(h => h.HuId != barCode).ToList();
                        base.gvHuListDataBind();
                    }
                    else
                    {
                        throw new BusinessException("条码{0}未扫入不需取消", barCode);
                    }
                }
                else
                {
                    throw new BusinessException("请扫描需要取消的条码");
                }
            }
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(null, null);
        }

        protected override void DoSubmit()
        {
            try
            {
                if (hus.Count == 0 || hus == null)
                {
                    this.tbBarCode.Focus();
                    this.isMark = true;
                    //this.lblMessage.Text = "请扫描物料条码";
                    return;//throw new BusinessException("未扫入物料条码,不可以提交");
                }
                this.smartDeviceService.DoFiReceipt(this.hus.Select(h => h.HuId).ToArray(), this.user.Code);
                this.Reset();
                this.lblMessage.Text = "生产入库成功";
                this.isMark = true;
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex.Message);
            }
        }

        protected override Hu DoCancel()
        {
            if (this.hus == null || this.hus.Count == 0)
            {
                this.Reset();
                return null;
            }

            Hu firstHu = hus.First();
            hus.Remove(firstHu);
            this.lblMessage.ForeColor = Color.Red;
            this.lblMessage.Text = string.Format("当前取消条码{0}", firstHu.HuId);
            this.gvHuListDataBind();
            return firstHu;
        }

        protected override void Reset()
        {
            this.hus = new List<Hu>();
            this.gvHuListDataBind();
            this.lblMessage.Text = string.Empty;
            this.tbBarCode.Text = string.Empty;
            this.isCancel = false;
            this.isMasterBind = true;
            this.tbBarCode.Focus();
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
        }
    }
}
