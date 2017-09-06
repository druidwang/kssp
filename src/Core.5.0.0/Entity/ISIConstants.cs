using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity
{
    public static class ISIConstants
    {
        #region ISI
        public static readonly string CODE_PREFIX_ISI = "ISI";
        public static readonly string CODE_PREFIX_PLAN = "PLN";
        public static readonly string CODE_PREFIX_ISSUE = "ISS";
        public static readonly string CODE_PREFIX_IMPROVE = "IMP";
        public static readonly string CODE_PREFIX_CHANGE = "CHG";
        public static readonly string CODE_PREFIX_PRIVACY = "PRY";
        public static readonly string CODE_PREFIX_AUDIT = "ADT";
        public static readonly string CODE_PREFIX_RESPONSE = "REP";
        public static readonly string CODE_PREFIX_PROJECT = "PRJ";
        public static readonly string CODE_PREFIX_PROJECT_ISSUE = "PIS";
        /// <summary>
        /// 工程更改
        /// </summary>
        public static readonly string CODE_PREFIX_ENGINEERING_CHANGE = "ENC";
        public static readonly string CODE_PREFIX_ENGINEERING_WORKFLOW = "WFS";

        public static readonly string CODE_MASTER_ISI_TYPE = "ISIType";
        public static readonly string ISI_TASK_TYPE_GENERAL = "General";
        public static readonly string ISI_TASK_TYPE_PLAN = "Plan";
        public static readonly string ISI_TASK_TYPE_WORKFLOW = "WFS";
        public static readonly string ISI_TASK_TYPE_ISSUE = "Issue";
        public static readonly string ISI_TASK_TYPE_IMPROVE = "Improve";
        public static readonly string ISI_TASK_TYPE_CHANGE = "Change";
        public static readonly string ISI_TASK_TYPE_PRIVACY = "Privacy";
        public static readonly string ISI_TASK_TYPE_RESPONSE = "Response";
        public static readonly string ISI_TASK_TYPE_PROJECT = "Project";
        public static readonly string ISI_TASK_TYPE_AUDIT = "Audit";
        public static readonly string ISI_TASK_TYPE_PROJECT_ISSUE = "PrjIss";
        /// <summary>
        /// 工程更改
        /// </summary>
        public static readonly string ISI_TASK_TYPE_ENGINEERING_CHANGE = "Enc";

        public static readonly IList<string> TaskTypeList = new List<string>() { ISIConstants.ISI_TASK_TYPE_PLAN, ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_IMPROVE,
                                                       ISIConstants.ISI_TASK_TYPE_CHANGE,ISIConstants.ISI_TASK_TYPE_PRIVACY,ISIConstants.ISI_TASK_TYPE_RESPONSE,
                                                        ISIConstants.ISI_TASK_TYPE_PROJECT,ISIConstants.ISI_TASK_TYPE_AUDIT,ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE,
                                                        ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE,ISIConstants.ISI_TASK_TYPE_WORKFLOW};

        public static readonly IDictionary<string, string> TaskTypeDic
                                = new Dictionary<string, string>() { { CODE_PREFIX_PLAN, ISI_TASK_TYPE_PLAN },
                                                                     { CODE_PREFIX_ISSUE, ISI_TASK_TYPE_ISSUE },
                                                                     { CODE_PREFIX_IMPROVE, ISI_TASK_TYPE_IMPROVE },
                                                                     { CODE_PREFIX_CHANGE, ISI_TASK_TYPE_CHANGE },
                                                                     { CODE_PREFIX_PRIVACY, ISI_TASK_TYPE_PRIVACY },
                                                                     { CODE_PREFIX_AUDIT, ISI_TASK_TYPE_AUDIT },
                                                                     { CODE_PREFIX_RESPONSE, ISI_TASK_TYPE_RESPONSE },
                                                                     { CODE_PREFIX_PROJECT, ISI_TASK_TYPE_PROJECT },
                                                                     { CODE_PREFIX_PROJECT_ISSUE, ISI_TASK_TYPE_PROJECT_ISSUE },
                                                                     { CODE_PREFIX_ENGINEERING_CHANGE, ISI_TASK_TYPE_ENGINEERING_CHANGE},
                                                                     { CODE_PREFIX_ENGINEERING_WORKFLOW,ISIConstants.ISI_TASK_TYPE_WORKFLOW}};


        public static readonly string ISI_LEVEL_SEPRATOR = "|";
        public static readonly string ISI_USER_SEPRATOR = ",";
        public static readonly string ISI_USER_SEPRATOR_VIEW = ", ";


        public static readonly string[] ISI_USERNAME_SEPRATOR = new string[] { ", ", "," };
        public static readonly char[] ISI_SEPRATOR = new char[] { '|', ';', '；', ',', '，', ' ' };

        public static readonly char[] ISI_FILE_SEPRATOR = new char[] { '*', '.', ' ', ';' };

        public static readonly string ISI_FILEEXTENSION = "ISI_FileExtension";

        public static readonly string ISI_CONTENTLENGTH = "ISI_ContentLength ";

        public static readonly string ISI_LEVEL_BASE = "0";

        public static readonly string ISI_LEVEL_COMMENT = "Comment";

        public static readonly string ISI_LEVEL_STATUS = "Status";

        public static readonly string ISI_LEVEL_APPROVE = "Approve";

        public static readonly string ISI_LEVEL_HELP = "Help";

        public static readonly string ISI_LEVEL_STARTPERCENT = "StartPercent";

        public static readonly string ISI_LEVEL_OPEN = "Open";

        public static readonly string ISI_LEVEL_COMPLETE = "Complete";

        public static readonly string SMS_SEPRATOR = "\r\n";

        public static readonly string TEXT_SEPRATOR2 = "\n";

        public static readonly string TEXT_SEPRATOR = "\r\n";

        public static readonly string EMAIL_SEPRATOR = "<br />";

        public static readonly string SCHEDULINGTYPE_SPECIAL = "Special";
        public static readonly string SCHEDULINGTYPE_GENERAL = "General";
        public static readonly string SCHEDULINGTYPE_TASKSUBTYPE = "TaskSubType";

        public static readonly string CODE_MASTER_PERMISSION_CATEGORY_TYPE_VALUE_ISI = "ISI";

        public static readonly string CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE = "TaskSubType";
        public static readonly string CODE_MASTER_ISI_TYPE_VALUE_TASKADMIN = "TaskAdmin";

        public static readonly string CODE_MASTER_ISI_TASK_VALUE_ISIADMIN = "ISIAdmin";
        public static readonly string CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN = "TaskFlowAdmin";
        public static readonly string CODE_MASTER_ISI_TASK_VALUE_VIEW = "ISIViewer";
        public static readonly string CODE_MASTER_ISI_TASK_VALUE_ASSIGN = "ISIAssign";
        public static readonly string CODE_MASTER_ISI_TASK_VALUE_CLOSE = "ISIClose";
        public static readonly string CODE_MASTER_WF_TASK_VALUE_WFADMIN = "WFAdmin";

        public static readonly string PERMISSION_PAGE_ISI_VALUE_ADDATTACHMENT = "AddAttachment";
        public static readonly string PERMISSION_PAGE_ISI_VALUE_DELETEATTACHMENT = "DeleteAttachment";
        public static readonly string PERMISSION_PAGE_ISI_VALUE_PLANREMIND = "PlanRemind";
        public static readonly string PERMISSION_PAGE_ISI_VALUE_5SREMIND = "5SRemind";
        public static readonly string PERMISSION_PAGE_ISI_VALUE_TASKREPORT = "TaskReport";
        public static readonly string PERMISSION_PAGE_ISI_VALUE_ISCADRE = "IsCadre";

        public static readonly string PERMISSION_PAGE_ISI_VALUE_DELETETASKSTATUS = "DeleteTaskStatus";

        public static readonly string ENTITY_PREFERENCE_CODE_EMPPRUN = "EMPPRun";
        public static readonly string ENTITY_PREFERENCE_WEBADDRESS = "WebAddress";

        public static readonly string ENTITY_PREFERENCE_CODE_ISI_CLIENT_ROWS = "ISIClientRefresh";
        public static readonly string ENTITY_PREFERENCE_CODE_ISI_ASSIGN_UP_TIME = "ISI_AssignUpTime";
        public static readonly string ENTITY_PREFERENCE_CODE_ISI_START_UP_TIME = "ISI_StartUpTime";
        public static readonly string ENTITY_PREFERENCE_CODE_ISI_CLOSE_UP_TIME = "ISI_CloseUpTime";

        public static readonly string ENTITY_PREFERENCE_ISI_MAC = "ISI_MAC";
        public static readonly string ENTITY_PREFERENCE_ISI_IP = "ISI_IP";
        public static readonly string ENTITY_PREFERENCE_ISI_PCNAME = "ISI_PCName";
        public static readonly string ENTITY_PREFERENCE_ISI_RESSUBSCRIBE = "ISI_ResSubscribe";
        //public static readonly string ENTITY_PREFERENCE_ISI_PCLOGINNAME = "ISI_PCLoginName";



        public static readonly string CODE_MASTER_ISI_SMS_EVENTHANDLER = "ISI_SMSEventHeadler";
        public static readonly string CODE_MASTER_ISI_SMS_EVENTHANDLER_MESSAGERECEIVEDINTERFACE = "MessageReceivedInterface";
        public static readonly string CODE_MASTER_ISI_SMS_EVENTHANDLER_STATUSRECEIVEDINTERFACE = "StatusReceivedInterface";
        public static readonly string CODE_MASTER_ISI_SMS_EVENTHANDLER_SUBMITRESPINTERFACE = "SubmitRespInterface";

        public static readonly string CODE_MASTER_ISI_PRIORITY = "ISIPriority";
        public static readonly string CODE_MASTER_ISI_PRIORITY_NORMAL = "Normal";
        public static readonly string CODE_MASTER_ISI_PRIORITY_URGENT = "Urgent";
        public static readonly string CODE_MASTER_ISI_PRIORITY_HIGH = "High";
        public static readonly string CODE_MASTER_ISI_PRIORITY_LOW = "Low";
        public static readonly string CODE_MASTER_ISI_PRIORITY_MAJOR = "Major";

        public static readonly string CODE_MASTER_BTD_FLOW_CATEGORY = "FlowCategory";
        /// <summary>
        /// 工业废弃物
        /// </summary>
        public static readonly string CODE_MASTER_BTD_FLOW_CATEGORY_SC = "SC";
        /// <summary>
        /// 备件
        /// </summary>
        public static readonly string CODE_MASTER_BTD_FLOW_CATEGORY_SP = "SP";

        public static readonly string CODE_MASTER_ISI_SEND_STATUS = "ISISendStatus";
        public static readonly string CODE_MASTER_ISI_SEND_STATUS_SUCCESS = "Success";
        public static readonly string CODE_MASTER_ISI_SEND_STATUS_FAIL = "Fail";
        public static readonly string CODE_MASTER_ISI_SEND_STATUS_NOTSEND = "NotSend";

        public static readonly string CODE_MASTER_ISI_FLAG = "ISIFlag";
        public static readonly string CODE_MASTER_ISI_FLAG_DI1 = "DI1";
        public static readonly string CODE_MASTER_ISI_FLAG_DI2 = "DI2";
        public static readonly string CODE_MASTER_ISI_FLAG_DI3 = "DI3";
        public static readonly string CODE_MASTER_ISI_FLAG_DI4 = "DI4";
        public static readonly string CODE_MASTER_ISI_FLAG_DI5 = "DI5";

        public static readonly string CODE_MASTER_ISI_COLOR = "ISIColor";
        public static readonly string CODE_MASTER_ISI_FLAG_RED = "red";
        public static readonly string CODE_MASTER_ISI_FLAG_GREEN = "green";
        public static readonly string CODE_MASTER_ISI_FLAG_YELLOW = "yellow";

        public static readonly string CODE_MASTER_ISI_STATUS = "ISIStatus";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_CREATE = "Create";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_DELETE = "Delete";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_SUBMIT = "Submit";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE = "In-Approve";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_APPROVE = "Approve";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_RETURN = "Return";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_REFUSE = "Refuse";
        //public static readonly string CODE_MASTER_ISI_STATUS_VALUE_SUSPEND = "Suspend";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE = "In-Dispute";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_CANCEL = "Cancel";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_INPROCESS = "In-Process";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_ASSIGN = "Assign";
        //public static readonly string CODE_MASTER_ISI_STATUS_VALUE_REASSIGN = "ReAssign";
        //public static readonly string CODE_MASTER_ISI_STATUS_VALUE_PAUSE = "Pause";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_COMPLETE = "Complete";
        public static readonly string CODE_MASTER_ISI_STATUS_VALUE_CLOSE = "Close";

        public static readonly string CODE_MASTER_WFS_STATUS = "WFSStatus";

        public static readonly string CODE_MASTER_WFS_STATUS_VALUE_UNAPPROVED = "Unapproved";

        public static readonly string CODE_MASTER_ISI_ORG = "ISIDepartment";

        //考核状态
        public static readonly string CODE_MASTER_ISI_CHECKUP_STATUS = "ISICheckupStatus";
        public static readonly string CODE_MASTER_ISI_CHECKUP_STATUS_CREATE = "Create";
        public static readonly string CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT = "Submit";
        public static readonly string CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL = "Approval";
        public static readonly string CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE = "Close";
        public static readonly string CODE_MASTER_ISI_CHECKUP_STATUS_CANCEL = "Cancel";

        public static readonly string CODE_MASTER_ISI_CHECKUPPROJECT_TYPE = "ISICheckupProjectType";
        public static readonly string CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_GENERAL = "General";
        public static readonly string CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_EMPLOYEE = "Employee";
        public static readonly string CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_CADRE = "Cadre";

        public static readonly string CODE_MASTER_ISI_CHECKUPPROJECTTYPE = "ISICheckupProjectType";
        public static readonly string CODE_MASTER_ISI_CHECKUPPROJECTTYPE_VALUE_GENERAL = "General";
        public static readonly string CODE_MASTER_ISI_CHECKUPPROJECTTYPE_VALUE_EMPLOYEE = "Employee";
        public static readonly string CODE_MASTER_ISI_CHECKUPPROJECTTYPE_VALUE_CADRE = "Cadre";

        public static readonly string ENTITY_PREFERENCE_CODE_ISI_AMOUNTLIMIT_CADRE = "ISIAmountLimitCadre";
        public static readonly string ENTITY_PREFERENCE_CODE_ISI_AMOUNTLIMIT_EMPLOYEE = "ISIAmountLimitEmployee";

        public static readonly string ENTITY_PREFERENCE_CODE_ISI_CHECKUPSUBMITREMIND = "ISICheckupSubmitRemind";
        public static readonly string ENTITY_PREFERENCE_CODE_ISI_CADRECHECKUPCC = "CadreCheckupCC";
        public static readonly string ENTITY_PREFERENCE_CODE_ISI_EMPLOYEECHECKUPCC = "EmployeeCheckupCC";


        public static readonly string ENTITY_PREFERENCE_CODE_ISI_TASKSTATUSUPDATEDAY = "ISITaskStatusUpdateDay";


        public static readonly string PERMISSION_PAGE_ISI_CHECKUP_VALUE_CREATECHECKUP = "CreateCheckup";
        public static readonly string PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP = "ApproveCheckup";
        public static readonly string PERMISSION_PAGE_ISI_CHECKUP_VALUE_CLOSECHECKUP = "CloseCheckup";
        public static readonly string PERMISSION_PAGE_ISI_CHECKUP_VALUE_EXPORTCHECKUP = "ExportCheckup";
        public static readonly string PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVEREMINDCHECKUP = "ApproveRemindCheckup";
        public static readonly string PERMISSION_PAGE_ISI_CHECKUP_VALUE_REMINDCHECKUP = "RemindCheckup";
        public static readonly string PERMISSION_PAGE_ISI_CHECKUP_VALUE_RECEIVEPUBLISHCHECKUP = "ReceivePublishCheckup";
        public static readonly string PERMISSION_PAGE_ISI_CHECKUP_VALUE_PUBLISHCHECKUP = "PublishCheckup";
        public static readonly string PERMISSION_PAGE_ISI_CHECKUP_VALUE_RECEIVECLOSEREMINDCHECKUP = "ReceiveCloseRemindCheckup";

        // 项目管理
        public static readonly string CODE_MASTER_ISI_PROJECT_PHASE = "ISIPhase";
        public static readonly string CODE_MASTER_ISI_PROJECTSUBTYPE_QUOTE = "Quote";//报价
        public static readonly string CODE_MASTER_ISI_PROJECTSUBTYPE_INITIATION = "Initiation";//开发
        #endregion

        public static readonly string CODE_MASTER_PERMISSION_FILTER_VALUE_FILTERADMIN = "FilterAdmin";

        #region 月度自评

        public static readonly string CODE_MASTER_SUMMARY_CHECKUPPROJECT = "Summary";

        public static readonly string CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN = "SummaryAdmin";
        public static readonly string CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE = "SummaryApprove";

        public static readonly string CODE_MASTER_SUMMARY_TYPE = "ISISummaryType";
        public static readonly string CODE_MASTER_SUMMARY_TYPE_EXCELLENT = "Excellent";
        public static readonly string CODE_MASTER_SUMMARY_TYPE_MODERATE = "Moderate";
        public static readonly string CODE_MASTER_SUMMARY_TYPE_POOR = "Poor";

        public static readonly string CODE_MASTER_SUMMARY_STATUS = "ISISummaryStatus";
        public static readonly string CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE = "Create";
        public static readonly string CODE_MASTER_SUMMARY_STATUS_VALUE_DELETE = "Delete";
        public static readonly string CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT = "Submit";
        public static readonly string CODE_MASTER_SUMMARY_STATUS_VALUE_CANCEL = "Cancel";
        public static readonly string CODE_MASTER_SUMMARY_STATUS_VALUE_INAPPROVE = "In-Approve";
        public static readonly string CODE_MASTER_SUMMARY_STATUS_VALUE_APPROVAL = "Approval";
        public static readonly string CODE_MASTER_SUMMARY_STATUS_VALUE_CLOSE = "Close";
        #endregion

        #region 工作流

        public static readonly string STRING_EMPTY = "空";

        public static readonly string TASKAPPLY_GEN_EMAIL = "Email";
        public static readonly string TASKAPPLY_GEN_PRINT = "Print";
        public static readonly string TASKAPPLY_GEN_HTML = "Html";

        public static readonly string CODE_MASTER_ISI_MSG_TYPE = "ISIMsgType";
        public static readonly string CODE_MASTER_ISI_MSG_TYPE_STATUS = "Status";
        public static readonly string CODE_MASTER_ISI_MSG_TYPE_APPROVE = "Approve";
        public static readonly string CODE_MASTER_ISI_MSG_TYPE_COMMENT = "Comment";

        public static readonly string CODE_MASTER_WFS_TYPE = "WFSType";
        public static readonly string CODE_MASTER_WFS_TYPE_QTY = "Qty";
        public static readonly string CODE_MASTER_WFS_TYPE_TEXTBOX = "TextBox";
        public static readonly string CODE_MASTER_WFS_TYPE_TEXTAREA = "TextArea";
        public static readonly string CODE_MASTER_WFS_TYPE_CHECKBOX = "CheckBox";
        public static readonly string CODE_MASTER_WFS_TYPE_DATE = "Date";
        public static readonly string CODE_MASTER_WFS_TYPE_DATETIME = "DateTime";
        public static readonly string CODE_MASTER_WFS_TYPE_RADIO = "Radio";
        public static readonly string CODE_MASTER_WFS_TYPE_LABEL = "Label";
        public static readonly string CODE_MASTER_WFS_TYPE_BLANK = "Blank";

        public static readonly string APPLY_AMOUNT = "Amount";
        public static readonly string APPLY_DATE = "Date";
        public static readonly string APPLY_DATETIME = "DateTime";
        public static readonly string APPLY_AMOUNT2 = "Amount2";
        public static readonly string APPLY_SHIPFROM = "ShipFrom";
        public static readonly string APPLY_SHIPTO = "ShipTo";

        /// <summary>
        /// 默认间隔
        /// </summary>
        public static readonly int CODE_MASTER_WFS_LEVEL_INTERVAL = 10000;
        /// <summary>
        /// 会签默认间隔
        /// </summary>
        public static readonly int CODE_MASTER_WF_COUNTERSIGN_LEVEL_INTERVAL = 100;


        public static readonly int? CODE_MASTER_WFS_LEVEL_DEFAULT = null;

        public static readonly int CODE_MASTER_WF_PRELEVEL = -1;

        public static readonly int CODE_MASTER_WFS_LEVEL1 = 10000;
        public static readonly int CODE_MASTER_WFS_LEVEL2 = 20000;
        public static readonly int CODE_MASTER_WFS_LEVEL3 = 30000;
        public static readonly int CODE_MASTER_WFS_LEVEL4 = 40000;
        public static readonly int CODE_MASTER_WFS_LEVEL5 = 50000;
        public static readonly int CODE_MASTER_WFS_LEVEL6 = 60000;
        public static readonly int CODE_MASTER_WFS_LEVEL7 = 70000;
        public static readonly int CODE_MASTER_WFS_LEVEL8 = 80000;
        public static readonly int CODE_MASTER_WFS_LEVEL9 = 90000;
        public static readonly int CODE_MASTER_WFS_LEVEL10 = 100000;
        public static readonly int CODE_MASTER_WFS_LEVEL11 = 110000;
        public static readonly int CODE_MASTER_WFS_LEVEL12 = 120000;
        public static readonly int CODE_MASTER_WFS_LEVEL13 = 130000;
        public static readonly int CODE_MASTER_WFS_LEVEL14 = 140000;
        public static readonly int CODE_MASTER_WFS_LEVEL15 = 150000;
        public static readonly int CODE_MASTER_WFS_LEVEL16 = 160000;
        public static readonly int CODE_MASTER_WFS_LEVEL17 = 170000;
        public static readonly int CODE_MASTER_WFS_LEVEL18 = 180000;
        public static readonly int CODE_MASTER_WFS_LEVEL19 = 190000;
        public static readonly int CODE_MASTER_WFS_LEVEL20 = 200000;
        public static readonly int CODE_MASTER_WFS_LEVEL_ULTIMATE = 900000;
        public static readonly int CODE_MASTER_WFS_LEVEL_COMPLETE = 900000000;


        //工时单位
        public static readonly string WORKHOUR_UOM = "H";

        #endregion

        #region 责任方阵
        public static readonly string PERMISSION_PAGE_VALUE_RESMATRIXCHANGELOG = "ResMatrixChangeLogRep";
        #endregion

        #region 新增异常报表
        public static readonly string PERMISSION_PAGE_VALUE_PRODUCTIONINOUT = "ProductionInOutRep";
        public static readonly string PERMISSION_PAGE_VALUE_SPCLOG = "SPCLogRep";
        #endregion
    }
}
