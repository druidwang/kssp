using System.Collections.Generic;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using System;
using System.IO;

namespace com.Sconit.Service
{
    public interface IStockTakeMgr
    {
        #region 创建盘点单
        void CreateStockTakeMaster(StockTakeMaster stockTakeMaster);
        #endregion

        #region 添加盘点库位
        void BatchUpdateStockTakeLocations(string stNo, IList<Location> addLocaitons, IList<Location> deleteLocations);

        void BatchUpdateStockTakeLocations(StockTakeMaster stockTakeMaster, IList<Location> addLocaitons, IList<Location> deleteLocations);

        #endregion

        #region 添加盘点零件
        void BatchUpdateStockTakeItems(string stNo, IList<Item> addItems, IList<Item> deleteItems);

        void BatchUpdateStockTakeItems(StockTakeMaster stockTakeMaster, IList<Item> addItems, IList<Item> deleteItems);
        #endregion

        #region 删除盘点单
        void DeleteStockTakeMaster(string stNo);

        void DeleteStockTakeMaster(StockTakeMaster stockTakeMaster);
        #endregion

        #region 释放盘点单
        void ReleaseStockTakeMaster(string stNo);

        void ReleaseStockTakeMaster(StockTakeMaster stockTakeMaster);
        #endregion

        #region 开始盘点
        void StartStockTakeMaster(string stNo);

        void StartStockTakeMaster(StockTakeMaster stockTakeMaster);
        #endregion

        #region 取消盘点
        void CancelStockTakeMaster(string stNo);

        void CancelStockTakeMaster(StockTakeMaster stockTakeMaster);
        #endregion

        #region 记录盘点结果
        void RecordStockTakeDetail(string stNo, IList<StockTakeDetail> stockTakeDetailList);

        void RecordStockTakeDetail(StockTakeMaster stockTakeMaster, IList<StockTakeDetail> stockTakeDetailList);
        #endregion

        #region 显示盘点结果
        IList<StockTakeResultSummary> ListStockTakeResult(string stNo, bool listShortage, bool listProfit, bool listMatch, IList<string> locationList, IList<string> binList, IList<string> itemList, DateTime? BaseInventoryDate);

        IList<StockTakeResultSummary> ListStockTakeResult(StockTakeMaster stockTakeMaster, bool listShortage, bool listProfit, bool listMatch, IList<string> locationList, IList<string> binList, IList<string> itemList, DateTime? BaseInventoryDate);

        IList<StockTakeResult> ListStockTakeResultDetail(string stNo, bool listShortage, bool listProfit, bool listMatch, IList<string> locationList, IList<string> binList, IList<string> itemList, DateTime? BaseInventoryDate);

        IList<StockTakeResult> ListStockTakeResultDetail(StockTakeMaster stockTakeMaster, bool listShortage, bool listProfit, bool listMatch, IList<string> locationList, IList<string> binList, IList<string> itemList, DateTime? BaseInventoryDate);
        #endregion

        #region 盘点完成
        void CompleteStockTakeMaster(string stNo, DateTime? baseInventoryDate);

        void CompleteStockTakeMaster(StockTakeMaster stockTakeMaster, DateTime? baseInventoryDate);
        #endregion

        #region 盘点调整
        void AdjustStockTakeResult(string stNo, DateTime? effectiveDate);

        void AdjustStockTakeResult(IList<int> stockTakeResultIdList, DateTime? effectiveDate);

        void AdjustStockTakeResult(IList<StockTakeResult> stockTakeResultList, DateTime? effectiveDate);
        #endregion

        #region 关闭盘点单
        void ManualCloseStockTakeMaster(string stNo);

        void ManualCloseStockTakeMaster(StockTakeMaster stockTakeMaster);
        #endregion

        #region 添加盘点明细
        void BatchUpdateStockTakeDetails(string stNo,
           IList<StockTakeDetail> addStockDetailList, IList<StockTakeDetail> updateStockDetailList, IList<StockTakeDetail> deleteStockDetailList);

        void BatchUpdateStockTakeDetails(StockTakeMaster stockTakeMaster,
            IList<StockTakeDetail> addStockDetailList, IList<StockTakeDetail> updateStockDetailList, IList<StockTakeDetail> deleteStockDetailList);
        #endregion

        #region 导入盘点明细
        void ImportStockTakeDetailFromXls(Stream inputStream, string stNo);
        #endregion

    }
}
