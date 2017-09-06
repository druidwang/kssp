/// <summary>
/// Summary description for AddressController
/// </summary>
namespace com.Sconit.Web.Controllers.MRP
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
    using com.Sconit.Entity.MRP.MD;
    using System;
    #endregion

    /// <summary>
    /// This controller response to control the Currency.
    /// </summary>
    public class MrpProductTypeController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the ProductType security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the ProductType 
        /// </summary>
        private static string selectCountStatement = "select count(*) from ProductType as i";

        /// <summary>
        /// hql to get all of the ProductType
        /// </summary>
        private static string selectStatement = "select i from ProductType as i";

        /// <summary>
        /// hql to get count of the ProductType by ProductType's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from ProductType as i where i.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for ProductType controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_ProductType_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ProductType Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ProductType_View")]
        public ActionResult List(GridCommand command, ProductTypeSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ProductType Search Model</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProductType_View")]
        public ActionResult _AjaxList(GridCommand command, ProductTypeSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ProductType>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_ProductType_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="ProductType">ProductType model</param>
        /// <returns>return to Edit action </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ProductType_Edit")]
        public ActionResult New(ProductType ProductType)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { ProductType.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, ProductType.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(ProductType);
                    SaveSuccessMessage(Resources.MRP.ProductType.ProductType_Added);
                    return RedirectToAction("Edit/" + ProductType.Code);
                }
            }

            return View(ProductType);
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">ProductType id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_ProductType_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ProductType ProductType = this.genericMgr.FindById<ProductType>(id);
                return View(ProductType);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="ProductType">ProductType model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_ProductType_Edit")]
        public ActionResult Edit(ProductType ProductType)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(ProductType);
                SaveSuccessMessage(Resources.MRP.ProductType.ProductType_Updated);
            }

            return View(ProductType);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">ProductType id for delete</param>
        /// <returns>return to list view</returns>
        [SconitAuthorize(Permissions = "Url_ProductType_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<ProductType>(id);
                SaveSuccessMessage(Resources.MRP.ProductType.ProductType_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ProductType Search Model</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ProductTypeSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = " order by i.Code ";
            }

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
