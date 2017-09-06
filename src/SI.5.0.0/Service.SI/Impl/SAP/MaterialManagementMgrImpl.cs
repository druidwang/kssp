using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Utility;
using com.Sconit.Entity.SI.SAP;
using System.Net;

namespace com.Sconit.Service.SI.SAP.Impl
{
    public class MaterialManagementMgrImpl : com.Sconit.Service.SI.Impl.BaseMgr,IMaterialManagementMgr
    {

        #region 导出采购订单信息给sap
        public static object ExportMMMES0001Lock = new object();
        public List<ErrorMessage> ExportMMMES0001Data()
        {
            lock (ExportMMMES0001Lock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPMMMES0001>("from SAPMMMES0001 where status=0");

                    //根据批次调用接口发送数据
                    MMMES0001.ZWS_MMMES0001 sapSeviceProxy = new MMMES0001.ZWS_MMMES0001();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    MMMES0001.ZfunMmmes0001 input = new MMMES0001.ZfunMmmes0001();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();

                    List<MMMES0001.ZstrMmmes0001> sendRows = new List<MMMES0001.ZstrMmmes0001>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new MMMES0001.ZstrMmmes0001
                                    {
                                        Bldat = r.BLDAT.ToString("yyyyMMdd"),
                                        Budat = r.BUDAT.ToString("yyyyMMdd"),
                                        BprmeC = r.BPRME_C,
                                        BprmeI = r.BPRME_I,
                                        Bsart = r.BSART,
                                        Bukrs = r.BUKRS,
                                        BwartC = r.BWART_C,
                                        BwartH = r.BWART_H,
                                        EbelpI = r.EBELP_I,
                                        Eindt = r.EINDT.ToString("yyyyMMdd"),
                                        Ekgrp = r.EKGRP,
                                        Ekorg = r.EKORG,
                                        Epstp = r.EPSTP,
                                        Lfsnr = r.LFSNR,
                                        Lifnr = r.LIFNR,
                                        Lgort = r.LGORT,
                                        Matnr = r.MATNR,
                                        MatnrC = r.MATNR_C,
                                        Netpr = r.NETPR,
                                        Peinh = r.PEINH,
                                        Retpo = r.RETPO,
                                        TargetQtyC = r.TARGET_QTY_C,
                                        TargetQtyI = r.TARGET_QTY_I,
                                        Waers = r.WAERS,
                                        Werks = r.WERKS,
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss"),
                                        Zmesguid = r.ZMESGUID,
                                        Zmespo = r.ZMESPO
                                    }).ToList();

                        try
                        {
                            input.ItData = sendRows.ToArray();
                            sapSeviceProxy.ZfunMmmes0001(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_MMMES0001 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "MMMES0001", BusinessConstants.SAPBUSINESSDATA_SYSTEMCODE, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输采购订单数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "MMMES0001", BusinessConstants.SAPBUSINESSDATA_SYSTEMCODE, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string logMessage = "导出采购订单数据给SAP出错。";
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

        #region 导出采购订单冲销信息给sap
        public static object ExportMMMES0002Lock = new object();
        public List<ErrorMessage> ExportMMMES0002Data()
        {
            lock (ExportMMMES0002Lock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {

                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPMMMES0002>("from SAPMMMES0002 where status=0");
                    //根据批次调用接口发送数据
                    MMMES0002.ZWS_MMMES0002 sapSeviceProxy = new MMMES0002.ZWS_MMMES0002();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    MMMES0002.ZfunMmmes0002 input = new MMMES0002.ZfunMmmes0002();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();

                    List<MMMES0002.ZstrMmmes0003> sendRows = new List<MMMES0002.ZstrMmmes0003>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new MMMES0002.ZstrMmmes0003
                                    {
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss"),
                                        Zmesguid = r.ZMESGUID,
                                        Zmespo = r.ZMESPO,
                                        Budat = r.CancelDate.ToString("yyyyMMdd"),
                                        Bldat = r.CancelDate.ToString("yyyyMMdd")
                                    }).ToList();

                        try
                        {
                            input.ItData = sendRows.ToArray();
                            sapSeviceProxy.ZfunMmmes0002(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_MMMES0002 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "MMMES0002", BusinessConstants.SAPBUSINESSDATA_SYSTEMCODE, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输采购订单冲销数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "SDMES0002", BusinessConstants.SAPBUSINESSDATA_SYSTEMCODE, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string logMessage = "导出采购订单冲销数据给SAP出错。";
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

        #region 导出库内事务信息给sap
        public static object ExportSTMES0001DataLock = new object();
        public List<ErrorMessage> ExportSTMES0001Data()
        {
            lock (ExportSTMES0001DataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPSTMES0001>("from SAPSTMES0001 where Status=0");

                    //根据批次调用接口发送数据
                    STMES0001.ZfunStmes0001 input = new STMES0001.ZfunStmes0001();
                    STMES0001.ZWS_STMES0001 sapSeviceProxy = new STMES0001.ZWS_STMES0001();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);

                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();
                    List<STMES0001.ZstrMmmes0002> sendRows = new List<STMES0001.ZstrMmmes0002>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new STMES0001.ZstrMmmes0002
                                    {
                                        Bldat = r.BLDAT.ToString("yyyyMMdd"),
                                        Budat = r.BUDAT.ToString("yyyyMMdd"),
                                        Bwartwa = r.BWARTWA,
                                        Epfmg = r.EPFMG,
                                        Erfme = r.ERFME,
                                        Kostl = r.KOSTL,
                                        Lgort = r.LGORT,
                                        Lifnr = r.LIFNR,
                                        Matnr1 = r.MATNR1,
                                        MatnrTh = r.MATNR_TH,
                                        Umlgo = r.UMLGO,
                                        Werks = r.WERKS,
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss"),
                                        Zmesguid = r.ZMESGUID,
                                        Zmesko = r.ZMESKO,
                                        Zmeskoseq = r.ZMESKOSEQ,
                                        Spart=r.SPART
                                    }).ToList();

                        try
                        {
                            input.ItData = sendRows.ToArray();
                            sapSeviceProxy.ZfunStmes0001(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_STMES0001 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "STMES0001", BusinessConstants.SAPBUSINESSDATA_SYSTEMCODE, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输库内移动业务数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "STMES0001", BusinessConstants.SAPBUSINESSDATA_SYSTEMCODE, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string logMessage = "导出输库内移动业务数据给SAP出错。";
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
    }
}
