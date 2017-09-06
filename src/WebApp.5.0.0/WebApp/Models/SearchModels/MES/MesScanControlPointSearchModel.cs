using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MES
{
    public class MesScanControlPointSearchModel : SearchModelBase
    {
        public string ControlPoint { get; set; }
        public string TraceCode { get; set; }
    }
}