using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MachineInstanceSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public int? DateType { get; set; }
        public string Region { get; set; }
      
        public int? MachineType { get; set; }
        public int? ShiftType { get; set; }

        public string DateIndex { get; set; }

        public string DateIndexTo { get; set; }

        public DateTime? DateIndexDate { get; set; }

        public DateTime? DateIndexToDate { get; set; }

        public DateTime? SnapTime { get; set; }

    }
}