using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;
using System.Drawing;
using com.Sconit.SmartDevice.CodeMaster;

namespace com.Sconit.SmartDevice
{
    public partial class UCMaterialReturn : UserControl
    {
        public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        protected DataGridTableStyle ts;
        protected DataGridTextBoxColumn columnHuId;
        private DataGridTextBoxColumn columnCurrentQty;
        private DataGridTextBoxColumn columnItemDescription;
        private DataGridTextBoxColumn columnItemCode;
        private DataGridTextBoxColumn columnReferenceItemCode;
        private DataGridTextBoxColumn columnUnitCount;
        private DataGridTextBoxColumn columnUom;
        private DataGridTextBoxColumn columnCarton;
        protected DataGridTextBoxColumn columnIsOdd;
        protected DataGridTextBoxColumn columnLotNo;

        private User user;
        private OrderMaster orderMaster;
        //private OrderMaster subOrderMaster;
        private List<Hu> hus;
        private bool isMark;
        private bool isCancel;
        private string barCode;
        private string op;
        private string facilityCode;
        private DateTime? effDate;
        private FlowMaster flowMaster;
        private string[][] huDetails;
        private MaterialReturnType materialReturnType;

        private SD_SmartDeviceService smartDeviceService;
        private int keyCodeDiff;

        public UCMaterialReturn(User user)
        {
            InitializeComponent();
            this.smartDeviceService = new SD_SmartDeviceService();
            this.smartDeviceService.Url = Utility.WEBSERVICE_URL;
            this.user = user;
            this.InitializeDataGrid();
            this.Reset();
        }

        private void tbBarCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.isMark)
            {
                this.isMark = false;
                this.tbBarCode.Focus();
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
                        if (barCode != string.Empty)
                        {
                            this.ScanBarCode();
                        }
                        else
                        {
                            this.btnOrder.Focus();
                        }
                    }
                    else if (((e.KeyData & Keys.KeyCode) == Keys.Escape))
                    {
                        if (!string.IsNullOrEmpty(barCode))
                        {
                            this.tbBarCode.Text = string.Empty;
                        }
                        else
                        {
                            this.DoCancel();
                        }
                    }
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F4)
                    else if (e.KeyValue == 115 + this.keyCodeDiff)
                    {
                        this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                    }
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F3)
                    else if (e.KeyValue == 114 + this.keyCodeDiff)
                    {
                        if (this.tabPanel.SelectedIndex == 0)
                        {
                            this.tabPanel.SelectedIndex = 1;
                        }
                        else if (this.tabPanel.SelectedIndex == 1)
                        {
                            this.tabPanel.SelectedIndex = 0;
                        }
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
                    //else if ((e.KeyData & Keys.KeyCode) == Keys.F1)
                    else if (e.KeyValue == 112 + this.keyCodeDiff)
                    {
                        //todo Help
                    }
                }
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
                if ((ex is System.Net.WebException) || (ex is SoapException))
                {
                    Utility.ShowMessageBox(ex);
                }
                else if (ex is BusinessException)
                {
                    Utility.ShowMessageBox(ex.Message);
                }
                else
                {
                    this.Reset();
                    Utility.ShowMessageBox(ex.Message);
                }
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
            }
        }

        private void DoCancel()
        {
            this.Reset();
            this.lblMessage.Text = "已全部取消";
        }

        private void DoSubmit()
        {
            try
            {
                if (this.orderMaster == null && this.flowMaster == null)
                {
                    throw new BusinessException("请扫描生产单或者生产线。");
                }

                if (this.orderMaster != null)
                {
                    this.materialReturnType = MaterialReturnType.WO2Hu;
                }
                else
                {
                    this.materialReturnType = MaterialReturnType.Flow2Hu;
                }

                switch (this.materialReturnType)
                {
                    case MaterialReturnType.Flow2Hu:
                        if (this.flowMaster == null)
                        {
                            throw new BusinessException("没有扫描生产线");
                        }
                        if (this.hus == null || this.hus.Count == 0)
                        {
                            throw new BusinessException("没有投料条码");
                        }
                        //var huIds = this.hus.Select(h => h.HuId).ToArray();
                        this.huDetails = new string[hus.Count][];
                        for (int i = 0; i < hus.Count; i++)
                        {
                            huDetails[i] = new string[2];
                            huDetails[i][0] = hus[i].HuId;
                            huDetails[i][1] = hus[i].Qty.ToString();
                        }
                        this.smartDeviceService.ReturnProdLineRawMaterial(this.flowMaster.Code, this.facilityCode, huDetails, this.effDate, this.user.Code);
                        break;
                    case MaterialReturnType.WO2Hu:
                        if (this.orderMaster == null)
                        {
                            throw new BusinessException("没有扫描生产单");
                        }
                        if (this.hus == null || this.hus.Count == 0)
                        {
                            throw new BusinessException("没有投料条码");
                        }
                        this.huDetails = new string[hus.Count][];
                        for (int i = 0; i < hus.Count; i++)
                        {
                            huDetails[i] = new string[2];
                            huDetails[i][0] = hus[i].HuId;
                            huDetails[i][1] = hus[i].Qty.ToString();
                        }
                        this.smartDeviceService.ReturnOrderRawMaterial(this.orderMaster.OrderNo, this.orderMaster.TraceCode, null, null, huDetails, this.effDate, this.user.Code);
                        break;
                    default:
                        break;
                }

                this.Reset();
                this.lblMessage.Text = "投料成功";
            }
            catch (Exception ex)
            {
                this.isMark = true;
                this.tbBarCode.Text = string.Empty;
                this.tbBarCode.Focus();
                Utility.ShowMessageBox(ex.Message);
            }
        }

        private void ScanBarCode()
        {
            this.barCode = this.tbBarCode.Text.Trim();
            this.lblMessage.Text = string.Empty;
            this.tbBarCode.Text = string.Empty;

            if (this.barCode.Length < 3)
            {
                throw new BusinessException("条码格式不合法");
            }
            this.op = Utility.GetBarCodeType(this.user.BarCodeTypes, this.barCode);

            if (this.orderMaster == null && this.flowMaster == null)
            {
                if (this.op == CodeMaster.BarCodeType.ORD.ToString())
                {
                    if (this.orderMaster == null)
                    {
                        this.Reset();
                        this.lblSeq.Text = "序号:";
                        this.lblFlow.Text = "产线:";
                        this.lblWo.Text = "工单:";
                        var orderMaster = this.smartDeviceService.GetOrder(this.barCode, true);
                        if (orderMaster.IsPause)
                        {
                            throw new BusinessException("订单已暂停");
                        }

                        //检查订单状态
                        if (orderMaster.Status != OrderStatus.InProcess)
                        {
                            throw new BusinessException("不是InProcess状态不能操作");
                        }

                        if (orderMaster.Type == OrderType.Production)
                        {
                            if (orderMaster.OrderDetails.Length != 1)
                            {
                                throw new BusinessException("只能对一料一单的生产单操作");
                            }

                            OrderDetail orderDetail = orderMaster.OrderDetails[0];

                            this.lblSeqInfo.Text = orderMaster.Sequence.ToString();
                            this.lblFlowInfo.Text = orderMaster.Flow;
                            this.lblWoInfo.Text = orderMaster.OrderNo;
                            this.lblVANInfo.Text = orderMaster.TraceCode;
                            this.lblFgInfo.Text = orderDetail.Item;
                            this.lblFgDescInfo.Text = orderDetail.ItemDescription;
                            this.tabPanel.SelectedIndex = 0;
                        }
                        else
                        {
                            throw new BusinessException("订单类型不正确:{0}", orderMaster.Type.ToString());
                        }

                        #region 界面控制
                        this.lblSeq.Visible = true;
                        this.lblFlow.Visible = true;
                        this.lblWo.Visible = true;
                        this.lblVAN.Visible = true;
                        this.lblFg.Visible = true;
                        this.lblFgDescription.Visible = true;
                        #endregion
                        this.orderMaster = orderMaster;
                    }
                }
                else if (this.op == CodeMaster.BarCodeType.Z.ToString() || this.op == CodeMaster.BarCodeType.F.ToString())
                {
                    this.Reset();
                    this.barCode = this.barCode.Substring(2, this.barCode.Length - 2);
                    if (this.op == CodeMaster.BarCodeType.F.ToString())
                    {
                        this.flowMaster = smartDeviceService.GetFlowMaster(this.barCode, false);
                    }
                    else
                    {
                        //this.flowMaster = smartDeviceService.GetFlowMasterByFacility(this.barCode, false);
                        if (flowMaster != null)
                        {
                            this.facilityCode = this.barCode;
                        }
                        throw new Exception("CodeMaster.BarCodeType.Z.");
                    }

                    //检查订单类型
                    if (this.flowMaster.Type != OrderType.Production)
                    {
                        throw new BusinessException("不是生产线设备不能退料。");
                    }

                    //是否有效
                    if (!this.flowMaster.IsActive)
                    {
                        throw new BusinessException("此生产线无效。");
                    }

                    //检查权限
                    //if (!Utility.HasPermission(this.flowMaster, this.user))
                    //{
                    //    throw new BusinessException("没有此生产线的权限");
                    //}
                    this.lblMessage.Text = this.flowMaster.Description;
                    //this.gvListDataBind();

                    this.lblSeq.Text = "产线:";
                    this.lblSeq.Visible = true;
                    this.lblSeqInfo.Text = this.flowMaster.Code;
                    this.lblFlow.Text = "描述:";
                    this.lblFlow.Visible = true;
                    this.lblFlowInfo.Text = this.flowMaster.Description;
                    this.lblWo.Text = "区域:";
                    this.lblWo.Visible = true;
                    this.lblWoInfo.Text = this.flowMaster.PartyFrom;

                    this.lblVANInfo.Text = string.Empty;

                    this.lblFgInfo.Text = string.Empty;

                    this.lblFgDescInfo.Text = string.Empty;
                    this.tabPanel.SelectedIndex = 0;
                }
            }
            else
            {
                if (this.barCode.Length == 17 && this.smartDeviceService.IsValidLotNo(this.barCode.Substring(9, 4)) == true)
                {
                    this.op = CodeMaster.BarCodeType.HU.ToString();
                }
                if (this.op == CodeMaster.BarCodeType.HU.ToString())
                {
                    if (this.hus == null)
                    {
                        this.hus = new List<Hu>();
                    }
                    if (this.flowMaster == null && this.orderMaster == null)
                    {
                        throw new BusinessException("请先扫描生产单或生产线");
                    }

                    Hu hu = smartDeviceService.GetHu(this.barCode);
                    if (hu == null)
                    {
                        throw new BusinessException("条码不存在");
                    }
                    hu.CurrentQty = hu.Qty;
                    var matchHu = this.hus.Where(h => h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));
                    if (matchHu != null && matchHu.Count() > 0)
                    {
                        throw new BusinessException("条码重复扫描!");
                    }
                    if (hu.Status == HuStatus.Location)
                    {
                        throw new BusinessException("条码在库存中,不能退料");
                    }

                    if (this.orderMaster != null)
                    {
                        this.hus.Insert(0, hu);
                        //this.lblVANInfo1.Text = this.orderMaster.TraceCode;
                        this.gvHuListDataBind();

                    }
                    else if (this.flowMaster != null)
                    {
                        this.hus.Insert(0, hu);
                        this.gvHuListDataBind();
                    }
                    else
                    {
                        this.Reset();
                        throw new BusinessException("请先扫描生产单或生产线");
                    }

                    this.tabPanel.SelectedIndex = 1;
                }


                else if (this.op == CodeMaster.BarCodeType.DATE.ToString())
                {
                    this.barCode = this.barCode.Substring(2, this.barCode.Length - 2);
                    this.effDate = this.smartDeviceService.GetEffDate(this.barCode);

                    this.lblMessage.Text = "生效时间:" + this.effDate.Value.ToString("yyyy-MM-dd HH:mm");
                    this.tbBarCode.Text = string.Empty;
                    this.tbBarCode.Focus();
                }
                else
                {
                    throw new BusinessException("条码格式不合法。");
                }
            }
            #region 旧代码
            //if (this.op == CodeMaster.BarCodeType.ORD.ToString())
            //{
            //    if (this.orderMaster == null)
            //    {
            //        this.Reset();
            //        this.lblSeq.Text = "序号:";
            //        this.lblFlow.Text = "产线:";
            //        this.lblWo.Text = "工单:";
            //        var orderMaster = this.smartDeviceService.GetOrder(this.barCode, true);
            //        if (orderMaster.IsPause)
            //        {
            //            throw new BusinessException("订单已暂停");
            //        }

            //        //检查订单状态
            //        if (orderMaster.Status != OrderStatus.InProcess)
            //        {
            //            throw new BusinessException("不是InProcess状态不能操作");
            //        }

            //        if (orderMaster.Type == OrderType.Production)
            //        {
            //            if (orderMaster.OrderDetails.Length != 1)
            //            {
            //                throw new BusinessException("只能对一料一单的生产单操作");
            //            }

            //            OrderDetail orderDetail = orderMaster.OrderDetails[0];

            //            this.lblSeqInfo.Text = orderMaster.Sequence.ToString();
            //            this.lblFlowInfo.Text = orderMaster.Flow;
            //            this.lblWoInfo.Text = orderMaster.OrderNo;
            //            this.lblVANInfo.Text = orderMaster.TraceCode;
            //            this.lblFgInfo.Text = orderDetail.Item;
            //            this.lblFgDescInfo.Text = orderDetail.ItemDescription;
            //            this.tabPanel.SelectedIndex = 0;
            //        }
            //        else
            //        {
            //            throw new BusinessException("订单类型不正确:{0}", orderMaster.Type.ToString());
            //        }

            //        #region 界面控制
            //        this.lblSeq.Visible = true;
            //        this.lblFlow.Visible = true;
            //        this.lblWo.Visible = true;
            //        this.lblVAN.Visible = true;
            //        this.lblFg.Visible = true;
            //        this.lblFgDescription.Visible = true;
            //        #endregion
            //        this.orderMaster = orderMaster;
            //    }
            //}

            //else if (this.op == CodeMaster.BarCodeType.HU.ToString())
            //{
            //    if (this.hus == null)
            //    {
            //        this.hus = new List<Hu>();
            //    }
            //    if (this.flowMaster == null && this.orderMaster == null)
            //    {
            //        throw new BusinessException("请先扫描生产单或生产线");
            //    }

            //    Hu hu = smartDeviceService.GetHu(this.barCode);
            //    if (hu == null)
            //    {
            //        throw new BusinessException("条码不存在");
            //    }
            //    hu.CurrentQty = hu.Qty;
            //    var matchHu = this.hus.Where(h => h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase));
            //    if (matchHu != null && matchHu.Count() > 0)
            //    {
            //        throw new BusinessException("条码重复扫描!");
            //    }
            //    if (hu.Status == HuStatus.Location)
            //    {
            //        throw new BusinessException("条码在库存中,不能退料");
            //    }

            //    if (this.orderMaster != null)
            //    {
            //        this.hus.Insert(0, hu);
            //        //this.lblVANInfo1.Text = this.orderMaster.TraceCode;
            //        this.gvHuListDataBind();

            //    }
            //    else if (this.flowMaster != null)
            //    {
            //        this.hus.Insert(0, hu);
            //        this.gvHuListDataBind();
            //    }
            //    else
            //    {
            //        this.Reset();
            //        throw new BusinessException("请先扫描生产单或生产线");
            //    }

            //    this.tabPanel.SelectedIndex = 1;
            //}
            

            //else if (this.op == CodeMaster.BarCodeType.DATE.ToString())
            //{
            //    this.barCode = this.barCode.Substring(2, this.barCode.Length - 2);
            //    this.effDate = this.smartDeviceService.GetEffDate(this.barCode);

            //    this.lblMessage.Text = "生效时间:" + this.effDate.Value.ToString("yyyy-MM-dd HH:mm");
            //    this.tbBarCode.Text = string.Empty;
            //    this.tbBarCode.Focus();
            //}
            //else if (this.op == CodeMaster.BarCodeType.Z.ToString() || this.op == CodeMaster.BarCodeType.F.ToString())
            //{
            //    this.Reset();
            //    this.barCode = this.barCode.Substring(2, this.barCode.Length - 2);
            //    if (this.op == CodeMaster.BarCodeType.F.ToString())
            //    {
            //        this.flowMaster = smartDeviceService.GetFlowMaster(this.barCode, false);
            //    }
            //    else
            //    {
            //        this.flowMaster = smartDeviceService.GetFlowMasterByFacility(this.barCode, false);
            //        if (flowMaster != null)
            //        {
            //            this.facilityCode = this.barCode;
            //        }
            //    }

            //    //检查订单类型
            //    if (this.flowMaster.Type != OrderType.Production)
            //    {
            //        throw new BusinessException("不是生产线设备不能退料。");
            //    }

            //    //是否有效
            //    if (!this.flowMaster.IsActive)
            //    {
            //        throw new BusinessException("此生产线无效。");
            //    }

            //    //检查权限
            //    //if (!Utility.HasPermission(this.flowMaster, this.user))
            //    //{
            //    //    throw new BusinessException("没有此生产线的权限");
            //    //}
            //    this.lblMessage.Text = this.flowMaster.Description;
            //    //this.gvListDataBind();

            //    this.lblSeq.Text = "产线:";
            //    this.lblSeq.Visible = true;
            //    this.lblSeqInfo.Text = this.flowMaster.Code;
            //    this.lblFlow.Text = "描述:";
            //    this.lblFlow.Visible = true;
            //    this.lblFlowInfo.Text = this.flowMaster.Description;
            //    this.lblWo.Text = "区域:";
            //    this.lblWo.Visible = true;
            //    this.lblWoInfo.Text = this.flowMaster.PartyFrom;

            //    this.lblVANInfo.Text = string.Empty;

            //    this.lblFgInfo.Text = string.Empty;

            //    this.lblFgDescInfo.Text = string.Empty;
            //    this.tabPanel.SelectedIndex = 0;
            //}
            //else
            //{
            //    throw new BusinessException("条码格式不合法1");
            //}
            #endregion
        }

        private void gvHuListDataBind()
        {
            List<Hu> hus = new List<Hu>();
            if (this.hus != null)
            {
                hus = this.hus;
            }

            this.ts = new DataGridTableStyle();
            this.ts.MappingName = hus.GetType().Name;

            this.ts.GridColumnStyles.Add(columnHuId);
            this.ts.GridColumnStyles.Add(columnCurrentQty);
            this.ts.GridColumnStyles.Add(columnLotNo);
            this.ts.GridColumnStyles.Add(columnUnitCount);
            this.ts.GridColumnStyles.Add(columnUom);
            this.ts.GridColumnStyles.Add(columnReferenceItemCode);
            this.ts.GridColumnStyles.Add(columnItemDescription);

            this.dgDetail.TableStyles.Clear();
            this.dgDetail.TableStyles.Add(this.ts);

            this.dgDetail.DataSource = hus;
            this.ResumeLayout();

            this.dgDetail.Visible = true;
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
        }

        private void Reset()
        {
            this.orderMaster = null;
            this.hus = new List<Hu>();
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.lblMessage.Text = string.Empty;

            this.isCancel = false;
            this.lblBarCode.ForeColor = Color.Black;
            this.tabPanel.SelectedIndex = 0;

            #region tab1
            this.lblSeqInfo.Text = string.Empty;
            this.lblFlowInfo.Text = string.Empty;
            this.lblWoInfo.Text = string.Empty;
            this.lblVANInfo.Text = string.Empty;
            this.lblFgInfo.Text = string.Empty;
            this.lblFgDescInfo.Text = string.Empty;


            this.lblSeq.Visible = false;
            this.lblFlow.Visible = false;
            this.lblWo.Visible = false;
            this.lblVAN.Visible = false;
            this.lblFg.Visible = false;
            this.lblFgDescription.Visible = false;
            #endregion

            this.dgDetail.Visible = false;
            //this.lblMessage.Text ="已全部取消";
            this.keyCodeDiff = Utility.GetKeyCodeDiff();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this.tbBarCode_KeyUp(sender, null);
        }


        private void tbBarCode_LostFocus(object sender, EventArgs e)
        {
            if (!this.btnOrder.Focused)
            {
                this.tbBarCode.Focus();
            }
        }

        private void btnOrder_KeyUp(object sender, KeyEventArgs e)
        {
            this.tbBarCode_KeyUp(sender, e);
        }

        private void InitializeDataGrid()
        {
            // 
            // columnItemCode
            // 
            this.columnItemCode = new DataGridTextBoxColumn();
            this.columnItemCode.Format = "";
            this.columnItemCode.FormatInfo = null;
            this.columnItemCode.HeaderText = "物料";
            this.columnItemCode.MappingName = "Item";
            this.columnItemCode.Width = 100;
            // 
            // columnLotNo
            // 
            this.columnLotNo = new DataGridTextBoxColumn();
            this.columnLotNo.Format = "";
            this.columnLotNo.FormatInfo = null;
            this.columnLotNo.HeaderText = "批号";
            this.columnLotNo.MappingName = "LotNo";
            this.columnLotNo.Width = 40;
            // 
            // columnCurrentQty
            // 
            this.columnCurrentQty = new DataGridTextBoxColumn();
            this.columnCurrentQty.Format = "0.##";
            this.columnCurrentQty.FormatInfo = null;
            this.columnCurrentQty.HeaderText = "数量";
            this.columnCurrentQty.MappingName = "CurrentQty";
            this.columnCurrentQty.Width = 40;
            // 
            // columnCarton
            // 
            this.columnCarton = new DataGridTextBoxColumn();
            this.columnCarton.Format = "";
            this.columnCarton.FormatInfo = null;
            this.columnCarton.HeaderText = "箱数";
            this.columnCarton.MappingName = "Carton";
            this.columnCarton.Width = 40;
            // 
            // columnUnitCount
            // 
            this.columnUnitCount = new DataGridTextBoxColumn();
            this.columnUnitCount.Format = "0.##";
            this.columnUnitCount.FormatInfo = null;
            this.columnUnitCount.HeaderText = "单包装";
            this.columnUnitCount.MappingName = "UnitCount";
            this.columnUnitCount.Width = 40;
            // 
            // columnUom
            // 
            this.columnUom = new DataGridTextBoxColumn();
            this.columnUom.Format = "";
            this.columnUom.FormatInfo = null;
            this.columnUom.HeaderText = "单位";
            this.columnUom.MappingName = "Uom";
            this.columnUom.Width = 40;
            // 
            // columnIsOdd
            // 
            this.columnIsOdd = new DataGridTextBoxColumn();
            this.columnIsOdd.Format = "";
            this.columnIsOdd.FormatInfo = null;
            this.columnIsOdd.HeaderText = "零头";
            this.columnIsOdd.MappingName = "IsOdd";
            this.columnIsOdd.Width = 40;
            // 
            // columnReferenceItemCode
            // 
            this.columnReferenceItemCode = new DataGridTextBoxColumn();
            this.columnReferenceItemCode.Format = "";
            this.columnReferenceItemCode.FormatInfo = null;
            this.columnReferenceItemCode.HeaderText = "参考物料";
            this.columnReferenceItemCode.MappingName = "ReferenceItemCode";
            this.columnReferenceItemCode.Width = 100;
            // 
            // columnItemDescription
            // 
            this.columnItemDescription = new DataGridTextBoxColumn();
            this.columnItemDescription.Format = "";
            this.columnItemDescription.FormatInfo = null;
            this.columnItemDescription.HeaderText = "描述";
            this.columnItemDescription.MappingName = "ItemDescription";
            this.columnItemDescription.Width = 150;
            // 
            // columnHuId
            // 
            this.columnHuId = new DataGridTextBoxColumn();
            this.columnHuId.Format = "";
            this.columnHuId.FormatInfo = null;
            this.columnHuId.HeaderText = "条码";
            this.columnHuId.MappingName = "HuId";
            this.columnHuId.NullText = "";
            this.columnHuId.Width = 150;

        }

    }
}
