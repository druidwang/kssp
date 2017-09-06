using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpShipPlanGroup
    {
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "ShipPlanGroup_OrderType", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public string OrderTypeDescription { get; set; }

        [Display(Name = "ShipPlanGroup_ItemDescription", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public string ItemDescription { get; set; }

        public string ParentItem { get; set; }
        public string SourceFlow { get; set; }//遇到生产线改变
        public string SourceParty { get; set; }//遇到生产线改变
        public bool IsStockOver { get; set; }

        public List<MrpShipPlan> MrpShipPlanList { get; set; }
    }

}
