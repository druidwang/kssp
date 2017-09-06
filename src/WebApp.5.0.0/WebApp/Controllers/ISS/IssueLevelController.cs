


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

    public class IssueLevelController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IssueLevel as il";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select il from IssueLevel as il";

        /// <summary>
        /// 
        /// </summary>
        private static string codeDuiplicateVerifyStatement = @"select count(*) from IssueLevel as il where il.Code = ?";


        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //
        // GET: /IssueLevel/

        [SconitAuthorize(Permissions = "Url_IssueLevel_View")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_IssueLevel_View")]
        public ActionResult List(GridCommand command, IssueLevelSearchModel searchModel)
        {
            TempData["IssueLevelSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_IssueLevel_View")]
        public ActionResult _AjaxList(GridCommand command, IssueLevelSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IssueLevel>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_IssueLevel_Edit")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueLevel_Edit")]
        public ActionResult New(IssueLevel issueLevel)
        {
            if (ModelState.IsValid)
            {
                //判断用户名不能重复
                if (this.genericMgr.FindAll<long>(codeDuiplicateVerifyStatement, new object[] { issueLevel.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.ISS.IssueLevel.Errors_Existing_IssueLevel, issueLevel.Code);
                }
                else
                {
                    genericMgr.CreateWithTrim(issueLevel);
                    SaveSuccessMessage(Resources.ISS.IssueLevel.IssueLevel_Added);
                    return RedirectToAction("Edit/" + issueLevel.Code);
                }
            }
            return View(issueLevel);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_IssueLevel_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueLevel issueLevel = this.genericMgr.FindById<IssueLevel>(id);
                return View(issueLevel);
            }
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">IssueLevel id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_IssueLevel_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<IssueLevel>(id);
                SaveSuccessMessage(Resources.ISS.IssueLevel.IssueLevel_Deleted);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueLevel_Edit")]
        public ActionResult Edit(IssueLevel issueLevel)
        {
            if (ModelState.IsValid)
            {
                genericMgr.UpdateWithTrim(issueLevel);
                SaveSuccessMessage(Resources.ISS.IssueLevel.IssueLevel_Updated);
            }

            return View(issueLevel);
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, IssueLevelSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "il", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "il", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "il", ref whereStatement, param);

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
