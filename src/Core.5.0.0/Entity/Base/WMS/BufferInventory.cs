using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.CodeMaster;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class BufferInventory : EntityBase
    {
        #region O/R Mapping Properties

        public string UUID { get; set; }

        [Display(Name = "BufferInventory_Location", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public string Location { get; set; }

        [Display(Name = "BufferInventory_Dock", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public string Dock { get; set; }

        [Display(Name = "BufferInventory_IOType", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public IOType IOType { get; set; }

        [Display(Name = "BufferInventory_Item", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public string Item { get; set; }

        [Display(Name = "BufferInventory_Uom", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public string Uom { get; set; }


        [Display(Name = "BufferInventory_UnitCount", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public Decimal UnitCount { get; set; }


        [Display(Name = "BufferInventory_Qty", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public Decimal Qty { get; set; }


        [Display(Name = "BufferInventory_LotNo", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public string LotNo { get; set; }


        [Display(Name = "BufferInventory_HuId", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public string HuId { get; set; }


        [Display(Name = "BufferInventory_IsLock", ResourceType = typeof(Resources.WMS.BufferInventory))]
        public Boolean IsLock { get; set; }

        public Boolean IsPack { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32 Version { get; set; }

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
            BufferInventory another = obj as BufferInventory;

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
