using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class RccpPlanMasterSearchModel : SearchModelBase
    {
        public string DateType { get; set; }
        public DateTime? SnapTime { get; set; }
        public string DateIndex { get; set; }
        public Int32? Status { get; set; }

        public string Item { get; set; }
        public DateTime? PlanVersion { get; set; }
        public bool? IsDown { get; set; }
        public DateTime DateIndexDate { get; set; }
    }
}