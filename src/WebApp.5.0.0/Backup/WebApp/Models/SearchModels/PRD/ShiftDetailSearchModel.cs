using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Web.Models.SearchModels.PRD
{
    public class ShiftDetailSearchModel:SearchModelBase
    {
        [Display(Name = "ShiftDetail_StartTime", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public DateTime? StartDate { get; set; }
        [Display(Name = "ShiftDetail_EndTime", ResourceType = typeof(Resources.MD.WorkingCalendar))]
        public DateTime? EndDate { get; set; }
    }
}