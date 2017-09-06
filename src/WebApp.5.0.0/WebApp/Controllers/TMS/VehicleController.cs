/// <summary>
/// Summary description for VehicleController
/// </summary>
namespace com.Sconit.Web.Controllers.TMS
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
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.TMS;
    using com.Sconit.Web.Models.SearchModels.TMS;
    using com.Sconit.Utility;
    #endregion

    /// <summary>
    /// This controller response to control the Vehicle.
    /// </summary>
    public class VehicleController : WebAppBaseController
    {
        /// <summary>
        /// hql to get count of the vehicle
        /// </summary>
        private static string selectCountStatement = "select count(*) from Vehicle as t";

        /// <summary>
        /// hql to get all of the vehicle
        /// </summary>
        private static string selectStatement = "select t from Vehicle as t";

        /// <summary>
        /// hql to get count of the vehicle by vehicle's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from Vehicle as t where t.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for Vehicle controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_Vehicle_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Vehicle Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Vehicle_View")]
        public ActionResult List(GridCommand command, VehicleSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Vehicle Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Vehicle_View")]
        public ActionResult _AjaxList(GridCommand command, VehicleSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Vehicle>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_Vehicle_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="vehicle">Vehicle Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Vehicle_Edit")]
        public ActionResult New(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                if (base.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { vehicle.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, vehicle.Code);
                }
                else
                {
                    base.genericMgr.Create(vehicle);
                    SaveSuccessMessage(Resources.TMS.Vehicle.Vehicle_Added);
                    return RedirectToAction("Edit/" + vehicle.Code);
                }
            }

            return View(vehicle);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">vehicle id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Vehicle_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                Vehicle vehicle = base.genericMgr.FindById<Vehicle>(id);
                return View(vehicle);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="vehicle">Vehicle Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_Vehicle_Edit")]
        public ActionResult Edit(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                base.genericMgr.Update(vehicle);
                SaveSuccessMessage(Resources.TMS.Vehicle.Vehicle_Updated);
            }

            return View(vehicle);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">vehicle id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_Vehicle_Edit")]
        public ActionResult Delete(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<Vehicle>(code);
                SaveSuccessMessage(Resources.TMS.Vehicle.Vehicle_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Vehicle Search Model</param>
        /// <returns>return vehicle search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, VehicleSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);

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
