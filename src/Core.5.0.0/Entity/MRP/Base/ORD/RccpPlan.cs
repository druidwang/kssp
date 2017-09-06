using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    [Serializable]
    public partial class RccpPlan : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }

        [Display(Name = "RccpPlan_DateIndex", ResourceType = typeof(Resources.MRP.RccpPlan))]
        public string DateIndex { get; set; }
        [Display(Name = "RccpPlan_Flow", ResourceType = typeof(Resources.MRP.RccpPlan))]
        public string Flow { get; set; }
        [Display(Name = "RccpPlan_Item", ResourceType = typeof(Resources.MRP.RccpPlan))]
        public string Item { get; set; }
        public com.Sconit.CodeMaster.TimeUnit DateType { get; set; }
        [Display(Name = "RccpPlan_Qty", ResourceType = typeof(Resources.MRP.RccpPlan))]
        public Double Qty { get; set; }
        [Display(Name = "RccpPlan_PlanVersion", ResourceType = typeof(Resources.MRP.RccpPlan))]
        public Int32 PlanVersion { get; set; }

        public string DateIndexTo { get; set; }

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
            RccpPlan another = obj as RccpPlan;

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
