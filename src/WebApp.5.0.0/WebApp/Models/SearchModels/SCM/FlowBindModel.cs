using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.SCM
{
    public class FlowBindModel : SearchModelBase
    {
        public string MasterFlow { get; set; }
        public string BindedFlow { get; set; }
        public string id { get; set; }
    }
}