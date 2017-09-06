﻿namespace com.Sconit.Web.Controllers.ORD
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using AutoMapper;
    using com.Sconit.Entity;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.MRP.MD;
    using com.Sconit.Entity.MRP.TRANS;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity.PRD;
    using com.Sconit.Entity.SCM;
    using com.Sconit.Entity.SYS;
    using com.Sconit.PrintModel.INV;
    using com.Sconit.PrintModel.ORD;
    using com.Sconit.Service;
    using com.Sconit.Service.MRP;
    using com.Sconit.Utility;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using com.Sconit.Web.Models.SearchModels.PRD;
    using Telerik.Web.Mvc;

    public class ProductionOrderController : WebAppBaseController
    {
        #region  hql
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from OrderMaster as o";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select o from OrderMaster as o";


        private static string selectOrderDetailStatement = "select d from OrderDetail as d where d.OrderNo=? order by d.Sequence asc,d.Id asc";


        private static string selectOrderOperationStatement = "select d from OrderOperation as d where d.OrderDetailId = ?";

        #endregion

        //private WCFServices.IPublishing proxy;
        //public IGenericMgr genericMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        //public IFlowMgr flowMgr { get; set; }
        //public IReportGen reportGen { get; set; }
        //public IItemMgr itemMgr { get; set; }
        public IProductionLineMgr productionLineMgr { get; set; }
        public IBomMgr bomMgr { get; set; }
        public IMrpOrderMgr mrpOrderMgr { get; set; }


        #region public method
        public ProductionOrderController()
        {
        }

        #region view
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View")]
        public ActionResult Index()
        {
            OrderMasterSearchModel searchModel = new OrderMasterSearchModel();
            searchModel.DateType = "0";
            searchModel.DateFrom = DateTime.Today.AddDays(-7);
            searchModel.DateTo = DateTime.Today;
            searchModel.MultiStatus = "0,1,2";
            TempData["OrderMasterSearchModel"] = searchModel;
            ViewBag.GetType = "View";
            return View();
        }


        [SconitAuthorize(Permissions = "Url_OrderDetail_Production")]
        public ActionResult DetailIndex()
        {
            TempData["OrderMasterSearchModel"] = null;
            return View();
        }
        #region
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedOrderMaster")]
        public ActionResult _SearchResult(GridCommand command, OrderMasterSearchModel searchModel)
        {
            ViewBag.OrderNo = searchModel.OrderNo;
            ViewBag.GetType = "Ajax";
            return PartialView();
        }
        #endregion
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View")]
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
            ViewBag.GetType = "View";
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View")]
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
            string whereStatement = string.Empty;
            if (searchModel.SearchForceMaterialOrder == "true")
            {
                whereStatement = " AND o.ProdLineFact ='EXV' AND o.SubType!=40 ";
            }
            //searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            var orderMasterList = GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command);
            foreach (var listData in orderMasterList.Data)
            {
                if (!string.IsNullOrWhiteSpace(listData.Shift))
                {
                    listData.Shift = genericMgr.FindById<ShiftMaster>(listData.Shift).Name;
                }
            }
            return PartialView(orderMasterList);
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
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)com.Sconit.CodeMaster.OrderType.Production,
                Type = NHibernate.NHibernateUtil.String
            });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.SubType, Type = NHibernate.NHibernateUtil.Int16 });
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

        #endregion

        #region edit
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Edit")]
        public ActionResult New()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive")]
        public JsonResult ReceiveOrder(string idStr, string qtyStr, string sQtyStr)
        {
            try
            {
                if (string.IsNullOrEmpty(idStr))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_DetailCanNotBeEmpty);
                }
                string[] idArr = idStr.Split(',');
                string[] qtyArr = qtyStr.Split(',');
                string[] sQtyArr = sQtyStr.Split(',');

                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    OrderDetail od = genericMgr.FindById<OrderDetail>(Convert.ToInt32(idArr[i]));
                    OrderDetailInput input = new OrderDetailInput();
                    input.ReceiveQty = Convert.ToDecimal(qtyArr[i]);
                    input.ScrapQty = Convert.ToDecimal(sQtyArr[i]);
                    od.AddOrderDetailInput(input);
                    orderDetailList.Add(od);
                }
                string printUrl = "";
                ReceiptMaster receiptMaster = orderMgr.ReceiveOrder(orderDetailList);
                if (receiptMaster.CreateHuOption == Sconit.CodeMaster.CreateHuOption.Receive)
                {
                    //打印
                    IList<Hu> huList = genericMgr.FindAll<Hu>("from Hu as h where h.ReceiptNo = ?", receiptMaster.ReceiptNo);
                    string orderNo = orderDetailList.Select(i => i.OrderNo).Distinct().Single();
                    string huTemplate = this.genericMgr.FindAll<string>("select HuTemplate from OrderMaster where OrderNo = ?", orderNo).Single();
                    if (string.IsNullOrWhiteSpace(huTemplate))
                    {
                        huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                    }
                    printUrl = PrintHuList(huList, huTemplate);
                }
                object obj = new { SuccessMessage = string.Format(Resources.ORD.OrderMaster.OrderMaster_Received, orderDetailList[0].OrderNo), SuccessData = orderDetailList[0].OrderNo, PrintUrl = printUrl };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Edit,Url_OrderMstr_Production_QuickNew")]
        public JsonResult NewCreateOrder(OrderMaster orderMaster, string[] Sequences, string[] Items, string[] UnitCounts, string[] Uoms,
             string[] LocationFroms, string[] LocationTos, string[] OrderedQtys, string[] HuTos, string[] Remarks)
        {
            try
            {
                #region 校验
                if (orderMaster.StartTime > orderMaster.WindowTime)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_StartToTimeCanNotGreaterWindowTime);
                }
                if (Items == null || Items.Length == 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_LackDetailCanNotCreateOrder);
                    return Json(null);
                }
                if (string.IsNullOrEmpty(orderMaster.Flow))
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_Flow);
                }
                //if (!orderMaster.IsQuick && orderMaster.WindowTime == DateTime.MinValue)
                //{
                //    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_WindowTime);
                //}
                //if (!orderMaster.IsQuick && orderMaster.StartTime == DateTime.MinValue)
                //{
                //    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_StartTime);
                //}
                #endregion
                #region Set windowtime and endtime base on manufacturedate
                ShiftDetail shiftDet = genericMgr.FindAll<ShiftDetail>("from ShiftDetail As s where s.Shift=? ", orderMaster.Shift).FirstOrDefault();
                string[] strArray = shiftDet.ShiftTime.Split('-');
                if (long.Parse(strArray[0].Replace(":", "")) > long.Parse(strArray[1].Replace(":", "")))
                {
                    orderMaster.WindowTime = DateTime.Parse(orderMaster.EffectiveDate.Value.AddDays(1).ToString("yyyy-MM-dd ") + strArray[1]);
                }
                else
                {
                    orderMaster.WindowTime = DateTime.Parse(orderMaster.EffectiveDate.Value.ToString("yyyy-MM-dd ") + strArray[1]);
                }
                orderMaster.StartTime = DateTime.Parse(orderMaster.EffectiveDate.Value.ToString("yyyy-MM-dd ") + strArray[0]);
                #endregion
                FlowMaster flow = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);
                DateTime effectiveDate = orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value.Date : DateTime.Now.Date;
                OrderMaster newOrder = orderMgr.TransferFlow2Order(flow, null, effectiveDate, false);
                newOrder.WindowTime = orderMaster.WindowTime;
                newOrder.StartTime = orderMaster.StartTime;
                newOrder.EffectiveDate = orderMaster.EffectiveDate;
                newOrder.ReferenceOrderNo = orderMaster.ReferenceOrderNo;
                newOrder.ExternalOrderNo = orderMaster.ExternalOrderNo;
                newOrder.Dock = orderMaster.Dock;
                newOrder.IsIndepentDemand = orderMaster.IsIndepentDemand;
                newOrder.Shift = orderMaster.Shift;
                newOrder.SubType = orderMaster.SubType;

                if (orderMaster.IsQuick)
                {
                    newOrder.WindowTime = DateTime.Now;
                    newOrder.StartTime = DateTime.Now;
                    newOrder.CreateHuOption = Sconit.CodeMaster.CreateHuOption.None;
                    newOrder.IsReceiveScanHu = false;
                    newOrder.IsShipScanHu = false;
                    newOrder.IsQuick = true;
                }

                #region OrderDetailList
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                var flowDetailList = flowMgr.GetFlowDetailList(orderMaster.Flow, false, true).ToDictionary(d => d.Item, d => d);
                bool isHasFlowDet = flowDetailList.Count > 0 ? true : false;
                for (int i = 0; i < Items.Length; i++)
                {
                    if (Convert.ToDecimal(OrderedQtys[i]) != 0)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.Sequence = Convert.ToInt32(Sequences[i]);
                        orderDetail.Item = Items[i];
                        orderDetail.UnitCount = Convert.ToDecimal(UnitCounts[i]);
                        orderDetail.Uom = Uoms[i];
                        orderDetail.LocationFrom = LocationFroms[i];
                        if (LocationTos.Length > 0)
                        {
                            orderDetail.LocationTo = LocationTos[i] == string.Empty ? null : LocationTos[i];
                        }
                        orderDetail.Direction = HuTos[i];
                        orderDetail.Remark = Remarks[i];
                        orderDetail.OrderedQty = Convert.ToDecimal(OrderedQtys[i]);
                        if (isHasFlowDet)
                        {
                            orderDetail.IsChangeUnitCount = flowDetailList.ValueOrDefault(orderDetail.Item).IsChangeUnitCount;
                            orderDetail.IsInspect = flow.IsInspect && flowDetailList.ValueOrDefault(orderDetail.Item).IsInspect;
                        }
                        else
                        {
                            orderDetail.IsChangeUnitCount = true;
                            orderDetail.IsInspect = false;
                        }
                            orderDetailList.Add(orderDetail);
                    }
                }

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    orderDetail.ItemDescription = genericMgr.FindById<Item>(orderDetail.Item).Description;
                    if (!string.IsNullOrEmpty(orderDetail.LocationFrom))
                    {
                        orderDetail.LocationFromName = genericMgr.FindById<Location>(orderDetail.LocationFrom).Name;
                    }
                    if (!string.IsNullOrEmpty(orderDetail.LocationTo))
                    {
                        orderDetail.LocationToName = genericMgr.FindById<Location>(orderDetail.LocationTo).Name;
                    }
                    orderDetail.Bom = bomMgr.FindItemBom(orderDetail.Item);
                }
                #endregion

                if (orderDetailList.Count == 0)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_OrderDetailIsEmpty);
                }

                if (!newOrder.IsQuick && orderDetailList.Where(p => p.OrderedQty < 0).Count() > 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_OrderCountCanNotLessThanZero);
                }

                newOrder.OrderDetails = orderDetailList;
                orderMgr.CreateOrder(newOrder);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Added, newOrder.OrderNo);
                return Json(new { OrderNo = newOrder.OrderNo });
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_QuickNew")]
        public ActionResult QuickNew()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_QuickNew")]
        public JsonResult QuickNew(OrderMaster orderMaster, [Bind(Prefix = "updated")]IEnumerable<OrderDetail> updatedOrderDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(orderMaster.Flow))
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_Flow);
                }
                if (orderMaster.EffectiveDate == null)
                {
                    //throw new BusinessException(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.ORD.OrderMaster.OrderMaster_EffectiveDate);
                }

                #region orderDetailList
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();

                if (updatedOrderDetails != null && updatedOrderDetails.Count() > 0)
                {
                    foreach (OrderDetail orderDetail in updatedOrderDetails)
                    {
                        if (orderDetail.OrderedQty != 0)
                        {
                            orderDetail.ItemDescription = genericMgr.FindById<Item>(orderDetail.Item).Description;
                            if (!string.IsNullOrEmpty(orderDetail.LocationFrom))
                            {
                                orderDetail.LocationFromName = genericMgr.FindById<Location>(orderDetail.LocationFrom).Name;
                            }
                            if (!string.IsNullOrEmpty(orderDetail.LocationTo))
                            {
                                orderDetail.LocationToName = genericMgr.FindById<Location>(orderDetail.LocationTo).Name;
                            }
                            orderDetail.Bom = bomMgr.FindItemBom(orderDetail.Item);
                            orderDetailList.Add(orderDetail);
                        }
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
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Added, newOrder.OrderNo);
                return Json(newOrder.OrderNo);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View")]
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
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View")]
        public ActionResult _Edit(string orderNo)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                return HttpNotFound();
            }
            OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);
            ViewBag.flow = orderMaster.Flow;
            ViewBag.orderNo = orderMaster.OrderNo;
            ViewBag.Status = orderMaster.Status;
            ViewBag.editorTemplate = orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create && this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Production_Edit") ? "" : "ReadonlyTextBox";
            return PartialView(orderMaster);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Edit")]
        public JsonResult _Edit(OrderMaster orderMaster)
        {
            try
            {
                if (orderMaster.StartTime > orderMaster.WindowTime)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_StartToTimeCanNotGreaterWindowTime);
                }
                OrderMaster newOrder = genericMgr.FindById<OrderMaster>(orderMaster.OrderNo);
                newOrder.WindowTime = orderMaster.WindowTime;
                newOrder.StartTime = orderMaster.StartTime;
                newOrder.Shift = orderMaster.Shift;
                newOrder.Dock = orderMaster.Dock;
                orderMgr.UpdateOrder(newOrder);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Saved, orderMaster.OrderNo);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(new { OrderNo = orderMaster.OrderNo });
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Edit")]
        public JsonResult _SaveBatchEditing(
            [Bind(Prefix = "updated")]IEnumerable<OrderDetail> updatedOrderDetails,
            [Bind(Prefix = "deleted")]IEnumerable<OrderDetail> deletedOrderDetails,
             string orderNo)
        {

            //orderMgr.BatchUpdateOrderDetails(orderNo, null, (IList<OrderDetail>)updatedOrderDetails, (IList<OrderDetail>)deletedOrderDetails);
            //IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>(selectOrderDetailStatement, orderNo);
            //return View(new GridModel(orderDetailList));
            try
            {
                orderMgr.BatchUpdateOrderDetails(orderNo, null, (IList<OrderDetail>)updatedOrderDetails, (IList<OrderDetail>)deletedOrderDetails);
                object obj = new { SuccessMessage = string.Format(Resources.INV.StockTake.StockTakeDetail_Saved, orderNo) };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }

        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Delete")]
        public ActionResult Delete(string id)
        {
            try
            {
                orderMgr.DeleteOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Deleted, id);
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return RedirectToAction("Edit", new { orderNo = id });
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Submit")]
        public ActionResult Submit(string id)
        {
            try
            {
                orderMgr.ReleaseOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Submited, id);

            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Submit")]
        public JsonResult StartOrder(string orderNo)
        {
            try
            {
                this.orderMgr.StartOrder(orderNo);
                var orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);
                object obj = new
                {
                    SuccessMessage = string.Format(Resources.ORD.OrderMaster.OrderMaster_Started, orderNo),
                    Flow = orderMaster.Flow
                };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Start")]
        public ActionResult Start(string id)
        {
            try
            {
                if (IsVanOrder(id))
                {
                    orderMgr.StartVanOrder(id);
                }
                else
                {
                    orderMgr.StartOrder(id);
                }

                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Started, id);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive")]
        public ActionResult VanReceive(string id)
        {
            try
            {
                orderMgr.ReceiveVanOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Received, id);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }


        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Close")]
        public ActionResult Close(string id)
        {
            try
            {
                orderMgr.ManualCloseOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Closed, id);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Cancel")]
        public ActionResult Cancel(string id)
        {
            try
            {
                orderMgr.CancelOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Canceled, id);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }


        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Check")]
        public ActionResult Check(string id)
        {
            try
            {
                orderMgr.CheckOrder(id);
                SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_Checked, id);
            }
            catch (BusinessException ex)
            {
                foreach (Message message in ex.GetMessages())
                {
                    SaveErrorMessage(message.GetMessageString());
                }
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_CreateRequisitionList")]
        public ActionResult CreateRequisitionList(string id)
        {
            try
            {
                string[] objectString = orderMgr.CreateRequisitionList(id);
                if (!string.IsNullOrEmpty(objectString[0]))
                {
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_RequisitionList_Created, objectString[0]);
                }
                if (!string.IsNullOrEmpty(objectString[1]))
                {
                    SaveWarningMessage(Resources.ORD.OrderMaster.OrderMaster_RequisitionList_ItemNotCreated, objectString[1]);
                }
                if (string.IsNullOrEmpty(objectString[0]) && string.IsNullOrEmpty(objectString[1]))
                {
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_RequisitionList_NoOrderCreate);
                }
            }
            catch (BusinessException ex)
            {
                foreach (Message message in ex.GetMessages())
                {
                    SaveErrorMessage(message.GetMessageString());
                }
            }
            return RedirectToAction("Edit", new { orderNo = id });
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_CreateRequisitionList")]
        public ActionResult _RequisitionDetailWindow(string orderNo)
        {

            ViewBag.MasterorderNo = orderNo;

            return PartialView();

        }



        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_CreateRequisitionList")]
        public ActionResult _RequisitionDetailList(string MasterorderNo, string Item, string LocationTo, string LocationFrom, string DetailOrderNo)
        {
            ViewBag.Item = Item;
            ViewBag.MasterorderNo = MasterorderNo;
            ViewBag.DetailOrderNo = DetailOrderNo;
            ViewBag.LocationTo = LocationTo;
            ViewBag.LocationTo = LocationFrom;
            return PartialView();

        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_KBOrderBomDetailList")]
        public ActionResult _KBOrderBomDetWindow(string orderNo)
        {
            IList<OrderDetail> list = genericMgr.FindAll<OrderDetail>(" from OrderDetail o where o.OrderNo=?", orderNo);
            ViewBag.OrderDetailid = string.Empty;
            foreach (OrderDetail od in list)
            {
                if (ViewBag.OrderDetailid == string.Empty)
                {
                    ViewBag.OrderDetailid = od.Id.ToString();
                }
                else
                {
                    ViewBag.OrderDetailid += "," + od.Id.ToString();
                }
            }
            ViewBag.MasterorderNo = orderNo;

            return PartialView();

        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_KBOrderBomDetailList")]
        public ActionResult _KBOrderBomDetList(string MasterorderNo, string Item, string Flow, string StartTime, string OrderNo, string EndTime, string OrderDetailid)
        {
            ViewBag.Item = Item;
            ViewBag.MasterorderNo = MasterorderNo;
            ViewBag.OrderNo = OrderNo;
            ViewBag.Flow = Flow;
            ViewBag.StartTime = StartTime;
            ViewBag.EndTime = EndTime;
            ViewBag.OrderDetailid = OrderDetailid;

            return PartialView();

        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_KBOrderBomDetailList")]
        public ActionResult _SelectKBOrderBomDetail(string MasterorderNo, string Item, string Flow, string StartTime, string OrderNo, string EndTime, string OrderDetailid)
        {
            string selectStatement = string.Empty;

            IList<object> param = new List<object>();

            string[] strArray = OrderDetailid.Split(',');


            foreach (var para in strArray)
            {
                if (selectStatement == string.Empty)
                {
                    selectStatement = @"select od.*,kb.Flow from ORD_OrderBomDet as od  
                           inner join  ORD_KBOrderBomDet kb
                           on od.OrderDetId= kb.OrderBomDetId 
                           where  od.OrderDetId in (?";
                }
                else
                {
                    selectStatement += ",?";
                }
                param.Add(para);
            }
            selectStatement += ")";
            HqlStatementHelper.AddEqStatement("Item", Item, "od", ref selectStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", Flow, "kb", ref selectStatement, param);
            if (!string.IsNullOrEmpty(StartTime) & !string.IsNullOrEmpty(EndTime))
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", StartTime, EndTime, "kb", ref selectStatement, param);
            }
            else if (!string.IsNullOrEmpty(StartTime) & string.IsNullOrEmpty(EndTime))
            {
                HqlStatementHelper.AddGeStatement("CreateDate", StartTime, "kb", ref selectStatement, param);
            }
            else if (string.IsNullOrEmpty(StartTime) & !string.IsNullOrEmpty(EndTime))
            {
                HqlStatementHelper.AddLeStatement("CreateDate", EndTime, "kb", ref selectStatement, param);
            }
            IList<OrderBomDetail> orderDetailList = genericMgr.FindAllWithNativeSql<OrderBomDetail>(selectStatement, param.ToArray());
            return PartialView(new GridModel(orderDetailList));
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_CreateRequisitionList")]
        public ActionResult _SelectRequisitionDetail(string MasterorderNo, string Item, string LocationTo, string LocationFrom, string DetailOrderNo)
        {
            string whereStatement = @"select od from OrderDetail as od
                where exists (select 1 from OrderMaster as om where  om.OrderNo= od.OrderNo and 
                om.ReferenceOrderNo='" + MasterorderNo + "')";
            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("OrderNo", DetailOrderNo, "od", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", Item, "od", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LocationTo", LocationTo, "od", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LocationFrom", LocationFrom, "od", ref whereStatement, param);

            whereStatement += "order by od.OrderNo desc ";

            IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>(whereStatement, param.ToArray());
            return PartialView(new GridModel(orderDetailList));
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View")]
        public ActionResult _OrderDetailList(string flow, string orderNo)
        {
            ViewBag.Visible = false;
            ViewBag.flow = flow;
            ViewBag.orderNo = orderNo;
            ViewBag.Status = null;
            FlowMaster flowMaster = null;
            if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(orderNo))
            {
                ViewBag.Visible = true;
            }

            if (!string.IsNullOrEmpty(orderNo))
            {
                OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
                ViewBag.Status = orderMaster.Status;
                ViewBag.Region = orderMaster.PartyFrom;
                ViewBag.IsManualCreateDetail = orderMaster.IsManualCreateDetail;
                ViewBag.IsReceiveScanHu = orderMaster.IsReceiveScanHu;
            }

            else if (!string.IsNullOrEmpty(flow))
            {
                flowMaster = genericMgr.FindById<FlowMaster>(flow);

                ViewBag.Status = com.Sconit.CodeMaster.OrderStatus.Create;
                ViewBag.Region = flowMaster.PartyFrom;
                ViewBag.IsManualCreateDetail = flowMaster.IsManualCreateDetail;
            }
            ViewBag.isEditable = ViewBag.Status == com.Sconit.CodeMaster.OrderStatus.Create && this.CurrentUser.UrlPermissions.Contains("Url_OrderMstr_Production_Edit");
            if (ViewBag.Status == com.Sconit.CodeMaster.OrderStatus.Create && ViewBag.IsManualCreateDetail)
            {
                #region combox
                IList<Uom> uoms = genericMgr.FindAll<Uom>();
                ViewData.Add("uoms", uoms);
                //IList<BomMaster> boms = genericMgr.FindAll<BomMaster>();
                //ViewData.Add("boms", boms);
                //IList<RoutingMaster> routings = genericMgr.FindAll<RoutingMaster>();
                //ViewData.Add("routings", routings);
                //IList<Location> locationFroms = new List<Location>();
                //IList<Location> locationTos = new List<Location>();
                #endregion
            }
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View")]
        public ActionResult _SelectBatchEditing(string orderNo, string flow)
        {
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();

            if (!string.IsNullOrEmpty(flow) || !string.IsNullOrEmpty(orderNo))
            {
                if (!string.IsNullOrEmpty(orderNo))
                {
                    orderDetailList = genericMgr.FindAll<OrderDetail>(selectOrderDetailStatement, orderNo);
                    foreach (var orderDetail in orderDetailList)
                    {
                        //orderDetail.CurrentReceiveQty = orderDetail.RemainReceivedQty;
                    }
                }
                else
                {
                    FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(flow);
                    if (flowMaster.IsListDet)
                    {
                        orderDetailList = orderMgr.TransformFlowMster2OrderDetailList(flowMaster, Sconit.CodeMaster.OrderSubType.Normal);
                    }
                }
            }
            return PartialView(new GridModel(orderDetailList));
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
                return string.Empty;
            }
        }


        [HttpGet]
        public ActionResult _OrderBomDetail(string id, string orderStatus, string region)
        {
            com.Sconit.CodeMaster.OrderStatus orderStatusEnum = (com.Sconit.CodeMaster.OrderStatus)Enum.Parse(typeof(com.Sconit.CodeMaster.OrderStatus), orderStatus, true);
            ViewBag.OrderDetailId = Convert.ToInt32(id);
            ViewBag.OrderStatus = (int)orderStatusEnum;
            ViewBag.Region = region;
            return PartialView();
        }

        public ActionResult _OrderBomDetailList(GridCommand command, OrderBomDetailSearchModel searchModel)
        {
            ViewBag.OrderDetailId = searchModel.OrderDetailId;
            ViewBag.OrderStatus = searchModel.OrderStatus;
            ViewBag.OrderedQty1 = genericMgr.FindById<OrderDetail>(searchModel.OrderDetailId).OrderedQty;
            ViewBag.ReadOnly = searchModel.OrderStatus == (int)com.Sconit.CodeMaster.OrderStatus.Create;
            IList<CodeDetail> backFlushMethodList = systemMgr.GetCodeDetails(Sconit.CodeMaster.CodeMaster.BackFlushMethod);
            IList<CodeDetail> feedMethodList = systemMgr.GetCodeDetails(Sconit.CodeMaster.CodeMaster.FeedMethod);

            ViewData["uoms"] = genericMgr.FindAll<Uom>();
            ViewData["BackFlushMethod"] = base.Transfer2DropDownList(com.Sconit.CodeMaster.CodeMaster.BackFlushMethod, backFlushMethodList);
            ViewData["FeedMethod"] = base.Transfer2DropDownList(com.Sconit.CodeMaster.CodeMaster.FeedMethod, feedMethodList);
            TempData["OrderBomDetailSearchModel"] = searchModel;
            return PartialView();
        }

        [GridAction]
        public ActionResult _SelectBomDetailBatchEditing(GridCommand command, OrderBomDetailSearchModel searchModel)
        {
            string whereStatement = "select b from OrderBomDetail as b where b.OrderDetailId = ?";
            IList<object> param = new List<object>();
            param.Add(searchModel.OrderDetailId);

            HqlStatementHelper.AddEqStatement("Operation", searchModel.Operation, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("OpReference", searchModel.OpReference, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "b", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "b", ref whereStatement, param);

            IList<OrderBomDetail> orderBomDetailList = genericMgr.FindAll<OrderBomDetail>(whereStatement, param.ToArray());
            return PartialView(new GridModel(orderBomDetailList));
        }

        public ActionResult _OrderOperationList(string id)
        {
            ViewBag.OrderDetailId = id;

            return PartialView();
        }

        [GridAction]
        public ActionResult _SelectOperationBatchEditing(GridCommand command, string orderDetailId)
        {
            IList<OrderOperation> orderOperationList = genericMgr.FindAll<OrderOperation>(selectOrderOperationStatement, orderDetailId);
            return View(new GridModel(orderOperationList));
        }
        #region
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public JsonResult _deleteBomDetailBatchById(string checkedItems, int orderDetailId)
        {
            try
            {
                IList<string> checkedItemsList = new List<string>();
                if (!string.IsNullOrWhiteSpace(checkedItems))
                {
                    checkedItemsList = checkedItems.Split(',');
                };
                IList<int> deletedItemsList = new List<int>();
                if (checkedItemsList.Count > 0)
                {
                    foreach (var checkedItem in checkedItemsList)
                    {
                        deletedItemsList.Add(int.Parse(checkedItem));
                    }
                }
                this.orderMgr.DeleteOrderBomDetails(deletedItemsList);
                object obj = new { SuccessMessage = string.Format(Resources.ORD.OrderBomDetail.OrderBomDetail_Deleted, orderDetailId) };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }
        #endregion

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public JsonResult _SaveBomDetailBatchEditing([Bind(Prefix =
            "inserted")]IEnumerable<OrderBomDetail> insertedBomDetails,
            [Bind(Prefix = "updated")]IEnumerable<OrderBomDetail> updatedBomDetails,
            [Bind(Prefix = "deleted")]IEnumerable<OrderBomDetail> deletedBomDetails, string OrderDetailId)
        {
            try
            {
                var orderDetail = this.genericMgr.FindById<OrderDetail>(int.Parse(OrderDetailId));
                IList<OrderBomDetail> newOrderBomDetailList = new List<OrderBomDetail>();
                IList<OrderBomDetail> updateOrderBomDetailList = new List<OrderBomDetail>();
                if (insertedBomDetails != null && insertedBomDetails.Count() > 0)
                {
                    foreach (OrderBomDetail orderBomDetail in insertedBomDetails)
                    {
                        PrepareOrderBomDetail(orderDetail, orderBomDetail);
                        orderBomDetail.BomUnitQty = orderBomDetail.OrderedQty / orderDetail.OrderedQty;
                        newOrderBomDetailList.Add(orderBomDetail);
                    }
                }
                if (updatedBomDetails != null && updatedBomDetails.Count() > 0)
                {
                    foreach (OrderBomDetail orderBomDetail in updatedBomDetails)
                    {
                        PrepareOrderBomDetail(orderDetail, orderBomDetail);
                        orderBomDetail.BomUnitQty = orderBomDetail.OrderedQty / orderDetail.OrderedQty;
                        updateOrderBomDetailList.Add(orderBomDetail);
                    }
                }

                orderMgr.BatchUpdateOrderBomDetails(orderDetail, newOrderBomDetailList, updateOrderBomDetailList, (IList<OrderBomDetail>)deletedBomDetails);

                object obj = new { SuccessMessage = string.Format(Resources.ORD.OrderBomDetail.OrderBomDetail_Saved, OrderDetailId) };
                return Json(obj);
            }
            catch (BusinessException e)
            {
                SaveErrorMessage(e.GetMessages()[0].GetMessageString());
                return Json(null);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
                return Json(null);
            }
        }

        #region 先注掉，保存物料清单和工艺流程的
        //[AcceptVerbs(HttpVerbs.Post)]
        //[GridAction]
        //public ActionResult _SaveBomDetailBatchEditing([Bind(Prefix =
        //    "inserted")]IEnumerable<OrderBomDetail> insertedBomDetails,
        //    [Bind(Prefix = "updated")]IEnumerable<OrderBomDetail> updatedBomDetails,
        //    [Bind(Prefix = "deleted")]IEnumerable<OrderBomDetail> deletedBomDetails, string id)
        //{
        //    if (insertedBomDetails != null)
        //    {
        //        orderMgr.AddOrderBomDetails(Convert.ToInt32(id), (IList<OrderBomDetail>)insertedBomDetails);
        //    }
        //    if (updatedBomDetails != null)
        //    {
        //        orderMgr.UpdateOrderBomDetails((IList<OrderBomDetail>)updatedBomDetails);
        //    }
        //    if (deletedBomDetails != null)
        //    {
        //        IList<int> deletedBomDetailIds = deletedBomDetails.Select(q => q.Id).ToList<int>();
        //        orderMgr.DeleteOrderBomDetails(deletedBomDetailIds);
        //    }
        //    IList<OrderBomDetail> orderBomDetailList = genericMgr.FindAll<OrderBomDetail>("select b from OrderBomDetail as b where b.OrderDetailId = ?", id);
        //    return View(new GridModel(orderBomDetailList));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //[GridAction]
        //public ActionResult _SaveOperationBatchEditing([Bind(Prefix =
        //    "inserted")]IEnumerable<OrderOperation> insertedOperations,
        //    [Bind(Prefix = "updated")]IEnumerable<OrderOperation> updatedOperations,
        //    [Bind(Prefix = "deleted")]IEnumerable<OrderOperation> deletedOperations, string id)
        //{
        //    if (insertedOperations != null)
        //    {
        //        orderMgr.AddOrderOperations(Convert.ToInt32(id), (IList<OrderOperation>)insertedOperations);
        //    }
        //    if (updatedOperations != null)
        //    {
        //        orderMgr.UpdateOrderOperations((IList<OrderOperation>)updatedOperations);
        //    }
        //    if (deletedOperations != null)
        //    {
        //        IList<int> deletedOperationIds = deletedOperations.Select(q => q.Id).ToList<int>();
        //        orderMgr.DeleteOrderBomDetails(deletedOperationIds);
        //    }
        //    IList<OrderOperation> orderOperationList = genericMgr.FindAll<OrderOperation>(selectOrderOperationStatement, id);
        //    return View(new GridModel(orderOperationList));
        //}

        //public ActionResult _WebBomDetail(string itemCode)
        //{
        //    WebOrderDetail webOrderDetail = new WebOrderDetail();

        //    Item item = genericMgr.FindById<Item>(itemCode);
        //    if (item != null)
        //    {
        //        webOrderDetail.Item = item.Code;
        //        webOrderDetail.ItemDescription = item.Description;
        //        webOrderDetail.Uom = item.Uom;
        //    }
        //    return this.Json(webOrderDetail);
        //}
        #endregion

        #endregion

        #region import
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Import")]
        public ActionResult Import()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Import")]
        public ActionResult ImportSapOrder(OrderMaster orderMaster)
        {
            try
            {
                if (string.IsNullOrEmpty(orderMaster.ExternalOrderNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_SAPProductionOrderNumberCanNotBeEmpty);
                }
                //SAPService.SAPService sapService = new SAPService.SAPService();
                //com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
                //string orderNo = sapService.GetProdOrder(user.Code, orderMaster.ExternalOrderNo);

                //if (string.IsNullOrWhiteSpace(orderNo))
                //{
                //    SaveErrorMessage(string.Format("SAP生产单{0}不存在或没有释放或没有维护LES生产线。", orderMaster.ExternalOrderNo));
                //}
                //else
                //{
                //    SaveSuccessMessage(string.Format("SAP生产单{0}导入成功，生成LES生产单号为{1}。", orderMaster.ExternalOrderNo, orderNo));
                //}
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }

            return View("Import");
        }
        #endregion

        #region sequece import
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_SeqImport")]
        public ActionResult SeqImport()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_SeqImport")]
        public ActionResult ImportSeq(OrderMaster orderMaster)
        {
            try
            {
                if (orderMaster.StartDate == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_DateCanNotBeEmpty);
                }
                //SAPService.SAPService sapService = new SAPService.SAPService();
                //com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
                //sapService.GetVanOrders(user.Code, orderMaster.StartDate.Value);

                //if (string.IsNullOrWhiteSpace(orderNo))
                //{
                //    SaveErrorMessage(string.Format("SAP生产单{0}不存在或没有释放或没有维护LES生产线。", orderMaster.ExternalOrderNo));
                //}
                //else
                //{
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ModelSeqImportSuccessfully);
                //}
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }

            return View("SeqImport");
        }
        #endregion

        #region receive
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive,Url_OrderMstr_Production_Receive_MI")]
        public ActionResult ReceiveIndex(int? id)
        {
            ViewBag.ResourceGroup = id;
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive")]
        public ActionResult ReceiveList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }

            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive")]
        public ActionResult _ReceiveAjaxList(GridCommand command, OrderMasterSearchModel searchModel)
        {

            string replaceFrom = "_ReceiveAjaxList";
            string replaceTo = "ReceiveList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            string whereStatement = " and o.Status = " + (int)com.Sconit.CodeMaster.OrderStatus.InProcess +
                                   " and exists (select 1 from OrderDetail as d where d.RecQty + d.ScrapQty  < d.OrderQty and d.OrderNo = o.OrderNo) ";
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
        }


        public ActionResult _ReceiveSearch(int? resourceGroup)
        {
            ViewBag.ResourceGroup = resourceGroup;
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive")]
        public ActionResult _ReceiveOrderDetailList(string orderNo)
        {
            ViewBag.IsVanOrder = IsVanOrder(orderNo);
            //OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);

            //DetachedCriteria criteria = DetachedCriteria.For<OrderDetail>();
            //criteria.Add(Expression.Eq("OrderNo", orderNo));
            //if (!orderMaster.IsOpenOrder)
            //{
            //    criteria.Add(Expression.LtProperty("ReceivedQty", "OrderedQty"));
            //}
            //IList<OrderDetail> orderDetailList = this.genericMgr.FindAll<OrderDetail>(criteria);
            IList<OrderDetail> orderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>(@"select * from ORD_OrderDet_4 where OrderNo=? ", orderNo);
            return PartialView(orderDetailList);
        }


        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive")]
        public ActionResult _EXReceiveOrderDetailList(string flow)
        {
            ViewBag.Flow = flow;
            return PartialView();
        }


        [GridAction]
        public ActionResult _EXReceiveOrderDetailListAjax(string flow)
        {
            IList<OrderMaster> orderMasterList = this.genericMgr.FindEntityWithNativeSql<OrderMaster>(@"select top 200 * from ORD_OrderMstr_4 where flow=? and status in(1,2) order by status desc", flow);
            if (orderMasterList.Count > 0)
            {
                string selectOrderDetStatement = string.Empty;
                IList<object> selectOrderDetParas = new List<object>();
                foreach (OrderMaster orderMaster in orderMasterList)
                {
                    if (selectOrderDetStatement == string.Empty)
                    {
                        selectOrderDetStatement = "select * from ORD_OrderDet_4 where OrderNo in(?";
                    }
                    else
                    {
                        selectOrderDetStatement += ",?";
                    }
                    selectOrderDetParas.Add(orderMaster.OrderNo);
                }
                selectOrderDetStatement += ")";

                IList<OrderDetail> orderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>(selectOrderDetStatement, selectOrderDetParas.ToArray());
                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    orderDetail.Status = orderMasterList.First(o => o.OrderNo == orderDetail.OrderNo).Status;
                }
                this.FillCodeDetailDescription<OrderDetail>(orderDetailList);
                return PartialView(new GridModel(orderDetailList.OrderByDescending(o => o.Status)));
            }
            else
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
        }

        public JsonResult _EXReceiveOrderDetail(int id, decimal currentReceiveQty, string remark)
        {
            try
            {
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();

                OrderDetail orderDetail = this.genericMgr.FindEntityWithNativeSql<OrderDetail>
                    ("select * from ORD_OrderDet_4 where Id=?", id).FirstOrDefault();
                OrderDetailInput orderDetailInput = new OrderDetailInput();
                orderDetailInput.ReceiveQty = currentReceiveQty;
                orderDetail.Remark = remark;
                orderDetail.AddOrderDetailInput(orderDetailInput);
                orderDetailList.Add(orderDetail);

                //IList<ItemEx> itemExList = this.genericMgr.FindAll<ItemEx>("from ItemEx where Code=?", orderDetail.Item);
                var item = this.genericMgr.FindById<Item>(orderDetail.Item);
                if (item.ItemOption == com.Sconit.CodeMaster.ItemOption.NeedAging)
                {
                    orderDetail.OldOption = Sconit.CodeMaster.HuOption.UnAging;
                }
                else
                {
                    orderDetail.OldOption = Sconit.CodeMaster.HuOption.NoNeed;
                }

                string printUrl = "";
                ReceiptMaster receiptMaster = orderMgr.ReceiveOrder(orderDetailList);
                if (receiptMaster.CreateHuOption == Sconit.CodeMaster.CreateHuOption.Receive)
                {
                    //打印
                    IList<Hu> huList = genericMgr.FindAll<Hu>("from Hu as h where h.ReceiptNo = ?", receiptMaster.ReceiptNo);
                    string orderNo = orderDetailList.Select(i => i.OrderNo).Distinct().Single();
                    string huTemplate = this.genericMgr.FindAll<string>("select HuTemplate from OrderMaster where OrderNo = ?", orderNo).Single();
                    if (string.IsNullOrWhiteSpace(huTemplate))
                    {
                        huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                    }
                    printUrl = PrintHuList(huList, huTemplate);
                }
                object obj = new { Status = "OK", SuccessMessage = string.Format(Resources.ORD.OrderMaster.OrderMaster_Received, orderDetailList[0].OrderNo), SuccessData = orderDetailList[0].OrderNo, PrintUrl = printUrl };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive_MI")]
        public ActionResult _MIReceiveOrderDetailList(string flow)
        {
            ViewBag.Flow = flow;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive_MI")]
        public ActionResult _MIReceiveOrderDetailListAjax(string flow)
        {
            var orderDetails = this.genericMgr.FindEntityWithNativeSql<OrderDetail>
                (@"select d.* from ORD_OrderDet_4 d join Ord_orderMstr_4 m on d.OrderNo = m.OrderNo
                    where (ScrapQty+RecQty<OrderQty-(d.MinUC*0.5)) and Flow =? and Status =2 
                    order by m.StartTime,m.OrderNo,d.Seq", flow);
            var orderMasterList = this.genericMgr.FindAllIn<OrderMaster>
                (" from OrderMaster where OrderNo in(?", orderDetails.Select(p => p.OrderNo).Distinct());

            foreach (OrderDetail orderDetail in orderDetails)
            {
                if (!string.IsNullOrWhiteSpace(orderDetail.Direction))
                {
                    var directions = genericMgr.FindAll<HuTo>(" from HuTo where Code =? ", orderDetail.Direction);
                    if (directions != null && directions.Count > 0)
                    {
                        orderDetail.DirectionDescription = directions.First().Description;
                    }
                    else
                    {
                        orderDetail.DirectionDescription = orderDetail.Direction;
                    }
                }
                var orderMaster = orderMasterList.Where(p => p.OrderNo == orderDetail.OrderNo).First();
                orderDetail.StartTime = orderMaster.StartTime;
                if (!string.IsNullOrWhiteSpace(orderMaster.Shift))
                {
                    orderDetail.ShiftName = this.genericMgr.FindById<ShiftMaster>(orderMaster.Shift).Name;
                }
            }
            this.FillCodeDetailDescription<OrderDetail>(orderDetails);
            return PartialView(new GridModel(orderDetails));
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive_MI")]
        public JsonResult _MIReceiveOrderDetail(int id, decimal currentReceiveQty)
        {
            try
            {
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                OrderDetail orderDetail = this.genericMgr.FindEntityWithNativeSql<OrderDetail>
                    ("select * from ORD_OrderDet_4 where Id=?", id).FirstOrDefault();
                OrderDetailInput orderDetailInput = new OrderDetailInput();
                orderDetailInput.ReceiveQty = currentReceiveQty;
                //orderDetail.Remark = remark;
                orderDetail.AddOrderDetailInput(orderDetailInput);
                orderDetailList.Add(orderDetail);

                var item = this.genericMgr.FindById<Item>(orderDetail.Item);
                if (item.ItemOption == com.Sconit.CodeMaster.ItemOption.NeedFilter)
                {
                    orderDetail.OldOption = Sconit.CodeMaster.HuOption.UnFilter;
                }
                else
                {
                    orderDetail.OldOption = Sconit.CodeMaster.HuOption.NoNeed;
                }

                string printUrl = "";
                ReceiptMaster receiptMaster = orderMgr.ReceiveOrder(orderDetailList);
                try
                {
                    if (receiptMaster.CreateHuOption == Sconit.CodeMaster.CreateHuOption.Receive)
                    {
                        //打印
                        //IList<Hu> huList = genericMgr.FindAll<Hu>("from Hu as h where h.ReceiptNo = ?", receiptMaster.ReceiptNo);
                        //string orderNo = orderDetailList.Select(i => i.OrderNo).Distinct().Single();
                        //string huTemplate = this.genericMgr.FindAll<string>("select HuTemplate from OrderMaster where OrderNo = ?", orderNo).Single();

                        var huIds = receiptMaster.ReceiptDetails.SelectMany(p => p.ReceiptLocationDetails.Select(q => q.HuId)).ToList();
                        IList<Hu> huList = new List<Hu>();
                        foreach (var huId in huIds)
                        {
                            huList.Add(this.genericMgr.FindById<Hu>(huId));
                        }
                        string huTemplate = huList.First().HuTemplate;
                        if (string.IsNullOrWhiteSpace(huTemplate))
                        {
                            huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                        }
                        printUrl = PrintHuList(huList, huTemplate);
                    }
                }
                catch (Exception ex)
                {
                    SaveErrorMessage(ex);
                }
                object obj = new
                {
                    Status = "OK",
                    SuccessMessage = string.Format(Resources.ORD.OrderMaster.OrderMaster_Received, orderDetailList[0].OrderNo),
                    SuccessData = orderDetailList[0].OrderNo,
                    PrintUrl = printUrl,
                    Flow = receiptMaster.Flow
                };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive")]
        public ActionResult _PRReceiveOrderDetailList(string flow)
        {
            ViewBag.Flow = flow;
            return PartialView();
        }

        [GridAction]
        public ActionResult _PRReceiveOrderDetailListAjax(string flow)
        {
            IList<OrderMaster> orderMasterList = this.genericMgr.FindEntityWithNativeSql<OrderMaster>
                (@"select top 200 * from ORD_OrderMstr_4 where flow=? and status in(1,2) order by status desc", flow);
            if (orderMasterList.Count > 0)
            {
                string selectOrderDetStatement = string.Empty;
                IList<object> selectOrderDetParas = new List<object>();
                foreach (OrderMaster orderMaster in orderMasterList)
                {
                    if (selectOrderDetStatement == string.Empty)
                    {
                        selectOrderDetStatement = "select * from ORD_OrderDet_4 where OrderNo in(?";
                    }
                    else
                    {
                        selectOrderDetStatement += ",?";
                    }
                    selectOrderDetParas.Add(orderMaster.OrderNo);
                }
                selectOrderDetStatement += ")";

                IList<OrderDetail> orderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>(selectOrderDetStatement, selectOrderDetParas.ToArray());
                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    orderDetail.Status = orderMasterList.First(o => o.OrderNo == orderDetail.OrderNo).Status;
                }
                this.FillCodeDetailDescription<OrderDetail>(orderDetailList);
                return PartialView(new GridModel(orderDetailList.OrderByDescending(o => o.Status)));
            }
            else
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
        }

        public JsonResult _PRReceiveOrderDetail(int id, decimal currentReceiveQty)
        {
            try
            {
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                OrderDetail orderDetail = this.genericMgr.FindEntityWithNativeSql<OrderDetail>
                    ("select * from ORD_OrderDet_4 where Id=?", id).FirstOrDefault();
                OrderDetailInput orderDetailInput = new OrderDetailInput();
                OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);
                orderMaster.CreateHuOption = Sconit.CodeMaster.CreateHuOption.None;
                orderMaster.IsShipScanHu = false;
                orderMaster.IsReceiveScanHu = false;
                this.genericMgr.Update(orderMaster);
                this.genericMgr.FlushSession();

                orderDetailInput.ReceiveQty = currentReceiveQty;
                orderDetail.AddOrderDetailInput(orderDetailInput);
                orderDetailList.Add(orderDetail);

                ReceiptMaster receiptMaster = orderMgr.ReceiveOrder(orderDetailList);
                string printUrl = "";
                //if (receiptMaster.CreateHuOption == Sconit.CodeMaster.CreateHuOption.Receive)
                //{
                //    //打印
                //    IList<Hu> huList = genericMgr.FindAll<Hu>("from Hu as h where h.ReceiptNo = ?", receiptMaster.ReceiptNo);
                //    string orderNo = orderDetailList.Select(i => i.OrderNo).Distinct().Single();
                //    string huTemplate = this.genericMgr.FindAll<string>("select HuTemplate from OrderMaster where OrderNo = ?", orderNo).Single();
                //    if (string.IsNullOrWhiteSpace(huTemplate))
                //    {
                //        huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                //    }
                //    printUrl = PrintHuList(huList, huTemplate);
                //}
                object obj = new
                {
                    Status = "OK",
                    SuccessMessage = string.Format(Resources.ORD.OrderMaster.OrderMaster_Received, orderDetailList[0].OrderNo),
                    SuccessData = orderDetailList[0].OrderNo,
                    PrintUrl = printUrl
                };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive")]
        public ActionResult ReceiveEdit(string orderNo)
        {
            OrderMaster order = genericMgr.FindById<OrderMaster>(orderNo);
            ViewBag.IsVanOrder = IsVanOrder(orderNo);
            return View(order);
        }
        #endregion

        #region materialin
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedOrderMaster")]
        public ActionResult MaterialInIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedOrderMaster")]
        public ActionResult ForceMaterialInIndex()
        {
            ViewBag.Visible = "false";
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedOrderMaster")]
        public JsonResult FeedQty(OrderMaster orderMaster, [Bind(Prefix = "updated")]IEnumerable<OrderBomDetail> updatedOrderBomDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(orderMaster.OrderNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionOrderCanNotBeEmpty);
                }
                //string ordercheckStr = SecurityHelper.GetProductionOrderStatement(orderMaster.OrderNo);
                orderMaster = orderMgr.GetAuthenticOrder(orderMaster.OrderNo);
                if (orderMaster == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionOrderIsWrongOrLackPermission);
                }

                #region orderBomdetailList
                IList<FeedInput> feedInputList = new List<FeedInput>();
                if (updatedOrderBomDetails != null && updatedOrderBomDetails.Count() > 0)
                {
                    foreach (OrderBomDetail orderBomDetail in updatedOrderBomDetails)
                    {
                        FeedInput feedInput = new FeedInput();

                        if (!string.IsNullOrEmpty(orderBomDetail.FeedLocation))
                        {
                            string locationcheckStr = "from Location as l where l.Region = ? and l.Code = ?";
                            Location location = genericMgr.FindAll<Location>(locationcheckStr, new object[] { orderMaster.PartyFrom, orderBomDetail.FeedLocation }).SingleOrDefault<Location>();
                            if (location == null)
                            {
                                throw new BusinessException(Resources.EXT.ControllerLan.Con_Item + orderBomDetail.Item + Resources.EXT.ControllerLan.Con_LocationIsWrong);
                            }
                            feedInput.LocationFrom = orderBomDetail.FeedLocation;
                        }
                        else
                        {
                            feedInput.LocationFrom = orderBomDetail.Location;
                        }

                        feedInput.OrderNo = orderMaster.OrderNo;
                        feedInput.Qty = orderBomDetail.FeedQty;
                        feedInput.Item = orderBomDetail.Item;
                        if (string.IsNullOrEmpty(orderBomDetail.Uom))
                        {
                            Item item = genericMgr.FindById<Item>(orderBomDetail.Item);
                            feedInput.Uom = item.Uom;
                        }
                        else
                        {
                            feedInput.Uom = orderBomDetail.Uom;
                        }
                        feedInput.Operation = orderBomDetail.Operation;
                        feedInput.OpReference = orderBomDetail.OpReference;
                        feedInputList.Add(feedInput);
                    }
                }
                #endregion

                if (feedInputList.Count == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BackFlushDetailIsEmpty);
                }

                productionLineMgr.FeedRawMaterial(orderMaster.OrderNo, feedInputList, false);
                object obj = new { SuccessMessage = string.Format(Resources.PRD.ProductLineLocationDetail.ProductLineLocationDetail_FeedIn) };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedOrderMaster")]
        public JsonResult ForceFeedQty(OrderMaster orderMaster,
            [Bind(Prefix = "inserted")]IEnumerable<OrderBomDetail> insertedOrderBomDetails,
            [Bind(Prefix = "updated")]IEnumerable<OrderBomDetail> updatedOrderBomDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(orderMaster.OrderNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionOrderCanNotBeEmpty);
                }
                orderMaster = genericMgr.FindById<OrderMaster>(orderMaster.OrderNo);

                #region orderBomdetailList

                IList<FeedInput> feedInputList = new List<FeedInput>();
                List<OrderBomDetail> orderBomDetailList = new List<OrderBomDetail>();
                if (insertedOrderBomDetails != null && insertedOrderBomDetails.Count() > 0)
                {
                    orderBomDetailList.AddRange(insertedOrderBomDetails);

                }
                if (updatedOrderBomDetails != null && updatedOrderBomDetails.Count() > 0)
                {
                    orderBomDetailList.AddRange(updatedOrderBomDetails);

                }
                if (orderBomDetailList.Count > 0)
                {
                    foreach (OrderBomDetail orderBomDetail in orderBomDetailList)
                    {
                        FeedInput feedInput = new FeedInput();
                        feedInput.LocationFrom = orderMaster.LocationFrom;
                        feedInput.OrderNo = orderMaster.OrderNo;
                        feedInput.Qty = orderBomDetail.FeedQty;
                        feedInput.Item = orderBomDetail.Item;
                        feedInput.Uom = orderBomDetail.Uom;
                        feedInput.QualityType = com.Sconit.CodeMaster.QualityType.Qualified;
                        feedInputList.Add(feedInput);
                    }
                }
                #endregion

                if (feedInputList.Count == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BackFlushDetailIsEmpty);
                }

                productionLineMgr.FeedRawMaterial(orderMaster.OrderNo, feedInputList, true);
                object obj = new { SuccessMessage = string.Format("{0}{1}", orderMaster.OrderNo, Resources.PRD.ProductLineLocationDetail.ProductLineLocationDetail_FeedIn) };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _SelectForceFeedQtyBatchEditing(string OrderNo, bool hasError)
        {
            var orderBomDetails = new List<OrderBomDetail>();
            if (!hasError)
            {
                //OrderNo = OrderNo == null ? string.Empty : OrderNo;
                //orderBomDetails = this.genericMgr.FindAll<OrderDetail>
                //   (" from OrderDetail where OrderNo=? ", OrderNo)
                //   .Select(p => new OrderBomDetail
                //   {
                //       Flow = p.Flow,
                //       Item = p.Item,
                //       ItemDescription = p.ItemDescription,
                //       ReferenceItemCode = p.ReferenceItemCode,
                //       Uom = p.Uom,
                //       UnitCount = p.UnitCount,
                //       FeedLocation = p.LocationTo,
                //       FeedQty = p.OrderedQty,
                //   })
                //   .Where(p => p.ItemDescription.Contains("塞芯"))
                //   .ToList();
            }
            return PartialView(new GridModel(orderBomDetails));
        }

        [GridAction]
        public ActionResult _SelectFeedQtyBatchEditing(string orderNo)
        {
            IList<OrderBomDetail> orderBomDetails = new List<OrderBomDetail>();
            OrderMaster orderMaster = orderMgr.GetAuthenticOrder(orderNo);
            if (orderMaster != null)
            {
                if (orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Submit || orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.InProcess || orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Complete)
                {
                    string selectOrderBomDetailStatement = "select d from OrderBomDetail as d where d.OrderNo = ? and d.BackFlushMethod = ?";
                    orderBomDetails = genericMgr.FindAll<OrderBomDetail>(selectOrderBomDetailStatement, new object[] { orderNo, (int)com.Sconit.CodeMaster.BackFlushMethod.WeightAverage });
                    if (orderBomDetails != null && orderBomDetails.Count > 0)
                    {
                        foreach (OrderBomDetail bomDetail in orderBomDetails)
                        {
                            Item item = genericMgr.FindById<Item>(bomDetail.Item);
                            if (item != null)
                            {
                                bomDetail.Item = item.Code;
                                bomDetail.ReferenceItemCode = item.ReferenceCode;
                                bomDetail.ItemDescription = item.Description;
                                bomDetail.UnitCount = item.UnitCount;
                                bomDetail.Uom = item.Uom;
                            }
                        }
                    }
                }
            }
            return PartialView(new GridModel(orderBomDetails));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedOrderMaster")]
        public ActionResult _QtyMaterialInEdit()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedOrderMaster")]
        public ActionResult _ForceQtyMaterialInEdit()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedOrderMaster")]
        public ActionResult _FeedQtyDetailList(string orderNo)
        {

            IList<Uom> uoms = genericMgr.FindAll<Uom>();
            ViewData.Add("uoms", uoms);

            #region 选默认库位的
            OrderMaster orderMaster = orderMgr.GetAuthenticOrder(orderNo);
            if (orderMaster != null)
            {
                ViewBag.Region = orderMaster.PartyFrom;
            }
            #endregion

            ViewBag.orderNo = orderNo;
            return PartialView();

        }

        //[GridAction(EnableCustomBinding = true)]
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedOrderMaster")]
        public ActionResult _ForceFeedQtyDetailList(string orderNo, string referenceOrderNo)
        {
            #region combox
            //IList<Uom> uoms = genericMgr.FindAll<Uom>();
            //ViewData.Add("uoms", uoms);
            #endregion

            #region 选默认库位的
            //OrderMaster orderMaster = this.orderMgr.GetAuthenticOrder(orderNo);
            //if (orderMaster != null)
            //{
            //    ViewBag.Region = orderMaster.PartyFrom;
            //}
            #endregion
            ViewBag.HasError = false;
            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                ViewBag.OrderNo = orderNo;
                ViewBag.ReferenceOrderNo = referenceOrderNo;
                var orderMasters = this.genericMgr.FindAll<OrderMaster>
                    (" from OrderMaster where OrderNo=?", orderNo);
                if (orderMasters == null || orderMasters.Count() == 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheProductionOrderNotExits, orderNo);
                    ViewBag.HasError = true;
                    return PartialView();
                }
                else
                {
                    var orderMaster = orderMasters.First();
                    if (orderMaster.Type != Sconit.CodeMaster.OrderType.Production)
                    {
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheOrderIsNotProductionOrderCanNotBackFlush, orderNo);
                        ViewBag.HasError = true;
                        return PartialView();
                    }
                    if (orderMaster.Status == Sconit.CodeMaster.OrderStatus.Close ||
                        orderMaster.Status == Sconit.CodeMaster.OrderStatus.Cancel)
                    {
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheProductionOrderAlreadyCancelledOrAlreadyClosedCanNotBackFlush, orderNo);
                        ViewBag.HasError = true;
                        return PartialView();
                    }
                    //var flowMaster = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);
                    //if (!flowMaster.IsCreatePickList)
                    //{
                    //    SaveErrorMessage("此生产单{0}无需投料", orderNo);
                    //    ViewBag.HasError = true;
                    //    return PartialView();
                    //}
                    var count = this.genericMgr.FindAll<long>
                               (" select Count(*) from OrderDetail where OrderNo=? and ItemDescription like ?",
                               new object[] { orderNo, "%"+Resources.EXT.ControllerLan.Con_ForceMaterial +"%"})[0];
                    if (count == 0)
                    {
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheProductionOrderNoNeedBackFlush, orderNo);
                        ViewBag.HasError = true;
                        return PartialView();
                    }
                }
                //var orderBackflushDetails = this.genericMgr.FindAll<OrderBackflushDetail>
                //    (" from OrderBackflushDetail where OrderNo=?", orderNo);
                //if (orderBackflushDetails != null && orderBackflushDetails.Count() > 0)
                //{
                //    SaveWarningMessage("此生产单{0}已经投料,请确认是否需要在此投料", orderNo);
                //}
            }
            else
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputProductionOrderNumber);
                ViewBag.HasError = true;
            }
            return PartialView();
        }
        #endregion

        #region return
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ReturnOrderMaster")]
        public ActionResult MaterialInReturnIndex()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ReturnOrderMaster")]
        public JsonResult ReturnQty(OrderMaster orderMaster, [Bind(Prefix =
             "inserted")]IEnumerable<OrderBomDetail> insertedOrderBomDetails, [Bind(Prefix =
             "updated")]IEnumerable<OrderBomDetail> updatedOrderBomDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(orderMaster.OrderNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionOrderCanNotBeEmpty);
                }
                orderMaster = this.orderMgr.GetAuthenticOrder(orderMaster.OrderNo);

                if (orderMaster == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionOrderIsWrongOrLackPermission);
                }
                #region orderBomdetailList

                IList<ReturnInput> returnInputList = new List<ReturnInput>();
                List<OrderBomDetail> orderBomDetailList = new List<OrderBomDetail>();
                if (insertedOrderBomDetails != null && insertedOrderBomDetails.Count() > 0)
                {
                    orderBomDetailList.AddRange(insertedOrderBomDetails);
                }
                if (updatedOrderBomDetails != null && updatedOrderBomDetails.Count() > 0)
                {
                    orderBomDetailList.AddRange(updatedOrderBomDetails);
                }
                if (orderBomDetailList.Count > 0)
                {
                    foreach (OrderBomDetail orderBomDetail in orderBomDetailList)
                    {
                        ReturnInput returnInput = new ReturnInput();
                        if (!string.IsNullOrEmpty(orderBomDetail.FeedLocation))
                        {
                            string locationcheckStr = "from Location as l where l.Region = ? and l.Code = ?";
                            Location location = genericMgr.FindAll<Location>(locationcheckStr, new object[] { orderMaster.PartyFrom, orderBomDetail.FeedLocation }).SingleOrDefault<Location>();
                            if (location == null)
                            {
                                throw new BusinessException(Resources.EXT.ControllerLan.Con_Item + orderBomDetail.Item + Resources.EXT.ControllerLan.Con_LocationIsWrong);
                            }
                            returnInput.LocationTo = orderBomDetail.FeedLocation;
                        }
                        else
                        {
                            returnInput.LocationTo = orderMaster.LocationTo;
                        }
                        returnInput.OrderNo = orderMaster.OrderNo;
                        returnInput.Qty = orderBomDetail.FeedQty;
                        returnInput.Item = orderBomDetail.Item;
                        if (string.IsNullOrEmpty(orderBomDetail.Uom))
                        {
                            Item item = genericMgr.FindById<Item>(orderBomDetail.Item);
                            returnInput.Uom = item.Uom;
                        }
                        else
                        {
                            returnInput.Uom = orderBomDetail.Uom;
                        }
                        returnInput.QualityType = com.Sconit.CodeMaster.QualityType.Qualified;
                        returnInputList.Add(returnInput);
                    }
                }
                #endregion

                if (returnInputList.Count == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReturnMaterialDetailBeEmpty);
                }

                productionLineMgr.ReturnRawMaterial(orderMaster.OrderNo, orderMaster.TraceCode, null, null, returnInputList);
                object obj = new { SuccessMessage = string.Format(Resources.PRD.ProductLineLocationDetail.OrderMasterLocationDetail_ReturnOut) };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ReturnOrderMaster")]
        public ActionResult _ReturnQtyDetailList(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            //#region combox
            //IList<Uom> uoms = genericMgr.FindAll<Uom>();
            //ViewData.Add("uoms", uoms);
            //#endregion

            //#region 选默认库位的
            //string ordercheckStr = SecurityHelper.CheckOrderStatement(orderNo, com.Sconit.CodeMaster.OrderType.Production);
            //OrderMaster orderMaster = genericMgr.FindAll<OrderMaster>(ordercheckStr).SingleOrDefault<OrderMaster>();
            //if (orderMaster != null)
            //{
            //    ViewBag.Region = orderMaster.PartyFrom;
            //}
            //#endregion
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _SelectReturnQtyBatchEditing(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchReturnStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ProductLineLocationDetail>(searchStatementModel, command));
            // return PartialView(new GridModel(new List<OrderBomDetail>()));
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ReturnOrderMaster")]
        public JsonResult _ReturnQty(string OrderNo, string IdStr, string CurrentReturnQtyStr)
        {
            try
            {
                if (string.IsNullOrEmpty(OrderNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionOrderCanNotBeEmpty);
                }

                OrderMaster orderMaster = this.orderMgr.GetAuthenticOrder(OrderNo);

                if (orderMaster == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionOrderIsWrongOrLackPermission);
                }

                #region ProdLineLocationDetList
                string[] idArr = IdStr.Split(',');
                string[] currentReturnQtyArr = CurrentReturnQtyStr.Split(',');
                IList<ReturnInput> returnInputList = new List<ReturnInput>();
                // IList<ProductLineLocationDetail> prodLineLocationDetList = new List<ProductLineLocationDetail>();
                string hql = string.Empty;
                IList<object> pare = new List<object>();
                //foreach (string id in idArr)
                //{
                //    if (string.IsNullOrEmpty(hql))
                //    {
                //        hql = "select p from ProductLineLocationDetail as p where p.Id in (?";
                //    }
                //    else {
                //        hql += ",?";
                //    }
                //    pare.Add(id);
                //}
                //hql += ")";
                //prodLineLocationDetList = this.genericMgr.FindAll<ProductLineLocationDetail>(hql,pare.ToArray());
                //if (prodLineLocationDetList!=null && prodLineLocationDetList.Count > 0)
                //{
                //    #region 退货数
                //    foreach (ProductLineLocationDetail productLineLocationDetail in prodLineLocationDetList)
                //    {
                //        for(int i=0;i<idArr.Length;i++)
                //        {
                //            if (idArr[i] == productLineLocationDetail.Id.ToString()) { 
                //                productLineLocationDetail.CurrentReturnQty=int.Parse(currentReturnQtyArr[i]);
                //            }
                //        }
                //    }
                //    #endregion

                //    foreach (ProductLineLocationDetail productLineLocationDetail in prodLineLocationDetList)
                //    {
                //        ReturnInput returnInput = new ReturnInput();
                //        if (!string.IsNullOrEmpty(productLineLocationDetail.LocationFrom))
                //        {
                //            string locationcheckStr = "from Location as l where l.Region = ? and l.Code = ?";
                //            Location location = genericMgr.FindAll<Location>(locationcheckStr, new object[] { orderMaster.PartyFrom, productLineLocationDetail.LocationFrom }).SingleOrDefault<Location>();
                //            if (location == null)
                //            {
                //                throw new BusinessException("物料" + productLineLocationDetail.Item + "的库位不正确");
                //            }
                //            returnInput.LocationTo = productLineLocationDetail.LocationFrom;
                //        }
                //        else
                //        {
                //            returnInput.LocationTo = orderMaster.LocationTo;
                //        }
                //        returnInput.OrderNo = orderMaster.OrderNo;
                //        returnInput.Qty = productLineLocationDetail.Qty;
                //        returnInput.Item = productLineLocationDetail.Item;
                //        returnInput.UnitQty = 1;
                //        if (string.IsNullOrEmpty(productLineLocationDetail.Uom))
                //        {
                //            Item item = genericMgr.FindById<Item>(productLineLocationDetail.Item);
                //            returnInput.Uom = item.Uom;
                //        }
                //        else
                //        {
                //            returnInput.Uom = productLineLocationDetail.Uom;
                //        }
                //        returnInput.QualityType = com.Sconit.CodeMaster.QualityType.Qualified;
                //        returnInput.ProductLineLocationDetailId = productLineLocationDetail.Id;
                //        returnInputList.Add(returnInput);
                //    }
                //}
                #endregion
                #region
                for (int i = 0; i < idArr.Length; i++)
                {
                    ReturnInput returnInput = new ReturnInput();
                    returnInput.ProductLineLocationDetailId = int.Parse(idArr[i]);
                    returnInput.Qty = Convert.ToDecimal(currentReturnQtyArr[i]);
                    returnInput.UnitQty = 1;
                    returnInputList.Add(returnInput);
                }

                #endregion

                if (returnInputList.Count == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReturnMaterialDetailBeEmpty);
                }

                productionLineMgr.ReturnRawMaterial(returnInputList);
                object obj = new { SuccessMessage = string.Format(Resources.PRD.ProductLineLocationDetail.OrderMasterLocationDetail_ReturnOut) };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }
        #endregion

        #region batch
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_BatchProcess")]
        public ActionResult BatchProcessIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_BatchProcess")]
        public ActionResult BatchProcessList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Submit")]
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
                catch (Exception ex)
                {
                    SaveErrorMessage(ex);
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Start")]
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
                        if (IsVanOrder(orderNo))
                        {
                            orderMgr.StartVanOrder(orderNo);
                        }
                        else
                        {
                            orderMgr.StartOrder(orderNo);
                        }
                    }
                    SaveSuccessMessage(Resources.ORD.OrderMaster.OrderMaster_BatchStarted);
                }
                catch (Exception ex)
                {
                    SaveErrorMessage(ex);
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Delete")]
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
                catch (Exception ex)
                {
                    SaveErrorMessage(ex);
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Cancel")]
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
                catch (Exception ex)
                {
                    SaveErrorMessage(ex);
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Close")]
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
                catch (Exception ex)
                {
                    SaveErrorMessage(ex);
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_BatchProcess")]
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
                catch (Exception ex)
                {
                    SaveErrorMessage(ex);
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_BatchProcess")]
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
                catch (Exception ex)
                {
                    SaveErrorMessage(ex);
                }
            }

            return RedirectToAction("BatchProcessList");
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_BatchProcess")]
        public ActionResult _AjaxBatchProcessList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            //searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            return PartialView(GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command));
        }
        #endregion

        #region pl pause
        [SconitAuthorize(Permissions = "Url_Production_ProductLine_Pause")]
        public ActionResult ProductLinePause()
        {
            return View();
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Production_ProductLine_Pause")]
        public ActionResult _PauseFlowList(string flow)
        {
            ViewBag.flow = flow;
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Production_ProductLine_Pause")]
        public ActionResult _AjaxPauseFlowList(string flow)
        {

            IList<FlowMaster> productLineList = new List<FlowMaster>();
            IList<object> para = new List<object>();
            string flowStr = "select f from FlowMaster as f where  f.Type in (" + (int)com.Sconit.CodeMaster.OrderType.Production + ") and f.IsActive = " + true;
            if (!string.IsNullOrEmpty(flow))
            {
                flowStr += " and f.Code like ? ";
                para.Add(flow + "%");
            }
            productLineList = genericMgr.FindAll<FlowMaster>(flowStr, para.ToArray());
            return View(new GridModel<FlowMaster>(productLineList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Production_ProductLine_Pause")]
        public ActionResult BatchPause(string flowStr, bool isPause)
        {
            try
            {
                string[] flowArray = flowStr.Split(',');
                foreach (string f in flowArray)
                {

                    FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(f);
                    flowMaster.IsPause = isPause;
                    genericMgr.Update(flowMaster);
                    if (isPause)
                    {
                        SaveSuccessMessage(Resources.SCM.FlowMaster.FlowMaster_Paused);
                    }
                    else
                    {
                        SaveSuccessMessage(Resources.SCM.FlowMaster.FlowMaster_Resumed);
                    }
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }

            return RedirectToAction("ProductLinePause", "ProductionOrder");
        }
        #endregion

        #region pause
        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Pause")]
        public ActionResult PauseIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Pause")]
        public ActionResult PauseList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            TempData["OrderMasterSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Pause")]
        public ActionResult _AjaxPauseOrderList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = " where o.Type = " + (int)com.Sconit.CodeMaster.OrderType.Production +
                //" and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal +
                                    " and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.Submit + "," + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + ")" +
                                    " and exists (select 1 from ProductLineMap as p where (p.ProductLine = o.Flow or p.CabFlow = o.Flow or p.ChassisFlow = o.Flow) and (p.CabFlow != null and p.ChassisFlow != null))";
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Pause")]
        public JsonResult PopPauseOrder(string orderNo)
        {
            string errorMessage = string.Empty;
            string successMessage = string.Empty;
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            try
            {
                if (orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Submit)
                {
                    orderMgr.PauseProductOrder(orderMaster, null);
                    successMessage = string.Format(Resources.ORD.OrderMaster.OrderMaster_Paused, orderNo);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(new { orderNo = orderNo, Routing = orderMaster.Routing, CurrentOperation = orderMaster.CurrentOperation, status = (int)orderMaster.Status, errorMessage = errorMessage, successMessage = successMessage });
        }



        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Pause")]
        public string Pause(string orderNo, int? pauseOperation)
        {
            try
            {
                if (pauseOperation == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProcessCanNotBeEmpty);
                }
                orderMgr.PauseProductOrder(orderNo, pauseOperation);
                return "Success";
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return string.Empty;
            }
        }

        #endregion

        #region Resume

        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Resume")]
        public ActionResult ResumeIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Resume")]
        public ActionResult ResumeList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Resume")]
        public ActionResult _AjaxResumeOrderList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            searchModel.IsPause = true;
            string whereStatement = " where o.Type = " + (int)com.Sconit.CodeMaster.OrderType.Production +
                //" and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal +
                                    " and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.Submit + "," + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + ")" +
                                    " and exists (select 1 from ProductLineMap as p where p.ProductLine = o.Flow and (p.CabFlow != null and p.ChassisFlow != null))";
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel, whereStatement);
            return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Pause")]
        public ActionResult PopResumeOrder(string orderNo)
        {
            return Json(new { orderNo = orderNo });
        }

        [SconitAuthorize(Permissions = "Url_Production_OrderMaster_Pause")]
        public string Resume(string orderNo, Int64? sequence)
        {
            try
            {
                if (sequence == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_SeqNumberCanNotBeEmpty);
                }
                orderMgr.ReStartProductOrder(orderNo, sequence.Value);
                return "Success";
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return string.Empty;
            }
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
            if (!string.IsNullOrWhiteSpace(orderMaster.Shift))
            {
                printOrderMstr.Shift = this.genericMgr.FindById<ShiftMaster>(orderMaster.Shift).Name;
            }
            IList<object> data = new List<object>();
            data.Add(printOrderMstr);
            data.Add(printOrderMstr.OrderDetails);
            AddPrintOrderBomDetail(orderMaster, data);
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
            orderMaster.OrderDetails = orderDetails;
            PrintOrderMaster printOrderMstr = Mapper.Map<OrderMaster, PrintOrderMaster>(orderMaster);
            if (!string.IsNullOrWhiteSpace(orderMaster.Shift))
            {
                printOrderMstr.Shift = this.genericMgr.FindById<ShiftMaster>(orderMaster.Shift).Name;
            }
            foreach (var orderDetail in printOrderMstr.OrderDetails)
            {
                //if (!string.IsNullOrWhiteSpace()
                //{

                //}
            }
            IList<object> data = new List<object>();
            data.Add(printOrderMstr);
            data.Add(printOrderMstr.OrderDetails);
            AddPrintOrderBomDetail(orderMaster, data);
            string reportFileUrl = reportGen.WriteToFile(orderMaster.OrderTemplate, data);
            return reportFileUrl;
        }

        private void AddPrintOrderBomDetail(OrderMaster orderMaster, IList<object> data)
        {
            if (orderMaster.ResourceGroup == Sconit.CodeMaster.ResourceGroup.EX)
            {
                IList<OrderBomDetail> orderBomDetails = queryMgr.FindAll<OrderBomDetail>("select od from OrderBomDetail as od where od.OrderNo=?", orderMaster.OrderNo);
                IList<OrderBackflushDetail> orderBackflushDetails = queryMgr.FindAll<OrderBackflushDetail>("select od from OrderBackflushDetail as od where od.OrderNo=?", orderMaster.OrderNo);
                //Mapping
                var printOrderBomDetails = Mapper.Map<IList<OrderBomDetail>, List<PrintOrderBomDetail>>(orderBomDetails);
                //Group by Item
                printOrderBomDetails = printOrderBomDetails.GroupBy(p => p.Item, (k, g) => new PrintOrderBomDetail
                {
                    Item = k,
                    ItemDescription = g.First().ItemDescription,
                    ReferenceItemCode = g.First().ReferenceItemCode,
                    OrderedQty = g.Sum(q => q.OrderedQty * q.UnitQty),
                    Uom = g.First().BaseUom,
                    BomUnitQty = g.First().BomUnitQty
                }).ToList();
                orderBackflushDetails = orderBackflushDetails.GroupBy(p => p.Item, (k, g) => new OrderBackflushDetail
                {
                    Item = k,
                    ItemDescription = g.First().ItemDescription,
                    ReferenceItemCode = g.First().ReferenceItemCode,
                    BackflushedQty = g.Sum(q => q.BackflushedQty * q.UnitQty),
                    Uom = g.First().BaseUom
                }).ToList();

                //把orderBackflushDetails中的backflushQty赋值到printOrderBomDetails.BackflushedQty
                printOrderBomDetails = (from p in printOrderBomDetails
                                        join q in orderBackflushDetails
                                        on p.Item equals q.Item into result
                                        from r in result.DefaultIfEmpty()
                                        select new PrintOrderBomDetail
                                        {
                                            Item = p.Item,
                                            ItemDescription = p.ItemDescription,
                                            ReferenceItemCode = p.ReferenceItemCode,
                                            OrderedQty = p.OrderedQty,
                                            Uom = p.Uom,
                                            BomUnitQty = p.BomUnitQty,
                                            BackflushedQty = r != null ? r.BackflushedQty : 0
                                        }).ToList();

                //添加orderBackflushDetail中存在而orderBomDetial中不存在项
                var newPrintOrderBomDetails = from p in orderBackflushDetails
                                              join q in printOrderBomDetails
                                              on p.Item equals q.Item into result
                                              from r in result.DefaultIfEmpty()
                                              where r == null
                                              select new PrintOrderBomDetail
                                              {
                                                  Item = p.Item,
                                                  ItemDescription = p.ItemDescription,
                                                  ReferenceItemCode = p.ReferenceItemCode,
                                                  OrderedQty = 0,
                                                  Uom = p.BaseUom,
                                                  BomUnitQty = 0,
                                                  BackflushedQty = p.BackflushedQty
                                              };
                printOrderBomDetails.AddRange(newPrintOrderBomDetails);

                data.Add(printOrderBomDetails);
            }
        }

        #endregion

        #region 分装生产单下线
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_FZComplete")]
        public ActionResult FZComplete()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_FZComplete")]
        public void OrdeNoScan(string orderNo)
        {
            try
            {
                string hql = "select o from OrderMaster as o where o.Type = " + (int)com.Sconit.CodeMaster.OrderType.Transfer +
                    //" and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal +
                                        " and o.Status =" + (int)com.Sconit.CodeMaster.OrderStatus.InProcess +
                                        " and o.OrderStrategy =" + (int)com.Sconit.CodeMaster.FlowStrategy.KIT +
                                        " and o.OrderNo = ?";
                //SecurityHelper.AddPartyFromPermissionStatement(ref hql, "o", "PartyFrom", com.Sconit.CodeMaster.OrderType.Transfer, false);
                //SecurityHelper.AddPartyToPermissionStatement(ref hql, "o", "PartyTo", com.Sconit.CodeMaster.OrderType.Transfer);
                SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref hql, "o", "Type", "o", "PartyFrom", "o", "PartyTo", com.Sconit.CodeMaster.OrderType.Transfer, false);
                IList<OrderMaster> orderList = genericMgr.FindAll<OrderMaster>(hql, orderNo);
                if (orderList == null || orderList.Count == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_CanNotFindCorrespondingDistributeToProductionOrder);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }

        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_FZComplete")]
        public ActionResult _FZOrderDetailList(string orderNo)
        {
            IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>("select d from OrderDetail as d where d.OrderNo = ?", orderNo);
            return View(orderDetailList);
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_FZComplete")]
        public JsonResult FZOrderComplete(string orderNo, bool CompleteAndFeed)
        {

            //void KitOrderOfflineAndFeed(string kitOrderNo);
            try
            {
                ViewBag.FZOrderNo = orderNo;
                ViewBag.CompleteAndFeed = CompleteAndFeed;

                if (string.IsNullOrEmpty(orderNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_DistributionProductionOrderNumberCanNotBeEmpty);
                }
                orderMgr.KitOrderOfflineAndFeed(orderNo);
                //OrderMaster orderMaster = orderMgr.LoadOrderMaster(orderNo, true, false, false);
                //foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                //{
                //    OrderDetailInput input = new OrderDetailInput();
                //    input.ReceiveQty = orderDetail.OrderedQty;
                //    IList<OrderDetailInput> orderDetailInputList = new List<OrderDetailInput>();
                //    orderDetailInputList.Add(input);
                //    orderDetail.OrderDetailInputs = orderDetailInputList;
                //}
                //orderMgr.ReceiveOrder(orderMaster.OrderDetails);
                SaveSuccessMessage(string.Format(Resources.ORD.OrderMaster.OrderMaster_FZReceived, orderNo));
                object obj = new { SuccessMessage = string.Format(Resources.ORD.OrderMaster.OrderMaster_FZReceived, orderNo), SuccessData = orderNo };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        #endregion

        #endregion

        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, OrderMasterSearchModel searchModel, string whereStatement)
        {
            IList<object> param = new List<object>();

            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "o", "Type", "o", "PartyFrom", "o", "PartyTo", com.Sconit.CodeMaster.OrderType.Production, false);

            HqlStatementHelper.AddLikeStatement("ReferenceOrderNo", searchModel.ReferenceOrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ExternalOrderNo", searchModel.ExternalOrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "o", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("CreateUserName", searchModel.CreateUserName, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Priority", searchModel.Priority, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsPause", searchModel.IsPause, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("TraceCode", searchModel.TraceCode, "o", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "o", ref whereStatement, param);

            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.DateFrom, searchModel.DateTo, "o", ref whereStatement, param);
            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "o", ref whereStatement, param);
            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.DateTo, "o", ref whereStatement, param);
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

        private void PrepareOrderBomDetail(OrderDetail orderDetail, OrderBomDetail orderBomDetail)
        {
            if (string.IsNullOrEmpty(orderBomDetail.Item))
            {
                throw new BusinessException(Resources.EXT.ControllerLan.Con_ItemCanNotBeEmpty);
            }
            if (string.IsNullOrEmpty(orderBomDetail.Location))
            {
                orderBomDetail.Location = orderDetail.LocationFrom;
            }
            //if (orderBomDetail.Operation == 0)
            //{
            //    throw new BusinessException("工序不能为空");
            //}
            //if (orderBomDetail.BomUnitQty == 0)
            //{
            //    throw new BusinessException("单位用量不能为空");
            //}
            Item item = genericMgr.FindById<Item>(orderBomDetail.Item);
            orderBomDetail.ItemDescription = item.Description;
            orderBomDetail.BaseUom = item.Uom;
            if (string.IsNullOrEmpty(orderBomDetail.Uom))
            {
                orderBomDetail.Uom = item.Uom;
            }
            if (string.IsNullOrEmpty(orderBomDetail.ReferenceItemCode))
            {
                orderBomDetail.ReferenceItemCode = item.ReferenceCode;
            }
            if (string.IsNullOrEmpty(orderBomDetail.OpReference))
            {
                orderBomDetail.OpReference = string.Empty;
            }

            if (orderBomDetail.BaseUom != orderBomDetail.Uom)
            {
                orderBomDetail.UnitQty = itemMgr.ConvertItemUomQty(orderBomDetail.Item, orderBomDetail.Uom, 1, orderBomDetail.BaseUom);
            }
            else
            {
                orderBomDetail.UnitQty = 1;
            }

            if (orderBomDetail.OrderedQty == 0)
            {
                orderBomDetail.OrderedQty = orderDetail.OrderedQty * orderBomDetail.BomUnitQty;
            }
        }
        #endregion

        #region
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
            return PartialView(GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command));
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
            var OrderDetailLists = GetAjaxPageDataProcedure<OrderDetail>(procedureSearchStatementModel, command);
            List<OrderDetail> OrderDetailList = OrderDetailLists.Data.ToList();
            ExportToXLS<OrderDetail>("OrderDetail.xls", OrderDetailList);
        }
        #endregion
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_OrderDetail_Procurement_View")]
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
            if (searchModel.SearchForceMaterialOrder == "true")
            {
                whereStatement = " AND o.ProdLineFact ='EXV' AND o.SubType!=40 ";
            }
            //searchModel.SubType = (int)com.Sconit.CodeMaster.OrderSubType.Normal;
            ProcedureSearchStatementModel procedureSearchStatementModel = PrepareSearchStatement_1(command, searchModel, whereStatement, false);
            var orderMasterList = GetAjaxPageDataProcedure<OrderMaster>(procedureSearchStatementModel, command);
            foreach (var listData in orderMasterList.Data)
            {
                if (!string.IsNullOrWhiteSpace(listData.Shift))
                {
                    listData.Shift = genericMgr.FindById<ShiftMaster>(listData.Shift).Name;
                }
            }
            ExportToXLS<OrderMaster>("ProductionOrderMaster.xls", orderMasterList.Data.ToList());
        }
        #endregion
        public Int16 _GetFlowResourGroup(string flow)
        {
            if (!string.IsNullOrEmpty(flow))
            {
                FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(flow);
                return (Int16)flowMaster.ResourceGroup;
            }
            return 0;
        }

        private bool IsVanOrder(string orderNo)
        {
            bool isVanOrder = false;
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            IList productLineMapCount = genericMgr.FindAll("select count(*) as counter from ProductLineMap where (ProductLine = ? or CabFlow = ? or ChassisFlow = ?) and (CabFlow is not null and ChassisFlow is not null)", new object[] { orderMaster.Flow, orderMaster.Flow, orderMaster.Flow });
            isVanOrder = productLineMapCount != null && productLineMapCount.Count > 0 && productLineMapCount[0] != null && (long)productLineMapCount[0] > 0 ? true : false;
            return isVanOrder;
        }

        private string PrintHuList(IList<Hu> huList, string huTemplate)
        {
            foreach (var hu in huList)
            {
                if (!string.IsNullOrWhiteSpace(hu.Direction))
                {
                    hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                }
            }
            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);

            IList<object> data = new List<object>();
            data.Add(printHuList);
            data.Add(CurrentUser.FullName);
            return reportGen.WriteToFile(huTemplate, data);
        }

        private ProcedureSearchStatementModel PrepareSearchDetailStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            //if (searchModel.SubType.HasValue)
            //{
            //    whereStatement = string.Format(" and exists (select 1 from OrderMaster  as o where  o.SubType ={0} and o.OrderNo=d.OrderNo ) ",
            //    (int)com.Sconit.CodeMaster.OrderSubType.Normal);
            //}
            //else
            //{
            //    whereStatement = " and exists (select 1 from OrderMaster  as o where o.OrderNo=d.OrderNo ) ";
            //}
            if (!string.IsNullOrWhiteSpace(searchModel.MultiStatus))
            {
                string statusSql = " and o.Status in( ";
                string[] statusArr = searchModel.MultiStatus.Split(',');
                for (int st = 0; st < statusArr.Length; st++)
                {
                    statusSql += "'" + statusArr[st] + "',";
                }
                statusSql = statusSql.Substring(0, statusSql.Length - 1) + ")";
                whereStatement = string.Format(" and exists (select 1 from OrderMaster  as o where o.OrderNo=d.OrderNo {1} ) ",
                   (int)com.Sconit.CodeMaster.OrderSubType.Normal, statusSql);
            }
            List<ProcedureParameter> paraList = new List<ProcedureParameter>();
            List<ProcedureParameter> pageParaList = new List<ProcedureParameter>();
            paraList.Add(new ProcedureParameter { Parameter = searchModel.OrderNo, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.Flow, Type = NHibernate.NHibernateUtil.String });
            paraList.Add(new ProcedureParameter
            {
                Parameter = (int)com.Sconit.CodeMaster.OrderType.Production,
                Type = NHibernate.NHibernateUtil.String
            });
            paraList.Add(new ProcedureParameter { Parameter = searchModel.SubType, Type = NHibernate.NHibernateUtil.Int16 });
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

        private SearchStatementModel PrepareSearchReturnStatement(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            IList<object> param = new List<object>();
            string whereStatement = "where exists (select 1 from OrderMaster  as o where o.OrderNo=p.OrderNo and o.Status in ("
                + (int)com.Sconit.CodeMaster.OrderStatus.Complete + "," + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + "))";
            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LocationFrom", searchModel.LocationFrom, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Operation", searchModel.Operation, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsClose", false, "p", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by p.CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = "select count(*) from  ProductLineLocationDetail as p";
            searchStatementModel.SelectStatement = "select p from  ProductLineLocationDetail as p";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// 工单投料
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedOrderMaster")]
        public ActionResult ImportProductionOrderDetail(IEnumerable<HttpPostedFileBase> attachments, string OrderNo)
        {
            try
            {
                if (string.IsNullOrEmpty(OrderNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_OrderNumberCanNotBeEmpty);
                }

                foreach (var file in attachments)
                {
                    productionLineMgr.FeedRawMaterialFromXls(file.InputStream, OrderNo, false, DateTime.Now);
                    object obj = Resources.EXT.ControllerLan.Con_BackFlushDetailInputSuccessfully;
                    return Json(new { status = obj }, "text/plain");
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Json(null);


        }

        /// <summary>
        /// 工单强制投料
        /// </summary>
        /// <param name="attachments"></param>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedOrderMaster")]
        public ActionResult ImportForceProductionOrderDetail(IEnumerable<HttpPostedFileBase> attachments, string OrderNo)
        {
            try
            {
                if (string.IsNullOrEmpty(OrderNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_OrderNumberCanNotBeEmpty);
                }

                foreach (var file in attachments)
                {
                    productionLineMgr.FeedRawMaterialFromXls(file.InputStream, OrderNo, true, DateTime.Now);
                    object obj = Resources.EXT.ControllerLan.Con_BackFlushDetailInputSuccessfully;
                    return Json(new { status = obj }, "text/plain");
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Content("");
        }

        #endregion

        #region ExScrap
        [SconitAuthorize(Permissions = "Url_ExProductionOrder_Scrap")]
        public ActionResult ScrapNew()
        {
            return View();
        }

        List<string[]> scrapTypeList = new List<string[]> {
                new string[] { "21", Resources.EXT.ControllerLan.Con_ScraptType1 }, new string[] { "22", Resources.EXT.ControllerLan.Con_ScraptType2 },new string[] { "23", Resources.EXT.ControllerLan.Con_ScraptType4 },
                new string[] { "24", Resources.EXT.ControllerLan.Con_ScraptType }, new string[] { "25", Resources.EXT.ControllerLan.Con_ScraptType3 } };

        [SconitAuthorize(Permissions = "Url_ExProductionOrder_Scrap")]
        public ActionResult _ScrapDetailList(string planNo, string flow, string dateIndex, string section)
        {
            ViewBag.PlanNo = planNo;
            ViewBag.Flow = flow;
            ViewBag.DateIndex = dateIndex;
            ViewBag.Section = section;


            ViewData["ScrapType"] = new SelectList(scrapTypeList.Select(p => new { Code = p[0], Name = p[1] }), "Code", "Name");

            //ViewData["ScrapType"] = new SelectList(scrapType.
            if (!string.IsNullOrEmpty(planNo))
            {
            }
            else if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(dateIndex))
            {
            }
            else
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputPlanOrderNumberOrProductionLineWeek);
            }

            return PartialView();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ExProductionOrder_Scrap")]
        public JsonResult _CreateScrapOrder(string[] planNos, string[] scrapTypes, double[] qtys)
        {
            try
            {
                IList<MrpExOrder> mrpExOrderList = new List<MrpExOrder>();
                if (planNos != null && planNos.Length > 0)
                {
                    for (int i = 0; i < planNos.Length; i++)
                    {
                        if (qtys[i] > 0)
                        {
                            MrpExOrder mrpExOrder = this.genericMgr.FindById<MrpExOrder>(planNos[i]);
                            mrpExOrder.CurrentQty = qtys[i];
                            mrpExOrder.ScrapType = scrapTypeList.Single(p => p[1] == scrapTypes[i])[0];
                            mrpExOrderList.Add(mrpExOrder);
                            if (string.IsNullOrWhiteSpace(scrapTypes[i]))
                            {
                                throw new BusinessException(Resources.EXT.ControllerLan.Con_ScraptTypeCanNotBeEmpty);
                            }
                        }
                    }
                }
                //var orderMasters = mrpOrderMgr.CreateExScrapOrder(mrpExOrderList);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ScraptCreatedSuccessfully);
                return Json("");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_ExProductionOrder_Scrap")]
        public ActionResult _SelectScraps(string planNo, string flow, string dateIndex, string section)
        {
            IList<MrpExOrder> mrpExOrderList = new List<MrpExOrder>();

            if (!string.IsNullOrEmpty(planNo))
            {
                mrpExOrderList = this.genericMgr.FindAll<MrpExOrder>
                 ("from MrpExOrder where PlanNo =? ", new object[] { planNo });
                if (mrpExOrderList.Count == 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CanNotFindTheSection, planNo);
                }
                else
                {
                    if (mrpExOrderList.First().Status == Sconit.CodeMaster.OrderStatus.Create)
                    {
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheSectionNotYetStartToProductionCanNotBeScrapted, planNo);
                        mrpExOrderList = new List<MrpExOrder>();
                    }
                }
            }
            else if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(dateIndex))
            {
                string hql = "from MrpExOrder where Status in(?,?,?,?) and ProductLine =? and DateIndex =? ";
                List<object> paramList = new List<object> {
                     com.Sconit.CodeMaster.OrderStatus.Submit,
                     com.Sconit.CodeMaster.OrderStatus.InProcess,
                     com.Sconit.CodeMaster.OrderStatus.Complete,
                     com.Sconit.CodeMaster.OrderStatus.Close,
                     flow, 
                     dateIndex 
                };
                if (!string.IsNullOrEmpty(section))
                {
                    hql += " and Section = ? ";
                    paramList.Add(section);
                }
                mrpExOrderList = this.genericMgr.FindAll<MrpExOrder>(hql, paramList.ToArray());
            }
            else
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseInputPlanOrderNumberOrProductionLineWeek);
            }

            foreach (var orderDetail in mrpExOrderList)
            {
                orderDetail.SectionDescription = this.genericMgr.FindById<Item>(orderDetail.Section).Description;
            }
            FillCodeDetailDescription(mrpExOrderList);
            return PartialView(new GridModel(mrpExOrderList));
        }

        #endregion

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Receive")]
        public JsonResult ReceiveProductionOrder(int[] Ids, decimal[] CurrentReceiveQtys, decimal[] CurrentScrapQtys)
        {
            try
            {
                if (Ids == null || Ids.Length == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_DetailCanNotBeEmpty);
                }
                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                for (int i = 0; i < Ids.Length; i++)
                {
                    OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(Ids[i]);
                    OrderDetailInput input = new OrderDetailInput();
                    input.ReceiveQty = CurrentReceiveQtys[i];
                    input.ScrapQty = CurrentScrapQtys[i];
                    orderDetail.AddOrderDetailInput(input);
                    orderDetailList.Add(orderDetail);
                }
                var orderMaster = this.genericMgr.FindById<OrderMaster>(orderDetailList[0].OrderNo);
                orderMaster.IsReceiveScanHu = false;
                orderMaster.IsShipScanHu = false;
                orderMaster.CreateHuOption = Sconit.CodeMaster.CreateHuOption.None;
                this.genericMgr.Update(orderMaster);
                this.genericMgr.FlushSession();
                ReceiptMaster receiptMaster = orderMgr.ReceiveOrder(orderDetailList);
                SaveSuccessMessage(string.Format(Resources.ORD.OrderMaster.OrderMaster_Received, orderMaster.OrderNo));
                return Json(new { OrderNo = orderMaster.OrderNo });
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        public ActionResult _WebOrderDetail(string flow, string itemCode, int? orderSubType)
        {
            WebOrderDetail webOrderDetail = new WebOrderDetail();
            bool hasFlowDetail = false;
            FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(flow);
            if (!string.IsNullOrEmpty(flow) && !string.IsNullOrEmpty(itemCode))
            {
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
                    webOrderDetail.LocationFrom = flowDetail.DefaultLocationFrom;
                    webOrderDetail.LocationTo = flowDetail.DefaultLocationTo;
                    hasFlowDetail = true;
                }
            }
            if (!hasFlowDetail)
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

        //public ActionResult _WebOrderDetail(string itemCode)
        //{
        //    if (!string.IsNullOrEmpty(itemCode))
        //    {
        //        WebOrderDetail webOrderDetail = new WebOrderDetail();
        //        Item item = genericMgr.FindById<Item>(itemCode);
        //        if (item != null)
        //        {
        //            webOrderDetail.Item = item.Code;
        //            webOrderDetail.ItemDescription = item.Description;
        //            webOrderDetail.UnitCount = item.UnitCount;
        //            webOrderDetail.Uom = item.Uom;
        //            webOrderDetail.MinUnitCount = item.UnitCount;
        //            webOrderDetail.ReferenceItemCode = item.ReferenceCode;
        //        }

        //        return this.Json(webOrderDetail);
        //    }
        //    return null;
        //}

    }
}
