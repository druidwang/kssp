using System.Data;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.SI;
using System.Data.SqlClient;
using System;
using com.Sconit.Entity.Exception;
using com.Sconit.Service.SI.SAP;
using com.Sconit.Web.Models;
using com.Sconit.Entity.SI.SAP;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity.SYS;
using System.Web.Routing;
using com.Sconit.Service;
using NHibernate.Type;
using NHibernate;
using com.Sconit.Entity;
using com.Sconit.Entity.INV;
using System.Text;

namespace com.Sconit.Web.Controllers.SI
{
    public class SAPInterfaceController : WebAppBaseController
    {
        #region   hql
        // SAPTransferLog
        private static string selectCountStatement = "select count(*) from SAPTransferLog as s";

        private static string selectStatement = "select s from SAPTransferLog as s";

        //SAPItem
        private static string selectSAPItemCountStatement = "select count(*) from SAPItem as s";

        private static string selectSAPItemStatement = "select s from SAPItem as s";

        //SAPBom
        private static string selectSAPBomCountStatement = "select count(*) from SAPBom as s";

        private static string selectSAPBomstatement = "select s from SAPBom as s";

        //  SAPUomConvertion
        private static string selectSAPUomConvertionCountStatement = "select count(*) from SAPUomConvertion as s";

        private static string selectSAPUomConvertionstatement = "select s from SAPUomConvertion as s";

        //SAPPriceList
        private static string selectSAPPriceListCountStatement = "select count(*) from SAPPriceList as s";

        private static string selectSAPPriceListstatement = "select s from SAPPriceList as s";


        //SAPSupplier
        private static string selectSAPSupplierCountStatement = "select count(*) from SAPSupplier as s";

        private static string selectSAPSupplierstatement = "select s from SAPSupplier as s";

        //SAPCustomer
        private static string selectSAPCustomerCountStatement = "select count(*) from SAPCustomer as s";

        private static string selectSAPCustomerstatement = "select s from SAPCustomer as s";

        //SAPSDNormal
        private static string selectSAPSDNormalCountStatement = "select count(*) from SAPSDMES0001 as s";

        private static string selectSAPSDNormalstatement = "select s from SAPSDMES0001 as s";

        //SAPSDCancel
        private static string selectSAPSDCancelCountStatement = "select count(*) from SAPSDMES0002 as s";

        private static string selectSAPSDCancelstatement = "select s from SAPSDMES0002 as s";

        //SAPPPMES0001
        private static string selectSAPPPMES0001CountStatement = "select count(*) from SAPPPMES0001 as s";

        private static string selectSAPPPMES0001statement = "select s from SAPPPMES0001 as s";

        //SAPPPMES0002
        private static string selectSAPPPMES0002CountStatement = "select count(*) from SAPPPMES0002 as s";

        private static string selectSAPPPMES0002statement = "select s from SAPPPMES0002 as s";

        //SAPPPMES0003
        private static string selectSAPPPMES0003CountStatement = "select count(*) from SAPPPMES0003 as s";

        private static string selectSAPPPMES0003statement = "select s from SAPPPMES0003 as s";

        //SAPPPMES0004
        private static string selectSAPPPMES0004CountStatement = "select count(*) from SAPPPMES0004 as s";

        private static string selectSAPPPMES0004statement = "select s from SAPPPMES0004 as s";

        //SAPPPMES0005
        private static string selectSAPPPMES0005CountStatement = "select count(*) from SAPPPMES0005 as s";

        private static string selectSAPPPMES0005statement = "select s from SAPPPMES0005 as s";

        //SAPPPMES0006
        private static string selectSAPPPMES0006CountStatement = "select count(*) from SAPPPMES0006 as s";

        private static string selectSAPPPMES0006statement = "select s from SAPPPMES0006 as s";

        //SAPMMMES0001
        private static string selectSAPMMMES0001CountStatement = "select count(*) from SAPMMMES0001 as s";

        private static string selectSAPMMMES0001statement = "select s from SAPMMMES0001 as s";

        //SAPMMMES0002
        private static string selectSAPMMMES0002CountStatement = "select count(*) from SAPMMMES0002 as s";

        private static string selectSAPMMMES0002statement = "select s from SAPMMMES0002 as s";

        //SAPSTMES0001
        private static string selectSAPSTMES0001CountStatement = "select count(*) from SAPSTMES0001 as s";

        private static string selectSAPSTMES0001statement = "select s from SAPSTMES0001 as s";
        #endregion

        public IMasterDataMgr masterDataMgr { get; set; }
        public ISalesDistributionMgr salesDistributionMgr { get; set; }
        public ISAPInterfaceCommonMgr sapInterfaceCommonMgr { get; set; }
        public IMaterialManagementMgr materialManagementMgr { get; set; }
        public IProductionPlanningMgr productionPlanningMgr { get; set; }
        public ISAPInterfaceCommonMgr iSAPInterfaceCommonMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public SAPInterfaceController()
        {

        }
        
        [SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index1()
        {
            return View();
        }

        #region   ReCall
        #region  item
        [SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        public JsonResult _ManualCallItem(string itemCode, DateTime? reqBeginDate)
        {
            try
            {
                bool isLoadAll = false;
                if (string.IsNullOrEmpty(itemCode) && reqBeginDate == null)
                {
                    isLoadAll = true;
                }
                var messages = masterDataMgr.GetSAPItem(itemCode, reqBeginDate, isLoadAll);
                if (messages.Count == 0)
                {
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ExecuteSuccessfully);
                }
                else
                {
                    string errorMsg = string.Empty;
                    foreach (var message in messages)
                    {
                        errorMsg = (string.IsNullOrEmpty(errorMsg) ? "" : errorMsg + ",") + message.Message;
                    }
                    SaveErrorMessage(errorMsg);
                }
                return Json(new { });
            }
            catch (BusinessException be)
            {
                SaveBusinessExceptionMessage(be);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return Json(null);
        }
        #endregion

        #region  Bom
        [SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        public JsonResult _ManualCallBom(string bomCode, DateTime? reqBeginDate)
        {
            try
            {
                bool isLoadAll = false;
                if (string.IsNullOrEmpty(bomCode) && reqBeginDate == null)
                {
                    isLoadAll = true;
                }
                var messages = masterDataMgr.GetSAPBom(bomCode, reqBeginDate, isLoadAll);
                if (messages.Count == 0)
                {
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ExecuteSuccessfully);
                }
                else
                {
                    string errorMsg = string.Empty;
                    foreach (var message in messages)
                    {
                        errorMsg = (string.IsNullOrEmpty(errorMsg) ? "" : errorMsg + ",") + message.Message;
                    }
                    SaveErrorMessage(errorMsg);
                }
                return Json(new { });
            }
            catch (BusinessException be)
            {
                SaveBusinessExceptionMessage(be);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return Json(null);
        }
        #endregion

        #region  UomConvert
        [SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        public JsonResult _ManualCallUomConv(string itemCode, DateTime? reqBeginDate)
        {
            try
            {
                bool isLoadAll = false;
                if (string.IsNullOrEmpty(itemCode) && reqBeginDate == null)
                {
                    isLoadAll = true;
                }
                var messages = masterDataMgr.GetSAPUomConv(itemCode, reqBeginDate, isLoadAll);
                if (messages.Count == 0)
                {
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ExecuteSuccessfully);
                }
                else
                {
                    string errorMsg = string.Empty;
                    foreach (var message in messages)
                    {
                        errorMsg = (string.IsNullOrEmpty(errorMsg) ? "" : errorMsg + ",") + message.Message;
                    }
                    SaveErrorMessage(errorMsg);
                }
                return Json(new { });
            }
            catch (BusinessException be)
            {
                SaveBusinessExceptionMessage(be);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return Json(null);
        }
        #endregion

        #region  PriceList
        [SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        public JsonResult _ManualCallPriceList(string itemCode, string supplierCode, DateTime? reqBeginDate)
        {
            try
            {
                bool isLoadAll = false;
                if (string.IsNullOrEmpty(itemCode) && string.IsNullOrEmpty(supplierCode) && reqBeginDate == null)
                {
                    isLoadAll = true;
                }
                var messages = masterDataMgr.GetSAPPriceList(itemCode, supplierCode, reqBeginDate, isLoadAll);
                if (messages.Count == 0)
                {
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ExecuteSuccessfully);
                }
                else
                {
                    string errorMsg = string.Empty;
                    foreach (var message in messages)
                    {
                        errorMsg = (string.IsNullOrEmpty(errorMsg)?"":errorMsg+",") + message.Message;
                    }
                    SaveErrorMessage(errorMsg);
                }
                return Json(new { });
            }
            catch (BusinessException be)
            {
                SaveBusinessExceptionMessage(be);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return Json(null);
        }
        #endregion

        #region  Supplier
        [SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        public JsonResult _ManualCallSupplier(string supplierCode, DateTime? reqBeginDate)
        {
            try
            {
                bool isLoadAll = false;
                if (string.IsNullOrEmpty(supplierCode) && reqBeginDate == null)
                {
                    isLoadAll = true;
                }
                var messages = masterDataMgr.GetSAPSupplier(supplierCode, reqBeginDate, isLoadAll);
                if (messages.Count == 0)
                {
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ExecuteSuccessfully);
                }
                else
                {
                    string errorMsg = string.Empty;
                    foreach (var message in messages)
                    {
                        errorMsg = (string.IsNullOrEmpty(errorMsg) ? "" : errorMsg + ",") + message.Message;
                    }
                    SaveErrorMessage(errorMsg);
                }
                return Json(new { });
            }
            catch (BusinessException be)
            {
                SaveBusinessExceptionMessage(be);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return Json(null);
        }
        #endregion

        #region  Customer
        [SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        public JsonResult _ManualCallCustomer(string customerCode, DateTime? reqBeginDate)
        {
            try
            {
                bool isLoadAll = false;
                if (string.IsNullOrEmpty(customerCode) && reqBeginDate == null)
                {
                    isLoadAll = true;
                }
                var messages = masterDataMgr.GetSAPCustomer(customerCode, reqBeginDate, isLoadAll);
                if (messages.Count == 0)
                {
                    SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ExecuteSuccessfully);
                }
                else
                {
                    string errorMsg = string.Empty;
                    foreach (var message in messages)
                    {
                        errorMsg = (string.IsNullOrEmpty(errorMsg) ? "" : errorMsg + ",") + message.Message;
                    }
                    SaveErrorMessage(errorMsg);
                }
                return Json(new { });
            }
            catch (BusinessException be)
            {
                SaveBusinessExceptionMessage(be);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return Json(null);
        }
        #endregion

        //#region  Customer
        //[SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        //public JsonResult _ManualCallBusiDataNormal()
        //{
        //    try
        //    {
        //        //var messages = iSAPInterfaceCommonMgr.GenBusinessOrderData();
        //        //if (messages.Count == 0)
        //        //{
        //        //    SaveSuccessMessage("调用成功。");
        //        //}
        //        //else
        //        //{
        //        //    string errorMsg = string.Empty;
        //        //    foreach (var message in messages)
        //        //    {
        //        //        errorMsg = (string.IsNullOrEmpty(errorMsg) ? "" : errorMsg + ",") + message.Message;
        //        //    }
        //        //    SaveErrorMessage(errorMsg);
        //        //}
        //        return Json(new { });
        //    }
        //    catch (BusinessException be)
        //    {
        //        SaveBusinessExceptionMessage(be);
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveErrorMessage(ex.Message);
        //    }
        //    return Json(null);
        //}
        //#endregion

        //#region  SDNormal
        //[SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        //public JsonResult _ManualCallSDNormal()
        //{
        //    try
        //    {
        //        var messages = sapInterfaceCommonMgr.GenMESData(Guid.NewGuid().ToString().Replace("-", ""), BusinessConstants.SAPBUSINESSDATA_SDORDER);
        //        messages.AddRange(salesDistributionMgr.ExportSDMES0001());
        //        messages.AddRange(salesDistributionMgr.ExportSDMES0002());
        //        if (messages.Count == 0)
        //        {
        //            SaveSuccessMessage("调用成功。");
        //        }
        //        else
        //        {
        //            string errorMsg = string.Empty;
        //            foreach (var message in messages)
        //            {
        //                errorMsg = (string.IsNullOrEmpty(errorMsg) ? "" : errorMsg + ",") + message.Message;
        //            }
        //            SaveErrorMessage(errorMsg);
        //        }

        //        return Json(new { });
        //    }
        //    catch (BusinessException be)
        //    {
        //        SaveBusinessExceptionMessage(be);
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveErrorMessage(ex.Message);
        //    }
        //    return Json(null);
        //}
        //#endregion

        //#region  MM
        //[SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        //public JsonResult _ManualCallMM()
        //{
        //    try
        //    {
        //        var messages = sapInterfaceCommonMgr.GenMESData(Guid.NewGuid().ToString().Replace("-", ""), BusinessConstants.SAPBUSINESSDATA_SYSTEMCODE);
        //        messages.AddRange(materialManagementMgr.ExportMMMES0001Data());
        //        messages.AddRange(materialManagementMgr.ExportMMMES0002Data());
        //        messages.AddRange(materialManagementMgr.ExportSTMES0001Data());
        //        if (messages.Count == 0)
        //        {
        //            SaveSuccessMessage("调用成功。");
        //        }
        //        else
        //        {
        //            string errorMsg = string.Empty;
        //            foreach (var message in messages)
        //            {
        //                errorMsg = (string.IsNullOrEmpty(errorMsg) ? "" : errorMsg + ",") + message.Message;
        //            }
        //            SaveErrorMessage(errorMsg);
        //        }

        //        return Json(new { });
        //    }
        //    catch (BusinessException be)
        //    {
        //        SaveBusinessExceptionMessage(be);
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveErrorMessage(ex.Message);
        //    }
        //    return Json(null);
        //}
        //#endregion

        //#region  PP
        //[SconitAuthorize(Permissions = "Url_SI_SAPInterface")]
        //public JsonResult _ManualCallPP()
        //{
        //    try
        //    {
        //        var messages = sapInterfaceCommonMgr.GenMESData(Guid.NewGuid().ToString().Replace("-", ""), BusinessConstants.SAPBUSINESSDATA_PPORDER);
        //        messages.AddRange(productionPlanningMgr.ExportPPMES0001Data());
        //        messages.AddRange(productionPlanningMgr.ExportPPMES0002Data());
        //        messages.AddRange(productionPlanningMgr.ExportPPMES0003Data());
        //        messages.AddRange(productionPlanningMgr.ExportPPMES0004Data());
        //        messages.AddRange(productionPlanningMgr.ExportPPMES0005Data());
        //        messages.AddRange(productionPlanningMgr.ExportPPMES0006Data());
        //        if (messages.Count == 0)
        //        {
        //            SaveSuccessMessage("调用成功。");
        //        }
        //        else
        //        {
        //            string errorMsg = string.Empty;
        //            foreach (var message in messages)
        //            {
        //                errorMsg = (string.IsNullOrEmpty(errorMsg) ? "" : errorMsg + ",") + message.Message;
        //            }
        //            SaveErrorMessage(errorMsg);
        //        }

        //        return Json(new { });
        //    }
        //    catch (BusinessException be)
        //    {
        //        SaveBusinessExceptionMessage(be);
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveErrorMessage(ex.Message);
        //    }
        //    return Json(null);
        //}
        //#endregion

        #region ManualCallBusiDataNormal
        public ActionResult _ManualCallBusiDataNormal(DateTime? selectedTime,int callType)
        {
            string spName = string.Empty;
            var result = new object[] { };
            DateTime curDate = genericMgr.FindAllWithNativeSql<DateTime>("Select GetDate()").FirstOrDefault();
            var batchNo = Guid.NewGuid().ToString().Replace("-", "");
            try
            {
                if (selectedTime.HasValue && selectedTime < curDate)
                {
                    curDate = selectedTime.Value;
                }
                else
                {
                    curDate = iSAPInterfaceCommonMgr.GenMesQtyData();
                }
                if (callType == 6)
                {
                    iSAPInterfaceCommonMgr.GenBusinessOrderData(curDate);
                }
                else if (callType == 7)
                {
                    iSAPInterfaceCommonMgr.GenBusinessAdjustOrderData(curDate);
                }
                else if (callType == 8)
                {
                    iSAPInterfaceCommonMgr.GenBusinessAdjustTailOrderData(curDate);
                }
                iSAPInterfaceCommonMgr.TransBusinessOrderData();
                //返回传输成功的行数
                string rMessage = genericMgr.FindAllWithNativeSql<string>("USP_SAP_ReturnRowCount").FirstOrDefault();
                rMessage = Resources.EXT.ControllerLan.Con_ExecuteSuccessfully + rMessage;
                SaveSuccessMessage(rMessage);
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_ExecuteUnsuccessfully);
            }
            
            return Json(null);
        }
        #endregion

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ReHandle(string batchNo, string interfaceCode)
        {
            try
            {
                SAPTransferLog transLog = this.genericMgr.FindAll<SAPTransferLog>("from SAPTransferLog where BatchNo=? and Interface=?",new object[]{batchNo,interfaceCode}).FirstOrDefault();
                string logMessage = string.Empty;
                if (interfaceCode == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_ITEM)
                {
                    var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessItem", new object[] { this.CurrentUser.Id, batchNo }).FirstOrDefault();
                    if ((int)result[0] != 1)
                    {
                        logMessage = Resources.EXT.ControllerLan.Con_ReProcessSAPMESItemUnsuccessfully + result[1].ToString() + "，"+Resources.EXT.ControllerLan.Con_FailureInformation+"：" + result[2].ToString(); 
                    }
                }
                else if (interfaceCode == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_BOM)
                {
                    var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessBom", new object[] { CurrentUser.Id, batchNo, false }).FirstOrDefault();
                    if ((int)result[0] != 1)
                    {
                        logMessage = Resources.EXT.ControllerLan.Con_ReProcessSAPMESBOMUnsuccessfully + result[1].ToString() + "，" + Resources.EXT.ControllerLan.Con_FailureInformation + "：" + result[2].ToString(); 
                    }
                }
                else if (interfaceCode == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_UOMCONV)
                {
                    var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessUomConv", new object[] { CurrentUser.Id, batchNo,false }).FirstOrDefault();
                    if ((int)result[0] != 1)
                    {
                        logMessage = Resources.EXT.ControllerLan.Con_ReProcessSAPMESUomTrandformUnsuccessfully + result[1].ToString() + "，" + Resources.EXT.ControllerLan.Con_FailureInformation + "：" + result[2].ToString(); 
                    }
                }
                else if (interfaceCode == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_PRICELIST)
                {
                    var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessPriceList", new object[] { CurrentUser.Id, batchNo }).FirstOrDefault();
                    if ((int)result[0] != 1)
                    {
                        logMessage = Resources.EXT.ControllerLan.Con_ReProcessSAPMESPriceListOrderUnsuccessfully + result[1].ToString() + "，" + Resources.EXT.ControllerLan.Con_FailureInformation + "：" + result[2].ToString(); 
                    }
                }
                else if (interfaceCode == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_SUPPLIER)
                {
                    var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessSupplier", new object[] { CurrentUser.Id, batchNo }).FirstOrDefault();
                    if ((int)result[0] != 1)
                    {
                        logMessage = Resources.EXT.ControllerLan.Con_ReProcessSAPMESSupplierUnsuccessfully + result[1].ToString() + "，" + Resources.EXT.ControllerLan.Con_FailureInformation + "：" + result[2].ToString(); 
                    }
                }
                else if (interfaceCode == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_CUSTOMER)
                {
                    var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessCustomer", new object[] { CurrentUser.Id, batchNo }).FirstOrDefault();
                    if ((int)result[0] != 1)
                    {
                        logMessage = Resources.EXT.ControllerLan.Con_ReProcessSAPMESCustomerUnsuccessfully + result[1].ToString() + "，" + Resources.EXT.ControllerLan.Con_FailureInformation + "：" + result[2].ToString(); 
                    }
                }
                else if (interfaceCode == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_SALESORDER_NORMAL)
                {
                    var errors = salesDistributionMgr.ExportSDMES0001();
                }
                else if (interfaceCode == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_SALESORDER_CANCEL)
                {
                    var errors = salesDistributionMgr.ExportSDMES0002();
                }

                if (string.IsNullOrEmpty(logMessage))
                {
                    transLog.Status = 1;
                    transLog.ErrorMsg = string.Empty;
                    this.genericMgr.Update(transLog);
                }
                return new RedirectToRouteResult(new RouteValueDictionary  
                                                       { 
                                                           { "action", "SAPTransferLogList" }, 
                                                           { "controller", "SAPInterface" },
                                                           {"successMessage",string.IsNullOrEmpty(logMessage)? Resources.EXT.ControllerLan.Con_ReprocessSuccessfully:string.Empty},
                                                           {"errorMessage",logMessage}                                          
                                                       });
            }
            catch (BusinessException ex)
            {                                                                                                              
                SaveBusinessExceptionMessage(ex);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        SaveErrorMessage(ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        SaveErrorMessage(ex.InnerException.Message);
                    }
                }
                else
                {
                    SaveErrorMessage(ex.Message);
                }
            }
            return Json(null);
        }


        private void SaveTransferLog(string batchNo, string errorMsg, string interf, string sysCode, int status)
        {
            SAPTransferLog transLog = new SAPTransferLog();
            transLog.BatchNo = batchNo;
            transLog.Interface = interf;
            transLog.Status = status;
            transLog.SysCode = sysCode;
            transLog.TransDate = DateTime.Now;
            transLog.ErrorMsg = errorMsg;
            this.genericMgr.Create(transLog);
        }
        #endregion

        #region    SAPTransferLog
        public ActionResult SAPTransferLogIndex()
        {
            return View();
        }
        public ActionResult SITransferLogIndex()
        {
            return View();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPTransferLog_View")]
        public ActionResult SAPTransferLogList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_SITransferLog_View")]
        public ActionResult SITransferLogList(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPTransferLog_View")]
        public ActionResult _AjaxList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            if (!string.IsNullOrWhiteSpace(searchModel.successMessage))
            {
                SaveSuccessMessage(searchModel.successMessage);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.errorMessage))
            {
                SaveSuccessMessage(searchModel.errorMessage);
            }
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            GridModel<SAPTransferLog> gridModel = GetAjaxPageData<SAPTransferLog>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = " where 1=1 ";

            IList<object> param = new List<object>();

            if (!string.IsNullOrWhiteSpace(searchModel.MultiInterfaces))
            {
                string interfaceSql = " and s.Interface in( ";
                string[] interfaceArr = searchModel.MultiInterfaces.Split(',');
                for (int st = 0; st < interfaceArr.Length; st++)
                {
                    interfaceSql += "'" + interfaceArr[st] + "',";
                }
                whereStatement += interfaceSql.Substring(0, interfaceSql.Length - 1) + ")";
            }
            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("BatchNo", searchModel.BatchNo, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("TransDate", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("TransDate", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("TransDate", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement="";
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by TransDate desc";
            }
            else
            {
                sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public ActionResult GetInterfaceDetail(string sapInterface, string batchNo)
        {
            string actionName = string.Empty;
            if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_ITEM)
            {
                actionName = "_GetSAPItemGrid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_BOM)
            {
                actionName = "_GetSapBomGrid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_UOMCONV)
            {

                actionName = "_GetSapUomComvGrid";
                //return GetSapUomComvGrid(batchNo);
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_PRICELIST)
            {
                actionName = "_GetSapPriceListGrid";
                //return GetSapPriceListGrid(batchNo);
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_SUPPLIER)
            {
                actionName = "_GetSapSupplierGrid";
                //return GetSapSupplierGrid(batchNo);
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPMASTERDATA_CUSTOMER)
            {
                actionName = "_GetSapCutomerGrid";
                //return GetSapCutomerGrid(batchNo);
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_SALESORDER_NORMAL)
            {
                actionName = "_GetSapSDNormalGrid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_SALESORDER_CANCEL)
            {
                actionName = "_GetSapSDCancelGrid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_PPORDER_0001)
            {
                actionName = "_GetSapPPMES0001Grid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_PPORDER_0002)
            {
                actionName = "_GetSapPPMES0002Grid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_PPORDER_0003)
            {
                actionName = "_GetSapPPMES0003Grid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_PPORDER_0004)
            {
                actionName = "_GetSapPPMES0004Grid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_PPORDER_0005)
            {
                actionName = "_GetSapPPMES0005Grid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_PPORDER_0006)
            {
                actionName = "_GetSapPPMES0006Grid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_MMORDER_0001)
            {
                actionName = "_GetSapMMMES0001Grid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_MMORDER_0002)
            {
                actionName = "_GetSapMMMES0002Grid";
            }
            else if (sapInterface == com.Sconit.Entity.BusinessConstants.SAPBUSINESSDATA_STORDER_0001)
            {
                actionName = "_GetSapSTMES0001Grid";
            }
            if (string.IsNullOrWhiteSpace(actionName))
            {
                return Json(null);
            }
            return new RedirectToRouteResult(new RouteValueDictionary  
                                                       { 
                                                           { "action", actionName}, 
                                                           { "controller", "SAPInterface" },
                                                           {"batchNo",batchNo}
                                                       });
        }

        public ActionResult _GetSAPItemGrid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPItem> sapItemList = this.genericMgr.FindAll<SAPItem>("select s from SAPItem as s where s.BatchNo=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling((decimal)(sapItemList.Count() / value));
            ViewBag.BatchNo = batchNo;
            ViewBag.PageSize = value;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPItem_View")]
        public ActionResult _AjaxItemBatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareItemSearchStatement(command, searchModel);
            GridModel<SAPItem> gridModel = GetAjaxPageData<SAPItem>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        public ActionResult _GetSapBomGrid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPBom> sapBomList = this.genericMgr.FindAll<SAPBom>("select s from SAPBom as s where s.BatchNo=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling((decimal)(sapBomList.Count() / value));
            ViewBag.BatchNo = batchNo;
            ViewBag.PageSize = value;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPBom_View")]
        public ActionResult _AjaxBomBatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareBomSearchStatement(command, searchModel);
            GridModel<SAPBom> gridModel = GetAjaxPageData<SAPBom>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        public ActionResult _GetSapUomComvGrid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPUomConvertion> sapUomComvList = this.genericMgr.FindAll<SAPUomConvertion>("select s from SAPUomConvertion as s where s.BatchNo=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling((decimal)(sapUomComvList.Count() / value));
            ViewBag.BatchNo = batchNo;
            ViewBag.PageSize = value;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPUomConvertion_View")]
        public ActionResult _AjaxUomConvBatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareUomConvertionSearchStatement(command, searchModel);
            GridModel<SAPUomConvertion> gridModel = GetAjaxPageData<SAPUomConvertion>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        public ActionResult _GetSapPriceListGrid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPPriceList> sapPriceListList = this.genericMgr.FindAll<SAPPriceList>("select s from SAPPriceList as s where s.BatchNo=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling((decimal)(sapPriceListList.Count() / value));
            ViewBag.BatchNo = batchNo;
            ViewBag.PageSize = value;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPPriceList_View")]
        public ActionResult _AjaxPriceListBatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPPriceListSearchStatement(command, searchModel);
            GridModel<SAPPriceList> gridModel = GetAjaxPageData<SAPPriceList>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        public ActionResult _GetSapSupplierGrid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPSupplier> sapSupplierList = this.genericMgr.FindAll<SAPSupplier>("select s from SAPSupplier as s where s.BatchNo=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling((decimal)(sapSupplierList.Count() / value));
            ViewBag.BatchNo = batchNo;
            ViewBag.PageSize = value;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPSupplier_View")]
        public ActionResult _AjaxSupplierBatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPSupplierSearchStatement(command, searchModel);
            GridModel<SAPSupplier> gridModel = GetAjaxPageData<SAPSupplier>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        public ActionResult _GetSapCutomerGrid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPCustomer> sapScutomerList = this.genericMgr.FindAll<SAPCustomer>("select s from SAPCustomer as s where s.BatchNo=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)sapScutomerList.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPCustomer_View")]
        public ActionResult _AjaxCustomerBatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPCustomerSearchStatement(command, searchModel);
            GridModel<SAPCustomer> gridModel = GetAjaxPageData<SAPCustomer>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        public ActionResult _GetSapSDNormalGrid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPSDMES0001> sapSDNormalList = this.genericMgr.FindAll<SAPSDMES0001>("select s from SAPSDMES0001 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)sapSDNormalList.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPSDNormal_View")]
        public ActionResult _AjaxSDNormalBatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPSDNormalSearchStatement(command, searchModel);
            GridModel<SAPSDMES0001> gridModel = GetAjaxPageData<SAPSDMES0001>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        public ActionResult _GetSAPSDCancelGrid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));
            IList<SAPSDMES0002> sapSDCancelList = this.genericMgr.FindAll<SAPSDMES0002>("select s from SAPSDMES0002 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)sapSDCancelList.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPSDCancel_View")]
        public ActionResult _AjaxSDCancelBatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPSDCancelSearchStatement(command, searchModel);
            GridModel<SAPSDMES0002> gridModel = GetAjaxPageData<SAPSDMES0002>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        #endregion

        #region SAPItemReport
        public ActionResult SAPItemIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPItem_View")]
        public ActionResult SAPItemList(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPItem_View")]
        public ActionResult _AjaxSAPItemList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareItemSearchStatement(command, searchModel);
            GridModel<SAPItem> gridModel = GetAjaxPageData<SAPItem>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareItemSearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("BatchNo", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR", searchModel.ItemCode, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPItemCountStatement;
            searchStatementModel.SelectStatement = selectSAPItemStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPItemXLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareItemSearchStatement(command, searchModel);
            GridModel<SAPItem> gridModel = GetAjaxPageData<SAPItem>(searchStatementModel, command);
            ExportToXLS<SAPItem>("SAPItem.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPBomReport
        public ActionResult SAPBomIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPBom_View")]
        public ActionResult SAPBomList(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPBom_View")]
        public ActionResult _AjaxSAPBomList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareBomSearchStatement(command, searchModel);
            GridModel<SAPBom> gridModel = GetAjaxPageData<SAPBom>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareBomSearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("BatchNo", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR", searchModel.BomCode, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPBomCountStatement;
            searchStatementModel.SelectStatement = selectSAPBomstatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        public void ExportSAPBomXLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareBomSearchStatement(command, searchModel);
            GridModel<SAPBom> gridModel = GetAjaxPageData<SAPBom>(searchStatementModel, command);
            ExportToXLS<SAPBom>("SAPBom.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPUomConvertion
        public ActionResult SAPUomConvertionIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPUomConvertion_View")]
        public ActionResult SAPUomConvertionList(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPUomConvertion_View")]
        public ActionResult _AjaxSAPUomConvertionList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareUomConvertionSearchStatement(command, searchModel);
            GridModel<SAPUomConvertion> gridModel = GetAjaxPageData<SAPUomConvertion>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareUomConvertionSearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("BatchNo", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR", searchModel.ItemCode, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPUomConvertionCountStatement;
            searchStatementModel.SelectStatement = selectSAPUomConvertionstatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPUomConvertionXLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareUomConvertionSearchStatement(command, searchModel);
            GridModel<SAPUomConvertion> gridModel = GetAjaxPageData<SAPUomConvertion>(searchStatementModel, command);
            ExportToXLS<SAPUomConvertion>("SAPUomConvertion.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPPriceListReport
        public ActionResult SAPPriceListIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPPriceList_View")]
        public ActionResult SAPPriceListList(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPPriceList_View")]
        public ActionResult _AjaxSAPPriceListList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPPriceListSearchStatement(command, searchModel);
            GridModel<SAPPriceList> gridModel = GetAjaxPageData<SAPPriceList>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPPriceListSearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("BatchNo", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR", searchModel.ItemCode, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LIFNR", searchModel.SupplierCode, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPPriceListCountStatement;
            searchStatementModel.SelectStatement = selectSAPPriceListstatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPPriceListXLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPPriceListSearchStatement(command, searchModel);
            GridModel<SAPPriceList> gridModel = GetAjaxPageData<SAPPriceList>(searchStatementModel, command);
            ExportToXLS<SAPPriceList>("SAPPriceList.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPSupplierReport
        public ActionResult SAPSupplierIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPSupplier_View")]
        public ActionResult SAPSupplierList(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPSupplier_View")]
        public ActionResult _AjaxSAPSupplierList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPSupplierSearchStatement(command, searchModel);
            GridModel<SAPSupplier> gridModel = GetAjaxPageData<SAPSupplier>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPSupplierSearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("BatchNo", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BUKRS", searchModel.BUKRS, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LIFNR", searchModel.SupplierCode, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPSupplierCountStatement;
            searchStatementModel.SelectStatement = selectSAPSupplierstatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPSupplierXLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPSupplierSearchStatement(command, searchModel);
            GridModel<SAPSupplier> gridModel = GetAjaxPageData<SAPSupplier>(searchStatementModel, command);
            ExportToXLS<SAPSupplier>("SAPSupplier.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPCustomerReport
        public ActionResult SAPCustomerIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPCustomer_View")]
        public ActionResult SAPCustomerList(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPCustomer_View")]
        public ActionResult _AjaxSAPCustomerList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPCustomerSearchStatement(command, searchModel);
            GridModel<SAPCustomer> gridModel = GetAjaxPageData<SAPCustomer>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPCustomerSearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("BatchNo", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BUKRS", searchModel.BUKRS, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("KUNNR", searchModel.CustomerCode, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPCustomerCountStatement;
            searchStatementModel.SelectStatement = selectSAPCustomerstatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPCustomerXLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPCustomerSearchStatement(command, searchModel);
            GridModel<SAPCustomer> gridModel = GetAjaxPageData<SAPCustomer>(searchStatementModel, command);
            ExportToXLS<SAPCustomer>("SAPCustomer.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPSDNormalReport
        public ActionResult SAPSDNormalIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPSDNormal_View")]
        public ActionResult SAPSDNormalList(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPSDNormal_View")]
        public ActionResult _AjaxSAPSDNormalList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPSDNormalSearchStatement(command, searchModel);
            GridModel<SAPSDMES0001> gridModel = GetAjaxPageData<SAPSDMES0001>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPSDNormalSearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESSO", searchModel.ZMESSO, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATERIAL", searchModel.MATERIAL, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PARTNNUMB", searchModel.PARTNNUMB, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LGORT", searchModel.LGORT, "s", ref whereStatement, param);

            string timeFieldsStr = "ZCSRQSJ";
            if (searchModel.TimeType == "1")
            {
                timeFieldsStr = "WADATIST";
            }
            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement(timeFieldsStr, searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement(timeFieldsStr, searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement(timeFieldsStr, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPSDNormalCountStatement;
            searchStatementModel.SelectStatement = selectSAPSDNormalstatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPSDNormalXLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPSDNormalSearchStatement(command, searchModel);
            GridModel<SAPSDMES0001> gridModel = GetAjaxPageData<SAPSDMES0001>(searchStatementModel, command);
            ExportToXLS<SAPSDMES0001>("SAPSDNormal.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPSDCancelReport
        public ActionResult SAPSDCancelIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPSDCancel_View")]
        public ActionResult SAPSDCancelList(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPSDCancel_View")]
        public ActionResult _AjaxSAPSDCancelList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPSDCancelSearchStatement(command, searchModel);
            GridModel<SAPSDMES0002> gridModel = GetAjaxPageData<SAPSDMES0002>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPSDCancelSearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESSO", searchModel.ZMESSO, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("ZCSRQSJ", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("ZCSRQSJ", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("ZCSRQSJ", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPSDCancelCountStatement;
            searchStatementModel.SelectStatement = selectSAPSDCancelstatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPSDCancelXLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPSDCancelSearchStatement(command, searchModel);
            GridModel<SAPSDMES0002> gridModel = GetAjaxPageData<SAPSDMES0002>(searchStatementModel, command);
            ExportToXLS<SAPSDMES0002>("SAPSDCancel.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPPPMES0001Report
        //GridPart
        public ActionResult _GetSAPPPMES0001Grid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPPPMES0001> SAPPPMES0001List = this.genericMgr.FindAll<SAPPPMES0001>("select s from SAPPPMES0001 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)SAPPPMES0001List.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0001_View")]
        public ActionResult _AjaxPPMES0001BatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0001SearchStatement(command, searchModel);
            GridModel<SAPPPMES0001> gridModel = GetAjaxPageData<SAPPPMES0001>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        //
        public ActionResult SAPPPMES0001Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0001_View")]
        public ActionResult SAPPPMES0001List(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPPPMES0001_View")]
        public ActionResult _AjaxSAPPPMES0001List(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0001SearchStatement(command, searchModel);
            GridModel<SAPPPMES0001> gridModel = GetAjaxPageData<SAPPPMES0001>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPPPMES0001SearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }

            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESSC", searchModel.ZMESSC, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR_H", searchModel.MATNR_H, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR_I", searchModel.MATNR_I, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LGORT_H", searchModel.LGORT_H, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LGORT_I", searchModel.LGORT_I, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BWART_H", searchModel.BWART_H, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BWART_I", searchModel.BWART_I, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MTSNR", searchModel.MTSNR, "s", ref whereStatement, param);

            string timeFieldsStr = "ZCSRQSJ";
            if (searchModel.TimeType == "1")
            {
                timeFieldsStr = "BUDAT";
            }
            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement(timeFieldsStr, searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement(timeFieldsStr, searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement(timeFieldsStr, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            if (!string.IsNullOrWhiteSpace(searchModel.MultiInterfaces))
            {

                string interfaceSql = " and s.OrderType in( ";
                if (string.IsNullOrWhiteSpace(whereStatement))
                {
                    interfaceSql = " where s.OrderType in( ";
                }
                string[] interfaceArr = searchModel.MultiInterfaces.Split(',');
                for (int st = 0; st < interfaceArr.Length; st++)
                {
                    interfaceSql += "'" + interfaceArr[st] + "',";
                }
                whereStatement += interfaceSql.Substring(0, interfaceSql.Length - 1) + ")";
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPPPMES0001CountStatement;
            searchStatementModel.SelectStatement = selectSAPPPMES0001statement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPPPMES0001XLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0001SearchStatement(command, searchModel);
            GridModel<SAPPPMES0001> gridModel = GetAjaxPageData<SAPPPMES0001>(searchStatementModel, command);
            ExportToXLS<SAPPPMES0001>("SAPPPMES0001.XLS", gridModel.Data.ToList());
        }
        #endregion
        #region SAPPPMES0002Report
        //GridPart
        public ActionResult _GetSAPPPMES0002Grid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPPPMES0002> SAPPPMES0002List = this.genericMgr.FindAll<SAPPPMES0002>("select s from SAPPPMES0002 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)SAPPPMES0002List.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0002_View")]
        public ActionResult _AjaxPPMES0002BatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0002SearchStatement(command, searchModel);
            GridModel<SAPPPMES0002> gridModel = GetAjaxPageData<SAPPPMES0002>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        //
        public ActionResult SAPPPMES0002Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0002_View")]
        public ActionResult SAPPPMES0002List(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPPPMES0002_View")]
        public ActionResult _AjaxSAPPPMES0002List(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0002SearchStatement(command, searchModel);
            GridModel<SAPPPMES0002> gridModel = GetAjaxPageData<SAPPPMES0002>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPPPMES0002SearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESSC", searchModel.ZMESSC, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZComnum", searchModel.ZComnum, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("ZCSRQSJ", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("ZCSRQSJ", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("ZCSRQSJ", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            if (!string.IsNullOrWhiteSpace(searchModel.MultiInterfaces))
            {

                string interfaceSql = " and s.OrderType in( ";
                if (string.IsNullOrWhiteSpace(whereStatement))
                {
                    interfaceSql = " where s.OrderType in( ";
                }
                string[] interfaceArr = searchModel.MultiInterfaces.Split(',');
                for (int st = 0; st < interfaceArr.Length; st++)
                {
                    interfaceSql += "'" + interfaceArr[st] + "',";
                }
                whereStatement += interfaceSql.Substring(0, interfaceSql.Length - 1) + ")";
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPPPMES0002CountStatement;
            searchStatementModel.SelectStatement = selectSAPPPMES0002statement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPPPMES0002XLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0002SearchStatement(command, searchModel);
            GridModel<SAPPPMES0002> gridModel = GetAjaxPageData<SAPPPMES0002>(searchStatementModel, command);
            ExportToXLS<SAPPPMES0002>("SAPPPMES0002.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPPPMES0003Report
        //GridPart
        public ActionResult _GetSAPPPMES0003Grid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPPPMES0003> SAPPPMES0003List = this.genericMgr.FindAll<SAPPPMES0003>("select s from SAPPPMES0003 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)SAPPPMES0003List.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0003_View")]
        public ActionResult _AjaxPPMES0003BatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0003SearchStatement(command, searchModel);
            GridModel<SAPPPMES0003> gridModel = GetAjaxPageData<SAPPPMES0003>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        //
        public ActionResult SAPPPMES0003Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0003_View")]
        public ActionResult SAPPPMES0003List(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPPPMES0003_View")]
        public ActionResult _AjaxSAPPPMES0003List(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0003SearchStatement(command, searchModel);
            GridModel<SAPPPMES0003> gridModel = GetAjaxPageData<SAPPPMES0003>(searchStatementModel, command);
            foreach (var sAPPPMES0003 in gridModel.Data)
            {
                sAPPPMES0003.ScrapId = sAPPPMES0003.MTSNR.Replace("OF", "").TrimStart('0');
            }
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPPPMES0003SearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESSC", searchModel.ZMESSC, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR_I", searchModel.MATNR_I, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LGORT_I", searchModel.LGORT_I, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BWART_I", searchModel.BWART_I, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MTSNR", searchModel.MTSNR, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZComnum", searchModel.ZComnum, "s", ref whereStatement, param);
            string timeFieldsStr = "ZCSRQSJ";
            if (searchModel.TimeType == "1")
            {
                timeFieldsStr = "BUDAT";
            }
            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement(timeFieldsStr, searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement(timeFieldsStr, searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement(timeFieldsStr, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPPPMES0003CountStatement;
            searchStatementModel.SelectStatement = selectSAPPPMES0003statement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPPPMES0003XLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0003SearchStatement(command, searchModel);
            GridModel<SAPPPMES0003> gridModel = GetAjaxPageData<SAPPPMES0003>(searchStatementModel, command);
            ExportToXLS<SAPPPMES0003>("SAPPPMES0003.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPPPMES0004Report
        //GridPart
        public ActionResult _GetSAPPPMES0004Grid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPPPMES0004> SAPPPMES0004List = this.genericMgr.FindAll<SAPPPMES0004>("select s from SAPPPMES0004 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)SAPPPMES0004List.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0004_View")]
        public ActionResult _AjaxPPMES0004BatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0004SearchStatement(command, searchModel);
            GridModel<SAPPPMES0004> gridModel = GetAjaxPageData<SAPPPMES0004>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        //
        public ActionResult SAPPPMES0004Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0004_View")]
        public ActionResult SAPPPMES0004List(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPPPMES0004_View")]
        public ActionResult _AjaxSAPPPMES0004List(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0004SearchStatement(command, searchModel);
            GridModel<SAPPPMES0004> gridModel = GetAjaxPageData<SAPPPMES0004>(searchStatementModel, command);
            foreach (var sAPPPMES0004 in gridModel.Data)
            {
                sAPPPMES0004.FilterId = sAPPPMES0004.ZComnum.Replace("RF", "").TrimStart('0');
            }
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPPPMES0004SearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESSC", searchModel.ZMESSC, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LFSNR", searchModel.LFSNR, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BWART_H", searchModel.BWART_H, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZComnum", searchModel.ZComnum, "s", ref whereStatement, param);

            string timeFieldsStr = "ZCSRQSJ";
            if (searchModel.TimeType == "1")
            {
                timeFieldsStr = "BUDAT";
            }
            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement(timeFieldsStr, searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement(timeFieldsStr, searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement(timeFieldsStr, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPPPMES0004CountStatement;
            searchStatementModel.SelectStatement = selectSAPPPMES0004statement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPPPMES0004XLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0004SearchStatement(command, searchModel);
            GridModel<SAPPPMES0004> gridModel = GetAjaxPageData<SAPPPMES0004>(searchStatementModel, command);
            foreach (var sAPPPMES0004 in gridModel.Data)
            {
                sAPPPMES0004.FilterId = sAPPPMES0004.ZComnum.Replace("RF", "").TrimStart('0');
            }
            ExportToXLS<SAPPPMES0004>("SAPPPMES0004.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPPPMES0005Report
        //GridPart
        public ActionResult _GetSAPPPMES0005Grid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPPPMES0005> SAPPPMES0005List = this.genericMgr.FindAll<SAPPPMES0005>("select s from SAPPPMES0005 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)SAPPPMES0005List.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0005_View")]
        public ActionResult _AjaxPPMES0005BatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0005SearchStatement(command, searchModel);
            GridModel<SAPPPMES0005> gridModel = GetAjaxPageData<SAPPPMES0005>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        //
        public ActionResult SAPPPMES0005Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0005_View")]
        public ActionResult SAPPPMES0005List(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPPPMES0005_View")]
        public ActionResult _AjaxSAPPPMES0005List(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0005SearchStatement(command, searchModel);
            GridModel<SAPPPMES0005> gridModel = GetAjaxPageData<SAPPPMES0005>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPPPMES0005SearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESSC", searchModel.ZMESSC, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR_I", searchModel.MATNR_I, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LGORT_I", searchModel.LGORT_I, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BWART_I", searchModel.BWART_I, "s", ref whereStatement, param);
            string timeFieldsStr = "ZCSRQSJ";
            if (searchModel.TimeType == "1")
            {
                timeFieldsStr = "BUDAT";
            }
            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement(timeFieldsStr, searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement(timeFieldsStr, searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement(timeFieldsStr, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPPPMES0005CountStatement;
            searchStatementModel.SelectStatement = selectSAPPPMES0005statement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPPPMES0005XLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0005SearchStatement(command, searchModel);
            GridModel<SAPPPMES0005> gridModel = GetAjaxPageData<SAPPPMES0005>(searchStatementModel, command);
            ExportToXLS<SAPPPMES0005>("SAPPPMES0005.XLS", gridModel.Data.ToList());
        }
        #endregion

        #region SAPPPMES0006Report
        //GridPart
        public ActionResult _GetSAPPPMES0006Grid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPPPMES0006> SAPPPMES0006List = this.genericMgr.FindAll<SAPPPMES0006>("select s from SAPPPMES0006 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)SAPPPMES0006List.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0006_View")]
        public ActionResult _AjaxPPMES0006BatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0006SearchStatement(command, searchModel);
            GridModel<SAPPPMES0006> gridModel = GetAjaxPageData<SAPPPMES0006>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        //
        public ActionResult SAPPPMES0006Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPPPMES0006_View")]
        public ActionResult SAPPPMES0006List(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPPPMES0006_View")]
        public ActionResult _AjaxSAPPPMES0006List(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0006SearchStatement(command, searchModel);
            GridModel<SAPPPMES0006> gridModel = GetAjaxPageData<SAPPPMES0006>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPPPMES0006SearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESSC", searchModel.ZMESSC, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LGORT", searchModel.LGORT, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR1", searchModel.MATNR1, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BWART", searchModel.BWART, "s", ref whereStatement, param);
            string timeFieldsStr = "ZCSRQSJ";
            if (searchModel.TimeType == "1")
            {
                timeFieldsStr = "BUDAT";
            }
            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement(timeFieldsStr, searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement(timeFieldsStr, searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement(timeFieldsStr, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPPPMES0006CountStatement;
            searchStatementModel.SelectStatement = selectSAPPPMES0006statement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPPPMES0006XLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPPPMES0006SearchStatement(command, searchModel);
            GridModel<SAPPPMES0006> gridModel = GetAjaxPageData<SAPPPMES0006>(searchStatementModel, command);
            ExportToXLS<SAPPPMES0006>("SAPPPMES0006.XLS", gridModel.Data.ToList());
        }
        #endregion
        #region SAPMMMES0001Report
        //GridPart
        public ActionResult _GetSAPMMMES0001Grid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPMMMES0001> SAPMMMES0001List = this.genericMgr.FindAll<SAPMMMES0001>("select s from SAPMMMES0001 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)SAPMMMES0001List.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPMMMES0001_View")]
        public ActionResult _AjaxMMMES0001BatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPMMMES0001SearchStatement(command, searchModel);
            GridModel<SAPMMMES0001> gridModel = GetAjaxPageData<SAPMMMES0001>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        //
        public ActionResult SAPMMMES0001Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPMMMES0001_View")]
        public ActionResult SAPMMMES0001List(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPMMMES0001_View")]
        public ActionResult _AjaxSAPMMMES0001List(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPMMMES0001SearchStatement(command, searchModel);
            GridModel<SAPMMMES0001> gridModel = GetAjaxPageData<SAPMMMES0001>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPMMMES0001SearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }



            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESPO", searchModel.ZMESPO, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LFSNR", searchModel.LFSNR, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LIFNR", searchModel.LIFNR, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LGORT", searchModel.LGORT, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR", searchModel.MATNR, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR_C", searchModel.MATNR_C, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BWART_H", searchModel.BWART_H, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BWART_C", searchModel.BWART_C, "s", ref whereStatement, param);

            string timeFieldsStr = "ZCSRQSJ";
            if (searchModel.TimeType == "1")
            {
                timeFieldsStr = "BUDAT";
            }
            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement(timeFieldsStr, searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement(timeFieldsStr, searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement(timeFieldsStr, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPMMMES0001CountStatement;
            searchStatementModel.SelectStatement = selectSAPMMMES0001statement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPMMMES0001XLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPMMMES0001SearchStatement(command, searchModel);
            GridModel<SAPMMMES0001> gridModel = GetAjaxPageData<SAPMMMES0001>(searchStatementModel, command);
            ExportToXLS<SAPMMMES0001>("SAPMMMES0001.XLS", gridModel.Data.ToList());
        }
        #endregion
        #region SAPMMMES0002Report
        //GridPart
        public ActionResult _GetSAPMMMES0002Grid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPMMMES0002> SAPMMMES0002List = this.genericMgr.FindAll<SAPMMMES0002>("select s from SAPMMMES0002 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)SAPMMMES0002List.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPMMMES0002_View")]
        public ActionResult _AjaxMMMES0002BatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPMMMES0002SearchStatement(command, searchModel);
            GridModel<SAPMMMES0002> gridModel = GetAjaxPageData<SAPMMMES0002>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        //
        public ActionResult SAPMMMES0002Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPMMMES0002_View")]
        public ActionResult SAPMMMES0002List(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPMMMES0002_View")]
        public ActionResult _AjaxSAPMMMES0002List(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPMMMES0002SearchStatement(command, searchModel);
            GridModel<SAPMMMES0002> gridModel = GetAjaxPageData<SAPMMMES0002>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPMMMES0002SearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESPO", searchModel.ZMESPO, "s", ref whereStatement, param);


            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("ZCSRQSJ", searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement("ZCSRQSJ", searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement("ZCSRQSJ", searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPMMMES0002CountStatement;
            searchStatementModel.SelectStatement = selectSAPMMMES0002statement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPMMMES0002XLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPMMMES0002SearchStatement(command, searchModel);
            GridModel<SAPMMMES0002> gridModel = GetAjaxPageData<SAPMMMES0002>(searchStatementModel, command);
            ExportToXLS<SAPMMMES0002>("SAPMMMES0002.XLS", gridModel.Data.ToList());
        }
        #endregion
        #region SAPSTMES0001Report
        //GridPart
        public ActionResult _GetSAPSTMES0001Grid(string batchNo)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DefaultPageSize));

            IList<SAPSTMES0001> SAPSTMES0001List = this.genericMgr.FindAll<SAPSTMES0001>("select s from SAPSTMES0001 as s where s.UniqueCode=?", batchNo);
            ViewBag.Total = (int)Math.Ceiling(((decimal)SAPSTMES0001List.Count() / (decimal)value));
            ViewBag.PageSize = value;
            ViewBag.BatchNo = batchNo;
            return View();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_SAPSTMES0001_View")]
        public ActionResult _AjaxSTMES0001BatchNoList(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            searchModel.Status = "10";
            SearchStatementModel searchStatementModel = this.PrepareSAPSTMES0001SearchStatement(command, searchModel);
            GridModel<SAPSTMES0001> gridModel = GetAjaxPageData<SAPSTMES0001>(searchStatementModel, command);
            return PartialView(gridModel);
        }
        //
        public ActionResult SAPSTMES0001Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_SAPSTMES0001_View")]
        public ActionResult SAPSTMES0001List(GridCommand command, SAPInterfaceSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_SAPSTMES0001_View")]
        public ActionResult _AjaxSAPSTMES0001List(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSAPSTMES0001SearchStatement(command, searchModel);
            GridModel<SAPSTMES0001> gridModel = GetAjaxPageData<SAPSTMES0001>(searchStatementModel, command);
            return PartialView(gridModel);
        }

        private SearchStatementModel PrepareSAPSTMES0001SearchStatement(GridCommand command, SAPInterfaceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.Status != "0" && searchModel.Status != "10")
            {
                HqlStatementHelper.AddEqStatement("Status", Convert.ToInt32(searchModel.Status), "s", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("UniqueCode", searchModel.BatchNo, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ZMESKO", searchModel.ZMESKO, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR1", searchModel.MATNR1, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LGORT", searchModel.LGORT, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("UMLGO", searchModel.UMLGO, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("BWARTWA", searchModel.BWARTWA, "s", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("MATNR_TH", searchModel.MATNR_TH, "s", ref whereStatement, param);

            string timeFieldsStr = "ZCSRQSJ";
            if (searchModel.TimeType == "1")
            {
                timeFieldsStr = "BUDAT";
            }
            if (searchModel.TransStartDate != null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddBetweenStatement(timeFieldsStr, searchModel.TransStartDate, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate != null & searchModel.TransEndDate == null)
            {
                HqlStatementHelper.AddGeStatement(timeFieldsStr, searchModel.TransStartDate, "s", ref whereStatement, param);
            }
            else if (searchModel.TransStartDate == null & searchModel.TransEndDate != null)
            {
                HqlStatementHelper.AddLeStatement(timeFieldsStr, searchModel.TransEndDate, "s", ref whereStatement, param);
            }
            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "StatusDesc")
                {
                    command.SortDescriptors[0].Member = "Status";
                }
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectSAPSTMES0001CountStatement;
            searchStatementModel.SelectStatement = selectSAPSTMES0001statement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        public void ExportSAPSTMES0001XLS(SAPInterfaceSearchModel searchModel)
        {
            int value = System.Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareSAPSTMES0001SearchStatement(command, searchModel);
            GridModel<SAPSTMES0001> gridModel = GetAjaxPageData<SAPSTMES0001>(searchStatementModel, command);
            ExportToXLS<SAPSTMES0001>("SAPSTMES0001.XLS", gridModel.Data.ToList());
        }
        #endregion


        public string _GetDataToTime(int calltype)
        {

            string sysCode="";
            if (calltype == 6)
            {
                sysCode = BusinessConstants.SAPBUSINESSDATA_SYSTEMCODE;
            }
            else if (calltype == 7)
            {
                sysCode = BusinessConstants.SAPBUSINESSDATA_SYSTEMCODEAdjust;
            }
            else 
            {
                sysCode = BusinessConstants.SAPBUSINESSDATA_SYSTEMCODEAdjustTail;
            }
            var dateindex = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(sysCode);
            var str = Resources.EXT.ControllerLan.Con_BusinessCurrentDateIs + dateindex.CurrTransDate.Value.ToString("yyyy-MM-dd HH:mm");
            return str.ToString();
        }
        #region ManualTransInv
        public ActionResult _ManualTransSnapShot(DateTime selectedTime)
        {
            try
            {
                salesDistributionMgr.ExportMESQTY0001(selectedTime);
                SaveSuccessMessage("Successful");
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_ExecuteUnsuccessfully);
            }
            return Json(null);
        }
        #endregion

        public string ManualCallMiscOrderNoType(string miscOrderNo)
        {
            string rMessage = "";
            try
            {
                rMessage = genericMgr.FindAllWithNativeSql<string>("USP_SAP_PP_UGetMiscOrderType '" + miscOrderNo + "'").FirstOrDefault();
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex.Message);
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_ExecuteUnsuccessfully);
            }
            return rMessage;
        }
    }
}
