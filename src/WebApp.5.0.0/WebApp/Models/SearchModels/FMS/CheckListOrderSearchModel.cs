using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.FMS
{
    public class CheckListOrderSearchModel : SearchModelBase
    {
        public string CheckListCode { get; set; }
        public string CheckListOrderNo { get; set; }
       
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string CreateUserName { get; set; }

    }
}