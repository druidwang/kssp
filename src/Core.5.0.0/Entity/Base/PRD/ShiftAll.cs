using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace com.Sconit.Entity.PRD
{
    [Serializable]
    public partial class ShiftAll : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ShiftMaster_Code", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string Code { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(100, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ShiftMaster_Name", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string Name { get; set; }
        public string Shift { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(256, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        //[RegularExpression("^(([1-9]{1})|([0-1][0-9])|([1-2][0-3])):([0-5][0-9])-(([1-9]{1})|([0-1][0-9])|([1-2][0-3])):([0-5][0-9])$|(([1-9]{1})|([0-1][0-9])|([1-2][0-3])):([0-5][0-9])-(([1-9]{1})|([0-1][0-9])|([1-2][0-3])):([0-5][0-9])$", ErrorMessageResourceName = "Errors_Form_ShiftTime", ErrorMessageResourceType = typeof(Resources.MD.WorkingCalendar))]
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
