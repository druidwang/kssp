using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SI.SAP
{
    public partial class SAPSupplier
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [Display(Name = "SAPSupplier_Status", ResourceType = typeof(Resources.SI.SAPSupplier))]
        public string StatusDesc { get { return this.Status.HasValue && this.Status == 1 ? "³É¹¦" : "Ê§°Ü"; } }

        #endregion
    }
}