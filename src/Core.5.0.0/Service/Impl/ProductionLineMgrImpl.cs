using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Services.Transaction;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity;
using com.Sconit.Entity.VIEW;
using com.Sconit.Entity.CUST;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using com.Sconit.Utility;
using System.Collections;
using AutoMapper;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class ProductionLineMgrImpl : BaseMgr, IProductionLineMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IBomMgr bomMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public IItemMgr itemMgr { get; set; }
        public IHuMgr huMgr { get; set; }

        #region 生产投料
        [Transaction(TransactionMode.Requires)]
        public void FeedRawMaterial(string productLine, string productLineFacility, IList<FeedInput> feedInputList)
        {
            FeedRawMaterial(productLine, productLineFacility, null, feedInputList, false, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedRawMaterial(string productLine, string productLineFacility, IList<FeedInput> feedInputList, DateTime effectiveDate)
        {
            FeedRawMaterial(productLine, productLineFacility, null, feedInputList, false, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedRawMaterial(string productLine, string productLineFacility, IList<FeedInput> feedInputList, bool isForceFeed)
        {
            FeedRawMaterial(productLine, productLineFacility, null, feedInputList, isForceFeed, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedRawMaterial(string productLine, string productLineFacility, IList<FeedInput> feedInputList, bool isForceFeed, DateTime effectiveDate)
        {
            FeedRawMaterial(productLine, productLineFacility, null, feedInputList, isForceFeed, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedRawMaterial(string orderNo, IList<FeedInput> feedInputList)
        {
            FeedRawMaterial(orderNo, feedInputList, false, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedRawMaterial(string orderNo, IList<FeedInput> feedInputList, DateTime effectiveDate)
        {
            FeedRawMaterial(orderNo, feedInputList, false, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedRawMaterial(string orderNo, IList<FeedInput> feedInputList, bool isForceFeed)
        {
            FeedRawMaterial(orderNo, feedInputList, isForceFeed, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedRawMaterial(string orderNo, IList<FeedInput> feedInputList, bool isForceFeed, DateTime effectiveDate)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            //暂时只支持生产单一条明细
            int orderDetailId = genericMgr.FindAll<int>("select Id from OrderDetail where OrderNo = ?", orderNo).First();

            if (orderMaster.Type != com.Sconit.CodeMaster.OrderType.Production && orderMaster.Type != com.Sconit.CodeMaster.OrderType.SubContract)
            {
                throw new TechnicalException("非生产单不能进行投料。");
            }

            #region 检查生产单
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.InProcess && orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Complete)
            {
                throw new BusinessException("状态为{0}的生产单不能投料。", systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
            #endregion

            #region 生产单暂停检查
            if (orderMaster.IsPause)
            {
                throw new BusinessException("生产单{0}已经暂停，不能投料。", orderMaster.OrderNo);
            }
            #endregion

            #region 批量对投料对象赋订单的值
            if (feedInputList != null)
            {
                foreach (FeedInput feedInput in feedInputList)
                {
                    feedInput.OrderNo = orderNo;
                    feedInput.OrderDetailId = orderDetailId;
                    feedInput.TraceCode = orderMaster.TraceCode;
                    feedInput.CurrentOrderMaster = orderMaster;
                    feedInput.OrderType = orderMaster.Type;
                    feedInput.OrderSubType = orderMaster.SubType;
                    feedInput.AUFNR = orderMaster.ExternalOrderNo;  //SAP工单号
                    feedInput.ProductLineFacility = orderMaster.ProductLineFacility;
                    if (string.IsNullOrWhiteSpace(feedInput.Uom))
                    {
                        feedInput.Uom = this.itemMgr.GetCacheItem(feedInput.Item).Uom;
                    }
                    if (string.IsNullOrWhiteSpace(feedInput.LocationFrom))
                    {
                        feedInput.LocationFrom = orderMaster.LocationFrom;
                    }
                }
            }
            #endregion

            FeedRawMaterial(orderMaster.Flow, orderMaster.ProductLineFacility, orderNo, feedInputList, isForceFeed, effectiveDate);
        }

        #region 旧FeedRawMaterial
        //private void FeedRawMaterial(string productLine, string productLineFacility, string orderNo, int? operation, string opReference, IList<FeedInput> feedInputList, bool isForceFeed, DateTime effectiveDate)
        //{
        //    #region 投料不能为空检验
        //    IList<FeedInput> noneZeroFeedInputList = null;
        //    if (feedInputList != null)
        //    {
        //        noneZeroFeedInputList = feedInputList.Where(f => f.Qty > 0 || !string.IsNullOrWhiteSpace(f.HuId)).ToList();
        //    }
        //    if (noneZeroFeedInputList == null || noneZeroFeedInputList.Count == 0)
        //    {
        //        throw new BusinessException("投料零件不能为空。");
        //    }
        //    #endregion

        //    #region 查找条码填充库位、数量和质量状态等数据
        //    foreach (FeedInput feedInput in noneZeroFeedInputList)
        //    {
        //        if (!string.IsNullOrWhiteSpace(feedInput.HuId))
        //        {
        //            string hql = "from LocationLotDetail where HuId = ?";
        //            IList<LocationLotDetail> huLocationList = this.genericMgr.FindAll<LocationLotDetail>(hql, feedInput.HuId);
        //            huLocationList = huLocationList.Where(h => h.Qty > 0).ToList();

        //            if (huLocationList == null || huLocationList.Count == 0)
        //            {
        //                Hu hu = this.genericMgr.FindAll<Hu>("from Hu where HuId = ?", feedInput.HuId).SingleOrDefault();

        //                if (hu == null)
        //                {
        //                    hu = this.huMgr.ResolveHu(feedInput.HuId);
        //                }

        //                #region 装箱
        ////                 public string Location { get; set; }
        ////public string HuId { get; set; }
        ////public CodeMaster.OccupyType OccupyType { get; set; }
        ////public string OccupyReferenceNo { get; set; }
        ////public IList<Int32> LocationLotDetailIdList { get; set; }

        ////public HuStatus CurrentHu { get; set; }
        ////public Location CurrentLocation { get; set; }


        ////                this.locationDetailMgr.InventoryPack(
        //                #endregion
        //                //ResolveHu(string extHuId);
        //                throw new BusinessException("投料的条码{0}不在任何库位中。", feedInput.HuId);
        //            }
        //            else if (huLocationList.Count > 1)
        //            {
        //                throw new TechnicalException("Hu " + feedInput.HuId + " in more than 1 location.");
        //            }

        //            LocationLotDetail locationLotDetail = huLocationList[0];
        //            //todo 检验投料的条码是否是OrderBomDetail上指定的库位投料

        //            //不用判断冻结和占用，出库方法中会判断
        //            feedInput.Item = locationLotDetail.Item;
        //            feedInput.LocationFrom = locationLotDetail.Location;
        //            feedInput.LotNo = locationLotDetail.LotNo;
        //            feedInput.QualityType = locationLotDetail.QualityType;
        //            feedInput.Qty = locationLotDetail.HuQty;
        //            feedInput.Uom = locationLotDetail.HuUom;
        //            feedInput.BaseUom = locationLotDetail.BaseUom;
        //            feedInput.UnitQty = locationLotDetail.UnitQty;
        //        }
        //    }
        //    #endregion

        //    #region 非强制投料，检验投料零件是否有效
        //    if (!isForceFeed)
        //    {
        //        if (orderNo == null)
        //        {
        //            #region 检验生产线是否有投料的零件
        //            IList<BomDetail> bomDetailList = this.bomMgr.GetProductLineWeightAverageBomDetail(productLine);

        //            foreach (FeedInput feedInput in noneZeroFeedInputList)
        //            {
        //                #region 查找物料是否是生产线上投料的
        //                if (bomDetailList != null && bomDetailList.Count > 0)
        //                {
        //                    bool findMatch = (from det in bomDetailList
        //                                      where det.Item == feedInput.Item
        //                                      select det).ToList().Count > 0;

        //                    #region 判断是否后续物料
        //                    if (!findMatch)
        //                    {
        //                        IList<ItemDiscontinue> disConItems = this.itemMgr.GetParentItemDiscontinues(feedInput.Item, effectiveDate);
        //                        if (disConItems != null && disConItems.Count > 0)
        //                        {
        //                            findMatch = (from det in bomDetailList
        //                                         join disConItem in disConItems
        //                                         on det.Item equals disConItem.Item
        //                                         where disConItem.Bom == null || disConItem.Bom == det.Bom
        //                                         select det).ToList().Count > 0;
        //                        }
        //                    }
        //                    #endregion

        //                    if (!findMatch)
        //                    {
        //                        throw new BusinessException("生产线{0}中没有需要投料的零件{1}。", productLine, feedInput.Item);
        //                    }
        //                }
        //                else
        //                {
        //                    throw new BusinessException("生产线{0}上没有需要投料的零件。", productLine);
        //                }
        //                #endregion
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            #region 检验生产单上是否有投料的零件
        //            //todo 需要考虑一张生产单多个成品的情况，指定成品零件号/明细行号

        //            #region 查找生产单Bom
        //            string hql = "from OrderBomDetail where OrderNo = ?";
        //            IList<object> para = new List<object>();
        //            para.Add(orderNo);
        //            if (operation.HasValue)
        //            {
        //                hql += " and Operation = ? and OpReference = ?";
        //                para.Add(operation.Value);
        //                para.Add(opReference);
        //            }
        //            IList<OrderBomDetail> orderBomDetailList = this.genericMgr.FindAll<OrderBomDetail>(hql, para.ToArray());
        //            #endregion

        //            #region 查找已投料关键件
        //            hql = "from ProductLineLocationDetail where OrderNo = ? and HuId is not null and ReserveNo is not null and ReserveLine is not null";
        //            para = new List<object>();
        //            para.Add(orderNo);
        //            if (operation.HasValue)
        //            {
        //                hql += " and Operation = ? and OpReference = ?";
        //                para.Add(operation.Value);
        //                para.Add(opReference);
        //            }
        //            IList<ProductLineLocationDetail> feedProductLineLocationDetailList = this.genericMgr.FindAll<ProductLineLocationDetail>(hql, para.ToArray());
        //            #endregion

        //            foreach (FeedInput feedInput in noneZeroFeedInputList)
        //            {
        //                #region 找到候选的生产单Bom
        //                IList<OrderBomDetail> matchedOrderBomDetailList = orderBomDetailList.Where(det => det.Item == feedInput.Item).ToList();

        //                #region 判断是否后续物料
        //                if (matchedOrderBomDetailList == null || matchedOrderBomDetailList.Count == 0)
        //                {
        //                    IList<ItemDiscontinue> disConItems = this.itemMgr.GetParentItemDiscontinues(feedInput.Item, effectiveDate);
        //                    matchedOrderBomDetailList = (from det in orderBomDetailList
        //                                                 join disConItem in disConItems
        //                                                 on det.Item equals disConItem.Item
        //                                                 where disConItem.Bom == null || disConItem.Bom == det.Bom
        //                                                 select det).ToList();
        //                }
        //                #endregion
        //                #endregion

        //                if (matchedOrderBomDetailList != null && matchedOrderBomDetailList.Count > 0)
        //                {
        //                    #region 关键件扫描，查找对应的生产单Bom，取Bom上的SAP生产单号、生产批号、预留号、行号、移动类型
        //                    if (!string.IsNullOrWhiteSpace(feedInput.HuId))
        //                    {
        //                        //投料的零件和Bom必须单位一致
        //                        matchedOrderBomDetailList = matchedOrderBomDetailList.Where(m => m.Uom == feedInput.Uom).ToList();
        //                        if (matchedOrderBomDetailList != null && matchedOrderBomDetailList.Count > 0)
        //                        {
        //                            #region 投料物料和生产单BOM循环匹配
        //                            bool findMatch = false;  //是否找到对应的生产单
        //                            foreach (OrderBomDetail matchedOrderBomDetail in matchedOrderBomDetailList.OrderBy(d => d.Operation)) //按照工序的顺序先后匹配
        //                            {
        //                                #region 根据预留号和行号汇总已投料数量
        //                                //已投料量
        //                                decimal feedQty = feedProductLineLocationDetailList.Where(locDet => locDet.ReserveNo == matchedOrderBomDetail.ReserveNo
        //                                    && locDet.ReserveLine == matchedOrderBomDetail.ReserveLine)
        //                                    .Sum(locDet => locDet.Qty);  //库存单位

        //                                //todo考虑指定预留号和行号投料
        //                                ////+本次投料量
        //                                //feedQty += noneZeroFeedInputList.Where(i => i.ReserveNo == matchedOrderBomDetail.ReserveNo
        //                                //    && i.ReserveLine == matchedOrderBomDetail.ReserveLine)
        //                                //    .Sum(i => i.Qty * i.UnitQty);  //库存单位
        //                                #endregion

        //                                #region 生产单Bom用量是否大于已投料+本次投料量
        //                                if (matchedOrderBomDetail.OrderedQty * matchedOrderBomDetail.UnitQty >= (feedQty + feedInput.Qty * feedInput.UnitQty))
        //                                {
        //                                    feedInput.ReserveNo = matchedOrderBomDetail.ReserveNo;
        //                                    feedInput.ReserveLine = matchedOrderBomDetail.ReserveLine;
        //                                    feedInput.AUFNR = matchedOrderBomDetail.AUFNR;
        //                                    feedInput.BWART = matchedOrderBomDetail.BWART;
        //                                    feedInput.ICHARG = matchedOrderBomDetail.ICHARG;

        //                                    findMatch = true;
        //                                }
        //                                #endregion
        //                            }

        //                            if (!findMatch)
        //                            {
        //                                throw new BusinessException("生产单{0}的零件{1}已经投料。", orderNo, feedInput.Item);
        //                            }
        //                            #endregion
        //                        }
        //                        else
        //                        {
        //                            throw new BusinessException("投料的零件{1}和生产单{0}中的Bom单位不一致。", feedInput.Item, orderNo);
        //                        }
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    throw new BusinessException("生产单{0}中没有需要投料的零件{1}。", orderNo, feedInput.Item);
        //                }
        //            }
        //            #endregion
        //        }
        //    }
        //    else
        //    {
        //        #region 强制投料，检查投料至工单的零件是否都有SAP生产单号和移动类型，没有需要赋工单上的SAP工单号和默认261移动类型
        //        if (orderNo != null)
        //        {
        //            string AUFNR = string.Empty;
        //            foreach (FeedInput feedInput in noneZeroFeedInputList)
        //            {
        //                if (!string.IsNullOrWhiteSpace(feedInput.AUFNR))
        //                {
        //                    if (AUFNR == string.Empty)
        //                    {
        //                        AUFNR = this.genericMgr.FindAll<string>("select ExternalOrderNo from OrderMaster where OrderNo = ?", orderNo).Single();
        //                    }
        //                    feedInput.AUFNR = AUFNR;
        //                }

        //                if (!string.IsNullOrWhiteSpace(feedInput.BWART))
        //                {
        //                    feedInput.BWART = "261";
        //                }
        //            }
        //        }
        //        #endregion
        //    }
        //    #endregion

        //    #region 缓存出库需要的字段
        //    foreach (FeedInput feedInput in noneZeroFeedInputList)
        //    {
        //        feedInput.ProductLine = productLine;
        //        feedInput.ProductLineFacility = productLineFacility;
        //        feedInput.CurrentProductLine = this.genericMgr.FindById<FlowMaster>(productLine);
        //        #region 生产线暂停检查
        //        if (feedInput.CurrentProductLine.IsPause)
        //        {
        //            throw new BusinessException("生产线{0}已经暂停，不能投料。", feedInput.ProductLine);
        //        }
        //        #endregion
        //        if (string.IsNullOrWhiteSpace(feedInput.LocationFrom))
        //        {
        //            throw new TechnicalException("LocationFrom not specified in FeedInput.");
        //        }
        //        feedInput.CurrentLocationFrom = this.genericMgr.FindById<Location>(feedInput.LocationFrom);
        //        feedInput.CurrentItem = this.genericMgr.FindById<Item>(feedInput.Item);

        //        if (string.IsNullOrWhiteSpace(feedInput.HuId))
        //        {

        //            if (string.IsNullOrWhiteSpace(feedInput.Uom))
        //            {
        //                throw new TechnicalException("Uom not specified in FeedInput.");
        //            }
        //            feedInput.BaseUom = feedInput.CurrentItem.Uom;
        //            if (feedInput.CurrentItem.Uom != feedInput.Uom)
        //            {
        //                feedInput.UnitQty = this.itemMgr.ConvertItemUomQty(feedInput.Item, feedInput.Uom, 1, feedInput.CurrentItem.Uom);  //记录单位转换系数
        //            }
        //            else
        //            {
        //                feedInput.UnitQty = 1;
        //            }
        //        }
        //    }
        //    #endregion

        //    #region 循环操作库存
        //    foreach (FeedInput feedInput in noneZeroFeedInputList)
        //    {
        //        #region 投料
        //        IList<InventoryTransaction> inventoryTransactionList = this.locationDetailMgr.FeedProductRawMaterial(feedInput, effectiveDate);
        //        #endregion
        //    }
        //    #endregion
        //}
        #endregion

        #region 新FeedRawMaterial 投料并消耗,结算,同时记录回冲数.库存事务只记录ISS-MIN
        private void FeedRawMaterial(string productLine, string productLineFacility, string orderNo, IList<FeedInput> feedInputList, bool isForceFeed, DateTime effectiveDate)
        {
            #region 投料不能为空检验
            IList<FeedInput> noneZeroFeedInputList = null;
            if (feedInputList != null)
            {
                noneZeroFeedInputList = feedInputList.Where(f => f.Qty > 0 || !string.IsNullOrWhiteSpace(f.HuId)).ToList();
            }
            if (noneZeroFeedInputList == null || noneZeroFeedInputList.Count == 0)
            {
                throw new BusinessException("投料零件不能为空。");
            }
            #endregion

            #region 查找条码填充数量和质量状态等数据
            foreach (FeedInput feedInput in noneZeroFeedInputList)
            {
                if (!string.IsNullOrWhiteSpace(feedInput.HuId))
                {
                    HuStatus huStatus = this.huMgr.GetHuStatus(feedInput.HuId);
                    //string hql = "from LocationLotDetail where HuId = ?";
                    //IList<LocationLotDetail> huLocationList = this.genericMgr.FindAll<LocationLotDetail>(hql, feedInput.HuId);
                    //huLocationList = huLocationList.Where(h => h.Qty > 0).ToList();

                    if (string.IsNullOrWhiteSpace(huStatus.Location))
                    {
                        throw new BusinessException("投料的条码{0}不在任何库位中。", feedInput.HuId);
                    }
                    else if (!string.IsNullOrWhiteSpace(huStatus.LocationFrom) || !string.IsNullOrWhiteSpace(huStatus.LocationTo))
                    {
                        throw new BusinessException("投料的条码{0}是在途库存，不能投料。", feedInput.HuId);
                    }
                    //else if (huLocationList.Count > 1)
                    //{
                    //    throw new TechnicalException("Hu " + feedInput.HuId + " in more than 1 location.");
                    //}

                    //LocationLotDetail locationLotDetail = huLocationList[0];
                    //todo 检验投料的条码是否是OrderBomDetail上指定的库位投料

                    //不用判断冻结和占用，出库方法中会判断
                    feedInput.Item = huStatus.Item;
                    feedInput.LocationFrom = huStatus.Location;
                    feedInput.LotNo = huStatus.LotNo;
                    feedInput.QualityType = huStatus.QualityType;
                    feedInput.Qty = huStatus.Qty;
                    feedInput.Uom = huStatus.Uom;
                    feedInput.BaseUom = huStatus.BaseUom;
                    feedInput.UnitQty = huStatus.UnitQty;
                }
            }
            #endregion

            #region 非强制投料，检验投料零件是否有效
            if (!isForceFeed)
            {
                if (orderNo == null)
                {
                    #region 检验生产线是否有投料的零件
                    IList<BomDetail> bomDetailList = this.bomMgr.GetProductLineWeightAverageBomDetail(productLine);

                    foreach (FeedInput feedInput in noneZeroFeedInputList)
                    {
                        #region 查找物料是否是生产线上投料的
                        if (bomDetailList != null && bomDetailList.Count > 0)
                        {
                            bool findMatch = (from det in bomDetailList
                                              where det.Item == feedInput.Item
                                              select det).ToList().Count > 0;

                            #region 判断是否后续物料
                            if (!findMatch)
                            {
                                IList<ItemDiscontinue> disConItems = this.itemMgr.GetParentItemDiscontinues(feedInput.Item, effectiveDate);
                                if (disConItems != null && disConItems.Count > 0)
                                {
                                    findMatch = (from det in bomDetailList
                                                 join disConItem in disConItems
                                                 on det.Item equals disConItem.Item
                                                 where disConItem.Bom == null || disConItem.Bom == det.Bom
                                                 select det).ToList().Count > 0;
                                }
                            }
                            #endregion

                            if (!findMatch)
                            {
                                throw new BusinessException("生产线{0}中没有需要投料的零件{1}。", productLine, feedInput.Item);
                            }
                        }
                        else
                        {
                            throw new BusinessException("生产线{0}上没有需要投料的零件。", productLine);
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 检验生产单上是否有投料的零件
                    //todo 需要考虑一张生产单多个成品的情况，指定成品零件号/明细行号
                    foreach (FeedInput feedInput in noneZeroFeedInputList)
                    {
                        #region 查找生产单Bom
                        string hql = "from OrderBomDetail where OrderNo = ? and Item = ?";
                        IList<object> para = new List<object>();
                        para.Add(orderNo);
                        para.Add(feedInput.Item);
                        if (string.IsNullOrWhiteSpace(feedInput.HuId))
                        {
                            hql += " and Location = ?";
                            para.Add(feedInput.LocationFrom);
                        }
                        IList<OrderBomDetail> matchedOrderBomDetailList = this.genericMgr.FindAll<OrderBomDetail>(hql, para.ToArray());
                        #endregion

                        #region 查找已投料关键件
                        hql = "from ProductLineLocationDetail where OrderNo = ? and Item = ? and HuId is not null and ReserveNo is not null and ReserveLine is not null and IsClose = ?";
                        para = new List<object>();
                        para.Add(orderNo);
                        para.Add(feedInput.Item);
                        para.Add(false);
                        if (string.IsNullOrWhiteSpace(feedInput.HuId))
                        {
                            hql += " and LocationFrom = ?";
                            para.Add(feedInput.LocationFrom);
                        }
                        IList<ProductLineLocationDetail> feedProductLineLocationDetailList = this.genericMgr.FindAll<ProductLineLocationDetail>(hql, para.ToArray());
                        #endregion

                        if (matchedOrderBomDetailList != null && matchedOrderBomDetailList.Count > 0)
                        {
                            #region 关键件扫描，查找对应的生产单Bom，取Bom上的SAP生产单号、生产批号、预留号、行号、移动类型
                            //投料的零件和Bom必须单位一致
                            matchedOrderBomDetailList = matchedOrderBomDetailList.Where(m => m.Uom == feedInput.Uom).ToList();
                            if (matchedOrderBomDetailList != null && matchedOrderBomDetailList.Count > 0)
                            {
                                #region 投料物料和生产单BOM循环匹配
                                bool findMatch = false;  //是否找到对应的生产单
                                foreach (OrderBomDetail matchedOrderBomDetail in matchedOrderBomDetailList) //按照工序的顺序先后匹配
                                {
                                    #region 根据预留号和行号汇总已投料数量
                                    //已投料量
                                    decimal feedQty = feedProductLineLocationDetailList.Where(locDet => locDet.ReserveNo == matchedOrderBomDetail.ReserveNo
                                        && locDet.ReserveLine == matchedOrderBomDetail.ReserveLine)
                                        .Sum(locDet => locDet.Qty - locDet.BackFlushQty - locDet.VoidQty);  //库存单位
                                    #endregion

                                    #region 生产单Bom用量是否大于已投料+本次投料量
                                    if (matchedOrderBomDetail.OrderedQty * matchedOrderBomDetail.UnitQty >= (feedQty + feedInput.Qty * feedInput.UnitQty))
                                    {
                                        feedInput.Operation = matchedOrderBomDetail.Operation;
                                        feedInput.OpReference = matchedOrderBomDetail.OpReference;
                                        feedInput.ReserveNo = matchedOrderBomDetail.ReserveNo;
                                        feedInput.ReserveLine = matchedOrderBomDetail.ReserveLine;
                                        feedInput.AUFNR = matchedOrderBomDetail.AUFNR;
                                        feedInput.BWART = matchedOrderBomDetail.BWART;
                                        feedInput.ICHARG = matchedOrderBomDetail.ICHARG;

                                        findMatch = true;
                                        break;
                                    }
                                    #endregion
                                }

                                if (!findMatch)
                                {
                                    throw new BusinessException("生产单{0}的零件{1}已经投料。", orderNo, feedInput.Item);
                                }
                                #endregion
                            }
                            else
                            {
                                throw new BusinessException("投料的零件{1}和生产单{0}中的Bom单位不一致。", orderNo, feedInput.Item);
                            }
                            #endregion
                        }
                        else
                        {
                            throw new BusinessException("生产单{0}中没有需要投料的零件{1}。", orderNo, feedInput.Item);
                        }
                    }
                    #endregion
                }
            }
            else
            {
                #region 强制投料，检查投料至工单的零件是否都有SAP生产单号和移动类型，没有需要赋工单上的SAP工单号和默认261移动类型
                //if (orderNo != null)
                //{
                //    string AUFNR = string.Empty;
                //    foreach (FeedInput feedInput in noneZeroFeedInputList)
                //    {
                //        if (!string.IsNullOrWhiteSpace(feedInput.AUFNR))
                //        {
                //            if (AUFNR == string.Empty)
                //            {
                //                AUFNR = this.genericMgr.FindAll<string>("select ExternalOrderNo from OrderMaster where OrderNo = ?", orderNo).Single();
                //            }
                //            feedInput.AUFNR = AUFNR;
                //        }

                //        if (!string.IsNullOrWhiteSpace(feedInput.BWART))
                //        {
                //            feedInput.BWART = "261";
                //        }
                //    }
                //}
                #endregion
            }
            #endregion

            #region 缓存出库需要的字段
            IList<Location> locationList = this.GetLocations(noneZeroFeedInputList.Select(f => f.LocationFrom).Distinct().ToList());
            IList<Item> feedItemList = this.itemMgr.GetItems(noneZeroFeedInputList.Select(f => f.Item).Distinct().ToList());
            foreach (FeedInput feedInput in noneZeroFeedInputList)
            {
                feedInput.ProductLine = productLine;
                feedInput.ProductLineFacility = productLineFacility;
                feedInput.CurrentProductLine = this.genericMgr.FindById<FlowMaster>(productLine);
                #region 生产线暂停检查
                if (feedInput.CurrentProductLine.IsPause)
                {
                    throw new BusinessException("生产线{0}已经暂停，不能投料。", feedInput.ProductLine);
                }
                #endregion
                if (string.IsNullOrWhiteSpace(feedInput.LocationFrom))
                {
                    throw new TechnicalException("LocationFrom not specified in FeedInput.");
                }
                //feedInput.CurrentLocationFrom = this.genericMgr.FindById<Location>(feedInput.LocationFrom);
                //feedInput.CurrentItem = this.genericMgr.FindById<Item>(feedInput.Item);
                feedInput.CurrentLocationFrom = locationList.Where(i => i.Code == feedInput.LocationFrom).Single();
                feedInput.CurrentItem = feedItemList.Where(i => i.Code == feedInput.Item).Single();

                if (string.IsNullOrWhiteSpace(feedInput.HuId))
                {

                    if (string.IsNullOrWhiteSpace(feedInput.Uom))
                    {
                        throw new TechnicalException("Uom not specified in FeedInput.");
                    }
                    feedInput.BaseUom = feedInput.CurrentItem.Uom;
                    if (feedInput.CurrentItem.Uom != feedInput.Uom)
                    {
                        feedInput.UnitQty = this.itemMgr.ConvertItemUomQty(feedInput.Item, feedInput.Uom, 1, feedInput.CurrentItem.Uom);  //记录单位转换系数
                    }
                    else
                    {
                        feedInput.UnitQty = 1;
                    }
                }
            }
            #endregion

            List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
            #region 循环操作库存
            foreach (FeedInput feedInput in noneZeroFeedInputList)
            {
                #region 投料
                inventoryTransactionList.AddRange(this.locationDetailMgr.FeedProductRawMaterial(feedInput, effectiveDate));
                #endregion
            }
            #endregion

            #region 记录工单投料明细
            var orderDetail = this.genericMgr.FindById<OrderDetail>(noneZeroFeedInputList[0].OrderDetailId.Value);

            DateTime dateTimeNow = DateTime.Now;
            User currentUser = SecurityContextHolder.Get();
            var orderBackflushDetailList = from trans in inventoryTransactionList
                                           group trans by trans.PlanBill into g
                                           select new OrderBackflushDetail
                                           {
                                               OrderNo = orderNo,
                                               OrderDetailId = orderDetail.Id,
                                               OrderDetailSequence = orderDetail.Sequence,
                                               //OrderBomDetailId = backflushInput.OrderBomDetail.Id,
                                               //OrderBomDetailSequence = backflushInput.OrderBomDetail.Sequence,
                                               //ReceiptNo = backflushInput.ReceiptNo,
                                               //ReceiptDetailId = backflushInput.ReceiptDetailId,
                                               //ReceiptDetailSequence = backflushInput.ReceiptDetailSequence,
                                               Bom = orderDetail.Bom,
                                               FGItem = orderDetail.Item,
                                               Item = g.First().Item,
                                               ItemDescription = itemMgr.GetCacheItem(g.First().Item).Description,
                                               ReferenceItemCode = itemMgr.GetCacheItem(g.First().Item).ReferenceCode,
                                               Uom = itemMgr.GetCacheItem(g.First().Item).Uom,
                                               BaseUom = itemMgr.GetCacheItem(g.First().Item).Uom,
                                               UnitQty = 1,
                                               //ManufactureParty = g.First().OrderBomDetail.ManufactureParty,
                                               //TraceCode = backflushInput.TraceCode,
                                               //HuId = backflushInput.HuId,
                                               //LotNo = backflushInput.LotNo,
                                               //Operation = backflushInput.Operation,
                                               //OpReference = backflushInput.OpReference,
                                               BackflushedQty = g.Sum(trans => trans.Qty),
                                               //BackflushedRejectQty = g.First().FGQualityType == CodeMaster.QualityType.Reject ? g.Sum(trans => trans.Qty) / g.First().UnitQty : 0,
                                               //BackflushedScrapQty = input.BackflushedQty,
                                               LocationFrom = orderDetail.LocationFrom,
                                               ProductLine = productLine,
                                               //ProductLineFacility = backflushInput.ProductLineFacility,
                                               //ReserveNo = backflushInput.OrderBomDetail.ReserveNo,
                                               //ReserveLine = backflushInput.OrderBomDetail.ReserveLine,
                                               //AUFNR = backflushInput.OrderBomDetail.AUFNR,
                                               //ICHARG = backflushInput.OrderBomDetail.ICHARG,
                                               //BWART = backflushInput.OrderBomDetail.BWART,
                                               NotReport = false,  //理论都需要汇报
                                               PlanBill = g.Key,
                                               EffectiveDate = effectiveDate,
                                               CreateUserId = currentUser.Id,
                                               CreateUserName = currentUser.FullName,
                                               CreateDate = dateTimeNow,
                                               IsVoid = false,
                                               ProductLineFacility = productLineFacility
                                           };

            foreach (OrderBackflushDetail orderBackflushDetail in orderBackflushDetailList)
            {
                this.genericMgr.Create(orderBackflushDetail);
            }
            #endregion
        }
        #endregion

        #region 导入投料
        public void FeedRawMaterialFromXls(Stream inputStream, string productLine, string productLineFacility, bool isForceFeed, DateTime effectiveDate)
        {
            FeedRawMaterialFromXls(inputStream, productLine, productLineFacility, null, isForceFeed, effectiveDate);
        }

        public void FeedRawMaterialFromXls(Stream inputStream, string orderNo, bool isForceFeed, DateTime effectiveDate)
        {
            FeedRawMaterialFromXls(inputStream, null, null, orderNo, isForceFeed, effectiveDate);
        }

        private void FeedRawMaterialFromXls(Stream inputStream, string productLine, string productLineFacility, string orderNo, bool isForceFeed, DateTime effectiveDate)
        {
            #region 导入数据
            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 11);

            #region 列定义
            int colItem = 1;//物料代码   
            int colUom = 3;//单位
            int colLocFrom = 4;// 来源库位
            int colQty = 5;//数量
            #endregion

            IList<FeedInput> feedInputList = new List<FeedInput>();
            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 1, 9))
                {
                    break;//边界
                }
                string itemCode = string.Empty;
                decimal qty = 0;
                string uomCode = string.Empty;
                string locationFromCode = string.Empty;
                string locationToCode = string.Empty;

                #region 读取数据
                #region 读取物料代码
                itemCode = ImportHelper.GetCellStringValue(row.GetCell(colItem));
                if (itemCode == null || itemCode.Trim() == string.Empty)
                {
                    ImportHelper.ThrowCommonError(row.RowNum, colItem, row.GetCell(colItem));
                }

                #endregion

                #region 读取单位
                uomCode = row.GetCell(colUom) != null ? row.GetCell(colUom).StringCellValue : string.Empty;
                if (uomCode == null || uomCode.Trim() == string.Empty)
                {
                    throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colUom.ToString());
                }
                #endregion

                #region 读取来源库位
                locationFromCode = row.GetCell(colLocFrom) != null ? row.GetCell(colLocFrom).StringCellValue : string.Empty;
                if (string.IsNullOrEmpty(locationFromCode))
                {
                    throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colLocFrom.ToString());
                }

                Location locationFrom = genericMgr.FindById<Location>(locationFromCode);
                #endregion

                #region 读取数量
                try
                {
                    qty = Convert.ToDecimal(row.GetCell(colQty).NumericCellValue);
                }
                catch
                {
                    ImportHelper.ThrowCommonError(row.RowNum, colQty, row.GetCell(colQty));
                }
                #endregion
                #endregion

                #region 填充数据
                FeedInput feedInput = new FeedInput();
                feedInput.LocationFrom = locationFromCode;

                feedInput.Item = itemCode;
                feedInput.Uom = uomCode;
                feedInput.Qty = qty;
                feedInput.BaseUom = uomCode;

                feedInputList.Add(feedInput);
                #endregion
            }

            #endregion

            #region 投料
            FeedRawMaterial(productLine, productLineFacility, orderNo, feedInputList, isForceFeed, effectiveDate);
            #endregion
        }
        #endregion

        #endregion

        #region 生产单投料，投Kit单料
        [Transaction(TransactionMode.Requires)]
        public void FeedKitOrder(string orderNo, string kitOrderNo)
        {
            FeedKitOrder(orderNo, kitOrderNo, false, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedKitOrder(string orderNo, string kitOrderNo, bool isForceFeed)
        {
            FeedKitOrder(orderNo, kitOrderNo, isForceFeed, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedKitOrder(string orderNo, string kitOrderNo, DateTime effectiveDate)
        {
            FeedKitOrder(orderNo, kitOrderNo, false, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedKitOrder(string orderNo, string kitOrderNo, bool isForceFeed, DateTime effectiveDate)
        {
            #region 查询订单
            IList<OrderMaster> orderMasterList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where OrderNo in (?, ?)",
                new object[] { orderNo, kitOrderNo });
            #endregion

            #region 检查
            OrderMaster productOrder = orderMasterList.Where(o => o.OrderNo == orderNo).Single();
            OrderMaster kitOrder = orderMasterList.Where(o => o.OrderNo == kitOrderNo).Single();

            if (productOrder.Type != CodeMaster.OrderType.Production
                || productOrder.Type == CodeMaster.OrderType.SubContract)
            {
                throw new TechnicalException("ProductOrder type is not correct.");
            }

            if (kitOrder.OrderStrategy != CodeMaster.FlowStrategy.KIT)
            {
                throw new TechnicalException("KitOrder strategy is not correct.");
            }

            if (productOrder.Status != CodeMaster.OrderStatus.InProcess
                && productOrder.Status != CodeMaster.OrderStatus.Complete)
            {
                throw new BusinessException("生产单{0}的状态为{1}，不能投料。", productOrder.OrderNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)productOrder.Status).ToString()));
            }

            if (kitOrder.Status != CodeMaster.OrderStatus.Close
                && kitOrder.Status != CodeMaster.OrderStatus.Complete)
            {
                throw new BusinessException("KIT单{0}没有收货，不能投料。", kitOrder.OrderNo);
            }

            if (!isForceFeed && productOrder.TraceCode != kitOrder.TraceCode)
            {
                throw new BusinessException("KIT单{0}的VAN号{1}和生产单的VAN号{2}不一致。", kitOrder.OrderNo, kitOrder.TraceCode, productOrder.TraceCode);
            }

            #region 查询Kit单是否已经投料
            if (this.genericMgr.FindAll<long>("select count(*) as counter from ProductFeed where FeedOrder = ?", kitOrder.OrderNo)[0] > 0)
            {
                throw new BusinessException("KIT单{0}已经投料。", kitOrder.OrderNo);
            }
            #endregion
            #endregion

            #region 根据工位找工序
            OrderOperation orderOperation = this.genericMgr.FindAll<OrderOperation>("from OrderOperation where OrderNo = ? and OpReference = ?",
                new object[] { productOrder.OrderNo, kitOrder.LocationTo }).FirstOrDefault();
            #endregion

            #region 查询Kit收货单明细
            //循环查找kit单的绑定订单，要全部投料至生产线上。
            IList<string> childKitOrderNoList = NestGetChildKitOrderNo(kitOrder.OrderNo);
            string selectKitReceiptStatement = NativeSqlStatement.SELECT_KIT_RECEIPT_STATEMENT;
            IList<object> selectKitReceiptParm = new List<object>();
            selectKitReceiptParm.Add(kitOrder.OrderNo);
            if (childKitOrderNoList != null && childKitOrderNoList.Count > 0)
            {
                foreach (string childKitOrderNo in childKitOrderNoList)
                {
                    selectKitReceiptStatement += ",?";
                    selectKitReceiptParm.Add(childKitOrderNo);
                }
            }
            selectKitReceiptStatement += ")";
            IList<ReceiptLocationDetail> receiptLocationDetailList = this.genericMgr.FindEntityWithNativeSql<ReceiptLocationDetail>(selectKitReceiptStatement, selectKitReceiptParm.ToArray());
            IList<Item> itemList = this.itemMgr.GetItems(receiptLocationDetailList.Select(det => det.Item).Distinct().ToList());
            //查找订单明细，需要取Kit单的预留号、行号、SAP生产单号、批号、移动类型
            IList<OrderDetail> orderDetailList = this.LoadOrderDetails(receiptLocationDetailList.Select(r => r.OrderDetailId.Value).Distinct().ToArray());

            IList<FeedInput> feedInputList = (from det in receiptLocationDetailList
                                              group det by new
                                              {
                                                  Item = det.Item,
                                                  QualityType = det.QualityType,
                                                  HuId = det.HuId,
                                                  ReserveNo = orderDetailList.Where(od => od.Id == det.OrderDetailId).Single().ReserveNo,
                                                  ReserveLine = orderDetailList.Where(od => od.Id == det.OrderDetailId).Single().ReserveLine,
                                                  AUFNR = orderDetailList.Where(od => od.Id == det.OrderDetailId).Single().AUFNR,
                                                  ICHARG = orderDetailList.Where(od => od.Id == det.OrderDetailId).Single().ICHARG,
                                                  BWART = orderDetailList.Where(od => od.Id == det.OrderDetailId).Single().BWART,
                                              } into result
                                              select new FeedInput
                                              {
                                                  Item = result.Key.Item,
                                                  QualityType = result.Key.QualityType,
                                                  HuId = result.Key.HuId,
                                                  LocationFrom = kitOrder.LocationTo,  //投料的扣料库位为Kit单的目的库位
                                                  Uom = itemList.Where(i => i.Code == result.Key.Item).Single().Uom,
                                                  ReserveNo = result.Key.ReserveNo,
                                                  ReserveLine = result.Key.ReserveLine,
                                                  AUFNR = result.Key.AUFNR,
                                                  ICHARG = result.Key.ICHARG,
                                                  BWART = result.Key.BWART,
                                                  Qty = result.Sum(det => det.Qty)
                                              }).ToList();

            #region FeedInput的ITEM赋基本单位
            SetUom4FeedInput(feedInputList);
            #endregion
            #endregion

            #region 记录Kit单投料
            ProductFeed productFeed = new ProductFeed();
            productFeed.TraceCode = productOrder.TraceCode;
            productFeed.FeedOrder = kitOrderNo;
            productFeed.ProductOrder = orderNo;

            this.genericMgr.Create(productFeed);
            #endregion

            #region 投料
            FeedRawMaterial(orderNo, feedInputList, true, effectiveDate);
            #endregion
        }

        private IList<string> NestGetChildKitOrderNo(string orderNo)
        {
            IList<string> childKitOrderNoList = this.genericMgr.FindAll<string>("select FeedOrder from ProductFeed where ProductOrder = ?", orderNo);

            if (childKitOrderNoList != null && childKitOrderNoList.Count > 0)
            {
                foreach (string childKitOrderNo in childKitOrderNoList)
                {
                    IList<string> nextChildKitOrderNoList = NestGetChildKitOrderNo(childKitOrderNo);
                    if (nextChildKitOrderNoList != null && nextChildKitOrderNoList.Count > 0)
                    {
                        ((List<string>)childKitOrderNoList).AddRange(nextChildKitOrderNoList);
                    }
                }
            }

            return childKitOrderNoList;
        }

        private void SetUom4FeedInput(IList<FeedInput> feedInputList)
        {
            #region FeedInput的ITEM赋基本单位
            IList<string> itemCodeList = feedInputList.Where(f => !string.IsNullOrWhiteSpace(f.HuId)).Select(f => f.Item).Distinct().ToList();
            if (itemCodeList != null && itemCodeList.Count > 0)
            {
                string selectItemStatement = string.Empty;
                IList<object> selectItemParas = new List<object>();
                foreach (string itemCode in itemCodeList)
                {
                    if (selectItemStatement == string.Empty)
                    {
                        selectItemStatement = "from Item where Code in (?";
                    }
                    else
                    {
                        selectItemStatement += ", ?";
                    }
                    selectItemParas.Add(itemCode);
                }
                selectItemStatement += ")";

                IList<Item> itemList = this.genericMgr.FindAll<Item>(selectItemStatement, selectItemParas.ToArray());

                foreach (FeedInput feedInput in feedInputList.Where(f => !string.IsNullOrWhiteSpace(f.HuId)))
                {
                    feedInput.Uom = itemList.Where(i => i.Code == feedInput.Item).Single().Uom; //基本单位
                }
            }
            #endregion
        }

        private IList<OrderDetail> LoadOrderDetails(int[] orderDetIdList)
        {
            IList<object> para = new List<object>();

            string selectOrderDetailStatement = string.Empty;
            foreach (int id in orderDetIdList)
            {
                if (selectOrderDetailStatement == string.Empty)
                {
                    selectOrderDetailStatement = "from OrderDetail where Id in (?";
                }
                else
                {
                    selectOrderDetailStatement += ",?";
                }
                para.Add(id);
            }
            selectOrderDetailStatement += ")";

            return this.genericMgr.FindAll<OrderDetail>(selectOrderDetailStatement, para.ToArray());
        }
        #endregion

        #region 生产单投料，投工单
        [Transaction(TransactionMode.Requires)]
        public void FeedProductOrder(string orderNo, string feedOrderNo)
        {
            FeedProductOrder(orderNo, feedOrderNo, false, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedProductOrder(string orderNo, string feedOrderNo, bool isForceFeed)
        {
            FeedProductOrder(orderNo, feedOrderNo, isForceFeed, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedProductOrder(string orderNo, string feedOrderNo, DateTime effectiveDate)
        {
            FeedProductOrder(orderNo, feedOrderNo, false, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void FeedProductOrder(string orderNo, string feedOrderNo, bool isForceFeed, DateTime effectiveDate)
        {
            #region 查询订单
            IList<OrderMaster> orderMasterList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where OrderNo in (?, ?)",
                new object[] { orderNo, feedOrderNo });

            #endregion

            #region 检查
            OrderMaster productOrder = orderMasterList.Where(o => o.OrderNo == orderNo).Single();
            OrderMaster feedProductOrder = orderMasterList.Where(o => o.OrderNo == feedOrderNo).Single();

            if (productOrder.Type != CodeMaster.OrderType.Production
                || productOrder.Type == CodeMaster.OrderType.SubContract)
            {
                throw new TechnicalException("ProductOrder type is not correct.");
            }

            if (feedProductOrder.Type != CodeMaster.OrderType.Production
                || feedProductOrder.Type == CodeMaster.OrderType.SubContract)
            {
                throw new TechnicalException("FeedProductOrder type is not correct.");
            }

            if (productOrder.Status != CodeMaster.OrderStatus.InProcess
                && productOrder.Status != CodeMaster.OrderStatus.Complete)
            {
                throw new BusinessException("父生产单{0}的状态为{1}，不能投料。", productOrder.OrderNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)productOrder.Status).ToString()));
            }

            if (feedProductOrder.Status != CodeMaster.OrderStatus.Complete && feedProductOrder.Status != CodeMaster.OrderStatus.Close)
            {
                throw new BusinessException("子生产单{0}没有完工，不能投料。", feedProductOrder.OrderNo);
            }

            if (!isForceFeed && productOrder.TraceCode != feedProductOrder.TraceCode)
            {
                throw new BusinessException("子生产单{0}的VAN号{1}和父生产单的VAN号{2}不一致。", feedProductOrder.OrderNo, feedProductOrder.TraceCode, productOrder.TraceCode);
            }

            #region 查询生产单是否已经投料
            if (this.genericMgr.FindAll<long>("select count(*) as counter from ProductFeed where FeedOrder = ?", feedProductOrder.OrderNo)[0] > 0)
            {
                throw new BusinessException("子生产单{0}已经投料。", feedProductOrder.OrderNo);
            }
            #endregion
            #endregion

            #region 根据工位找工序
            OrderOperation orderOperation = this.genericMgr.FindAll<OrderOperation>("from OrderOperation where OrderNo = ? and OpReference = ?",
                new object[] { productOrder.OrderNo, feedProductOrder.LocationTo }).FirstOrDefault();
            #endregion

            #region 查找投料工单的收货记录
            string selectFeedOrderReceiptStatement = "from ReceiptLocationDetail where OrderDetailId in (select Id from OrderDetail where OrderNo = ?))";
            IList<ReceiptLocationDetail> receiptLocationDetailList = this.genericMgr.FindAll<ReceiptLocationDetail>(selectFeedOrderReceiptStatement, feedProductOrder.OrderNo);
            IList<Item> itemList = this.itemMgr.GetItems(receiptLocationDetailList.Select(det => det.Item).Distinct().ToList());

            IList<FeedInput> feedInputList = (from det in receiptLocationDetailList
                                              group det by new { Item = det.Item, QualityType = det.QualityType, HuId = det.HuId } into result
                                              select new FeedInput
                                              {
                                                  Item = result.Key.Item,
                                                  QualityType = result.Key.QualityType,
                                                  HuId = result.Key.HuId,
                                                  LocationFrom = feedProductOrder.LocationTo,  //投料的扣料库位为Kit单的目的库位
                                                  Uom = itemList.Where(i => i.Code == result.Key.Item).Single().Uom,
                                                  Qty = result.Sum(det => det.Qty),
                                                  NotReport = true,                         //过滤掉驾驶室和底盘总成，不需要传给SAP
                                              }).ToList();

            #region FeedInput的ITEM赋基本单位
            SetUom4FeedInput(feedInputList);
            #endregion
            #endregion

            #region 记录生产单投料
            ProductFeed productFeed = new ProductFeed();
            productFeed.TraceCode = productOrder.TraceCode;
            productFeed.FeedOrder = feedOrderNo;
            productFeed.ProductOrder = orderNo;

            this.genericMgr.Create(productFeed);
            #endregion

            #region 投料
            //投子工单一定是强制投料，因为父工单的Bom不包含子工单的成品
            FeedRawMaterial(orderNo, feedInputList, true, effectiveDate);
            #endregion
        }
        #endregion

        #region 生产退料
        [Transaction(TransactionMode.Requires)]
        public void ReturnRawMaterial(string productLine, string productLineFacility, IList<ReturnInput> returnInputList)
        {
            ReturnRawMaterial(productLine, productLineFacility, null, null, null, null, returnInputList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReturnRawMaterial(string productLine, string productLineFacility, IList<ReturnInput> returnInputList, DateTime effectiveDate)
        {
            ReturnRawMaterial(productLine, productLineFacility, null, null, null, null, returnInputList, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReturnRawMaterial(string orderNo, string traceCode, int? operation, string opReference, IList<ReturnInput> returnInputList)
        {
            ReturnRawMaterial(orderNo, traceCode, operation, opReference, returnInputList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReturnRawMaterial(string orderNo, string traceCode, int? operation, string opReference, IList<ReturnInput> returnInputList, DateTime effectiveDate)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);

            if (orderMaster.Type != com.Sconit.CodeMaster.OrderType.Production && orderMaster.Type != com.Sconit.CodeMaster.OrderType.SubContract)
            {
                throw new TechnicalException("非生产单不能进行退料。");
            }

            #region 检查生产单
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.InProcess && orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Complete)
            {
                throw new BusinessException("状态为{0}的生产单不能退料。", systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
            #endregion

            #region 批量对投料对象赋订单的值
            if (returnInputList != null)
            {
                foreach (ReturnInput returnInput in returnInputList)
                {
                    returnInput.OrderNo = orderNo;
                    returnInput.CurrentOrderMaster = orderMaster;
                    returnInput.OrderType = orderMaster.Type;
                    returnInput.OrderSubType = orderMaster.SubType;
                    returnInput.Operation = operation;
                    returnInput.OpReference = opReference;
                }
            }
            #endregion

            ReturnRawMaterial(orderMaster.Flow, orderMaster.ProductLineFacility, orderNo, traceCode, operation, opReference, returnInputList, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReturnRawMaterial(IList<ReturnInput> returnInputList)
        {
            ReturnRawMaterial(returnInputList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReturnRawMaterial(IList<ReturnInput> returnInputList, DateTime effectiveDate)
        {
            #region 投料不能为空检验
            IList<ReturnInput> noneZeroReturnInputList = null;
            if (returnInputList != null)
            {
                noneZeroReturnInputList = returnInputList.Where(f => f.Qty > 0 && f.UnitQty > 0 && f.ProductLineLocationDetailId.HasValue).ToList();
            }
            if (noneZeroReturnInputList == null || noneZeroReturnInputList.Count == 0)
            {
                throw new BusinessException("退料零件不能为空。");
            }
            #endregion

            #region 循环操作库存
            foreach (ReturnInput returnInput in returnInputList)
            {
                #region 退料
                IList<InventoryTransaction> inventoryTransactionList = this.locationDetailMgr.ReturnProductRawMaterial(returnInput, effectiveDate);
                #endregion
            }
            #endregion
        }

        private void ReturnRawMaterial(string productLine, string productLineFacility, string orderNo, string traceCode, int? operation, string opReference, IList<ReturnInput> returnInputList, DateTime effectiveDate)
        {
            #region 投料不能为空检验
            IList<ReturnInput> noneZeroReturnInputList = null;
            if (returnInputList != null)
            {
                noneZeroReturnInputList = returnInputList.Where(f => f.Qty > 0).ToList();
            }
            if (noneZeroReturnInputList == null || noneZeroReturnInputList.Count == 0)
            {
                throw new BusinessException("退料零件不能为空。");
            }
            #endregion

            #region 查找条码填充零件、数量和质量状态等数据
            foreach (ReturnInput returnInput in noneZeroReturnInputList)
            {
                if (!string.IsNullOrWhiteSpace(returnInput.HuId))
                {
                    Hu hu = this.genericMgr.FindById<Hu>(returnInput.HuId);

                    //todo 检验投料的条码是否是OrderBomDetail上指定的库位投料

                    //不用判断冻结和占用，出库方法中会判断
                    returnInput.Item = hu.Item;
                    returnInput.LotNo = hu.LotNo;
                    //returnInput.QualityType = hu.QualityType;
                    returnInput.QualityType = com.Sconit.CodeMaster.QualityType.Qualified;
                    returnInput.Qty = hu.Qty;
                    returnInput.Uom = hu.Uom;
                    returnInput.BaseUom = hu.BaseUom;
                    returnInput.UnitQty = hu.UnitQty;
                }
            }
            #endregion

            #region 缓存出库需要的字段
            //IList<Item> feedItemList = this.itemMgr.GetItems(noneZeroReturnInputList.Select(r => r.Item).Distinct().ToList());
            foreach (ReturnInput returnInput in noneZeroReturnInputList)
            {
                returnInput.ProductLine = productLine;
                returnInput.ProductLineFacility = productLineFacility;
                returnInput.CurrentProductLine = this.genericMgr.FindById<FlowMaster>(productLine);
                //if (string.IsNullOrWhiteSpace(returnInput.LocationTo) && string.IsNullOrWhiteSpace(returnInput.HuId))
                //{
                //    throw new TechnicalException("LocationTo not specified in FeedInput.");
                //}

                if (!string.IsNullOrWhiteSpace(returnInput.LocationTo))
                {
                    returnInput.CurrentLocationTo = this.genericMgr.FindById<Location>(returnInput.LocationTo);
                }

                if (string.IsNullOrWhiteSpace(returnInput.HuId))
                {
                    Item item = this.genericMgr.FindById<Item>(returnInput.Item);
                    if (string.IsNullOrWhiteSpace(returnInput.Uom))
                    {
                        throw new TechnicalException("Uom not specified in FeedInput.");
                    }
                    returnInput.BaseUom = item.Uom;
                    if (item.Uom != returnInput.Uom)
                    {
                        returnInput.UnitQty = this.itemMgr.ConvertItemUomQty(returnInput.Item, returnInput.Uom, 1, item.Uom);  //记录单位转换系数
                    }
                    else
                    {
                        returnInput.UnitQty = 1;
                    }
                }
            }
            #endregion

            #region 循环操作库存
            foreach (ReturnInput returnInput in noneZeroReturnInputList)
            {
                #region 退料
                IList<InventoryTransaction> inventoryTransactionList = this.locationDetailMgr.ReturnProductRawMaterial(returnInput, effectiveDate);
                #endregion
            }
            #endregion
        }
        #endregion

        #region 加权平均回冲
        public void BackflushWeightAverage(string productLine, string productLineFacility, IList<WeightAverageBackflushInput> backflushInputList)
        {
            BackflushWeightAverage(productLine, productLineFacility, backflushInputList, DateTime.Now);
        }

        public void BackflushWeightAverage(string productLine, string productLineFacility, IList<WeightAverageBackflushInput> backflushInputList, DateTime effectiveDate)
        {
            if ((from f in backflushInputList
                 group f by f.Item into result
                 where result.Count() > 1
                 select result).Count() > 0)
            {
                throw new TechnicalException("Backflush item duplicate.");
            }

            #region 投料不能为空检验
            IList<WeightAverageBackflushInput> noneZeroBackflushInputList = null;
            if (backflushInputList != null)
            {
                noneZeroBackflushInputList = backflushInputList.Where(f => f.Qty > 0).ToList();
            }
            if (noneZeroBackflushInputList == null || noneZeroBackflushInputList.Count == 0)
            {
                throw new BusinessException("回冲零件不能为空。");
            }
            #endregion

            FlowMaster CurrentProductLine = this.genericMgr.FindById<FlowMaster>(productLine);
            IList<Item> feedItemList = this.itemMgr.GetItems(noneZeroBackflushInputList.Select(f => f.Item).Distinct().ToList());
            foreach (WeightAverageBackflushInput backflushInput in noneZeroBackflushInputList)
            {
                backflushInput.ProductLine = productLine;
                backflushInput.ProductLineFacility = productLineFacility;
                backflushInput.CurrentProductLine = CurrentProductLine;
                //Item item = this.genericMgr.FindById<Item>(backflushInput.Item);
                Item item = feedItemList.Where(f => f.Code == backflushInput.Item).Single();
                backflushInput.BaseUom = item.Uom;

                if (backflushInput.Uom != backflushInput.Uom)
                {
                    backflushInput.UnitQty = this.itemMgr.ConvertItemUomQty(backflushInput.Item, backflushInput.Uom, 1, item.Uom);  //记录单位转换系数
                }
                else
                {
                    backflushInput.UnitQty = 1;
                }
            }

            this.locationDetailMgr.BackflushProductWeightAverageRawMaterial(backflushInputList, effectiveDate);
        }
        #endregion

        #region 回冲物料
        [Transaction(TransactionMode.Requires)]
        public void BackflushProductOrder(IList<OrderDetail> orderDetailList)
        {
            this.BackflushProductOrder(orderDetailList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void BackflushProductOrder(IList<OrderDetail> orderDetailList, DateTime effectiveDate)
        {
            OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderDetailList.First().OrderNo);
            this.BackflushProductOrder(orderDetailList, orderMaster, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void BackflushProductOrder(IList<OrderDetail> orderDetailList, OrderMaster orderMaster, DateTime effectiveDate)
        {
            #region 判断是否全0收货
            if (orderDetailList == null || orderDetailList.Count == 0)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_ReceiveDetailIsEmpty);
            }

            IList<OrderDetail> nonZeroOrderDetailList = orderDetailList.Where(o => o.ReceiveQtyInput != 0 || o.ScrapQtyInput != 0).ToList();

            if (nonZeroOrderDetailList.Count == 0)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_ReceiveDetailIsEmpty);
            }

            if (nonZeroOrderDetailList.Count > 1)
            {
                //throw new BusinessException("收货明细大于1。");
            }
            var isSubContract = (orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract && orderMaster.SubType == CodeMaster.OrderSubType.Normal);
            //委外订单可以合并订单收货
            if (nonZeroOrderDetailList.Select(p => p.OrderNo).Distinct().Count() > 1 && !isSubContract)
            {
                throw new BusinessException("不能跨生产单同时收货");
            }
            #endregion

            #region 查询生产单和生产线
            //不能跨生产单同时收货
            if (!isSubContract)
            {
                string orderNo = nonZeroOrderDetailList.Select(o => o.OrderNo).Distinct().Single();
            }
            //OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);
            if (orderMaster.CurrentFlowMaster == null)
            {
                orderMaster.CurrentFlowMaster = genericMgr.FindById<FlowMaster>(orderMaster.Flow);
            }
            #endregion

            var allOrderBomDetailDic = this.genericMgr.FindAllIn<OrderBomDetail>
                    (" from OrderBomDetail where OrderDetailId in(? ", nonZeroOrderDetailList.Select(p => (object)p.Id))
                    .GroupBy(p => p.OrderDetailId, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.ToList());

            var itemDiscontinueDic = (this.genericMgr.FindAllIn<ItemDiscontinue>
                ("from ItemDiscontinue where StartDate < ? and (EndDate is null or EndDate >= ?) and Item in(?",
                allOrderBomDetailDic.SelectMany(p => p.Value).Select(p => p.Item).Distinct(), new object[] { DateTime.Now, DateTime.Now })
                ?? new List<ItemDiscontinue>())
                .GroupBy(p => p.Item, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.OrderBy(q => q.Priority).ToList());

            //先简单处理:只考虑相同的原材料库位
            Dictionary<string, decimal> invDic = new Dictionary<string, decimal>();
            if (itemDiscontinueDic.Count > 0)
            {
                List<string> paramItems = new List<string>();
                paramItems.AddRange(itemDiscontinueDic.Keys);
                paramItems.AddRange(itemDiscontinueDic.SelectMany(p => p.Value.Select(q => q.DiscontinueItem).Distinct()));

                invDic = genericMgr.FindAllIn<LocationLotDetail>(
                    " from LocationLotDetail where location =? and item in(?", paramItems, new object[] { orderMaster.LocationFrom })
                   .GroupBy(p => p.Item, (k, g) => new { k, Qty = g.Where(q => q.IsATP).Sum(q => q.Qty) })
                   .Where(p => p.Qty > 0)
                   .ToDictionary(d => d.k, d => d.Qty);
            }

            foreach (var orderDetail in nonZeroOrderDetailList)
            {
                List<BackflushInput> backflushInputList = new List<BackflushInput>();
                #region 查询订单Bom
                var orderBomDetailList = allOrderBomDetailDic.ValueOrDefault(orderDetail.Id) ?? new List<OrderBomDetail>();
                #endregion

                #region 查询待回冲物料
                //投料在生产单上的物料
                //backflushInputList.AddRange(GetProductLineLocationDetailInOrder(orderMaster, nonZeroOrderDetailList, productLine));

                #region 查询冲投料至生产线的物料
                //IList<ProductLineLocationDetail> productLineLocationDetailList =GetProductLineLocationDetail(orderMaster, orderNo);
                #endregion

                #region 根据OrderBomDetail生成收货回冲和加权平均的回冲记录
                foreach (OrderBomDetail orderBomDetail in orderBomDetailList)//.Where(bomDet => bomDet.BackFlushMethod != CodeMaster.BackFlushMethod.BackFlushOrder)
                {
                    //先回冲有库存主物料
                    //再回冲有库存的替代物料
                    //然后再回冲主物料成负库存
                    var itemDiscontinues = itemDiscontinueDic.ValueOrDefault(orderBomDetail.Item);
                    foreach (var orderDetailInput in orderDetail.OrderDetailInputs)
                    {
                        foreach (var receiptDetail in orderDetailInput.ReceiptDetails)
                        {
                            var currentBackfushQty = (orderDetailInput.ReceiveQty + orderDetailInput.ScrapQty) * orderBomDetail.BomUnitQty;
                            if (itemDiscontinues != null)
                            {
                                var invQty = invDic.ValueOrDefault(orderBomDetail.Item);
                                if (invQty >= currentBackfushQty)
                                {
                                    AddBackflushInput(orderMaster, orderDetail, backflushInputList, orderBomDetail, orderDetailInput, receiptDetail, currentBackfushQty);
                                    invDic[orderBomDetail.Item] = invQty - currentBackfushQty;
                                    currentBackfushQty = 0;
                                }
                                else
                                {
                                    if (invQty > 0)
                                    {
                                        AddBackflushInput(orderMaster, orderDetail, backflushInputList, orderBomDetail, orderDetailInput, receiptDetail, invQty);
                                        invDic[orderBomDetail.Item] = 0;
                                        currentBackfushQty -= invQty;
                                    }

                                    foreach (var itemDiscontinue in itemDiscontinues)
                                    {
                                        if (currentBackfushQty > 0)
                                        {
                                            invQty = invDic.ValueOrDefault(itemDiscontinue.DiscontinueItem);
                                            var newOrderBomDetail = Mapper.Map<OrderBomDetail, OrderBomDetail>(orderBomDetail);
                                            var newItem = this.itemMgr.GetCacheItem(itemDiscontinue.DiscontinueItem);
                                            newOrderBomDetail.Item = newItem.Code;
                                            newOrderBomDetail.ItemDescription = newItem.Description;
                                            newOrderBomDetail.ReferenceItemCode = newItem.ReferenceCode;
                                            newOrderBomDetail.Uom = newItem.Uom;
                                            newOrderBomDetail.BaseUom = newItem.Uom;
                                            newOrderBomDetail.UnitQty = itemDiscontinue.UnitQty;
                                            if (invQty >= currentBackfushQty)
                                            {
                                                AddBackflushInput(orderMaster, orderDetail, backflushInputList, newOrderBomDetail, orderDetailInput, receiptDetail, currentBackfushQty);
                                                invDic[orderBomDetail.Item] = invQty - currentBackfushQty;
                                                currentBackfushQty = 0;
                                                break;
                                            }
                                            else
                                            {
                                                AddBackflushInput(orderMaster, orderDetail, backflushInputList, newOrderBomDetail, orderDetailInput, receiptDetail, invQty);
                                                invDic[orderBomDetail.Item] = 0;
                                                currentBackfushQty -= invQty;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    if (currentBackfushQty > 0)
                                    {
                                        var newOrderBomDetail = Mapper.Map<OrderBomDetail, OrderBomDetail>(orderBomDetail);
                                        AddBackflushInput(orderMaster, orderDetail, backflushInputList, newOrderBomDetail, orderDetailInput, receiptDetail, currentBackfushQty);
                                    }
                                }
                            }
                            else
                            {
                                AddBackflushInput(orderMaster, orderDetail, backflushInputList, orderBomDetail, orderDetailInput, receiptDetail, currentBackfushQty);
                            }
                        }
                    }
                }
                #endregion
                #endregion

                #region 回冲物料
                foreach (var backflushInput in backflushInputList)
                {
                    backflushInput.IpNo = orderMaster.IpNo;
                }
                this.locationDetailMgr.BackflushProductMaterial(backflushInputList);
                #endregion

                #region 记录工单投料明细/新
                CreateOrderBackflushDetails(effectiveDate, backflushInputList);
                #endregion
            }
        }

        private static void AddBackflushInput(OrderMaster orderMaster, OrderDetail orderDetail, List<BackflushInput> backflushInputList, OrderBomDetail orderBomDetail, OrderDetailInput orderDetailInput, ReceiptDetail receiptDetail, decimal backFlushInputQty)
        {
            BackflushInput backFlushInput = new BackflushInput();

            backFlushInput.ProductLine = orderMaster.Flow;
            backFlushInput.ProductLineFacility = orderMaster.ProductLineFacility;
            backFlushInput.CurrentProductLine = orderMaster.CurrentFlowMaster;
            backFlushInput.OrderNo = orderMaster.OrderNo;
            backFlushInput.OrderType = orderMaster.Type;
            backFlushInput.OrderSubType = orderMaster.SubType;
            backFlushInput.OrderDetailId = orderDetail.Id;
            backFlushInput.OrderDetailSequence = orderDetail.Sequence;
            backFlushInput.OrderBomDetail = orderBomDetail;
            backFlushInput.ReceiptNo = receiptDetail.ReceiptNo;
            backFlushInput.ReceiptDetailId = receiptDetail.Id;
            backFlushInput.ReceiptDetailSequence = receiptDetail.Sequence;
            backFlushInput.FGItem = orderDetail.Item;
            backFlushInput.Item = orderBomDetail.Item;
            backFlushInput.ItemDescription = orderBomDetail.ItemDescription;
            backFlushInput.ReferenceItemCode = orderBomDetail.ReferenceItemCode;
            backFlushInput.Uom = orderBomDetail.Uom;
            backFlushInput.BaseUom = orderBomDetail.BaseUom;
            backFlushInput.UnitQty = orderBomDetail.UnitQty;   //基本单位转换率 = 订单单位/库存单位，转换为库存单位消耗 = 单位用量（订单单位） / 基本单位转换率
            backFlushInput.TraceCode = orderDetailInput.TraceCode;
            //backFlushInput.QualityType = CodeMaster.QualityType.Qualified;
            backFlushInput.FGQualityType = orderDetailInput.QualityType;  //收货成品的质量状态
            //backFlushInput.HuId = orderDetailInput.HuId;
            //backFlushInput.LotNo = orderDetailInput.LotNo;
            backFlushInput.Qty = backFlushInputQty;
            backFlushInput.ReceivedQty = receiptDetail.ReceivedQty;

            if (orderBomDetail.BackFlushMethod == CodeMaster.BackFlushMethod.GoodsReceive)
            {
                #region 按比例回冲
                //backFlushInput.ProductLineLocationDetailList = productLineLocationDetailList.Where(p => p.Item == orderBomDetail.Item).ToList();
                backFlushInput.Operation = orderBomDetail.Operation;
                backFlushInput.OpReference = orderBomDetail.OpReference;
                #endregion
            }
            backflushInputList.Add(backFlushInput);
        }

        private IList<ProductLineLocationDetail> GetProductLineLocationDetail(OrderMaster orderMaster, string orderNo)
        {
            string selectProductLineLocationDetailStatement = "from ProductLineLocationDetail where ProductLine = ? and OrderNo is null and Item in (select Item from OrderBomDetail where OrderNo = ?)";
            IList<object> selectProductLineLocationDetailPara = new List<object>();
            selectProductLineLocationDetailPara.Add(orderMaster.Flow);
            selectProductLineLocationDetailPara.Add(orderNo);
            if (!string.IsNullOrWhiteSpace(orderMaster.ProductLineFacility))
            {
                selectProductLineLocationDetailStatement += " and ProductLineFacility = ?";
                selectProductLineLocationDetailPara.Add(orderMaster.ProductLineFacility);
            }
            IList<ProductLineLocationDetail> productLineLocationDetailList = this.genericMgr.FindAll<ProductLineLocationDetail>(selectProductLineLocationDetailStatement, selectProductLineLocationDetailPara.ToArray());
            return productLineLocationDetailList;
        }

        private List<BackflushInput> GetProductLineLocationDetailInOrder(OrderMaster orderMaster, IList<OrderDetail> nonZeroOrderDetailList, FlowMaster productLine)
        {
            var backflushInputList = new List<BackflushInput>();
            #region 查询冲投料至工单的物料
            string selectProductLineLocationDetailInOrderStatement = "from ProductLineLocationDetail where OrderNo = ?";
            IList<object> selectProductLineLocationDetailInOrderPara = new List<object>();
            selectProductLineLocationDetailInOrderPara.Add(orderMaster.OrderNo);
            #region 判断是否有追溯码
            foreach (OrderDetail orderDetail in nonZeroOrderDetailList)
            {
                foreach (OrderDetailInput orderDetailInput in orderDetail.OrderDetailInputs)
                {
                    if (!string.IsNullOrWhiteSpace(orderDetailInput.TraceCode))
                    {
                        if (selectProductLineLocationDetailInOrderPara.Count == 1)
                        {
                            selectProductLineLocationDetailInOrderStatement += " and TraceCode in (?";
                        }
                        else
                        {
                            selectProductLineLocationDetailInOrderStatement += ",?";
                        }
                        selectProductLineLocationDetailInOrderPara.Add(orderDetailInput.TraceCode);
                    }
                }
            }
            if (selectProductLineLocationDetailInOrderPara.Count > 1)
            {
                selectProductLineLocationDetailInOrderStatement += ")";
            }
            #endregion
            IList<ProductLineLocationDetail> productLineLocationDetailInOrderList = this.genericMgr.FindAll<ProductLineLocationDetail>(selectProductLineLocationDetailInOrderStatement, selectProductLineLocationDetailInOrderPara.ToArray());
            #endregion

            #region 投料至工单的在制品全部要回冲
            var groupedProductLineLocationDetailList = from locDet in productLineLocationDetailInOrderList
                                                       group locDet by new
                                                       {
                                                           OrderDetailId = locDet.OrderDetailId,
                                                           Item = locDet.Item,
                                                       } into gj
                                                       select new
                                                       {
                                                           OrderDetailId = gj.Key.OrderDetailId,
                                                           Item = gj.Key.Item,
                                                           ProductLineLocationDetailList = gj.ToList()
                                                       };

            IList<Item> itemList = this.itemMgr.GetItems(groupedProductLineLocationDetailList.Select(d => d.Item).Distinct().ToList());

            foreach (var groupedProductLineLocationDetail in groupedProductLineLocationDetailList)
            {
                //OrderDetail orderDetail = orderDetailList.Where(o => o.Id == groupedProductLineLocationDetail.OrderDetailId).Single();
                //OrderDetail orderDetail = nonZeroOrderDetailList[0];
                //OrderDetailInput orderDetailInput = orderDetail.OrderDetailInputs[0];

                foreach (var orderDetail in nonZeroOrderDetailList)
                {
                    foreach (var orderDetailInput in orderDetail.OrderDetailInputs)
                    {
                        foreach (var receiptDetail in orderDetailInput.ReceiptDetails)
                        {
                            Item item = itemList.Where(i => i.Code == groupedProductLineLocationDetail.Item).Single();

                            BackflushInput backFlushInput = new BackflushInput();

                            backFlushInput.ProductLine = orderMaster.Flow;
                            backFlushInput.ProductLineFacility = orderMaster.ProductLineFacility;
                            backFlushInput.CurrentProductLine = productLine;
                            backFlushInput.OrderNo = orderMaster.OrderNo;
                            backFlushInput.OrderType = orderMaster.Type;
                            backFlushInput.OrderSubType = orderMaster.SubType;
                            backFlushInput.OrderDetailId = orderDetail.Id;
                            backFlushInput.OrderDetailSequence = orderDetail.Sequence;
                            backFlushInput.ReceiptNo = receiptDetail.ReceiptNo;
                            backFlushInput.ReceiptDetailId = receiptDetail.Id;
                            backFlushInput.ReceiptDetailSequence = receiptDetail.Sequence;
                            //backFlushInput.ReceiptDetailSequence = orderDetailInput.ReceiptDetail.Sequence;
                            backFlushInput.FGItem = orderDetail.Item;
                            backFlushInput.Item = item.Code;
                            backFlushInput.ItemDescription = item.Description;
                            backFlushInput.ReferenceItemCode = item.ReferenceCode;
                            backFlushInput.Uom = item.Uom;
                            backFlushInput.BaseUom = item.Uom;
                            backFlushInput.UnitQty = 1;
                            backFlushInput.TraceCode = orderDetailInput.TraceCode;
                            //backFlushInput.QualityType = CodeMaster.QualityType.Qualified;
                            backFlushInput.FGQualityType = orderDetailInput.QualityType;
                            backFlushInput.Qty = groupedProductLineLocationDetail.ProductLineLocationDetailList.Sum(g => g.RemainBackFlushQty);
                            backFlushInput.ProductLineLocationDetailList = groupedProductLineLocationDetail.ProductLineLocationDetailList;

                            backflushInputList.Add(backFlushInput);
                        }
                    }
                }
            }
            #endregion
            return backflushInputList;
        }

        private void CreateOrderBackflushDetails(DateTime effectiveDate, IList<BackflushInput> backflushInputList)
        {
            DateTime dateTimeNow = DateTime.Now;
            User currentUser = SecurityContextHolder.Get();
            List<OrderBackflushDetail> orderBackflushDetailList = new List<OrderBackflushDetail>();

            #region 汇总收货回冲投料明细
            foreach (BackflushInput backflushInput in backflushInputList.Where(input => input.OrderBomDetail != null))// && input.OrderBomDetail.BackFlushMethod == CodeMaster.BackFlushMethod.GoodsReceive
            {
                orderBackflushDetailList.AddRange(from trans in backflushInput.InventoryTransactionList
                                                  group trans by trans.PlanBill into g
                                                  select new OrderBackflushDetail
                                                  {
                                                      OrderNo = backflushInput.OrderNo,
                                                      OrderDetailId = backflushInput.OrderDetailId,
                                                      OrderDetailSequence = backflushInput.OrderDetailSequence,
                                                      OrderBomDetailId = backflushInput.OrderBomDetail.Id,
                                                      OrderBomDetailSequence = backflushInput.OrderBomDetail.Sequence,
                                                      ReceiptNo = backflushInput.ReceiptNo,
                                                      ReceiptDetailId = backflushInput.ReceiptDetailId,
                                                      ReceiptDetailSequence = backflushInput.ReceiptDetailSequence,
                                                      Bom = backflushInput.OrderBomDetail != null ? backflushInput.OrderBomDetail.Bom : null,
                                                      FGItem = backflushInput.FGItem,
                                                      Item = backflushInput.Item,
                                                      ItemDescription = backflushInput.ItemDescription,
                                                      ReferenceItemCode = backflushInput.ReferenceItemCode,
                                                      Uom = backflushInput.Uom,
                                                      BaseUom = backflushInput.BaseUom,
                                                      UnitQty = backflushInput.UnitQty,
                                                      ManufactureParty = backflushInput.OrderBomDetail.ManufactureParty,
                                                      TraceCode = backflushInput.TraceCode,
                                                      HuId = backflushInput.HuId,
                                                      LotNo = backflushInput.LotNo,
                                                      Operation = backflushInput.Operation,
                                                      OpReference = backflushInput.OpReference,
                                                      BackflushedQty = backflushInput.FGQualityType == CodeMaster.QualityType.Qualified ? g.Sum(trans => trans.Qty) / backflushInput.UnitQty : 0,   //根据收货成品的质量状态记录至不同的回冲数量中
                                                      BackflushedRejectQty = backflushInput.FGQualityType == CodeMaster.QualityType.Reject ? g.Sum(trans => trans.Qty) / backflushInput.UnitQty : 0,
                                                      //BackflushedScrapQty = input.BackflushedQty,
                                                      LocationFrom = backflushInput.OrderBomDetail.Location,
                                                      ProductLine = backflushInput.ProductLine,
                                                      ProductLineFacility = backflushInput.ProductLineFacility,
                                                      ReserveNo = backflushInput.OrderBomDetail.ReserveNo,
                                                      ReserveLine = backflushInput.OrderBomDetail.ReserveLine,
                                                      AUFNR = backflushInput.OrderBomDetail.AUFNR,
                                                      ICHARG = backflushInput.OrderBomDetail.ICHARG,
                                                      BWART = backflushInput.OrderBomDetail.BWART,
                                                      NotReport = false,  //理论都需要汇报
                                                      PlanBill = g.Key,
                                                      EffectiveDate = effectiveDate,
                                                      CreateUserId = currentUser.Id,
                                                      CreateUserName = currentUser.FullName,
                                                      CreateDate = dateTimeNow,
                                                      IsVoid = false,
                                                      ReceivedQty = backflushInput.ReceivedQty
                                                  });
            }
            #endregion

            #region 汇总工单在制品投料明细
            /*
            foreach (BackflushInput backflushInput in backflushInputList.Where(i => i.OrderBomDetail == null
                                                    || i.OrderBomDetail.BackFlushMethod == CodeMaster.BackFlushMethod.BackFlushOrder))
            {
                ((List<OrderBackflushDetail>)orderBackflushDetailList).AddRange(from p in backflushInput.InventoryTransactionList
                                                                                group p by new
                                                                                {
                                                                                    HuId = p.HuId,
                                                                                    LotNo = p.LotNo,
                                                                                    Operation = p.Operation,
                                                                                    OpReference = p.OpReference,
                                                                                    LocationFrom = p.OrgLocation,
                                                                                    PlanBill = p.PlanBill,
                                                                                    ReserveNo = p.ReserveNo,
                                                                                    ReserveLine = p.ReserveLine,
                                                                                    AUFNR = p.AUFNR,
                                                                                    ICHARG = p.ICHARG,
                                                                                    BWART = p.BWART,
                                                                                    NotReport = p.NotReport,
                                                                                } into result
                                                                                select new OrderBackflushDetail
                                                                                {
                                                                                    OrderNo = backflushInput.OrderNo,
                                                                                    OrderDetailId = backflushInput.OrderDetailId,
                                                                                    OrderDetailSequence = backflushInput.OrderDetailSequence,
                                                                                    OrderBomDetailId = backflushInput.OrderBomDetail != null ? (int?)backflushInput.OrderBomDetail.Id : null,
                                                                                    OrderBomDetailSequence = backflushInput.OrderBomDetail != null ? (int?)backflushInput.OrderBomDetail.Sequence : null,
                                                                                    ReceiptNo = backflushInput.ReceiptNo,
                                                                                    ReceiptDetailId = backflushInput.ReceiptDetailId,
                                                                                    ReceiptDetailSequence = backflushInput.ReceiptDetailSequence,
                                                                                    Bom = backflushInput.OrderBomDetail != null ? backflushInput.OrderBomDetail.Bom : null,
                                                                                    FGItem = backflushInput.FGItem,
                                                                                    Item = backflushInput.Item,
                                                                                    ItemDescription = backflushInput.ItemDescription,
                                                                                    ReferenceItemCode = backflushInput.ReferenceItemCode,
                                                                                    Uom = backflushInput.Uom,
                                                                                    BaseUom = backflushInput.BaseUom,
                                                                                    UnitQty = backflushInput.UnitQty,
                                                                                    ManufactureParty = backflushInput.OrderBomDetail != null ? backflushInput.OrderBomDetail.ManufactureParty : null,
                                                                                    TraceCode = backflushInput.TraceCode,
                                                                                    HuId = result.Key.HuId,
                                                                                    LotNo = result.Key.LotNo,
                                                                                    Operation = result.Key.Operation,
                                                                                    OpReference = result.Key.OpReference,
                                                                                    BackflushedQty = backflushInput.FGQualityType == CodeMaster.QualityType.Qualified ? result.Sum(p => p.Qty / backflushInput.UnitQty) : 0,   //根据收货成品的质量状态记录至不同的回冲数量中
                                                                                    BackflushedRejectQty = backflushInput.FGQualityType == CodeMaster.QualityType.Reject ? result.Sum(p => p.Qty / backflushInput.UnitQty) : 0,
                                                                                    //BackflushedScrapQty = input.BackflushedQty,
                                                                                    LocationFrom = result.Key.LocationFrom,
                                                                                    ProductLine = backflushInput.ProductLine,
                                                                                    ProductLineFacility = backflushInput.ProductLineFacility,
                                                                                    ReserveNo = result.Key.ReserveNo,
                                                                                    ReserveLine = result.Key.ReserveLine,
                                                                                    AUFNR = result.Key.AUFNR,
                                                                                    ICHARG = result.Key.ICHARG,
                                                                                    BWART = result.Key.BWART,
                                                                                    NotReport = result.Key.NotReport,   //过滤掉驾驶室和底盘总成
                                                                                    PlanBill = result.Key.PlanBill,
                                                                                    EffectiveDate = effectiveDate,
                                                                                    CreateUserId = currentUser.Id,
                                                                                    CreateUserName = currentUser.FullName,
                                                                                    CreateDate = dateTimeNow,
                                                                                    IsVoid = false,
                                                                                });
            }
             * */
            #endregion

            foreach (OrderBackflushDetail orderBackflushDetail in orderBackflushDetailList)
            {
                this.genericMgr.Create(orderBackflushDetail);
            }
        }

        private IList<Location> GetLocations(IList<string> locationCodeList)
        {
            string hql = string.Empty;
            IList<object> paras = new List<object>();
            foreach (string locationCode in locationCodeList)
            {
                if (hql == string.Empty)
                {
                    hql = "from Location where Code in (?";
                }
                else
                {
                    hql += ", ?";
                }
                paras.Add(locationCode);
            }
            hql += ")";
            return this.genericMgr.FindAll<Location>(hql, paras.ToArray());
        }
        #endregion

        #region 生产线暂停
        [Transaction(TransactionMode.Requires)]
        public void PauseProductLine(string productLineCode)
        {
            FlowMaster productLine = this.genericMgr.FindById<FlowMaster>(productLineCode);

            if (productLine.IsPause)
            {
                throw new BusinessException("生产线{0}已经暂停。", productLineCode);
            }

            User user = SecurityContextHolder.Get();
            this.genericMgr.FindAllWithNamedQuery("USP_Busi_PauseProductLine", new object[] { productLineCode, user.Id, user.FullName });

            //#region 获取所有活动生产单
            //IList<OrderMaster> productOrderList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where Flow = ? and Status in (?, ?, ?)",
            //    new object[] { productLineCode, CodeMaster.OrderStatus.Create, CodeMaster.OrderStatus.Submit, CodeMaster.OrderStatus.InProcess });
            //#endregion

            //#region 更新生产线
            //productLine.IsPause = true;
            //productLine.PauseTime = DateTime.Now;
            //this.genericMgr.Update(productLine);
            //#endregion

            //#region 更新生产单排序单和Kit单
            //if (productOrderList != null && productOrderList.Count > 0)
            //{
            //    #region 获取所有活动排序单和Kit单
            //    string updateSeqAndKitOrderStatement = string.Empty;
            //    IList<object> updateSeqAndKitOrderParas = new List<object>();
            //    foreach (OrderMaster productOrder in productOrderList)
            //    {
            //        if (updateSeqAndKitOrderStatement == string.Empty) 
            //         {
            //             updateSeqAndKitOrderStatement = "update from OrderMaster set IsProductLinePause = ?  and PauseTime = ? where Status in (?, ?, ?) and TraceCode in (?";
            //             updateSeqAndKitOrderParas.Add(true);
            //             updateSeqAndKitOrderParas.Add(DateTime.Now);
            //             updateSeqAndKitOrderParas.Add(CodeMaster.OrderStatus.Create);
            //             updateSeqAndKitOrderParas.Add(CodeMaster.OrderStatus.Submit);
            //             updateSeqAndKitOrderParas.Add(CodeMaster.OrderStatus.InProcess);
            //         }
            //         else
            //         {
            //             updateSeqAndKitOrderStatement += ", ?";
            //         }
            //         updateSeqAndKitOrderParas.Add(productOrder.TraceCode);
            //    }

            //    this.genericMgr.Update(updateSeqAndKitOrderStatement, updateSeqAndKitOrderParas.ToArray());
            //    #endregion
            //}
            //#endregion
        }
        #endregion

        #region 生产线恢复暂停
        [Transaction(TransactionMode.Requires)]
        public void ReStartProductLine(string productLineCode)
        {
            FlowMaster productLine = this.genericMgr.FindById<FlowMaster>(productLineCode);

            if (!productLine.IsPause)
            {
                throw new BusinessException("生产线{0}没有暂停。", productLineCode);
            }

            User user = SecurityContextHolder.Get();
            this.genericMgr.FindAllWithNamedQuery("USP_Busi_RestartProductLine", new object[] { productLineCode, user.Id, user.FullName });

            //#region 获取所有活动生产单
            //IList<OrderMaster> productOrderList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where Flow = ? and Status in (?, ?, ?)",
            //    new object[] { productLineCode, CodeMaster.OrderStatus.Create, CodeMaster.OrderStatus.Submit, CodeMaster.OrderStatus.InProcess });
            //#endregion

            //#region 更新生产线
            //productLine.IsPause = false;
            //productLine.PauseTime = null;
            //this.genericMgr.Update(productLine);
            //#endregion

            //#region 更新生产单排序单和Kit单
            //if (productOrderList != null && productOrderList.Count > 0)
            //{
            //    #region 获取所有活动排序单和Kit单
            //    string updateSeqAndKitOrderStatement = string.Empty;
            //    IList<object> updateSeqAndKitOrderParas = new List<object>();
            //    foreach (OrderMaster productOrder in productOrderList)
            //    {
            //        if (updateSeqAndKitOrderStatement == string.Empty)
            //        {
            //            updateSeqAndKitOrderStatement = "update from OrderMaster set IsProductLinePause = ?  and PauseTime = ? where Status in (?, ?, ?) and TraceCode in (?";
            //            updateSeqAndKitOrderParas.Add(false);
            //            updateSeqAndKitOrderParas.Add(null);
            //            updateSeqAndKitOrderParas.Add(CodeMaster.OrderStatus.Create);
            //            updateSeqAndKitOrderParas.Add(CodeMaster.OrderStatus.Submit);
            //            updateSeqAndKitOrderParas.Add(CodeMaster.OrderStatus.InProcess);
            //        }
            //        else
            //        {
            //            updateSeqAndKitOrderStatement += ", ?";
            //        }
            //        updateSeqAndKitOrderParas.Add(productOrder.TraceCode);
            //    }

            //    this.genericMgr.Update(updateSeqAndKitOrderStatement, updateSeqAndKitOrderParas.ToArray());
            //    #endregion
            //}
            //#endregion
        }
        #endregion
    }
}
