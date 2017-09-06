using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class ShipPlanGroupSearchModel : SearchModelBase
    {
        public DateTime? PlanVersion { get; set; }

        public DateTime? DateIndex { get; set; }

        public string Item { get; set; }

        public string ProductLine { get; set; }

    }
}