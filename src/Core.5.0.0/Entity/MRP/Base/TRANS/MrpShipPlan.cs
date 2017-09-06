using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpShipPlan : EntityBase
    {
        #region O/R Mapping Properties

        public int Id { get; set; }

        [Display(Name = "MrpShipPlan_Flow", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string Flow { get; set; }
        [Display(Name = "MrpShipPlan_OrderType", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public CodeMaster.OrderType OrderType { get; set; }
        [Display(Name = "MrpShipPlan_Item", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string Item { get; set; }
        [Display(Name = "MrpShipPlan_StartTime", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public DateTime StartTime { get; set; }
        [Display(Name = "MrpShipPlan_WindowTime", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public DateTime WindowTime { get; set; }
        [Display(Name = "MrpShipPlan_LocationFrom", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string LocationFrom { get; set; }
        [Display(Name = "MrpShipPlan_LocationTo", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string LocationTo { get; set; }
        public int SourceId { get; set; }
        [Display(Name = "MrpShipPlan_Qty", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public double Qty { get; set; }
        [Display(Name = "MrpShipPlan_SourceType", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public CodeMaster.MrpSourceType SourceType { get; set; }

        [Display(Name = "MrpShipPlan_PlanVersion", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public DateTime PlanVersion { get; set; }
        public int GroupId { get; set; }
        [Display(Name = "MrpShipPlan_Bom", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string Bom { get; set; }//不一定等于Item
        [Display(Name = "MrpShipPlan_ParentItem", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string ParentItem { get; set; }
        [Display(Name = "MrpShipPlan_SourceFlow", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string SourceFlow { get; set; }//遇到生产线改变
        [Display(Name = "MrpShipPlan_SourceParty", ResourceType = typeof(Resources.MRP.MrpShipPlan))]
        public string SourceParty { get; set; }//遇到生产线改变

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
            MrpShipPlan another = obj as MrpShipPlan;

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
