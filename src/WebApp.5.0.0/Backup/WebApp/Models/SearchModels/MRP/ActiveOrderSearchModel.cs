using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class ActiveOrderSearchModel : SearchModelBase
    {
        public DateTime SnapTime{ get; set; }
        public string Flow { get; set; }
        public string Item { get; set; }
        public string OrderNo { get; set; }
        public string Location { get; set; }
        public bool IsIndepentDemand { get; set; }
    }
}