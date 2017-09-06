/// <summary>
/// Summary description for MaintainPlanController
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
    /// This controller response to control the MaintainPlan.
    /// </summary>
    public class MaintainPlanController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the maintainPlan security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the maintainPlan
        /// </summary>
        private static string selectCountStatement = "select count(*) from MaintainPlan as m";

        /// <summary>
        /// hql to get all of the maintainPlan
        /// </summary>
        private static string selectStatement = "select m from MaintainPlan as m";

        /// <summary>
        /// hql to get count of the maintainPlan by maintainPlan's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from MaintainPlan as m where m.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for MaintainPlan controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_MaintainPlan_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MaintainPlan Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MaintainPlan_View")]
        public ActionResult List(GridCommand command, MaintainPlanSearchModel searchModel)
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
        /// <param name="searchModel">MaintainPlan Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MaintainPlan_View")]
        public ActionResult _AjaxList(GridCommand command, MaintainPlanSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<MaintainPlan>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_MaintainPlan_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="maintainPlan">MaintainPlan Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_MaintainPlan_Edit")]
        public ActionResult New(MaintainPlan maintainPlan)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { maintainPlan.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, maintainPlan.Code);
                }
                else
                {
                    
                    this.genericMgr.CreateWithTrim(maintainPlan);
                    SaveSuccessMessage(Resources.FMS.MaintainPlan.MaintainPlan_Added);
                    return RedirectToAction("Edit/" + maintainPlan.Code);
                }
            }

            return View(maintainPlan);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">maintainPlan id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MaintainPlan_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                MaintainPlan maintainPlan = this.genericMgr.FindById<MaintainPlan>(id);
                return View(maintainPlan);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="maintainPlan">MaintainPlan Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_MaintainPlan_Edit")]
        public ActionResult Edit(MaintainPlan maintainPlan)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(maintainPlan);
                SaveSuccessMessage(Resources.FMS.MaintainPlan.MaintainPlan_Updated);
            }

            return View(maintainPlan);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">maintainPlan id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_MaintainPlan_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<MaintainPlan>(id);
                SaveSuccessMessage(Resources.FMS.MaintainPlan.MaintainPlan_Deleted);
                return RedirectToAction("List");
            }
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MaintainPlan_Edit")]
        public ActionResult _MaintainPlanItem(string maintainPlanCode)
        {
            IList<MaintainPlanItem> maintainPlanItemList = genericMgr.FindAll<MaintainPlanItem>("from MaintainPlanItem where MaintainPlanCode=?", maintainPlanCode);
            return PartialView(maintainPlanItemList);

        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MaintainPlan Search Model</param>
        /// <returns>return maintainPlan search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, MaintainPlanSearchModel searchModel)
        {            
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "m", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Anywhere, "m", ref whereStatement, param);
         
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
