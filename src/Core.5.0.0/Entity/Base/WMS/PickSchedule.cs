using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class PickSchedule : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PickSchedule_PickScheduleNo", ResourceType = typeof(Resources.WMS.PickSchedule))]
		public string PickScheduleNo { get; set; }

        [Display(Name = "PickSchedule_PickLeadTime", ResourceType = typeof(Resources.WMS.PickSchedule))]
		public Decimal PickLeadTime { get; set; }

        [Display(Name = "PickSchedule_RepackLeadTime", ResourceType = typeof(Resources.WMS.PickSchedule))]
		public Decimal RepackLeadTime { get; set; }

        [Display(Name = "PickSchedule_SpreadLeadTime", ResourceType = typeof(Resources.WMS.PickSchedule))]
		public Decimal SpreadLeadTime { get; set; }

        [Display(Name = "PickSchedule_EmergentPickLeadTime", ResourceType = typeof(Resources.WMS.PickSchedule))]
        public Decimal EmergentPickLeadTime { get; set; }

        [Display(Name = "PickSchedule_EmergentRepackLeadTime", ResourceType = typeof(Resources.WMS.PickSchedule))]
        public Decimal EmergentRepackLeadTime { get; set; }

        [Display(Name = "PickSchedule_EmergentSpreadLeadTime", ResourceType = typeof(Resources.WMS.PickSchedule))]
        public Decimal EmergentSpreadLeadTime { get; set; }

		public Int32 CreateUserId { get; set; }

        [Display(Name = "PickSchedule_CreateUserName", ResourceType = typeof(Resources.WMS.PickSchedule))]
		public string CreateUserName { get; set; }

        [Display(Name = "PickSchedule_CreateDate", ResourceType = typeof(Resources.WMS.PickSchedule))]
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (PickScheduleNo != null)
            {
                return PickScheduleNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            PickSchedule another = obj as PickSchedule;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.PickScheduleNo == another.PickScheduleNo);
            }
        } 
    }
	
}
