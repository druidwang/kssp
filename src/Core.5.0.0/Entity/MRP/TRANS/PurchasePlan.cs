using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class PurchasePlan
    {
        [Display(Name = "PurchasePlan_CurrentQty", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public double CurrentQty { get; set; }

        [Display(Name = "PurchasePlan_ItemDescription", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public string ItemDescription { get; set; }

        [Display(Name = "FlowDetail_Uom", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public string Uom { get; set; }

        [Display(Name = "FlowDetail_UnitCount", ResourceType = typeof(Resources.SCM.FlowDetail))]
        public Decimal UnitCount { get; set; }

        public string DateIndexValue { get; set; }
    }
}
