using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class RccpPlanSearchModel : SearchModelBase
    {
        public string Flow { get; set; }
        public Int16? Status { get; set; }
        public DateTime? StartDate{ get; set; }
        public DateTime? EndDate { get; set; }
        public string StartWeek { get; set; }
        public string EndWeek { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }
        public Int32 PlanVersion { get; set; }
        public string DisplayName { get; set; }
        public string ImportType { get; set; }
        public string StartMonth { get; set; }
        public string EndMonth { get; set; }
     
    }
}