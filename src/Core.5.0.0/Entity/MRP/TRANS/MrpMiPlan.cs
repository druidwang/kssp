using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpMiPlan
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

        [Range(0, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpMiPlan_CurrentCheQty", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public double CurrentCheQty { get; set; }

        [Display(Name = "MrpMiPlan_ItemDescription", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string ItemDescription { get; set; }
        [Display(Name = "MrpMiPlan_HuTo", ResourceType = typeof(Resources.MRP.MrpMiPlan))]
        public string HuToDescription { get; set; }

        public string ParentItem { get; set; }
        public string SourceFlow { get; set; }
        public string SourceParty { get; set; }
        public double SumQty { get; set; }

        public List<MrpMiPlanDetail> MrpMiPlanDetailList { get; set; }

    }

}
