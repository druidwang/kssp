using System;
using com.Sconit.Entity.SYS;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    public partial class TransportBillDetail
    {
        #region Non O/R Mapping Properties


        public Decimal CurrentBillQty { get; set; }

        public Decimal CurrentBillAmount { get; set; }

        public Decimal CurrentDiscount { get; set; }

        #endregion
    }
}