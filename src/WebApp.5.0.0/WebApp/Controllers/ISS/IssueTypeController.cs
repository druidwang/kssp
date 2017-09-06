


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

    public class IssueTypeController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IssueType as it";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select it from IssueType as it";

        /// <summary>
        /// 
        /// </summary>
        private static string codeDuiplicateVerifyStatement = @"select count(*) from IssueType as it where it.Code = ?";


        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //
        // GET: /IssueType/

        [SconitAuthorize(Permissions = "Url_IssueType_View")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_IssueType_View")]
        public ActionResult List(GridCommand command, IssueTypeSearchModel searchModel)
        {
            TempData["IssueTypeSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_IssueType_View")]
        public ActionResult _AjaxList(GridCommand command, IssueTypeSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IssueType>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_IssueType_Edit")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueType_Edit")]
        public ActionResult New(IssueType issueType)
        {
            if (ModelState.IsValid)
            {
                //判断用户名不能重复
                if (this.genericMgr.FindAll<long>(codeDuiplicateVerifyStatement, new object[] { issueType.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.ISS.IssueType.Errors_Existing_IssueType, issueType.Code);
                }
                else
                {
                    genericMgr.CreateWithTrim(issueType);
                    SaveSuccessMessage(Resources.ISS.IssueType.IssueType_Added);
                    return RedirectToAction("Edit/" + issueType.Code);
                }
            }
            return View(issueType);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_IssueType_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueType issueType = this.genericMgr.FindById<IssueType>(id);
                return View(issueType);
            }
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">IssueType id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_IssueType_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<IssueType>(id);
                SaveSuccessMessage(Resources.ISS.IssueType.IssueType_Deleted);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueType_Edit")]
        public ActionResult Edit(IssueType issueType)
        {
            if (ModelState.IsValid)
            {
                genericMgr.UpdateWithTrim(issueType);
                SaveSuccessMessage(Resources.ISS.IssueType.IssueType_Updated);
            }

            return View(issueType);
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, IssueTypeSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "it", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "it", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "it", ref whereStatement, param);

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
