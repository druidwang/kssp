using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpMiPlanDetail
    {
        [Display(Name = "MrpMiPlan_ParentItemDescription", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string ParentItemDescription { get; set; }
        [Display(Name = "MrpMiPlan_SourcePartyDescription", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string SourcePartyDescription { get; set; }
        [Display(Name = "MrpMiPlan_SourceFlowDescription", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string SourceFlowDescription { get; set; }
    }

}
