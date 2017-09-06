using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class DeliveryBarCode : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "DeliveryBarCode_BarCode", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string BarCode { get; set; }

        [Display(Name = "DeliveryBarCode_OrderNo", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string OrderNo { get; set; }

        [Display(Name = "DeliveryBarCode_OrderNo", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public Int32 OrderSequence { get; set; }

        [Display(Name = "DeliveryBarCode_StartTime", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public DateTime? StartTime { get; set; }

        [Display(Name = "DeliveryBarCode_WindowTime", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public DateTime? WindowTime { get; set; }

        [Display(Name = "DeliveryBarCode_Item", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string Item { get; set; }

        [Display(Name = "DeliveryBarCode_ItemDescription", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ItemDescription { get; set; }

        [Display(Name = "DeliveryBarCode_ReferenceItemCode", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ReferenceItemCode { get; set; }

        [Display(Name = "DeliveryBarCode_Uom", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string Uom { get; set; }

        [Display(Name = "DeliveryBarCode_UnitCount", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public Decimal UnitCount { get; set; }

        [Display(Name = "DeliveryBarCode_UnitCountDescription", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string UnitCountDescription { get; set; }

        [Display(Name = "DeliveryBarCode_Qty", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public Decimal Qty { get; set; }

        [Display(Name = "DeliveryBarCode_Priority", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public Int16 Priority { get; set; }

        [Display(Name = "DeliveryBarCode_PartyFrom", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string PartyFrom { get; set; }

        [Display(Name = "DeliveryBarCode_PartyFromName", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string PartyFromName { get; set; }

        [Display(Name = "DeliveryBarCode_PartyTo", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string PartyTo { get; set; }

        [Display(Name = "DeliveryBarCode_PartyToName", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string PartyToName { get; set; }

        [Display(Name = "DeliveryBarCode_ShipFrom", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipFrom { get; set; }

        [Display(Name = "DeliveryBarCode_ShipFromAddress", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipFromAddress { get; set; }

        [Display(Name = "DeliveryBarCode_ShipFromTel", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipFromTel { get; set; }

        [Display(Name = "DeliveryBarCode_ShipFromCell", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipFromCell { get; set; }

        [Display(Name = "DeliveryBarCode_ShipFromFax", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipFromFax { get; set; }

        [Display(Name = "DeliveryBarCode_ShipFromContact", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipFromContact { get; set; }

        [Display(Name = "DeliveryBarCode_ShipTo", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipTo { get; set; }

        [Display(Name = "DeliveryBarCode_ShipToAddress", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipToAddress { get; set; }

        [Display(Name = "DeliveryBarCode_ShipToTel", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipToTel { get; set; }

        [Display(Name = "DeliveryBarCode_ShipToCell", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipToCell { get; set; }

        [Display(Name = "DeliveryBarCode_ShipToFax", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipToFax { get; set; }

        [Display(Name = "DeliveryBarCode_ShipToContact", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string ShipToContact { get; set; }

        [Display(Name = "DeliveryBarCode_LocationFrom", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string LocationFrom { get; set; }

        [Display(Name = "DeliveryBarCode_LocationFromName", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string LocationFromName { get; set; }

        [Display(Name = "DeliveryBarCode_LocationTo", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string LocationTo { get; set; }

        [Display(Name = "DeliveryBarCode_LocationToName", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string LocationToName { get; set; }

        [Display(Name = "DeliveryBarCode_Station", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string Station { get; set; }

        [Display(Name = "DeliveryBarCode_Dock", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string Dock { get; set; }

        [Display(Name = "DeliveryBarCode_IsActive", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public Boolean? IsActive { get; set; }

        [Display(Name = "DeliveryBarCode_HuId", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string HuId { get; set; }
        public Int32 CreateUserId { get; set; }

        [Display(Name = "DeliveryBarCode_CreateUserName", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string CreateUserName { get; set; }

        [Display(Name = "DeliveryBarCode_CreateDate", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "DeliveryBarCode_LastModifyUserName", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "DeliveryBarCode_LastModifyDate", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public DateTime LastModifyDate { get; set; }
        public Int32 Version { get; set; }
        public Int32 ShipPlanId { get; set; }

        [Display(Name = "DeliveryBarCode_Flow", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public string Flow { get; set; }

        [Display(Name = "DeliveryBarCode_IsPickHu", ResourceType = typeof(Resources.WMS.DeliveryBarCode))]
        public Boolean IsPickHu { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (BarCode != null)
            {
                return BarCode.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            DeliveryBarCode another = obj as DeliveryBarCode;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.BarCode == another.BarCode);
            }
        }
    }

}
