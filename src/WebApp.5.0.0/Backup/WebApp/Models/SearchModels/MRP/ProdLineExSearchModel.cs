using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MRP
{
    public class ProdLineExSearchModel : SearchModelBase
    {
        public string ProductLine { get; set; }
        
        public string Item { get; set; }
        
        public Double? Speed { get; set; }
        
        public Int16? ApsPriority { get; set; }
        
        public Double? SetupTime { get; set; }
        
        public Double? SwitchTime { get; set; }
        
        public Double? MaintenanceTime { get; set; }
        
        public Int32? SpeedTimes { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}