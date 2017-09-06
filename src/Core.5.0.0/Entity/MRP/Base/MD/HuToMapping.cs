using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class HuToMapping : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        //HuTo	Party	Flow	Item
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "HuToMapping_HuTo", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string HuTo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "HuToMapping_Party", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string Party { get; set; }
        [Display(Name = "HuToMapping_Flow", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string Flow { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "HuToMapping_Item", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string Item { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "HuToMapping_Fg", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string Fg { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "HuToMapping_CreateUserName", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public string CreateUserName { get; set; }
        [Display(Name = "HuToMapping_CreateDate", ResourceType = typeof(Resources.MRP.HuToMapping))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
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
            HuToMapping another = obj as HuToMapping;

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
