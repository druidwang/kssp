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
    using System.Reflection;
    using com.Sconit.Entity.INV;
    using System.Web;
    using System.ComponentModel;
    using com.Sconit.Entity.PRD;
    using com.Sconit.Entity.MRP.TRANS;
    using System.Web.Helpers;
    using com.Sconit.Web.Models.SearchModels.MRP;
    using com.Sconit.Entity.MRP.ORD;
    using com.Sconit.Service.MRP;
    using com.Sconit.Entity.MRP.MD;

    public class ProcurementOrderController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        //private static string selectCountStatement = "select count(*) from OrderMaster as o";
        //private static string selectStatement = "select o from OrderMaster as o";
        //private static string selectOrderDetailCountStatement = "select count(*) from OrderDetail as o";
        //private static string selectOrderDetailToStatement = "select o from OrderDetail as o";
        //private static string selectFlowDetailStatement = "select f from FlowDetail as f where f.Flow = ? order by f.Sequence asc,f.Id asc";
        //private static string selectOneFlowDetailStatement = "select d from FlowDetail as d where d.Flow = ? and d.Item = ?";
        private static string selectOrderDetailStatement = "select d from OrderDetail as d where d.OrderNo=? order by d.Sequence asc,d.Id asc";

        //public IGenericMgr genericMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        //public IFlowMgr flowMgr { get; set; }
        //public IReportGen reportGen { get; set; }
        //public IItemMgr itemMgr { get; set; }

        public IHuMgr huMgr { get; set; }
        public IBomMgr bomMgr { get; set; }
        public IMrpMgr mrpMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public ICustomizationMgr customizationMgr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ProcurementOrderController()
        {
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_View")]
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

        [SconitAuthorize(Permissions = "Url_OrderDetail_Procurement_Return")]
        public ActionResult ReturnDetailIndex()
        {
            return View();
        }

        #region View
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_View")]
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
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_View")]
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

            //string whereStatement = " where o.OrderStrategy != " + (int)com.Sconit.CodeMaster.FlowStrategy.SEQ + " and o.Type in (" + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
            //            + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + "," + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ")"
            //            + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            string whereStatement = "and o.OrderStrategy != " + (int)com.Sconit.CodeMaster.FlowStrategy.SEQ;

            searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
            //GridModel<OrderMaster> returnGrid = GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command);
            //if (returnGrid.Data != null && returnGrid.Data.Count() > 0)
            //{
            //    foreach (var data in returnGrid.Data)
            //    {

            //    }
            //}
        }

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
                        + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine,
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Edit")]
        public ActionResult New()
        {
            ViewBag.flow = null;
            ViewBag.orderNo = null;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Edit,Url_OrderMstr_Procurement_ReturnNew,Url_OrderMstr_Procurement_QuickNew")]
        public JsonResult NewOrder(OrderMaster orderMaster, int[] Sequences, string[] Items, decimal[] UnitCounts,
            string[] Uoms, string[] LocationFroms, string[] LocationTos, decimal[] OrderedQtys, decimal[] RequiredQtys, string[] Hutos,
            string[] Remarks)
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
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.Sequence = Sequences[i];
                    orderDetail.Item = Items[i];
                    orderDetail.UnitCount = UnitCounts[i];
                    orderDetail.Uom = Uoms[i];
                    if (LocationFroms != null && LocationFroms.Length > 0)
                    {
                        orderDetail.LocationFrom = LocationFroms[i] == string.Empty ? null : LocationFroms[i];
                    }
                    if (LocationTos != null && LocationTos.Length > 0)
                    {
                        orderDetail.LocationTo = LocationTos[i] == string.Empty ? null : LocationTos[i];
                    }
                    if (RequiredQtys != null && RequiredQtys.Length > 0)
                    {
                        orderDetail.RequiredQty = RequiredQtys[i];
                    }
                    orderDetail.OrderedQty = OrderedQtys[i];
                    if (Hutos != null && Hutos.Length > 0)
                    {
                        orderDetail.Direction = Hutos[i];
                    }
                    if (Remarks != null && Remarks.Length > 0)
                    {
                        orderDetail.Remark = Remarks[i];
                    }
                    orderDetList.Add(orderDetail);
                }

                if (string.IsNullOrEmpty(orderMaster.Flow))
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_Flow);
                }
                if (!orderMaster.IsQuick && orderMaster.SubType == Sconit.CodeMaster.OrderSubType.Normal && orderMaster.WindowTime == DateTime.MinValue)
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_WindowTime);
                }
                if (!orderMaster.IsQuick && orderMaster.SubType == Sconit.CodeMaster.OrderSubType.Normal && orderMaster.StartTime == DateTime.MinValue)
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_StartTime);
                }

                #region orderDetailList
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                IList<FlowDetail> flowDetailList = flowMgr.GetFlowDetailList(orderMaster.Flow, false, true);
                if (orderDetList != null && orderDetList.Count() > 0)
                {
                    foreach (OrderDetail orderDetail in orderDetList)
                    {
                        OrderDetail newOrderDetail = RefreshOrderDetail(flowDetailList, orderDetail, orderMaster.SubType);
                        orderDetailList.Add(newOrderDetail);
                    }
                }

                #endregion

                if (orderDetailList.Count == 0)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_OrderDetailIsEmpty);
                }

                FlowMaster flow = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);

                #region 策略是SEQ的明细只能有一条
                if (flow.FlowStrategy == com.Sconit.CodeMaster.FlowStrategy.SEQ && orderDetailList.Count > 1)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_SEQOrderDetailMoreThanOne);
                }
                #endregion

                if (orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    flow = flowMgr.GetReverseFlow(flow, null);                  
                    //  this.genericMgr.CleanSession();
                }
                DateTime effectiveDate = orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value : DateTime.Now;
                OrderMaster newOrder = orderMgr.TransferFlow2Order(flow, null, effectiveDate, false);
                newOrder.IsQuick = orderMaster.IsQuick;

                if (orderMaster.WindowTime == DateTime.MinValue)
                {
                    orderMaster.WindowTime = DateTime.Now;
                }
                if (orderMaster.StartTime == DateTime.MinValue)
                {
                    orderMaster.StartTime = DateTime.Now;
                }

                newOrder.WindowTime = orderMaster.WindowTime;
                newOrder.StartTime = orderMaster.StartTime;
                if (newOrder.WindowTime == newOrder.StartTime)
                {
                    var flowStrategy = this.genericMgr.FindById<FlowStrategy>(orderMaster.Flow);
                    newOrder.StartTime = newOrder.WindowTime.AddHours(
                        -Utility.DateTimeHelper.TimeTranfer(flowStrategy.LeadTime, flowStrategy.TimeUnit, Sconit.CodeMaster.TimeUnit.Hour));
                }

                newOrder.ReferenceOrderNo = orderMaster.ReferenceOrderNo;
                newOrder.ExternalOrderNo = orderMaster.ExternalOrderNo;
                newOrder.IsIndepentDemand = orderMaster.IsIndepentDemand;
                newOrder.QualityType = orderMaster.QualityType;
                newOrder.WMSNo = orderMaster.WMSNo;

                newOrder.Priority = orderMaster.Priority;

                if (orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    newOrder.SubType = orderMaster.SubType;
                    newOrder.IsAutoRelease = false;
                    if (newOrder.IsQuick)
                    {
                        newOrder.IsShipScanHu = false;
                        newOrder.IsReceiveScanHu = false;
                    }
                }

                newOrder.OrderDetails = orderDetailList;

                orderMgr.CreateOrder(newOrder);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Added, newOrder.OrderNo);
                //object obj = new { newOrder.OrderNo };
                return Json(new { OrderNo = newOrder.OrderNo });
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Edit,Url_OrderMstr_Procurement_ReturnNew,Url_OrderMstr_Procurement_QuickNew")]
        public JsonResult CreateOrder(OrderMaster orderMaster,
            [Bind(Prefix = "inserted")]IEnumerable<OrderDetail> insertedOrderDetails,
            [Bind(Prefix = "updated")]IEnumerable<OrderDetail> updatedOrderDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(orderMaster.Flow))
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_Flow);
                }
                if (!orderMaster.IsQuick && orderMaster.SubType == Sconit.CodeMaster.OrderSubType.Normal && orderMaster.WindowTime == DateTime.MinValue)
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_WindowTime);
                }
                if (!orderMaster.IsQuick && orderMaster.SubType == Sconit.CodeMaster.OrderSubType.Normal && orderMaster.StartTime == DateTime.MinValue)
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_StartTime);
                }

                #region orderDetailList
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                IList<FlowDetail> flowDetailList = flowMgr.GetFlowDetailList(orderMaster.Flow, false, true);
                if (insertedOrderDetails != null && insertedOrderDetails.Count() > 0)
                {
                    foreach (OrderDetail orderDetail in insertedOrderDetails)
                    {
                        OrderDetail newOrderDetail = RefreshOrderDetail(flowDetailList, orderDetail, orderMaster.SubType);
                        orderDetailList.Add(newOrderDetail);
                    }
                }
                if (updatedOrderDetails != null && updatedOrderDetails.Count() > 0)
                {
                    foreach (OrderDetail orderDetail in updatedOrderDetails)
                    {
                        if (!string.IsNullOrEmpty(orderDetail.LocationFrom))
                        {
                            orderDetail.LocationFromName = genericMgr.FindById<Location>(orderDetail.LocationFrom).Name;
                        }
                        if (!string.IsNullOrEmpty(orderDetail.LocationTo))
                        {
                            orderDetail.LocationToName = genericMgr.FindById<Location>(orderDetail.LocationTo).Name;
                        }

                        if (!string.IsNullOrWhiteSpace(orderDetail.ManufactureParty))
                        {
                            string sql = "select m from FlowMaster as m where m.Type=" + (int)com.Sconit.CodeMaster.OrderType.Procurement + " and m.PartyFrom='" + orderDetail.ManufactureParty + "' and  m.Code in (select distinct d.Flow from FlowDetail as d where d.Item='" + orderDetail.Item + "' )";

                            IList<FlowMaster> flowMasterCount = genericMgr.FindAll<FlowMaster>(sql);
                            if (flowMasterCount.Count == 0)
                            {
                                orderDetail.ManufactureParty = null;
                            }
                        }
                        else
                        {
                            orderDetail.ManufactureParty = null;
                        }

                        orderDetailList.Add(orderDetail);
                    }
                }
                #endregion

                if (orderDetailList.Count == 0)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_OrderDetailIsEmpty);
                }

                FlowMaster flow = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);

                #region 策略是SEQ的明细只能有一条
                if (flow.FlowStrategy == com.Sconit.CodeMaster.FlowStrategy.SEQ && orderDetailList.Count > 1)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_SEQOrderDetailMoreThanOne);
                }
                #endregion
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
                if (orderMaster.WindowTime == DateTime.MinValue)
                {
                    orderMaster.WindowTime = DateTime.Now;
                }
                if (orderMaster.StartTime == DateTime.MinValue)
                {
                    orderMaster.StartTime = DateTime.Now;
                }
                newOrder.ReferenceOrderNo = orderMaster.ReferenceOrderNo;
                newOrder.ExternalOrderNo = orderMaster.ExternalOrderNo;
                newOrder.Priority = orderMaster.Priority;
                if (orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    newOrder.SubType = orderMaster.SubType;
                    newOrder.IsAutoRelease = false;
                }

                newOrder.OrderDetails = orderDetailList;

                orderMgr.CreateOrder(newOrder);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Added, newOrder.OrderNo);
                return Json(new { OrderNo = newOrder.OrderNo });
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_ReturnNew")]
        public ActionResult ReturnNew()
        {

            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_QuickNew")]
        public ActionResult QuickNew()
        {

            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_View")]
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
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_View")]
        public ActionResult _Edit(string orderNo)
        {
            OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);
            ViewBag.isEditable = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create && this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Procurement_Edit");
            ViewBag.editorTemplate = ViewBag.isEditable ? "" : "ReadonlyTextBox";
            return PartialView(orderMaster);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Edit")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Delete")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Submit")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Start")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Close")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Cancel")]
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

        public ActionResult _CurrencyValue(string value)
        {
            PriceListMaster master = null;

            try
            {
                master = genericMgr.FindById<PriceListMaster>(value);
                return new JsonResult { Data = master.Currency };
            }
            catch (Exception)
            {
                return new JsonResult { Data = "" };
            }
        }

        public ActionResult _OrderDetailList(string flow, string orderNo, int? orderSubType)
        {
            ViewBag.isManualCreateDetail = false;
            ViewBag.flow = flow;
            ViewBag.orderNo = orderNo;
            ViewBag.newOrEdit = "New";
            ViewBag.status = null;
            ViewBag.onsumitType = 0;
            ViewBag.orderSubType = (orderSubType != null && orderSubType.Value == (int)com.Sconit.CodeMaster.OrderSubType.Return) ? com.Sconit.CodeMaster.OrderSubType.Return : com.Sconit.CodeMaster.OrderSubType.Normal;
            ViewBag.ds = "";
            ViewBag.isEditable = this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Procurement_Edit");
            ViewBag.IsProcurement = false;

            if (!string.IsNullOrEmpty(flow))
            {
                FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(flow);
                ViewBag.PartyFrom = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? flowMaster.PartyFrom : flowMaster.PartyTo;
                ViewBag.PartyTo = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? flowMaster.PartyTo : flowMaster.PartyFrom;

                ViewBag.isManualCreateDetail = flowMaster.IsManualCreateDetail;
                ViewBag.status = com.Sconit.CodeMaster.OrderStatus.Create;
                ViewBag.IsListPrice = false;
                if (flowMaster.IsListPrice && this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Procurement_ListPrice"))
                {
                    ViewBag.IsListPrice = true;
                }
                ViewBag.IsListDet = flowMaster.IsListDet;
                if (flowMaster.Type == Sconit.CodeMaster.OrderType.Procurement
                   || flowMaster.Type == Sconit.CodeMaster.OrderType.CustomerGoods
                   || flowMaster.Type == Sconit.CodeMaster.OrderType.ScheduleLine
                   || flowMaster.Type == Sconit.CodeMaster.OrderType.SubContract)
                {
                    ViewBag.IsProcurement = true;
                }
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
                ViewBag.status = orderMaster.Status;
                ViewBag.PartyFrom = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? orderMaster.PartyFrom : orderMaster.PartyTo;
                ViewBag.PartyTo = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? orderMaster.PartyTo : orderMaster.PartyFrom;
                ViewBag.newOrEdit = "Edit";
                ViewBag.isManualCreateDetail = ViewBag.status == com.Sconit.CodeMaster.OrderStatus.Create && orderMaster.IsManualCreateDetail;
                ViewBag.IsListPrice = false;
                if (orderMaster.IsListPrice && this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Procurement_ListPrice"))
                {
                    ViewBag.IsListPrice = true;
                }
                if (orderMaster.Type == Sconit.CodeMaster.OrderType.Procurement
                || orderMaster.Type == Sconit.CodeMaster.OrderType.CustomerGoods
                || orderMaster.Type == Sconit.CodeMaster.OrderType.ScheduleLine
                || orderMaster.Type == Sconit.CodeMaster.OrderType.SubContract)
                {
                    ViewBag.IsProcurement = true;
                }
            }

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
            foreach (var orderDetail in orderDetailList)
            {
                orderDetail.IsProvisionalEstimateDesc = orderDetail.IsProvisionalEstimate ? Resources.EXT.ControllerLan.Con_ProvisionalEstimate : Resources.EXT.ControllerLan.Con_NotProvisionalEstimate;
            }
            return PartialView(new GridModel(orderDetailList));
        }

        [GridAction]
        public ActionResult _SelectBatchEditingTo(string ReferenceOrderNo, string flow)
        {
            var flowDetailDic = this.flowMgr.GetFlowDetailList(flow, false, true)
                .GroupBy(p => p.Item, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.First());
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();
            IList<OrderBomDetail> orderBomDetailList = null;

            //查找是否是
            if (!string.IsNullOrEmpty(ReferenceOrderNo))
            {
                var orderMaster = (this.genericMgr.FindAll<OrderMaster>(" from OrderMaster where OrderNo =? ", ReferenceOrderNo) ?? new List<OrderMaster>())
                    .FirstOrDefault();
                //var orderMaster = this.genericMgr.FindById<OrderMaster>(ReferenceOrderNo);
                if (orderMaster != null)
                {
                    if (orderMaster.Type == Sconit.CodeMaster.OrderType.Production
                        || orderMaster.Type == Sconit.CodeMaster.OrderType.SubContract)
                    {
                        orderBomDetailList = genericMgr.FindAll<OrderBomDetail>
                            (string.Format(" from OrderBomDetail o where o.OrderNo='{0}' ", ReferenceOrderNo));

                        foreach (var bomDetail in orderBomDetailList)
                        {
                            var flowDetail = flowDetailDic.ValueOrDefault(bomDetail.Item);
                            if (flowDetail != null && flowDetail.DefaultLocationTo == bomDetail.Location)
                            {
                                var orderDetail = Mapper.Map<FlowDetail, OrderDetail>(flowDetail);
                                orderDetail.RequiredQty = itemMgr.ConvertItemUomQty(orderDetail.Item, bomDetail.Uom, bomDetail.OrderedQty, flowDetail.Uom);
                                orderDetail.ItemDescription = bomDetail.ItemDescription;
                                orderDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                                orderDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                                //orderDetail.OrderedQty = orderMgr.GetRoundOrderQty(firstFlowDetail, orderDetail.RequiredQty);
                                orderDetailList.Add(orderDetail);
                            }
                        }
                    }
                    else
                    {
                        IList<OrderDetail> listOrderDetail = genericMgr.FindAll<OrderDetail>(selectOrderDetailStatement, ReferenceOrderNo);
                        foreach (var orderDetail in listOrderDetail)
                        {
                            var flowDetail = flowDetailDic.ValueOrDefault(orderDetail.Item);
                            if (flowDetail != null)
                            {
                                var newOrderDetail = Mapper.Map<FlowDetail, OrderDetail>(flowDetail);
                                newOrderDetail.RequiredQty = itemMgr.ConvertItemUomQty(orderDetail.Item, orderDetail.Uom, orderDetail.OrderedQty, flowDetail.Uom);
                                newOrderDetail.ItemDescription = orderDetail.ItemDescription;
                                orderDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                                orderDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                                //orderDetail.OrderedQty = orderMgr.GetRoundOrderQty(firstFlowDetail, orderDetail.RequiredQty);
                                orderDetailList.Add(newOrderDetail);
                            }
                        }
                    }
                }
            }

            var orderDetails = orderDetailList.GroupBy(p => p.Item, (k, g) => new
                {
                    RequiredQty = g.Sum(q => q.RequiredQty),
                    OrderDetail = g.First()
                })
                .Select(p =>
                {
                    OrderDetail orderDetail = p.OrderDetail;
                    orderDetail.RequiredQty = p.RequiredQty;
                    orderDetail.OrderedQty = orderMgr.GetRoundOrderQty(orderDetail, orderDetail.RequiredQty);
                    return orderDetail;
                })
                .OrderBy(p => p.Sequence).ToList();

            return PartialView(new GridModel(orderDetails));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Edit")]
        public JsonResult _SaveBatchEditing(
            [Bind(Prefix = "inserted")]IEnumerable<OrderDetail> insertedOrderDetails,
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
                    //现在控件控制不住，改了Item也默认是之前的,加好的只能改数量和库位
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
                        updateOrderDetail.UnitPrice = orderDetail.UnitPrice;
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
                        webOrderDetail.MinUnitCount = item.UnitCount;
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

                FlowStrategy flowStrategy = genericMgr.FindById<FlowStrategy>(flow);
                if (flowStrategy != null)
                {
                    double leadTime = DateTimeHelper.TimeTranfer(flowStrategy.LeadTime, flowStrategy.TimeUnit, Sconit.CodeMaster.TimeUnit.Hour);
                    startDate = startDate.AddHours(-leadTime);
                }
                return startDate.ToString("yyyy-MM-dd HH:mm");
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region receive

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Receive")]
        public ActionResult ReceiveIndex()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Receive")]
        public ActionResult Receive(GridCommand command, OrderMasterSearchModel searchModel)
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

            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Receive")]
        public ActionResult _AjaxReceiveOrderList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }
            else
            {
                ProcedureSearchStatementModel procedureSearchStatementModel = PrepareReceiveSearchStatement_1(command, searchModel);
                return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Receive")]
        public ActionResult _ReceiveOrderDetailList(string checkedOrders)
        {
            string[] checkedOrderArray = checkedOrders.Split(',');
            DetachedCriteria criteria = DetachedCriteria.For<OrderDetail>();
            criteria.Add(Expression.In("OrderNo", checkedOrderArray));
            criteria.Add(Expression.LtProperty("ReceivedQty", "OrderedQty"));
            IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>(criteria);
            return PartialView(orderDetailList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Receive")]
        public ActionResult ReceiveEdit(string checkedOrders)
        {
            ViewBag.CheckedOrders = checkedOrders;
            string[] checkedOrderArray = checkedOrders.Split(',');
            OrderMaster order = genericMgr.FindById<OrderMaster>(checkedOrderArray[0]);
            return View(order);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Receive")]
        public JsonResult ReceiveOrder(string idStr, string qtyStr, string checkedOrders)
        {
            try
            {
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                if (!string.IsNullOrEmpty(idStr))
                {

                    string[] idArray = idStr.Split(',');
                    string[] qtyArray = qtyStr.Split(',');

                    for (int i = 0; i < idArray.Count(); i++)
                    {
                        if (Convert.ToDecimal(qtyArray[i]) > 0)
                        {
                            OrderDetail od = genericMgr.FindById<OrderDetail>(Convert.ToInt32(idArray[i]));
                            OrderDetailInput input = new OrderDetailInput();
                            input.ReceiveQty = Convert.ToDecimal(qtyArray[i]);
                            od.AddOrderDetailInput(input);
                            orderDetailList.Add(od);
                        }
                    }
                }
                if (orderDetailList.Count() == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReceiveDetailCanNotBeEmpty);
                }

                orderMgr.ReceiveOrder(orderDetailList);
                object obj = new { SuccessMessage = string.Format(Resources.ORD.OrderMaster.OrderMaster_Received, checkedOrders), SuccessData = checkedOrders };
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
        [SconitAuthorize(Permissions = "Url_OrderDetail_Procurement_View")]
        public ActionResult DetailIndex()
        {
            TempData["OrderMasterSearchModel"] = null;
            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
        public ActionResult BatchProcessIndex()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
        public ActionResult BatchProcessList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
        public ActionResult _AjaxBatchProcessList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = "and o.OrderStrategy != " + (int)com.Sconit.CodeMaster.FlowStrategy.SEQ;
            searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Submit")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Delete")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Cancel")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Close")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
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


        #endregion

        #region 打印导出
        public void SaveToClient(string orderNo)
        {
            try
            {
                OrderMaster orderMaster = queryMgr.FindById<OrderMaster>(orderNo);
                IList<OrderDetail> orderDetails = queryMgr.FindAll<OrderDetail>("select od from OrderDetail as od where od.OrderNo=?", orderNo);
                foreach (var orderDetail in orderDetails)
                {
                    if (!string.IsNullOrEmpty(orderDetail.Direction))
                    {
                        try
                        {
                            orderDetail.Direction = this.genericMgr.FindById<HuTo>(orderDetail.Direction).CodeDescription;
                        }
                        catch (Exception)
                        { }
                    }
                }
                orderMaster.OrderDetails = orderDetails;
                PrintOrderMaster printOrderMstr = Mapper.Map<OrderMaster, PrintOrderMaster>(orderMaster);
                IList<object> data = new List<object>();
                data.Add(printOrderMstr);
                data.Add(printOrderMstr.OrderDetails);
                //Dictionary<int, IList<object>> dic = new Dictionary<int, IList<object>>();
                //dic.Add(0, data);
                //dic.Add(1, data);
                //dic.Add(2, data);
                //string reportFileUrl = reportGen.WriteToFile(orderMaster.OrderTemplate, data);
                reportGen.WriteToClient(orderMaster.OrderTemplate, data, orderMaster.OrderNo + ".xls");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
            //return reportFileUrl;
            //reportGen.WriteToFile(orderMaster.OrderTemplate, data);
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
            orderMaster.OrderDetails = orderDetails;
            PrintOrderMaster printOrderMstr = Mapper.Map<OrderMaster, PrintOrderMaster>(orderMaster);
            IList<object> data = new List<object>();
            data.Add(printOrderMstr);
            data.Add(printOrderMstr.OrderDetails);
            string reportFileUrl = reportGen.WriteToFile(orderMaster.OrderTemplate, data);
            //reportGen.WriteToClient(orderMaster.OrderTemplate, data, orderMaster.OrderTemplate);

            return reportFileUrl;
            //reportGen.WriteToFile(orderMaster.OrderTemplate, data);
        }

        #endregion

        #region 打印配送标签
        //public string PrintDistributeLabel(string orderNo)
        //{
        //    IList<Hu> huList = new List<Hu>();
        //    OrderMaster orderMaster = queryMgr.FindById<OrderMaster>(orderNo);
        //    IList<OrderDetail> orderDetails = queryMgr.FindAll<OrderDetail>("select od from OrderDetail as od where od.OrderNo=?", orderNo);

        //    huList = huMgr.MatchNewHuForRepack(orderDetails, orderMaster.OrderStrategy == Sconit.CodeMaster.FlowStrategy.JIT ? true : false);

        //    IList<PrintOrderDetail> printOrderetails= Mapper.Map<IList<OrderDetail>,IList<PrintOrderDetail>>(orderDetails);
        //    IList<object> data = new List<object>();
        //    data.Add(printOrderetails);
        //    string reportFileUrl = reportGen.WriteToFile("DistributeLabel.xls", data);
        //    return reportFileUrl;
        //}
        #endregion

        #region return order
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_ReturnIndex")]
        public ActionResult ReturnIndex()
        {
            return View();
        }

        #region return edit

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_ReturnIndex")]
        public ActionResult ReturnList(GridCommand command, OrderMasterSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_ReturnIndex")]
        public ActionResult _ReturnAjaxList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string replaceFrom = "_ReturnAjaxList";
            string replaceTo = "ReturnList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }
            string whereStatement = "and o.OrderStrategy != " + (int)com.Sconit.CodeMaster.FlowStrategy.SEQ;
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
                //string sql = "select count(*) as counter from FlowMaster as m where m.Type=" + (int)com.Sconit.CodeMaster.OrderType.Procurement + " and m.PartyFrom='" + orderDetail.ManufactureParty + "' and  m.Code in (select distinct d.Flow from FlowDetail as d where d.Item='" + orderDetail.Item + "' )";

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

            return newOrderDetail;
        }
        #endregion

        #region Nondelivery 要货未到信息
        [SconitAuthorize(Permissions = "Url_Nondelivery_Procurement_View")]
        public ActionResult NondeliveryIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Nondelivery_Procurement_View")]
        public ActionResult NondeliveryList(GridCommand command, OrderMasterSearchModel searchModel)
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
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxNondeliveryList(GridCommand command, OrderMasterSearchModel searchModel)
        {

            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
            SearchStatementModel searchStatementModel = PrepareNondeliverySearchStatement(command, searchModel);
            GridModel<OrderDetail> gridList = GetAjaxPageData<OrderDetail>(searchStatementModel, command);
            if (gridList.Data != null && gridList.Data.Count() > 0)
            {
                string[] orderNoArray = gridList.Data.Select(o => o.OrderNo).ToArray();
                string hql = string.Empty;
                IList<object> param = new List<object>();
                foreach (string orderNo in orderNoArray)
                {
                    if (string.IsNullOrEmpty(hql))
                    {
                        hql = "select o from  OrderMaster as o where o.OrderNo in (?";

                    }
                    else
                    {
                        hql += ",?";
                    }
                    param.Add(orderNo);
                }
                hql += ")";
                IList<OrderMaster> OrderMstrList = this.genericMgr.FindAll<OrderMaster>(hql, param.ToArray());
                if (OrderMstrList != null && OrderMstrList.Count > 0)
                {
                    foreach (OrderMaster orM in OrderMstrList)
                    {
                        foreach (OrderDetail orD in gridList.Data)
                        {
                            if (orD.OrderNo == orM.OrderNo)
                            {
                                orD.WindowTime = orM.WindowTime;
                                orD.PartyFromName = orM.PartyFromName + "[" + orM.PartyFrom + "]";
                                orD.PartyToName = orM.PartyToName + "[" + orM.PartyTo + "]";
                            }
                            else
                            {
                                continue;
                            }
                        }

                    }
                }
            }
            return PartialView(gridList);
        }

        private SearchStatementModel PrepareNondeliverySearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            StringBuilder sb = new StringBuilder();
            string whereStatement = string.Empty;
            if (searchModel.Type.HasValue && searchModel.Type > 0)
            {
                whereStatement = "where exists(select 1 from OrderMaster as o where o.OrderNo=d.OrderNo and o.OrderStrategy != "
                   + (int)com.Sconit.CodeMaster.FlowStrategy.SEQ + " and o.Type = "
                   + searchModel.Type
                   + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            }
            else
            {
                whereStatement = "where exists(select 1 from OrderMaster as o where o.OrderNo=d.OrderNo and o.OrderStrategy != "
                    + (int)com.Sconit.CodeMaster.FlowStrategy.SEQ + " and o.Type in ("
                    + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                    + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + ")"
                    + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            }
            sb.Append(whereStatement);
            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                sb.Append(string.Format(" and o.WindowTime between '{0}' and '{1}'", searchModel.DateFrom, searchModel.DateTo));

            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                sb.Append(string.Format(" and o.WindowTime >= '{0}'", searchModel.DateFrom));

            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                sb.Append(string.Format(" and o.WindowTime <= '{0}'", searchModel.DateTo));

            }
            if (!string.IsNullOrEmpty(searchModel.Flow))
            {
                sb.Append(string.Format(" and o.Flow = '{0}'", searchModel.Flow));
            }
            if (!string.IsNullOrEmpty(searchModel.PartyFrom))
            {
                sb.Append(string.Format(" and o.PartyFrom = '{0}'", searchModel.PartyFrom));
            }
            if (!string.IsNullOrEmpty(searchModel.PartyTo))
            {
                sb.Append(string.Format(" and o.PartyTo = '{0}'", searchModel.PartyTo));
            }
            if (searchModel.LocationTo != null)
            {
                sb.Append(string.Format(" and o.LocationTo = '{0}'", searchModel.LocationTo));
            }
            sb.Append(")");
            whereStatement = sb.ToString();
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "d", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "d", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " and d.ReceivedQty<d.OrderedQty order by d.CreateDate desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = "select count(*) from OrderDetail as d";
            searchStatementModel.SelectStatement = "select d from OrderDetail as d";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderDetail_Procurement_View")]
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
        [SconitAuthorize(Permissions = "Url_OrderDetail_Procurement_View")]
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
            var list = GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command);
            IList<OrderMaster> OrderList = genericMgr.FindAllIn<OrderMaster>("from OrderMaster where OrderNo in (?", list.Data.Select(o => o.OrderNo).Distinct());
            foreach (var data in list.Data)
            {
                data.WindowTime = OrderList.Where(o => o.OrderNo == data.OrderNo).FirstOrDefault().WindowTime;
            }
            return PartialView(list);
        }
        #region  Export detail search
        [SconitAuthorize(Permissions = "Url_OrderDetail_Procurement_View")]
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
            IList<OrderMaster> OrderList = genericMgr.FindAllIn<OrderMaster>("from OrderMaster where OrderNo in (?", list.Data.Select(o => o.OrderNo).Distinct());
            foreach (var data in list.Data)
            {
                data.WindowTime = OrderList.Where(o => o.OrderNo == data.OrderNo).FirstOrDefault().WindowTime;
            }
            ExportToXLS<OrderDetail>("ProcumentOrderDetail.xls", list.Data.ToList());
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
                else if (command.SortDescriptors[0].Member == "ItemDescription")
                {
                    command.SortDescriptors[0].Member = "Item";
                }
                else if (command.SortDescriptors[0].Member == "UnitCountDescription")
                {
                    command.SortDescriptors[0].Member = "UnitCount";
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
        [SconitAuthorize(Permissions = "Url_OrderDetail_Procurement_Return")]
        public ActionResult ReturnDetailList(GridCommand command, OrderMasterSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_OrderDetail_Procurement_Return")]
        public ActionResult _AjaxReturnOrderDetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchReturnDetailStatement(command, searchModel);
            return PartialView(GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command));
        }
        #region  Export return detail search
        [SconitAuthorize(Permissions = "Url_OrderDetail_Procurement_Return")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportReturn(OrderMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchReturnDetailStatement(command, searchModel);
            var list = GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command);
            ExportToXLS<OrderDetail>("ProcumentReturnOrderDetail.xls", list.Data.ToList());
        }
        #endregion

        private ProcedureSearchStatementModel PrepareSearchReturnDetailStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Format(" and exists (select 1 from OrderMaster  as o where  o.SubType ={0} and o.OrderNo=d.OrderNo ) ",
                (int)com.Sconit.CodeMaster.OrderSubType.Return);

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


        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Import")]
        public ActionResult ImportProcurementOrder(IEnumerable<HttpPostedFileBase> attachments,
          string Flow, DateTime? StartTime, DateTime? WindowTime, string EffectiveDate,
          string ReferenceOrderNo, string ExternalOrderNo, com.Sconit.CodeMaster.OrderPriority? Priority)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Flow))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_FlowCanNotBeEmpty);
                }

                if (!StartTime.HasValue)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_StartToTimeCanNotBeEmpty);

                }

                if (!WindowTime.HasValue)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_WindowTimeCanNotBeEmpty);
                }
                foreach (var file in attachments)
                {
                    string orderNo = orderMgr.CreateProcurementOrderFromXls(file.InputStream, Flow, ExternalOrderNo, ReferenceOrderNo,
                           StartTime.Value, WindowTime.Value, Priority.Value);

                    object obj = Resources.EXT.ControllerLan.Con_PurchaseOrder + orderNo + Resources.EXT.ControllerLan.Con_GeneratedSuccessfully;
                    return Json(new { status = obj }, "text/plain");
                }
            }
            catch (BusinessException ex)
            {
                Response.Write(ex.GetMessages()[0].GetMessageString());
            }
            return Content("");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Import")]
        public ActionResult Import()
        {
            return View();
        }

        private ProcedureSearchStatementModel PrepareReceiveSearchStatement_1(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = " and o.IsRecScanHu = 0 and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + "," + (int)com.Sconit.CodeMaster.OrderStatus.Submit + ")"
                                    + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal
                                    + " and exists (select 1 from OrderDetail as d where d.RecQty < d.OrderQty and d.OrderNo = o.OrderNo) ";
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
                        + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine,
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
        [SconitAuthorize(Permissions = "Url_OrderDetail_UnitPrice_View")]
        public ActionResult DetailUnitPriceIndex()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderDetail_UnitPrice_View")]
        public ActionResult DetailUnitPriceList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (string.IsNullOrEmpty(searchModel.OrderNo))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_OrderNumberCanNotBeEmpty);
                return View();
            }
            IList<OrderMaster> orderMasterList = genericMgr.FindAll<OrderMaster>(" from OrderMaster o where o.OrderNo=?", searchModel.OrderNo);
            if (orderMasterList.Count < 1)
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_OrderNumberNotExits);
                return View();
            }
            else
            {
                if (orderMasterList[0].Status == com.Sconit.CodeMaster.OrderStatus.Cancel || orderMasterList[0].Status == com.Sconit.CodeMaster.OrderStatus.Close || orderMasterList[0].Status == com.Sconit.CodeMaster.OrderStatus.Complete
                    || orderMasterList[0].Status == com.Sconit.CodeMaster.OrderStatus.Submit || orderMasterList[0].Status == com.Sconit.CodeMaster.OrderStatus.InProcess)
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_TheOrderNumberAlreadyCanNotModificate);
                    return View();
                }
                if (orderMasterList[0].Type != Sconit.CodeMaster.OrderType.Procurement
                && orderMasterList[0].Type != Sconit.CodeMaster.OrderType.SubContract)
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_IsNotPurchaseConsignmentOrderCanNotModifyPrice);
                    return View();
                }
                if (!Utility.SecurityHelper.HasPermission(orderMasterList[0]))
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_LackModificateTheOrderPermission);
                    return View();
                }
            }

            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }

            ViewBag.Item = searchModel.Item;
            ViewBag.OrderNo = searchModel.OrderNo;
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderDetail_UnitPrice_View")]
        public ActionResult _DetailUnitPriceAjaxList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel) || string.IsNullOrEmpty(searchModel.OrderNo))
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputOrderNumber);
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
            var orderMaster = orderMgr.GetAuthenticOrder(searchModel.OrderNo);

            if (orderMaster == null
                || orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Cancel
                || orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Close
                || orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Complete)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_OrderNumberNotExits);
                return PartialView(new GridModel(new List<OrderDetail>()));
            }

            if (orderMaster.Type != Sconit.CodeMaster.OrderType.Procurement
             && orderMaster.Type != Sconit.CodeMaster.OrderType.SubContract)
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
            if (!Utility.SecurityHelper.HasPermission(orderMaster))
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }

            var orderDetailList = this.genericMgr.FindAll<OrderDetail>("from OrderDetail where OrderNo =?", searchModel.OrderNo);
            return PartialView(new GridModel(orderDetailList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderDetail_UnitPrice_View")]
        public ActionResult _DetailUnitPriceUpdate(OrderDetail updateOrderDetail, string Item, string OrderNo)
        {
            try
            {
                ModelState.Remove("Item");
                OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(updateOrderDetail.Id);
                orderDetail.UnitPrice = updateOrderDetail.UnitPrice;
                orderDetail.IsProvisionalEstimate = updateOrderDetail.IsProvisionalEstimate;
                genericMgr.Update(orderDetail);

                IList<object> param = new List<object>();
                string hql = " from OrderDetail o where 1=1";
                if (!string.IsNullOrEmpty(OrderNo))
                {
                    hql += " and o.OrderNo=?";
                    param.Add(OrderNo);
                }
                if (!string.IsNullOrEmpty(Item))
                {
                    hql += " and o.Item=?";
                    param.Add(Item);
                }

                IList<OrderDetail> OrderDetailList = genericMgr.FindAll<OrderDetail>(hql, param.ToArray());
                return PartialView(new GridModel(OrderDetailList));
            }
            catch (BusinessException ex)
            {
                //Response.TrySkipIisCustomErrors = true;
                //Response.StatusCode = 500;
                //Response.Write(ex.GetMessages()[0].GetMessageString());
                //return Json(null);
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return Content("");
            }
        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_New")]
        public ActionResult _OrderDetailListFromReferenceOrder(string flow, string ReferenceOrderNo)
        {
            ViewBag.isManualCreateDetail = false;
            ViewBag.flow = flow;
            ViewBag.ReferenceOrderNo = ReferenceOrderNo;
            ViewBag.newOrEdit = "New";
            ViewBag.status = null;
            ViewBag.orderSubType = com.Sconit.CodeMaster.OrderSubType.Normal;
            ViewBag.onsumitType = 1;
            FlowMaster flowMaster = null;

            if (!string.IsNullOrEmpty(flow))
            {
                flowMaster = genericMgr.FindById<FlowMaster>(flow);
                ViewBag.PartyFrom = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? flowMaster.PartyFrom : flowMaster.PartyTo;
                ViewBag.PartyTo = ViewBag.orderSubType == com.Sconit.CodeMaster.OrderSubType.Normal ? flowMaster.PartyTo : flowMaster.PartyFrom;
                ViewBag.isManualCreateDetail = flowMaster.IsManualCreateDetail;
            }
            ViewBag.IsProcurement = false;
            if (flowMaster.Type == Sconit.CodeMaster.OrderType.Procurement
                || flowMaster.Type == Sconit.CodeMaster.OrderType.CustomerGoods
                || flowMaster.Type == Sconit.CodeMaster.OrderType.ScheduleLine
                || flowMaster.Type == Sconit.CodeMaster.OrderType.SubContract)
            {
                ViewBag.IsProcurement = true;
            }
            #region comboBox
            if (flowMaster.IsManualCreateDetail)
            {
                IList<Uom> uoms = genericMgr.FindAll<Uom>();
                ViewData.Add("uoms", uoms);
            }
            #endregion
            return PartialView();
        }

        #region RequisitionOrder
        [SconitAuthorize(Permissions = "Url_ProcurementOrder_RequisitionOrder")]
        public ActionResult RequisitionOrder()
        {
            RequisitionOrderSearchModel searchModel = new RequisitionOrderSearchModel();
            searchModel.DateFrom = DateTime.Today;
            searchModel.DateTo = DateTime.Today.AddDays(1);
            ProcessSearchModel(null, searchModel);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ProcurementOrder_RequisitionOrder")]
        public ActionResult _RequisitionOrderList(RequisitionOrderSearchModel searchModel)
        {
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_ProcurementOrder_RequisitionOrder")]
        public ActionResult RequisitionTransferOrder(RequisitionTransferOrderSearchModel searchModel)
        {
            ViewBag.SearchModel = searchModel;
            searchModel.StartTime = DateTime.Parse(_WindowTime(searchModel.TransferFlow, searchModel.WindowTime.ToString("yyyy-MM-dd HH:mm")));
            return View(searchModel);
        }

        [SconitAuthorize(Permissions = "Url_ProcurementOrder_RequisitionOrder")]
        public ActionResult _RequisitionSectionPlanList(RequisitionOrderSearchModel searchModel)
        {
            return PartialView();
        }

        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ProcurementOrder_RequisitionOrder")]
        public JsonResult _SelectRequisitionSectionPlanList(RequisitionOrderSearchModel searchModel)
        {
            IList<MrpExSectionPlan> planList = new List<MrpExSectionPlan>();
            try
            {
                if (searchModel != null && !string.IsNullOrEmpty(searchModel.TransferFlow)
                    && searchModel.DateFrom.HasValue && searchModel.DateTo.HasValue)
                {
                    var paramList = new List<object>();
                    paramList.Add(Sconit.CodeMaster.OrderStatus.Submit);
                    paramList.Add(Sconit.CodeMaster.OrderStatus.InProcess);
                    paramList.Add(Utility.DateTimeHelper.GetWeekOfYear(searchModel.DateFrom.Value));
                    paramList.Add(Utility.DateTimeHelper.GetWeekOfYear(searchModel.DateTo.Value));
                    paramList.Add(searchModel.DateFrom.Value);
                    paramList.Add(searchModel.DateFrom.Value);
                    paramList.Add(searchModel.DateFrom.Value);
                    paramList.Add(searchModel.DateTo.Value);
                    string sql = @"select s.* from MRP_MrpExSectionPlan s inner join MRP_MrpExOrder p on s.Id = p.SectionId
                                            where p.Status in(?,?) and (p.DateIndex =? or p.DateIndex=?)
                                            and (p.StartTime<=? and p.WindowTime>=? ) or (p.StartTime>=? and p.StartTime<=?) ";
                    if (!string.IsNullOrWhiteSpace(searchModel.ProductLine))
                    {
                        paramList.Add(searchModel.ProductLine);
                        sql += " and p.ProductLine = ? ";
                    }
                    planList = this.genericMgr.FindEntityWithNativeSql<MrpExSectionPlan>(sql, paramList.ToArray());
                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputTransferFlowStartToTimeEndTimeSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(planList);
        }

        private decimal GetRoundOrderQty(FlowDetail flowDetail, decimal orderQty)
        {
            if (orderQty < 0)
            {
                return 0;
            }
            if (flowDetail.MinLotSize > 0 && orderQty < flowDetail.MinLotSize)
            {
                orderQty = flowDetail.MinLotSize;
            }
            if (flowDetail.UnitCount > 0)
            {
                if (flowDetail.RoundUpOption == com.Sconit.CodeMaster.RoundUpOption.ToUp)
                {
                    orderQty = Math.Ceiling(orderQty / flowDetail.UnitCount) * flowDetail.UnitCount;
                }
                else if (flowDetail.RoundUpOption == com.Sconit.CodeMaster.RoundUpOption.ToDown)
                {
                    orderQty = Math.Floor(orderQty / flowDetail.UnitCount) * flowDetail.UnitCount;
                }
            }
            return orderQty;
        }

        [SconitAuthorize(Permissions = "Url_ProcurementOrder_RequisitionOrder")]
        public ActionResult _RequisitionOrderDetailList(RequisitionOrderSearchModel searchModel)
        {
            var orderDetailList = new List<OrderDetail>();
            try
            {
                #region
                IList<string> prodLineList = new List<string>();
                if (!string.IsNullOrWhiteSpace(searchModel.ProductLine))
                {
                    prodLineList.Add(searchModel.ProductLine.Trim());
                }
                else
                {
                    prodLineList = this.genericMgr.FindAll<string>
                    ("select Code from FlowMaster where ResourceGroup = ?", com.Sconit.CodeMaster.ResourceGroup.EX);
                }
                var flowDetailDic = this.flowMgr.GetFlowDetailList(searchModel.TransferFlow, false, true)
                        .GroupBy(p => p.Item, (k, g) => new { Item = k, FlowDetail = g.First() })
                        .ToDictionary(d => d.Item, d => d.FlowDetail);

                var shiftPlanList = new List<MrpExShiftPlan>();
                DateTime currentDate = searchModel.DateFrom.Value.Date;
                while (currentDate <= searchModel.DateTo.Value.Date)
                {
                    foreach (var flow in prodLineList)
                    {
                        var shiftPlans = mrpMgr.GetMrpExShiftPlanList(currentDate, flow) ?? new List<MrpExShiftPlan>();
                        shiftPlanList.AddRange(shiftPlans);
                    }
                    currentDate = currentDate.AddDays(1);
                }

                foreach (var shiftPlan in shiftPlanList)
                {
                    double _minutes = (shiftPlan.CalShiftQty - shiftPlan.ShiftQty) * (24 / shiftPlan.ShiftType) * 60;
                    _minutes = _minutes > 0 ? _minutes : 0;
                    double qty = _minutes * shiftPlan.Speed / shiftPlan.RateQty;
                    shiftPlan.CurrentQty = shiftPlan.Qty - qty;
                    shiftPlan.CurrentQty = shiftPlan.CurrentQty > 0 ? shiftPlan.CurrentQty : 0.0;
                }

                DateTime winTime1 = searchModel.DateFrom.Value;
                DateTime winTime2 = searchModel.DateTo.Value;

                var huToMappings = this.genericMgr.FindAll<HuToMapping>();

                #region 计算winTime1和winTime2之间的需求
                orderDetailList = shiftPlanList.Where(p => p.StartTime < winTime2 && p.WindowTime > winTime1)
                    .Where(p => p.WindowTime > p.StartTime)
                    .Where(p => p.Bom != com.Sconit.Entity.BusinessConstants.VIRTUALSECTION && !string.IsNullOrWhiteSpace(p.Bom))
                    .Select(p => new
                    {
                        Tracer = p,
                        Flow = p.ProductLine,
                        Item = p.Item,
                        Bom = p.Bom,
                        Qty = p.CurrentQty * ((p.WindowTime < winTime2 ? p.WindowTime : winTime2) - (p.StartTime > winTime1 ? p.StartTime : winTime1)).TotalMinutes / (p.WindowTime - p.StartTime).TotalMinutes
                    })
                    .GroupBy(p => new { p.Flow, p.Item, p.Bom }, (k, g) => new
                    {
                        Flow = k.Flow,
                        ParentItem = k.Item,
                        UnitQty = itemMgr.ConvertItemUomQty(k.Item, itemMgr.GetCacheItem(k.Item).Uom, 1, bomMgr.GetCacheBomMaster(k.Bom).Uom),//Bom单位转成物料单位
                        Qty = (decimal)(g.Sum(p => p.Qty)),
                        Details = bomMgr.GetFlatBomDetail(k.Bom, winTime1),
                        Tracers = g
                    })
                    .SelectMany(p => p.Details.Where(q => flowDetailDic.ContainsKey(q.Item)).Select(q => new
                    {
                        Flow = p.Flow,
                        Direction = customizationMgr.GetHuTo(huToMappings, p.Flow, p.ParentItem),
                        Item = q.Item,
                        Qty = itemMgr.ConvertItemUomQty(q.Item, q.Uom, q.CalculatedQty * p.UnitQty * p.Qty, flowDetailDic.ValueOrDefault(q.Item).Uom),//转订单单位
                        Tracers = p.Tracers
                    }))
                    .GroupBy(p => new { p.Flow, p.Direction, p.Item }, (k, g) => new
                    {
                        Flow = k.Flow,
                        Direction = k.Direction,
                        Item = itemMgr.GetCacheItem(k.Item),
                        FlowDetail = flowDetailDic.ValueOrDefault(k.Item),
                        Qty = g.Sum(q => q.Qty),
                        Tracers = g.SelectMany(q => q.Tracers)
                    })
                    .Select(p => new Entity.ORD.OrderDetail
                    {
                        Item = p.Item.Code,
                        UnitCount = p.FlowDetail.UnitCount,
                        Uom = p.FlowDetail.Uom,
                        ItemDescription = p.Item.Description,
                        Sequence = p.FlowDetail.Sequence,
                        RequiredQty = p.Qty,
                        OrderedQty = GetRoundOrderQty(p.FlowDetail, p.Qty),
                        LocationFrom = p.FlowDetail.LocationFrom,
                        LocationTo = p.FlowDetail.LocationTo,
                        Direction = p.Direction,
                        Remark = string.Format("{0}{1}", string.IsNullOrWhiteSpace(p.Direction) ? p.Flow + " " : null, GetOrderDetailRemark(p.Tracers.Select(q => string.IsNullOrWhiteSpace(q.Tracer.Section) ? q.Tracer.Item : q.Tracer.Section).Distinct())),
                        OrderTracerList = p.Tracers.Select(q => new Entity.ORD.OrderTracer
                        {
                            Code = q.Tracer.Section,
                            FinishedQty = 0,
                            Item = q.Item,
                            //OrderDetailId = ,
                            OrderedQty = (decimal)q.Tracer.CurrentQty,
                            Qty = (decimal)q.Qty,
                            RefId = q.Tracer.Id,
                            ReqTime = q.Tracer.StartTime
                        }).ToList()
                    })
                    //.Where(p => p.OrderedQty > 0)
                    .OrderBy(p => p.Direction)
                    .ThenBy(p => p.Sequence)
                    .ToList();
                #endregion

                string location = flowDetailDic.First().Value.DefaultLocationTo;
                //Dictionary<string, decimal> invATPQtyDic = locationDetailMgr.GetInvATPQty(location, orderDetailList.Select(p => p.Item).ToList());
                foreach (var orderDetail in orderDetailList)
                {
                    //orderDetail.InvQty = invATPQtyDic.ValueOrDefault(orderDetail.Item);
                    var invQty = 0;//orderDetail.InvQty > 0 ? orderDetail.InvQty : 0;
                    orderDetail.OrderedQty = GetRoundOrderQty(flowDetailDic.ValueOrDefault(orderDetail.Item), orderDetail.RequiredQty - invQty);
                }
                #endregion
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return PartialView(orderDetailList);
        }

        private string GetOrderDetailRemark(IEnumerable<string> sections)
        {
            string remark = string.Empty;
            foreach (var section in sections)
            {
                var itemDesc = itemMgr.GetCacheItem(section).Description;
                if (remark == string.Empty)
                {
                    remark = itemDesc;
                }
                else
                {
                    remark += "," + itemDesc;
                }
            }
            return remark;
        }

        /* 废弃
                        #region
                FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(searchModel.TransferFlow);

                var flowDetails = this.flowMgr.GetFlowDetailList(flowMaster);
                flowDetails = (from det in flowDetails
                               group det by
                               new
                               {
                                   LocationFrom = !string.IsNullOrWhiteSpace(det.LocationFrom) ? det.LocationFrom : det.CurrentFlowMaster.LocationFrom,
                                   LocationTo = !string.IsNullOrWhiteSpace(det.LocationTo) ? det.LocationTo : det.CurrentFlowMaster.LocationTo,
                                   StartDate = det.StartDate,
                                   EndDate = det.EndDate,
                                   Item = det.Item,
                               } into result
                               select result.Max<FlowDetail>()).ToList();

                var locationCodes = (flowDetails.Select(p => p.LocationFrom).Union(flowDetails.Select(p => p.LocationTo)))
                    .Distinct().Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                if (!string.IsNullOrWhiteSpace(flowMaster.LocationFrom))
                {
                    locationCodes.Add(flowMaster.LocationFrom);
                }
                if (!string.IsNullOrWhiteSpace(flowMaster.LocationTo))
                {
                    locationCodes.Add(flowMaster.LocationTo);
                }
                var locations = this.genericMgr.FindAllIn<Location>(" from Location where Code in(?", locationCodes);
                #endregion

                if (searchModel.IsPlan)
                {


                    #region IsPlan
                    foreach (var detail in searchModel.Details)
                    {
                        var item = detail[0];
                        var productLine = detail[1];
                        var qty = decimal.Parse(detail[2]);
                        var bomDetailList = bomMgr.GetFlatBomDetail(item, DateTime.Now);

                        var fg = itemMgr.GetCacheItem(item);
                        var bomMaster = this.bomMgr.GetCacheBomMaster(item);
                        //1.将bomMaster的单位转成基本单位 
                        var fgQty = itemMgr.ConvertItemUomQty(fg.Code, bomMaster.Uom, 1, fg.Uom);

                        foreach (var flowDetail in flowDetails)
                        {
                            var bomDetails = bomDetailList.Where(p => p.Item == flowDetail.Item);
                            foreach (var bomDetail in bomDetails)
                            {
                                var orderDetail = orderDetailList.FirstOrDefault(p => p.Item == flowDetail.Item && p.Direction == productLine);
                                if (orderDetail != null)
                                {
                                    orderDetail.OrderedQty += (itemMgr.ConvertItemUomQty(item, bomDetail.Uom, bomDetail.CalculatedQty * qty, flowDetail.Uom) / fgQty);
                                }
                                else
                                {
                                    OrderDetail newOrderDetail = Mapper.Map<FlowDetail, OrderDetail>(flowDetail); ;
                                    newOrderDetail.ItemDescription = bomDetail.CurrentItem.Description;
                                    newOrderDetail.OrderedQty = itemMgr.ConvertItemUomQty(item, bomDetail.Uom, bomDetail.CalculatedQty * qty, flowDetail.Uom);
                                    newOrderDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flowMaster.LocationFrom : flowDetail.LocationFrom;
                                    newOrderDetail.LocationFromName = string.IsNullOrWhiteSpace(newOrderDetail.LocationFrom) ? null : locations.First(p => p.Code == newOrderDetail.LocationFrom).Name;
                                    newOrderDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flowMaster.LocationTo : flowDetail.LocationTo;
                                    newOrderDetail.LocationToName = string.IsNullOrWhiteSpace(newOrderDetail.LocationTo) ? null : locations.First(p => p.Code == newOrderDetail.LocationTo).Name;
                                    //newOrderDetail.Direction = productLine;
                                    orderDetailList.Add(newOrderDetail);
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region IsOrder
                    foreach (var detail in searchModel.Details)
                    {
                        var referenceOrderNo = detail[0];
                        var orderBomDetailList = genericMgr.FindAll<OrderBomDetail>
                              (string.Format(" from OrderBomDetail o where o.OrderNo='{0}' ", referenceOrderNo));
                        foreach (var bomDetail in orderBomDetailList)
                        {
                            var flowDetail = flowDetails.FirstOrDefault(r => r.Item == bomDetail.Item);
                            if (flowDetail != null)
                            {
                                var oldOrderDetails = orderDetailList.Where(o => o.Item == bomDetail.Item);
                                if (oldOrderDetails != null && oldOrderDetails.Count() > 0)
                                {
                                    var orderDetail = oldOrderDetails.First();
                                    orderDetail.OrderedQty += itemMgr.ConvertItemUomQty(orderDetail.Item, bomDetail.Uom, bomDetail.OrderedQty, orderDetail.Uom);
                                }
                                else
                                {
                                    var orderDetail = new OrderDetail();
                                    orderDetail.Item = bomDetail.Item;
                                    orderDetail.ItemDescription = bomDetail.ItemDescription;
                                    orderDetail.ManufactureParty = bomDetail.ManufactureParty;
                                    orderDetail.ReferenceItemCode = bomDetail.ReferenceItemCode;
                                    orderDetail.Sequence = flowDetail.Sequence;
                                    orderDetail.UnitCount = flowDetail.UnitCount;
                                    orderDetail.UnitQty = bomDetail.UnitQty;
                                    orderDetail.UnitCountDescription = flowDetail.UnitCountDescription;
                                    orderDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flowMaster.LocationFrom : flowDetail.LocationFrom;
                                    orderDetail.LocationFromName = string.IsNullOrWhiteSpace(orderDetail.LocationFrom) ? null : locations.First(p => p.Code == orderDetail.LocationFrom).Name;
                                    orderDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flowMaster.LocationTo : flowDetail.LocationTo;
                                    orderDetail.LocationToName = string.IsNullOrWhiteSpace(orderDetail.LocationTo) ? null : locations.First(p => p.Code == orderDetail.LocationTo).Name;
                                    orderDetail.Uom = bomDetail.Uom;
                                    orderDetail.OrderedQty = itemMgr.ConvertItemUomQty(orderDetail.Item, bomDetail.Uom, bomDetail.OrderedQty, flowDetail.Uom);
                                    orderDetailList.Add(orderDetail);
                                }
                            }
                        }
                    }
                    #endregion
                }
         
         */

        [GridAction]
        [SconitAuthorize(Permissions = "Url_ProcurementOrder_RequisitionOrder")]
        public JsonResult _SelectRequisitionOrderList(RequisitionOrderSearchModel searchModel)
        {
            IList<MrpExSectionPlan> planList = new List<MrpExSectionPlan>();
            try
            {
                if (searchModel != null && !string.IsNullOrEmpty(searchModel.TransferFlow) && searchModel.DateFrom.HasValue && searchModel.DateTo.HasValue)
                {

                }
                else
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputTransferFlowStartToTimeEndTimeSearch);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(planList);
        }

        #endregion


        #region NewFromPlan
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_New")]
        public ActionResult NewFromPlan(MaterailPlanSearchModel searchModel)
        {
            //SearchCacheModel searchCacheModel = this.ProcessSearchModel(null, searchModel);
            FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(searchModel.Flow);
            OrderMaster orderMaster = this.orderMgr.TransferFlow2Order(flowMaster, false);
            orderMaster.WindowTime = searchModel.WindowTime;
            ViewBag.WindowTime = searchModel.WindowTime;
            ViewBag.PlanVersion = searchModel.PlanVersion;
            ViewBag.FlowDescription = flowMaster.Description;
            ViewBag.MaterialsGroup = searchModel.MaterialsGroup;
            ViewBag.Item = searchModel.Item;

            FlowStrategy flowStrategy = genericMgr.FindById<FlowStrategy>(searchModel.Flow);
            if (flowStrategy != null)
            {
                double leadTime = DateTimeHelper.TimeTranfer(flowStrategy.LeadTime, flowStrategy.TimeUnit, Sconit.CodeMaster.TimeUnit.Hour);
                orderMaster.StartTime = orderMaster.WindowTime.AddHours(-leadTime);
            }
            ViewBag.BackUrl = searchModel.BackUrl;
            //ViewBag.isEditable = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create && this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Procurement_Edit");
            //ViewBag.editorTemplate = ViewBag.isEditable ? "" : "ReadonlyTextBox";

            return View(orderMaster);
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_New")]
        public ActionResult _OrderDetailListFromPlan(string Flow, DateTime WindowTime, DateTime PlanVersion, string MaterialsGroup, string Item, string checkedItems)
        {
            FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(Flow);
            ViewBag.isManualCreateDetail = false;
            ViewBag.Flow = Flow;
            ViewBag.WindowTime = WindowTime;
            ViewBag.MaterialsGroup = MaterialsGroup;
            ViewBag.Item = Item;
            ViewBag.newOrEdit = "New";
            ViewBag.status = null;
            ViewBag.orderSubType = com.Sconit.CodeMaster.OrderSubType.Normal;
            ViewBag.PartyFrom = flowMaster.PartyFrom;
            ViewBag.PartyTo = flowMaster.PartyTo;
            ViewBag.isManualCreateDetail = flowMaster.IsManualCreateDetail;
            ViewBag.status = com.Sconit.CodeMaster.OrderStatus.Create;
            ViewBag.IsListPrice = false;
            ViewBag.PlanVersion = PlanVersion;
            ViewBag.FlowDescription = flowMaster.Description;
            ViewBag.MaterialsGroup = MaterialsGroup;
            ViewBag.Item = Item;
            ViewBag.CheckedItems = checkedItems;
            if (flowMaster.IsListPrice && this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Procurement_ListPrice"))
            {
                ViewBag.IsListPrice = true;
            }
            ViewBag.IsProcurement = false;
            if (flowMaster.Type == Sconit.CodeMaster.OrderType.Procurement
                || flowMaster.Type == Sconit.CodeMaster.OrderType.CustomerGoods
                || flowMaster.Type == Sconit.CodeMaster.OrderType.ScheduleLine
                || flowMaster.Type == Sconit.CodeMaster.OrderType.SubContract)
            {
                ViewBag.IsProcurement = true;
            }
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
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_New")]
        public ActionResult _SelectDetailFromPlan(string Flow, DateTime WindowTime, DateTime PlanVersion, string MaterialsGroup, string Item, string CheckedItems)
        {
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();
            IList<string> checkedItemsArray = new List<string>();
            if (!string.IsNullOrWhiteSpace(CheckedItems))
            {
                checkedItemsArray = CheckedItems.Split(',');
            };
            string selectStatement = string.Empty;
            if (!string.IsNullOrEmpty(Flow) && WindowTime > DateTime.MinValue)
            {
                var mrpPlanList = new List<MrpPlan>();
                var flowMaster = this.genericMgr.FindById<FlowMaster>(Flow);
                var flowDetails = this.flowMgr.GetFlowDetailList(flowMaster, true);

                if (flowMaster.Type == Sconit.CodeMaster.OrderType.SubContractTransfer ||
                    flowMaster.Type == Sconit.CodeMaster.OrderType.Transfer)
                {
                    var paramList = new List<object> { Flow, PlanVersion, WindowTime };
                    var hql = " from TransferPlan as r where r.Flow=? and r.PlanVersion=? and WindowTime=? ";
                    if (!string.IsNullOrEmpty(Item))
                    {
                        hql += " and r.Item=?";
                        paramList.Add(Item);
                    }
                    var hqlIn = string.Empty;
                    if (!string.IsNullOrEmpty(CheckedItems))
                    {
                        foreach (var item in checkedItemsArray)
                        {
                            if (hqlIn == string.Empty)
                            {
                                hqlIn = " and r.Item in (?";
                            }
                            else
                            {
                                hqlIn += ", ?";
                            }
                            paramList.Add(item);
                        }
                        hqlIn += ") ";
                    }
                    hql = hql + hqlIn;
                    if (!string.IsNullOrEmpty(MaterialsGroup))
                    {
                        hql += " and exists (select 1 from Item as i where r.Item = i.Code and i.MaterialsGroup =? )";
                        paramList.Add(MaterialsGroup);
                    }

                    IList<TransferPlan> transferPlanList = genericMgr.FindAll<TransferPlan>(hql, paramList.ToArray());

                    mrpPlanList = (transferPlanList
                        .GroupBy(p => p.Item, (k, g) => new MrpPlan
                        {
                            Item = k,
                            Qty = g.Sum(q => q.Qty)
                        })).ToList();
                }
                else
                {
                    var paramList = new List<object> { Flow, PlanVersion, WindowTime };
                    var hql = " from PurchasePlan as r where r.Flow=? and r.PlanVersion=? and WindowTime=? ";
                    if (!string.IsNullOrEmpty(Item))
                    {
                        hql += " and r.Item=?";
                        paramList.Add(Item);
                    }
                    var hqlIn = string.Empty;
                    if (!string.IsNullOrEmpty(CheckedItems))
                    {
                        foreach (var item in checkedItemsArray)
                        {
                            if (hqlIn == string.Empty)
                            {
                                hqlIn = " and r.Item in (?";
                            }
                            else
                            {
                                hqlIn += ", ?";
                            }
                            paramList.Add(item);
                        }
                        hqlIn += ") ";
                    }
                    hql = hql + hqlIn;
                    if (!string.IsNullOrEmpty(MaterialsGroup))
                    {
                        hql += " and exists (select 1 from Item as i where r.Item = i.Code and i.MaterialsGroup =? )";
                        paramList.Add(MaterialsGroup);
                    }
                    IList<PurchasePlan> purchasePlanList = genericMgr.FindAll<PurchasePlan>(hql, paramList.ToArray());

                    mrpPlanList = (purchasePlanList
                        .GroupBy(p => p.Item, (k, g) => new MrpPlan
                        {
                            Item = k,
                            Qty = g.Sum(q => q.Qty)
                        })).ToList();
                }

                Dictionary<string, decimal> invATPQtyDic = locationDetailMgr.GetInvATPQty(flowMaster.LocationTo, mrpPlanList.Select(p => p.Item).ToList());
                Dictionary<string, decimal> inTransQtyDic = locationDetailMgr.GetPurchaseInTransQty(flowMaster.LocationTo, mrpPlanList.Select(p => p.Item).ToList());
                foreach (var plan in mrpPlanList)
                {
                    var flowDetail = flowDetails.Where(p => p.Item == plan.Item &&
                      (p.StartDate.HasValue ? p.StartDate.Value <= WindowTime : true
                          && p.EndDate.HasValue ? p.EndDate.Value >= WindowTime : true))
                      .FirstOrDefault();
                    if (flowDetail == null && !flowMaster.IsManualCreateDetail)
                    {
                        continue;
                    }
                    OrderDetail newOrderDetail = new OrderDetail();
                    Item item = genericMgr.FindById<Item>(plan.Item);
                    if (flowDetail == null)
                    {
                        flowDetail = new FlowDetail();
                        flowDetail.UnitCount = item.UnitCount;
                        flowDetail.Uom = item.Uom;
                        //flowDetail.Sequence = item.UnitCount;
                        flowDetail.MinUnitCount = item.UnitCount;
                    }
                    newOrderDetail.Item = item.Code;
                    newOrderDetail.UnitCount = flowDetail.UnitCount;
                    newOrderDetail.Uom = flowDetail.Uom;
                    newOrderDetail.ItemDescription = item.Description;
                    newOrderDetail.Sequence = flowDetail.Sequence;
                    newOrderDetail.MinUnitCount = flowDetail.MinUnitCount;
                    newOrderDetail.RequiredQty = itemMgr.ConvertItemUomQty(item.Code, item.Uom, (decimal)plan.Qty, newOrderDetail.Uom);
                    newOrderDetail.InvQty = itemMgr.ConvertItemUomQty(item.Code, item.Uom, invATPQtyDic.ValueOrDefault(newOrderDetail.Item), newOrderDetail.Uom);
                    newOrderDetail.InTransQty = itemMgr.ConvertItemUomQty(item.Code, item.Uom, inTransQtyDic.ValueOrDefault(newOrderDetail.Item), newOrderDetail.Uom);
                    newOrderDetail.MaxStock = flowDetail.MaxStock;

                    decimal qty = newOrderDetail.RequiredQty;// +newOrderDetail.MaxStock - newOrderDetail.InvQty - newOrderDetail.InTransQty;
                    if (flowMaster.Type == Sconit.CodeMaster.OrderType.SubContractTransfer ||
                    flowMaster.Type == Sconit.CodeMaster.OrderType.Transfer)
                    {
                        qty = newOrderDetail.RequiredQty + newOrderDetail.MaxStock - newOrderDetail.InvQty - newOrderDetail.InTransQty;
                    }

                    if (qty > 0)
                    {
                        newOrderDetail.OrderedQty = orderMgr.GetRoundOrderQty(flowDetail, qty);
                    }
                    else
                    {
                        newOrderDetail.OrderedQty = 0;
                    }
                    if (flowMaster.Type == Sconit.CodeMaster.OrderType.SubContractTransfer || flowMaster.Type == Sconit.CodeMaster.OrderType.Transfer)
                    {
                        newOrderDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                        newOrderDetail.LocationFromName = genericMgr.FindById<Location>(newOrderDetail.LocationFrom).Name;
                    }
                    newOrderDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                    newOrderDetail.LocationToName = genericMgr.FindById<Location>(newOrderDetail.LocationTo).Name;
                    orderDetailList.Add(newOrderDetail);
                }
            }
            return View(new GridModel(orderDetailList.OrderBy(p => p.Sequence)));
        }
        #endregion

        public ActionResult _OrderTracerList(string id)
        {
            ViewBag.OrderDetailId = id;

            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_View")]
        public ActionResult _SelectOrderTracerList(GridCommand command, string orderDetailId)
        {
            IList<OrderTracer> orderTracerList = genericMgr.FindAll<OrderTracer>
                (" from OrderTracer where OrderDetailId=? ", orderDetailId);
            foreach (var orderTracer in orderTracerList)
            {
                orderTracer.ItemDescription = itemMgr.GetCacheItem(orderTracer.Item).Description;
            }
            return PartialView(new GridModel(orderTracerList));
        }
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_View")]
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

            //string whereStatement = " where o.OrderStrategy != " + (int)com.Sconit.CodeMaster.FlowStrategy.SEQ + " and o.Type in (" + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
            //            + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + "," + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ")"
            //            + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            string whereStatement = "and o.OrderStrategy != " + (int)com.Sconit.CodeMaster.FlowStrategy.SEQ;
            searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            ExportToXLS<OrderMaster>("ProcurementOrderMaster.xls", GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
        #region  Export  master  search about return
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_ReturnIndex")]
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
            string whereStatement = "and o.OrderStrategy != " + (int)com.Sconit.CodeMaster.FlowStrategy.SEQ;
            searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Return;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, true);
            ExportToXLS<OrderMaster>("ProcurementReturnOrderMaster.xls", GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
    }

}
