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

namespace com.Sconit.Service.SI.SAP.Impl
{
    public class SalesDistributionMgrImpl : com.Sconit.Service.SI.Impl.BaseMgr, ISalesDistributionMgr
    {
        #region 导出库存快照给sap
        public static object ExportMESQty0001Lock = new object();
        public List<ErrorMessage> ExportMESQTY0001()
        {
            lock (ExportMESQty0001Lock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {

                    //获取状态是0的数据按时间排序
                    DateTime mesInvSnap= this.genericMgr.FindAllWithNativeSql<DateTime>("Select MAX(MesInvSnaptime) from Inv_SIExecution_SnapTime ").FirstOrDefault();
                    var result = this.genericMgr.FindAllIn<SAPSnapShotInv>("from SAPSnapShotInv where MesTotalInv <> '0' and  MesInvSnaptime in (? ", new object[] { mesInvSnap });
                    //根据批次调用接口发送数据
                    MESQTY0001.ZWS_MESQTY0001 sapSeviceProxy = new MESQTY0001.ZWS_MESQTY0001();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    MESQTY0001.ZfunMesqty0001 input = new MESQTY0001.ZfunMesqty0001();

                    List<MESQTY0001.ZstrMesqty0001> sendRows = new List<MESQTY0001.ZstrMesqty0001>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    sendRows = (from r in result
                                select new MESQTY0001.ZstrMesqty0001
                                {
                                    Mesinvsnaptime = r.MesInvSnaptime.ToString("yyyyMMddHHmmss"),
                                    Lgort=r.LGORT,
                                    Matnr=r.MATNR,
                                    Lifnr=r.LIFNR,
                                    Mescsqty=r.MesCsQty,
                                    Mesinspectqty=r.MesInspectQty,
                                    Mesqualifiedqty=r.MesQualifiedQty,
                                    Mesrejectqty=r.MesRejectQty,
                                    Messalesipqty=r.MesSalesIpQty,
                                    Mestotalinv=r.MesTotalInv,
                                    Mestransipqty=r.MesTransIpQty,
                                    Werks=r.WERKS
                                }).ToList();

                    try
                    {
                        input.ItMesqty0001 = sendRows.ToArray();
                        sapSeviceProxy.ZfunMesqty0001(input);
                        this.SaveTransferLog(mesInvSnap.ToString("yyyyMMddHHmmss"), "SUCCESS", "MESQTY0001", "SnapShot", 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                    }
                    catch (Exception ex)
                    {
                        string logMessage = "传输MES库存信息数据给SAP时失败,快照为：" + mesInvSnap.ToString("yyyyMMddHHmmss") + "，失败信息：" + ex.Message;
                        errorMessageList.Add(new ErrorMessage
                        {
                            Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                            Message = logMessage
                        });
                        this.SaveTransferLog(mesInvSnap.ToString("yyyyMMddHHmmss"), logMessage, "MESQTY0001", "SnapShot", 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                    }
                }
                catch (Exception ex)
                {
                    string logMessage = "导出MES库存信息数据给SAP出错。";
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
        #region 导出销售订单信息给sap
        public static object ExportSDMES0001Lock = new object();
        public List<ErrorMessage> ExportSDMES0001()
        {
            lock (ExportSDMES0001Lock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPSDMES0001>("from SAPSDMES0001 where status=0");
                    //重新调用
                    //if (!string.IsNullOrWhiteSpace(UniqueCode))
                    //{
                    //    result = this.genericMgr.FindAll<SAPSDMES0001>("from SAPSDMES0001 where UniqueCode = ? ", UniqueCode);
                    //}
                    //根据批次调用接口发送数据
                    SDMES0001.ZWS_SDMES0001 sapSeviceProxy = new SDMES0001.ZWS_SDMES0001();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    SDMES0001.ZfunSdmes0001 input = new SDMES0001.ZfunSdmes0001();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();

                    List<SDMES0001.ZstrSdmes0006> sendRows = new List<SDMES0001.ZstrSdmes0006>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new SDMES0001.ZstrSdmes0006
                                    {
                                        DistrChan = r.DISTRCHAN,
                                        Division = r.DIVISION,
                                        DocDate = r.DOCDATE.ToString("yyyyMMdd"),
                                        DocType = r.DOCTYPE,
                                        ItmNumber =  r.ITMNUMBER,
                                        Lgort = r.LGORT,
                                        Lifex = r.LIFEX,
                                        Material = r.MATERIAL,
                                        OrdReason = r.ORDREASON,
                                        PartnNumb = r.PARTNNUMB,
                                        PriceDate = r.PRICEDATE.ToString("yyyyMMdd"),
                                        SalesOrg = r.SALESORG,
                                        TargetQty = r.TARGETQTY,
                                        Vrkme = r.VRKME,
                                        WadatIst = r.WADATIST.ToString("yyyyMMdd"),
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss"),
                                        Zmesguid = r.ZMESGUID,
                                        Zmesso = r.ZMESSO,
                                        Zmessoseq = r.ZMESSOSEQ,
                                        Saleordno = r.SALEORDNO

                                    }).ToList();

                        try
                        {
                            input.ItData = sendRows.ToArray();
                            sapSeviceProxy.ZfunSdmes0001(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_SDMES0001 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "SDMES0001", BusinessConstants.SAPBUSINESSDATA_SDORDER, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输销售订单数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "SDMES0001", BusinessConstants.SAPBUSINESSDATA_SDORDER, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string logMessage = "导出销售订单数据给SAP出错。";
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

        #region 导出销售订单冲销信息给sap
        public static object ExportSDMES0002Lock = new object();
        public List<ErrorMessage> ExportSDMES0002()
        {
            lock (ExportSDMES0002Lock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {

                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAll<SAPSDMES0002>("from SAPSDMES0002 where status=0");
                    //重新调用
                    //if (!string.IsNullOrWhiteSpace(UniqueCode))
                    //{
                    //    result = this.genericMgr.FindAll<SAPSDMES0002>("from SAPSDMES0002 where UniqueCode = ? ", UniqueCode);
                    //}
                    //根据批次调用接口发送数据
                    SDMES0002.ZWS_SDMES0002 sapSeviceProxy = new SDMES0002.ZWS_SDMES0002();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    SDMES0002.ZfunSdmes0002 input = new SDMES0002.ZfunSdmes0002();
                    List<string> uniqueCodeList = result.Select(r => r.UniqueCode).Distinct().ToList();

                    List<SDMES0002.ZstrSdmes0007> sendRows = new List<SDMES0002.ZstrSdmes0007>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    foreach (var uniqueCode in uniqueCodeList)
                    {
                        transStartDate = DateTime.Now;
                        sendRows = (from r in result
                                    where r.UniqueCode == uniqueCode
                                    select new SDMES0002.ZstrSdmes0007
                                    {
                                        Zcsrqsj = r.ZCSRQSJ.ToString("yyyyMMddHHmmss"),
                                        Zmesguid = r.ZMESGUID,
                                        Zmesso = r.ZMESSO

                                    }).ToList();

                        try
                        {
                            input.ItHeader = sendRows.ToArray();
                            sapSeviceProxy.ZfunSdmes0002(input);
                            this.genericMgr.FindAllWithNativeSql("update SAP_Interface_SDMES0002 set status=1 where UniqueCode=?", uniqueCode);
                            this.SaveTransferLog(uniqueCode, "SUCCESS", "SDMES0002", BusinessConstants.SAPBUSINESSDATA_SDORDER, 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                        catch (Exception ex)
                        {
                            string logMessage = "传输销售订单冲销数据给SAP时失败,批次号为：" + uniqueCode + "，失败信息：" + ex.Message;
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                                Message = logMessage
                            });
                            this.SaveTransferLog(uniqueCode, logMessage, "SDMES0002", BusinessConstants.SAPBUSINESSDATA_SDORDER, 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string logMessage = "导出销售订单冲销数据给SAP出错。";
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
        #region 生成销售接口数据
        public static object GenSDMESDataLock = new object();
        public List<ErrorMessage> GenSDMESData()
        {
            lock (GenSDMESDataLock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {
                    String batchNo = Guid.NewGuid().ToString().Replace("-", "");
                    DateTime currDate = DateTime.Now.AddMinutes(-5);
                    var timeCtrl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(BusinessConstants.SAPMASTERDATA_ITEM);
                    var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ExportSalesOrder", new object[] { batchNo, timeCtrl.CurrTransDate, currDate }).FirstOrDefault();
                    if ((int)result[0] != 1)
                    {
                        string logMessage = "创建销售订单接口数据失败,批次号为：" + result[1].ToString() + "，失败信息：" + result[2].ToString();
                        //log.Error(logMessage);
                        errorMessageList.Add(new ErrorMessage
                        {
                            Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                            Message = logMessage
                        });
                        //记录错误日志
                        //this.SaveTransferLog(batchNo, logMessage, "SDMES0001", BusinessConstants.SAPBUSINESSDATA_SDORDER, 2, 1);

                    }
                    else
                    {
                        //记录成功日志
                        //this.SaveTransferLog(batchNo, "SUCCESS", "SDMES0001", BusinessConstants.SAPBUSINESSDATA_SDORDER, 1, 1);
                    }
                }
                catch (Exception ex)
                {
                    string logMessage = "生成销售订单接口数据出错。";
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

        public List<ErrorMessage> ExportMESQTY0001(DateTime mesInvSnap)
        {
            lock (ExportMESQty0001Lock)
            {
                var errorMessageList = new List<ErrorMessage>();

                try
                {

                    //获取状态是0的数据按时间排序
                    var result = this.genericMgr.FindAllIn<SAPSnapShotInv>("from SAPSnapShotInv where MesTotalInv <> '0' and  MesInvSnaptime in (? ", new object[] { mesInvSnap });
                    //根据批次调用接口发送数据
                    MESQTY0001.ZWS_MESQTY0001 sapSeviceProxy = new MESQTY0001.ZWS_MESQTY0001();
                    sapSeviceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                    sapSeviceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                    sapSeviceProxy.Url = GetServiceUrl(sapSeviceProxy.Url);
                    MESQTY0001.ZfunMesqty0001 input = new MESQTY0001.ZfunMesqty0001();

                    List<MESQTY0001.ZstrMesqty0001> sendRows = new List<MESQTY0001.ZstrMesqty0001>();
                    DateTime? transStartDate = DateTime.Now;
                    DateTime? dataFromDate = null;
                    DateTime? dataToDate = null;
                    sendRows = (from r in result
                                select new MESQTY0001.ZstrMesqty0001
                                {
                                    Mesinvsnaptime = r.MesInvSnaptime.ToString("yyyyMMddHHmmss"),
                                    Lgort=r.LGORT,
                                    Matnr=r.MATNR,
                                    Lifnr=r.LIFNR,
                                    Mescsqty=r.MesCsQty,
                                    Mesinspectqty=r.MesInspectQty,
                                    Mesqualifiedqty=r.MesQualifiedQty,
                                    Mesrejectqty=r.MesRejectQty,
                                    Messalesipqty=r.MesSalesIpQty,
                                    Mestotalinv=r.MesTotalInv,
                                    Mestransipqty=r.MesTransIpQty,
                                    Werks=r.WERKS
                                }).ToList();

                    try
                    {
                        input.ItMesqty0001 = sendRows.ToArray();
                        sapSeviceProxy.ZfunMesqty0001(input);
                        //this.SaveTransferLog(mesInvSnap.ToString("yyyyMMddHHmmss"), "SUCCESS", "MESQTY0001", "SnapShot", 1, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                    }
                    catch (Exception ex)
                    {
                        string logMessage = "传输MES库存信息数据给SAP时失败,快照为：" + mesInvSnap.ToString("yyyyMMddHHmmss") + "，失败信息：" + ex.Message;
                        errorMessageList.Add(new ErrorMessage
                        {
                            Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                            Message = logMessage
                        });
                        //this.SaveTransferLog(mesInvSnap.ToString("yyyyMMddHHmmss"), logMessage, "MESQTY0001", "SnapShot", 2, sendRows.Count(), transStartDate, dataFromDate, dataToDate);
                    }
                }
                catch (Exception ex)
                {
                    string logMessage = "导出MES库存信息数据给SAP出错。";
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.ExportBusDataToSAPErrorTemplate,
                        Exception = ex,
                        Message = logMessage
                    });
                }

                //this.SendErrorMessage(errorMessageList);
                return errorMessageList;
            }
        }
    }
}
