using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.BIL
{
    public class PriceListDetailSearchModel : SearchModelBase
    {
        public string Item { get; set; }

        public string ShipFrom { get; set; }

        public string ShipTo { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}