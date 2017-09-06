using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class FlowItemSearchModel:SearchModelBase
    {
        public string Flow { get; set; }
        public string Item { get; set; }
        public string SearchType { get; set; }

    }
}