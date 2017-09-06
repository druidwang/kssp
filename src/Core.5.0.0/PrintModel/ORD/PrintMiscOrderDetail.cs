using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace com.Sconit.PrintModel.ORD
{
    [Serializable]
    [DataContract]
    public partial class PrintMiscOrderDetail : PrintBase
    {
        #region O/R Mapping Properties
        [DataMember]
		public Int32 Id { get; set; }

        [DataMember]
		public string MiscOrderNo { get; set; }

        [DataMember]
		public Int32 Sequence { get; set; }

        [DataMember]
        public string Item { get; set; }

        [DataMember]
        public string ItemDescription { get; set; }

        [DataMember]
        public string ReferenceItemCode { get; set; }

        [DataMember]
        public string Uom { get; set; }
        
        [DataMember]
        public string BaseUom { get; set; }

        [DataMember]
        public Decimal UnitCount { get; set; }

        [DataMember]
        public Decimal UnitQty { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string ReserveNo { get; set; }

        [DataMember]
        public string ReserveLine { get; set; }

        [DataMember]
        public Decimal Qty { get; set; }

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
        public string ExternalOrderNo { get; set; }

        [DataMember]
        public string ExternalSequence { get; set; }
        #endregion

    }
	
}
