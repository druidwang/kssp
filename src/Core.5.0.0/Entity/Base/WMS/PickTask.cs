using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class PickTask : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public string UUID { get; set; }


        [Display(Name = "PickTask_Id", ResourceType = typeof(Resources.WMS.PickTask))]
        public Int32 Id { get; set; }

        public com.Sconit.CodeMaster.OrderPriority Priority { get; set; }

        [Display(Name = "PickTask_Item", ResourceType = typeof(Resources.WMS.PickTask))]
        public string Item { get; set; }

        [Display(Name = "PickTask_ItemDescription", ResourceType = typeof(Resources.WMS.PickTask))]
        public string ItemDescription { get; set; }

        [Display(Name = "PickTask_ReferenceItemCode", ResourceType = typeof(Resources.WMS.PickTask))]
        public string ReferenceItemCode { get; set; }

        [Display(Name = "PickTask_Uom", ResourceType = typeof(Resources.WMS.PickTask))]
        public string Uom { get; set; }

        [Display(Name = "PickTask_BaseUom", ResourceType = typeof(Resources.WMS.PickTask))]
        public string BaseUom { get; set; }

        [Display(Name = "PickTask_UnitQty", ResourceType = typeof(Resources.WMS.PickTask))]
        public Decimal UnitQty { get; set; }

        [Display(Name = "PickTask_UnitCount", ResourceType = typeof(Resources.WMS.PickTask))]
        public Decimal UnitCount { get; set; }

        [Display(Name = "PickTask_UCDescription", ResourceType = typeof(Resources.WMS.PickTask))]
        public string UCDescription { get; set; }

        [Display(Name = "PickTask_OrderQty", ResourceType = typeof(Resources.WMS.PickTask))]
        public Decimal OrderQty { get; set; }

        [Display(Name = "PickTask_PickQty", ResourceType = typeof(Resources.WMS.PickTask))]
        public Decimal PickQty { get; set; }

        [Display(Name = "PickTask_Location", ResourceType = typeof(Resources.WMS.PickTask))]
        public string Location { get; set; }

        [Display(Name = "PickTask_Area", ResourceType = typeof(Resources.WMS.PickTask))]
        public string Area { get; set; }

        [Display(Name = "PickTask_Bin", ResourceType = typeof(Resources.WMS.PickTask))]
        public string Bin { get; set; }

        [Display(Name = "PickTask_LotNo", ResourceType = typeof(Resources.WMS.PickTask))]
        public string LotNo { get; set; }

        [Display(Name = "PickTask_HuId", ResourceType = typeof(Resources.WMS.PickTask))]
        public string HuId { get; set; }

        public Boolean NeedRepack { get; set; }
        public Boolean IsPickHu { get; set; }

        [Display(Name = "PickTask_PickBy", ResourceType = typeof(Resources.WMS.PickTask))]
        public com.Sconit.CodeMaster.PickBy PickBy { get; set; }

        [Display(Name = "PickTask_PickGroup", ResourceType = typeof(Resources.WMS.PickTask))]
        public string PickGroup { get; set; }
        public Int32? PickUserId { get; set; }

        [Display(Name = "PickTask_PickUserName", ResourceType = typeof(Resources.WMS.PickTask))]
        public string PickUserName { get; set; }


        [Display(Name = "PickTask_StartTime", ResourceType = typeof(Resources.WMS.PickTask))]
        public DateTime StartTime { get; set; }

        [Display(Name = "PickTask_WindowTime", ResourceType = typeof(Resources.WMS.PickTask))]
        public DateTime WinTime { get; set; }

        [Display(Name = "PickTask_IsActive", ResourceType = typeof(Resources.WMS.PickTask))]
        public Boolean IsActive { get; set; }
        public Int32 CreateUserId { get; set; }

        [Display(Name = "PickTask_CreateUserName", ResourceType = typeof(Resources.WMS.PickTask))]
        public string CreateUserName { get; set; }

        [Display(Name = "PickTask_CreateDate", ResourceType = typeof(Resources.WMS.PickTask))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32? CloseUser { get; set; }

        [Display(Name = "PickTask_CloseUserName", ResourceType = typeof(Resources.WMS.PickTask))]
        public string CloseUserName { get; set; }

        [Display(Name = "PickTask_CloseDate", ResourceType = typeof(Resources.WMS.PickTask))]
        public DateTime? CloseDate { get; set; }
        public Int32 Version { get; set; }
        public Decimal ShipUnitCount { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (UUID != null)
            {
                return UUID.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            PickTask another = obj as PickTask;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.UUID == another.UUID);
            }
        }
    }

}
