using System;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Service.MRP;
using com.Sconit.Utility;
using com.Sconit.Entity.SYS;
using System.Net.Mail;
using com.Sconit.Entity;
using com.Sconit.Entity.SI.SAP;
using com.Sconit.Entity.SI;
namespace com.Sconit.Service.SI.Impl
{
    public class BaseMgr
    {
        #region
        public IGenericMgr genericMgr { get; set; }

        public IFinanceCalendarMgr financeCalendarMgr { get; set; }
        public IItemMgr itemMgr { get; set; }
        public IWorkingCalendarMgr workingCalendarMgr { get; set; }
        public IBomMgr bomMgr { get; set; }
        public IFlowMgr flowMgr { get; set; }

        public IPlanMgr planMgr { get; set; }
        public IOrderMgr orderMgr { get; set; }
        public IIpMgr ipMgr { get; set; }
        public IMiscOrderMgr miscOrderMgr { get; set; }
        public IPickListMgr pickListMgr { get; set; }
        public IInspectMgr inspectMgr { get; set; }
        public IReceiptMgr receiptMgr { get; set; }
        public IProductionLineMgr productionLineMgr { get; set; }

        public IHuMgr huMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }

        public IStockTakeMgr stockTakeMgr { get; set; }
        public ICustomizationMgr customizationMgr { get; set; }

        public IEmailMgr emailMgr { get; set; }
        public ISecurityMgr securityMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }

        public NVelocityTemplateRepository vmReporsitory { get; set; }
        #endregion

        #region 静态变量 SAP Web Service的IP和Port
        private static string sapService_IP { get; set; }
        public string SAPService_IP
        {
            get
            {
                if (sapService_IP == null)
                {
                    sapService_IP = this.genericMgr.FindById<com.Sconit.Entity.SYS.EntityPreference>
                           ((int)com.Sconit.Entity.SYS.EntityPreference.CodeEnum.SAPSERVICEIP).Value;
                }

                return sapService_IP;
            }
        }

        private static string sapService_Port { get; set; }
        public string SAPService_Port
        {
            get
            {
                if (sapService_Port == null)
                {
                    sapService_Port = this.genericMgr.FindById<com.Sconit.Entity.SYS.EntityPreference>
                           ((int)com.Sconit.Entity.SYS.EntityPreference.CodeEnum.SAPSERVICEPORT).Value;
                }

                return sapService_Port;
            }
        }

        private static string sapService_TimeOut { get; set; }
        public string SAPService_TimeOut
        {
            get
            {
                if (sapService_TimeOut == null)
                {
                    sapService_TimeOut = this.genericMgr.FindById<com.Sconit.Entity.SYS.EntityPreference>
                           ((int)com.Sconit.Entity.SYS.EntityPreference.CodeEnum.SAPSERVICETIMEOUT).Value;
                }

                return sapService_TimeOut;
            }
        }

        private static string sapService_UserName { get; set; }
        public string SAPService_UserName
        {
            get
            {
                if (sapService_UserName == null)
                {
                    sapService_UserName = this.genericMgr.FindById<com.Sconit.Entity.SYS.EntityPreference>
                           ((int)com.Sconit.Entity.SYS.EntityPreference.CodeEnum.SAPSERVICEUSERNAME).Value;
                }

                return sapService_UserName;
            }
        }

        private static string sapService_Password { get; set; }
        public string SAPService_Password
        {
            get
            {
                if (sapService_Password == null)
                {
                    sapService_Password = this.genericMgr.FindById<com.Sconit.Entity.SYS.EntityPreference>
                           ((int)com.Sconit.Entity.SYS.EntityPreference.CodeEnum.SAPSERVICEPASSWORD).Value;
                }

                return sapService_Password;
            }
        }
        #endregion

        #region 通用方法 发送邮件，获取WebService地址
        protected void SendErrorMessage(IList<com.Sconit.Utility.ErrorMessage> errorMessageList)
        {
            var distinctTemplates = errorMessageList.Select(t => t.Template).Distinct();
            foreach (var nVelocityTemplate in distinctTemplates)
            {
                try
                {
                    LogToUser logToUser = this.genericMgr.FindById<LogToUser>((int)nVelocityTemplate);
                    //MessageSubscribe messageSubscribe = genericMgr.FindById<MessageSubscribe>((int)nVelocityTemplate);
                    var q_ItemErrors = errorMessageList.Where(t => t.Template == nVelocityTemplate);
                    if (!string.IsNullOrWhiteSpace(logToUser.Emails) && !q_ItemErrors.FirstOrDefault().Message.Contains("没有从SAP处获取到任何数据"))
                    {
                        IDictionary<string, object> data = new Dictionary<string, object>();
                        data.Add("Title", logToUser.Descritpion);
                        data.Add("Message", q_ItemErrors.FirstOrDefault().Message);
                        data.Add("StackTrace", q_ItemErrors.FirstOrDefault().Exception == null ? "" : q_ItemErrors.FirstOrDefault().Exception.StackTrace);
                        string content = vmReporsitory.RenderTemplate(logToUser.Template, data);
                        //emailMgr.SetProxyNetworkCredential(this.SAPService_UserName, this.SAPService_Password);
                        emailMgr.AsyncSendEmail(logToUser.Descritpion, content, logToUser.Emails, MailPriority.High);
                    }
                    genericMgr.FlushSession();
                }
                catch (Exception ex)
                {
                    //log.Fatal(ex);
                    genericMgr.CleanSession();
                }
            }
        }

        protected string GetServiceUrl(string originalUrl)
        {
            string[] urlDetails = originalUrl.Split('/');
            string serviceUrl = string.Empty;
            if (!string.IsNullOrEmpty(this.SAPService_IP))
            {
                if (!string.IsNullOrEmpty(this.SAPService_Port))
                {
                    urlDetails[2] = this.SAPService_IP + ":" + this.SAPService_Port;
                }
                else
                {
                    urlDetails[2] = this.SAPService_IP;
                }
            }
            for (int i = 0; i < urlDetails.Length; i++)
            {
                if (i < urlDetails.Length - 1)
                {
                    serviceUrl = serviceUrl + urlDetails[i] + "/";
                }
                else
                {
                    serviceUrl = serviceUrl + urlDetails[i];
                }
            }
            return serviceUrl;
        }
        #endregion

        #region 获取数据到中间表
        public void ExecuteToInternalTable()
        {
            com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
            DateTime currDate = DateTime.Now.AddMinutes(-5);
            //Entity.SI.SAP.SAPTransferTimeControl timeCtrl = this.genericMgr.FindById<Entity.SI.SAP.SAPTransferTimeControl>(0);
            var result = this.genericMgr.FindAllWithNamedQuery<object[]>("USP_IF_ProcessSAPBusinessData", new object[] { currDate }).FirstOrDefault();
            //在存储过程中将中间数据存入到临时表，按顺序执行出现错误抛异常
        }
        #endregion

        #region
        public void SaveTransferLog(string batchNo, string errorMsg, string interf, string sysCode, int status, int rowcount, DateTime? transStartDate, DateTime? dataFromDate, DateTime? dataToDate)
        {
            SAPTransferLog transLog = new SAPTransferLog();
            transLog.BatchNo = batchNo;
            transLog.Interface = interf;
            transLog.Status = status;
            transLog.SysCode = sysCode;
            transLog.TransDate = DateTime.Now;
            transLog.TransStartDate = transStartDate;
            transLog.DataFromDate = dataFromDate;
            transLog.DataToDate = dataToDate;
            transLog.ErrorMsg = errorMsg;
            transLog.RowCounts = rowcount;
            this.genericMgr.Create(transLog);
        }
        #endregion
    }
}
