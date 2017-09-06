using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.INV
{
    public partial class StockTakeMaster
    {
        #region Non O/R Mapping Properties
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.StockTakeType, ValueField = "Type")]
        [Display(Name = "StockTakeMaster_Type", ResourceType = typeof(Resources.INV.StockTake))]
        public string TypeDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ValueField = "Status")]
        [Display(Name = "StockTakeMaster_Status", ResourceType = typeof(Resources.INV.StockTake))]
        public string StockTakeStatusDescription { get; set; }

        public IList<StockTakeDetail> StockTakeDetails { get; set; }

        public IList<StockTakeResult> StockTakeResults { get; set; }

        public IList<StockTakeItem> StockTakeItems { get; set; }

        public IList<StockTakeLocation> StockTakeLocations { get; set; }
        #endregion

        
    }
}