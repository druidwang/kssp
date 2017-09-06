/// <summary>
/// Summary description for FacilityTransController
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
    /// This controller response to control the FacilityTrans.
    /// </summary>
    public class FacilityTransController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the facilityTrans security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the facilityTrans
        /// </summary>
        private static string selectCountStatement = "select count(*) from FacilityTrans as f";

        /// <summary>
        /// hql to get all of the facilityTrans
        /// </summary>
        private static string selectStatement = "select f from FacilityTrans as f";


        #region public actions
        /// <summary>
        /// Index action for FacilityTrans controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_FacilityTrans_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">FacilityTrans Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityTrans_View")]
        public ActionResult List(GridCommand command, FacilityTransSearchModel searchModel)
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
        /// <param name="searchModel">FacilityTrans Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityTrans_View")]
        public ActionResult _AjaxList(GridCommand command, FacilityTransSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<FacilityTrans>(searchStatementModel, command));
        }


        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">facilityTrans id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_FacilityTrans_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                FacilityTrans facilityTrans = this.genericMgr.FindById<FacilityTrans>(int.Parse(id));
                facilityTrans.TransTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.FacilityTransType, facilityTrans.TransType.ToString());
                return View(facilityTrans);
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">FacilityTrans Search Model</param>
        /// <returns>return facilityTrans search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, FacilityTransSearchModel searchModel)
        {            
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("FCID", searchModel.FCID, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Name", searchModel.Name, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("AssetNo", searchModel.AssetNo, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Category", searchModel.Category, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("TransType", searchModel.TransType, "f", ref whereStatement, param);

            if (searchModel.DateFrom != null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "f", ref whereStatement, param);
            }
            if (searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLtStatement("CreateDate", searchModel.DateTo.Value.AddDays(1), "f", ref whereStatement, param);
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
