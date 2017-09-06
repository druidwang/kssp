using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.FMS
{
    public class FacilityOrderSearchModel:SearchModelBase
    {
        public string FacilityOrderNo { get; set; }
       
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
     
    }
}