using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.MRP.VIEW
{
    public class ContainerView
    {
        public string Container { get; set; }
        public string ContainerDescription { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public double Qty { get; set; }
        public double ContainerQty { get; set; }
    }

}