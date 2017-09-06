using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.SD_WMS
{
    [Serializable]
    public class PickTask
    {
        public Int32 Id { get; set; }
        public com.Sconit.CodeMaster.OrderPriority Priority { get; set; }

        public string Item { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public string BaseUom { get; set; }

        public Decimal UnitQty { get; set; }

        public Decimal UnitCount { get; set; }

        public string Location { get; set; }

        public Decimal OrderQty { get; set; }

        public Decimal PickQty { get; set; }

        public string Area { get; set; }

        public string Bin { get; set; }

        public string LotNo { get; set; }

        public string HuId { get; set; }

        public Boolean IsPickHu { get; set; }

        public string PickGroup { get; set; }
        public Int32? PickUserId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime WinTime { get; set; }

        public Decimal CurrentQty { get; set; }
    }
}
