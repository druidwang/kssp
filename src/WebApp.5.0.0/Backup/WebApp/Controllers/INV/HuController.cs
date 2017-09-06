using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Web.Models;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.SCM;
using com.Sconit.Service;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.PrintModel.INV;
using AutoMapper;
using com.Sconit.Utility.Report;
using com.Sconit.Web.Models.SearchModels.BIL;
using com.Sconit.Entity.BIL;
using com.Sconit.Web.Models.SearchModels.SCM;
using com.Sconit.Web.Models.SearchModels.ORD;
using com.Sconit.Entity;
using com.Sconit.Entity.MRP.MD;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Entity.SYS;
using NHibernate;

namespace com.Sconit.Web.Controllers.INV
{
    public class HuController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from Hu as h";
        private static string selectStatement = "select h from Hu as h";

        private static string selectCountFlowStatement = "select count(*) from FlowDetail as h";
        private static string selectFlowStatement = "select h from FlowDetail as h";

        private static string selectIpCountStatement = "select count(*) from IpDetail as h";
        private static string selectIpStatement = "select h from IpDetail as h";

        private static string selectLocationCountStatement = "select count(*) from IpLocationDetail as h";
        private static string selectLocationStatement = "select h from IpLocationDetail as h";

        private static string selectOrderCountStatement = "select count(*) from OrderDetail as o";
        private static string selectOrderStatement = "select o from OrderDetail as o";

        //public IGenericMgr genericMgr { get; set; }
        //public IFlowMgr flowMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        public IHuMgr huMgr { get; set; }
        //public IReportGen reportGen { get; set; }

        #region public method
        public ActionResult Index()
        {
            ViewBag.CreateUserName = this.CurrentUser.FullName;
            return View();
        }


        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult New()
        {
            TempData["FlowDetailSearchModel"] = null;


            return View();
        }
        [SconitAuthorize(Permissions = "Inventory_ShelfLifeWarning")]
        public ActionResult ShelfLifeWarningIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Inventory_ShelfLifeWarning")]
        public ActionResult ShelfLifeWarningList(GridCommand command, HuSearchModel searchModel)
        {
            TempData["HuSearchModel"] = searchModel;
            ViewBag.SearchCondition = searchModel.SearchCondition;
            ViewBag.IsSumByItem = searchModel.IsSumByItem;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }
        #region exportShelfLifeWarning
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Inventory_ShelfLifeWarning")]
        public void Export(LocationLotDetailSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = !this.CheckSearchModelIsNull(searchModel) ? 0 : value;
            ReportSearchStatementModel reportSearchStatementModel = PrepareShelfLifeWarningSearchStatement(command, searchModel);
            var list = GetHuAjaxPageData<Hu>(reportSearchStatementModel);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var listdata in list.Data)
            {
                listdata.ExpireDateValue = listdata.ExpireDate == null ? "" : listdata.ExpireDate.Value.ToString("yyyy-MM-dd");
                listdata.RemindExpireDateValue = listdata.RemindExpireDate == null ? "" : listdata.RemindExpireDate.Value.ToString("yyyy-MM-dd");
                listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).MaterialsGroup;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            }
            var filename = "";
            if (searchModel.SearchCondition == 1)
            {
                filename = "OutOfExpireTimeWarning.xls";
            }
            else if (searchModel.SearchCondition == 3)
            {
                filename = "ShelfLifeWarningSummary.xls";
            }
            else
            {
                filename = "ShelfLifeWarning.xls";
            }
            ExportToXLS<Hu>(filename, list.Data.ToList());
        }
        #endregion
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Inventory_ShelfLifeWarning")]
        public ActionResult _AjaxShelfLifeWarningList(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            if (command.Page == null)
            {
                command.Page = 1;
            }
            //command.PageSize = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(com.Sconit.Entity.SYS.EntityPreference.CodeEnum.MaxRowSizeOnPage)); ;
            ReportSearchStatementModel reportSearchStatementModel = PrepareShelfLifeWarningSearchStatement(command, searchModel);
            var list = GetHuAjaxPageData<Hu>(reportSearchStatementModel);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var listdata in list.Data)
            {
                listdata.ExpireDateValue = listdata.ExpireDate == null ? "": listdata.ExpireDate.Value.ToString("yyyy-MM-dd");
                listdata.RemindExpireDateValue = listdata.RemindExpireDate == null ? "" : listdata.RemindExpireDate.Value.ToString("yyyy-MM-dd");
                listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).MaterialsGroup;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            }

            //foreach (var listData in list.Data)
            //{
            //    if (listData.RemindExpireDate <= DateTime.Now && listData.ExpireDate >= DateTime.Now)
            //    {
            //        listData.ExpireStatus = "预警";
            //    }
            //    else if (listData.ExpireDate < DateTime.Now)
            //    {
            //        listData.ExpireStatus = "过期";
            //    }
            //}
            return PartialView(list);
        }

        private ReportSearchStatementModel PrepareShelfLifeWarningSearchStatement(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            ReportSearchStatementModel reportSearchStatementModel = new ReportSearchStatementModel();
            reportSearchStatementModel.ProcedureName = "USP_Report_ShelfLifeWarning";

            SqlParameter[] parameters = new SqlParameter[6];

            parameters[0] = new SqlParameter("@Location", SqlDbType.VarChar, 8000);
            parameters[0].Value = searchModel.Location;

            parameters[1] = new SqlParameter("@Item", SqlDbType.VarChar, 8000);
            parameters[1].Value = searchModel.Item;

            if (searchModel.SearchCondition == 0)
            {
                searchModel.GetType = "ByExpireTime";
            }
            else if (searchModel.SearchCondition == 1)
            {
                searchModel.GetType = "ByOutOfExpireTime";
            }
            else if (searchModel.SearchCondition == 2)
            {
                searchModel.GetType = "ByRemindExpireTime";
            }
            else
            {
                searchModel.GetType = "Summary";
            }
            parameters[2] = new SqlParameter("@Type", SqlDbType.VarChar);
            parameters[2].Value = searchModel.GetType;

            parameters[3] = new SqlParameter("@PageSize", SqlDbType.VarChar);
            parameters[3].Value = command.PageSize;

            parameters[4] = new SqlParameter("@Page", SqlDbType.VarChar);
            parameters[4].Value = command.Page;

            //parameters[5] = new SqlParameter("@IsSumByItem", SqlDbType.VarChar);
            //parameters[5].Value = searchModel.IsSumByItem;
            reportSearchStatementModel.Parameters = parameters;

            return reportSearchStatementModel;
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_View")]
        public ActionResult List(GridCommand command, HuSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_View")]
        public ActionResult _AjaxList(GridCommand command, HuSearchModel searchModel)
        {
            TempData["HuSearchModel"] = searchModel;
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            var list = GetAjaxPageData<Hu>(searchStatementModel, command);
            foreach (var data in list.Data)
            {
                data.HuStatus = huMgr.GetHuStatus(data.HuId);
                data.ItemDescription = string.IsNullOrWhiteSpace(data.ReferenceItemCode) ? data.ItemDescription : data.ItemDescription + "[" + data.ReferenceItemCode + "]";
            }

            return PartialView(list);
        }

        public ActionResult HuDetail(string id, string BackUrl)
        {
            Hu hu = genericMgr.FindById<Hu>(id);
            hu.HuStatus = huMgr.GetHuStatus(id);
            FillCodeDetailDescription(hu);
            FillCodeDetailDescription(hu.HuStatus);
            if (!string.IsNullOrWhiteSpace(hu.ItemVersion))
            {
                hu.ItemVersion = hu.ItemVersion + "[" + genericMgr.FindById<ProductType>(hu.ItemVersion).Description + "]";
            }
            ViewBag.BackUrl = BackUrl;
            if (ViewBag.BackUrl == null)
            {
                ViewBag.BackUrl = "/Hu/List";
            }
            return View("HuDetail", string.Empty, hu);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _NewHuTrack(string HuId)
        {
            if (string.IsNullOrEmpty(HuId))
            {
                return HttpNotFound();
            }
            else
            {
                string selectStatement = "from Hu as l where l.RefHu =? order by HuId ";
                IList<Hu> newHuList = genericMgr.FindAll<Hu>(selectStatement, HuId);
                FillCodeDetailDescription<Hu>(newHuList);
                return PartialView(newHuList);
            }
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _Loctrans(string HuId)
        {
            if (string.IsNullOrEmpty(HuId))
            {
                return HttpNotFound();
            }
            else
            {
                string selectStatement = "from LocationTransaction as l where l.HuId=? order by Id ";
                IList<LocationTransaction> loTranDetailList = genericMgr.FindAll<LocationTransaction>(selectStatement, HuId);
                FillCodeDetailDescription<LocationTransaction>(loTranDetailList);
                return PartialView(loTranDetailList);
            }
        }
        #region  FlowMaster
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult FlowMaster()
        {
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult _FlowDetailList(GridCommand command, FlowDetailSearchModel searchModel)
        {
            TempData["FlowDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult _AjaxFlowDetailList(GridCommand command, FlowDetailSearchModel searchModel)
        {
            try
            {
                SearchStatementModel searchStatementModel = PrepareDetailFlowSearchStatement(command, searchModel);
                GridModel<FlowDetail> List = GetAjaxPageData<FlowDetail>(searchStatementModel, command);
                FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(searchModel.Flow);

                if (!Utility.SecurityHelper.HasPermission(flowMaster))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_LackTheFlowPermission, flowMaster.Code);
                }

                foreach (FlowDetail flowDetail in List.Data)
                {
                    flowDetail.ManufactureParty = flowMaster.PartyFrom;
                    flowDetail.LotNo = LotNoHelper.GenerateLotNo();
                    Item item = genericMgr.FindById<Item>(flowDetail.Item);
                    flowDetail.ItemDescription = item.Description;
                }
                return PartialView(List);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return PartialView(new GridModel(new List<FlowDetail>()));
            }
        }

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public JsonResult CreateHuByFlow(string FlowidStr, string FlowucStr, string FlowsupplierLotNoStr, string FlowqtyStr, bool FlowisExport,
             string FlowmanufactureDateStr, string FlowremarkStr, bool FlowCheckExport)
        {
            try
            {
                IList<FlowDetail> nonZeroFlowDetailList = new List<FlowDetail>();
                if (!string.IsNullOrEmpty(FlowidStr))
                {
                    string[] manufactureDateArray = FlowmanufactureDateStr.Split(',');
                    string[] idArray = FlowidStr.Split(',');
                    string[] ucArray = FlowucStr.Split(',');
                    string[] supplierLotNoArray = FlowsupplierLotNoStr.Split(',');
                    string[] qtyArray = FlowqtyStr.Split(',');
                    string[] remarkArray = FlowremarkStr.Split(',');
                    FlowMaster flowMaster = null;

                    if (idArray != null && idArray.Count() > 0)
                    {
                        for (int i = 0; i < idArray.Count(); i++)
                        {
                            //string lotNo = manufactureDateArray[i].Replace("-", "");
                            FlowDetail flowDetail = genericMgr.FindById<FlowDetail>(Convert.ToInt32(idArray[i]));
                            if (flowMaster == null)
                            {
                                flowMaster = genericMgr.FindById<FlowMaster>(flowDetail.Flow);
                            }
                            if (flowMaster != null && flowMaster.UcDeviation >= 0)
                            {
                                double ucDeviation = flowMaster.UcDeviation;
                                flowDetail.MaxUc = flowDetail.UnitCount * Convert.ToDecimal((ucDeviation / 100) + 1);
                                flowDetail.MinUc = flowDetail.UnitCount * Convert.ToDecimal(1 - (ucDeviation / 100));
                            }
                            else
                            {
                                flowDetail.MaxUc = decimal.MaxValue;
                                flowDetail.MinUc = 0;
                            }

                            flowDetail.ManufactureDate = Convert.ToDateTime(manufactureDateArray[i]);
                            flowDetail.SupplierLotNo = supplierLotNoArray[i];
                            flowDetail.LotNo = Utility.LotNoHelper.GenerateLotNo(flowDetail.ManufactureDate);
                            flowDetail.ManufactureParty = flowMaster.PartyFrom;
                            flowDetail.UnitCount = Convert.ToDecimal(ucArray[i]);
                            flowDetail.Remark = remarkArray[i];

                            var qtys = qtyArray[i].Split(' ');
                            if (qtys.Length == 1)
                            {
                                decimal qty = decimal.Parse(qtys[0]);
                                if (qty / flowDetail.UnitCount > 500)
                                {
                                    throw new BusinessException(string.Format(Resources.EXT.ControllerLan.Con_PrintedBarcodeQuantityCanNotExceedFiveHundred));
                                }
                                for (decimal j = 0; j < qty; j += flowDetail.UnitCount)
                                {
                                    var newFlowDetail = Mapper.Map<FlowDetail, FlowDetail>(flowDetail);
                                    bool isLastHu = (qty - j) <= flowDetail.MaxUc && (qty - j) >= flowDetail.MinUc;
                                    if (isLastHu)
                                    {
                                        newFlowDetail.HuQty = (qty - j);
                                    }
                                    else
                                    {
                                        newFlowDetail.HuQty = (qty - j) < flowDetail.UnitCount ? (qty - j) : flowDetail.UnitCount;
                                    }
                                    if (newFlowDetail.HuQty > flowDetail.MaxUc || newFlowDetail.HuQty < flowDetail.MinUc)
                                    {
                                        SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_QuantityInputError,
                                                    newFlowDetail.Item, flowDetail.MaxUc.ToString("0.##"), flowDetail.MinUc.ToString("0.##")));
                                        object fail1 = new { Fail = "True" };

                                        return Json(fail1);
                                        //throw new BusinessException(string.Format("数量输入错误,根据单包装浮动范围物料代码{0}的最大数量为{1},最小数量为{2}",
                                        //    newFlowDetail.Item, flowDetail.MaxUc, flowDetail.MinUc));
                                    }
                                    nonZeroFlowDetailList.Add(newFlowDetail);
                                    if (isLastHu) break;
                                }
                            }
                            else
                            {
                                for (int j = 0; j < qtys.Length; j++)
                                {
                                    if (qtys[j].Length > 0)
                                    {
                                        var newFlowDetail = Mapper.Map<FlowDetail, FlowDetail>(flowDetail);
                                        newFlowDetail.HuQty = Convert.ToDecimal(qtys[j]);
                                        if (newFlowDetail.HuQty > flowDetail.MaxUc || newFlowDetail.HuQty < flowDetail.MinUc)
                                        {
                                            SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_QuantityInputError,
                                                    newFlowDetail.Item, flowDetail.MaxUc.ToString("0.##"), flowDetail.MinUc.ToString("0.##")));
                                            object fail = new { Fail = "True" };

                                            return Json(fail);
                                            //throw new BusinessException(string.Format("数量输入错误,根据单包装浮动范围物料代码{0}的最大数量为{1},最小数量为{2}",
                                            //    newFlowDetail.Item, flowDetail.MaxUc, flowDetail.MinUc));
                                        }
                                        nonZeroFlowDetailList.Add(newFlowDetail);
                                    }
                                }
                            }
                        }
                    }

                    this.genericMgr.CleanSession();
                    if (flowMaster != null)
                    {
                        if (FlowisExport && FlowCheckExport)
                        {
                            var flowCheckExport = string.Format("OK");
                            return Json(new { FlowCheckExport = flowCheckExport });
                        }

                        IList<Hu> huList = huMgr.CreateHu(flowMaster, nonZeroFlowDetailList);
                        string manufacturePartyDescription = queryMgr.FindById<Party>(flowMaster.PartyFrom).Name;
                        foreach (var hu in huList)
                        {
                            if (!string.IsNullOrEmpty(hu.ManufactureParty))
                            {
                                hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                            }
                            if (!string.IsNullOrWhiteSpace(hu.Direction))
                            {
                                hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                            }
                        }
                        if (FlowisExport)
                        {
                            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
                            IList<object> data = new List<object>();
                            data.Add(printHuList);
                            data.Add(CurrentUser.FullName);
                            reportGen.WriteToClient(flowMaster.HuTemplate, data, flowMaster.HuTemplate);
                            object obj1 = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_BarcodeExportedSuccessfully, huList.Count) };
                            return Json(obj1);
                        }
                        else
                        {
                            string printUrl = PrintHuList(huList, flowMaster.HuTemplate);
                            object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_BarcodePrintedSuccessfully_1, huList.Count), PrintUrl = printUrl };
                            return Json(obj);
                        }
                    }
                }
                return Json(null);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        #endregion

        #region OrderMaster

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult OrderMaster()
        {
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult OrderDetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            ViewBag.OrderNo = searchModel.OrderNo;
            TempData["OrderMasterSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult _AjaxOrderDetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            try
            {
                com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
                var orderMaster = this.genericMgr.FindById<OrderMaster>(searchModel.OrderNo);
                if (!Utility.SecurityHelper.HasPermission(orderMaster))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_LackTheOrderNumberPermission, searchModel.OrderNo);
                }
                if (orderMaster.Type != com.Sconit.CodeMaster.OrderType.Production)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_InputOrderNumberIsNotProductionOrderNumber, searchModel.OrderNo);
                }
                var orderDetailList = this.genericMgr.FindAll<OrderDetail>
                 (" from OrderDetail where OrderNo = ?", searchModel.OrderNo);

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    orderDetail.ManufactureParty = orderMaster.PartyFrom;
                    orderDetail.HuQty = orderDetail.OrderedQty;
                    orderDetail.LotNo = LotNoHelper.GenerateLotNo();
                    orderDetail.ManufactureDateStrFormat = orderMaster.StartTime.ToString("yyyy-MM-dd");
                    if (!orderDetail.IsChangeUnitCount)
                    {
                        //orderDetail.HuQty = Math.Ceiling(orderDetail.HuQty / orderDetail.UnitCount) * orderDetail.UnitCount;
                    }
                }
                return PartialView(new GridModel(orderDetailList));
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
        }

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public JsonResult CreateHuByOrderDetail(string OrderDetailidStr, string OrderDetailucStr, string OrderDetailsupplierLotNoStr, string OrderDetailqtyStr
            , bool OrderDetailisExport, string OrderDetailimanufactureDateStr)
        {
            try
            {
                IList<OrderDetail> nonZeroOrderDetailList = new List<OrderDetail>();
                if (!string.IsNullOrEmpty(OrderDetailidStr))
                {
                    string[] manufactureDateArray = OrderDetailimanufactureDateStr.Split(',');
                    string[] idArray = OrderDetailidStr.Split(',');
                    string[] ucArray = OrderDetailucStr.Split(',');
                    string[] supplierLotNoArray = OrderDetailsupplierLotNoStr.Split(',');
                    string[] qtyArray = OrderDetailqtyStr.Split(',');
                    OrderMaster orderMaster = null;

                    if (idArray != null && idArray.Count() > 0)
                    {
                        for (int i = 0; i < idArray.Count(); i++)
                        {
                            //string lotNo = manufactureDateArray[i].Replace("-", "");
                            OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(Convert.ToInt32(idArray[i]));
                            if (orderMaster == null)
                            {
                                orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);
                                //orderMaster.HuTemplate = orderMaster.HuTemplate.Trim();
                            }

                            orderDetail.UnitCount = Convert.ToDecimal(ucArray[i]);
                            orderDetail.SupplierLotNo = supplierLotNoArray[i];
                            orderDetail.ManufactureDate = Convert.ToDateTime(manufactureDateArray[i]);
                            orderDetail.LotNo = Utility.LotNoHelper.GenerateLotNo(orderDetail.ManufactureDate);
                            orderDetail.ManufactureParty = orderMaster.PartyFrom;
                            orderDetail.HuQty = Convert.ToDecimal(qtyArray[i]);
                            nonZeroOrderDetailList.Add(orderDetail);
                        }
                    }
                    this.genericMgr.CleanSession();
                    if (orderMaster != null)
                    {
                        IList<Hu> huList = huMgr.CreateHu(orderMaster, nonZeroOrderDetailList);
                        foreach (var hu in huList)
                        {
                            hu.ManufacturePartyDescription = orderMaster.PartyFromName;
                            if (!string.IsNullOrWhiteSpace(hu.Direction))
                            {
                                hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                            }
                        }
                        if (string.IsNullOrEmpty(orderMaster.HuTemplate))
                        {
                            orderMaster.HuTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                        }
                        if (OrderDetailisExport)
                        {
                            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
                            IList<object> data = new List<object>();
                            data.Add(printHuList);
                            data.Add(CurrentUser.FullName);

                            reportGen.WriteToClient(orderMaster.HuTemplate, data, orderMaster.HuTemplate);
                            return Json(null);
                        }
                        else
                        {
                            string printUrl = PrintHuList(huList, orderMaster.HuTemplate);
                            object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_BarcodePrintedSuccessfully_1, huList.Count), PrintUrl = printUrl };
                            return Json(obj);
                        }
                    }
                }
                return Json(null);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }
        #endregion

        #region IpMaster
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult IpMaster()
        {
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult IpDetailList(GridCommand command, IpDetailSearchModel searchModel)
        {
            TempData["IpDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (string.IsNullOrEmpty(searchModel.IpNo))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_ShipOrderSearchConditionIsNeeded);
            }
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult _AjaxIpDetailList(GridCommand command, IpDetailSearchModel searchModel)
        {
            try
            {
                if (string.IsNullOrEmpty(searchModel.IpNo))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ShipOrderNumberNotExits);
                }
                else
                {
                    var ipmaster = genericMgr.FindById<IpMaster>(searchModel.IpNo);
                    if (!Utility.SecurityHelper.HasPermission(ipmaster))
                    {
                        throw new BusinessException(Resources.EXT.ControllerLan.Con_LackTheShipOrderNumberPermission, searchModel.IpNo);
                    }
                    SearchStatementModel searchStatementModel = PrepareIpDetailSearchStatement(command, searchModel);
                    GridModel<IpDetail> List = GetAjaxPageData<IpDetail>(searchStatementModel, command);
                    foreach (IpDetail ipDetail in List.Data)
                    {
                        ipDetail.ManufactureParty = ipmaster.PartyFrom;
                        ipDetail.LotNo = LotNoHelper.GenerateLotNo();
                        ipDetail.HuQty = ipDetail.Qty;
                        if (!ipDetail.IsChangeUnitCount)
                        {
                            ipDetail.HuQty = Math.Ceiling(ipDetail.HuQty / ipDetail.UnitCount) * ipDetail.UnitCount;
                        }
                    }
                    return PartialView(List);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return PartialView(new GridModel(new List<IpDetail>()));
            }
        }

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public JsonResult CreateHuByIpDetail(string IpDetailidStr, string IpDetailucStr, string IpDetailsupplierLotNoStr, string IpDetailqtyStr,
             bool IpDetailisExport, string FlowremarkStrn, string IpDetailmanufactureDateStr, bool IpDetailisCheckExport)
        {
            try
            {
                IList<IpDetail> nonZeroIpDetailList = new List<IpDetail>();
                if (!string.IsNullOrEmpty(IpDetailidStr))
                {
                    string[] manufactureDateArray = IpDetailmanufactureDateStr.Split(',');
                    string[] idArray = IpDetailidStr.Split(',');
                    string[] ucArray = IpDetailucStr.Split(',');
                    string[] supplierLotNoArray = IpDetailsupplierLotNoStr.Split(',');
                    string[] qtyArray = IpDetailqtyStr.Split(',');
                    string[] remarkArray = FlowremarkStrn.Split(',');
                    IpMaster ipMaster = null;
                    FlowMaster flowMaster = null;

                    if (idArray != null && idArray.Count() > 0)
                    {
                        for (int i = 0; i < idArray.Count(); i++)
                        {
                            IpDetail ipDetail = genericMgr.FindById<IpDetail>(Convert.ToInt32(idArray[i]));
                            if (ipMaster == null)
                            {
                                ipMaster = genericMgr.FindById<IpMaster>(ipDetail.IpNo);
                                //ipMaster.HuTemplate = ipMaster.HuTemplate.Trim();
                                if (!string.IsNullOrEmpty(ipMaster.Flow))
                                {
                                    flowMaster = genericMgr.FindById<FlowMaster>(ipMaster.Flow);
                                }
                            }
                            if (flowMaster != null && flowMaster.UcDeviation >= 0)
                            {
                                double ucDeviation = flowMaster.UcDeviation;
                                ipDetail.MaxUc = ipDetail.UnitCount * Convert.ToDecimal(((ucDeviation / 100) + 1));
                                ipDetail.MinUc = ipDetail.UnitCount * Convert.ToDecimal((1 - (ucDeviation / 100)));
                            }
                            else
                            {
                                ipDetail.MaxUc = decimal.MaxValue;
                                ipDetail.MinUc = 0;
                            }
                            //string lotNo = manufactureDateArray[i].Replace("-", "");
                            ipDetail.UnitCount = Convert.ToDecimal(ucArray[i]);
                            ipDetail.SupplierLotNo = supplierLotNoArray[i];
                            ipDetail.ManufactureParty = ipMaster.PartyFrom;
                            ipDetail.ManufactureDate = Convert.ToDateTime(manufactureDateArray[i]);
                            ipDetail.LotNo = Utility.LotNoHelper.GenerateLotNo(ipDetail.ManufactureDate);
                            ipDetail.Remark = remarkArray[i];
                            //ipDetail.HuQty = Convert.ToDecimal(qtyArray[i]);

                            var qtys = qtyArray[i].Split(' ');
                            if (qtys.Length == 1)
                            {
                                decimal qty = decimal.Parse(qtys[0]);
                                if (qty / ipDetail.UnitCount > 500)
                                {
                                    throw new BusinessException(string.Format(Resources.EXT.ControllerLan.Con_PrintedBarcodeQuantityCanNotExceedFiveHundred));
                                }
                                for (decimal j = 0; j < qty; j += ipDetail.UnitCount)
                                {
                                    var newIpDetail = Mapper.Map<IpDetail, IpDetail>(ipDetail);
                                    newIpDetail.HuQty = (qty - j) < newIpDetail.UnitCount ? (qty - j) : newIpDetail.UnitCount;
                                    if (newIpDetail.HuQty > ipDetail.MaxUc || newIpDetail.HuQty < ipDetail.MinUc)
                                    {
                                        SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_QuantityInputError,
                                                    newIpDetail.Item, ipDetail.MaxUc.ToString("0.##"), ipDetail.MinUc.ToString("0.##")));
                                        object fail1 = new { Fail = "True" };

                                        return Json(fail1);
                                        //throw new BusinessException(string.Format("数量输入错误,根据单包装浮动范围物料代码{0}的最大数量为{1},最小数量为{2}",
                                        //     newIpDetail.Item, ipDetail.MaxUc, ipDetail.MinUc));
                                    }
                                    nonZeroIpDetailList.Add(newIpDetail);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < qtys.Length; j++)
                                {
                                    if (qtys[j].Length > 0)
                                    {
                                        var newIpDetail = Mapper.Map<IpDetail, IpDetail>(ipDetail);
                                        newIpDetail.HuQty = Convert.ToDecimal(qtys[j]);
                                        if (newIpDetail.HuQty > ipDetail.MaxUc || newIpDetail.HuQty < ipDetail.MinUc)
                                        {
                                            SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_QuantityInputError,
                                                        newIpDetail.Item, ipDetail.MaxUc.ToString("0.##"), ipDetail.MinUc.ToString("0.##")));
                                            object fail = new { Fail = "True" };

                                            return Json(fail);
                                            //throw new BusinessException(string.Format("数量输入错误,根据单包装浮动范围物料代码{0}的最大数量为{1},最小数量为{2}",
                                            //     newIpDetail.Item, ipDetail.MaxUc, ipDetail.MinUc));
                                        }
                                        nonZeroIpDetailList.Add(newIpDetail);
                                    }
                                }
                            }
                        }
                    }
                    this.genericMgr.CleanSession();
                    if (ipMaster != null)
                    {
                        if (IpDetailisExport && IpDetailisCheckExport)
                        {
                            var ipDetailisCheckExport = string.Format("OK");
                            return Json(new { IpDetailisCheckExport = ipDetailisCheckExport });
                        }

                        IList<Hu> huList = huMgr.CreateHu(ipMaster, nonZeroIpDetailList);
                        foreach (var hu in huList)
                        {
                            hu.ManufacturePartyDescription = ipMaster.PartyFromName;
                            if (!string.IsNullOrWhiteSpace(hu.Direction))
                            {
                                hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                            }
                        }
                        if (string.IsNullOrEmpty(ipMaster.HuTemplate))
                        {
                            ipMaster.HuTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                        }
                        if (IpDetailisExport)
                        {
                            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
                            IList<object> data = new List<object>();
                            data.Add(printHuList);
                            data.Add(CurrentUser.FullName);
                            reportGen.WriteToClient(ipMaster.HuTemplate, data, ipMaster.IpNo + "Hu.xls");
                            return Json(null);
                        }
                        else
                        {
                            string printUrl = PrintHuList(huList, ipMaster.HuTemplate);
                            object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_BarcodePrintedSuccessfully_1, huList.Count), PrintUrl = printUrl };
                            return Json(obj);
                        }
                    }
                }
                return Json(null);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        #region IpLocationDetail

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public JsonResult PrintByIpLocationDetail(string checkedHuIds)
        {
            try
            {
                string[] checkedHuIdArray = checkedHuIds.Split(',');
                string selectStatement = string.Empty;
                IList<object> param = new List<object>();
                foreach (var huId in checkedHuIdArray)
                {
                    if (selectStatement == string.Empty)
                    {
                        selectStatement = "from Hu where HuId in (?";
                    }
                    else
                    {
                        selectStatement += ",?";
                    }
                    param.Add(huId);
                }
                selectStatement += ")";

                IList<Hu> huList = genericMgr.FindAll<Hu>(selectStatement, param.ToArray());
                SaveSuccessMessage(Resources.INV.Hu.Hu_HuPrintedByIpMaster);
                string huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                string printUrl = PrintHuList(huList, huTemplate);
                object obj = new { PrintUrl = printUrl };
                return Json(obj);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult _AjaxIpLocationDetailList(GridCommand command, IpDetailSearchModel searchModel)
        {

            if (string.IsNullOrEmpty(searchModel.IpNo))
            {
                return PartialView(new GridModel(new List<IpLocationDetail>()));
            }
            else
            {
                SearchStatementModel searchStatementModel = PrepareLocationDetailSearchStatement(command, searchModel);
                return PartialView(GetAjaxPageData<IpLocationDetail>(searchStatementModel, command));
            }
        }
        #endregion

        #endregion

        #region Item
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult Item()
        {
            IList<FlowMaster> flowMasterList = genericMgr.FindAllIn<FlowMaster>("from FlowMaster as i where i.ResourceGroup=? and IsActive=1 ", new object[] { com.Sconit.CodeMaster.ResourceGroup.FI });
            var mlist = from c in CurrentUser.RegionPermissions
                       join p in flowMasterList on c equals p.PartyFrom
                       select new { p };

            ViewBag.PrintHuPermissions = mlist.ToList().Count() > 1;
            return PartialView();
        }

        public ActionResult _GetItemDetail(string itemCode)
        {
            Item item = genericMgr.FindById<Item>(itemCode);
            if (item != null)
            {
                item.MinUnitCount = item.UnitCount;
            }

            return this.Json(item);
        }

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public JsonResult CreateHuByItem(string ItemCode, string HuUom, decimal HuUnitCount, decimal HuQty, string ManufactureParty,
            bool IsExport, string SupplierLotNo, string ManufactureDate, string Direction, int HuOption, string HuTemplate, string Remark)
        {
            try
            {
                Item item = genericMgr.FindById<Item>(ItemCode);
                item.HuUom = HuUom;
                item.HuUnitCount = HuUnitCount;
                item.ManufactureDate = Convert.ToDateTime(ManufactureDate);
                item.HuQty = HuQty;
                item.ManufactureParty = ManufactureParty;
                item.LotNo = Utility.LotNoHelper.GenerateLotNo(item.ManufactureDate);//ManufactureDate.Replace("-", "");
                item.SupplierLotNo = SupplierLotNo;
                item.HuOption = (CodeMaster.HuOption)HuOption;
                item.Deriction = Direction;
                item.Remark = Remark;
                string huTemplate = HuTemplate;
                if (string.IsNullOrWhiteSpace(huTemplate))
                {
                    huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                }
                item.HuTemplate = huTemplate;

                IList<Hu> huList = huMgr.CreateHu(item);
                foreach (var hu in huList)
                {
                    if (!string.IsNullOrEmpty(hu.ManufactureParty))
                    {
                        if (!string.IsNullOrEmpty(hu.ManufactureParty))
                        {
                            hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                        }
                        if (!string.IsNullOrWhiteSpace(hu.Direction))
                        {
                            hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                        }
                    }
                }
                if (IsExport)
                {
                    IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
                    IList<object> data = new List<object>();
                    data.Add(printHuList);
                    data.Add(CurrentUser.FullName);

                    reportGen.WriteToClient(huTemplate, data, huTemplate);
                    return Json(null);
                }
                else
                {
                    string printUrl = PrintHuList(huList, huTemplate);
                    object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_BarcodePrintedSuccessfully_1, huList.Count), PrintUrl = printUrl };
                    return Json(obj);
                }
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }

        #endregion

        #region Parts

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult Parts()
        {
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult PartsDetailList(GridCommand command, string searchOrderNo)
        {
            ViewBag.SearchOrderNo = searchOrderNo;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public ActionResult _AjaxPartsDetailList(GridCommand command, string searchOrderNo)
        {
            try
            {
                com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
                var orderMaster = this.genericMgr.FindById<OrderMaster>(searchOrderNo);
                if (!Utility.SecurityHelper.HasPermission(orderMaster))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_LackTheOrderNumberPermission, searchOrderNo);
                }
                if (orderMaster.Type != com.Sconit.CodeMaster.OrderType.Production)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_InputOrderNumberIsNotProductionOrderNumber, searchOrderNo);
                }
                var orderDetailList = this.genericMgr.FindAll<OrderDetail>(" from OrderDetail where OrderNo = ?", searchOrderNo);

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    orderDetail.ManufactureParty = orderMaster.PartyFrom;
                    orderDetail.HuQty = orderDetail.OrderedQty;
                    orderDetail.LotNo = LotNoHelper.GenerateLotNo();
                }
                return PartialView(new GridModel(orderDetailList));
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
        }

        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public JsonResult CreatePartsHu(string partsDetailidStr, string partsDetailucStr, string partsDetailsupplierLotNoStr, string partsDetailqtyStr
            , bool partsDetailisExport, string partsDetailimanufactureDateStr)
        {
            try
            {
                IList<OrderDetail> nonZeroOrderDetailList = new List<OrderDetail>();
                if (!string.IsNullOrEmpty(partsDetailidStr))
                {
                    string[] manufactureDateArray = partsDetailimanufactureDateStr.Split(',');
                    string[] idArray = partsDetailidStr.Split(',');
                    string[] ucArray = partsDetailucStr.Split(',');
                    string[] supplierLotNoArray = partsDetailsupplierLotNoStr.Split(',');
                    string[] qtyArray = partsDetailqtyStr.Split(',');
                    OrderMaster orderMaster = null;

                    if (idArray != null && idArray.Count() > 0)
                    {
                        for (int i = 0; i < idArray.Count(); i++)
                        {
                            OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(Convert.ToInt32(idArray[i]));
                            if (orderMaster == null)
                            {
                                orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);
                            }

                            orderDetail.UnitCount = Convert.ToDecimal(ucArray[i]);
                            orderDetail.SupplierLotNo = supplierLotNoArray[i];
                            orderDetail.ManufactureDate = Convert.ToDateTime(manufactureDateArray[i]);
                            orderDetail.LotNo = Utility.LotNoHelper.GenerateLotNo(orderDetail.ManufactureDate);
                            orderDetail.ManufactureParty = orderMaster.PartyFrom;
                            orderDetail.HuQty = Convert.ToDecimal(qtyArray[i]);
                            nonZeroOrderDetailList.Add(orderDetail);
                        }
                    }
                    this.genericMgr.CleanSession();
                    if (orderMaster != null)
                    {
                        IList<Hu> huList = this.GetPartsHu(orderMaster, nonZeroOrderDetailList);
                        foreach (var hu in huList)
                        {
                            hu.ManufacturePartyDescription = orderMaster.PartyFromName;
                            if (!string.IsNullOrWhiteSpace(hu.Direction))
                            {
                                hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                            }
                        }
                        //if (string.IsNullOrEmpty(orderMaster.HuTemplate))
                        //{
                        //    orderMaster.HuTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                        //}
                        if (partsDetailisExport)
                        {
                            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
                            IList<object> data = new List<object>();
                            data.Add(printHuList);
                            data.Add(CurrentUser.FullName);

                            reportGen.WriteToClient("BarCodeParts.xls", data, "BarCodeParts.xls");
                            return Json(null);
                        }
                        else
                        {
                            string printUrl = PrintHuList(huList, "BarCodeParts.xls");
                            object obj = new { SuccessMessage = string.Format(Resources.EXT.ControllerLan.Con_BarcodePrintedSuccessfully_1, huList.Count), PrintUrl = printUrl };
                            return Json(obj);
                        }
                    }
                }
                return Json(null);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }
        #endregion

        #region 打印导出
        [HttpPost]
        public void SaveToClient(string huId)
        {
            string huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
            string[] checkedOrderArray = huId.Split(',');
            string selectStatement = string.Empty;
            IList<object> selectPartyPara = new List<object>();
            foreach (var para in checkedOrderArray)
            {
                if (selectStatement == string.Empty)
                {
                    selectStatement = "from Hu where HuId in (?";
                }
                else
                {
                    selectStatement += ",?";
                }
                selectPartyPara.Add(para);
            }
            selectStatement += ")";

            IList<Hu> huList = genericMgr.FindAll<Hu>(selectStatement, selectPartyPara.ToArray());
            foreach (var hu in huList)
            {
                if (!string.IsNullOrEmpty(hu.ManufactureParty))
                {
                    hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                }
                if (!string.IsNullOrWhiteSpace(hu.HuTemplate))
                {
                    huTemplate = hu.HuTemplate;
                }

                if (!string.IsNullOrWhiteSpace(hu.Direction))
                {
                    hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                }
            }
            IList<PrintHu> printHu = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
            IList<object> data = new List<object>();
            data.Add(printHu);
            data.Add(CurrentUser.FullName);
            reportGen.WriteToClient(huTemplate, data, huTemplate);
        }

        [HttpPost]
        public void SaveToClientTo(string checkedOrders)
        {
            string defaultHuTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);

            string[] checkedOrderArray = checkedOrders.Split(',');
            string selectStatement = string.Empty;
            IList<object> selectPartyPara = new List<object>();
            foreach (var para in checkedOrderArray)
            {
                if (selectStatement == string.Empty)
                {
                    selectStatement = "from Hu where HuId in (?";
                }
                else
                {
                    selectStatement += ",?";
                }
                selectPartyPara.Add(para);
            }
            selectStatement += ")";

            IList<Hu> huList = genericMgr.FindAll<Hu>(selectStatement, selectPartyPara.ToArray());
            foreach (var hu in huList)
            {
                if (!string.IsNullOrEmpty(hu.ManufactureParty))
                {
                    hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                }
                if (string.IsNullOrWhiteSpace(hu.HuTemplate))
                {
                    hu.HuTemplate = defaultHuTemplate;
                }
                if (!string.IsNullOrWhiteSpace(hu.Direction))
                {
                    hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                }
            }
            var huGroupList = huList.GroupBy(p => p.HuTemplate, (k, g) => new { k, g });
            foreach (var huGroup in huGroupList)
            {
                IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huGroup.g.ToList());
                IList<object> data = new List<object>();
                data.Add(printHuList);
                data.Add(CurrentUser.FullName);
                reportGen.WriteToClient(huGroup.k, data, huGroup.k);
            }
        }

        public string Print(string huId)
        {
            Hu hu = queryMgr.FindById<Hu>(huId);
            if (!string.IsNullOrEmpty(hu.ManufactureParty))
            {
                hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
            }
            if (!string.IsNullOrWhiteSpace(hu.Direction))
            {
                hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
            }
            string huTemplate = hu.HuTemplate;
            if (string.IsNullOrWhiteSpace(huTemplate))
            {
                huTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
            }
            IList<PrintHu> huList = new List<PrintHu>();
            PrintHu printHu = Mapper.Map<Hu, PrintHu>(hu);
            huList.Add(printHu);
            IList<object> data = new List<object>();
            data.Add(huList);
            data.Add(CurrentUser.FullName);
            return reportGen.WriteToFile(huTemplate, data);
        }
        [HttpPost]
        public JsonResult CheckExportTemplate(string checkedOrders)
        {
            string[] checkedOrderArray = checkedOrders.Split(',');
            string selectStatement = string.Empty;
            List<object> huIds = new List<object>();
            foreach (var checkedOrder in checkedOrderArray)
            {
                huIds.Add(checkedOrder);
            }
            var templateCount = genericMgr.FindAllIn<Hu>("from Hu as i where i.HuId in (?", huIds).Select(o => o.HuTemplate).Distinct().Count();
            var message = "OK";
            if (templateCount > 1)
            {
                message = string.Format(Resources.EXT.ControllerLan.Con_SelectedBarcodePrintedTemplatesAreInconsistent);
            }
            return Json(new { Message = message });
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Inventory_Hu_New")]
        public JsonResult _PrintHuList(string checkedOrders)
        {
            string[] checkedOrderArray = checkedOrders.Split(',');
            string selectStatement = string.Empty;
            IList<object> selectPartyPara = new List<object>();
            foreach (var para in checkedOrderArray)
            {
                if (selectStatement == string.Empty)
                {
                    selectStatement = "from Hu where HuId in (?";
                }
                else
                {
                    selectStatement += ",?";
                }
                selectPartyPara.Add(para);
            }
            selectStatement += ")";

            IList<Hu> huList = genericMgr.FindAll<Hu>(selectStatement, selectPartyPara.ToArray());
            var defaultHuTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
            foreach (var hu in huList)
            {
                if (!string.IsNullOrEmpty(hu.ManufactureParty))
                {
                    hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                }

                if (string.IsNullOrWhiteSpace(hu.HuTemplate))
                {
                    hu.HuTemplate = defaultHuTemplate;
                }
                //if (!string.IsNullOrWhiteSpace(hu.Direction))
                //{
                //    hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                //}
            }
            var huGroupList = huList.GroupBy(p => p.HuTemplate, (k, g) => new { k, g });
            List<string> printUrls = new List<string>();
            foreach (var huGroup in huGroupList)
            {
                string reportFileUrl = PrintHuList(huGroup.g.ToList(), huGroup.k);
                printUrls.Add(reportFileUrl);
            }
            object obj = new { SuccessMessage = Resources.EXT.ControllerLan.Con_BarcodePrintedSuccessfully, PrintUrl = printUrls };
            return Json(obj);
        }

        public string PrintHuList(IList<Hu> huList, string huTemplate)
        {
            foreach (var hu in huList)
            {
                if (!string.IsNullOrWhiteSpace(hu.Direction))
                {
                    hu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
                }
            }
            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);

            IList<object> data = new List<object>();
            data.Add(printHuList);
            data.Add(CurrentUser.FullName);
            return reportGen.WriteToFile(huTemplate, data);
        }

        private IList<Hu> GetPartsHu(OrderMaster orderMaster, IList<OrderDetail> orderDetailList)
        {
            IList<Hu> huList = new List<Hu>();
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                int huCount = Convert.ToInt32(orderDetail.HuQty % orderDetail.UnitCount == 0 ? orderDetail.HuQty / orderDetail.UnitCount : (orderDetail.HuQty / orderDetail.UnitCount) + 1);
                var lastHuQty = orderDetail.HuQty % orderDetail.UnitCount;
                for (int i = 0; i < huCount; i++)
                {
                    var item = this.itemMgr.GetCacheItem(orderDetail.Item);
                    Hu hu = new Hu();
                    //hu.HuId = huId;
                    hu.LotNo = orderDetail.LotNo;

                    hu.Qty = (i + 1 == huCount) ? (lastHuQty > 0 ? lastHuQty : orderDetail.UnitCount) : orderDetail.UnitCount;

                    hu.Item = orderDetail.Item;
                    hu.ItemDescription = orderDetail.ItemDescription;
                    hu.BaseUom = orderDetail.BaseUom;
                    hu.Uom = orderDetail.Uom;
                    hu.UnitCount = orderDetail.UnitCount;
                    hu.UnitQty = orderDetail.UnitQty;
                    hu.HuTemplate = orderMaster.HuTemplate;
                    hu.ManufactureParty = orderDetail.ManufactureParty;
                    hu.ManufactureDate = orderDetail.ManufactureDate;
                    hu.PrintCount = 0;
                    hu.ConcessionCount = 0;
                    hu.ReferenceItemCode = orderDetail.ReferenceItemCode;
                    //hu.IsOdd = hu.Qty < hu.UnitCount;
                    hu.IsChangeUnitCount = orderDetail.IsChangeUnitCount;
                    hu.UnitCountDescription = orderDetail.UnitCountDescription;
                    hu.SupplierLotNo = orderDetail.SupplierLotNo;
                    hu.ContainerDesc = orderDetail.ContainerDescription;
                    hu.LocationTo = string.IsNullOrWhiteSpace(orderDetail.LocationTo) ? orderMaster.LocationTo : orderDetail.LocationTo;
                    hu.OrderNo = orderDetail.OrderNo;
                    hu.Shift = orderMaster.Shift;
                    hu.Flow = orderMaster.Flow;

                    hu.MaterialsGroup = GetMaterialsGroupDescrption(item.MaterialsGroup);
                    //hu.HuOption = GetHuOption(item);
                    
                    huList.Add(hu);
                }
            }
            return huList;
        }

        private string GetMaterialsGroupDescrption(string materialsGroupCode)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(materialsGroupCode))
                {
                    var itemCategorys = this.genericMgr.FindAll<string>
                        ("select Description from ItemCategory where Code = ? and SubCategory = ? ",
                        new object[] { materialsGroupCode, (int)CodeMaster.SubCategory.MaterialsGroup },
                        new NHibernate.Type.IType[] { NHibernateUtil.String, NHibernateUtil.Int32 });
                    if (itemCategorys != null && itemCategorys.Count > 0)
                    {
                        return itemCategorys[0];
                    }
                }
            }
            catch (Exception)
            { }
            return materialsGroupCode;
        }
        #endregion

        #endregion

        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, HuSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("HuId", searchModel.HuId, HqlStatementHelper.LikeMatchMode.Anywhere, "h", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("CreateUserName", searchModel.CreateUserName, HqlStatementHelper.LikeMatchMode.Start, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("SupplierLotNo", searchModel.SupplierLotNo, "h", ref whereStatement, param);

            if (searchModel.LotNo != null & searchModel.LotNoTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("LotNo", searchModel.LotNo, searchModel.LotNoTo, "h", ref whereStatement, param);
            }
            else if (searchModel.LotNo != null & searchModel.LotNoTo == null)
            {
                HqlStatementHelper.AddGeStatement("LotNo", searchModel.LotNo, "h", ref whereStatement, param);
            }
            else if (searchModel.LotNo == null & searchModel.LotNoTo != null)
            {
                HqlStatementHelper.AddLeStatement("LotNo", searchModel.LotNoTo, "h", ref whereStatement, param);
            }

            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "h", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "h", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "h", ref whereStatement, param);
            }

            if (searchModel.RemindExpireDate_start != null & searchModel.RemindExpireDate_End != null)
            {
                HqlStatementHelper.AddBetweenStatement("RemindExpireDate", searchModel.RemindExpireDate_start, searchModel.RemindExpireDate_End, "h", ref whereStatement, param);
            }
            else if (searchModel.RemindExpireDate_start != null & searchModel.RemindExpireDate_End == null)
            {
                HqlStatementHelper.AddGeStatement("RemindExpireDate", searchModel.RemindExpireDate_start, "h", ref whereStatement, param);
            }
            else if (searchModel.RemindExpireDate_start == null & searchModel.RemindExpireDate_End != null)
            {
                HqlStatementHelper.AddLeStatement("RemindExpireDate", searchModel.RemindExpireDate_End, "h", ref whereStatement, param);
            }


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        private SearchStatementModel PrepareSearchStatementTime(GridCommand command, HuSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Item", searchModel.HuId, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LocTo", searchModel.Location, "h", ref whereStatement, param);

            //if (searchModel.RemindExpireDate_start != null & searchModel.RemindExpireDate_End != null)
            //{
            //    HqlStatementHelper.AddBetweenStatement("RemindExpireDate", searchModel.RemindExpireDate_start, searchModel.RemindExpireDate_End, "h", ref whereStatement, param);
            //}
            //else if (searchModel.RemindExpireDate_start != null & searchModel.RemindExpireDate_End == null)
            //{
            //    HqlStatementHelper.AddGeStatement("RemindExpireDate", searchModel.RemindExpireDate_start, "h", ref whereStatement, param);
            //}
            //else if (searchModel.RemindExpireDate_start == null & searchModel.RemindExpireDate_End != null)
            //{
            //    HqlStatementHelper.AddLeStatement("RemindExpireDate", searchModel.RemindExpireDate_End, "h", ref whereStatement, param);
            //}
            //else
            //{
            //HqlStatementHelper.AddLeStatement("RemindExpireDate", DateTime.Now, "h", ref whereStatement, param);
            //}
            if (searchModel.StartDate != null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "h", ref whereStatement, param);
            }
            else if (searchModel.StartDate != null & searchModel.EndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "h", ref whereStatement, param);
            }
            else if (searchModel.StartDate == null & searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "h", ref whereStatement, param);
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

        #region private




        private SearchStatementModel PrepareDetailFlowSearchStatement(GridCommand command, FlowDetailSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            DateTime dateTimeNow = DateTime.Now;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "h", ref whereStatement, param);
            if (whereStatement == string.Empty)
            {
                whereStatement = " where (StartDate = null or StartDate <= ?)";
            }
            else
            {
                whereStatement += " and (StartDate = null or StartDate <= ?)";
            }
            param.Add(dateTimeNow);
            whereStatement += " and (EndDate = null or EndDate >= ?)";
            param.Add(dateTimeNow);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountFlowStatement;
            searchStatementModel.SelectStatement = selectFlowStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        private SearchStatementModel PrepareOrderDetailSearchStatement(GridCommand command, string orderNo)
        {

            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            //SecurityHelper.AddPartyFromPermissionStatement(ref whereStatement, "p", "Party", com.Sconit.CodeMaster.OrderType.Procurement, false);
            HqlStatementHelper.AddEqStatement("OrderNo", orderNo, "o", ref whereStatement, param);


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectOrderCountStatement;
            searchStatementModel.SelectStatement = selectOrderStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        private SearchStatementModel PrepareIpDetailSearchStatement(GridCommand command, IpDetailSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IpNo", searchModel.IpNo, "h", ref whereStatement, param);


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectIpCountStatement;
            searchStatementModel.SelectStatement = selectIpStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        private SearchStatementModel PrepareLocationDetailSearchStatement(GridCommand command, IpDetailSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IpNo", searchModel.IpNo, "h", ref whereStatement, param);


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectLocationCountStatement;
            searchStatementModel.SelectStatement = selectLocationStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        #endregion

    }
}
