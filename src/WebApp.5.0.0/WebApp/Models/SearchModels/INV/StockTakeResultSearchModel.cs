using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class StockTakeResultSearchModel:SearchModelBase
    {
        public String Item { get; set; }
        public String Location { get; set; }
        public String LocationBin { get; set; }
        public Boolean IsLoss { get; set; }
        public Boolean IsProfit { get; set; }
        public Boolean IsBreakEven { get; set; }
        public String StNo { get; set; }
        public Boolean? IsAdjust { get; set; }
        public Boolean IsScanHu { get; set; }
    }
}