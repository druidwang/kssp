using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ISS
{
    [Serializable]
    public partial class IssueAddress : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Code", ResourceType = typeof(Resources.ISS.IssueAddress))]
        public string Code { get; set; }
		[Display(Name = "Description", ResourceType = typeof(Resources.ISS.IssueAddress))]
		public string Description { get; set; }
        [Display(Name = "ParentIssueAddress", ResourceType = typeof(Resources.ISS.IssueAddress))]
        public IssueAddress ParentIssueAddress { get; set; }
		[Display(Name = "Sequence", ResourceType = typeof(Resources.ISS.IssueAddress))]
		public Int32 Sequence { get; set; }
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
            IssueAddress another = obj as IssueAddress;

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
