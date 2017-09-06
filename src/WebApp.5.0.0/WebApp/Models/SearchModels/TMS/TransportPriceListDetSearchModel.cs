using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.Sconit.Web.Models.SearchModels.TMS
{
    public class TransportPriceListDetSearchModel : SearchModelBase
    {
        public string ShipFrom { get; set; }

        public string ShipTo { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
