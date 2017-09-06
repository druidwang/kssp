using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MrpPlanFiSearchModel : SearchModelBase
    {
        public DateTime PlanVersion { get; set; }
        public string Flow { get; set; }
        public DateTime PlanDate{ get; set; }
        public string Shift { get; set; }
    }
}