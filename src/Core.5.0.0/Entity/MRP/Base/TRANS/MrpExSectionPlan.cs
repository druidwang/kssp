using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpExSectionPlan : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 10)]
        [Display(Name = "MrpExSectionPlan_ProductLine", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public string ProductLine { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 30)]
        [Display(Name = "MrpExSectionPlan_Section", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public string Section { get; set; }
        [Display(Name = "MrpExSectionPlan_DateIndex", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public string DateIndex { get; set; }
        [Display(Name = "MrpExSectionPlan_PlanVersion", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public DateTime PlanVersion { get; set; }
        public DateTime PlanDate { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 20)]
        [Display(Name = "MrpExSectionPlan_Sequence", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public Int32 Sequence { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 90)]
        [Display(Name = "MrpExSectionPlan_Qty", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public Double Qty { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 100)]
        [Display(Name = "MrpExSectionPlan_CorrectionQty", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double CorrectionQty { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 110)]
        [Display(Name = "MrpExSectionPlan_AdjustQty", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double AdjustQty { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 50)]
        [Display(Name = "MrpExSectionPlan_Speed", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public Double Speed { get; set; }
        [Display(Name = "MrpExSectionPlan_ApsPriority", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public CodeMaster.ApsPriorityType ApsPriority { get; set; }
        [Display(Name = "MrpExSectionPlan_SwichTime", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public Double SwitchTime { get; set; }
        [Display(Name = "MrpExSectionPlan_SpeedTimes", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public Int32 SpeedTimes { get; set; }
        [Display(Name = "MrpExSectionPlan_Quota", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double Quota { get; set; }
        [Display(Name = "MrpExSectionPlan_MinLotSize", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double MinLotSize { get; set; }
        [Display(Name = "MrpExSectionPlan_EconomicLotSize", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double EconomicLotSize { get; set; }
        [Display(Name = "MaxLotSize", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double MaxLotSize { get; set; }
        [Display(Name = "MrpExSectionPlan_TurnQty", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public Int32 TurnQty { get; set; }
        [Display(Name = "MrpExSectionPlan_Correction", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double Correction { get; set; }
        [Display(Name = "MrpExSectionPlan_ShiftType", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public CodeMaster.ShiftType ShiftType { get; set; }
        [Display(Name = "MrpExSectionPlan_TotalAps", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public Int32 TotalAps { get; set; }
        [Display(Name = "MrpExSectionPlan_TotalQuota", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double TotalQuota { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 60)]
        [Display(Name = "MrpExSectionPlan_StartTime", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public DateTime StartTime { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 70)]
        [Display(Name = "MrpExSectionPlan_WindowTime", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public DateTime WindowTime { get; set; }
        [Display(Name = "MrpExSectionPlan_UpTime", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public Double UpTime { get; set; }
        #endregion

        [Display(Name = "MrpExSectionPlan_PlanNo", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public string PlanNo { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 80)]
        [Display(Name = "MrpExSectionPlan_ShiftQty", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public double ShiftQty { get; set; }
        [Display(Name = "MrpExSectionPlan_ProductType", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public string ProductType { get; set; }

        [Display(Name = "MrpExSectionPlan_Remark", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public string Remark { get; set; }
        public DateTime LatestStartTime { get; set; }
        public string Logs { get; set; }
        public Boolean IsEconomic { get; set; }
        public Double CurrentSwitchTime { get; set; }
        [Export(ExportName = "EXPlanSection", ExportSeq = 40)]
        [Display(Name = "MrpExSectionPlan_SectionDescription", ResourceType = typeof(Resources.MRP.MrpExSectionPlan))]
        public string SectionDescription { get; set; }

        public DateTime LastModifyDate { get; set; }
        public Boolean IsOld { get; set; }

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
            MrpExSectionPlan another = obj as MrpExSectionPlan;

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
