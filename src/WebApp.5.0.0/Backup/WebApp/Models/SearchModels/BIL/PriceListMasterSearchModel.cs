using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.BIL
{
    public class PriceListMasterSearchModel:SearchModelBase
    {
        public string Code { get; set; }
        public string Party { get; set; }
    }
}