using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.BIL
{
    [Serializable]
    public partial class BillTransaction : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public string OrderNo { get; set; }
		public string IpNo { get; set; }
		public string ExternalIpNo { get; set; }
		public string ReceiptNo { get; set; }
		public string ExternalReceiptNo { get; set; }
		public Boolean IsIncludeTax { get; set; }
		public string Item { get; set; }
		public string ItemDescription { get; set; }
		public string Uom { get; set; }
		public Decimal UnitCount { get; set; }
		//public string HuId { get; set; }
        public com.Sconit.CodeMaster.BillTransactionType TransactionType { get; set; }
		public string Party { get; set; }
		public string PartyName { get; set; }
		public string PriceList { get; set; }
		public string Currency { get; set; }
		public Decimal UnitPrice { get; set; }
		public Boolean IsProvisionalEstimate { get; set; }
		public string Tax { get; set; }
		public Decimal BillQty { get; set; }
		public Decimal BillAmount { get; set; }
		public string LocationFrom { get; set; }
		public string SettleLocation { get; set; }
		public DateTime EffectiveDate { get; set; }
        //public Int32 PlanBill { get; set; }
        public Int32 ActingBill { get; set; }
        public Int32 BillDetail { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
        public Decimal UnitQty { get; set; }
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
            BillTransaction another = obj as BillTransaction;

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
