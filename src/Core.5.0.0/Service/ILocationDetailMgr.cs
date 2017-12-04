using System.Collections.Generic;
using com.Sconit.Entity.SYS;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.VIEW;
using System;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.INP;

namespace com.Sconit.Service
{
    public interface ILocationDetailMgr
    {
        LocationLotDetail GetHuLocationLotDetail(string huId);

        IList<LocationLotDetail> GetHuLocationLotDetails(IList<string> huIdList);

        IList<InventoryTransaction> InventoryOut(IpDetail ipDetail);

        IList<InventoryTransaction> InventoryOut(IpDetail ipDetail, DateTime effectiveDate);

        //IList<InventoryTransaction> CancelInventoryOut(IpDetail ipDetail);

        //IList<InventoryTransaction> CancelInventoryOut(IpDetail ipDetail, DateTime effectiveDate);

        IList<InventoryTransaction> InventoryIn(ReceiptDetail receiptDetail);

        IList<InventoryTransaction> InventoryIn(ReceiptDetail receiptDetail, DateTime effectiveDate);

        IList<InventoryTransaction> FeedProductRawMaterial(FeedInput feedInput);

        IList<InventoryTransaction> FeedProductRawMaterial(FeedInput feedInput, DateTime effectiveDate);

        IList<InventoryTransaction> ReturnProductRawMaterial(ReturnInput returnInput);

        IList<InventoryTransaction> ReturnProductRawMaterial(ReturnInput returnInput, DateTime effectiveDate);

        IList<InventoryTransaction> BackflushProductWeightAverageRawMaterial(IList<WeightAverageBackflushInput> backFlushInputList);

        IList<InventoryTransaction> BackflushProductWeightAverageRawMaterial(IList<WeightAverageBackflushInput> backFlushInputList, DateTime effectiveDate);

        IList<InventoryTransaction> BackflushProductMaterial(IList<BackflushInput> backflushInputList);

        IList<InventoryTransaction> BackflushProductMaterial(IList<BackflushInput> backflushInputList, DateTime effectiveDate);

        IList<InventoryTransaction> CancelBackflushProductMaterial(IList<BackflushInput> backflushInputList);

        IList<InventoryTransaction> CancelBackflushProductMaterial(IList<BackflushInput> backflushInputList, DateTime effectiveDate);

        IList<InventoryTransaction> InventoryInspect(InspectMaster inspectMaster);

        IList<InventoryTransaction> InventoryInspect(InspectMaster inspectMaster, DateTime effectiveDate);

        IList<InventoryTransaction> InspectJudge(InspectMaster inspectMaster, IList<InspectResult> inspectResultList);

        IList<InventoryTransaction> InspectJudge(InspectMaster inspectMaster, IList<InspectResult> inspectResultList, DateTime effectiveDate);

        IList<InventoryTransaction> ConcessionToUse(ConcessionMaster consessionMaster);

        IList<InventoryTransaction> ConcessionToUse(ConcessionMaster consessionMaster, DateTime effectiveDate);

        IList<InventoryTransaction> InventoryPack(IList<InventoryPack> inventoryPackList);

        IList<InventoryTransaction> InventoryPack(IList<InventoryPack> inventoryPackList, DateTime effectiveDate);

        IList<InventoryTransaction> InventoryUnPack(IList<InventoryUnPack> inventoryUnPackList);

        IList<InventoryTransaction> InventoryUnPack(IList<InventoryUnPack> inventoryUnPackList, DateTime effectiveDate);

        IList<InventoryTransaction> InventoryRePack(IList<InventoryRePack> inventoryRePackList);

        IList<InventoryTransaction> InventoryRePack(IList<InventoryRePack> inventoryRePackList, DateTime effectiveDate);

        IList<InventoryTransaction> StockTakeAdjust(StockTakeMaster stockTakeMaster, IList<StockTakeResult> stockTakeResultList);

        IList<InventoryTransaction> StockTakeAdjust(StockTakeMaster stockTakeMaster, IList<StockTakeResult> stockTakeResultList, DateTime effectiveDate);

        void InventoryPick(IList<InventoryPick> inventoryPickList);

        void InventoryPut(IList<InventoryPut> inventoryPutList);

        void InventoryPut(IList<StockTakeDetail> stockTakeDetailList);

        IList<LocationLotDetail> InventoryOccupy(IList<InventoryOccupy> inventoryOccupyList);

        IList<LocationLotDetail> CancelInventoryOccupy(CodeMaster.OccupyType occupyType, string occupyReferenceNo);

        IList<LocationLotDetail> CancelInventoryOccupy(CodeMaster.OccupyType occupyType, string occupyReferenceNo, IList<string> huIdList);

        IList<InventoryTransaction> InventoryOtherInOut(MiscOrderMaster miscOrderMaster, MiscOrderDetail miscOrderDetail);

        IList<InventoryTransaction> InventoryOtherInOut(MiscOrderMaster miscOrderMaster, MiscOrderDetail miscOrderDetail, DateTime effectiveDate);

        IList<InventoryTransaction> CancelInventoryOtherInOut(MiscOrderMaster miscOrderMaster);

        IList<InventoryTransaction> CancelInventoryOtherInOut(MiscOrderMaster miscOrderMaster, DateTime effectiveDate);

        //IList<HistoryInventory> GetHistoryLocationDetails(string locatoin, IList<string> itemList, DateTime historyDate, string SortDesc, int PageSize, int Page);

        //IList<HuHistoryInventory> GetHuHistoryLocationDetails(string locatoin, IList<string> itemList, DateTime historyDate);

        IList<InventoryTransaction> InventoryExchange(IList<ItemExchange> itemExchangeList);

        IList<InventoryTransaction> CancelInventoryExchange(IList<ItemExchange> itemExchangeList);

        void InventoryFreeze(IList<string> huIdList);

        void InventoryFreeze(IList<string> huIdList,string reason);

        void InventoryFreeze(string item, string location, string lotNo, string manufactureParty);

        void InventoryUnFreeze(IList<string> huIdList);

        void InventoryUnFreeze(IList<string> huIdList, string reason);

        void InventoryUnFreeze(string item, string location, string lotNo, string manufactureParty);

        #region 客户化代码
        IList<InventoryTransaction> InventoryRePack(IList<InventoryRePack> inventoryRePackList, Boolean isCheckOccupy, DateTime effectiveDate);

        #endregion

        IList<LocationDetailIOB> GetLocationDetailIOB(string location, string item, DateTime startDate, DateTime endDate);

        Dictionary<string, decimal> GetInvATPQty(string location, List<string> items);

        Dictionary<string, decimal> GetPurchaseInTransQty(string location, List<string> items);

        void SettleLocaitonLotDetail(List<Int64> locationLotDetIdList);

        IList<Hu> Repack(string huId, IList<ProductBarCode> checkedProductBarCodeList, IList<ProductBarCode> uncheckedProductBarCodeList);

        List<Hu> DevanningHu(Hu hu);


        void DeleteLocationBin(string id);
    }
}
