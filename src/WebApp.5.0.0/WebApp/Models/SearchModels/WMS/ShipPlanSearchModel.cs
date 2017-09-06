using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.WMS
{
    public class ShipPlanSearchModel : SearchModelBase
    {
        public string Item { get; set; }
        public string OrderNo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Flow { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsShipScanHu { get; set; }
    }
}