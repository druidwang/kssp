using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpSnapMaster
    {
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.MessageType, ValueField = "Status")]
        [Display(Name = "MrpSnapMaster_Status", ResourceType = typeof(Resources.MRP.MrpSnapMaster))]
        public string StatusDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.SnapType, ValueField = "Type")]
        [Display(Name = "MrpSnapMaster_Type", ResourceType = typeof(Resources.MRP.MrpSnapMaster))]
        public string TypeDescription { get; set; }

        [Display(Name = "MrpSnapMaster_SnapTime", ResourceType = typeof(Resources.MRP.MrpSnapMaster))]
        public string SnapTimeShow
        {
            get
            {
                if (this.SnapTime == DateTime.MinValue)
                {
                    return null;
                }
                else
                {
                    return this.SnapTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            set
            {
                this.SnapTimeShow = value;
            }
        }
    }
}
