namespace com.Sconit.Web.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using com.Sconit.Service;
    using Models;
    using Telerik.Web.Mvc;
    using AutoMapper;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using com.Sconit.Utility;

    public class ReportOrderController : Controller
    {
        public IGenericMgr genericMgr { get; set; }

        public ActionResult Index(OrderMasterSearchModel searchModel)
        {
            TempData["SearchModel"] = searchModel;
            return View();
        }

        [GridAction]
        public ActionResult _OrderMasterHierarchyAjax()
        {
            string hql = " select o from OrderMaster as o ";
            OrderMasterSearchModel searchModel = (OrderMasterSearchModel)TempData["SearchModel"];
            IList<OrderMaster> orderMasterList = new List<OrderMaster>();
            if (searchModel != null)
            {
                IList<object> param = new List<object>();
                string whereStatement = " where 1=1";
                HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
                HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "o", ref whereStatement, param);
                HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "o", ref whereStatement, param);
                HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "o", ref whereStatement, param);
                HqlStatementHelper.AddLikeStatement("CreateUserName", searchModel.CreateUserName, HqlStatementHelper.LikeMatchMode.Start, "o", ref whereStatement, param);
                HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "o", ref whereStatement, param);
                HqlStatementHelper.AddEqStatement("Priority", searchModel.Priority, "o", ref whereStatement, param);
                HqlStatementHelper.AddEqStatement("SubType", searchModel.SubType, "o", ref whereStatement, param);

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
                if (param.Count > 0)
                {
                    hql += whereStatement;
                    orderMasterList = genericMgr.FindAll<OrderMaster>(hql, param);
                }
            }
            orderMasterList = genericMgr.FindAll<OrderMaster>(hql);

            return View(new GridModel(orderMasterList));
        }

        [GridAction]
        public ActionResult _OrdersDetailHierarchyAjax(string orderNo)
        {
            IList<OrderDetail> orderDetailList =
                genericMgr.FindAll<OrderDetail>("select o from OrderDetail as o where o.OrderNo = ?", orderNo);

            return View(new GridModel(orderDetailList));
        }

        [GridAction]
        public ActionResult _OrderBomDetailsHierarchyAjax(int orderDetailId)
        {
            IList<OrderBomDetail> orderDetaiBomlList =
                genericMgr.FindAll<OrderBomDetail>("select o from OrderBomDetail as o where o.OrderDetailId = ?", orderDetailId);

            return View(new GridModel(orderDetaiBomlList));
        }
    }
}
