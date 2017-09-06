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
    public partial class UCMisInOut : UCBase
    {
        private MiscOrderMaster miscOrderMaster;

        private static UCMisInOut ucMisInOut;
        private static object obj = new object();


        public UCMisInOut(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.lblMessage.Text = "请扫描计划外出/入库单";
            base.btnOrder.Text = "确定";
        }

        public static UCMisInOut GetUCMisInOut(User user)
        {
            if (ucMisInOut == null)
            {
                lock (obj)
                {
                    if (ucMisInOut == null)
                    {
                        ucMisInOut = new UCMisInOut(user);
                    }
                }
            }
            ucMisInOut.user = user;
            ucMisInOut.lblMessage.Text = "请扫描计划外出/入库单";
            ucMisInOut.Reset();
            return ucMisInOut;
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
                    var miscOrderMaster = smartDeviceService.GetMisOrder(barCode);
                    if (miscOrderMaster == null)
                    {
                        throw new BusinessException("扫描的计划外出库单不存在");
                    }
                    else if (miscOrderMaster.Status!=MiscOrderStatus.Create)
                    {
                        if (miscOrderMaster.Type == MiscOrderType.GI)
                        {
                            throw new BusinessException("计划外出库单{0}状态不是创建", miscOrderMaster.MiscOrderNo);
                        }
                        else
                        {
                            throw new BusinessException("计划外入库单{0}状态不是创建", miscOrderMaster.MiscOrderNo);
                        }
                    }
                    else if (!miscOrderMaster.IsScanHu)
                    {
                        if (miscOrderMaster.Type == MiscOrderType.GI)
                        {
                            throw new BusinessException("计划外出库单{0}不需要扫描物料条码", miscOrderMaster.MiscOrderNo);
                        }
                        else
                        {
                            throw new BusinessException("计划外入库单{0}不需要扫描物料条码", miscOrderMaster.MiscOrderNo);
                        }                  
                    }
                    else
                    {
                        this.miscOrderMaster = miscOrderMaster;
                        this.lblMessage.Text = "请扫描物料条码";
                    }
                }
                else
                {
                    throw new BusinessException("请先扫描计划外出/入库单");
                }
            }
            else if (!this.isCancel)
            {
                if (op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if (this.miscOrderMaster.Type == MiscOrderType.GI)
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
                        else if (hu.IsFreeze)
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
                        if (hus.Where(h => h.HuId == barCode).ToList().Count > 0)
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
                            throw new BusinessException("条码已在库存中,不能入库");
                        }
                        else if (hu.IsFreeze)
                        {
                            throw new BusinessException("条码被冻结,不能入库");
                        }
                        else if (this.miscOrderMaster.QualityType != hu.QualityType)
                        {
                            throw new BusinessException("条码的质量状态不符合计划外入库单质量状态");
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


        protected override void DoSubmit()
        {

            try
            {
                if (hus.Equals(null) || hus.Count == 0)
                {
                    throw new BusinessException("未扫入物料条码,不可以提交");
                }

                smartDeviceService.BatchUpdateMiscOrderDetails(this.miscOrderMaster.MiscOrderNo, hus.Select(h => h.HuId).ToArray(), this.user.Code);
                if (this.miscOrderMaster.Type == MiscOrderType.GI)
                {
                    this.lblMessage.Text = "出库成功";
                }
                else
                {
                    this.lblMessage.Text = "入库成功";
                }
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
                this.lblMessage.Text = "请扫描计划外出/入库单";
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
            //this.lblMessage.Text = string.Empty;
            this.tbBarCode.Text = string.Empty;
            this.isCancel = false;
            this.isMasterBind = true;
            this.tbBarCode.Focus();
        }
    }
}
