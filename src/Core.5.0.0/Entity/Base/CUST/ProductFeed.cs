using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class ProductFeed : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		//[Display(Name = "Id", ResourceType = typeof(Resources.CUST.KitFeed))]
		public Int32 Id { get; set; }
		//[Display(Name = "ProductOrder", ResourceType = typeof(Resources.CUST.KitFeed))]
        public string TraceCode { get; set; }
        public string ProductOrder { get; set; }
		//[Display(Name = "KitOrder", ResourceType = typeof(Resources.CUST.KitFeed))]
		public string FeedOrder { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.CUST.KitFeed))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.CUST.KitFeed))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.CUST.KitFeed))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.CUST.KitFeed))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.CUST.KitFeed))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.CUST.KitFeed))]
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
            ProductFeed another = obj as ProductFeed;

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
