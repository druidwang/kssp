using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.MRP.VIEW;
using com.Sconit.Service;
using com.Sconit.Service.MRP;
using com.Sconit.Utility;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.MRP;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;
using com.Sconit.Entity.SCM;
using System.Data.SqlClient;
using com.Sconit.Entity.VIEW;

namespace com.Sconit.Web.Controllers.MRP
{
    public class MaterialPlanController : WebAppBaseController
    {
        public IPlanMgr planMgr { get; set; }
        public IRccpMgr rccpMgr { get; set; }

        private static string selectCountStatement = "select count(*) from PurchasePlan as r";
        private static string selectStatement = "select r from PurchasePlan as r";

        #region Plan
        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_TransferPlan")]
        public ActionResult TransferPlan()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_PurchasePlan")]
        public ActionResult PurchasePlan()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_PurchasePlanAdjust")]
        public ActionResult PurchasePlanAdjust()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_SupplierPlan")]
        public ActionResult SupplierPlan()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_TransferPlan")]
        public string _GetTransferPlan(MaterailPlanSearchModel searchModel)
        {
            var flowMaster = this.genericMgr.FindById<FlowMaster>(searchModel.Flow);
            var flowStrategy = this.genericMgr.FindById<FlowStrategy>(flowMaster.Code);
            var leadDay = Utility.DateTimeHelper.TimeTranfer(flowStrategy.LeadTime, flowStrategy.TimeUnit, CodeMaster.TimeUnit.Day);

            if (!Utility.SecurityHelper.HasPermission(flowMaster))
            {
                return Resources.EXT.ControllerLan.Con_LackTheFlowPermission_1;
            }

            SearchCacheModel searchCacheModel = this.ProcessSearchModel(null, searchModel);
            IList<object> param = new List<object>();
            string hql = " from TransferPlan as r where r.Flow=? and r.PlanVersion=? ";
            param.Add(searchModel.Flow);
            param.Add(searchModel.PlanVersion);

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                hql += " and r.Item=?";
                param.Add(searchModel.Item);
            }
            if (!string.IsNullOrEmpty(searchModel.MaterialsGroup))
            {
                hql += " and exists (select 1 from Item as i where r.Item = i.Code and i.MaterialsGroup =? )";
                param.Add(searchModel.MaterialsGroup);
            }

            IList<TransferPlan> transferPlanList = genericMgr.FindAll<TransferPlan>(hql, param.ToArray());
            var flowDic = this.flowMgr.GetFlowDetailList(searchModel.Flow, true)
                            .GroupBy(g => g.Item, (Item, GroupItem) => new { Item, GroupItem.First().Sequence })
                            .ToDictionary(d => d.Item, d => d.Sequence);
            var planList = from p in transferPlanList
                           select new Plan
                           {
                               Sequence = flowDic.ValueOrDefault(p.Item),
                               Flow = p.Flow,
                               Item = p.Item,
                               Qty = p.Qty,
                               PlanDate = searchModel.IsStartTime ? p.WindowTime.AddDays(-leadDay) : p.WindowTime,
                               WindowTime = p.WindowTime,
                               StartTime = p.WindowTime.AddDays(-leadDay)
                           };
            searchModel.DateType = 4;
            return GetPlanString(planList.ToList(), searchModel);
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_SupplierPlan,Url_Mrp_MaterialPlan_PurchasePlan")]
        public string GetPurchasePlan(MaterailPlanSearchModel searchModel)
        {
            var flowMaster = this.genericMgr.FindById<FlowMaster>(searchModel.Flow);
            var flowStrategy = this.genericMgr.FindById<FlowStrategy>(flowMaster.Code);
            var leadDay = Utility.DateTimeHelper.TimeTranfer(flowStrategy.RccpLeadTime, flowStrategy.TimeUnit, CodeMaster.TimeUnit.Day);

            if (searchModel.DateType == (int)CodeMaster.TimeUnit.Week)
            {
                leadDay = Math.Round(leadDay / 7) * 7;
            }
            else
            {
                leadDay = Math.Round(leadDay / 30) * 30;
            }


            if (!Utility.SecurityHelper.HasPermission(flowMaster))
            {
                return Resources.EXT.ControllerLan.Con_LackTheFlowPermission_1;
            }

            SearchCacheModel searchCacheModel = this.ProcessSearchModel(null, searchModel);
            IList<object> param = new List<object>();
            string hql = " from PurchasePlan as r where r.Flow=? and r.PlanVersion=? ";
            param.Add(searchModel.Flow);
            param.Add(searchModel.PlanVersion);

            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                hql += " and r.Item=?";
                param.Add(searchModel.Item);
            }

            if (!string.IsNullOrEmpty(searchModel.MaterialsGroup))
            {
                hql += " and exists (select 1 from Item as i where r.Item = i.Code and i.MaterialsGroup =? )";
                param.Add(searchModel.MaterialsGroup);
            }

            IList<PurchasePlan> purchasePlanList = genericMgr.FindAll<PurchasePlan>(hql, param.ToArray());
            var flowDic = this.flowMgr.GetFlowDetailList(searchModel.Flow, true)
                .GroupBy(g => g.Item, (Item, GroupItem) => new { Item, GroupItem.First().Sequence })
                .ToDictionary(d => d.Item, d => d.Sequence);

            var planList = from p in purchasePlanList
                           select new Plan
                           {
                               Sequence = flowDic.ValueOrDefault(p.Item),
                               Flow = p.Flow,
                               Item = p.Item,
                               Qty = p.Qty,
                               PlanDate = searchModel.IsStartTime ? p.WindowTime.AddDays(-leadDay) : p.WindowTime,
                               WindowTime = p.WindowTime,
                               StartTime = p.WindowTime.AddDays(-leadDay)
                           };

            return GetPlanString(planList.ToList(), searchModel);
        }

        [HttpPost]
        public JsonResult GetPlanTypes(string flow)
        {
            var flowStrategy = this.genericMgr.FindById<FlowStrategy>(flow);
            return Json(new
            {
                flowStrategy.IsCheckMrpDailyPlan,
                flowStrategy.IsCheckMrpWeeklyPlan,
                flowStrategy.IsCheckMrpMonthlyPlan
            });
        }

        private string GetPlanString(List<Plan> planList, MaterailPlanSearchModel searchModel)
        {
            bool isPlanRelease = genericMgr.FindAllIn<PurchasePlanMaster>(@" from PurchasePlanMaster as r where r.Flow=? and r.PlanVersion=? ",
                new object[] { searchModel.Flow, searchModel.PlanVersion }).FirstOrDefault().IsRelease;
            DateTime planVersion = searchModel.PlanVersion.Value;
            CodeMaster.TimeUnit dateType = (CodeMaster.TimeUnit)searchModel.DateType;

            if (planList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Item);
            str.Append("</th>");

            str.Append("<th style=\"min-width: 120px;\">");
            str.Append(Resources.EXT.ControllerLan.Con_ItemDescription);
            str.Append("</th>");

            str.Append("<th style=\"min-width: 30px;\">");
            str.Append(Resources.EXT.ControllerLan.Con_Uom);
            str.Append("</th>");
            if (!(searchModel.IsSupplier.HasValue && searchModel.IsSupplier.Value))
            {
                str.Append("<th style=\"min-width: 80px;\">");
                str.Append(Resources.EXT.ControllerLan.Con_Inventory);
                str.Append("</th>");

                str.Append("<th style=\"min-width: 80px;\">");
                str.Append(Resources.EXT.ControllerLan.Con_Inventory1);
                str.Append("</th>");

                str.Append("<th style=\"min-width: 80px;\">");
                str.Append(Resources.EXT.ControllerLan.Con_Inventory2);
                str.Append("</th>");

                str.Append("<th style=\"min-width: 80px;\">");
                str.Append(Resources.EXT.ControllerLan.Con_Others);
                str.Append("</th>");

                //str.Append("<th style=\"min-width: 80px;\">");
                //str.Append("正常库存");
                //str.Append("</th>");

                str.Append("<th style=\"min-width: 80px;\">");
                str.Append(Resources.EXT.ControllerLan.Con_InqualifiedInventory);
                str.Append("</th>");

                str.Append("<th style=\"min-width: 80px;\">");
                str.Append(Resources.EXT.ControllerLan.Con_FreezeInventory);
                str.Append("</th>");
            }
            var dateTimeIndexs = planList
                .GroupBy(p => p.PlanDate, (k, g) => new { PlanDate = k, WindowTime = g.First().WindowTime })
                .OrderBy(r => r.PlanDate).Take(16);

            foreach (var planDateIndex in dateTimeIndexs)
            {
                //todo 计划转订单 Url_Mrp_MaterialPlan_PlanToOrder
                str.Append("<th style=\"min-width: 50px;\">");

                string head_date = string.Empty;
                if (dateType == CodeMaster.TimeUnit.Day)
                {
                    head_date = planDateIndex.PlanDate.ToString("MM-dd");
                }
                else if (dateType == CodeMaster.TimeUnit.Week)
                {
                    head_date = DateTimeHelper.GetWeekOfYear(planDateIndex.PlanDate);
                }
                else if (dateType == CodeMaster.TimeUnit.Month)
                {
                    head_date = planDateIndex.PlanDate.ToString("yyyy-MM");
                }

                if (this.CurrentUser.UrlPermissions.Contains("Url_Mrp_MaterialPlan_PlanToOrder") && isPlanRelease && !searchModel.IsSupplier.HasValue)
                {
                    head_date = string.Format("<a href='http://{0}{1}ProcurementOrder/NewFromPlan?Flow={2}&WindowTime={3}&PlanVersion={4}&BackUrl={5}&MaterialsGroup={6}&Item={7}'>{8}</a>",
                               HttpContext.Request.Url.Authority, HttpContext.Request.ApplicationPath, planList.First().Flow,
                               planDateIndex.WindowTime.ToString("yyyy-MM-dd"), planVersion.ToString(), searchModel.BackUrl, searchModel.MaterialsGroup, searchModel.Item, head_date);
                }
                str.Append(head_date);
                str.Append("</th>");
            }
            str.Append("</tr></thead><tbody>");

            var planGroupByItem = from p in planList
                                  orderby p.Sequence, p.Item
                                  group p by
                                  new
                                  {
                                      Item = p.Item
                                  } into g
                                  select new
                                  {
                                      Item = g.Key.Item,
                                      Dic = (from q in g
                                             group q by q.PlanDate into g1
                                             select new
                                             {
                                                 PlanDate = g1.Key,
                                                 Qty = g1.Sum(r => r.Qty)
                                             }).ToDictionary(d => d.PlanDate, d => d.Qty)
                                  };
            //实时库存
            string sql = @"select l.* from VIEW_LocationDet as l inner join MD_Location as loc on l.Location = loc.Code where 1 = 1 and loc.isMrp=1 ";
            if (planGroupByItem.Count()>0)
            {
                string itemsInSql = " and l.Item in( ";
                foreach(var item in planGroupByItem)
                {
                    itemsInSql += "'" + item.Item + "',";
                }
                sql += itemsInSql.Substring(0, itemsInSql.Length - 1) + ")";
            }

            IList<LocationDetailView> locationDetailViewList = this.genericMgr.FindEntityWithNativeSql<LocationDetailView>(sql);

            var inventoryBalances = locationDetailViewList
            .GroupBy(p => new { p.Item })
            .ToDictionary(d => d.Key.Item, d => new { Qty = d.Sum(q => q.Qty), QualifyQty = d.Sum(q => q.QualifyQty), RejectQty = d.Sum(q => q.RejectQty), FreezeQty = d.Sum(q => q.FreezeQty)});

            var inventoryBalancesByLocation = locationDetailViewList
           .GroupBy(p => new { p.Location,p.Item })
           .ToDictionary(d => new{d.Key.Location,d.Key.Item}, d => new { Qty = d.Sum(q => q.Qty)});

            int l = 0;
            foreach (var planItem in planGroupByItem)
            {
                Item newItem = itemMgr.GetCacheItem(planItem.Item);
                var inventoryBalance = inventoryBalances.ValueOrDefault(planItem.Item);
                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }
                var inv9101 = inventoryBalancesByLocation.ValueOrDefault(new { Location = "9101", Item = planItem.Item });
                var inv9102 = inventoryBalancesByLocation.ValueOrDefault(new { Location = "9102", Item = planItem.Item });
                var inv9103 = inventoryBalancesByLocation.ValueOrDefault(new { Location = "9103", Item = planItem.Item });
                str.Append("<td >");
                str.Append(planItem.Item);
                str.Append("</td>");
                str.Append("<td >");
                str.Append(newItem.Description);
                str.Append("</td>");
                str.Append("<td>");
                str.Append(newItem.Uom);
                str.Append("</td>");
                if (!(searchModel.IsSupplier.HasValue && searchModel.IsSupplier.Value))
                {
                    str.Append("<td>");
                    str.Append(inv9101==null?"0":inv9101.Qty.ToString("0.##"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(inv9102 == null ? "0" : inv9102.Qty.ToString("0.##"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(inv9103 == null ? "0" : inv9103.Qty.ToString("0.##"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append((inventoryBalance == null ? 0 : (inventoryBalance.Qty - inventoryBalance.RejectQty - inventoryBalance.FreezeQty)) - (inv9101 == null ? 0 : inv9101.Qty) - (inv9102 == null ? 0 : inv9102.Qty) - (inv9103 == null ? 0 : inv9103.Qty));
                    str.Append("</td>");

                    //str.Append("<td>");
                    //str.Append((inventoryBalance == null ? 0 : (inventoryBalance.Qty - inventoryBalance.RejectQty - inventoryBalance.FreezeQty)).ToString("0.##"));
                    //str.Append("</td>");

                    str.Append("<td>");
                    str.Append((inventoryBalance == null ? 0 : inventoryBalance.RejectQty).ToString("0.##"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append((inventoryBalance == null ? 0 : inventoryBalance.FreezeQty).ToString("0.##"));
                    str.Append("</td>");
                }
                #region
                foreach (var dateTimeIndex in dateTimeIndexs)
                {
                    var qty = planItem.Dic.ValueOrDefault(dateTimeIndex.PlanDate);
                    qty = qty > 0 ? qty : 0;
                    str.Append("<td>");
                    if (newItem.Uom == "PC" || newItem.Uom == "EA")
                    {
                        str.Append(qty.ToString("0"));
                    }
                    else
                    {
                        str.Append(qty.ToString("0.##"));
                    }
                    str.Append("</td>");
                }
                #endregion

                str.Append("</tr>");
            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }

        #endregion

        #region
        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_PurchasePlanAdjust")]
        public ActionResult _GetPurchasePlanList(GridCommand command, MaterailPlanSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            ViewBag.Flow = searchModel.Flow;
            ViewBag.Item = searchModel.Item;
            ViewBag.PlanVersion = searchModel.PlanVersion;
            ViewBag.PlanDate = searchModel.PlanDate;
            ViewBag.PlanDateTo = searchModel.PlanDateTo;
            ViewBag.IsStartTime = searchModel.IsStartTime;
            ViewBag.DateType = searchModel.DateType;
            if (searchModel.PlanVersion == null)
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_VersionTimeCanNotBeEmpty);
            }

            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_PurchasePlanAdjust")]
        public JsonResult _SaveBatchEditing([Bind(Prefix = "updated")]IEnumerable<PurchasePlan> updatedPlans)
        {
            try
            {
                if (updatedPlans != null)
                {
                    //judge if the version was repleased

                    string isReleaseMsg = _GetIfRelease(updatedPlans.FirstOrDefault().PlanVersion, updatedPlans.FirstOrDefault().Flow);
                    if (isReleaseMsg == Resources.EXT.ControllerLan.Con_AlreadyReleased)
                    {
                        throw new BusinessException(Resources.EXT.ControllerLan.Con_TheFlowCorrespondingVersionAlreadyReleasedCanNotEdit);
                    }
                    foreach (var plan in updatedPlans)
                    {
                        PurchasePlan purchasePlan = genericMgr.FindById<PurchasePlan>(plan.Id);
                        purchasePlan.Qty = plan.CurrentQty;
                        this.genericMgr.Update(purchasePlan);
                    }
                }
                object obj = new { };
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedEditSuccessfully);
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_PurchasePlanAdjust")]
        public ActionResult _Update(GridCommand command, PurchasePlan purchasePlan,
                    DateTime? NplanVersion, string NplanDate, string Nflow, string Nitem, bool? NisStartTime, int NDateType)
        {
            MaterailPlanSearchModel searchModel = new MaterailPlanSearchModel();
            searchModel.Flow = Nflow;
            searchModel.IsStartTime = NisStartTime.HasValue ? NisStartTime.Value : false;
            searchModel.Item = Nitem;
            searchModel.PlanDate = NplanDate;
            searchModel.PlanVersion = NplanVersion;
            searchModel.DateType = NDateType;
            PurchasePlan newPurchasePlan = genericMgr.FindById<PurchasePlan>(purchasePlan.Id);
            newPurchasePlan.Qty = purchasePlan.CurrentQty;
            genericMgr.Update(newPurchasePlan);

            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<PurchasePlan> purchasePlanList = GetAjaxPageData<PurchasePlan>(searchStatementModel, command);
            foreach (var purchasePlanEntity in purchasePlanList.Data)
            {
                purchasePlanEntity.CurrentQty = purchasePlanEntity.Qty;
                purchasePlanEntity.ItemDescription = this.itemMgr.GetCacheItem(purchasePlanEntity.Item).Description;
            }
            return PartialView(purchasePlanList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_PurchasePlanAdjust")]
        public ActionResult _AjaxList(GridCommand command, MaterailPlanSearchModel searchModel)
        {
            if (searchModel.PlanVersion == null)
            {
                return PartialView(new GridModel(new List<PurchasePlan>()));
            }
            command.PageSize = 1000;
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<PurchasePlan> purchasePlanList = GetAjaxPageData<PurchasePlan>(searchStatementModel, command);
            var flowDic = this.flowMgr.GetFlowDetailList(searchModel.Flow, true)
                            .ToDictionary(d=>d.Item,d=>d);
            foreach (var purchasePlan in purchasePlanList.Data)
            {
                
                purchasePlan.CurrentQty  =Math.Round( purchasePlan.Qty,2);
                purchasePlan.ItemDescription = this.itemMgr.GetCacheItem(purchasePlan.Item).Description;
                if (purchasePlan.DateType == CodeMaster.TimeUnit.Week)
                {
                    purchasePlan.DateIndexValue = Utility.DateTimeHelper.GetWeekOfYear(purchasePlan.WindowTime);
                }
                else
                {
                    purchasePlan.DateIndexValue = purchasePlan.WindowTime.ToString("yyyy-MM");
                }
                purchasePlan.Uom = flowDic.ValueOrDefault(purchasePlan.Item).Uom;
                purchasePlan.UnitCount = flowDic.ValueOrDefault(purchasePlan.Item).UnitCount;
            }
            return PartialView(purchasePlanList);
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, MaterailPlanSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            DateTime fromTime = DateTime.Now;
            DateTime toTime = DateTime.Now;
            IList<object> param = new List<object>();
            if (searchModel.DateType == 5)
            {
                var dateFrom = Utility.DateTimeHelper.GetWeekIndexDateFrom(searchModel.PlanDate);
                var dateTo = Utility.DateTimeHelper.GetWeekIndexDateFrom(searchModel.PlanDateTo);
                dateTo = dateTo.AddDays(6);
                fromTime = DateTime.Parse(dateFrom.ToString());
                toTime = DateTime.Parse(dateTo.ToString());
            }
            else if (searchModel.DateType == 6)
            {
                fromTime = DateTime.Parse(searchModel.PlanDate);
                toTime = DateTime.Parse(searchModel.PlanDateTo).AddMonths(1).AddMinutes(-1);
            }
            //可以查到所有的fromTime以后的数据
            //toTime = DateTime.MaxValue;
            HqlStatementHelper.AddEqStatement("PlanVersion", searchModel.PlanVersion, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "r", ref whereStatement, param);
            if (!string.IsNullOrWhiteSpace(searchModel.Item))
            {
                HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "r", ref whereStatement, param);
            }
            if (searchModel.IsStartTime && !string.IsNullOrWhiteSpace(searchModel.PlanDate))
            {
                if (searchModel.DateType == 6 || searchModel.DateType == 5)
                {
                    HqlStatementHelper.AddBetweenStatement("StartTime", fromTime, toTime, "r", ref whereStatement, param);

                }
                else
                {
                    HqlStatementHelper.AddEqStatement("StartTime", DateTime.Parse(searchModel.PlanDate), "r", ref whereStatement, param);
                }
            }
            else
            {
                if (searchModel.DateType == 6 || searchModel.DateType == 5)
                {
                    HqlStatementHelper.AddBetweenStatement("WindowTime", fromTime, toTime, "r", ref whereStatement, param);

                }
                else
                {
                    HqlStatementHelper.AddEqStatement("WindowTime", DateTime.Parse(searchModel.PlanDate), "r", ref whereStatement, param);
                }
            }
            HqlStatementHelper.AddEqStatement("DateType", searchModel.DateType, "r", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("DateType", searchModel.DateType, "r", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                if (searchModel.IsStartTime)
                {
                    sortingStatement = " order by Item ,StartTime";
                }
                else
                {
                    sortingStatement = " order by Item ,WindowTime";
                }
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion
        #region Daily purchase plan
        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_SupplierPlan,Url_Mrp_MaterialPlan_PurchasePlan")]
        public string GetPurchasePlanDaily(MaterailPlanSearchModel searchModel)
        {
            string ProcedureName = "USP_Busi_MRP_PurchaseDailyPlan";
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@Flow", searchModel.Flow);
            sqlParams[1] = new SqlParameter("@Item", searchModel.Item);
            sqlParams[2] = new SqlParameter("@ItemGroup", searchModel.MaterialsGroup);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            return GetTableHtmlByStoredProcedure(ProcedureName, sqlParams);
        }
        #endregion
        #region Export purchase plan
        [SconitAuthorize(Permissions = "Url_Mrp_MaterialPlan_SupplierPlan,Url_Mrp_MaterialPlan_PurchasePlan")]
        public ActionResult Export(MaterailPlanSearchModel searchModel)
        {
            var table = searchModel.DateType == 4 ? GetPurchasePlanDaily(searchModel) : GetPurchasePlan(searchModel);
            return new DownloadFileActionResult(table, "PurchasePlanQuery.xls");
        }
        #endregion
        public string _GetIfRelease(DateTime planversion,string flow)
        {
            bool isPlanRelease = genericMgr.FindAllIn<PurchasePlanMaster>(@" from PurchasePlanMaster as r where r.Flow=? and r.PlanVersion=? ",
                new object[] { flow, planversion }).FirstOrDefault().IsRelease;
            string strReturn = isPlanRelease ? Resources.EXT.ControllerLan.Con_AlreadyReleased : Resources.EXT.ControllerLan.Con_NotReleased;
            return strReturn;
        }
        class Plan
        {
            public int Sequence { get; set; }
            public string Flow { get; set; }
            public string Item { get; set; }
            public double Qty { get; set; }
            public DateTime PlanDate { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime WindowTime { get; set; }
        }
    }
}
