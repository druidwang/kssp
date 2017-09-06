using System;
using com.Sconit.Entity.SCM;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class OrderBinding : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
        [Display(Name = "OrderBinding_OrderNo", ResourceType = typeof(Resources.ORD.OrderBinding))]
		public string OrderNo { get; set; }
        [Display(Name = "OrderBinding_BindFlow", ResourceType = typeof(Resources.ORD.OrderBinding))]
		public string BindFlow { get; set; }
        [Display(Name = "OrderBinding_BindFlowStrategy", ResourceType = typeof(Resources.ORD.OrderBinding))]
        public com.Sconit.CodeMaster.FlowStrategy BindFlowStrategy { get; set; }
        [Display(Name = "OrderBinding_BindOrderNo", ResourceType = typeof(Resources.ORD.OrderBinding))]
		public string BindOrderNo { get; set; }
        //public Int32 BindOrderDetailId { get; set; }
        [Display(Name = "OrderBinding_BindType", ResourceType = typeof(Resources.ORD.OrderBinding))]
        public com.Sconit.CodeMaster.BindType BindType { get; set; }
        public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
		public Int32 Version { get; set; }
        
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
            OrderBinding another = obj as OrderBinding;

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
