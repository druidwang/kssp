using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpExItemPlan : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Export(ExportName = "EXPlanItem", ExportSeq = 10)]
        [Display(Name = "MrpExItemPlan_Sequence", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public Int32 Sequence { get; set; }
        public Int32 SectionId { get; set; }
        [Export(ExportName = "EXPlanItem", ExportSeq = 5)]
        [Display(Name = "MrpExItemPlan_ProductLine", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public string ProductLine { get; set; }
        [Export(ExportName = "EXPlanItem", ExportSeq = 20)]
        [Display(Name = "MrpExItemPlan_Item", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public string Item { get; set; }

        [Display(Name = "MrpExItemPlan_DateIndex", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public string DateIndex { get; set; }
        [Display(Name = "MrpExItemPlan_PlanVersion", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public DateTime PlanVersion { get; set; }
        [Display(Name = "MrpExItemPlan_Section", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public string Section { get; set; }
        [Export(ExportName = "EXPlanItem", ExportSeq = 40)]
        [Display(Name = "MrpExItemPlan_RateQty", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public double RateQty { get; set; }

        [Display(Name = "MrpExItemPlan_PlanQty", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public Double PlanQty { get; set; }//–Ë«Û∆Ω∫‚
        [Export(ExportName = "EXPlanItem", ExportSeq = 50)]
        [Display(Name = "MrpExItemPlan_Qty", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public Double Qty { get; set; }
        [Export(ExportName = "EXPlanItem", ExportSeq = 60)]
        [Display(Name = "MrpExItemPlan_CorrectionQty", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public Double CorrectionQty { get; set; }
        [Export(ExportName = "EXPlanItem", ExportSeq = 70)]
        [Display(Name = "MrpExItemPlan_AdjustQty", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public double AdjustQty { get; set; }

        public double UnitCount { get; set; }
        //public string Shift { get; set; }
        [Export(ExportName = "EXPlanItem", ExportSeq = 30)]
        [Display(Name = "MrpExItemPlan_ItemDescritpion", ResourceType = typeof(Resources.MRP.MrpExItemPlan))]
        public string ItemDescription { get; set; }

        public double UsedQty { get; set; }
        public double InvQty { get; set; }

        public DateTime PlanDate { get; set; }
        public Boolean IsOld { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Id != 0)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MrpExItemPlan another = obj as MrpExItemPlan;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }
    }
}
