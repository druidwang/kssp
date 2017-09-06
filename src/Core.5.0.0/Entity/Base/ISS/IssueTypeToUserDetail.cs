using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Entity.ISS
{
    [Serializable]
    public partial class IssueTypeToUserDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        //[Display(Name = "Id", ResourceType = typeof(Resources.ISS.IssueTypeToUserDetail))]
		public Int32 Id { get; set; }
        [Display(Name = "IssueTypeTo", ResourceType = typeof(Resources.ISS.IssueTypeToUserDetail))]
        public IssueTypeToMaster IssueTypeTo { get; set; }
        [Display(Name = "User", ResourceType = typeof(Resources.ISS.IssueTypeToUserDetail))]
        public User User { get; set; }
        [Display(Name = "IsEmail", ResourceType = typeof(Resources.ISS.IssueTypeToUserDetail))]
		public Boolean IsEmail { get; set; }
        [Display(Name = "IsSMS", ResourceType = typeof(Resources.ISS.IssueTypeToUserDetail))]
		public Boolean IsSMS { get; set; }
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
            IssueTypeToUserDetail another = obj as IssueTypeToUserDetail;

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
