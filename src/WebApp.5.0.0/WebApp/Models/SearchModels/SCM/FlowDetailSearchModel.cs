using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.SCM
{
    public class FlowDetailSearchModel : SearchModelBase
    {
        public string Item { get; set; }
        public string Flow { get; set; }
        public string flowCode { get; set; }
        public bool? IsChangeUnitCount { get; set; }
    }
}