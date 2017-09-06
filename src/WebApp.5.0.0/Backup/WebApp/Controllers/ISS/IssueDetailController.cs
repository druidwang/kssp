



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

    public class IssueDetailController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IssueDetail as iDet ";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select iDet from IssueDetail as iDet ";

        

        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //
        // GET: /IssueDetail/

        [SconitAuthorize(Permissions = "Url_IssueDetail_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_IssueDetail_View")]
        public ActionResult List(GridCommand command, IssueDetailSearchModel searchModel)
        {
            //SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            //SearchStatementModel searchStatementModel = PrepareSearchStatement(command, (IssueDetailSearchModel)searchCacheModel.SearchObject);
            //return View(GetPageData<IssueDetail>(searchStatementModel, command));
            TempData["IssueDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_IssueDetail_View")]
        public ActionResult _AjaxList(GridCommand command, IssueDetailSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IssueDetail>(searchStatementModel, command));
        }

        
        private SearchStatementModel PrepareSearchStatement(GridCommand command, IssueDetailSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("IssueCode", searchModel.IssueCode, HqlStatementHelper.LikeMatchMode.Start, "iDet", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Email", searchModel.Email, HqlStatementHelper.LikeMatchMode.Start, "iDet", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("MobilePhone", searchModel.MobilePhone, HqlStatementHelper.LikeMatchMode.Start, "iDet", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IssueLevel", searchModel.IssueLevel, "iDet", ref whereStatement, param);
            

            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.DateFrom, searchModel.DateTo, "iDet", ref whereStatement, param);
            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "iDet", ref whereStatement, param);
            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.DateTo, "iDet", ref whereStatement, param);
            }

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "UserName")
                {
                    command.SortDescriptors[0].Member = "User";
                }
            }


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
