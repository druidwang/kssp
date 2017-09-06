using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Entity.ISS
{
    [Serializable]
    public partial class IssueDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public Int32 Id { get; set; }
		[Display(Name = "IssueCode", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public string IssueCode { get; set; }
		[Display(Name = "Sequence", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public Int32 Sequence { get; set; }
		[Display(Name = "IssueLevel", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public string IssueLevel { get; set; }
		[Display(Name = "IsSubmit", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public Boolean IsSubmit { get; set; }
		[Display(Name = "IsInProcess", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public Boolean IsInProcess { get; set; }
		[Display(Name = "IsDefault", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public Boolean IsDefault { get; set; }
        [Display(Name = "IsEmail", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public Boolean IsEmail { get; set; }
        [Display(Name = "IsSMS", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public Boolean IsSMS { get; set; }
		[Display(Name = "Priority", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public Int16 Priority { get; set; }
        //[Display(Name = "User", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public User User { get; set; }
		
		[Display(Name = "Email", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public string Email { get; set; }
		[Display(Name = "EmailStatus", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public com.Sconit.CodeMaster.SendStatus EmailStatus { get; set; }
		[Display(Name = "EmailCount", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public Int32 EmailCount { get; set; }
        [Display(Name = "MobilePhone", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public string MobilePhone { get; set; }
        [Display(Name = "SMSStatus", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public com.Sconit.CodeMaster.SendStatus SMSStatus { get; set; }
        [Display(Name = "SMSCount", ResourceType = typeof(Resources.ISS.IssueDetail))]
        public Int32 SMSCount { get; set; }
		[Display(Name = "IsActive", ResourceType = typeof(Resources.ISS.IssueDetail))]
		public Boolean IsActive { get; set; }
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

        public Int32? IssueTypeToUserDetailId { get; set; }
        public Int32? IssueTypeToRoleDetailId { get; set; }

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
            IssueDetail another = obj as IssueDetail;

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
