using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    [DataContract]
    public partial class PrintIpMaster : PrintBase
    {
        #region O/R Mapping Properties
        
        [DataMember]
		public string IpNo { get; set; }

        [DataMember]
		public string ExternalIpNo { get; set; }

        [DataMember]
        public Int16 OrderType { get; set; }

        [DataMember]
        public Int16 OrderSubType { get; set; }
        //public com.Sconit.CodeMaster.OrderType OrderType { get; set; }

        [DataMember]
        public Int16 QualityType { get; set; }
        //public com.Sconit.CodeMaster.QualityType QualityType { get; set; }

        [DataMember]
        public DateTime DepartTime { get; set; }

        [DataMember]
        public DateTime WindowTime { get; set; }

        [DataMember]
        public DateTime ArriveTime { get; set; }

        [DataMember]
		public string PartyFrom { get; set; }

        [DataMember]
		public string PartyFromName { get; set; }

        [DataMember]
		public string PartyTo { get; set; }

        [DataMember]
		public string PartyToName { get; set; }

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
        public string Flow { get; set; }

        [DataMember]
        public string LocationToName { get; set; }

        [DataMember]
        public string LocationTo { get; set; }

        [DataMember]
        public string LocationFromName { get; set; }

        [DataMember]
        public string LocationFrom { get; set; }

        [DataMember]
        public string AsnTemplate { get; set; }

        [DataMember]
		public string CreateUserName { get; set; }

        [DataMember]
		public DateTime CreateDate { get; set; }

        [DataMember]
        public DateTime EffectiveDate { get; set; }

        [DataMember]
        public IList<PrintIpDetail> IpDetails { get; set; }
        #endregion

    }
	
}
