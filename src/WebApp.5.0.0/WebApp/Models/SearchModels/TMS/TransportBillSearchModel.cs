using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.TMS
{
    public class TransportBillSearchModel : SearchModelBase
    {
        public string BillNo { get; set; }
        public string ExternalBillNo { get; set; }
        public string Status { get; set; }
        public string Carrier { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartTime { get; set; }
        
    }
}