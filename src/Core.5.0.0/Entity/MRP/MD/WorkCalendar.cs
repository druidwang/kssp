using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    public partial class WorkCalendar
    {
        #region Non O/R Mapping Properties
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TimeUnit, ValueField = "DateType")]
        [Display(Name = "MrpWorkCalendar_DateType", ResourceType = typeof(Resources.MRP.MrpWorkCalendar))]
        public string DateTypeDescription { get; set; }

        #endregion
    }
}