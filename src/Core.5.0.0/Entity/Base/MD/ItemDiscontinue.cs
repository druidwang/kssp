using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class ItemDiscontinue : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public Int32 Id { get; set; }
        [Display(Name = "ItemDiscontinue_Item", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public string Item { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemDiscontinue_DisconItem", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public string DiscontinueItem { get; set; }
        [Display(Name = "ItemDiscontinue_Bom", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public string Bom { get; set; }
        [Display(Name = "ItemDiscontinue_UnitQty", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public Decimal UnitQty { get; set; }
        [Display(Name = "ItemDiscontinue_Priority", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public Int32 Priority { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "ItemDiscontinue_StartDate", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public DateTime StartDate { get; set; }

         [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "ItemDiscontinue_EndDate", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public DateTime? EndDate { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.MD.ItemDiscontinue))]
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
            ItemDiscontinue another = obj as ItemDiscontinue;

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
