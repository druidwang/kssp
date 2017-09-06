using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.FMS
{
    public class FacilityTransSearchModel : SearchModelBase
    {
        public string FCID { get; set; }
        public string Name { get; set; }
        public string AssetNo { get; set; }
        public string Category { get; set; }
        public string TransType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}