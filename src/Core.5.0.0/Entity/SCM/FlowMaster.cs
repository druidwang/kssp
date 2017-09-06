using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.SCM
{
    public partial class FlowMaster
    {
        #region Non O/R Mapping Properties

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "Type")]
        [Display(Name = "OrderMaster_Type", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public string FlowTypeDescription { get; set; }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.FlowStrategy, ValueField = "FlowStrategy")]
        public string FlowStrategyDescription { get; set; }
        public string CheckFlowCode { get; set; }


        [Display(Name = "FlowMaster_BillTerm", ResourceType = typeof(Resources.SCM.FlowMaster))]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderBillTerm, ValueField = "BillTerm")]
        public string BillTermDescription { get; set; }
        [Display(Name = "FlowMaster_ReceiveGapTo", ResourceType = typeof(Resources.SCM.FlowMaster))]
        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.ReceiveGapTo, ValueField = "ReceiveGapTo")]
        public string ReceiveGapToDescription { get; set; }
        public string CodeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Description))
                {
                    return this.Code;
                }
                else
                {
                    return this.Code + " [" + this.Description + "]";
                }

            }
        }

        public IList<FlowDetail> FlowDetails { get; set; }
        public IList<FlowBinding> FlowBindings { get; set; }
        public FlowStrategy CurrentFlowStrategy { get; set; }

        #endregion

        #region methods
        public void AddFlowDetail(FlowDetail flowDetail)
        {
            if (FlowDetails == null)
            {
                FlowDetails = new List<FlowDetail>();
            }
            FlowDetails.Add(flowDetail);
        }
        #endregion
    }
}