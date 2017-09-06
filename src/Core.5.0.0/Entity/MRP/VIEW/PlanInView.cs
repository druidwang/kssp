using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.VIEW
{
    public class PlanInView
    {
        public string Item { get; set; }
        public string Flow { get; set; }
        public string Shift { get; set; }
        public string StartTime { get; set; }
        public string WindowTime { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public double Qty { get; set; }
    }
}