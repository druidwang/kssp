using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.PRD
{
    public partial class RoutingDetail
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.TimeUnit, ValueField = "TimeUnit")]
        [Display(Name = "RoutingDetail_TimeUnit", ResourceType = typeof(Resources.PRD.Routing))]
        public string TimeUnitDescription { get; set; }
        #endregion
    }


    public partial class Operation
    {
        public Int32 OpCode { get; set; }
    }
}