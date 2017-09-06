/// <summary>
/// Summary description for AddressController
/// </summary>

namespace com.Sconit.Web.Controllers.PRD
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
    using com.Sconit.Entity.MRP.MD;
    using System;
    using com.Sconit.Web.Models.SearchModels.INV;
    using com.Sconit.Entity.PRD;
    #endregion

    /// <summary>
    /// This controller response to control the Currency.
    /// </summary>
    public class SmallSparePartChkController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the SmallSparePartChk security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the SmallSparePartChk 
        /// </summary>
        private static string selectCountStatement = "select count(*) from SmallSparePartChk as i";

        /// <summary>
        /// hql to get all of the SmallSparePartChk
        /// </summary>
        private static string selectStatement = "select i from SmallSparePartChk as i";

        /// <summary>
        /// hql to get count of the SmallSparePartChk by SmallSparePartChk's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from SmallSparePartChk as i where i.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for SmallSparePartChk controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_SmallSparePartChk_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">SmallSparePartChk Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_SmallSparePartChk_View")]
        public ActionResult List(GridCommand command, HuSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">SmallSparePartChk Search Model</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SmallSparePartChk_View")]
        public ActionResult _AjaxList(GridCommand command, HuSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<SmallSparePartChk>(searchStatementModel, command));
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">SmallSparePartChk Search Model</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, HuSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Huid", searchModel.HuId, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("SpareItem", searchModel.SpareItem, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("HuItem", searchModel.HuItem, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("CreateUserName", searchModel.CreateUserName, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "i", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "i", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "i", ref whereStatement, param);
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = " order by i.CreateDate ";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        #region ExportRpt
        [SconitAuthorize(Permissions = "Url_SmallSparePartChk_View")]
        public void ExportXLS(HuSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<SmallSparePartChk> gridModel = GetAjaxPageData<SmallSparePartChk>(searchStatementModel, command);
            var fileName = string.Format("SmallMatchSparePartCheck.xls");
            ExportToXLS<SmallSparePartChk>(fileName, gridModel.Data.ToList());
        }
        #endregion
    }
}
