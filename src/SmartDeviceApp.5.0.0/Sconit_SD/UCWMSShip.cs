using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCWMSShip : UCBase
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        private DataGridTextBoxColumn columnIpNo;
        private DataGridTextBoxColumn columnBoxCount;
        private DataGridTextBoxColumn columnShipFrom;
        private DataGridTextBoxColumn columnShipTo;
        private static UCWMSShip ucWMSShip;
        private static object obj = new object();
        private TransportOrderMaster transOrderMaster;
        private DateTime? effDate;

        public UCWMSShip(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.btnOrder.Text = "发货";
        }

        public static UCWMSShip GetUCWMSShip(User user)
        {
            if (ucWMSShip == null)
            {
                lock (obj)
                {
                    if (ucWMSShip == null)
                    {
                        ucWMSShip = new UCWMSShip(user);
                    }
                }
            }
            ucWMSShip.user = user;
            ucWMSShip.Reset();
            return ucWMSShip;
        }


        protected override void ScanBarCode()
        {
            base.ScanBarCode();

            if (base.op == CodeMaster.BarCodeType.T.ToString())
            {
                var transOrder = this.smartDeviceService.GetTransOrder(base.barCode);
                if (transOrder != null && transOrder.TransportOrderDetailList != null)
                {
                    this.transOrderMaster = transOrder;
                    this.gvListDataBind();
                }
                
            }
            else if (base.op == CodeMaster.BarCodeType.HU.ToString() || base.op == CodeMaster.BarCodeType.DC.ToString())
            {
                if (this.transOrderMaster == null )
                {
                    DialogResult dr = MessageBox.Show("本次发货不扫描运单发货?", "非运单发货", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.No)
                    {
                        this.tbBarCode.Focus();
                        return;
                    }
                    //throw new BusinessException("请先扫描订单");
                }

                Hu hu = smartDeviceService.GetShipHu(base.op == CodeMaster.BarCodeType.HU.ToString() ? base.barCode:string.Empty , base.op == CodeMaster.BarCodeType.HU.ToString() ? string.Empty : base.barCode);

                this.hus.Add(hu);
            }
            else if (base.op == CodeMaster.BarCodeType.DATE.ToString())
            {
                base.barCode = base.barCode.Substring(2, base.barCode.Length - 2);
                this.effDate = base.smartDeviceService.GetEffDate(base.barCode);

                this.lblMessage.Text = "生效时间:" + this.effDate.Value.ToString("yyyy-MM-dd HH:mm");
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
            }
            else
            {
                throw new BusinessException("条码格式不合法");
            }
        }

        protected override void Reset()
        {
            this.transOrderMaster = new TransportOrderMaster();
            base.Reset();
            this.lblMessage.Text = "请扫描运单或条码";
            this.effDate = null;
        }

        protected override void DoSubmit()
        {
            try
            {

                this.smartDeviceService.DoShipWMS((this.transOrderMaster != null ? this.transOrderMaster.OrderNo : string.Empty), this.hus.Select(h => h.HuId).ToArray(), this.user.Code);
                //string ipNo = this.smartDeviceService.DoShipOrder(orderDetailInputList.ToArray(), this.effDate, this.user.Code);
                this.Reset();
                base.lblMessage.Text = string.Format("发货成功");
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

        private void MatchHu(Hu hu)
        {
            base.CheckHu(hu);

            this.gvHuListDataBind();
        }



        protected override Hu DoCancel()
        {
            Hu firstHu = base.DoCancel();
            this.CancelHu(firstHu);
            return firstHu;
        }

        private void CancelHu(Hu hu)
        {
            //if (this.orderMasters == null || this.orderMasters.Count() == 0)
            if (this.hus == null)
            {
                //this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                this.Reset();
                return;
            }

            if (hu != null)
            {
                base.hus = base.hus.Where(h => !h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase)).ToList();
                this.gvHuListDataBind();
            }
        }

        protected override void gvListDataBind()
        {
            this.tbBarCode.Text = string.Empty;
            //this.tbBarCode.Focus();
            //this.dgList.DataSource = details;
            //ts.MappingName = details.GetType().Name;
            this.ts = new DataGridTableStyle();

            this.ts.GridColumnStyles.Add(columnIpNo);
            this.ts.GridColumnStyles.Add(columnBoxCount);
            this.ts.GridColumnStyles.Add(columnShipFrom);
            //this.ts.GridColumnStyles.Add(columnManufactureParty);
            this.ts.GridColumnStyles.Add(columnShipTo);
            //this.ts.GridColumnStyles.Add(columnIsOdd);


            this.dgList.TableStyles.Clear();
            this.dgList.TableStyles.Add(this.ts);
            this.dgList.DataSource = this.transOrderMaster.TransportOrderDetailList;
            this.ts.MappingName = this.transOrderMaster.TransportOrderDetailList.GetType().Name;

            this.ResumeLayout();
            this.isMasterBind = true;
        }

        private void InitializeDataGrid()
        {
            this.columnIpNo = new DataGridTextBoxColumn();
            this.columnIpNo.Format = "";
            this.columnIpNo.FormatInfo = null;
            this.columnIpNo.HeaderText = "送货单号";
            this.columnIpNo.MappingName = "IpNo";
            this.columnIpNo.Width = 50;

            this.columnBoxCount = new DataGridTextBoxColumn();
            this.columnBoxCount.Format = "";
            this.columnBoxCount.FormatInfo = null;
            this.columnBoxCount.HeaderText = "箱数";
            this.columnBoxCount.MappingName = "BoxCount";
            this.columnBoxCount.Width = 50;

            this.columnShipFrom = new DataGridTextBoxColumn();
            this.columnShipFrom.Format = "";
            this.columnShipFrom.FormatInfo = null;
            this.columnShipFrom.HeaderText = "来源";
            this.columnShipFrom.MappingName = "ShipFrom";
            this.columnShipFrom.Width = 50;

            this.columnShipTo = new DataGridTextBoxColumn();
            this.columnShipTo.Format = "";
            this.columnShipTo.FormatInfo = null;
            this.columnShipTo.HeaderText = "目的";
            this.columnShipTo.MappingName = "ShipTo";
            this.columnShipTo.Width = 50;

        }
    }
}
