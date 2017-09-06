using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class CustomerSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
    }
}
