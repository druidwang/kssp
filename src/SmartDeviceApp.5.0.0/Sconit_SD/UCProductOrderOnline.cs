using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;
using System;
using System.Linq;
using System.Drawing;

namespace com.Sconit.SmartDevice
{
    public partial class UCProductOrderOnline : UserControl
    {
        SD_SmartDeviceService smartDeviceService;
        private User user;
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;
        private OrderMaster orderMaster;
        private IList<Hu> huList;
        //private int numOfScan;
        private bool isMark;
        private bool isCancel;
        private Location location;

        private static UCProductOrderOnline ucProductOrderOnline;
        private static object obj = new object();
        private int keyCodeDiff;

        public UCProductOrderOnline(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.Reset();
        }

        public static UCProductOrderOnline GetUCProductOrderOnline(User user)
        {
            if (ucProductOrderOnline == null)
            {
                lock (obj)
                {
                    if (ucProductOrderOnline == null)
                    {
                        ucProductOrderOnline = new UCProductOrderOnline(user);
                    }
                }
            }
            ucProductOrderOnline.user = user;
            return ucProductOrderOnline;
        }


        private void btnBack_Click(object sender, System.EventArgs e)
        {
            this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
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
                        this.ScanBarCode();
                    }
                    else if (((e.KeyData & Keys.KeyCode) == Keys.Escape))
                    {
                        if (!string.IsNullOrEmpty(barCode))
                        {
                            this.tbBarCode.Text = string.Empty;
                        }
                        else
                        {
                            if (huList!=null && huList.Count > 0)
                            {
                                Hu hu = huList.ElementAt(0);
                                huList.Remove(hu);
                                if (huList.Count > 0)
                                {
                                    this.lblBarCodeInfo.Text = huList[0].HuId;
                                    this.lblItemDescInfo.Text = huList[0].ItemDescription;
                                    this.lblRefItemInfo.Text = huList[0].ReferenceItemCode;
                                    this.lblItemInfo.Text = huList[0].Item;
                                }
                            }
                            else
                            {
                                this.Reset();
                            }
                            //this.Reset();
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
                            this.lblMessage.Text = "正常模式.";
                        }
                    }
                    else if ((e.KeyData & Keys.KeyCode) == Keys.F1)
                    {
                        //todo Help
                    }
                }
            }
            catch (SoapException ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                //this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex.Message);
            }
            catch (BusinessException ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                //this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex);
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.Reset();
                Utility.ShowMessageBox(ex.Message);
            }
        }

        private void DoSubmit()
        {
            try
            {
                if (huList.Where(h => !string.IsNullOrEmpty(h.OrderNo)).Count() > 0)
                {
                    if (huList.Count > 1)
                    {
                        throw new BusinessException("子工单或条码数量大于1。");
                    }

                    //smartDeviceService.StartVanOrderFeedOrderNo(this.orderMaster.OrderNo, huList[0].OrderNo, this.user.Code);
                }
                else
                {
                    //smartDeviceService.StartVanOrderFeedHuIds(this.orderMaster.OrderNo, location != null ? location.Code : null, huList.ToArray(), this.user.Code);
                }
                this.Reset();
                this.lblMessage.Text = "上线成功";
            }
            catch (SoapException ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                //this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex.Message);
            }
            catch (BusinessException ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                //this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex);
            }
            catch (Exception ex)
            {
                Utility.ShowMessageBox(ex.Message);
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                //this.tbBarCode.Focus();
            }

            //throw new NotImplementedException();
        }

        private void ScanBarCode()
        {
            string barCode = this.tbBarCode.Text.Trim();
            this.tbBarCode.Focus();
            this.tbBarCode.Text = string.Empty;
            string op = Utility.GetBarCodeType(this.user.BarCodeTypes, barCode);
            if (barCode.Length < 3)
            {
                throw new BusinessException("条码格式不合法");
            }

            if (this.orderMaster == null)
            {
                if (op == CodeMaster.BarCodeType.ORD.ToString())
                {
                    var orderMaster = this.smartDeviceService.GetOrder(barCode, false);

                    if (orderMaster.IsPause || orderMaster.Status != OrderStatus.Submit)
                    {
                        throw new BusinessException("生产单已暂停或状态不正确");
                    }
                    else if (orderMaster.Type != OrderType.Production)
                    {
                        throw new BusinessException("不是生产单不可以上线{0}", orderMaster.OrderNo);
                    }
                    else
                    {
                        if (!Utility.HasPermission(orderMaster, user))
                        {
                            throw new BusinessException("没有操作生产单{0}的权限", orderMaster.OrderNo);
                        }

                        //this.numOfScan = this.numOfScan + 1;
                        this.lblMessage.Text = "请继续扫描条码或者确认上线";
                        this.lblStartTimeInfo.Text = orderMaster.StartTime.ToString("yyyy-MM-dd HH:mm");
                        this.lblFlowInfo.Text = orderMaster.Flow;
                        this.lblSeqInfo.Text = orderMaster.Sequence.ToString();
                        this.lblVANInfo.Text = orderMaster.TraceCode;
                        this.lblWoInfo.Text = orderMaster.OrderNo;
                        this.orderMaster = orderMaster;
                    }
                }
                else
                {
                    this.Reset();
                    this.lblMessage.Text = "";
                    throw new BusinessException("请先扫描生产单");
                }
            }
            else
            {
                if (barCode.Length == 17 && this.smartDeviceService.IsValidLotNo(barCode.Substring(9, 4)) == true)
                {
                    op = CodeMaster.BarCodeType.HU.ToString();
                }
                if (op == CodeMaster.BarCodeType.ORD.ToString())
                {
                    Hu hu = null;// smartDeviceService.GetHuByOrderNo(barCode);
                    if (hu == null)
                    {
                        throw new BusinessException("未找到子生产单{0}的下线条码。", orderMaster.OrderNo);
                    }
                    else if (huList.Count()>0 && huList.Where(h => h.HuId == hu.HuId).Count()>0)
                    {
                        throw new BusinessException("子生产单{0}已扫描。", orderMaster.OrderNo);
                    }

                    if (hu.IsFreeze)
                    {
                        throw new BusinessException("条码被冻结。");
                    }
                    if (hu.OccupyType != OccupyType.None)
                    {
                        throw new BusinessException("条码被{0}占用。", hu.OccupyReferenceNo);
                    }
                    //if (!smartDeviceService.VerifyOrderCompareToHu(this.orderMaster.OrderNo, hu.HuId))
                    //{
                    //    throw new BusinessException("物料{0}不能投到生产单{1}", this.orderMaster.OrderNo, hu.HuId);
                    //}
                    huList.Insert(0, hu);

                    this.lblBarCodeInfo.Text = hu.HuId;
                    this.lblItemDescInfo.Text = hu.ItemDescription;
                    this.lblRefItemInfo.Text = hu.ReferenceItemCode;
                    this.lblItemInfo.Text = hu.Item;
                    return;
                }
                else if (op == CodeMaster.BarCodeType.HU.ToString())
                {
                    Hu hu = new Hu();
                    try
                    {
                        hu = this.smartDeviceService.GetHu(barCode);
                        if (!string.IsNullOrEmpty(hu.OrderNo))
                        {
                            hu.OrderNo = string.Empty;
                        }
                    }
                    catch
                    {
                        if (barCode.Length == 17)
                        {
                            hu = this.smartDeviceService.ResolveHu(barCode, this.user.Code);
                            hu.IsEffective = false;
                        }
                    }
                    if (hu == null)
                    {
                        throw new BusinessException("条码不存在");
                    }
                    hu.CurrentQty = hu.Qty;
                    var matchHu = this.huList.Where(h => h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));
                    if (matchHu != null && matchHu.Count() > 0)
                    {
                        throw new BusinessException("条码重复扫描!");
                    }
                    if (hu.IsFreeze)
                    {
                        throw new BusinessException("条码被冻结!");
                    }
                    if (hu.OccupyType != OccupyType.None)
                    {
                        throw new BusinessException("条码被{0}占用!", hu.OccupyReferenceNo);
                    }
                    if (!Utility.HasPermission(user.Permissions, null, true, false, hu.Region, null))
                    {
                        throw new BusinessException("没有此条码的权限");
                    }
                    //if (hu.Status != HuStatus.Location)
                    //{
                    //    throw new BusinessException("条码不在库存中,不能投料");
                    //}
                    if (hu.Status != HuStatus.Location)
                    {
                        if (this.location == null)
                        {
                            throw new BusinessException("请先扫描库位条码。");
                        }
                        hu.IsEffective = false;
                    }

                    //if (!smartDeviceService.VerifyOrderCompareToHu(this.orderMaster.OrderNo, hu.HuId))
                    //{
                    //    throw new BusinessException("物料{0}不能投到生产单{1}", this.orderMaster.OrderNo, hu.HuId);
                    //}
                    huList.Insert(0, hu);
                    this.lblBarCodeInfo.Text = hu.HuId;
                    this.lblItemDescInfo.Text = hu.ItemDescription;
                    this.lblRefItemInfo.Text = hu.ReferenceItemCode;
                    this.lblItemInfo.Text = hu.Item;
                    return;

                }
                else if (op == CodeMaster.BarCodeType.L.ToString())
                {
                    barCode = barCode.Substring(2, barCode.Length - 2);
                    Location location = smartDeviceService.GetLocation(barCode);

                    //检查权限
                    if (!Utility.HasPermission(user.Permissions, OrderType.Transfer, false, true, null, location.Region))
                    {
                        throw new BusinessException("没有此区域的权限");
                    }
                    this.location = location;
                }
                else
                {
                    throw new BusinessException("条码格式不合法");
                }
            }
        }

        private void Reset()
        {
            this.orderMaster = null;
            this.huList = new List<Hu>();
            this.isCancel = false;
            //this.numOfScan = 0;
            this.lblItemInfo.Text = string.Empty;
            this.lblRefItemInfo.Text = string.Empty;
            this.lblBarCodeInfo.Text = string.Empty;
            this.lblFlowInfo.Text = string.Empty;
            this.lblStartTimeInfo.Text = string.Empty;
            this.lblVANInfo.Text = string.Empty;
            this.lblWoInfo.Text = string.Empty;
            this.lblSeqInfo.Text = string.Empty;
            this.lblItemDescInfo.Text = string.Empty;
            this.tbBarCode.Text = string.Empty;
            this.lblMessage.Text = string.Empty;
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (this.orderMaster == null)
            {
                this.Reset();
                this.lblMessage.Text = "请扫描生产单";
                this.lblMessage.ForeColor = Color.Red;
                return;
            }
            //else if (huList == null || huList.Count == 0)
            //{
            //    this.lblMessage.Text = "请扫描上线条码或者子生产单";
            //    this.lblMessage.ForeColor = Color.Red;
            //    return;
            //}
            this.tbBarCode_KeyUp(sender, null);
            //this.DoSubmit();
        }
    }
}
