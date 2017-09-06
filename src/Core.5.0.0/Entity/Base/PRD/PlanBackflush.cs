using System;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Entity.PRD
{
    [Serializable]
    public partial class PlanBackflush : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Int32 Id { get; set; }
		//[Display(Name = "ProductLine", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public string ProductLine { get; set; }
		//[Display(Name = "ProductLineFacility", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public string ProductLineFacility { get; set; }
		//[Display(Name = "OrderNo", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public string OrderNo { get; set; }
		//[Display(Name = "OrderType", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
		//[Display(Name = "OrderSubType", ResourceType = typeof(Resources.PRD.PlanBackflush))]
        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }
		//[Display(Name = "OrderDetailSequence", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Int32 OrderDetailSequence { get; set; }
		//[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Int32 OrderDetailId { get; set; }
		//[Display(Name = "OrderBomDetailId", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Int32 OrderBomDetailId { get; set; }
        public Int32 OrderBomDetailSequence { get; set; }
        //[Display(Name = "ReceiptNo", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public string ReceiptNo { get; set; }
		//[Display(Name = "ReceiptDetailId", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Int32 ReceiptDetailId { get; set; }
        //[Display(Name = "ReceiptDetailSequence", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Int32 ReceiptDetailSequence { get; set; }
        public string Bom { get; set; }
        //[Display(Name = "Item", ResourceType = typeof(Resources.PRD.PlanBackflush))]
        public string FGItem { get; set; }
        public string Item { get; set; }
		//[Display(Name = "ItemDescription", ResourceType = typeof(Resources.PRD.PlanBackflush))]
        public string ItemDescription { get; set; }
		//[Display(Name = "ReferenceItemCode", ResourceType = typeof(Resources.PRD.PlanBackflush))]
        public string ReferenceItemCode { get; set; }
		//[Display(Name = "Uom", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public string Uom { get; set; }
		//[Display(Name = "BaseUom", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public string BaseUom { get; set; }
		//[Display(Name = "UnitCount", ResourceType = typeof(Resources.PRD.PlanBackflush))]
        //public Decimal UnitCount { get; set; }
		//[Display(Name = "UnitQty", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Decimal UnitQty { get; set; }
        public Decimal Qty { get; set; }
		//[Display(Name = "ManufactureParty", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public string ManufactureParty { get; set; }
		//[Display(Name = "IsClose", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Boolean IsClose { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public DateTime LastModifyDate { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.PRD.PlanBackflush))]
		public Int32 Version { get; set; }

        public string ReserveNo { get; set; }
        public string ReserveLine { get; set; }
        public string AUFNR { get; set; }
        public string ICHARG { get; set; }
        public string BWART { get; set; }
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
            PlanBackflush another = obj as PlanBackflush;

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
