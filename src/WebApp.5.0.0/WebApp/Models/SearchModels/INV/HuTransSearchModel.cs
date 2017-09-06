using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.INV
{
    public class HuTransSearchModel : SearchModelBase
    {
        public String HuId { get; set; }
        public String Item { get; set; }
        public String LotNo { get; set; }
        public String LotNoTo { get; set; }
        public String Bin { get; set; }
        public String Location { get; set; }
        public String CreateUserName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public String SearchType { get; set; }
        public bool IsIncludeAll { get; set; }
    }
}