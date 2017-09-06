using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.BILL
{
    [Serializable]
    [DataContract]
    public partial class PrintBillMaster : PrintBase
    {
        #region O/R Mapping Properties

        [DataMember]
        public string BillNo { get; set; }
        [DataMember]
        public string ExternalBillNo { get; set; }
        [DataMember]
        public string ReferenceBillNo { get; set; }
        [DataMember]
        public Int16 Type { get; set; }
        [DataMember]
        public Int16 SubType { get; set; }
        [DataMember]
        public Int16 Status { get; set; }
        [DataMember]
        public string BillAddress { get; set; }
        [DataMember]
        public string BillAddressDescription { get; set; }
        [DataMember]
        public string Party { get; set; }
        [DataMember]
        public string PartyName { get; set; }
        [DataMember]
        public string Currency { get; set; }
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
        public DateTime? ReleaseDate { get; set; }
        [DataMember]
        public Int32? ReleaseUserId { get; set; }
        [DataMember]
        public string ReleaseUserName { get; set; }
        [DataMember]
        public DateTime? CloseDate { get; set; }
        [DataMember]
        public Int32? CloseUserId { get; set; }
        [DataMember]
        public string CloseUserName { get; set; }
        [DataMember]
        public DateTime? CancelDate { get; set; }
        [DataMember]
        public Int32? CancelUserId { get; set; }
        [DataMember]
        public string CancelUserName { get; set; }
        [DataMember]
        public string CancelReason { get; set; }
        [DataMember]
        public string InvoiceNo { get; set; }
        [DataMember]
        public DateTime? InvoiceDate { get; set; }
        [DataMember]
        public Decimal Amount { get; set; }

        [DataMember]
        public IList<PrintBillDetail> BillDetails { get; set; }
        #endregion

    }
	
}
