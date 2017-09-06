using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpExItemPlan
    {
        [Export(ExportName = "EXPlanItem", ExportSeq = 36)]
        [Display(Name = "MrpExSectionPlan_StartTime", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public DateTime StartTime { get; set; }
        [Export(ExportName = "EXPlanItem", ExportSeq = 39)]
        [Display(Name = "MrpExSectionPlan_WindowTime", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public DateTime WindowTime { get; set; }
        public double CurrentQty { get; set; }

        public MrpExSectionPlan MrpExSectionPlan { get; set; }

        /// <summary>
        /// 需求比
        /// </summary>
        public double RequirePlanRate
        {
            get
            {
                if (PlanQty == 0)
                {
                    return 0;
                }
                return Math.Round(CurrentQty / PlanQty, 5);
            }
        }
        [Export(ExportName = "EXPlanItem", ExportSeq = 80)]
        [Display(Name = "MrpExItemPlan_TotalQty", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public double TotalQty
        {
            get
            {
                return this.Qty + this.CorrectionQty + this.AdjustQty;
            }
        }
        [Export(ExportName = "EXPlanItem", ExportSeq = 90, ExportTitle = "MrpExItemPlan_TotalLength", ExportTitleResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public double TotalLength
        {
            get
            {
                return this.TotalQty * this.RateQty;
            }
        }

        private double _planQtyRate;
        /// <summary>
        /// 配平比
        /// </summary>
        public double PlanQtyRate
        {
            get
            {
                return Math.Round(_planQtyRate, 5);
            }
            set
            {
                _planQtyRate = value;
            }
        }

    }

}
