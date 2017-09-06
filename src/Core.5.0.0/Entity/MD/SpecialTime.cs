using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    public partial class SpecialTime
    {
        #region Non O/R Mapping Properties
        
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.WorkingCalendarType, ValueField = "Type")]
        [Display(Name = "SpecialTime_Type", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string TypeDescription { get; set; }
        #endregion
    }
}