using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class SnapMrpPlan : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        public DateTime SnapTime { get; set; }

        [Display(Name = "MrpPlan_PlanDate", ResourceType = typeof(Resources.MRP.MrpPlan))]
        public DateTime PlanDate { get; set; }
        [Display(Name = "MrpPlan_Item", ResourceType = typeof(Resources.MRP.MrpPlan))]
        public string Item { get; set; }
        [Display(Name = "MrpPlan_Flow", ResourceType = typeof(Resources.MRP.MrpPlan))]
        public string Flow { get; set; }
        [Display(Name = "MrpPlan_Location", ResourceType = typeof(Resources.MRP.MrpPlan))]
        public string Location { get; set; }

        [Display(Name = "MrpPlan_OrderType", ResourceType = typeof(Resources.MRP.MrpPlan))]
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        [Display(Name = "MrpPlan_Qty", ResourceType = typeof(Resources.MRP.MrpPlan))] 
        public Double Qty { get; set; }
        [Display(Name = "MrpPlan_PlanVersion", ResourceType = typeof(Resources.MRP.MrpPlan))] 
        public Int32 PlanVersion { get; set; }
        [Display(Name = "MrpPlan_Party", ResourceType = typeof(Resources.MRP.MrpPlan))] 
        public string Party { get; set; }

        public double OrderQty { get; set; }
        public double ShippedQty { get; set; }
        public double ReceivedQty { get; set; }
        #endregion
    }

}
