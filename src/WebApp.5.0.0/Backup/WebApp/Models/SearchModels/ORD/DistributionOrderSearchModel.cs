using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class DistributionOrderSearchModel : SearchModelBase
    {
        public string OrderNo { get; set; }
        public string Flow { get; set; }
        public Int16? Type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Int16? Priority { get; set; }
        public Int16? Status { get; set; }
        public string PartyFrom { get; set; }
        public string PartyFromName { get; set; }
        public string PartyTo { get; set; }
        public string PartyToName { get; set; }
        public string Shift { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string CreateUserName { get; set; }
    }
}