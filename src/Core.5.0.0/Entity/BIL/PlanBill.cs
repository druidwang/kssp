using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.BIL
{
    public partial class PlanBill
    {
        #region Non O/R Mapping Properties

        public decimal CurrentVoidQty { get; set; }
        [Export(ExportName = "PlanBill", ExportSeq = 60)]
        [Display(Name = "PlanBill_CurrentActingQty", ResourceType = typeof(Resources.BIL.PlanBill))]
        public decimal CurrentActingQty { get; set; }
        public string CurrentLocation { get; set; }
        public string CurrentHuId { get; set; }
        [Export(ExportName = "PlanBill", ExportSeq = 70)]
        [Display(Name = "PlanBill_CurrentQty", ResourceType = typeof(Resources.BIL.PlanBill))]
        public decimal CurrentQty { get; set; }
        [Export(ExportName = "PlanBill", ExportSeq = 23)]
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }
        [Export(ExportName = "PlanBill", ExportSeq = 26)]
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }

        #endregion
    }
}