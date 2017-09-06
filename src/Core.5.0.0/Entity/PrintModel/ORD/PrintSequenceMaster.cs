using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using com.Sconit.PrintModel;


namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    [DataContract]
    public partial class PrintSequenceMaster : PrintBase
    {
        #region O/R Mapping Properties

        [DataMember]
        public string SequenceNo { get; set; }
        
        [DataMember]
        public string Flow { get; set; }
        
        [DataMember]
        public Int16 Status { get; set; }
        
        [DataMember]
        public Int16 OrderType { get; set; }
        
        [DataMember]
        public Int16 QualityType { get; set; }

        [DataMember]
		public DateTime StartTime { get; set; }

        [DataMember]
        public DateTime WindowTime { get; set; }
        
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
        public string LocationFrom { get; set; }

        [DataMember]
        public string LocationFromName { get; set; }

        [DataMember]
        public string LocationTo { get; set; }

        [DataMember]
        public string LocationToName { get; set; }

        [DataMember]
        public string Dock { get; set; }

        [DataMember]
        public string Container { get; set; }

        [DataMember]
        public string ContainerDescription { get; set; }

        [DataMember]
        public Boolean IsAutoReceive { get; set; }

        [DataMember]
        public Boolean IsPrintAsn { get; set; }

        [DataMember]
        public Boolean IsPrintReceipt { get; set; }

        [DataMember]
        public Boolean IsCheckPartyFromAuthority { get; set; }

        [DataMember]
        public Boolean IsCheckPartyToAuthority { get; set; }

        [DataMember]
        public string AsnTemplate { get; set; }

        [DataMember]
        public string ReceiptTemplate { get; set; }

        [DataMember]
        public Int32 CreateUserId { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public Int32 PackUserId { get; set; }

        [DataMember]
        public string PackUserName { get; set; }

        [DataMember]
        public DateTime PackDate { get; set; }

        [DataMember]
        public Int32 ShipUserId { get; set; }

        [DataMember]
        public string ShipUserName { get; set; }

        [DataMember]
        public DateTime ShipDate { get; set; }

        [DataMember]
        public DateTime? CancelDate { get; set; }

        [DataMember]
        public Int32? CancelUserId { get; set; }

        [DataMember]
        public string CancelUserName { get; set; }

        [DataMember]
        public DateTime? CloseDate { get; set; }

        [DataMember]
        public Int32? CloseUserId { get; set; }

        [DataMember]
        public string CloseUserName { get; set; }

        [DataMember]
        public IList<PrintSequenceDetail> SequenceDetails { get; set; }
        #endregion

    }
	
}
