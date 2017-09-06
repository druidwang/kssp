using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.MRP.VIEW
{
    #region //////////////ShiftPlan//////////////////
    public class ShiftPlanView
    {
        public string ProductLine { get; set; }
        public ShiftPlanHead PlanHead { get; set; }
        public List<ShiftPlanBody> PlanBodyList { get; set; }
    }

    public class ShiftPlanHead
    {
        public List<ShiftPlanColumnCell> ColumnCellList { get; set; }
    }

    public class ShiftPlanBody
    {
        public string Flow { get; set; }
        public string Machine { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ItemReference { get; set; }
        public string Uom { get; set; }
        public Double ShiftQuota { get; set; }
        public List<ShiftPlanRowCell> RowCellList { get; set; }
    }

    public class ShiftPlanRowCell
    {
        public string Flow { get; set; }
        public string Machine { get; set; }
        public string Item { get; set; }
        public string Shift { get; set; }
        public DateTime PlanDate { get; set; }
        public Double Qty { get; set; }
    }

    public class ShiftPlanColumnCell
    {
        public string Shift { get; set; }
        public DateTime PlanDate { get; set; }
    }
    #endregion
}
