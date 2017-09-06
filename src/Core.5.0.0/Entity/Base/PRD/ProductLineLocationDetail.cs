using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.INV;

namespace com.Sconit.Entity.PRD
{
    [Serializable]
    public partial class ProductLineLocationDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Int32 Id { get; set; }
        [Display(Name = "ProductLineLocationDetail_ProductLine", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public string ProductLine { get; set; }
        [Display(Name = "ProductLineLocationDetail_ProductLineFacility", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public string ProductLineFacility { get; set; }
        [Display(Name = "ProductLineLocationDetail_OrderNo", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public string OrderNo { get; set; }
		//[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Int32? OrderDetailId { get; set; }
        [Display(Name = "ProductLineLocationDetail_Operation", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Int32? Operation { get; set; }
        [Display(Name = "ProductLineLocationDetail_OpReference", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public string OpReference { get; set; }
        [Display(Name = "ProductLineLocationDetail_TraceCode", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public string TraceCode { get; set; }
        [Display(Name = "ProductLineLocationDetail_Item", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public string Item { get; set; }
        [Display(Name = "ProductLineLocationDetail_ItemDescription", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public string ItemDescription { get; set; }
        [Display(Name = "ProductLineLocationDetail_ReferenceItemCode", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public string ReferenceItemCode { get; set; }
        [Display(Name = "ProductLineLocationDetail_HuId", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public string HuId { get; set; }
        [Display(Name = "ProductLineLocationDetail_LotNo", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public string LotNo { get; set; }
		//[Display(Name = "IsConsignment", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Boolean IsConsignment { get; set; }
		//[Display(Name = "PlanBill", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Int32? PlanBill { get; set; }
		//[Display(Name = "QualityType", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        [Display(Name = "ProductLineLocationDetail_Qty", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Decimal Qty { get; set; }
        [Display(Name = "ProductLineLocationDetail_BackFlushQty", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Decimal BackFlushQty { get; set; }
        [Display(Name = "ProductLineLocationDetail_VoidQty", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Decimal VoidQty { get; set; }
        [Display(Name = "ProductLineLocationDetail_IsClose", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public Boolean IsClose { get; set; }
         [Display(Name = "ProductLineLocationDetail_LocationFrom", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
        public string LocationFrom { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Int32 CreateUserId { get; set; }
        [Display(Name = "ProductLineLocationDetail_CreateUserName", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public string CreateUserName { get; set; }
        [Display(Name = "ProductLineLocationDetail_CreateDate", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public DateTime LastModifyDate { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.PRD.ProductLineLocationDetail))]
		public Int32 Version { get; set; }
        public string ReserveNo { get; set; }    //预留号
        public string ReserveLine { get; set; }  //预留行号
        public string AUFNR { get; set; }        //SAP生产单号
        public string ICHARG { get; set; }       //SAP批次号
        public string BWART { get; set; }        //移动类型
        public bool NotReport { get; set; }       //不导给SAP
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
            ProductLineLocationDetail another = obj as ProductLineLocationDetail;

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
