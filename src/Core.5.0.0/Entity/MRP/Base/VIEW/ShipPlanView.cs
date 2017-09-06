using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.VIEW
{
    [Serializable]
    public partial class ShipPlanView : EntityBase
    {
        #region O/R Mapping Properties

        public string Flow { get; set; }
        public string FlowType { get; set; }
        public string Item { get; set; }
        public string Uom { get; set; }
        public Decimal UnitCount { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime WindowTime { get; set; }
        public Decimal Qty { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string ItemReference { get; set; }
        public string SourceType { get; set; }
        public string PeriodType { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Flow != null && Item != null && Uom != null && UnitCount != 0 && StartTime != null && WindowTime != null && EffectiveDate != null)
            {
                return Flow.GetHashCode() ^ Item.GetHashCode() ^ Uom.GetHashCode() ^ UnitCount.GetHashCode() ^ StartTime.GetHashCode() ^ WindowTime.GetHashCode() ^ EffectiveDate.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ShipPlanView another = obj as ShipPlanView;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Flow == another.Flow) && (this.Item == another.Item) && (this.Uom == another.Uom) && (this.UnitCount == another.UnitCount) && (this.StartTime == another.StartTime) && (this.WindowTime == another.WindowTime) && (this.EffectiveDate == another.EffectiveDate);
            }
        }
    }

}
