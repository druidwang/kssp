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
    using com.Sconit.Entity.PRD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.MD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.CUST;
    using com.Sconit.Entity.Exception;
    using System.Web;
    using System;
    #endregion

    /// <summary>
    /// This controller response to control the Item.
    /// </summary>
    public class ItemController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the Item security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        //public IItemMgr itemMgr { get; set; }
        #endregion

        #region hql
        /// <summary>
        /// hql to get count of the Item 
        /// </summary>
        private static string selectCountStatement = "select count(*) from Item as i";

        /// <summary>
        /// hql to get all of the Item
        /// </summary>
        private static string selectStatement = "select i from Item as i";

        /// <summary>
        /// hql to get count of the Item by Item's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from Item as i where i.Code = ?";

        /// <summary>
        /// hql to get count of the Item 
        /// </summary>
        private static string selectItemRefCountStatement = "select count(*) from ItemReference as i";

        /// <summary>
        /// hql to get all of the Item
        /// </summary>
        private static string selectItemRefStatement = "select i from ItemReference as i";

        /// <summary>
        /// hql to get count of the Item by Item's code
        /// </summary>
        private static string itemRefDuiplicateVerifyStatement = @"select count(*) from ItemReference as i where i.Id = ?";

        /// <summary>
        /// hql for ItemReference
        /// </summary>
        private static string itemAndPartyDuiplicateVerifyStatement = @"select count(*) from ItemReference as i where i.Item = ? and i.Party= ?";

        /// <summary>
        /// hql for ItemReference
        /// </summary>
        private static string itemAndPartyIsNullDuiplicateVerifyStatement = @"select count(*) from ItemReference as i where i.Item = ? and i.Party is null";

        /// <summary>
        /// hql
        /// </summary>
        private static string selectItemPackageCountStatement = "select count(*) from ItemPackage as i";

        /// <summary>
        /// hql
        /// </summary>
        private static string selectItemPackageStatement = "select i from ItemPackage as i";

        /// <summary>
        /// hql 
        /// </summary>
        private static string itemPackageDuiplicateVerifyStatement = @"select count(*) from ItemPackage as i where i.Id = ?";

        /// <summary>
        /// hql 
        /// </summary>
        private static string itemAndUnitCountIsExistStatement = @"select count(*) from ItemPackage as i where i.Item = ? and i.UnitCount = ?";
        #endregion

        #region Item
        /// <summary>
        /// Index action for Item controller
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Item_View")]
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
        [SconitAuthorize(Permissions = "Url_Item_View")]
        public ActionResult List(GridCommand command, ItemSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_Item_View")]
        public ActionResult _AjaxList(GridCommand command, ItemSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<Item> gridModel = GetAjaxPageData<Item>(searchStatementModel, command);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var listdata in gridModel.Data)
            {
                listdata.ItemCategoryDesc = GetItemCategory(listdata.ItemCategory,Sconit.CodeMaster.SubCategory.ItemCategory,itemCategoryList).Description;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            }
            return PartialView(gridModel);
        }

        #region 导出物料
        [SconitAuthorize(Permissions = "Url_Item_View")]
        public void ExportXLS(ItemSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<Item> gridModel = GetAjaxPageData<Item>(searchStatementModel, command);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var listdata in gridModel.Data)
            {
                listdata.ItemCategoryDesc = GetItemCategory(listdata.ItemCategory, Sconit.CodeMaster.SubCategory.ItemCategory, itemCategoryList).Description;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            }
            var fileName = string.Format("Item.xls");
            ExportToXLS<Item>(fileName, gridModel.Data.ToList());
        }
        #endregion

        //private ItemCategory GetItemCategory(string code, Sconit.CodeMaster.SubCategory subCategory, IList<ItemCategory> itemCategoryList)
        //{
        //    var itemCategory = itemCategoryList.Where(p => p.Code == code && p.SubCategory == subCategory).FirstOrDefault() ?? new ItemCategory();
        //    return itemCategory;
        //}

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="item">Item model</param>
        /// <returns>return to Edit action </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult New(Item item)
        {
            bool canSave = true;
            if (ModelState.IsValid)
            {

                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { item.Code })[0] > 0)
                {
                    canSave = false;
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, item.Code);

                }

                if (item.Bom != null)
                {
                    if (this.genericMgr.FindAll<BomMaster>("from BomMaster where Code= ? and IsActive = ? ", new object[] { item.Bom, true }).Count == 0)
                    {
                        canSave = false;
                        SaveErrorMessage(Resources.MD.Item.ItemErrors_NotExisting_Bom);
                    }
                }
                if (item.Routing != null)
                {
                    if (this.genericMgr.FindAll<RoutingMaster>("from RoutingMaster where Code= ? and IsActive = ? ", new object[] { item.Routing, true }).Count == 0)
                    {
                        canSave = false;
                        SaveErrorMessage(Resources.MD.Item.ItemErrors_NotExisting_Routing);
                    }
                }
                if (item.Location != null)
                {
                    if (this.genericMgr.FindAll<Location>("from Location where Code= ? and IsActive = ? ", new object[] { item.Location, true }).Count == 0)
                    {
                        canSave = false;
                        SaveErrorMessage(Resources.MD.Item.ItemErrors_NotExisting_Location);
                    }
                }
                if (!string.IsNullOrWhiteSpace(item.ReferenceCode) && item.IsActive)
                {
                    if (this.genericMgr.FindAll<Item>("from Item where IsActive =? and ReferenceCode =?",
                        new object[] { true, item.ReferenceCode }).Count > 0)
                    {
                        canSave = false;
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheCustomerPartNumberAlreadyExists);
                    }
                }
                //if (string.IsNullOrWhiteSpace(item.Division))
                //{
                //        canSave = false;
                //        SaveErrorMessage("产品组不能为空。");
                //}
                if (canSave)
                {
                    itemMgr.CreateItem(item);
                    SaveSuccessMessage(Resources.MD.Item.Item_Added);
                    return RedirectToAction("_EditList/" + item.Code);
                }
            }

            return View(item);
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">Item id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Item_View")]//sycdebug change permission from "edit" to "view"
        public ActionResult _EditList(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                //ViewBag.IsSection = this.genericMgr.FindById<Item>(id).IsSection;
                return View("_EditList", string.Empty, id);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">Item id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Item_View")]
        public ActionResult Edit(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                Item item = this.genericMgr.FindById<Item>(id);
                return PartialView(item);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="item">Item model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                bool canSave = true;
                if (item.Bom != null)
                {
                    if (this.genericMgr.FindAll<BomMaster>("from BomMaster where Code= ? and IsActive = ? ",
                        new object[] { item.Bom, true }).Count == 0)
                    {
                        canSave = false;
                        SaveErrorMessage(Resources.MD.Item.ItemErrors_NotExisting_Bom);
                    }
                }
                if (item.Routing != null)
                {
                    if (this.genericMgr.FindAll<RoutingMaster>("from RoutingMaster where Code= ? and IsActive = ? ",
                        new object[] { item.Routing, true }).Count == 0)
                    {
                        canSave = false;
                        SaveErrorMessage(Resources.MD.Item.ItemErrors_NotExisting_Routing);
                    }
                }
                if (item.Location != null)
                {
                    if (this.genericMgr.FindAll<Location>("from Location where Code= ? and IsActive = ? ",
                        new object[] { item.Location, true }).Count == 0)
                    {
                        canSave = false;
                        SaveErrorMessage(Resources.MD.Item.ItemErrors_NotExisting_Location);
                    }
                }

                //if (!string.IsNullOrWhiteSpace(item.ReferenceCode) && item.IsActive)
                //{
                //    if (this.genericMgr.FindAll<Item>("from Item where IsActive =? and ReferenceCode =? and Code <>? ",
                //        new object[] { true, item.ReferenceCode, item.Code }).Count > 0)
                //    {
                //        canSave = false;
                //        SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheCustomerPartNumberAlreadyExists);
                //    }
                //}
                //if (string.IsNullOrWhiteSpace(item.Division))
                //{
                //    canSave = false;
                //    SaveErrorMessage("产品组不能为空。");
                //}
                if (canSave)
                {
                    this.itemMgr.UpdateItem(item);
                    SaveSuccessMessage(Resources.MD.Item.Item_Updated);
                }
            }

            return PartialView(item);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">Item id for delete</param>
        /// <returns>return to list view</returns>
        [SconitAuthorize(Permissions = "Url_Item_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.itemMgr.DeleteItem(id);
                SaveSuccessMessage(Resources.MD.Item.Item_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        #region ItemReference
        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="itemRefsearchModel">ItemRef Search model</param>
        /// <param name="id">ItemRef id</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Item_View")]
        public ActionResult ItemRefResult(GridCommand command, ItemRefSearchModel itemRefsearchModel, string id)
        {

            ViewBag.ItemCode = id;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="itemRefsearchModel">ItemRef Search Model</param>
        /// <param name="itemCode">ItemRef itemCode</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Item_View")]
        public ActionResult _AjaxItemReferenceList(GridCommand command, ItemRefSearchModel itemRefsearchModel, string itemCode)
        {
            SearchStatementModel searchStatementModel = this.ItemRefPrepareSearchStatement(command, itemRefsearchModel, itemCode);
            return PartialView(GetAjaxPageData<ItemReference>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="id">ItemRef id</param>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult _ItemRefNew(string id)
        {
            ItemReference itemRef = new ItemReference();
            itemRef.Item = id;
            return PartialView(itemRef);
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="itemReference">ItemReference model</param>
        /// <returns>return to Edit action </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult _ItemRefNew(ItemReference itemReference)
        {
            bool isError = false;
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(itemRefDuiplicateVerifyStatement, new object[] { itemReference.Id })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, itemReference.Id.ToString());
                    isError = true;
                }
                else
                {
                    if (itemReference.Party != null)
                    {
                        if (this.genericMgr.FindAll<long>(itemAndPartyDuiplicateVerifyStatement, new object[] { itemReference.Item, itemReference.Party })[0] > 0)
                        {
                            SaveErrorMessage(Resources.MD.ItemRef.ItemPartyErrors_Existing_Code);
                            isError = true;
                        }
                    }
                    else
                    {
                        if (this.genericMgr.FindAll<long>(itemAndPartyIsNullDuiplicateVerifyStatement, new object[] { itemReference.Item })[0] > 0)
                        {
                            SaveErrorMessage(Resources.MD.ItemRef.ItemPartyErrors_Existing_Code);
                            isError = true;
                        }
                    }
                }

                if (!isError)
                {
                    this.genericMgr.CreateWithTrim(itemReference);
                    SaveSuccessMessage(Resources.MD.ItemRef.ItemRef_Added);
                    return RedirectToAction("ItemRefResult/" + itemReference.Item);
                }
            }

            return PartialView(itemReference);
        }

        /// <summary>
        /// _ItemRefEdit action
        /// </summary>
        /// <param name="id">ItemRef id for Edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult _ItemRefEdit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                ItemReference itemReference = this.genericMgr.FindById<ItemReference>(id);
                return PartialView(itemReference);
            }
        }

        /// <summary>
        /// _ItemRefEdit action
        /// </summary>
        /// <param name="itemReference">ItemReference Model</param>
        /// <returns>return to _ItemRefEdit action</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult _ItemRefEdit(ItemReference itemReference)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(itemReference);
                SaveSuccessMessage(Resources.MD.ItemRef.ItemRef_Updated);
            }

            ////return new RedirectToRouteResult(new RouteValueDictionary  
            ////                                       { 
            ////                                           { "action", "_ItemRefEdit" }, 
            ////                                           { "controller", "Item" },
            ////                                           { "id", itemReference.Id }
            ////                                       });
            return PartialView(itemReference);
        }

        /// <summary>
        /// ItemRefDelete action
        /// </summary>
        /// <param name="id">ItemReference id for delete</param>
        /// <param name="item">ItemReference item</param>
        /// <returns>return to ItemRefDelete action</returns>
        [SconitAuthorize(Permissions = "Url_Item_Delete")]
        public ActionResult ItemRefDelete(int? id, string item)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<ItemReference>(id);
                SaveSuccessMessage(Resources.MD.ItemRef.ItemRef_Deleted);
                return RedirectToAction("ItemRefResult/" + item);
            }
        }
        #endregion

        #region ItemPackage
        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="itemRefsearchModel">itemPackage Search model</param>
        /// <param name="id">itemPackage id</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Item_View")]
        public ActionResult ItemPackage(GridCommand command, ItemPackageSearchModel itemPackageSearchModel, string id)
        {
            ViewBag.ItemCode = id;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="itemRefsearchModel">ItemPackage Search Model</param>
        /// <param name="itemCode">ItemPackage itemCode</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Item_View")]
        public ActionResult _AjaxItemPackageList(GridCommand command, ItemPackageSearchModel itemPackageSearchModel, string itemCode)
        {
            SearchStatementModel searchStatementModel = this.ItemPackagePrepareSearchStatement(command, itemPackageSearchModel, itemCode);
            return PartialView(GetAjaxPageData<ItemPackage>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="id">ItemPackage id</param>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult ItemPackageNew(string id)
        {
            ItemPackage itemPackage = new ItemPackage();
            itemPackage.Item = id;
            return PartialView(itemPackage);
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="itemPackage">ItemPackage model</param>
        /// <returns>return to Edit action </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult ItemPackageNew(ItemPackage itemPackage)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(itemPackageDuiplicateVerifyStatement, new object[] { itemPackage.Id })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, itemPackage.Id.ToString());
                }
                else if (this.genericMgr.FindAll<long>(itemAndUnitCountIsExistStatement, new object[] { itemPackage.Item, itemPackage.UnitCount })[0] > 0)
                {
                    SaveErrorMessage(Resources.MD.ItemPackage.ItemPackageErrors_Existing_ItemAndUnitCount);
                }
                else if (itemPackage.UnitCount <= 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_UnitcountQuantityMustGreater);
                }
                else
                {
                    if (itemPackage.Description == null)
                    {
                        itemPackage.Description = string.Empty;
                    }
                    this.genericMgr.CreateWithTrim(itemPackage);

                    SaveSuccessMessage(Resources.MD.ItemPackage.ItemPackage_Added);
                    return RedirectToAction("ItemPackage/" + itemPackage.Item);
                }
            }

            return PartialView(itemPackage);
        }

        /// <summary>
        /// ItemPackageEdit action
        /// </summary>
        /// <param name="id">ItemPackage id for Edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Item_View")]
        public ActionResult ItemPackageEdit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                ItemPackage itemPackage = this.genericMgr.FindById<ItemPackage>(id);
                return PartialView(itemPackage);
            }
        }

        /////// <summary>
        /////// ItemPackageEdit action
        /////// </summary>
        /////// <param name="itemReference">ItemPackage Model</param>
        /////// <returns>return to ItemPackageEdit action</returns>
        ////[HttpPost]
        ////[SconitAuthorize(Permission = "Url_Item_Edit")]
        ////public ActionResult ItemPackageEdit(ItemPackage itemPackage)
        ////{
        ////    if (ModelState.IsValid)
        ////    {
        ////        if (itemPackage.IsDefault)
        ////        {
        ////            IList<ItemPackage> itemPackageList = queryMgr.FindAll<ItemPackage>("from ItemPackage as i where i.Item = ?", itemPackage.Item);
        ////            for (int i = 0; i < itemPackageList.Count; i++)
        ////            {
        ////                itemPackageList[i].IsDefault = false;
        ////                this.genericMgr.UpdateWithTrim(itemPackageList[i]);
        ////            }
        ////        }
        ////        this.genericMgr.UpdateWithTrim(itemPackage);
        ////        SaveSuccessMessage(Resources.MD.ItemPackage.ItemPackage_Updated);
        ////    }

        ////    ////return new RedirectToRouteResult(new RouteValueDictionary  
        ////    ////                                       { 
        ////    ////                                           { "action", "_ItemRefEdit" }, 
        ////    ////                                           { "controller", "Item" },
        ////    ////                                           { "id", itemReference.Id }
        ////    ////                                       });
        ////    return PartialView(itemPackage);
        ////}

        /// <summary>
        /// ItemPackageDelete action
        /// </summary>
        /// <param name="id">itemPackage id for delete</param>
        /// <param name="item">itemPackage item</param>
        /// <returns>return to ItemPackageDelete action</returns>
        [SconitAuthorize(Permissions = "Url_Item_Delete")]
        public ActionResult ItemPackageDelete(int? id, string item)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<ItemPackage>(id);
                SaveSuccessMessage(Resources.MD.ItemPackage.ItemPackage_Deleted);
                return RedirectToAction("ItemPackage/" + item);
            }
        }
        #endregion

        #region ItemEx
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult ItemExEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ItemEx itemEx = new ItemEx();
                IList<ItemEx> ItemExList = this.genericMgr.FindAll<ItemEx>("from ItemEx as i where i.Code=?", id);
                if (ItemExList != null && ItemExList.Count > 0)
                {
                    ViewBag.IsNull = false;
                    itemEx = ItemExList[0];
                }
                else
                {
                    ViewBag.IsNull = true;
                    itemEx.Code = id;
                }
                return PartialView(itemEx);

            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult ItemExEdit(ItemEx itemEx)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IList<ItemEx> ItemExList = this.genericMgr.FindAll<ItemEx>("from ItemEx as i where i.Code=?", itemEx.Code);
                    if (ItemExList != null && ItemExList.Count > 0)
                    {
                        ItemEx ie = ItemExList[0];
                        itemEx.CreateDate = ie.CreateDate;
                        itemEx.CreateUserId = ie.CreateUserId;
                        itemEx.CreateUserName = ie.CreateUserName;
                        this.genericMgr.UpdateWithTrim(itemEx);
                    }
                    else
                    {
                        this.genericMgr.CreateWithTrim(itemEx);
                    }
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ExtrusionParamUpdatedSuccessfully);
                }
                else
                {
                    foreach (var item in ModelState)
                    {
                        throw new BusinessException(Resources.EXT.ControllerLan.Con_AgingTimeAgingMathodSpecCanNotBeEmpty);
                    }
                }
            }
            catch (BusinessException be)
            {
                SaveErrorMessage(be.GetMessages()[0].GetMessageString());
            }
            TempData["TabIndex"] = 3;
            return new RedirectToRouteResult(new RouteValueDictionary { { "action", "_EditList" }, { "controller", "Item" }, { "id", itemEx.Code } });
        }

        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult ItemExDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<ItemEx>(id);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ExtrusionParamDeletedSuccessfully);
                TempData["TabIndex"] = 3;
                return new RedirectToRouteResult(new RouteValueDictionary { { "action", "_EditList" }, { "controller", "Item" }, { "id", id } });

            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Item Search Model</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, ItemSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("ReferenceCode", searchModel.ReferenceCode, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Anywhere, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsVirtual", searchModel.IsVirtual, "i", ref whereStatement, param);
            if (!string.IsNullOrWhiteSpace(searchModel.ItemCategory))
                HqlStatementHelper.AddEqStatement("ItemCategory", searchModel.ItemCategory, "i", ref whereStatement, param);
            if (!string.IsNullOrWhiteSpace(searchModel.MaterialsGroup))
                HqlStatementHelper.AddEqStatement("MaterialsGroup", searchModel.MaterialsGroup, "i", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="itemRefsearchModel">ItemRef Search Model</param>
        /// <param name="id">ItemRef id</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel ItemRefPrepareSearchStatement(GridCommand command, ItemRefSearchModel itemRefsearchModel, string id)
        {
            string whereStatement = " where i.Item='" + id + "'";

            IList<object> param = new List<object>();

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectItemRefCountStatement;
            searchStatementModel.SelectStatement = selectItemRefStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="itemRefsearchModel">ItemPackage Search Model</param>
        /// <param name="id">ItemPackage id</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel ItemPackagePrepareSearchStatement(GridCommand command, ItemPackageSearchModel itemPackageSearchModel, string id)
        {
            string whereStatement = " where i.Item='" + id + "'";

            IList<object> param = new List<object>();

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectItemPackageCountStatement;
            searchStatementModel.SelectStatement = selectItemPackageStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        #region Import
        [SconitAuthorize(Permissions = "Url_Item_Edit")]
        public ActionResult Import(IEnumerable<HttpPostedFileBase> attachments)
        {
            try
            {
                foreach (var file in attachments)
                {
                    itemMgr.ImportItem(file.InputStream);
                }
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ItemSuccessfullyImport);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Content(string.Empty);
        }

        #endregion

    }
}
