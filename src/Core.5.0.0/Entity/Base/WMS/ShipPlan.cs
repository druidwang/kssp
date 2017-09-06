using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class ShipPlan : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "ShipPlan_Id", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Int32 Id { get; set; }

        [Display(Name = "ShipPlan_Flow", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string Flow { get; set; }

        [Display(Name = "ShipPlan_OrderNo", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string OrderNo { get; set; }

        [Display(Name = "ShipPlan_OrderSequence", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Int32 OrderSequence { get; set; }
        public Int32 OrderDetailId { get; set; }

        [Display(Name = "ShipPlan_StartTime", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public DateTime StartTime { get; set; }

        [Display(Name = "ShipPlan_WindowTime", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public DateTime WindowTime { get; set; }

        [Display(Name = "ShipPlan_Item", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string Item { get; set; }

        [Display(Name = "ShipPlan_ItemDescription", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ItemDescription { get; set; }

        [Display(Name = "ShipPlan_ReferenceItemCode", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ReferenceItemCode { get; set; }

        [Display(Name = "ShipPlan_Uom", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string Uom { get; set; }

        [Display(Name = "ShipPlan_BaseUom", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string BaseUom { get; set; }

        [Display(Name = "ShipPlan_UnitQty", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Decimal UnitQty { get; set; }

        [Display(Name = "ShipPlan_UnitCount", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Decimal UnitCount { get; set; }

        [Display(Name = "ShipPlan_UnitCountDescription", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string UnitCountDescription { get; set; }

        [Display(Name = "ShipPlan_OrderQty", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Decimal OrderQty { get; set; }

        [Display(Name = "ShipPlan_ShipQty", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Decimal ShipQty { get; set; }

        public Decimal LockQty { get; set; }

        [Display(Name = "ShipPlan_PickQty", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Decimal PickQty { get; set; }

        [Display(Name = "ShipPlan_PickedQty", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Decimal PickedQty { get; set; }

        [Display(Name = "ShipPlan_Priority", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public com.Sconit.CodeMaster.OrderPriority Priority { get; set; }

        public string PartyFrom { get; set; }

        [Display(Name = "ShipPlan_PartyFromName", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string PartyFromName { get; set; }

        public string PartyTo { get; set; }

        [Display(Name = "ShipPlan_PartyToName", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string PartyToName { get; set; }

        [Display(Name = "ShipPlan_ShipFrom", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipFrom { get; set; }

        [Display(Name = "ShipPlan_ShipFromAddress", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipFromAddress { get; set; }

        [Display(Name = "ShipPlan_ShipFromTel", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipFromTel { get; set; }


        [Display(Name = "ShipPlan_ShipFromCell", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipFromCell { get; set; }

        [Display(Name = "ShipPlan_ShipFromFax", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipFromFax { get; set; }

        [Display(Name = "ShipPlan_ShipFromContact", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipFromContact { get; set; }

        [Display(Name = "ShipPlan_ShipTo", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipTo { get; set; }

        [Display(Name = "ShipPlan_ShipToAddress", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipToAddress { get; set; }

        [Display(Name = "ShipPlan_ShipToTel", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipToTel { get; set; }

        [Display(Name = "ShipPlan_ShipToCell", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipToCell { get; set; }

        [Display(Name = "ShipPlan_ShipToFax", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipToFax { get; set; }

        [Display(Name = "ShipPlan_ShipToContact", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipToContact { get; set; }

        [Display(Name = "ShipPlan_LocationFrom", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string LocationFrom { get; set; }

        [Display(Name = "ShipPlan_LocationFromName", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string LocationFromName { get; set; }

        [Display(Name = "ShipPlan_Dock", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string Dock { get; set; }

        [Display(Name = "ShipPlan_IsOccupyInventory", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Boolean IsOccupyInventory { get; set; }

        [Display(Name = "ShipPlan_IsShipScanHu", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Boolean IsShipScanHu { get; set; }

        [Display(Name = "ShipPlan_IsActive", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public Boolean IsActive { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "ShipPlan_CreateUserName", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string CreateUserName { get; set; }

        [Display(Name = "ShipPlan_CreateDate", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "ShipPlan_LastModifyUserName", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "ShipPlan_LastModifyDate", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public DateTime LastModifyDate { get; set; }
        public Int32? CloseUserId { get; set; }

        [Display(Name = "ShipPlan_CloseUserName", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string CloseUserName { get; set; }

        [Display(Name = "ShipPlan_CloseDate", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public DateTime? CloseDate { get; set; }

        public Int32? ShipUserId { get; set; }

        [Display(Name = "ShipPlan_ShipUserName", ResourceType = typeof(Resources.WMS.ShipPlan))]
        public string ShipUserName { get; set; }

        public Int32 Version { get; set; }

        public string LocationTo { get; set; }
        public string LocationToName { get; set; }
        public string Station { get; set; }
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }

        public Decimal PackQty { get; set; }
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
            ShipPlan another = obj as ShipPlan;

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
