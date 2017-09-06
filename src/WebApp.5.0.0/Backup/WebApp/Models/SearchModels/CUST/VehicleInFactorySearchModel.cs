using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.CUST
{
    public class VehicleInFactorySearchModel : SearchModelBase
    {
        public string OrderNo { get; set; }
        public com.Sconit.CodeMaster.VehicleInFactoryStatus? Status { get; set; }
        public string VehicleNo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Plant { get; set; }
        public bool IsInFactory { get; set; }
    }
}