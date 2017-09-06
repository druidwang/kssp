using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
using System.Collections.Generic;
//TODO: Add other using statements here

namespace com.Sconit.Entity.INP
{
    public partial class RejectMaster
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.HandleResult, ValueField = "HandleResult")]
        [Display(Name = "RejectMaster_HandleResult", ResourceType = typeof(Resources.INP.RejectMaster))]
        public string RejectMasterHandleResultDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.RejectStatus, ValueField = "Status")]
        [Display(Name = "RejectMaster_Status", ResourceType = typeof(Resources.INP.RejectMaster))]
        public string RejectMasterStatusDescription { get; set; }

        public IList<RejectDetail> RejectDetails { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.InspectType, ValueField = "InspectType")]
        [Display(Name = "RejectMaster_Type", ResourceType = typeof(Resources.INP.RejectMaster))]
        public string InspectTypeDescription { get; set; }


        public string InspectNo { get; set; }
        #endregion

        public void AddRejectDetail(RejectDetail rejectDetail)
        {
            if (RejectDetails == null)
            {
                RejectDetails = new List<RejectDetail>();
            }

            RejectDetails.Add(rejectDetail);
        }
    }
}