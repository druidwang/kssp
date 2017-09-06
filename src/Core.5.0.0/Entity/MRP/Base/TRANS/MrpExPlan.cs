using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpExPlan : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        public string DateIndex { get; set; }
        public DateTime PlanVersion { get; set; }
        public string Section { get; set; }
        public double SectionQty { get; set; }
        public string Item { get; set; }
        public double NetQty { get; set; }
        public double ItemQty { get; set; }
        public Double RateQty { get; set; }
        public double InvQty { get; set; }
        public double SafeStock { get; set; }
        public DateTime LatestStartTime { get; set; }
        public Double UnitCount { get; set; }

        public DateTime StartTime { get; set; }
        public double PlanInQty { get; set; }
        public double PlanOutQty { get; set; }

        public string Flows { get; set; }
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
            MrpExPlan another = obj as MrpExPlan;

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
