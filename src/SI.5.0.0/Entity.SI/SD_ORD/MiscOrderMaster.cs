using System;

namespace com.Sconit.Entity.SI.SD_ORD
{
    [Serializable]
    public partial class MiscOrderMaster 
    {
        #region O/R Mapping Properties

        //计划外出入库单号
        public string MiscOrderNo { get; set; }

        //计划外出入库类型
        public com.Sconit.CodeMaster.MiscOrderType Type { get; set; }

        //状态
        public com.Sconit.CodeMaster.MiscOrderStatus Status { get; set; }

        //是否扫描条码
        public Boolean IsScanHu { get; set; }

        //质量状态
        public CodeMaster.QualityType QualityType { get; set; }

        //移动类型
        public string MoveType { get; set; }

        //冲销时的移动类型
        public string CancelMoveType { get; set; }

        //供货工厂对应的区域，跨工厂移库时代表出库区域
        public string Region { get; set; }

        //库存地点对应的库位
        public string Location { get; set; }

        //收货地点，为SAP库位不用翻译
        public string ReceiveLocation { get; set; }

        //移动原因
        public string Note { get; set; }

        //成本重新
        public string CostCenter { get; set; }

        //出货通知
        public string Asn { get; set; }

        //内部订单号
        public string ReferenceNo { get; set; }

        //工厂代码，为SAP代码，非跨工厂移库时，会从Region字段找到对应的Plant填到该字段上面
        public string DeliverRegion { get; set; }

        //WBS元素
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
