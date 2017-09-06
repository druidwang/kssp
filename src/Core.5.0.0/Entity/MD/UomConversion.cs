using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.MD
{
    public partial class UomConversion
    {
        #region Non O/R Mapping Properties
        [Export(ExportName = "UomConversion", ExportSeq = 50)]
        [Display(Name = "UomConvert_Item", ResourceType = typeof(Resources.MD.UomConvert))]
        public string ItemCode {get;set;}
        [Export(ExportName = "UomConversion", ExportSeq = 60)]
        [Display(Name = "Item_Description", ResourceType = typeof(Resources.MD.Item))]
        public string ItemDescription { get; set; }

        public decimal? Qty { get; set; }

        public bool IsAsc { get; set; }

        #endregion
    }
}