using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.CUST
{
    public class DefectCodeSearchModel : SearchModelBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Assemblies { get; set; }
        public string ProductCode { get; set; }
        public DateTime? CreateDateStart { get; set; }
        public DateTime? CreateDateEnd { get; set; }

    }
}