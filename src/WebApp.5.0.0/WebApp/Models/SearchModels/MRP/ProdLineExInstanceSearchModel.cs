using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class ProdLineExInstanceSearchModel : SearchModelBase
    {
        public string ProductLine { get; set; }
        
        public string Item { get; set; }

        public int? DateType { get; set; }

        public int? ShiftType { get; set; }

        public int? ApsPriority { get; set; }

        public string DateIndex { get; set; }

        public string DateIndexTo { get; set; }

        public DateTime? DateIndexDate { get; set; }

        public DateTime? DateIndexToDate { get; set; }

        public DateTime? SnapTime { get; set; }
    }
}