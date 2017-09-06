using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MrpFlowDetailSearchModel : SearchModelBase
    {
        public DateTime SnapTime { get; set; }

        public string Flow { get; set; }

        public string Item { get; set; }

        public string PartyFrom { get; set; }

        public string PartyTo { get; set; }

        public string LocationFrom { get; set; }

        public string LocationTo { get; set; }

       
    }
}