
namespace com.Sconit.Web.Controllers.ISI
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
    using com.Sconit.Web.Models.SearchModels.ISI;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.ISI;
    using com.Sconit.Entity.Exception;
    using System;
    using com.Sconit.Web.Models.SearchModels.MD;
    using com.Sconit.Utility;
    #endregion

    public class TaskMasterController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from TaskMaster as t";


        private static string selectStatement = "select t from TaskMaster as t";


        private static string duiplicateVerifyStatement = @"select count(*) from TaskMaster as t where t.Code = ?";

        #region public actions
        public ITaskMgr taskMgr { get; set; }

        [SconitAuthorize(Permissions = "Url_TaskMaster_View")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_TaskMaster_View")]
        public ActionResult List(GridCommand command, TaskMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_TaskMaster_View")]
        public ActionResult _AjaxList(GridCommand command, TaskMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<TaskMaster>(searchStatementModel, command));
        }


        [SconitAuthorize(Permissions = "Url_TaskMaster_Edit")]
        public ActionResult New()
        {
            return View();
        }


        [HttpPost]
        [SconitAuthorize(Permissions = "Url_TaskMaster_Edit")]
        public ActionResult New(TaskMaster task)
        {
            if (ModelState.IsValid)
            {

                TaskSubType taskSubType = genericMgr.FindById<TaskSubType>(task.TaskSubType);
                task.Type = taskSubType.Type;

                taskMgr.CreateTask(task);
                SaveSuccessMessage(Resources.ISI.TaskMaster.TaskMaster_Added);
                return RedirectToAction("Edit/" + task.Code);

            }

            return View(task);
        }

        [SconitAuthorize(Permissions = "Url_TaskMaster_Edit")]
        public ActionResult Edit(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return HttpNotFound();
            }
            else
            {
                return View("Edit", string.Empty, code);
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_TaskMaster_Edit")]
        public ActionResult _Edit(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return HttpNotFound();
            }
            else
            {
                TaskMaster task = base.genericMgr.FindById<TaskMaster>(code);
                task.StatusDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.TaskStatus, ((int)task.Status).ToString());
                return PartialView(task);
            }
        }



        [SconitAuthorize(Permissions = "Url_TaskMaster_Edit")]
        public ActionResult _Edit(TaskMaster task)
        {
            try
            {
                taskMgr.UpdateTask(task);
                SaveSuccessMessage(Resources.ISI.TaskMaster.TaskMaster_Updated);
            }

            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { code = task.Code });
        }


        [SconitAuthorize(Permissions = "Url_TaskMaster_Edit")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<TaskMaster>(id);
                SaveSuccessMessage(Resources.ISI.TaskMaster.TaskMaster_Deleted);
                return RedirectToAction("List");
            }
        }


        [SconitAuthorize(Permissions = "Url_TaskMaster_Submit")]
        public ActionResult Submit(string id)
        {

            try
            {
                taskMgr.SubmitTask(id, this.CurrentUser);
                SaveSuccessMessage(Resources.ISI.TaskMaster.TaskMaster_Submited);
            }

            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { code = id });
        }

        [SconitAuthorize(Permissions = "Url_TaskMaster_Assign")]
        public JsonResult Assign(string code, string assignStartUser, DateTime? planStartDate, DateTime? planCompleteDate, string expectedResults, string description2)
        {
            try
            {
                if (planStartDate == null)
                {
                    SaveErrorMessage("预计开始日期不能为空");
                }
                else if (planCompleteDate == null)
                {
                    SaveErrorMessage("预计完成日期不能为空");
                }
                else
                {
                    taskMgr.AssignTask(code, assignStartUser, planStartDate.Value, planCompleteDate.Value, expectedResults, description2, this.CurrentUser);
                    SaveSuccessMessage(Resources.ISI.TaskMaster.TaskMaster_Assigned);
                }
            }

            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return Json(new { Code = code });
        }



        [SconitAuthorize(Permissions = "Url_TaskMaster_Start")]
        public ActionResult Start(string id)
        {
            try
            {
                taskMgr.ConfirmTask(id, this.CurrentUser);
                SaveSuccessMessage(Resources.ISI.TaskMaster.TaskMaster_Started);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { code = id });
        }

        [SconitAuthorize(Permissions = "Url_TaskMaster_Complete")]
        public ActionResult Complete(string id)
        {
            try
            {
                taskMgr.CompleteTask(id, this.CurrentUser);
                SaveSuccessMessage(Resources.ISI.TaskMaster.TaskMaster_Completed);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { code = id });
        }

        [SconitAuthorize(Permissions = "Url_TaskMaster_Close")]
        public ActionResult Close(string id)
        {
            try
            {
                taskMgr.CloseTask(id, this.CurrentUser);
                SaveSuccessMessage(Resources.ISI.TaskMaster.TaskMaster_Closed);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { code = id });

        }


        #region 进展
        private static string selectTaskStatusCountStatement = "select count(*) from TaskStatus as  t  ";
        private static string selectTaskStatusStatement = "select t from TaskStatus as  t ";

        [SconitAuthorize(Permissions = "Url_TaskMaster_View")]
        public ActionResult _TaskStatus(string code)
        {
            ViewBag.TaskCode = code;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_TaskMaster_View")]
        public ActionResult _TaskStatusList(GridCommand command, TaskStatusSearchModel searchModel, string taskCode)
        {
            ViewBag.TaskCode = taskCode;
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_TaskMaster_View")]
        public ActionResult _AjaxTaskStatusList(GridCommand command, TaskStatusSearchModel searchModel, string taskCode)
        {
            ViewBag.TaskCode = taskCode;
            SearchStatementModel searchStatementModel = PrepareSearchTaskStatusStatement(command, searchModel, taskCode);
            return PartialView(GetAjaxPageData<TaskStatus>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_TaskMaster_View")]
        public ActionResult _TaskStatusNew(string taskCode)
        {
            TaskStatus taskStatus = new TaskStatus();
            TaskMaster task = genericMgr.FindById<TaskMaster>(taskCode);
            taskStatus.TaskCode = taskCode;
            taskStatus.Color = task.Color;
            taskStatus.Description1 = task.Description1;
            taskStatus.Priority = task.Priority;
            taskStatus.Subject = task.Subject;
            taskStatus.StatusDescription = task.StatusDescription;
            taskStatus.TaskAddress = task.TaskAddress;
            taskStatus.TaskSubType = task.TaskSubType;
            return PartialView(taskStatus);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
        public ActionResult _TaskStatusNew(TaskStatus taskStatus)
        {
            if (ModelState.IsValid)
            {
                taskMgr.CreateTaskStatus(taskStatus, this.CurrentUser, false);
                SaveSuccessMessage(Resources.ISI.TaskStatus.TaskStatus_Added);
                return RedirectToAction("_TaskStatusEdit/" + taskStatus.Id);
            }
            return PartialView(taskStatus);
        }

        [SconitAuthorize(Permissions = "Url_Customer_Delete")]
        public ActionResult DeleteTaskStatus(int? id, string taskCode)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<TaskStatus>(id);
                SaveSuccessMessage(Resources.ISI.TaskStatus.TaskStatus_Deleted);
                return new RedirectToRouteResult(new RouteValueDictionary { 
                                                        { "action", "_TaskStatusList" }, 
                                                        { "controller", "TaskMaster" }, 
                                                        { "TaskCode", taskCode } });
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Customer_Edit")]
        public ActionResult _TaskStatusEdit(int? Id)
        {
            if (!Id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                TaskStatus taskStatus = base.genericMgr.FindById<TaskStatus>(Id);
                TaskMaster task = genericMgr.FindById<TaskMaster>(taskStatus.TaskCode);
                taskStatus.Color = task.Color;
                taskStatus.Description1 = task.Description1;
                taskStatus.Priority = task.Priority;
                taskStatus.Subject = task.Subject;
                taskStatus.StatusDescription = task.StatusDescription;
                taskStatus.TaskSubType = task.TaskSubType;
                taskStatus.TaskAddress = task.TaskAddress;

                return PartialView(taskStatus);
            }

        }



        #endregion

        #endregion


        #region private actiions
        private SearchStatementModel PrepareSearchStatement(GridCommand command, TaskMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);

            if (command.SortDescriptors.Count > 0)
            {
                //if (command.SortDescriptors[0].Member == "AddressTypeDescription")
                //{
                //    command.SortDescriptors[0].Member = "Type";
                //}
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

        private SearchStatementModel PrepareSearchTaskStatusStatement(GridCommand command, TaskStatusSearchModel searchModel, string taskCode)
        {
            string whereStatement = "  where t.TaskCode ='" + taskCode + "'";
            IList<object> param = new List<object>();
            //HqlStatementHelper.AddLikeStatement("Party", searchModel.Party, HqlStatementHelper.LikeMatchMode.Anywhere, "pa", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "t", ref whereStatement, param);
            if (searchModel.StartDate != null)
            {
                HqlStatementHelper.AddGeStatement("StartDate", searchModel.StartDate.Value, "t", ref whereStatement, param);
            }
            if (searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("StartDate", searchModel.EndDate.Value, "t", ref whereStatement, param);
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectTaskStatusCountStatement;
            searchStatementModel.SelectStatement = selectTaskStatusStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        #endregion
    }
}
