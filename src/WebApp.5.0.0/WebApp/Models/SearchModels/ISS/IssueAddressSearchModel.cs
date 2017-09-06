using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ISS
{
    public class IssueAddressSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string ParentIssueAddressCode { get; set; }
        public string Description { get; set; }
    }
}