using System;

namespace com.Sconit.Entity.SI.SD_ORD
{
    [Serializable]
    public partial class MiscOrderMaster 
    {
        #region O/R Mapping Properties

        //�ƻ������ⵥ��
        public string MiscOrderNo { get; set; }

        //�ƻ�����������
        public com.Sconit.CodeMaster.MiscOrderType Type { get; set; }

        //״̬
        public com.Sconit.CodeMaster.MiscOrderStatus Status { get; set; }

        //�Ƿ�ɨ������
        public Boolean IsScanHu { get; set; }

        //����״̬
        public CodeMaster.QualityType QualityType { get; set; }

        //�ƶ�����
        public string MoveType { get; set; }

        //����ʱ���ƶ�����
        public string CancelMoveType { get; set; }

        //����������Ӧ�����򣬿繤���ƿ�ʱ�����������
        public string Region { get; set; }

        //���ص��Ӧ�Ŀ�λ
        public string Location { get; set; }

        //�ջ��ص㣬ΪSAP��λ���÷���
        public string ReceiveLocation { get; set; }

        //�ƶ�ԭ��
        public string Note { get; set; }

        //�ɱ�����
        public string CostCenter { get; set; }

        //����֪ͨ
        public string Asn { get; set; }

        //�ڲ�������
        public string ReferenceNo { get; set; }

        //�������룬ΪSAP���룬�ǿ繤���ƿ�ʱ�����Region�ֶ��ҵ���Ӧ��Plant����ֶ�����
        public string DeliverRegion { get; set; }

        //WBSԪ��
        public string WBS { get; set; }

        public DateTime EffectiveDate { get; set; }

        //public Int32 CreateUserId { get; set; }

        //public string CreateUserName { get; set; }

        //public DateTime CreateDate { get; set; }

        //public Int32 LastModifyUserId { get; set; }

        //public string LastModifyUserName { get; set; }

        //public DateTime LastModifyDate { get; set; }

        //public DateTime? CloseDate { get; set; }

        //public Int32? CloseUserId { get; set; }

        //public string CloseUserName { get; set; }

        //public DateTime? CancelDate { get; set; }

        //public Int32? CancelUserId { get; set; }

        //public string CancelUserName { get; set; }

        public Int32 Version { get; set; }
        #endregion

    }

}
