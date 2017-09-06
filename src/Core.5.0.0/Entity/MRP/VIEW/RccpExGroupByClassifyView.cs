using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MRP.TRANS;

namespace com.Sconit.Entity.MRP.VIEW
{
    public class RccpExGroupByClassifyView
    {
        public string Classify { get; set; }
        public string DateIndex { get; set; }

        public Double Qty { get; set; }

        public string Css { get; set; }

        //¸¨Öú×Ö¶Î
        public IEnumerable<RccpExView> RccpExViews { get; set; }
    }
}
