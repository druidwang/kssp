using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class FailCode : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FailCode_Code", ResourceType = typeof(Resources.CUST.FailCode))]
        public string Code { get; set; }
        [Display(Name = "FailCode_CHNDescription", ResourceType = typeof(Resources.CUST.FailCode))]
        public string CHNDescription { get; set; }
        [Display(Name = "FailCode_ENGDescription", ResourceType = typeof(Resources.CUST.FailCode))]
        public string ENGDescription { get; set; }

        public CodeMaster.HandleResult HandleResult { get; set; }
        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.CUST.FailCode))]
        public Int32 CreateUserId { get; set; }
        //[Display(Name = "CreateUserName", ResourceType = typeof(Resources.CUST.FailCode))]
        public string CreateUserName { get; set; }
        //[Display(Name = "CreateDate", ResourceType = typeof(Resources.CUST.FailCode))]
        public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.CUST.FailCode))]
        public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.CUST.FailCode))]
        public string LastModifyUserName { get; set; }
        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.CUST.FailCode))]
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
            FailCode another = obj as FailCode;

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
