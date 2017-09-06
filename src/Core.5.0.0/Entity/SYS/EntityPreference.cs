using System;
using System.ComponentModel.DataAnnotations;
//TODO: Add other using statements here

namespace com.Sconit.Entity.SYS
{
    public partial class EntityPreference
    {
        #region Non O/R Mapping Properties

        public enum CodeEnum
        {
            DefaultPageSize = 10001,
            SessionCachedSearchStatementCount = 10002,
            ItemFilterMode = 10003,
            ItemFilterMinimumChars = 10004,
            InProcessIssueWaitingTime = 10005,
            CompleteIssueWaitingTime = 10006,
            SMTPEmailAddr = 10007,
            SMTPEmailHost = 10008,
            SMTPEmailPasswd = 10009,
            IsRecordLocatoinTransactionDetail = 10010,
            DecimalLength = 10011,
            DefaultPickStrategy = 10012,
            SIWebServiceUserName = 10013,
            SIWebServicePassword = 10014,
            SIWebServiceTimeOut = 10015,
            AllowManualCreateProcurementOrder = 10016,
            //WMSAnjiRegion = 10017,
            MaxRowSizeOnPage = 10018,
            DefaultBarCodeTemplate = 10019,
            ExceptionMailTo = 11020,
            IsAllowCreatePurchaseOrderWithNoPrice = 11021,
            IsAllowCreateSalesOrderWithNoPrice = 110210,
            GridDefaultMultiRowsCount = 11022,
            SAPSERVICEUSERNAME = 11001,
            SAPSERVICEPASSWORD = 11002,
            SAPSERVICETIMEOUT = 11003,
            SAPSERVICEIP = 11004,
            SAPSERVICEPORT = 11005,
            ProdLineWarningColors = 20001,
            MiCleanTime = 20002,
            MiFilterCapacity = 20003,
            MiContainerLocations = 20004,
            HistoryPasswordCount = 20007,  //N次之内密码不能重复
            PasswordExpiredDays = 20008, //密码过期日期
            PasswordLength= 20009, //密码长度
            PasswordComplexity = 20010, //是否启用密码复杂度
            FordEdiBakFolder = 90001,
            FordEdiErrorFolder = 90002,
            FordEdiFileFolder = 90003,
            MaxRowSize = 90100,
            FordFlow = 90112,
            SystemFlag = 90102,
            SmartDeviceVersion = 90103,
            EKORG = 90105,//采购组织
            BUKRS = 90106,//公司代码
            WERKS = 90107,//工厂

            //密码最长存留期
            PassawordActive = 11071,
            //帐号锁定阀值
            PasswordLockCount = 11072,
            //公司名称
            CompanyName = 11073,
            //公司网址
            WebAddress = 11074,
            //上报时间
            StartUpTime = 11075,

            //默认配送标签模板
            DefaultDeliveryBarCodeTemplate = 90114,

            //强制移库先进先出
            IsForceFIFO = 110220,
        }

        [Display(Name = "EntityPreference_Desc", ResourceType = typeof(Resources.SYS.EntityPreference))]
        public string EntityPreferenceDesc { get; set; }
        #endregion
    }
}