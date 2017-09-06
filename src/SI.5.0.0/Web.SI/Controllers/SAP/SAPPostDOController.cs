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

namespace com.Sconit.Web.Controllers.SI.SAP
{
    public class SAPPostDOController : WebAppBaseController
    {
        //
        // GET: /SequenceOrder/

        private static string selectCountStatement = "select count(*) from PostDO as p";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select p from PostDO as p";



        public SAPPostDOController()
        {

        }
        //public IQueryMgr siMgr { get { return GetService<IQueryMgr>("siMgr"); } }
        [SconitAuthorize(Permissions = "Url_SI_SAP_PostDO_View")]
        public ActionResult Index()
        {
            return View();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_SI_SAP_PostDO_View")]
        public ActionResult List(GridCommand command, PostDOSearchModel searchModel)
        {
            TempData["PostDOSearchModel"] = searchModel;
            ViewBag.PageSize = 20;
            return View();
        }

      

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SI_SAP_PostDO_View")]
        public ActionResult _AjaxList(GridCommand command, PostDOSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<PostDO> gridlist = GetAjaxPageData<PostDO>(searchStatementModel, command);
            foreach (PostDO inv in gridlist.Data)
            {
                inv.StatusName = GetEnumDescription(inv.Status);
            }
          
            return PartialView(gridlist);
            //return PartialView(GetAjaxPageData<ReceiptMaster>(searchStatementModel, command));
        }




        private SearchStatementModel PrepareSearchStatement(GridCommand command, PostDOSearchModel searchModel)
        {
            string whereStatement = "";

            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("OrderNo", searchModel.OrderNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ReceiptNo", searchModel.ReceiptNo, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "p", ref whereStatement, param);
          

            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "p", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "p", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "p", ref whereStatement, param);
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
        public static string GetEnumDescription(object enumSubitem)
        {
            enumSubitem = (Enum)enumSubitem;
            string strValue = enumSubitem.ToString();

            FieldInfo fieldinfo = enumSubitem.GetType().GetField(strValue);

            if (fieldinfo != null)
            {

                Object[] objs = fieldinfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (objs == null || objs.Length == 0)
                {
                    return strValue;
                }
                else
                {
                    DescriptionAttribute da = (DescriptionAttribute)objs[0];
                    return da.Description;
                }
            }
            else
            {
                return "未找到的状态";
            }

        }
    }
}
