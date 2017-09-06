using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.Sconit.Entity.MD;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class PartyAddressSearchModel : SearchModelBase
    {
        public string  AddressCode { get; set; }
        public string AddressContent { get; set; }

    }
}