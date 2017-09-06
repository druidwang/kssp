using System;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    public partial class PrintPickListDetail : PrintBase
    {
        #region O/R Mapping Properties
        [DataMember]
		public Int32 Id { get; set; }


        [DataMember]
        public Int32 Sequence { get; set; }

        [DataMember]
        public string Direction { get; set; }

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
        public string ManufactureParty { get; set; }

        [DataMember]
        public string LocationFrom { get; set; }

        [DataMember]
		public string LocationFromName { get; set; }

        [DataMember]
        public string Area { get; set; }

        [DataMember]
		public string Bin { get; set; }

        [DataMember]
        public string LocationTo { get; set; }

        [DataMember]
        public string LocationToName { get; set; }

        [DataMember]
		public Decimal Qty { get; set; }

        [DataMember]
		public string HuId { get; set; }

        [DataMember]
        public string LotNo { get; set; }

        [DataMember]
        public Boolean IsInspect { get; set; }

        [DataMember]
        public string Memo { get; set; }

        [DataMember]
        public Boolean IsOdd { get; set; }

        [DataMember]
        public Boolean IsDevan { get; set; }

        [DataMember]
        public Boolean IsInventory { get; set; }

        [DataMember]
        public Decimal PickedQty { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }
		
        #endregion
    }
	
}
