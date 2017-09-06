using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Web.Models;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using com.Sconit.Service.MRP;
using com.Sconit.Service;
using com.Sconit.Entity.MRP.VIEW;
using Telerik.Web.Mvc.UI;
using com.Sconit.Entity.MD;
using System.Text;
using com.Sconit.Entity.SCM;

namespace com.Sconit.Web.Controllers.MRP
{
    public class MrpPlanController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from MrpPlan as m";

        private static string selectStatement = "select m from MrpPlan as m";

        public IPlanMgr planMgr { get; set; }

        //public IGenericMgr genericMgr { get; set; }

        #region Public Method
        [SconitAuthorize(Permissions = "MRP_MrpPlan_View")]
        public ActionResult Index()
        {
            return View();
        }



        [SconitAuthorize(Permissions = "MRP_MrpPlan_View")]
        public ActionResult _GetMrpPlanList(MrpPlanSearchModel searchModel)
        {
            ViewBag.PageSize = 20;
            ViewBag.ImportType = searchModel.ImportType;
            ViewBag.Flow = searchModel.Flow;
            ViewBag.StartDate = searchModel.StartDate;
            ViewBag.Item = searchModel.Item;
            ViewBag.EndDate = searchModel.EndDate;
            ViewBag.StartWeek = searchModel.StartWeek;
            ViewBag.EndWeek = searchModel.EndWeek;
            return View();

        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "MRP_MrpPlan_View")]
        public ActionResult _Update(GridCommand command, MrpPlan mrpPlan, string importTypeTo, string flowTo, DateTime? startDateTo
            , DateTime? endDateTo, string startWeekTo, string endWeekTo, string itemTo)
        {
            MrpPlanSearchModel searchModel = new MrpPlanSearchModel();
            searchModel.ImportType = importTypeTo;
            searchModel.Flow = flowTo;
            searchModel.Item = itemTo;
            searchModel.StartDate = startDateTo;
            searchModel.EndDate = endDateTo;
            searchModel.StartWeek = startWeekTo;
            searchModel.EndWeek = endWeekTo;

            MrpPlan newMrpPlan = genericMgr.FindAll<MrpPlan>(
                @" from MrpPlan as m where  m.PlanDate=? and m.Item=? and m.Flow=? and m.Location=?",
                new object[] { mrpPlan.PlanDate, mrpPlan.Item, mrpPlan.Flow, mrpPlan.Location })[0];
            if (mrpPlan.Qty != newMrpPlan.Qty || mrpPlan.OrderQty != newMrpPlan.OrderQty)
            {
                newMrpPlan.CurrentQty = mrpPlan.Qty;
                newMrpPlan.OrderQty = mrpPlan.OrderQty;
                this.planMgr.UpdateMrpPlan(newMrpPlan);
            }

            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<MrpPlan>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "MRP_MrpPlan_View")]
        public ActionResult _AjaxList(GridCommand command, MrpPlanSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<MrpPlan>(searchStatementModel, command));
        }
        private SearchStatementModel PrepareSearchStatement(GridCommand command, MrpPlanSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.ImportType == "0")
            {
                if (searchModel.StartDate != null & searchModel.EndDate != null)
                {
                    HqlStatementHelper.AddBetweenStatement("PlanDate", searchModel.StartDate, searchModel.EndDate, "m", ref whereStatement, param);
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(searchModel.StartWeek) && !string.IsNullOrEmpty(searchModel.EndWeek))
                {
                    DateTime dateFrom = com.Sconit.Utility.DateTimeHelper.GetWeekIndexDateFrom(searchModel.StartWeek);
                    DateTime dateTo = com.Sconit.Utility.DateTimeHelper.GetWeekIndexDateFrom(searchModel.EndWeek);
                    HqlStatementHelper.AddBetweenStatement("PlanDate", dateFrom, dateTo, "m", ref whereStatement, param);
                }

            }

            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "m", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        //public string _GetMaxPlanVersion(string table)
        //{
        //    string sql = "select max(planversion) as planVersion from " + table + " ";

        //    IList<object> planVersions = this.queryMgr.FindAllWithNativeSql<object>(sql);

        //    return planVersions.Count > 0 ? planVersions[0].ToString() : "";
        //}
        #endregion

        #region New
        [SconitAuthorize(Permissions = "MRP_MrpPlan_New")]
        public ActionResult New()
        {
            return View();
        }


        [SconitAuthorize(Permissions = "MRP_MrpPlan_New")]
        public ActionResult ImportMrpPlanDay(IEnumerable<HttpPostedFileBase> mrpPlanImportDay, string flow,
            DateTime? startDate, DateTime? endDate, com.Sconit.CodeMaster.TimeUnit periodType)
        {
            try
            {
                if (!(mrpPlanImportDay.Count() > 0))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ImportTemplateDetailBeEmpty);
                }
                if (startDate == null || endDate == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_StartToTimeAndEndTimeCanNotBeEmpty);
                }

                foreach (var file in mrpPlanImportDay)
                {
                    planMgr.ReadDailyMrpPlanFromXls(file.InputStream, startDate, endDate, flow, false);
                }
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CustomerCalendarDailyPlanImportSuccessfully);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Content("");
        }

        [SconitAuthorize(Permissions = "MRP_MrpPlan_New")]
        public ActionResult ImportMrpPlanWeek(IEnumerable<HttpPostedFileBase> mrpPlanImportWeek,
            string startWeek, string endWeek, string flow, com.Sconit.CodeMaster.TimeUnit periodType)
        {
            try
            {
                if (!(mrpPlanImportWeek.Count() > 0))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ImportTemplateDetailBeEmpty);
                }
                if (string.IsNullOrEmpty(startWeek) || string.IsNullOrEmpty(endWeek))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_StartWeekAndEndWeekCanNotBeEmpty);
                }
                foreach (var file in mrpPlanImportWeek)
                {
                    planMgr.ReadWeeklyMrpPlanFromXls(file.InputStream, startWeek, endWeek, flow, false);
                }
                object obj = Resources.EXT.ControllerLan.Con_CustomerCalendarWeekPlanImportSuccessfully;

                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_CustomerCalendarDailyPlanImportSuccessfully);
                return Json(new { status = obj }, "text/plain");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Content("");
        }

        public string _GetStartTime(string weekOfYear)
        {
            if (string.IsNullOrWhiteSpace(weekOfYear))
            {
                return string.Empty;
            }
            else
            {
                return DateTimeHelper.GetWeekIndexDateFrom(weekOfYear).ToString("yyyy-MM-dd");
            }

        }

        public ActionResult _GetPlanDailyLog(string id)
        {
            return Content("");
        }

        #endregion

        [SconitAuthorize(Permissions = "Url_Mrp_MrpPlan_View")]
        public ActionResult MrpPlanView()
        {
            //IList<object> planVersions = this.queryMgr.FindAllWithNativeSql<object>
            //    ("select MAX(PlanVersion) from MRP_MrpPlan");

            //ViewBag.CurrentPlanVersion = planVersions[0];
            return View();
        }

        public object _GetMaxPlanVersion(string flow)
        {
            IList<object> planVersions = this.queryMgr.FindAllWithNativeSql<object>
                 ("select MAX(PlanVersion) from MRP_MrpPlan where Flow =? ", flow);

            return planVersions[0];
        }
        #region Export MrpPlanView
        public ActionResult Export(MrpPlanSearchModel searchModel)
        {
            var table = _GetMrpPlanView(searchModel);
            return new DownloadFileActionResult(table, "ProdLineQty.xls");
        }
        #endregion
        public string _GetMrpPlanView(MrpPlanSearchModel searchModel)
        {
            //var flowMaster = this.genericMgr.FindById<FlowMaster>(searchModel.Flow);
            //if (!Utility.SecurityHelper.HasPermission(flowMaster))
            //{
            //    return "没有此路线的权限";
            //}

            SearchCacheModel searchCacheModel = this.ProcessSearchModel(null, searchModel);


            IList<object> param = new List<object>();
            string hql = "  from MrpPlan as m where m.PlanDate>=? and m.PlanDate<? ";
            //param.Add(searchModel.Flow);
            param.Add(searchModel.StartDate);
            param.Add(searchModel.StartDate.Value.AddDays(14));

            if (!string.IsNullOrEmpty(searchModel.Flow))
            {
                hql += " and m.Flow=?";
                param.Add(searchModel.Flow);
            }

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                hql += " and m.Item=?";
                param.Add(searchModel.Item);
            }

            IList<MrpPlan> mrpPlanList = genericMgr.FindAll<MrpPlan>(hql, param.ToArray());
            string reqUrl = HttpContext.Request.Url.Authority + HttpContext.Request.ApplicationPath;

            return planMgr.GetStringMrpPlanView(mrpPlanList, searchModel.StartDate.Value, searchModel.PlanVersion, reqUrl);
        }
    }
}




