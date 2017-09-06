using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.INV
{
    public partial class ItemExchange
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.QualityType, ValueField = "QualityType")]
        [Display(Name = "ItemExchange_QualityType", ResourceType = typeof(Resources.INV.ItemExchange))]
        public string QualityTypeDescription { get; set; }
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ItemExchangeType, ValueField = "ItemExchangeType")]
        [Display(Name = "ItemExchange_ExchangeType", ResourceType = typeof(Resources.INV.ItemExchange))]
        public string ItemExchangeTypeDesc { get; set; }
        [Display(Name = "Item_Description", ResourceType = typeof(Resources.MD.Item))]
        public string ItemToDesc { get; set; }
        [Display(Name = "Item_Description", ResourceType = typeof(Resources.MD.Item))]
        public string ItemFromDesc { get; set; }

        public string UomFrom { get; set; }
        public string UomTo { get; set; }

        #endregion
    }
}