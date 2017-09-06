using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Services.Transaction;
using com.Sconit.Entity.CUST;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.CUST;
using com.Sconit.Utility;
using Telerik.Web.Mvc;


namespace com.Sconit.Web.Controllers.CUST
{
    public class GeneralLedgerController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        private static string selectCountStatement = "select count(*) from GeneralLedger as h";

        private static string selectStatement = "select h from GeneralLedger as h";

        #region GeneralLedger Method

        #region View

        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_GeneralLedger_View")]
        public ActionResult List(GridCommand command, GeneralLedgerSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_GeneralLedger_View")]
        public ActionResult _AjaxList(GridCommand command, GeneralLedgerSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<GeneralLedger>(searchStatementModel, command));
        }
        #endregion

        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_GeneralLedger_View")]
        public ActionResult Edit(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return HttpNotFound();
            }
            else
            {
                GeneralLedger generalLedger = this.genericMgr.FindAll<GeneralLedger>("select h from GeneralLedger as h where h.Code=?", new object[] { Code })[0];
                return View(generalLedger);
            }
        }

        [SconitAuthorize(Permissions = "Url_GeneralLedger_View")]
        public ActionResult Edit(GeneralLedger generalLedger)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this.genericMgr.UpdateWithTrim(generalLedger);
                    SaveSuccessMessage(Resources.CUST.GeneralLedger.GeneralLedger_Updated);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return View(generalLedger);
        }


        [SconitAuthorize(Permissions = "Url_GeneralLedger_View")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_GeneralLedger_View")]
        public ActionResult New(GeneralLedger generalLedger)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this.genericMgr.CreateWithTrim(generalLedger);
                    SaveSuccessMessage(Resources.CUST.GeneralLedger.GeneralLedger_Added);
                    string Code = generalLedger.Code;

                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "GeneralLedger" }, { "Code", Code } });
                }
            }
            catch (Exception e)
            {
                SaveErrorMessage(e);
            }

            return View(generalLedger);
        }

        [SconitAuthorize(Permissions = "Url_GeneralLedger_View")]
        public ActionResult Delete(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return HttpNotFound();
            }
            else
            {
                GeneralLedger generalLedger = this.genericMgr.FindAll<GeneralLedger>("select h from GeneralLedger as h where h.Code=? ", new object[] { Code })[0];
                this.genericMgr.Delete(generalLedger);
                SaveSuccessMessage(Resources.CUST.GeneralLedger.GeneralLedger_Deleted);
                return RedirectToAction("List");
            }
        }

        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, GeneralLedgerSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "h", ref whereStatement, param);

            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "h", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by h.CreateDate desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion
    }
}
