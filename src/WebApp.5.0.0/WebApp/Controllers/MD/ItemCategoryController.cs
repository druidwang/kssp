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
    public class ItemCategoryController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the ItemCategory security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the ItemCategory 
        /// </summary>
        private static string selectCountStatement = "select count(*) from ItemCategory as i";

        /// <summary>
        /// hql to get all of the ItemCategory
        /// </summary>
        private static string selectStatement = "select i from ItemCategory as i";

        /// <summary>
        /// hql to get count of the ItemCategory by ItemCategory's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from ItemCategory as i where i.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for ItemCategory controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_ItemCategory_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ItemCategory Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ItemCategory_View")]
        public ActionResult List(GridCommand command, ItemCategorySearchModel searchModel)
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
        /// <param name="searchModel">ItemCategory Search Model</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ItemCategory_View")]
        public ActionResult _AjaxList(GridCommand command, ItemCategorySearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ItemCategory>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_ItemCategory_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="itemCategory">ItemCategory model</param>
        /// <returns>return to Edit action </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ItemCategory_Edit")]
        public ActionResult New(ItemCategory itemCategory)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { itemCategory.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, itemCategory.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(itemCategory);
                    SaveSuccessMessage(Resources.MD.ItemCategory.ItemCategory_Added);
                    return RedirectToAction("Edit/" + itemCategory.Code);
                }
            }

            return View(itemCategory);
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">ItemCategory id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_ItemCategory_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ItemCategory itemCategory = this.genericMgr.FindById<ItemCategory>(id);
                if (!string.IsNullOrEmpty(itemCategory.ParentCategory))
                {
                    itemCategory.ParentCategoryDescription = this.genericMgr.FindById<ItemCategory>(itemCategory.ParentCategory).Description;
                }
                return View(itemCategory);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="itemCategory">ItemCategory model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_ItemCategory_Edit")]
        public ActionResult Edit(ItemCategory itemCategory)
        {
            if (ModelState.IsValid)
            {
                bool isExist = false;
                bool isDiffrent = false;
                IList<ItemCategory> itemCategoryList = queryMgr.FindAll<ItemCategory>("from ItemCategory as i where i.ParentCategory = ?", itemCategory.Code);
                for (int i = 0; i < itemCategoryList.Count; i++)
                {
                    if (itemCategoryList[i].Code == itemCategory.ParentCategory)
                    {
                        isExist = true;
                        break;
                    }
                }

                if (itemCategory.ParentCategory == itemCategory.Code)
                {
                    isDiffrent = true;
                }

                if (isExist)
                {
                    SaveErrorMessage(Resources.MD.ItemCategory.ItemCategoryErrors_IsParent);
                }
                else if (isDiffrent)
                {
                    SaveErrorMessage(Resources.MD.ItemCategory.ItemCategoryErrors_IsDiffrent);
                }
                else
                {
                    this.genericMgr.UpdateWithTrim(itemCategory);
                    SaveSuccessMessage(Resources.MD.ItemCategory.ItemCategory_Updated);
                }
            }

            return View(itemCategory);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">ItemCategory id for delete</param>
        /// <returns>return to list view</returns>
        [SconitAuthorize(Permissions = "Url_ItemCategory_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<ItemCategory>(id);
                SaveSuccessMessage(Resources.MD.ItemCategory.ItemCategory_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ItemCategory Search Model</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ItemCategorySearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("SubCategory", searchModel.SubCategory, "i", ref whereStatement, param);
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
