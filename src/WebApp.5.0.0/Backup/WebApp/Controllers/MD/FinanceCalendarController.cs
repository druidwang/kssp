/// <summary>
/// Summary description for FinanceCalendarController
/// </summary>
namespace com.Sconit.Web.Controllers.MD
{
    #region reference
    using System;
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
    #endregion

    /// <summary>
    /// This controller response to control the FinanceCalendar.
    /// </summary>
    public class FinanceCalendarController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the FinanceCalendar security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        public IFinanceCalendarMgr FinanceCalendarMgr { get; set; }
        /// <summary>
        /// hql to get count of the FinanceCalendar
        /// </summary>
        private static string selectCountStatement = "select count(*) from FinanceCalendar as f";

        /// <summary>
        /// hql to get all of the FinanceCalendar
        /// </summary>
        private static string selectStatement = "select f from FinanceCalendar as f";

        /// <summary>
        /// hql to get count of the FinanceCalendar by FinanceCalendar's id
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from FinanceCalendar as f where f.Id = ?";
        private static string duiplicateverifyYearMothGroupStatement = @"select count(*) from FinanceCalendar as f where f.FinanceYear = ? and f.FinanceMonth = ? ";

        #region public actions
        /// <summary>
        /// Index action for FinanceCalendar controller
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_FinanceCalendar_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">FinanceCalendar Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_FinanceCalendar_View")]
        public ActionResult List(GridCommand command, FinanceCalendarSearchModel searchModel)
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
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">FinanceCalendar Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FinanceCalendar_View")]
        public ActionResult _AjaxList(GridCommand command, FinanceCalendarSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<FinanceCalendar>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_FinanceCalendar_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="financeCalendar">FinanceCalendar Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_FinanceCalendar_Edit")]
        public ActionResult New(FinanceCalendar financeCalendar)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { financeCalendar.Id })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code);
                }
                else if (this.genericMgr.FindAll<long>(duiplicateverifyYearMothGroupStatement, new object[] { financeCalendar.FinanceYear, financeCalendar.FinanceMonth })[0] > 0)
                {
                    SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_Existing_YearAndMonth);
                }
                else
                {
                    if (System.DateTime.Compare((System.DateTime)financeCalendar.EndDate, (System.DateTime)financeCalendar.StartDate) < 1)
                    {
                        SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
                    }
                    else
                    {
                        this.genericMgr.CreateWithTrim(financeCalendar);
                        SaveSuccessMessage(Resources.MD.FinanceCalendar.FinanceCalendar_Added);
                        return RedirectToAction("Edit/" + financeCalendar.Id);
                    }
                }
            }

            return View(financeCalendar);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">financeCalendar id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_FinanceCalendar_View")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                FinanceCalendar financeCalendar = this.genericMgr.FindById<FinanceCalendar>(id);
                return View(financeCalendar);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="financeCalendar">financeCalendar Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_FinanceCalendar_Edit")]
        public ActionResult Edit(FinanceCalendar financeCalendar)
        {
            if (ModelState.IsValid)
            {
                if (System.DateTime.Compare((System.DateTime)financeCalendar.EndDate, (System.DateTime)financeCalendar.StartDate) < 1)
                {
                    SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
                }
                else
                {
                    this.genericMgr.UpdateWithTrim(financeCalendar);
                    SaveSuccessMessage(Resources.MD.FinanceCalendar.FinanceCalendar_Updated);
                }
            }

            return View(financeCalendar);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">financeCalendar id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_FinanceCalendar_Delete")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<FinanceCalendar>(id);
                SaveSuccessMessage(Resources.MD.FinanceCalendar.FinanceCalendar_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">FinanceCalendar Search Model</param>
        /// <returns>return FinanceCalendar search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, FinanceCalendarSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            if (!searchModel.FinanceYear.HasValue)
            {
                searchModel.FinanceYear = DateTime.Today.Year;
            }

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("FinanceYear", searchModel.FinanceYear, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("FinanceMonth", searchModel.FinanceMonth, "f", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        [SconitAuthorize(Permissions = "Url_FinanceCalendar_Close")]
        public ActionResult Close()
        {
            FinanceCalendar FinanceCalendar = FinanceCalendarMgr.GetNowEffectiveFinanceCalendar();
            return View(FinanceCalendar);
        }

        [SconitAuthorize(Permissions = "Url_FinanceCalendar_Close")]
        public ActionResult CloseFinanceCalendar()
        {
            try
            {
                FinanceCalendarMgr.CloseFinanceCalendar();
            }
            catch (System.Exception e)
            {

                SaveErrorMessage(e.Message);
            }

            return RedirectToAction("Close");
        }
    }
}
