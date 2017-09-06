using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class LocationLotDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public Int32 Id { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 10)]
        [Display(Name = "LocationLotDetail_Location", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public string Location { get; set; }
        //[Display(Name = "Bin", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        [Export(ExportName = "LocationLotDetail", ExportSeq = 20)]
        [Display(Name = "LocationLotDetail_Bin", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public string Bin { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 30)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "LocationLotDetail_Item", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public string Item { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 7, ExportTitle = "LocationLotDetail_ManufactureDate", ExportTitleResourceType = typeof(Resources.INV.LocationLotDetail))]
        [Display(Name = "LocationLotDetail_LotNo", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public string LotNo { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 5)]
        [Display(Name = "LocationLotDetail_HuId", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public string HuId { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 180)]
        [Display(Name = "LocationLotDetail_Qty", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Decimal Qty { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 90)]
        [Display(Name = "LocationLotDetail_IsConsignment", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Boolean IsConsignment { get; set; }
        //[Display(Name = "PlanBill", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Int32? PlanBill { get; set; }
        [Display(Name = "LocationLotDetail_QualityType", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 100)]
        [Display(Name = "LocationLotDetail_IsFreeze", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public Boolean IsFreeze { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 110)]
        [Display(Name = "LocationLotDetail_IsATP", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Boolean IsATP { get; set; }
        //[Display(Name = "OccupyType", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public com.Sconit.CodeMaster.OccupyType OccupyType { get; set; }
        //[Display(Name = "OccupyReferenceNo", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string OccupyReferenceNo { get; set; }
        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Int32 CreateUserId { get; set; }
        //[Display(Name = "CreateUserName", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public string CreateUserName { get; set; }
        [Display(Name = "LocationLotDetail_CreateDate", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public string LastModifyUserName { get; set; }
        [Display(Name = "LocationLotDetail_LastModifyDate", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public DateTime LastModifyDate { get; set; }
        //	[Display(Name = "Version", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Int32 Version { get; set; }

        public string Area { get; set; }
        public Int32 BinSequence { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 160)]
        [Display(Name = "LocationLotDetail_HuQty", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public Decimal HuQty { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 70)]
        [Display(Name = "LocationLotDetail_UnitCount", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public Decimal UnitCount { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 170)]
        [Display(Name = "LocationLotDetail_HuUom", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string HuUom { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 190)]
        [Display(Name = "LocationLotDetail_BaseUom", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string BaseUom { get; set; }
        public Decimal UnitQty { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 85)]
        [Display(Name = "LocationLotDetail_ManufactureParty", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string ManufactureParty { get; set; }
        //[Export(ExportName = "LocationLotDetail", ExportSeq = 130)]
        [Display(Name = "LocationLotDetail_ManufactureDate", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public DateTime ManufactureDate { get; set; }
        public DateTime FirstInventoryDate { get; set; }

        public string ConsigementParty { get; set; }
        [Export(ExportName = "LocationLotDetail", ExportSeq = 80)]
        [Display(Name = "LocationLotDetail_IsOdd", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public Boolean IsOdd { get; set; }
        [Display(Name = "LocationLotDetail_Direction", ResourceType = typeof(Resources.INV.LocationLotDetail))]
        public string Direction { get; set; }

        public string SupplierLotNo { get; set; }

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
            LocationLotDetail another = obj as LocationLotDetail;

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
