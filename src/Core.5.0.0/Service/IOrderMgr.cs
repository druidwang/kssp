using System;
using System.Collections.Generic;
using com.Sconit.Entity.INP;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SCM;
using System.IO;
using com.Sconit.Entity.INV;

namespace com.Sconit.Service
{
    public interface IOrderMgr
    {
        #region 路线转订单
        OrderMaster TransferFlow2Order(FlowMaster flowMaster, bool isTransferDetail);
        OrderMaster TransferFlow2Order(FlowMaster flowMaster, IList<string> itemCodeList);
        OrderMaster TransferFlow2Order(FlowMaster flowMaster, IList<string> itemCodeList, DateTime effectiveDate);
        OrderMaster TransferFlow2Order(FlowMaster flowMaster, IList<string> itemCodeList, DateTime effectiveDate, bool isTransferDetail);
        IList<OrderDetail> TransformFlowMster2OrderDetailList(FlowMaster flow, CodeMaster.OrderSubType orderSubType);
        #endregion

        #region 订单增删改
        void CreateOrder(OrderMaster orderMaster);
        void CreateOrder(OrderMaster orderMaster, bool expandOrderBomDetail);
        void UpdateOrder(OrderMaster orderMaster);
        void DeleteOrder(string orderNo);
        #endregion

        #region 重新绑定路线
        //IList<OrderBinding> CreateBindOrder(OrderMaster orderMaster);
        void ReCreateBindOrder(OrderBinding orderBinding);
        #endregion

        #region 批量更新订单明细
        IList<OrderDetail> BatchUpdateOrderDetails(string orderNo, IList<OrderDetail> addOrderDetailList, IList<OrderDetail> updateOrderDetailList, IList<OrderDetail> deleteOrderDetailList);
        IList<OrderDetail> AddOrderDetails(string orderNo, IList<OrderDetail> orderDetailList);
        IList<OrderDetail> UpdateOrderDetails(IList<OrderDetail> orderDetailList);
        void DeleteOrderDetails(IList<int> orderDetailIds);
        IList<OrderDetail> UpdateTireOrderDetails(string ordreNo, IList<OrderDetail> orderDetailList);
        #endregion

        #region 批量更新订单绑定
        void BatchUpdateOrderBindings(string orderNo, IList<OrderBinding> addOrderBindingList, IList<OrderBinding> deleteOrderBindingList);
        void AddOrderBindings(string orderNo, IList<OrderBinding> orderBindingList);
        void DeleteOrderBindings(IList<int> orderBindingIds);
        #endregion

        #region 批量更新工序
        void BatchUpdateOrderOperations(int orderDetailId, IList<OrderOperation> addOrderOperationList, IList<OrderOperation> updateOrderOperationList, IList<OrderOperation> deleteOrderOperationList);
        void AddOrderOperations(int orderDetailId, IList<OrderOperation> orderOperationList);
        void UpdateOrderOperations(IList<OrderOperation> orderOperations);
        void DeleteOrderOperations(IList<int> orderOperationIds);
        IList<OrderOperation> ExpandOrderOperation(int orderDetailId);
        #endregion

        #region 批量更新订单Bom
        void BatchUpdateOrderBomDetails(OrderDetail orderDetail, IList<OrderBomDetail> addOrderBomDetailList, IList<OrderBomDetail> updateOrderBomDetailList, IList<OrderBomDetail> deleteOrderBomDetailList);
        void AddOrderBomDetails(int orderDetailId, IList<OrderBomDetail> orderBomDetailList);
        void UpdateOrderBomDetails(IList<OrderBomDetail> orderBomDetails);
        void DeleteOrderBomDetails(IList<int> orderBomDetailIds);
        IList<OrderBomDetail> ExpandOrderBomDetail(int orderDetailId);
        object[] ExpandOrderOperationAndBomDetail(int orderDetailId);
        #endregion

        #region 释放订单
        void ReleaseOrder(string orderNo);
        void ReleaseOrder(OrderMaster orderMaster);
        //void ReleaseOrder(OrderMaster orderMaster, bool isCreateBindOrder);
        #endregion

        #region 订单上线
        void StartOrder(string orderNo);
        void StartOrder(OrderMaster orderMaster);
        #endregion

        #region 计划协议发货
        IpMaster ShipScheduleLine(IList<ScheduleLineInput> scheduleLineInputList);
        #endregion

        #region 订单发货
        IpMaster ShipOrder(IList<OrderDetail> orderDetailList, bool isOpPallet = false);
        IpMaster ShipOrder(IList<OrderDetail> orderDetailList, DateTime effectiveDate, bool isOpPallet = false);
        IpMaster ShipOrder(IList<OrderDetail> orderDetailList, bool isCheckKitTraceItem, DateTime effectiveDate, bool isOpPallet = false);
        #endregion

        #region 拣货单发货
        IpMaster ShipPickList(string pickListNo);
        IpMaster ShipPickList(string pickListNo, DateTime effectiveDate);
        IpMaster ShipPickList(PickListMaster pickListMaster);
        IpMaster ShipPickList(PickListMaster pickListMaster, DateTime effectiveDate);
        IpMaster ShipPickList(IList<PickListDetail> pickListDetailList);
        IpMaster ShipPickList(IList<PickListDetail> pickListDetailList, DateTime effectiveDate);
        #endregion

        #region 订单收货
        ReceiptMaster ReceiveOrder(IList<OrderDetail> orderDetailList);
        ReceiptMaster ReceiveOrder(IList<OrderDetail> orderDetailList, DateTime effectiveDate);
        ReceiptMaster ReceiveOrder(IList<OrderDetail> orderDetailList, bool isCheckKitTraceItem, DateTime effectiveDate);
        #endregion

        #region 送货单收货
        ReceiptMaster ReceiveIp(IList<IpDetail> ipDetailList);
        ReceiptMaster ReceiveIp(IList<IpDetail> ipDetailList, DateTime effectiveDate);
        ReceiptMaster ReceiveIp(IList<IpDetail> ipDetailList, bool isCheckKitTraceItem, DateTime effectiveDate);
        #endregion

        #region 送货单差异调整
        ReceiptMaster AdjustIpGap(IList<IpDetail> ipDetailList, CodeMaster.IpGapAdjustOption ipGapAdjustOption);
        ReceiptMaster AdjustIpGap(IList<IpDetail> ipDetailList, CodeMaster.IpGapAdjustOption ipGapAdjustOption, DateTime effectiveDate);
        #endregion

        #region 取消订单
        void CancelOrder(string orderNo);
        void CancelOrder(OrderMaster orderMaster);
        #endregion

        #region 关闭订单
        void ManualCloseOrder(string orderNo);
        void ManualCloseOrder(OrderMaster orderMaster);
        void AutoCloseOrder();
        #endregion

        #region 生产单暂停
        void PauseProductOrder(string orderNo, int? pauseOperation);
        void PauseProductOrder(OrderMaster orderMaster, int? pauseOperation);
        #endregion

        #region 生产单暂停恢复
        void ReStartProductOrder(string orderNo, Int64 orderSequence);
        void ReStartProductOrder(OrderMaster orderMaster, Int64 orderSequence);
        #endregion

        #region 排序单创建
        IList<SequenceMaster> CreatSequenceOrder();
        #endregion

        #region 排序单装箱
        void PackSequenceOrder(string sequenceNo, IList<string> huIdList);

        void PackSequenceOrder(SequenceMaster sequenceMaster, IList<string> huIdList);
        #endregion

        #region 排序单装箱取消
        void UnPackSequenceOrder(string sequenceNo);

        void UnPackSequenceOrder(SequenceMaster sequenceMaster);
        #endregion

        #region 排序单发货
        IpMaster ShipSequenceOrder(string sequenceNo);

        IpMaster ShipSequenceOrder(string sequenceNo, DateTime effectiveDate);

        IpMaster ShipSequenceOrder(SequenceMaster sequenceMaster);

        IpMaster ShipSequenceOrder(SequenceMaster sequenceMaster, DateTime effectiveDate);
        #endregion

        #region 供应商排序发货
        IpMaster ShipSequenceOrderBySupplier(string sequenceNo);
        IpMaster ShipSequenceOrderBySupplier(string sequenceNo, DateTime effectiveDate);
        #endregion
        //object[] ExpandOrderOperationAndBomDetail(int orderDetail);
        //void AddOrderDetail(OrderDetail orderDetail);
        //void UpdateOrderDetail(OrderDetail orderDetail);
        //void DeleteOrderDetails(IList<int> orderDetailIds);
        //void AddOrderBinding(OrderBinding orderBinding);
        //void DeleteOrderBindings(IList<int> orderBindingIds);
        //IList<OrderOperation> ExpandOrderOperation(int orderDetail);
        //void AddOrderOperation(OrderOperation orderOperation);
        //void UpdateOrderOperations(IList<OrderOperation> orderOperations);
        //void DeleteOrderOperations(IList<int> orderOperationIds);
        //IList<OrderBomDetail> ExpandOrderOperation(int orderDetail);
        //void AddOrderBomDetail(OrderBomDetail orderBomDetail);
        //void UpdateOrderBomDetails(IList<OrderBomDetail> orderBomDetails);
        //void DeleteOrderBomDetails(IList<int> orderBomDetailIds);

        //void SimulateReleaseOrder(string orderNo);
        //void OrderATPCheck(string orderNo);
        //void PauseOrder(string orderNo);

        #region 加载订单
        OrderMaster LoadOrderMaster(string orderNo, bool includeDetail, bool includeOperation, bool includeBomDetail);
        #endregion

        #region 根据投料的条码查找投料的工位
        IList<OrderOperation> FindFeedOrderOperation(string orderNo, string huId);
        #endregion

        #region 路线可用性检查
        void CheckOrder(string orderNo);

        void CheckOrder(OrderMaster orderMaster);
        #endregion

        #region 创建不合格品移库单
        void CreateRejectTransfer(Location location, IList<RejectDetail> rejectDetailList);
        #endregion

        #region 创建不合格品退货单
        OrderMaster CreateReturnOrder(FlowMaster flowMaster, IList<RejectDetail> rejectDetailList);
        OrderMaster CreateReturnOrder(FlowMaster flowMaster, IList<InspectResult> inspectResultList);
        #endregion

        #region 创建待验品移库单
        void CreateInspectTransfer(Location location, IList<InspectDetail> inspectDetailList);
        #endregion

        #region 试制车拉料
        string[] CreateRequisitionList(string orderNo);

        string[] CreateRequisitionList(OrderMaster orderMaster);
        #endregion

        #region 紧急拉料
        string[] CreateEmTransferOrderFromXls(Stream inputStream);
        #endregion

        #region 自由移库
        string CreateTransferOrderFromXls(Stream inputStream, string regionFromCode, string regionToCode, DateTime effectiveDate);
        string CreateFreeTransferOrderMaster(string regionFromCode, string regionToCode, IList<OrderDetail> orderDetailList, DateTime effectiveDate);
        #endregion

        #region 页面条码移库
        string CreateHuTransferOrder(string flowCode, IList<string> huIdList, DateTime effectiveDate);
        #endregion

        #region 客户化功能
        #region 整车上线并投料（驾驶室上线投驾驶室，底盘上总装投底盘），并记录需要递延扣减
        void StartVanOrder(string orderNo);

        void StartVanOrder(string orderNo, string feedOrderNo);

        void StartVanOrder(string orderNo, string feedOrderNo, bool isForce);

        void StartVanOrder(string orderNo, IList<string> feedHuIdList);

        void StartVanOrder(string orderNo, IList<string> feedHuIdList, bool isForce);
        #endregion

        #region 空车上线
        void StartEmptyVanOrder(string flow);
        #endregion

        #region 整车/驾驶室/底盘下线
        ReceiptMaster ReceiveVanOrder(string orderNo);
        #endregion

        #region 递延扣减
        void DeferredFeed(string flow);
        #endregion

        #region 分装生产单下线
        void KitOrderOffline(IList<OrderDetail> orderDetailList, IList<string> feedKitOrderNoList);
        void KitOrderOffline(IList<OrderDetail> orderDetailList, IList<string> feedKitOrderNoList, bool isForceFeed);
        void KitOrderOffline(IList<OrderDetail> orderDetailList, IList<string> feedKitOrderNoList, DateTime effectiveDate);
        void KitOrderOffline(IList<OrderDetail> orderDetailList, IList<string> feedKitOrderNoList, bool isForceFeed, DateTime effectiveDate);
        #endregion

        #region 分装生产单下线并投料
        void KitOrderOfflineAndFeed(string kitOrderNo);
        #endregion

        #region Kit单投Kit单
        //void FeedKitOrder(string parentKitOrderNo, string childKitOrderNo);
        //void FeedKitOrder(string parentKitOrderNo, string childKitOrderNo, bool isForceFeed);
        //void FeedKitOrder(string parentKitOrderNo, string childKitOrderNo, DateTime effectiveDate);
        //void FeedKitOrder(string parentKitOrderNo, string childKitOrderNo, bool isForceFeed, DateTime effectiveDate);
        #endregion
        #endregion

        #region 交货单过账
        void DistributionReceiveOrder(OrderMaster orderMaster);
        #endregion

        #region 高级仓库发货
        void ProcessShipPlanResult4Hu(string transportOrderNo, IList<string> huIdList, DateTime? effDate);
        #endregion

        string CreateProcurementOrderFromXls(Stream inputStream, string flowCode, string extOrderNo, string refOrderNo,
            DateTime startTime, DateTime windowTime, CodeMaster.OrderPriority priority);

        OrderMaster GetAuthenticOrder(string orderNo);

        void DoItemTrace(string orderNo, List<string> huIdList);

        void CancelItemTrace(string orderNo, List<string> huIdList);

        decimal GetRoundOrderQty(FlowDetail flowDetail, decimal orderQty);
        decimal GetRoundOrderQty(OrderDetail orderDetail, decimal orderQty);

        void CleanOrder(List<string> flowCodeList);

        string PrintTraceCode();

        string PrintTraceCode(string orderNo);

        void ReportOrderOp(int op);

        string ReceiveTraceCode(IList<string> traceCodeList);

        IList<Hu> ReceiveTraceCode(IList<OrderDetail> orderDetList, IList<String> traceCodes);

        OrderMaster GetOrderMasterByOrderNoAndExtNo(string orderNo, bool includeDetail, bool includeOperation, bool includeBomDetail);
    }

    public interface ISequenceMgr
    {
        IList<SequenceMaster> CreateSequenceOrderByFlow(FlowStrategy flowStrategy, IList<object[]> orderDetailAryList);
    }
}
