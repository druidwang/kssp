using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.CUST
{
    public class CostCenterSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}