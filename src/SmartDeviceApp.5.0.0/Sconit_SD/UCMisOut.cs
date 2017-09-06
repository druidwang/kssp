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
    public partial class UCMisOut : UCBase
    {
        private MiscOrderMaster miscOrderMaster;

        private static UCMisOut ucMisOut;
        private static object obj = new object();


        private UCMisOut(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.btnOrder.Text = "出库";
            base.lblMessage.Text = "请扫描计划外出库单";
            base.lblMessage.ForeColor = Color.Black;
        }

        public static UCMisOut GetUCMisOut(User user)
        {
            if (ucMisOut == null)
            {
                lock (obj)
                {
                    if (ucMisOut == null)
                    {
                        ucMisOut = new UCMisOut(user);
                    }
                }
            }
            ucMisOut.user = user;
            ucMisOut.Reset();
            return ucMisOut;
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

            if (this.miscOrderMaster == null && !this.isCancel)
            {
                if (op == CodeMaster.BarCodeType.MIS.ToString())
                {
                    this.miscOrderMaster = smartDeviceService.GetMisOrder(barCode);
                    if (this.miscOrderMaster == null)
                    {
                        throw new BusinessException("扫描的计划外出库单不存在");
                    }
                    else if (this.miscOrderMaster.Status!=MiscOrderStatus.Create)
                    {
                        throw new BusinessException("计划外出库单{0}状态不是创建", this.miscOrderMaster.MiscOrderNo);
                    }
                    else if (!this.miscOrderMaster.IsScanHu)
                    {
                        throw new BusinessException("计划外出库单{0}不需要扫描物料条码", this.miscOrderMaster.MiscOrderNo);
                    }
                    else
                    {
                        this.lblMessage.Text = "请扫描物料条码";
                        this.lblMessage.ForeColor = Color.Black;
                    }
                }
                else
                {
                    throw new BusinessException("请先扫描计划外出库单");
                }
            }
            else if (!this.isCancel)
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
                        throw new BusinessException("条码不在库存中,不能出库");
                    }
                    else if(hu.IsFreeze)
                    {
                        throw new BusinessException("条码被冻结,不能出库");
                    }
                    else if (this.miscOrderMaster.QualityType != hu.QualityType)
                    {
                        throw new BusinessException("条码的质量状态不符合计划外出库单质量状态");
                    }
                    else if (!string.IsNullOrEmpty(this.miscOrderMaster.Region) && this.miscOrderMaster.Region != hu.Region)
                    {
                        throw new BusinessException("条码的区域不符合计划外出库单区域");
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
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(null, null);
        }

        protected override void DoSubmit()
        {
            if (hus.Equals(null))
            {
                throw new BusinessException("未扫入物料条码,不可以提交");
            }

            smartDeviceService.BatchUpdateMiscOrderDetails(this.miscOrderMaster.MiscOrderNo, hus.Select(h => h.HuId).ToArray(),this.user.Code);
            this.Reset();
            this.lblMessage.ForeColor = Color.Green;
            this.lblMessage.Text = "出库成功";
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
            this.miscOrderMaster = null;
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
