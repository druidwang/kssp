using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class StockTakeItem : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.INV.StockTake))]
		public Int32 Id { get; set; }
        //[Display(Name = "StNo", ResourceType = typeof(Resources.INV.StockTake))]
		public string StNo { get; set; }
        [Display(Name = "StockTakeItem_Item", ResourceType = typeof(Resources.INV.StockTakeItem))]
		public string Item { get; set; }
        [Display(Name = "StockTakeItem_ItemDescription", ResourceType = typeof(Resources.INV.StockTakeItem))]
        public string ItemDescription { get; set; }
        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.INV.StockTake))]
		public Int32 CreateUserId { get; set; }
        //[Display(Name = "CreateUserName", ResourceType = typeof(Resources.INV.StockTake))]
		public string CreateUserName { get; set; }
        //[Display(Name = "CreateDate", ResourceType = typeof(Resources.INV.StockTake))]
		public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INV.StockTake))]
		public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INV.StockTake))]
		public string LastModifyUserName { get; set; }
        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.INV.StockTake))]
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
            StockTakeItem another = obj as StockTakeItem;

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
