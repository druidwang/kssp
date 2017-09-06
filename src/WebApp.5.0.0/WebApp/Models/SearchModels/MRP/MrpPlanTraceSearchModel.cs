using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MrpPlanTraceSearchModel : SearchModelBase
    {
        public int? ResourceGroup { get; set; }
        public string ProductLine { get; set; }
        public string Item { get; set; }
    }
}