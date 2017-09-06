using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class MrpExOrderSearchModel : SearchModelBase
    {
        public string PlanNo { get; set; }
        public string DateIndex { get; set; }
        public string Production { get; set; }
        public string Section { get; set; }
    }
}