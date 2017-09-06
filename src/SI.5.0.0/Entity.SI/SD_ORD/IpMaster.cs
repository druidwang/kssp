using System;
using System.Collections.Generic;
namespace com.Sconit.Entity.SI.SD_ORD
{
    [Serializable]
    public partial class IpMaster 
    {
        #region O/R Mapping Properties

		public string IpNo { get; set; }
		public string ExternalIpNo { get; set; }
		public string GapIpNo { get; set; }
        public com.Sconit.CodeMaster.IpType Type { get; set; }
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public com.Sconit.CodeMaster.IpStatus Status { get; set; }
		public DateTime DepartTime { get; set; }
		public DateTime ArriveTime { get; set; }
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
        //public string ShipTo { get; set; }
        //public string ShipToAddress { get; set; }
        //public string ShipToTel { get; set; }
        //public string ShipToCell { get; set; }
        //public string ShipToFax { get; set; }
        //public string ShipToContact { get; set; }
		public string Dock { get; set; }
		public Boolean IsAutoReceive { get; set; }
        public Boolean IsShipScanHu { get; set; }
        public Boolean IsReceiveScanHu { get; set; }
		public Boolean IsPrintAsn { get; set; }
		public Boolean IsAsnPrinted { get; set; }
		public Boolean IsPrintReceipt { get; set; }
		public Boolean IsReceiveExceed { get; set; }
		public Boolean IsReceiveFulfillUC { get; set; }
		public Boolean IsReceiveFifo { get; set; }
		public Boolean IsAsnAutoClose { get; set; }
		public Boolean IsAsnUniqueReceive { get; set; }
        public com.Sconit.CodeMaster.ReceiveGapTo ReceiveGapTo { get; set; }
        //public string AsnTemplate { get; set; }
        //public string ReceiptTemplate { get; set; }
        //public string HuTemplate { get; set; }
        //public Int32 CreateUserId { get; set; }
        //public string CreateUserName { get; set; }
        //public DateTime CreateDate { get; set; }
        //public Int32 LastModifyUserId { get; set; }
        //public string LastModifyUserName { get; set; }
        //public DateTime LastModifyDate { get; set; }
        //public DateTime? CloseDate { get; set; }
        //public Int32? CloseUserId { get; set; }
        //public string CloseUserName { get; set; }
        //public string CloseReason { get; set; }
        //public Int32 Version { get; set; }
        public Boolean IsCheckPartyFromAuthority { get; set; }
        public Boolean IsCheckPartyToAuthority { get; set; }
        public DateTime EffectiveDate { get; set; }
        #endregion

        public List<IpDetail> IpDetails { get; set; }

        public List<IpDetailInput> IpDetailInputs { get; set; }
    }
	
}
