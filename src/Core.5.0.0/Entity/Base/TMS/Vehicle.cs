using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class Vehicle : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Vehicle_Code", ResourceType = typeof(Resources.TMS.Vehicle))]
		public string Code { get; set; }

        [Display(Name = "Vehicle_Description", ResourceType = typeof(Resources.TMS.Vehicle))]
        public string Description { get; set; }

        [Display(Name = "Vehicle_DrivingNo", ResourceType = typeof(Resources.TMS.Vehicle))]
		public string DrivingNo { get; set; }

        [Display(Name = "Vehicle_Carrier", ResourceType = typeof(Resources.TMS.Vehicle))]
        public string Carrier { get; set; }

        [Display(Name = "Vehicle_Phone", ResourceType = typeof(Resources.TMS.Vehicle))]
		public string Phone { get; set; }

        [Display(Name = "Vehicle_MobilePhone", ResourceType = typeof(Resources.TMS.Vehicle))]
        public string MobilePhone { get; set; }

        [Display(Name = "Vehicle_VIN", ResourceType = typeof(Resources.TMS.Vehicle))]
        public string VIN { get; set; }

        [Display(Name = "Vehicle_EngineNo", ResourceType = typeof(Resources.TMS.Vehicle))]
		public string EngineNo { get; set; }

        [Display(Name = "Vehicle_Address", ResourceType = typeof(Resources.TMS.Vehicle))]
		public string Address { get; set; }

        [Display(Name = "Vehicle_Fax", ResourceType = typeof(Resources.TMS.Vehicle))]
		public string Fax { get; set; }

        [Display(Name = "Vehicle_Driver", ResourceType = typeof(Resources.TMS.Vehicle))]
		public string Driver { get; set; }


        [Display(Name = "Vehicle_Tonnage", ResourceType = typeof(Resources.TMS.Vehicle))]
		public string Tonnage { get; set; }

        [Display(Name = "Vehicle_CreateUserName", ResourceType = typeof(Resources.TMS.Vehicle))]
		public string CreateUserName { get; set; }
        public Int32 CreateUserId { get; set; }

        [Display(Name = "Vehicle_CreateDate", ResourceType = typeof(Resources.TMS.Vehicle))]
		public DateTime CreateDate { get; set; }
		public string LastModifyUserName { get; set; }
        public Int32 LastModifyUserId { get; set; }
		public DateTime LastModifyDate { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            Vehicle another = obj as Vehicle;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Code == another.Code);
            }
        } 
    }
	
}
