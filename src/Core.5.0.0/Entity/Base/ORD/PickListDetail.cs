using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class PickListDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		

		//[Display(Name = "Id", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public Int32 Id { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 20)]
		[Display(Name = "PickListDetail_PickListNo", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public string PickListNo { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 30)]
        [Display(Name = "PickListDetail_OrderNo", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string OrderNo { get; set; }

        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }

        public com.Sconit.CodeMaster.OrderSubType OrderSubType { get; set; }

		//[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public Int32 OrderDetailId { get; set; }

        public Int32 OrderDetailSequence { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 10)]
        [Display(Name = "PickListDetail_Sequence", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public Int32 Sequence { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 40)]
        [Display(Name = "PickListDetail_Item", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string Item { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq =60)]
        [Display(Name = "PickListDetail_ItemDescription", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public string ItemDescription { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 50)]
        [Display(Name = "PickListDetail_ReferenceItemCode", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string ReferenceItemCode { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 80)]
        [Display(Name = "PickListDetail_Uom", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public string Uom { get; set; }

        public string BaseUom { get; set; }

        public Decimal UnitQty { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 70)]
        [Display(Name = "PickListDetail_UnitCount", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public Decimal UnitCount { get; set; }

        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        [Display(Name = "PickListDetail_ManufactureParty", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string ManufactureParty { get; set; }
        //[Display(Name = "LocationFrom", ResourceType = typeof(Resources.ORD.PickListDetail))]
        //[Export(ExportName = "PickListDetail", ExportSeq = 130)]
        [Display(Name = "PickListDetail_LocationFrom", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string LocationFrom { get; set; }

        [Display(Name = "PickListDetail_LocationFromName", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public string LocationFromName { get; set; }

		//[Display(Name = "Area", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string Area { get; set; }

        [Display(Name = "PickListDetail_Bin", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public string Bin { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 90)]
        [Display(Name = "PickListDetail_LocationTo", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string LocationTo { get; set; }

        [Display(Name = "PickListDetail_LocationToName", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string LocationToName { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 110)]
        [Display(Name = "PickListDetail_Qty", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public Decimal Qty { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 120)]
		[Display(Name = "PickListDetail_PickedQty", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public Decimal PickedQty { get; set; }

        [Display(Name = "PickListDetail_HuId", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public string HuId { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 100)]
        [Display(Name = "PickListDetail_LotNo", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string LotNo { get; set; }

		//[Display(Name = "IsInspect", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public Boolean IsInspect { get; set; }

		//[Display(Name = "Memo", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string Memo { get; set; }

        public string PickStrategy { get; set; }

        public Boolean IsClose { get; set; }
        [Display(Name = "PickListDetail_IsOdd", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public Boolean IsOdd { get; set; }
        [Display(Name = "PickListDetail_IsDevan", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public Boolean IsDevan { get; set; }
        [Display(Name = "PickListDetail_IsInventory", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public Boolean IsInventory { get; set; }

        [Display(Name = "PickListDetail_IsMatchDirection", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public Boolean IsMatchDirection { get; set; }

        [Display(Name = "PickListDetail_Direction", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string Direction { get; set; }

        public decimal UcDeviation { get; set; }

        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public Int32 CreateUserId { get; set; }

		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string CreateUserName { get; set; }

		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public DateTime CreateDate { get; set; }

		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public Int32 LastModifyUserId { get; set; }

		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public string LastModifyUserName { get; set; }

		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public DateTime LastModifyDate { get; set; }

		//[Display(Name = "Version", ResourceType = typeof(Resources.ORD.PickListDetail))]
		public Int32 Version { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? WindowTime { get; set; }
		
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
            PickListDetail another = obj as PickListDetail;

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
