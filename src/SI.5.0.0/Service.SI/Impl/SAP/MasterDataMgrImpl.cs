using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Utility;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using System.Net;
using com.Sconit.Entity.SI.SAP;
using com.Sconit.Entity.SYS;
using Castle.Services.Transaction;
namespace com.Sconit.Service.SI.SAP.Impl
{
    public class MasterDataMgrImpl : com.Sconit.Service.SI.Impl.BaseMgr, IMasterDataMgr
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("DebugLog");

        #region SAPItem
        private static object ImportItemLock = new object();
        [Transaction(TransactionMode.Requires)]
        public List<ErrorMessage> GetSAPItem(string itemCode, DateTime? reqBeginDate, bool isLoadAll)
        {
            string plant = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.WERKS);
            List<ErrorMessage> errorMessageList = new List<ErrorMessage>();

            DateTime currDate = DateTime.Now.AddMinutes(-5);
            String batchNo = Guid.NewGuid().ToString().Replace("-", "");
            User user = SecurityContextHolder.Get();
            lock (ImportItemLock)
            {
                var timeCtrl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(BusinessConstants.SAPMASTERDATA_ITEM);
                //获取时间的起始值，如果有输入根据输入来获取sap数据
                if (!isLoadAll)
                {
                    if (!reqBeginDate.HasValue)
                    {
                        reqBeginDate = timeCtrl.CurrTransDate;
                    }
                }
                else
                {
                    reqBeginDate = null;
                }
                //初始化sap接口
                MDMMES0001.ZWS_MDMMES0001 sapServiceProxy = new MDMMES0001.ZWS_MDMMES0001();
                sapServiceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                sapServiceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                sapServiceProxy.Url = GetServiceUrl(sapServiceProxy.Url);

                //初始化接口数据
                MDMMES0001.ZfunMdmmes0001 data = new MDMMES0001.ZfunMdmmes0001();
                //data.ItMdmmes0001 = new MDMMES0001.ZstrMdmmes0001[] { };
                if (reqBeginDate.HasValue)
                {
                    data.LDate = reqBeginDate.Value.ToString("yyyyMMdd");
                    data.LTime = reqBeginDate.Value.ToString("HHmmss");
                }
                else
                {
                    data.LDate = string.Empty;
                    data.LTime = string.Empty;
                }
                data.LMatnr = itemCode;
                data.LWerks = plant;
                int getRows = 0;
                bool isGetDataOK=false;
                DateTime? transStartDate=DateTime.Now;
                DateTime? dataFromDate = reqBeginDate;
                DateTime? dataToDate = currDate;
                System.Threading.Thread.Sleep(10000);
                try
                {
                    MDMMES0001.ZfunMdmmes0001Response output = sapServiceProxy.ZfunMdmmes0001(data);
                    isGetDataOK=true;
                    getRows = output.ItMdmmes0001.Count();
                    if (output.ItMdmmes0001.Length > 0)
                    {
                        foreach (var item in output.ItMdmmes0001)
                        {
                            try
                            {
                                Entity.SI.SAP.SAPItem sapItem = new Entity.SI.SAP.SAPItem();
                                sapItem.BISMT = item.Bismt;
                                sapItem.LVORM = item.Lvorm;
                                sapItem.MAKTX = item.Maktx;
                                sapItem.MATKL = item.Matkl;
                                sapItem.MATNR = item.Matnr.ToString().TrimStart('0');
                                sapItem.MEINS = item.Meins;
                                sapItem.MTART = item.Mtart;
                                sapItem.MTBEZ = item.Mtbez;
                                sapItem.SOBSL = item.Sobsl;
                                sapItem.SPART = item.Spart;
                                sapItem.WERKS = item.Werks;
                                sapItem.WGBEZ = item.Wgbez;

                                sapItem.BatchNo = batchNo;
                                sapItem.Status = 0;
                                sapItem.CreateDate = DateTime.Now;
                                this.genericMgr.Create(sapItem);

                                //处理数据，存入中间表
                            }
                            catch (Exception ex)
                            {
                                //失败的话发送邮件
                                //log.Error("llllllllll", ex == null ? new Exception():ex);
                                string logMessage = string.Format("将批次{0}中SAP物料主数据{1}存储中间表储出错。", item.Matnr, batchNo);
                                errorMessageList.Add(new ErrorMessage
                                {
                                    Template = NVelocityTemplateRepository.TemplateEnum.SAPItemErrorTemplate,
                                    Message = logMessage
                                });
                                //以零件号做为批次号记日志
                                this.SaveTransferLog(item.Matnr, logMessage, BusinessConstants.SAPMASTERDATA_ITEM, BusinessConstants.SAPMASTERDATA_ITEM, 2, 1, transStartDate,dataFromDate,dataToDate);
                            }

                        }
                        transStartDate = DateTime.Now;
                        //调用存储过程 根据存储过程是否失败确定执行情况
                        var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessItem", new object[] { user.Id, batchNo }).FirstOrDefault();
                        if ((int)result[0] != 1)
                        {
                            string logMessage = "创建MES物料失败,批次号为：" + result[1].ToString() + "，失败信息：" + result[2].ToString();
                            ////log.Error(logMessage);
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPItemErrorTemplate,
                                Message = logMessage
                            });
                            //记录错误日志
                            this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_ITEM, BusinessConstants.SAPMASTERDATA_ITEM, 2, output.ItMdmmes0001.Count(), transStartDate, dataFromDate, dataToDate);

                        }
                        else
                        {
                            //记录成功日志
                            this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_ITEM, BusinessConstants.SAPMASTERDATA_ITEM, 1, output.ItMdmmes0001.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                    else
                    {
                        this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_ITEM, BusinessConstants.SAPMASTERDATA_ITEM, 1, 0, transStartDate, dataFromDate, dataToDate);
                        if (user.Code.ToUpper() != "MONITOR")
                        {
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPItemErrorTemplate,
                                Message = "没有从SAP处获取到任何数据."
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log.Error("llllllllll", ex == null ? new Exception():ex);
                    string logMessage = "处理Web服务获取SAP物料失败,失败的批次号为:" + batchNo + "。";
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.SAPItemErrorTemplate,
                        Exception = ex,
                        Message = logMessage + (ex.Message ?? "") + (ex.InnerException.Message ?? "")
                    });
                    this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_ITEM, BusinessConstants.SAPMASTERDATA_ITEM, 2, getRows, transStartDate, dataFromDate, dataToDate);
                }
                if (isGetDataOK == true)
                {
                    timeCtrl.LastTransDate = reqBeginDate;
                    timeCtrl.CurrTransDate = currDate;
                    if (user.Code.ToUpper() == "MONITOR") this.genericMgr.Update(timeCtrl);
                }
                this.SendErrorMessage(errorMessageList);
            }
            return errorMessageList;
        }
        #endregion

        #region SAPBom
        private static object ImportBomLock = new object();
        [Transaction(TransactionMode.Requires)]
        public List<ErrorMessage> GetSAPBom(string bom, DateTime? reqBeginDate, bool isLoadAll)
        {
            List<ErrorMessage> errorMessageList = new List<ErrorMessage>();
            User user = SecurityContextHolder.Get();
            DateTime currDate = DateTime.Now.AddMinutes(-5);
            String batchNo = Guid.NewGuid().ToString().Replace("-", "");
            string plant = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.WERKS);
            lock (ImportBomLock)
            {
                //获取时间的起始值，如果有输入根据输入来获取sap数据
                Entity.SI.SAP.SAPTransferTimeControl timeCtrl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(BusinessConstants.SAPMASTERDATA_BOM);
                if (!isLoadAll)
                {
                    if (!reqBeginDate.HasValue)
                    {
                        reqBeginDate = timeCtrl.CurrTransDate;
                    }
                }
                else
                {
                    reqBeginDate = null;
                }

                //初始化sap接口
                MDMMES0002.ZWS_MDMMES0002 sapServiceProxy = new MDMMES0002.ZWS_MDMMES0002();
                sapServiceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                sapServiceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                sapServiceProxy.Url = GetServiceUrl(sapServiceProxy.Url);
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = reqBeginDate;
                DateTime? dataToDate = currDate;
                //初始化接口数据
                MDMMES0002.ZfunMdmmes0002 data = new MDMMES0002.ZfunMdmmes0002();
                //data.ItMdmmes0002 = new MDMMES0002.ZstrMdmmes0002[] { };
                if (reqBeginDate.HasValue)
                {
                    data.LDate = reqBeginDate.Value.ToString("yyyyMMdd");
                    data.LTime = reqBeginDate.Value.ToString("HHmmss");
                }
                else
                {
                    data.LDate = string.Empty;
                    data.LTime = string.Empty;
                }
                data.LBomcode = bom;
                data.LWerks = plant;
                MDMMES0002.ZfunMdmmes0002Response output = sapServiceProxy.ZfunMdmmes0002(data);

                try
                {
                    if (output.ItMdmmes0002.Length > 0)
                    {
                        foreach (var item in output.ItMdmmes0002)
                        {
                            try
                            {
                                Entity.SI.SAP.SAPBom sapBom = new Entity.SI.SAP.SAPBom();
                                sapBom.AUSCH = item.Ausch;
                                sapBom.BMEIN = item.Bmein;
                                sapBom.BMENG = item.Bmeng;
                                sapBom.IDNRK = item.Idnrk.ToString().TrimStart('0');
                                sapBom.MAKTX = item.Maktx;
                                sapBom.MATNR = item.Matnr.ToString().TrimStart('0');
                                sapBom.MEINS = item.Meins;
                                sapBom.MENGE = item.Menge;
                                sapBom.DATUV = item.Datuv;
                                sapBom.VORNR = item.Vornr;

                                sapBom.BatchNo = batchNo;
                                sapBom.Status = 0;
                                sapBom.CreateDate = DateTime.Now;
                                this.genericMgr.Create(sapBom);

                                //处理数据，存入中间表
                            }
                            catch (Exception)
                            {
                                //失败的话发送邮件
                                string logMessage = string.Format("将批次{0}中SAPBom主数据{1}存储中间表储出错。", item.Matnr, batchNo);
                                ////log.Error(logMessage);
                                errorMessageList.Add(new ErrorMessage
                                {
                                    Template = NVelocityTemplateRepository.TemplateEnum.SAPBomErrorTemplate,
                                    Message = logMessage
                                });
                                //以零件号做为批次号记日志
                                this.SaveTransferLog(item.Matnr, logMessage, BusinessConstants.SAPMASTERDATA_BOM, BusinessConstants.SAPMASTERDATA_BOM, 2, 1, transStartDate, dataFromDate, dataToDate);
                            }

                        }
                        transStartDate = DateTime.Now;
                        //调用存储过程 根据存储过程是否失败确定执行情况
                        var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessBom", new object[] { user.Id, batchNo, isLoadAll }).FirstOrDefault();
                        if ((int)result[0] != 1)
                        {
                            string logMessage = "创建MESBom失败,批次号为：" + result[1].ToString() + "，失败信息：" + result[2].ToString();
                            ////log.Error(logMessage);
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPBomErrorTemplate,
                                Message = logMessage
                            });
                            //记录错误日志
                            this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_BOM, BusinessConstants.SAPMASTERDATA_BOM, 2, output.ItMdmmes0002.Count(), transStartDate, dataFromDate, dataToDate);

                        }
                        else
                        {
                            //记录成功日志
                            this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_BOM, BusinessConstants.SAPMASTERDATA_BOM, 1, output.ItMdmmes0002.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                    else
                    {
                        this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_BOM, BusinessConstants.SAPMASTERDATA_BOM, 1, 0, transStartDate, dataFromDate, dataToDate);
                        if (user.Code.ToUpper() != "MONITOR")
                        {
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPBomErrorTemplate,
                                Message = "没有从SAP处获取到任何数据."
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    string logMessage = "处理Web服务获取SAPBom失败,失败的批次号为:" + batchNo + "。";
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.SAPBomErrorTemplate,
                        Exception = ex,
                        Message = logMessage + (ex.Message ?? "") + (ex.InnerException.Message ?? "")
                    });
                    this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_BOM, BusinessConstants.SAPMASTERDATA_BOM, 2, output.ItMdmmes0002.Count(), transStartDate, dataFromDate, dataToDate);
                }
                timeCtrl.LastTransDate = reqBeginDate;
                timeCtrl.CurrTransDate = currDate;
                if (user.Code.ToUpper() == "MONITOR") this.genericMgr.Update(timeCtrl);
                this.SendErrorMessage(errorMessageList);
            }
            return errorMessageList;
        }
        #endregion

        #region SAPUomConv
        private static object ImportUomConvLock = new object();
        [Transaction(TransactionMode.Requires)]
        public List<ErrorMessage> GetSAPUomConv(string itemCode, DateTime? reqBeginDate, bool isLoadAll)
        {
            List<ErrorMessage> errorMessageList = new List<ErrorMessage>();
            User user = SecurityContextHolder.Get();
            DateTime currDate = DateTime.Now.AddMinutes(-5);
            String batchNo = Guid.NewGuid().ToString().Replace("-", "");

            lock (ImportUomConvLock)
            {
                //获取时间的起始值，如果有输入根据输入来获取sap数据
                Entity.SI.SAP.SAPTransferTimeControl timeCtrl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(BusinessConstants.SAPMASTERDATA_UOMCONV);
                if (!isLoadAll)
                {
                    if (!reqBeginDate.HasValue)
                    {
                        reqBeginDate = timeCtrl.CurrTransDate;
                    }
                }
                else
                {
                    reqBeginDate = null;
                }

                //初始化sap接口
                MDMMES0003.ZWS_MDMMES0003 sapServiceProxy = new MDMMES0003.ZWS_MDMMES0003();
                sapServiceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                sapServiceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                sapServiceProxy.Url = GetServiceUrl(sapServiceProxy.Url);
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = reqBeginDate;
                DateTime? dataToDate = currDate;
                //初始化接口数据
                MDMMES0003.ZfunMdmmes0003 data = new MDMMES0003.ZfunMdmmes0003();
                //data.ItMdmmes0003 = new MDMMES0003.ZstrMdmmes0003[] { };
                if (reqBeginDate.HasValue)
                {
                    data.LDate = reqBeginDate.Value.ToString("yyyyMMdd");
                    data.LTime = reqBeginDate.Value.ToString("HHmmss");
                }
                else
                {
                    data.LDate = string.Empty;
                    data.LTime = string.Empty;
                }
                data.LMatnr = itemCode;
                MDMMES0003.ZfunMdmmes0003Response output = sapServiceProxy.ZfunMdmmes0003(data);

                try
                {
                    if (output.ItMdmmes0003.Length > 0)
                    {
                        foreach (var item in output.ItMdmmes0003)
                        {
                            try
                            {
                                Entity.SI.SAP.SAPUomConvertion sapUomConv = new Entity.SI.SAP.SAPUomConvertion();
                                sapUomConv.MATNR = item.Matnr.ToString().TrimStart('0');
                                sapUomConv.MEINH = item.Meinh;
                                sapUomConv.MEINS = item.Meins;
                                sapUomConv.UMREN = item.Umren;
                                sapUomConv.UMREZ = item.Umrez;


                                sapUomConv.BatchNo = batchNo;
                                sapUomConv.Status = 0;
                                sapUomConv.CreateDate = DateTime.Now;
                                this.genericMgr.Create(sapUomConv);

                                //处理数据，存入中间表
                            }
                            catch (Exception)
                            {
                                //失败的话发送邮件
                                string logMessage = string.Format("将批次{0}中SAP单位转换主数据{1}存储中间表储出错。", item.Matnr, batchNo);
                                ////log.Error(logMessage);
                                errorMessageList.Add(new ErrorMessage
                                {
                                    Template = NVelocityTemplateRepository.TemplateEnum.SAPUomConvErrorTemplate,
                                    Message = logMessage
                                });
                                //以零件号做为批次号记日志
                                this.SaveTransferLog(item.Matnr, logMessage, BusinessConstants.SAPMASTERDATA_UOMCONV, BusinessConstants.SAPMASTERDATA_UOMCONV, 2, 1, transStartDate, dataFromDate, dataToDate);
                            }

                        }
                        transStartDate = DateTime.Now;
                        //调用存储过程 根据存储过程是否失败确定执行情况
                        var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessUomConv", new object[] { user.Id, batchNo, isLoadAll }).FirstOrDefault();
                        if ((int)result[0] != 1)
                        {
                            string logMessage = "创建MES单位转换失败,批次号为：" + result[1].ToString() + "，失败信息：" + result[2].ToString();
                            ////log.Error(logMessage);
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPUomConvErrorTemplate,
                                Message = logMessage
                            });
                            //记录错误日志
                            this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_UOMCONV, BusinessConstants.SAPMASTERDATA_UOMCONV, 2, output.ItMdmmes0003.Count(), transStartDate, dataFromDate, dataToDate);

                        }
                        else
                        {
                            //记录成功日志
                            this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_UOMCONV, BusinessConstants.SAPMASTERDATA_UOMCONV, 1, output.ItMdmmes0003.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                    else
                    {
                        this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_UOMCONV, BusinessConstants.SAPMASTERDATA_UOMCONV, 1, 0, transStartDate, dataFromDate, dataToDate);
                        if (user.Code.ToUpper() != "MONITOR")
                        {
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPUomConvErrorTemplate,
                                Message = "没有从SAP处获取到任何数据."
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log.Error("llllllllll", ex == null ? new Exception():ex);
                    string logMessage = "处理Web服务获取SAP单位转换失败(UomConv),失败的批次号为:" + batchNo + "。";
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.SAPUomConvErrorTemplate,
                        Exception = ex,
                        Message = logMessage + (ex.Message ?? "") + (ex.InnerException.Message ?? "")
                    });
                    this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_UOMCONV, BusinessConstants.SAPMASTERDATA_UOMCONV, 2, output.ItMdmmes0003.Count(), transStartDate, dataFromDate, dataToDate);
                }
                timeCtrl.LastTransDate = reqBeginDate;
                timeCtrl.CurrTransDate = currDate;
                if (user.Code.ToUpper() == "MONITOR") this.genericMgr.Update(timeCtrl);
                this.SendErrorMessage(errorMessageList);
            }
            return errorMessageList;
        }
        #endregion

        #region SAPPriceList
        private static object ImportPriceListvLock = new object();
        [Transaction(TransactionMode.Requires)]
        public List<ErrorMessage> GetSAPPriceList(string itemCode, string supplierCode, DateTime? reqBeginDate, bool isLoadAll)
        {
            List<ErrorMessage> errorMessageList = new List<ErrorMessage>();
            User user = SecurityContextHolder.Get();
            DateTime currDate = DateTime.Now.AddMinutes(-5);
            String batchNo = Guid.NewGuid().ToString().Replace("-", "");
            string plant = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.WERKS);
            string purchaseOrg = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.EKORG);
            lock (ImportPriceListvLock)
            {
                //获取时间的起始值，如果有输入根据输入来获取sap数据
                Entity.SI.SAP.SAPTransferTimeControl timeCtrl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(BusinessConstants.SAPMASTERDATA_PRICELIST);
                if (!isLoadAll)
                {
                    if (!reqBeginDate.HasValue)
                    {
                        reqBeginDate = timeCtrl.CurrTransDate;
                    }
                }
                else
                {
                    reqBeginDate = null;
                }

                //初始化sap接口
                MDMMES0004.ZWS_MDMMES0004 sapServiceProxy = new MDMMES0004.ZWS_MDMMES0004();
                sapServiceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                sapServiceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                sapServiceProxy.Url = GetServiceUrl(sapServiceProxy.Url);
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = reqBeginDate;
                DateTime? dataToDate = currDate;
                //初始化接口数据
                MDMMES0004.ZfunMdmmes0004 data = new MDMMES0004.ZfunMdmmes0004();
                //data.ItMdmmes0004 = new MDMMES0004.ZstrMdmmes0004[] { };
                if (reqBeginDate.HasValue)
                {
                    data.LDate = reqBeginDate.Value.ToString("yyyyMMdd");
                }
                else
                {
                    data.LDate = string.Empty;
                }
                data.LMatnr = itemCode;
                data.LLifnr = supplierCode;
                data.LWerks = plant;
                data.LEkorg = purchaseOrg;
                MDMMES0004.ZfunMdmmes0004Response output = sapServiceProxy.ZfunMdmmes0004(data);

                try
                {
                    if (output.ItMdmmes0004.Length > 0)
                    {
                        foreach (var item in output.ItMdmmes0004)
                        {
                            try
                            {
                                Entity.SI.SAP.SAPPriceList sapPriceList = new Entity.SI.SAP.SAPPriceList();
                                sapPriceList.MATNR = item.Matnr.ToString().TrimStart('0');
                                sapPriceList.BPRME = item.Bprme;
                                sapPriceList.ERDAT = item.Erdat;
                                sapPriceList.LIFNR = item.Lifnr.ToString().TrimStart('0');
                                sapPriceList.MATNR = item.Matnr.ToString().TrimStart('0');
                                sapPriceList.MWSKZ = item.Mwskz;
                                sapPriceList.NETPR = item.Netpr;
                                sapPriceList.NORMB = item.Normb;
                                sapPriceList.PRDAT = item.Prdat;
                                sapPriceList.WAERS = item.Waers;

                                sapPriceList.BatchNo = batchNo;
                                sapPriceList.Status = 0;
                                sapPriceList.CreateDate = DateTime.Now;
                                this.genericMgr.Create(sapPriceList);

                                //处理数据，存入中间表
                            }
                            catch (Exception)
                            {
                                //失败的话发送邮件
                                string logMessage = string.Format("将批次{0}中SAP采购价格单主数据{1}存储中间表储出错。", item.Matnr, batchNo);
                                ////log.Error(logMessage);
                                errorMessageList.Add(new ErrorMessage
                                {
                                    Template = NVelocityTemplateRepository.TemplateEnum.SAPPriceListErrorTemplate,
                                    Message = logMessage
                                });
                                //以零件号做为批次号记日志
                                this.SaveTransferLog(item.Matnr, logMessage, BusinessConstants.SAPMASTERDATA_PRICELIST, BusinessConstants.SAPMASTERDATA_PRICELIST, 2, 1, transStartDate, dataFromDate, dataToDate);
                            }

                        }

                        //调用存储过程 根据存储过程是否失败确定执行情况
                        //log.Error(user!=null?user.Id.ToString():"id11111111");
                        transStartDate = DateTime.Now;
                        var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessPriceList", new object[] { user.Id, batchNo }).FirstOrDefault();
                        if ((int)result[0] != 1)
                        {
                            string logMessage = "创建MES价格单失败,批次号为：" + result[1].ToString() + "，失败信息：" + result[2].ToString();
                            ////log.Error(logMessage);
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPPriceListErrorTemplate,
                                Message = logMessage
                            });
                            //记录错误日志
                            this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_PRICELIST, BusinessConstants.SAPMASTERDATA_PRICELIST, 2, output.ItMdmmes0004.Count(), transStartDate, dataFromDate, dataToDate);

                        }
                        else
                        {
                            //记录成功日志
                            this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_PRICELIST, BusinessConstants.SAPMASTERDATA_PRICELIST, 1, output.ItMdmmes0004.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                    else
                    {
                        this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_PRICELIST, BusinessConstants.SAPMASTERDATA_PRICELIST, 1, 0, transStartDate, dataFromDate, dataToDate);
                        if (user.Code.ToUpper() != "MONITOR")
                        {
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPPriceListErrorTemplate,
                                Message = "没有从SAP处获取到任何数据."
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log.Error("llllllllllPriceList", ex == null ? new Exception() : ex);
                    //log.Error("msg" + (ex.Message ?? "") + "Innermsg"+(ex.InnerException.Message ?? ""));
                    string logMessage = "处理Web服务获取SAP单位转换失败(PriceList),失败的批次号为:" + batchNo + "。";
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.SAPPriceListErrorTemplate,
                        Exception = ex,
                        Message = logMessage + (ex.Message ?? "") + (ex.InnerException.Message ?? "")
                    });
                    this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_PRICELIST, BusinessConstants.SAPMASTERDATA_PRICELIST, 2, output.ItMdmmes0004.Count(), transStartDate, dataFromDate, dataToDate);
                }
                //log.Error("7777777");
                timeCtrl.LastTransDate = reqBeginDate;
                timeCtrl.CurrTransDate = currDate;
                if (user.Code.ToUpper() == "MONITOR") this.genericMgr.Update(timeCtrl);
                this.SendErrorMessage(errorMessageList);
            }
            return errorMessageList;
        }
        #endregion

        #region SAPSupplier
        private static object ImportSupplierLock = new object();
        [Transaction(TransactionMode.Requires)]
        public List<ErrorMessage> GetSAPSupplier(string supplier, DateTime? reqBeginDate, bool isLoadAll)
        {
            List<ErrorMessage> errorMessageList = new List<ErrorMessage>();
            User user = SecurityContextHolder.Get();
            DateTime currDate = DateTime.Now.AddMinutes(-5);
            String batchNo = Guid.NewGuid().ToString().Replace("-", "");
            string firm = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.BUKRS);
            lock (ImportSupplierLock)
            {
                //获取时间的起始值，如果有输入根据输入来获取sap数据
                Entity.SI.SAP.SAPTransferTimeControl timeCtrl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(BusinessConstants.SAPMASTERDATA_SUPPLIER);
                if (!isLoadAll)
                {
                    if (!reqBeginDate.HasValue)
                    {
                        reqBeginDate = timeCtrl.CurrTransDate;
                    }
                }
                else
                {
                    reqBeginDate = null;
                }
                //初始化sap接口
                MDMMES0005.ZWS_MDMMES0005 sapServiceProxy = new MDMMES0005.ZWS_MDMMES0005();
                sapServiceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                sapServiceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                sapServiceProxy.Url = GetServiceUrl(sapServiceProxy.Url);
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = reqBeginDate;
                DateTime? dataToDate = currDate;
                //初始化接口数据
                MDMMES0005.ZfunMdmmes0005 data = new MDMMES0005.ZfunMdmmes0005();
                //data.ItMdmmes0005 = new MDMMES0005.ZstrMdmmes0005[] { };
                if (reqBeginDate.HasValue)
                {
                    data.LDate = reqBeginDate.Value.ToString("yyyyMMdd");
                    data.LTime = reqBeginDate.Value.ToString("HHmmss");
                }
                else
                {
                    data.LDate = string.Empty;
                    data.LTime = string.Empty;
                }
                data.LLifnr = supplier;
                data.LBukrs = firm;
                MDMMES0005.ZfunMdmmes0005Response output = sapServiceProxy.ZfunMdmmes0005(data);

                try
                {
                    if (output.ItMdmmes0005.Length > 0)
                    {
                        foreach (var item in output.ItMdmmes0005)
                        {
                            try
                            {
                                Entity.SI.SAP.SAPSupplier sapSupplier = new Entity.SI.SAP.SAPSupplier();
                                sapSupplier.LIFNR = item.Lifnr.ToString().TrimStart('0');
                                sapSupplier.NAME1 = item.Name1;
                                sapSupplier.BUKRS = item.Bukrs;
                                sapSupplier.COUNTRY = item.Country;
                                sapSupplier.CITY = item.City;
                                sapSupplier.REMARK = item.Remark;
                                sapSupplier.TELF1 = item.Telf1;
                                sapSupplier.TELFX = item.Telfx;
                                sapSupplier.PARNR = item.Parnr;
                                sapSupplier.PSTLZ = item.Pstlz;
                                sapSupplier.TELBX = item.Telbx;
                                sapSupplier.TELF2 = item.Telf2;
                                sapSupplier.EKGRP = item.Ekgrp;
                                sapSupplier.LOEVM = item.Loevm;

                                sapSupplier.BatchNo = batchNo;
                                sapSupplier.Status = 0;
                                sapSupplier.CreateDate = DateTime.Now;
                                this.genericMgr.Create(sapSupplier);

                                //处理数据，存入中间表
                            }
                            catch (Exception)
                            {
                                //失败的话发送邮件
                                string logMessage = string.Format("将批次{0}中SAPSupplier主数据{1}存储中间表储出错。", item.Lifnr, batchNo);
                                ////log.Error(logMessage);
                                errorMessageList.Add(new ErrorMessage
                                {
                                    Template = NVelocityTemplateRepository.TemplateEnum.SAPSupplierErrorTemplate,
                                    Message = logMessage
                                });
                                //以供应商代码号做为批次号记日志
                                this.SaveTransferLog(item.Lifnr, logMessage, BusinessConstants.SAPMASTERDATA_SUPPLIER, BusinessConstants.SAPMASTERDATA_SUPPLIER, 2, 1, transStartDate, dataFromDate, dataToDate);
                            }

                        }
                        transStartDate = DateTime.Now;
                        //调用存储过程 根据存储过程是否失败确定执行情况
                        var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessSupplier", new object[] { user.Id, batchNo }).FirstOrDefault();
                        if ((int)result[0] != 1)
                        {
                            string logMessage = "创建MESSupplier失败,批次号为：" + result[1].ToString() + "，失败信息：" + result[2].ToString();
                            ////log.Error(logMessage);
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPSupplierErrorTemplate,
                                Message = logMessage
                            });
                            //记录错误日志
                            this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_SUPPLIER, BusinessConstants.SAPMASTERDATA_SUPPLIER, 2, output.ItMdmmes0005.Count(), transStartDate, dataFromDate, dataToDate);

                        }
                        else
                        {
                            //记录成功日志
                            this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_SUPPLIER, BusinessConstants.SAPMASTERDATA_SUPPLIER, 1, output.ItMdmmes0005.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                    else
                    {
                        this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_SUPPLIER, BusinessConstants.SAPMASTERDATA_SUPPLIER, 1, 0, transStartDate, dataFromDate, dataToDate);
                        if (user.Code.ToUpper() != "MONITOR")
                        {
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPSupplierErrorTemplate,
                                Message = "没有从SAP处获取到任何数据."
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log.Error("llllllllll", ex == null ? new Exception():ex);
                    string logMessage = "处理Web服务获取SAPSupplier失败,失败的批次号为:" + batchNo + "。";
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.SAPSupplierErrorTemplate,
                        Exception = ex,
                        Message = logMessage + (ex.Message ?? "") + (ex.InnerException.Message ?? "")
                    });
                    this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_SUPPLIER, BusinessConstants.SAPMASTERDATA_SUPPLIER, 2, output.ItMdmmes0005.Count(), transStartDate, dataFromDate, dataToDate);
                }
                timeCtrl.LastTransDate = reqBeginDate;
                timeCtrl.CurrTransDate = currDate;
                if (user.Code.ToUpper() == "MONITOR") this.genericMgr.Update(timeCtrl);
                this.SendErrorMessage(errorMessageList);
            }
            return errorMessageList;
        }
        #endregion

        #region SAPCustomer
        private static object ImportCustomerLock = new object();
        [Transaction(TransactionMode.Requires)]
        public List<ErrorMessage> GetSAPCustomer(string customer, DateTime? reqBeginDate, bool isLoadAll)
        {
            List<ErrorMessage> errorMessageList = new List<ErrorMessage>();
            User user = SecurityContextHolder.Get();
            DateTime currDate = DateTime.Now.AddMinutes(-5);
            String batchNo = Guid.NewGuid().ToString().Replace("-", "");
            string firm = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.BUKRS);
            lock (ImportCustomerLock)
            {
                //获取时间的起始值，如果有输入根据输入来获取sap数据
                Entity.SI.SAP.SAPTransferTimeControl timeCtrl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(BusinessConstants.SAPMASTERDATA_CUSTOMER);
                if (!isLoadAll)
                {
                    if (!reqBeginDate.HasValue)
                    {
                        reqBeginDate = timeCtrl.CurrTransDate;
                    }
                }
                else
                {
                    reqBeginDate = null;
                }

                //初始化sap接口
                MDMMES0006.ZWS_MDMMES0006 sapServiceProxy = new MDMMES0006.ZWS_MDMMES0006();
                sapServiceProxy.Credentials = new NetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                sapServiceProxy.Timeout = int.Parse(this.SAPService_TimeOut);
                sapServiceProxy.Url = GetServiceUrl(sapServiceProxy.Url);
                DateTime? transStartDate = DateTime.Now;
                DateTime? dataFromDate = reqBeginDate;
                DateTime? dataToDate = currDate;
                //初始化接口数据
                MDMMES0006.ZfunMdmmes0006 data = new MDMMES0006.ZfunMdmmes0006();
                //data.ItMdmmes0006 = new MDMMES0006.ZstrMdmmes0006[] { };
                if (reqBeginDate.HasValue)
                {
                    data.LDate = reqBeginDate.Value.ToString("yyyyMMdd");
                    data.LTime = reqBeginDate.Value.ToString("HHmmss");
                }
                else
                {
                    data.LDate = string.Empty;
                    data.LTime = string.Empty;
                }
                data.LKunnr = customer;
                data.LBukrs = firm;
                MDMMES0006.ZfunMdmmes0006Response output = sapServiceProxy.ZfunMdmmes0006(data);

                try
                {
                    if (output.ItMdmmes0006.Length > 0)
                    {
                        foreach (var item in output.ItMdmmes0006)
                        {
                            try
                            {
                                Entity.SI.SAP.SAPCustomer sapCustomer = new Entity.SI.SAP.SAPCustomer();
                                sapCustomer.KUNNR = item.Kunnr.ToString().TrimStart('0');
                                sapCustomer.NAME1 = item.Name1;
                                sapCustomer.BUKRS = item.Bukrs;
                                sapCustomer.COUNTRY = item.Country;
                                sapCustomer.CITY = item.City;
                                sapCustomer.REMARK = item.Remark;
                                sapCustomer.TELF1 = item.Telf1;
                                sapCustomer.TELFX = item.Telfx;
                                sapCustomer.PARNR = item.Parnr;
                                sapCustomer.PSTLZ = item.Pstlz;
                                sapCustomer.TELBX = item.Telbx;
                                sapCustomer.TELF2 = item.Telf2;
                                sapCustomer.LOEVM = item.Loevm;

                                sapCustomer.BatchNo = batchNo;
                                sapCustomer.Status = 0;
                                sapCustomer.CreateDate = DateTime.Now;
                                this.genericMgr.Create(sapCustomer);

                                //处理数据，存入中间表
                            }
                            catch (Exception)
                            {
                                //失败的话发送邮件
                                string logMessage = string.Format("将批次{0}中SAPCustomer主数据{1}存储中间表储出错。", item.Kunnr, batchNo);
                                ////log.Error(logMessage);
                                errorMessageList.Add(new ErrorMessage
                                {
                                    Template = NVelocityTemplateRepository.TemplateEnum.SAPCustomertErrorTemplate,
                                    Message = logMessage
                                });
                                //以客户代码号做为批次号记日志
                                this.SaveTransferLog(item.Kunnr, logMessage, BusinessConstants.SAPMASTERDATA_CUSTOMER, BusinessConstants.SAPMASTERDATA_CUSTOMER, 2, 1, transStartDate, dataFromDate, dataToDate);
                            }

                        }
                        transStartDate = DateTime.Now;
                        //调用存储过程 根据存储过程是否失败确定执行情况
                        var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_SAP_ProcessCustomer", new object[] { user.Id, batchNo }).FirstOrDefault();
                        if ((int)result[0] != 1)
                        {
                            string logMessage = "创建MESCustomer失败,批次号为：" + result[1].ToString() + "，失败信息：" + result[2].ToString();
                            ////log.Error(logMessage);
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPCustomertErrorTemplate,
                                Message = logMessage
                            });
                            //记录错误日志
                            this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_CUSTOMER, BusinessConstants.SAPMASTERDATA_CUSTOMER, 2, output.ItMdmmes0006.Count(), transStartDate, dataFromDate, dataToDate);

                        }
                        else
                        {
                            //记录成功日志
                            this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_CUSTOMER, BusinessConstants.SAPMASTERDATA_CUSTOMER, 1, output.ItMdmmes0006.Count(), transStartDate, dataFromDate, dataToDate);
                        }
                    }
                    else
                    {
                        this.SaveTransferLog(batchNo, "SUCCESS", BusinessConstants.SAPMASTERDATA_CUSTOMER, BusinessConstants.SAPMASTERDATA_CUSTOMER, 1, 0, transStartDate, dataFromDate, dataToDate);
                        if (user.Code.ToUpper() != "MONITOR")
                        {
                            errorMessageList.Add(new ErrorMessage
                            {
                                Template = NVelocityTemplateRepository.TemplateEnum.SAPCustomertErrorTemplate,
                                Message = "没有从SAP处获取到任何数据."
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log.Error("llllllllll", ex == null ? new Exception():ex);
                    string logMessage = "处理Web服务获取SAPCustomer失败,失败的批次号为:" + batchNo + "。";
                    errorMessageList.Add(new ErrorMessage
                    {
                        Template = NVelocityTemplateRepository.TemplateEnum.SAPCustomertErrorTemplate,
                        Exception = ex,
                        Message = logMessage + (ex.Message ?? "") + (ex.InnerException.Message ?? "")
                    });
                    this.SaveTransferLog(batchNo, logMessage, BusinessConstants.SAPMASTERDATA_CUSTOMER, BusinessConstants.SAPMASTERDATA_CUSTOMER, 2, output.ItMdmmes0006.Count(), transStartDate, dataFromDate, dataToDate);
                }
                timeCtrl.LastTransDate = reqBeginDate;
                timeCtrl.CurrTransDate = currDate;
                if (user.Code.ToUpper() == "MONITOR") this.genericMgr.Update(timeCtrl);
                this.SendErrorMessage(errorMessageList);
            }
            return errorMessageList;
        }
        #endregion

    }
}
