using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.SD_TMS
{
    [Serializable]
    public class TransportOrderDetail
    {
        public Int32 Id { get; set; }
        public string OrderNo { get; set; }
        public Int32 Sequence { get; set; }
        public string IpNo { get; set; }
        public Int32 OrderRouteFrom { get; set; }
        public Int32 OrderRouteTo { get; set; }
        public Int32? EstPalletQty { get; set; }
        public Int32? PalletQty { get; set; }
        public Decimal? EstVolume { get; set; }
        public Decimal? Volume { get; set; }
        public Decimal? EstWeight { get; set; }
        public Decimal? Weight { get; set; }
        public Int32? EstBoxCount { get; set; }
        public Int32? BoxCount { get; set; }
        public DateTime? LoadTime { get; set; }
        public DateTime? UnloadTime { get; set; }
        public string Dock { get; set; }
        public Decimal? Distance { get; set; }
        public Boolean IsReceived { get; set; }
    }
}
