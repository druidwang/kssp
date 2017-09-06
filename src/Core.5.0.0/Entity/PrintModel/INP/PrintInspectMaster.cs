using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace com.Sconit.PrintModel.INP
{
    [Serializable]
    [DataContract]
    public partial class PrintInspectMaster : PrintBase
    {
        #region O/R Mapping Properties

        [DataMember]
        public string InspectNo { get; set; }
        [DataMember]
        public string ReferenceNo { get; set; }
        [DataMember]
        public string Region { get; set; }
        [DataMember]
        public Boolean IsATP { get; set; }
        [DataMember]
        public Boolean IsPrint { get; set; }
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
        public string ManufactureParty { get; set; }
        [DataMember]
        public string WMSNo { get; set; }
        [DataMember]
        public string IpNo { get; set; }
        [DataMember]
        public string ReceiptNo { get; set; }

        [DataMember]
        public IList<PrintInspectDetail> InspectDetails { get; set; }
        #endregion

    }
	
}
