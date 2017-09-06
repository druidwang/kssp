using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class SpecialTime : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "SpecialTime_Region", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string Region { get; set; }
        [Display(Name = "SpecialTime_Flow", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string Flow { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "SpecialTime_StartTime", ResourceType = typeof(Resources.MD.WorkingCalendar))]
		public DateTime StartTime { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "SpecialTime_EndTime", ResourceType = typeof(Resources.MD.WorkingCalendar))]
		public DateTime EndTime { get; set; }
        [Display(Name = "SpecialTime_Description", ResourceType = typeof(Resources.MD.WorkingCalendar))]
		public string Description { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "SpecialTime_Type", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public com.Sconit.CodeMaster.WorkingCalendarType Type { get; set; }
        [Display(Name = "SpecialTime_Remarks", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string Remarks { get; set; }

        /// <summary>
        /// –›œ¢¿‡–Õ
        /// </summary>
        public CodeMaster.HolidayType HolidayType { get; set; }

		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (Id != 0)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            SpecialTime another = obj as SpecialTime;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Id == another.Id);
            }
        } 
    }
	
}
