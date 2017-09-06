using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.BIL
{
    [Serializable]
    public partial class BillMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Export(ExportName = "BillNotice", ExportSeq = 10)]
        [Export(ExportName = "ProcurementBillSearch", ExportSeq = 10)]
        [Export(ExportName = "SaleBillSearch", ExportSeq = 10)]
        [Display(Name = "TransportBillMaster_BillNo", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public string BillNo { get; set; }
        [Export(ExportName = "BillNotice", ExportSeq = 30)]
        [Export(ExportName = "ProcurementBillSearch", ExportSeq = 30)]
        [Export(ExportName = "SaleBillSearch", ExportSeq = 30)]
        [Display(Name = "TransportBillMaster_ExternalBillNo", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public string ExternalBillNo { get; set; }
        [Display(Name = "TransportBillMaster_ReferenceBillNo", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
    
        public com.Sconit.CodeMaster.BillType Type { get; set; }
        public CodeMaster.BillSubType SubType { get; set; }
        [Display(Name = "TransportBillMaster_Status", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public CodeMaster.BillStatus Status { get; set; }
        [Display(Name = "TransportBillMaster_BillAddress", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public string BillAddress { get; set; }
        public string BillAddressDescription { get; set; }
        public string Party { get; set; }
        [Export(ExportName = "BillNotice", ExportSeq = 20)]
        [Export(ExportName = "ProcurementBillSearch", ExportSeq = 20)]
        [Export(ExportName = "SaleBillSearch", ExportSeq = 20, ExportTitle = "ActingBill_PartyName_Distribution", ExportTitleResourceType = typeof(Resources.BIL.ActingBill))]
        [Display(Name = "TransportBillMaster_PartyNm", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public string PartyName { get; set; }
        public string Currency { get; set; }
        //public Boolean IsIncludeTax { get; set; }
        //public string Tax { get; set; }
        //[Display(Name = "TransportBillMaster_EffectiveDate", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        //public DateTime EffectiveDate { get; set; }
        public Int32 CreateUserId { get; set; }
        [Export(ExportName = "BillNotice", ExportSeq = 50)]
        [Export(ExportName = "ProcurementBillSearch", ExportSeq = 50)]
        [Export(ExportName = "SaleBillSearch", ExportSeq = 50)]
        [Display(Name = "TransportBillMaster_CreateUserName", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public string CreateUserName { get; set; }
        [Export(ExportName = "BillNotice", ExportSeq = 40)]
        [Export(ExportName = "ProcurementBillSearch", ExportSeq = 40)]
        [Export(ExportName = "SaleBillSearch", ExportSeq = 40)]
        [Display(Name = "TransportBillMaster_CreateDate", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32 Version { get; set; }

        public DateTime? ReleaseDate { get; set; }
        public Int32? ReleaseUserId { get; set; }
        public string ReleaseUserName { get; set; }

        public DateTime? CloseDate { get; set; }
        public Int32? CloseUserId { get; set; }
        public string CloseUserName { get; set; }

        public DateTime? CancelDate { get; set; }
        public Int32? CancelUserId { get; set; }
        public string CancelUserName { get; set; }
        public string CancelReason { get; set; }
        [Display(Name = "TransportBillMaster_InvoiceNo", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public string InvoiceNo { get; set; }
        [Display(Name = "TransportBillMaster_InvoiceDate", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public DateTime? InvoiceDate { get; set; }

        public Decimal Amount { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (BillNo != null)
            {
                return BillNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            BillMaster another = obj as BillMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.BillNo == another.BillNo);
            }
        }
    }

}
