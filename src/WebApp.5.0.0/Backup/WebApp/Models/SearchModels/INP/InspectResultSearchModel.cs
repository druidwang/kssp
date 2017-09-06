using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INP
{
    public class InspectResultSearchModel : SearchModelBase
    {
        public string InspectNo { get; set; }
        public string Item { get; set; }
        public string RejectNo { get; set; }
        public string IpNo { get; set; }
        public string ReceiptNo { get; set; }
        public string WMSNo { get; set; }
        public string CreateUserName { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? InspectType { get; set; }
        public int? Status { get; set; }
        public int? HandleResult { get; set; }
    }
}