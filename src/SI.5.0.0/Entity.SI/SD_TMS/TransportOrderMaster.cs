using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.SI.SD_TMS
{
    [Serializable]
    public class TransportOrderMaster
    {
        public string OrderNo { get; set; }
        public string Flow { get; set; }
        public string FlowDescription { get; set; }
        public com.Sconit.CodeMaster.TransportStatus Status { get; set; }
        public string Carrier { get; set; }
        public string Vehicle { get; set; }
        public string Tonnage { get; set; }
        public string DrivingNo { get; set; }
        public string Driver { get; set; }
        public string DriverMobilePhone { get; set; }
        public Decimal? LoadVolume { get; set; }
        public Decimal? LoadWeight { get; set; }
        public Decimal? MinLoadRate { get; set; }
        public Boolean IsAutoRelease { get; set; }
        public Boolean IsAutoStart { get; set; }
        public Boolean MultiSitePick { get; set; }
        public string ShipFrom { get; set; }
        public string ShipTo { get; set; }
        public com.Sconit.CodeMaster.TransportMode TransportMode { get; set; }
        public string PriceList { get; set; }
        public string BillAddress { get; set; }

        public string LicenseNo { get; set; }

        public List<TransportOrderDetail> TransportOrderDetailList { get; set; }
    }
}
