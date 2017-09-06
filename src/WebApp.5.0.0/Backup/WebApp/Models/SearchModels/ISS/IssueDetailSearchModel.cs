
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ISS
{
    public class IssueDetailSearchModel : SearchModelBase
    {
        public string IssueCode { get; set; }
        public string Content { get; set; }
        public string IssueLevel { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}