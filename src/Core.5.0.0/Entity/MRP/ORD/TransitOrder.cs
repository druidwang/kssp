using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    public partial class TransitOrder
    {
        public Double Qty { get; set; }
        public Double TransitQty
        {
            get
            {
                return this.ShippedQty - this.ReceivedQty > 0 ? this.ShippedQty - this.ReceivedQty : 0;
            }
        }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "TransitOrder_OrderType", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public string OrderTypeDescription { get; set; }
        [Display(Name = "OrderDetail_ItemDescription", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ItemDescription { get; set; }
    }

}
