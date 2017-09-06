using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class PurchasePlan : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        [Display(Name = "PurchasePlan_Flow", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public string Flow { get; set; }
        [Display(Name = "PurchasePlan_Item", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public string Item { get; set; }
        [Display(Name = "PurchasePlan_LocationTo", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public string LocationTo { get; set; }
        [Display(Name = "PurchasePlan_Qty", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public double Qty { get; set; }
        [Display(Name = "PurchasePlan_PlanQty", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public double PlanQty { get; set; }
        [Display(Name = "PurchasePlan_WindowTime", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public DateTime WindowTime { get; set; }
        [Display(Name = "PurchasePlan_StartTime", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public DateTime StartTime { get; set; }
        [Display(Name = "PurchasePlan_OrderType", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        [Display(Name = "PurchasePlan_PlanVersion", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public DateTime PlanVersion { get; set; }
        [Display(Name = "PurchasePlan_DateType", ResourceType = typeof(Resources.MRP.PurchasePlan))]
        public Sconit.CodeMaster.TimeUnit DateType { get; set; }
     
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
            PurchasePlan another = obj as PurchasePlan;

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
