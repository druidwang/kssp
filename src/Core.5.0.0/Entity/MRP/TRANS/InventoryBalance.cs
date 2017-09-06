using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class InventoryBalance
    {
        [Display(Name = "OrderDetail_ItemDescription", ResourceType = typeof(Resources.ORD.OrderDetail))]
        public string ItemDescription { get; set; }
        public Double ActiveQty
        {
            get
            {
                return Qty - MaxStock;
            }
        }
    }
	
}
