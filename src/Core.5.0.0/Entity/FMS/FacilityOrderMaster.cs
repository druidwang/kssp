using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    public partial class FacilityOrderMaster
    {
        #region Non O/R Mapping Properties
        public IList<FacilityOrderDetail> FacilityOrderDetails { get; set; }

      
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.FacilityOrderStatus, ValueField = "Status")]
        [Display(Name = "FacilityOrderMaster_Status", ResourceType = typeof(Resources.FMS.FacilityOrderMaster))]
        public string FacilityOrderStatusDescription { get; set; }

        public void AddFacilityOrderDetail(FacilityOrderDetail facilityOrderDetail)
        {
            if (FacilityOrderDetails == null)
            {
                FacilityOrderDetails = new List<FacilityOrderDetail>();
            }
            FacilityOrderDetails.Add(facilityOrderDetail);
        }    
        #endregion
    }
}