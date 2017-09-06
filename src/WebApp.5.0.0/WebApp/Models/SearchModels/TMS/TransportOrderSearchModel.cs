using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.Sconit.Web.Models.SearchModels.TMS
{
    public class TransportOrderSearchModel : SearchModelBase
    {
        public string OrderNo { get; set; }

        public string Carrier { get; set; }

        public Int16? Status { get; set; }

        public string MultiStatus { get; set; }

        public string ShipFrom { get; set; }

        public string ShipTo { get; set; }

        public string IpNo { get; set; }

        public Int16? TransportMode { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
