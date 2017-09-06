using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class OrderDetailTrace : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
		public Int32 Id { get; set; }
		//[Display(Name = "OrderNo", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
		public string OrderNo { get; set; }
		//[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
		public Int32 OrderDetailId { get; set; }
        //[Display(Name = "OrderDetailTrace_TraceCode", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
		public string TraceCode { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.ORD.OrderDetailTrace))]
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
            OrderDetailTrace another = obj as OrderDetailTrace;

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
