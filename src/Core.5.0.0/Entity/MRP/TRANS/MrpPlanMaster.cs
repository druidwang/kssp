using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpPlanMaster
    {
        [Display(Name = "MrpPlanMaster_PlanVersion", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
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
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.MessageType, ValueField = "Status")]
        [Display(Name = "MrpPlanMaster_Status", ResourceType = typeof(Resources.MRP.MrpPlanMaster))]
        public string StatusDescription { get; set; }

        public string ShortWord { get; set; }

        public string SiteMapPath
        {
            get {
                if (this.ResourceGroup== CodeMaster.ResourceGroup.MI)
                {
                    return "Url_Mrp_MrpSchedule_Mi";
                }
                else if (this.ResourceGroup == CodeMaster.ResourceGroup.EX)
                {
                    return "Url_Mrp_MrpSchedule_Ex";
                }
                else if (this.ResourceGroup == CodeMaster.ResourceGroup.FI)
                {
                    return "Url_Mrp_MrpSchedule_Fi";
                }
                else
                {
                    return null;
                }
            }
        }

    }

}
