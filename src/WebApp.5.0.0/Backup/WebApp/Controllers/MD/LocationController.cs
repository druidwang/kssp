/// <summary>
/// Summary description for LocationController
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
    /// This controller response to control the Location.
    /// </summary>
    public class LocationController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the Location security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        #region hql
        /// <summary>
        /// hql to get count of the location
        /// </summary>
        private static string selectCountStatement = "select count(*) from Location as u";

        /// <summary>
        /// hql to get all of the location
        /// </summary>
        private static string selectStatement = "select u from Location as u";

        /// <summary>
        /// hql to get count of the location by location's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from Location as u where u.Code = ?";

        /// <summary>
        /// hql to get count of the LocationBin
        /// </summary>
        private static string locationBinselectCountStatement = "select count(*) from LocationBin as l";

        /// <summary>
        /// hql to get total of the LocationBin
        /// </summary>
        private static string locationBinselectStatement = "select l from LocationBin as l";

        /// <summary>
        /// hql to get count of the LocationBin by code
        /// </summary>
        private static string locationBinDuiplicateVerifyStatement = @"select count(*) from LocationBin as l where l.Code = ?";

        /// <summary>
        /// hql to get count of the locationArea 
        /// </summary>
        private static string locationAreaselectCountStatement = "select count(*) from LocationArea as l";

        /// <summary>
        /// hql to get all of the locationArea 
        /// </summary>
        private static string locationAreaselectStatement = "select l from LocationArea as l";

        /// <summary>
        /// hql to get count of the locationArea by locationArea's code
        /// </summary>
        private static string locationAreaDuiplicateVerifyStatement = @"select count(*) from LocationArea as l where l.Code = ?";

        #endregion
        #region location
        /// <summary>
        /// Index action for Location controller
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List acion
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Location Search Model</param>
        /// <returns>return to the result action</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult List(GridCommand command, LocationSearchModel searchModel)
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
        /// <param name="searchModel">Location Search Model</param>
        /// <returns>return to the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _AjaxList(GridCommand command, LocationSearchModel searchModel)
        {
            searchModel.IsActive = true;
            if (searchModel.IsIncludeInActive)
            {
                searchModel.IsActive = false;
            }
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Location>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_Location_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="location">loction model</param>
        /// <returns>return to edit view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Location_Edit")]
        public ActionResult New(Location location)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { location.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, location.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(location);
                    SaveSuccessMessage(Resources.MD.Location.Location_Added);
                    return RedirectToAction("Edit/" + location.Code);
                }
            }

            return View(location);
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">location id for edit</param>
        /// <returns>return to edit view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.LocationCode = id;
                return View("Edit", string.Empty, id);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">location id for edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                Location location = this.genericMgr.FindById<Location>(id);
                return PartialView(location);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="location">location model</param>
        /// <returns>return to edit action</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Location_Edit")]
        public ActionResult _Edit(Location location)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(location);
                SaveSuccessMessage(Resources.MD.Location.Location_Updated);
            }

            return PartialView(location);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">loction id for delete</param>
        /// <returns>return to list action</returns>
        [SconitAuthorize(Permissions = "Url_Location_Delete")]
        public ActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    this.genericMgr.DeleteById<Location>(id);
                    SaveSuccessMessage(Resources.MD.Location.Location_Deleted);
                    return RedirectToAction("List");
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_DeletedUnsuccessfully);
                return RedirectToAction("List");
            }
        }
        #endregion

        #region LocationArea
        /// <summary>
        /// _LocationArea action
        /// </summary>
        /// <param name="id">Location Code</param>
        /// <returns>rediret view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _LocationArea(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.LocationCode = id;
                return PartialView();
            }
        }

        /// <summary>
        /// LocationAreaList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Location Search Model</param>
        /// <param name="locationCode">location Code</param>
        /// <returns>return to the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _LocationAreaList(GridCommand command, LocationAreaSearchModel searchModel, string locationCode)
        {

            ViewBag.LocationCode = locationCode;
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = this.LocationAreaPrepareSearchStatement(command, (LocationAreaSearchModel)searchCacheModel.SearchObject, locationCode);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            return PartialView(GetPageData<LocationArea>(searchStatementModel, command));
        }

        /// <summary>
        /// AjaxLocationAreaList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">LocationArea Search Model</param>
        /// <param name="locationCode">Location Code</param>
        /// <returns>return to the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _AjaxLocationAreaList(GridCommand command, LocationAreaSearchModel searchModel, string locationCode)
        {
            string replaceFrom = "_AjaxLocationAreaList";
            string replaceTo = "_LocationAreaList/";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.LocationAreaPrepareSearchStatement(command, searchModel, locationCode);
            return PartialView(GetAjaxPageData<LocationArea>(searchStatementModel, command));
        }

        /// <summary>
        /// LocationAreaNew action
        /// </summary>
        /// <param name="locationCode">Location Code</param>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_LocationAreaBin_Edit")]
        public ActionResult _LocationAreaNew(string locationCode)
        {
            ViewBag.LocationCode = locationCode;
            return PartialView();
        }

        /// <summary>
        /// LocationAreaNew action
        /// </summary>
        /// <param name="locationArea">LocationArea Model</param>
        /// <param name="locationCode">Location Code</param>
        /// <returns>return to the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_LocationAreaBin_Edit")]
        public ActionResult _LocationAreaNew(LocationArea locationArea, string locationCode)
        {
            locationArea.Location = locationCode;
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(locationAreaDuiplicateVerifyStatement, new object[] { locationArea.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, locationArea.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(locationArea);
                    SaveSuccessMessage(Resources.MD.LocationArea.LocationArea_Added);
                    return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_LocationAreaList" }, 
                                                       { "controller", "Location" },
                                                       { "LocationCode", locationCode }
                                                   });
                }
            }
            ViewBag.LocationCode = locationCode;
            return PartialView(locationArea);
        }

        /// <summary>
        /// LocationAreaEdit action
        /// </summary>
        /// <param name="id">LocationArea id for Edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _LocationAreaEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                LocationArea locationArea = this.genericMgr.FindById<LocationArea>(id);
                return PartialView(locationArea);
            }
        }

        /// <summary>
        /// LocationAreaEdit action
        /// </summary>
        /// <param name="locationArea">LocationArea Model</param>
        /// <returns>return to LocationAreaEdit action</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_LocationAreaBin_Edit")]
        public ActionResult _LocationAreaEdit(LocationArea locationArea)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(locationArea);
                SaveSuccessMessage(Resources.MD.LocationArea.LocationArea_Updated);
            }

            return PartialView(locationArea);
        }

        /// <summary>
        /// LocationAreaDelete action
        /// </summary>
        /// <param name="id">LocationArea id for delete</param>
        /// <param name="locationCode">location Code</param>
        /// <returns>return to LocationAreaList action</returns>
        [SconitAuthorize(Permissions = "Url_Location_Delete")]
        public ActionResult _LocationAreaDelete(string id, string locationCode)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    //this.genericMgr.DeleteById<LocationArea>(id);
                    SaveSuccessMessage(Resources.MD.LocationArea.LocationArea_Deleted);
                    return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_LocationAreaList" }, 
                                                       { "controller", "Location" },
                                                       { "LocationCode", locationCode }
                                                   });
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_DeletedUnsuccessfully);
                return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_LocationAreaList" }, 
                                                       { "controller", "Location" },
                                                       { "LocationCode", locationCode }
                                                   });
            }
        }
        #endregion

        #region LocationBin
        /// <summary>
        /// LocationBin action
        /// </summary>
        /// <param name="id">Location Code</param>
        /// <returns>rediret view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _LocationBin(string id)
        {
            ViewBag.LocationCode = id;
            return PartialView();
        }

        /// <summary>
        /// LocationBinList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">LocationBin Search Model</param>
        /// <param name="locationCode">location Code</param>
        /// <returns>return to the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _LocationBinList(GridCommand command, LocationBinSearchModel searchModel, string locationCode)
        {
            ViewBag.LocationCode = locationCode;
            //SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            //SearchStatementModel searchStatementModel = this.LocationBinPrepareSearchStatement(command, (LocationBinSearchModel)searchCacheModel.SearchObject, locationCode);
            //return PartialView(GetPageData<LocationBin>(searchStatementModel, command));
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            return PartialView();
        }

        /// <summary>
        /// LocationBinList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">LocationBin Search Model</param>
        /// <param name="locationCode">location Code</param>
        /// <returns>return to the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _AjaxLocationBinList(GridCommand command, LocationBinSearchModel searchModel, string locationCode)
        {
            string replaceFrom = "_AjaxLocationBinList";
            string replaceTo = "_LocationBinList/";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.LocationBinPrepareSearchStatement(command, searchModel, locationCode);
            return PartialView(GetAjaxPageData<LocationBin>(searchStatementModel, command));
        }

        /// <summary>
        /// LocationBinNew action
        /// </summary>
        /// <param name="locationCode">Location Code</param>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_LocationAreaBin_Edit")]
        public ActionResult _LocationBinNew(string locationCode)
        {
            ViewBag.LocationCode = locationCode;
            return PartialView();
        }

        /// <summary>
        /// LocationBinNew action
        /// </summary>
        /// <param name="locationBin">LocationBin Model</param>
        /// <param name="locationCode">location Code</param>
        /// <returns>return to the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_LocationAreaBin_Edit")]
        public ActionResult _LocationBinNew(LocationBin locationBin, string locationCode)
        {
            locationBin.Location = locationCode;
            ViewBag.LocationCode = locationCode;
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(locationBinDuiplicateVerifyStatement, new object[] { locationBin.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, locationBin.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(locationBin);
                    SaveSuccessMessage(Resources.MD.LocationBin.LocationBin_Added);
                    return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_LocationBinList" }, 
                                                       { "controller", "Location" },
                                                       { "LocationCode", locationCode }
                                                   });
                }
            }

            return PartialView(locationBin);
        }

        /// <summary>
        /// LocationBinEdit action
        /// </summary>
        /// <param name="id">LocationBin id for Edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Location_View")]
        public ActionResult _LocationBinEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                LocationBin locationBin = this.genericMgr.FindById<LocationBin>(id);
                return PartialView(locationBin);
            }
        }

        /// <summary>
        /// LocationBinEdit action
        /// </summary>
        /// <param name="locationBin">LocationBin Model</param>
        /// <returns>return to the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_LocationAreaBin_Edit")]
        public ActionResult _LocationBinEdit(LocationBin locationBin)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(locationBin);
                SaveSuccessMessage(Resources.MD.LocationBin.LocationBin_Updated);
            }

            return PartialView(locationBin);
        }

        /// <summary>
        /// LocationBinDelete action
        /// </summary>
        /// <param name="id">LocationBin id for delete</param>
        /// <param name="locationCode">location Code</param>
        /// <returns>return to the result view</returns>
        [SconitAuthorize(Permissions = "Url_Location_Delete")]
        public ActionResult _LocationBinDelete(string id, string locationCode)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                //this.genericMgr.DeleteById<LocationBin>(id);
                SaveSuccessMessage(Resources.MD.LocationBin.LocationBin_Deleted);
                return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_LocationBinList" }, 
                                                       { "controller", "Location" },
                                                       { "LocationCode", locationCode }
                                                   });
            }
        }
        #endregion
        #region  Export Bin search
        [SconitAuthorize(Permissions = "Url_Location_View")]
        [GridAction(EnableCustomBinding = true)]
        public void ExportBin(LocationBinSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return;
            }
            SearchStatementModel searchStatementModel = this.LocationBinPrepareSearchStatement(command, searchModel, searchModel.LocationCode);
            ExportToXLS<LocationBin>("LocationBin.xls", GetAjaxPageData<LocationBin>(searchStatementModel, command).Data.ToList());
        }
        #endregion
        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">LocationBin Search Model</param>
        /// <param name="locationCode">location Code</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel LocationBinPrepareSearchStatement(GridCommand command, LocationBinSearchModel searchModel, string locationCode)
        {
            string whereStatement = "where l.Location = '" + locationCode + "'";

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "l", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Start, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Area", searchModel.Area, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Sequence", searchModel.Sequence, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "l", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = locationBinselectCountStatement;
            searchStatementModel.SelectStatement = locationBinselectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">LocationArea Search Model</param>
        /// <param name="locationCode">location Code</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel LocationAreaPrepareSearchStatement(GridCommand command, LocationAreaSearchModel searchModel, string locationCode)
        {
            string whereLocationStatement = "where l.Location = '" + locationCode + "'";
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "l", ref whereLocationStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Start, "l", ref whereLocationStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = locationAreaselectCountStatement;
            searchStatementModel.SelectStatement = locationAreaselectStatement;
            searchStatementModel.WhereStatement = whereLocationStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Location Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, LocationSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("SAPLocation", searchModel.SAPLocation, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Region", searchModel.Region, "u", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "u", ref whereStatement, param);
            if (searchModel.AllowNegaInv)
            {
                HqlStatementHelper.AddEqStatement("AllowNegative", searchModel.AllowNegaInv, "u", ref whereStatement, param);
            }

            if (searchModel.IsMRP)
            {
                HqlStatementHelper.AddEqStatement("IsMRP", searchModel.IsMRP, "u", ref whereStatement, param);
            }

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
