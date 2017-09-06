using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class FacilityTrans : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        [Display(Name = "FacilityTrans_FCID", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string FCID { get; set; }

        [Display(Name = "FacilityTrans_TransType", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public CodeMaster.FacilityTransType TransType { get; set; }
        public Int32 FromChargePersonId { get; set; }
        public Int32 ToChargePersonId { get; set; }

        [Display(Name = "FacilityTrans_FromOrganization", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string FromOrganization { get; set; }

        [Display(Name = "FacilityTrans_ToOrganization", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string ToOrganization { get; set; }

        [Display(Name = "FacilityTrans_Remark", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string Remark { get; set; }

        [Display(Name = "FacilityTrans_EffDate", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public DateTime? EffDate { get; set; }

        [Display(Name = "FacilityTrans_Attachment", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string Attachment { get; set; }

        [Display(Name = "FacilityTrans_Category", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string FacilityCategory { get; set; }

        [Display(Name = "FacilityTrans_FromChargeSite", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string FromChargeSite { get; set; }

        [Display(Name = "FacilityTrans_ToChargeSite", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string ToChargeSite { get; set; }

        [Display(Name = "FacilityTrans_FromChargePerson", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string FromChargePersonName { get; set; }

        [Display(Name = "FacilityTrans_ToChargePerson", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string ToChargePersonName { get; set; }

        [Display(Name = "FacilityTrans_StartDate", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public DateTime? StartDate { get; set; }

        [Display(Name = "FacilityTrans_EndDate", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public DateTime? EndDate { get; set; }

        [Display(Name = "FacilityTrans_AssetNo", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string AssetNo { get; set; }

        [Display(Name = "FacilityTrans_FacilityName", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string FacilityName { get; set; }


        public string BatchNo { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "FacilityTrans_CreateUser", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public string CreateUserName { get; set; }

        [Display(Name = "FacilityTrans_CreateDate", ResourceType = typeof(Resources.FMS.FacilityTrans))]
        public DateTime CreateDate { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Id != 0)
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
            FacilityTrans another = obj as FacilityTrans;

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
