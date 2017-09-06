/// <summary>
/// Summary description for AddressController
/// </summary>
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
    using com.Sconit.Utility;
    #endregion

    /// <summary>
    /// This controller response to control the Address.
    /// </summary>
    public class TaskSubTypeController : WebAppBaseController
    {
        /// <summary>
        /// hql to get count of the address
        /// </summary>
        private static string selectCountStatement = "select count(*) from TaskSubType as t";

        /// <summary>
        /// hql to get all of the address
        /// </summary>
        private static string selectStatement = "select t from TaskSubType as t";

        /// <summary>
        /// hql to get count of the address by address's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from TaskSubType as t where t.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for Address controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_TaskSubType_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Address Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_TaskSubType_View")]
        public ActionResult List(GridCommand command, TaskSubTypeSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Address Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_TaskSubType_View")]
        public ActionResult _AjaxList(GridCommand command, TaskSubTypeSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<TaskSubType>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_TaskSubType_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="address">Address Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_TaskSubType_Edit")]
        public ActionResult New(TaskSubType taskSubType)
        {
            if (ModelState.IsValid)
            {
                if (base.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { taskSubType.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, taskSubType.Code);
                }
                else
                {
                    taskSubType.IsActive = true;
                    base.genericMgr.Create(taskSubType);
                    SaveSuccessMessage(Resources.ISI.TaskSubType.TaskSubType_SubType_Added);
                    return RedirectToAction("Edit/" + taskSubType.Code);
                }
            }

            return View(taskSubType);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">address id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_TaskSubType_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                TaskSubType taskSubType = base.genericMgr.FindById<TaskSubType>(id);
                return View(taskSubType);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="address">Address Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_TaskSubType_Edit")]
        public ActionResult Edit(TaskSubType taskSubType)
        {
            if (ModelState.IsValid)
            {
                base.genericMgr.Update(taskSubType);
                SaveSuccessMessage(Resources.ISI.TaskSubType.TaskSubType_SubType_Updated);
            }

            return View(taskSubType);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">address id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_TaskSubType_Edit")]
        public ActionResult Delete(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<TaskSubType>(code);
                SaveSuccessMessage(Resources.ISI.TaskSubType.TaskSubType_SubType_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Address Search Model</param>
        /// <returns>return address search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, TaskSubTypeSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "AddressTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Type";
                }
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
