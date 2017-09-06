using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class VehicleInFactoryDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        [Display(Name = "VehicleInFactory_OrderNo", ResourceType = typeof(Resources.CUST.VehicleInFactoryDetail))]
        public string OrderNo { get; set; }
        [Display(Name = "VehicleInFactory_IpNo", ResourceType = typeof(Resources.CUST.VehicleInFactoryDetail))]
        public string IpNo { get; set; }
        [Display(Name = "VehicleInFactory_Dock", ResourceType = typeof(Resources.CUST.VehicleInFactoryDetail))]
        public string Dock { get; set; }
        [Display(Name = "VehicleInFactory_IsCose", ResourceType = typeof(Resources.CUST.VehicleInFactoryDetail))]
        public Boolean IsClose { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "VehicleInFactory_CreateUserName", ResourceType = typeof(Resources.CUST.VehicleInFactoryDetail))]
        public string CreateUserName { get; set; }

        [Display(Name = "VehicleInFactory_CreateDate", ResourceType = typeof(Resources.CUST.VehicleInFactoryDetail))]
        public DateTime CreateDate { get; set; }

        public Int32? CloseUserId { get; set; }

        [Display(Name = "VehicleInFactory_CloseUserName", ResourceType = typeof(Resources.CUST.VehicleInFactoryDetail))]
        public string CloseUserName { get; set; }

        [Display(Name = "VehicleInFactory_CloseDate", ResourceType = typeof(Resources.CUST.VehicleInFactoryDetail))]
        public DateTime? CloseDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        public string LastModifyUserName { get; set; }

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
            VehicleInFactoryDetail another = obj as VehicleInFactoryDetail;

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
