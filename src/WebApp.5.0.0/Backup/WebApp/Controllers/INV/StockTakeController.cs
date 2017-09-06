/// <summary>
/// Summary description for AddressController
/// </summary>
namespace com.Sconit.Web.Controllers.INV
{
    #region reference
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.INV;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.Exception;
    using Telerik.Web.Mvc.UI;
    using System.Web;
    using System.IO;
    using com.Sconit.PrintModel.INV;
    using AutoMapper;
    using com.Sconit.Utility.Report;
    using System;
    using com.Sconit.Entity.VIEW;
    using NHibernate.Type;
    #endregion

    /// <summary>
    /// This controller response to control the StockTake.
    /// </summary>
    public class StockTakeController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.genericMgr which main consider the StockTake security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        public IStockTakeMgr stockTakeMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        //public IReportGen reportGen { get; set; }
        //public IImportMgr ImportMgr { get; set; }

        #endregion

        #region hql
        /// <summary>
        /// hql to get count of the StockTakeMaster
        /// </summary>
        private static string selectCountStatement = "select count(*) from StockTakeMaster as s";

        /// <summary>
        /// hql to get all of the StockTakeMaster
        /// </summary>
        private static string selectStatement = "select s from StockTakeMaster as s";

        /// <summary>
        /// hql to get count of the StockTakeResult
        /// </summary>
        private static string selectStockTakeResultCountStatement = "select count(*) from StockTakeResult as s";

        /// <summary>
        /// hql to get all of the StockTakeResult
        /// </summary>
        private static string selectStockTakeResultStatement = "select s from StockTakeResult s";

        #endregion

        #region StockTakeMaster


        /// <summary>
        /// Index action for StockTakeMaster controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult _ExistLocHuIdChk(string StNo)
        {
            ViewBag.StNo = StNo;
            return PartialView();
        }
        public ActionResult _ExistLocHuIdChkList(GridCommand command, string stNo)
        {
            ViewBag.StNo = stNo;
            return PartialView();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public ActionResult _SelectExistLocHuIdDetail(string stNo)
        {
            IList<StockTakeResult> needToDeleteStDet = this.genericMgr.FindEntityWithNativeSql<StockTakeResult>(@"
                select b.* from VIEW_LocationLotDet a , INV_StockTakeResult b   where  b.StNo = ? and a.HuId =b.HuId and a.Location !=b.Location  ", stNo);
            IList<LocationLotDetail> existsInLocationHuLists = new List<LocationLotDetail>();
            if (needToDeleteStDet.Count > 0)
            {
                existsInLocationHuLists = this.genericMgr.FindEntityWithNativeSql<LocationLotDetail>(@"
                select a.* from VIEW_LocationLotDet a , INV_StockTakeDet b   where  b.StNo = ? and a.HuId =b.HuId and a.Location !=b.Location ", stNo);
            }
            return PartialView(new GridModel(existsInLocationHuLists));
        }
        [HttpPost]
        public JsonResult _DeleteStHuId(string stNo)
        {
            try
            {
                IList<StockTakeResult> needToDeleteStDet = this.genericMgr.FindEntityWithNativeSql<StockTakeResult>(@"
                select b.* from VIEW_LocationLotDet a , INV_StockTakeResult b   where  b.StNo = ? and a.HuId =b.HuId and a.Location !=b.Location  ", stNo);
                
                
                
                if (needToDeleteStDet.Count == 0)
                {
                    return Json(new { SuccessMessage = Resources.EXT.ControllerLan.Con_LackExitedStockTakeResult });
                }
                {
                    this.genericMgr.Delete<StockTakeResult>(needToDeleteStDet);
                    return Json(new { SuccessMessage = Resources.EXT.ControllerLan.Con_ExitedStockTakeResultIsDeleted });
                }
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }
        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">StockTakeMaster Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public ActionResult StockTakeMasterList(GridCommand command, StockTakeMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">StockTakeMaster Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public ActionResult _AjaxStockTakeMasterList(GridCommand command, StockTakeMasterSearchModel searchModel)
        {
            string replaceFrom = "_AjaxStockTakeMasterList";
            string replaceTo = "StockTakeMasterList";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);

            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<StockTakeMaster>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="address">StockTakeMaster Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult New(StockTakeMaster stockTakeMaster)
        {
            ModelState.Remove("StNo");
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(stockTakeMaster.Region))
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_AreaCanNotBeEmpty);
                }
                else
                {
                    //页面不加生效日期
                    stockTakeMaster.EffectiveDate = DateTime.Now;
                    
                    stockTakeMaster.Status = Sconit.CodeMaster.StockTakeStatus.Create;
                    this.stockTakeMgr.CreateStockTakeMaster(stockTakeMaster);
                    SaveSuccessMessage(Resources.INV.StockTake.StockTakeMaster_Added);
                    return RedirectToAction("Edit/" + stockTakeMaster.StNo);
                }
            }
            return View(stockTakeMaster);
        }

        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                StockTakeMaster StockTakeMaster = genericMgr.FindById<StockTakeMaster>(id);
                return View("Edit", string.Empty, StockTakeMaster);
            }



        }

        /// <summary>
        /// StockTakeMasterEdit view
        /// </summary>
        /// <param name="id">StockTakeMaster id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _StockTakeMasterEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(id);
                stockTakeMaster.TypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.StockTakeType, ((int)stockTakeMaster.Type).ToString());
                stockTakeMaster.StockTakeStatusDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString());
                return PartialView(stockTakeMaster);
            }
        }

        #region Location
        /// <summary>
        /// StockTakeLocation action
        /// </summary>
        /// <param name="region">StockTakeLocation region for StockTakeLocation</param>
        /// <returns>return the StockTakeLocation result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _StockTakeLocation(string stNo)
        {
            ViewBag.stNo = stNo;
            StockTakeMaster stockStakeMaster = genericMgr.FindById<StockTakeMaster>(stNo);
            ViewBag.Status = stockStakeMaster.Status;
            if (stockStakeMaster.Status == com.Sconit.CodeMaster.StockTakeStatus.Create)
            {
                ViewData["locations"] = genericMgr.FindAll<Location>("from Location where IsActive = 1 and Region = ?", stockStakeMaster.Region);
            }
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _Select(string stNo)
        {
            IList<StockTakeLocation> stockTakeLocationList = genericMgr.FindAll<StockTakeLocation>("from StockTakeLocation as s where s.StNo=?", stNo);
            return PartialView(new GridModel(stockTakeLocationList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _InsertAjaxEditing(string stNo, string Location, string bins)
        {
            if (ModelState.IsValid)
            {
                IList<StockTakeLocation> stockTakeLocationByLoc = genericMgr.FindAll<StockTakeLocation>
                    ("from StockTakeLocation as s where s.Location=? and s.StNo=?", new object[] { Location, stNo });
                if (stockTakeLocationByLoc.Count > 0)
                {
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_LocationAlreadyExists);
                }
                else
                {
                    string inValidBin = string.Empty;
                    Location location = genericMgr.FindById<Location>(Location);
                    if (!string.IsNullOrWhiteSpace(bins))
                    {
                        var binList = bins.Split(',').ToList();
                        var locationBins = this.genericMgr.FindAll<LocationBin>
                            ("from LocationBin where Location =?", Location)
                            .Select(p => p.Code).ToList();
                        foreach (var bin in binList)
                        {
                            if (!locationBins.Contains(bin))
                            {
                                if (inValidBin == string.Empty)
                                {
                                    inValidBin = bin;
                                }
                                else
                                {
                                    inValidBin += "," + bin;
                                }
                            }
                        }
                    }
                    if (inValidBin == string.Empty)
                    {
                        StockTakeLocation stLocation = new StockTakeLocation();
                        stLocation.Location = Location;
                        stLocation.LocationName = location.Name;
                        stLocation.StNo = stNo;
                        stLocation.Bins = bins;
                        genericMgr.CreateWithTrim(stLocation);
                        SaveSuccessMessage(Resources.INV.StockTakeLocation.StockTakeLocation_Added);
                    }
                    else
                    {
                        SaveSuccessMessage(Resources.EXT.ControllerLan.Con_BinIsNotInLocation, inValidBin, Location);
                    }
                }
            }

            IList<StockTakeLocation> stockTakeLocationList = genericMgr.FindAll<StockTakeLocation>
                ("from StockTakeLocation as s where s.StNo=?", stNo);
            return PartialView(new GridModel(stockTakeLocationList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _DeleteLocation(string stNo, string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<StockTakeLocation>(int.Parse(Id));
                SaveSuccessMessage(Resources.INV.StockTakeLocation.StockTakeLocation_Deleted);
            }
            IList<StockTakeLocation> stockTakeLocationList = genericMgr.FindAll<StockTakeLocation>("from StockTakeLocation as s where s.StNo=?", stNo);
            return PartialView(new GridModel(stockTakeLocationList));
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _UpdateLocation(StockTakeLocation stLocation)
        {
            IList<StockTakeLocation> isEmpty = genericMgr.FindAll<StockTakeLocation>
                ("from StockTakeLocation as s where s.Location=? and s.StNo=? and Id <> ? ",
                new object[] { stLocation.Location, stLocation.StNo, stLocation.Id });
            if (isEmpty.Count > 0)
            {
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_LocationAlreadyExistsPleaseReselect);
            }
            else
            {
                string inValidBin = string.Empty;
                if (!string.IsNullOrWhiteSpace(stLocation.Bins))
                {
                    var binList = stLocation.Bins.Split(',').ToList();
                    var locationBins = this.genericMgr.FindAll<LocationBin>
                        ("from LocationBin where Location =?", stLocation.Location)
                        .Select(p => p.Code).ToList();
                    foreach (var bin in binList)
                    {
                        if (!locationBins.Contains(bin))
                        {
                            if (inValidBin == string.Empty)
                            {
                                inValidBin = bin;
                            }
                            else
                            {
                                inValidBin += "," + bin;
                            }
                        }
                    }
                }

                if (inValidBin == string.Empty)
                {
                    Location location = genericMgr.FindById<Location>(stLocation.Location);
                    stLocation.LocationName = location.Name;
                    stLocation.Bins = stLocation.Bins;
                    genericMgr.UpdateWithTrim(stLocation);
                    SaveSuccessMessage(Resources.INV.StockTakeLocation.StockTakeLocation_Updated);
                }
                else
                {
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_BinIsNotInLocation, inValidBin, stLocation.Location);
                }
            }

            IList<StockTakeLocation> stockTakeLocationList = genericMgr.FindAll<StockTakeLocation>("from StockTakeLocation as s where s.StNo=?", stLocation.StNo);
            return PartialView(new GridModel(stockTakeLocationList));
        }

        #endregion
        #region Item
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _StockTakeItem(string stNo, string Status)
        {
            ViewBag.stNo = stNo;
            ViewBag.Status = Status;
            // ViewData["Item"] = genericMgr.FindAll<Item>();
            return PartialView();
        }


        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _SelectItem(string stNo)
        {
            IList<StockTakeItem> stockTakeItem = genericMgr.FindAll<StockTakeItem>("from StockTakeItem as s where s.StNo=?", stNo);
            return PartialView(new GridModel(stockTakeItem));
        }

        public string _ItemDescription(string Item)
        {
            if (!string.IsNullOrEmpty(Item))
            {
                Item item = genericMgr.FindById<Item>(Item);
                if (item != null)
                {
                    return item.Description;
                }

            }
            return null;
        }

        public string _LocationDescription(string Location)
        {
            if (!string.IsNullOrEmpty(Location))
            {
                Location location = genericMgr.FindById<Location>(Location);
                if (location != null)
                {
                    return location.Name;
                }

            }
            return null;
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _InsertItem(string stNo, string Item)
        {
            if (ModelState.IsValid)
            {
                IList<StockTakeItem> stockTakeByItem = genericMgr.FindAll<StockTakeItem>("from StockTakeItem as s where s.Item=? and s.StNo=?", new object[] { Item, stNo });
                if (stockTakeByItem.Count > 0)
                {
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_PartAlreadyExists);
                }
                else
                {
                    Item item = genericMgr.FindById<Item>(Item);
                    StockTakeItem stItem = new StockTakeItem();
                    stItem.Item = item.Code;
                    stItem.ItemDescription = item.Description;
                    stItem.StNo = stNo;
                    genericMgr.CreateWithTrim(stItem);
                    SaveSuccessMessage(Resources.INV.StockTakeItem.StockTakeItem_Added);
                }
            }
            IList<StockTakeItem> stockTakeItem = genericMgr.FindAll<StockTakeItem>("from StockTakeItem as s where s.StNo=?", stNo);
            return PartialView(new GridModel(stockTakeItem));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _DeleteItem(string stNo, string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<StockTakeItem>(int.Parse(Id));
                SaveSuccessMessage(Resources.INV.StockTakeItem.StockTakeItem_Deleted);
            }
            IList<StockTakeItem> stockTakeItem = genericMgr.FindAll<StockTakeItem>("from StockTakeItem as s where s.StNo=?", stNo);
            return PartialView(new GridModel(stockTakeItem));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _UpdateItem(StockTakeItem stItem)
        {
            IList<StockTakeItem> isEmpty = genericMgr.FindAll<StockTakeItem>("from StockTakeItem as s where s.Item=? and s.StNo=?", new object[] { stItem.Item, stItem.StNo });
            if (isEmpty.Count > 0)
            {
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_PartAlreadyExists);
            }
            else
            {
                Item item = genericMgr.FindById<Item>(stItem.Item);
                //  stItem = genericMgr.FindById<StockTakeItem>(stItem.Id);
                stItem.ItemDescription = item.CodeDescription;
                genericMgr.UpdateWithTrim(stItem);
                SaveSuccessMessage(Resources.INV.StockTakeItem.StockTakeItem_Updated);
            }

            IList<StockTakeItem> stockTakeLocationList = genericMgr.FindAll<StockTakeItem>("from StockTakeItem as s where s.StNo=?", stItem.StNo);
            return PartialView(new GridModel(stockTakeLocationList));
        }

        #endregion
        #region edit

        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult btnDelete(string id)
        {
            try
            {
                StockTakeMaster StockTakeMaster = genericMgr.FindById<StockTakeMaster>(id);
                stockTakeMgr.DeleteStockTakeMaster(StockTakeMaster);
                SaveSuccessMessage(Resources.INV.StockTake.StockTakeMaster_Deleted);
                return RedirectToAction("StockTakeMasterList");
            }
            catch (BusinessException ex)
            {

                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                return RedirectToAction("Edit/" + id);
            }

        }


        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult btnSubmit(string id)
        {

            try
            {
                StockTakeMaster StockTakeMaster = genericMgr.FindById<StockTakeMaster>(id);
                stockTakeMgr.ReleaseStockTakeMaster(StockTakeMaster);
                SaveSuccessMessage(Resources.INV.StockTake.StockTakeMaster_Submit);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit/" + id);
        }


        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult btnStart(string id)
        {

            try
            {
                StockTakeMaster StockTakeMaster = genericMgr.FindById<StockTakeMaster>(id);
                stockTakeMgr.StartStockTakeMaster(StockTakeMaster);
                SaveSuccessMessage(Resources.INV.StockTake.StockTakeMaster_Start);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit/" + id);
        }

        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult btnCancel(string id)
        {

            try
            {
                StockTakeMaster StockTakeMaster = genericMgr.FindById<StockTakeMaster>(id);
                stockTakeMgr.CancelStockTakeMaster(StockTakeMaster);
                SaveSuccessMessage(Resources.INV.StockTake.StockTakeMaster_Cancel);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit/" + id);
        }

        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult btnComplete(StockTakeMaster stockTakeMaster)
        {

            // StockTakeMaster stockTakeMaster = (StockTakeMaster)id;
            try
            {
                StockTakeMaster StockTakeMaster = genericMgr.FindById<StockTakeMaster>(stockTakeMaster.StNo);
                stockTakeMgr.CompleteStockTakeMaster(StockTakeMaster, stockTakeMaster.BaseInventoryDate);
                SaveSuccessMessage(Resources.INV.StockTake.StockTakeMaster_Complete);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit/" + stockTakeMaster.StNo);
        }

        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult btnClose(string id)
        {

            try
            {
                StockTakeMaster StockTakeMaster = genericMgr.FindById<StockTakeMaster>(id);
                stockTakeMgr.ManualCloseStockTakeMaster(StockTakeMaster);
                SaveSuccessMessage(Resources.INV.StockTake.StockTakeMaster_Close);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit/" + id);
        }
        #endregion
        #endregion

        #region  StockTakeDetail

        /// <summary>
        /// StockTakeDetailSearch action
        /// </summary>
        /// <param name="id">StockTakeMaster Code</param>
        /// <returns>rediret view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _StockTakeDetailSearch(string id, string Status, bool IsScanHu)
        {
            //StockTakeDetail stockTakeDetail = this.genericMgr.FindById<StockTakeDetail>(id);
            ViewBag.IsScanHu = IsScanHu;
            ViewBag.StNo = id;
            ViewBag.Status = Status;

            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _StockTakeDetail(GridCommand command, StockTakeDetailSearchModel searchModel, string Status, bool IsScanHu)
        {
            ViewBag.ItemCode = searchModel.ItemCode;
            ViewBag.StNo = searchModel.StNo;
            ViewBag.IsScanHu = IsScanHu;
            ViewBag.Status = Status;
            ViewData["Uom"] = genericMgr.FindAll<Uom>();
            TempData["StockTakeDetailSearchModel"] = searchModel;
            return PartialView();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public ActionResult _AjaxStockTakeDetail(GridCommand command, StockTakeDetailSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.StockTakeDetailPrepareSearchStatement(command, searchModel);
            var lists = GetAjaxPageData<StockTakeDetail>(searchStatementModel, command);
            var itemlist = itemMgr.GetCacheAllItem();
            foreach (var list in lists.Data)
            {
                list.QualityTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.QualityType, ((int)list.QualityType).ToString());
                list.ItemDescription = itemlist.ValueOrDefault(list.Item).FullDescription;
            }
            return PartialView(lists);
        }
        #region
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public void ExportSTDetailXLS(StockTakeDetailSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            //Set "LocationBin" value to null from "undefined" since "LocationBin" ActiveX was not created when StNo needn't scan barcode.
            if (!searchModel.IsScanHu) searchModel.LocationBin = null;
            SearchStatementModel searchStatementModel = this.StockTakeDetailPrepareSearchStatement(command, searchModel);
            GridModel<StockTakeDetail> gridModel = GetAjaxPageData<StockTakeDetail>(searchStatementModel, command);
            foreach (var list in gridModel.Data)
            {
                list.QualityTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.QualityType, ((int)list.QualityType).ToString());
            }
            var fileName = string.Format("StokcTakeDetails.{0}.xls", searchModel.StNo);
            if (searchModel.IsScanHu)
            {
                fileName = string.Format("StokcTakeDetailsScanHu.{0}.xls", searchModel.StNo);
            }
            ExportToXLS<StockTakeDetail>(fileName, gridModel.Data.ToList());
        }

        #endregion
        public ActionResult _WebBomDetail(string itemCode)
        {
            WebOrderDetail webOrderDetail = new WebOrderDetail();

            Item item = genericMgr.FindById<Item>(itemCode);
            if (item != null)
            {
                webOrderDetail.Item = item.Code;
                webOrderDetail.ItemDescription = item.Description;
                webOrderDetail.Uom = item.Uom;
            }
            return this.Json(webOrderDetail);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public JsonResult _SaveStockTakeDetailBatchEditing([Bind(Prefix =
            "inserted")]IEnumerable<StockTakeDetail> insertedStockTakeDetails,
            [Bind(Prefix = "updated")]IEnumerable<StockTakeDetail> updatedStockTakeDetails,
            [Bind(Prefix = "deleted")]IEnumerable<StockTakeDetail> deletedStockTakeDetails, string id)
        {

            try
            {
                IList<StockTakeDetail> newStockTakeDetailList = new List<StockTakeDetail>();
                IList<StockTakeDetail> updateStockTakeDetailList = new List<StockTakeDetail>();
                if (insertedStockTakeDetails != null)
                {
                    foreach (var stockTakeDetail in insertedStockTakeDetails)
                    {
                        if (stockTakeDetail.Qty < 0)
                        {
                            throw new BusinessException(Resources.EXT.ControllerLan.Con_QuantityMustGreaterOrEqual);

                        }
                        else if (stockTakeDetail.Item == null)
                        {
                            throw new BusinessException(Resources.EXT.ControllerLan.Con_ItemCanNotBeEmpty);

                        }
                        else if (stockTakeDetail.Location == null)
                        {
                            throw new BusinessException(Resources.EXT.ControllerLan.Con_LocationCanNotBeEmpty);
                        }
                        else
                        {
                            newStockTakeDetailList.Add(stockTakeDetail);
                        }
                    }
                }
                if (updatedStockTakeDetails != null)
                {
                    foreach (var stockTakeDetail in updatedStockTakeDetails)
                    {
                        if (stockTakeDetail.Qty < 0)
                        {
                            throw new BusinessException(Resources.EXT.ControllerLan.Con_QuantityMustGreaterOrEqual);

                        }
                        else if (stockTakeDetail.Item == null)
                        {
                            throw new BusinessException(Resources.EXT.ControllerLan.Con_ItemCanNotBeEmpty);

                        }
                        else if (stockTakeDetail.Location == null)
                        {
                            throw new BusinessException(Resources.EXT.ControllerLan.Con_LocationCanNotBeEmpty);
                        }
                        else
                        {
                            updateStockTakeDetailList.Add(stockTakeDetail);
                        }
                    }
                }

                stockTakeMgr.BatchUpdateStockTakeDetails(id, newStockTakeDetailList, updateStockTakeDetailList, (IList<StockTakeDetail>)deletedStockTakeDetails);
                object obj = new { SuccessMessage = string.Format(Resources.INV.StockTake.StockTakeDetail_Saved, id) };
                return Json(obj);

                //}
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }

        }

        public JsonResult _DeleteStockTakeDetail(string Ids, string stNo)
        {
            string[] IdArray = Ids.Split(',');
            int[] ids = new int[IdArray.Length];
            IList<StockTakeDetail> StockTakeDetailDeleteList = new List<StockTakeDetail>();
            foreach (var item in IdArray)
            {
                StockTakeDetail stockTakeDetail = this.genericMgr.FindById<StockTakeDetail>(int.Parse(item));
                StockTakeDetailDeleteList.Add(stockTakeDetail);
            }

            try
            {
                stockTakeMgr.BatchUpdateStockTakeDetails(stNo, null, null, StockTakeDetailDeleteList);
                object obj = new { SuccessMessage = string.Format(Resources.INV.StockTake.StockTakeDetail_Saved, stNo) };
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

        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult ImportStcokTakeDetail(IEnumerable<HttpPostedFileBase> attachments, string stNo)
        {
            try
            {
                foreach (var file in attachments)
                {
                    stockTakeMgr.ImportStockTakeDetailFromXls(file.InputStream, stNo);
                }
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_StockTakeDetailSuccessfullyImport);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Content("");
        }

        #endregion

        #region  StockTakeResult

        /// <summary>
        /// StockTakeDetailSearch action
        /// </summary>
        /// <param name="id">StockTakeMaster Code</param>
        /// <returns>rediret view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _StockTakeResultSearch(string id, string Status, bool IsScanHu)
        {
            ViewBag.StNo = id;
            ViewBag.Status = Status;
            ViewBag.IsScanHu = IsScanHu;
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_New")]
        public ActionResult _StockTakeResult(GridCommand command, StockTakeResultSearchModel searchModel, bool IsScanHu)
        {
            ViewBag.Item = searchModel.Item;
            ViewBag.StNo = searchModel.StNo;
            ViewBag.IsScanHu = IsScanHu;
            ViewBag.Status = genericMgr.FindById<StockTakeMaster>(searchModel.StNo).Status.ToString();
            TempData["StockTakeResultSearchModel"] = searchModel;
            return PartialView();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public ActionResult _AjaxStockTakeResult(GridCommand command, StockTakeResultSearchModel searchModel)
        {
            //SearchStatementModel searchStatementModel = this.StockTakeResultPrepareSearchStatement(command, searchModel);
            //return PartialView(GetAjaxPageData<StockTakeResult>(searchStatementModel, command));
            IList<string> locationList = new List<string>();
            if (searchModel.Location != null && searchModel.Location != string.Empty)
            {
                locationList.Add(searchModel.Location);
            }
            IList<string> binList = new List<string>();
            if (searchModel.LocationBin != null && searchModel.LocationBin != string.Empty)
            {
                binList.Add(searchModel.LocationBin);
            }
            IList<string> itemList = new List<string>();
            if (searchModel.Item != null && searchModel.Item != string.Empty)
            {
                itemList.Add(searchModel.Item);
            }
            IList<StockTakeResultSummary> stockTakeResultSummaryList = stockTakeMgr.ListStockTakeResult(searchModel.StNo, searchModel.IsLoss, searchModel.IsProfit, searchModel.IsBreakEven, locationList, binList, itemList, null);
            FillCodeDetailDescription<StockTakeResultSummary>(stockTakeResultSummaryList);
            var itemlist = itemMgr.GetCacheAllItem();
            foreach (var StockTakeResult in stockTakeResultSummaryList)
            {
                StockTakeResult.ItemDescription = itemlist.ValueOrDefault(StockTakeResult.Item).FullDescription;
            }
            return PartialView(new GridModel(stockTakeResultSummaryList));
        }

        #region
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public void ExportSTResultXLS(StockTakeResultSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            IList<string> locationList = new List<string>();
            if (searchModel.Location != null && searchModel.Location != string.Empty)
            {
                locationList.Add(searchModel.Location);
            }
            IList<string> binList = new List<string>();
            if (searchModel.LocationBin != null && searchModel.LocationBin != string.Empty)
            {
                binList.Add(searchModel.LocationBin);
            }
            IList<string> itemList = new List<string>();
            if (searchModel.Item != null && searchModel.Item != string.Empty)
            {
                itemList.Add(searchModel.Item);
            }
            IList<StockTakeResultSummary> stockTakeResultSummaryList = stockTakeMgr.ListStockTakeResult(searchModel.StNo, searchModel.IsLoss, searchModel.IsProfit, searchModel.IsBreakEven, locationList, binList, itemList, null);
            FillCodeDetailDescription<StockTakeResultSummary>(stockTakeResultSummaryList);
            var fileName = string.Format("StokcTakeResult.{0}.xls", searchModel.StNo);
            if (searchModel.IsScanHu)
            {
                fileName = string.Format("StokcTakeResultScanHu.{0}.xls", searchModel.StNo);
            }
            ExportToXLS<StockTakeResultSummary>(fileName, stockTakeResultSummaryList);
        }

        #endregion

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_StockTake_View")]
        public ActionResult _AjaxStockTakeResultIsScanHu(GridCommand command, StockTakeResultSearchModel searchModel)
        {
            IList<string> locationList = new List<string>();
            if (searchModel.Location != null && searchModel.Location != string.Empty)
            {
                locationList.Add(searchModel.Location);
            }
            IList<string> binList = new List<string>();
            if (searchModel.LocationBin != null && searchModel.LocationBin != string.Empty)
            {
                binList.Add(searchModel.LocationBin);
            }
            IList<string> itemList = new List<string>();
            if (searchModel.Item != null && searchModel.Item != string.Empty)
            {
                itemList.Add(searchModel.Item);
            }
            IList<StockTakeResultSummary> StockTakeResultSummaryList = stockTakeMgr.ListStockTakeResult(searchModel.StNo, searchModel.IsLoss, searchModel.IsProfit, searchModel.IsBreakEven, locationList, binList, itemList, null);
            var itemlist = itemMgr.GetCacheAllItem();
            foreach (var StockTakeResult in StockTakeResultSummaryList)
            {
                StockTakeResult.ItemDescription = itemlist.ValueOrDefault(StockTakeResult.Item).FullDescription;
            }
            return PartialView(new GridModel(StockTakeResultSummaryList));
        }

        public ActionResult _StockTakeResultDetail(string Location, string LocationBin, string Item, string StNo, bool listShortage, bool listProfit, bool listMatch)
        {
            if (StNo == null || StNo == string.Empty)
            {
                return null;
            }

            IList<string> locationList = new List<string>();
            if (Location != null && Location != string.Empty)
            {
                locationList.Add(Location);
            }
            IList<string> binList = new List<string>();
            if (LocationBin != null && LocationBin != string.Empty)
            {
                binList.Add(LocationBin);
            }
            IList<string> itemList = new List<string>();
            if (Item != null && Item != string.Empty)
            {
                itemList.Add(Item);
            }
            ViewBag.Status = genericMgr.FindById<StockTakeMaster>(StNo).Status.ToString();
            IList<StockTakeResult> stockTakeResultList = stockTakeMgr.ListStockTakeResultDetail(StNo, listShortage, listProfit, listMatch, locationList, binList, itemList, null);
            FillCodeDetailDescription<StockTakeResult>(stockTakeResultList);
            return PartialView(stockTakeResultList);
        }


        public JsonResult _AllAdjust(string StNo)
        {
            try
            {
                stockTakeMgr.AdjustStockTakeResult(StNo, null);
                object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_StockTakeAdjustSuccessfully, StNo) };
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

        [GridAction(EnableCustomBinding = true)]
        public JsonResult _btnAdjust(string Ids, string StNo)
        {
            //ViewBag.CheckedOrders = checkedOrders;
            string[] IdArray = Ids.Split(',');
            int[] ids = new int[IdArray.Length];
            int i = 0;
            foreach (var item in IdArray)
            {
                ids[i++] = int.Parse(item);
            }

            try
            {
                stockTakeMgr.AdjustStockTakeResult(ids, null);
                object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_StockTakeAdjustSuccessfully, StNo) };
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

        #region Print&eExport
        public string Print(string StNo)
        {
            StockTakeMaster stockTakeMaster = queryMgr.FindById<StockTakeMaster>(StNo);
            PrintStockTakeMaster printStockTakeMaster = Mapper.Map<StockTakeMaster, PrintStockTakeMaster>(stockTakeMaster);
            IList<StockTakeLocation> stockTakeLocationList = genericMgr.FindAll<StockTakeLocation>("from StockTakeLocation as s where s.StNo=?", StNo);
            IList<StockTakeItem> stockTakeItemList = genericMgr.FindAll<StockTakeItem>("from StockTakeItem s where s.StNo=?", StNo);
            foreach (var stockTakeItem in stockTakeItemList)
            {
                stockTakeItem.Uom = this.itemMgr.GetCacheItem(stockTakeItem.Item).Uom;
            }
            IList<object> data = new List<object>();
            data.Add(printStockTakeMaster);
            data.Add(stockTakeLocationList);
            data.Add(stockTakeItemList);

            string reportFileUrl = reportGen.WriteToFile("ST_StockTaking.xls", data);
            //reportGen.WriteToClient(orderMaster.OrderTemplate, data, orderMaster.OrderTemplate);

            return reportFileUrl;
            //reportGen.WriteToFile(orderMaster.OrderTemplate, data);
        }
        public void SaveToClient(string stNo)
        {
            try
            {
                StockTakeMaster stockTakeMaster = queryMgr.FindById<StockTakeMaster>(stNo);
                PrintStockTakeMaster printStockTakeMaster = Mapper.Map<StockTakeMaster, PrintStockTakeMaster>(stockTakeMaster);
                IList<StockTakeLocation> stockTakeLocationList = genericMgr.FindAll<StockTakeLocation>("from StockTakeLocation as s where s.StNo=?", stNo);
                IList<StockTakeItem> stockTakeItemList = genericMgr.FindAll<StockTakeItem>("from StockTakeItem s where s.StNo=?", stNo);
                foreach (var stockTakeItem in stockTakeItemList)
                {
                    stockTakeItem.Uom = this.itemMgr.GetCacheItem(stockTakeItem.Item).Uom;
                }
                IList<object> data = new List<object>();
                data.Add(printStockTakeMaster);
                data.Add(stockTakeLocationList);
                data.Add(stockTakeItemList);
                reportGen.WriteXlsToClient("ST_StockTaking.xls", data, stNo + ".xls");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
        }
        #endregion

        #region Prepare

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">StockTakeMaster Search Model</param>
        /// <returns>return StockTakeMaster search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, StockTakeMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            SecurityHelper.AddRegionPermissionStatement(ref whereStatement, "s", "Region");
            HqlStatementHelper.AddLikeStatement("StNo", searchModel.StNo, HqlStatementHelper.LikeMatchMode.End, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Region", searchModel.Region, "s", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("CostCenter", searchModel.CostCenter, HqlStatementHelper.LikeMatchMode.End, "s", ref whereStatement, param);

            if (searchModel.StockTakeStartDate != null & searchModel.StockTakeEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StockTakeStartDate, searchModel.StockTakeEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.StockTakeStartDate != null & searchModel.StockTakeEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StockTakeStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.StockTakeStartDate == null & searchModel.StockTakeEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.StockTakeEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StockTakeStatusDescription")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by s.CreateDate desc";
            }
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
        /// <param name="searchModel">StockTakeResult Search Model</param>
        /// <returns>return StockTakeResult search model</returns>
        private SearchStatementModel StockTakeDetailPrepareSearchStatement(GridCommand command, StockTakeDetailSearchModel searchModel)
        {
            string whereStatement = " where 1=1";
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("StNo", searchModel.StNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.ItemCode, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Bin", searchModel.LocationBin, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "s", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = "select count(*) from StockTakeDetail as s";
            searchStatementModel.SelectStatement = "select s from StockTakeDetail as s";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">StockTakeResult Search Model</param>
        /// <returns>return StockTakeResult search model</returns>
        private SearchStatementModel StockTakeResultPrepareSearchStatement(GridCommand command, StockTakeResultSearchModel searchModel)
        {
            string whereStatement = " where 1=1";
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("StNo", searchModel.StNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "s", ref whereStatement, param);
            if (searchModel.IsAdjust == true)
            {
                searchModel.IsAdjust = false;
                HqlStatementHelper.AddEqStatement("IsAdjust", searchModel.IsAdjust, "s", ref whereStatement, param);
            }

            if (searchModel.IsLoss == true)
            {
                whereStatement += " and (DiffQty<0";
                if (searchModel.IsProfit == true)
                {
                    whereStatement += " or DiffQty>0";
                }
                if (searchModel.IsBreakEven == true)
                {
                    whereStatement += " or DiffQty=0";
                }
                whereStatement += ")";
            }
            else if (searchModel.IsProfit == true)
            {
                whereStatement += " and (DiffQty>0";
                if (searchModel.IsBreakEven == true)
                {
                    whereStatement += " or DiffQty=0";
                }
                whereStatement += ")";
            }
            else if (searchModel.IsBreakEven == true || (searchModel.IsBreakEven == false & searchModel.IsLoss == false & searchModel.IsProfit == false))
            {
                whereStatement += " and DiffQty=0";
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectStockTakeResultCountStatement;
            searchStatementModel.SelectStatement = selectStockTakeResultStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private string getSearchHql(StockTakeResultSearchModel searchModel)
        {
            string hql = "from StockTakeResult as s where s.StNo='" + searchModel.StNo + "'";
            if (searchModel.Item != null)
            {
                hql += "and s.Item='" + searchModel.Item + "'";
            }
            if (searchModel.Location != null)
            {
                hql += "and s.Location='" + searchModel.Location + "'";
            }
            if (searchModel.LocationBin != null)
            {
                hql += "and s.Bin='" + searchModel.LocationBin + "'";
            }
            if (searchModel.Item != null)
            {
                hql += "and s.Item='" + searchModel.Item + "'";
            }

            if (searchModel.IsAdjust == true)
            {
                searchModel.IsAdjust = false;
                hql += "and s.IsAdjust='" + searchModel.IsAdjust + "'";
            }

            if (searchModel.IsLoss == true)
            {
                hql += " and (DiffQty<0";
                if (searchModel.IsProfit == true)
                {
                    hql += " or DiffQty>0";
                }
                if (searchModel.IsBreakEven == true)
                {
                    hql += " or DiffQty=0";
                }
                hql += ")";
            }
            else if (searchModel.IsProfit == true)
            {
                hql += " and (DiffQty>0";
                if (searchModel.IsBreakEven == true)
                {
                    hql += " or DiffQty=0";
                }
                hql += ")";
            }
            else if (searchModel.IsBreakEven == true || (searchModel.IsBreakEven == false & searchModel.IsLoss == false & searchModel.IsProfit == false))
            {
                hql += " and DiffQty=0";
            }

            hql += "";

            return hql;
        }
        #endregion
    }
}
