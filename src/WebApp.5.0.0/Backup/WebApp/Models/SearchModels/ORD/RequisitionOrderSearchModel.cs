using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class RequisitionOrderSearchModel : SearchModelBase
    {
        public string TransferFlow { get; set; }
        public string ProductLine { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class RequisitionTransferOrderSearchModel
    {
        public bool IsPlan { get; set; }
        public string TransferFlow { get; set; }
        public DateTime WindowTime { get; set; }
        public DateTime StartTime { get; set; }
        //Item(Section),ProductLine,Qty//OrderNo
        public string[][] Details { get; set; }
    }
}