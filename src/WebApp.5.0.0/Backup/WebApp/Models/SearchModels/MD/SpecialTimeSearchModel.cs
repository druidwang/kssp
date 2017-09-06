using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class SpecialTimeSearchModel:SearchModelBase
    {
        public Int32 Id { get; set; }
        public string Region { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Description { get; set; }
        public int? Type { get; set; }
        public string Flow { get; set; }
    }
}