using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    public partial class ProdLineEx
    {
        #region Non O/R Mapping Properties
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ShiftType, ValueField = "ShiftType")]
        [Display(Name = "ProdLineEx_ShiftType", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string ShiftTypeDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ApsPriorityType, ValueField = "ApsPriority")]
        [Display(Name = "ProdLineEx_ApsPriority", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string ApsPriorityDescription { get; set; }
        //TODO: Add Non O/R Mapping Properties here. 

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ProductType, ValueField = "ProductType")]
        [Display(Name = "ProdLineEx_ProductType", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string ProductTypeDescription { get; set; }

        [Display(Name = "ProdLineEx_ItemDesc", ResourceType = typeof(Resources.MRP.ProdLineEx))]
        public string ItemDesc { get; set; }
        #endregion
    }
}