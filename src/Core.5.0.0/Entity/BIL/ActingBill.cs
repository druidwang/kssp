using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.BIL
{
    public partial class ActingBill
    {
        #region Non O/R Mapping Properties

        public decimal CurrentVoidQty { get; set; }
        public string CheckOrderNo { get; set; }
        [Export(ExportName = "ProcurementActBill", ExportSeq = 120)]
        [Export(ExportName = "SalesActBill", ExportSeq = 140)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 40, ExportTitle = "CodeDetail_InspectType_Quantity", ExportTitleResourceType = typeof(Resources.SYS.CodeDetail))]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 40, ExportTitle = "CodeDetail_InspectType_Quantity", ExportTitleResourceType = typeof(Resources.SYS.CodeDetail))]
        [Display(Name = "ActingBill_CurrentBillQty", ResourceType = typeof(Resources.BIL.ActingBill))]
        public Decimal CurrentBillQty { get; set; }
        [Display(Name = "ActingBill_CurrentBillAmount", ResourceType = typeof(Resources.BIL.ActingBill))]
        [Export(ExportName = "SalesActBill", ExportSeq = 160)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 140)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 45, ExportTitle = "ActingBill_BillAmount", ExportTitleResourceType = typeof(Resources.BIL.ActingBill))]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 45, ExportTitle = "ActingBill_BillAmount", ExportTitleResourceType = typeof(Resources.BIL.ActingBill))]
        public Decimal CurrentBillAmount { get; set; }
        [Export(ExportName = "SalesActBill", ExportSeq = 150)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 130)]
        [Display(Name = "ActingBill_CurrentDiscount", ResourceType = typeof(Resources.BIL.ActingBill))]
        public Decimal CurrentDiscount { get; set; }
        [Display(Name = "ActingBill_CurrentRecalculatePrice", ResourceType = typeof(Resources.BIL.ActingBill))]
        public decimal CurrentRecalculatePrice { get; set; }

        #endregion
    }
}