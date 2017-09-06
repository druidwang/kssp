using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.WMS
{
    public class PickResultSearchModel : SearchModelBase
    {
        public string Item { get; set; }
        public string OrderNo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Location { get; set; }
    }
}