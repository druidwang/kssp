using System;
using System.Runtime.Serialization;
using System.Collections.Generic;


namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    [DataContract]
    public partial class PrintReceiptMaster : PrintBase
    {
        #region O/R Mapping Properties

        [DataMember]
		public string ReceiptNo { get; set; }

        [DataMember]
        public string ExternalReceiptNo { get; set; }

        [DataMember]
		public string IpNo { get; set; }

        [DataMember]
        public Int16 OrderType { get; set; }
        //public com.Sconit.CodeMaster.OrderType OrderType { get; set; }

        [DataMember]
        public Int16 OrderSubType { get; set; }

        [DataMember]
        public Int16 QualityType { get; set; }
        //public com.Sconit.CodeMaster.QualityType QualityType { get; set; }

        [DataMember]
		public string PartyFrom { get; set; }

        [DataMember]
		public string PartyFromName { get; set; }

        [DataMember]
		public string PartyTo { get; set; }

        [DataMember]
		public string PartyToName { get; set; }

        [DataMember]
        public string LocationFrom { get; set; }

        [DataMember]
        public string LocationFromName { get; set; }

        [DataMember]
        public string LocationTo { get; set; }

        [DataMember]
        public string LocationToName { get; set; }

        [DataMember]
		public string ShipFrom { get; set; }

        [DataMember]
        public string ShipFromAddress { get; set; }

        [DataMember]
        public string ShipFromTel { get; set; }

        [DataMember]
        public string ShipFromCell { get; set; }

        [DataMember]
        public string ShipFromFax { get; set; }

        [DataMember]
        public string ShipFromContact { get; set; }

        [DataMember]
		public string ShipTo { get; set; }

        [DataMember]
        public string ShipToAddress { get; set; }

        [DataMember]
        public string ShipToTel { get; set; }

        [DataMember]
        public string ShipToCell { get; set; }

        [DataMember]
        public string ShipToFax { get; set; }

        [DataMember]
        public string ShipToContact { get; set; }

        [DataMember]
		public string Dock { get; set; }

        [DataMember]
        public DateTime EffectiveDate { get; set; }

        [DataMember]
        public string ReceiptTemplate { get; set; }

        [DataMember]
        public Int32 CreateUserId { get; set; }

        [DataMember]
		public string CreateUserName { get; set; }

        [DataMember]
		public DateTime CreateDate { get; set; }

        [DataMember]
        public string Flow { get; set; }

        [DataMember]
        public IList<PrintReceiptDetail> ReceiptDetails { get; set; }
        #endregion
    }
	
}
