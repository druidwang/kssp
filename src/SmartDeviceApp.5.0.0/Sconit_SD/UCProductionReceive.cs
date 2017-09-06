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
    public partial class UCProductionReceive : UCBase
    {
        private MiscOrderMaster miscOrderMaster;

        private static UCProductionReceive ucProductionReceive;
        private static object obj = new object();
        private Location location;
        private Bin bin;


        public UCProductionReceive(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.lblMessage.Text = "请先扫描库位或库格";
            base.btnOrder.Text = "确定";
        }

        public static UCProductionReceive GetUCProductionReceive(User user)
        {
            if (ucProductionReceive == null)
            {
                lock (obj)
                {
                    if (ucProductionReceive == null)
                    {
                        ucProductionReceive = new UCProductionReceive(user);
                    }
                }
            }
            ucProductionReceive.user = user;
            ucProductionReceive.lblMessage.Text = "请扫描成品入库单";
            ucProductionReceive.Reset();
            return ucProductionReceive;
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(null, null);
        }

        private void tbBarCode_LostFocus(object sender, EventArgs e)
        {
            if (!this.btnOrder.Focused)
            {
                this.tbBarCode.Focus();
            }
        }


        protected override void ScanBarCode()
        {
            base.ScanBarCode();

            if (bin == null && location == null)
            {
                if (base.op == CodeMaster.BarCodeType.B.ToString())
                {

                    base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                    bin = smartDeviceService.GetBin(base.barCode);

                    this.lblMessage.Text = "目的库格:" + bin.Code;
                }
                else if (base.op == CodeMaster.BarCodeType.L.ToString())
                {

                    base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                    location = smartDeviceService.GetLocation(base.barCode);

                    this.lblMessage.Text = "目的库位:" + location.Code;
                }
            }

            else if (op == CodeMaster.BarCodeType.HU.ToString() || op == CodeMaster.BarCodeType.TP.ToString())
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
                    if (!hu.IsPallet)
                    {
                        if (!string.IsNullOrEmpty(hu.PalletCode))
                        {
                            throw new BusinessException("条码已与托盘绑定，请扫描托盘。");
                        }
                        else if (hu.IsExternal)
                        {
                            throw new BusinessException("外部条码请扫描托盘入库。");
                        }

                        else if (hu.Status == HuStatus.Location)
                        {
                            throw new BusinessException("条码已在库存中,不能入库");
                        }
                        else if (hu.IsFreeze)
                        {
                            throw new BusinessException("条码被冻结,不能入库");
                        }
                        else if (hu.OccupyType != OccupyType.None)
                        {
                            throw new BusinessException("条码被{0}占用!", hu.OccupyReferenceNo);
                        }
                        else
                        {
                            hu.CurrentQty = hu.Qty;
                            hus.Add(hu);
                            this.gvHuListDataBind();
                            this.isCancel = false;
                        }
                    }
                    else
                    {
                        op = CodeMaster.BarCodeType.TP.ToString();
                    }
                }

                if (op == CodeMaster.BarCodeType.TP.ToString())
                {

                    Hu[] huArray = smartDeviceService.GetHuListByPallet(barCode);
                    foreach (Hu hu in huArray)
                    {
                        if (hus.Where(h => h.HuId == hu.HuId).ToList().Count > 0)
                        {
                            throw new BusinessException("请不要重复扫描条码");
                        }

                        else if (hu.Status == HuStatus.Location)
                        {
                            throw new BusinessException("条码已在库存中,不能入库");
                        }
                        else if (hu.IsFreeze)
                        {
                            throw new BusinessException("条码被冻结,不能入库");
                        }
                        else if (hu.OccupyType != OccupyType.None)
                        {
                            throw new BusinessException("条码被{0}占用!", hu.OccupyReferenceNo);
                        }
                        else
                        {
                            hus.Add(hu);
                            this.gvHuListDataBind();
                            this.isCancel = false;
                        }
                    }
                }

            }

         
            else
            {
                throw new BusinessException("条码格式不合法");
            }
           
        }


        protected override void DoSubmit()
        {

            try
            {
                if (hus.Equals(null) || hus.Count == 0)
                {
                    throw new BusinessException("未扫入物料条码,不可以提交");
                }
                smartDeviceService.QuickCreateMiscOrder(hus.Select(h => h.HuId).ToArray(),location == null ? string.Empty:location.Code,bin == null? string.Empty:bin.Code,1,this.user.Code);

                this.lblMessage.Text = "入库成功";

                this.Reset();
                this.isMark = true;
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

        protected override Hu DoCancel()
        {
            if (this.hus == null || this.hus.Count == 0)
            {
                this.Reset();
                this.lblMessage.Text = "请扫描条码";
                return null;
            }

            Hu firstHu = hus.First();
            this.lblMessage.Text = "已取消条码:" + firstHu.HuId;
            hus.Remove(firstHu);
            this.gvHuListDataBind();

            return firstHu;
        }

        protected override void Reset()
        {
            this.hus = new List<Hu>();
            this.miscOrderMaster = null;
            this.gvHuListDataBind();
            this.location = null;
            this.bin = null;
            this.tbBarCode.Text = string.Empty;
            this.isCancel = false;
            this.isMasterBind = true;
            this.tbBarCode.Focus();
        }
    }
}
