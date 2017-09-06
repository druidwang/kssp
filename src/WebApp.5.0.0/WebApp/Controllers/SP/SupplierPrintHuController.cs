using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SCM;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using System.Collections;
using com.Sconit.Utility.Report;
using com.Sconit.Entity;
using com.Sconit.PrintModel.INV;
using AutoMapper;
using NHibernate.Criterion;
using com.Sconit.Web.Models.SearchModels.SCM;
using com.Sconit.Web.Models.SearchModels.ORD;
using System;
using com.Sconit.Entity.MRP.MD;

namespace com.Sconit.Web.Controllers.SP
{
    public class SupplierPrintHuController : WebAppBaseController
    {
        private static string selectCountStatement = "select count(*) from Hu as h";
        private static string selectStatement = "select h from Hu as h";
        private static string selectCountFlowStatement = "select count(*) from FlowDetail as h";
        private static string selectFlowStatement = "select h from FlowDetail as h";
        private static string selectIpCountStatement = "select count(*) from IpDetail as h";
        private static string selectIpStatement = "select h from IpDetail as h";
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
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult New()
        {
            TempData["FlowDetailSearchModel"] = null;
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult List(GridCommand command, HuSearchModel searchModel)
        {

            TempData["HuSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult _AjaxList(GridCommand command, HuSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Hu>(searchStatementModel, command));
        }

        #region FlowMaster

        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult FlowMaster()
        {
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult _FlowDetailList(GridCommand command, FlowDetailSearchModel searchModel)
        {
            TempData["FlowDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult _AjaxFlowDetailList(GridCommand command, FlowDetailSearchModel searchModel)
        {
            try
            {
                FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(searchModel.Flow);
                if (!Utility.SecurityHelper.HasPermission(flowMaster, true))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_LackTheFlowPermission, flowMaster.Code);
                }
                SearchStatementModel searchStatementModel = PrepareDetailFlowSearchStatement(command, searchModel);
                GridModel<FlowDetail> List = GetAjaxPageData<FlowDetail>(searchStatementModel, command);
                if (!string.IsNullOrEmpty(searchModel.Flow))
                {
                    foreach (FlowDetail flowDetail in List.Data)
                    {
                        flowDetail.ManufactureParty = flowMaster.PartyFrom;
                        flowDetail.LotNo = LotNoHelper.GenerateLotNo();
                        Item item = genericMgr.FindById<Item>(flowDetail.Item);
                        flowDetail.ItemDescription = item.Description;
                    }
                }
                return PartialView(List);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return PartialView(new GridModel(new List<FlowDetail>()));
            }
        }

        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
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
                                var ucDeviation = flowMaster.UcDeviation;
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
                        if (FlowCheckExport)
                        {
                            var flowCheckExport = string.Format("OK");
                            return Json(new { FlowCheckExport = flowCheckExport });
                        }
                        IList<Hu> huList = huMgr.CreateHu(flowMaster, nonZeroFlowDetailList);
                        if (FlowisExport)
                        {

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
                            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
                            IList<object> data = new List<object>();
                            data.Add(printHuList);
                            data.Add(CurrentUser.FullName);
                            reportGen.WriteToClient(flowMaster.HuTemplate, data, flowMaster.HuTemplate);
                            return Json(null);
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


        #endregion

        #region  IpMaster
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult IpMaster()
        {
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult IpDetailList(GridCommand command, IpDetailSearchModel searchModel)
        {
            TempData["IpDetailSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult _AjaxIpDetailList(GridCommand command, IpDetailSearchModel searchModel)
        {
            try
            {
                var ipmaster = genericMgr.FindById<IpMaster>(searchModel.IpNo);
                if (!Utility.SecurityHelper.HasPermission(ipmaster, true))
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
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return PartialView(new GridModel(new List<IpDetail>()));
            }
        }

        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
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
                                var ucDeviation = flowMaster.UcDeviation;
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
                        if (IpDetailisCheckExport)
                        {
                            var ipDetailisCheckExport = string.Format("OK");
                            return Json(new { IpDetailisCheckExport = ipDetailisCheckExport });
                        }
                        IList<Hu> huList = huMgr.CreateHu(ipMaster, nonZeroIpDetailList);
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
                            reportGen.WriteToClient(ipMaster.HuTemplate, data, ipMaster.HuTemplate);
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

        #endregion

        #region OrderMaster

        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult OrderMaster()
        {
            return PartialView();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult OrderDetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            ViewBag.OrderNo = searchModel.OrderNo;
            TempData["OrderMasterSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public ActionResult _AjaxOrderDetailList(GridCommand command, OrderMasterSearchModel searchModel)
        {
            try
            {
                com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
                var orderMaster = this.genericMgr.FindById<OrderMaster>(searchModel.OrderNo);
                if (!Utility.SecurityHelper.HasPermission(orderMaster, true))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_LackTheOrderNumberPermission, searchModel.OrderNo);
                }

                var orderDetailList = this.genericMgr.FindAll<OrderDetail>
                 (" from OrderDetail where OrderNo = ?", searchModel.OrderNo);

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    orderDetail.ManufactureParty = orderMaster.PartyFrom;
                    orderDetail.HuQty = orderDetail.OrderedQty;
                    orderDetail.LotNo = LotNoHelper.GenerateLotNo();
                    if (!orderDetail.IsChangeUnitCount)
                    {
                        orderDetail.HuQty = Math.Ceiling(orderDetail.HuQty / orderDetail.UnitCount) * orderDetail.UnitCount;
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

        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public JsonResult CreateHuByOrderDetail(string OrderDetailidStr, string OrderDetailucStr, string OrderDetailsupplierLotNoStr,
            string OrderDetailqtyStr, bool OrderDetailisExport, string OrderDetailimanufactureDateStr)
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
                    if (string.IsNullOrEmpty(orderMaster.HuTemplate))
                    {
                        orderMaster.HuTemplate = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
                    }

                    if (orderMaster != null)
                    {
                        IList<Hu> huList = huMgr.CreateHu(orderMaster, nonZeroOrderDetailList);
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
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }


        private SearchStatementModel PrepareOrderDetailSearchStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("OrderNo", searchModel.OrderNo, "o", ref whereStatement, param);


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectOrderCountStatement;
            searchStatementModel.SelectStatement = selectOrderStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        #endregion

        #region 打印
        public string PrintHuList(IList<Hu> huList, string huTemplate)
        {

            IList<PrintHu> printHuList = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);

            IList<object> data = new List<object>();
            data.Add(printHuList);
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
            IList<Hu> nulltemplates = genericMgr.FindAllIn<Hu>("from Hu as i where i.HuId in (?", huIds).Where(p => string.IsNullOrWhiteSpace(p.HuTemplate)).ToList();
            var templateCount = genericMgr.FindAllIn<Hu>("from Hu as i where i.HuId in (?", huIds).Select(o => o.HuTemplate).Distinct().Count();
            var message = "OK";
            if (templateCount > 1)
            {
                message = string.Format(Resources.EXT.ControllerLan.Con_SelectedBarcodePrintedTemplatesAreInconsistent);
            }
            if (nulltemplates.Count() > 0)
            {
                message = string.Join(",", nulltemplates.Select(p => p.HuId))+" no template information.";
            }
            return Json(new { Message = message });
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Supplier_Print_ADD")]
        public JsonResult _PrintHus(string checkedOrders)
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
            foreach (var hu in huList)
            {
                if (!string.IsNullOrEmpty(hu.ManufactureParty))
                {
                    hu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
                }
            }
            //string template = "BarCodePurchase2D.xls";//systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);

            List<string> reportFileUrls = new List<string>();

            var huGroups = huList.GroupBy(p => p.HuTemplate);
            foreach (var huGroup in huGroups)
            {
                string reportFileUrl = PrintHuList(huGroup.ToList(), huGroup.Key);
                reportFileUrls.Add(reportFileUrl);
            }

            object obj = new { SuccessMessage = Resources.EXT.ControllerLan.Con_BarcodePrintedSuccessfully, PrintUrl = reportFileUrls };
            return Json(obj);
        }

        public void _ExportHus(string checkedOrders)
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

            string updateStatement = string.Empty;
            IList<object> updatePartyPara = new List<object>();
            foreach (var para in checkedOrderArray)
            {
                if (updateStatement == string.Empty)
                {
                    updateStatement = "update Hu set PrintCount=PrintCount+1 where Huid in (?";
                }
                else
                {
                    updateStatement += ",?";
                }
                updatePartyPara.Add(para);
            }
            updateStatement += ")";

            genericMgr.Update(updateStatement, updatePartyPara.ToArray());

            string huTemplate = huList.First().HuTemplate;
            IList<PrintHu> printHu = Mapper.Map<IList<Hu>, IList<PrintHu>>(huList);
            IList<object> data = new List<object>();
            data.Add(printHu);
            data.Add(CurrentUser.FullName);
            reportGen.WriteToClient(huTemplate, data, huTemplate);
        }

        public string Print(string huId, string huTemplate)
        {
            Hu hu = queryMgr.FindById<Hu>(huId);
            IList<PrintHu> huList = new List<PrintHu>();

            PrintHu printHu = Mapper.Map<Hu, PrintHu>(hu);

            if (!string.IsNullOrEmpty(hu.ManufactureParty))
            {
                printHu.ManufacturePartyDescription = queryMgr.FindById<Party>(hu.ManufactureParty).Name;
            }
            huList.Add(printHu);
            IList<object> data = new List<object>();
            data.Add(huList);
            data.Add(CurrentUser.FullName);
            return reportGen.WriteToFile(huTemplate, data);
        }

        #endregion

        #endregion

        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, HuSearchModel searchModel)
        {
            string whereStatement = " where Qty >0 ";

            com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("HuId", searchModel.HuId, HqlStatementHelper.LikeMatchMode.Start, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "h", ref whereStatement, param);

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
            HqlStatementHelper.AddEqStatement("ManufactureParty", searchModel.ManufactureParty, "h", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("CreateUserId", user.Id, "h", ref whereStatement, param);
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
        #endregion
    }
}
