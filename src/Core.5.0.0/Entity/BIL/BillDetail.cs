using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
//TODO: Add other using statements here

namespace com.Sconit.Entity.BIL
{
    public partial class BillDetail
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        public string CheckOrderNo { get; set; }
        [Display(Name = "BillDetail_BillQty", ResourceType = typeof(Resources.BIL.BillDetail))]
        public Decimal BillQty { get; set; }
        [Display(Name = "BillDetail_BilledQty", ResourceType = typeof(Resources.BIL.BillDetail))]
        public Decimal BilledQty { get; set; }
        [Display(Name = "BillDetail_CurrentBillQty", ResourceType = typeof(Resources.BIL.BillDetail))]
        public Decimal CurrentBillQty { get; set; }

        [Display(Name = "BillDetail_CurrentBillAmount", ResourceType = typeof(Resources.BIL.BillDetail))]
        public Decimal CurrentBillAmount { get; set; }

        [Display(Name = "BillDetail_Discount", ResourceType = typeof(Resources.BIL.BillDetail))]
        public Decimal CurrentDiscount { get; set; }


        #endregion
    }
}