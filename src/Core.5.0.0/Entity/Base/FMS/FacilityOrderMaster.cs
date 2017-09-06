using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class FacilityOrderMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "FacilityOrderMaster_FacilityOrderNo", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
        public string FacilityOrderNo { get; set; }
        [Display(Name = "FacilityOrderMaster_ReferenceNo", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
		public string ReferenceNo { get; set; }
        [Display(Name = "FacilityOrderMaster_Region", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
		public string Region { get; set; }
        [Display(Name = "FacilityOrderMaster_Status", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
        public com.Sconit.CodeMaster.FacilityOrderStatus Status { get; set; }
       

		public Int32 CreateUserId { get; set; }
        [Display(Name = "FacilityOrderMaster_CreateUserName", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
		public string CreateUserName { get; set; }
        [Display(Name = "FacilityOrderMaster_CreateDate", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
		public DateTime CreateDate { get; set; }
	
		public Int32 LastModifyUserId { get; set; }
	
        [Display(Name = "FacilityOrderMaster_LastModifyUserNm", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
        public string LastModifyUserName { get; set; }
        [Display(Name = "FacilityOrderMaster_LastModifyDate", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
		public DateTime LastModifyDate { get; set; }
		public Int32 Version { get; set; }
        [Display(Name = "FacilityOrderMaster_Type", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
        public com.Sconit.CodeMaster.FacilityOrderType Type { get; set; }
     
        #endregion

		public override int GetHashCode()
        {
            if (FacilityOrderNo != null)
            {
                return FacilityOrderNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            FacilityOrderMaster another = obj as FacilityOrderMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.FacilityOrderNo == another.FacilityOrderNo);
            }
        } 
    }
	
}
