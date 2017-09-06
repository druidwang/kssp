using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class PartySearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Plant { get; set; }
        public Boolean IsActive { get; set; }
        public string ShortCode { get; set; }
        public string Workshop { get; set; }
    }
}