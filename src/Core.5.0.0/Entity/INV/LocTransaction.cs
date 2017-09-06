using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.INV
{
    public partial class LocTransaction
    {
        #region Non O/R Mapping Properties
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public com.Sconit.CodeMaster.TransactionType TransactionType { get; set; }
        public Decimal Qty { get; set; }
        public Decimal CSQty { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }

        #endregion
    }
}