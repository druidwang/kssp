using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    public partial class RccpPlan
    {
        #region O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TimeUnit, ValueField = "DateType")]
        [Display(Name = "RccpPlan_DateType", ResourceType = typeof(Resources.MRP.RccpPlan))]
        public string DateTypeDescription { get; set; }
        #endregion
    }
}
