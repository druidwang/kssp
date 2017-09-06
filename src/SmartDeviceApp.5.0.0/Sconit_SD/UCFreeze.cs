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
    public partial class UCFreeze : UCBase
    {
        public new event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        private static UCFreeze ucFreeze;
        private static object obj = new object();
        private int keyCodeDiff;

        public UCFreeze(User user)
            : base(user)
        {
            this.InitializeComponent();
        }

        public static UCFreeze GetUCFreeze(User user)
        {
            if (ucFreeze == null)
            {
                lock (obj)
                {
                    if (ucFreeze == null)
                    {
                        ucFreeze = new UCFreeze(user);
                    }
                }
            }
            ucFreeze.user = user;
            ucFreeze.Reset();
            ucFreeze.lblMessage.Text = "请扫描需要冻结的条码";
            return ucFreeze;
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
                            this.lblMessage.Text = "正常模式";
                        }
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
                    if (hus.Where(h => h.HuId == barCode).ToList().Count > 0)
                    {
                        throw new BusinessException("请不要重复扫描条码");
                    }
                    Hu hu = this.smartDeviceService.GetHu(barCode);
                    if (hu == null)
                    {
                        throw new BusinessException("此条码不存在");
                    }
                    else if (hu.Status != HuStatus.Location)
                    {
                        throw new BusinessException("条码{0}不在库存中",hu.HuId);
                    }
                    else if (hu.IsFreeze == true)
                    {
                        throw new BusinessException("条码{0}已经被冻结", hu.HuId);
                    }
                    if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
                    {
                        throw new BusinessException("没有此条码的权限");
                    }
                    hu.CurrentQty = hu.Qty;
                    this.hus.Insert(0, hu);
                    //detailStringArray.Add(new string[] { hu.HuId, this.binCode, this.locationCode != null ? this.locationCode : string.Empty });
                    base.gvHuListDataBind();

                    this.isCancel = false;
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
                if (hus == null || hus.Count==0)
                {
                    this.Reset();
                    return;
                    //throw new BusinessException("未扫入物料条码,不可以提交");
                }
                this.smartDeviceService.InventoryFreeze(this.hus.Select(h => h.HuId).ToArray(), this.user.Code);
                this.Reset();
                this.lblMessage.Text = "冻结成功";
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
            if (this.hus == null || this.hus.Count==0)
            {
                this.Reset();
                return null;
            }

            Hu firstHu = hus.First();
            hus.Remove(firstHu);
            this.lblMessage.ForeColor = Color.Red;
            this.lblMessage.Text = string.Format("当前取消条码{0}",firstHu.HuId);
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
            this.lblBarCode.ForeColor = Color.Black;
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
            this.tbBarCode.Focus();
        }

    }
}
