namespace com.Sconit.Entity.SI.SD_ORD
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public partial class PickListMaster
    {
        #region O/R Mapping Properties

        public string PickListNo { get; set; }
        public string Flow { get; set; }
        public com.Sconit.CodeMaster.PickListStatus Status { get; set; }
        public com.Sconit.CodeMaster.OrderType OrderType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime WindowTime { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public string Dock { get; set; }
        public Boolean IsAutoReceive { get; set; }
        public Boolean IsReceiveScan { get; set; }
        public Boolean IsPrintAsn { get; set; }
        public Boolean IsPrintReceipt { get; set; }
        public Boolean IsReceiveExceed { get; set; }
        public Boolean IsReceiveFulfillUC { get; set; }
        public Boolean IsReceiveFifo { get; set; }
        public Boolean IsAsnUniqueReceive { get; set; }
        public Boolean IsCheckPartyFromAuthority { get; set; }
        public Boolean IsCheckPartyToAuthority { get; set; }
        public CodeMaster.CreateHuOption CreateHuOption { get; set; }
        public com.Sconit.CodeMaster.ReceiveGapTo ReceiveGapTo { get; set; }
        public DateTime EffectiveDate { get; set; }
        #endregion

        #region ¼ð»õµ¥Ã÷Ï¸
        public List<PickListDetail> PickListDetails { get; set; }

        #endregion

    }

}
