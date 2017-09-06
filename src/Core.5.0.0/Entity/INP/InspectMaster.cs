using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.INP
{
    public partial class InspectMaster
    {
        #region Non O/R Mapping Properties
        public IList<InspectDetail> InspectDetails { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.InspectType, ValueField = "Type")]
        [Display(Name = "InspectMaster_Type", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string InspectTypeDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.InspectStatus, ValueField = "Status")]
        [Display(Name = "InspectMaster_Status", ResourceType = typeof(Resources.INP.InspectMaster))]
        public string InspectStatusDescription { get; set; }

        public void AddInspectDetail(InspectDetail inspectDetail)
        {
            if (InspectDetails == null)
            {
                InspectDetails = new List<InspectDetail>();
            }
            InspectDetails.Add(inspectDetail);
        }    
        #endregion
    }
}