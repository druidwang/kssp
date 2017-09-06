using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using com.Sconit.PrintModel;

namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    [DataContract]
    public partial class PrintSequenceDetail : PrintBase
    {
        #region O/R Mapping Properties
        [DataMember]
		public Int32 Id { get; set; }

        [DataMember]
        public string SequenceNo { get; set; }

        [DataMember]
        public string OrderNo { get; set; }

        [DataMember]
        public string TraceCode { get; set; }

        [DataMember]
        public Int32 OrderDetailId { get; set; }

        [DataMember]
        public Int32 OrderDetailSequence { get; set; }
        
        [DataMember]
        public Int64 Sequence { get; set; }

        [DataMember]
        public string Item { get; set; }

        [DataMember]
        public string ItemDescription { get; set; }

        [DataMember]
        public string ReferenceItemCode { get; set; }

        [DataMember]
        public string Uom { get; set; }

        [DataMember]
        public Decimal UnitQty { get; set; }

        [DataMember]
        public string BaseUom { get; set; }

        [DataMember]
        public Decimal UnitCount { get; set; }

        [DataMember]
        public Int16 QualityType { get; set; }

        [DataMember]
        public string ManufactureParty { get; set; }

        [DataMember]
        public Decimal Qty { get; set; }

        [DataMember]
        public Boolean IsClose { get; set; }

        [DataMember]
        public string HuId { get; set; }

        [DataMember]
        public string LotNo { get; set; }

        [DataMember]
        public Int32 CreateUserId { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        #endregion
    }
	
}
