using com.Sconit.Entity.SCM;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class OrderBinding
    {
        #region Non O/R Mapping Properties
        public FlowMaster CurrentBindFlowMaster { get; set; }

        public OrderMaster CurrentBindOrderMaster { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.BindType, ValueField = "BindType")]
        [Display(Name = "OrderBinding_BindType", ResourceType = typeof(Resources.ORD.OrderBinding))]
        public string BindTypeDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.FlowStrategy, ValueField = "BindFlowStrategy")]
        [Display(Name = "OrderBinding_BindFlowStrategy", ResourceType = typeof(Resources.ORD.OrderBinding))]
        public string BindFlowStrategyDescription { get; set; }

        public string ControlName { get; set; }
        #endregion
    }
}