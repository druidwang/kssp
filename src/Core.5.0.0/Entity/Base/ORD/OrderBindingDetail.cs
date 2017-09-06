using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class OrderBindingDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public Int32 Id { get; set; }
		//[Display(Name = "OrderBindingId", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public Int32 OrderBindingId { get; set; }
		//[Display(Name = "OrderNo", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public string OrderNo { get; set; }
		//[Display(Name = "BindOrderNo", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public string BindOrderNo { get; set; }
		//[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public Int32 OrderDetailId { get; set; }
		//[Display(Name = "BindOrderDetailId", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public Int32 BindOrderDetailId { get; set; }
        public Int32? OrderBomDetailId { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.ORD.OrderBindingDetail))]
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
            OrderBindingDetail another = obj as OrderBindingDetail;

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
