using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class SwitchTradingSearchModel : SearchModelBase
    {
        public string Flow { get; set; }
        public string Supplier { get; set; }
        public string Customer { get; set; }
        public string PurchaseGroup { get; set; }
    }
}