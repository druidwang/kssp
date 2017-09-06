using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using Telerik.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Web.Models;
using com.Sconit.Entity.INV;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Entity.SYS;
using com.Sconit.Entity.MD;

namespace com.Sconit.Web.Controllers.INV
{
    public class LocationTransactionController : WebAppBaseController
    {
        #region Properties
        //public IGenericMgr genericMgr { get; set; }
        //
        // GET: /LocationTransaction/
        #endregion

        private static string selectCountStatement = "select count(*) from LocationTransaction as l";

        private static string selectStatement = "from  LocationTransaction as l";

        #region public

        public ActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> LocationTransaction Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Menu.Inventory.InventoryTrans")]
        public ActionResult List(GridCommand command, LocationTransactionSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);

            return View();
        }


        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> LocationTransaction Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Menu.Inventory.InventoryTrans")]
        public ActionResult _AjaxList(GridCommand command, LocationTransactionSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<LocationTransaction>()));
            }
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<LocationTransaction> gridModel = GetAjaxPageData<LocationTransaction>(searchStatementModel, command);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var listdata in gridModel.Data)
            {
                listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).MaterialsGroup;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            }
            return PartialView(gridModel);
        }
        #endregion
        #region
        [SconitAuthorize(Permissions = "Menu.Inventory.InventoryTrans")]
        public void ExportXLS(LocationTransactionSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<LocationTransaction> gridModel = GetAjaxPageData<LocationTransaction>(searchStatementModel, command);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            IList<string> hus= gridModel.Data.ToList().Where(p => p.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_PO || p.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_IIC_VOID).Where(p=>!string.IsNullOrWhiteSpace(p.HuId)).Select(p=>p.HuId).Distinct().ToList();
            IList<Hu> huIds=new List<Hu>();
            if (hus.Count > 0)
            {
                huIds = genericMgr.FindAllIn<Hu>(" from Hu where HuId in (? ", hus).Where(p => !string.IsNullOrWhiteSpace(p.SupplierLotNo)).ToList();
            }
            var huDic = huIds.ToDictionary(d => d.HuId, d => d.SupplierLotNo);
            foreach (var listdata in gridModel.Data)
            {
                if (huDic.Count > 0 && !string.IsNullOrWhiteSpace(listdata.HuId))
                {
                    listdata.SupplierLotNo = huDic.ValueOrDefault(listdata.HuId);
                }
                listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).MaterialsGroup;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            }
            ExportToXLS<LocationTransaction>("LocationTransReport.xls", gridModel.Data.ToList());
        }

        [HttpPost]
        public JsonResult CheckExportQty(LocationTransactionSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<LocationTransaction> gridModel = GetAjaxPageData<LocationTransaction>(searchStatementModel, command);

            var message = "OK";
            if (value < gridModel.Total)
            {
                message = string.Format(Resources.EXT.ControllerLan.Con_ExportUnsuccessfully, value);
            }
            return Json(new { Message = message });
        }
        #endregion
        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, LocationTransactionSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LocationFrom", searchModel.LocationFrom, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LocationTo", searchModel.LocationTo, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("TransactionType", searchModel.TransactionType, "l", ref whereStatement, param);
            string timeType = searchModel.TimeType == 1 ? "CreateDate" : "EffectiveDate";
            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement(timeType, searchModel.StartDate, searchModel.EndDate, "l", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement(timeType, searchModel.StartDate, "l", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement(timeType, searchModel.EndDate, "l", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by l."+timeType+" desc";
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
