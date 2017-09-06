using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpShipPlanGroup : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        [Display(Name = "ShipPlanGroup_ProductLine", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public string Flow { get; set; }
        [Display(Name = "ShipPlanGroup_Item", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public string Item { get; set; }
        [Display(Name = "ShipPlanGroup_Qty", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public double Qty { get; set; }
        [Display(Name = "ShipPlanGroup_LocationFrom", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public string LocationFrom { get; set; }
        [Display(Name = "ShipPlanGroup_LocationTo", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public string LocationTo { get; set; }
        [Display(Name = "ShipPlanGroup_WindowTime", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public DateTime WindowTime { get; set; }
        [Display(Name = "ShipPlanGroup_StartTime", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public DateTime StartTime { get; set; }
        [Display(Name = "ShipPlanGroup_OrderType", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }

        public double ShipQty { get; set; }

        public bool IsDiscontinueItem { get; set; }
        [Display(Name = "ShipPlanGroup_PlanVersion", ResourceType = typeof(Resources.MRP.ShipPlanGroup))]
        public DateTime PlanVersion { get; set; }

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
            MrpShipPlanGroup another = obj as MrpShipPlanGroup;

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
