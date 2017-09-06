


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

    public class IssueAddressController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IssueAddress as ia left join ia.ParentIssueAddress pia ";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select ia from IssueAddress as ia left join ia.ParentIssueAddress pia ";

        /// <summary>
        /// 
        /// </summary>
        private static string codeDuiplicateVerifyStatement = @"select count(*) from IssueAddress as ia where ia.Code = ?";


        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        //
        // GET: /IssueAddress/

        [SconitAuthorize(Permissions = "Url_IssueAddress_View")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_IssueAddress_View")]
        public ActionResult List(GridCommand command, IssueAddressSearchModel searchModel)
        {
            TempData["IssueAddressSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_IssueAddress_View")]
        public ActionResult _AjaxList(GridCommand command, IssueAddressSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IssueAddress>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_IssueAddress_Edit")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueAddress_Edit")]
        public ActionResult New(IssueAddress issueAddress)
        {
            if (!string.IsNullOrWhiteSpace(issueAddress.ParentIssueAddressCode))
            {
                ViewBag.ParentIssueAddressCode = issueAddress.ParentIssueAddressCode;
                IssueAddress parentIssueAddress = new IssueAddress();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                parentIssueAddress.Code = issueAddress.ParentIssueAddressCode;
                issueAddress.ParentIssueAddress = parentIssueAddress;
                ModelState.Remove("ParentIssueAddress");
            }
            if (ModelState.IsValid)
            {
                //判断用户名不能重复
                if (this.genericMgr.FindAll<long>(codeDuiplicateVerifyStatement, new object[] { issueAddress.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.ISS.IssueAddress.Errors_Existing_IssueAddress, issueAddress.Code);
                }
                else
                {
                    genericMgr.CreateWithTrim(issueAddress);
                    SaveSuccessMessage(Resources.ISS.IssueAddress.IssueAddress_Added);
                    return RedirectToAction("Edit/" + issueAddress.Code);
                }
            }
            return View(issueAddress);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_IssueAddress_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueAddress issueAddress = this.genericMgr.FindById<IssueAddress>(id);
                return View(issueAddress);
            }
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">IssueAddress id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_IssueAddress_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<IssueAddress>(id);
                SaveSuccessMessage(Resources.ISS.IssueAddress.IssueAddress_Deleted);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueAddress_Edit")]
        public ActionResult Edit(IssueAddress issueAddress)
        {
            if (!string.IsNullOrWhiteSpace(issueAddress.ParentIssueAddressCode))
            {
                ViewBag.ParentIssueAddressId = issueAddress.ParentIssueAddressCode;
                IssueAddress parentIssueAddress = new IssueAddress();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                parentIssueAddress.Code = issueAddress.ParentIssueAddressCode;
                issueAddress.ParentIssueAddress = parentIssueAddress;
                ModelState.Remove("ParentIssueAddress");
            }
            if (ModelState.IsValid)
            {
                genericMgr.UpdateWithTrim(issueAddress);
                SaveSuccessMessage(Resources.ISS.IssueAddress.IssueAddress_Updated);
            }

            return View(issueAddress);
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, IssueAddressSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (!string.IsNullOrWhiteSpace(searchModel.ParentIssueAddressCode))
            {
                HqlStatementHelper.AddEqStatement("Code", searchModel.ParentIssueAddressCode, "pia", ref whereStatement, param);
            }
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "ia", ref whereStatement, param);

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
