using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeanEngine.Utility;

namespace LeanEngine.Entity
{
    public class OrderTracer : EntityBase
    {
        public Enumerators.TracerType TracerType { get; set; }

        public string Code { get; set; }

        public DateTime ReqTime { get; set; }

        public Item Item { get; set; }

        public decimal OrderedQty { get; set; }

        public decimal FinishedQty { get; set; }

        public decimal Qty { get; set; }

        public int RefOrderLocTransId { get; set; }

        //JIT2策略专用
        public string Location { get; set; }
    }
}
