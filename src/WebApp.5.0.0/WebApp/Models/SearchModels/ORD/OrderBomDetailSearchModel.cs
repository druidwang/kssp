using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Web.Models.SearchModels.ORD
{
    public class OrderBomDetailSearchModel : SearchModelBase
    {
        public int? Operation { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }
        public string OpReference { get; set; }
        public int OrderDetailId { get; set; }
        public int OrderStatus { get; set; }
    }
}