using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SI.SAP
{
    public partial class SAPItem
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [Display(Name = "SAPItem_Status", ResourceType = typeof(Resources.SI.SAPItem))]
        public string StatusDesc { get { return this.Status == 1 ? "³É¹¦" : "Ê§°Ü"; } }
        #endregion
    }
}