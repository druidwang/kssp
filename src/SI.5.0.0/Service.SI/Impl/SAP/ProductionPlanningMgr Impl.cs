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
    public class ProductionPlanningMgrImpl : com.Sconit.Service.SI.Impl.BaseMgr, IProductionPlanningMgr
    {
        #region 导出生产计划事务信息给sap
        public static object ExportPPMES0001DataLock = new object();
        public List<ErrorMessage> ExportPPMES0001Data()
        {
            lock (ExportPPMES0001DataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPPPMES0001>("from SAPPPMES0001 where Status=0");

                    //根据批次调用接口发送数据
                    //PPMES0001.PPMES0001 PPMES0001 = new PPMES0001.PPMES0001();
                    PPMES0001.ZWS_PPMES0001 sapSeviceProxy = new PPMES0001.ZWS_PPMES0001();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    PPMES0001.ZfunPpmes0001 input = new PPMES0001.ZfunPpmes0001();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();
                    List<PPMES0001.ZstrPpmes0001> sendRows = new List<PPMES0001.ZstrPpmes0001>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new PPMES0001.ZstrPpmes0001
                                    {
                                        Zmessc = r.ZMESSC,
                                        Zmesln = r.ZMESLN,
                                        Zptype = r.ZPTYPE,
                                        Aufart = r.AUFART,
                                        Werks = r.WERKS,
                                        MatnrH = r.MATNR_H,
                                        GamngH = r.GAMNG_H,
                                        GmeinH = r.GMEIN_H,
                                        Gltrp = r.GLTRP.ToString("yyyyMMdd"),
                                        Gstrp = r.GSTRP.ToString("yyyyMMdd"),
                                        Bldat = r.BLDAT.ToString("yyyyMMdd"),
                                        Budat = r.BUDAT.ToString("yyyyMMdd"),
                                        Lfsnr = r.LFSNR,
                                        BwartH = r.BWART_H,
                                        LgortH = r.LGORT_H,
                                        ErfmgH = r.ERFMG_H,
                                        Zcomnum = r.ZComnum,
                                        LmngaH = r.LMNGA_H,
                                        Mtsnr = r.MTSNR,
                                        Ism = r.ISM,
                                        BwartI = r.BWART_I,
                                        MatnrI = r.MATNR_I,
                                        ErfmgI = r.ERFMG_I,
                                        GmeinI = r.GMEIN_I,
                                        LgortI = r.LGORT_I,
                                        Zmesguid = r.ZMESGUID,
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss")
                                    }).ToList();

                        try
                        {
                            input.ItPpmes0001 = sendRows.ToArray();
                            PPMES0001.ZfunPpmes0001Response rep = new PPMES0001.ZfunPpmes0001Response();
                            rep.ItReturn = new PPMES0001.Bapiret2[10];
                            rep = sapSeviceProxy.ZfunPpmes0001(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_PPMES0001 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "PPMES0001", BusinessConstants.SAPBUSINESSDATA_PPORDER, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输库内移动业务数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "PPMES0001", BusinessConstants.SAPBUSINESSDATA_PPORDER, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
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

        #region 导出生产计划事务信息给sap
        public static object ExportPPMES0002DataLock = new object();
        public List<ErrorMessage> ExportPPMES0002Data()
        {
            lock (ExportPPMES0002DataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPPPMES0002>("from SAPPPMES0002 where Status=0");

                    //根据批次调用接口发送数据
                    //PPMES0002.PPMES0002 PPMES0002 = new PPMES0002.PPMES0002();
                    PPMES0002.ZWS_PPMES0002 sapSeviceProxy = new PPMES0002.ZWS_PPMES0002();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    PPMES0002.ZfunPpmes0002 input = new PPMES0002.ZfunPpmes0002();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();
                    List<PPMES0002.ZstrPpmes0002> sendRows = new List<PPMES0002.ZstrPpmes0002>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new PPMES0002.ZstrPpmes0002
                                    {
                                        Zmessc = r.ZMESSC,
                                        Zmesln = r.ZMESLN,
                                        Zptype = r.ZPTYPE,
                                        Zcomnum = r.ZComnum,
                                        Zmesguid = r.ZMESGUID,
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss"),
                                        Budat = r.CancelDate.ToString("yyyyMMdd"),
                                        Bldat = r.CancelDate.ToString("yyyyMMdd")
                                    }).ToList();

                        try
                        {
                            input.ItPpmes0002 = sendRows.ToArray();
                            sapSeviceProxy.ZfunPpmes0002(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_PPMES0002 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "PPMES0002", BusinessConstants.SAPBUSINESSDATA_PPORDER, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输库内移动业务数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "PPMES0002", BusinessConstants.SAPBUSINESSDATA_PPORDER, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
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

        #region 导出生产计划事务信息给sap
        public static object ExportPPMES0003DataLock = new object();
        public List<ErrorMessage> ExportPPMES0003Data()
        {
            lock (ExportPPMES0003DataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPPPMES0003>("from SAPPPMES0003 where Status=0");

                    //根据批次调用接口发送数据
                    //PPMES0003.PPMES0003 PPMES0003 = new PPMES0003.PPMES0003();
                    PPMES0003.ZWS_PPMES0003 sapSeviceProxy = new PPMES0003.ZWS_PPMES0003();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    PPMES0003.ZfunPpmes0003 input = new PPMES0003.ZfunPpmes0003();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();
                    List<PPMES0003.ZstrPpmes0003> sendRows = new List<PPMES0003.ZstrPpmes0003>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new PPMES0003.ZstrPpmes0003
                                    {
                                        Zmessc = r.ZMESSC,
                                        Zmesln = r.ZMESLN,
                                        Zptype = r.ZPTYPE,
                                        Werks = r.WERKS,
                                        Xmnga = r.XMNGA,
                                        Grund = r.GRUND,
                                        Zcomnum = r.ZComnum,
                                        Bldat = r.BLDAT.ToString("yyyyMMdd"),
                                        Budat = r.BUDAT.ToString("yyyyMMdd"),
                                        Mtsnr = r.MTSNR,
                                        BwartI = r.BWART_I,
                                        MatnrI = r.MATNR_I,
                                        ErfmgI = r.ERFMG_I,
                                        GmeinI = r.GMEIN_I,
                                        LgortI = r.LGORT_I,
                                        Zmesguid = r.ZMESGUID,
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss")
                                    }).ToList();

                        try
                        {
                            input.ItPpmes0003 = sendRows.ToArray();
                            PPMES0003.ZfunPpmes0003Response rep = new PPMES0003.ZfunPpmes0003Response();
                            rep.ItReturn = new PPMES0003.Bapiret2[10];
                            rep = sapSeviceProxy.ZfunPpmes0003(input);
                            sapSeviceProxy.ZfunPpmes0003(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_PPMES0003 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "PPMES0003", BusinessConstants.SAPBUSINESSDATA_PPORDER, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输库内移动业务数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "PPMES0003", BusinessConstants.SAPBUSINESSDATA_PPORDER, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
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

        #region 导出生产计划事务信息给sap
        public static object ExportPPMES0004DataLock = new object();
        public List<ErrorMessage> ExportPPMES0004Data()
        {
            lock (ExportPPMES0004DataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPPPMES0004>("from SAPPPMES0004 where Status=0");

                    //根据批次调用接口发送数据
                    //PPMES0004.PPMES0004 PPMES0004 = new PPMES0004.PPMES0004();
                    PPMES0004.ZWS_PPMES0004 sapSeviceProxy = new PPMES0004.ZWS_PPMES0004();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    PPMES0004.ZfunPpmes0004 input = new PPMES0004.ZfunPpmes0004();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();
                    List<PPMES0004.ZstrPpmes0004> sendRows = new List<PPMES0004.ZstrPpmes0004>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new PPMES0004.ZstrPpmes0004
                                    {
                                        Zmessc = r.ZMESSC,
                                        Zmesln = r.ZMESLN,
                                        Zptype = r.ZPTYPE,
                                        Bldat = r.BLDAT.ToString("yyyyMMdd"),
                                        Budat = r.BUDAT.ToString("yyyyMMdd"),
                                        Grund = r.GRUND,
                                        BwartH = r.BWART_H,
                                        LgortH = r.LGORT_H,
                                        ErfmgH = r.ERFMG_H,
                                        Zcomnum = r.ZComnum,
                                        Lfsnr = r.LFSNR,
                                        LmngaH = r.LMNGA_H,
                                        Xmnga = r.XMNGA,
                                        Zmesguid = r.ZMESGUID,
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss")
                                    }).ToList();

                        try
                        {
                            input.ItPpmes0004 = sendRows.ToArray();
                            sapSeviceProxy.ZfunPpmes0004(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_PPMES0004 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "PPMES0004", BusinessConstants.SAPBUSINESSDATA_PPORDER, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输库内移动业务数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "PPMES0004", BusinessConstants.SAPBUSINESSDATA_PPORDER, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
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

        #region 导出生产计划事务信息给sap
        public static object ExportPPMES0005DataLock = new object();
        public List<ErrorMessage> ExportPPMES0005Data()
        {
            lock (ExportPPMES0005DataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPPPMES0005>("from SAPPPMES0005 where Status=0");

                    //根据批次调用接口发送数据
                    //PPMES0005.PPMES0005 PPMES0005 = new PPMES0005.PPMES0005();
                    PPMES0005.ZWS_PPMES0005 sapSeviceProxy = new PPMES0005.ZWS_PPMES0005();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    PPMES0005.ZfunPpmes0005 input = new PPMES0005.ZfunPpmes0005();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();
                    List<PPMES0005.ZstrPpmes0005> sendRows = new List<PPMES0005.ZstrPpmes0005>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new PPMES0005.ZstrPpmes0005
                                    {
                                        Zmessc = r.ZMESSC,
                                        Zmesln = r.ZMESLN,
                                        Zptype = r.ZPTYPE,
                                        Aufart = r.AUFART,
                                        Werks = r.WERKS,
                                        Matxt = r.MATXT,
                                        GamngH = r.GAMNG_H,
                                        GmeinH = r.GMEIN_H,
                                        Gltrp = r.GLTRP.ToString("yyyyMMdd"),
                                        Gstrp = r.GSTRP.ToString("yyyyMMdd"),
                                        Bldat = r.BLDAT.ToString("yyyyMMdd"),
                                        Budat = r.BUDAT.ToString("yyyyMMdd"),
                                        BwartI = r.BWART_I,
                                        MatnrI = r.MATNR_I,
                                        ErfmgI = r.ERFMG_I,
                                        GmeinI = r.GMEIN_I,
                                        LgortI = r.LGORT_I,
                                        LgortH = r.LGORT_I,
                                        Zmesguid = r.ZMESGUID,
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss")
                                    }).ToList();

                        try
                        {
                            input.ItPpmes0005 = sendRows.ToArray();
                            sapSeviceProxy.ZfunPpmes0005(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_PPMES0005 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "PPMES0005", BusinessConstants.SAPBUSINESSDATA_PPORDER, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输库内移动业务数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "PPMES0005", BusinessConstants.SAPBUSINESSDATA_PPORDER, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
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

        #region 导出生产计划事务信息给sap
        public static object ExportPPMES0006DataLock = new object();
        public List<ErrorMessage> ExportPPMES0006Data()
        {
            lock (ExportPPMES0006DataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPPPMES0006>("from SAPPPMES0006 where Status=0");

                    //根据批次调用接口发送数据
                    //PPMES0006.PPMES0006 PPMES0006 = new PPMES0006.PPMES0006();
                    PPMES0006.ZWS_PPMES0006 sapSeviceProxy = new PPMES0006.ZWS_PPMES0006();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    PPMES0006.ZfunPpmes0006 input = new PPMES0006.ZfunPpmes0006();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();
                    List<PPMES0006.ZstrPpmes0006> sendRows = new List<PPMES0006.ZstrPpmes0006>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new PPMES0006.ZstrPpmes0006
                                    {
                                        Zmessc = r.ZMESSC,
                                        Zmesln = r.ZMESLN,
                                        Zptype = r.ZPTYPE,
                                        Aufnr = r.AUFNR,
                                        Werks = r.WERKS,
                                        Zcomnum = r.ZComnum,
                                        LmngaH = r.LMNGA_H,
                                        Ism = r.ISM,
                                        Bldat = r.BLDAT.ToString("yyyyMMdd"),
                                        Budat = r.BUDAT.ToString("yyyyMMdd"),
                                        Lgort = r.LGORT,
                                        Bwart = r.BWART,
                                        Nplnr = r.NPLNR,
                                        Vornr = r.VORNR,
                                        Rsnum = r.RSNUM,
                                        Rspos = r.RSPOS,
                                        Matnr1 = r.MATNR1,
                                        Mtsnr = r.MTSNR,
                                        Lfsnr = r.LFSNR,
                                        Epfmg = r.EPFMG,
                                        Erfme = r.ERFME,
                                        Zmesguid = r.ZMESGUID,
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss")
                                    }).ToList();

                        try
                        {
                            input.ItPpmes0006 = sendRows.ToArray();
                            sapSeviceProxy.ZfunPpmes0006(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_PPMES0006 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "PPMES0006", BusinessConstants.SAPBUSINESSDATA_PPORDER, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输库内移动业务数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "PPMES0006", BusinessConstants.SAPBUSINESSDATA_PPORDER, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
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
