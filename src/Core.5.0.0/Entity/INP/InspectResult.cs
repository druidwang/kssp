using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.INP
{
    public partial class InspectResult
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.JudgeResult, ValueField = "JudgeResult")]
        [Display(Name = "InspectResult_JudgeResult", ResourceType = typeof(Resources.INP.InspectResult))]
        public string JudgeResultDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.InspectDefect, ValueField = "Defect")]
        [Display(Name = "InspectResult_Defect", ResourceType = typeof(Resources.INP.InspectResult))]
        public string DefectDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.HandleResult, ValueField = "RejectHandleResult")]
        [Display(Name = "InspectResult_RejectHandleResult", ResourceType = typeof(Resources.INP.InspectResult))]
        public string RejectHandleResultDescription { get; set; }

        [Display(Name = "InspectResult_FailCode", ResourceType = typeof(Resources.INP.InspectResult))]
        public string FailCodeDescription { get; set; }

        public bool IsCheck { get; set; }

        public decimal TobeHandleQty
        {
            get
            {
                return this.JudgeQty - this.HandleQty;
            }
        }

        [Display(Name = "InspectResult_CurrentHandleQty", ResourceType = typeof(Resources.INP.InspectResult))]
        public decimal CurrentHandleQty { get; set; }

        [Display(Name = "InspectResult_CurrentFailCode", ResourceType = typeof(Resources.INP.InspectResult))]
        public string CurrentFailCode { get; set; }

        [Display(Name = "InspectResult_IsHandle", ResourceType = typeof(Resources.INP.InspectResult))]
        public Boolean CurrentIsHandle { get; set; }

        [Display(Name = "InspectResult_ItemDesc", ResourceType = typeof(Resources.INP.InspectResult))]
        public string ItemFullDescription
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.ReferenceItemCode) ? this.ItemDescription : this.ItemDescription + "[" + this.ReferenceItemCode + "]";
            }
        }
        [Display(Name = "Hu_SupplierLotNo", ResourceType = typeof(Resources.INV.Hu))]
        public string SupplierLotNo { get; set; }
        [Display(Name = "Hu_ExpireDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? ExpireDate { get; set; }
        #endregion
    }
}