using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class PickListMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Export(ExportName = "PickListMaster", ExportSeq = 10)]
        [Display(Name = "PickListMaster_PickListNo", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string PickListNo { get; set; }
        [Export(ExportName = "PickListMaster", ExportSeq = 30)]
        [Display(Name = "PickListMaster_Flow", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string Flow { get; set; }

        [Display(Name = "PickListMaster_Status", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public com.Sconit.CodeMaster.PickListStatus Status { get; set; }

        [Display(Name = "PickListMaster_OrderType", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public com.Sconit.CodeMaster.OrderType OrderType { get; set; }

        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }

        [Display(Name = "PickListMaster_StartTime", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public DateTime StartTime { get; set; }

        [Display(Name = "PickListMaster_WindowTime", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public DateTime WindowTime { get; set; }

        [Display(Name = "PickListMaster_PartyFrom", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string PartyFrom { get; set; }
        [Export(ExportName = "PickListMaster", ExportSeq = 50)]
        [Display(Name = "PickListMaster_PartyFromName", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string PartyFromName { get; set; }

        [Display(Name = "PickListMaster_PartyTo", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string PartyTo { get; set; }
        [Export(ExportName = "PickListMaster", ExportSeq = 60)]
        [Display(Name = "PickListMaster_PartyToName", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string PartyToName { get; set; }

        [Display(Name = "PickListMaster_ShipFrom", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipFrom { get; set; }

        [Display(Name = "PickListMaster_ShipFromAddress", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipFromAddress { get; set; }

        [Display(Name = "PickListMaster_ShipFromTel", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipFromTel { get; set; }

        [Display(Name = "PickListMaster_ShipFromCell", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipFromCell { get; set; }

        [Display(Name = "PickListMaster_ShipFromFax", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipFromFax { get; set; }

        [Display(Name = "PickListMaster_ShipFromContact", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipFromContact { get; set; }

        [Display(Name = "PickListMaster_ShipTo", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipTo { get; set; }

        [Display(Name = "PickListMaster_ShipToAddress", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipToAddress { get; set; }

        [Display(Name = "PickListMaster_ShipToTel", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipToTel { get; set; }

        [Display(Name = "PickListMaster_ShipToCell", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipToCell { get; set; }

        [Display(Name = "PickListMaster_ShipToFax", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipToFax { get; set; }

        [Display(Name = "PickListMaster_ShipToContact", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string ShipToContact { get; set; }

        [Display(Name = "PickListMaster_Dock", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string Dock { get; set; }

        [Display(Name = "PickListMaster_IsAutoReceive", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsAutoReceive { get; set; }

        [Display(Name = "PickListMaster_IsReceiveScanHu", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsReceiveScanHu { get; set; }

        [Display(Name = "PickListMaster_IsPrintAsn", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsPrintAsn { get; set; }

        [Display(Name = "PickListMaster_IsPrintReceipt", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsPrintReceipt { get; set; }

        [Display(Name = "PickListMaster_IsReceiveExceed", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsReceiveExceed { get; set; }

        [Display(Name = "PickListMaster_IsReceiveFulfillUC", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsReceiveFulfillUC { get; set; }

        [Display(Name = "PickListMaster_IsReceiveFifo", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsReceiveFifo { get; set; }

        [Display(Name = "PickListMaster_IsAsnUniqueReceive", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsAsnUniqueReceive { get; set; }

        public CodeMaster.CreateHuOption CreateHuOption { get; set; }

        [Display(Name = "PickListMaster_IsCheckPartyFromAuthority", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsCheckPartyFromAuthority { get; set; }

        [Display(Name = "PickListMaster_IsCheckPartyToAuthority", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Boolean IsCheckPartyToAuthority { get; set; }

		//[Display(Name = "ReceiveGapTo", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public com.Sconit.CodeMaster.ReceiveGapTo ReceiveGapTo { get; set; }

        [Display(Name = "PickListMaster_AsnTemplate", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string AsnTemplate { get; set; }

        [Display(Name = "PickListMaster_ReceiptTemplate", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string ReceiptTemplate { get; set; }

        [Display(Name = "PickListMaster_HuTemplate", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string HuTemplate { get; set; }

		//[Display(Name = "EffectiveDate", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public DateTime EffectiveDate { get; set; }

		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.PickListMaster))]


        public Int32 CreateUserId { get; set; }
        [Export(ExportName = "PickListMaster", ExportSeq = 80)]
        [Display(Name = "PickListMaster_CreateUserName", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string CreateUserName { get; set; }
        [Export(ExportName = "PickListMaster", ExportSeq = 90)]
        [Display(Name = "PickListMaster_CreateDate", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public DateTime CreateDate { get; set; }

		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Int32 LastModifyUserId { get; set; }

        [Display(Name = "PickListMaster_LastModifyUserName", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string LastModifyUserName { get; set; }

        [Display(Name = "PickListMaster_LastModifyDate", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public DateTime LastModifyDate { get; set; }

        //public DateTime? ReleaseDate { get; set; }

        //public Int32? ReleaseUserId { get; set; }

        //public string ReleaseUserName { get; set; }

        [Display(Name = "PickListMaster_StartDate", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public DateTime? StartDate { get; set; }
		//[Display(Name = "StartUser", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public Int32? StartUserId { get; set; }
        [Display(Name = "PickListMaster_StartUserName", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string StartUserName { get; set; }
        [Display(Name = "PickListMaster_CompleteDate", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public DateTime? CompleteDate { get; set; }
		//[Display(Name = "CompleteUser", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public Int32? CompleteUserId { get; set; }
        [Display(Name = "PickListMaster_CompleteUserName", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string CompleteUserName { get; set; }
       [Display(Name = "PickListMaster_CloseDate", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public DateTime? CloseDate { get; set; }
		//[Display(Name = "CloseUser", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public Int32? CloseUserId { get; set; }
        [Display(Name = "PickListMaster_CloseUserName", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string CloseUserName { get; set; }
        [Display(Name = "PickListMaster_CancelDate", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public DateTime? CancelDate { get; set; }
		//[Display(Name = "CancelUser", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public Int32? CancelUserId { get; set; }
        [Display(Name = "PickListMaster_CancelUserName", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string CancelUserName { get; set; }
        [Display(Name = "PickListMaster_CancelReason", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public string CancelReason { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.ORD.PickListMaster))]
		public Int32 Version { get; set; }
        [Export(ExportName = "PickListMaster", ExportSeq = 20)]
        [Display(Name = "PickListMaster_IpNo", ResourceType = typeof(Resources.ORD.PickListMaster))]
        public string IpNo { get; set; }
        #endregion

		public override int GetHashCode()
        {
			if (PickListNo != null)
            {
                return PickListNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            PickListMaster another = obj as PickListMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.PickListNo == another.PickListNo);
            }
        } 
    }
	
}
