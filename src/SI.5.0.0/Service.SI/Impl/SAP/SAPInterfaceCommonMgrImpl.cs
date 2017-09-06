using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Utility;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity;
using com.Sconit.Entity.SI.SAP;
using System.Net;
using System.Data.SqlClient;
using Castle.Services.Transaction;
using System.Data;

namespace com.Sconit.Service.SI.SAP.Impl
{
    [Transactional]
    public class SAPInterfaceCommonMgrImpl : com.Sconit.Service.SI.Impl.BaseMgr, ISAPInterfaceCommonMgr
    {
        public ISalesDistributionMgr salesDistributionMgr { get; set; }
        public IMaterialManagementMgr materialManagementMgr { get; set; }
        public IProductionPlanningMgr productionPlanningMgr { get; set; }
        public com.Sconit.Persistence.ISqlDao sqlDao { get; set; }

        #region Export Mes Qty
        public static object GenMesQtyDataLock = new object();
        public DateTime GenMesQtyData()
        {
            //lock (GenMesQtyDataLock)
            //{
                var errorMessageList = new List<ErrorMessage>();
                DateTime currDate;
                string dBName = genericMgr.FindAllWithNativeSql<string>("Select DB_NAME()").FirstOrDefault();
                currDate = genericMgr.FindAllWithNativeSql<DateTime>("Select GetDate()").FirstOrDefault();
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = null;
                DateTime? dataToDate = null;
                try
                {
                    var batchNo = Guid.NewGuid().ToString().Replace("-", "");

                    //Save inv of current time
                    User user = SecurityContextHolder.Get();
                    var result = this.genericMgr.FindAllWithNamedQuery<object>("USP_SAP_SaveInvForSIExecution_New", new object[] { batchNo, DateTime.Now, DateTime.Now, user.Id });
                    if (result.Count()>0)
                    {
                        currDate = (DateTime)result[0];
                    }
                    var messages = new List<ErrorMessage>();
                    if (dBName.ToUpper() == "SCONIT")
                    {
                        messages.AddRange(salesDistributionMgr.ExportMESQTY0001());
                    }
                    //记录成功日志
                    this.SaveTransferLog(batchNo, "SUCCESS", "GenMesInvSnap", "GenMesInvSnap", 1, 1, transStartDate, dataFromDate, dataToDate);
                }
                catch (Exception ex)
                {
                    string logMessage = "业务接口快照数据生成失败，失败的时间为:" + currDate.ToString("yyyy-MM-dd HH:mm:ss");
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                        Exception = ex,
                        Message = logMessage
                    });
                }
                this.SendErrorMessage(errorMessageList);
                return currDate;
            //}


        }
        #endregion
        #region transmit all business data
        public static object TransBusinessOrderDataLock = new object();
        public List<ErrorMessage> TransBusinessOrderData()
        {
            lock (TransBusinessOrderDataLock)
            {
                var errorMessageList = new List<ErrorMessage>();
                string dBName = genericMgr.FindAllWithNativeSql<string>("Select DB_NAME()").FirstOrDefault();
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = null;
                DateTime? dataToDate = null;
                try
                {
                    //DateTime currDate = DateTime.Now;
                    var messages = new List<ErrorMessage>();
                    if (dBName.ToUpper() == "SCONIT" || (dBName.ToUpper() == "SCONIT_TEST" && this.SAPService_IP == "10.166.1.92"))
                    {
                        messages.AddRange(materialManagementMgr.ExportMMMES0001Data());
                        messages.AddRange(materialManagementMgr.ExportMMMES0002Data());
                        messages.AddRange(materialManagementMgr.ExportSTMES0001Data());
                        messages.AddRange(productionPlanningMgr.ExportPPMES0001Data());
                        messages.AddRange(productionPlanningMgr.ExportPPMES0002Data());
                        messages.AddRange(productionPlanningMgr.ExportPPMES0003Data());
                        messages.AddRange(productionPlanningMgr.ExportPPMES0004Data());
                        messages.AddRange(productionPlanningMgr.ExportPPMES0005Data());
                        messages.AddRange(productionPlanningMgr.ExportPPMES0006Data());
                        messages.AddRange(materialManagementMgr.ExportSTMES0001Data());
                        messages.AddRange(salesDistributionMgr.ExportSDMES0001());
                        messages.AddRange(salesDistributionMgr.ExportSDMES0002());
                    }
                    //记录成功日志
                    this.SaveTransferLog("TransBusinessData", "SUCCESS", "TransBusinessData", "TransBusinessData", 1, 0, transStartDate, dataFromDate, dataToDate);
                }
                catch (Exception ex)
                {
                    string logMessage = "业务接口数据传输失败，失败的时间点为:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                        Exception = ex,
                        Message = logMessage
                    });
                }
                this.SendErrorMessage(errorMessageList);
                return errorMessageList;
            }


        }
        #endregion
        #region Export and transmit all business data
        public static object GenBusinessOrderDataLock = new object();
        [Transaction(TransactionMode.Requires)]
        public List<ErrorMessage> GenBusinessOrderData(DateTime currDate)
        {
            SAPTransferTimeControl timeControl = new SAPTransferTimeControl();
            lock (GenBusinessOrderDataLock)
            {
                var errorMessageList = new List<ErrorMessage>();
                string dBName = genericMgr.FindAllWithNativeSql<string>("Select DB_NAME()").FirstOrDefault();
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = currDate;
                DateTime? dataToDate = DateTime.Now;
                try
                {
                    //DateTime currDate = DateTime.Now;
                    var timeContrlCode = BusinessConstants.SAPBUSINESSDATA_SYSTEMCODE;
                    timeControl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(timeContrlCode);
                    dataFromDate = timeControl.CurrTransDate;
                    var batchNo = Guid.NewGuid().ToString().Replace("-", "");
                    if (!timeControl.CurrTransDate.HasValue)
                    {
                        return errorMessageList;
                    }
                    if (timeControl.CurrTransDate > currDate)
                    {
                        return errorMessageList;
                    }
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_MM_ExportPurOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_MM_ExportTransOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_ExportMIOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_ExportMIFilterOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_ExportEXOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_ExportEXScraptOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_ExportFIOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_ExportReworkOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_ExportTrailMiscOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ExportSalesOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_UpdateEXScraptMES26", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_UpdateFIScrapt", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    //data transmission
                    var messages = new List<ErrorMessage>();
                    //记录成功日志
                    timeControl.LastTransDate = timeControl.CurrTransDate;
                    timeControl.CurrTransDate = currDate;
                    this.genericMgr.Update(timeControl);
                    this.SaveTransferLog(timeContrlCode, "SUCCESS", timeContrlCode, timeContrlCode, 1,0, transStartDate, dataFromDate, dataToDate);

                }
                catch (Exception ex)
                {
                    string logMessage = "业务接口数据生成失，失败的时间段为:" + timeControl.CurrTransDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + " 到 " + currDate.ToString("yyyy-MM-dd HH:mm:ss");
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                        Exception = ex,
                        Message = logMessage
                    });
                    this.SendErrorMessage(errorMessageList);
                    throw ex;

                }
                this.SendErrorMessage(errorMessageList);
                return errorMessageList;
            }


        }
        #endregion
        #region Export and transmit adjust business data
        public static object GenBusinessAdjustOrderDataLock = new object();
        [Transaction(TransactionMode.Requires)]
        public List<ErrorMessage> GenBusinessAdjustOrderData(DateTime currDate)
        {
            SAPTransferTimeControl timeControl = new SAPTransferTimeControl();
            lock (GenBusinessAdjustOrderDataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                string dBName = genericMgr.FindAllWithNativeSql<string>("Select DB_NAME()").FirstOrDefault();
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = currDate;
                DateTime? dataToDate = DateTime.Now;
                try
                {
                    var timeContrlCode = BusinessConstants.SAPBUSINESSDATA_SYSTEMCODEAdjust;
                    timeControl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(timeContrlCode);
                    var batchNo = Guid.NewGuid().ToString().Replace("-", "");
                    dataFromDate = timeControl.CurrTransDate;
                    if (!timeControl.CurrTransDate.HasValue)
                    {
                        return errorMessageList;
                    }
                    if (timeControl.CurrTransDate > currDate)
                    {
                        return errorMessageList;
                    }
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_ExportAdjustOrder", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    //data transmission
                    var messages = new List<ErrorMessage>();
                    //记录成功日志
                    timeControl.LastTransDate = timeControl.CurrTransDate;
                    timeControl.CurrTransDate = currDate;
                    this.genericMgr.Update(timeControl);
                    this.SaveTransferLog(timeContrlCode, "SUCCESS", timeContrlCode, timeContrlCode, 1, 0, transStartDate, dataFromDate, dataToDate);
                }
                catch (Exception ex)
                {
                    string logMessage = "业务调整单接口数据生成失，失败的时间段为:" + timeControl.CurrTransDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + " 到 " + currDate.ToString("yyyy-MM-dd HH:mm:ss");
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataAdjustToSAPErrorTemplate,
                        Exception = ex,
                        Message = logMessage
                    });
                    this.SendErrorMessage(errorMessageList);
                    throw ex;
                }
                this.SendErrorMessage(errorMessageList);
                return errorMessageList;
            }
        }
        #endregion
        #region Export and transmit AdjustTail business data
        public static object GenBusinessAdjustTailOrderDataLock = new object();
        [Transaction(TransactionMode.Requires)]
        public List<ErrorMessage> GenBusinessAdjustTailOrderData(DateTime currDate)
        {
            SAPTransferTimeControl timeControl = new SAPTransferTimeControl();
            lock (GenBusinessAdjustTailOrderDataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                string dBName = genericMgr.FindAllWithNativeSql<string>("Select DB_NAME()").FirstOrDefault();
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = currDate;
                DateTime? dataToDate = currDate;
                try
                {
                    var timeContrlCode = BusinessConstants.SAPBUSINESSDATA_SYSTEMCODEAdjustTail;
                    timeControl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(timeContrlCode);
                    var batchNo = Guid.NewGuid().ToString().Replace("-", "");
                    dataFromDate = timeControl.CurrTransDate;
                    //Save inv of current time
                    if (!timeControl.CurrTransDate.HasValue)
                    {
                        return errorMessageList;
                    }
                    if (timeControl.CurrTransDate > currDate)
                    {
                        return errorMessageList;
                    }
                    this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_PP_ExportAdjustOrder_TailQty", new object[] { Guid.NewGuid().ToString().Replace("-", ""), timeControl.CurrTransDate, currDate }).FirstOrDefault();
                    //data transmission
                    var messages = new List<ErrorMessage>();
                    //记录成功日志
                    timeControl.LastTransDate = timeControl.CurrTransDate;
                    timeControl.CurrTransDate = currDate;
                    this.genericMgr.Update(timeControl);
                    this.SaveTransferLog(timeContrlCode, "SUCCESS", timeContrlCode, timeContrlCode, 1, 0, transStartDate, dataFromDate, dataToDate);
                }
                catch (Exception ex)
                {
                    string logMessage = "尾差接口数据生成失，失败的时间段为:" + timeControl.CurrTransDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + " 到 " + currDate.ToString("yyyy-MM-dd HH:mm:ss");
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataAdjustToSAPErrorTemplate,
                        Exception = ex,
                        Message = logMessage
                    });
                    this.SendErrorMessage(errorMessageList);
                    throw ex;
                }
                this.SendErrorMessage(errorMessageList);
                return errorMessageList;
            }
        }
        #endregion
    }
}
