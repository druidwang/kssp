using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    [Serializable]
    public partial class RccpPlanLog : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 PlanId { get; set; }//联合主键
        public Int32 PlanVersion { get; set; }//联合主键

        public string DateIndex { get; set; }
        public string Flow { get; set; }
        public string Item { get; set; }
        public com.Sconit.CodeMaster.TimeUnit DateType { get; set; }

        public Double Qty { get; set; }
        public string Uom { get; set; }
        public decimal UnitQty { get; set; }
        public string ItemDescription { get; set; }
        public string ItemReference { get; set; }
        public string DateIndexTo { get; set; }

        public DateTime CreateDate { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (PlanId != 0 && PlanVersion != 0)
            {
                return PlanId.GetHashCode()
                    ^ PlanVersion.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            RccpPlanLog another = obj as RccpPlanLog;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.PlanId == another.PlanId)
                    && (this.PlanVersion == another.PlanVersion);
            }
        }
    }

}
