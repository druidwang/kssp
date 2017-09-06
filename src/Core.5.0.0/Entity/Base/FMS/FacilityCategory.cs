using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class FacilityCategory : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [StringLength(50, ErrorMessageResourceName = "Errors_Common_FieldLengthExceed", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "FacilityCategory_Code", ResourceType = typeof(Resources.FMS.FacilityCategory))]
        public string Code { get; set; }

        [Display(Name = "FacilityCategory_ChargeOrganization", ResourceType = typeof(Resources.FMS.FacilityCategory))]
        public string ChargeOrganization { get; set; }

        [Display(Name = "FacilityCategory_ChargeSite", ResourceType = typeof(Resources.FMS.FacilityCategory))]
        public string ChargeSite { get; set; }
        public Int32 ChargePersonId { get; set; }

        [Display(Name = "FacilityCategory_Description", ResourceType = typeof(Resources.FMS.FacilityCategory))]
        public string Description { get; set; }

        [Display(Name = "FacilityCategory_ChargePerson", ResourceType = typeof(Resources.FMS.FacilityCategory))]
        public string ChargePersonName { get; set; }

        [Display(Name = "FacilityCategory_ParentCategory", ResourceType = typeof(Resources.FMS.FacilityCategory))]
        public string ParentCategory { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
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
            FacilityCategory another = obj as FacilityCategory;

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
