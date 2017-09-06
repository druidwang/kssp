using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class ItemKit : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemKit_KitItem", ResourceType = typeof(Resources.MD.ItemKit))]
		public string KitItem { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemKit_ChildItem", ResourceType = typeof(Resources.MD.ItemKit))]
		public Item ChildItem { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemKit_Qty", ResourceType = typeof(Resources.MD.ItemKit))]
		public Decimal Qty { get; set; }
        [Display(Name = "ItemKit_IsActive", ResourceType = typeof(Resources.MD.ItemKit))]
		public Boolean IsActive { get; set; }
		public Int32 CreateUserId { get; set; }
        [Display(Name = "ItemKit_CreateUserName", ResourceType = typeof(Resources.MD.ItemKit))]
		public string CreateUserName { get; set; }
        [Display(Name = "ItemKit_CreateDate", ResourceType = typeof(Resources.MD.ItemKit))]
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
            ItemKit another = obj as ItemKit;

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
