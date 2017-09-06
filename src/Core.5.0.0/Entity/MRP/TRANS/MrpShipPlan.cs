using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.MRP.MD;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpShipPlan
    {

        [Display(Name = "MrpShipPlan_ItemDescription", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string ItemDescription { get; set; }
        //public string Location { get; set; }
        [Display(Name = "MrpShipPlan_OrderNo", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string OrderNo { get; set; }

        public MrpFlowDetail MrpFlowDetail { get; set; }
    }

}
