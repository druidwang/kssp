using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.BIL
{
    public partial class BillMaster
    {
        #region Non O/R Mapping Properties


        public DateTime? EndDate { get; set; }
        public IList<BillDetail> BillDetails { get; set; }
        [Export(ExportName = "BillNotice", ExportSeq = 60)]
        [Export(ExportName = "ProcurementBillSearch", ExportSeq = 60)]
        [Export(ExportName = "SaleBillSearch", ExportSeq = 60)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.BillStatus, ValueField = "Status")]
        [Display(Name = "TransportBillMaster_Status", ResourceType = typeof(Resources.BIL.TransportBillMaster))]
        public string StatusDescription { get; set; }


        public void AddBillDetail(BillDetail billDetail)
        {
            if (this.BillDetails == null)
            {
                this.BillDetails = new List<BillDetail>();
            }
            this.BillDetails.Add(billDetail);
        }

        public void RemoveBillDetail(BillDetail billDetail)
        {
            if (this.BillDetails != null)
            {
                this.BillDetails.Remove(billDetail);
            }
        }
        #endregion
    }
}