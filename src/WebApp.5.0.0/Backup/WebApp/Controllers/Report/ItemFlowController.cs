using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Web.Models;
using com.Sconit.Service;
using Telerik.Web.Mvc.UI;
using com.Sconit.Web.Models.SearchModels.MD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.MD;

namespace com.Sconit.Web.Controllers.Report
{
    public class ItemFlowController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        //public IFlowMgr flowMgr { get; set; }
        //public IItemMgr itemMgr { get; set; }
        public IBomMgr bomMgr { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_ItemFlow_View")]
        public ActionResult _SearchResult(GridCommand command, FlowItemSearchModel searchModel)
        {
            ViewBag.Item = searchModel.Item;
            ViewBag.Flow = searchModel.Flow;
            ViewBag.SearchType = searchModel.SearchType;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ItemFlow_View")]
        public ActionResult _AjaxList(GridCommand command, FlowItemSearchModel searchModel)
        {
            var itemlist = itemMgr.GetCacheAllItem();
            if (searchModel.SearchType == "searchFlow")
            {
                GridModel<FlowDetail> flowList = new GridModel<FlowDetail>();
                var flowMasterList = this.genericMgr.FindAll<FlowMaster>
                    (" from FlowMaster where code like ? ", "%" + searchModel.Flow + "%");
                var flowDetails = new List<FlowDetail>();
                foreach (var flowMaster in flowMasterList)
                {
                    flowDetails.AddRange(flowMgr.GetFlowDetailList(flowMaster.Code));
                }
                foreach (var flowDetail in flowDetails)
                {
                    var item = itemMgr.GetCacheItem(flowDetail.Item);
                    flowDetail.WarnLeadTime = item.WarnLeadTime;
                    flowDetail.Warranty = item.Warranty;
                    flowDetail.ItemDescription = item.FullDescription;
                }
                FillFlowDetial(flowDetails);
                foreach (var flowDetail in flowDetails)
                {
                    flowDetail.ItemDescription = itemlist.ValueOrDefault(flowDetail.Item).FullDescription;
                }
                flowList.Total = flowDetails.Count();
                flowList.Data = flowDetails;
           
                return PartialView(flowList);
            }
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<FlowDetail> List = GetAjaxPageData<FlowDetail>(searchStatementModel, command);

            FillFlowDetial(List.Data);
            foreach (var list in List.Data)
            {
                list.ItemDescription = itemlist.ValueOrDefault(list.Item).FullDescription;
            }
            return PartialView(List);
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, FlowItemSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "d", ref  whereStatement, param);


            string sortingStatement = string.Empty;
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by CreateDate desc";
            }
            else
            {
                sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = "select count(*) from FlowDetail as d";
            searchStatementModel.SelectStatement = "select d from FlowDetail as d";
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [SconitAuthorize(Permissions = "Url_ItemFlow_View")]
        public ActionResult _AjaxLoadingTree(TreeViewItem node)
        {
            try
            {
                string parentId = !String.IsNullOrEmpty(node.Value) ? (node.Value) : null;
                IList<TreeViewItemModel> nodes = new List<TreeViewItemModel>();

                if (parentId != null)
                {
                    string[] s = parentId.Split(';');
                    if (s != null && s.Length == 4)
                    {
                        FlowDetail currentFlowDetail = new FlowDetail();
                        currentFlowDetail.Item = s[0];
                        string locationFrom = s[1];
                        currentFlowDetail.Flow = s[2];
                        currentFlowDetail.Bom = s[3];
                        var locationFroms = locationFrom.Split(',');
                        currentFlowDetail.DefaultLocationFrom = locationFroms[0];
                        if (locationFroms.Length == 2)
                        {
                            currentFlowDetail.DefaultExtraLocationFrom = locationFroms[1];
                        }
                        currentFlowDetail.CurrentFlowMaster = this.genericMgr.FindById<FlowMaster>(currentFlowDetail.Flow);

                        var newFlowDetails = new List<FlowDetail>();
                        if (currentFlowDetail.CurrentFlowMaster.Type == CodeMaster.OrderType.Production
                            || currentFlowDetail.CurrentFlowMaster.Type == CodeMaster.OrderType.SubContract)
                        {
                            string bom = string.IsNullOrWhiteSpace(currentFlowDetail.Bom) ? currentFlowDetail.Item : currentFlowDetail.Bom;

                            var bomDetails = bomMgr.GetFlatBomDetail(bom, DateTime.Now);
                            newFlowDetails.AddRange(bomDetails.Select(p => new FlowDetail
                            {
                                Item = p.Item,
                                DefaultLocationFrom = string.IsNullOrWhiteSpace(p.Location) ? currentFlowDetail.DefaultLocationFrom : p.Location,
                                DefaultExtraLocationFrom = currentFlowDetail.DefaultExtraLocationFrom
                            }));
                        }
                        else
                        {
                            newFlowDetails.Add(new FlowDetail
                            {
                                Item = currentFlowDetail.Item,
                                DefaultLocationFrom = currentFlowDetail.DefaultLocationFrom,
                                DefaultExtraLocationFrom = currentFlowDetail.DefaultExtraLocationFrom
                            });
                            if (!string.IsNullOrWhiteSpace(currentFlowDetail.Bom)
                                && currentFlowDetail.CurrentFlowMaster.Type == CodeMaster.OrderType.Procurement)
                            {
                                var bomDetails = bomMgr.GetFlatBomDetail(currentFlowDetail.Bom, DateTime.Now);
                                newFlowDetails.AddRange(bomDetails.Select(p => new FlowDetail
                                {
                                    Item = p.Item,
                                    DefaultLocationFrom = string.IsNullOrWhiteSpace(p.Location) ? currentFlowDetail.DefaultLocationFrom : p.Location,
                                    DefaultExtraLocationFrom = currentFlowDetail.DefaultExtraLocationFrom
                                }));
                            }
                        }

                        var flowDetailList = this.genericMgr.FindAllIn<FlowDetail>
                            (" from FlowDetail where Item in(?", newFlowDetails.Select(p => p.Item).Distinct());
                        foreach (var flowDetail in flowDetailList)
                        {
                            flowDetail.CurrentFlowMaster = genericMgr.FindById<FlowMaster>(flowDetail.Flow);
                            flowDetail.DefaultLocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom)
                                ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                            flowDetail.DefaultLocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo)
                                ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                            flowDetail.DefaultExtraLocationFrom = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationFrom)
                                ? flowDetail.CurrentFlowMaster.ExtraLocationFrom : flowDetail.ExtraLocationFrom;
                            flowDetail.DefaultExtraLocationTo = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationTo)
                                ? flowDetail.CurrentFlowMaster.ExtraLocationTo : flowDetail.ExtraLocationTo;
                        }

                        foreach (var newFlowDetail in newFlowDetails)
                        {
                            flowDetailList = flowDetailList.Where(p => p.Item == newFlowDetail.Item).ToList();
                            var nextFlowDetails = flowDetailList.Where(p => newFlowDetail.DefaultLocationFrom == p.DefaultLocationTo);

                            #region 如果没有找到，考虑其他来源库位
                            if (nextFlowDetails.Count() == 0 && !string.IsNullOrWhiteSpace(newFlowDetail.DefaultExtraLocationFrom))
                            {
                                var locations = newFlowDetail.DefaultExtraLocationFrom.Split('|').Distinct();
                                foreach (var location in locations)
                                {
                                    nextFlowDetails = flowDetailList.Where(f => f.DefaultLocationTo == location);
                                    if (nextFlowDetails.Count() > 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region 如果没有找到，考虑其他目的库位
                            if (nextFlowDetails.Count() == 0)
                            {
                                var locations = flowDetailList.Where(p => !string.IsNullOrWhiteSpace(p.DefaultExtraLocationTo))
                                    .SelectMany(p => p.DefaultExtraLocationTo.Split('|')).Distinct();
                                foreach (var location in locations)
                                {
                                    nextFlowDetails = flowDetailList.Where(f => location == newFlowDetail.DefaultLocationFrom);
                                    if (nextFlowDetails.Count() > 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            #endregion

                            FillFlowDetial(nextFlowDetails);
                            foreach (var nextFlowDetail in nextFlowDetails)
                            {
                                Item item = itemMgr.GetCacheItem(nextFlowDetail.Item);
                                TreeViewItemModel tvim = new TreeViewItemModel();
                                tvim.Text = string.Format(Resources.EXT.ControllerLan.Con_LineMaterialLocationFromLocationToMRPweights,
                                    nextFlowDetail.Flow, nextFlowDetail.CurrentFlowMaster.Description, nextFlowDetail.Item,
                                    item.Description, nextFlowDetail.LocationFrom, nextFlowDetail.LocationTo,
                                    nextFlowDetail.MrpWeight, nextFlowDetail.CurrentFlowMaster.FlowTypeDescription);
                                tvim.Value = string.Format("{0};{1};{2};{3}",
                                     nextFlowDetail.Item, nextFlowDetail.LocationFrom, nextFlowDetail.Flow, nextFlowDetail.Bom);
                                tvim.LoadOnDemand = true;
                                nodes.Add(tvim);
                            }
                        }
                    }
                }
                return new JsonResult { Data = nodes };
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return new JsonResult { };
            }
        }

        [SconitAuthorize(Permissions = "Url_ItemFlow_View")]
        public ActionResult _ScmModelResult(FlowItemSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(new GridCommand(), searchModel);
            GridModel<FlowDetail> List = GetAjaxPageData<FlowDetail>(searchStatementModel, null);
            FillFlowDetial(List.Data);
            return View(List.Data);
        }

        private void FillFlowDetial(IEnumerable<FlowDetail> flowDetailList)
        {
            foreach (FlowDetail flowDetail in flowDetailList)
            {
                if (flowDetail.CurrentFlowMaster == null)
                {
                    flowDetail.CurrentFlowMaster = genericMgr.FindById<FlowMaster>(flowDetail.Flow);
                }
                FillCodeDetailDescription<FlowMaster>(flowDetail.CurrentFlowMaster);
                flowDetail.PartyFrom = flowDetail.CurrentFlowMaster.PartyFrom;
                flowDetail.PartyTo = flowDetail.CurrentFlowMaster.PartyTo;
                flowDetail.FlowStrategy = flowDetail.CurrentFlowMaster.FlowStrategy.ToString();

                flowDetail.ItemDescription = itemMgr.GetCacheItem(flowDetail.Item).Description;
                if (!flowDetail.CurrentFlowMaster.IsMRP)
                {
                    flowDetail.MrpWeight = 0;
                }
                if (string.IsNullOrEmpty(flowDetail.LocationFrom))
                {
                    flowDetail.LocationFrom = flowDetail.CurrentFlowMaster.LocationFrom;
                }
                if (string.IsNullOrEmpty(flowDetail.LocationTo))
                {
                    flowDetail.LocationTo = flowDetail.CurrentFlowMaster.LocationTo;
                }

                if (!string.IsNullOrEmpty(flowDetail.LocationFrom))
                {
                    var extraLocationFrom = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationFrom) ?
                        flowDetail.CurrentFlowMaster.ExtraLocationFrom : flowDetail.ExtraLocationFrom;
                    if (!string.IsNullOrWhiteSpace(extraLocationFrom))
                    {
                        flowDetail.LocationFrom += ("," + extraLocationFrom);
                    }
                }
                if (!string.IsNullOrEmpty(flowDetail.LocationTo))
                {
                    var extraLocationTo = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationTo) ?
                        flowDetail.CurrentFlowMaster.ExtraLocationTo : flowDetail.ExtraLocationTo;
                    if (!string.IsNullOrWhiteSpace(extraLocationTo))
                    {
                        flowDetail.LocationTo += ("," + extraLocationTo);
                    }
                }
            }
        }

    }
}
