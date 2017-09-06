using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.BIL
{
    public class PlanBillSearchModel : SearchModelBase
    {
        public string Item { get; set; }
        public string ReceiptNo { get; set; }
        public string Party { get; set; }
        public string Flow { get; set; }
        public string Currency { get; set; }
        public string OrderNo { get; set; }
        public DateTime? CreateDate_start { get; set; }
        public DateTime? CreateDate_End { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartTime { get; set; }
        public int? Type { get; set; }
    }
}