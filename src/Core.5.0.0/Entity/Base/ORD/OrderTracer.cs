using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class OrderTracer : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Display(Name = "Code", ResourceType = typeof(Resources.ORD.OrderTracer))]
        public string Code { get; set; }
        [Display(Name = "ReqTime", ResourceType = typeof(Resources.ORD.OrderTracer))]
        public DateTime? ReqTime { get; set; }
        [Display(Name = "Item", ResourceType = typeof(Resources.ORD.OrderTracer))]
        public string Item { get; set; }
        [Display(Name = "OrderedQty", ResourceType = typeof(Resources.ORD.OrderTracer))]
        public Decimal? OrderedQty { get; set; }
        [Display(Name = "FinishedQty", ResourceType = typeof(Resources.ORD.OrderTracer))]
        public Decimal? FinishedQty { get; set; }
        [Display(Name = "Qty", ResourceType = typeof(Resources.ORD.OrderTracer))]
        public Decimal? Qty { get; set; }
        [Display(Name = "RefId", ResourceType = typeof(Resources.ORD.OrderTracer))]
        public Int32? RefId { get; set; }
        [Display(Name = "OrderDetailId", ResourceType = typeof(Resources.ORD.OrderTracer))]
        public Int32 OrderDetailId { get; set; }
        
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
            OrderTracer another = obj as OrderTracer;

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
