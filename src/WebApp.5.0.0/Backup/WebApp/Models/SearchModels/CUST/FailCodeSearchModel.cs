using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.CUST
{
    public class FailCodeSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string CHNDescription { get; set; }
        public string ENGDescription { get; set; }
    }
}