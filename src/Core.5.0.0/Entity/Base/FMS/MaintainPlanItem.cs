using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class MaintainPlanItem : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

   
        public Int32 Id { get; set; }
        [Display(Name = "MaintainPlanCode", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public string MaintainPlanCode { get; set; }
        [Display(Name = "MaintainPlanItem_Sequence", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public Int32 Sequence { get; set; }
   
        [Display(Name = "MaintainPlanItem_Item", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public string Item { get; set; }
        [Display(Name = "MaintainPlanItem_ItemDesc", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public string ItemDescription { get; set; }


     
        [Display(Name = "MaintainPlanItem_LocationFrom", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public string LocationFrom { get; set; }

        [Display(Name = "MaintainPlanItem_Uom", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public string Uom { get; set; }
        public string BaseUom { get; set; }


        [Display(Name = "MaintainPlanItem_Qty", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public Decimal Qty { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "MaintainPlanItem_CreateUserName", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public string CreateUserName { get; set; }

        [Display(Name = "MaintainPlanItem_CreateDate", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "InspectMaster_LastModifyUserNm", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "InspectMaster_LastModifyDate", ResourceType = typeof(Resources.FMS.MaintainPlanItem))]
        public DateTime LastModifyDate { get; set; }

   
      
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
            MaintainPlanItem another = obj as MaintainPlanItem;

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
