using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class OrderDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 20)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 10)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 10)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 10)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 10)]
        [Display(Name = "OrderDetail_OrderNo", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string OrderNo { get; set; }

        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }

        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 130)]
        //[Export(ExportName = "DistributionOrderDetail", ExportSeq = 130)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 120)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "OrderMaster_Type", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string OrderTypeDescription { get; set; }

        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 10)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 20)]
        //[Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 40)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 20)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 30)]
        [Display(Name = "OrderDetail_Sequence", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Int32 Sequence { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 40)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 30)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 30)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 30)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 20)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "OrderDetail_Item", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Item { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 60)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 50)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 60)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 50)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 50)]
        [Display(Name = "OrderDetail_ItemDescription", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ItemDescription { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 50)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 40)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 50)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 40)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 40)]
        [Display(Name = "OrderDetail_ReferenceItemCode", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ReferenceItemCode { get; set; }

        public string BaseUom { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 80)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 70)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 80)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 60)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 70)]
        [Display(Name = "OrderDetail_Uom", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Uom { get; set; }
        //[Export(ExportName = "OrderDetail", ExportSeq =125)]
        //[Export(ExportName = "ProcumentOrderDetail", ExportSeq = 140)]
        [Display(Name = "OrderDetail_PartyFrom", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string PartyFrom { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 70)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 60)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 60)]
        [Display(Name = "OrderDetail_UnitCount", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal UnitCount { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 90)]
        //[Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 90)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 70)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 80)]
        [Display(Name = "OrderDetail_UnitCountDescription", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string UnitCountDescription { get; set; }
        //[Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 70)]
        [Display(Name = "OrderDetail_MinUnitCount", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal MinUnitCount { get; set; }

        [Display(Name = "OrderDetail_QualityType", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }

        [Display(Name = "OrderDetail_ManufactureParty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ManufactureParty { get; set; }

        [Display(Name = "OrderDetail_RequiredQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal RequiredQty { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 120)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 100)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 130, ExportTitle = "OrderDetail_OrderedQtyReturn", ExportTitleResourceType = typeof(@Resources.ORD.OrderDetail))]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 90)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 110)]
        [Display(Name = "OrderDetail_OrderedQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal OrderedQty { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 130)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 110)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 120)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 110)]
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 100)]
        [Display(Name = "OrderDetail_ShippedQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal ShippedQty { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 140)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 120)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 120)] 
        [Display(Name = "OrderDetail_ReceivedQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal ReceivedQty { get; set; }

        [Display(Name = "OrderDetail_RejectedQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal RejectedQty { get; set; }

        [Display(Name = "OrderDetail_ScrapQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal ScrapQty { get; set; }
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 100)]
        [Display(Name = "OrderDetail_PickedQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal PickedQty { get; set; }

        [Display(Name = "OrderDetail_UnitQty", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal UnitQty { get; set; }

        [Display(Name = "OrderDetail_ReceiveLotSize", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal? ReceiveLotSize { get; set; }

        [Display(Name = "OrderDetail_LocationFrom", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string LocationFrom { get; set; }

        [Display(Name = "OrderDetail_LocationFromName", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string LocationFromName { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 110)]
        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 80)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 110)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 80)]
        //[Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 90)]
        [Display(Name = "OrderDetail_LocationTo", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string LocationTo { get; set; }

        [Display(Name = "OrderDetail_LocationToName", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string LocationToName { get; set; }

        [Display(Name = "OrderDetail_IsInspect", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Boolean IsInspect { get; set; }

        //[Display(Name = "OrderDetail_InspectLocation", ResourceType = typeof(Resources.ORD.OrderDetail))]
        //public string InspectLocation { get; set; }

        //[Display(Name = "OrderDetail_InspectLocationName", ResourceType = typeof(Resources.ORD.OrderDetail))]
        //public string InspectLocationName { get; set; }

        //[Display(Name = "OrderDetail_RejectLocation", ResourceType = typeof(Resources.ORD.OrderDetail))]
        //public string RejectLocation { get; set; }

        //[Display(Name = "OrderDetail_RejectLocationName", ResourceType = typeof(Resources.ORD.OrderDetail))]
        //public string RejectLocationName { get; set; }

        [Display(Name = "OrderDetail_BillAddress", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string BillAddress { get; set; }

        [Display(Name = "OrderDetail_BillAddressDescription", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string BillAddressDescription { get; set; }

        [Display(Name = "OrderDetail_PriceList", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string PriceList { get; set; }

        [Display(Name = "OrderDetail_UnitPrice", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Decimal? UnitPrice { get; set; }

        [Display(Name = "OrderDetail_IsProvisionalEstimate", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Boolean IsProvisionalEstimate { get; set; }

        [Display(Name = "OrderDetail_Tax", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Tax { get; set; }

        [Display(Name = "OrderDetail_IsIncludeTax", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Boolean IsIncludeTax { get; set; }

        [Display(Name = "OrderDetail_Bom", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Bom { get; set; }

        [Display(Name = "OrderDetail_Routing", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Routing { get; set; }

        //[Display(Name = "OrderDetail_ProductionScan", ResourceType = typeof(Resources.ORD.OrderDetail))]
        //public string ProductionScan { get; set; }

        //[Display(Name = "OrderDetail_HuLotSize", ResourceType = typeof(Resources.ORD.OrderDetail))]
        //public Decimal? HuLotSize { get; set; }

        [Display(Name = "OrderDetail_BillTerm", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public com.Sconit.CodeMaster.OrderBillTerm BillTerm { get; set; }

        public string ZOPWZ { get; set; }
        public string ZOPID { get; set; }
        public string ZOPDS { get; set; }

        public Int32 CreateUserId { get; set; }

        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 120)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 140)]
        [Display(Name = "OrderDetail_CreateUserName", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string CreateUserName { get; set; }

        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 130)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 150)]
        [Display(Name = "OrderDetail_CreateDate", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "OrderDetail_LastModifyUserName", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "OrderDetail_LastModifyDate", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public DateTime LastModifyDate { get; set; }

        [Display(Name = "OrderDetail_Version", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public Int32 Version { get; set; }
        [Display(Name = "OrderDetail_Container", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Container { get; set; }
        [Display(Name = "OrderDetail_ContainerDescription", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ContainerDescription { get; set; }

        public string Currency { get; set; }

        public string PickStrategy { get; set; }

        public string ExtraDemandSource { get; set; }

        public Boolean IsScanHu { get; set; }

        [Export(ExportName = "ProcumentOrderDetail", ExportSeq = 15)]
        [Export(ExportName = "DistributionOrderDetail", ExportSeq = 15)]
        [Display(Name = "OrderDetail_ExternalOrderNo", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ExternalOrderNo { get; set; }

        public string ExternalSequence { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        //
        public CodeMaster.ScheduleType ScheduleType { get; set; }

        public string ReserveNo { get; set; }

        public string ReserveLine { get; set; }

        public string BinTo { get; set; }
        [Export(ExportName = "OrderDetail", ExportSeq = 30)]
        [Export(ExportName = "ProcumentReturnOrderDetail", ExportSeq = 20)]
        [Display(Name = "OrderDetail_WMSSeq", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string WMSSeq { get; set; }

        public Boolean IsChangeUnitCount { get; set; }

        public string AUFNR { get; set; }
        public string ICHARG { get; set; }
        public string BWART { get; set; }


        [Display(Name = "OrderDetail_Direction", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string Direction { get; set; }
        [Export(ExportName = "DistributionReturnOrderDetail", ExportSeq = 140)]
        [Display(Name = "Hu_Remark", ResourceType = typeof(Resources.INV.Hu))]
        public string Remark { get; set; }

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
            OrderDetail another = obj as OrderDetail;

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
