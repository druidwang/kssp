using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Entity.ISS
{
    [Serializable]
    public partial class IssueLog : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.ISS.IssueLog))]
		public Int32 Id { get; set; }
        [Display(Name = "IssueCode", ResourceType = typeof(Resources.ISS.IssueLog))]
		public string Issue { get; set; }
		//[Display(Name = "IssueDetail", ResourceType = typeof(Resources.ISS.IssueLog))]
		public Int32? IssueDetail { get; set; }
        [Display(Name = "Level", ResourceType = typeof(Resources.ISS.IssueLog))]
        public string Level { get; set; }
		[Display(Name = "Content", ResourceType = typeof(Resources.ISS.IssueLog))]
		public string Content { get; set; }
        [Display(Name = "User", ResourceType = typeof(Resources.ISS.IssueLog))]
        public User User { get; set; }
		[Display(Name = "EmailStatus", ResourceType = typeof(Resources.ISS.IssueLog))]
		public string EmailStatus { get; set; }
		[Display(Name = "SMSStatus", ResourceType = typeof(Resources.ISS.IssueLog))]
		public string SMSStatus { get; set; }
		[Display(Name = "Email", ResourceType = typeof(Resources.ISS.IssueLog))]
		public string Email { get; set; }
		[Display(Name = "MPhone", ResourceType = typeof(Resources.ISS.IssueLog))]
		public string MPhone { get; set; }
		[Display(Name = "IsEmail", ResourceType = typeof(Resources.ISS.IssueLog))]
		public Boolean? IsEmail { get; set; }
		[Display(Name = "IsSMS", ResourceType = typeof(Resources.ISS.IssueLog))]
		public Boolean? IsSMS { get; set; }

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
            IssueLog another = obj as IssueLog;

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
