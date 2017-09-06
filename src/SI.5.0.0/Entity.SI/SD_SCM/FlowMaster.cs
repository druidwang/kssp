namespace com.Sconit.Entity.SI.SD_SCM
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class FlowMaster 
    {
        #region O/R Mapping Properties

		public string Code { get; set; }

		public string Description { get; set; }

		public Boolean IsActive { get; set; }

        public com.Sconit.CodeMaster.OrderType Type { get; set; }

		public string ReferenceFlow { get; set; }

		public string PartyFrom { get; set; }

		public string PartyTo { get; set; }

		//public string ShipFrom { get; set; }

        //public string ShipTo { get; set; }

        public string LocationFrom { get; set; }

        public string LocationTo { get; set; }

        //public string BillAddress { get; set; }

        //public string PriceList { get; set; }

        //public string Dock { get; set; }

        public string Routing { get; set; }

        //public string ReturnRouting { get; set; }

        //public Boolean IsAutoCreate { get; set; }

        //public Boolean IsAutoRelease { get; set; }

        //public Boolean IsAutoStart { get; set; }

        //public Boolean IsAutoShip { get; set; }

        //public Boolean IsAutoReceive { get; set; }

        //public Boolean IsAutoBill { get; set; }

        //public Boolean IsListDet { get; set; }

        public Boolean IsManualCreateDetail { get; set; }

        //public Boolean IsListPrice { get; set; }

        //public Boolean IsPrintOrder { get; set; }

        //public Boolean IsPrintAsn { get; set; }

        //public Boolean IsPrintRceipt { get; set; }

        //public Boolean IsShipExceed { get; set; }

        //public Boolean IsReceiveExceed { get; set; }

        public Boolean IsOrderFulfillUC { get; set; }

        //public Boolean IsShipFulfillUC { get; set; }

        //public Boolean IsReceiveFulfillUC { get; set; }

        //public Boolean IsShipScanHu { get; set; }

        //public Boolean IsReceiveScanHu { get; set; }

        //public Boolean IsCreatePickList { get; set; }

        //public Boolean IsInspect { get; set; }

        //public Boolean IsRejectInspect { get; set; }

        //public Boolean IsReceiveFifo { get; set; }

        //public Boolean IsShipByOrder { get; set; }

        //public Boolean IsAsnUniqueReceive { get; set; }

        //public Boolean IsMRP { get; set; }

        //public com.Sconit.CodeMaster.ReceiveGapTo ReceiveGapTo { get; set; }

        //public string ReceiptTemplate { get; set; }

        //public string OrderTemplate { get; set; }

        //public string AsnTemplate { get; set; }

        //public string HuTemplate { get; set; }

        //public com.Sconit.CodeMaster.OrderBillTerm BillTerm { get; set; }

        //public com.Sconit.CodeMaster.CreateHuOption CreateHuOption { get; set; }

        //public Int32 MaxOrderCount { get; set; }

        //public com.Sconit.CodeMaster.MRPOption MRPOption { get; set; }

        //public Boolean IsPause { get; set; }

        //public DateTime? PauseTime { get; set; }

        public Boolean IsCheckPartyFromAuthority { get; set; }

        public Boolean IsCheckPartyToAuthority { get; set; }

        //public Boolean IsShipFifo { get; set; }

        //public string ExtraDemandSource { get; set; }

        //public com.Sconit.CodeMaster.FlowStrategy FlowStrategy { get; set; }

        //public string PickStrategy { get; set; }


        #endregion

        #region 
        public string Bin { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public List<FlowDetail> FlowDetails { get; set; }
        #endregion
    }
	
}
