using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class LocationBinSearchModel:SearchModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
        public string LocationCode { get; set; }
        public string Location { get; set; }
        public Int32? Sequence { get; set; }
        public Boolean IsActive { get; set; }
    }
}