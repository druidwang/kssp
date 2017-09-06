using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class GeneralLedger : EntityBase, IAuditable
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "GeneralLedger_Code", ResourceType = typeof(Resources.CUST.GeneralLedger))]
        public string Code { get; set; }
        [Display(Name = "GeneralLedger_Description", ResourceType = typeof(Resources.CUST.GeneralLedger))]
        public string Description { get; set; }
        public Int32 CreateUserId { get; set; }
        [Display(Name = "GeneralLedger_CreateUserName", ResourceType = typeof(Resources.CUST.GeneralLedger))]
        public string CreateUserName { get; set; }
        [Display(Name = "GeneralLedger_CreateDate", ResourceType = typeof(Resources.CUST.GeneralLedger))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

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
            GeneralLedger another = obj as GeneralLedger;

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
