using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ISS
{
    [Serializable]
    public partial class IssueLevel : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
		[Display(Name = "Code", ResourceType = typeof(Resources.ISS.IssueLevel))]
		public string Code { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(100, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
		[Display(Name = "Description", ResourceType = typeof(Resources.ISS.IssueLevel))]
		public string Description { get; set; }
		[Display(Name = "IsActive", ResourceType = typeof(Resources.ISS.IssueLevel))]
		public Boolean IsActive { get; set; }
		[Display(Name = "IsSubmit", ResourceType = typeof(Resources.ISS.IssueLevel))]
		public Boolean IsSubmit { get; set; }
		[Display(Name = "IsInProcess", ResourceType = typeof(Resources.ISS.IssueLevel))]
		public Boolean IsInProcess { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Range(1, 999999)]
        [Display(Name = "Sequence", ResourceType = typeof(Resources.ISS.IssueLevel))]
		public Int32 Sequence { get; set; }
		[Display(Name = "IsDefault", ResourceType = typeof(Resources.ISS.IssueLevel))]
		public Boolean IsDefault { get; set; }
        public Int32 CreateUserId { get; set; }
        [Display(Name = "Common_CreateUserName", ResourceType = typeof(Resources.SYS.Global))]
        public string CreateUserName { get; set; }
        [Display(Name = "Common_CreateDate", ResourceType = typeof(Resources.SYS.Global))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "Common_LastModifyUserName", ResourceType = typeof(Resources.SYS.Global))]
        public string LastModifyUserName { get; set; }
        [Display(Name = "Common_LastModifyDate", ResourceType = typeof(Resources.SYS.Global))]
        public DateTime LastModifyDate { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            IssueLevel another = obj as IssueLevel;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Code == another.Code);
            }
        } 
    }
	
}
