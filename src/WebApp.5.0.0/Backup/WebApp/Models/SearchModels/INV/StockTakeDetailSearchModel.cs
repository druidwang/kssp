using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class StockTakeDetailSearchModel : SearchModelBase
    {
        public string ItemCode { get; set; }
        public string Location { get; set; }
        public string LocationBin { get; set; }
        public string StNo { get; set; }
        public bool IsScanHu { get; set; }
    }
}