using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    [Serializable]
    public partial class MrpPlanLog : EntityBase
    {
        #region O/R Mapping Properties
        public DateTime PlanDate { get; set; }
        public string Item { get; set; }
        public Int32 PlanVersion { get; set; }
        public string Flow { get; set; }
        public string Location { get; set; }


        public DateTime WindowTime { get; set; }

        public Double Qty { get; set; }
        public string Uom { get; set; }
        public decimal UnitQty { get; set; }
        public string ItemDescription { get; set; }
        public string ItemReference { get; set; }

        public DateTime CreateDate { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (PlanDate != null && Item != null && PlanVersion != 0 && Flow != null && Location != null)
            {
                return PlanDate.GetHashCode()
                    ^ Item.GetHashCode()
                    ^ PlanVersion.GetHashCode()
                    ^ Flow.GetHashCode()
                    ^ Location.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MrpPlanLog another = obj as MrpPlanLog;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.PlanDate == another.PlanDate)
                    && (this.Item == another.Item)
                    && (this.PlanVersion == another.PlanVersion)
                    && (this.Flow == another.Flow)
                    && (this.Location == another.Location);
            }
        }
    }

}
