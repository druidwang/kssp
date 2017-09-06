using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SI.SAP
{
    public partial class SAPTransferLog
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [Display(Name = "SAPTransferLog_Status", ResourceType = typeof(Resources.SI.SAPTransferLog))]
        public string StatusDesc { get { return this.Status.HasValue && this.Status == 1 ? "³É¹¦" : "Ê§°Ü"; } }
        #endregion
    }
}