using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class ItemSearchModel:SearchModelBase
    {
        public string Code { get; set; }
        public string Uom { get; set; }
        public string Description { get; set; }
        public Decimal UnitCount { get; set; }
        public string ItemCategory { get; set; }
        public string MaterialsGroup { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsPurchase { get; set; }
        public Boolean IsSales { get; set; }
        public Boolean IsManufacture { get; set; }
        public Boolean IsSubContract { get; set; }
        public Boolean IsCustomerGoods { get; set; }
        public Boolean IsVirtual { get; set; }
        public Boolean IsKit { get; set; }
        public string Bom { get; set; }
        public string Location { get; set; }
        public string Routing { get; set; }
        public string ReferenceCode { get; set; }
    }
}