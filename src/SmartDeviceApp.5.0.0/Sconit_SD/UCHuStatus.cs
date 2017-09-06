using System;
using System.Windows.Forms;
using System.Drawing;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCHuStatus : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private SD_SmartDeviceService smartDeviceService;
        private User user;
        private Hu hu;
        private bool isMark;
        private int btnIndex;

        public UCHuStatus(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.Reset();
        }

        private void tbBarCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.isMark)
            {
                this.isMark = false;
                return;
            }
            try
            {
                if (e == null || (e.KeyData & Keys.KeyCode) == Keys.Enter)
                {
                    this.ScanBarCode();
                }
                else if (((e.KeyData & Keys.KeyCode) == Keys.Escape))
                {
                    if (this.hu != null)
                    {
                        this.Reset();
                    }
                    else
                    {
                        this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                    }
                }
                else if ((e.KeyData & Keys.KeyCode) == Keys.F1)
                {
                    //todo Help
                }
                else if ((e.KeyData & Keys.KeyCode) == Keys.F4)
                {
                    this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                }
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.Reset();
                Utility.ShowMessageBox(ex.Message);
            }
        }

        private void ScanBarCode()
        {
            if (this.tbBarCode.Text.Trim() == string.Empty)
            {
                if (this.btnIndex == 1)
                {
                    this.btnMore2_Click(null, null);
                }
                else if (this.btnIndex == 2)
                {
                    this.btnMore3_Click(null, null);
                }
                else if (this.btnIndex == 3)
                {
                    this.btnMore1_Click(null, null);
                }
            }
            else
            {
                this.hu = smartDeviceService.GetHu(this.tbBarCode.Text.Trim());
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                this.btnMore1_Click(null, null);
            }
        }

        private void Reset()
        {
            this.hu = null; this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.label01.Text = "物料号:";
            this.label02.Text = "参考号:";
            this.label03.Text = "质量状态:";
            this.label04.Text = "数量:";
            this.label05.Text = "单位:";
            this.label06.Text = "库位:";
            this.label07.Text = "库格:";
            this.label08.Text = "状态:";
            this.label09.Text = "制造时间:";
            this.label10.Text = "条码选项:";
            this.lbl01.Text = string.Empty;
            this.lbl02.Text = string.Empty;
            this.lbl03.Text = string.Empty;
            this.lbl04.Text = string.Empty;
            this.lbl05.Text = string.Empty;
            this.lbl06.Text = string.Empty;
            this.lbl07.Text = string.Empty;
            this.lbl08.Text = string.Empty;
            this.lbl09.Text = string.Empty;
            this.lbl10.Text = string.Empty;
            this.lblBarCodeInfo.Text = string.Empty;
            this.lblItemDescInfo.Text = string.Empty;
        }

        private void btnMore1_Click(object sender, EventArgs e)
        {
            this.label01.Text = "物料号:";
            this.label02.Text = "参考号:";
            this.label03.Text = "质量状态:";
            this.label04.Text = "数量:";
            this.label05.Text = "单位:";
            this.label06.Text = "库位:";
            this.label07.Text = "库格:";
            this.label08.Text = "状态:";
            this.label09.Text = "制造时间:";
            this.label10.Text = "条码选项:";
            //
            if (this.hu != null)
            {
                this.lbl01.Text = this.hu.Item;
                this.lbl02.Text = this.hu.ReferenceItemCode;

                if (this.hu.QualityType == QualityType.Inspect)
                {
                    this.lbl03.Text = "待检验";
                    this.lbl03.ForeColor = Color.Yellow;
                }
                else if (this.hu.QualityType == QualityType.Qualified)
                {
                    this.lbl03.Text = "合格";
                    this.lbl03.ForeColor = Color.Green;
                }
                else if (this.hu.QualityType == QualityType.Reject)
                {
                    this.lbl03.Text = "不合格";
                    this.lbl03.ForeColor = Color.Red;
                }

                this.lbl04.Text = this.hu.Qty.ToString("0.########");
                this.lbl05.Text = this.hu.Uom;
                this.lbl06.Text = this.hu.Location;
                this.lbl07.Text = this.hu.Bin;
                if (this.hu.Status == HuStatus.NA)
                {
                    this.lbl08.Text = "无效库存条码";
                    this.lbl08.ForeColor = Color.Red;
                }
                else if (this.hu.Status == HuStatus.Location)
                {
                    this.lbl08.Text = "在库存中";
                    this.lbl08.ForeColor = Color.Black;
                }
                else if (this.hu.Status == HuStatus.Ip)
                {
                    this.lbl08.Text = "在途";
                    this.lbl08.ForeColor = Color.Black;
                }

                this.lbl09.Text = this.hu.ManufactureDate.ToString("yyyy-MM-dd HH:mm");

                if (this.hu.HuOption == HuOption.Aged)
                {
                    this.lbl10.Text = "已老化";
                }
                else if (this.hu.HuOption == HuOption.Filtered)
                {
                    this.lbl10.Text = "已过滤";
                }
                else if (this.hu.HuOption == HuOption.NoNeed)
                {
                    this.lbl10.Text = "无需";
                }
                else if (this.hu.HuOption == HuOption.UnAging)
                {
                    this.lbl10.Text = "未老化";
                }
                else if (this.hu.HuOption == HuOption.UnFilter)
                {
                    this.lbl10.Text = "未过滤";
                }
                this.lblBarCodeInfo.Text = this.hu.HuId;
                this.lblItemDescInfo.Text = this.hu.ItemDescription;
            }
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.btnIndex = 1;
        }

        private void btnMore2_Click(object sender, EventArgs e)
        {
            this.label01.Text = "库存单位:";
            this.label02.Text = "单包装:";
            this.label03.Text = "路线:";
            this.label04.Text = "过期时间:";
            this.label05.Text = "初次入库:";
            this.label06.Text = "打印次数:";
            this.label07.Text = "方向:";
            this.label08.Text = "创建用户:";
            this.label09.Text = "创建时间:";
            this.label10.Text = "区域";
            //
            if (this.hu != null)
            {
                this.lbl01.Text = this.hu.BaseUom;
                this.lbl02.Text = this.hu.UnitCount.ToString("0.########");
                this.lbl03.Text = this.hu.Flow;
                this.lbl03.ForeColor = Color.Black;

                this.lbl04.Text = this.hu.ExpireDate.HasValue ? this.hu.ExpireDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;
                this.lbl05.Text = this.hu.FirstInventoryDate.HasValue ? this.hu.FirstInventoryDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;
                this.lbl06.Text = this.hu.PrintCount.ToString();

                this.lbl07.Text = this.hu.Direction;
                this.lbl08.Text = this.hu.CreateUserName;
                this.lbl09.Text = this.hu.CreateDate.ToString("yyyy-MM-dd");
                this.lbl10.Text = this.hu.Region;
                this.lblBarCodeInfo.Text = this.hu.HuId;
                this.lblItemDescInfo.Text = this.hu.ItemDescription;
            }
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.btnIndex = 2;
        }

        private void btnMore3_Click(object sender, EventArgs e)
        {
            this.label01.Text = "是否冻结:";
            this.label02.Text = "是否寄售:";
            this.label03.Text = "是否ATP:";
            this.label04.Text = "来源库位:";
            this.label05.Text = "目的库位:";
            this.label06.Text = "占用状态:";
            this.label07.Text = "占用订单:";
            this.label08.Text = "参考条码";
            this.label09.Text = "订单号";
            this.label10.Text = "收货单";
            //
            if (this.hu != null)
            {
                this.lbl01.Text = this.hu.IsFreeze ? "是" : "否";
                this.lbl02.Text = this.hu.IsConsignment ? "是" : "否";
                this.lbl03.Text = this.hu.IsATP ? "是" : "否";
                this.lbl03.ForeColor = Color.Black;
                this.lbl04.Text = this.hu.LocationFrom;
                this.lbl05.Text = this.hu.LocationTo;
                if (this.hu.OccupyType== OccupyType.Inspect)
                {
                    this.lbl06.Text = "检验单";
                }
                else  if (this.hu.OccupyType == OccupyType.Pick)
                {
                    this.lbl06.Text = "拣货单";
                }
                else if (this.hu.OccupyType == OccupyType.Sequence)
                {
                    this.lbl06.Text = "排序单";
                }
                else if (this.hu.OccupyType == OccupyType.None)
                {
                    this.lbl06.Text = "无";
                }
                this.lbl07.Text = this.hu.OccupyReferenceNo;
                this.lbl08.Text = this.hu.RefHu;
                this.lbl09.Text = this.hu.OrderNo;
                this.lbl10.Text = this.hu.ReceiptNo;
                this.lblBarCodeInfo.Text = this.hu.HuId;
                this.lblItemDescInfo.Text = this.hu.ItemDescription;
            }
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.btnIndex = 3;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(null, null);
        }

    }
}
