using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class HuToSearchModel : SearchModelBase
    {
        public string Code { get; set; }
    }

    public class HuToMappingSearchModel : SearchModelBase
    {
        public string HuTo { get; set; }
        public string Party { get; set; }
        public string Flow { get; set; }
        public string Item { get; set; }
    }
}