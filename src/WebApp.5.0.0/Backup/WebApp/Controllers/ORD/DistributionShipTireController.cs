using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.ORD;
using com.Sconit.Entity.ORD;
using com.Sconit.Web.Models;
using com.Sconit.Service;
using NHibernate.Criterion;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.Exception;
using AutoMapper;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Utility.Report;
using com.Sconit.Entity.CUST;

namespace com.Sconit.Web.Controllers.ORD
{
    public class DistributionShipTireController : WebAppBaseController
    {

       
        //
        // GET: /DistributionShipTire/
        private static string selectCountStatement = "select count(*) from OrderMaster as o";
        private static string selectStatement = "select o from OrderMaster as o";

        //public IGenericMgr genericMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        public IIpMgr IpMgr { get; set; }
        //public IReportGen reportGen { get; set; }

        #region Public
        #region View
        [SconitAuthorize(Permissions = "Url_ShipTire")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ShipTire")]
        public ActionResult List(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            this.CheckFlow(searchModel.Flow, true);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        private bool CheckFlow(string Flow,bool IsList) {
            if (string.IsNullOrEmpty(Flow))
            {
                if (IsList)
                {
                    SaveWarningMessage(Resources.EXT.ControllerLan.Con_PleaseChooseFlowToSearch);
                }
                return false;
            }
            else {
                IList<ProductLineMap> pmList = this.genericMgr.FindAll<ProductLineMap>("from ProductLineMap as p where p.TireFlow=?", Flow);
                if (pmList != null && pmList.Count > 0)
                {
                    if (IsList)
                    {
                        TempData["_AjaxMessage"] = "";
                    }
                    return true;
                }
                else {
                    if (IsList)
                    {
                        SaveWarningMessage(Resources.EXT.ControllerLan.Con_FlowChooseIsWrongPleaseConfirm);
                    }
                    return false;
                }
            
            }
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            if (!this.CheckFlow(searchModel.Flow,false))
            {
                return PartialView(new GridModel(new List<OrderMaster>()));
            }
            else
            {
                SearchStatementModel searchStatementModel = PrepareShipSearchStatement(command, searchModel);
                return PartialView(GetAjaxPageData<OrderMaster>(searchStatementModel, command));
            }
        }
        #endregion

        #region Edit
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ShipTire")]
        public ActionResult Edit(string OrderNo, string Status)
        {
            ViewBag.OrderNo = OrderNo;
            ViewBag.Status = Status;
            string selectStatement = string.Empty;
            selectStatement = "from OrderMaster where OrderNo =?";

            IList<OrderMaster> Lists = genericMgr.FindAll<OrderMaster>(selectStatement, OrderNo);
            IpMaster order = null;
            try
            {
                order = IpMgr.MergeOrderMaster2IpMaster(Lists);
            }
            catch (Exception ex)
            {

                SaveWarningMessage(ex.Message);
            }
            return View(order);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ShipTire")]
        public ActionResult _ShipOrderDetailList(GridCommand command, string OrderNo,string Status)
        {
            ViewBag.OrderNo = OrderNo;
            ViewBag.Status = Status;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ShipTire")]
        public ActionResult _AjaxShipOrderDetailList(GridCommand command, string OrderNo)
        {
            string[] checkedOrderArray = { OrderNo };
            DetachedCriteria criteria = DetachedCriteria.For<OrderDetail>();
            criteria.Add(Expression.In("OrderNo", checkedOrderArray));
            criteria.Add(Expression.LtProperty("ShippedQty", "OrderedQty"));
            IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>(criteria);

            return PartialView(new GridModel<OrderDetail>(orderDetailList));
        }
        
        public JsonResult SaveOrderDetail(string idStr, string ManufacturePartyStr,string OrderNo)
        {
            string[] idArray = idStr.Split(',');
            string[] ManufacturePartyStrArray = ManufacturePartyStr.Split(',');
             IList<OrderDetail> deleteorderDetails=new List<OrderDetail> ();
             IList<OrderDetail> addorderDetails=new List<OrderDetail> ();
            IList<OrderDetail> updataorderDetails=new List<OrderDetail> ();
            try
            {
                for (int i = 0; i < idArray.Count(); i++)
                {
                    OrderDetail od = genericMgr.FindById<OrderDetail>(Convert.ToInt32(idArray[i]));
                    od.ManufactureParty=ManufacturePartyStrArray[i];
                    updataorderDetails.Add(od);
                }
                IList<OrderDetail> newOrderDetails = orderMgr.UpdateTireOrderDetails(OrderNo, updataorderDetails);
                object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_UpdatedSuccessfully), OrderNo = OrderNo };
                return Json(obj);
                    
            }
            catch (BusinessException ex)
            {
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
           
        }
        #endregion

        #region Print
        public string Print(string orderNo)
        {
            OrderMaster orderMaster = queryMgr.FindById<OrderMaster>(orderNo);
            IList<OrderDetail> orderDetails = queryMgr.FindAll<OrderDetail>("select od from OrderDetail as od where od.OrderNo=?", orderNo);
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

        #endregion

        private SearchStatementModel PrepareShipSearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {

            string whereStatement = " where o.Type =" + (int)com.Sconit.CodeMaster.OrderType.Transfer + ""
                                    + " and o.IsShipScanHu = 0 and o.Status in (" + (int)com.Sconit.CodeMaster.OrderStatus.Submit + "," + (int)com.Sconit.CodeMaster.OrderStatus.InProcess + ")"
                                    + " and o.SubType = " + (int)com.Sconit.CodeMaster.OrderSubType.Normal

                                    + " and exists (select 1 from OrderDetail as d where d.ShippedQty < d.OrderedQty and d.OrderNo = o.OrderNo) ";
            //SecurityHelper.AddPartyFromPermissionStatement(ref whereStatement, "o", "PartyFrom", com.Sconit.CodeMaster.OrderType.Procurement, true);
            SecurityHelper.AddPartyFromAndPartyToPermissionStatement(ref whereStatement, "o", "Type", "o", "PartyFrom", "o", "PartyTo", com.Sconit.CodeMaster.OrderType.Transfer, true);
            IList<object> param = new List<object>();
            //if (!string.IsNullOrEmpty(searchModel.OrderNo))
            //{
                HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Anywhere, "o", ref whereStatement, param);
           // }
           // else if (!string.IsNullOrEmpty(searchModel.Flow))
           // {
                HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "o", ref whereStatement, param);
           // }
           // else if (!string.IsNullOrEmpty(searchModel.PartyFrom) && !string.IsNullOrEmpty(searchModel.PartyTo))
           // {
                HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "o", ref whereStatement, param);
                HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "o", ref whereStatement, param);

           // }
           // else if (!string.IsNullOrEmpty(searchModel.Dock))
           // {
                HqlStatementHelper.AddLikeStatement("Dock", searchModel.Dock, HqlStatementHelper.LikeMatchMode.Anywhere, "o", ref whereStatement, param);
           // }
                HqlStatementHelper.AddEqStatement("TraceCode", searchModel.TraceCode, "o", ref whereStatement, param);
                HqlStatementHelper.AddEqStatement("ExternalOrderNo", searchModel.ExternalOrderNo, "o", ref whereStatement, param);


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

    }
}
