using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.PRD
{
    public class RoutingDetailSearchModel:SearchModelBase 
    {
        public string Operation { get; set; }
        public string routingMasterCode { get; set; }
    }
}