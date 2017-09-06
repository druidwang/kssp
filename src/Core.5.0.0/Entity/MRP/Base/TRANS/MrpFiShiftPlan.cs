using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpFiShiftPlan : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        [Display(Name = "MrpFiShiftPlan_ProductLine", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string ProductLine { get; set; }
        [Display(Name = "MrpFiShiftPlan_Item", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string Item { get; set; }
        //[Display(Name = "MrpFiShiftPlan_Location", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        //public string Location { get; set; }
        [Display(Name = "MrpFiShiftPlan_PlanDate", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public DateTime PlanDate { get; set; }
        [Display(Name = "MrpFiShiftPlan_Shift", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string Shift { get; set; }
        [Display(Name = "MrpFiShiftPlan_Sequence", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public int Sequence { get; set; }
        [Display(Name = "MrpFiShiftPlan_PlanVersion", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public DateTime PlanVersion { get; set; }

        [Display(Name = "MrpFiShiftPlan_Qty", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public double Qty { get; set; }

        [Display(Name = "MrpFiShiftPlan_Machine", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string Machine { get; set; }
        [Display(Name = "MrpFiShiftPlan_MachineDescription", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string MachineDescription { get; set; }
        [Display(Name = "MrpFiShiftPlan_MachineQty", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public Double MachineQty { get; set; }
        [Display(Name = "MrpFiShiftPlan_MachineType", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public com.Sconit.CodeMaster.MachineType MachineType { get; set; }
        [Display(Name = "MrpFiShiftPlan_Island", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string Island { get; set; }
        [Display(Name = "MrpFiShiftPlan_IslandDescription", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public string IslandDescription { get; set; }
        //[Display(Name = "MrpFiShiftPlan_DateIndex", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        //public string DateIndex { get; set; }
        [Display(Name = "MrpFiShiftPlan_ShiftQuota", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public Double ShiftQuota { get; set; }
        [Display(Name = "MrpFiShiftPlan_ShiftType", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }
        [Display(Name = "MrpFiShiftPlan_WorkDayPerWeek", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public Double WorkDayPerWeek { get; set; }
        [Display(Name = "MrpFiShiftPlan_ShiftPerDay", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public int ShiftPerDay { get; set; }

        [Display(Name = "MrpFiShiftPlan_UnitCount", ResourceType = typeof(Resources.MRP.MrpFiShiftPlan))]
        public double UnitCount { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime WindowTime { get; set; }

        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string Bom { get; set; }

        public int OrderDetailId { get; set; }
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
            MrpFiShiftPlan another = obj as MrpFiShiftPlan;

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
