using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class SchedulingView
    {
        public Int32? Id { get; set; }
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; }
        public string TaskSubTypeCode { get; set; }
        public string TaskSubTypeDesc { get; set; }
        public string StartUser { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public string WorkdayType { get; set; }
        public string SchedulingType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Boolean IsAutoAssign { get; set; }
    }
}