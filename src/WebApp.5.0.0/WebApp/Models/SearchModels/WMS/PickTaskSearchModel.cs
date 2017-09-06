using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.WMS
{
    public class PickTaskSearchModel : SearchModelBase
    {
        public string Item { get; set; }
        public string PickUser { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Location { get; set; }
        public Boolean IsActive { get; set; }
        public string PickGroup { get; set; }
    }
}