
namespace com.Sconit.Web.Controllers.SP
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.SCM;
    using System;
    using AutoMapper;
    using com.Sconit.Entity.MD;
    using NHibernate.Criterion;
    using com.Sconit.Entity.Exception;
    using System.Text;
    using com.Sconit.Entity.SYS;
    using System.ComponentModel;

    public class SupplierOrderController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from OrderMaster as o";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select o from OrderMaster as o";

        //private static string selectFlowDetailStatement = "select f from FlowDetail as f where f.Flow = ? order by f.Sequence asc,f.Id asc";

        private static string selectOrderDetailStatement = "select d from OrderDetail as d where d.OrderNo=? order by d.Sequence asc,d.Id asc";

        private static string selectFlowDetailStatement = "select f from FlowDetail as f where f.Flow = ? order by f.Sequence asc,f.Id asc";

        private static string selectOneFlowDetailStatement = "select d from FlowDetail as d where d.Flow = ? and d.Item = ?";


        private static string selecLocationStatement = "select l from Location as l where l.Region = ? and l.IsActive = ?";

        //public IGenericMgr genericMgr { get; set; }

        //public WCFPrintMgrImpl wcfPrintMgr { get; set; }

        //public IOrderMgr orderMgr { get; set; }

        //public IFlowMgr flowMgr { get; set; }

        //private WCFServices.IPublishing proxy;

        /// <summary>
        /// 
        /// </summary>
        public SupplierOrderController()
        {
        }

        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Query")]
        public ActionResult Index()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Detail_Query")]
        public ActionResult DetaiIndex()
        {
            return View();
        }

        public ActionResult ReturnDetailIndex()
        {
            return View();
        }

       
        #region edit
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Query")]
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
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Query")]
        public ActionResult _AjaxList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel);
            //SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Single_Return_Query")]
        public ActionResult ReturnIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Single_Return_Query")]
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
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Single_Return_Query")]
        public ActionResult _ReturnAjaxList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }
            //string whereStatement = " where o.Type in (" + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
            //            + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ")"
            //            + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Return;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareReturnSearchStatement_1(command, searchModel);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
        }

        private ProcedureSearchStatementModel PrepareReturnSearchStatement_1(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Format(" and o.Status<>'{0}' and o.SubType='{1}' ", (int)com.Sconit.CodeMaster.OrderStatus.Create, (int)com.Sconit.CodeMaster.OrderSubType.Return);

            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine,
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
            paraList.Add(new ProcedureParameter { Parameter = true, Type = NHibernate.NHibernateUtil.Boolean });
            paraList.Add(new ProcedureParameter { Parameter = true, Type = NHibernate.NHibernateUtil.Boolean });
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



        private SearchStatementModel PrepareSearchStatement(GridCommand command, OrderMasterSearchModel searchModel, string whereStatement)
        {

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "o", ref whereStatement, param);

            //SecurityHelper.AddPartyFromPermissionStatement(ref whereStatement, "o", "PartyFrom", com.Sconit.CodeMaster.OrderType.Procurement, true);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "o", "Type", "o", "PartyFrom", "o", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, true);

            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "o", ref whereStatement, param);

            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "o", ref whereStatement, param);

            HqlStatementHelper.AddLikeStatement("WMSNo", searchModel.WMSNO, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("CreateUserName", searchModel.CreateUserName, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Priority", searchModel.Priority, "o", ref whereStatement, param);


            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("StartTime", searchModel.DateFrom, searchModel.DateTo, "o", ref whereStatement, param);
            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                HqlStatementHelper.AddGeStatement("StartTime", searchModel.DateFrom, "o", ref whereStatement, param);
            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("StartTime", searchModel.DateTo, "o", ref whereStatement, param);
            }
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


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by o.CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Deliver")]
        public ActionResult New()
        {
            ViewBag.flow = null;
            ViewBag.orderNo = null;
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Deliver")]
        public string New(OrderMaster orderMaster, [Bind(Prefix =
             "inserted")]IEnumerable<OrderDetail> insertedOrderDetails, [Bind(Prefix =
             "updated")]IEnumerable<OrderDetail> updatedOrderDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(orderMaster.Flow))
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_Flow);
                }
                if (orderMaster.WindowTime == null)
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_WindowTime);
                }
                if (orderMaster.StartTime == null)
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_StartTime);
                }

                #region orderDetailList
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                if (insertedOrderDetails != null && insertedOrderDetails.Count() > 0)
                {
                    foreach (OrderDetail orderDetail in insertedOrderDetails)
                    {
                        OrderDetail newOrderDetail = RefreshOrderDetail(orderMaster.Flow, orderDetail);
                        orderDetailList.Add(newOrderDetail);
                    }
                }
                if (updatedOrderDetails != null && updatedOrderDetails.Count() > 0)
                {
                    foreach (OrderDetail orderDetail in updatedOrderDetails)
                    {

                        if (!string.IsNullOrEmpty(orderDetail.LocationTo))
                        {
                            orderDetail.LocationToName = genericMgr.FindById<Location>(orderDetail.LocationTo).Name;
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

                DateTime effectiveDate = orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value : DateTime.Now;
                OrderMaster newOrder = orderMgr.TransferFlow2Order(flow, null, effectiveDate, false);

                newOrder.WindowTime = orderMaster.WindowTime;
                newOrder.StartTime = orderMaster.StartTime;
                newOrder.ReferenceOrderNo = orderMaster.ReferenceOrderNo;
                newOrder.ExternalOrderNo = orderMaster.ExternalOrderNo;

                newOrder.OrderDetails = orderDetailList;

                orderMgr.CreateOrder(newOrder);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Added);
                return newOrder.OrderNo;
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return string.Empty;
            }
        }



        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_QuickNew")]
        public ActionResult QuickNew()
        {

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_QuickNew")]
        public string QuickNew(OrderMaster orderMaster, [Bind(Prefix =
             "inserted")]IEnumerable<OrderDetail> insertedOrderDetails, [Bind(Prefix =
             "updated")]IEnumerable<OrderDetail> updatedOrderDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(orderMaster.Flow))
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_Flow);
                }
                if (orderMaster.EffectiveDate == null)
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_EffectiveDate);
                }

                #region orderDetailList
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                if (insertedOrderDetails != null && insertedOrderDetails.Count() > 0)
                {
                    foreach (OrderDetail orderDetail in insertedOrderDetails)
                    {
                        OrderDetail newOrderDetail = RefreshOrderDetail(orderMaster.Flow, orderDetail);
                        orderDetailList.Add(newOrderDetail);
                    }
                }
                if (updatedOrderDetails != null && updatedOrderDetails.Count() > 0)
                {
                    foreach (OrderDetail orderDetail in updatedOrderDetails)
                    {

                        if (!string.IsNullOrEmpty(orderDetail.LocationTo))
                        {
                            orderDetail.LocationToName = genericMgr.FindById<Location>(orderDetail.LocationTo).Name;
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
                DateTime effectiveDate = orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value : DateTime.Now;
                OrderMaster newOrder = orderMgr.TransferFlow2Order(flow, null, effectiveDate, false);

                newOrder.ReferenceOrderNo = orderMaster.ReferenceOrderNo;
                newOrder.ExternalOrderNo = orderMaster.ExternalOrderNo;
                newOrder.IsQuick = true;
                newOrder.OrderDetails = orderDetailList;
                newOrder.WindowTime = DateTime.Now;
                newOrder.StartTime = DateTime.Now;

                orderMgr.CreateOrder(newOrder);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Added);
                return newOrder.OrderNo;
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return string.Empty;
            }
        }

        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Deliver")]
        public ActionResult Edit(string orderNo)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                return HttpNotFound();
            }
            else
            {
                return View("Edit", string.Empty, orderNo);
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Deliver")]
        public ActionResult _Edit(string orderNo)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                return HttpNotFound();
            }
            OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);
            ViewBag.flow = orderMaster.Flow;
            ViewBag.orderNo = orderMaster.OrderNo;
            ViewBag.isEditable = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create;
            ViewBag.editorTemplate = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create ? "" : "ReadonlyTextBox";

            ViewBag.showRelease = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create;
            ViewBag.showShip = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Submit || orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.InProcess;
            ViewBag.showReceive = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Submit || orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.InProcess;
            ViewBag.showComplete = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.InProcess;
            ViewBag.showCancel = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Submit;

            return PartialView(orderMaster);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Deliver")]
        public ActionResult _Edit(OrderMaster orderMaster)
        {
            orderMgr.UpdateOrder(orderMaster);
            SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Saved);
            return RedirectToAction("Edit/" + orderMaster.OrderNo);
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                orderMgr.DeleteOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Deleted);
                return RedirectToAction("List");
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Submit")]
        public ActionResult Submit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                #region 打印
                //proxy = wcfPrintMgr.CreateProxy();
                //CreateProxy();
                //WCFServices.Message message = new WCFServices.Message();
                //message.EventData = "xyz";
                //message.OrderNumber = "ABC";
                //proxy.Publish(message, "ABC");
                #endregion

                orderMgr.ReleaseOrder(id);

                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Submited, id);
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_Supplier_Start")]
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

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Complete")]
        public ActionResult Complete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                orderMgr.ManualCloseOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Completed);
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Cancel")]
        public ActionResult Cancel(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                orderMgr.CancelOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Canceled);
                return RedirectToAction("Edit/" + id);
            }
        }

        public ActionResult _OrderDetailList(string flow, string orderNo)
        {

            ViewBag.isManualCreateDetail = false;
            ViewBag.flow = flow;
            ViewBag.orderNo = orderNo;
            ViewBag.newOrEdit = "New";
            ViewBag.status = null;

            FlowMaster flowMaster = null;


            if (!string.IsNullOrEmpty(flow))
            {
                string flowcheckStr = "select f from FlowMaster as f where code = ? and type in (" + (int)com.Sconit.CodeMaster.OrderType.Procurement + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + "," + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + "," + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ") and f.IsActive = " + true;
                flowMaster = genericMgr.FindAll<FlowMaster>(flowcheckStr, flow).SingleOrDefault<FlowMaster>();
                if (flowMaster != null)
                {
                    ViewBag.isManualCreateDetail = flowMaster.IsManualCreateDetail;
                    ViewBag.status = com.Sconit.CodeMaster.OrderStatus.Create;
                }
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                ViewBag.status = genericMgr.FindById<OrderMaster>(orderNo).Status;
                ViewBag.newOrEdit = "Edit";
                ViewBag.isManualCreateDetail = ViewBag.status == com.Sconit.CodeMaster.OrderStatus.Create;
            }
            if (ViewBag.Status == com.Sconit.CodeMaster.OrderStatus.Create)
            {
                #region comboBox
                //IList<Item> items = genericMgr.FindAll<Item>(selecItemStatement, true);
                //ViewData.Add("items", items);
                IList<Uom> uoms = genericMgr.FindAll<Uom>();
                ViewData.Add("uoms", uoms);
                IList<Location> locationTos = new List<Location>();
                if (flowMaster.PartyTo != null)
                {
                    locationTos = genericMgr.FindAll<Location>(selecLocationStatement, new object[] { flowMaster.PartyTo, true });
                }
                ViewData.Add("locationTos", locationTos);
                #endregion
            }

            return PartialView();
        }

        [GridAction]
        public ActionResult _SelectBatchEditing(string orderNo, string flow)
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
                    string flowcheckStr = "select f from FlowMaster as f where code = ? and type in (" + (int)com.Sconit.CodeMaster.OrderType.Procurement + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + "," + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + "," + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + ") and f.IsActive = " + true;
                    FlowMaster flowMaster = genericMgr.FindAll<FlowMaster>(flowcheckStr, flow).SingleOrDefault<FlowMaster>();
                    if (flowMaster != null)
                    {
                        orderDetailList = TransformFlowDetailList2OrderDetailList(flow);
                    }
                }
            }
            return View(new GridModel(orderDetailList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Deliver")]
        public ActionResult _SaveBatchEditing([Bind(Prefix =
            "inserted")]IEnumerable<OrderDetail> insertedOrderDetails,
            [Bind(Prefix = "updated")]IEnumerable<OrderDetail> updatedOrderDetails,
            [Bind(Prefix = "deleted")]IEnumerable<OrderDetail> deletedOrderDetails,
            string newOrEdit, string flow, string orderNo)
        {
            if (newOrEdit == "Edit")
            {
                IList<OrderDetail> newOrderDetailList = new List<OrderDetail>();
                IList<OrderDetail> updateOrderDetailList = new List<OrderDetail>();
                if (insertedOrderDetails != null)
                {
                    foreach (var orderDetail in insertedOrderDetails)
                    {
                        OrderDetail newOrderDetail = RefreshOrderDetail(flow, orderDetail);
                        newOrderDetail.OrderNo = orderNo;
                        newOrderDetailList.Add(newOrderDetail);
                    }
                }
                if (updatedOrderDetails != null)
                {
                    //现在控件控制不住，改了Item也默认是之前的,加好的只能改数量
                    foreach (var orderDetail in updatedOrderDetails)
                    {
                        decimal qty = orderDetail.OrderedQty;
                        OrderDetail updateOrderDetail = genericMgr.FindById<OrderDetail>(orderDetail.Id);
                        updateOrderDetail.OrderedQty = qty;
                        updateOrderDetailList.Add(updateOrderDetail);
                    }
                }

                orderMgr.BatchUpdateOrderDetails(orderNo, newOrderDetailList, updateOrderDetailList, (IList<OrderDetail>)deletedOrderDetails);

                IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>(selectOrderDetailStatement, orderNo);
                return View(new GridModel(orderDetailList));
            }
            return View();
        }

        public ActionResult _WebOrderDetail(string flow, string itemCode)
        {
            if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(itemCode))
            {

                WebOrderDetail webOrderDetail = new WebOrderDetail();
                FlowDetail flowDetail = genericMgr.FindAll<FlowDetail>(selectOneFlowDetailStatement, new object[] { flow, itemCode }).SingleOrDefault<FlowDetail>();

                if (flowDetail != null)
                {
                    webOrderDetail.Item = flowDetail.Item;
                    webOrderDetail.ItemDescription = genericMgr.FindById<Item>(flowDetail.Item).Description;
                    webOrderDetail.UnitCount = flowDetail.UnitCount;
                    webOrderDetail.Uom = flowDetail.Uom;
                    webOrderDetail.Sequence = flowDetail.Sequence;
                    //默认库位
                    webOrderDetail.LocationTo = flowDetail.LocationTo;
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
                    }
                }
                return this.Json(webOrderDetail);
            }
            return null;
        }


        public String _WindowTime(string flow, string windowTime)
        {
            return DateTime.Parse(windowTime).ToString();
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
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = PrepareReceiveSearchStatement(command, (OrderMasterSearchModel)searchCacheModel.SearchObject);
            return View(GetPageData<OrderMaster>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Receive")]
        public ActionResult _AjaxReceiveOrderList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareReceiveSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
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
            string[] checkedOrderArray = checkedOrders.Split(',');
            OrderMaster order = genericMgr.FindById<OrderMaster>(checkedOrderArray[0]);
            return View(order);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_Receive")]
        public ActionResult ReceiveOrder(int[] id, decimal[] currentReceiveQty)
        {
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();
            for (int i = 0; i < currentReceiveQty.Count(); i++)
            {
                if (currentReceiveQty[i] > 0)
                {
                    OrderDetail od = genericMgr.FindById<OrderDetail>(id[i]);

                    OrderDetailInput input = new OrderDetailInput();
                    input.ReceiveQty = currentReceiveQty[i];
                    od.AddOrderDetailInput(input);
                    orderDetailList.Add(od);
                }
            }

            if (orderDetailList.Count() == 0)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    orderMgr.ReceiveOrder(orderDetailList);
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Received);
                    return RedirectToAction("ReceiveIndex");
                }
                catch (BusinessException ex)
                {
                    SaveBusinessExceptionMessage(ex);
                    return View();
                }
            }
        }

        #endregion

        #region batch
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
        public ActionResult BatchProcessIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
        public ActionResult BatchProcessList()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
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
                        SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Submited, orderNo);
                    }
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessIndex");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
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
                        SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Deleted, orderNo);
                    }
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessIndex");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
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
                        SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Canceled, orderNo);
                    }
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessIndex");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
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
                        SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Completed, orderNo);
                    }
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }

            return RedirectToAction("BatchProcessIndex");
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

            return RedirectToAction("BatchProcessIndex");
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

            return RedirectToAction("BatchProcessIndex");
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Procurement_BatchProcess")]
        public ActionResult _AjaxBatchProcessList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
        }
        #endregion

        #region private method
        private IList<OrderDetail> TransformFlowDetailList2OrderDetailList(string flow)
        {
            IList<FlowDetail> flowDetailList = flowMgr.GetFlowDetailList(flow);
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();
            foreach (FlowDetail flowDetail in flowDetailList)
            {
                OrderDetail orderDetail = new OrderDetail();
                Mapper.Map<FlowDetail, OrderDetail>(flowDetail, orderDetail);
                orderDetail.Id = flowDetail.Id;
                orderDetail.ItemDescription = genericMgr.FindById<Item>(flowDetail.Item).Description;
                orderDetailList.Add(orderDetail);
            }
            return orderDetailList;
        }
        private ProcedureSearchStatementModel PrepareSearchStatement_1(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Format(" and o.Status<>'{0}' and o.SubType='{1}' ", (int)com.Sconit.CodeMaster.OrderStatus.Create, (int)com.Sconit.CodeMaster.OrderSubType.Normal);

            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract,
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
            paraList.Add(new ProcedureParameter { Parameter = true, Type = NHibernate.NHibernateUtil.Boolean });
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




        private SearchStatementModel PrepareSearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = " where o.Type in (" + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + ")"
                                    + "and o.Status<>" + (int)com.Sconit.CodeMaster.OrderStatus.Create
                                    + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("WMSNo", searchModel.WMSNO, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ReferenceOrderNo", searchModel.ReferenceOrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ExternalOrderNo", searchModel.ExternalOrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "o", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Dock", searchModel.Dock, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Priority", searchModel.Priority, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Sequence", searchModel.Sequence, "o", ref whereStatement, param);

            //SecurityHelper.AddPartyFromPermissionStatement(ref whereStatement, "o", "PartyFrom", com.Sconit.CodeMaster.OrderType.Procurement, true);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "o", "Type", "o", "PartyFrom", "o", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, true);

            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("StartTime", searchModel.DateFrom, searchModel.DateTo, "o", ref whereStatement, param);
            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                HqlStatementHelper.AddGeStatement("StartTime", searchModel.DateFrom, "o", ref whereStatement, param);
            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("StartTime", searchModel.DateTo, "o", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;

            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel PrepareReceiveSearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {

            string whereStatement = " where o.Type in (" + (int)com.Sconit.CodeMaster.OrderType.Procurement + "," + (int)com.Sconit.CodeMaster.OrderType.Transfer + "," + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + "," + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ")"
                                    + " and o.IsReceiveScanHu = 0 and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + "," + (int)com.Sconit.CodeMaster.OrderStatus.Submit + ")"
                                    + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal
                                    + " and exists (select 1 from OrderDetail as d where d.ReceivedQty < d.OrderedQty and d.OrderNo = o.OrderNo) ";
            //SecurityHelper.AddPartyFromPermissionStatement(ref whereStatement, "o", "PartyFrom", com.Sconit.CodeMaster.OrderType.Procurement, true);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "o", "Type", "o", "PartyFrom", "o", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, true);

            IList<object> param = new List<object>();

            if (!string.IsNullOrEmpty(searchModel.OrderNo))
            {
                HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            }
            else if (!string.IsNullOrEmpty(searchModel.Flow))
            {
                HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "o", ref whereStatement, param);
            }
            else if (!string.IsNullOrEmpty(searchModel.PartyFrom) && !string.IsNullOrEmpty(searchModel.PartyTo))
            {
                HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "o", ref whereStatement, param);
                HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "o", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private OrderDetail RefreshOrderDetail(string flow, OrderDetail orderDetail)
        {

            OrderDetail newOrderDetail = new OrderDetail();
            IList<FlowDetail> flowDetailList = genericMgr.FindAll<FlowDetail>(selectFlowDetailStatement, flow);
            FlowDetail flowDetail = flowDetailList.Where<FlowDetail>(q => q.Item == orderDetail.Item).SingleOrDefault();
            if (flowDetail != null)
            {
                Mapper.Map<FlowDetail, OrderDetail>(flowDetail, newOrderDetail);
                newOrderDetail.Sequence = orderDetail.Sequence == 0 ? newOrderDetail.Sequence : orderDetail.Sequence;
                newOrderDetail.UnitCount = orderDetail.UnitCount == 0 ? orderDetail.UnitCount : orderDetail.UnitCount;
                newOrderDetail.Uom = String.IsNullOrWhiteSpace(orderDetail.Uom) ? orderDetail.Uom : orderDetail.Uom;
                newOrderDetail.ItemDescription = genericMgr.FindById<Item>(orderDetail.Item).Description;

            }
            else
            {
                Item item = genericMgr.FindById<Item>(orderDetail.Item);
                if (item != null)
                {
                    newOrderDetail.Item = item.Code;
                    newOrderDetail.UnitCount = newOrderDetail.UnitCount == 0 ? item.UnitCount : orderDetail.UnitCount;
                    newOrderDetail.Uom = String.IsNullOrWhiteSpace(newOrderDetail.Uom) ? item.Uom : orderDetail.Uom;
                    newOrderDetail.ItemDescription = item.Description;
                    newOrderDetail.Sequence = orderDetail.Sequence;
                }
            }
            newOrderDetail.OrderedQty = orderDetail.OrderedQty;
            if (!string.IsNullOrEmpty(orderDetail.LocationTo))
            {
                newOrderDetail.LocationTo = orderDetail.LocationTo;
                newOrderDetail.LocationToName = genericMgr.FindById<Location>(orderDetail.LocationTo).Name;
            }
            return newOrderDetail;
        }
        #endregion

        #region detail
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Single_Return_Detail_Query")]
        public ActionResult ReturnDetailList(GridCommand command, OrderMasterSearchModel searchModel)
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
        public ActionResult _AjaxReturnOrderDetList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
            string whereStatement = "and exists(select 1 from OrderMaster  as o where o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Return + " and o.OrderNo=d.OrderNo)";
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command));
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Detail_Query")]
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
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Detail_Query")]
        public ActionResult _AjaxOrderDetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchDetailStatement(command, searchModel);
            return PartialView(GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command));
        }

        private ProcedureSearchStatementModel PrepareSearchDetailStatement(GridCommand command, OrderMasterSearchModel searchModel, string whereStatement)
        {

            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter
            {
                Parameter = +(int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract,
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
            paraList.Add(new ProcedureParameter { Parameter = true, Type = NHibernate.NHibernateUtil.Boolean });
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


        private ProcedureSearchStatementModel PrepareSearchDetailStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Format(" and exists (select 1 from OrderMaster  as o where  o.SubType ={0} and o.OrderNo=d.OrderNo ) ",
                (int)com.Sconit.CodeMaster.OrderSubType.Normal);

            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
                    + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract,
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
            paraList.Add(new ProcedureParameter { Parameter = true, Type = NHibernate.NHibernateUtil.Boolean });
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
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_Supplier_Lading_Query")]
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
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel);
            //SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            ExportToXLS<OrderMaster>("SupplierOrderMaster.xls", GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
        #region  Export master search about return
        [SconitAuthorize(Permissions = "Url_Single_Return_Query")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportReturnMstr(OrderMasterSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return ;
            }
            //string whereStatement = " where o.Type in (" + (int)com.Sconit.CodeMaster.OrderType.CustomerGoods + "," + (int)com.Sconit.CodeMaster.OrderType.Procurement
            //            + "," + (int)com.Sconit.CodeMaster.OrderType.SubContract + "," + (int)com.Sconit.CodeMaster.OrderType.ScheduleLine + ")"
            //            + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Return;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareReturnSearchStatement_1(command, searchModel);
            ExportToXLS<OrderMaster>("SupplierReturnOrderMaster.xls", GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command).Data.ToList());
        }
        #endregion
    }
}
