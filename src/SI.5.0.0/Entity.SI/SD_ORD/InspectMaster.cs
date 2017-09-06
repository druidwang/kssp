
namespace com.Sconit.Entity.SI.SD_ORD
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public partial class InspectMaster
    {
        #region O/R Mapping Properties
        public string InspectNo { get; set; }
        public string ReferenceNo { get; set; }
        public string Region { get; set; }
        public com.Sconit.CodeMaster.InspectStatus Status { get; set; }
        public Boolean IsATP { get; set; }
        public Boolean IsPrint { get; set; }
        public com.Sconit.CodeMaster.InspectType Type { get; set; }
        #endregion

        #region ¸¨Öú×Ö¶Î
        public List<InspectDetail> InspectDetails { get; set; }
        //public com.Sconit.CodeMaster.JudgeResult JudgeResult { get; set; }
        public string FailCode { get; set; }
        #endregion
    }
}
