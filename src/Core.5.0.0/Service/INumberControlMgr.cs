using System.Collections.Generic;
using com.Sconit.Entity.INP;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.ISS;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.TMS;
using com.Sconit.Entity.WMS;
using com.Sconit.Entity.FMS;

namespace com.Sconit.Service
{
    public interface INumberControlMgr
    {
        string GetNextSequenceo(string codePrefix);

        string GetOrderNo(OrderMaster orderMaster);

        string GetIpNo(IpMaster ipMaster);

        string GetReceiptNo(ReceiptMaster receiptMaster);

        string GetStockTakeNo(StockTakeMaster stockTake);

        string GetIssueNo(IssueMaster issueMaster);

        string GetInspectNo(InspectMaster inspectMaster);

        string GetPickListNo(PickListMaster pickListMaster);

        string GetRejectNo(RejectMaster rejectMaster);

        string GetSequenceNo(SequenceMaster sequenceMaster);

        string GetConcessionNo(ConcessionMaster concessionOrder);

        string GetMiscOrderNo(MiscOrderMaster miscOrderMaster);

        string GetVehicleInFactoryNo(VehicleInFactoryMaster vehicleInFactoryMaster);

        string GetKanBanCardNo();

        string GetBillNo(BillMaster billMaster);

        string GetTransportOrderNo(TransportOrderMaster transportOrderMaster);

        IDictionary<string, decimal> GetHuId(FlowDetail flowDetail);

        IDictionary<string, decimal> GetHuId(OrderDetail orderDetail);

        IDictionary<string, decimal> GetHuId(IpDetail ipDetail);

        IDictionary<string, decimal> GetHuId(ReceiptDetail receiptDetail);

        IDictionary<string, decimal> GetHuId(Item item);

        string GetContainerId(string prefix);

        IDictionary<string, decimal> GetHuId(string lotNo, string item, string manufactureParty, decimal qty, decimal unitCount);

        string GetNextSequence(string code);

        string GetShipmentNo();

        string GetTaskNo(string prefix);

        IDictionary<string, decimal> GetDeliveryBarCode(ShipPlan shipPlan);

        string GetPackingListCode();

        string GetTransportBillNo(TransportBillMaster billMaster);

        string GetFCID(FacilityMaster facilityMaster);

        string GetTraceCode();

        string GetPalletCode();
    }
}
