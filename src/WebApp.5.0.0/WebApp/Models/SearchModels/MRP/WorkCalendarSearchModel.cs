using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class WorkCalendarSearchModel : SearchModelBase
    {
        public string DateIndexTo { get; set; }
        public Int16? DateType { get; set; }
        public DateTime? DateIndexDate { get; set; }
        public DateTime? DateIndexToDate { get; set; }
        public string DateIndex { get; set; }
        public string ProductLine { get; set; }
        public int ResourceGroup { get; set; }
      
    }
}