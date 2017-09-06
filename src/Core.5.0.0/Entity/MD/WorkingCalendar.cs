using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    public partial class WorkingCalendar
    {
        #region Non O/R Mapping Properties

        
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.WorkingCalendarType, ValueField = "Type")]
        [Display(Name = "WorkingCalendar_Type", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string WorkingCalendarTypeDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.DayOfWeek, ValueField = "DayOfWeek")]
        [Display(Name = "WorkingCalendar_DayOfWeek", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public string WorkingCalendarDayOfWeek { get; set; }
        #endregion
    }
}