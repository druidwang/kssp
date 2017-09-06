using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using com.Sconit.Entity.ORD;
namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    [DataContract]
    public partial class PrintOrderMaster : PrintBase
    {
        #region O/R Mapping Properties
        [DataMember]
        public string remark { get; set; }

        [DataMember]
        public string OrderNo { get; set; }

        [DataMember]
        public string IpNo { get; set; }

        [DataMember]
        public string Flow { get; set; }

        [DataMember]
        public string FlowDescription { get; set; }

        [DataMember]
        public string LocationFrom { get; set; }

        [DataMember]
        public string LocationFromName { get; set; }

        [DataMember]
        public string LocationTo { get; set; }

        [DataMember]
        public string LocationToName { get; set; }

        [DataMember]
        public string ProdLineFact { get; set; }

        [DataMember]
        public Int16 OrderStrategy { get; set; }
        //public virtual FlowStrategy OrderStrategy { get; set; }

        [DataMember]
        public string ReferenceOrderNo { get; set; }

        [DataMember]
        public string ExternalOrderNo { get; set; }

        [DataMember]
        public Int16 Type { get; set; }
        //public virtual OrderType Type { get; set; }

        [DataMember]
        public Int16 SubType { get; set; }
        //public virtual OrderSubType SubType { get; set; }

        [DataMember]
        public Int16 QualityType { get; set; }
        //public virtual QualityType QualityType { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public DateTime WindowTime { get; set; }

        [DataMember]
        public DateTime? EffDate { get; set; }

        [DataMember]
        public Int16 Priority { get; set; }
        //public virtual OrderPriority Priority { get; set; }

        [DataMember]
        public Int16 Status { get; set; }
        //public OrderStatus Status { get; set; }

        [DataMember]
        public Int64 Sequence { get; set; }

        [DataMember]
        public string PartyFrom { get; set; }

        [DataMember]
        public string PartyFromName { get; set; }

        [DataMember]
        public string PartyTo { get; set; }

        [DataMember]
        public string PartyToName { get; set; }

        [DataMember]
        public string ShipFrom { get; set; }

        [DataMember]
        public string ShipFromAddress { get; set; }

        [DataMember]
        public string ShipFromTel { get; set; }

        [DataMember]
        public string ShipFromCell { get; set; }

        [DataMember]
        public string ShipFromFax { get; set; }

        [DataMember]
        public string ShipFromContact { get; set; }

        [DataMember]
        public string ShipTo { get; set; }

        [DataMember]
        public string ShipToAddress { get; set; }

        [DataMember]
        public string ShipToTel { get; set; }

        [DataMember]
        public string ShipToCell { get; set; }

        [DataMember]
        public string ShipToFax { get; set; }

        [DataMember]
        public string ShipToContact { get; set; }

        [DataMember]
        public string Shift { get; set; }

        [DataMember]
        public Boolean IsInspect { get; set; }

        [DataMember]
        public Boolean IsListPrice { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string Dock { get; set; }

        [DataMember]
        public string OrderTemplate { get; set; }

        [DataMember]
        public string TraceCode { get; set; }

        [DataMember]
        public Int32 CreateUserId { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public DateTime? ReleaseDate { get; set; }

        [DataMember]
        public Int32? ReleaseUser { get; set; }

        [DataMember]
        public string ReleaseUserName { get; set; }

        [DataMember]
        public IList<PrintOrderDetail> OrderDetails { get; set; }

        #endregion

    }
}
