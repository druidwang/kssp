using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.PRD
{
    public class ProductLineLocationDetailSearchModel : SearchModelBase
    {
        public string ProductLine { get; set; }
        public string ProductLineFacility { get; set; }
        public string HuId { get; set; }
        public string Item { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreateUserName { get; set; }
        public bool IncludeClose { get; set; }
        public string OrderNo { get; set; }
        public string LocationFrom { get; set; }
        public string Operation { get; set; }
    }
}