using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    public partial class TransportFlowCarrier
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TransportMode, ValueField = "TransportMode")]
        [Display(Name = "TransportFlowCarrier_TransportMode", ResourceType = typeof(Resources.TMS.TransportFlow))]
        public string TransportModeDescription { get; set; }

        #endregion
    }
}