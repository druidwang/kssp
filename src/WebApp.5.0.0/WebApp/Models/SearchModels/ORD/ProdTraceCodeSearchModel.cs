using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class ProdTraceCodeSearchModel : SearchModelBase
    {
        public string TraceCode { get; set; }
        public string HuId { get; set; }
    }
}