using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class ShiftDetailSearchModel:SearchModelBase
    {
        public Int32 Id { get; set; }
		public string Shift { get; set; }
		public string ShiftTime { get; set; }
		public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}