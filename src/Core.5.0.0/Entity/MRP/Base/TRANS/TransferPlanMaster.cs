using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class TransferPlanMaster : EntityBase
    {
        public DateTime PlanVersion { get; set; }
        public string Flow { get; set; }
        public DateTime SnapTime { get; set; }
        public DateTime SourcePlanVersion { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }

        public override int GetHashCode()
        {
            if (PlanVersion != DateTime.MinValue && Flow != null)
            {
                return PlanVersion.GetHashCode() ^ Flow.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            TransferPlanMaster another = obj as TransferPlanMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.PlanVersion == another.PlanVersion) && (this.Flow == another.Flow);
            }
        }
    }
}
