using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class StockTakeMasterSearchModel:SearchModelBase
    {
        public String StNo { get; set; }
        public String Region { get; set; }
        public int? Type { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int? Status { get; set; }
        public Boolean IsScanHu { get; set; }
        public Boolean IsDynamic { get; set; }
        public int? GroupBy { get; set; }
        public DateTime? StockTakeStartDate { get; set; }
        public DateTime? StockTakeEndDate { get; set; }
        public string CostCenter { get; set; }
    }
}