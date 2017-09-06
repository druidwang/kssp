using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.BIL
{
    public partial class PriceListMaster
    {
        #region Non O/R Mapping Properties

        
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.PriceListType, ValueField = "Type")]
        [Display(Name = "PriceListMaster_Type", ResourceType = typeof(Resources.BIL.PriceListMaster))]
        public string PriceListTypeDescription { get; set; }
        [Display(Name = "PriceListMaster_DistributionPartyName", ResourceType = typeof(Resources.BIL.PriceListMaster))]
        public string PartyName { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.InterfacePriceType, ValueField = "InterfacePriceType")]
        [Display(Name = "PriceListMaster_InterfacePriceType", ResourceType = typeof(Resources.BIL.PriceListMaster))]
        public string InterfacePriceTypeDesc { get; set; }
        
        #endregion
    }
}