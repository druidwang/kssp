using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class MiscOrderDetail : EntityBase,IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public Int32 Id { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 10)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 10)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 10)]
        [Export(ExportName = "Detail", ExportSeq = 10)]
        [Display(Name = "MiscOrderMstr_MiscOrderNo", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
		public string MiscOrderNo { get; set; }
        [Display(Name = "MiscOrderDetail_Sequence", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public Int32 Sequence { get; set; }
        public string WMSSeq { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 90)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 90)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 90)]
        [Export(ExportName = "Detail", ExportSeq = 70)]
        [Display(Name = "MiscOrderDetail_Item", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public string Item { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 100)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 100)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 100)]
        [Export(ExportName = "Detail", ExportSeq = 80)]
        [Display(Name = "MiscOrderDetail_ItemDescription", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public string ItemDescription { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 110)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 110)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 110)]
        [Export(ExportName = "Detail", ExportSeq = 90)]
        [Display(Name = "MiscOrderDetail_ReferenceItemCode", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public string ReferenceItemCode { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 130)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 130)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 130)]
        [Export(ExportName = "Detail", ExportSeq = 100)]
        [Display(Name = "MiscOrderDetail_Uom", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public string Uom { get; set; }
		//[Display(Name = "BaseUom", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public string BaseUom { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 120)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 120)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 120)]
        [Export(ExportName = "Detail", ExportSeq = 100)]
        [Display(Name = "MiscOrderDetail_UnitCount", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public Decimal UnitCount { get; set; }
        [Display(Name = "MiscOrderDetail_UnitQty", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public Decimal UnitQty { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 30)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 30)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 30)]
        [Export(ExportName = "Detail", ExportSeq = 30)]
        [Display(Name = "MiscOrderDetail_Location", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public string Location { get; set; }
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 70)]
       [Display(Name = "MiscOrderDetail_ReserveNo", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public string ReserveNo { get; set; }
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 80)]
       [Display(Name = "MiscOrderDetail_ReserveLine", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public string ReserveLine { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrderDet", ExportSeq = 140)]
        [Export(ExportName = "ProductionReworkOrderDet", ExportSeq = 140)]
        [Export(ExportName = "ProductionTrailMiscOrderDet", ExportSeq = 140)]
        [Export(ExportName = "Detail", ExportSeq = 120)]
       [Display(Name = "MiscOrderDetail_Qty", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public Decimal Qty { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public Int32 CreateUserId { get; set; }
        [Display(Name = "MiscOrderMstr_CreateUserName", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
		public string CreateUserName { get; set; }
        [Display(Name = "MiscOrderMstr_CreateDate", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
		public DateTime LastModifyDate { get; set; }

        [Display(Name = "MiscOrderDetail_ManufactureParty", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
        public string ManufactureParty { get; set; }

        [Display(Name = "MiscOrderDetail_EBELN", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
        public string EBELN { get; set; }
        [Display(Name = "MiscOrderDetail_EBELP", ResourceType = typeof(Resources.ORD.MiscOrderDetail))]
        public string EBELP { get; set; }
        [Display(Name = "StockTakeMaster_Remark", ResourceType = typeof(Resources.INV.StockTake))]
        public string Remark { get; set; }
        public Decimal WorkHour { get; set; }
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
            MiscOrderDetail another = obj as MiscOrderDetail;

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
