


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

    public class IssueNoController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IssueNo  as it";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select it from IssueNo as it";

        /// <summary>
        /// 
        /// </summary>
        private static string codeDuiplicateVerifyStatement = @"select count(*) from IssueNo as it where ino.Code = ?";


        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //
        // GET: /IssueNo/

        [SconitAuthorize(Permissions = "Url_IssueNo_View")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_IssueNo_View")]
        public ActionResult List(GridCommand command, IssueNoSearchModel searchModel)
        {
            TempData["IssueNoSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_IssueNo_View")]
        public ActionResult _AjaxList(GridCommand command, IssueNoSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IssueNo>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_IssueNo_Edit")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueNo_Edit")]
        public ActionResult New(IssueNo issueNo)
        {
            if (!string.IsNullOrWhiteSpace(issueNo.IssueTypeCode))
            {
                ViewBag.IssueTypeCode = issueNo.IssueTypeCode;
                IssueType issueType = new IssueType();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueType.Code = issueNo.IssueTypeCode;
                issueNo.IssueType = issueType;
                ModelState.Remove("IssueType");
            }
            if (ModelState.IsValid)
            {
                //判断用户名不能重复
                if (this.genericMgr.FindAll<long>(codeDuiplicateVerifyStatement, new object[] { issueNo.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.ISS.IssueNo.Errors_Existing_IssueNo, issueNo.Code);
                }
                else
                {
                    genericMgr.CreateWithTrim(issueNo);
                    SaveSuccessMessage(Resources.ISS.IssueNo.IssueNo_Added);
                    return RedirectToAction("Edit/" + issueNo.Code);
                }
            }
            return View(issueNo);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_IssueNo_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueNo issueNo = this.genericMgr.FindById<IssueNo>(id);
                return View(issueNo);
            }
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">IssueNo id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_IssueNo_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<IssueNo>(id);
                SaveSuccessMessage(Resources.ISS.IssueNo.IssueNo_Deleted);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueNo_Edit")]
        public ActionResult Edit(IssueNo issueNo)
        {
            if (!string.IsNullOrWhiteSpace(issueNo.IssueTypeCode))
            {
                ViewBag.IssueTypeCode = issueNo.IssueTypeCode;
                IssueType issueType = new IssueType();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueType.Code = issueNo.IssueTypeCode;
                issueNo.IssueType = issueType;
                ModelState.Remove("IssueType");
            }
            if (ModelState.IsValid)
            {
                genericMgr.UpdateWithTrim(issueNo);
                SaveSuccessMessage(Resources.ISS.IssueNo.IssueNo_Updated);
            }

            return View(issueNo);
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, IssueNoSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "it", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "it", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IssueTypeCode", searchModel.IssueTypeCode, "it", ref whereStatement, param);
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "IssueTypeCode")
                {
                    command.SortDescriptors[0].Member = "IssueType";
                }
                else if (command.SortDescriptors[0].Member == "IssueTypeDescription")
                {
                    command.SortDescriptors[0].Member = "IssueType";
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
