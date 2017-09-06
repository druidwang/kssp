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
    using System;
    #endregion

    /// <summary>
    /// This controller response to control the MrpExWorkHour.
    /// </summary>
    public class MrpExWorkHourController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the MrpExWorkHour security 
        /// </summary>

        #endregion

        /// <summary>
        /// hql to get count of the mrpExWorkHour 
        /// </summary>
        private static string selectCountStatement = "select count(*) from MrpExWorkHour as u";

        /// <summary>
        /// hql to get all of the mrpExWorkHour
        /// </summary>
        private static string selectStatement = "select u from MrpExWorkHour as u";

        #region public actions
        /// <summary>
        /// Index action for MrpExWorkHour controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExWorkHour_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MrpExWorkHour Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_MrpExWorkHour_View")]
        public ActionResult List(GridCommand command, MrpExWorkHourSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MrpExWorkHour Search Model</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MrpExWorkHour_View")]
        public ActionResult _AjaxList(GridCommand command, MrpExWorkHourSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<MrpExWorkHour>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExWorkHour_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="mrpExWorkHour">mrpExWorkHour model</param>
        /// <returns>return to Edit action </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_MrpExWorkHour_Edit")]
        public ActionResult New(MrpExWorkHour mrpExWorkHour)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isDuiplicate = CheckDuiplicate(mrpExWorkHour);

                    if (!isDuiplicate)
                    {
                        mrpExWorkHour.ItemDescription = itemMgr.GetCacheItem(mrpExWorkHour.Item).Description;
                        this.genericMgr.CreateWithTrim(mrpExWorkHour);
                        SaveSuccessMessage(Resources.MRP.MrpExWorkHour.MrpExWorkHour_Added);
                        return RedirectToAction("Edit/" + mrpExWorkHour.Id);
                    }
                }
                catch (Exception ex)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_NewAddedUnsuccessfully_1, ex.Message);
                }
            }

            return View(mrpExWorkHour);
        }

        private bool CheckDuiplicate(MrpExWorkHour mrpExWorkHour)
        {
            if (mrpExWorkHour.WindowTime < mrpExWorkHour.StartTime)
            {
                SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_EndTimeNeedToBeGreaterStartToTime, mrpExWorkHour.StartTime, mrpExWorkHour.WindowTime));
                return true;
            }
            //check 同一生产线是否有重叠
            string hql = @" from MrpExWorkHour as u where u.Flow = ? and StartTime>? and WindowTime<? ";
            var list = this.genericMgr.FindAll<MrpExWorkHour>(hql,
                new object[] { mrpExWorkHour.Flow, mrpExWorkHour.StartTime.AddDays(-7), mrpExWorkHour.WindowTime.AddDays(7) })
                ?? new List<MrpExWorkHour>();
            bool isDuiplicate = false;
            foreach (var wh in list)
            {
                if (wh.Id == mrpExWorkHour.Id)
                {
                    continue;
                }
                if (mrpExWorkHour.StartTime < wh.WindowTime && mrpExWorkHour.StartTime >= wh.StartTime)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, mrpExWorkHour.Flow);
                    isDuiplicate = true;
                    SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_StartTimeAlreadyIncluded, wh.Id, wh.StartTime, wh.WindowTime));
                    break;
                }
                if (mrpExWorkHour.WindowTime <= wh.WindowTime && mrpExWorkHour.WindowTime > wh.StartTime)
                {
                    isDuiplicate = true;
                    SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_FinishTimeAlreadyIncluded, wh.Id, wh.StartTime, wh.WindowTime));
                    break;
                }

                if (mrpExWorkHour.StartTime <= wh.StartTime && mrpExWorkHour.WindowTime >= wh.WindowTime)
                {
                    isDuiplicate = true;
                    SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_IdStartToTimeAndEndTimeContainCurrentTime, wh.Id, wh.StartTime, wh.WindowTime));
                    break;
                }

            }
            //todo 校验给时间段是否生产了此断面
            return isDuiplicate;
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">mrpExWorkHour id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MrpExWorkHour_Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                MrpExWorkHour mrpExWorkHour = this.genericMgr.FindById<MrpExWorkHour>(id.Value);
                return View(mrpExWorkHour);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="mrpExWorkHour">mrpExWorkHour model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExWorkHour_Edit")]
        public ActionResult Edit(MrpExWorkHour mrpExWorkHour)
        {
            if (ModelState.IsValid)
            {
                bool isDuiplicate = CheckDuiplicate(mrpExWorkHour);
                if (!isDuiplicate)
                {
                    this.genericMgr.UpdateWithTrim(mrpExWorkHour);
                    SaveSuccessMessage(Resources.MRP.MrpExWorkHour.MrpExWorkHour_Updated);
                }
            }
            return View(mrpExWorkHour);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">mrpExWorkHour id for delete</param>
        /// <returns>return to list view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExWorkHour_Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<MrpExWorkHour>(id.Value);
                SaveSuccessMessage(Resources.MRP.MrpExWorkHour.MrpExWorkHour_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MrpExWorkHour Search Model</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, MrpExWorkHourSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            searchModel.DateFrom = searchModel.DateFrom == DateTime.MinValue ? DateTime.Today.AddDays(-1) : searchModel.DateFrom;
            searchModel.DateTo = searchModel.DateTo == DateTime.MinValue ? DateTime.Today.AddDays(1) : searchModel.DateTo;

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
    }
}
