using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.INV;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class IpLocationDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

       // [Display(Name = "IpLocationDetail_Id", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public Int32 Id { get; set; }
        [Display(Name = "IpLocationDetail_IpNo", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public string IpNo { get; set; }
		//[Display(Name = "IpDetailId", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public Int32 IpDetailId { get; set; }
		//[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        public Int32? OrderDetailId { get; set; }
        [Display(Name = "IpLocationDetail_Item", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public string Item { get; set; }
        [Display(Name = "IpLocationDetail_HuId", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public string HuId { get; set; }
        [Display(Name = "IpLocationDetail_LotNo", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public string LotNo { get; set; }
		//[Display(Name = "IsConsignment", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
        public Boolean IsCreatePlanBill { get; set; }
        [Display(Name = "LocationLotDetail_IsConsignment", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Boolean IsConsignment { get; set; }
		//[Display(Name = "PlanBillId", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public Int32? PlanBill { get; set; }
        public Int32? ActingBill { get; set; }
		//[Display(Name = "QualityType", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
		//[Display(Name = "IsFreeze", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
        [Display(Name = "LocationLotDetail_IsFreeze", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Boolean IsFreeze { get; set; }
        [Display(Name = "LocationLotDetail_IsATP", ResourceType = typeof(Resources.INV.LocationLotDetail))]
		public Boolean IsATP { get; set; }
        public com.Sconit.CodeMaster.OccupyType OccupyType { get; set; }
        public string OccupyReferenceNo { get; set; }
        [Display(Name = "IpLocationDetail_Qty", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public Decimal Qty { get; set; }
        [Display(Name = "IpLocationDetail_IsClose", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
        public Boolean IsClose { get; set; }
        [Display(Name = "IpLocationDetail_ReceivedQty", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public Decimal ReceivedQty { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public Int32 CreateUserId { get; set; }
		//[Display(Name = "CreateUserName", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public string CreateUserName { get; set; }
		//[Display(Name = "CreateDate", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public DateTime CreateDate { get; set; }
		//[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public Int32 LastModifyUserId { get; set; }
		//[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public string LastModifyUserName { get; set; }
		//[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public DateTime LastModifyDate { get; set; }
		//[Display(Name = "Version", ResourceType = typeof(Resources.ORD.IpLocationDetail))]
		public Int32 Version { get; set; }
        public string WMSSeq { get; set; }

        public Boolean IsMatchHu { get; set; }
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
            IpLocationDetail another = obj as IpLocationDetail;

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
