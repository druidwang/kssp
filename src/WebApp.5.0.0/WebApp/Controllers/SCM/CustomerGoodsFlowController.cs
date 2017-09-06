using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using com.Sconit.Entity.SCM;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.SCM;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.SYS;
using System.Web;
using System;
using System.Text;

namespace com.Sconit.Web.Controllers.SCM
{
    public class CustomerGoodsFlowController : WebAppBaseController
    {
        //
        // GET:

        //public IGenericMgr genericMgr { get; set; }
        //public IFlowMgr flowMgr { get; set; }

        private static string selectCountStatement = "select count(*) from FlowMaster as f ";
        private static string selectStatement = "select f from FlowMaster as f";

        private static string selectCountDetailStatement = "select count(*) from FlowDetail as f ";
        private static string selectDetailStatement = "select f from FlowDetail as f";

        private static string selectCountBindStatement = @"select count(*) 
                                                      from FlowBinding as f join f.MasterFlow as mf join f.BindedFlow as bf";
        private static string selectBindStatement = @"select f
                                                      from FlowBinding as f join f.MasterFlow as mf join f.BindedFlow as bf";
        //private static string userNameDuiplicateVerifyStatement = @"select count(*) from FlowMaster as u where u.Code = ?";

        public CustomerGoodsFlowController()
        {
        }

        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult Index()
        {
            return View();
        }

        #region FlowMaster

        [GridAction]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult List(GridCommand command, FlowSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _AjaxList(GridCommand command, FlowSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<FlowMaster>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult New(FlowMaster flow)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>("select count(*) from FlowMaster as f where f.Code = ?", flow.Code)[0] > 0)
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, flow.Code);
                }
                else if (string.IsNullOrEmpty(flow.PartyFrom))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_PartyFrom);
                }
                else if (string.IsNullOrEmpty(flow.PartyTo))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_PartyTo);
                }
                else if (string.IsNullOrEmpty(flow.LocationTo))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_LocationTo);
                }
                else if (string.IsNullOrEmpty(flow.ShipFrom))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_ShipFrom);
                }
                else if (string.IsNullOrEmpty(flow.ShipTo))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_ShipTo);
                }
                else
                {
                    flow.FlowStrategy = com.Sconit.CodeMaster.FlowStrategy.Manual;
                    flow.Type = com.Sconit.CodeMaster.OrderType.CustomerGoods;
                    flowMgr.CreateFlow(flow);

                    SaveSuccessMessage(Resources.SCM.FlowMaster.FlowMaster_Added);
                    return RedirectToAction("Edit/" + flow.Code);
                }
            }
            return View(flow);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                return View("Edit", string.Empty, id);
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            if (TempData["SaveErrorMessage"] != null)
            {
                string errorMessage = (TempData["SaveErrorMessage"]).ToString();
                SaveErrorMessage(errorMessage);
            }
            FlowMaster flow = this.genericMgr.FindById<FlowMaster>(id);
            return PartialView(flow);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult _Edit(FlowMaster flow)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(flow.PartyFrom))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_PartyFrom);
                }
                else if (string.IsNullOrEmpty(flow.PartyTo))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_PartyTo);
                }
                else if (string.IsNullOrEmpty(flow.LocationTo))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_LocationTo);
                }
                else if (string.IsNullOrEmpty(flow.ShipFrom))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_ShipFrom);
                }
                else if (string.IsNullOrEmpty(flow.ShipTo))
                {
                    base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Common_FieldRequired, Resources.SCM.FlowMaster.FlowMaster_ShipTo);
                }
                else
                {
                    flow.Type = com.Sconit.CodeMaster.OrderType.CustomerGoods;
                    genericMgr.UpdateWithTrim(flow);
                    SaveSuccessMessage(Resources.SCM.FlowMaster.FlowMaster_Updated);
                }
            }

            return PartialView(flow);
        }
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Delete")]
        public ActionResult FlowDel(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    flowMgr.DeleteFlow(id);
                    SaveSuccessMessage(Resources.SCM.FlowMaster.FlowMaster_Deleted);
                    return RedirectToAction("List");
                }
                catch (System.Exception ex)
                {
                    if (ex is System.NullReferenceException)
                    {
                        //SaveErrorMessage("路线已经被引用，不能删除。");
                        TempData["SaveErrorMessage"] = Resources.EXT.ControllerLan.Con_RouteHasBeenCitedCanNotBeDeleted;
                    }
                    else
                    {
                        TempData["SaveErrorMessage"] = ex.Message;
                    }
                    TempData["TabIndex"] = 0;
                    return View("Edit", string.Empty, id);
                }
            }
        }

        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Delete")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                genericMgr.DeleteById<FlowMaster>(id);
                SaveSuccessMessage(Resources.ACC.User.User_Deleted);
                return RedirectToAction("List");
            }
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, FlowSearchModel searchModel)
        {
            string whereStatement = " where f.Type=6 ";

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Anywhere, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LocationTo", searchModel.LocationTo,"f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "f", ref whereStatement, param);

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

        #region Strategy
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _Strategy(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            FlowStrategy flowStrategy = this.flowMgr.GetFlowStrategy(id);
            if (flowStrategy == null)
            {
                flowStrategy = new FlowStrategy();
                flowStrategy.Flow = id;
            }
            ViewBag.NextWindowTime = flowStrategy.NextWindowTime;
            ViewBag.NextOrderTime = flowStrategy.NextOrderTime;
            ViewBag.WindowTimeType = flowStrategy.WindowTimeType;
            ViewBag.MrpLeadTime = flowStrategy.RccpLeadTime;
            return PartialView(flowStrategy);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult _Strategy(FlowStrategy flowStrategy)
        {
            ViewBag.WindowTimeType = flowStrategy.WindowTimeType;
            if (ModelState.IsValid)
            {

                flowMgr.UpdateFlowStrategy(flowStrategy);
                SaveSuccessMessage(Resources.SCM.FlowStrategy.FlowStrategy_Updated);
            }
            return PartialView(flowStrategy);
        }
        #endregion

        #region FlowDetail
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _DetailSearch(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            ViewBag.flow = id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _Detail(GridCommand command, FlowDetailSearchModel searchModel, string flowCode)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _AjaxDetailList(GridCommand command, FlowDetailSearchModel searchModel, string flowCode)
        {
            SearchStatementModel searchStatementModel = PrepareDetailSearchStatement(command, searchModel, flowCode);
            GridModel<FlowDetail> gridList = GetAjaxPageData<FlowDetail>(searchStatementModel, command);
            if (gridList.Data != null && gridList.Data.Count() > 0)
            {
                foreach (FlowDetail flowDetail in gridList.Data)
                {
                    flowDetail.ItemDescription = this.genericMgr.FindById<Item>(flowDetail.Item).Description;
                }
            }
            return PartialView(gridList);
        }

        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult _DetailNew(string id)
        {
            FlowDetail flowDetail = new FlowDetail();
            FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(id);
            flowDetail.LocationTo = flowMaster.LocationTo;
            flowDetail.Flow = id;
            return PartialView(flowDetail);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult _DetailNew(FlowDetail flowDetail, string id)
        {
            if (ModelState.IsValid)
            {
                if (false)//暂不做控制
                {
                    // base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, flowDetail.Code);
                }
                else
                {
                    IList<FlowDetail> flowDetailList = this.genericMgr.FindAll<FlowDetail>("select fd from FlowDetail as fd where fd.Flow='" + flowDetail.Flow + "' and fd.Item='" + flowDetail.Item + "'");
                    if (flowDetailList.Count > 0)
                    {
                        SaveErrorMessage(Resources.EXT.ControllerLan.Con_ItemAlreadyExistsPleaseReselect);
                        return PartialView(flowDetail);
                    }
                    flowDetail.BaseUom = this.genericMgr.FindById<Item>(flowDetail.Item).Uom;
                    flowDetail.RoundUpOption = Sconit.CodeMaster.RoundUpOption.ToUp;
                    FlowDetail flowDetailItem = this.genericMgr.FindAllIn<FlowDetail>
                        ("from FlowDetail where Flow = ? ", new object[] { flowDetail.Flow }).OrderByDescending(p => p.Sequence).FirstOrDefault();
                    flowDetail.Sequence = 10;
                    if (flowDetailItem != null)
                    {
                        flowDetail.Sequence = flowDetail.Sequence + flowDetailItem.Sequence;
                    }
                    flowMgr.CreateFlowDetail(flowDetail);
                    SaveSuccessMessage(Resources.SCM.FlowDetail.FlowDetail_Added);
                    return RedirectToAction("_Detail/" + flowDetail.Flow);
                }
            }
            return PartialView(flowDetail);
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Delete")]
        public ActionResult btnDel(int? id, string Flow)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                // FlowDetail FlowDetail=genericMgr.FindById<FlowDetail>(id);
                genericMgr.DeleteById<FlowDetail>(id);
                SaveSuccessMessage(Resources.SCM.FlowDetail.FlowDetail_Deleted);
                return RedirectToAction("_Detail/" + Flow);
            }
        }

        public ActionResult GetRefItemCode(string item)
        {
            Item itemEntity = this.genericMgr.FindById<Item>(item);
            if (itemEntity == null)
            {
                itemEntity = new Item(); ;
            }
            return Json(itemEntity);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _DetailEdit(int? id)
        {
            if (!id.HasValue || id.Value == 0)
            {
                return HttpNotFound();
            }
            FlowDetail flowDetail = this.genericMgr.FindById<FlowDetail>(id);
            return PartialView(flowDetail);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult _DetailEdit(FlowDetail flowDetail, int? id)
        {
            if (ModelState.IsValid)
            {
                flowDetail.BaseUom = this.genericMgr.FindById<Item>(flowDetail.Item).Uom;
                flowMgr.UpdateFlowDetail(flowDetail);
                SaveSuccessMessage(Resources.SCM.FlowDetail.FlowDetail_Updated);
            }

            TempData["TabIndex"] = 2;
            return PartialView(flowDetail);
        }

        private SearchStatementModel PrepareDetailSearchStatement(GridCommand command, FlowDetailSearchModel searchModel, string flowCode)
        {
            string whereStatement = " where f.Flow='" + flowCode + "'";
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("Item", searchModel.Item, HqlStatementHelper.LikeMatchMode.Start, "f", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "f", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by f.Sequence asc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountDetailStatement;
            searchStatementModel.SelectStatement = selectDetailStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        #endregion

        #region Bind
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _Binding(GridCommand command, FlowBindModel searchModel, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            ViewBag.flow = id;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = PrepareBindSearchStatement(command, (FlowBindModel)searchCacheModel.SearchObject, id);
            return PartialView(GetPageData<FlowBinding>(searchStatementModel, command));
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _Binded(GridCommand command, FlowBindModel searchModel, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            ViewBag.flow = id;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = PrepareBindedSearchStatement(command, (FlowBindModel)searchCacheModel.SearchObject, id);
            return PartialView(GetPageData<FlowBinding>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _AjaxBinding(GridCommand command, FlowBindModel searchModel, string id)
        {
            SearchStatementModel searchStatementModel = PrepareBindSearchStatement(command, searchModel, id);
            return PartialView(GetAjaxPageData<FlowBinding>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _AjaxBinded(GridCommand command, FlowBindModel searchModel, string id)
        {
            SearchStatementModel searchStatementModel = PrepareBindedSearchStatement(command, searchModel, id);
            return PartialView(GetAjaxPageData<FlowBinding>(searchStatementModel, command));
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_View")]
        public ActionResult _BindingEdit(int? id)
        {
            if (!id.HasValue || id.Value == 0)
            {
                return HttpNotFound();
            }
            FlowBinding flowBinding = this.genericMgr.FindById<FlowBinding>(id);
            return PartialView(flowBinding);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult _BindingEdit(FlowBinding flowBinding)
        {
            ModelState.Remove("MasterFlow.Description");
            ModelState.Remove("BindedFlow.Description");
            if (ModelState.IsValid)
            {
                flowBinding.BindedFlow = this.genericMgr.FindById<FlowMaster>(flowBinding.BindedFlowCode);
                genericMgr.UpdateWithTrim(flowBinding);
                SaveSuccessMessage(Resources.SCM.FlowBinding.FlowBinding_Updated);
            }

            return PartialView(flowBinding);
        }

        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult _BindingNew(string id)
        {
            FlowMaster flow = this.genericMgr.FindById<FlowMaster>(id);
            FlowBinding flowBinding = new FlowBinding();
            flowBinding.MasterFlow = flow;
            return PartialView(flowBinding);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult _BindingNew(FlowBinding flowBinding)
        {
            ModelState.Remove("MasterFlow.Description");
            ModelState.Remove("BindedFlow.Description");
            if (ModelState.IsValid)
            {
                if (false)//暂不做控制&& this.genericMgr.FindAll<long>("sql")[0] > 0
                {
                    // base.SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, flowDetail.Code);
                }
                else
                {
                    flowBinding.BindedFlow = this.genericMgr.FindById<FlowMaster>(flowBinding.BindedFlowCode);
                    this.genericMgr.CreateWithTrim(flowBinding);
                    SaveSuccessMessage(Resources.SCM.FlowDetail.FlowDetail_Added);
                    return RedirectToAction("_Binding/" + flowBinding.MasterFlow.Code);
                }
            }
            return PartialView(flowBinding);
        }


        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Delete")]
        public ActionResult _BindingDelete(string id)
        {
            FlowBinding flowBinding = genericMgr.FindById<FlowBinding>(int.Parse(id));
            this.genericMgr.DeleteById<FlowBinding>(int.Parse(id));
            SaveSuccessMessage(Resources.SCM.FlowBinding.FlowDetail_Deleted);
            return RedirectToAction("_Binding/" + flowBinding.MasterFlow.Code);
        }

        private SearchStatementModel PrepareBindSearchStatement(GridCommand command, FlowBindModel searchModel, string id)
        {
            string whereStatement = " where mf.Code='" + id + "'";
            IList<object> param = new List<object>();

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "BindedFlow.Code")
                {
                    command.SortDescriptors[0].Member = "bf.Code";
                }
                else if (command.SortDescriptors[0].Member == "BindedFlow.Description")
                {
                    command.SortDescriptors[0].Member = "bf.Description";
                }
                else if (command.SortDescriptors[0].Member == "BindTypeDescription")
                {
                    command.SortDescriptors[0].Member = "f.BindType";
                }
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountBindStatement;
            searchStatementModel.SelectStatement = selectBindStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel PrepareBindedSearchStatement(GridCommand command, FlowBindModel searchModel, string id)
        {
            string whereStatement = " where bf.Code='" + id + "'";
            IList<object> param = new List<object>();



            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountBindStatement;
            searchStatementModel.SelectStatement = selectBindStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        #endregion


        #region Import
        [SconitAuthorize(Permissions = "Url_CustomerGoodsFlow_Edit")]
        public ActionResult Import(IEnumerable<HttpPostedFileBase> flowattachments)
        {
            try
            {
                foreach (var file in flowattachments)
                {
                    flowMgr.ImportFlow(file.InputStream, CodeMaster.OrderType.CustomerGoods);
                }
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_FlowImportSuccessfully);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
            }
            return Content(string.Empty);
        }

        #endregion
        #region Export all related flow Infor
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementFlow_View")]
        public ActionResult Export(FlowSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            IList<FlowMaster> flowMasters = GetAjaxPageData<FlowMaster>(searchStatementModel, command).Data.ToList();
            List<FlowDetail> flowDetails = new List<FlowDetail>();
            IList<FlowStrategy> flowStrategys = genericMgr.FindAll<FlowStrategy>();
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var flowMaster in flowMasters)
            {
                IList<FlowDetail> flowDetailLists = flowMgr.GetFlowDetailList(flowMaster.Code);
                FillCodeDetailDescription<FlowDetail>(flowDetailLists);
                foreach (var flowDetailList in flowDetailLists)
                {
                    //var zzz =  ;
                    //BillTermDescription ReceiveGapTo
                    //flowMaster.BillTerm   TranslateCodeDetailDescription
                    var item = itemMgr.GetCacheItem(flowDetailList.Item);
                    flowDetailList.CurrentFlowMaster = flowMaster;
                    flowDetailList.ItemDescription = item.Description;
                    flowDetailList.MaterialsGroup = item.MaterialsGroup;
                    flowDetailList.Warranty = item.Warranty;
                    flowDetailList.ItemOption = item.ItemOption;
                    flowDetailList.MaterialsGroupDesc = GetItemCategory(flowDetailList.MaterialsGroup,  itemCategoryList).Description;

                    flowDetailList.CurrentFlowStrategy = flowStrategys.Where(p => p.Flow == flowMaster.Code).FirstOrDefault() ?? new FlowStrategy();
                
                }
                flowDetails.AddRange(flowDetailLists);
            }
            var table = GetStringFlowInforView(flowDetails, true);
            return new DownloadFileActionResult(table, "CustomerFlowDetail.xls");
        }
        #endregion
        #region GetStringRccpFlowInforView
        private string GetStringFlowInforView(IList<FlowDetail> flowDetails, bool isPercentage, bool isCss = false)
        {
            if (flowDetails.Count() == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            int l = 0;
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_Code, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsActive, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_Description, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_ReferenceFlow, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_PartyFrom, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_PartyTo, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_LocationTo, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_Dock, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_ShipFrom, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_ShipTo, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_ReceiveGapTo, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_HuTemplate, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsReceiveScanHu, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsShipScanHu, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsPrintOrder, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_OrderTemplate, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsPrintAsn, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_AsnTemplate, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsPrintRceipt, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_ReceiptTemplate, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsOrderFulfillUC, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsShipFulfillUC, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsReceiveFulfillUC, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsListDet, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsManualCreateDetail, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsShipByOrder, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsShipExceed, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsReceiveExceed, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsAsnUniqueReceive, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsCheckPartyToAuth, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsAutoCreate, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsAutoRelease, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsAutoShip, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsAutoReceive, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsInspect, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowMaster.FlowMaster_IsMRP, true, false, l));
            //FlowStrategy
            str = str.Append(Str(Resources.SCM.FlowStrategy.FlowStrategy_LeadTime, true, false, l));
            str = str.Append(Str(Resources.EXT.ControllerLan.Con_EmergencyLead, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowStrategy.FlowStrategy_RccpLeadTime, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowStrategy.FlowStrategy_TimeUnit, true, false, l));
            //FlowDetails
            //str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_Flow, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_Sequence, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_Item, true, false, l));
            str = str.Append(Str(Resources.MD.Item.Item_Description, true, false, l));
            str = str.Append(Str(Resources.MD.Item.Item_MaterialsGroup, true, false, l));
            str = str.Append(Str(Resources.MD.Item.Item_MaterialsGroupDesc, true, false, l));
            str = str.Append(Str(Resources.MD.Item.Item_Warranty, true, false, l));
            str = str.Append(Str(Resources.MD.Item.Item_ItemOption, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_ReferenceItemCode, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_Uom, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_UnitCount, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_UnitCountDescription, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_StartDate, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_EndDate, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_SafeStock, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_MaxStock, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_MinLotSize, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_BatchSize, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_OrderLotSize, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_ReceiveLotSize, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_RoundUpOption, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_IsAutoCreate, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_IsInspect, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_MrpWeight, true, false, l));
            str = str.Append(Str(Resources.SCM.FlowDetail.FlowDetail_MrpPriority, true, false, l));
            str.Append("</tr>");
            foreach (var flowDetail in flowDetails)
            {
                l++;
                str = str.Append(Str(flowDetail.CurrentFlowMaster.Code, false, true, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsActive, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.Description, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.ReferenceFlow, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.PartyFrom, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.PartyTo, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.LocationTo, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.Dock, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.ShipFrom, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.ShipTo, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.ReceiveGapToDescription, false, false, l));
                str = str.Append(Str(systemMgr.TranslateCodeDetailDescription(flowDetail.CurrentFlowMaster.OrderTemplate.Replace(".xls", "")), false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsReceiveScanHu, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsShipScanHu, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsPrintOrder, false, false, l));
                str = str.Append(Str(systemMgr.TranslateCodeDetailDescription(flowDetail.CurrentFlowMaster.OrderTemplate.Replace(".xls", "")), false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsPrintAsn, false, false, l));
                str = str.Append(Str(systemMgr.TranslateCodeDetailDescription(flowDetail.CurrentFlowMaster.AsnTemplate.Replace(".xls", "")), false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsPrintRceipt, false, false, l));
                str = str.Append(Str(systemMgr.TranslateCodeDetailDescription(flowDetail.CurrentFlowMaster.ReceiptTemplate.Replace(".xls", "")), false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsOrderFulfillUC, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsShipFulfillUC, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsReceiveFulfillUC, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsListDet, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsManualCreateDetail, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsShipByOrder, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsShipExceed, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsReceiveExceed, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsAsnUniqueReceive, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsCheckPartyToAuthority, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsAutoCreate, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsAutoRelease, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsAutoShip, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsAutoReceive, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsInspect, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowMaster.IsMRP, false, false, l));
                //FlowStrategy
                str = str.Append(Str(flowDetail.CurrentFlowStrategy.LeadTime, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowStrategy.EmergencyLeadTime, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowStrategy.RccpLeadTime, false, false, l));
                str = str.Append(Str(flowDetail.CurrentFlowStrategy.TimeUnit, false, false, l));
                //FlowDetails
                //str = str.Append(Str(flowDetail.Flow, false, false, l));
                str = str.Append(Str(flowDetail.Sequence, false, false, l));
                str = str.Append(Str(flowDetail.Item, false, false, l));
                str = str.Append(Str(flowDetail.ItemDescription, false, false, l));
                str = str.Append(Str(flowDetail.MaterialsGroup, false, false, l));
                str = str.Append(Str(flowDetail.MaterialsGroupDesc, false, false, l));
                str = str.Append(Str(flowDetail.Warranty, false, false, l));
                str = str.Append(Str(systemMgr.GetCodeDetailDescription(CodeMaster.CodeMaster.ItemOption, (int)flowDetail.ItemOption), false, false, l));

                str = str.Append(Str(flowDetail.ReferenceItemCode, false, false, l));
                str = str.Append(Str(flowDetail.Uom, false, false, l));
                str = str.Append(Str(flowDetail.UnitCount, false, false, l));
                str = str.Append(Str(flowDetail.UnitCountDescription, false, false, l));
                str = str.Append(Str(flowDetail.StartDate, false, false, l));
                str = str.Append(Str(flowDetail.EndDate, false, false, l));
                str = str.Append(Str(flowDetail.SafeStock, false, false, l));
                str = str.Append(Str(flowDetail.MaxStock, false, false, l));
                str = str.Append(Str(flowDetail.MinLotSize, false, false, l));
                str = str.Append(Str(flowDetail.BatchSize, false, false, l));
                str = str.Append(Str(flowDetail.OrderLotSize, false, false, l));
                str = str.Append(Str(flowDetail.ReceiveLotSize, false, false, l));
                str = str.Append(Str(flowDetail.RoundUpOptionDescription, false, false, l));
                str = str.Append(Str(flowDetail.IsAutoCreate, false, false, l));
                str = str.Append(Str(flowDetail.IsInspect, false, false, l));
                str = str.Append(Str(flowDetail.MrpWeight, false, false, l));
                str = str.Append(Str(flowDetail.MrpPriority, false, false, l));
                str.Append("</tr>");
            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }
        private StringBuilder Str(object appendStr, bool isHead, bool newRow, int l)
        {
            StringBuilder str = new StringBuilder();
            if (newRow)
            {
                str.Append("<tr>");
            }
            if (isHead)
            {
                str.Append("<th>");
                str.Append(appendStr);
                str.Append("</th>");
            }
            else
            {
                str.Append("<td>");
                str.Append(appendStr);
                str.Append("</td>");
            }
            return str.Replace(">True<", ">Y<").Replace(">False<", ">N<");
        }
        #endregion
    }
}
