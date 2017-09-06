using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.INP
{
    [Serializable]
    [DataContract]
    public partial class PrintInspectDetail : PrintBase
    {
        #region O/R Mapping Properties
        [DataMember]
        public Int32 Id { get; set; }
        [DataMember]
        public string InspectNo { get; set; }
        [DataMember]
        public Int32 Sequence { get; set; }
        [DataMember]
        public string Item { get; set; }
        [DataMember]
        public string ItemDescription { get; set; }
        [DataMember]
        public string ReferenceItemCode { get; set; }
        [DataMember]
        public Decimal UnitCount { get; set; }
        [DataMember]
        public string Uom { get; set; }
        [DataMember]
        public string BaseUom { get; set; }
        [DataMember]
        public Decimal UnitQty { get; set; }
        [DataMember]
        public string HuId { get; set; }
        [DataMember]
        public string LotNo { get; set; }
        [DataMember]
        public string LocationFrom { get; set; }
        [DataMember]
        public string CurrentLocation { get; set; }
        [DataMember]
        public Decimal InspectQty { get; set; }
        [DataMember]
        public Decimal QualifyQty { get; set; }
        [DataMember]
        public Decimal RejectQty { get; set; }
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
        public Boolean IsJudge { get; set; }
        [DataMember]
        public string ManufactureParty { get; set; }
        [DataMember]
        public string WMSSeq { get; set; }
        [DataMember]
        public Int32 IpDetailSequence { get; set; }
        [DataMember]
        public Int32 ReceiptDetailSequence { get; set; }
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public string FailCode { get; set; }


        #endregion


    }

}
