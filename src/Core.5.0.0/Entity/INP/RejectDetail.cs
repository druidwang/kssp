using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.INP
{
    public partial class RejectDetail
    {
        #region Non O/R Mapping Properties

        public bool IsCheck { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.InspectDefect, ValueField = "Defect")]
        [Display(Name = "RejectDetail_Defect", ResourceType = typeof(Resources.INP.RejectDetail))]
        public string DefectDescription { get; set; }

        [Display(Name = "RejectDetail_CurrentHandleQty", ResourceType = typeof(Resources.INP.RejectDetail))]
        public decimal CurrentHandleQty { get; set; }

        public decimal TobeHandleQty
        {
            get
            {
                return this.HandleQty - this.HandledQty;
            }
        }
        #endregion
    }
}