using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    [Serializable]
    public partial class TransitOrder : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Display(Name = "TransitOrder_OrderNo", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public string OrderNo { get; set; }
        public Int32 OrderDetailId { get; set; }
        [Display(Name = "TransitOrder_IpNo", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public string IpNo { get; set; }
        [Display(Name = "TransitOrder_Flow", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public string Flow { get; set; }
        [Display(Name = "TransitOrder_OrderType", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        [Display(Name = "TransitOrder_Location", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public string Location { get; set; }
        [Display(Name = "TransitOrder_Item", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public string Item { get; set; }
        [Display(Name = "TransitOrder_ShippedQty", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public Double ShippedQty { get; set; }
        [Display(Name = "TransitOrder_ReceivedQty", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public Double ReceivedQty { get; set; }
        [Display(Name = "TransitOrder_StartTime", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public DateTime StartTime { get; set; }
        [Display(Name = "TransitOrder_WindowTime", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public DateTime WindowTime { get; set; }
        [Display(Name = "TransitOrder_SnapTime", ResourceType = typeof(Resources.MRP.TransitOrder))]
        public DateTime SnapTime { get; set; }
        //public string PartyTo { get; set; }
        //public string Uom { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Id != 0)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            TransitOrder another = obj as TransitOrder;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }
    }

}
