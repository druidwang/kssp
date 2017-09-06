using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MrpPlanMasterSearchModel : SearchModelBase
    {
        public DateTime? PlanVersion{ get; set; }
        public Int32? Status{ get; set; }
        public Int32? ResourceGroup { get; set; }
        public string DateIndex { get; set; }
        public bool IsRelease { get; set; }
    }
}