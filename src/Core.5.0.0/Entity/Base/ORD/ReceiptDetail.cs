using System;
using com.Sconit.Entity.INV;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class ReceiptDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        
        public Int32 Id { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 20)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 20)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 20)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 20)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 10)]
        [Display(Name = "ReceiptDetail_ReceiptNo", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string ReceiptNo { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 10)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 10)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 10)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 10)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 20)]
        [Display(Name = "ReceiptDetail_Sequence", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public Int32 Sequence { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 40)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 30)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 30)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 30)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 30)]
        [Display(Name = "ReceiptDetail_OrderNo", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string OrderNo { get; set; }
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 130)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "OrderMaster_Type", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string OrderTypeDescription { get; set; }

        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        
        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }
        
        public Int32? OrderDetailId { get; set; }
        
        public Int32 OrderDetailSequence { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 30)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 40)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 40)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 40)]
         [Display(Name = "ReceiptDetail_IpNo", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string IpNo { get; set; }
        
        public Int32? IpDetailId { get; set; }

        public Int32 IpDetailSequence { get; set; }

        //[Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 120)]
        //[Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 120)]
        //[Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 110)]
        [Display(Name = "ReceiptDetail_IpDetailType", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public CodeMaster.IpDetailType IpDetailType { get; set; }

        public CodeMaster.IpGapAdjustOption IpGapAdjustOption { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 70)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 60)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 60)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 60)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 60)]
        [Display(Name = "ReceiptDetail_Item", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string Item { get; set; }

        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 70)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 70)]
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 80)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 70)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 70)]
        [Display(Name = "ReceiptDetail_ItemDescription", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string ItemDescription { get; set; }

        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 90)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 80)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 80)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 80)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 80)]
        [Display(Name = "ReceiptDetail_ReferenceItemCode", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string ReferenceItemCode { get; set; }

        public string BaseUom { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 110)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 100)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 100)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 100)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 100)]
        [Display(Name = "ReceiptDetail_Uom", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string Uom { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 100)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 90)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 90)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 90)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 90)]
        [Display(Name = "ReceiptDetail_UnitCount", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public Decimal UnitCount { get; set; }

        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 120)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 120)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 130)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 130)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 120)]
        [Display(Name = "ReceiptDetail_ReceivedQty", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public Decimal ReceivedQty { get; set; }
        //public Decimal RejectedQty { get; set; }
        public Decimal ScrapQty { get; set; }
		
        public Decimal UnitQty { get; set; }

        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 110)]
        [Display(Name = "ReceiptDetail_LocationFrom", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string LocationFrom { get; set; }

        public string LocationFromName { get; set; }

        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 60)]
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 55)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 110)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 110)]
        [Display(Name = "ReceiptDetail_LocationTo", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string LocationTo { get; set; }

        public string LocationToName { get; set; }

        public Boolean IsInspect { get; set; }

        public string BillAddress { get; set; }
        
        public string PriceList { get; set; }
        
        public Decimal? UnitPrice { get; set; }
        
        public string Currency { get; set; }
        
        public Boolean IsProvisionalEstimate { get; set; }
        
        public string Tax { get; set; }
        
        public Boolean IsIncludeTax { get; set; }
        
        public com.Sconit.CodeMaster.OrderBillTerm BillTerm { get; set; }

        public Int32 CreateUserId { get; set; }
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 135)]
        [Display(Name = "ReceiptMaster_CreateUserName", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string CreateUserName { get; set; }
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 140)]
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 140)]
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 130)]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 130)]
        [Display(Name = "ReceiptMaster_CreateDate", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32 Version { get; set; }
        [Display(Name = "ReceiptDetail_ManufactureParty", ResourceType = typeof(Resources.ORD.ReceiptDetail))]
        public string ManufactureParty { get; set; }
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 25)]
        [Display(Name = "ReceiptMaster_ExternalReceiptNo", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string ExternalOrderNo { get; set; }

        public string ExternalSequence { get; set; }
        [Export(ExportName = "DistributionReceiptMaster", ExportSeq = 50, ExportTitle = "ReceiptMaster_Flow", ExportTitleResourceType = typeof(Resources.ORD.ReceiptMaster))] 
        [Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 50, ExportTitle = "ReceiptMaster_Flow", ExportTitleResourceType = typeof(Resources.ORD.ReceiptMaster))] 
        [Export(ExportName = "DailsOfProcurementReceiptMaster", ExportSeq = 50, ExportTitle = "ReceiptMaster_Flow", ExportTitleResourceType = typeof(Resources.ORD.ReceiptMaster))] 
        [Export(ExportName = "DailsOfReceiptMaster", ExportSeq = 40, ExportTitle = "ReceiptMaster_Flow", ExportTitleResourceType = typeof(Resources.ORD.ReceiptMaster))]
        [Export(ExportName = "DailsOfSupplierReceiptMaster", ExportSeq = 50, ExportTitle = "ReceiptMaster_Flow", ExportTitleResourceType = typeof(Resources.ORD.ReceiptMaster))]
        public string Flow { get; set; }
        //[Export(ExportName = "DailsOfDistributionReceiptMaster", ExportSeq = 25)]
        //[Display(Name = "ReceiptMaster_ExternalReceiptNo", ResourceType = typeof(Resources.ORD.ReceiptMaster))]
        //public string ExternalReceiptNo { get; set; }
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
            ReceiptDetail another = obj as ReceiptDetail;

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
