using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MaterailPlanSearchModel : SearchModelBase
    {
        public DateTime? PlanVersion { get; set; }
        public string PlanDate { get; set; }
        public string PlanDateTo { get; set; }
        public string Flow { get; set; }
        public string Item { get; set; }
        public bool IsStartTime { get; set; }
        public bool? IsSupplier { get; set; }
        public int DateType { get; set; }
        public string BackUrl { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime WindowTime { get; set; }
        public string MaterialsGroup { get; set; }
    }
}