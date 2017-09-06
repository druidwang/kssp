using System;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    public partial class PrintReceiptDetail : PrintBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        
        [DataMember]
        public string ReceiptNo { get; set; }

        [DataMember]
        public Int32 Sequence { get; set; }

        [DataMember]
        public Int32 IpDetailSequence { get; set; }

        [DataMember]
        public string Item { get; set; }
        
        [DataMember]
        public string ItemDescription { get; set; }

        [DataMember]
        public string ReferenceItemCode { get; set; }

        [DataMember]
        public string Uom { get; set; }

        [DataMember]
        public Decimal UnitCount { get; set; }

        [DataMember]
        public Int16 QualityType { get; set; }
        //public com.Sconit.CodeMaster.QualityType QualityType { get; set; }

        [DataMember]
        public Decimal ReceivedQty { get; set; }

        [DataMember]
        public string LocationFrom { get; set; }

        [DataMember]
        public string LocationFromName { get; set; }

        [DataMember]
        public string LocationTo { get; set; }

        [DataMember]
        public string LocationToName { get; set; }

        [DataMember]
        public Boolean IsInspect { get; set; }

        [DataMember]
        public Int32 CreateUserId { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public string ManufactureParty { get; set; }

        [DataMember]
        public Decimal ShippedQty { get; set; }

        [DataMember]
        public string OrderNo { get; set; }

        [DataMember]
        public Int16 IpDetailType { get; set; }

        [DataMember]
        public int BoxQty { get; set; }
        #endregion
    }
	
}
