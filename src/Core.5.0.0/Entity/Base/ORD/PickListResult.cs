using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.INV;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class PickListResult : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.ORD.PickListResult))]
		public Int32 Id { get; set; }
        [Display(Name = "PickListResult_PickListNo", ResourceType = typeof(Resources.ORD.PickListResult))]
		public string PickListNo { get; set; }
		//[Display(Name = "PickListDetailId", ResourceType = typeof(Resources.ORD.PickListResult))]
		public Int32 PickListDetailId { get; set; }
        public Int32 OrderDetailId { get; set; }
        [Display(Name = "PickListResult_Item", ResourceType = typeof(Resources.ORD.PickListResult))]
		public string Item { get; set; }
        [Display(Name = "PickListResult_ItemDescription", ResourceType = typeof(Resources.ORD.PickListResult))]
		public string ItemDescription { get; set; }
		//[Display(Name = "ReferenceItemCode", ResourceType = typeof(Resources.ORD.PickListResult))]
		public string ReferenceItemCode { get; set; }
        [Display(Name = "PickListResult_Uom", ResourceType = typeof(Resources.ORD.PickListResult))]
		public string Uom { get; set; }
        public string BaseUom { get; set; }
        public Decimal UnitQty { get; set; }
        [Display(Name = "PickListResult_UnitCount", ResourceType = typeof(Resources.ORD.PickListResult))]
		public Decimal UnitCount { get; set; }
        [Display(Name = "PickListResult_HuId", ResourceType = typeof(Resources.ORD.PickListResult))]
		public string HuId { get; set; }
        [Display(Name = "PickListResult_LotNo", ResourceType = typeof(Resources.ORD.PickListResult))]
		public string LotNo { get; set; }
		//[Display(Name = "IsConsignment", ResourceType = typeof(Resources.ORD.PickListResult))]
		public Boolean IsConsignment { get; set; }
		//[Display(Name = "PlanBill", ResourceType = typeof(Resources.ORD.PickListResult))]
		public Int32? PlanBill { get; set; }
        //[Display(Name = "QualityType", ResourceType = typeof(Resources.ORD.PickListResult))]
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public Boolean IsFreeze { get; set; }
        public Boolean IsATP { get; set; }
        [Display(Name = "PickListResult_Qty", ResourceType = typeof(Resources.ORD.PickListResult))]
		public Decimal Qty { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.PickListResult))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.ORD.PickListResult))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.ORD.PickListResult))]
		public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
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
            PickListResult another = obj as PickListResult;

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
