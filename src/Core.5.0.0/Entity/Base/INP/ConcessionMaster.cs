using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INP
{
    [Serializable]
    public partial class ConcessionMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "ConcessionMaster_ConcessionNo", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public string ConcessionNo { get; set; }
        [Display(Name = "ConcessionMaster_RejectNo", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public string RejectNo { get; set; }
        [Display(Name = "ConcessionMaster_ReferenceNo", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public string ReferenceNo { get; set; }
        [Display(Name = "ConcessionMaster_Status", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public com.Sconit.CodeMaster.ConcessionStatus Status { get; set; }
        [Display(Name = "ConcessionMaster_Region", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public string Region { get; set; }
		//[Display(Name = "IsPrint", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public Boolean IsPrint { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public Int32 CreateUserId { get; set; }
        [Display(Name = "ConcessionMaster_CreateUserName", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public string CreateUserName { get; set; }
        [Display(Name = "ConcessionMaster_CreateDate", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public DateTime LastModifyDate { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.INP.ConcessionMaster))]
		public Int32 Version { get; set; }

        public string WMSNo { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (ConcessionNo != null)
            {
                return ConcessionNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ConcessionMaster another = obj as ConcessionMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.ConcessionNo == another.ConcessionNo);
            }
        } 
    }
	
}
