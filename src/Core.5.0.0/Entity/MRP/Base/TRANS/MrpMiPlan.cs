using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpMiPlan : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }

        [Display(Name = "MrpMiPlan_PlanDate", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public DateTime PlanDate { get; set; }
        [Display(Name = "MrpMiPlan_Item", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string Item { get; set; }
        [Display(Name = "MrpMiPlan_ProductLine", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string ProductLine { get; set; }
        [Display(Name = "MrpMiPlan_LocationFrom", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string LocationFrom { get; set; }
        [Display(Name = "MrpMiPlan_LocationTo", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string LocationTo { get; set; }
        [Display(Name = "MrpMiPlan_PlanVersion", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public DateTime PlanVersion { get; set; }

        [Display(Name = "MrpMiPlan_Sequence", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public int Sequence { get; set; }

        [Display(Name = "MrpMiPlan_WorkHour", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double WorkHour { get; set; }

        //³µ
        [Display(Name = "MrpMiPlan_UnitCount", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double UnitCount { get; set; }

        [Display(Name = "MrpMiPlan_Qty", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double Qty { get; set; }
        [Display(Name = "MrpMiPlan_AdjustQty", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double AdjustQty { get; set; }
        [Display(Name = "MrpMiPlan_MrpPriority", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double MrpPriority { get; set; }
        [Display(Name = "MrpMiPlan_MaxStock", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double MaxStock { get; set; }
        [Display(Name = "MrpMiPlan_SafeStock", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double SafeStock { get; set; }
        [Display(Name = "MrpMiPlan_InvQty", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double InvQty { get; set; }

        //public DateTime SnapTime { get; set; }

        [Display(Name = "MrpMiPlan_Bom", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string Bom { get; set; }
        [Display(Name = "MrpMiPlan_Uom", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string Uom { get; set; }

        //[Display(Name = "MrpMiPlan_ParentItem", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        //public string ParentItem { get; set; }
        //[Display(Name = "MrpMiPlan_SourceFlow", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        //public string SourceFlow { get; set; }
        //[Display(Name = "MrpMiPlan_SourceParty", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        //public string SourceParty { get; set; }
        [Display(Name = "MrpMiPlan_HuTo", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string HuTo { get; set; }

        public string Logs { get; set; }
        public double BatchSize { get; set; } 

        #endregion

    }
}
