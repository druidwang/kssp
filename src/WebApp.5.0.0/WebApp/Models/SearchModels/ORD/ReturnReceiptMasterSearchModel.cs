using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class ReturnReceiptMasterSearchModel : SearchModelBase
    {
        public string ReceiptNo { get; set; }
        public string IpNo { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public int? GoodsReceiptOrderType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Dock { get; set; }
        public int? Status { get; set; }
    }
}