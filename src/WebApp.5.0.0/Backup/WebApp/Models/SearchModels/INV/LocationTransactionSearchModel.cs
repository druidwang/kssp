using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class LocationTransactionSearchModel : SearchModelBase
    {
        public string TransactionType { get; set; }
        public string Item { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public string Location { get; set; }
        public int TimeType { get; set; }
    }
}