using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCSpChk : UCBase
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        private List<Hu> oldHus;
        private List<Hu> newHus;
        private bool isOldHu;
        private DateTime? effDate;
        private int keyCodeDiff;

        public UCSpChk(User user)
            : base(user)
        {
            InitializeComponent();
            base.btnOrder.Text = "Check";
            base.btnOrder.Enabled = false;
            base.btnOrder.Visible = false;
            this.isOldHu = true;
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
        }

        protected override void tbBarCode_KeyUp(object sender, KeyEventArgs e)
        {
            this.isMark = false;
            base.tbBarCode_KeyUp(sender, e);
            if (sender is TextBox)
            {
            }
        }

        protected override void ScanBarCode()
        {
            if (oldHus.Count() == 1)
            {
                DoSubmit();
                return;
            }
            base.ScanBarCode();

            if (base.op == CodeMaster.BarCodeType.HU.ToString())
            {
                Hu hu = smartDeviceService.GetHu(barCode);
                this.MatchHu(hu);
                this.isOldHu = !this.isOldHu;
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


            //if (hu.IsFreeze)
            //{
            //    throw new BusinessException("条码被冻结!");
            //}
            //if (hu.OccupyType != OccupyType.None)
            //{
            //    throw new BusinessException("条码被{0}占用!", hu.OccupyReferenceNo);
            //}

            if (!base.isCancel)
            {
                if (this.isOldHu) //old
                {
                    this.oldHus.Insert(0, hu);
                    this.gvListDataBind();
                }
                else //new
                {

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
            this.lblMessage.Text = "先扫描小料二维码再扫描配料一维码";
        }

        protected override void gvHuListDataBind()
        {
            this.hus = this.newHus;
            base.gvHuListDataBind();
            //base.dgList.DataSource = this.newHus;
            base.ts.MappingName = this.newHus.GetType().Name;
            this.isMasterBind = false;
            this.lblMessage.Text = "先扫描小料二维码再扫描配料一维码";
        }

        protected override void Reset()
        {
            this.oldHus = new List<Hu>();
            this.newHus = new List<Hu>();
            this.effDate = null;
            base.Reset();
            isOldHu = true;
            this.lblMessage.Text = "先扫描小料二维码再扫描配料一维码";
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
            this.tbBarCode.Focus();
        }

        protected override void DoSubmit()
        {
            try
            {
                string spareItem = "";
                if (this.oldHus == null)
                {
                    Reset();
                    return;
                }
                spareItem = tbBarCode.Text;
                base.smartDeviceService.RecSmallChkSparePart(oldHus.ElementAtOrDefault(0).HuId, spareItem, this.user.Code);

                if (this.tbBarCode.Text != oldHus.ElementAtOrDefault(0).Item)
                {
                    throw new BusinessException(string.Format("小料与配料{0}的料号不一致", tbBarCode.Text));
                }
                this.Reset();

                this.lblMessage.Text = string.Format("小料与配料{0}的料号一致",spareItem);
                this.isMark = true;
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
