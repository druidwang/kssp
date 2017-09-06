using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.MD;
using com.Sconit.Web.Models;
using com.Sconit.Entity.MD;
using com.Sconit.Service;

namespace com.Sconit.Web.Controllers.MD
{
    public class ItemKitController : WebAppBaseController
    {

        //
        // GET: /ItemDiscontinue/

        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the address security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        #region hql

        private static string ItemKitCountStatement = "select count(*) from ItemKit as i";

        private static string selectStatement = "select i from ItemKit as i";

        private static string isExistsStatement = @"select count(*) from ItemKit as i where i.KitItem=? and ChildItem=?";

        #endregion

        #region View
        [SconitAuthorize(Permissions = "Url_ItemKit_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ItemDiscontinue Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ItemKit_View")]
        public ActionResult List(GridCommand command, ItemKitSearchModel searchModel)
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
        /// <param name="searchModel">ItemDiscontinue Search Model</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ItemKit_View")]
        public ActionResult _AjaxList(GridCommand command, ItemKitSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ItemKit>(searchStatementModel, command));
        }

        #endregion

        #region Edit

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_ItemKit_View")]
        public ActionResult New(ItemKit itemKit)
        {
            itemKit.ChildItem = itemKit.ChildItemCode != null ? this.genericMgr.FindById<Item>(itemKit.ChildItemCode) : null;
            if (itemKit.ChildItem != null) ModelState.Remove("ChildItem");
            if (ModelState.IsValid)
            {
                itemKit.IsActive = true;
                if (this.genericMgr.FindAll<long>(isExistsStatement, new object[] { itemKit.KitItem, itemKit.ChildItem })[0] > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheKitAlreadyExists);
                }
                else if (string.IsNullOrEmpty(itemKit.KitItem) || itemKit.ChildItem == null || itemKit.Qty==null )
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_PleaseFillCanNotBeEmpty);
                }
                else
                {
                    genericMgr.CreateWithTrim(itemKit);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_KitCreatedSuccessfully);
                    return RedirectToAction("Edit/" + itemKit.Id);
                }
            }
           // TempData["ItemDiscontinue"] = itemKit;
            return View(itemKit);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_ItemKit_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            ItemKit itemKit = this.genericMgr.FindById<ItemKit>(int.Parse(id));
            return View(itemKit);
        }

        [SconitAuthorize(Permissions = "Url_ItemKit_View")]
        public ActionResult _UpdateSave(ItemKit itemKit)
        {
            itemKit.ChildItem = itemKit.ChildItemCode != null ? this.genericMgr.FindById<Item>(itemKit.ChildItemCode) : null;

            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(isExistsStatement, new object[] { itemKit.KitItem, itemKit.ChildItem })[0] > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheKitAlreadyExists);
                }
                else
                {
                    this.genericMgr.UpdateWithTrim(itemKit);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_KitUpdatedSuccessfully);
                    return RedirectToAction("List");
                }
            }
            return RedirectToAction("Edit/" + itemKit.Id);

        }

        #endregion

        #region private

        private SearchStatementModel PrepareSearchStatement(GridCommand command, ItemKitSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("KitItem", searchModel.KitItem, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ChildItem.Code", searchModel.ChildItem, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("CreateUserName", searchModel.CreateUserName, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "i", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = ItemKitCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion

    }
}
