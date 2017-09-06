using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class PurchasePlanMaster : EntityBase
    {
        public int Id { get; set; }
        public DateTime SnapTime { get; set; }
        public DateTime PlanVersion { get; set; }
        public DateTime SourcePlanVersion { get; set; }
        public CodeMaster.TimeUnit DateType { get; set; }
        public string Flow { get; set; }
        public string FlowDescription { get; set; }
        public bool IsRelease { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

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
            PurchasePlanMaster another = obj as PurchasePlanMaster;

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
