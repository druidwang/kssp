namespace com.Sconit.Web.Controllers.MRP
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.MRP.TRANS;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Service;
    using com.Sconit.Service.MRP;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.MRP;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;

    public class MrpInvInController : WebAppBaseController
    {
        public IPlanMgr planMgr { get; set; }

        #region view
        [SconitAuthorize(Permissions = "Url_Mrp_MrpInvIn_Index")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpInvIn_Index")]
        public string _GetList(DateTime? planVersion, CodeMaster.ResourceGroup? resourceGroup, string flow, bool isShowDetail)
        {
            if (!resourceGroup.HasValue || !planVersion.HasValue)
            {
                return Resources.EXT.ControllerLan.Con_PlanVersionAndResourceGroupCanNotBeEmpty;
            }
            return planMgr.GetMrpInvIn(planVersion.Value, resourceGroup.Value, flow, isShowDetail);
        }
        #endregion
        #region Export InvInPlan
        [SconitAuthorize(Permissions = "Url_Mrp_MrpInvIn_Index")]
        public ActionResult Export(DateTime? planVersion, CodeMaster.ResourceGroup? resourceGroup, string flow )
        {
            var table = _GetList(planVersion, resourceGroup, flow, true);
            return new DownloadFileActionResult(table, "InvInPlan.xls");
        }
        #endregion
        #region 废弃
        private static string selectStatement = "select m from MrpShipPlanGroup as m";

        private static string selectCountStatement = "select count(*) from MrpShipPlanGroup as m ";

        [SconitAuthorize(Permissions = "Url_Mrp_MrpInvIn_Index")]
        [GridAction]
        public ActionResult List(GridCommand command, ShipPlanGroupSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            ViewBag.PlanVersion = searchModel.PlanVersion;
            ViewBag.DateIndex = searchModel.DateIndex;
            ViewBag.ProductLine = searchModel.ProductLine;
            ViewBag.Item = searchModel.Item;
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpInvIn_Index")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, ShipPlanGroupSearchModel searchModel)
        {
            ViewBag.PlanVersion = searchModel.PlanVersion;
            ViewBag.DateIndex = searchModel.DateIndex;
            ViewBag.ProductLine = searchModel.ProductLine;
            ViewBag.Item = searchModel.Item;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<MrpShipPlanGroup> MrpShipPlanGroupList = GetAjaxPageData<MrpShipPlanGroup>(searchStatementModel, command);
            foreach (var mrpShipPlanGroup in MrpShipPlanGroupList.Data)
            {
                mrpShipPlanGroup.ItemDescription = genericMgr.FindById<Item>(mrpShipPlanGroup.Item).Description;
            }
            return PartialView(MrpShipPlanGroupList);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpInvIn_Index")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                MrpShipPlanGroup mrpShipPlanGroup = this.genericMgr.FindById<MrpShipPlanGroup>(int.Parse(id));
                mrpShipPlanGroup.Item = genericMgr.FindById<Item>(mrpShipPlanGroup.Item).Description;
                return View(mrpShipPlanGroup);
            }
        }

        [SconitAuthorize(Permissions = "Url_Mrp_MrpInvIn_Index")]
        public ActionResult _MrpShipPlanGroupList(string id)
        {
            string hql = selectStatement + " where m.Id = '" + id + "'";
            IList<MrpShipPlanGroup> mrpShipPlanGroupList = genericMgr.FindAll<MrpShipPlanGroup>(hql);
            mrpShipPlanGroupList[0].ItemDescription = genericMgr.FindById<Item>(mrpShipPlanGroupList[0].Item).Description;
            return PartialView(new GridModel<MrpShipPlanGroup>(mrpShipPlanGroupList));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpInvIn_Index")]
        public ActionResult _AjaxMrpShipPlan(string id)
        {
            var shipPlanList = new List<MrpShipPlan>();
            string hql = " from MrpShipPlan as m where m.GroupId = '" + id + "'";
            IList<MrpShipPlan> mrpShipPlanList = genericMgr.FindAll<MrpShipPlan>(hql);
            foreach (var mrpShipPlan in mrpShipPlanList)
            {
                mrpShipPlan.ItemDescription = itemMgr.GetCacheItem(mrpShipPlan.Item).Description;
                if (mrpShipPlan.SourceType == Sconit.CodeMaster.MrpSourceType.StockLack
                    || mrpShipPlan.SourceType == Sconit.CodeMaster.MrpSourceType.StockOver)
                {
                    //var loclotdet = this.genericMgr.FindById<LocationLotDetail>(mrpShipPlan.SourceId);
                    //mrpShipPlan.Location = loclotdet.Location;
                    shipPlanList.Add(mrpShipPlan);
                }
                else
                {
                    IterateMrpShipPlan(shipPlanList, mrpShipPlan, 0);
                }
            }
            return PartialView(new GridModel<MrpShipPlan>(shipPlanList));
        }

        private void IterateMrpShipPlan(List<MrpShipPlan> shipPlanList, MrpShipPlan mrpShipPlan, int i)
        {
            if (mrpShipPlan.OrderType == Sconit.CodeMaster.OrderType.Distribution)
            {
                if (mrpShipPlan.SourceType == Sconit.CodeMaster.MrpSourceType.Order)
                {
                    mrpShipPlan.OrderNo = this.genericMgr.FindById<OrderDetail>(mrpShipPlan.SourceId).OrderNo;
                }
                shipPlanList.Add(mrpShipPlan);
            }
            else
            {
                var newShipPlans = this.genericMgr.FindAll<MrpShipPlan>
                    (@"from MrpShipPlan as m where Id=? ", new object[] { mrpShipPlan.SourceId });
                if (newShipPlans == null || newShipPlans.Count() == 0)
                {
                    if (mrpShipPlan.SourceType == Sconit.CodeMaster.MrpSourceType.Order)
                    {
                        mrpShipPlan.OrderNo = this.genericMgr.FindById<OrderDetail>(mrpShipPlan.SourceId).OrderNo;
                    }
                    mrpShipPlan.ItemDescription = itemMgr.GetCacheItem(mrpShipPlan.Item).Description;
                    shipPlanList.Add(mrpShipPlan);
                }
                else
                {
                    foreach (var newShipPlan in newShipPlans)
                    {
                        newShipPlan.ItemDescription = itemMgr.GetCacheItem(newShipPlan.Item).Description;
                        if (newShipPlan.OrderType == Sconit.CodeMaster.OrderType.Distribution
                            || newShipPlan.OrderType == Sconit.CodeMaster.OrderType.Production
                            || newShipPlan.OrderType == Sconit.CodeMaster.OrderType.SubContract)
                        {
                            if (newShipPlan.SourceType == Sconit.CodeMaster.MrpSourceType.Order)
                            {
                                newShipPlan.OrderNo = this.genericMgr.FindById<OrderDetail>(newShipPlan.SourceId).OrderNo;
                            }
                            shipPlanList.Add(newShipPlan);
                        }
                        else
                        {
                            i++;
                            if (i < 10)
                            {
                                IterateMrpShipPlan(shipPlanList, newShipPlan, i);
                            }
                        }
                    }
                }
            }
        }
       

        private SearchStatementModel PrepareSearchStatement(GridCommand command, ShipPlanGroupSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("PlanVersion", searchModel.PlanVersion, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.ProductLine, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("WindowTime", searchModel.DateIndex, "m", ref whereStatement, param);

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