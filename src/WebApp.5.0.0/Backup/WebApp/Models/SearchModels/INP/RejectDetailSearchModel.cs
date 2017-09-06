using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INP
{
    public class RejectDetailSearchModel:SearchModelBase
    {
        public string RejectNo { get; set; }
        public Int16? Status { get; set; }
        public string InspectNo { get; set; }
        public Int32? HandleResult { get; set; }
        public string CreateUserName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string IpNo { get; set; }
        public string ReceiptNo { get; set; }
        public string WMSNo { get; set; }
        public string Item { get; set; }
        public string CurrentLocation { get; set; }

        public string JudgeUserName { get; set; }

        public bool? IsContainHu { get; set; }
    }
}