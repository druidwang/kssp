using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpFiMachinePlan : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        public DateTime PlanVersion { get; set; }
        public DateTime PlanDate { get; set; }
        public string ProductLine { get; set; }
        public string Island { get; set; }
        public string IslandDescription { get; set; }
        public string Machine { get; set; }
        public string MachineDescription { get; set; }
        public Double MachineQty { get; set; }
        public com.Sconit.CodeMaster.MachineType MachineType { get; set; }
        public Double ShiftQuota { get; set; }
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }
        public Double WorkDayPerWeek { get; set; }
        public int ShiftPerDay { get; set; }
        public double UnitCount { get; set; }
        public double ShiftQty { get; set; }
        public string ShiftSplit { get; set; }
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
            MrpFiMachinePlan another = obj as MrpFiMachinePlan;

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
