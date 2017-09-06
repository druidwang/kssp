using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class MiscOrderSearchModel : SearchModelBase
    {
        public string MiscOrderNo { get; set; }
        public Int16? Status { get; set; }
        public string GeneralLedger { get; set; }
        public string Location { get; set; }
        public string MoveType { get; set; }
        public string CostCenter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreateUserName { get; set; }
        public string SearchType { get; set; }

        //public string FactoryCode { get; set; }
        //public string Remarks1 { get; set; }
        //public string Remarks2 { get; set; }
        //public string Remarks3 { get; set; }
        //public string Remarks4 { get; set; }

        public string Flow { get; set; }
        public string Item { get; set; }
        public string Note { get; set; }
        public Int16? SubType { get; set; }
        public string WBS { get; set; }
        public Int16? QualityType { get; set; }
    }

}