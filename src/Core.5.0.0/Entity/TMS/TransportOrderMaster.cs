using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    public partial class TransportOrderMaster
    {
        #region Non O/R Mapping Properties

        public IList<TransportOrderRoute> TransportOrderRouteList { get; set; }
        public IList<TransportOrderDetail> TransportOrderDetailList { get; set; }

        [Display(Name = "TransportOrderMaster_TransportMode", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string TransportModeDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TransportMode, ValueField = "TransportMode")]
        [Display(Name = "TransportOrderMaster_Status", ResourceType = typeof(Resources.TMS.TransportOrder))]
        public string StatusDescription { get; set; }
        
        #endregion
    }
}