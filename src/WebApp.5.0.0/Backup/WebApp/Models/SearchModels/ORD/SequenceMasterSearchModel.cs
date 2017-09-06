using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class SequenceMasterSearchModel:SearchModelBase
    {
        public string SequenceNo { get; set; }
        public string Flow { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Item { get; set; }
        public string Status { get; set; }
        public string TraceCode { get; set; }



    }
}