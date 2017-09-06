/// <summary>
/// Summary description for FacilityCategoryController
/// </summary>
namespace com.Sconit.Web.Controllers.FMS
{
    #region reference
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
    using com.Sconit.Web.Models.SearchModels.FMS;
    using com.Sconit.Entity.FMS;
    using com.Sconit.Entity.ACC;
    #endregion

    /// <summary>
    /// This controller response to control the FacilityCategory.
    /// </summary>
    public class FacilityCategoryController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the facilityCategory security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the facilityCategory
        /// </summary>
        private static string selectCountStatement = "select count(*) from FacilityCategory as f";

        /// <summary>
        /// hql to get all of the facilityCategory
        /// </summary>
        private static string selectStatement = "select f from FacilityCategory as f";

        /// <summary>
        /// hql to get count of the facilityCategory by facilityCategory's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from FacilityCategory as f where f.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for FacilityCategory controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_FacilityCategory_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">FacilityCategory Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityCategory_View")]
        public ActionResult List(GridCommand command, FacilityCategorySearchModel searchModel)
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
        /// <param name="searchModel">FacilityCategory Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityCategory_View")]
        public ActionResult _AjaxList(GridCommand command, FacilityCategorySearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<FacilityCategory>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_FacilityCategory_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="facilityCategory">FacilityCategory Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_FacilityCategory_Edit")]
        public ActionResult New(FacilityCategory facilityCategory)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { facilityCategory.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, facilityCategory.Code);
                }
                else
                {
                    facilityCategory.ChargePersonName = genericMgr.FindById<User>(facilityCategory.ChargePersonId).FullName;
                    this.genericMgr.CreateWithTrim(facilityCategory);
                    SaveSuccessMessage(Resources.FMS.FacilityCategory.FacilityCategory_Added);
                    return RedirectToAction("Edit/" + facilityCategory.Code);
                }
            }

            return View(facilityCategory);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">facilityCategory id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_FacilityCategory_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                FacilityCategory facilityCategory = this.genericMgr.FindById<FacilityCategory>(id);
                return View(facilityCategory);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="facilityCategory">FacilityCategory Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_FacilityCategory_Edit")]
        public ActionResult Edit(FacilityCategory facilityCategory)
        {
            if (ModelState.IsValid)
            {
                facilityCategory.ChargePersonName = genericMgr.FindById<User>(facilityCategory.ChargePersonId).FullName;
                this.genericMgr.UpdateWithTrim(facilityCategory);
                SaveSuccessMessage(Resources.FMS.FacilityCategory.FacilityCategory_Updated);
            }

            return View(facilityCategory);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">facilityCategory id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_FacilityCategory_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<FacilityCategory>(id);
                SaveSuccessMessage(Resources.FMS.FacilityCategory.FacilityCategory_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">FacilityCategory Search Model</param>
        /// <returns>return facilityCategory search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, FacilityCategorySearchModel searchModel)
        {            
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ChargePersonName", searchModel.ChargePersonName, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
        
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
