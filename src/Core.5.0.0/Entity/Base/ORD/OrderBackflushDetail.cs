using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class OrderBackflushDetail : EntityBase
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Int32 Id { get; set; }
		//[Display(Name = "OrderNo", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string OrderNo { get; set; }
		//[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Int32 OrderDetailId { get; set; }
		//[Display(Name = "OrderDetailSeq", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
        public Int32 OrderDetailSequence { get; set; }
		//[Display(Name = "OrderBomDetailId", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Int32? OrderBomDetailId { get; set; }
		//[Display(Name = "OrderBomDetailSeq", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
        public Int32? OrderBomDetailSequence { get; set; }
		//[Display(Name = "ReceiptNo", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string ReceiptNo { get; set; }
		//[Display(Name = "ReceiptDetailId", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Int32 ReceiptDetailId { get; set; }
		//[Display(Name = "ReceiptDetailSeq", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
        public Int32 ReceiptDetailSequence { get; set; }
		//[Display(Name = "Bom", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string Bom { get; set; }
        [Display(Name = "OrderBackflushDetail_Item", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string Item { get; set; }
        [Display(Name = "OrderBackflushDetail_ItemDescription", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string ItemDescription { get; set; }
        [Display(Name = "OrderBackflushDetail_ReferenceItemCode", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string ReferenceItemCode { get; set; }
        [Display(Name = "OrderBackflushDetail_Uom", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string Uom { get; set; }
		//[Display(Name = "BaseUom", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string BaseUom { get; set; }
		//[Display(Name = "UnitQty", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Decimal UnitQty { get; set; }
		//[Display(Name = "ManufactureParty", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string ManufactureParty { get; set; }
		//[Display(Name = "TraceCode", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string TraceCode { get; set; }
		//[Display(Name = "HuId", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
        public string HuId { get; set; }
		//[Display(Name = "LotNo", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
        public string LotNo { get; set; }
		//[Display(Name = "Operation", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Int32? Operation { get; set; }
		//[Display(Name = "OpReference", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string OpReference { get; set; }
        [Display(Name = "OrderBackflushDetail_BackflushedQty", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Decimal BackflushedQty { get; set; }
		//[Display(Name = "BackflushedRejectQty", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Decimal BackflushedRejectQty { get; set; }
		//[Display(Name = "BackflushedScrapQty", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Decimal BackflushedScrapQty { get; set; }
		//[Display(Name = "LocationFrom", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string LocationFrom { get; set; }
		//[Display(Name = "ProductLine", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string ProductLine { get; set; }
		//[Display(Name = "ProductLineFacility", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string ProductLineFacility { get; set; }
		//[Display(Name = "EffectiveDate", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public DateTime EffectiveDate { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.ORD.OrderBackflushDetail))]
		public DateTime CreateDate { get; set; }

        public string ReserveNo { get; set; }
        public string ReserveLine { get; set; }
        public string AUFNR { get; set; }
        public Int32? PlanBill { get; set; }
        public string ICHARG { get; set; }
        public string BWART { get; set; }
        public Boolean NotReport { get; set; }
        public string FGItem { get; set; }
        public Boolean IsVoid { get; set; }
        public decimal ReceivedQty { get; set; }
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
            OrderBackflushDetail another = obj as OrderBackflushDetail;

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
