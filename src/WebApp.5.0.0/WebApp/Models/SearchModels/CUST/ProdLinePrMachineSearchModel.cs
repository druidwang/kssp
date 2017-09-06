using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.CUST
{
    public class ProdLinePrMachineSearchModel : SearchModelBase
    {
        public string ProductResource { get; set; }

        public string Machine { get; set; }
    }
}