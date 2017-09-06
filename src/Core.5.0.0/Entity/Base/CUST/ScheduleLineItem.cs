using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class ScheduleLineItem : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		public Int32 Id { get; set; }
        [Display(Name = "Party_Supplier_ShortCode", ResourceType = typeof(Resources.MD.Party))]
        public string Supplier { get; set; }
        [Display(Name = "Item_Code", ResourceType = typeof(Resources.MD.Item))]
        public string Item { get; set; }
        public Boolean IsClose { get; set; }

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
            ScheduleLineItem another = obj as ScheduleLineItem;

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
