using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.SD_WMS
{
    [Serializable]
    public class DeliverBarCode
    {
        public string BarCode { get; set; }
        public string OrderNo { get; set; }
        public Int32 OrderSequence { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? WindowTime { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public Decimal UnitCount { get; set; }
        public Decimal Qty { get; set; }
        public Int16 Priority { get; set; }
        public string Station { get; set; }
        public string Dock { get; set; }
        public Boolean? IsActive { get; set; }
        public string HuId { get; set; }
        public Int32 Version { get; set; }
        public Int32 ShipPlanId { get; set; }
        public string Flow { get; set; }
        public Boolean IsPickHu { get; set; }
    }
}
