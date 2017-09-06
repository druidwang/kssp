using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.MES
{
    public class InventoryRequest
    {
        public string RequestId { get; set; }

        public InventoryRequestCondition Data { get; set; }

        public string Requester { get; set; }

        public DateTime RequestDate { get; set; }
    }

    public class InventoryRequestCondition
    { 
        public string MaterialCode { get; set; }
        public string WarehouseCode { get; set; }
        public string FactoryCode { get; set; }
        public string BarCode { get; set; }
        public string BatchNo { get; set; }

    }
}
