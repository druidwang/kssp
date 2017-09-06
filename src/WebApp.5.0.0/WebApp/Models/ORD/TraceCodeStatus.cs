using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models
{
    [Serializable]
    public class TraceCodeStatus
    {
        public string TraceCode { get; set; }
        public string Op1 { get; set; }
        public string Op2 { get; set; }
        public string Op3 { get; set; }
        public string Op4 { get; set; }
        public string Op5 { get; set; }
        public string Op6 { get; set; }

    }
}