


namespace com.Sconit.Web.Controllers.ISS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ISS;
    using com.Sconit.Web.Controllers.ACC;
    using com.Sconit.Entity.ISS;
    using com.Sconit.Service;
    using com.Sconit.Entity;

    public class SMSStatusController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from SMSStatus as sms";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select sms from SMSStatus as sms";

    

        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //
        // GET: /SMSStatus/

        [SconitAuthorize(Permissions = "Url_SMSStatus_View")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_SMSStatus_View")]
        public ActionResult List(GridCommand command, SMSStatusSearchModel searchModel)
        {
            TempData["SMSStatusSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SMSStatus_View")]
        public ActionResult _AjaxList(GridCommand command, SMSStatusSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<SMSStatus>(searchStatementModel, command));
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, SMSStatusSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Issue", searchModel.Issue, HqlStatementHelper.LikeMatchMode.Start, "sms", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("MsgID", searchModel.MsgID, HqlStatementHelper.LikeMatchMode.Start, "sms", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Content", searchModel.Content, HqlStatementHelper.LikeMatchMode.Start, "sms", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("DestID", searchModel.DestID, HqlStatementHelper.LikeMatchMode.Start, "sms", ref whereStatement, param);
            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.DateFrom, searchModel.DateTo, "sms", ref whereStatement, param);
            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "sms", ref whereStatement, param);
            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.DateTo, "sms", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("EventHandler", SMSStatus.SMSEventHeadler_MESSAGERECEIVEDINTERFACE, "sms", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

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
