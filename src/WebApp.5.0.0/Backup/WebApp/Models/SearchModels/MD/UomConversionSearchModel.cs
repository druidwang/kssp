using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.Sconit.Entity.MD;
namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class UomConversionSearchModel:SearchModelBase
    {
       // public Item Item { get; set; }
       // public string ItemDescription { get; set; }
       // public string BaseUom { get; set; }
       // public string AlterUom { get; set; }
       // public Decimal BaseQty { get; set; }
       // public Decimal AlterQty { get; set; }
        public string ItemCode { get; set; }
        public string Uom { get; set; }
    }
}