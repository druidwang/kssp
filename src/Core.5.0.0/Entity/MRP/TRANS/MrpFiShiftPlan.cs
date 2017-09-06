using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.ORD;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpFiShiftPlan
    {
        [Display(Name = "MrpFiShiftPlan_Uom", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string Uom { get; set; }
        [Display(Name = "MrpFiShiftPlan_ItemDescription", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string ItemDescription { get; set; }
        [Display(Name = "MrpFiShiftPlan_ReferenceItemCode", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string ReferenceItemCode { get; set; }

        public OrderDetail OrderDetail { get; set; }
    }
}
