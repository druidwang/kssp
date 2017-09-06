using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.BIL
{
    [Serializable]
    public partial class PlanBill : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "PlanBill_Id", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Int32 Id { get; set; }
        [Display(Name = "PlanBill_OrderNo", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string OrderNo { get; set; }
        [Display(Name = "PlanBill_IpNo", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string IpNo { get; set; }
		//[Display(Name = "ExternalIpNo", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string ExternalIpNo { get; set; }
        [Display(Name = "PlanBill_ReceiptNo", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string ReceiptNo { get; set; }
		//[Display(Name = "ExternalReceiptNo", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string ExternalReceiptNo { get; set; }
		//[Display(Name = "Type", ResourceType = typeof(Resources.BIL.PlanBill))]
        public com.Sconit.CodeMaster.BillType Type { get; set; }
        [Export(ExportName = "PlanBill", ExportSeq = 10)]
		[Display(Name = "PlanBill_Item", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string Item { get; set; }
        [Export(ExportName = "PlanBill", ExportSeq = 20)]
        [Display(Name = "PlanBill_ItemDescription", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string ItemDescription { get; set; }
        [Export(ExportName = "PlanBill", ExportSeq = 40)]
        [Display(Name = "PlanBill_Uom", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string Uom { get; set; }
		[Display(Name = "PlanBill_UnitCount", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Decimal UnitCount { get; set; }
		//[Display(Name = "BillTerm", ResourceType = typeof(Resources.BIL.PlanBill))]
		public com.Sconit.CodeMaster.OrderBillTerm BillTerm { get; set; }
		//[Display(Name = "BillAddress", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string BillAddress { get; set; }
		//[Display(Name = "BillAddressDescription", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string BillAddressDescription { get; set; }
        [Display(Name = "PlanBill_Party", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string Party { get; set; }
		//[Display(Name = "PartyName", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string PartyName { get; set; }
		//[Display(Name = "PriceList", ResourceType = typeof(Resources.BIL.PlanBill))]
        public string PriceList { get; set; }
        [Export(ExportName = "PlanBill", ExportSeq = 40)]
        [Display(Name = "PlanBill_Currency", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string Currency { get; set; }
		//[Display(Name = "UnitPrice", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Decimal UnitPrice { get; set; }
		//[Display(Name = "IsProvisionalEstimate", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Boolean IsProvisionalEstimate { get; set; }
		//[Display(Name = "Tax", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string Tax { get; set; }
		//[Display(Name = "IsIncludeTax", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Boolean IsIncludeTax { get; set; }
		//[Display(Name = "PlanAmount", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Decimal PlanAmount { get; set; }
		//[Display(Name = "ActingAmount", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Decimal ActingAmount { get; set; }
        public Decimal VoidAmount { get; set; }
        [Display(Name = "PlanBill_PlanQty", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Decimal PlanQty { get; set; }
        [Display(Name = "PlanBill_ActingQty", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Decimal ActingQty { get; set; }
        public Decimal VoidQty { get; set; }
        //[Display(Name = "UnitQty", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Decimal UnitQty { get; set; }
		//[Display(Name = "HuId", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string HuId { get; set; }
		//[Display(Name = "LocationFrom", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string LocationFrom { get; set; }
		//[Display(Name = "EffectiveDate", ResourceType = typeof(Resources.BIL.PlanBill))]
		public DateTime EffectiveDate { get; set; }
        public Boolean IsClose { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string CreateUserName { get; set; }
        [Display(Name = "PlanBill_CreateDate", ResourceType = typeof(Resources.BIL.PlanBill))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.BIL.PlanBill))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.BIL.PlanBill))]
		public DateTime LastModifyDate { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.BIL.PlanBill))]
		public Int32 Version { get; set; }

        public string Flow { get; set; }
        [Export(ExportName = "PlanBill", ExportSeq = 30)]
        [Display(Name = "PlanBill_ReferenceItemCode", ResourceType = typeof(Resources.BIL.PlanBill))]
        public string ReferenceItemCode { get; set; }
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
            PlanBill another = obj as PlanBill;

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
