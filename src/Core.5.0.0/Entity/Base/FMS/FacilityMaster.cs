using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using System.ComponentModel.DataAnnotations;
using com.Sconit.CodeMaster;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class FacilityMaster : EntityBase,IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "FacilityMaster_FCID", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string FCID { get; set; }

        [Display(Name = "FacilityMaster_Name", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string Name { get; set; }

        [Display(Name = "FacilityMaster_Specification", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string Specification { get; set; }

        [Display(Name = "FacilityMaster_Capacity", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string Capacity { get; set; }

        [Display(Name = "FacilityMaster_ManufactureDate", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public DateTime? ManufactureDate { get; set; }

        [Display(Name = "FacilityMaster_Manufacturer", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string Manufacturer { get; set; }

        [Display(Name = "FacilityMaster_SerialNo", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string SerialNo { get; set; }

        [Display(Name = "FacilityMaster_AssetNo", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string AssetNo { get; set; }

        [Display(Name = "FacilityMaster_WarrantyInfo", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string WarrantyInfo { get; set; }

        [Display(Name = "FacilityMaster_TechInfo", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string TechInfo { get; set; }

        [Display(Name = "FacilityMaster_Supplier", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string Supplier { get; set; }

        [Display(Name = "FacilityMaster_SupplierInfo", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string SupplierInfo { get; set; }

        [Display(Name = "FacilityMaster_PONo", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string PONo { get; set; }

        [Display(Name = "FacilityMaster_EffDate", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string EffDate { get; set; }

        [Display(Name = "FacilityMaster_Price", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string Price { get; set; }

        [Display(Name = "FacilityMaster_Owner", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string Owner { get; set; }

        [Display(Name = "FacilityMaster_OwnerDescription", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string OwnerDescription { get; set; }

        [Display(Name = "FacilityMaster_Remark", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string Remark { get; set; }

        [Display(Name = "FacilityMaster_Status", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public FacilityStatus Status { get; set; }

        [Display(Name = "FacilityMaster_IsInStore", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public Boolean IsInStore { get; set; }

        public Int32 OldChargePersonId { get; set; }

        public Int32 CurrChargePersonId { get; set; }

        [Display(Name = "FacilityMaster_ChargeSite", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string ChargeSite { get; set; }

        [Display(Name = "FacilityMaster_ChargeOrganization", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string ChargeOrganization { get; set; }

        [Display(Name = "FacilityMaster_ChargeDate", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public DateTime? ChargeDate { get; set; }

        [Display(Name = "FacilityMaster_Category", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string Category { get; set; }

        [Display(Name = "FacilityMaster_OldChargePersonName", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string OldChargePersonName { get; set; }

        [Display(Name = "FacilityMaster_ChargePerson", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string CurrChargePersonName { get; set; }

        [Display(Name = "FacilityMaster_IsOffBalance", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public Boolean IsOffBalance { get; set; }

        [Display(Name = "FacilityMaster_IsAsset", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public Boolean IsAsset { get; set; }

        [Display(Name = "FacilityMaster_IsActive", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public Boolean IsActive { get; set; }

        [Display(Name = "FacilityMaster_RefenceCode", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string RefenceCode { get; set; }

        [Display(Name = "FacilityMaster_MaintainGroup", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string MaintainGroup { get; set; }

        [Display(Name = "FacilityMaster_PrintTemplate", ResourceType = typeof(Resources.FMS.FacilityMaster))]
        public string PrintTemplate { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }


        #endregion

        public override int GetHashCode()
        {
            if (FCID != null)
            {
                return FCID.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            FacilityMaster another = obj as FacilityMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.FCID == another.FCID);
            }
        }
    }

}
