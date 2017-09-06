using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class FinanceCalendar : EntityBase, IAuditable
    {

        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FinanceCalendar_Id", ResourceType = typeof(Resources.MD.FinanceCalendar))]
		public Int32 Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [RegularExpression("[0-9]{4}", ErrorMessageResourceName = "Errors_FinanceCalendar_Year", ErrorMessageResourceType = typeof(Resources.MD.FinanceCalendar))]
        [Display(Name = "FinanceCalendar_FinanceYear", ResourceType = typeof(Resources.MD.FinanceCalendar))]
		public Int32 FinanceYear { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [RegularExpression("0([1-9]{1})|(1[1|2])|([1-9]{1})", ErrorMessageResourceName = "Errors_FinanceCalendar_Month", ErrorMessageResourceType = typeof(Resources.MD.FinanceCalendar))]
        [Display(Name = "FinanceCalendar_FinanceMonth", ResourceType = typeof(Resources.MD.FinanceCalendar))]
		public Int32 FinanceMonth { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FinanceCalendar_StartDate", ResourceType = typeof(Resources.MD.FinanceCalendar))]
		public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FinanceCalendar_EndDate", ResourceType = typeof(Resources.MD.FinanceCalendar))]
		public DateTime? EndDate { get; set; }
        [Display(Name = "FinanceCalendar_IsClose", ResourceType = typeof(Resources.MD.FinanceCalendar))]
		public Boolean IsClose { get; set; }
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
            FinanceCalendar another = obj as FinanceCalendar;

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
