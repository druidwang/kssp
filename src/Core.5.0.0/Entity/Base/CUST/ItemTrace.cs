using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class ItemTrace : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemTrace_Item", ResourceType = typeof(Resources.CUST.ItemTrace))]
		public string Item { get; set; }
        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.CUST.ItemTrace))]
		public Int32 CreateUserId { get; set; }
        //[Display(Name = "CreateUserName", ResourceType = typeof(Resources.CUST.ItemTrace))]
		public string CreateUserName { get; set; }
        //[Display(Name = "CreateDate", ResourceType = typeof(Resources.CUST.ItemTrace))]
		public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.CUST.ItemTrace))]
		public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.CUST.ItemTrace))]
		public string LastModifyUserName { get; set; }
        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.CUST.ItemTrace))]
		public DateTime LastModifyDate { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (Item != null)
            {
                return Item.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ItemTrace another = obj as ItemTrace;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Item == another.Item);
            }
        } 
    }
	
}
