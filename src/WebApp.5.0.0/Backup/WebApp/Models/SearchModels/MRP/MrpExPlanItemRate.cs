using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class MrpExPlanItemRateSearchModel : SearchModelBase
    {
        public string Section { get; set; }
        public string Item { get; set; }
        public string SectionDesc { get; set; }
    }
}