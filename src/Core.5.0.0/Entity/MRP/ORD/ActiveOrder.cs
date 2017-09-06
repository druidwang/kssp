using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    public partial class ActiveOrder
    {
        public Double DemandQty
        {
            get
            {
                return this.OrderQty - this.ShippedQty > 0 ? this.OrderQty - this.ShippedQty : 0;
            }
        }
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "ActiveOrder_OrderType", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public string OrderTypeDescription { get; set; }
        [Display(Name = "OrderDetail_ItemDescription", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ItemDescription { get; set; }
    }

}
