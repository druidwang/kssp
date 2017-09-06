using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ISS
{
    [Serializable]
    public partial class IssueTypeToRoleDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        //[Display(Name = "Id", ResourceType = typeof(Resources.ISS.IssueTypeToRoleDetail))]
		public Int32 Id { get; set; }
        //[Display(Name = "IssueTypeTo", ResourceType = typeof(Resources.ISS.IssueTypeToRoleDetail))]
		public string IssueTypeTo { get; set; }
        //[Display(Name = "RoleId", ResourceType = typeof(Resources.ISS.IssueTypeToRoleDetail))]
		public Int32 RoleId { get; set; }
        //[Display(Name = "IsEmail", ResourceType = typeof(Resources.ISS.IssueTypeToRoleDetail))]
		public Boolean IsEmail { get; set; }
        //[Display(Name = "IsSMS", ResourceType = typeof(Resources.ISS.IssueTypeToRoleDetail))]
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
            IssueTypeToRoleDetail another = obj as IssueTypeToRoleDetail;

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
