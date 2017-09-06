using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.Sconit.Web.Models.SearchModels.MD
{
    public class ItemDiscontinueSearchModel : SearchModelBase
    {

        public string Item{ get; set; }
        public string DiscontinueItem { get; set; }
        public string UnitQty { get; set; }
        public string Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
      

    }
}
