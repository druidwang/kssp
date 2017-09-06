using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.INV
{
    public partial class StockTakeDetail
    {
        #region Non O/R Mapping Properties

        //已经存在老的记录
        public decimal? OldQty { get; set; }

        /// <summary>
        /// null new, true update, false delete
        /// </summary>
        public bool? IsUpdate { get; set; }

        //TODO: Add Non O/R Mapping Properties here. 
        [Export(ExportName = "StokcTakeDetailsScanHu", ExportSeq = 50)]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.QualityType, ValueField = "QualityType")]
        [Display(Name = "ItemExchange_QualityType", ResourceType = typeof(Resources.INV.ItemExchange))]
        public string QualityTypeDescription { get; set; }
        #endregion
    }
}