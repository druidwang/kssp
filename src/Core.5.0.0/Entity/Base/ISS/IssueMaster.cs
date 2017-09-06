using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Entity.ISS
{
    [Serializable]
    public partial class IssueMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Display(Name = "Code", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string Code { get; set; }
       // [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "IssueAddress", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string IssueAddress { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "IssueType", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public IssueType IssueType { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "IssueNo", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public IssueNo IssueNo { get; set; }
        [Display(Name = "IssueSubject", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string IssueSubject { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "BackYards", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string BackYards { get; set; }
        //[DataType(DataType.MultilineText)]
        [StringLength(100)]
        [Display(Name = "Content", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string Content { get; set; }
        [StringLength(100)]
        [Display(Name = "Solution", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string Solution { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Status", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public com.Sconit.CodeMaster.IssueStatus Status { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Priority", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public com.Sconit.CodeMaster.IssuePriority Priority { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Type", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public com.Sconit.CodeMaster.IssueType Type { get; set; }
        [Display(Name = "UserName", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "MobilePhone", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string MobilePhone { get; set; }
        [Display(Name = "FinishedDate", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public DateTime? FinishedDate { get; set; }
        [Display(Name = "FinishedUser", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public User FinishedUser { get; set; }
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

        [Display(Name = "ReleaseDate", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public DateTime? ReleaseDate { get; set; }
        //[Display(Name = "ReleaseUser", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public Int32? ReleaseUser { get; set; }
        [Display(Name = "ReleaseUserName", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string ReleaseUserName { get; set; }
        [Display(Name = "StartDate", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public DateTime? StartDate { get; set; }
        //[Display(Name = "StartUser", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public Int32? StartUser { get; set; }
        [Display(Name = "StartUserName", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string StartUserName { get; set; }
        [Display(Name = "CloseDate", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public DateTime? CloseDate { get; set; }
        //[Display(Name = "CloseUser", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public Int32? CloseUser { get; set; }
        [Display(Name = "CloseUserName", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string CloseUserName { get; set; }
        [Display(Name = "CancelDate", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public DateTime? CancelDate { get; set; }
        //[Display(Name = "CancelUser", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public Int32? CancelUser { get; set; }
        [Display(Name = "CancelUserName", ResourceType = typeof(Resources.ISS.IssueMaster))]
		public string CancelUserName { get; set; }
        [Display(Name = "CompleteDate", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public DateTime? CompleteDate { get; set; }
        //[Display(Name = "CompleteUser", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public Int32? CompleteUser { get; set; }
        [Display(Name = "CompleteUserName", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string CompleteUserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FailCode", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string FailCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "DefectCode", ResourceType = typeof(Resources.ISS.IssueMaster))]
        public string DefectCode { get; set; }
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
            IssueMaster another = obj as IssueMaster;

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
