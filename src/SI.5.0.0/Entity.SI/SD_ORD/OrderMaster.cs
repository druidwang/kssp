using System;
using System.Collections.Generic;
using com.Sconit.Entity.SI.SD_INV;

namespace com.Sconit.Entity.SI.SD_ORD
{
    public class OrderMaster
    {
        
        public string OrderNo { get; set; }
        public string Flow { get; set; }
        //public string ProductLineFacility { get; set; }
        public string ReferenceOrderNo { get; set; }
        public string ExternalOrderNo { get; set; }
        public com.Sconit.CodeMaster.OrderType Type { get; set; }
        public com.Sconit.CodeMaster.OrderSubType SubType { get; set; }
        public com.Sconit.CodeMaster.QualityType QualityType { get; set; }
        public DateTime StartTime { get; set; }
        public com.Sconit.CodeMaster.OrderStatus Status { get; set; }
        public Int64 Sequence { get; set; }
        public string PartyFrom { get; set; }
        //public string PartyFromName { get; set; }
        public string PartyTo { get; set; }
        //public string PartyToName { get; set; }
        public string ShipFrom { get; set; }
        public string ShipTo { get; set; }
        public string LocationFrom { get; set; }
        //public string LocationFromName { get; set; }
        public string LocationTo { get; set; }
        public string Dock { get; set; }
        public Boolean IsAutoReceive { get; set; }
        public Boolean IsShipExceed { get; set; }
        public Boolean IsReceiveExceed { get; set; }
        public Boolean IsOrderFulfillUC { get; set; }
        public Boolean IsShipFulfillUC { get; set; }
        public Boolean IsReceiveFulfillUC { get; set; }
        public Boolean IsShipScanHu { get; set; }
        public Boolean IsReceiveScanHu { get; set; }
        public Boolean IsReceiveFifo { get; set; }
        public Boolean IsShipByOrder { get; set; }
        public Boolean IsAsnUniqueReceive { get; set; }
        public Boolean IsAsnAutoClose { get; set; }
        public com.Sconit.CodeMaster.ReceiveGapTo ReceiveGapTo { get; set; }
        public com.Sconit.CodeMaster.CreateHuOption CreateHuOption { get; set; }
        public Boolean IsCheckPartyFromAuthority { get; set; }
        public Boolean IsCheckPartyToAuthority { get; set; }
        public Boolean IsShipFifo { get; set; }
        public Boolean IsOpenOrder { get; set; }
        public Boolean IsPause { get; set; }
        //public FlowStrategy.StrategyEnum OrderStrategy { get; set; }
        public string Station { get; set; }
        public string Op { get; set; }
        public string TraceCode { get; set; }
        public com.Sconit.CodeMaster.FlowStrategy OrderStrategy { get; set; }
        public DateTime CloseDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public string Shift { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }  
    }
}
