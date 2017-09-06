using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    public partial class TransportActingBill
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 

        public Decimal CurrentBillQty { get; set; }

        public Decimal CurrentBillAmount { get; set; }

        public Decimal CurrentRecalculatePrice { get; set; }

        public Decimal CurrentDiscount { get; set; }
        #endregion
    }
}