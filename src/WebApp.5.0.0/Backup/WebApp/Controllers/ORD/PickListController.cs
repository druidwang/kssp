/// <summary>
/// Summary description for PickListController
/// </summary>
namespace com.Sconit.Web.Controllers.ORD
{
    #region reference
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using NHibernate.Criterion;
    using System;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Utility.Report;
    using AutoMapper;
    using com.Sconit.PrintModel.ORD;
    using System.Text;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.MRP.MD;
    #endregion

    /// <summary>
    /// This controller response to control the PickList.
    /// </summary>
    public class PickListController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the PickList security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //public IReportGen reportGen { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the PickList
        /// </summary>
        private static string selectCountStatement = "select count(*) from PickListMaster as p";

        /// <summary>
        /// hql to get all of the PickList
        /// </summary>
        private static string selectStatement = "select p from PickListMaster as p";


        private static string selectOrderCountStatement = "select count(*) from OrderMaster as o";

        private static string selectOrderStatement = "select o from OrderMaster as o";

        //public IGenericMgr genericMgr { get; set; }

        public IPickListMgr pickListMgr { get; set; }

        //public IOrderMgr orderMgr { get; set; }

        public IIpMgr IpMgr { get; set; }
        #region public actions
        /// <summary>
        /// Index action for PickList controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_PickList_View")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_PickListDetail_View")]
        public ActionResult DetailIndex()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_PickList_Ship")]
        public ActionResult ShipIndex()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _PickDetailList(string PickListNo)
        {
            IList<PickListDetail> pickDetailList = new List<PickListDetail>();
            try
            {
                ViewBag.PickListNo = PickListNo;
                string Message = Resources.EXT.ControllerLan.Con_PickOrderNumberNotExits;

                IList<PickListMaster> pickListMasterList = this.genericMgr.FindAll<PickListMaster>("from PickListMaster as p where p.PickListNo=?", PickListNo);

                if (pickListMasterList.Count > 0)
                {
                    if (pickListMasterList[0].Status != com.Sconit.CodeMaster.PickListStatus.InProcess)
                    {
                        Message = string.Format(Resources.EXT.ControllerLan.Con_OnlyExecuteStatusPickOrderCanBeShipped);
                        throw new BusinessException(Message);
                    }
                    pickDetailList = this.genericMgr.FindAll<PickListDetail>("from PickListDetail as d where d.PickListNo=?", PickListNo);
                    ViewBag.Message = string.Empty;
                }
                else
                {
                    throw new BusinessException(Message);
                }
            }
            catch (Exception be)
            {
                SaveErrorMessage(be);
            }
            return View(pickDetailList);
        }

        public void PickListship(string idStr, string qtyStr)
        {
            try
            {
                string[] idArray = idStr.Split(',');
                string[] CurrentPickQtyArray = qtyStr.Split(',');

                IList<object> para = new List<object>();
                string hql = string.Empty;
                int i = 0;
                foreach (string id in idArray)
                {
                    if (hql == string.Empty)
                    {
                        hql = "from PickListDetail as d where d.Id in (?";
                    }
                    else
                    {
                        hql += ",?";
                    }
                    para.Add(id);
                }
                hql += ")";
                IList<PickListDetail> pickListDetailList = this.genericMgr.FindAll<PickListDetail>(hql, para.ToArray());

                if (pickListDetailList != null && pickListDetailList.Count > 0)
                {
                    foreach (PickListDetail pickListDetail in pickListDetailList)
                    {
                        for (int j = 0; j < idArray.Length; j++)
                        {
                            if (pickListDetail.Id.ToString() == idArray[j])
                            {
                                pickListDetail.CurrentPickQty = Convert.ToDecimal(CurrentPickQtyArray[i]);
                            }
                        }

                    }
                }


            }
            catch (Exception be)
            {
                throw;
            }
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_View")]
        public ActionResult List(GridCommand command, PickListSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_PickList_Ship")]
        public ActionResult ShipList(GridCommand command, PickListSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_View")]
        public ActionResult _AjaxList(GridCommand command, PickListSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<PickListMaster>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_Ship")]
        public ActionResult _AjaxShipList(GridCommand command, PickListSearchModel searchModel)
        {
            string replaceFrom = "_AjaxShipList";
            string replaceTo = "ShipList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.PrepareShipSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<PickListMaster>(searchStatementModel, command));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_PickList_View")]
        public ActionResult _PickListDetailHierarchyAjax(string pickListNo)
        {
            string hql = "select d from PickListDetail as d where d.PickListNo = ?";
            IList<PickListDetail> pickListDetailList = genericMgr.FindAll<PickListDetail>(hql, pickListNo);
            return View(new GridModel(pickListDetailList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_PickList_View")]
        public ActionResult _PickListResultHierarchyAjax(int PickListDetailId)
        {
            string hql = "select d from PickListResult as d where d.PickListDetailId = ?";
            IList<PickListResult> pickListResultList = genericMgr.FindAll<PickListResult>(hql, PickListDetailId);
            return View(new GridModel(pickListResultList));
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_PickList_View")]
        public ActionResult Edit(string id)
        {

            PickListMaster pickListMaster = this.genericMgr.FindById<PickListMaster>(id);
            ViewBag.status = pickListMaster.Status;
            return View(pickListMaster);

        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_PickList_Ship")]
        public ActionResult ShipEdit(string id)
        {
            PickListMaster pickListMaster = this.genericMgr.FindById<PickListMaster>(id);
            ViewBag.status = pickListMaster.Status;
            return View(pickListMaster);
        }


        [SconitAuthorize(Permissions = "Url_PickList_Start")]
        public ActionResult Start(string id)
        {
            try
            {
                pickListMgr.StartPickList(id);

                SaveSuccessMessage(Resources.ORD.PickListMaster.PickListMaster_Started);
                return RedirectToAction("Edit/" + id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_PickList_Cancel")]
        public ActionResult Cancel(string id)
        {
            try
            {
                pickListMgr.CancelPickList(id);
                SaveSuccessMessage(Resources.ORD.PickListMaster.PickListMaster_Canceled);
                return RedirectToAction("Edit/" + id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return RedirectToAction("Edit/" + id);
            }

        }

        [SconitAuthorize(Permissions = "Url_PickList_Ship")]
        public ActionResult Ship(string id)
        {
            try
            {
                PickListMaster pickListMaster = genericMgr.FindById<PickListMaster>(id);
                orderMgr.ShipPickList(pickListMaster);
                SaveSuccessMessage(Resources.ORD.PickListMaster.PickListMaster_Shipped);
                return RedirectToAction("ShipEdit/" + id);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return RedirectToAction("ShipEdit/" + id);
            }

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_View")]
        public ActionResult PickListDetail(string pickListNo)
        {
            ViewBag.pickListNo = pickListNo;
            var pickListMaster = this.genericMgr.FindById<PickListMaster>(pickListNo);
            ViewBag.Status = pickListMaster.Status;
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_Ship")]
        public ActionResult ShipPickListDetail(string pickListNo)
        {
            ViewBag.pickListNo = pickListNo;
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_PickList_New")]
        public ActionResult NewDetailIndex()
        {
            TempData["OrderMasterSearchModel"] = null;
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_PickList_New")]
        public ActionResult DetailNew(GridCommand command, OrderMasterSearchModel searchModel, string PageSize)
        {
            if (TempData["OrderMasterSearchModel"] == null)
            {
                TempData["OrderMasterSearchModel"] = searchModel;
            }
            if (this.CheckSearchModelIsNull(searchModel))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_PleaseMustInputOneSearchConditionExceptRowNumber);
            }
            this.ProcessPageSize(command.PageSize);
            ViewBag.PageSize = (PageSize != null && PageSize != "") ? int.Parse(PageSize) : 20;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_New")]
        public ActionResult _AjaxNewDetail(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
            SearchStatementModel searchStatementModel = PrepareOrderDetailSearchStatement(command, searchModel);
            GridModel<OrderDetail> List = GetAjaxPageData<OrderDetail>(searchStatementModel, command);
            IList<OrderDetail> orderDetailList = List.Data.Where(o => o.CurrentPickListQty > 0).ToList();
            if (orderDetailList != null && orderDetailList.Count > 0)
            {
                var locations = orderDetailList.Select(p => p.LocationFrom).Distinct();
                var items = orderDetailList.Select(p => p.Item).Distinct().ToList();

                var paraList = new List<object>();
                paraList.AddRange(locations);
                paraList.AddRange(items);
                var sql = string.Empty;
                foreach (var location in locations)
                {
                    if (sql == string.Empty)
                    {
                        sql = @"select Location,Item,SUM(qty) from VIEW_LocationLotDet where LotNo is not null
                    and QualityType =0 and IsFreeze = 0 and IsATP = 1 and OccupyType = 0
                    and Location in(? ";
                    }
                    else
                    {
                        sql += ",?";
                    }
                }
                sql += ")";
                foreach (var item in items)
                {
                    if (items.IndexOf(item) == 0)
                    {
                        sql += " and Item in(? ";
                    }
                    else
                    {
                        sql += ",? ";
                    }
                }
                sql += " ) group by Location,Item ";

                var invs = this.genericMgr.FindAllWithNativeSql<object[]>(sql, paraList.ToArray())
                    .Select(p => new { Location = (string)p[0], Item = (string)p[1], Qty = (decimal)p[2] });

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);
                    //IList<LocationLotDetail> locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>
                    //    ("USP_Busi_GetPlusInventory",
                    //    new Object[] {
                    //        orderDetail.LocationFrom, orderDetail.Item,
                    //        com.Sconit.CodeMaster.QualityType.Qualified, 
                    //        com.Sconit.CodeMaster.OccupyType.None, 
                    //        false 
                    //    });

                    orderDetail.WindowTime = orderMaster.WindowTime;
                    orderDetail.LocationQty = invs.Where(p => p.Location == orderDetail.LocationFrom && p.Item == orderDetail.Item).Sum(p => p.Qty);
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
                }
            }
            GridModel<OrderDetail> gridList = new GridModel<OrderDetail>();
            gridList.Data = orderDetailList;
            gridList.Total = List.Total;
            // return PartialView(List);
            return PartialView(gridList);
        }

        #region  Old New PickList
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_New")]
        public ActionResult NewIndex()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_New")]
        public ActionResult New(GridCommand command, OrderMasterSearchModel searchModel)
        {
            TempData["OrderMasterSearchModel"] = searchModel;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_New")]
        public ActionResult _AjaxNewOrderList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PreparePickListSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_New")]
        public ActionResult NewEdit(string checkedOrders)
        {
            string[] checkedOrderArray = checkedOrders.Split(',');
            string selectStatement = string.Empty;
            IList<object> selectPartyPara = new List<object>();
            foreach (var para in checkedOrderArray)
            {
                if (selectStatement == string.Empty)
                {
                    selectStatement = "from OrderMaster where OrderNo in (?";
                }
                else
                {
                    selectStatement += ",?";
                }
                selectPartyPara.Add(para);
            }
            selectStatement += ")";

            IList<OrderMaster> Lists = genericMgr.FindAll<OrderMaster>(selectStatement, selectPartyPara.ToArray());
            IpMaster order = null;
            try
            {
                order = IpMgr.MergeOrderMaster2IpMaster(Lists);
            }
            catch (BusinessException ex)
            {

                SaveWarningMessage(ex.GetMessages()[0].GetMessageString());
            }
            return View(order);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_New")]
        public ActionResult _NewOrderDetailList(string checkedOrders)
        {
            string[] checkedOrderArray = checkedOrders.Split(',');
            DetachedCriteria criteria = DetachedCriteria.For<OrderDetail>();
            criteria.Add(Expression.In("OrderNo", checkedOrderArray));
            criteria.Add(Expression.GtProperty("OrderedQty", "PickedQty"));

            IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>(criteria);
            return PartialView(orderDetailList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_PickList_New")]
        public ActionResult CreatePickList(string idStr, string qtyStr, OrderMasterSearchModel searchModel)
        {
            try
            {
                if (string.IsNullOrEmpty(idStr))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_PickDetailCanNotBeEmpty);
                }
                string[] idArr = idStr.Split(',');
                string[] qtyArr = qtyStr.Split(',');

                IList<OrderDetail> orderDetailList = new List<OrderDetail>();
                for (int i = 0; i < idArr.Count(); i++)
                {

                    OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(Convert.ToInt32(idArr[i]));
                    OrderDetailInput orderDetailInput = new OrderDetailInput();
                    orderDetailInput.PickQty = Convert.ToDecimal(qtyArr[i]);
                    orderDetail.AddOrderDetailInput(orderDetailInput);
                    //校验还没发
                    orderDetailList.Add(orderDetail);
                }

                PickListMaster pickListMaster = pickListMgr.CreatePickList(orderDetailList);
                SaveSuccessMessage(Resources.ORD.PickListMaster.PickListMaster_Created);
                return RedirectToAction("Edit/" + pickListMaster.PickListNo);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                TempData["OrderMasterSearchModel"] = searchModel;
                return RedirectToAction("DetailNew");

            }

        }

        #endregion


        public ActionResult DeletePickListResult(int[] checkedResults, string pickListNo)
        {
            try
            {
                if (checkedResults == null || checkedResults.Count() == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_PleaseFirstlyChooseOneDetail);
                }
                DetachedCriteria criteria = DetachedCriteria.For<PickListResult>();
                criteria.Add(Expression.In("Id", checkedResults.ToArray()));
                IList<PickListResult> pickListResultList = genericMgr.FindAll<PickListResult>(criteria);
                pickListMgr.DeletePickListResult(pickListResultList);
                SaveSuccessMessage(Resources.ORD.PickListMaster.PickListMaster_PickListResultDeleted);
                return RedirectToAction("Edit/" + pickListNo);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return RedirectToAction("Edit/" + pickListNo);
            }
        }

        #region 打印导出
        public void SaveToClient(string pickListNo)
        {
            PickListMaster pickListMaster = queryMgr.FindById<PickListMaster>(pickListNo);
            IList<PickListDetail> pickListDetails = queryMgr.FindAll<PickListDetail>("select pl from PickListDetail as pl where pl.PickListNo=?", pickListNo);
            pickListMaster.PickListDetails = pickListDetails;
            PrintPickListMaster printPickListMaster = Mapper.Map<PickListMaster, PrintPickListMaster>(pickListMaster);
            IList<object> data = new List<object>();
            data.Add(printPickListMaster);
            data.Add(printPickListMaster.PickListDetails);
            reportGen.WriteToClient("PickList.xls", data, pickListMaster.PickListNo + ".xls");

        }

        public string Print(string pickListNo)
        {
            PickListMaster pickListMaster = queryMgr.FindById<PickListMaster>(pickListNo);
            IList<PickListDetail> pickListDetails = queryMgr.FindAll<PickListDetail>("select pl from PickListDetail as pl where pl.PickListNo=?", pickListNo);
            pickListMaster.PickListDetails = pickListDetails;
            PrintPickListMaster printPickListMaster = Mapper.Map<PickListMaster, PrintPickListMaster>(pickListMaster);
            IList<object> data = new List<object>();
            data.Add(printPickListMaster);
            data.Add(printPickListMaster.PickListDetails);
            return reportGen.WriteToFile("PickList.xls", data);
        }
        #endregion

        #endregion

        #region private actions
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PickListSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "pl", "OrderType", "pl", "PartyFrom", "pl", "PartyTo", com.Sconit.CodeMaster.OrderType.Distribution, false);
            // It should be logic "and" between  checking pemission hql commands  and other search condition(PickListNo,Status...etc)
            //whereStatement = whereStatement.Insert(6, " (") + ")";
            HqlStatementHelper.AddLikeStatement("PickListNo", searchModel.PickListNo, HqlStatementHelper.LikeMatchMode.End, "pl", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("IpNo", searchModel.IpNo, HqlStatementHelper.LikeMatchMode.End, "pl", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "pl", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "pl", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "pl", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "pl", ref whereStatement, param);

            if (searchModel.StartTime != null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartTime, searchModel.EndTime, "pl", ref whereStatement, param);
            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartTime, "pl", ref whereStatement, param);
            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndTime, "pl", ref whereStatement, param);
            }

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "OrderTypeDescription")
                {
                    command.SortDescriptors[0].Member = "OrderType";
                }
                else if (command.SortDescriptors[0].Member == "OrderStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by pl.CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = "select count(*) from PickListMaster as pl";
            searchStatementModel.SelectStatement = "select pl from PickListMaster as pl";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel PrepareShipSearchStatement(GridCommand command, PickListSearchModel searchModel)
        {
            string whereStatement = " where p.Status =" + (int)com.Sconit.CodeMaster.PickListStatus.InProcess;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("PickListNo", searchModel.PickListNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "p", ref whereStatement, param);

            if (searchModel.StartTime != null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartTime, searchModel.EndTime, "p", ref whereStatement, param);
            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartTime, "p", ref whereStatement, param);
            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndTime, "p", ref whereStatement, param);
            }

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "OrderTypeDescription")
                {
                    command.SortDescriptors[0].Member = "OrderType";
                }
                else if (command.SortDescriptors[0].Member == "OrderStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by p.CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel PreparePickListSearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            if (searchModel.Type.HasValue && searchModel.Type > 0)
            {
                whereStatement = " where o.Type =" + searchModel.Type
                    + " and o.IsShipScanHu = 1 and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + "," + (int)com.Sconit.CodeMaster.OrderStatus.Submit + ")"
                    + " and exists (select 1 from OrderDetail as d where d.PickedQty < d.OrderedQty and d.OrderNo = o.OrderNo) ";
            }
            else
            {
                whereStatement = " where o.Type in (" + (int)com.Sconit.CodeMaster.OrderType.Distribution + ","
                   + (int)com.Sconit.CodeMaster.OrderType.Transfer + ","
                   + (int)com.Sconit.CodeMaster.OrderType.SubContractTransfer + ")"
                   + " and o.IsShipScanHu = 1 and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + "," + (int)com.Sconit.CodeMaster.OrderStatus.Submit + ")"
                   + " and exists (select 1 from OrderDetail as d where d.PickedQty < d.OrderedQty and d.OrderNo = o.OrderNo) ";
            }
            IList<object> param = new List<object>();
            if (!string.IsNullOrEmpty(searchModel.OrderNo))
            {
                HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
            }
            if (!string.IsNullOrEmpty(searchModel.Flow))
            {
                HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "o", ref whereStatement, param);
            }
            if (!string.IsNullOrEmpty(searchModel.PartyFrom))
            {
                HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "o", ref whereStatement, param);
            }
            if (!string.IsNullOrEmpty(searchModel.PartyTo))
            {
                HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "o", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by o.CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectOrderCountStatement;
            searchStatementModel.SelectStatement = selectOrderStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        #endregion
        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderDetail_Distribution")]
        public ActionResult DetailList(GridCommand command, PickListSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";

                IList<PickListDetail> list = genericMgr.FindAll<PickListDetail>(PrepareSearchDetailStatement(command, searchModel)); //GetPageData<OrderDetail>(searchStatementModel, command);

                int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
                if (list.Count > value)
                {
                    SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_DataExceedRow, value));
                }
                return View(list.Take(value));
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
                return View(new List<PickListDetail>());
            }
        }
        #region  Export  pickList detail search
        [SconitAuthorize(Permissions = "Url_OrderDetail_Distribution")]
        [GridAction(EnableCustomBinding = true)]
        public void Export(PickListSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchCacheModel.SearchObject) ? 0 : value;

            TempData["_AjaxMessage"] = "";
            IList<PickListDetail> lists = genericMgr.FindAll<PickListDetail>(PrepareSearchDetailStatement(command, searchModel)); 
            if (lists.Count > value)
            {
                SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_DataExceedRow, value));
            }
            var listItemCount = lists.GroupBy(p => p.Item, (k, g) => new { k, g })
                .ToDictionary(d => d.k, d => d.g.Count());
            foreach (var list in lists)
            {
                list.ItemCount = listItemCount.Where(p => p.Key == list.Item).FirstOrDefault().Value;
            }
            lists = lists.OrderBy(o => o.Item).ToList();
            ExportToXLS<PickListDetail>("PickListDetail.xls", lists);
        }
        #endregion
        private string PrepareSearchDetailStatement(GridCommand command, PickListSearchModel searchModel)
        {
            StringBuilder Sb = new StringBuilder();
            string whereStatement = " select  d from PickListDetail as d  where exists (select 1 from PickListMaster  as o"
                                     + " where o.PickListNo=d.PickListNo ";

            Sb.Append(whereStatement);

            if (searchModel.Flow != null)
            {
                Sb.Append(string.Format(" and o.Flow = '{0}'", searchModel.Flow));
            }

            if (searchModel.Status != null)
            {
                Sb.Append(string.Format(" and o.Status = '{0}'", searchModel.Status));
            }

            if (!string.IsNullOrEmpty(searchModel.PickListNo))
            {
                Sb.Append(string.Format(" and o.PickListNo like '%{0}%'", searchModel.PickListNo));
            }
            if (!string.IsNullOrEmpty(searchModel.PartyFrom))
            {
                Sb.Append(string.Format(" and o.PartyFrom = '{0}'", searchModel.PartyFrom));
            }
            if (!string.IsNullOrEmpty(searchModel.PartyTo))
            {
                Sb.Append(string.Format(" and o.PartyTo = '{0}'", searchModel.PartyTo));
            }

            string str = Sb.ToString();
            //SecurityHelper.AddPartyFromPermissionStatement(ref str, "o", "PartyFrom", com.Sconit.CodeMaster.OrderType.Procurement, true);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref str, "o", "Type", "o", "PartyFrom", "o", "PartyTo", com.Sconit.CodeMaster.OrderType.Procurement, true);
            if (searchModel.StartTime != null & searchModel.EndTime != null)
            {
                Sb.Append(string.Format(" and o.CreateDate between '{0}' and '{1}'", searchModel.StartTime, searchModel.EndTime));

            }
            else if (searchModel.StartTime != null & searchModel.EndTime == null)
            {
                Sb.Append(string.Format(" and o.CreateDate >= '{0}'", searchModel.StartTime));

            }
            else if (searchModel.StartTime == null & searchModel.EndTime != null)
            {
                Sb.Append(string.Format(" and o.CreateDate <= '{0}'", searchModel.EndTime));

            }

            Sb.Append(" )");

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                Sb.Append(string.Format(" and  d.Item like '{0}%' ", searchModel.Item));
            }

            if (searchModel.Type.HasValue && searchModel.Type > 0)
            {
                Sb.Append(string.Format(" and  d.OrderType ='{0}' ", searchModel.Type));
            }

            return Sb.ToString();
        }

        private SearchStatementModel PrepareOrderDetailSearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            IList<object> param = new List<object>();
            StringBuilder Sb = new StringBuilder();
            string whereStatement = " where exists (select 1 from OrderMaster  as o where o.OrderNo=d.OrderNo ";

            Sb.Append(whereStatement);

            if (searchModel.Flow != null)
            {
                Sb.Append(string.Format(" and o.Flow = '{0}'", searchModel.Flow));
            }

            if (searchModel.OrderNo != null)
            {
                Sb.Append(string.Format(" and o.OrderNo like '%{0}'", searchModel.OrderNo));
            }
            if (!string.IsNullOrEmpty(searchModel.PartyFrom))
            {
                Sb.Append(string.Format(" and o.PartyFrom = '{0}'", searchModel.PartyFrom));

            }
            if (!string.IsNullOrEmpty(searchModel.PartyTo))
            {
                Sb.Append(string.Format(" and o.PartyTo = '{0}'", searchModel.PartyTo));

            }
            if (searchModel.Type.HasValue && searchModel.Type > 0)
            {
                Sb.Append(string.Format(" and  d.OrderType ='{0}' ", searchModel.Type));
            }

            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                Sb.Append(string.Format(" and o.CreateDate between '{0}' and '{1}'", searchModel.DateFrom, searchModel.DateTo));

            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                Sb.Append(string.Format(" and o.CreateDate >= '{0}'", searchModel.DateTo));

            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                Sb.Append(string.Format(" and o.CreateDate <= '{0}'", searchModel.DateTo));
            }
            //满足条件1.移库或销售类型订单;2.订单数-已发数-拣货数〉0;3.排序单和分装生产单不能创建拣货单
            Sb.Append(" and o.Type in (2,3,7) and (OrderQty-ShipQty-PickQty)>0  ");
            Sb.Append("and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.Submit + "," + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + "))");
            whereStatement = Sb.ToString();
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "d", ref whereStatement, param);
            if (searchModel.LocationFrom != null && searchModel.LocationFromTo == null)
            {
                HqlStatementHelper.AddEqStatement("LocationFrom", searchModel.LocationFrom, "d", ref whereStatement, param);
            }
            else if (searchModel.LocationFrom != null && searchModel.LocationFromTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("LocationFrom", searchModel.LocationFrom, searchModel.LocationFromTo, "d", ref whereStatement, param);
            }

            if (searchModel.LocationTo != null && searchModel.LocationToTo == null)
            {
                HqlStatementHelper.AddEqStatement("LocationTo", searchModel.LocationTo, "d", ref whereStatement, param);
            }
            else if (searchModel.LocationTo != null && searchModel.LocationToTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("LocationTo", searchModel.LocationTo, searchModel.LocationToTo, "d", ref whereStatement, param);
            }

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "WindowTime")
                {
                    command.SortDescriptors.Remove(command.SortDescriptors[0]);
                    // command.SortDescriptors[0].Member = "Type";
                }
                else if (command.SortDescriptors[0].Member == "CurrentPickListQty")
                {
                    command.SortDescriptors.Remove(command.SortDescriptors[0]);
                }
                else if (command.SortDescriptors[0].Member == "CurrentPickQty")
                {
                    command.SortDescriptors.Remove(command.SortDescriptors[0]);
                }
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by d.CreateDate desc";
            }


            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = "select count(*) from OrderDetail as d";
            searchStatementModel.SelectStatement = "select d from OrderDetail as d";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #region  Export master search
        [SconitAuthorize(Permissions = "Url_PickList_View")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportMstr(PickListSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return ;
            }
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            ExportToXLS<PickListMaster>("PickListMaster.xls", GetAjaxPageData<PickListMaster>(searchStatementModel, command).Data.ToList());
        }
        #endregion
    }
}

