using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class SupplierShipAddressSearchModel : SearchModelBase
    {
        public string AddressCode { get; set; }
        public string AddressContent { get; set; }
    }
}
