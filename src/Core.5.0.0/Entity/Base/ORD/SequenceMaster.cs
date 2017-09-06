using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class SequenceMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "SequenceMaster_SequenceNo", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string SequenceNo { get; set; }
        [Display(Name = "SequenceMaster_Flow", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string Flow { get; set; }
        [Display(Name = "SequenceMaster_Status", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public CodeMaster.SequenceStatus Status { get; set; }
        [Display(Name = "SequenceMaster_OrderType", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        [Display(Name = "SequenceMaster_QualityType", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }

       // [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd/HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "SequenceMaster_StartTime", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public DateTime StartTime { get; set; }

        //[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "SequenceMaster_WindowTime", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public DateTime WindowTime { get; set; }
        [Display(Name = "SequenceMaster_PartyFrom", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string PartyFrom { get; set; }
        [Display(Name = "SequenceMaster_PartyFromName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string PartyFromName { get; set; }
        [Display(Name = "SequenceMaster_PartyTo", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string PartyTo { get; set; }
        [Display(Name = "SequenceMaster_PartyToName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string PartyToName { get; set; }
        [Display(Name = "SequenceMaster_ShipFrom", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipFrom { get; set; }
        [Display(Name = "SequenceMaster_ShipFromAddress", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipFromAddress { get; set; }
        [Display(Name = "SequenceMaster_ShipFromTel", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipFromTel { get; set; }
        [Display(Name = "SequenceMaster_ShipFromCell", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipFromCell { get; set; }
        [Display(Name = "SequenceMaster_ShipFromFax", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipFromFax { get; set; }
        [Display(Name = "SequenceMaster_ShipFromContact", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipFromContact { get; set; }
        [Display(Name = "SequenceMaster_ShipTo", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipTo { get; set; }
        [Display(Name = "SequenceMaster_ShipToAddress", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipToAddress { get; set; }
        [Display(Name = "SequenceMaster_ShipToTel", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipToTel { get; set; }
        [Display(Name = "SequenceMaster_ShipToCell", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipToCell { get; set; }
        [Display(Name = "SequenceMaster_ShipToFax", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipToFax { get; set; }
        [Display(Name = "SequenceMaster_ShipToContact", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ShipToContact { get; set; }
        [Display(Name = "SequenceMaster_LocationFrom", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string LocationFrom { get; set; }
        [Display(Name = "SequenceMaster_LocationFromName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string LocationFromName { get; set; }
        [Display(Name = "SequenceMaster_LocationTo", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string LocationTo { get; set; }
        [Display(Name = "SequenceMaster_LocationToName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string LocationToName { get; set; }
        [Display(Name = "SequenceMaster_Dock", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string Dock { get; set; }
        [Display(Name = "SequenceMaster_Container", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string Container { get; set; }
        [Display(Name = "SequenceMaster_ContainerDescription", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string ContainerDescription { get; set; }
        [Display(Name = "SequenceMaster_IsAutoReceive", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public Boolean IsAutoReceive { get; set; }
        [Display(Name = "SequenceMaster_IsPrintAsn", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public Boolean IsPrintAsn { get; set; }
        [Display(Name = "SequenceMaster_IsPrintReceipt", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public Boolean IsPrintReceipt { get; set; }
        public Boolean IsCheckPartyFromAuthority { get; set; }
        public Boolean IsCheckPartyToAuthority { get; set; }
        [Display(Name = "SequenceMaster_AsnTemplate", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public string AsnTemplate { get; set; }
        [Display(Name = "SequenceMaster_ReceiptTemplate", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public string ReceiptTemplate { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public Int32 CreateUserId { get; set; }
        [Display(Name = "SequenceMaster_CreateUserName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string CreateUserName { get; set; }
       [Display(Name = "SequenceMaster_CreateDate", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public Int32 LastModifyUserId { get; set; }
        [Display(Name = "SequenceMaster_LastModifyUserName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public string LastModifyUserName { get; set; }
       [Display(Name = "SequenceMaster_LastModifyDate", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public DateTime LastModifyDate { get; set; }
       [Display(Name = "SequenceMaster_Version", ResourceType = typeof(Resources.ORD.SequenceMaster))]
		public Int32 Version { get; set; }
        public Int32 PackUserId { get; set; }
        [Display(Name = "SequenceMaster_PackUserName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public string PackUserName { get; set; }
        [Display(Name = "SequenceMaster_PackDate", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public DateTime? PackDate { get; set; }
        public Int32 ShipUserId { get; set; }
        [Display(Name = "SequenceMaster_ShipUserName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public string ShipUserName { get; set; }
        [Display(Name = "SequenceMaster_ShipDate", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public DateTime? ShipDate { get; set; }
        [Display(Name = "SequenceMaster_CancelDate", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public DateTime? CancelDate { get; set; }
        
        public Int32? CancelUserId { get; set; }
        [Display(Name = "SequenceMaster_CancelUserName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public string CancelUserName { get; set; }
        [Display(Name = "SequenceMaster_CloseDate", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public DateTime? CloseDate { get; set; }
        public Int32? CloseUserId { get; set; }
        [Display(Name = "SequenceMaster_CloseUserName", ResourceType = typeof(Resources.ORD.SequenceMaster))]
        public string CloseUserName { get; set; }
        #endregion

		public override int GetHashCode()
        {
			if (SequenceNo != null)
            {
                return SequenceNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            SequenceMaster another = obj as SequenceMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.SequenceNo == another.SequenceNo);
            }
        } 
    }
	
}
