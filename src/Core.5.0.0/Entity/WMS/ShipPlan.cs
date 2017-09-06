using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.WMS
{
    public partial class ShipPlan
    {
        #region Non O/R Mapping Properties

        [Display(Name = "ShipPlan_DiaplayOrderQty", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string DiaplayOrderQty
        {
            get
            {
                return this.ShipQty.ToString("F0") + "/" + this.OrderQty.ToString("F0");
            }
        }

        [Display(Name = "ShipPlan_DiaplayPickQty", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string DiaplayPickQty
        {
            get
            {
                return this.PickedQty.ToString("F0") + "/" + this.PickQty.ToString("F0");
            }
        }


        public decimal ToPickQty
        {
            get
            {
                return this.OrderQty - this.PickQty;
            }
        }

          [Display(Name = "ShipPlan_ToDeliveryBarCodeQty", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public decimal ToDeliveryBarCodeQty { get; set; }

        #endregion
    }
}