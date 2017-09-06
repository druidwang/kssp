using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class ShipmentMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "ShipmentMaster_ShipmentNo", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public string ShipmentNo { get; set; }
        [Display(Name = "ShipmentMaster_VehicleNo", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public string VehicleNo { get; set; }
        public string WorkShop { get; set; }
         [Display(Name = "ShipmentMaster_AddressTo", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public string AddressTo { get; set; }
         [Display(Name = "ShipmentMaster_Driver", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
         public string Driver { get; set; }
        [Display(Name = "ShipmentMaster_CaseQty", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
         public Int32 CaseQty { get; set; }

        [Display(Name = "ShipmentMaster_Status", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public com.Sconit.CodeMaster.BillMasterStatus Status { get; set; }
        [Display(Name = "ShipmentMaster_Shipper", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public string Shipper { get; set; }
        public Int32 SubmitUserId { get; set; }
        [Display(Name = "ShipmentMaster_SubmitUserName", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public string SubmitUserName { get; set; }
        [Display(Name = "ShipmentMaster_SubmitDate", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public DateTime? SubmitDate { get; set; }
        [Display(Name = "ShipmentMaster_PassDate", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public DateTime? PassDate { get; set; }
        [Display(Name = "ShipmentMaster_PassPerson", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public string PassPerson { get; set; }

        public Int32 PassUserId { get; set; }


        public Int32 CreateUserId { get; set; }
        [Display(Name = "ShipmentMaster_CreateUserName", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public string CreateUserName { get; set; }
        [Display(Name = "ShipmentMaster_CreateDate", ResourceType = typeof(Resources.ORD.ShipmentMaster))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
  
        #endregion

        public override int GetHashCode()
        {
            if (ShipmentNo != null)
            {
                return ShipmentNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ShipmentMaster another = obj as ShipmentMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.ShipmentNo == another.ShipmentNo);
            }
        }
    }

}
