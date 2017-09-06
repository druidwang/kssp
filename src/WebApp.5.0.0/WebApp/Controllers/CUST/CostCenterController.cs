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
    public class CostCenterController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        private static string selectCountStatement = "select count(*) from CostCenter as h";

        private static string selectStatement = "select h from CostCenter as h";

        #region CostCenter Method

        #region View

        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_CostCenter_View")]
        public ActionResult List(GridCommand command, CostCenterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CostCenter_View")]
        public ActionResult _AjaxList(GridCommand command, CostCenterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<CostCenter>(searchStatementModel, command));
        }
        #endregion

        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CostCenter_View")]
        public ActionResult Edit(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return HttpNotFound();
            }
            else
            {
                CostCenter costCenter = this.genericMgr.FindAll<CostCenter>("select h from CostCenter as h where h.Code=?", new object[] { Code })[0];
                return View(costCenter);
            }
        }

        [SconitAuthorize(Permissions = "Url_CostCenter_View")]
        public ActionResult Edit(CostCenter costCenter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this.genericMgr.UpdateWithTrim(costCenter);
                    SaveSuccessMessage(Resources.CUST.CostCenter.CostCenter_Updated);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return View(costCenter);
        }


        [SconitAuthorize(Permissions = "Url_CostCenter_View")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CostCenter_View")]
        public ActionResult New(CostCenter costCenter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this.genericMgr.CreateWithTrim(costCenter);
                    SaveSuccessMessage(Resources.CUST.CostCenter.CostCenter_Added);
                    string Code = costCenter.Code;

                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "CostCenter" }, { "Code", Code } });
                }
            }
            catch (Exception e)
            {
                SaveErrorMessage(e);
            }

            return View(costCenter);
        }

        [SconitAuthorize(Permissions = "Url_CostCenter_View")]
        public ActionResult Delete(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return HttpNotFound();
            }
            else
            {
                CostCenter costCenter = this.genericMgr.FindAll<CostCenter>("select h from CostCenter as h where h.Code=? ", new object[] { Code })[0];
                this.genericMgr.Delete(costCenter);
                SaveSuccessMessage(Resources.CUST.CostCenter.CostCenter_Deleted);
                return RedirectToAction("List");
            }
        }

        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, CostCenterSearchModel searchModel)
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
