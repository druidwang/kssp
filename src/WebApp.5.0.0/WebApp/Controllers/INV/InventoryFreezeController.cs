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
using com.Sconit.Entity.Exception;

namespace com.Sconit.Web.Controllers.INV
{
    public class InventoryFreezeController : WebAppBaseController
    {
        #region Properties
        //public IGenericMgr genericMgr { get; set; }
        public ILocationDetailMgr LocDetGeneMgr { get; set; }
        //public IItemMgr itemMgr { get; set; }
        //
        // GET: /LocationTransaction/
        #endregion

        private static string selectCountStatement = "select count(*) from LocationLotDetail as l";

        private static string selectStatement = "from  LocationLotDetail as l";

        #region public

        public ActionResult Index()
        {
            return View();
        }
        
        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> LocationLotDetail Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryFreeze_View")]
        public ActionResult List(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            if (!string.IsNullOrEmpty(searchModel.Location))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_LocationSearchConditionCanNotBeEmpty);
            }
            TempData["LocationLotDetailSearchModel"] = searchModel;
            return View();
        }
        
        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel"> LocationLotDetail Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_InventoryFreeze_View")]
        public ActionResult _AjaxList(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.Location))
            {
                return PartialView(new GridModel(new List<LocationLotDetail>()));
            }

            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);

            var list = GetAjaxPageData<LocationLotDetail>(searchStatementModel, command);
            foreach (var data in list.Data)
            {
                data.ItemDescription = itemMgr.GetCacheItem(data.Item).Deriction;
            }
            return PartialView(GetAjaxPageData<LocationLotDetail>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Inventory_InventoryFreeze_Freeze,Url_Inventory_InventoryFreeze_UnFreeze")]
        public ActionResult PopFreeze(Boolean IsFreeze)
        {
            ViewBag.IsFreeze = IsFreeze;
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_Inventory_InventoryFreeze_Freeze")]
        public JsonResult _Freeze(string item, string location, string lotNo, string manufactureParty)
        {
            try
            {
                if (item == null || item == "")
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ItemCanNotBeEmpty);
                }
                if (location == null || location == "")
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_LocationCanNotBeEmpty);
                }
                if (lotNo == null || lotNo == "")
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BatchNumberCanNotBeEmpty);
                }
                LocDetGeneMgr.InventoryFreeze(item, location, lotNo, manufactureParty);
                object obj = new { SuccessMessage = string.Format(Resources.INV.LocationLotDetail.LocationLotDetail_Freezed, item) };
                return Json(obj);
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }

        [SconitAuthorize(Permissions = "Url_Inventory_InventoryFreeze_UnFreeze")]
        public JsonResult _UnFreeze(string item, string location, string lotNo, string manufactureParty)
        {
            try
            {
                if (item == null || item == "")
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ItemCanNotBeEmpty);
                }
                if (location == null || location == "")
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_LocationCanNotBeEmpty);
                }
                if (lotNo == null || lotNo == "")
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BatchNumberCanNotBeEmpty);
                }
                LocDetGeneMgr.InventoryUnFreeze(item, location, lotNo, manufactureParty);
                object obj = new { SuccessMessage = string.Format(Resources.INV.LocationLotDetail.LocationLotDetail_UnFreezed, item) };
                return Json(obj);
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }

        #endregion

        #region private
        private SearchStatementModel PrepareSearchStatement(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            string whereStatement = " where HuId is not null";

            IList<object> param = new List<object>();
            SecurityHelper.AddLocationPermissionStatement(ref whereStatement, "l", "Location");
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LotNo", searchModel.LotNo, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ManufactureParty", searchModel.ManufactureParty, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsFreeze", searchModel.IsFreeze, "l", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

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
