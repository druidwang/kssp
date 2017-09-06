using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class IpDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 150)]
        [Display(Name = "IpDetail_IpDetailType", ResourceType = typeof(Resources.ORD.IpDetail))]
        public com.Sconit.CodeMaster.IpDetailType Type { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 20)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 10)]
        [Display(Name = "IpDetail_IpNo", ResourceType = typeof(Resources.ORD.IpDetail))]
		public string IpNo { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 10)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 50)]
        [Display(Name = "IpDetail_Sequence", ResourceType = typeof(Resources.ORD.IpDetail))]
        public Int32 Sequence { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 30)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 20)]
        [Display(Name = "IpDetail_OrderNo", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string OrderNo { get; set; }
        
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        
        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }
        
        public Int32? OrderDetailId { get; set; }
        
        public Int32 OrderDetailSequence { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 50)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 40)]
        [Display(Name = "IpDetail_Item", ResourceType = typeof(Resources.ORD.IpDetail))]
		public string Item { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 70)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 70)]
        [Display(Name = "IpDetail_ItemDescription", ResourceType = typeof(Resources.ORD.IpDetail))]
		public string ItemDescription { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 60)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 60)]
        [Display(Name = "IpDetail_RefItemCode", ResourceType = typeof(Resources.ORD.IpDetail))]
		public string ReferenceItemCode { get; set; }
        
        public string BaseUom { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 90)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 90)]
        [Display(Name = "IpDetail_Uom", ResourceType = typeof(Resources.ORD.IpDetail))]
		public string Uom { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 80)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 80)]
        [Display(Name = "IpDetail_UnitCount", ResourceType = typeof(Resources.ORD.IpDetail))]
		public Decimal UnitCount { get; set; }
         [Display(Name = "IpDetail_UnitCountDescription", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string UnitCountDescription { get; set; }
        [Display(Name = "IpDetail_Container", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string Container { get; set; }
        [Display(Name = "IpDetail_ContainerDescription", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string ContainerDescription { get; set; }

        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
       [Display(Name = "IpDetail_ManufactureParty", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string ManufactureParty { get; set; }

        //public string HuId { get; set; }
        //public string LotNo { get; set; }
        //public Boolean IsConsignment { get; set; }
        //public Int32? PlanBillId { get; set; }
        //public Boolean IsFreeze { get; set; }
        //public Boolean IsATP { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 130)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 110)]
        [Display(Name = "IpDetail_Qty", ResourceType = typeof(Resources.ORD.IpDetail))]
		public Decimal Qty { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 140)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 120)]
        [Display(Name = "IpDetail_ReceivedQty", ResourceType = typeof(Resources.ORD.IpDetail))]
		public Decimal ReceivedQty { get; set; }

		public Decimal UnitQty { get; set; }

        public string LocationFrom { get; set; }
        
        [Display(Name = "IpDetail_LocationFromName", ResourceType = typeof(Resources.ORD.IpDetail))]
		public string LocationFromName { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 100)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 100)]
         [Display(Name = "IpDetail_LocationTo", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string LocationTo { get; set; }

        [Display(Name = "IpDetail_LocationToName", ResourceType = typeof(Resources.ORD.IpDetail))]
		public string LocationToName { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 160)]
        [Display(Name = "IpDetail_IsInspect", ResourceType = typeof(Resources.ORD.IpDetail))]
        public Boolean IsInspect { get; set; }

        //public string InspectLocation { get; set; }
        //public string InspectLocationName { get; set; }
        //public string RejectLocation { get; set; }
        //public string RejectLocationName { get; set; }

		public string BillAddress { get; set; }

		public string PriceList { get; set; }
         [Display(Name = "IpDetail_UnitPrice", ResourceType = typeof(Resources.ORD.IpDetail))]
		public Decimal? UnitPrice { get; set; }

		public string Currency { get; set; }
         [Display(Name = "IpDetail_IsProvisionalEstimate", ResourceType = typeof(Resources.ORD.IpDetail))]
        public Boolean IsProvisionalEstimate { get; set; }

		public string Tax { get; set; }

		public Boolean IsIncludeTax { get; set; }

        public com.Sconit.CodeMaster.OrderBillTerm BillTerm { get; set; }

        //public DateTime? EffectiveDate { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 180)]
        [Display(Name = "IpDetail_IsClose", ResourceType = typeof(Resources.ORD.IpDetail))]
        public Boolean IsClose { get; set; }

        public string GapReceiptNo { get; set; }

        public Int32? GapIpDetailId { get; set; }

		public Int32 CreateUserId { get; set; }
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 170)]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 150)]
        [Display(Name = "IpDetail_CreateUserNm", ResourceType = typeof(Resources.ORD.IpDetail))]
        public string CreateUserName { get; set; }
        [Display(Name = "IpMaster_CreateDate", ResourceType = typeof(Resources.ORD.IpMaster))]
        [Export(ExportName = "DistributionIpDetail", ExportSeq = 145)]
        public DateTime CreateDate { get; set; }

		public Int32 LastModifyUserId { get; set; }

		public string LastModifyUserName { get; set; }

		public DateTime LastModifyDate { get; set; }

		public Int32 Version { get; set; }

        public string ExternalOrderNo { get; set; }

        public string ExternalSequence { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? WindowTime { get; set; }

        public string BinTo { get; set; }

        public Boolean IsScanHu { get; set; }

        public Boolean IsChangeUnitCount { get; set; }
        [Export(ExportName = "DistributionIpDetail", ExportSeq =30, ExportTitle = "IpMaster_Flow", ExportTitleResourceType = typeof(Resources.ORD.IpMaster))] 
        [Export(ExportName = "ProcumentIpDetail", ExportSeq = 40, ExportTitle = "IpMaster_Flow", ExportTitleResourceType = typeof(Resources.ORD.IpMaster))] 
        public string Flow { get; set; }

        public string PalletCode { get; set; }
        public Decimal PalletLotSize { get; set; }
        public Decimal PackageVolume { get; set; }
        public Decimal PackageWeight { get; set; }
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
            IpDetail another = obj as IpDetail;

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
