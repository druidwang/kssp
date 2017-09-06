using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class FacilityOrderDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

   
        public Int32 Id { get; set; }
        [Display(Name = "FacilityOrderNo", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string FacilityOrderNo { get; set; }
        [Display(Name = "FacilityOrderDetail_Sequence", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public Int32 Sequence { get; set; }
   
        [Display(Name = "FacilityOrderDetail_Item", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string Item { get; set; }
        [Display(Name = "FacilityOrderDetail_ItemDesc", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string ItemDescription { get; set; }


        [Display(Name = "FacilityOrderDetail_FacilityId", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string FacilityId { get; set; }
        [Display(Name = "FacilityOrderDetail_FacilityName", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string FacilityName { get; set; }
        [Display(Name = "FacilityOrderDetail_LocationFrom", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string LocationFrom { get; set; }

        [Display(Name = "FacilityOrderDetail_Uom", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string Uom { get; set; }
        public string BaseUom { get; set; }


        [Display(Name = "FacilityOrderDetail_PlanQty", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public Decimal PlanQty { get; set; }

        [Display(Name = "FacilityOrderDetail_ActQty", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public Decimal ActualQty { get; set; }

   
        public Int32 CreateUserId { get; set; }

        [Display(Name = "FacilityOrderDetail_CreateUserName", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string CreateUserName { get; set; }

        [Display(Name = "FacilityOrderDetail_CreateDate", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "InspectMaster_LastModifyUserNm", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "InspectMaster_LastModifyDate", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public DateTime LastModifyDate { get; set; }

        public Int32 Version { get; set; }
      
        [Display(Name = "FacilityOrderDetail_Note", ResourceType = typeof(Resources.FMS.FacilityOrderDetail))]
        public string Note { get; set; }
      
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
            FacilityOrderDetail another = obj as FacilityOrderDetail;

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
