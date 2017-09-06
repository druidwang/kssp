namespace com.Sconit.Web.Controllers.ORD
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using AutoMapper;
    using com.Sconit.Entity.BIL;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity.SCM;
    using com.Sconit.Entity.SYS;
    using com.Sconit.PrintModel.ORD;
    using com.Sconit.Service;
    using com.Sconit.Utility;
    using com.Sconit.Utility.Report;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using NHibernate.Criterion;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity;
    using System.ComponentModel;
    using com.Sconit.Entity.MRP.ORD;
    using com.Sconit.Web.Models.SearchModels.MRP;
    using com.Sconit.Entity.MRP.MD;

    public class DistributionOrderController : WebAppBaseController
    {
        private static string selectCountStatement = "select count(*) from OrderMaster as o";
        private static string selectStatement = "select o from OrderMaster as o";
        private static string selectFlowDetailStatement = "select f from FlowDetail as f where f.Flow = ? order by f.Sequence asc,f.Id asc";
        private static string selectOrderDetailStatement = "select d from OrderDetail as d where d.OrderNo=? order by d.Sequence asc,d.Id asc";
        private static string selectOneFlowDetailStatement = "select d from FlowDetail as d where d.Flow = ? and d.Item = ?";

        private static string selectReceiptCountStatement = "select count(*) from OrderDetail as d";
        private static string selectReceiptStatement = "select d from OrderDetail as d";

        //public IGenericMgr genericMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        //public IFlowMgr flowMgr { get; set; }
        //public IReportGen reportGen { get; set; }
        //public IItemMgr itemMgr { get; set; }

        public DistributionOrderController()
        {
        }

        #region edit

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_QuickNew")]
        public ActionResult QuickNew()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderDetail_Distribution")]
        public ActionResult DetailIndex()
        {
            TempData["OrderMasterSearchModel"] = null;
            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderDetail_Distribution")]
        public ActionResult ReturnDetailIndex()
        {
            return View();
        }


        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_ReturnNew")]
        public ActionResult ReturnNew()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_View")]
        public ActionResult Index()
        {
            OrderMasterSearchModel searchModel = new OrderMasterSearchModel();
            searchModel.DateType = "0";
            searchModel.DateFrom = DateTime.Today.AddDays(-7);
            searchModel.DateTo = DateTime.Today;
            searchModel.MultiStatus = "0,1,2";
            TempData["OrderMasterSearchModel"] = searchModel;
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_View")]
        public ActionResult List(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_View")]
        public ActionResult _AjaxList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }

            if (searchModel.QueryOrderType == 1)
            {
                searchModel.ExternalOrderNo = searchModel.OrderNo;
                searchModel.OrderNo = null;
            }
            if (searchModel.QueryOrderType == 2)
            {
                searchModel.ReferenceOrderNo = searchModel.OrderNo;
                searchModel.OrderNo = null;
            }
            //string whereStatement = " where o.Type in (" + (int)com.Sconit.CodeMaster.OrderType.Distribution + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + "," + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + ")"
            //          + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            //SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel, whereStatement);
            //return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
            string whereStatement = string.Empty;
            searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Edit")]
        public ActionResult New()
        {
            ViewBag.flow = null;
            ViewBag.orderNo = null;
            ViewBag.HasIsIndepentDemandPermssion = false;
            if (this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Distribution_New_ShowIndependent"))
            {
                ViewBag.HasIsIndepentDemandPermssion = true;
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Edit,Url_OrderMstr_Distribution_QuickNew,Url_OrderMstr_Distribution_ReturnNew")]
        public JsonResult NewCreateOrderFromPlan(OrderMaster orderMaster,
             string[] Sequences, string[] Items, string[] UnitCounts, string[] Uoms,
             string[] LocationFroms, string[] LocationTos, string[] OrderedQtys, string[] HuTos, string[] Remarks)
        {
            DateTime planDate = DateTime.Parse(orderMaster.ExternalOrderNo + " 00:00:00");
            IList<object> param = new List<object>();
            string hql = "  from MrpPlan as m where m.Flow=? and m.PlanDate=? ";
            param.Add(orderMaster.Flow);
            param.Add(planDate);

            IList<MrpPlan> mrpPlanList = genericMgr.FindAll<MrpPlan>(hql, param.ToArray());

            var view = NewCreateOrder(orderMaster, Sequences, Items, UnitCounts, Uoms, LocationFroms, LocationTos, OrderedQtys, HuTos, Remarks);

            for (int i = 0; i < Items.Length; i++)
            {
                string item = Items[i];
                double qty = Convert.ToDouble(OrderedQtys[i]);
                var plan = mrpPlanList.Single(p => p.Item == item);
                plan.OrderQty += qty;
                this.genericMgr.Update(plan);
            }
            return view;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Edit,Url_OrderMstr_Distribution_QuickNew,Url_OrderMstr_Distribution_ReturnNew")]
        public JsonResult NewCreateOrder(OrderMaster orderMaster,
             string[] Sequences, string[] Items, string[] UnitCounts, string[] Uoms,
             string[] LocationFroms, string[] LocationTos, string[] OrderedQtys, string[] HuTos, string[] Remarks)
        {
            try
            {
                if (Items == null || Items.Length == 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_LackDetailCanNotCreateOrder);
                    return Json(null);
                }

                IList<OrderDetail> orderDetList = new List<OrderDetail>();
                for (int i = 0; i < Items.Length; i++)
                {
                    OrderDetail od = new OrderDetail();
                    od.Sequence = Convert.ToInt32(Sequences[i]);
                    od.Item = Items[i];
                    od.UnitCount = Convert.ToDecimal(UnitCounts[i]);
                    od.Uom = Uoms[i];
                    od.LocationFrom = LocationFroms[i];
                    //od.LocationTo = LocationTosArray[i];
                    od.Direction = HuTos[i];
                    if (Remarks != null && Remarks.Length > 0)
                    {
                        od.Remark = Remarks[i];
                    }
                    if (LocationTos != null && LocationTos.Length > 0)
                    {
                        od.LocationTo = LocationTos[i] == string.Empty ? null : LocationTos[i];
                    }
                    od.OrderedQty = Convert.ToDecimal(OrderedQtys[i]);


                    //把外部订单号记到明细上，现在要查询
                    od.ExternalOrderNo = orderMaster.ExternalOrderNo;

                    orderDetList.Add(od);
                }

                if (string.IsNullOrEmpty(orderMaster.Flow))
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_Flow);
                }
                if (orderMaster.WindowTime == DateTime.MinValue)
                {
                    orderMaster.WindowTime = DateTime.Now;
                    //throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_WindowTime);
                }
                if (orderMaster.StartTime == DateTime.MinValue)
                {
                    orderMaster.StartTime = DateTime.Now;
                    //throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_StartTime);
                }
                #region orderDetailList
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                IList<FlowDetail> flowDetailList = flowMgr.GetFlowDetailList(orderMaster.Flow, false, true);

                foreach (OrderDetail orderDetail in orderDetList)
                {
                    OrderDetail newOrderDetail = RefreshOrderDetail(flowDetailList, orderDetail, orderMaster.SubType);
                    orderDetailList.Add(newOrderDetail);
                }
                #endregion

                if (orderDetailList.Count == 0)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_OrderDetailIsEmpty);
                }

                FlowMaster flow = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);
                if (orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    flow = flowMgr.GetReverseFlow(flow, null);
                    //  this.genericMgr.CleanSession();
                }
                DateTime effectiveDate = orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value : DateTime.Now;
                OrderMaster newOrder = orderMgr.TransferFlow2Order(flow, null, effectiveDate, false);
                newOrder.IsQuick = orderMaster.IsQuick;

                newOrder.WindowTime = orderMaster.IsQuick ? DateTime.Now : orderMaster.WindowTime;
                newOrder.StartTime = orderMaster.IsQuick ? DateTime.Now : orderMaster.StartTime;
                newOrder.ReferenceOrderNo = orderMaster.ReferenceOrderNo;
                newOrder.ExternalOrderNo = orderMaster.ExternalOrderNo;
                newOrder.Priority = orderMaster.Priority;
                newOrder.IsIndepentDemand = orderMaster.IsIndepentDemand;
                newOrder.QualityType = orderMaster.QualityType;
                if (orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    newOrder.SubType = orderMaster.SubType;
                    if (newOrder.IsQuick)
                    {
                        newOrder.IsShipScanHu = false;
                        newOrder.IsReceiveScanHu = false;
                    }
                }
                newOrder.OrderDetails = orderDetailList;
                orderMgr.CreateOrder(newOrder);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Added, newOrder.OrderNo);
                return Json(new { orderNo = newOrder.OrderNo });
            }
            catch (BusinessException ex)
            {
                //Response.TrySkipIisCustomErrors = true;
                //Response.StatusCode = 500;
                //Response.Write(ex.GetMessages()[0].GetMessageString());
                SaveErrorMessage(ex);
                return Json(null);
            }

        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_View")]
        public ActionResult Edit(string orderNo, int? SubType)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.SubType = SubType;
                return View("Edit", string.Empty, orderNo);
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_View")]
        public ActionResult _Edit(string orderNo)
        {
            OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);

            ViewBag.isEditable = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create && this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Distribution_Edit");
            ViewBag.editorTemplate = ViewBag.isEditable ? "" : "ReadonlyTextBox";

            return PartialView(orderMaster);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Edit")]
        public ActionResult _Edit(OrderMaster orderMaster)
        {
            try
            {
                #region 默认值
                OrderMaster oldOrderMaster = genericMgr.FindById<OrderMaster>(orderMaster.OrderNo);
                #region BillAddress赋值

                if (oldOrderMaster.BillAddress != orderMaster.BillAddress)
                {
                    oldOrderMaster.BillAddressDescription = genericMgr.FindById<Address>(orderMaster.BillAddress).AddressContent;
                }
                #endregion

                #region ShipFrom赋值
                if (oldOrderMaster.ShipFrom != orderMaster.ShipFrom)
                {
                    Address shipFrom = genericMgr.FindById<Address>(orderMaster.ShipFrom);
                    oldOrderMaster.ShipFromAddress = shipFrom.AddressContent;
                    oldOrderMaster.ShipFromCell = shipFrom.MobilePhone;
                    oldOrderMaster.ShipFromTel = shipFrom.TelPhone;
                    oldOrderMaster.ShipFromFax = shipFrom.Fax;
                    oldOrderMaster.ShipFromContact = shipFrom.ContactPersonName;
                }
                #endregion

                #region ShipTo赋值
                if (oldOrderMaster.ShipTo != orderMaster.ShipTo)
                {
                    Address shipTo = genericMgr.FindById<Address>(orderMaster.ShipTo);
                    oldOrderMaster.ShipToAddress = shipTo.AddressContent;
                    oldOrderMaster.ShipToCell = shipTo.MobilePhone;
                    oldOrderMaster.ShipToTel = shipTo.TelPhone;
                    oldOrderMaster.ShipToFax = shipTo.Fax;
                    oldOrderMaster.ShipToContact = shipTo.ContactPersonName;
                }
                #endregion

                #region LocationFrom赋值
                if (!string.IsNullOrEmpty(orderMaster.LocationFrom) && (oldOrderMaster.LocationFrom != orderMaster.LocationFrom))
                {
                    oldOrderMaster.LocationFrom = orderMaster.LocationFrom;
                    oldOrderMaster.LocationFromName = genericMgr.FindById<Location>(orderMaster.LocationFrom).Name;
                }
                #endregion

                #region LocationTo赋值
                if (!string.IsNullOrEmpty(orderMaster.LocationTo) && (oldOrderMaster.LocationTo != orderMaster.LocationTo))
                {
                    oldOrderMaster.LocationTo = orderMaster.LocationTo;
                    oldOrderMaster.LocationToName = genericMgr.FindById<Location>(orderMaster.LocationTo).Name;
                }
                #endregion

                #region PriceList赋值
                if (oldOrderMaster.PriceList != orderMaster.PriceList)
                {
                    oldOrderMaster.Currency = genericMgr.FindById<PriceListMaster>(orderMaster.PriceList).Currency;
                }
                #endregion

                #region 新增值
                oldOrderMaster.Priority = orderMaster.Priority;
                oldOrderMaster.IsOpenOrder = orderMaster.IsOpenOrder;
                oldOrderMaster.Sequence = orderMaster.Sequence;
                oldOrderMaster.WindowTime = orderMaster.WindowTime;
                oldOrderMaster.StartTime = orderMaster.StartTime;
                oldOrderMaster.Dock = orderMaster.Dock;
                oldOrderMaster.BillTerm = orderMaster.BillTerm;
                oldOrderMaster.Currency = orderMaster.Currency;
                oldOrderMaster.HuTemplate = orderMaster.HuTemplate;
                oldOrderMaster.OrderTemplate = orderMaster.OrderTemplate;
                oldOrderMaster.AsnTemplate = orderMaster.AsnTemplate;
                oldOrderMaster.ReceiptTemplate = orderMaster.ReceiptTemplate;
                oldOrderMaster.IsReceiveScanHu = orderMaster.IsReceiveScanHu;
                oldOrderMaster.IsShipScanHu = orderMaster.IsShipScanHu;
                oldOrderMaster.IsReceiveFulfillUC = orderMaster.IsReceiveFulfillUC;
                oldOrderMaster.IsPrintOrder = orderMaster.IsPrintOrder;
                oldOrderMaster.IsPrintAsn = orderMaster.IsPrintAsn;
                oldOrderMaster.IsPrintReceipt = orderMaster.IsPrintReceipt;
                oldOrderMaster.IsOrderFulfillUC = orderMaster.IsOrderFulfillUC;
                oldOrderMaster.IsShipFulfillUC = orderMaster.IsShipFulfillUC;
                oldOrderMaster.IsReceiveFulfillUC = orderMaster.IsReceiveFulfillUC;
                oldOrderMaster.IsManualCreateDetail = orderMaster.IsManualCreateDetail;
                oldOrderMaster.IsListPrice = orderMaster.IsListPrice;
                oldOrderMaster.IsCreatePickList = orderMaster.IsCreatePickList;
                oldOrderMaster.IsShipByOrder = orderMaster.IsShipByOrder;
                oldOrderMaster.IsReceiveExceed = orderMaster.IsReceiveExceed;
                oldOrderMaster.IsShipExceed = orderMaster.IsShipExceed;
                oldOrderMaster.IsAsnUniqueReceive = orderMaster.IsAsnUniqueReceive;
                oldOrderMaster.IsAutoRelease = orderMaster.IsAutoRelease;
                oldOrderMaster.IsAutoStart = orderMaster.IsAutoStart;
                oldOrderMaster.IsAutoShip = orderMaster.IsAutoShip;
                oldOrderMaster.IsAutoReceive = orderMaster.IsAutoReceive;
                oldOrderMaster.IsAutoBill = orderMaster.IsAutoBill;
                oldOrderMaster.IsInspect = orderMaster.IsInspect;

                #endregion
                #endregion

                orderMgr.UpdateOrder(oldOrderMaster);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Saved, orderMaster.OrderNo);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { orderNo = orderMaster.OrderNo });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Delete")]
        public ActionResult Delete(string id)
        {
            try
            {
                orderMgr.DeleteOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Deleted, id);
                return RedirectToAction("List");
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return RedirectToAction("Edit", new { orderNo = id });
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Submit")]
        public ActionResult Submit(string id)
        {
            try
            {
                orderMgr.ReleaseOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Submited, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Start")]
        public ActionResult Start(string id)
        {
            try
            {
                orderMgr.StartOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Started, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Close")]
        public ActionResult Close(string id)
        {
            try
            {
                orderMgr.ManualCloseOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Closed, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Cancel")]
        public ActionResult Cancel(string id)
        {
            try
            {
                orderMgr.CancelOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Canceled, id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_View")]
        public ActionResult _OrderDetailList(string flow, string orderNo, int? orderSubType)
        {
            ViewBag.isManualCreateDetail = false;
            ViewBag.flow = flow;
            ViewBag.orderNo = orderNo;
            ViewBag.newOrEdit = "New";
            ViewBag.status = null;
            ViewBag.orderSubType = (orderSubType != null && orderSubType.Value == (int)com.Sconit.CodeMaster.OrderSubType.Return) ? com.Sconit.CodeMaster.OrderSubType.Return : com.Sconit.CodeMaster.OrderSubType.Normal;

            if (!string.IsNullOrEmpty(flow))
            {
                FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(flow);

                ViewBag.PartyFrom = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? flowMaster.PartyFrom : flowMaster.PartyTo;
                ViewBag.PartyTo = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? flowMaster.PartyTo : flowMaster.PartyFrom;
                ViewBag.isManualCreateDetail = flowMaster.IsManualCreateDetail;
                ViewBag.status = com.Sconit.CodeMaster.OrderStatus.Create;
                ViewBag.IsListPrice = flowMaster.IsListPrice;
                ViewBag.IsDistribution = flowMaster.Type == Sconit.CodeMaster.OrderType.Distribution;
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
                ViewBag.status = orderMaster.Status;
                ViewBag.PartyFrom = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? orderMaster.PartyFrom : orderMaster.PartyTo;
                ViewBag.PartyTo = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? orderMaster.PartyTo : orderMaster.PartyFrom;

                ViewBag.newOrEdit = "Edit";
                ViewBag.isManualCreateDetail = ViewBag.status == com.Sconit.CodeMaster.OrderStatus.Create && orderMaster.IsManualCreateDetail;
                ViewBag.IsListPrice = orderMaster.IsListPrice;
                ViewBag.IsDistribution = orderMaster.Type == Sconit.CodeMaster.OrderType.Distribution;
            }

            if (ViewBag.Status == com.Sconit.CodeMaster.OrderStatus.Create && ViewBag.IsManualCreateDetail)
            {
                #region comboBox
                IList<Uom> uoms = genericMgr.FindAll<Uom>();
                ViewData.Add("uoms", uoms);
                #endregion
            }
            ViewBag.isEditable = ViewBag.status == com.Sconit.CodeMaster.OrderStatus.Create && this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Distribution_Edit");

            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_View")]
        public ActionResult _SelectBatchEditing(string orderNo, string flow, com.Sconit.CodeMaster.OrderSubType orderSubType)
        {
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();

            if (!string.IsNullOrEmpty(flow) || !string.IsNullOrEmpty(orderNo))
            {
                if (!string.IsNullOrEmpty(orderNo))
                {
                    orderDetailList = genericMgr.FindAll<OrderDetail>(selectOrderDetailStatement, orderNo);
                }
                else
                {
                    FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(flow);
                    if (flowMaster.IsListDet)
                    {
                        orderDetailList = orderMgr.TransformFlowMster2OrderDetailList(flowMaster, orderSubType);
                    }
                }
            }
            return View(new GridModel(orderDetailList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Edit")]
        public JsonResult _SaveBatchEditing([Bind(Prefix =
            "inserted")]IEnumerable<OrderDetail> insertedOrderDetails,
            [Bind(Prefix = "updated")]IEnumerable<OrderDetail> updatedOrderDetails,
            [Bind(Prefix = "deleted")]IEnumerable<OrderDetail> deletedOrderDetails,
            string flow, string orderNo, com.Sconit.CodeMaster.OrderSubType orderSubType)
        {
            try
            {
                IList<OrderDetail> newOrderDetailList = new List<OrderDetail>();
                IList<OrderDetail> updateOrderDetailList = new List<OrderDetail>();
                IList<FlowDetail> flowDetailList = flowMgr.GetFlowDetailList(flow, false, true);
                if (insertedOrderDetails != null)
                {
                    foreach (var orderDetail in insertedOrderDetails)
                    {
                        OrderDetail newOrderDetail = RefreshOrderDetail(flowDetailList, orderDetail, orderSubType);
                        newOrderDetail.OrderNo = orderNo;
                        newOrderDetailList.Add(newOrderDetail);
                    }
                }
                if (updatedOrderDetails != null)
                {
                    //现在控件控制不住，改了Item也默认是之前的,加好的只能改数量,库位
                    foreach (var orderDetail in updatedOrderDetails)
                    {
                        decimal qty = orderDetail.OrderedQty;
                        OrderDetail updateOrderDetail = genericMgr.FindById<OrderDetail>(orderDetail.Id);
                        updateOrderDetail.OrderedQty = qty;
                        if (!string.IsNullOrEmpty(orderDetail.LocationFrom) && updateOrderDetail.LocationFrom != orderDetail.LocationFrom)
                        {
                            updateOrderDetail.LocationFrom = orderDetail.LocationFrom;
                            updateOrderDetail.LocationFromName = genericMgr.FindById<Location>(orderDetail.LocationFrom).Name;
                        }
                        if (!string.IsNullOrEmpty(orderDetail.LocationTo) && updateOrderDetail.LocationTo != orderDetail.LocationTo)
                        {
                            updateOrderDetail.LocationTo = orderDetail.LocationTo;
                            updateOrderDetail.LocationToName = genericMgr.FindById<Location>(orderDetail.LocationTo).Name;
                        }
                        if (!string.IsNullOrWhiteSpace(orderDetail.ManufactureParty))
                        {
                            string sql = "select m from FlowMaster as m where m.Type=" + (int)com.Sconit.CodeMaster.OrderType.Procurement + " and m.PartyFrom='" + orderDetail.ManufactureParty + "' and  m.Code in (select distinct d.Flow from FlowDetail as d where d.Item='" + orderDetail.Item + "' )";

                            IList<FlowMaster> flowMasterCount = genericMgr.FindAll<FlowMaster>(sql);
                            if (flowMasterCount.Count > 0)
                            {
                                updateOrderDetail.ManufactureParty = orderDetail.ManufactureParty;
                            }
                            else
                            {
                                updateOrderDetail.ManufactureParty = null;
                            }
                        }
                        else
                        {
                            updateOrderDetail.ManufactureParty = null;
                        }
                        updateOrderDetail.Direction = orderDetail.Direction;
                        updateOrderDetail.Remark = orderDetail.Remark;

                        updateOrderDetailList.Add(updateOrderDetail);
                    }
                }

                orderMgr.BatchUpdateOrderDetails(orderNo, newOrderDetailList, updateOrderDetailList, (IList<OrderDetail>)deletedOrderDetails);

                object obj = new { SuccessMessage = string.Format(Resources.INV.StockTake.StockTakeDetail_Saved, orderNo) };
                return Json(obj);
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }

        public ActionResult _WebOrderDetail(string flow, string itemCode, com.Sconit.CodeMaster.OrderSubType orderSubType)
        {
            if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(itemCode))
            {
                FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(flow);
                WebOrderDetail webOrderDetail = new WebOrderDetail();
                IList<FlowDetail> flowDetailList = flowMgr.GetFlowDetailList(flowMaster, false, true);
                FlowDetail flowDetail = flowDetailList.Where(d => d.Item == itemCode).FirstOrDefault<FlowDetail>();

                if (flowDetail != null)
                {
                    webOrderDetail.Item = flowDetail.Item;
                    webOrderDetail.ItemDescription = genericMgr.FindById<Item>(flowDetail.Item).Description;
                    webOrderDetail.UnitCount = flowDetail.UnitCount;
                    webOrderDetail.Uom = flowDetail.Uom;
                    webOrderDetail.Sequence = flowDetail.Sequence;
                    webOrderDetail.ReferenceItemCode = flowDetail.ReferenceItemCode;
                    webOrderDetail.MinUnitCount = flowDetail.MinUnitCount;
                    webOrderDetail.UnitCountDescription = flowDetail.UnitCountDescription;
                    webOrderDetail.Container = flowDetail.Container;
                    webOrderDetail.ContainerDescription = flowDetail.ContainerDescription;
                    //默认库位
                    webOrderDetail.LocationFrom = orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? flowDetail.DefaultLocationFrom : flowDetail.DefaultLocationTo;
                    webOrderDetail.LocationTo = orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? flowDetail.DefaultLocationTo : flowDetail.DefaultLocationFrom;
                }
                else
                {
                    Item item = genericMgr.FindById<Item>(itemCode);
                    if (item != null)
                    {
                        webOrderDetail.Item = item.Code;
                        webOrderDetail.ItemDescription = item.Description;
                        webOrderDetail.UnitCount = item.UnitCount;
                        webOrderDetail.Uom = item.Uom;
                        webOrderDetail.LocationFrom = flowMaster.LocationFrom;
                        webOrderDetail.LocationTo = flowMaster.LocationTo;
                    }
                }
                return this.Json(webOrderDetail);
            }
            return null;
        }

        public String _WindowTime(string flow, string windowTime)
        {
            try
            {
                DateTime startDate = DateTime.Parse(windowTime);
                FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(flow);
                if (flowMaster != null)
                {
                    FlowStrategy flowStrategy = genericMgr.FindById<FlowStrategy>(flow);
                    if (flowStrategy != null)
                    {
                        double leadTime = DateTimeHelper.TimeTranfer(flowStrategy.LeadTime, flowStrategy.TimeUnit, Sconit.CodeMaster.TimeUnit.Hour);
                        startDate = startDate.AddHours(-leadTime);
                    }
                }
                return startDate.ToString("yyyy-MM-dd HH:mm");
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region ship
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Ship")]
        public ActionResult ShipIndex()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Ship")]
        public ActionResult Ship(GridCommand command, OrderMasterSearchModel searchModel)
        {
            this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Ship")]
        public ActionResult _AjaxShipOrderList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareShipSearchStatement_1(command, searchModel);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Ship")]
        public ActionResult _ShipOrderDetailList(string checkedOrders)
        {
            string[] checkedOrderArray = checkedOrders.Split(',');
            DetachedCriteria criteria = DetachedCriteria.For<OrderDetail>();
            criteria.Add(Expression.In("OrderNo", checkedOrderArray));
            criteria.Add(Expression.LtProperty("ShippedQty", "OrderedQty"));
            IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>(criteria);
            return PartialView(orderDetailList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Ship")]
        public ActionResult ShipEdit(string checkedOrders)
        {
            ViewBag.CheckedOrders = checkedOrders;
            string[] checkedOrderArray = checkedOrders.Split(',');
            OrderMaster order = genericMgr.FindById<OrderMaster>(checkedOrderArray[0]);
            return View(order);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Ship")]
        public JsonResult ShipOrder(string idStr, string qtyStr, string checkedOrders)
        {
            try
            {
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] idArray = idStr.Split(',');
                    string[] qtyArray = qtyStr.Split(',');

                    for (int i = 0; i < qtyArray.Count(); i++)
                    {
                        if (Convert.ToDecimal(qtyArray[i]) > 0)
                        {
                            OrderDetail od = genericMgr.FindById<OrderDetail>(Convert.ToInt32(idArray[i]));

                            OrderDetailInput input = new OrderDetailInput();
                            input.ShipQty = Convert.ToDecimal(qtyArray[i]);
                            od.AddOrderDetailInput(input);
                            orderDetailList.Add(od);
                        }
                    }
                }
                if (orderDetailList.Count() == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ShippingDetailCanNotBeEmpty);
                }

                IpMaster ipMaster = orderMgr.ShipOrder(orderDetailList, false, DateTime.Now);
                // SaveSuccessMessage("订单" + checkedOrders + "发货成功。");
                object obj = new { SuccessMessage = string.Format(Resources.ORD.OrderMaster.OrderMaster_Shipped, checkedOrders), IpNo = ipMaster.IpNo };
                return Json(obj);
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }

        #endregion

        #region batch

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_BatchProcess")]
        public ActionResult BatchProcessIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_BatchProcess")]
        public ActionResult BatchProcessList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            TempData["OrderMasterSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Submit")]
        public ActionResult BatchSubmit(string orderStr)
        {
            if (string.IsNullOrEmpty(orderStr))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseChooseOneDetail);
            }
            else
            {
                string[] orderArray = orderStr.Split(',');
                try
                {
                    foreach (string orderNo in orderArray)
                    {
                        orderMgr.ReleaseOrder(orderNo);

                    }
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_BatchSubmited);
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Start")]
        public ActionResult BatchStart(string orderStr)
        {
            if (string.IsNullOrEmpty(orderStr))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseChooseOneDetail);
            }
            else
            {
                string[] orderArray = orderStr.Split(',');
                try
                {
                    foreach (string orderNo in orderArray)
                    {
                        orderMgr.StartOrder(orderNo);

                    }
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_BatchStarted);
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Delete")]
        public ActionResult BatchDelete(string orderStr)
        {
            if (string.IsNullOrEmpty(orderStr))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseChooseOneDetail);
            }
            else
            {
                string[] orderArray = orderStr.Split(',');
                try
                {
                    foreach (string orderNo in orderArray)
                    {
                        orderMgr.DeleteOrder(orderNo);

                    }
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_BatchDeleted);
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Cancel")]
        public ActionResult BatchCancel(string orderStr)
        {
            if (string.IsNullOrEmpty(orderStr))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseChooseOneDetail);
            }
            else
            {
                string[] orderArray = orderStr.Split(',');
                try
                {
                    foreach (string orderNo in orderArray)
                    {
                        orderMgr.CancelOrder(orderNo);

                    }
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_BatchCanceled);
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Close")]
        public ActionResult BatchClose(string orderStr)
        {
            if (string.IsNullOrEmpty(orderStr))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseChooseOneDetail);
            }
            else
            {
                string[] orderArray = orderStr.Split(',');
                try
                {
                    foreach (string orderNo in orderArray)
                    {
                        orderMgr.ManualCloseOrder(orderNo);

                    }
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_BatchClosed);
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_BatchProcess")]
        public ActionResult BatchExport(string orderStr)
        {
            if (string.IsNullOrEmpty(orderStr))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseChooseOneDetail);
            }
            else
            {
                string[] orderArray = orderStr.Split(',');
                try
                {
                    foreach (string orderNo in orderArray)
                    {
                        // orderMgr.ManualCloseOrder(orderNo);
                    }
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_BatchProcess")]
        public ActionResult BatchPrint(string orderStr)
        {
            if (string.IsNullOrEmpty(orderStr))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseChooseOneDetail);
            }
            else
            {
                string[] orderArray = orderStr.Split(',');
                try
                {
                    foreach (string orderNo in orderArray)
                    {
                        //  orderMgr.ManualCloseOrder(orderNo);

                    }
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_BatchProcess")]
        public ActionResult _AjaxBatchProcessList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
        }
        #endregion

        #region 打印导出
        public void SaveToClient(string orderNo)
        {
            OrderMaster orderMaster = queryMgr.FindById<OrderMaster>(orderNo);
            IList<OrderDetail> orderDetails = queryMgr.FindAll<OrderDetail>("select od from OrderDetail as od where od.OrderNo=?", orderNo);
            foreach (var orderDetail in orderDetails)
            {
                if (!string.IsNullOrEmpty(orderDetail.Direction))
                {
                    orderDetail.Direction = this.genericMgr.FindById<HuTo>(orderDetail.Direction).CodeDescription;
                }
            }
            orderMaster.OrderDetails = orderDetails;
            PrintOrderMaster printOrderMstr = Mapper.Map<OrderMaster, PrintOrderMaster>(orderMaster);
            IList<object> data = new List<object>();
            data.Add(printOrderMstr);
            data.Add(printOrderMstr.OrderDetails);
            reportGen.WriteToClient(orderMaster.OrderTemplate, data, orderMaster.OrderNo + ".xls");
        }

        public string Print(string orderNo)
        {
            OrderMaster orderMaster = queryMgr.FindById<OrderMaster>(orderNo);
            IList<OrderDetail> orderDetails = queryMgr.FindAll<OrderDetail>("select od from OrderDetail as od where od.OrderNo=?", orderNo);
            foreach (var orderDetail in orderDetails)
            {
                if (!string.IsNullOrEmpty(orderDetail.Direction))
                {
                    orderDetail.Direction = this.genericMgr.FindById<HuTo>(orderDetail.Direction).CodeDescription;
                }
            }
            IList<OrderBomDetail> orderBomDetails = new List<OrderBomDetail>();
            orderMaster.OrderDetails = orderDetails;
            PrintOrderMaster printOrderMstr = Mapper.Map<OrderMaster, PrintOrderMaster>(orderMaster);
            IList<object> data = new List<object>();
            data.Add(printOrderMstr);
            data.Add(printOrderMstr.OrderDetails);
            if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Production)
            {
                string selectOrderBomDetailStatement = "select d from OrderBomDetail as d where d.OrderNo = ?";
                orderBomDetails = genericMgr.FindAll<OrderBomDetail>(selectOrderBomDetailStatement, new object[] { orderNo });
                data.Add(Mapper.Map<IList<OrderBomDetail>, IList<PrintOrderBomDetail>>(orderBomDetails));
            }
            return reportGen.WriteToFile(orderMaster.OrderTemplate, data);
        }

        #endregion

        #region return order
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_ReturnIndex")]
        public ActionResult ReturnIndex()
        {
            return View();
        }

        #region return edit

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_ReturnIndex")]
        public ActionResult ReturnList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_ReturnIndex")]
        public ActionResult _ReturnAjaxList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string replaceFrom = "_ReturnAjaxList";
            string replaceTo = "ReturnList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }
            //string whereStatement = " where o.Type in (" + (int)com.Sconit.CodeMaster.OrderType.Distribution + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + "," + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + ")"
            //                      + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Return;
            //SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel, whereStatement);
            //return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
            string whereStatement = string.Empty;
            searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Return;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, true);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
        }
        #endregion

        #endregion

        #region private method

        private OrderDetail RefreshOrderDetail(IList<FlowDetail> flowDetailList, OrderDetail orderDetail, com.Sconit.CodeMaster.OrderSubType orderSubType)
        {
            OrderDetail newOrderDetail = new OrderDetail();
            //IList<FlowDetail> flowDetailList = flowMgr.GetFlowDetailList(flow, false, true);
            FlowDetail flowDetail = flowDetailList.FirstOrDefault(q => q.Item == orderDetail.Item && q.Uom == orderDetail.Uom && q.UnitCount == orderDetail.UnitCount);
            var item = itemMgr.GetCacheItem(orderDetail.Item);
            if (flowDetail != null)
            {
                Mapper.Map<FlowDetail, OrderDetail>(flowDetail, newOrderDetail);
                if (orderSubType == Sconit.CodeMaster.OrderSubType.Return)
                {
                    newOrderDetail.LocationFrom = flowDetail.LocationTo;
                    newOrderDetail.LocationTo = flowDetail.LocationFrom;
                    newOrderDetail.IsInspect = flowDetail.IsRejectInspect && flowDetail.CurrentFlowMaster.IsRejectInspect;
                }
                else
                {
                    newOrderDetail.IsInspect = flowDetail.IsInspect && flowDetail.CurrentFlowMaster.IsInspect;
                }
                newOrderDetail.Sequence = orderDetail.Sequence == 0 ? newOrderDetail.Sequence : orderDetail.Sequence;
                newOrderDetail.UnitCount = orderDetail.UnitCount == 0 ? newOrderDetail.UnitCount : orderDetail.UnitCount;
                newOrderDetail.Uom = String.IsNullOrWhiteSpace(orderDetail.Uom) ? newOrderDetail.Uom : orderDetail.Uom;
                newOrderDetail.ItemDescription = genericMgr.FindById<Item>(orderDetail.Item).Description;
                newOrderDetail.MinUnitCount = orderDetail.MinUnitCount == 0 ? newOrderDetail.MinUnitCount : orderDetail.MinUnitCount;
            }
            else
            {
                //Item item = genericMgr.FindById<Item>(orderDetail.Item);
                if (item != null)
                {
                    newOrderDetail.Item = item.Code;
                    newOrderDetail.UnitCount = orderDetail.UnitCount == 0 ? item.UnitCount : orderDetail.UnitCount;
                    newOrderDetail.Uom = String.IsNullOrWhiteSpace(orderDetail.Uom) ? item.Uom : orderDetail.Uom;
                    newOrderDetail.ItemDescription = item.Description;
                    newOrderDetail.Sequence = orderDetail.Sequence;
                    newOrderDetail.MinUnitCount = orderDetail.MinUnitCount == 0 ? item.UnitCount : orderDetail.MinUnitCount;
                }
            }

            if (string.IsNullOrWhiteSpace(newOrderDetail.ReferenceItemCode))
            {
                newOrderDetail.ReferenceItemCode = item.ReferenceCode;
            }
            newOrderDetail.RequiredQty = orderDetail.RequiredQty;
            newOrderDetail.OrderedQty = orderDetail.OrderedQty;
            newOrderDetail.UnitPrice = orderDetail.UnitPrice;
            newOrderDetail.Direction = orderDetail.Direction;
            if (!string.IsNullOrEmpty(orderDetail.LocationFrom))
            {
                newOrderDetail.LocationFrom = orderDetail.LocationFrom;
            }
            if (!string.IsNullOrEmpty(orderDetail.LocationTo))
            {
                newOrderDetail.LocationTo = orderDetail.LocationTo;
            }
            if (!string.IsNullOrEmpty(orderDetail.Remark))
            {
                newOrderDetail.Remark = orderDetail.Remark;
            }
            if (!string.IsNullOrEmpty(newOrderDetail.LocationFrom))
            {
                newOrderDetail.LocationFromName = genericMgr.FindById<Location>(newOrderDetail.LocationFrom).Name;
            }
            if (!string.IsNullOrEmpty(newOrderDetail.LocationTo))
            {
                newOrderDetail.LocationToName = genericMgr.FindById<Location>(newOrderDetail.LocationTo).Name;
            }
            if (!string.IsNullOrWhiteSpace(orderDetail.ManufactureParty))
            {
                //string sql = "select count(*) as counter from FlowMaster as m where m.Type="
                //    + (int)com.Sconit.CodeMaster.OrderType.Procurement
                //    + " and m.PartyFrom='"
                //    + orderDetail.ManufactureParty
                //    + "' and  m.Code in (select distinct d.Flow from FlowDetail as d where d.Item='"
                //    + orderDetail.Item
                //    + "' )";

                //IList flowMasterCount = genericMgr.FindAll(sql);
                //if (flowMasterCount != null && flowMasterCount.Count > 0 && flowMasterCount[0] != null && (long)flowMasterCount[0] > 0)
                //{
                //    newOrderDetail.ManufactureParty = orderDetail.ManufactureParty;
                //}
                //else
                //{
                //    newOrderDetail.ManufactureParty = null;
                //}
            }
            else
            {
                newOrderDetail.ManufactureParty = null;
            }
            newOrderDetail.Direction = orderDetail.Direction;
            return newOrderDetail;
        }
        #endregion


        #region  明细查询
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderDetail_Distribution")]
        public ActionResult DetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            TempData["OrderMasterSearchModel"] = searchModel;
            if (this.CheckSearchModelIsNull(searchModel))
            {
                ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
                return View();
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
                return View(new List<OrderDetail>());
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderDetail_Distribution")]
        public ActionResult _AjaxOrderDetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }

            if (searchModel.QueryOrderType == 1)
            {
                searchModel.ExternalOrderNo = searchModel.OrderNo;
                searchModel.OrderNo = null;
            }
            if (searchModel.QueryOrderType == 2)
            {
                searchModel.ReferenceOrderNo = searchModel.OrderNo;
                searchModel.OrderNo = null;
            }
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel);
            return PartialView(GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command));
        }
        #region  Export detail search
        [GridAction(EnableCustomBinding = true)]
        public void Export(OrderMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (searchModel.QueryOrderType == 1)
            {
                searchModel.ExternalOrderNo = searchModel.OrderNo;
                searchModel.OrderNo = null;
            }
            if (searchModel.QueryOrderType == 2)
            {
                searchModel.ReferenceOrderNo = searchModel.OrderNo;
                searchModel.OrderNo = null;
            }
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel);
            var list = GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command);

            ExportToXLS<OrderDetail>("DistributionOrderDetail.xls", list.Data.ToList());
        }
        #endregion

        private ProcedureSearchStatementModel PrepareSearchDetailStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Format(" and exists (select 1 from OrderMaster  as o where  o.SubType ={0} and o.OrderNo=d.OrderNo ) ",
                (int)com.Sconit.CodeMaster.OrderSubType.Normal);
            if (!string.IsNullOrWhiteSpace(searchModel.MultiStatus))
            {
                string statusSql = " and o.Status in( ";
                string[] statusArr = searchModel.MultiStatus.Split(',');
                for (int st = 0; st < statusArr.Length; st++)
                {
                    statusSql += "'" + statusArr[st] + "',";
                }
                statusSql = statusSql.Substring(0, statusSql.Length - 1) + ")";
                whereStatement = string.Format(" and exists (select 1 from OrderMaster  as o where  o.SubType ={0} and o.OrderNo=d.OrderNo {1} ) ",
                   (int)com.Sconit.CodeMaster.OrderSubType.Normal, statusSql);
            }

            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            if (searchModel.Type.HasValue && searchModel.Type > 0)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.Type, Type = NHibernate.NHibernateUtil.String });
            }
            else
            {
                paraList.Add(new ProcedureParameter
                {
                    Parameter = (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                       + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                       + (int)com.Sconit.CodeMaster.OrderType.Distribution + ","
                       + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                    Type = NHibernate.NHibernateUtil.String
                });
            }
            paraList.Add(new ProcedureParameter { Parameter = searchModel.SubType, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Priority, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalOrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ReferenceOrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.TraceCode, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.CreateUserName, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateFrom, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateTo, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateType, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Sequence, Type = NHibernate.NHibernateUtil.Int64 });
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Item, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ManufactureParty, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.WmSSeq, Type = NHibernate.NHibernateUtil.String });

            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });

            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });


            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "OrderTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
                }
                else if (command.SortDescriptors[0].Member == "OrderPriorityDescription")
                {
                    command.SortDescriptors[0].Member = "Priority";
                }
                else if (command.SortDescriptors[0].Member == "OrderStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : null, Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? (command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc") : "asc", Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.PageSize, Type = NHibernate.NHibernateUtil.Int32 });
            pageParaList.Add(new ProcedureParameter { Parameter = command.Page, Type = NHibernate.NHibernateUtil.Int32 });

            var procedureSearchStatementModel = new ProcedureSearchStatementModel();
            procedureSearchStatementModel.Parameters = paraList;
            procedureSearchStatementModel.PageParameters = pageParaList;
            procedureSearchStatementModel.CountProcedure = "USP_Search_OrderDetCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_OrderDet";

            return procedureSearchStatementModel;
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderDetail_Distribution_Return")]
        public ActionResult ReturnDetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (this.CheckSearchModelIsNull(searchModel))
            {
                SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
                ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
                return View();
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
                return View(new List<OrderDetail>());
            }

        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderDetail_Distribution_Return")]
        public ActionResult _AjaxReturnOrderDetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareReturnOrderDetailSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command));
        }
        #region  Export return detail search
        [SconitAuthorize(Permissions = "Url_OrderDetail_Distribution_Return")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportReturn(OrderMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareReturnOrderDetailSearchStatement(command, searchModel);
            var list = GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command);
            ExportToXLS<OrderDetail>("DistributionReturnOrderDetail.xls", list.Data.ToList());
        }
        #endregion
        private ProcedureSearchStatementModel PrepareReturnOrderDetailSearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Format(" and exists (select 1 from OrderMaster  as o where  o.SubType ={0} and o.OrderNo=d.OrderNo ) ",
                (int)com.Sconit.CodeMaster.OrderSubType.Return);

            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                   + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                   + (int)com.Sconit.CodeMaster.OrderType.Distribution + ","
                   + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                Type = NHibernate.NHibernateUtil.String
            });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.SubType, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Priority, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalOrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ReferenceOrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.TraceCode, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.CreateUserName, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateFrom, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateTo, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateType, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Sequence, Type = NHibernate.NHibernateUtil.Int64 });
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Item, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ManufactureParty, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.WmSSeq, Type = NHibernate.NHibernateUtil.String });

            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });

            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });


            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "OrderTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
                }
                else if (command.SortDescriptors[0].Member == "OrderPriorityDescription")
                {
                    command.SortDescriptors[0].Member = "Priority";
                }
                else if (command.SortDescriptors[0].Member == "OrderStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : null, Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? (command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc") : "asc", Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.PageSize, Type = NHibernate.NHibernateUtil.Int32 });
            pageParaList.Add(new ProcedureParameter { Parameter = command.Page, Type = NHibernate.NHibernateUtil.Int32 });

            var procedureSearchStatementModel = new ProcedureSearchStatementModel();
            procedureSearchStatementModel.Parameters = paraList;
            procedureSearchStatementModel.PageParameters = pageParaList;
            procedureSearchStatementModel.CountProcedure = "USP_Search_OrderDetCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_OrderDet";

            return procedureSearchStatementModel;
        }



        #endregion

        #region 交货单过账
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Receipt")]
        public ActionResult ReceiptIndex()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Receipt")]
        public ActionResult ReceiptList(GridCommand command, OrderMasterSearchModel searchModel)
        {

            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }

            List<string> exterNoList = this.getExterOrderNo(((OrderMasterSearchModel)searchCacheModel.SearchObject).ExternalOrderNo, true);
            if (exterNoList.Count == 0)
            {
                return View(new List<OrderDetail>());
            }
            IList<OrderDetail> list = genericMgr.FindAll<OrderDetail>(PrepareReceiptSearchStatement(command, exterNoList));
            if (list.Count > 0)
            {
                IList<string> OrderNos = list.Select(i => i.OrderNo).Distinct().ToList();
                string sql = "select OrderNo,ExtOrderNo from view_ordermstr where OrderNo in (";
                foreach (string orderNo in OrderNos)
                {
                    sql += "'" + orderNo + "',";
                }
                sql = sql.Substring(0, sql.Length - 1) + ")";

                IList<object[]> objectList = this.queryMgr.FindAllWithNativeSql<object[]>(sql);
                foreach (var item in objectList)
                {
                    foreach (OrderDetail orderDetail in list)
                    {
                        if ((string)item[0] == orderDetail.OrderNo)
                        {
                            orderDetail.ExternalOrderNo = (string)item[1];
                        }
                    }
                }
            }
            return View(list);
        }

        private List<string> getExterOrderNo(string exterNo, bool bb)
        {
            if (!string.IsNullOrEmpty(exterNo))
            {

                string externo = exterNo.Replace("\r\n", ",");
                string[] externalOrderNo = externo.Split(',');
                List<string> newExterOrderno = new List<string>();
                foreach (string item in externalOrderNo)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    newExterOrderno.Add(item);
                }
                if (newExterOrderno.Count >= 100)
                {
                    if (bb)
                    {
                        SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_InputDeliveryOrderNumberToMuchThatExceedTheMaximum, newExterOrderno.Count));
                    }
                }
                else
                {
                    TempData["_AjaxMessage"] = "";
                    return newExterOrderno;
                }

            }
            else
            {
                if (bb)
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_PleaseInputDeliveryOrderNumber);
                }
            }
            return new List<string>();
        }


        private string PrepareReceiptSearchStatement(GridCommand command, List<string> exterNoList)
        {
            StringBuilder sb = new StringBuilder();
            string whereStatement = "select d from OrderDetail as d where  exists (select 1 from OrderMaster as o where o.Type ='"
                + (int)com.Sconit.CodeMaster.OrderType.Distribution
                + "'and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.Create + ","
                + (int)com.Sconit.CodeMaster.OrderStatus.Submit + "," + (int)com.Sconit.CodeMaster.OrderStatus.InProcess
                + ") and o.ExternalOrderNo in (";

            sb.Append(whereStatement);
            foreach (string item in exterNoList)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                else
                {
                    sb.Append("'" + item + "',");
                }
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(") and d.OrderNo = o.OrderNo)");
            return sb.ToString();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_Ship")]
        public JsonResult ReceiveOrder(string idStr, string qtyStr, string orderNoStr)
        {
            StringBuilder ExterNoSb = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(idStr))
                {
                    string[] idArray = idStr.Split(',');
                    string[] qtyArray = qtyStr.Split(',');
                    string[] orderNoArray = orderNoStr.Split(',');
                    #region 将交货单明细分组
                    IList<OrderDetail> detailList = new List<OrderDetail>();
                    IList<OrderMaster> masterList = new List<OrderMaster>();
                    //所有的交货明细
                    for (int i = 0; i < idArray.Count(); i++)
                    {
                        if (Convert.ToDecimal(qtyArray[i]) > 0)
                        {
                            OrderDetail od = genericMgr.FindById<OrderDetail>(Convert.ToInt32(idArray[i]));
                            //od.CurrentReceiveQty = int.Parse(qtyArray[i]);
                            OrderDetailInput input = new OrderDetailInput();
                            input.ReceiveQty = Convert.ToDecimal(qtyArray[i]);
                            od.AddOrderDetailInput(input);
                            detailList.Add(od);
                        }
                    }
                    //所有的交货单头
                    foreach (string orderNo in orderNoArray)
                    {
                        OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
                        if (!masterList.Contains(orderMaster))
                        {
                            masterList.Add(orderMaster);
                        }
                    }
                    //所有的明细存到头里面 进行分组
                    foreach (OrderMaster orderMaster in masterList)
                    {
                        IList<OrderDetail> ordDetailList = new List<OrderDetail>();
                        foreach (OrderDetail orderDetail in detailList)
                        {
                            if (orderMaster.OrderNo == orderDetail.OrderNo)
                            {
                                ordDetailList.Add(orderDetail);
                            }
                        }
                        orderMaster.OrderDetails = ordDetailList;
                    }
                    #endregion

                    BusinessException businessException = new BusinessException();

                    foreach (OrderMaster orderMaster in masterList)
                    {
                        try
                        {
                            orderMgr.DistributionReceiveOrder(orderMaster);
                            ExterNoSb.Append(orderMaster.ExternalOrderNo + ",");
                        }
                        catch (BusinessException ex)
                        {
                            businessException.AddMessage(ex.GetMessages()[0].GetMessageString() + Resources.EXT.ControllerLan.Con_DeliveryOrderNumber + orderMaster.ExternalOrderNo);
                        }
                    }
                    if (businessException.HasMessage)
                    {
                        throw businessException;
                    }
                    object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_DeliveryOrder + ExterNoSb.Remove(ExterNoSb.Length - 1, 1) + Resources.EXT.ControllerLan.Con_PostingSuccessfully) };
                    return Json(obj);
                }
                else
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_PostDetailCanNotBeEmpty);
                }
            }
            catch (BusinessException ex)
            {

                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                string messagesStr = "";
                IList<Message> messageList = ex.GetMessages();
                foreach (Message message in messageList)
                {
                    messagesStr += message.GetMessageString();
                }


                string succesMessage = string.Empty;
                if (ExterNoSb.ToString() != "")
                {
                    succesMessage = string.Format("<li>" + Resources.EXT.ControllerLan.Con_DeliveryOrder + ExterNoSb.Remove(ExterNoSb.Length - 1, 1) + Resources.EXT.ControllerLan.Con_PostingSuccessfully+"。</li>");

                }
                Response.Write(messagesStr + "*" + succesMessage);
                return Json(null);
            }
        }

        #endregion

        #region 重新查询存储过程
        private ProcedureSearchStatementModel PrepareSearchStatement_1(GridCommand command, OrderMasterSearchModel searchModel, string whereStatement, bool isReturn)
        {
            if (!string.IsNullOrWhiteSpace(searchModel.MultiStatus))
            {
                string statusSql = " and o.Status in( ";
                string[] statusArr = searchModel.MultiStatus.Split(',');
                for (int st = 0; st < statusArr.Length; st++)
                {
                    statusSql += "'" + statusArr[st] + "',";
                }
                whereStatement += statusSql.Substring(0, statusSql.Length - 1) + ")";
            }
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            if (searchModel.Type.HasValue && searchModel.Type.Value > 0)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.Type, Type = NHibernate.NHibernateUtil.String });
            }
            else
            {
                paraList.Add(new ProcedureParameter
                {
                    Parameter = (int)com.Sconit.CodeMaster.OrderType.Distribution + ","
                    + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                    Type = NHibernate.NHibernateUtil.String
                });
            }
            paraList.Add(new ProcedureParameter { Parameter = searchModel.SubType, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Priority, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalOrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ReferenceOrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.TraceCode, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.CreateUserName, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateFrom, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateTo, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateType, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Sequence, Type = NHibernate.NHibernateUtil.Int64 });
            paraList.Add(new ProcedureParameter { Parameter = isReturn, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });


            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "OrderTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
                }
                else if (command.SortDescriptors[0].Member == "OrderPriorityDescription")
                {
                    command.SortDescriptors[0].Member = "Priority";
                }
                else if (command.SortDescriptors[0].Member == "OrderStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : null, Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? (command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc") : "asc", Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.PageSize, Type = NHibernate.NHibernateUtil.Int32 });
            pageParaList.Add(new ProcedureParameter { Parameter = command.Page, Type = NHibernate.NHibernateUtil.Int32 });

            var procedureSearchStatementModel = new ProcedureSearchStatementModel();
            procedureSearchStatementModel.Parameters = paraList;
            procedureSearchStatementModel.PageParameters = pageParaList;
            procedureSearchStatementModel.CountProcedure = "USP_Search_OrderMstrCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_OrderMstr";

            return procedureSearchStatementModel;
        }

        private ProcedureSearchStatementModel PrepareShipSearchStatement_1(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = " and o.IsShipScanHu = 0 and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.Submit + "," + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + ")"
                //+ " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal
                                    + " and exists (select 1 from OrderDetail as d where d.ShipQty < d.OrderQty and d.OrderNo = o.OrderNo) ";
            if (!string.IsNullOrEmpty(searchModel.Dock))
            {
                whereStatement += "and o.Dock=" + searchModel.Dock + "";
            }
            if (!string.IsNullOrEmpty(searchModel.ExternalOrderNo))
            {
                whereStatement += "and o.Dock=" + searchModel.ExternalOrderNo + "";
            }
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            if (searchModel.Type.HasValue && searchModel.Type.Value > 0)
            {
                paraList.Add(new ProcedureParameter { Parameter = searchModel.Type, Type = NHibernate.NHibernateUtil.String });
            }
            else
            {
                paraList.Add(new ProcedureParameter
                {
                    Parameter = (int)com.Sconit.CodeMaster.OrderType.Distribution + ","
                    + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer,
                    Type = NHibernate.NHibernateUtil.String
                });
            }
            paraList.Add(new ProcedureParameter { Parameter = searchModel.SubType, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyFrom, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.PartyTo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Status, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Priority, Type = NHibernate.NHibernateUtil.Int16 });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ExternalOrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.ReferenceOrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.TraceCode, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.CreateUserName, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateFrom, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateTo, Type = NHibernate.NHibernateUtil.DateTime });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.DateType, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Sequence, Type = NHibernate.NHibernateUtil.Int64 });
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = false, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = CurrentUser.Id, Type = NHibernate.NHibernateUtil.Int32 });
            paraList.Add(new ProcedureParameter { Parameter = whereStatement, Type = NHibernate.NHibernateUtil.String });


            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "OrderTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
                }
                else if (command.SortDescriptors[0].Member == "OrderPriorityDescription")
                {
                    command.SortDescriptors[0].Member = "Priority";
                }
                else if (command.SortDescriptors[0].Member == "OrderStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : null, Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.SortDescriptors.Count > 0 ? (command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc") : "asc", Type = NHibernate.NHibernateUtil.String });
            pageParaList.Add(new ProcedureParameter { Parameter = command.PageSize, Type = NHibernate.NHibernateUtil.Int32 });
            pageParaList.Add(new ProcedureParameter { Parameter = command.Page, Type = NHibernate.NHibernateUtil.Int32 });

            var procedureSearchStatementModel = new ProcedureSearchStatementModel();
            procedureSearchStatementModel.Parameters = paraList;
            procedureSearchStatementModel.PageParameters = pageParaList;
            procedureSearchStatementModel.CountProcedure = "USP_Search_OrderMstrCount";
            procedureSearchStatementModel.SelectProcedure = "USP_Search_OrderMstr";

            return procedureSearchStatementModel;
        }
        #endregion


        #region NewFromPlan
        //[SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_New")]
        //public ActionResult NewFromPlan(string Flow, string PlanDate)
        //{
        //    if (string.IsNullOrWhiteSpace(Flow) || string.IsNullOrWhiteSpace(PlanDate))
        //    {
        //        return HttpNotFound();
        //    }
        //    else
        //    {
        //        return View("NewFromPlan", Flow, PlanDate);
        //    }
        //}

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_New")]
        public ActionResult NewFromPlan(MrpPlanSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(null, searchModel);
            FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(searchModel.Flow);
            OrderMaster orderMaster = this.orderMgr.TransferFlow2Order(flowMaster, false);
            orderMaster.ExternalOrderNo = searchModel.PlanDate.ToString("yyyy-MM-dd");
            ViewBag.PlanDate = searchModel.PlanDate;
            orderMaster.StartTime = searchModel.PlanDate;
            ViewBag.FlowDescription = flowMaster.Description;
            FlowStrategy flowStrategy = genericMgr.FindById<FlowStrategy>(searchModel.Flow);
            if (flowStrategy != null)
            {
                double leadTime = DateTimeHelper.TimeTranfer(flowStrategy.LeadTime, flowStrategy.TimeUnit, Sconit.CodeMaster.TimeUnit.Hour);
                orderMaster.WindowTime = orderMaster.StartTime.AddHours(leadTime);
            }
            ViewBag.BackUrl = searchModel.BackUrl;
            //ViewBag.isEditable = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create;
            //ViewBag.editorTemplate = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create ? "" : "ReadonlyTextBox";

            return View(orderMaster);
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_New")]
        public ActionResult _OrderDetailListFromPlan(string Flow, string PlanDate)
        {
            FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(Flow);
            ViewBag.isManualCreateDetail = false;
            ViewBag.Flow = Flow;
            ViewBag.FlowDescription = flowMaster.Description;
            ViewBag.PlanDate = PlanDate;
            ViewBag.newOrEdit = "New";
            ViewBag.status = null;
            ViewBag.orderSubType = com.Sconit.CodeMaster.OrderSubType.Normal;
            ViewBag.PartyFrom = flowMaster.PartyFrom;
            ViewBag.PartyTo = flowMaster.PartyTo;
            ViewBag.isManualCreateDetail = flowMaster.IsManualCreateDetail;
            ViewBag.status = com.Sconit.CodeMaster.OrderStatus.Create;
            ViewBag.IsListPrice = flowMaster.IsListPrice;
            ViewBag.IsDistribution = flowMaster.Type == Sconit.CodeMaster.OrderType.Distribution;
            if (ViewBag.Status == com.Sconit.CodeMaster.OrderStatus.Create && ViewBag.IsManualCreateDetail)
            {
                #region comboBox
                IList<Uom> uoms = genericMgr.FindAll<Uom>();
                ViewData.Add("uoms", uoms);
                #endregion
            }
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_New")]
        public ActionResult _SelectDetailFromPlan(string Flow, string PlanDate)
        {
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();
            if (!string.IsNullOrEmpty(Flow) && !string.IsNullOrEmpty(PlanDate))
            {
                DateTime planDate = DateTime.Parse(PlanDate);
                IList<object> param = new List<object>();
                string hql = "  from MrpPlan as m where m.Flow=? and m.PlanDate=? ";
                param.Add(Flow);
                param.Add(planDate);

                IList<MrpPlan> mrpPlanList = genericMgr.FindAll<MrpPlan>(hql, param.ToArray());
                var flowDetails = this.flowMgr.GetFlowDetailList(Flow, true);
                foreach (var plan in mrpPlanList)
                {
                    var flowDetail = flowDetails.Where(p => p.Item == plan.Item &&
                        (p.StartDate.HasValue ? p.StartDate.Value <= planDate : true
                            && p.EndDate.HasValue ? p.EndDate.Value >= planDate : true))
                        .FirstOrDefault();
                    if (flowDetail == null)
                    {
                        continue;
                    }
                    OrderDetail newOrderDetail = new OrderDetail();
                    Item item = genericMgr.FindById<Item>(plan.Item);
                    newOrderDetail.Item = item.Code;
                    newOrderDetail.UnitCount = flowDetail.UnitCount;
                    newOrderDetail.Uom = flowDetail.Uom;
                    newOrderDetail.ItemDescription = item.Description;
                    newOrderDetail.Sequence = flowDetail.Sequence;
                    newOrderDetail.MinUnitCount = flowDetail.MinUnitCount;
                    newOrderDetail.RequiredQty = itemMgr.ConvertItemUomQty(item.Code, item.Uom, (decimal)plan.Qty, newOrderDetail.Uom);
                    newOrderDetail.ShippedQty = itemMgr.ConvertItemUomQty(item.Code, item.Uom, (decimal)plan.OrderQty, newOrderDetail.Uom);
                    newOrderDetail.OrderedQty = newOrderDetail.RequiredQty - newOrderDetail.ShippedQty;
                    newOrderDetail.OrderedQty = newOrderDetail.OrderedQty > 0 ? newOrderDetail.OrderedQty : 0;

                    newOrderDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                    newOrderDetail.LocationFromName = genericMgr.FindById<Location>(newOrderDetail.LocationFrom).Name;
                    //newOrderDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                    //newOrderDetail.LocationToName = genericMgr.FindById<Location>(newOrderDetail.LocationTo).Name;
                    orderDetailList.Add(newOrderDetail);
                }
            }
            return View(new GridModel(orderDetailList.OrderBy(p => p.Sequence)));
        }
        #endregion
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_View")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportMstr(OrderMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }

            if (searchModel.QueryOrderType == 1)
            {
                searchModel.ExternalOrderNo = searchModel.OrderNo;
                searchModel.OrderNo = null;
            }
            if (searchModel.QueryOrderType == 2)
            {
                searchModel.ReferenceOrderNo = searchModel.OrderNo;
                searchModel.OrderNo = null;
            }
            string whereStatement = string.Empty;
            searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            ExportToXLS<OrderMaster>("DistributionOrderMaster.xls", GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
        #region  Export master search about return
        [SconitAuthorize(Permissions = "Url_OrderMstr_Distribution_ReturnIndex")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportReturnMstr(OrderMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }
            string whereStatement = string.Empty;
            searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Return;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, true);
            ExportToXLS<OrderMaster>("DistributionReturnOrderMaster.xls", GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
    }
}
