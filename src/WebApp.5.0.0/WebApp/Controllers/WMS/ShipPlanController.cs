
namespace com.Sconit.Web.Controllers.WMS
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using Telerik.Web.Mvc;
    using System.Web.Routing;
    using com.Sconit.Web.Models;
    using com.Sconit.Utility;
    using com.Sconit.Web.Models.SearchModels.WMS;
    using com.Sconit.Entity.WMS;
    using com.Sconit.Entity.Exception;

    public class ShipPlanController : WebAppBaseController
    {
        #region 发货任务
      
        private static string selectCountStatement = "select count(*) from ShipPlan as p";

        private static string selectStatement = "select p from ShipPlan as p";

        public IShipPlanMgr shipPlanMgr { get; set; }

        #region 查询
        [SconitAuthorize(Permissions = "Url_ShipPlan_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ShipPlan_View")]
        public ActionResult List(GridCommand command, ShipPlanSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ShipPlan_View")]
        public ActionResult _AjaxList(GridCommand command, ShipPlanSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ShipPlan>(searchStatementModel, command));
        }

        #endregion

        #region 发货
        [SconitAuthorize(Permissions = "Url_ShipPlan_Ship")]
        public ActionResult ShipIndex()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_ShipPlan_Ship")]
        public ActionResult ShipList(GridCommand command, ShipPlanSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ShipPlan_Ship")]
        public ActionResult _AjaxShipList(GridCommand command, ShipPlanSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareShipSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ShipPlan>(searchStatementModel, command));
        }

        #endregion

        #region 分派
        [SconitAuthorize(Permissions = "Url_ShipPlan_Assign")]
        public ActionResult Assign()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ShipPlan_Assign")]
        public ActionResult _ShipPlanList(string pickGroupCode)
        {

            ViewBag.pickGroupCode = pickGroupCode;

            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_ShipPlan_Assign")]
        public ActionResult _SelectAssignBatchEditing(string pickGroupCode)
        {
            IList<ShipPlan> pickTaskList = new List<ShipPlan>();

            if (!string.IsNullOrEmpty(pickGroupCode))
            {
                string repackGroupSql = "select r from PickRule as r where r.PickGroupCode = ?";
                IList<PickRule> pickRuleList = genericMgr.FindAll<PickRule>(repackGroupSql, pickGroupCode);

                if (pickRuleList != null && pickRuleList.Count > 0)
                {
                    string pickRuleSql = string.Empty;
                    IList<object> param = new List<object>();

                    foreach (PickRule r in pickRuleList)
                    {
                        if (string.IsNullOrEmpty(pickRuleSql))
                        {
                            pickRuleSql += "select p from ShipPlan as p where p.ShipUserId is null and p.LocationFrom in (?";
                            param.Add(r.Location);
                        }
                        else
                        {
                            pickRuleSql += ",?";
                            param.Add(r.Location);
                        }
                    }

                    if (!string.IsNullOrEmpty(pickRuleSql))
                    {
                        pickRuleSql += ")";
                    }

                    pickTaskList = genericMgr.FindAll<ShipPlan>(pickRuleSql, param.ToList());
                }




            }
            return View(new GridModel(pickTaskList));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_ShipPlan_Assign")]
        public ActionResult AssignShipPlan(String checkedShipPlans, String assignUser)
        {
            try
            {

                IList<ShipPlan> shipPlanList = new List<ShipPlan>();
                if (!string.IsNullOrEmpty(checkedShipPlans))
                {

                    string[] idArray = checkedShipPlans.Split(',');


                    for (int i = 0; i < idArray.Count(); i++)
                    {

                        ShipPlan rt = genericMgr.FindById<ShipPlan>(Convert.ToInt32(idArray[i]));

                        shipPlanList.Add(rt);

                    }
                }
                shipPlanMgr.AssignShipPlan(shipPlanList, assignUser);
                SaveSuccessMessage(Resources.WMS.ShipPlan.ShipPlan_Assigned);
                object obj = new { SuccessMessage = string.Format(Resources.WMS.ShipPlan.ShipPlan_Assigned, checkedShipPlans), SuccessData = checkedShipPlans };
                return Json(obj);

            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }
        #endregion


        private SearchStatementModel PrepareSearchStatement(GridCommand command, ShipPlanSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("OrderNo", searchModel.OrderNo, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereStatement, param);

            if (searchModel.DateFrom != null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "p", ref whereStatement, param);
            }
            else if (searchModel.DateTo != null )
            {
                HqlStatementHelper.AddLtStatement("CreateDate", searchModel.DateTo.Value.AddDays(1), "p", ref whereStatement, param);
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

        private SearchStatementModel PrepareShipSearchStatement(GridCommand command, ShipPlanSearchModel searchModel)
        {
            string whereStatement = " where p.PickedQty > p.ShipQty "; 
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("OrderNo", searchModel.OrderNo, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "p", ref whereStatement, param);
           

            if (searchModel.DateFrom != null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "p", ref whereStatement, param);
            }
            else if (searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLtStatement("CreateDate", searchModel.DateTo.Value.AddDays(1), "p", ref whereStatement, param);
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
        #endregion

      
    }
}
