using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.CUST
{
    public class ProdLinePrSearchModel : SearchModelBase
    {
        public string ProductLine { get; set; }

        public string Machine { get; set; }

        public string Item { get; set; }
    }
}