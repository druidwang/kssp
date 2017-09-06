using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.INV;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class ReceiptLocationDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        //[Display(Name = "Id", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Int32 Id { get; set; }
        [Display(Name = "ReceiptLocationDetail_ReceiptNo", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public string ReceiptNo { get; set; }
        //[Display(Name = "ReceiptDetailId", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Int32 ReceiptDetailId { get; set; }
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        //[Display(Name = "OrderDetailId", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Int32? OrderDetailId { get; set; }
        [Display(Name = "ReceiptLocationDetail_Item", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public string Item { get; set; }
        [Display(Name = "ReceiptLocationDetail_HuId", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public string HuId { get; set; }
        [Display(Name = "ReceiptLocationDetail_LotNo", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public string LotNo { get; set; }
        //[Display(Name = "PlanBill", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Boolean IsCreatePlanBill { get; set; }
        [Display(Name = "ReceiptLocationDetail_IsConsignment", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Boolean IsConsignment { get; set; }
        public Int32? PlanBill { get; set; }
        //[Display(Name = "ActingBill", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Int32? ActingBill { get; set; }
        //[Display(Name = "QualityType", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        //[Display(Name = "IsFreeze", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Boolean IsFreeze { get; set; }
        //[Display(Name = "IsATP", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Boolean IsATP { get; set; }
        //[Display(Name = "OccupyType", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public com.Sconit.CodeMaster.OccupyType OccupyType { get; set; }
        //[Display(Name = "OccupyReferenceNo", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public string OccupyReferenceNo { get; set; }
        [Display(Name = "ReceiptLocationDetail_Qty", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Decimal Qty { get; set; }
        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Int32 CreateUserId { get; set; }
        //[Display(Name = "CreateUserName", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public string CreateUserName { get; set; }
        //[Display(Name = "CreateDate", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public string LastModifyUserName { get; set; }
        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public DateTime LastModifyDate { get; set; }
        //[Display(Name = "Version", ResourceType = typeof(Resources.ORD.ReceiptLocationDetail))]
        public Int32 Version { get; set; }
        public string WMSSeq { get; set; }
        public string ContainId { get; set; }
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
            ReceiptLocationDetail another = obj as ReceiptLocationDetail;

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
