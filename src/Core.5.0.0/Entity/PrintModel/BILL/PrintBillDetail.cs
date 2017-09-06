using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.BILL
{
    [Serializable]
    public partial class PrintBillDetail : PrintBase
    {
        #region O/R Mapping Properties
        [DataMember]
        public Int32 Id { get; set; }
        [DataMember]
        public string BillNo { get; set; }
        [DataMember]
        public Int32 ActingBillId { get; set; }
        [DataMember]
        public string Item { get; set; }
        [DataMember]
        public string ItemDescription { get; set; }
        [DataMember]
        public string Uom { get; set; }
        [DataMember]
        public Decimal UnitCount { get; set; }
        [DataMember]
        public Decimal Qty { get; set; }
        [DataMember]
        public string PriceList { get; set; }
        [DataMember]
        public Decimal Amount { get; set; }
        [DataMember]
        public Decimal UnitPrice { get; set; }
        [DataMember]
        public string OrderNo { get; set; }
        [DataMember]
        public string IpNo { get; set; }
        [DataMember]
        public string ExternalIpNo { get; set; }
        [DataMember]
        public string ReceiptNo { get; set; }
        [DataMember]
        public string ExternalReceiptNo { get; set; }
        [DataMember]
        public Int32 CreateUserId { get; set; }
        [DataMember]
        public string CreateUserName { get; set; }
        [DataMember]
        public DateTime CreateDate { get; set; }
        [DataMember]
        public Int32 LastModifyUserId { get; set; }
        [DataMember]
        public string LastModifyUserName { get; set; }
        [DataMember]
        public DateTime LastModifyDate { get; set; }
        [DataMember]
        public Int32 Version { get; set; }
        [DataMember]
        public string Flow { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public bool IsIncludeTax { get; set; }
        [DataMember]
        public string Tax { get; set; }
        [DataMember]
        public string ReferenceItemCode { get; set; }
        [DataMember]
        public string Party { get; set; }
       [DataMember]
        public string PartyName { get; set; }
        [DataMember]
        public Int16 Type { get; set; }
        [DataMember]
        public string LocationFrom { get; set; }
        [DataMember]
        public bool IsProvisionalEstimate { get; set; }
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        #endregion
    }
	
}
