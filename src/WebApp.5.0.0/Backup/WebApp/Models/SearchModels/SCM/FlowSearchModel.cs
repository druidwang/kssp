using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.SCM
{
    public class FlowSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public Boolean IsActive { get; set; }
        public string DAUAT { get; set; }
    }
}