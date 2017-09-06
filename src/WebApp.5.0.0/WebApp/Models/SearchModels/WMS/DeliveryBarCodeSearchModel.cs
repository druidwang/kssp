using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.WMS
{
    public class DeliveryBarCodeSearchModel : SearchModelBase
    {
        public String BarCode { get; set; }
        public String HuId { get; set; }
        public String Flow { get; set; }
        public String OrderNo { get; set; }
        public String Item { get; set; }
        public String CreateUserName { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }

        public string ShipFrom { get; set; }
        public string ShipTo { get; set; }
       
    }
}