using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models
{
    [Serializable]
    public class WebOrderDetail
    {
        public Int32 Sequence { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public Decimal UnitCount { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string Bom { get; set; }
        public string Routing { get; set; }
        public Decimal MinUnitCount { get; set; }
        public string UnitCountDescription { get; set; }
        public string Container { get; set; }
        public string ContainerDescription { get; set; }
        public string ReferenceItemCode { get; set; }
        public string ManufactureParty { get; set; }

    }
}