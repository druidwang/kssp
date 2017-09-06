using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;
//TODO: Add other using statements here

namespace com.Sconit.Entity.SCM
{
    public partial class FlowStrategy
    {
        #region Non O/R Mapping Properties
        //[CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.Strategy, ValueField = "Strategy")]
        //[Display(Name = "FlowStrategy_Strategy", ResourceType = typeof(Resources.SCM.FlowStrategy))]
        //public string StrategyDescription { get; set; }

        public DateTime CurrentNextWindowTime { get; set; }
        public DateTime CurrentNextOrderTime { get; set; }

        #endregion
    }
}