/// <summary>
/// Summary description for UomController
/// </summary>
namespace com.Sconit.Web.Controllers.MD
{
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
    using System;

    /// <summary>
    /// This controller response to control the Uom.
    /// </summary>
    public class UomController : WebAppBaseController
    {
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the Uom security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        /// <summary>
        /// Hql for Uom
        /// </summary>
        private static string selectCountStatement = "select count(*) from Uom as u";

        /// <summary>
        /// Hql for Uom
        /// </summary>
        private static string selectStatement = "select u from Uom as u";

        /// <summary>
        /// Hql for Uom
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from Uom as u where u.Code = ?";

        /// <summary>
        /// Hql for UomConvert
        /// </summary>
        private static string uomConvertselectCountStatement = "select count(*) from UomConversion as u left join u.Item as i";

        /// <summary>
        /// Hql for UomConvert
        /// </summary>
        private static string uomConvertselectStatement = "select u from UomConversion u left join u.Item as i";

        /// <summary>
        /// Hql for UomConvert
        /// </summary>
        private static string uomConvertDuiplicateVerifyStatement = @"select count(*) from UomConversion as u where u.Id = ?";

        /// <summary>
        /// Hql for UomConvert
        /// </summary>
        private static string uomConvertIsExistAndItemIsNull = @"select count(*) from UomConversion as u where u.Item is null and u.BaseUom=? and u.AlterUom=?";

        /// <summary>
        /// Hql for UomConvert
        /// </summary>
        private static string uomConvertIsExist = @"select count(*) from UomConversion u left join u.Item as i where i.Code=? and u.BaseUom=? and u.AlterUom=?";

        #region Uom
        /// <summary>
        /// Index action for Uom controller
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Search action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public ActionResult _Search()
        {
            return PartialView();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Uom Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public ActionResult List(GridCommand command, UomSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">GridCommand Telerik</param>
        /// <param name="searchModel">Uom Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public ActionResult _AjaxList(GridCommand command, UomSearchModel searchModel)
        {
            string replaceFrom = "_AjaxList";
            string replaceTo = "List/";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Uom>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Uom_Edit")]
        public ActionResult New()
        {
            return PartialView();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="uom">uom model</param>
        /// <returns>return to _edit view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Uom_Edit")]
        public ActionResult New(Uom uom)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { uom.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, uom.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(uom);
                    SaveSuccessMessage(Resources.MD.Uom.Uom_Added);
                    return RedirectToAction("_Edit/" + uom.Code);
                }
            }

            return PartialView(uom);
        }

        /// <summary>
        /// _edit action
        /// </summary>
        /// <param name="id">uom id for edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public ActionResult _Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                Uom uom = this.genericMgr.FindById<Uom>(id);
                return PartialView(uom);
            }
        }


        [SconitAuthorize(Permissions = "Url_Uom_Edit")]
        public ActionResult _Edit(Uom uom)
        {
            if (ModelState.IsValid)
            {

                    this.genericMgr.UpdateWithTrim(uom);
                    SaveSuccessMessage(Resources.MD.Uom.Uom_Updated);
                    return RedirectToAction("_Edit/" + uom.Code);
                
                 
            }
            return PartialView(uom);
        }

        /// <summary>
        /// delete action
        /// </summary>
        /// <param name="id">uom id for delete</param>
        /// <returns>return to list action</returns>
        [SconitAuthorize(Permissions = "Url_Uom_Delete")]
        public ActionResult Delete(string Code)
        {
            try
            {
                this.genericMgr.DeleteById<Uom>(Code);
                SaveSuccessMessage(Resources.MD.Uom.Uom_Deleted);
                return RedirectToAction("List");
            }
            catch (Exception)
            {
                SaveErrorMessage(Resources.MD.Uom.Uom_DeletedError);
                return RedirectToAction("_Edit/" + Code);
            }

        }
        #endregion

        #region UomConvert
        /// <summary>
        /// _UomConvert action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public ActionResult _UomConvert()
        {
            return PartialView();
        }

        /// <summary>
        /// _UomConvertList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">UomConversion Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public ActionResult _UomConvertList(GridCommand command, UomConversionSearchModel searchModel)
        {

            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        /// <summary>
        /// _AjaxUomConvertList
        /// </summary>
        /// <param name="command">GridCommand Telerik</param>
        /// <param name="searchModel">Conversion Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public ActionResult _AjaxUomConvertList(GridCommand command, UomConversionSearchModel searchModel)
        {
            string replaceFrom = "_AjaxUomConvertList";
            string replaceTo = "_UomConvertList/";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);


            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<UomConversion> uomConvList = GetAjaxPageData<UomConversion>(searchStatementModel, command);
            foreach (var item in uomConvList.Data)
            {
                if (item.Item != null)
                {
                    item.ItemCode = item.Item.Code;
                    item.ItemDescription = item.Item.Description;
                }
            }

            return PartialView(uomConvList);
        }
        #region 导出单位变量转换
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public void ExportXLS(UomConversionSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<UomConversion> uomConvList = GetAjaxPageData<UomConversion>(searchStatementModel, command);
            foreach (var item in uomConvList.Data)
            {
                if (item.Item != null)
                {
                    item.ItemCode = item.Item.Code;
                    item.ItemDescription = item.Item.Description;
                }
            }
            var fileName = string.Format("UomConversion.xls");
            ExportToXLS<UomConversion>(fileName, uomConvList.Data.ToList());
        }
        #endregion
        /// <summary>
        /// _UomConvertNew action
        /// </summary>
        /// <returns>rediret action</returns>
        [SconitAuthorize(Permissions = "Url_Uom_Edit")]
        public ActionResult _UomConvertNew()
        {
            return PartialView();
        }

        /// <summary>
        /// _UomConvertNew action
        /// </summary>
        /// <param name="uomConversion">UomConversion model</param>
        /// <returns>return to the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Uom_Edit")]
        public ActionResult _UomConvertNew(UomConversion uomConversion)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(uomConvertDuiplicateVerifyStatement, new object[] { uomConversion.Id })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code);
                }
                else if (uomConversion.BaseUom == uomConversion.AlterUom)
                {
                    SaveErrorMessage(Resources.MD.UomConvert.Errors_Existing_Uom);
                }
                else if (uomConversion.ItemCode == null)
                {
                    if (this.genericMgr.FindAll<long>(uomConvertIsExistAndItemIsNull, new object[] { uomConversion.BaseUom, uomConversion.AlterUom })[0] > 0)
                    {
                        SaveErrorMessage(Resources.MD.UomConvert.Errors_Existing_UomConv);
                    }
                    else
                    {
                        this.genericMgr.CreateWithTrim(uomConversion);
                        SaveSuccessMessage(Resources.MD.UomConvert.UomConvert_Added);
                        return RedirectToAction("_UomConvertEdit/" + uomConversion.Id);
                    }
                }
                else
                {
                    if (this.genericMgr.FindAll<Item>("from Item where Code=?", uomConversion.ItemCode).Count < 1)
                    {
                        SaveErrorMessage(Resources.MD.UomConvert.Errors_NotExisting_Item);
                    }
                    else
                    {
                        if (this.genericMgr.FindAll<long>(uomConvertIsExist, new object[] { uomConversion.ItemCode, uomConversion.BaseUom, uomConversion.AlterUom })[0] > 0)
                        {
                            SaveErrorMessage(Resources.MD.UomConvert.Errors_Existing_UomConv);
                        }
                        else
                        {
                            Item item = this.genericMgr.FindById<Item>(uomConversion.ItemCode);
                            uomConversion.Item = item;
                            this.genericMgr.CreateWithTrim(uomConversion);
                            SaveSuccessMessage(Resources.MD.UomConvert.UomConvert_Added);
                            return RedirectToAction("_UomConvertEdit/" + uomConversion.Id);
                        }
                    }
                }
            }

            return PartialView(uomConversion);
        }

        /// <summary>
        /// _UomConvertEdit action
        /// </summary>
        /// <param name="id">UomConvert id for edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Uom_View")]
        public ActionResult _UomConvertEdit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                UomConversion uomConv = this.genericMgr.FindById<UomConversion>(id);
                if (uomConv.Item != null)
                {
                    uomConv.ItemCode = uomConv.Item.Code;
                }

                return PartialView(uomConv);
            }
        }

        /// <summary>
        /// _UomConvertEdit action
        /// </summary>
        /// <param name="uomConv">UomConversion model</param>
        /// <returns>return to _UomConvertEdit action</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Uom_Edit")]
        public ActionResult _UomConvertEdit(UomConversion uomConv)
        {
            if (uomConv.ItemCode != null)
            {
                Item item = this.genericMgr.FindById<Item>(uomConv.ItemCode);
                uomConv.Item = item;
            }
            else
            {
                uomConv.Item = null;
            }


            ModelState.Remove("Item.Code");
            ModelState.Remove("Item.Description");
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(uomConv);
                SaveSuccessMessage(Resources.MD.UomConvert.UomConvert_Updated);
            }
            ////return new RedirectToRouteResult(new RouteValueDictionary  
            ////                                       { 
            ////                                           { "action", "_UomConvertEdit" }, 
            ////                                           { "controller", "Uom" },
            ////                                           { "id", uomConv.Id }
            ////                                       });

            return PartialView(uomConv);
        }

        /// <summary>
        /// _UomConvertDelete action
        /// </summary>
        /// <param name="id">UomConvert id for delete</param>
        /// <returns>return to the _UomConvertList</returns>
        [SconitAuthorize(Permissions = "Url_Uom_Delete")]
        public ActionResult _UomConvertDelete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<UomConversion>(id);
                SaveSuccessMessage(Resources.MD.UomConvert.UomConvert_Deleted);
                return RedirectToAction("_UomConvertList");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">GridCommand Telerik</param>
        /// <param name="searchModel">UomConversion Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, UomConversionSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            if (!string.IsNullOrEmpty(searchModel.Uom))
            {
                whereStatement = " where (u.BaseUom = ? or u.AlterUom = ?)";
                param.Add(searchModel.Uom);
                param.Add(searchModel.Uom);
            }
            HqlStatementHelper.AddLikeStatement("Code", searchModel.ItemCode, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "AlterUom")
                {
                    command.SortDescriptors[0].Member = "u.AlterUom";
                }
                else if (command.SortDescriptors[0].Member == "AlterQty")
                {
                    command.SortDescriptors[0].Member = "u.AlterQty";
                }
                else if (command.SortDescriptors[0].Member == "BaseUom")
                {
                    command.SortDescriptors[0].Member = "u.BaseUom";
                }
                else if (command.SortDescriptors[0].Member == "BaseQty")
                {
                    command.SortDescriptors[0].Member = "u.BaseQty";
                }
                else if (command.SortDescriptors[0].Member == "ItemCode")
                {
                    command.SortDescriptors[0].Member = "u.Item";
                }
                else if (command.SortDescriptors[0].Member == "ItemDescription")
                {
                    command.SortDescriptors[0].Member = "i.Description";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = uomConvertselectCountStatement;
            searchStatementModel.SelectStatement = uomConvertselectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">GridCommand Telerik</param>
        /// <param name="searchModel">Uom Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, UomSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by u.Sequence ";
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
