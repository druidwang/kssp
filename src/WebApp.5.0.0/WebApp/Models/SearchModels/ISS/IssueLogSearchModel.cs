using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ISS
{
    public class IssueLogSearchModel : SearchModelBase
    {
        public string IssueCode { get; set; }
        public string Content { get; set; }
        public string Level { get; set; }
        public string CreateUser { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}