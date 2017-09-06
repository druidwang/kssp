using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpMiShiftPlan
    {
        [Display(Name = "MrpMiPlan_CheQty", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double CheQty
        {
            get { return TotalQty / UnitCount; }
        }

        [Display(Name = "MrpMiPlan_TotalQty", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double TotalQty
        {
            get { return Qty + AdjustQty; }
        }

        [Display(Name = "MrpMiShiftPlan_ItemDescription", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public string ItemDescription { get; set; }

        [Display(Name = "MrpMiShiftPlan_HuTo", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public string HuToDescription { get; set; }

        [Range(0, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpMiShiftPlan_CurrentCheQty", ResourceType = typeof(Resources.MRP.MrpMiShiftPlan))]
        public double CurrentCheQty { get; set; }
    }
}
