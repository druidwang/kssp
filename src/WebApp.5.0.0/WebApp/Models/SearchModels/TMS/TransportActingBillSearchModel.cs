using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.TMS
{
    public class TransportActingBillSearchModel : SearchModelBase
    {
        public string OrderNo { get; set; }
        public string Flow { get; set; }
    
        public string Carrier { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartTime { get; set; }
      
    }
}