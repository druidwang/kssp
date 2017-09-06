using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class PurchasePlanMasterSearchModel : SearchModelBase
    {
        public DateTime? SnapTime { get; set; }
        public DateTime? MiPlanVersion { get; set; }
        public DateTime? PlanVersion { get; set; }
        public Int32? Status { get; set; }
        public Int32? DateType { get; set; }
        public bool IsRelease { get; set; }
        public string Flow { get; set; }
    }
}