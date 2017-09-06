using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class Driver : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Driver_Code", ResourceType = typeof(Resources.TMS.Driver))]
        public string Code { get; set; }

        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "Driver_Name", ResourceType = typeof(Resources.TMS.Driver))]
        public string Name { get; set; }

        [Display(Name = "Driver_PostCode", ResourceType = typeof(Resources.TMS.Driver))]
        public string PostCode { get; set; }

        [Display(Name = "Driver_Phone", ResourceType = typeof(Resources.TMS.Driver))]
        public string Phone { get; set; }

        [Display(Name = "Driver_MobilePhone", ResourceType = typeof(Resources.TMS.Driver))]
        public string MobilePhone { get; set; }

        [Display(Name = "Driver_Email", ResourceType = typeof(Resources.TMS.Driver))]
        public string Email { get; set; }

        [Display(Name = "Driver_Fax", ResourceType = typeof(Resources.TMS.Driver))]
        public string Fax { get; set; }

        [Display(Name = "Driver_IdNumber", ResourceType = typeof(Resources.TMS.Driver))]
        public string IdNumber { get; set; }

        [Display(Name = "Driver_Address", ResourceType = typeof(Resources.TMS.Driver))]
        public string Address { get; set; }

        public string LicenseNo { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "Driver_CreateUserName", ResourceType = typeof(Resources.TMS.Driver))]
        public string CreateUserName { get; set; }
        [Display(Name = "Driver_CreateDate", ResourceType = typeof(Resources.TMS.Driver))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        public string LastModifyUserName { get; set; }

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
            Driver another = obj as Driver;

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
