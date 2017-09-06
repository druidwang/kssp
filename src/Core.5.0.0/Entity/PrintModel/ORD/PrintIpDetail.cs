using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    public partial class PrintIpDetail : PrintBase
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }

        [DataMember]
		public string IpNo { get; set; }

        [DataMember]
        public Int32 Sequence { get; set; }

        [DataMember]
        public string OrderNo { get; set; }

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
        public string ManufactureParty { get; set; }

        [DataMember]
		public Decimal Qty { get; set; }

        [DataMember]
        public decimal BoxQty { get; set; }

        [DataMember]
        public Decimal ReceivedQty { get; set; }

        [DataMember]
        public Decimal ShippedQty { get; set; }

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
        public DateTime? WindowTime { get; set; }

        [DataMember]
        public Int32 BillTerm { get; set; }

        [DataMember]
        public string BinTo { get; set; }

        [DataMember]
        public string ExternalOrderNo { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public string Tax { get; set; }
        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public decimal GapQty { get; set; }

        [DataMember]
        public decimal OrderQty { get; set; }
        #endregion
    }
	
}
