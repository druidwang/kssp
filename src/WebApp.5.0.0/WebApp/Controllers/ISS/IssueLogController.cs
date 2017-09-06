


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

    public class IssueLogController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IssueLog as il join il.User u ";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select il from IssueLog as il join il.User u ";

        

        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //
        // GET: /IssueLog/

        [SconitAuthorize(Permissions = "Url_IssueLog_View")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_IssueLog_View")]
        public ActionResult List(GridCommand command, IssueLogSearchModel searchModel)
        {
            TempData["IssueDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_IssueLog_View")]
        public ActionResult _AjaxList(GridCommand command, IssueLogSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IssueLog>(searchStatementModel, command));
        }

        
        private SearchStatementModel PrepareSearchStatement(GridCommand command, IssueLogSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Issue", searchModel.IssueCode, HqlStatementHelper.LikeMatchMode.Start, "il", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Content", searchModel.Content, HqlStatementHelper.LikeMatchMode.Start, "il", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Level", searchModel.Level, "il", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Code", searchModel.CreateUser, "il", ref whereStatement, param);

            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.DateFrom, searchModel.DateTo, "il", ref whereStatement, param);
            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "il", ref whereStatement, param);
            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.DateTo, "il", ref whereStatement, param);
            }

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "User.Code")
                {
                    command.SortDescriptors[0].Member = "u.Code";
                }
                else if (command.SortDescriptors[0].Member == "User.FullName")
                {
                    command.SortDescriptors[0].Member = "User";
                }
                else if (command.SortDescriptors[0].Member == "Content")
                {
                    command.SortDescriptors[0].Member = "il.Content";
                }
                else if (command.SortDescriptors[0].Member == "Level")
                {
                    command.SortDescriptors[0].Member = "il.Level";
                }
                else if (command.SortDescriptors[0].Member == "Email")
                {
                    command.SortDescriptors[0].Member = "il.Email";
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
