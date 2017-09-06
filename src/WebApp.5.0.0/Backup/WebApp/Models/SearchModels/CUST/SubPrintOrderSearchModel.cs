using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.CUST
{
    public class SubPrintOrderSearchModel : SearchModelBase
    {
        public string UserCode { get; set; }        
        public string Region { get; set; }
        public string Flow { get; set; }
        public string Location { get; set; }
        public string ExcelTemplate { get; set; }
        public string Printer { get; set; }
        public string Client { get; set; }
    }
}