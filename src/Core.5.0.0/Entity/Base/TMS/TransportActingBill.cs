using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.CodeMaster;
//TODO: Add other using statements here

namespace com.Sconit.Entity.TMS
{
    [Serializable]
    public partial class TransportActingBill : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        public Int32 Version { get; set; }

        public bool IsBilled { get; set; }

        public string Currency { get; set; }

        public string Flow { get; set; }

        public string FlowDescription { get; set; }

        public string OrderNo { get; set; }

        public com.Sconit.CodeMaster.BillType Type { get; set; }

        public string BillAddress { get; set; }

        public string BillAddressDescription { get; set; }

        public string CarrierDescription { get; set; }

        public string Carrier { get; set; }

        public string FreightNo { get; set; }

        public Decimal BillAmount { get; set; }

        public Decimal BillingAmount { get; set; }

        public Decimal BilledAmount { get; set; }

        public string PriceList { get; set; }

        public Boolean IsIncludeTax { get; set; }

        public string TaxCode { get; set; }

        public Boolean IsProvisionalEstimate { get; set; }

        public DateTime EffectiveDate { get; set; }

        public TransportPricingMethod PricingMethod { get; set; }

        public Boolean IsClose { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

        public Decimal UnitPrice { get; set; }

        public int RoundUpOpt { get; set; }

        public decimal BilledQty { get; set; }

        public Decimal BillingQty { get; set; }

        public decimal BillQty { get; set; }

        public int PriceListDetail { get; set; }

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
            TransportActingBill another = obj as TransportActingBill;

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
