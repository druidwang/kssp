using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpMiShiftPlan : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }

        [Display(Name = "MrpMiShiftPlan_Sequence", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public int Sequence { get; set; }
        [Display(Name = "MrpMiShiftPlan_ProductLine", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public string ProductLine { get; set; }
        [Display(Name = "MrpMiShiftPlan_PlanVersion", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public DateTime PlanVersion { get; set; }
        [Display(Name = "MrpMiShiftPlan_PlanDate", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public DateTime PlanDate { get; set; }
        //[Display(Name = "MrpMiShiftPlan_Location", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        //public string Location { get; set; }
        [Display(Name = "MrpMiShiftPlan_Shift", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public string Shift { get; set; }
        [Display(Name = "MrpMiShiftPlan_Item", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public string Item { get; set; }

        [Display(Name = "MrpMiShiftPlan_Qty", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public double Qty { get; set; }
        [Display(Name = "MrpMiShiftPlan_AdjustQty", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public double AdjustQty { get; set; }
        [Display(Name = "MrpMiShiftPlan_Uom", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public string Uom { get; set; }
        [Display(Name = "MrpMiShiftPlan_UnitCount", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public Double UnitCount { get; set; }

        [Display(Name = "MrpMiShiftPlan_HuTo", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public string HuTo { get; set; }

        [Display(Name = "MrpMiShiftPlan_OrderedQty", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public Double OrderedQty { get; set; }

        public double WorkHour { get; set; }
        //public double MaxStock { get; set; }
        //public double SafeStock { get; set; }
        //public double InvQty { get; set; }


        public string Bom { get; set; }//不一定等于Item
        public DateTime StartTime { get; set; }
        public DateTime WindowTime { get; set; }

        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }

        public DateTime CreateDate { get; set; }
        #endregion

    }
}
