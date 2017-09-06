/// <summary>
/// Summary description for WorkingCalendarController
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
    using com.Sconit.Entity.PRD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Entity.VIEW;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.MD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using System;
    using com.Sconit.Entity.MRP.MD;
    using System.Text;


    /// <summary>
    /// This controller response to control the WorkingCalendar.
    /// </summary>
    public class WorkingCalendarController : WebAppBaseController
    {
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the WorkingCalendar security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        /// <summary>
        /// Gets or sets the this.WorkingCalendarMgr which main consider the WorkingCalendar security 
        /// </summary>
        public IWorkingCalendarMgr WorkingCalendarMgr { get; set; }

        #region hql
        /// <summary>
        /// hql for ShiftMaster
        /// </summary>
        private static string shiftMstrSelectCountStatement = "select count(*) from ShiftMaster as s";

        /// <summary>
        /// hql for ShiftMaster
        /// </summary>
        private static string shiftMstrSelectStatement = "select s from ShiftMaster as s";

        /// <summary>
        /// hql for ShiftMaster
        /// </summary>
        private static string shiftMstrDuiplicateVerifyStatement = @"select count(*) from ShiftMaster as s where s.Code = ?";

        /// <summary>
        /// hql for ShiftDetail
        /// </summary>
        private static string shiftDetailSelectCountStatement = "select count(*) from ShiftDetail as s";

        /// <summary>
        /// hql for ShiftDetail
        /// </summary>
        private static string shiftDetailSelectStatement = "select s from ShiftDetail as s";

        /// <summary>
        /// hql for WorkingCalendar
        /// </summary>
        private static string selectCountStatement = "select count(*) from WorkingCalendar as w";

        /// <summary>
        /// hql for WorkingCalendar
        /// </summary>
        private static string selectStatement = "select w from WorkingCalendar as w";

        /// <summary>
        /// hql for WorkingCalendar
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from WorkingCalendar as w where w.Id = ?";

        /// <summary>
        /// hql for WorkingCalendar
        /// </summary>
        private static string isExistNullDayOfWeekDuiplicateVerifyStatement = @"select count(*) from WorkingCalendar as w where w.Region is null and w.DayOfWeek=?";

        /// <summary>
        /// hql for WorkingCalendar
        /// </summary>
        private static string isExistDayOfWeekDuiplicateVerifyStatement = @"select count(*) from WorkingCalendar as w where w.Region = ? and w.DayOfWeek=?";

        /// <summary>
        /// hql for WorkingCalendar
        /// </summary>
        private static string selectShift = "select w.Shift from WorkingShift as w where w.WorkingCalendar = ?";

        /// <summary>
        /// hql for WorkingCalendar
        /// </summary>
        private static string selectShiftNotWithWorkingCalendar = @"select s from ShiftMaster as s where s.Code not in (select w.Shift from WorkingShift as w where w.WorkingCalendar = ?)";

        /// <summary>
        /// hql for SpecialTime
        /// </summary>
        private static string specialTimeSelectCountStatement = "select count(*) from SpecialTime as s";

        /// <summary>
        /// hql for SpecialTime
        /// </summary>
        private static string specialTimeSelectStatement = "select s from SpecialTime as s";

        /// <summary>
        /// hql for SpecialTime
        /// </summary>
        private static string specialTimeDuiplicateVerifyStatement = @" select count(*) from SpecialTime where Region=? and Type=? and Flow=? and StartTime<? and EndTime>? ";//插入相同的不能再你的区间中


        private static string specialTimeDuiplicateVerifyToStatement = @" select count(*) from SpecialTime where Region=? and Type=? and Flow=? and StartTime>? and EndTime<?";//插入相同的不能包含已有的区间中
        #endregion
         
        #region WorkingCalendar View
        /// <summary>
        /// _ViewSearch action
        /// </summary>
        /// <returns>rediret view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult _ViewSearch()
        {
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult _ViewList(string shiftDetail_StartTime, string shiftDetail_EndTime, string region, string Flow)
        {
            ViewBag.Flow = Flow;
            TempData["WorkingCalendarSearchModelregion"] = region;
            if (string.IsNullOrEmpty(shiftDetail_StartTime) || string.IsNullOrEmpty(shiftDetail_EndTime))
            {
                SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_Existing_StartDateAndEndDate);
            }
            else if (System.DateTime.Compare(System.DateTime.Parse(shiftDetail_EndTime), System.DateTime.Parse(shiftDetail_StartTime)) < 1)
            {
                SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
            }
            else
            {
                IList<WorkingCalendarView> workingCalendarViewList = this.WorkingCalendarMgr.GetWorkingCalendarViewList(region, Flow, System.DateTime.Parse(shiftDetail_StartTime), System.DateTime.Parse(shiftDetail_EndTime));
                return PartialView(workingCalendarViewList);
            }
            return PartialView("_ViewSearch");
        }
        #endregion


        #region Shift
        /// <summary>
        /// ShiftMstr action
        /// </summary>
        /// <returns>rediret view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult ShiftMstr()
        {
            return PartialView();
        }

        /// <summary>
        /// ShiftMstrList action
        /// </summary>
        /// <param name="command"> GridCommand ss</param>
        /// <param name="searchModel">ShiftMaster Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult ShiftMstrList(GridCommand command, ShiftMasterSearchModel searchModel)
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
        /// _AjaxShiftMstrList action
        /// </summary>
        /// <param name="command">Telerik GridCommand </param>
        /// <param name="searchModel">ShiftMaster Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult _AjaxShiftMstrList(GridCommand command, ShiftMasterSearchModel searchModel)
        {
            string replaceFrom = "_AjaxShiftMstrList";
            string replaceTo = "ShiftMstrList/";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.ShiftMstrPrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ShiftMaster>(searchStatementModel, command));
        }

        /// <summary>
        /// ShiftMstrNew action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult ShiftMstrNew()
        {
            return PartialView();
        }

        /// <summary>
        /// ShiftMstrNew action
        /// </summary>
        /// <param name="shiftAll">shiftAll model</param>
        /// <param name="shiftMaster">shiftMaster model</param>
        /// <param name="shiftDetail">shiftDetail model</param>
        /// <returns>return to ShiftMstrEdit action</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult ShiftMstrNew(ShiftAll shiftAll, ShiftMaster shiftMaster, ShiftDetail shiftDetail)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(shiftMstrDuiplicateVerifyStatement, new object[] { shiftMaster.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, shiftMaster.Code.ToString());
                }
                else
                {
                    if (shiftAll.EndDate != null & shiftAll.StartDate != null)
                    {
                        if (System.DateTime.Compare((System.DateTime)shiftAll.EndDate, (System.DateTime)shiftAll.StartDate) < 1)
                        {
                            SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
                            return PartialView(shiftAll);
                        }
                    }

                    shiftDetail.Shift = shiftAll.Code;
                    WorkingCalendarMgr.CreateShiftMasterAndShiftDetail(shiftMaster, shiftDetail);
                    SaveSuccessMessage(Resources.MD.WorkingCalendar.ShiftMaster_Added);
                    return RedirectToAction("ShiftMstrEdit/" + shiftAll.Code);
                }
            }

            return PartialView(shiftAll);
        }

        /// <summary>
        /// ShiftMstrEdit action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ShiftDetail Search Model</param>
        /// <param name="id">ShiftDetail id for edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult ShiftMstrEdit(GridCommand command, ShiftDetailSearchModel searchModel, string id)
        {
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
                SearchStatementModel searchStatementModel = this.ShiftDetailPrepareSearchStatement(command, (ShiftDetailSearchModel)searchCacheModel.SearchObject, id);
                ViewBag.shiftDetList = GetPageData<ShiftDetail>(searchStatementModel, command);
                ShiftMaster shiftMaster = this.genericMgr.FindById<ShiftMaster>(id);
                return PartialView(shiftMaster);
            }
        }

        /// <summary>
        /// ShiftMstrEdit action
        /// </summary>
        /// <param name="shiftMaster">shiftMaster model</param>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ShiftDetail Search Model</param>
        /// <returns>return to the result view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult ShiftMstrEdit(ShiftMaster shiftMaster, GridCommand command, ShiftDetailSearchModel searchModel)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(shiftMaster);
                SaveSuccessMessage(Resources.MD.WorkingCalendar.ShiftMaster_Updated);
            }
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = this.ShiftDetailPrepareSearchStatement(command, (ShiftDetailSearchModel)searchCacheModel.SearchObject, shiftMaster.Code);
            ViewBag.shiftDetList = GetPageData<ShiftDetail>(searchStatementModel, command);
            return PartialView(shiftMaster);
        }

        /// <summary>
        /// _AjaxShiftDetailList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ShiftDetail Search Model</param>
        /// <param name="id">ShiftDetail id</param>
        /// <returns>return to the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult _AjaxShiftDetailList(GridCommand command, ShiftDetailSearchModel searchModel, string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                SearchStatementModel searchStatementModel = this.ShiftDetailPrepareSearchStatement(command, searchModel, id);
                return PartialView(GetAjaxPageData<ShiftDetail>(searchStatementModel, command));
            }
        }

        /// <summary>
        /// ShiftDetNew action
        /// </summary>
        /// <param name="id">ShiftDetial id for new</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult ShiftDetNew(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                ShiftMaster shiftMaster = this.genericMgr.FindById<ShiftMaster>(id);
                ShiftAll shiftAll = new ShiftAll();
                shiftAll.Code = shiftMaster.Code;
                shiftAll.Name = shiftMaster.Name;
                return PartialView(shiftAll);
            }
        }

        /// <summary>
        /// ShiftDetNew action
        /// </summary>
        /// <param name="shiftAll">shiftAll model</param>
        /// <param name="shiftDetail">shiftDetail model</param>
        /// <returns>return to ShiftMstrEdit action</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult ShiftDetNew(ShiftAll shiftAll, ShiftDetail shiftDetail)
        {
            if (ModelState.IsValid)
            {
                if (shiftAll.EndDate != null & shiftAll.StartDate != null)
                {
                    if (System.DateTime.Compare((System.DateTime)shiftAll.EndDate, (System.DateTime)shiftAll.StartDate) < 1)
                    {
                        SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
                        return PartialView(shiftAll);
                    }
                }
                shiftDetail.Shift = shiftAll.Code;
                this.genericMgr.CreateWithTrim(shiftDetail);
                SaveSuccessMessage(Resources.MD.WorkingCalendar.ShiftDetail_Added);
                return RedirectToAction("ShiftMstrEdit/" + shiftAll.Code);
            }

            return PartialView(shiftAll);
        }

        /// <summary>
        /// ShiftMstrDelete action
        /// </summary>
        /// <param name="id">ShiftMaster id for delete</param>
        /// <returns>return to ShiftMstrList view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Delete")]
        public ActionResult ShiftMstrDelete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    this.WorkingCalendarMgr.DeleteShiftMaster(id);
                    SaveSuccessMessage(Resources.MD.WorkingCalendar.ShiftMaster_Deleted);
                    return RedirectToAction("ShiftMstrList");
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_DeletedUnsuccessfullyTheShiftIsOccupied);
            }
            return RedirectToAction("ShiftMstrEdit/" +id);
        }

        /// <summary>
        /// ShiftDetEdit action
        /// </summary>
        /// <param name="id">ShiftDetail id for edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult ShiftDetEdit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                ShiftDetail shiftDetail = this.genericMgr.FindById<ShiftDetail>(id);
                ShiftAll shiftAll = new ShiftAll();
                shiftAll.Code = shiftDetail.Shift;
                shiftAll.ShiftTime = shiftDetail.ShiftTime;
                shiftAll.StartDate = shiftDetail.StartDate;
                shiftAll.EndDate = shiftDetail.EndDate;
                shiftAll.Id = shiftDetail.Id;
                ShiftMaster shiftMaster = this.genericMgr.FindById<ShiftMaster>(shiftDetail.Shift);
                shiftAll.Name = shiftMaster.Name;
                return PartialView(shiftAll);
            }
        }

        /// <summary>
        /// ShiftDetEdit action
        /// </summary>
        /// <param name="shiftAll">shiftAll action</param>
        /// <param name="shiftDetail">shiftDetail action</param>
        /// <returns>return to the result view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult ShiftDetEdit(ShiftAll shiftAll, ShiftDetail shiftDetail)
        {
            shiftDetail.Shift = shiftAll.Code;
            if (ModelState.IsValid)
            {
                if (shiftAll.EndDate != null & shiftAll.StartDate != null)
                {
                    if (System.DateTime.Compare((System.DateTime)shiftAll.EndDate, (System.DateTime)shiftAll.StartDate) < 1)
                    {
                        SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
                        return PartialView(shiftAll);
                    }
                }
                this.genericMgr.UpdateWithTrim(shiftDetail);
                SaveSuccessMessage(Resources.MD.WorkingCalendar.ShiftDetail_Updated);
            }

            return PartialView(shiftAll);
        }

        /// <summary>
        /// ShiftDetDelete action
        /// </summary>
        /// <param name="id">ShiftDetail id for delete</param>
        /// <returns>return to ShiftMstrEdit action</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Delete")]
        public ActionResult ShiftDetDelete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                ShiftDetail shiftDetail = this.genericMgr.FindById<ShiftDetail>(id);
                this.genericMgr.DeleteById<ShiftDetail>(id);
                SaveSuccessMessage(Resources.MD.WorkingCalendar.ShiftDetail_Deleted);

                return RedirectToAction("ShiftMstrEdit/" + shiftDetail.Shift);
            }
        }
        #endregion

        #region  WorkingCalendar
        /// <summary>
        /// Index action for Currency controller
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// _Search action 
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult _Search(GridCommand command, WorkingCalendarSearchModel searchModel)
        {
            TempData["WorkingCalendarSearchModel"] = searchModel;
            return PartialView();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command"> Telerik GridCommand</param>
        /// <param name="searchModel">WorkingCalendar Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult List(GridCommand command, WorkingCalendarSearchModel searchModel)
        {
            //TempData["WorkingCalendarSearchModel"] = searchModel;
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        /// <summary>
        /// _AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">WorkingCalendar Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult _AjaxList(GridCommand command, WorkingCalendarSearchModel searchModel)
        {
            string replaceFrom = "_AjaxList";
            string replaceTo = "List/";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<WorkingCalendar>(searchStatementModel, command));
        }

        /// <summary>
        /// new action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult New()
        {
            return PartialView();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="workingCalendar">workingCalendar model</param>
        /// <returns>return to the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult New(WorkingCalendar workingCalendar)
        {
            if (ModelState.IsValid)
            {
                #region old
                //if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { workingCalendar.Id })[0] > 0)
                //{
                //    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, workingCalendar.Id.ToString());
                //}
                //else
                //{
                //    bool isExist = false;
                //    if (workingCalendar.Region == null)
                //    {
                //        if (this.genericMgr.FindAll<long>(isExistNullDayOfWeekDuiplicateVerifyStatement, new object[] { workingCalendar.DayOfWeek })[0] > 0)
                //        {
                //            isExist = true;
                //        }
                //    }
                //    else
                //    {
                //        if(!string.IsNullOrWhiteSpace(workingCalendar.Flow))
                //        {
                //            string hql = "select count(*) from WorkingCalendar as w where w.Region = ? and w.DayOfWeek=? and w.Flow=?";
                //            if (this.genericMgr.FindAll<long>(hql, new object[] { workingCalendar.Region, workingCalendar.DayOfWeek,workingCalendar.Flow })[0] > 0)
                //            {
                //                isExist = true;
                //            }
                //        }
                //        else 
                //        {
                //            if (this.genericMgr.FindAll<long>(isExistDayOfWeekDuiplicateVerifyStatement, new object[] { workingCalendar.Region, workingCalendar.DayOfWeek })[0] > 0)
                //            {
                //                isExist = true;
                //            }
                //        }
                //    }

                //    if (isExist)
                //    {
                //        SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_Existing_RegionAndDayOfWeek);
                //    }
                //    else
                //    {
                //        this.genericMgr.CreateWithTrim(workingCalendar);
                //        SaveSuccessMessage(Resources.MD.WorkingCalendar.WorkingCalendar_Added);
                //        return RedirectToAction("Edit/" + workingCalendar.Id);
                //    }
                //}
                #endregion

                try
                {
                    this.genericMgr.CreateWithTrim(workingCalendar);
                    SaveSuccessMessage(Resources.MD.WorkingCalendar.WorkingCalendar_Added);
                    return RedirectToAction("Edit/" + workingCalendar.Id);
                }
                catch (Exception ex)
                {
                    if (ex is NHibernate.Exceptions.DataException)
                    {
                        SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_Existing_RegionAndDayOfWeek);
                    }
                    else
                    {
                        SaveErrorMessage(ex.Message);
                    }
                }
            }

            return PartialView(workingCalendar);
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">WorkingCalendar id for edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.AssignedShift = this.genericMgr.FindAll<ShiftMaster>(selectShift, id);
                ViewBag.UnAssignedShift = this.genericMgr.FindAll<ShiftMaster>(selectShiftNotWithWorkingCalendar, id);
                WorkingCalendar workingCalendar = this.genericMgr.FindById<WorkingCalendar>(id);
                return PartialView(workingCalendar);
            }
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="workingCalendar">workingCalendar model</param>
        /// <param name="id">workingCalendar id for edit</param>
        /// <returns>return to the result view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult Edit(WorkingCalendar workingCalendar, int? id)
        {
            try
            {
                WorkingCalendar oldWorkingCalendarById = new WorkingCalendar();
                if (!id.HasValue)
                {
                    return HttpNotFound();
                }
                else
                {
                    oldWorkingCalendarById = this.genericMgr.FindById<WorkingCalendar>(id);
                }
                workingCalendar.Region = oldWorkingCalendarById.Region;
                if (ModelState.IsValid)
                {
                    bool isExist = false;
                    if (workingCalendar.DayOfWeek != oldWorkingCalendarById.DayOfWeek)
                    {
                        //if (workingCalendar.Region == null)
                        //{
                        //    if (this.genericMgr.FindAll<long>(isExistNullDayOfWeekDuiplicateVerifyStatement, new object[] { workingCalendar.DayOfWeek })[0] > 0)
                        //    {
                        //        isExist = true;
                        //    }
                        //}
                        //else
                        //{
                        //    if (this.genericMgr.FindAll<long>(isExistDayOfWeekDuiplicateVerifyStatement, new object[] { workingCalendar.Region, workingCalendar.DayOfWeek })[0] > 0)
                        //    {
                        //        isExist = true;
                        //    }
                        //}
                        //"select count(*) from WorkingCalendar as w where w.Region = ? and w.DayOfWeek=?";
                        string hql = "select count(*) from WorkingCalendar as w where w.DayOfWeek=?";
                        IList<object> obj = new List<object>();
                        obj.Add(workingCalendar.DayOfWeek);
                        workingCalendar.Region = oldWorkingCalendarById.Region;
                        if (!string.IsNullOrEmpty(workingCalendar.Region))
                        {
                            hql += "and w.Region = ?";
                            obj.Add(workingCalendar.DayOfWeek);
                        }
                        if (!string.IsNullOrEmpty(workingCalendar.Flow))
                        {
                            hql += "and w.Flow = ?";
                            obj.Add(workingCalendar.Flow);
                        }
                        if (this.genericMgr.FindAll<long>(hql, obj.ToArray())[0] > 0)
                        {
                            isExist = true;
                        }
                    }

                    if (isExist)
                    {
                        SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_Existing_RegionAndDayOfWeek);
                    }
                    else
                    {
                        string code = string.Empty;
                        if (this.Request.Params["Code[]"] != null)
                        {
                            code = this.Request.Params["Code[]"].ToString();
                            IList<string> shiftList = (code.Split(',')).ToList<string>();
                            this.WorkingCalendarMgr.UpdateWorkingCalendar(workingCalendar, shiftList);
                        }
                        else
                        {
                            this.WorkingCalendarMgr.UpdateWorkingCalendar(workingCalendar, null);
                        }

                        SaveSuccessMessage(Resources.MD.WorkingCalendar.WorkingCalendar_Updated);
                    }

                    ViewBag.AssignedShift = this.genericMgr.FindAll<ShiftMaster>(selectShift, id);
                    ViewBag.UnAssignedShift = this.genericMgr.FindAll<ShiftMaster>(selectShiftNotWithWorkingCalendar, id);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage((ex.InnerException).InnerException.ToString());
                ViewBag.AssignedShift = this.genericMgr.FindAll<ShiftMaster>(selectShift, id);
                ViewBag.UnAssignedShift = this.genericMgr.FindAll<ShiftMaster>(selectShiftNotWithWorkingCalendar, id);
                return PartialView(workingCalendar);
            }

            return PartialView(workingCalendar);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">workingCalendar id for delete</param>
        /// <returns>return to list view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Delete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return HttpNotFound();
                }
                else
                {
                    this.genericMgr.DeleteById<WorkingCalendar>(id);
                    SaveSuccessMessage(Resources.MD.WorkingCalendar.WorkingCalendar_Deleted);
                    return RedirectToAction("List");
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_DeletedUnsuccessfullyTheShiftIsOccupied);
            }
            return RedirectToAction("Edit/" +id);
        }

        #endregion

        #region SpecialTime
        /// <summary>
        /// SpecialTime action
        /// </summary>
        /// <returns>rediret view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult SpecialTime()
        {
            return PartialView();
        }

        /// <summary>
        /// SpecialTimeList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">SpecialTime Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult SpecialTimeList(GridCommand command, SpecialTimeSearchModel searchModel)
        {
            //TempData["SpecialTimeSearchModel"] = searchModel;
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        /// <summary>
        /// _AjaxSpecialTimeList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">SpecialTime Search Model</param>
        /// <returns>return to the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult _AjaxSpecialTimeList(GridCommand command, SpecialTimeSearchModel searchModel)
        {

            string replaceFrom = "_AjaxSpecialTimeList";
            string replaceTo = "SpecialTimeList/";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);

            SearchStatementModel searchStatementModel = this.SpecialTimePrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<SpecialTime>(searchStatementModel, command));
        }

        /// <summary>
        /// SpecialTimeNew action
        /// </summary>
        /// <returns>rediret view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult SpecialTimeNew()
        {
            SpecialTime st = new SpecialTime();
            st.StartTime = System.DateTime.Now;
            st.EndTime = System.DateTime.Now;
            return PartialView(st);
        }

        /// <summary>
        /// SpecialTimeNew action
        /// </summary>
        /// <param name="specialTime">specialTime model</param>
        /// <returns>return to the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult SpecialTimeNew(SpecialTime specialTime)
        {
            object[] searchPara= new object[specialTime.Flow==null?3:4];
            if (specialTime.Flow == null)
            {
                searchPara = new object[] { specialTime.Region, specialTime.Type, specialTime.StartTime, specialTime.EndTime };
            }
            else
            {
                searchPara = new object[] { specialTime.Region, specialTime.Type, specialTime.Flow, specialTime.StartTime, specialTime.EndTime };
            }
            if (ModelState.IsValid)
            {
                string hql = "select count(*) from SpecialTime where Type=?";
                IList<object> param = new List<object>();
                if (!string.IsNullOrEmpty(specialTime.Region))
                {
                    hql += " and Region=?";
                    param.Add(specialTime.Region);
                }
                //if (specialTime.Type!=-1)
                //{
                //    hql += " and Region=?";
                //    param.Add(specialTime.Region);
                //}
                if (!string.IsNullOrEmpty(specialTime.Region))
                {
                    param.Add(specialTime.Region);
                }
                if (!string.IsNullOrEmpty(specialTime.Region))
                {
                    param.Add(specialTime.Region);
                }
                if (!string.IsNullOrEmpty(specialTime.Region))
                {
                    param.Add(specialTime.Region);
                }




                if (this.genericMgr.FindAll<long>(specialTimeDuiplicateVerifyStatement.Replace( "and Flow=?",specialTime.Flow==null?"":" and Flow=?"), searchPara)[0] > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ErrorSameAreaTypeFlowTimeSpanAlreadyExits);
                }
                else if (this.genericMgr.FindAll<long>(specialTimeDuiplicateVerifyToStatement.Replace("and Flow=?", specialTime.Flow == null ? "" : " and Flow=?"), searchPara)[0] > 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_ErrorSameAreaTypeFlowTimeSpanAlreadyExits);
                }
                else
                {
                    if (System.DateTime.Compare(specialTime.EndTime, specialTime.StartTime) < 1)
                    {
                        SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
                    }
                    else
                    {
                        this.genericMgr.CreateWithTrim(specialTime);
                        SaveSuccessMessage(Resources.MD.WorkingCalendar.SpecialTime_Added);
                        return RedirectToAction("SpecialTimeList/" + specialTime.Id);
                    }
                }
            }

            return PartialView(specialTime);
        }

        /// <summary>
        /// SpecialTimeEdit action
        /// </summary>
        /// <param name="id">SpecialTime id for edit</param>
        /// <returns>return to the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_View")]
        public ActionResult SpecialTimeEdit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                SpecialTime specialTime = this.genericMgr.FindById<SpecialTime>(id);
                return PartialView(specialTime);
            }
        }

        /// <summary>
        /// SpecialTimeEdit action
        /// </summary>
        /// <param name="specialTime">specialTime model</param>
        /// <returns>return to the result view</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Edit")]
        public ActionResult SpecialTimeEdit(SpecialTime specialTime)
        {
            if (ModelState.IsValid)
            {
                if (System.DateTime.Compare(specialTime.EndTime, specialTime.StartTime) < 1)
                {
                    SaveErrorMessage(Resources.MD.WorkingCalendar.Errors_StartDateGreaterThanEndDate);
                }
                else
                {
                    this.genericMgr.UpdateWithTrim(specialTime);
                    SaveSuccessMessage(Resources.MD.WorkingCalendar.SpecialTime_Updated);
                }
            }

            return PartialView(specialTime);
        }

        /// <summary>
        /// SpecialTimeDelete action
        /// </summary>
        /// <param name="id">SpecialTime id for delete</param>
        /// <returns>return to SpecialTimeList action</returns>
        [SconitAuthorize(Permissions = "Url_WorkingCalendar_Delete")]
        public ActionResult SpecialTimeDelete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<SpecialTime>(id);
                SaveSuccessMessage(Resources.MD.WorkingCalendar.SpecialTime_Deleted);
                return RedirectToAction("SpecialTimeList");
            }
        }
        #endregion

        #region SearchStatement
        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">SpecialTime Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel SpecialTimePrepareSearchStatement(GridCommand command, SpecialTimeSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Region == null)
            {
                // whereStatement = " where Region is null";
                HqlStatementHelper.AddLikeStatement("Region", string.Empty, HqlStatementHelper.LikeMatchMode.Start, "s", ref whereStatement, param);
            }
            else
            {
                HqlStatementHelper.AddEqStatement("Region", searchModel.Region, "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "s", ref whereStatement, param);

            if (searchModel.StartTime != null)
            {
                HqlStatementHelper.AddGeStatement("StartTime", searchModel.StartTime, "s", ref whereStatement, param);
            }
            if (searchModel.EndTime != null)
            {
                HqlStatementHelper.AddLeStatement("EndTime", searchModel.EndTime, "s", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = specialTimeSelectCountStatement;
            searchStatementModel.SelectStatement = specialTimeSelectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = "Order by StartTime desc";
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">WorkingCalendar Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, WorkingCalendarSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Region == null)
            {
                // whereStatement = " where Region is null";
                HqlStatementHelper.AddLikeStatement("Region", string.Empty, HqlStatementHelper.LikeMatchMode.Start, "w", ref whereStatement, param);
            }
            else
            {
                HqlStatementHelper.AddEqStatement("Region", searchModel.Region, "w", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "w", ref whereStatement, param);
            ////HqlStatementHelper.AddLikeStatement("DayOfWeek", searchModel.DayOfWeek, HqlStatementHelper.LikeMatchMode.Anywhere, "w", ref whereStatement, param);
            ////HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "w", ref whereStatement, param);
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
        /// <param name="searchModel">ShiftMaster Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel ShiftMstrPrepareSearchStatement(GridCommand command, ShiftMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "s", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Start, "s", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = shiftMstrSelectCountStatement;
            searchStatementModel.SelectStatement = shiftMstrSelectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">ShiftDetail Search Model</param>
        /// /// <param name="id">ShiftDetail id</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel ShiftDetailPrepareSearchStatement(GridCommand command, ShiftDetailSearchModel searchModel, string id)
        {
            string whereStatement = " where s.Shift ='" + id + "'";

            IList<object> param = new List<object>();
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = shiftDetailSelectCountStatement;
            searchStatementModel.SelectStatement = shiftDetailSelectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">WorkingCalendar Search Model</param>
        /// <returns>Search Statement</returns>
        private SearchStatementModel ViewPrepareSearchStatement(GridCommand command, WorkingCalendarSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = shiftDetailSelectCountStatement;
            searchStatementModel.SelectStatement = shiftDetailSelectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion
     
    }
}
