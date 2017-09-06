using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.ORD
{
    public partial class MrpPlan
    {
        #region O/R Mapping Properties

        public string PlanToDate
        {
            get { return this.PlanDate.ToString("yyyy-MM-dd"); }
            set { this.PlanDate = Convert.ToDateTime(value); }
        }

        [CodeDetailDescriptionAttribute(CodeMaster = com.Sconit.CodeMaster.CodeMaster.OrderType, ValueField = "OrderType")]
        [Display(Name = "MrpPlan_OrderType", ResourceType = typeof(Resources.MRP.MrpPlan))]
        public string OrderTypeDescription { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public double CurrentQty { get; set; }

        public double LeftQty
        {
            get
            {
                var leftQty = Qty - OrderQty;
                leftQty = leftQty > 0 ? leftQty : 0;
                return leftQty;
            }
        }

        #endregion
    }


}
