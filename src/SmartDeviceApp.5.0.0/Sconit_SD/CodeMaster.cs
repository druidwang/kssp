using System;

using System.Collections.Generic;
using System.Text;

namespace com.Sconit.SmartDevice.CodeMaster
{
    public enum TerminalPermission
    {
        //�շ�
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
        Client_SQShip,//˫�ŷ���
        Client_QuickSeqShip,
        Client_PurchaseReturn,
        Client_ProductionReceive,
        Client_QuickTransfer,     //���䷢��
        Client_ProductionReturn,   //��Ʒ������
        Client_DistributionReturn,  //�����˻�


        //�ֿ�
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

        //����
        Client_Inspect,
        Client_Qualify,
        Client_Reject,
        Client_Freeze,
        Client_UnFreeze,
        Client_WorkerWaste,

        //����
        Client_AnDon,
        Client_ProductionOnline,   //��������
        Client_MaterialIn,         //����Ͷ��
        Client_ProductionOffline,      //��������
        Client_ChassisOnline,   //��������
        Client_CabOnline,   //��ʻ������
        Client_TransKeyScan,
        Client_AssemblyOnline,  //��װ����
        Client_MaterialReturn,         //����Ͷ��
        Client_SubAssemblyOffLine,    //��װ����������
        Client_ForceMaterialIn,
        Client_FiReceipt,
        Client_StartAging,
        Client_Aging,
        Client_Filter,
        Client_SparePartChk,

        //��ת��
        Client_EmptyContainerShip,//���䷢��
        Client_EmptyContainerReceive,//�����ջ�
        Client_FullContainerReceive,//�����ջ�

        Client_Location_Bin_Transfer,//����λ,����ƿ�

        //�߼��ֿ�
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
        /// ������ʵ��
        /// </summary>
        Z,
        /// <summary>
        /// ������
        /// </summary>
        F,
        /// <summary>
        /// ���
        /// </summary>
        B,
        /// <summary>
        /// ����
        /// </summary>
        D,
        /// <summary>
        /// ����
        /// </summary>
        I,
        /// <summary>
        /// ��λ
        /// </summary>
        L,
        /// <summary>
        /// ��ת��
        /// </summary>
        C,
        /// <summary>
        /// ����
        /// </summary>
        K,
        /// <summary>
        /// WMS�ͻ�����
        /// </summary>
        W,
        /// <summary>
        /// WMS�����ͻ�����
        /// </summary>
        SP,
        HU,
        /// <summary>
        /// ���ͱ�ǩ
        /// </summary>
        DC,
        DATE,
        /// <summary>
        /// ����
        /// </summary>
        TP
    }

    public enum MaterialInType
    {
        NA,
        /// <summary>
        /// ������Ͷ��
        /// </summary>
        Hu2Flow,
        /// <summary>
        /// ����Ͷ�ϵ�������
        /// </summary>
        Hu2WO,
        /// <summary>
        /// ����Ͷ�ϵ�KIT
        /// </summary>
        Hu2KIT,
        /// <summary>
        /// KITͶ�ϵ�KIT
        /// </summary>
        KIT2KIT,
        /// <summary>
        /// KITͶ�ϵ�������
        /// </summary>
        KIT2WO,
        /// <summary>
        /// ������Ͷ�ϵ�������
        /// </summary>
        WO2WO,
    }

    public enum MaterialReturnType
    {
        /// <summary>
        /// ������������
        /// </summary>
        Flow2Hu,
        /// <summary>
        /// ���������ϵ�����
        /// </summary>
        WO2Hu,
    }
}
