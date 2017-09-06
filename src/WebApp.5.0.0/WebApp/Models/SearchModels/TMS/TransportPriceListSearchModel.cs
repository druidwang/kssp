using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.Sconit.Web.Models.SearchModels.TMS
{
    public class TransportPriceListSearchModel : SearchModelBase
    {
        public string Code { get; set; }

        public string Carrier { get; set; }

        public Boolean IsActive { get; set; }
    }
}
