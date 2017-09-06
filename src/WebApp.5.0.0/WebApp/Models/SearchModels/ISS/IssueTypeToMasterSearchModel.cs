using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ISS
{
    public class IssueTypeToMasterSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string IssueType { get; set; }
        public string IssueLevel { get; set; }
        public Boolean IsActive { get; set; }
    }
}