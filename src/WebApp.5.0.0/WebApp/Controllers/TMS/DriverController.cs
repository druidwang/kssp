/// <summary>
/// Summary description for AddressController
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
    /// This controller response to control the Address.
    /// </summary>
    public class DriverController : WebAppBaseController
    {
        /// <summary>
        /// hql to get count of the address
        /// </summary>
        private static string selectCountStatement = "select count(*) from Driver as t";

        /// <summary>
        /// hql to get all of the address
        /// </summary>
        private static string selectStatement = "select t from Driver as t";

        /// <summary>
        /// hql to get count of the address by address's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from Driver as t where t.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for Address controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_Driver_View")]
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
        [SconitAuthorize(Permissions = "Url_Driver_View")]
        public ActionResult List(GridCommand command, DriverSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_Driver_View")]
        public ActionResult _AjaxList(GridCommand command, DriverSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Driver>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_Driver_Edit")]
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
        [SconitAuthorize(Permissions = "Url_Driver_Edit")]
        public ActionResult New(Driver Driver)
        {
            if (ModelState.IsValid)
            {
                if (base.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { Driver.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, Driver.Code);
                }
                else
                {
                    base.genericMgr.Create(Driver);
                    SaveSuccessMessage(Resources.MD.Address.Address_Added);
                    return RedirectToAction("Edit/" + Driver.Code);
                }
            }

            return View(Driver);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">address id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Driver_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                Driver Driver = base.genericMgr.FindById<Driver>(id);
                return View(Driver);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="address">Address Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_Driver_Edit")]
        public ActionResult Edit(Driver Driver)
        {
            if (ModelState.IsValid)
            {
                base.genericMgr.Update(Driver);
                SaveSuccessMessage(Resources.TMS.Driver.Driver_Updated);
            }

            return View(Driver);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">address id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_Driver_Edit")]
        public ActionResult Delete(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return HttpNotFound();
            }
            else
            {
                base.genericMgr.DeleteById<Driver>(code);
                SaveSuccessMessage(Resources.TMS.Driver.Driver_Deleted);
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
        private SearchStatementModel PrepareSearchStatement(GridCommand command, DriverSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Name, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);

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
