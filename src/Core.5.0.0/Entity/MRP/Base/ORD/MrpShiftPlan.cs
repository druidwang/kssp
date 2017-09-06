using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    [Serializable]
    public partial class MrpShiftPlan : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }

        public DateTime PlanDate { get; set; }
        public string Item { get; set; }
        public string Flow { get; set; }
        public string Shift { get; set; }

        public string Uom { get; set; }
        public Double Qty { get; set; }

        public Int32 PlanVersion { get; set; }
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
            MrpShiftPlan another = obj as MrpShiftPlan;

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
