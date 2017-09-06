using com.Sconit.Service;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models;
using com.Sconit.Entity.MES;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using com.Sconit.Web.Models.SearchModels.MES;
using System.Web;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using com.Sconit.Utility;
namespace com.Sconit.Web.Controllers.MES
{


    public class FacilityParamaterController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from MesScanControlPoint as c";

        /// <summary>
        /// hql to get all of the Container
        /// </summary>
        private static string selectStatement = "select c from MesScanControlPoint as c";



        [SconitAuthorize(Permissions = "Url_FacilityParamater_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Container Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityParamater_View")]
        public ActionResult List(GridCommand command, MesScanControlPointSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Container Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_FacilityParamater_View")]
        public ActionResult _AjaxList(GridCommand command, MesScanControlPointSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<MesScanControlPoint>(searchStatementModel, command));
        }





        private SearchStatementModel PrepareSearchStatement(GridCommand command, MesScanControlPointSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("TraceCode", searchModel.TraceCode, HqlStatementHelper.LikeMatchMode.Start, "c", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ControlPoint", searchModel.ControlPoint, HqlStatementHelper.LikeMatchMode.Start, "c", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type",CodeMaster.FacilityParamaterType.Paramater,"c", ref whereStatement, param);

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