using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class RccpPlanMaster
    {
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.MessageType, ValueField = "Status")]
        [Display(Name = "RccpPlanMaster_Status", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public string StatusDescription { get; set; }
        [Display(Name = "RccpPlanMaster_PlanVersion", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public string PlanVersionShow
        {
            get
            {
                if (this.PlanVersion == DateTime.MinValue)
                {
                    return null;
                }
                else
                {
                    return this.PlanVersion.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            set
            {
                this.PlanVersionShow = value;
            }
        }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TimeUnit, ValueField = "DateType")]
        [Display(Name = "RccpPlanMaster_DateType", ResourceType = typeof(Resources.MRP.RccpPlanMaster))]
        public string DateTypeDescription { get; set; }
    }
}
