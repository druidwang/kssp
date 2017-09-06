using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class IpMasterSearchModel:SearchModelBase
    {
        public string IpNo { get; set; }
        public int? Status { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public int? IpOrderType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ShipFromAddress { get; set; }
        public string type { get; set; }
        public string Dock { get; set; }
        public string Item { get; set; }
        public string OrderNo { get; set; }
        public string ExternalIpNo { get; set; }
        public string ManufactureParty { get; set; }
        public string Flow { get; set; }
        public int? OrderSubType { get; set; }
        public int? IpDetailType { get; set; }
        public string CreateUserName { get; set; }
    }
}