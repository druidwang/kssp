using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    [Serializable]
    public partial class ActiveOrder : EntityBase
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        [Display(Name = "ActiveOrder_OrderNo", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public string OrderNo { get; set; }
        public Int32 OrderDetId { get; set; }
        [Display(Name = "ActiveOrder_Flow", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public string Flow { get; set; }
        public CodeMaster.OrderType OrderType { get; set; }
        [Display(Name = "ActiveOrder_LocationFrom", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public string LocationFrom { get; set; }
        [Display(Name = "ActiveOrder_LocationTo", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public string LocationTo { get; set; }
        [Display(Name = "ActiveOrder_Item", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public string Item { get; set; }
        [Display(Name = "ActiveOrder_StartTime", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public DateTime StartTime { get; set; }
        [Display(Name = "ActiveOrder_WindowTime", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public DateTime WindowTime { get; set; }
        [Display(Name = "ActiveOrder_OrderQty", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public Double OrderQty { get; set; }
        [Display(Name = "ActiveOrder_ShippedQty", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public Double ShippedQty { get; set; }
        [Display(Name = "ActiveOrder_ReceivedQty", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public Double ReceivedQty { get; set; }
        //public Decimal TransitQty { get; set; }
        [Display(Name = "ActiveOrder_SnapTime", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public DateTime SnapTime { get; set; }

        [Display(Name = "ActiveOrder_PartyTo", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public string PartyTo { get; set; }
        [Display(Name = "ActiveOrder_PartyFrom", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public string PartyFrom { get; set; }
        [Display(Name = "ActiveOrder_IsIndepentDemand", ResourceType = typeof(Resources.MRP.ActiveOrder))]
        public Boolean IsIndepentDemand { get; set; }

        public CodeMaster.ResourceGroup ResourceGroup { get; set; }
        public string Shift { get; set; }
        public string Bom { get; set; }
        #endregion

    }

}
