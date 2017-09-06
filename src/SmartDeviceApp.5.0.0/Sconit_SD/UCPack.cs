using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCPack : UCBase
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        private List<GroupHu> groupHus;
        private Location location;
        private string binCode;
        protected DateTime? effDate;

        public UCPack(User user)
            : base(user)
        {
            InitializeComponent();
            base.btnOrder.Text = "装箱";
        }

        protected override void ScanBarCode()
        {
            base.ScanBarCode();

            if (base.op == CodeMaster.BarCodeType.L.ToString())
            {
                base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                var location = smartDeviceService.GetLocation(base.barCode);
                //检查权限
                if (!Utility.HasPermission(user.Permissions, null, false, true, null, location.Region))
                {
                    throw new BusinessException("没有此库位的权限");
                }
                this.location = location;
                this.lblMessage.Text = "当前库位:" + this.location.Code;
            }
            else if (base.op == CodeMaster.BarCodeType.B.ToString())
            {
                base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                Bin bin = smartDeviceService.GetBin(base.barCode);
                //检查权限
                if (!Utility.HasPermission(user.Permissions, OrderType.Transfer, false, true, null, bin.Region))
                {
                    throw new BusinessException("没有此库格的权限");
                }
                this.location = smartDeviceService.GetLocation(bin.Location);
                this.binCode = bin.Code;
                this.lblMessage.Text = "库格:" + bin.Code;
            }
            else if (base.op == CodeMaster.BarCodeType.DATE.ToString())
            {
                base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                this.effDate = base.smartDeviceService.GetEffDate(base.barCode);

                this.lblMessage.Text = "生效时间:" + this.effDate.Value.ToString("yyyy-MM-dd HH:mm");
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
            }
            else if (base.op == CodeMaster.BarCodeType.HU.ToString())
            {
                //nothing todo
            }
            else
            {
                throw new BusinessException("条码格式不合法");
            }


            if (this.location == null)
            {
                throw new BusinessException("请先扫描库位");
            }
            else
            {
                if (base.op == CodeMaster.BarCodeType.HU.ToString())
                {
                    Hu hu = smartDeviceService.GetHu(barCode);
                    if (hu.Qty <= 0)
                    {
                        throw new BusinessException("此条码的数量为0,不能装箱");
                    }
                    if (hu.Status == HuStatus.Location)
                    {
                        throw new BusinessException("条码已在库位中,不能装箱");
                    }
                    if (hu.Status == HuStatus.Ip)
                    {
                        throw new BusinessException("条码已在途,不能装箱");
                    }
                    this.MatchHu(hu);
                }
            }
        }

        protected void MatchHu(Hu hu)
        {
            base.CheckHu(hu);
            if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
            {
                throw new BusinessException("没有此条码的权限");
            }

            if (!base.isCancel)
            {
                GroupHu groupHu = new GroupHu();

                if (groupHus == null)
                {
                    groupHus = new List<GroupHu>();
                }

                var q = groupHus
                    .Where(g => g.Item.Equals(hu.Item, StringComparison.OrdinalIgnoreCase)
                        && g.Uom.Equals(hu.Uom, StringComparison.OrdinalIgnoreCase)
                        && g.UnitCount == hu.UnitCount);
                if (q.Count() > 0)
                {
                    groupHu = q.Single();
                }
                else
                {
                    groupHu.Item = hu.Item;
                    groupHu.ItemDescription = hu.ItemDescription;
                    groupHu.ReferenceItemCode = hu.ReferenceItemCode;
                    groupHu.UnitCount = hu.UnitCount;
                    groupHu.Uom = hu.Uom;

                    var flowDetailList = new List<FlowDetail>();
                    groupHus.Add(groupHu);
                }

                groupHu.CurrentQty += hu.Qty;
                groupHu.Carton++;
                //
                base.hus.Insert(0, hu);
            }
            else
            {
                this.CancelHu(hu);
            }
            this.gvListDataBind();
        }

        protected override void gvListDataBind()
        {
            base.columnIsOdd.Width = 0;
            base.columnLotNo.Width = 0;
            base.gvListDataBind();

            base.dgList.DataSource = groupHus.Where(g => g.CurrentQty > 0).ToList();
            base.ts.MappingName = groupHus.GetType().Name;
        }

        protected override void gvHuListDataBind()
        {
            base.gvHuListDataBind();
        }

        protected override void Reset()
        {
            this.groupHus = new List<GroupHu>();
            base.Reset();
            this.location = null;
            this.binCode = null;
            this.effDate = null;
            this.lblMessage.Text = "正常模式,请扫描库位或库格条码";
        }

        protected override void DoSubmit()
        {
            try
            {
                if (base.hus == null || base.hus.Count == 0)
                {
                    throw new BusinessException("没有装新明细");
                }
                base.smartDeviceService.DoPack(base.hus.Select(h => h.HuId).ToArray(), this.location.Code, this.effDate, this.user.Code);

                string message = "装箱成功";
                if (!string.IsNullOrEmpty(this.binCode))
                {
                    foreach (var hu in base.hus)
                    {
                        try
                        {
                            this.smartDeviceService.DoPutAway(hu.HuId, this.binCode, this.user.Code);
                        }
                        catch (Exception)
                        {
                            message += string.Format("条码{0}上架失败.", hu.HuId);
                        }
                    }
                    message = "装箱并上架成功";
                }
                this.Reset();
                this.lblMessage.Text = message;
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
            Hu firstHu = base.DoCancel();
            this.CancelHu(firstHu);
            return firstHu;
        }

        private void CancelHu(Hu hu)
        {
            if (this.groupHus == null || this.groupHus.Count == 0)
            {
                //this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                this.Reset();
            }
            if (hu != null)
            {
                var q = this.groupHus
                    .Where(g => g.Item == hu.Item
                        && g.Uom == hu.Uom
                        && g.UnitCount == hu.UnitCount);

                if (q.Count() > 0)
                {
                    var firstFlowDetail = q.First();
                    if (firstFlowDetail.CurrentQty >= hu.UnitCount)
                    {
                        firstFlowDetail.CurrentQty -= hu.UnitCount;
                        firstFlowDetail.Carton--;
                        Hu cancelHu = base.hus.FirstOrDefault(h => h.HuId == hu.HuId);
                        base.hus.Remove(cancelHu);
                        this.gvHuListDataBind();
                    }
                    else
                    {
                        throw new BusinessException("没有可取消的物料{0}", hu.Item);
                    }
                }
                else
                {
                    throw new BusinessException("没有可取消的物料{0}", hu.Item);
                }
            }
        }
    }
}
