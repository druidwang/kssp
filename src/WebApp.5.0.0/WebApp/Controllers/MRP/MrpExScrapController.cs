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
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Web.Models.SearchModels.MRP;
    using com.Sconit.Entity.MRP.ORD;
    using com.Sconit.Service.MRP;
    using System;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.MRP.TRANS;
    #endregion

    /// <summary>
    /// This controller response to control the MrpExScrap.
    /// </summary>
    public class MrpExScrapController : WebAppBaseController
    {
        public IMrpOrderMgr mrpOrderMgr { get; set; }
        public IMrpMgr mrpMgr { get; set; }
        public IFinanceCalendarMgr financeCalendarMgr { get; set; }
        public IBomMgr bomMgr { get; set; }

        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the MrpExScrap security 
        /// </summary>



        #endregion

        /// <summary>
        /// hql to get count of the mrpExScrap 
        /// </summary>
        private static string selectCountStatement = "select count(*) from MrpExScrap as u";

        /// <summary>
        /// hql to get all of the mrpExScrap
        /// </summary>
        private static string selectStatement = "select u from MrpExScrap as u";

        /// <summary>
        /// hql to get count of the mrpExScrap by mrpExScrap's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from MrpExScrap as u where u.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for MrpExScrap controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExScrap_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MrpExScrap Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_MrpExScrap_View")]
        public ActionResult List(GridCommand command, MrpExScrapSearchModel searchModel)
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
        /// <param name="searchModel">MrpExScrap Search Model</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MrpExScrap_View")]
        public ActionResult _AjaxList(GridCommand command, MrpExScrapSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<MrpExScrap>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExScrap_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="mrpExScrap">mrpExScrap model</param>
        /// <returns>return to Edit action </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_MrpExScrap_Edit")]
        public ActionResult New(MrpExScrap mrpExScrap)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //todo 校验给时间段是否生产了此断面
                    if (false)
                    {
                        //SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, mrpExScrap.Flow);
                    }
                    else
                    {
                        //mrpExScrapMgr.CreateMrpExScrap(mrpExScrap);
                        mrpOrderMgr.CreateExScrapOrder(mrpExScrap);
                        SaveSuccessMessage(Resources.MRP.MrpExScrap.MrpExScrap_Added);
                        return RedirectToAction("Edit/" + mrpExScrap.Id);
                    }
                }
                catch (Exception ex)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_NewAddedUnsuccessfully_1, ex.Message);
                }
            }
            return View(mrpExScrap);
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">mrpExScrap id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MrpExScrap_Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                MrpExScrap mrpExScrap = this.genericMgr.FindById<MrpExScrap>(id.Value);
                FillCodeDetailDescription<MrpExScrap>(mrpExScrap);
                return View(mrpExScrap);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="mrpExScrap">mrpExScrap model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExScrap_Edit")]
        public ActionResult Edit(MrpExScrap mrpExScrap)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(mrpExScrap);
                SaveSuccessMessage(Resources.MRP.MrpExScrap.MrpExScrap_Updated);
            }
            return View(mrpExScrap);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">mrpExScrap id for delete</param>
        /// <returns>return to list view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExScrap_Cancel")]
        public ActionResult Cancel(int id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    var mrpExScrap = this.genericMgr.FindById<MrpExScrap>(id);
                    mrpOrderMgr.CancelExScrapOrder(mrpExScrap);
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CancelledSuccessfully);
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_CancelledUnsuccessfully, ex.Message);
                    return RedirectToAction("Edit", id);
                }
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MrpExScrap Search Model</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, MrpExScrapSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            searchModel.DateFrom = searchModel.DateFrom == DateTime.MinValue ? DateTime.Today.AddDays(-1) : searchModel.DateFrom;
            searchModel.DateTo = searchModel.DateTo == DateTime.MinValue ? DateTime.Today.AddDays(1) : searchModel.DateTo;
            HqlStatementHelper.AddEqStatement("Id", searchModel.Id, "u", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "u", ref whereStatement, param);
            HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.DateFrom, searchModel.DateTo.AddDays(1), "u", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by u.Id desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }


        [HttpPost]
        [SconitAuthorize(Permissions = "Url_MrpExScrap_View")]
        public JsonResult GetOrder(string orderNo)
        {
            try
            {
                var orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);
                if (orderMaster.SubType != Sconit.CodeMaster.OrderSubType.Normal)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_OrderTypeIsWrong);
                }
                if (orderMaster.ResourceGroup != Sconit.CodeMaster.ResourceGroup.EX)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_IsNotExtrusionProductionOrder);
                }
                #region 判断库存生效日期是否有效
                FinanceCalendar financeCalendar = financeCalendarMgr.GetNowEffectiveFinanceCalendar();
                //前开后闭
                if (financeCalendar.StartDate >= orderMaster.StartTime)
                {
                    throw new BusinessException(string.Format(Resources.EXT.ControllerLan.Con_InventoryEffectiveDateLessThanCurrentFinancialMothStartToDate, orderMaster.StartTime, financeCalendar.StartDate));
                }
                #endregion
                var orderDetail = this.genericMgr.FindAll<OrderDetail>(" from OrderDetail where OrderNo =? ", orderNo, 0, 1).First();
                var bomDetail = this.bomMgr.GetOnlyNextLevelBomDetail(orderDetail.Item, orderMaster.StartTime)
                    .Where(p => p.Item.StartsWith("29")).FirstOrDefault();
                if (bomDetail != null)
                {
                    var item = itemMgr.GetCacheItem(bomDetail.Item);
                    orderDetail.Item = item.Code;
                    orderDetail.ItemDescription = item.Description;
                }

                orderDetail.StartTime = orderMaster.StartTime;
                orderDetail.ShiftName = orderMaster.Shift;
                orderDetail.Flow = orderMaster.Flow;
                return Json(orderDetail);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public bool IsShiftPlanContainsItem(DateTime planDate, string flow, string shift, string itemCode)
        {
            var shiftList = mrpMgr.GetMrpExShiftPlanList(planDate, flow, shift) ?? new List<MrpExShiftPlan>();

            if (shiftList.Select(p => p.Item).Contains(itemCode)
                || shiftList.Where(p => !string.IsNullOrWhiteSpace(p.Section)).Select(p => p.Section).Contains(itemCode))
            {
                return true;
            }
            return false;
        }
    }
}
