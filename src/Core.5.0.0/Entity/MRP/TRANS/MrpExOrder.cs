using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpExOrder
    {
        [Display(Name = "MrpExOrder_SectionDescription", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public string SectionDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderStatus, ValueField = "Status")]
        [Display(Name = "MrpExOrder_Status", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public string OrderStatusDescription { get; set; }

        [Display(Name = "MrpExOrder_CurrentQty", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public double CurrentQty { get; set; }

        [Display(Name = "MrpExOrder_ScrapType", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        public string ScrapType { get; set; }
        //[Display(Name = "MrpExOrder_ScrapType", ResourceType = typeof(Resources.MRP.MrpExOrder))]
        //public string ScrapType { get; set; }
    }

}
