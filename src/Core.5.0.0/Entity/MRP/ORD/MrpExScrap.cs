using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    public partial class MrpExScrap
    {
        #region O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ScheduleType, ValueField = "ScrapType")]
        [Display(Name = "MrpExScrap_ScrapType", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public string ScrapTypeDescription { get; set; }

        #endregion
    }


}
