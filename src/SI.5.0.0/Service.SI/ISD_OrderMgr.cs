using System.Collections.Generic;
using System;
using com.Sconit.Entity.SI.SD_ORD;
namespace com.Sconit.Service.SI
{
    public interface ISD_OrderMgr
    {
        Entity.SI.SD_ORD.OrderMaster GetOrderByOrderNoAndExtNo(string orderNo, bool includeDetail);

        Entity.SI.SD_ORD.OrderMaster GetOrder(string orderNo, bool includeDetail);

        Entity.SI.SD_ORD.OrderMaster GetOrderKeyParts(string orderNo);

        Entity.SI.SD_ORD.IpMaster GetIp(string ipNo, bool includeDetail);

        Entity.SI.SD_ORD.IpMaster GetIpByWmsIpNo(string wmsIpNo, bool includeDetail);

        Entity.SI.SD_ORD.SequenceMaster GetSeq(string seqNo, bool includeDetail);

        Entity.SI.SD_ORD.PickListMaster GetPickList(string pickListNo, bool includeDetail);

        string ShipPickList(string pickListNo);

        Entity.SI.SD_ORD.MiscOrderMaster GetMis(string MisNo);

        void StartPickList(string pickListNo);

        void BatchUpdateMiscOrderDetails(string miscOrderNo, IList<string> addHuIdList);

        void ConfirmMiscOrder(string miscOrderNo, IList<string> addHuIdList);

        void QuickCreateMiscOrder(IList<string> addHuIdList, string locationCode, string binCode, int type);

        void StartVanOrder(string orderNo, string location, IList<com.Sconit.Entity.SI.SD_INV.Hu> feedHuList);

        void StartVanOrder(string orderNo, string feedOrderNo);

        void PackSequenceOrder(string sequenceNo, List<string> huIdList);

        void UnPackSequenceOrder(string sequenceNo);

        void ShipSequenceOrder(string sequenceNo);

        void ShipSequenceOrderBySupplier(string sequenceNo);

        Entity.SI.SD_ORD.InspectMaster GetInspect(string inspectNo, bool includeDetail);

        Entity.SI.SD_INV.Hu GetHuByOrderNo(string orderNo);

        Boolean VerifyOrderCompareToHu(string orderNo, string huId);

        List<string> GetProdLineStation(string orderNo, string huId);

        #region 投料
        //投料到生产线
        //void FeedProdLineRawMaterial(string productLine, string productLineFacility, string[][] huDetails, bool isForceFeed, DateTime? effectiveDate);
        void FeedProdLineRawMaterial(string productLine, string productLineFacility, string location, List<com.Sconit.Entity.SI.SD_INV.Hu> hus, bool isForceFeed, DateTime? effectiveDate);
        //投料到生产单
        //void FeedOrderRawMaterial(string orderNo, string[][] huDetails, bool isForceFeed, DateTime? effectiveDate);
        void FeedOrderRawMaterial(string orderNo, string location, List<com.Sconit.Entity.SI.SD_INV.Hu> hus, bool isForceFeed, DateTime? effectiveDate);
        //KIT投料到生产单
        void FeedKitOrder(string orderNo, string kitOrderNo, bool isForceFeed, DateTime? effectiveDate);
        //生产单投料到生产单
        void FeedProductOrder(string orderNo, string productOrderNo, bool isForceFeed, DateTime? effectiveDate);
        #endregion

        void ReturnOrderRawMaterial(string orderNo, string traceCode, int? operation, string opReference, string[][] huDetails, DateTime? effectiveDate);

        void ReturnProdLineRawMaterial(string productLine, string productLineFacility, string[][] huDetails, DateTime? effectiveDate);

        string DoShipOrder(List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList, DateTime? effDate, bool isOpPallet = false);

        string DoReceiveOrder(List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList, DateTime? effDate);

        string DoReceiveIp(List<Entity.SI.SD_ORD.IpDetailInput> ipDetailInputList, DateTime? effDate);

        void DoReceiveKit(string kitNo, DateTime? effDate);

        void DoTransfer(Entity.SI.SD_SCM.FlowMaster flowMaster, List<Entity.SI.SD_SCM.FlowDetailInput> flowDetailInputList, bool isFifo = true, bool isOpPallet = false);

        void DoPickList(List<Entity.SI.SD_ORD.PickListDetailInput> pickListDetailInputList);

        void DoAnDon(List<AnDonInput> anDonInputList);

        void DoKitOrderScanKeyPart(string[][] huDetails, string orderNo);

        AnDonInput GetKanBanCard(string cardNo);

        void DoInspect(List<string> huIdList, DateTime? effDate);

        void DoWorkersWaste(List<string> huIdList, DateTime? effDate);

        void DoRepackAndShipOrder(List<Entity.SI.SD_INV.Hu> huList, DateTime? effDate);

        void DoJudgeInspect(Entity.SI.SD_ORD.InspectMaster inspectMaster, List<string> HuIdList, DateTime? effDate);

        void DoReturnOrder(string flowCode, List<string> huIdList, DateTime? effectiveDate);

        void KitOrderOffline(string kitOrderNo, List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList, IList<string> feedKitOrderNoList, DateTime? effectiveDate);

        List<com.Sconit.Entity.SI.SD_ORD.OrderMaster> GetKitBindingOrders(string orderNo);

        List<string> GetItemTraces(string orderNo);

        void DoItemTrace(string orderNo, List<string> huIdList);

        void CancelItemTrace(string orderNo, List<string> huIdList);

        Entity.SI.SD_INV.Hu DoReceiveProdOrder(string huId);

        Entity.SI.SD_INV.Hu CancelReceiveProdOrder(string huId);

        void DoReceiveProdOrder(List<string> huIdList);

        Entity.SI.SD_INV.Hu DoFilter(string huId, decimal outQty);

        Entity.SI.SD_INV.Hu StartAging(string huId);

        Entity.SI.SD_INV.Hu DoAging(string huId);

        void RecSmallChkSparePart(string huId, string spareItem, string userCode);

    }
}
