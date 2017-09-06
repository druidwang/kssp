using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.PRD
{
    [Serializable]
    public partial class ShiftDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        public string Shift { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(256, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ShiftDetail_Time", ResourceType = typeof(Resources.MD.WorkingCalendar))]
		public string ShiftTime { get; set; }
        [Display(Name = "ShiftDetail_StartTime", ResourceType = typeof(Resources.MD.WorkingCalendar))]
		public DateTime? StartDate { get; set; }
        [Display(Name = "ShiftDetail_EndTime", ResourceType = typeof(Resources.MD.WorkingCalendar))]
		public DateTime? EndDate { get; set; }
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
            ShiftDetail another = obj as ShiftDetail;

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
