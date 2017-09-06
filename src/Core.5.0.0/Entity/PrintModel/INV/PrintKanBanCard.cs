using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace com.Sconit.PrintModel.INV
{
    [Serializable]
    [DataContract]
    public partial class PrintKanBanCard 
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Flow { get; set; }

        [DataMember]
        public string LocationTo { get; set; }
        [DataMember]
        public string Item { get; set; }
        [DataMember]
        public string ItemDescription { get; set; }
        [DataMember]
        public string ItemCategory { get; set; }
        [DataMember]
        public string Uom { get; set; }
        [DataMember]
        public string ManufactureParty { get; set; }
        [DataMember]
        public Decimal UnitCount { get; set; }
        [DataMember]
        public string PackType { get; set; }
        [DataMember]
        public string Qty { get; set; }
        [DataMember]
        public Int32 StationUseQty { get; set; }
        [DataMember]
        public string MultiStation { get; set; }
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public Int32 Sequence { get; set; }
        [DataMember]
        public Int32? ThumbNo { get; set; }
        [DataMember]
        public string ReferenceItemCode { get; set; }

        [DataMember]
        public string Routing { get; set; }
        [DataMember]
        public IList<PrintKanBanCardInfo> KanBanDetails { get; set; }

    }

}
