using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    public partial class MrpFlowDetail
    {
        public double HaltTime { get; set; }
        public double TrialTime { get; set; }

        //public Double CalcStock
        //{
        //    get
        //    {
        //        return this.MaxStock > this.SafeStock ? this.MaxStock : this.SafeStock;
        //    }
        //}

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "Type")]
        [Display(Name = "MrpFlowDetail_Type", ResourceType = typeof(Resources.MRP.MrpFlowDetail))]
        public string TypeDescription { get; set; }
    }

}