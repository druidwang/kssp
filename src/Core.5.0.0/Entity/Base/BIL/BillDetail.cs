using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.BIL
{
    [Serializable]
    public partial class BillDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string BillNo { get; set; }
        public Int32 ActingBillId { get; set; }
        [Display(Name = "BillDetail_Item", ResourceType = typeof(Resources.BIL.BillDetail))]
        public string Item { get; set; }
        [Display(Name = "BillDetail_ItemDescription", ResourceType = typeof(Resources.BIL.BillDetail))]
        public string ItemDescription { get; set; }
        [Display(Name = "BillDetail_Uom", ResourceType = typeof(Resources.BIL.BillDetail))]
        public string Uom { get; set; }
        public Decimal UnitCount { get; set; }
        public Decimal Qty { get; set; }
        public string PriceList { get; set; }
        public Decimal Amount { get; set; }
        [Display(Name = "BillDetail_UnitPrice", ResourceType = typeof(Resources.BIL.BillDetail))]
        public Decimal UnitPrice { get; set; }
        public string OrderNo { get; set; }
        public string IpNo { get; set; }
        public string ExternalIpNo { get; set; }
        [Display(Name = "BillDetail_ReceiptNo", ResourceType = typeof(Resources.BIL.BillDetail))]
        public string ReceiptNo { get; set; }
        [Display(Name = "BillDetail_ExternalReceiptNo", ResourceType = typeof(Resources.BIL.BillDetail))]
        public string ExternalReceiptNo { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32 Version { get; set; }

        public string Flow { get; set; }
        [Display(Name = "BillDetail_Currency", ResourceType = typeof(Resources.BIL.BillDetail))]
        public string Currency { get; set; }
        public bool IsIncludeTax { get; set; }
        public string Tax { get; set; }
        [Display(Name = "BillDetail_ReferenceItemCode", ResourceType = typeof(Resources.BIL.BillDetail))]
        public string ReferenceItemCode { get; set; }
        public string Party { get; set; }
        [Display(Name = "BillDetail_PartyName", ResourceType = typeof(Resources.BIL.BillDetail))]
        public string PartyName { get; set; }
        public com.Sconit.CodeMaster.BillType Type { get; set; }
        public string LocationFrom { get; set; }
        public bool IsProvisionalEstimate { get; set; }
        [Display(Name = "BillDetail_EffectiveDate", ResourceType = typeof(Resources.BIL.BillDetail))]
        public DateTime EffectiveDate { get; set; }
        /// <summary>
        /// уш©ш
        /// </summary>
        [Display(Name = "BillDetail_Discount", ResourceType = typeof(Resources.BIL.BillDetail))]
        public Decimal Discount { get; set; }
        public Decimal UnitQty { get; set; }
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
            BillDetail another = obj as BillDetail;

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
