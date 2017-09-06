using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class LocationTransactionDetail : EntityBase
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public Int64 Id { get; set; }
        public Int64 LocationTransactionId { get; set; }
		//[Display(Name = "OrderNo", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string OrderNo { get; set; }
		//[Display(Name = "OrderType", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }
		//[Display(Name = "OrderDetailSequence", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public Int32 OrderDetailSequence { get; set; }
		//[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public Int32 OrderDetailId { get; set; }
		//[Display(Name = "OrderBomDetailId", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public Int32 OrderBomDetailId { get; set; }
        public Int32 OrderBomDetailSequence { get; set; }
        //[Display(Name = "IpNo", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string IpNo { get; set; }
		//[Display(Name = "IpDetailSequence", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        public Int32 IpDetailId { get; set; }
		public Int32 IpDetailSequence { get; set; }
		//[Display(Name = "ReceiptNo", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string ReceiptNo { get; set; }
		//[Display(Name = "ReceiptDetailSequence", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        public Int32 ReceiptDetailId { get; set; }
        public Int32 ReceiptDetailSequence { get; set; }
        public string SequenceNo { get; set; }
        public Int32 BillTransactionId { get; set; }
        public Int32 LocationLotDetailId { get; set; }
		//[Display(Name = "Item", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string Item { get; set; }
		//[Display(Name = "ItemDescription", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        //public string ItemDescription { get; set; }
		//[Display(Name = "Uom", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		//public string Uom { get; set; }
		//[Display(Name = "Qty", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public Decimal Qty { get; set; }
        public Boolean IsConsignment { get; set; }
        public Int32 PlanBill { get; set; }
        public Decimal PlanBillQty { get; set; }
        public Int32 ActingBill { get; set; }
        public Decimal ActingBillQty { get; set; }
		//[Display(Name = "QualityType", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
		//[Display(Name = "HuId", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string HuId { get; set; }
		//[Display(Name = "LotNo", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string LotNo { get; set; }
		//[Display(Name = "TransactionType", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        public com.Sconit.CodeMaster.TransactionType TransactionType { get; set; }
        public com.Sconit.CodeMaster.TransactionIOType IOType { get; set; }
		//[Display(Name = "PartyFrom", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string PartyFrom { get; set; }
		//[Display(Name = "PartyFromName", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        //public string PartyFromName { get; set; }
		//[Display(Name = "PartyTo", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string PartyTo { get; set; }
		//[Display(Name = "PartyToName", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        //public string PartyToName { get; set; }
		//[Display(Name = "LocationFrom", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string LocationFrom { get; set; }
		//[Display(Name = "LocationFromName", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        //public string LocationFromName { get; set; }
		//[Display(Name = "LocationTo", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public string LocationTo { get; set; }
        public string Bin { get; set; }
		//[Display(Name = "LocationToName", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        //public string LocationToName { get; set; }
		//[Display(Name = "LocationIOReason", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        public Int32 PlanBackflushId { get; set; }
        public string LocationIOReason { get; set; }
		//[Display(Name = "LocationIOReasonDesc", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        //public string LocationIOReasonDesc { get; set; }
		//[Display(Name = "EffectiveDate", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public DateTime EffectiveDate { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
        //public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.INV.LocationTransactionDetail))]
		public DateTime CreateDate { get; set; }

        public string TraceCode { get; set; }
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
            LocationTransactionDetail another = obj as LocationTransactionDetail;

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
