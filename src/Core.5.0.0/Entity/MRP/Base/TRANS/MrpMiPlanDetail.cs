using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpMiPlanDetail : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        public int PlanId { get; set; }
        [Display(Name = "MrpMiPlan_ParentItem", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string ParentItem { get; set; }
        [Display(Name = "MrpMiPlan_SourceFlow", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string SourceFlow { get; set; }
        [Display(Name = "MrpMiPlan_SourceParty", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string SourceParty { get; set; }
        [Display(Name = "MrpMiPlan_Qty", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double Qty { get; set; }
        #endregion

    }
}
