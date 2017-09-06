using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.INP
{
    [Serializable]
    public partial class InspectMaster : EntityBase,IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "InspectMaster_InspectNo", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string InspectNo { get; set; }
        [Display(Name = "InspectMaster_ReferenceNo", ResourceType = typeof(Resources.INP.InspectMaster))]
		public string ReferenceNo { get; set; }
        [Display(Name = "InspectMaster_Region", ResourceType = typeof(Resources.INP.InspectMaster))]
		public string Region { get; set; }
        [Display(Name = "InspectMaster_Status", ResourceType = typeof(Resources.INP.InspectMaster))]
        public com.Sconit.CodeMaster.InspectStatus Status { get; set; }
        [Display(Name = "InspectMaster_IsATP", ResourceType = typeof(Resources.INP.InspectMaster))]
        public Boolean IsATP { get; set; }
		[Display(Name = "InspectMaster_IsPrint", ResourceType = typeof(Resources.INP.InspectMaster))]
		public Boolean IsPrint { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.INP.InspectMaster))]
		public Int32 CreateUserId { get; set; }
        [Display(Name = "InspectMaster_CreateUserName", ResourceType = typeof(Resources.INP.InspectMaster))]
		public string CreateUserName { get; set; }
        [Display(Name = "InspectMaster_CreateDate", ResourceType = typeof(Resources.INP.InspectMaster))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INP.InspectMaster))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INP.InspectMaster))]
        [Display(Name = "InspectMaster_LastModifyUserNm", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.INP.InspectMaster))]
        [Display(Name = "InspectMaster_LastModifyDate", ResourceType = typeof(Resources.INP.InspectMaster))]
		public DateTime LastModifyDate { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.INP.InspectMaster))]
		public Int32 Version { get; set; }
        [Display(Name = "InspectMaster_Type", ResourceType = typeof(Resources.INP.InspectMaster))]
        public com.Sconit.CodeMaster.InspectType Type { get; set; }
        public string ManufactureParty { get; set; }
        [Display(Name = "InspectMaster_WMSNo", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string WMSNo { get; set; }
        [Display(Name = "InspectMaster_IpNo", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string IpNo { get; set; }
        [Display(Name = "InspectMaster_ReceiptNo", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string ReceiptNo { get; set; }

        [Display(Name = "InspectMaster_PartyFrom", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string PartyFrom { get; set; }
        [Display(Name = "InspectMaster_PartyFromName", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string PartyFromName { get; set; }
        #endregion

		public override int GetHashCode()
        {
			if (InspectNo != null)
            {
                return InspectNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            InspectMaster another = obj as InspectMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.InspectNo == another.InspectNo);
            }
        } 
    }
	
}
