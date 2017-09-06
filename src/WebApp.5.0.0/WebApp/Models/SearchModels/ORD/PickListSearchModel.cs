using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class PickListSearchModel:SearchModelBase
    {
        public string PickListNo { get; set; }
        public string IpNo { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Status { get; set; }
        public string Item { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public string Flow { get; set; }
        public string LocationTo { get; set; }
        public int? Type { get; set; }
    }
}