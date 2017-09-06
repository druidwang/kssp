using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.INP
{
    [Serializable]
    public partial class InspectDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        //[Display(Name = "Id", ResourceType = typeof(Resources.INP.InspectDetail))]
        public Int32 Id { get; set; }
        //[Display(Name = "InspectNo", ResourceType = typeof(Resources.INP.InspectDetail))]
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 10, ExportTitle = "InspectMaster_InspectNo", ExportTitleResourceType = typeof(Resources.INP.InspectMaster))]
        public string InspectNo { get; set; }
        [Display(Name = "InspectDetail_Sequence", ResourceType = typeof(Resources.INP.InspectDetail))]
        public Int32 Sequence { get; set; }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 20)]
        [Display(Name = "InspectDetail_Item", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string Item { get; set; }
        [Display(Name = "InspectDetail_ItemDesc", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string ItemDescription { get; set; }
        [Display(Name = "InspectDetail_RefItemCode", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string ReferenceItemCode { get; set; }
        [Display(Name = "InspectDetail_UC", ResourceType = typeof(Resources.INP.InspectDetail))]
        public Decimal UnitCount { get; set; }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 40)]
        [Display(Name = "InspectDetail_Uom", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string Uom { get; set; }
        public string BaseUom { get; set; }
        public Decimal UnitQty { get; set; }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 50)]
        [Display(Name = "InspectDetail_HuId", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string HuId { get; set; }
        [Display(Name = "InspectDetail_LotNo", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string LotNo { get; set; }
        [Display(Name = "InspectDetail_LocationFrom", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string LocationFrom { get; set; }
        [Display(Name = "InspectDetail_CurrentLocation", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string CurrentLocation { get; set; }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 60)]
        [Display(Name = "InspectDetail_InspectQtyTotal", ResourceType = typeof(Resources.INP.InspectDetail))]
        public Decimal InspectQty { get; set; }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 70)]
        [Display(Name = "InspectDetail_QualifyQty", ResourceType = typeof(Resources.INP.InspectDetail))]
        public Decimal QualifyQty { get; set; }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 80)]
        [Display(Name = "InspectDetail_RejectQty", ResourceType = typeof(Resources.INP.InspectDetail))]
        public Decimal RejectQty { get; set; }
        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.INP.InspectDetail))]
        public Int32 CreateUserId { get; set; }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 100)]
        [Display(Name = "InspectDetail_CreateUserName", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string CreateUserName { get; set; }
        [Export(ExportName = "InspectOrderDetail", ExportSeq = 110)]
        [Display(Name = "InspectDetail_CreateDate", ResourceType = typeof(Resources.INP.InspectDetail))]
        public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INP.InspectDetail))]
        public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INP.InspectDetail))]
        [Display(Name = "InspectMaster_LastModifyUserNm", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string LastModifyUserName { get; set; }
        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.INP.InspectDetail))]
        [Display(Name = "InspectMaster_LastModifyDate", ResourceType = typeof(Resources.INP.InspectMaster))]
        public DateTime LastModifyDate { get; set; }
        //[Display(Name = "Version", ResourceType = typeof(Resources.INP.InspectDetail))]
        public Int32 Version { get; set; }
        public Boolean IsJudge { get; set; }
        [Display(Name = "InspectDetail_ManufactureParty", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string ManufactureParty { get; set; }
        public string WMSSeq { get; set; }
        public Int32 IpDetailSequence { get; set; }
        public Int32 ReceiptDetailSequence { get; set; }
        [Display(Name = "InspectDetail_Note", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string Note { get; set; }
        [Display(Name = "InspectDetail_FailCode", ResourceType = typeof(Resources.INP.InspectDetail))]
        public string FailCode { get; set; }

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
            InspectDetail another = obj as InspectDetail;

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
