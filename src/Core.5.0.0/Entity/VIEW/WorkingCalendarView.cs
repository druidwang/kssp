using System;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.VIEW
{
    public class WorkingCalendarView
    {
        [Display(Name = "WorkingCalendar_Date", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public DateTime Date { get; set; }
        [Display(Name = "WorkingCalendar_DayOfWeek", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public DayOfWeek DayOfWeek { get; set; }
        [Display(Name = "ShiftMaster_Code", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public String ShiftCode { get; set; }
        [Display(Name = "ShiftMaster_Name", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public String ShiftName { get; set; }
        [Display(Name = "WorkingCalendar_DateFrom", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public DateTime DateFrom { get; set; }
        [Display(Name = "WorkingCalendar_DateTo", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public DateTime DateTo { get; set; }
        [Display(Name = "WorkingCalendar_Type", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public com.Sconit.CodeMaster.WorkingCalendarType Type { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.DayOfWeek, ValueField = "DayOfWeek")]
        public string DayOfWeekDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.WorkingCalendarType, ValueField = "Type")]
        public string TypeDescription { get; set; }

       
    }
}
