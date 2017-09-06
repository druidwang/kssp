using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    [Serializable]
    public partial class MrpShiftPlanLog : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 PlanId { get; set; }
        public Int32 PlanVersion { get; set; }

        public Double Qty { get; set; }
        public string Uom { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (PlanId != 0 && PlanVersion != 0)
            {
                return PlanId.GetHashCode() ^ PlanVersion.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MrpShiftPlanLog another = obj as MrpShiftPlanLog;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.PlanId == another.PlanId) && (this.PlanVersion == another.PlanVersion);
            }
        }
    }

}
