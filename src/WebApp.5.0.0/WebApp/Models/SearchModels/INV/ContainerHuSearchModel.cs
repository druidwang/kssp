using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class ContainerHuSearchModel : SearchModelBase
    {
        public String ContainerId { get; set; }
     
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Container { get; set; }

        public string Location { get; set; }

        public string HuId { get; set; }
    
    }
}