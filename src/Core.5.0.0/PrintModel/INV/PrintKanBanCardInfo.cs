using System;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.INV
{
    [Serializable]
    [DataContract]
    public partial class PrintKanBanCardInfo 
    {
        [DataMember]
        public string CardNo { get; set; }
        [DataMember]
        public string KBICode { get; set; }
        [DataMember]
        public int Sequence { get; set; }
    }

}
