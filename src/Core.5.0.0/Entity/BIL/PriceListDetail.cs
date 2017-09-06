using System;
using System.ComponentModel.DataAnnotations;
//TODO: Add other using statements here

namespace com.Sconit.Entity.BIL
{
    public partial class PriceListDetail
    {
        #region Non O/R Mapping Properties

        [Display(Name = "PriceListDetail_PriceList", ResourceType = typeof(Resources.BIL.PriceListDetail))]
        public string PriceListCode { get; set; }
        [Display(Name = "PriceListDetail_PriceList_ItemDesc", ResourceType = typeof(Resources.BIL.PriceListDetail))]
        public string ItemDesc { get; set; }
        #endregion
    }
}