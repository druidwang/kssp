using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class RepackTask : EntityBase
    {
        #region O/R Mapping Properties

        public string UUID { get; set; }

        public Int32 Id { get; set; }

        [Display(Name = "RepackTask_Item", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string Item { get; set; }

        [Display(Name = "RepackTask_ItemDescription", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string ItemDescription { get; set; }

        [Display(Name = "RepackTask_ReferenceItemCode", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string ReferenceItemCode { get; set; }

        [Display(Name = "RepackTask_Uom", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string Uom { get; set; }

        [Display(Name = "RepackTask_BaseUom", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string BaseUom { get; set; }

        [Display(Name = "RepackTask_UnitQty", ResourceType = typeof(Resources.WMS.RepackTask))]
        public Decimal UnitQty { get; set; }

        [Display(Name = "RepackTask_UnitCount", ResourceType = typeof(Resources.WMS.RepackTask))]
        public Decimal UnitCount { get; set; }

        [Display(Name = "RepackTask_UCDescription", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string UCDescription { get; set; }

        [Display(Name = "RepackTask_Qty", ResourceType = typeof(Resources.WMS.RepackTask))]
        public Decimal Qty { get; set; }

        [Display(Name = "RepackTask_RepackQty", ResourceType = typeof(Resources.WMS.RepackTask))]
        public Decimal RepackQty { get; set; }

        [Display(Name = "RepackTask_Location", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string Location { get; set; }
        public CodeMaster.OrderPriority Priority { get; set; }
        public string RepackGroup { get; set; }
        public Int32? RepackUserId { get; set; }

        [Display(Name = "RepackTask_RepackUserName", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string RepackUserName { get; set; }

        [Display(Name = "RepackTask_StartTime", ResourceType = typeof(Resources.WMS.RepackTask))]
        public DateTime StartTime { get; set; }

        [Display(Name = "RepackTask_WindowTime", ResourceType = typeof(Resources.WMS.RepackTask))]
        public DateTime WindowTime { get; set; }

        [Display(Name = "RepackTask_IsActive", ResourceType = typeof(Resources.WMS.RepackTask))]
        public Boolean IsActive { get; set; }
        public Int32 CreateUserId { get; set; }

        [Display(Name = "RepackTask_CreateUserName", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string CreateUserName { get; set; }

        [Display(Name = "RepackTask_CreateDate", ResourceType = typeof(Resources.WMS.RepackTask))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Int32? CloseUser { get; set; }

        [Display(Name = "RepackTask_CloseUserName", ResourceType = typeof(Resources.WMS.RepackTask))]
        public string CloseUserName { get; set; }

        [Display(Name = "RepackTask_CloseDate", ResourceType = typeof(Resources.WMS.RepackTask))]
        public DateTime? CloseDate { get; set; }
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
            RepackTask another = obj as RepackTask;

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
