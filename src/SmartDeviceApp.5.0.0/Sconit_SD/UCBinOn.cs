using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using com.Sconit.SmartDevice.SmartDeviceRef;

namespace com.Sconit.SmartDevice
{
    public partial class UCBinOn : UCBase
    {
        private Bin _bin;
        private static object obj = new object();


        public UCBinOn(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.btnOrder.Text = "上架";
            this.Reset();
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

            if (_bin == null && !this.isCancel)
            {
                if (op == CodeMaster.BarCodeType.B.ToString())
                {
                    barCode = barCode.Substring(2, barCode.Length - 2);
                    this._bin = smartDeviceService.GetBin(barCode);
                    if (this._bin == null)
                    {
                        throw new BusinessException("扫描的库格不存在");
                    }
                    else
                    {
                        this.lblMessage.Text = "请扫描物料/容器条码";
                        this.lblMessage.ForeColor = Color.Black;
                    }
                }
                else
                {
                    throw new BusinessException("请先扫描库格条码");
                }
            }
            else if (!this.isCancel)
            {
                if (op == CodeMaster.BarCodeType.COT.ToString())
                {
                    var huList = smartDeviceService.GetContainerHu(barCode).ToList();
                    this.hus.AddRange(huList);
                }
                else if (op == CodeMaster.BarCodeType.HU.ToString())
                {
                    Hu hu = this.smartDeviceService.GetHu(barCode);
                    if (hu == null)
                    {
                        throw new BusinessException("此条码不存在");
                    }
                    else if (hu.Status != HuStatus.Location)
                    {
                        throw new BusinessException("条码不在库存中,不能装入容器");
                    }
                    else if (hu.IsFreeze)
                    {
                        throw new BusinessException("条码被冻结,不能装入容器");
                    }
                    else if (hu.OccupyType != OccupyType.None)
                    {
                        throw new BusinessException("条码被{0}占用!", hu.OccupyReferenceNo);
                    }
                    else if (!string.IsNullOrEmpty(hu.PalletCode))
                    {
                        throw new BusinessException("条码已与托盘绑定，请扫描托盘。");
                    }
                    else
                    {
                        hus.Add(hu);

                        this.isCancel = false;
                    }
                }
                else if (op == CodeMaster.BarCodeType.TP.ToString())
                {
                    Hu[] huArray = smartDeviceService.GetHuListByPallet(barCode);
                    foreach (Hu hu in huArray)
                    {
                        if (hu == null)
                        {
                            throw new BusinessException("此条码不存在");
                        }
                        else if (hu.Status != HuStatus.Location)
                        {
                            throw new BusinessException("条码不在库存中,不能装入容器");
                        }
                        else if (hu.IsFreeze)
                        {
                            throw new BusinessException("条码被冻结,不能装入容器");
                        }
                        else if (hu.OccupyType != OccupyType.None)
                        {
                            throw new BusinessException("条码被{0}占用!", hu.OccupyReferenceNo);
                        }
                        else
                        {
                            hus.Add(hu);
                            this.isCancel = false;
                        }
                    }
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
            }
            this.gvHuListDataBind();
        }

        protected override void gvHuListDataBind()
        {
            base.gvHuListDataBind();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(null, null);
        }

        protected override void DoSubmit()
        {
            //this.Reset();
            if (hus.Equals(null))
            {
                throw new BusinessException("未扫入物料条码,不可以提交");
            }

            smartDeviceService.OnBin(this._bin.Code, hus.Select(h => h.HuId).ToArray(), this.user.Code);
            this.Reset();
            this.lblMessage.ForeColor = Color.Green;
            this.lblMessage.Text = "操作成功";
            this.isMark = true;
        }

        protected override Hu DoCancel()
        {
            if (this.hus == null || this.hus.Count == 0)
            {
                this.Reset();
                return null;
            }

            Hu firstHu = hus.First();
            this.lblMessage.Text = "已取消条码:" + firstHu.HuId;
            this.lblMessage.ForeColor = Color.Red;
            hus.Remove(firstHu);
            this.gvHuListDataBind();

            return firstHu;
        }

        protected override void Reset()
        {
            this.hus = new List<Hu>();
            this._bin = null;
            base.lblMessage.Text = "请扫描库格条码";
            base.lblMessage.ForeColor = Color.Black;
            //this.lblMessage.Text = string.Empty;
            this.tbBarCode.Text = string.Empty;
            this.isCancel = false;
            this.isMasterBind = true;
            this.lblBarCode.ForeColor = Color.Black;
            this.gvHuListDataBind();
            this.tbBarCode.Focus();
        }
    }
}
