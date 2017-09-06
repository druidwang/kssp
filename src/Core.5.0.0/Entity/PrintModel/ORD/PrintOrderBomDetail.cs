using System;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    [DataContract]
    public partial class PrintOrderBomDetail
    {
        #region O/R Mapping Properties
        [DataMember]
        public Int32 Id { get; set; }

        [DataMember]
        public string OrderNo { get; set; }

        [DataMember]
        public Int16 OrderType { get; set; }

        [DataMember]
        public Int16 OrderSubType { get; set; }

        [DataMember]
        public Int32 OrderDetailId { get; set; }

        [DataMember]
        public Int32 OrderDetailSequence { get; set; }

        [DataMember]
        public Int32 Sequence { get; set; }

        [DataMember]
        public string Bom { get; set; }

        [DataMember]
        public string Item { get; set; }

        [DataMember]
        public string ReferenceItemCode { get; set; }

        [DataMember]
        public string ItemDescription { get; set; }

        [DataMember]
        public string Uom { get; set; }  //Bomµ¥Î»

        [DataMember]
        public string BaseUom { get; set; }

        [DataMember]
        public string ManufactureParty { get; set; }

        [DataMember]
        public Int32 Operation { get; set; }

        [DataMember]
        public string OpReference { get; set; }

        [DataMember]
        public Decimal OrderedQty { get; set; }

        [DataMember]
        public Decimal BackflushedQty { get; set; }

        [DataMember]
        public Decimal BackflushedRejectQty { get; set; }

        [DataMember]
        public Decimal BackflushedScrapQty { get; set; }

        [DataMember]
        public Decimal UnitQty { get; set; }

        [DataMember]
        public Decimal BomUnitQty { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public Boolean IsPrint { get; set; }

        [DataMember]
        public Int16 BackFlushMethod { get; set; }

        [DataMember]
        public Int16 FeedMethod { get; set; }

        [DataMember]
        public Boolean IsScanHu { get; set; }

        [DataMember]
        public Boolean IsAutoFeed { get; set; }

        [DataMember]
        public DateTime EstimateConsumeTime { get; set; }

        [DataMember]
        public Int32 CreateUserId { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        #endregion
    }

}
