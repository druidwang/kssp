using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.VIEW
{
    [Serializable]
    public partial class TransitInventoryView : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string Flow { get; set; }
        public string Item { get; set; }
        public string Uom { get; set; }
        public Decimal UnitCount { get; set; }
        public string Location { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? WindowTime { get; set; }
        public Decimal? TransitQty { get; set; }
        public DateTime EffectiveDate{ get; set; }
        
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
            TransitInventoryView another = obj as TransitInventoryView;

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
