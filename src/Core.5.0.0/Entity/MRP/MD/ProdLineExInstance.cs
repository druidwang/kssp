using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    public partial class ProdLineExInstance
    {
        #region Non O/R Mapping Properties
        [Export(ExportName = "EXCalendar", ExportSeq = 60)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ShiftType, ValueField = "ShiftType")]
        [Display(Name = "ProdLineExInstance_ShiftType", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string ShiftTypeDescription { get; set; }

        [Export(ExportName = "EXCalendar", ExportSeq = 50)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TimeUnit, ValueField = "DateType")]
        [Display(Name = "ProdLineExInstance_DateType", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string DateTypeDescription { get; set; }

        [Export(ExportName = "EXCalendar", ExportSeq = 90)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ApsPriorityType, ValueField = "ApsPriority")]
        [Display(Name = "ProdLineExInstance_ApsPriority", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string ApsPriorityDescription { get; set; }
        [Export(ExportName = "EXCalendar", ExportSeq = 130)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ProductType, ValueField = "ProductType")]
        [Display(Name = "ProdLineExInstance_ProductType", ResourceType = typeof(Resources.MRP.ProdLineExInstance))]
        public string ProductTypeDescription { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProdLineEx_ItemDesc", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string ItemDesc { get; set; }
        public DateTime? DateIndexDate { get; set; }

        #endregion
    }
}