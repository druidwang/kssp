using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class WorkingCalendarSearchModel:SearchModelBase
    {
        public Int32 Id { get; set; }
        public string Region { get; set; }
        public string DayOfWeek { get; set; }
        public int? Type { get; set; }
        public string shiftDetail_StartTime { get; set; }
        public string shiftDetail_EndTime { get; set; }
        public string Flow { get; set; }
    }
}