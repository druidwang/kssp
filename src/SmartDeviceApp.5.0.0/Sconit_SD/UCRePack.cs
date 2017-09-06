using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCRePack : UCBase
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        private List<Hu> oldHus;
        private List<Hu> newHus;
        private bool isOldHu;
        private DateTime? effDate;
        private int keyCodeDiff;

        public UCRePack(User user)
            : base(user)
        {
            InitializeComponent();
            base.btnOrder.Text = "翻箱";
            this.isOldHu = true;
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
        }

        protected override void tbBarCode_KeyUp(object sender, KeyEventArgs e)
        {
            base.tbBarCode_KeyUp(sender, e);
            if (sender is TextBox)
            {
                //if ((e.KeyData & Keys.KeyCode) == Keys.F3)
                if (e.KeyValue == 114 + this.keyCodeDiff)
                {
                    this.isOldHu = !this.isOldHu;
                }
            }
        }

        protected override void ScanBarCode()
        {
            base.ScanBarCode();

            if (base.op == CodeMaster.BarCodeType.HU.ToString())
            {
                Hu hu = smartDeviceService.GetHu(barCode);
                if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
                {
                    throw new BusinessException("没有此条码的权限");
                }
                this.MatchHu(hu);
            }
            else
            {
                throw new BusinessException("条码格式不合法");
            }
        }

        private void MatchHu(Hu hu)
        {
            if (hu == null)
            {
                throw new BusinessException("条码不存在");
            }
            if (hu.Qty <= 0)
            {
                throw new BusinessException("条码的数量须大于0");
            }


            hu.CurrentQty = hu.Qty;


            if (hu.IsFreeze)
            {
                throw new BusinessException("条码被冻结!");
            }
            if (hu.OccupyType != OccupyType.None)
            {
                throw new BusinessException("条码被{0}占用!", hu.OccupyReferenceNo);
            }

            if (!base.isCancel)
            {
                if (this.isOldHu) //old
                {
                    if (hu.Status != HuStatus.Location)
                    {
                        throw new BusinessException("旧条码不库位中不能翻箱");
                    }
                    if (this.oldHus.Count > 0 && !this.oldHus[0].Location.Equals(hu.Location, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BusinessException("条码库位{0}和库位{1}不一致不能翻箱", hu.Location, this.oldHus[0].Location);
                    }

                    if (this.oldHus.Count(h => h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase)) > 0)
                    {
                        throw new BusinessException("条码重复扫描!");
                    }
                    if (this.oldHus.Count > 0 && !this.oldHus[0].Item.Equals(hu.Item, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BusinessException("条码物料号{0}不一致不能翻箱", hu.Item);
                    }
                    this.oldHus.Insert(0, hu);
                    this.gvListDataBind();
                }
                else //new
                {
                    if (hu.Status == HuStatus.Ip)
                    {
                        throw new BusinessException("新条码已在途不能装箱");
                    }
                    if (hu.Status == HuStatus.Location)
                    {
                        throw new BusinessException("新条码已在库位中不能装箱");
                    }
                    if (this.newHus.Count(h => h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase)) > 0)
                    {
                        throw new BusinessException("条码重复扫描!");
                    }
                    if (this.newHus.Count > 0 && !this.newHus[0].Item.Equals(hu.Item, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BusinessException("条码物料号{0}不一致不能翻箱", hu.Item);
                    }
                    this.newHus.Insert(0, hu);
                    this.gvHuListDataBind();
                }
            }
            else
            {
                this.CancelHu(hu);
            }
        }

        protected override void gvListDataBind()
        {
            this.hus = this.oldHus;
            base.gvHuListDataBind();
            //base.dgList.DataSource = this.oldHus;
            base.ts.MappingName = this.oldHus.GetType().Name;
            this.isMasterBind = true;
            this.lblMessage.Text = "请扫描旧条码或者F3切换至新条码扫描";
        }

        protected override void gvHuListDataBind()
        {
            this.hus = this.newHus;
            base.gvHuListDataBind();
            //base.dgList.DataSource = this.newHus;
            base.ts.MappingName = this.newHus.GetType().Name;
            this.isMasterBind = false;
            this.lblMessage.Text = "请扫描新条码或者F3切换至旧条码扫描";
        }

        protected override void Reset()
        {
            this.oldHus = new List<Hu>();
            this.newHus = new List<Hu>();
            this.effDate = null;
            base.Reset();
            this.lblMessage.Text = "请扫描旧条码或者按F3扫描新条码";
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
            this.tbBarCode.Focus();
        }

        protected override void DoSubmit()
        {
            try
            {
                if (this.oldHus == null && this.newHus == null)
                {
                    Reset();
                    return;
                }

                if (this.oldHus == null || this.newHus == null)
                {
                    throw new BusinessException("翻箱前条码或翻箱后条码为空");
                }

                if (this.oldHus.Sum(o => o.CurrentQty) != this.newHus.Sum(o => o.CurrentQty))
                {
                    throw new BusinessException("翻箱前条码或翻箱后数量不一致");
                }

                base.smartDeviceService.DoRePack(this.oldHus.Select(h => h.HuId).ToArray(), this.newHus.Select(h => h.HuId).ToArray(), this.effDate, this.user.Code);
                this.Reset();
                this.lblMessage.Text = "翻箱成功";
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
            if (this.newHus == null || this.newHus.Count == 0 || this.oldHus == null || this.oldHus.Count == 0)
            {
                this.Reset();
                return null;
            }
            Hu firstHu = newHus.First();
            if (this.isMasterBind)
            {
                firstHu = oldHus.First();
            }

            this.lblMessage.Text = "已取消条码:" + firstHu.HuId;

            this.CancelHu(firstHu);

            return firstHu;
        }

        private void CancelHu(Hu hu)
        {
            if (this.isOldHu) //old
            {
                var q = this.oldHus
                 .Where(g => g.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));
                if (q.Count() > 0)
                {
                    var firstHu = q.First();
                    this.oldHus.Remove(firstHu);
                    this.gvListDataBind();
                }
                else
                {
                    throw new BusinessException("没有可取消的物料{0}", hu.Item);
                }
            }
            else //new
            {
                var q = this.newHus
                 .Where(g => g.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));
                if (q.Count() > 0)
                {
                    var firstHu = q.First();
                    this.newHus.Remove(firstHu);
                    this.gvHuListDataBind();
                }
                else
                {
                    throw new BusinessException("没有可取消的物料{0}", hu.Item);
                }
            }
        }
    }
}
