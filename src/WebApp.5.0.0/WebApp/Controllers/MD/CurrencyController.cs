/// <summary>
/// Summary description for AddressController
/// </summary>
namespace com.Sconit.Web.Controllers.MD
{
    #region reference
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.MD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    #endregion

    /// <summary>
    /// This controller response to control the Currency.
    /// </summary>
    public class CurrencyController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the Currency security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        public ICurrencyMgr currencyMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the currency 
        /// </summary>
        private static string selectCountStatement = "select count(*) from Currency as u";

        /// <summary>
        /// hql to get all of the currency
        /// </summary>
        private static string selectStatement = "select u from Currency as u";

        /// <summary>
        /// hql to get count of the currency by currency's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from Currency as u where u.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for Currency controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_Currency_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Currency Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Currency_View")]
        public ActionResult List(GridCommand command, CurrencySearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Currency Search Model</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Currency_View")]
        public ActionResult _AjaxList(GridCommand command, CurrencySearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Currency>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Currency_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="currency">currency model</param>
        /// <returns>return to Edit action </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Currency_Edit")]
        public ActionResult New(Currency currency)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { currency.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, currency.Code);
                }
                else
                {
                    currencyMgr.CreateCurrency(currency);
                    SaveSuccessMessage(Resources.MD.Currency.Currency_Added);
                    return RedirectToAction("Edit/" + currency.Code);
                }
            }

            return View(currency);
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">currency id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Currency_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                Currency currency = this.genericMgr.FindById<Currency>(id);
                return View(currency);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="currency">currency model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_Currency_Edit")]
        public ActionResult Edit(Currency currency)
        {
            if (ModelState.IsValid)
            {
                currencyMgr.UpdateCurrency(currency);
                SaveSuccessMessage(Resources.MD.Currency.Currency_Updated);
            }

            return View(currency);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">currency id for delete</param>
        /// <returns>return to list view</returns>
        [SconitAuthorize(Permissions = "Url_Currency_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<Currency>(id);
                SaveSuccessMessage(Resources.MD.Currency.Currency_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Currency Search Model</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, CurrencySearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
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
