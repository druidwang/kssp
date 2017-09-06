using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class WorkingCalendar : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Display(Name = "WorkingCalendar_Id", ResourceType = typeof(Resources.MD.WorkingCalendar))]
		public Int32 Id { get; set; }
        [Display(Name = "WorkingCalendar_Region", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string Region { get; set; }
        [Display(Name = "WorkingCalendar_Flow", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string Flow { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "WorkingCalendar_DayOfWeek", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public DayOfWeek DayOfWeek { get; set; }
        [Display(Name = "WorkingCalendar_Type", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public com.Sconit.CodeMaster.WorkingCalendarType Type { get; set; }
        [Display(Name = "WorkingCalendar_Remarks", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string Remarks { get; set; }
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
            WorkingCalendar another = obj as WorkingCalendar;

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
