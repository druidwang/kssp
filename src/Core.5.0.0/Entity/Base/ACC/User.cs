using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ACC
{
    [Serializable]
    public partial class User : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		public Int32 Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName  = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName  = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "User_Code", ResourceType = typeof(Resources.ACC.User))]
		public string Code { get; set; }

		public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName  = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "User_FirstName", ResourceType = typeof(Resources.ACC.User))]
		public string FirstName { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "User_LastName", ResourceType = typeof(Resources.ACC.User))]
		public string LastName { get; set; }

        [Display(Name = "User_Type", ResourceType = typeof(Resources.ACC.User))]
        public com.Sconit.CodeMaster.UserType Type { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "User_Email", ResourceType = typeof(Resources.ACC.User))]
		public string Email { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "User_TelPhone", ResourceType = typeof(Resources.ACC.User))]
		public string TelPhone { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "User_MobilePhone", ResourceType = typeof(Resources.ACC.User))]
		public string MobilePhone { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "User_Language", ResourceType = typeof(Resources.ACC.User))]
		public string Language { get; set; }

        [Display(Name = "User_IsActive", ResourceType = typeof(Resources.ACC.User))]
		public Boolean IsActive { get; set; }

        [Display(Name = "User_AccountLocked", ResourceType = typeof(Resources.ACC.User))]
        public Boolean AccountLocked { get; set; }

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

        [Display(Name = "User_LastPasswordModifyDate", ResourceType = typeof(Resources.ACC.User))]
        public DateTime LastPasswordModifyDate { get; set; }

        [Display(Name = "User_IsDomainUser", ResourceType = typeof(Resources.ACC.User))]
        public string DomainUser { get; set; }

        [Display(Name = "User_Depart", ResourceType = typeof(Resources.ACC.User))]
        public string Depart { get; set; }

        [Display(Name = "User_IpAddress", ResourceType = typeof(Resources.ACC.User))]
        public string IpAddress { get; set; }

        [Display(Name = "User_Position", ResourceType = typeof(Resources.ACC.User))]
        public string Position { get; set; }

        [Display(Name = "User_LastAccessDate", ResourceType = typeof(Resources.ACC.User))]
        public DateTime? LastAccessDate { get; set; }

        [Display(Name = "User_AccountExpired", ResourceType = typeof(Resources.ACC.User))]
        public DateTime AccountExpired { get; set; }
        
        [Display(Name = "User_PasswordExpired", ResourceType = typeof(Resources.ACC.User))]
        public DateTime PasswordExpired { get; set; }

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
            User another = obj as User;

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
