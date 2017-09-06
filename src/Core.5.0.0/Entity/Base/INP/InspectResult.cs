using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INP
{
    [Serializable]
    public partial class InspectResult : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        //[Display(Name = "Id", ResourceType = typeof(Resources.INP.InspectResult))]
        public Int32 Id { get; set; }
        [Display(Name = "InspectResult_InpNo", ResourceType = typeof(Resources.INP.InspectResult))]
        public string InspectNo { get; set; }
        public Int32 InspectDetailId { get; set; }
        [Display(Name = "InspectResult_InspectDetailSequence", ResourceType = typeof(Resources.INP.InspectResult))]
        public Int32 InspectDetailSequence { get; set; }
        [Display(Name = "InspectResult_Item", ResourceType = typeof(Resources.INP.InspectResult))]
        public string Item { get; set; }
        [Display(Name = "InspectResult_ItemDesc", ResourceType = typeof(Resources.INP.InspectResult))]
        public string ItemDescription { get; set; }
        [Display(Name = "InspectResult_RefItemCode", ResourceType = typeof(Resources.INP.InspectResult))]
        public string ReferenceItemCode { get; set; }
        [Display(Name = "InspectResult_Uom", ResourceType = typeof(Resources.INP.InspectResult))]
        public string Uom { get; set; }
        [Display(Name = "InspectResult_UC", ResourceType = typeof(Resources.INP.InspectResult))]
        public Decimal UnitCount { get; set; }
        public string BaseUom { get; set; }
        public Decimal UnitQty { get; set; }
        [Display(Name = "InspectResult_HuId", ResourceType = typeof(Resources.INP.InspectResult))]
        public string HuId { get; set; }
        [Display(Name = "InspectResult_LotNo", ResourceType = typeof(Resources.INP.InspectResult))]
        public string LotNo { get; set; }
        [Display(Name = "InspectResult_LocationFrom", ResourceType = typeof(Resources.INP.InspectResult))]
        public string LocationFrom { get; set; }
        [Display(Name = "InspectResult_CurrentLocation", ResourceType = typeof(Resources.INP.InspectResult))]
        public string CurrentLocation { get; set; }
        [Display(Name = "InspectResult_JudgeResult", ResourceType = typeof(Resources.INP.InspectResult))]
        public CodeMaster.JudgeResult JudgeResult { get; set; }
        [Display(Name = "InspectResult_JudgeQty", ResourceType = typeof(Resources.INP.InspectResult))]
        public Decimal JudgeQty { get; set; }
        [Display(Name = "InspectResult_HandleQty", ResourceType = typeof(Resources.INP.InspectResult))]
        public Decimal HandleQty { get; set; }
        [Display(Name = "InspectResult_IsHandle", ResourceType = typeof(Resources.INP.InspectResult))]
        public Boolean IsHandle { get; set; }
        //[Display(Name = "InspectResult_CreateUserId", ResourceType = typeof(Resources.INP.InspectResult))]
        public Int32 CreateUserId { get; set; }
        [Display(Name = "InspectResult_CreateUserName", ResourceType = typeof(Resources.INP.InspectResult))]
        public string CreateUserName { get; set; }
        [Display(Name = "InspectResult_CreateDate", ResourceType = typeof(Resources.INP.InspectResult))]
        public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INP.InspectResult))]
        public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INP.InspectResult))]
        [Display(Name = "InspectMaster_LastModifyUserNm", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string LastModifyUserName { get; set; }
        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.INP.InspectResult))]
        [Display(Name = "InspectMaster_LastModifyDate", ResourceType = typeof(Resources.INP.InspectMaster))]
        public DateTime LastModifyDate { get; set; }
        //[Display(Name = "Version", ResourceType = typeof(Resources.INP.InspectResult))]
        public Int32 Version { get; set; }
        [Display(Name = "InspectResult_Defect", ResourceType = typeof(Resources.INP.InspectResult))]
        public string Defect { get; set; }
        [Display(Name = "InspectResult_ManufactureParty", ResourceType = typeof(Resources.INP.InspectResult))]
        public string ManufactureParty { get; set; }
        [Display(Name = "InspectResult_WMSNo", ResourceType = typeof(Resources.INP.InspectResult))]
        public string WMSNo { get; set; }
        public string WMSSeq { get; set; }
        [Display(Name = "InspectResult_IpNo", ResourceType = typeof(Resources.INP.InspectResult))]
        public string IpNo { get; set; }
        public Int32 IpDetailSequence { get; set; }
        [Display(Name = "InspectResult_FailCode", ResourceType = typeof(Resources.INP.InspectResult))]
        public string FailCode { get; set; }
        [Display(Name = "InspectResult_ReceiptNo", ResourceType = typeof(Resources.INP.InspectResult))]
        public string ReceiptNo { get; set; }
        public Int32 ReceiptDetailSequence { get; set; }
        [Display(Name = "InspectResult_Note", ResourceType = typeof(Resources.INP.InspectResult))]
        public string Note { get; set; }
        [Display(Name = "InspectResult_RejectHandleResult", ResourceType = typeof(Resources.INP.InspectResult))]
        public CodeMaster.HandleResult RejectHandleResult { get; set; }
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
            InspectResult another = obj as InspectResult;

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
