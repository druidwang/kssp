using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class SequenceDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Int32 Id { get; set; }
        [Display(Name = "SequenceDetail_SequenceNo", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string SequenceNo { get; set; }
        [Display(Name = "SequenceDetail_OrderNo", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string OrderNo { get; set; }
        [Display(Name = "SequenceDetail_TraceCode", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string TraceCode { get; set; }
		//[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Int32 OrderDetailId { get; set; }
		//[Display(Name = "OrderDetailSeq", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Int32 OrderDetailSequence { get; set; }
        [Display(Name = "SequenceDetail_Sequence", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Int64 Sequence { get; set; }
        [Display(Name = "SequenceDetail_Item", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string Item { get; set; }
        [Display(Name = "SequenceDetail_ItemDescription", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string ItemDescription { get; set; }
        [Display(Name = "SequenceDetail_ReferenceItemCode", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string ReferenceItemCode { get; set; }
        [Display(Name = "SequenceDetail_Uom", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string Uom { get; set; }
		//[Display(Name = "UnitQty", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Decimal UnitQty { get; set; }
		//[Display(Name = "BaseUom", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string BaseUom { get; set; }
        [Display(Name = "SequenceDetail_UnitCount", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Decimal UnitCount { get; set; }
		//[Display(Name = "QualityType", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public CodeMaster.QualityType QualityType { get; set; }
        [Display(Name = "SequenceDetail_ManufactureParty", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string ManufactureParty { get; set; }
        [Display(Name = "SequenceDetail_Qty", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Decimal Qty { get; set; }
        [Display(Name = "SequenceDetail_IsClose", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Boolean IsClose { get; set; }
        [Display(Name = "SequenceDetail_HuId", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string HuId { get; set; }
        [Display(Name = "SequenceDetail_LotNo", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string LotNo { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public DateTime LastModifyDate { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.ORD.SequenceDetail))]
		public Int32 Version { get; set; }
        [Display(Name = "SequenceDetail_StartTime", ResourceType = typeof(Resources.ORD.SequenceDetail))]
        public DateTime? StartTime { get; set; }
        [Display(Name = "SequenceDetail_WindowTime", ResourceType = typeof(Resources.ORD.SequenceDetail))]
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
            SequenceDetail another = obj as SequenceDetail;

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
