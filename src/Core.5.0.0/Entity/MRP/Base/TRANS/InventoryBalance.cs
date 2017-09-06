using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class InventoryBalance : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Display(Name = "InventoryBalance_Location", ResourceType = typeof(Resources.MRP.InventoryBalance))]
        public string Location { get; set; }
        [Display(Name = "InventoryBalance_Item", ResourceType = typeof(Resources.MRP.InventoryBalance))]
        public string Item { get; set; }
         [Display(Name = "InventoryBalance_Qty", ResourceType = typeof(Resources.MRP.InventoryBalance))]
        public Double Qty { get; set; }
        //public Double StaticQty { get; set; }
       [Display(Name = "InventoryBalance_SnapTime", ResourceType = typeof(Resources.MRP.InventoryBalance))]
        public DateTime SnapTime { get; set; }
        [Display(Name = "InventoryBalance_SafeStock", ResourceType = typeof(Resources.MRP.InventoryBalance))]
        public Double SafeStock { get; set; }
       [Display(Name = "InventoryBalance_MaxStock", ResourceType = typeof(Resources.MRP.InventoryBalance))]
        public Double MaxStock { get; set; }
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
            InventoryBalance another = obj as InventoryBalance;

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
