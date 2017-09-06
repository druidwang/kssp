using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class KanBanCardSearchModel : SearchModelBase
    {
        public string Flow { get; set; }
        public string LocatoinTo { get; set; }
        public string Item { get; set; }
        public string Sequence { get; set; }
        public string Note { get; set; }
        public string Code { get; set; }
    }
}