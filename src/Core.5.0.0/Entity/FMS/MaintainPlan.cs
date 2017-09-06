using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    public partial class MaintainPlan 
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.MaintainPlanType, ValueField = "Type")]
        [Display(Name = "MaintainPlan_Type", ResourceType = typeof(Resources.FMS.MaintainPlan))]
        public string TypeDescription { get; set; }

        #endregion
    }
	
}
