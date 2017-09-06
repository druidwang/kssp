using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    public partial class MrpPlanLog
    {
        public string Party { get; set; }
        public CodeMaster.OrderType OrderType { get; set; }
    }

}
