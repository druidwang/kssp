using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.Sconit.Web.Models.SearchModels.PRD
{
    public class BomDetailSearchModel : SearchModelBase
    {
        public string BomDetail_Bom { get; set; }
        public string BomDetail_Item { get; set; }
        public Int32 BomMrpOption { get; set; }
    }
}