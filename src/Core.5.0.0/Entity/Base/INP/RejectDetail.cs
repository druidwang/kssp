using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INP
{
    [Serializable]
    public partial class RejectDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "RejectDetail_Id", ResourceType = typeof(Resources.INP.RejectDetail))]
        public Int32 Id { get; set; }
        [Display(Name = "RejectDetail_InspResultId", ResourceType = typeof(Resources.INP.RejectDetail))]
        public Int32 InspectResultId { get; set; }
        [Display(Name = "RejectDetail_InpNo", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string InspectNo { get; set; }
        [Display(Name = "RejectDetail_RejNo", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string RejectNo { get; set; }
        public Int32 Sequence { get; set; }
        [Display(Name = "RejectDetail_Item", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string Item { get; set; }
        [Display(Name = "RejectDetail_ItemDesc", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string ItemDescription { get; set; }
        [Display(Name = "RejectDetail_RefItemCode", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string ReferenceItemCode { get; set; }
        [Display(Name = "RejectDetail_Uom", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string Uom { get; set; }
        [Display(Name = "RejectDetail_UC", ResourceType = typeof(Resources.INP.RejectDetail))]
        public Decimal UnitCount { get; set; }
        public string BaseUom { get; set; }
        public Decimal UnitQty { get; set; }
        [Display(Name = "RejectDetail_HuId", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string HuId { get; set; }
        [Display(Name = "RejectDetail_LotNo", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string LotNo { get; set; }
        [Display(Name = "RejectDetail_LocationFrom", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string LocationFrom { get; set; }
        [Display(Name = "RejectDetail_CurrentLocation", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string CurrentLocation { get; set; }
        [Display(Name = "RejectDetail_HandleQty", ResourceType = typeof(Resources.INP.RejectDetail))]
        public Decimal HandleQty { get; set; }
        [Display(Name = "RejectDetail_HandledQty", ResourceType = typeof(Resources.INP.RejectDetail))]
        public Decimal HandledQty { get; set; }
        [Display(Name = "RejectDetail_Defect", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string Defect { get; set; }
        [Display(Name = "RejectDetail_FailCode", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string FailCode { get; set; }
        //[Display(Name = "JudgeUserId", ResourceType = typeof(Resources.INP.RejectDetail))]
        public Int32 JudgeUserId { get; set; }
        [Display(Name = "RejectDetail_JudgeUserName", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string JudgeUserName { get; set; }
        [Display(Name = "RejectDetail_JudgeDate", ResourceType = typeof(Resources.INP.RejectDetail))]
        public DateTime JudgeDate { get; set; }
        public Int32 CreateUserId { get; set; }
        [Display(Name = "RejectDetail_CreateUserName", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INP.RejectDetail))]
        public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string LastModifyUserName { get; set; }
        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.INP.RejectDetail))]
        public DateTime LastModifyDate { get; set; }
        //[Display(Name = "Version", ResourceType = typeof(Resources.INP.RejectDetail))]
        public Int32 Version { get; set; }
        [Display(Name = "RejectDetail_ManufactureParty", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string ManufactureParty { get; set; }
        [Display(Name = "RejectDetail_IpNo", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string IpNo { get; set; }
        public Int32 IpDetailSequence { get; set; }
        [Display(Name = "RejectDetail_WMSNo", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string WMSNo { get; set; }
        public string WMSSeq { get; set; }
        [Display(Name = "RejectDetail_ReceiptNo", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string ReceiptNo { get; set; }
        public Int32 ReceiptDetailSequence { get; set; }
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
            RejectDetail another = obj as RejectDetail;

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
