using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpFiPlan : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }

        public DateTime PlanDate { get; set; }
        public string ProductLine { get; set; }
        public string Item { get; set; }
        public string LocationTo { get; set; }
        public string LocationFrom { get; set; }
        public string Bom { get; set; }
        public DateTime PlanVersion { get; set; }

        public string Machine { get; set; }
        public string MachineDescription { get; set; }
        public Double MachineQty { get; set; }
        public string Island { get; set; }
        public string IslandDescription { get; set; }
        public Double IslandQty { get; set; }
        public int Sequence { get; set; }
        public com.Sconit.CodeMaster.MachineType MachineType { get; set; }
        //public string DateIndex { get; set; }
        public double ShiftQuota { get; set; }
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }
        public Double WorkDayPerWeek { get; set; }
        public int ShiftPerDay { get; set; }

        //public CodeMaster.ItemPriority ItemPriority { get; set; }

        public double Qty { get; set; }

        public double AdjustQty { get; set; }
        public double UnitCount { get; set; }
        //public DateTime EffectiveDate { get; set; }

        public bool IsRelease { get; set; }

        //辅助字段 当前期末库存
        public double CurrentInvQty { get; set; }
        //累计需求数 总需求-总计划需求 根据收发存计算出来的
        //public double AccReqQty { get; set; }

        public double InvQty { get; set; }
        public double SafeStock { get; set; }
        public double MaxStock { get; set; }
        public string Logs { get; set; }

        //占用岛区数
        public double OccupyIslandQty { get; set; }
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
            MrpFiPlan another = obj as MrpFiPlan;

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
