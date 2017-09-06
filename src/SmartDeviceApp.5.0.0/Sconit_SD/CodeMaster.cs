using System;

using System.Collections.Generic;
using System.Text;

namespace com.Sconit.SmartDevice.CodeMaster
{
    public enum TerminalPermission
    {
        //收发
        Client_Receive,
        Client_PickListOnline,
        Client_PickList,
        Client_PickListShip,
        Client_OrderShip,
        Client_SeqPack,
        Client_SeqCancel,
        Client_SeqShip,
        Client_QuickReturn,
        Client_ForceRecive,
        Client_QuickPick,
        Client_SQShip,//双桥发货
        Client_QuickSeqShip,
        Client_PurchaseReturn,
        Client_ProductionReceive,
        Client_QuickTransfer,     //车间发料
        Client_ProductionReturn,   //成品入库冲销
        Client_DistributionReturn,  //销售退货


        //仓库
        Client_Transfer,
        Client_RePack,
        Client_Pack,
        Client_UnPack,
        Client_PutAway,
        Client_Pickup,
        Client_StockTaking,
        Client_HuStatus,
        Client_MiscInOut,
        Client_HuClone,
        Client_BindContainerIn,
        Client_BindContainerOut,
        //Client_MiscOut,

        //质量
        Client_Inspect,
        Client_Qualify,
        Client_Reject,
        Client_Freeze,
        Client_UnFreeze,
        Client_WorkerWaste,

        //生产
        Client_AnDon,
        Client_ProductionOnline,   //生产上线
        Client_MaterialIn,         //生产投料
        Client_ProductionOffline,      //生产下线
        Client_ChassisOnline,   //底盘上线
        Client_CabOnline,   //驾驶室上线
        Client_TransKeyScan,
        Client_AssemblyOnline,  //总装上线
        Client_MaterialReturn,         //生产投料
        Client_SubAssemblyOffLine,    //分装生产单下线
        Client_ForceMaterialIn,
        Client_FiReceipt,
        Client_StartAging,
        Client_Aging,
        Client_Filter,
        Client_SparePartChk,

        //周转箱
        Client_EmptyContainerShip,//空箱发货
        Client_EmptyContainerReceive,//空箱收货
        Client_FullContainerReceive,//满箱收货

        Client_Location_Bin_Transfer,//按库位,库格移库

        //高级仓库
        Client_WMSPickGoods,
        Client_WMSDeliverBarCode,
        Client_WMSPickGoodsQty,
        Client_WMSTransfer,
        Client_WMSShip,
        Client_WMSRepack,

        M_Switch,
    }

    public enum BarCodeType
    {
        ORD,
        ASN,
        PIK,
        STT,
        INS,
        SEQ,
        MIS,
        /// <summary>
        /// 
        /// </summary>
        COT,
        T,
        /// <summary>
        /// 生产线实例
        /// </summary>
        Z,
        /// <summary>
        /// 生产线
        /// </summary>
        F,
        /// <summary>
        /// 库格
        /// </summary>
        B,
        /// <summary>
        /// 道口
        /// </summary>
        D,
        /// <summary>
        /// 检验
        /// </summary>
        I,
        /// <summary>
        /// 库位
        /// </summary>
        L,
        /// <summary>
        /// 周转箱
        /// </summary>
        C,
        /// <summary>
        /// 看板
        /// </summary>
        K,
        /// <summary>
        /// WMS送货单号
        /// </summary>
        W,
        /// <summary>
        /// WMS排序送货单号
        /// </summary>
        SP,
        HU,
        /// <summary>
        /// 配送标签
        /// </summary>
        DC,
        DATE,
        /// <summary>
        /// 托盘
        /// </summary>
        TP
    }

    public enum MaterialInType
    {
        NA,
        /// <summary>
        /// 生产线投料
        /// </summary>
        Hu2Flow,
        /// <summary>
        /// 条码投料到生产单
        /// </summary>
        Hu2WO,
        /// <summary>
        /// 条码投料到KIT
        /// </summary>
        Hu2KIT,
        /// <summary>
        /// KIT投料到KIT
        /// </summary>
        KIT2KIT,
        /// <summary>
        /// KIT投料到生产单
        /// </summary>
        KIT2WO,
        /// <summary>
        /// 生产单投料到生产单
        /// </summary>
        WO2WO,
    }

    public enum MaterialReturnType
    {
        /// <summary>
        /// 生产线退料料
        /// </summary>
        Flow2Hu,
        /// <summary>
        /// 生产单退料到条码
        /// </summary>
        WO2Hu,
    }
}
