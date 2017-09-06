using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class MaintainPlan : EntityBase
    {
        #region O/R Mapping Properties

        [Display(Name = "MaintainPlan_Code", ResourceType = typeof(Resources.FMS.MaintainPlan))]
		public string Code{get;set;}

          [Display(Name = "MaintainPlan_Description", ResourceType = typeof(Resources.FMS.MaintainPlan))]
        public string Description { get; set; }

          [Display(Name = "MaintainPlan_Type", ResourceType = typeof(Resources.FMS.MaintainPlan))]
        public CodeMaster.MaintainPlanType Type { get; set; }

          [Display(Name = "MaintainPlan_Period", ResourceType = typeof(Resources.FMS.MaintainPlan))]
        public Int32 Period { get; set; }

          [Display(Name = "MaintainPlan_LeadTime", ResourceType = typeof(Resources.FMS.MaintainPlan))]
        public Int32 LeadTime { get; set; }

          [Display(Name = "MaintainPlan_TypePeriod", ResourceType = typeof(Resources.FMS.MaintainPlan))]
        public Int32 TypePeriod { get; set; }

          [Display(Name = "MaintainPlan_FacilityCategory", ResourceType = typeof(Resources.FMS.MaintainPlan))]
        public string FacilityCategory { get; set; }

          [Display(Name = "MaintainPlan_StartUpUser", ResourceType = typeof(Resources.FMS.MaintainPlan))]
        public string StartUpUser { get; set; }

        #endregion

		public override int GetHashCode()
        {
			if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MaintainPlan another = obj as MaintainPlan;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Code == another.Code);
            }
        } 
    }
	
}
