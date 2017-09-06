using System.Data;
using System.Web.Mvc;
using com.Sconit.Web.Util;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.SI.SAP;
using System.Data.SqlClient;
using System;
using System.Linq;
using com.Sconit.Web.Models;
using System.Collections.Generic;
using com.Sconit.Entity.SAP.TRANS;
using com.Sconit.Service;
using System.ComponentModel;
using System.Reflection;
using com.Sconit.Entity.SAP.ORD;
using com.Sconit.Entity.CUST;

namespace com.Sconit.Web.Controllers.SI.SAP
{
    public class SAPOpReportController : WebAppBaseController
    {
        //
        // GET: /SequenceOrder/

        public IGenericMgr genericMgr { get; set; }
      
        /// <summary>
        /// 
        /// </summary>




        public SAPOpReportController()
        {

        }
        //public IQueryMgr siMgr { get { return GetService<IQueryMgr>("siMgr"); } }
        [SconitAuthorize(Permissions = "Url_CUST_OpReport_View")]
        public ActionResult Index()
        {
            return View();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CUST_OpReport_View")]
        public ActionResult List(GridCommand command, OpReportSearchModel searchModel)
        {
            TempData["OpReportSearchModel"] = searchModel;
            ViewBag.PageSize = 20;
            return View();
        }

      

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CUST_OpReport_View")]
        public ActionResult _AjaxList(GridCommand command, OpReportSearchModel searchModel)
        {
            string selectStatement = "select o from OperationReport as o where 1=1 ";
             IList<object> param = new List<object>();
             if (!string.IsNullOrEmpty(searchModel.OrderNo))
             {
                 selectStatement += " and o.OrderNo=?";
                 param.Add(searchModel.OrderNo);
             }
             if (!string.IsNullOrEmpty(searchModel.ExtOrderNo))
             {
                 selectStatement += " and o.ExternalOrderNo=?";
                 param.Add(searchModel.ExtOrderNo);
             }
             if (!string.IsNullOrEmpty(searchModel.Operation))
             {
                 selectStatement += " and o.Operation=?";
                 param.Add(searchModel.Operation);
             }
             if (searchModel.StartDate != null & searchModel.EndDate != null)
             {
                 selectStatement += " and o.CreateDate between ? and ?";
                 param.Add(searchModel.StartDate);
                 param.Add(searchModel.EndDate);
             }
             else if (searchModel.StartDate != null & searchModel.EndDate == null)
             {
                 selectStatement += " and o.CreateDate >= ? ";
                 param.Add(searchModel.StartDate);
             }
             else if (searchModel.StartDate == null & searchModel.EndDate != null)
             {
                 selectStatement += " and o.CreateDate <= ? ";
                 param.Add(searchModel.EndDate);
             }
             IList<OperationReport> list = genericMgr.FindAll<OperationReport>(selectStatement, param.ToArray());

             return PartialView(new GridModel(list));
            //return PartialView(GetAjaxPageData<ReceiptMaster>(searchStatementModel, command));
        }

    }
}
