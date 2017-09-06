using System;
using com.Sconit.PrintModel;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    [DataContract]
    public partial class PrintMiscOrderMaster : PrintBase
    {
        #region O/R Mapping Properties

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string MiscOrderNo { get; set; }

        [DataMember]
        public Int16 Type { get; set; }

        [DataMember]
        public Int16 Status { get; set; }

        [DataMember]
        public Boolean IsScanHu { get; set; }

        [DataMember]
        public Int16 QualityType { get; set; }

        [DataMember]
        public string QualityTypeDescription { get; set; }

        [DataMember]
        public string MoveType { get; set; }

        [DataMember]
        public string CancelMoveType { get; set; }

        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string ReceiveLocation { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public string CostCenter { get; set; }

        [DataMember]
        public string Asn { get; set; }

        [DataMember]
        public string ReferenceNo { get; set; }

        [DataMember]
        public string DeliverRegion { get; set; }

        [DataMember]
        public string WBS { get; set; }

        [DataMember]
        public DateTime EffectiveDate { get; set; }

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
        public string Flow { get; set; }

        #endregion
        public IList<PrintMiscOrderDetail> MiscOrderDetails { get; set; }
    }

}
