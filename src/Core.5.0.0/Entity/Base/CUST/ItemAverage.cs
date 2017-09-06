using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class ItemAverage : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Item", ResourceType = typeof(Resources.CUST.ItemAverage))]
		public string Item { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.CUST.ItemAverage))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.CUST.ItemAverage))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.CUST.ItemAverage))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.CUST.ItemAverage))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.CUST.ItemAverage))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.CUST.ItemAverage))]
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
            ItemAverage another = obj as ItemAverage;

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
