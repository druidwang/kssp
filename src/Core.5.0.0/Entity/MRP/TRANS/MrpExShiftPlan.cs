using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    public partial class MrpExShiftPlan
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

        [Range(0, 1000000, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExShiftPlan_CurrentQty", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public double CurrentQty { get; set; }


        public MrpExOrder MrpExOrder { get; set; }

        public double CalShiftQty { get; set; }

        public double CorrectionQty { get; set; }

        [Export(ExportName = "ExportMrpExShiftPlan", ExportSeq = 160)]
        [Display(Name = "MrpExShiftPlan_CorrectionRate", ResourceType = typeof(Resources.MRP.MrpExShiftPlan))]
        public string CorrectionRate { get { return (this.Qty + this.CorrectionQty) > 0 ? (this.ReceivedQty * 100 / (this.Qty + this.CorrectionQty)).ToString("F2") + "ге" : string.Empty; } }
        

        #endregion
    }
}