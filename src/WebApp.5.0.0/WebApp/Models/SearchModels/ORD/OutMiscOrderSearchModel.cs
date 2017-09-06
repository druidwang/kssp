using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class OutMiscOrderSearchModel:SearchModelBase
    {
        public string MiscOrderNo { get; set; }
        public Int16? Status { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public string MoveType { get; set; }
        public string CostCenter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreateUserName { get; set; }

        
    }
}