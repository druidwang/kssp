using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INP
{
    [Serializable]
    public partial class ConcessionDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public Int32 Id { get; set; }
		//[Display(Name = "ConcessionNo", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string ConcessionNo { get; set; }
        [Display(Name = "ConcessionDetail_Sequence", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public Int32 Sequence { get; set; }
		[Display(Name = "ConcessionDetail_Item", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string Item { get; set; }
        [Display(Name = "ConcessionDetail_ItemDescription", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string ItemDescription { get; set; }
        [Display(Name = "ConcessionDetail_ReferenceItemCode", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string ReferenceItemCode { get; set; }
        [Display(Name = "ConcessionDetail_UnitCount", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public Decimal UnitCount { get; set; }
        [Display(Name = "ConcessionDetail_Uom", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string Uom { get; set; }
		//[Display(Name = "BaseUom", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string BaseUom { get; set; }
		//[Display(Name = "UnitQty", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public Decimal UnitQty { get; set; }
        [Display(Name = "ConcessionDetail_HuId", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string HuId { get; set; }
        [Display(Name = "ConcessionDetail_LotNo", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string LotNo { get; set; }
        [Display(Name = "ConcessionDetail_LocationFrom", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string LocationFrom { get; set; }
        [Display(Name = "ConcessionDetail_LocationTo", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string LocationTo { get; set; }
        [Display(Name = "ConcessionDetail_Qty", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public Decimal Qty { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.INP.ConcessionDetail))]
		public DateTime LastModifyDate { get; set; }

        public string WMSSeq { get; set; }
        
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
            ConcessionDetail another = obj as ConcessionDetail;

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
