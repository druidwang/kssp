using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.Sconit.Web.Models.SearchModels.TMS
{
    public class TransportFlowSearchModel : SearchModelBase
    {
        public string Flow { get; set; }

        public string Description { get; set; }

        public Boolean IsActive { get; set; }
    }
}
