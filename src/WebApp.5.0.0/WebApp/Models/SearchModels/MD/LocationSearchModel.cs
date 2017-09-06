using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

 namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class LocationSearchModel:SearchModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public int? Type { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsIncludeInActive { get; set; }
        public Boolean AllowNegaInv { get; set; }
        public Boolean EnableAdvWM { get; set; }
        public Boolean ISCS { get; set; }
        public Boolean IsMRP { get; set; }
        public string SAPLocation { get; set; }
        
    }
}