using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class ItemProperty : EntityBase, IAuditable
    {
        public Int32 Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemProperty_RmItem", ResourceType = typeof(Resources.CUST.ItemProperty))]
        public string RmItem { get; set; }
         [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemProperty_Viscosity", ResourceType = typeof(Resources.CUST.ItemProperty))]
        public com.Sconit.CodeMaster.TimeUnit Viscosity { get; set; }

        [Display(Name = "ItemProperty_SfgItem", ResourceType = typeof(Resources.CUST.ItemProperty))]
        public string SfgItem { get; set; }
        [Display(Name = "ItemProperty_Flow", ResourceType = typeof(Resources.CUST.ItemProperty))]
        public string Flow { get; set; }
        public Int32 CreateUserId { get; set; }
         [Display(Name = "ItemProperty_CreateUserName", ResourceType = typeof(Resources.CUST.ItemProperty))]
        public string CreateUserName { get; set; }
         [Display(Name = "ItemProperty_CreateDate", ResourceType = typeof(Resources.CUST.ItemProperty))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
       
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
            ItemProperty another = obj as ItemProperty;

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
