using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ISS
{
    public class IssueMasterSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string IssueSubject { get; set; }
        public string IssueTypeCode { get; set; }
        public string IssueAddressCode { get; set; }
        public string IssueNoCode { get; set; }
        public Int16? Type { get; set; }
        public Int16? Priority { get; set; }
        public Int16? Status { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Content { get; set; }

        public string BackYards { get; set; }
        public string Solution { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string FailCode { get; set; }
        public string DefectCode { get; set; }
    }
}