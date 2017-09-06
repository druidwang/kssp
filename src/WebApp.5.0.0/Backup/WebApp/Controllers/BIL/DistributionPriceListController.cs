/// <summary>
/// Summary description for LocationController
/// </summary>
namespace com.Sconit.Web.Controllers.BIL
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.BIL;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.BIL;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;

    /// <summary>
    /// This controller response to control the Location.
    /// </summary>
    public class DistributionPriceListController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the PriceList security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql 
        /// </summary>
        private static string selectCountStatement = "select count(*) from PriceListMaster as p";

        /// <summary>
        /// hql 
        /// </summary>
        private static string selectStatement = "select p from PriceListMaster as p";

        /// <summary>
        /// hql 
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from PriceListMaster as p where p.Code = ?";

        /// <summary>
        /// hql 
        /// </summary>
        private static string priceListDetailSelectCountStatement = "select count(*) from PriceListDetail as p";

        /// <summary>
        /// hql
        /// </summary>
        private static string priceListDetailSelectStatement = "select p from PriceListDetail as p";

        /// <summary>
        /// hql 
        /// </summary>
        private static string priceListDetailDuiplicateVerifyStatement = @"select count(*) from PriceListDetail as p where p.Id = ?";


        #region PriceListMaster
        /// <summary>
        /// Index action for PriceListMaster controller
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List acion
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">PriceListMaster Search Model</param>
        /// <returns>return to the result action</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_View")]
        public ActionResult List(GridCommand command, PriceListMasterSearchModel searchModel)
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
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">PriceListMaster Search Model</param>
        /// <returns>return to the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_View")]
        public ActionResult _AjaxList(GridCommand command, PriceListMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<PriceListMaster> PriceListMasterLists = GetAjaxPageData<PriceListMaster>(searchStatementModel, command);
            foreach (var PriceListMasterList in PriceListMasterLists.Data)
            {
                if (!string.IsNullOrWhiteSpace(PriceListMasterList.Party))
                {
                    PriceListMasterList.PartyName = this.genericMgr.FindById<Party>(PriceListMasterList.Party).Name;
                }
            }
            return PartialView(PriceListMasterLists);
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="location">PriceListMaster model</param>
        /// <returns>return to edit view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Edit")]
        public ActionResult New(PriceListMaster priceListMaster)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { priceListMaster.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, priceListMaster.Code);
                }
                else
                {
                    if (this.genericMgr.FindAll<Customer>("from Customer where Code=?", priceListMaster.Party).Count < 1)
                    {
                        SaveErrorMessage(Resources.BIL.PriceListMaster.Errors_NotExisting_Party);
                    }
                    else
                    {
                        priceListMaster.Type = com.Sconit.CodeMaster.PriceListType.Distribution;
                        this.genericMgr.CreateWithTrim(priceListMaster);
                        SaveSuccessMessage(Resources.BIL.PriceListMaster.PriceListMaster_Added);
                        return RedirectToAction("Edit/" + priceListMaster.Code);
                    }
                }
            }

            return View(priceListMaster);
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">PriceListMaster id for edit</param>
        /// <returns>return to edit view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                return View("Edit", string.Empty, id);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">PriceListMaster id for edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Edit")]
        public ActionResult _Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                PriceListMaster priceListMaster = this.genericMgr.FindById<PriceListMaster>(id);
                priceListMaster.PartyName = this.genericMgr.FindById<Party>(priceListMaster.Party).Name;
                return PartialView(priceListMaster);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="location">PriceListMaster model</param>
        /// <returns>return to edit action</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Edit")]
        public ActionResult _Edit(PriceListMaster priceListMaster)
        {
            if (ModelState.IsValid)
            {
                priceListMaster.Type = com.Sconit.CodeMaster.PriceListType.Distribution;
                this.genericMgr.UpdateWithTrim(priceListMaster);
                SaveSuccessMessage(Resources.BIL.PriceListMaster.PriceListMaster_Updated);
            }

            return PartialView(priceListMaster);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">PriceListMaster id for delete</param>
        /// <returns>return to list action</returns>
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<PriceListMaster>(id);
                SaveSuccessMessage(Resources.BIL.PriceListMaster.PriceListMaster_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        #region PriceListDetail
        /// <summary>
        /// _PriceListDetail action
        /// </summary>
        /// <param name="id">PriceList Code</param>
        /// <returns>rediret view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_View")]
        public ActionResult _PriceListDetail(GridCommand command, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.PriceListCode = id;
                ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
                return PartialView();
            }
        }

        /// <summary>
        /// PriceListDetailList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">PriceListDetail Search Model</param>
        /// <param name="locationCode">PriceList Code</param>
        /// <returns>return to the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_View")]
        public ActionResult _PriceListDetailList(GridCommand command, PriceListDetailSearchModel searchModel, string priceListCode)
        {
            ViewBag.LocationCode = priceListCode;

            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);

            return PartialView();
        }

        /// <summary>
        /// AjaxPriceListDetailList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">PriceListDetail Search Model</param>
        /// <param name="locationCode">PriceList Code</param>
        /// <returns>return to the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_View")]
        public ActionResult _AjaxPriceListDetailList(GridCommand command, PriceListDetailSearchModel searchModel, string priceListCode)
        {
            SearchStatementModel searchStatementModel = this.LocationAreaPrepareSearchStatement(command, searchModel, priceListCode);
            GridModel<PriceListDetail> priceListDetailList = GetAjaxPageData<PriceListDetail>(searchStatementModel, command);
            foreach (var priceList in priceListDetailList.Data)
            {
                if (priceList.PriceList != null)
                {
                    priceList.PriceListCode = priceList.PriceList.Code;
                }
                priceList.ItemDesc = this.itemMgr.GetCacheItem(priceList.Item).Description;
            }

            return PartialView(priceListDetailList);
        }

        /// <summary>
        /// PriceListDetailNew action
        /// </summary>
        /// <param name="locationCode">PriceList Code</param>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Edit")]
        public ActionResult _PriceListDetailNew(string priceListCode)
        {
            ViewBag.PriceListCode = priceListCode;
            return PartialView();
        }

        /// <summary>
        /// PriceListDetailNew action
        /// </summary>
        /// <param name="locationArea">PriceListDetail Model</param>
        /// <param name="locationCode">PriceList Code</param>
        /// <returns>return to the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Edit")]
        public ActionResult _PriceListDetailNew(PriceListDetail priceListDetail, string priceListCode)
        {
            if (!string.IsNullOrEmpty(priceListCode))
            {
                ViewBag.PriceListCode = priceListCode;
                PriceListMaster priceListMaster = this.genericMgr.FindById<PriceListMaster>(priceListCode);
                priceListDetail.PriceList = priceListMaster;
                ModelState.Remove("PriceList");
            }
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(priceListDetailDuiplicateVerifyStatement, new object[] { priceListDetail.Id })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, priceListDetail.Id.ToString());
                }
                else
                {
                    bool isError = false;
                    if (this.genericMgr.FindAll<Item>("from Item where Code=?", priceListDetail.Item).Count < 1)
                    {
                        SaveErrorMessage(Resources.BIL.PriceListDetail.Errors_NotExisting_Item);
                        isError = true;
                    }
                    else if (priceListDetail.EndDate != null)
                    {
                        if (System.DateTime.Compare((System.DateTime)priceListDetail.EndDate, (System.DateTime)priceListDetail.StartDate) < 1)
                        {
                            SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
                            isError = true;
                        }
                    }

                    if (!isError)
                    {
                        this.genericMgr.CreateWithTrim(priceListDetail);
                        SaveSuccessMessage(Resources.BIL.PriceListDetail.PriceListDetail_Added);
                        return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_PriceListDetailList" }, 
                                                       { "controller", "DistributionPriceList" },
                                                       { "PriceListCode", priceListCode }
                                                   });
                    }
                }
            }

            return PartialView(priceListDetail);
        }

        /// <summary>
        /// PriceListDetailEdit action
        /// </summary>
        /// <param name="id">PriceListDetail id for Edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Edit")]
        public ActionResult _PriceListDetailEdit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {

                PriceListDetail priceListDetail = this.genericMgr.FindById<PriceListDetail>(id);
                if (priceListDetail.PriceList != null)
                {
                    priceListDetail.PriceListCode = priceListDetail.PriceList.Code;
                }

                return PartialView(priceListDetail);
            }
        }

        /// <summary>
        /// PriceListDetailEdit action
        /// </summary>
        /// <param name="locationArea">PriceListDetail Model</param>
        /// <returns>return to PriceListDetailEdit action</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Edit")]
        public ActionResult _PriceListDetailEdit(PriceListDetail priceListDetail)
        {
            if (priceListDetail.PriceListCode != null)
            {
                PriceListMaster priceListMaster = this.genericMgr.FindById<PriceListMaster>(priceListDetail.PriceListCode);
                priceListDetail.PriceList = priceListMaster;
                ModelState.Remove("PriceList");
            }

            if (ModelState.IsValid)
            {
                bool isError = false;
                if (priceListDetail.EndDate != null)
                {
                    if (System.DateTime.Compare((System.DateTime)priceListDetail.EndDate, (System.DateTime)priceListDetail.StartDate) < 1)
                    {
                        SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
                        isError = true;
                    }
                }
                if (!isError)
                {
                    this.genericMgr.UpdateWithTrim(priceListDetail);
                    SaveSuccessMessage(Resources.BIL.PriceListDetail.PriceListDetail_Updated);
                }
            }

            return PartialView(priceListDetail);
        }

        /// <summary>
        /// PriceListDetailDelete action
        /// </summary>
        /// <param name="id">PriceListDetail id for delete</param>
        /// <param name="locationCode">PriceList Code</param>
        /// <returns>return to LocationAreaList action</returns>
        [SconitAuthorize(Permissions = "Url_DistributionPriceList_Delete")]
        public ActionResult _PriceListDetailDelete(int? id, string priceListCode)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<PriceListDetail>(id);
                SaveSuccessMessage(Resources.BIL.PriceListDetail.PriceListDetail_Deleted);
                return new RedirectToRouteResult(new RouteValueDictionary  
                                                   { 
                                                       { "action", "_PriceListDetailList" }, 
                                                       { "controller", "DistributionPriceList" },
                                                       { "PriceListCode", priceListCode }
                                                   });
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">PriceListDetail Search Model</param>
        /// <param name="locationCode">PriceList Code</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel LocationAreaPrepareSearchStatement(GridCommand command, PriceListDetailSearchModel searchModel, string priceListDetail)
        {
            string whereLocationStatement = "where p.PriceList = '" + priceListDetail + "'";
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereLocationStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = priceListDetailSelectCountStatement;
            searchStatementModel.SelectStatement = priceListDetailSelectStatement;
            searchStatementModel.WhereStatement = whereLocationStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">PriceListMaster Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PriceListMasterSearchModel searchModel)
        {
            string whereStatement = "where p.Type='2'";

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Party", searchModel.Party, "p", ref whereStatement, param);

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
