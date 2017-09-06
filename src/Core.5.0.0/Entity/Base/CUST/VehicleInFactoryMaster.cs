using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class VehicleInFactoryMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "VehicleInFactory_OrderNo", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public string OrderNo { get; set; }
        [Display(Name = "VehicleInFactory_VehicleNo", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public string VehicleNo { get; set; }
        [Display(Name = "VehicleInFactory_Status", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public com.Sconit.CodeMaster.VehicleInFactoryStatus Status { get; set; }
        [Display(Name = "VehicleInFactory_Plant", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public string Plant { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "VehicleInFactory_CreateUserName", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public string CreateUserName { get; set; }

        [Display(Name = "VehicleInFactory_CreateDate", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public DateTime CreateDate { get; set; }

        public Int32? CloseUserId { get; set; }

        [Display(Name = "VehicleInFactory_CloseUserName", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public string CloseUserName { get; set; }

        [Display(Name = "VehicleInFactory_CloseDate", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public DateTime? CloseDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "VehicleInFactory_LastModifyUserName", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "VehicleInFactory_LastModifyDate", ResourceType = typeof(Resources.CUST.VehicleInFactoryMaster))]
        public DateTime LastModifyDate { get; set; }

        #endregion

        public override int GetHashCode()
        {
            if (OrderNo != null)
            {
                return OrderNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            VehicleInFactoryMaster another = obj as VehicleInFactoryMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.OrderNo == another.OrderNo);
            }
        }
    }

}
