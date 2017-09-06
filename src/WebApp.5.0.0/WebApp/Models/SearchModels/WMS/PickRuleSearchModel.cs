using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.WMS
{
    public class PickRuleSearchModel : SearchModelBase
    {
        public string PickGroupCode { get; set; }
        public string Location { get; set; }
    }
}