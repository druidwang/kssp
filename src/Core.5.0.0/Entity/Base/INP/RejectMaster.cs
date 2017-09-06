using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INP
{
    [Serializable]
    public partial class RejectMaster : EntityBase,IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "RejectMaster_RejectNo", ResourceType = typeof(Resources.INP.RejectMaster))]
		public string RejectNo { get; set; }
        public string Region { get; set; }
        [Display(Name = "RejectMaster_ReferenceNo", ResourceType = typeof(Resources.INP.RejectMaster))]
		public string ReferenceNo { get; set; }
        [Display(Name = "RejectMaster_Status", ResourceType = typeof(Resources.INP.RejectMaster))]
        public com.Sconit.CodeMaster.RejectStatus Status { get; set; }
        [Display(Name = "RejectMaster_IsPrint", ResourceType = typeof(Resources.INP.RejectMaster))]
		public Boolean IsPrint { get; set; }
        [Display(Name = "RejectMaster_HandleResult", ResourceType = typeof(Resources.INP.RejectMaster))]
        public com.Sconit.CodeMaster.HandleResult HandleResult { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.INP.RejectMaster))]
		public Int32 CreateUserId { get; set; }
        [Display(Name = "RejectMaster_CreateUserName", ResourceType = typeof(Resources.INP.RejectMaster))]
		public string CreateUserName { get; set; }
        [Display(Name = "RejectMaster_CreateDate", ResourceType = typeof(Resources.INP.RejectMaster))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INP.RejectMaster))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INP.RejectMaster))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.INP.RejectMaster))]
		public DateTime LastModifyDate { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.INP.RejectMaster))]
		public Int32 Version { get; set; }
        public com.Sconit.CodeMaster.InspectType InspectType { get; set; }

        #endregion

		public override int GetHashCode()
        {
			if (RejectNo != null)
            {
                return RejectNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            RejectMaster another = obj as RejectMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.RejectNo == another.RejectNo);
            }
        } 
    }
	
}
