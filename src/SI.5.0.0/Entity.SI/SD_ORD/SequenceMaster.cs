using System;
using System.Collections.Generic;

namespace com.Sconit.Entity.SI.SD_ORD
{
    [Serializable]
    public partial class SequenceMaster 
    {
		public string SequenceNo { get; set; }
		public string Flow { get; set; }
        public CodeMaster.SequenceStatus Status { get; set; }
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime WindowTime { get; set; }
		public string PartyFrom { get; set; }
		//public string PartyFromName { get; set; }
		public string PartyTo { get; set; }
		//public string PartyToName { get; set; }
		public string ShipFrom { get; set; }
        //public string ShipFromAddress { get; set; }
        //public string ShipFromTel { get; set; }
        //public string ShipFromCell { get; set; }
        //public string ShipFromFax { get; set; }
        //public string ShipFromContact { get; set; }
		public string ShipTo { get; set; }
        //public string ShipToAddress { get; set; }
        //public string ShipToTel { get; set; }
        //public string ShipToCell { get; set; }
        //public string ShipToFax { get; set; }
        //public string ShipToContact { get; set; }
		public string LocationFrom { get; set; }
		//public string LocationFromName { get; set; }
		public string LocationTo { get; set; }
		//public string LocationToName { get; set; }
		public string Dock { get; set; }
		//public string Container { get; set; }
		//public string ContainerDescription { get; set; }
        public Boolean IsAutoReceive { get; set; }
        public Boolean IsPrintAsn { get; set; }
        public Boolean IsPrintReceipt { get; set; }
        public Boolean IsCheckPartyFromAuthority { get; set; }
        public Boolean IsCheckPartyToAuthority { get; set; }
        //public string AsnTemplate { get; set; }
        //public string ReceiptTemplate { get; set; }
        //public Int32 PackUserId { get; set; }
        //public string PackUserName { get; set; }
        //public DateTime PackDate { get; set; }
        //public Int32 ShipUserId { get; set; }
        //public string ShipUserName { get; set; }
        //public DateTime ShipDate { get; set; }
        //public DateTime? CancelDate { get; set; }
        //public Int32? CancelUserId { get; set; }
        //public string CancelUserName { get; set; }
        //public DateTime? CloseDate { get; set; }
        //public Int32? CloseUserId { get; set; }
        //public string CloseUserName { get; set; }

        public List<SequenceDetail> SequenceDetails { get; set; } 

    }
	
}
