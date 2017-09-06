using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MrpPlanExSearchModel : SearchModelBase
    {
        public string PlanVersion { get; set; }
        public string Flow { get; set; }
        public string DateIndex { get; set; }
        public string Section { get; set; }
    }
}