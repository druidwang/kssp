using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SCM
{
    public partial class PickStrategy
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.PickOddOption, ValueField = "OddOption")]
        [Display(Name = "PickStrategy_OddOption", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public string PickOddOptionDescription { get; set; }


        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ShipStrategy, ValueField = "ShipStrategy")]
        [Display(Name = "PickStrategy_ShipStrategy", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public string ShipStrategyDescription { get; set; }

        #endregion
    }
}