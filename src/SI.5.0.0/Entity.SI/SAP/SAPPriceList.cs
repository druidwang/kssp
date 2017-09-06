using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SI.SAP
{
    public partial class SAPPriceList
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [Display(Name = "SAPPriceList_Status", ResourceType = typeof(Resources.SI.SAPPriceList))]
        public string StatusDesc { get { return this.Status.HasValue && this.Status == 1 ? "³É¹¦" : "Ê§°Ü"; } }
        #endregion
    }
}