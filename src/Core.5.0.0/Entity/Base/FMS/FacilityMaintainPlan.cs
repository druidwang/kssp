using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class FacilityMaintainPlan : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        public MaintainPlan MaintainPlan { get; set; }

        public string FCID { get; set; }

        [Display(Name = "FacilityMaintainPlan_StartDate", ResourceType = typeof(Resources.FMS.FacilityMaintainPlan))]
        public DateTime? StartDate { get; set; }

        [Display(Name = "FacilityMaintainPlan_NextMaintainDate", ResourceType = typeof(Resources.FMS.FacilityMaintainPlan))]
        public DateTime? NextMaintainDate { get; set; }

        [Display(Name = "FacilityMaintainPlan_NextWarnDate", ResourceType = typeof(Resources.FMS.FacilityMaintainPlan))]
        public DateTime? NextWarnDate { get; set; }

        [Display(Name = "FacilityMaintainPlan_StartQty", ResourceType = typeof(Resources.FMS.FacilityMaintainPlan))]
        public Decimal StartQty { get; set; }

        [Display(Name = "FacilityMaintainPlan_NextWarnQty", ResourceType = typeof(Resources.FMS.FacilityMaintainPlan))]
        public Decimal NextWarnQty { get; set; }

        [Display(Name = "FacilityMaintainPlan_NextMaintainQty", ResourceType = typeof(Resources.FMS.FacilityMaintainPlan))]
        public Decimal NextMaintainQty { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (Id != null)
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
            FacilityMaintainPlan another = obj as FacilityMaintainPlan;

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
