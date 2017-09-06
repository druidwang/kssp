using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INP
{
    public class ConcessionMasterSearchModel : SearchModelBase
    {
        public string ConcessionNo { get; set; }
        public string RejectNo { get; set; }
        public Int16? Status { get; set; }
        public string CreateUserName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}