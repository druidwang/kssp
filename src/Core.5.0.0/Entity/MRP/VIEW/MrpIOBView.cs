using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity.MRP.VIEW
{
    #region MrpIOBView
    public class MrpIOBView
    {
        public IOBHead IOBHead { get; set; }
        public List<IOBBody> IOBBodyList { get; set; }
    }

    public class IOBHead
    {
        public List<IOBColumnCell> ColumnCellList { get; set; }
    }

    public class IOBBody
    {
        public string Flow { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ItemReference { get; set; }
        public string Uom { get; set; }
        public Double InvStart { get; set; }
        public Double SafeStock { get; set; }
        public Double MaxStock { get; set; }
        public Double StockOver { get; set; }
        public Double StockLack { get; set; }
        public List<IOBRowCell> RowCellList { get; set; }
    }

    public class IOBRowCell
    {
        public string Flow { get; set; }
        public string Item { get; set; }
        public DateTime PlanDate { get; set; }
        public Double InQty { get; set; }
        public Double OutQty { get; set; }
        public Double EndQty { get; set; }
    }

    public class IOBColumnCell
    {
        public DateTime PlanDate { get; set; }
    }
    #endregion

}
