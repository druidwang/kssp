using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class ReceiptMasterSearchModel : SearchModelBase
    {
        public string ReceiptNo { get; set; }
        public string IpNo { get; set; }
        public string OrderNo { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public int? GoodsReceiptOrderType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Dock { get; set; }
        public int? Status { get; set; }
        public string Item { get; set; }
        public string ExternalReceiptNo { get; set; }
        public int? OrderSubType { get; set; }
        public int? IpDetailType { get; set; }
        public int? OrderType { get; set; }
        public string ManufactureParty { get; set; }
        public string Flow { get; set; }
        public string CreateUserName { get; set; }
        public bool IsReturn { get; set; }
        public bool IsDiff { get; set; }
        //public int? OrderType { get; set; }
    }
}