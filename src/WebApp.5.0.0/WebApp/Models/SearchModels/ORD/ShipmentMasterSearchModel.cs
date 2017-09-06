using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class ShipmentMasterSearchModel : SearchModelBase
    {
        public string ShipmentNo { get; set; }
        public string VehicleNo { get; set; }
        public bool IsGoout { get; set; }
        public string Driver { get; set; }
        public string Shipper { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


    }
}