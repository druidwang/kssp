using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class ItemPackage : EntityBase,IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemPackage_Id", ResourceType = typeof(Resources.MD.ItemPackage))]
		public Int32 Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemPackage_Item", ResourceType = typeof(Resources.MD.ItemPackage))]
		public string Item { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemPackage_UC", ResourceType = typeof(Resources.MD.ItemPackage))]
		public Decimal UnitCount { get; set; }
        [Display(Name = "ItemPackage_IsDefault", ResourceType = typeof(Resources.MD.ItemPackage))]
		public Boolean IsDefault { get; set; }
        [Display(Name = "ItemPackage_Description", ResourceType = typeof(Resources.MD.ItemPackage))]
        public string Description { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
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
            ItemPackage another = obj as ItemPackage;

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
