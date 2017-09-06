using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.VIEW
{
    [Serializable]
    public partial class HuStatus : EntityBase
    {
        #region O/R Mapping Properties

        [Display(Name = "HuStatus_HuId", ResourceType = typeof(Resources.View.HuStatus))]
		public string HuId { get; set; }
        [Display(Name = "HuStatus_LotNo", ResourceType = typeof(Resources.View.HuStatus))]
		public string LotNo { get; set; }
        [Display(Name = "HuStatus_Item", ResourceType = typeof(Resources.View.HuStatus))]
        public string Item { get; set; }
        [Display(Name = "HuStatus_ItemDescription", ResourceType = typeof(Resources.View.HuStatus))]
        public string ItemDescription { get; set; }
        [Display(Name = "HuStatus_ReferenceItemCode", ResourceType = typeof(Resources.View.HuStatus))]
		public string ReferenceItemCode { get; set; }
        [Display(Name = "HuStatus_Uom", ResourceType = typeof(Resources.View.HuStatus))]
		public string Uom { get; set; }
		//[Display(Name = "BaseUom", ResourceType = typeof(Resources.View.HuStatus))]
		public string BaseUom { get; set; }
        [Display(Name = "HuStatus_UnitCount", ResourceType = typeof(Resources.View.HuStatus))]
		public Decimal UnitCount { get; set; }
        [Display(Name = "HuStatus_Qty", ResourceType = typeof(Resources.View.HuStatus))]
		public Decimal Qty { get; set; }
		//[Display(Name = "UnitQty", ResourceType = typeof(Resources.View.HuStatus))]
		public Decimal UnitQty { get; set; }
        [Display(Name = "HuStatus_ManufactureDate", ResourceType = typeof(Resources.View.HuStatus))]
		public DateTime ManufactureDate { get; set; }
        [Display(Name = "HuStatus_ManufactureParty", ResourceType = typeof(Resources.View.HuStatus))]
		public string ManufactureParty { get; set; }
		//[Display(Name = "ExpireDate", ResourceType = typeof(Resources.View.HuStatus))]
		public DateTime? ExpireDate { get; set; }
		//[Display(Name = "RemindExpireDate", ResourceType = typeof(Resources.View.HuStatus))]
		public DateTime? RemindExpireDate { get; set; }
		//[Display(Name = "FirstInventoryDate", ResourceType = typeof(Resources.View.HuStatus))]
		public DateTime? FirstInventoryDate { get; set; }
		//[Display(Name = "HuTemplate", ResourceType = typeof(Resources.View.HuStatus))]
		public string HuTemplate { get; set; }
		//[Display(Name = "PrintCount", ResourceType = typeof(Resources.View.HuStatus))]
		public Int16 PrintCount { get; set; }
        public Boolean IsOdd { get; set; }
        [Display(Name = "Hu_Status", ResourceType = typeof(Resources.View.HuStatus))]
        public com.Sconit.CodeMaster.HuStatus Status { get; set; }
        [Display(Name = "Region", ResourceType = typeof(Resources.View.HuStatus))]
        public string Region { get; set; }
        [Display(Name = "HuStatus_Location", ResourceType = typeof(Resources.View.HuStatus))]
		public string Location { get; set; }
		[Display(Name = "Bin", ResourceType = typeof(Resources.View.HuStatus))]
		public string Bin { get; set; }
        [Display(Name = "Hu_IpNo", ResourceType = typeof(Resources.INV.Hu))]
        public string IpNo { get; set; }
        [Display(Name = "HuStatus_LocationFrom", ResourceType = typeof(Resources.View.HuStatus))]
		public string LocationFrom { get; set; }
        [Display(Name = "HuStatus_LocationTo", ResourceType = typeof(Resources.View.HuStatus))]
		public string LocationTo { get; set; }
        [Display(Name = "IsConsignment", ResourceType = typeof(Resources.View.HuStatus))]
		public Boolean IsConsignment { get; set; }
		//[Display(Name = "PlanBill", ResourceType = typeof(Resources.View.HuStatus))]
		public Int32? PlanBill { get; set; }
        [Display(Name = "QualityType", ResourceType = typeof(Resources.View.HuStatus))]
		public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        [Display(Name = "IsFreeze", ResourceType = typeof(Resources.View.HuStatus))]
		public Boolean IsFreeze { get; set; }
        [Display(Name = "IsATP", ResourceType = typeof(Resources.View.HuStatus))]
		public Boolean IsATP { get; set; }
        [Display(Name = "OccupyType", ResourceType = typeof(Resources.View.HuStatus))]
        public com.Sconit.CodeMaster.OccupyType OccupyType { get; set; }
        [Display(Name = "OccupyReferenceNo", ResourceType = typeof(Resources.View.HuStatus))]
		public string OccupyReferenceNo { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.View.HuStatus))]
		public Int32 CreateUserId { get; set; }
        [Display(Name = "HuStatus_CreateUserName", ResourceType = typeof(Resources.View.HuStatus))]
		public string CreateUserName { get; set; }
        [Display(Name = "HuStatus_CreateDate", ResourceType = typeof(Resources.View.HuStatus))]
		public DateTime CreateDate { get; set; }
        public Int16 ConcessionCount { get; set; }
        public string OrderNo { get; set; }
        public string ReceiptNo { get; set; }
        public string SupplierLotNo { get; set; }
        public Boolean IsChangeUnitCount { get; set; }
        public string UnitCountDescription { get; set; }
        public CodeMaster.HuOption HuOption { get; set; }
        public string Direction { get; set; }

        public string PalletCode { get; set; }

        public Boolean IsExternal { get; set; }
        public Boolean IsPallet { get; set; }

        #endregion

		public override int GetHashCode()
        {
			if (HuId != null)
            {
                return HuId.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            HuStatus another = obj as HuStatus;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.HuId == another.HuId);
            }
        } 
    }
	
}
