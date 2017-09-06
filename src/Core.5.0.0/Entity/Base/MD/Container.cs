using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class Container : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Container_Code", ResourceType = typeof(Resources.MD.Container))]
		public string Code { get; set; }
        [Display(Name = "Container_Description", ResourceType = typeof(Resources.MD.Container))]
		public string Description { get; set; }
        [Display(Name = "Container_IsActive", ResourceType = typeof(Resources.MD.Container))]
		public Boolean IsActive { get; set; }

        [Display(Name = "Container_Qty", ResourceType = typeof(Resources.MD.Container))]
        public decimal Qty { get; set; }
        [Display(Name = "Container_InventoryType", ResourceType = typeof(Resources.MD.Container))]
        public com.Sconit.CodeMaster.InventoryType InventoryType { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.MD.Container))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.MD.Container))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.MD.Container))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.MD.Container))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.MD.Container))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.MD.Container))]
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
            Container another = obj as Container;

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
