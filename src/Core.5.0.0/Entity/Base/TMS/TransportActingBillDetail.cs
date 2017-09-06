using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.CodeMaster;
//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportActingBillDetail : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        public Int32 ActBill { get; set; }

        public string ShipFrom { get; set; }

        public string ShipFromAddress { get; set; }

        public string ShipTo { get; set; }

        public string ShipToAddress { get; set; }

        public string Currency { get; set; }

        public string IpNo { get; set; }

        public Decimal BillAmount { get; set; }

        public Boolean IsIncludeTax { get; set; }

        public string TaxCode { get; set; }

        public Decimal UnitPrice { get; set; }

        public decimal BillQty { get; set; }

        public string PriceList { get; set; }

        public int PriceListDetail { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

     

        #endregion

        public override int GetHashCode()
        {
            if (Id != null)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            TransportActingBillDetail another = obj as TransportActingBillDetail;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }
    }

}
