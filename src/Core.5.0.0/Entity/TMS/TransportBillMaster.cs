using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    public partial class TransportBillMaster
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.BillStatus, ValueField = "Status")]
        [Display(Name = "TransportBillMaster_Status", ResourceType = typeof(Resources.TMS.TransportBillMaster))]
        public string StatusDescription { get; set; }

        public IList<TransportBillDetail> BillDetails { get; set; }

        public void AddBillDetail(TransportBillDetail billDetail)
        {
            if (this.BillDetails == null)
            {
                this.BillDetails = new List<TransportBillDetail>();
            }
            this.BillDetails.Add(billDetail);
        }

        public void RemoveBillDetail(TransportBillDetail billDetail)
        {
            if (this.BillDetails != null)
            {
                this.BillDetails.Remove(billDetail);
            }
        }
        #endregion
    }
}