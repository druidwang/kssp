using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class LocationAreaSearchModel:SearchModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public Boolean IsActive { get; set; }
    }
}