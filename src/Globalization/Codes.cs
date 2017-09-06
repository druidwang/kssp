using System.Runtime.Serialization;

namespace com.Sconit.CodeMaster
{

    public enum PermissionCategoryType
    {
        Menu = 0,
        Url = 1,
        Region = 2,
        Customer = 3,
        Supplier = 4,
        Terminal = 5,
        SI = 6,
        OrderType = 7,
        Carrier = 8
    }


    public enum RoleType
    {
        Normal = 0,
        System = 1
    }

    public enum UserType
    {
        Normal = 0,
        System = 1,
        Supplier = 2,
    }

    public enum BillType
    {
        /// <summary>
        /// 采购
        /// </summary>
        Procurement = 0,
        /// <summary>
        /// 销售
        /// </summary>
        Distribution = 1,
        /// <summary>
        /// 运输
        /// </summary>
        Transport = 2,
    }

    public enum BillSubType
    {
        /// <summary>
        /// 采购
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 销售
        /// </summary>
        Cancel = 1
    }

    public enum InterfacePriceType
    {
        /// <summary>
        /// 采购
        /// </summary>
        Purchase = 10,
        /// <summary>
        /// 委外
        /// </summary>
        Subconctract = 20,
        /// <summary>
        /// 寄售价格
        /// </summary>
        Consignment = 30,

    }

    public enum BillTransactionType
    {
        POPlanBill = 1,
        POPlanBillVoid = 2,
        SOPlanBill = 3,
        SOPlanBillVoid = 4,

        POSettle = 11,
        POSettleVoid = 12,
        SOSettle = 13,
        SOSettleVoid = 14,

        POBilled = 21,
        POBilledVoid = 22,
        SOBilled = 23,
        SOBilledVoid = 24,
    }

    public enum PriceListType
    {
        Procuement = 1,
        Distribution = 2
    }

    public enum InspectType
    {
        Quantity = 1,
        Barcode = 2
    }

    public enum InspectStatus
    {
        Submit = 0,
        InProcess = 1,
        Close = 2
    }

    public enum JudgeResult
    {
        Qualified = 0,
        Rejected = 1
    }


    public enum OrderType
    {
        Procurement = 1,
        Transfer = 2,
        Distribution = 3,
        Production = 4,
        SubContract = 5,
        CustomerGoods = 6,
        SubContractTransfer = 7,
        ScheduleLine = 8
    }

    public enum OrderPriority
    {
        Normal = 0,
        Urgent = 1
    }

    public enum LocationLevel
    {
        Donotcollect = 0,//不汇总
        Factory = 3,  //工厂
        Region = 1,  //区域
        SinglePlant = 2  //分厂
    }

    public enum SendStatus
    {
        NotSend = 0,
        Success = 1,
        Fail = 2
    }

    public enum OrderStatus
    {
        Create = 0,
        Submit = 1,
        InProcess = 2,
        Complete = 3,
        Close = 4,
        Cancel = 5
    }

    public enum ReceiveGapTo
    {
        AdjectLocFromInv = 0,      //调整发货方库存
        RecordIpGap = 1           //记录收货差异
    }


    public enum CreateHuOption
    {
        //NoCreate = 0,  //不创建
        //Release = 1,   //释放
        //Start =2,      //上线
        //Ship = 3,      //发货
        //Receive = 4    //收货

        None = 0,  //不创建
        //Submit = 1,  //释放创建
        //Start = 2,   //上线创建
        Ship = 3, //发货创建
        Receive = 4   //收货创建
    }


    public enum ReCalculatePriceOption
    {
        Manual = 0,  //手工
        Submit = 1,   //释放
        Start = 2,      //上线
        Ship = 3,      //发货
        Receive = 4    //收货
    }


    public enum OrderBillTerm
    {
        NA = 0,                    //空
        ReceivingSettlement = 1,   //收货结算
        AfterInspection = 2,       //检验结算
        OnlineBilling = 3,         //上线结算
        LinearClearing = 4,        //下线结算
        ConsignmentBilling = 5     //寄售结算
    }


    public enum OrderSubType
    {
        Normal = 0,//SY01
        Return = 1,  //退货   采购、销售、移库、生产、客供、委外
        Other = 40,//挤出废品报工
    }

    public enum AddressType
    {
        ShipAddress = 0,
        BillAddress = 1
    }

    public enum InventoryType
    {
        Quality = 0,  //按数量
        Barcode = 1   //按条码
    }

    //public enum ItemType
    //{
    //    Purchase = 1,
    //    Sales = 2,
    //    Manufacture = 3,
    //    SubContract = 4,
    //    CustomerGoods = 5,
    //    Virtual = 6,
    //    Kit = 7
    //}

    //public enum PartyType
    //{
    //    Region = 0,
    //    Supplier = 1,
    //    Customer = 2
    //}

    public enum WorkingCalendarType
    {
        Work = 0,
        Rest = 1
    }

    //public enum SpecialTimeType
    //{
    //    Work = 0,
    //    Rest = 1
    //}

    public enum RejectStatus
    {
        Create = 0,
        Submit = 1,
        InProcess = 2,
        Close = 3
    }


    public enum QualityType  //库存质量状态
    {
        Qualified = 0,   //正常
        Inspect = 1,  //待验
        Reject = 2,    //不合格品
    }

    public enum OccupyType  //库存质量状态
    {
        None = 0,   //Not Occupy
        Pick = 1,   //拣货
        Inspect = 2,//检验
        Sequence = 3,//排序
        MiscOrder = 4,//计划外出入库
        //AutoPick = 5//系统拣货，只占用库存，没有拣货单
    }


    public enum TimeUnit
    {
        NA = 0,
        Second = 1,
        Minute = 2,
        Hour = 3,
        Day = 4,
        Week = 5,
        Month = 6,
        Quarter = 7,
        Year = 8
    }

    public enum WindowTimeType
    {
        FixedWindowTime = 0,
        CycledWindowTime = 1,
    }

    public enum StockTakeType
    {
        Part = 0,
        All = 1
    }

    public enum StockTakeStatus
    {
        Create = 0,
        Submit = 1,
        InProcess = 2,
        Complete = 3,
        Close = 4,
        Cancel = 5
    }

    public enum IssuePriority
    {
        Normal = 0,
        Urgent = 1
    }
    public enum IssueStatus
    {
        Create = 0,
        Submit = 1,
        InProcess = 2,
        Complete = 3,
        Close = 4,
        Cancel = 5
    }
    public enum IssueType
    {
        Issue = 0,
        Improvement = 1,
        Changepoint = 2
    }

    /// <summary>
    /// 库存事务
    /// </summary>
    public enum TransactionType
    {
        LOC_INI = 0,          //库存初始化
        //销售
        ISS_SO = 101,           //销售出库
        ISS_SO_VOID = 102,      //销售出库冲销
        RCT_SO = 103,           //销售退货入库
        RCT_SO_VOID = 104,      //销售退货入库冲销    
        //采购
        RCT_PO = 201,           //采购入库
        RCT_PO_VOID = 202,      //采购入库冲销
        ISS_PO = 203,           //采购退货
        ISS_PO_VOID = 204,      //采购退货冲销
        RCT_SL = 205,           //计划协议入库
        RCT_SL_VOID = 206,      //计划协议入库冲销
        ISS_SL = 207,           //计划协议退货
        ISS_SL_VOID = 208,      //计划协议退货冲销
        //库内事务
        ISS_TR = 301,           //移库出库
        ISS_TR_VOID = 302,      //移库出库冲销
        RCT_TR = 303,           //移库入库
        RCT_TR_VOID = 304,      //移库入库冲销
        ISS_TR_RTN = 311,       //移库退货出库
        ISS_TR_RTN_VOID = 312,  //移库退货出库冲销
        RCT_TR_RTN = 313,       //移库退货入库
        RCT_TR_RTN_VOID = 314,  //移库退货入库冲销
        ISS_STR = 305,          //委外移库出库
        ISS_STR_VOID = 306,     //委外移库出库冲销
        RCT_STR = 307,          //委外移库入库
        RCT_STR_VOID = 308,     //委外移库入库冲销
        ISS_STR_RTN = 315,      //委外移库退货出库
        ISS_STR_RTN_VOID = 316, //委外移库退货出库冲销
        RCT_STR_RTN = 317,      //委外移库退货入库
        RCT_STR_RTN_VOID = 318, //委外移库退货入库冲销
        ISS_REP = 351,          //翻箱出库
        RCT_REP = 352,          //翻箱入库
        ISS_PUT = 353,          //上架出库
        RCT_PUT = 354,          //上架入库
        ISS_PIK = 355,          //下架出库
        RCT_PIK = 356,          //下架入库
        ISS_IIC = 361,          //库存物料替换出库
        ISS_IIC_VOID = 362,     //库存物料替换出库冲销
        RCT_IIC = 363,          //库存物料替换入库
        RCT_IIC_VOID = 364,     //库存物料替换入库冲销
        //生产
        ISS_WO = 401,           //生产出库/原材料
        ISS_WO_VOID = 402,      //生产出库/原材料冲销
        ISS_WO_BF = 403,        //生产投料回冲出库/出生产线
        ISS_WO_BF_VOID = 404,   //生产投料回冲出库/出生产线冲销
        RCT_WO = 405,           //生产入库/成品
        RCT_WO_VOID = 406,      //生产入库/成品冲销
        ISS_MIN = 407,          //生产投料出库
        ISS_MIN_RTN = 408,      //生产投料退库出库
        RCT_MIN = 409,          //生产投料入库/入生产线
        RCT_MIN_RTN = 410,      //生产投料出库/出生产线

        //委外
        ISS_SWO = 411,          //委外生产出库/原材料
        ISS_SWO_VOID = 412,     //委外生产出库/原材料冲销
        ISS_SWO_BF = 413,       //委外生产投料回冲出库/出生产线
        ISS_SWO_BF_VOID = 414,  //委外生产投料回冲出库/出生产线冲销
        RCT_SWO = 415,          //委外生产入库/成品
        RCT_SWO_VOID = 416,     //委外生产入库/成品冲销
        ISS_SWO_RTN = 417,      //委外收货退货出库
        ISS_SWO_RTN_VOID = 418, //委外收货退货出库冲销
        RCT_SWO_RTN = 419,      //委外收货退货入库
        RCT_SWO_RTN_VOID = 420, //委外收货退货入库冲销

        ISS_WO_RTN = 421,      //原材料回用收货出库
        ISS_WO_RTN_VOID = 422, //原材料回用收货出库冲销
        RCT_WO_RTN = 423,      //原材料回用收货入库
        RCT_WO_RTN_VOID = 424, //原材料回用收货入库冲销

        //检验
        ISS_INP = 501,          //报验出库
        RCT_INP = 502,          //报验入库
        ISS_ISL = 503,          //隔离出库
        RCT_ISL = 504,          //隔离入库
        ISS_INP_QDII = 505,     //检验合格出库 
        RCT_INP_QDII = 506,     //检验合格入库 
        ISS_INP_REJ = 507,      //检验不合格出库 
        RCT_INP_REJ = 508,      //检验不合格入库 
        ISS_INP_CCS = 509,      //让步使用出库
        RCT_INP_CCS = 510,      //让步使用入库
        //调整
        CYC_CNT = 601,          //盘点差异出库
        CYC_CNT_VOID = 602,     //盘点差异入库
        ISS_UNP = 603,          //计划外出库
        ISS_UNP_VOID = 604,     //计划外出库冲销
        RCT_UNP = 605,          //计划外入库
        RCT_UNP_VOID = 606,     //计划外入库冲销

        //客户化
        //ISS_AGE = 901,          //老化出库
        //ISS_AGE_VOID = 902,     //老化出库冲销
        //RCT_AGE = 903,          //老化入库
        //RCT_AGE_VOID = 904,     //老化入库冲销
        //ISS_FLT = 905,          //过滤出库
        //ISS_FLT_VOID = 906,     //过滤出库冲销
        //RCT_FLT = 907,          //过滤入库
        //RCT_FLT_VOID = 908,     //过滤入库冲销
    }

    public enum HandleResult
    {
        Qualify = 1,
        Scrap = 2,
        Return = 3,//退货
        Rework = 4,
        Concession = 5,  //让步
        WorkersWaste = 6  //让步
    }

    public enum BarCodeType
    {
        FLOW,
        ORDER,
        PICKLIST,
        ASN,
        INSPECTION,
        STOCKTAKE,
        LOCATION,
        BIN,
        CONTAINER,
        HU
    }

    public enum ModuleType
    {
        Client_Receive,
        Client_PickListOnline,
        Client_PickList,
        Client_PickListShip,
        Client_OrderShip,
        Client_SeqPack,

        Client_Transfer,
        Client_RePack,
        Client_Pack,
        Client_UnPack,
        Client_PutAway,
        Client_Pickup,
        Client_StockTaking,
        Client_HuStatus,
        Client_MiscIn,
        Client_MiscOut,

        Client_Inspect,
        Client_Qualify,
        Client_Reject,

        Client_AnDon,
        Client_ProductionOnline,   //生产上线
        Client_MaterialIn,         //生产投料
        Client_ProductionOffline,      //生产下线
        Client_ChassisOnline,   //底盘上线
        Client_CabOnline,   //驾驶室上线
        Client_AssemblyOnline,  //总装上线
        Client_MaterialReturn,         //生产投料

        //周转箱
        Client_EmptyContainerShip,//空箱发货
        Client_EmptyContainerReceive,//空箱收货
        Client_FullContainerReceive,//满箱收货

        M_Switch
    }

    public enum IpType
    {
        Normal = 0,
        SEQ = 1,
        KIT = 2,
    }

    public enum ItemExchangeType
    {
        ItemExchange = 0,//物料替换
        Aging = 1,//老化
        Filter = 2,//过滤
    }

    public enum IpDetailType
    {
        Normal = 0,
        Gap = 1
    }
    public enum IpStatus
    {
        Submit = 0,
        InProcess = 1,
        Close = 2,
        Cancel = 3
    }

    public enum PickListStatus
    {
        //Create = 0,
        Submit = 0,
        InProcess = 1,
        Close = 2,
        Cancel = 3
    }

    public enum BomStructureType
    {
        Normal = 0,   //普通结构
        Virtual = 1   //虚结构
    }

    public enum FeedMethod
    {
        None = 0,             //不投料
        ProductionOrder = 1,  //投料至工单成品
        ProductionLine = 2    //投料至生产线
    }

    public enum BackFlushMethod
    {
        GoodsReceive = 0,   //收货回冲，按标准BOM线冲生产线在制品，不够冲线边
        BackFlushOrder = 1, //只回冲工单在制品
        WeightAverage = 2   //加权平均
    }


    public enum BindType
    {
        Submit = 1,	//提交
        Start = 2	//上线
    }

    public enum RoundUpOption
    {
        ToUp = 1,    //向上圆整
        None = 0,    //不圆整
        ToDown = 2  //向下圆整
    }

    public enum MRPOption
    {
        OrderBeforePlan = 0,   //订单优先
        OrderOnly = 1,         //只看订单
        PlanAddOrder = 2,        //订单计划一起考虑
        PlanMinusOrder = 3,     //订单减掉计划
        PlanOnly = 4,             //只看计划
        SafeStockOnly = 5         //只看安全库存
    }


    public enum FlowStrategy
    {
        NA = 0,
        Manual = 1,
        KB = 2,
        JIT = 3,
        SEQ = 4,
        KIT = 5,
        MRP = 6,
        ANDON = 7,
        JIT2 = 8,
        JIT_EX = 9,
        KB2 = 10
    }

    //public enum SpecialValue
    //{
    //    BlankValue,
    //    AllValue
    //}

    public enum DocumentsType
    {
        ORD_Procurement = 1001,
        ORD_Transfer = 1002,
        ORD_Distribution = 1003,
        ORD_Production = 1004,
        ORD_SubContract = 1005,
        ORD_CustomerGoods = 1006,
        ORD_SubContractTransfer = 1007,
        ORD_ScheduleLine = 1008,
        ASN_Procurement = 1101,
        ASN_Transfer = 1102,
        ASN_Distribution = 1103,
        ASN_SubContract = 1105,
        ASN_CustomerGoods = 1106,
        ASN_SubContractTransfer = 1107,
        ASN_ScheduleLine = 1108,
        REC_Procurement = 1201,
        REC_Transfer = 1202,
        REC_Distribution = 1203,
        REC_Production = 1204,
        REC_SubContract = 1205,
        REC_CustomerGoods = 1206,
        REC_SubContractTransfer = 1207,
        REC_ScheduleLine = 1208,
        PIK_Transfer = 1302,
        PIK_Distribution = 1303,
        PIK_SubContractTransfer = 1307,
        BIL_Procurement = 1401,
        BIL_Distribution = 1403,
        RED_Procurement = 1411,
        RED_Distribution = 1413,
        MIS_Out = 1501,
        MIS_In = 1502,
        //MIS_SY03 = 1531,
        //MIS_SY05 = 1551,
        INS = 1601,
        REJ = 1611,
        STT = 1701,
        SEQ = 1801,
        CON = 1901,
        INV_Hu = 2001,
        VEH = 2011,
    }

    public enum LikeMatchMode
    {
        Anywhere,
        Start,
        End,
    }
    public enum HuStatus
    {
        NA = 0,
        Location = 1,
        Ip = 2,
    }

    public enum CodeMasterType
    {
        System = 0,
        Editable = 1
    }

    public enum PickOddOption
    {
        OddFirst = 0,     //零头先发
        NotPick = 1       //零头不发
    }

    public enum ShipStrategy
    {
        FIFO = 0,         //先进先出
        LIFO = 1          //后进先出
    }

    public enum CodeMaster
    {
        PermissionCategoryType,
        RoleType,
        UserType,
        BillType,
        BillTransactionType,
        PriceListType,
        InspectType,
        InspectStatus,
        JudgeResult,
        OrderType,
        OrderSubType,
        ReceiveGapTo,
        CreateHuOption,
        ReCalculatePriceOption,
        OrderPriority,
        OrderStatus,
        SendStatus,
        AddressType,
        InventoryType,
        //ItemType,
        WorkingCalendarType,
        RejectStatus,
        QualityType,
        OccupyType,
        TimeUnit,
        HandleResult,
        StockTakeType,
        StockTakeStatus,
        IssueType,
        IssuePriority,
        IssueStatus,
        IpType,
        IpDetailType,
        IpStatus,
        PickListStatus,
        BomStructureType,
        FeedMethod,
        BackFlushMethod,
        BindType,
        RoundUpOption,
        MRPOption,
        FlowStrategy,
        DocumentsType,
        HuStatus,
        LocationType,
        Language,
        LogLevel,
        DayOfWeek,
        HuTemplate,
        OrderTemplate,
        AsnTemplate,
        ReceiptTemplate,
        //FlowType,
        Strategy,
        InspectDefect,
        RoutingTimeUnit,
        BackFlushInShortHandle,
        SMSEventHeadler,
        SequenceStatus,
        ReceiptStatus,
        ConcessionStatus,
        MiscOrderType,
        MiscOrderStatus,
        PickOddOption,
        ShipStrategy,
        OrderBillTerm,
        CodeMasterType,
        MessageType,
        RePackType,
        JobRunStatus,
        TriggerStatus,
        MessagePriority,
        MessageStatus,
        EmailStatus,
        ScheduleType,
        TransactionIOType,
        TransactionType,
        IpGapAdjustOption,
        VehicleInFactoryStatus,
        LocationLevel,
        WindowTimeType,
        ApsPriorityType,
        ItemPriority,
        ResourceGroup,
        BillMasterStatus,
        PlanStatus,
        BomMrpOption,
        BillStatus,
        ItemOption,
        SubCategory,
        ShiftType,
        MachineType,
        QueryOrderType,
        Viscosity,
        HolidayType,
        ProductType,
        HuOption,
        PickListTemplate,
        SnapType,
        MrpSourceType,
        ItemExchangeType,
        ConsignmentStatus,
        FreezeStatus,
        InterfacePriceType,
        TaskPriority,
        TaskType,
        TaskStatus,
        TransportMode,
        TransportStatus,
        IOType,
        PickBy,
        TransportPricingMethod,
        FacilityStatus,
        FacilityTransType,
        MaintainPlanType,
        FacilityParamaterType,
        FacilityOrderType,
        FacilityOrderStatus,
        CheckListOrderStatus,
        LeadTimeOption,
        BarCodeMode,
    }

    public enum MessageType
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    public enum SequenceStatus
    {
        Submit = 0,
        Pack = 1,
        Ship = 2,
        Close = 3,
        Cancel = 4
    }

    public enum ReceiptStatus
    {
        Close = 0,
        Cancel = 1
    }

    public enum RePackType
    {
        In = 0,
        Out = 1
    }

    public enum TransactionIOType
    {
        In = 0,
        Out = 1
    }

    public enum IOType
    {
        In = 0,
        Out = 1
    }
    //public enum DocumentTypeEnum
    //{
    //    ORD,
    //    ASN,
    //    PIK,
    //    STT,
    //    INS
    //}

    public enum ConcessionStatus
    {
        Create = 0,
        Submit = 1,
        Close = 2
    }

    public enum JobRunStatus
    {
        InProcess = 0,
        Success = 1,
        Failure = 2,
    }

    public enum TriggerStatus
    {
        Open = 0,
        Close = 1,
    }

    public enum MessagePriority
    {
        Normal = 0,
        Low = 1,
        High = 2,
    }

    public enum MessageStatus
    {
        Open = 0,
        Close = 1
    }

    public enum EmailStatus
    {
        Open = 0,
        Close = 1
    }

    public enum MiscOrderType
    {
        GI = 0,
        GR = 1,
    }

    public enum MiscOrderSubType
    {
        COST = 0,
        SY03 = 30,
        /// <summary>
        /// 下线过程废品（包括在线待处理产品报废、线外挑选废品、后道退回来料废品）
        /// </summary>
        MES26 = 26,
        SY04 = 40,
        SY05 = 50,
        /// <summary>
        /// 后加工废品报废，不消耗库存
        /// </summary>
        MES27 = 27,
    }

    public enum MiscOrderStatus
    {
        Create = 0,
        Close = 1,
        Cancel = 2
    }

    public enum ScheduleType
    {
        Firm = 0,
        Forecast = 1,
        BackLog = 2,
        /// <summary>
        /// //挤出头子,包含在BOM废品率中
        /// </summary>
        MES21 = 21,
        /// <summary>
        /// //挤出皮子料,包含在BOM废品率中
        /// </summary>
        MES22 = 22,
        /// <summary>
        /// //挤出工艺损耗,包含在BOM废品率中
        /// </summary>
        MES23 = 23,
        /// <summary>
        /// //挤出牵引废品,不包含在BOM废品率中,需废品报工
        /// </summary>
        MES24 = 24,
        /// <summary>
        /// //在线过程废品,不包含在BOM废品率中,需废品报工
        /// </summary>
        MES25 = 25,
        /// <summary>
        /// //下线过程废品（包括在线待处理产品报废、线外挑选废品、后道退回来料废品）
        /// </summary>
        MES26 = 26,
        MES27 = 27,
        MES28 = 28,
        MES29 = 29,
        SY01 = 101,
        SY02 = 102,
        SY03 = 103,
        SY04 = 104,
        SY05 = 105,
        SY41 = 141,
        SY42 = 142,
        SY43 = 143,
    }

    public enum IpGapAdjustOption
    {
        GI = 0,
        GR = 1
    }

    public enum VehicleInFactoryStatus
    {
        Submit = 0,
        InProcess = 1,
        Close = 2
    }

    public enum GridDisplayType
    {
        Summary = 0,
        Detail = 1
    }

    public enum MQStatusEnum
    {
        Pending = 0,
        Success = 1,
        Fail = 2,
        Cancel = 3
    }

    public enum PlanStatus
    {
        //Error = -1,
        Create = 0,
        Submit = 1,
        Close = 2,
        Cancle = 3
    }

    public enum MrpSourceType
    {
        ExDay = 1,
        Order = 2,
        Plan = 3,
        StockLack = 4,
        StockOver = 5,
        Discontinue = 6,
        ShipGroupPlan = 7,
        //IndepentOrder = 8,
        MiShift = 10,
        ExShift = 20,
        FiShift = 30,
    }

    public enum ApsPriorityType
    {
        Normal = 5,  //常用
        Backup = 3, //备用
        //Urgent = 1,  //应急
    }

    public enum BillStatus
    {
        Create = 0,
        Submit = 1,
        Close = 2,
        Cancel = 3,
        Void = 4
    }

    public enum BillMasterStatus
    {
        Create = 0,
        Submit = 1,
        Close = 2,
        Cancel = 3
    }

    public enum ResourceGroup
    {
        Other = 0,
        /// <summary>
        /// 炼胶
        /// </summary>
        MI = 10,
        /// <summary>
        /// 挤出
        /// </summary>
        EX = 20,
        /// <summary>
        /// 后加工
        /// </summary>
        FI = 30,
    }

    public enum ItemPriority
    {
        /// <summary>
        /// 常规
        /// </summary>
        Normal = 0,//
        /// <summary>
        /// 备件
        /// </summary>
        Spare = 1,
    }

    public enum HuOption
    {
        /// <summary>
        /// 无需老化/过滤
        /// </summary>
        NoNeed = 0,//
        /// <summary>
        /// 未老化
        /// </summary>
        UnAging = 1,
        /// <summary>
        /// 已老化
        /// </summary>
        Aged = 2,
        /// <summary>
        /// 未过滤
        /// </summary>
        UnFilter = 3,
        /// <summary>
        /// 已过滤
        /// </summary>
        Filtered = 4,

    }

    public enum BomMrpOption
    {
        All = 0,   //Mrp和生产都要用到
        ProductionOnly = 1,   //只用于生产
        MrpOnly = 2, //只用于Mrp
    }

    public enum MachineType
    {
        Kit = 1,    //成套
        Single = 2, //单件
    }

    public enum ItemOption
    {
        NA = 0,
        NeedAging = 1,//需要老化
        NeedFilter = 2,//需要过滤
    }

    public enum SubCategory
    {
        ItemCategory = 0,//物料类型
        //TypeOne = 1,//分类一
        //TypeTwo = 2,//分类二
        //TypeThree = 3,//分类三
        //TypeFour = 4,//分类四
        MaterialsGroup = 5,//物料组

    }

    public enum ShiftType
    {
        TwoShiftPerDay = 2, //12小时/班
        ThreeShiftPerDay = 3,//8小时/班
    }

    public enum QueryOrderType
    {
        OrderNo = 0,
        ExternalOrderNo = 1,
        ReferenceOrderNo = 2,
    }
    //等级高中低
    public enum Viscosity
    {
        High = 1,
        Inthe = 2,
        Low = 3,
    }

    public enum HolidayType
    {
        Holiday = 10,
        Trial = 20,
        Halt = 30
    }
    /*
    public enum ProductType
    {
        A = 10,//A	批产01 SY01
        B = 20,//B	工业化02 SY02
        C = 30,//C	开发03 SY03
        D = 40,//D	备模
        E = 50,//E	工装
        F = 60,//F	设备
        G = 70,//G	材料
        H = 80,//H	工艺
        J = 100,//J	移线
        K = 110,//K	攻关
        L = 120,//L	改进
        M = 130,//M	保养
        N = 140,//N	能源
        P = 160,//P	停产
        Q = 170,//Q 工程更改
        R = 180,//R 配件
        S = 190,//S 塞芯
        Z = 260,//Z	其他
    }
    */
    public enum SnapType
    {
        Rccp = 0,
        Mrp = 1
    }

    public enum FreezeStatus
    {
        True = 1,
        False = 0
    }

    public enum ConsignmentStatus
    {
        True = 1,
        False = 0
    }

    public enum TaskPriority
    {
        Normal = 0,
        Urgent = 1,
        High = 2,
        Major = 3,
        Low = 4,
    }

    public enum TaskType
    {
        Audit = 0,
        Change = 1,
        Ecn = 2,
        General = 3,
        Improve = 4,
        Issue = 5,
        Plan = 6,
        Privacy = 7,
        PrjectIssue = 8,
        Project = 9,
        Response = 10,
    }

    public enum TaskStatus
    {
        Create = 0,
        Submit = 1,
        Cancel = 2,
        Assign = 3,
        InProcess = 4,
        Complete = 5,
        Close = 6,
    }

    public enum TransportMode
    {
        Land = 0,
        Sea = 1,
        Air = 2
    }

    public enum TransportStatus
    {
        Create = 0,
        Submit = 1,
        InProcess = 2,
        Close = 3,
        Cancel = 4,
    }

    public enum TransportPricingMethod
    {
        Chartered = 0,
        Distance = 1,
        Weight = 2,
        Volume = 3,
        LadderVolume = 4
    }

    public enum PickBy
    {
        LotNo = 0,
        Hu = 1
    }

    public enum PickGroupType
    {
        Ship = 0,
        Pick = 1,
        Repack = 2
    }

    /// <summary>
    /// 设施状态
    /// </summary>
    public enum FacilityStatus
    {
        Create = 0,
        Idle = 1,
        InUse = 2,
        Maintaining = 3,
        Inspecting = 4,
        Failure = 5,
        Repairing = 6
    }

    /// <summary>
    /// 设施事务类型
    /// </summary>
    public enum FacilityTransType
    {
        Create = 101,
        Enable = 102,
        StartUse = 103,
        FinishUse = 104,
        StartMaintain = 105,
        FinishMaintain = 106,
        StartInspect = 107,
        FinishInspect = 108,
        StartRepair = 109,
        FinishRepair = 110,
        Envelop = 111,
        Lend = 112,
        Sell = 113,
        Lose = 114,
        Scrap = 115
    }

    /// <summary>
    /// 预防策略类型
    /// </summary>
    public enum MaintainPlanType
    {
        Once = 0,
        Year = 1,
        Month = 2,
        Week = 3,
        Day = 4,
        Hour = 5,
        Minute = 6,
        Second = 7,
        Times = 8
    }


    /// <summary>
    /// 设备信息类型
    /// </summary>
    public enum FacilityParamaterType
    {
        Scan = 0,
        Paramater = 1
    }


    public enum FacilityOrderType
    {
        Maintain = 1,
        Fix = 2,
        Inspect = 3
    }

    public enum FacilityOrderStatus
    {
        Create = 0,
        Submit = 1,
        InProcess = 2,
        Close = 3
    }


    public enum CheckListOrderStatus
    {
        Create = 0,
        Submit = 1
    }

    public enum LeadTimeOption
    {
        Strategy = 0,
        ShiftDetail = 1,
    }

    public enum BarCodeMode
    {
        //NoneBarCode = 0,   //不启用条码
        Box = 1,            //按箱管理
        Pallet = 2,         //按托管理
        LotNo = 3,          //按批号管理
    }
}
