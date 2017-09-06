using System;
using System.Collections.Generic;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.PRD;
using System.IO;

namespace com.Sconit.Service
{
    public interface IProductionLineMgr
    {
        #region 投料
        void FeedRawMaterial(string productLine, string productLineFacility, IList<FeedInput> feedInputList);

        void FeedRawMaterial(string productLine, string productLineFacility, IList<FeedInput> feedInputList, DateTime effectiveDate);

        void FeedRawMaterial(string productLine, string productLineFacility, IList<FeedInput> feedInputList, bool isForceFeed);

        void FeedRawMaterial(string productLine, string productLineFacility, IList<FeedInput> feedInputList, bool isForceFeed, DateTime effectiveDate);

        void FeedRawMaterial(string orderNo, IList<FeedInput> feedInputList);

        void FeedRawMaterial(string orderNo, IList<FeedInput> feedInputList, DateTime effectiveDate);

        void FeedRawMaterial(string orderNo, IList<FeedInput> feedInputList, bool isForceFeed);

        void FeedRawMaterial(string orderNo, IList<FeedInput> feedInputList, bool isForceFeed, DateTime effectiveDate);
        #endregion

        #region 生产单投料，投Kit单料
        void FeedKitOrder(string orderNo, string kitOrderNo);

        void FeedKitOrder(string orderNo, string kitOrderNo, bool isForceFeed);

        void FeedKitOrder(string orderNo, string kitOrderNo, DateTime effectiveDate);

        void FeedKitOrder(string orderNo, string kitOrderNo, bool isForceFeed, DateTime effectiveDate);
        #endregion

        #region 生产单投料，投工单
        void FeedProductOrder(string orderNo, string feedOrderNo);

        void FeedProductOrder(string orderNo, string feedOrderNo, bool isForceFeed);

        void FeedProductOrder(string orderNo, string feedOrderNo, DateTime effectiveDate);

        void FeedProductOrder(string orderNo, string feedOrderNo, bool isForceFeed, DateTime effectiveDate);
        #endregion

        #region 退料
        void ReturnRawMaterial(string productLine, string productLineFacility, IList<ReturnInput> returnInputList);

        void ReturnRawMaterial(string productLine, string productLineFacility, IList<ReturnInput> returnInputList, DateTime effectiveDate);

        void ReturnRawMaterial(string orderNo, string traceCode, Int32? operation, string opReference, IList<ReturnInput> returnInputList);

        void ReturnRawMaterial(string orderNo, string traceCode, Int32? operation, string opReference, IList<ReturnInput> returnInputList, DateTime effectiveDate);

        void ReturnRawMaterial(IList<ReturnInput> returnInputList);

        void ReturnRawMaterial(IList<ReturnInput> returnInputList, DateTime effectiveDate);
        #endregion

        #region 加权平均回冲
        void BackflushWeightAverage(string productLine, string productLineFacility, IList<WeightAverageBackflushInput> backflushInputList);

        void BackflushWeightAverage(string productLine, string productLineFacility, IList<WeightAverageBackflushInput> backflushInputList, DateTime effectiveDate);
        #endregion

        #region 回冲物料
        void BackflushProductOrder(IList<OrderDetail> orderDetailList);

        void BackflushProductOrder(IList<OrderDetail> orderDetailList, DateTime effectiveDate);

        void BackflushProductOrder(IList<OrderDetail> orderDetailList, OrderMaster orderMaster, DateTime effectiveDate);
        #endregion

        #region 生产线暂停
        void PauseProductLine(string productLineCode);
        #endregion

        #region 生产线恢复暂停
        void ReStartProductLine(string productLineCode);
        #endregion

        #region 导入投料/退料
        void FeedRawMaterialFromXls(Stream inputStream, string productLine, string productLineFacility, bool isForceFeed, DateTime effectiveDate);

        void FeedRawMaterialFromXls(Stream inputStream, string orderNo, bool isForceFeed, DateTime effectiveDate);
        #endregion
    }
}
