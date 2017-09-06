using System;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity.INV;
using NHibernate.Criterion;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity;
using System.Linq;
using com.Sconit.Entity.MD;
using NHibernate;
using NHibernate.Type;
using com.Sconit.Entity.VIEW;
using System.IO;
using NPOI.HSSF.UserModel;
using System.Collections;
using NPOI.SS.UserModel;
using com.Sconit.Utility;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class StockTakeMgrImpl : BaseMgr, IStockTakeMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IHuMgr huMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public IItemMgr itemMgr { get; set; }


        private static string SelectStockTakeDetailStatement = "from StockTakeDetail as s where StNo = ? ";
        //private static string SelectStockTakeResultStatement = "from StockTakeResult as s where StNo = ? ";
        private static string SelectStockTakeLocationStatement = "select Location from StockTakeLocation as s where StNo = ? ";
        private static string SelectStockTakeItemStatement = "select Item from StockTakeItem as s where StNo = ? ";

        #region 创建盘点单
        [Transaction(TransactionMode.Requires)]
        public void CreateStockTakeMaster(StockTakeMaster stockTakeMaster)
        {
            stockTakeMaster.StNo = numberControlMgr.GetStockTakeNo(stockTakeMaster);
            this.genericMgr.Create(stockTakeMaster);
        }
        #endregion

        #region 添加盘点库位
        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateStockTakeLocations(string stNo, IList<Location> addLocaitons, IList<Location> deleteLocations)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            BatchUpdateStockTakeLocations(stockTakeMaster, addLocaitons, deleteLocations);
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateStockTakeLocations(StockTakeMaster stockTakeMaster, IList<Location> addLocaitons, IList<Location> deleteLocations)
        {
            #region 检查
            if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.Create)
            {
                throw new BusinessException("状态为{1}的盘点单{0}不能添加库位。", stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }

            #region 判断是否有重复库位
            if (addLocaitons != null && addLocaitons.Count > 0)
            {
                IList<string> locCodes = addLocaitons.Select(l => l.Code).ToList();

                #region 查询盘点库位列表
                IList<string> stockTakeLocationList = this.genericMgr.FindAll<string>(SelectStockTakeLocationStatement, stockTakeMaster.StNo);
                #endregion
                if (stockTakeLocationList != null && stockTakeLocationList.Count > 0)
                {
                    ((List<string>)locCodes).AddRange(stockTakeLocationList);
                }

                var locCounts = from loc in locCodes
                                group loc by loc into result
                                select new
                                {
                                    LocationCode = result.Key,
                                    Count = result.Count()
                                };

                BusinessException businessException = new BusinessException();
                foreach (var locCount in locCounts.Where(l => l.Count > 1))
                {
                    businessException.AddMessage("不能重复添加库位{0}", locCount.LocationCode);
                }

                if (businessException.HasMessage)
                {
                    throw businessException;
                }
            }
            #endregion
            #endregion

            #region 新增盘点库位
            if (addLocaitons != null && addLocaitons.Count > 0)
            {
                IList<StockTakeLocation> addStockTakeLocationList = (from loc in addLocaitons
                                                                     select new StockTakeLocation
                                                                     {
                                                                         StNo = stockTakeMaster.StNo,
                                                                         Location = loc.Code,
                                                                         LocationName = loc.Name,
                                                                         Bins = loc.Bins
                                                                     }).ToList();

                foreach (StockTakeLocation addStockTakeLocation in addStockTakeLocationList)
                {
                    this.genericMgr.Create(addStockTakeLocation);
                }
            }
            #endregion

            #region 删除盘点库位
            if (deleteLocations != null && deleteLocations.Count > 0)
            {
                string deleteStockTakeLocationStatement = string.Empty;
                IList<object> deleteStockTakeLocationParas = new List<object>();
                IList<IType> deleteStockTakeLocationTypes = new List<IType>();

                foreach (Location location in deleteLocations)
                {
                    if (deleteStockTakeLocationStatement == string.Empty)
                    {
                        deleteStockTakeLocationStatement = "from StockTakeLocation where StNo = ?  and Location in (?";
                        deleteStockTakeLocationParas.Add(stockTakeMaster.StNo);
                        deleteStockTakeLocationTypes.Add(NHibernateUtil.String);
                    }
                    else
                    {
                        deleteStockTakeLocationStatement += ", ?";
                    }
                    deleteStockTakeLocationParas.Add(location.Code);
                    deleteStockTakeLocationTypes.Add(NHibernateUtil.String);
                }

                this.genericMgr.Delete(deleteStockTakeLocationStatement, deleteStockTakeLocationParas.ToArray(), deleteStockTakeLocationTypes.ToArray());
            }
            #endregion
        }

        #endregion

        #region 添加盘点零件
        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateStockTakeItems(string stNo, IList<Item> addItems, IList<Item> deleteItems)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            BatchUpdateStockTakeItems(stockTakeMaster, addItems, deleteItems);
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateStockTakeItems(StockTakeMaster stockTakeMaster, IList<Item> addItems, IList<Item> deleteItems)
        {
            #region 检查
            if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.Create)
            {
                throw new BusinessException("状态为{1}的盘点单{0}不能添加零件。", stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }

            if (stockTakeMaster.Type == CodeMaster.StockTakeType.All)
            {
                throw new BusinessException("盘点单{0}类型为全盘不能添加零件。", stockTakeMaster.StNo);
            }

            #region 判断是否有重复库位
            if (addItems != null && addItems.Count > 0)
            {
                IList<string> itemCodes = addItems.Select(l => l.Code).ToList();

                #region 查询盘点零件列表
                IList<string> stockTakeItemList = this.genericMgr.FindAll<string>(SelectStockTakeItemStatement, stockTakeMaster.StNo);
                #endregion
                if (stockTakeItemList != null && stockTakeItemList.Count > 0)
                {
                    ((List<string>)itemCodes).AddRange(stockTakeItemList);
                }

                var itemCounts = from itemCode in itemCodes
                                 group itemCode by itemCode into result
                                 select new
                                 {
                                     ItemCode = result.Key,
                                     Count = result.Count()
                                 };

                BusinessException businessException = new BusinessException();
                foreach (var itemCount in itemCounts.Where(l => l.Count > 1))
                {
                    businessException.AddMessage("不能重复添加零件{0}", itemCount.ItemCode);
                }

                if (businessException.HasMessage)
                {
                    throw businessException;
                }
            }
            #endregion
            #endregion

            #region 新增盘点零件
            if (addItems != null && addItems.Count > 0)
            {
                IList<StockTakeItem> addStockTakeItemList = (from item in addItems
                                                             select new StockTakeItem
                                                             {
                                                                 StNo = stockTakeMaster.StNo,
                                                                 Item = item.Code,
                                                                 ItemDescription = item.Description
                                                             }).ToList();

                foreach (StockTakeItem addStockTakeItem in addStockTakeItemList)
                {
                    this.genericMgr.Create(addStockTakeItem);
                }
            }
            #endregion

            #region 删除盘点零件
            if (deleteItems != null && deleteItems.Count > 0)
            {
                string deleteStockTakeItemStatement = string.Empty;
                IList<object> deleteStockTakeItemParas = new List<object>();
                IList<IType> deleteStockTakeItemTypes = new List<IType>();

                foreach (Item item in deleteItems)
                {
                    if (deleteStockTakeItemStatement == string.Empty)
                    {
                        deleteStockTakeItemStatement = "from StockTakeItem where StNo = ? and Item in (?";
                        deleteStockTakeItemParas.Add(stockTakeMaster.StNo);
                        deleteStockTakeItemTypes.Add(NHibernateUtil.String);
                    }
                    else
                    {
                        deleteStockTakeItemStatement += ", ?";
                    }
                    deleteStockTakeItemParas.Add(item.Code);
                    deleteStockTakeItemTypes.Add(NHibernateUtil.String);
                }

                this.genericMgr.Delete(deleteStockTakeItemStatement, deleteStockTakeItemParas.ToArray(), deleteStockTakeItemTypes.ToArray());
            }
            #endregion
        }
        #endregion

        #region 删除盘点单
        [Transaction(TransactionMode.Requires)]
        public void DeleteStockTakeMaster(string stNo)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            DeleteStockTakeMaster(stockTakeMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteStockTakeMaster(StockTakeMaster stockTakeMaster)
        {
            if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.Create)
            {
                throw new BusinessException(Resources.INV.StockTake.Error_StatusErrorWhenDelete,
                    stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }

            this.genericMgr.Delete("from StockTakeItem where StNo = ?", stockTakeMaster.StNo, NHibernateUtil.String);
            this.genericMgr.Delete("from StockTakeLocation where StNo = ?", stockTakeMaster.StNo, NHibernateUtil.String);
            this.genericMgr.Delete("from StockTakeMaster where StNo = ?", stockTakeMaster.StNo, NHibernateUtil.String);
        }
        #endregion

        #region 释放盘点单
        [Transaction(TransactionMode.Requires)]
        public void ReleaseStockTakeMaster(string stNo)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            ReleaseStockTakeMaster(stockTakeMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseStockTakeMaster(StockTakeMaster stockTakeMaster)
        {
            if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.Create)
            {
                throw new BusinessException(Resources.INV.StockTake.Error_StatusErrorWhenSubmit,
                    stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }

            IList<string> stockTakeLocationList = this.genericMgr.FindAll<string>(SelectStockTakeLocationStatement, stockTakeMaster.StNo);

            if (stockTakeLocationList == null || stockTakeLocationList.Count == 0)
            {
                throw new BusinessException("请选择盘点的库位。");
            }

            if (stockTakeMaster.Type == CodeMaster.StockTakeType.Part)
            {
                IList<string> stockTakeItemList = this.genericMgr.FindAll<string>(SelectStockTakeItemStatement, stockTakeMaster.StNo);
                if (stockTakeItemList == null || stockTakeItemList.Count == 0)
                {
                    throw new BusinessException("请选择盘点的零件。");
                }
            }

            User user = SecurityContextHolder.Get();
            stockTakeMaster.Status = CodeMaster.StockTakeStatus.Submit;
            stockTakeMaster.ReleaseUserId = user.Id;
            stockTakeMaster.ReleaseUserName = user.FullName;
            stockTakeMaster.ReleaseDate = DateTime.Now;

            this.genericMgr.Update(stockTakeMaster);
        }
        #endregion

        #region 开始盘点
        [Transaction(TransactionMode.Requires)]
        public void StartStockTakeMaster(string stNo)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            StartStockTakeMaster(stockTakeMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartStockTakeMaster(StockTakeMaster stockTakeMaster)
        {
            if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.Submit)
            {
                throw new BusinessException(Resources.INV.StockTake.Error_StatusErrorWhenSubmit,
                    stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }

            User user = SecurityContextHolder.Get();
            stockTakeMaster.Status = CodeMaster.StockTakeStatus.InProcess;
            stockTakeMaster.StartUserId = user.Id;
            stockTakeMaster.StartUserName = user.FullName;
            stockTakeMaster.StartDate = DateTime.Now;

            #region 记录库存
            IList<string> stockTakeLocationList = this.genericMgr.FindAll<string>(SelectStockTakeLocationStatement, stockTakeMaster.StNo);
            IList<string> stockTakeItemList = this.genericMgr.FindAll<string>(SelectStockTakeItemStatement, stockTakeMaster.StNo);
            if (!stockTakeMaster.EffectiveDate.HasValue)
            {
                stockTakeMaster.EffectiveDate = DateTime.Now;
            }
            stockTakeMaster.BaseInventoryDate = DateTime.Now;
            var stockTakeInvList = new List<StockTakeInv>();
            if (stockTakeMaster.IsScanHu)
            {
                string selectHuLocationLotDetailStatement = "from LocationLotDetail where HuId is not null ";
                IList<object> selectHuLocationLotDetailParm = new List<object>();
                selectHuLocationLotDetailStatement += GetWhereStatement(selectHuLocationLotDetailParm, stockTakeItemList, stockTakeLocationList, null);
                var locationLotDetailList = this.genericMgr.FindAll<LocationLotDetail>(selectHuLocationLotDetailStatement, selectHuLocationLotDetailParm.ToArray());

                stockTakeInvList = (from inv in locationLotDetailList
                                    select new StockTakeInv
                                    {
                                        Location = inv.Location,
                                        Item = inv.Item,
                                        HuId = inv.HuId,
                                        LotNo = inv.LotNo,
                                        Qty = inv.Qty,
                                        QualityType = inv.QualityType,
                                        IsCS = inv.IsConsignment,
                                        Bin = inv.Bin
                                    }).ToList();
            }
            else
            {
                string selectLocationLotDetailStatement = "from LocationLotDetail where HuId is null";
                IList<object> selectLocationLotDetailParm = new List<object>();
                selectLocationLotDetailStatement += GetWhereStatement(selectLocationLotDetailParm, stockTakeItemList, stockTakeLocationList, null);
                var locationLotDetailList = this.genericMgr.FindAll<LocationLotDetail>(selectLocationLotDetailStatement, selectLocationLotDetailParm.ToArray());
                stockTakeInvList = (from inv in locationLotDetailList
                                    group inv by new { inv.Item, inv.Location, inv.QualityType } into g
                                    select new StockTakeInv
                                    {
                                        Location = g.Key.Location,
                                        Item = g.Key.Item,
                                        QualityType = g.Key.QualityType,
                                        Qty = g.Sum(p => p.Qty)
                                    }).ToList();

                //string selectLocationDetailStatement = "from LocationDetailView where HuId is null";
                //IList<object> selectLocationDetailParm = new List<object>();
                //selectLocationDetailStatement += GetWhereStatement(selectLocationDetailParm, stockTakeItemList, stockTakeLocationList, null);
                //IList<LocationDetailView> locationDetailList = this.genericMgr.FindAll<LocationDetailView>(selectLocationDetailStatement, selectLocationDetailParm.ToArray());

                //#region 合格品
                //stockTakeInvList = (from det in locationDetailList
                //                    where det.QualifyQty != 0
                //                    select new StockTakeInv
                //                    {
                //                        Location = det.Location,
                //                        Item = det.Item,
                //                        Qty = det.QualifyQty,
                //                        QualityType = CodeMaster.QualityType.Qualified,
                //                    }).ToList();
                //#endregion

                //#region 待验品
                //stockTakeInvList.AddRange(from det in locationDetailList
                //                          where det.InspectQty != 0
                //                          select new StockTakeInv
                //                          {
                //                              Location = det.Location,
                //                              Item = det.Item,
                //                              Qty = det.InspectQty,
                //                              QualityType = CodeMaster.QualityType.Inspect,
                //                          });
                //#endregion

                //#region 不合格品
                //stockTakeInvList.AddRange(from det in locationDetailList
                //                          where det.RejectQty != 0
                //                          select new StockTakeInv
                //                          {
                //                              Location = det.Location,
                //                              Item = det.Item,
                //                              Qty = det.RejectQty,
                //                              QualityType = CodeMaster.QualityType.Reject,
                //                          });
                //#endregion
            }

            foreach (var stockTakeInv in stockTakeInvList)
            {
                stockTakeInv.StNo = stockTakeMaster.StNo;
                this.genericMgr.Create(stockTakeInv);
            }
            #endregion

            this.genericMgr.Update(stockTakeMaster);
        }
        #endregion

        #region 取消盘点
        [Transaction(TransactionMode.Requires)]
        public void CancelStockTakeMaster(string stNo)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            CancelStockTakeMaster(stockTakeMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelStockTakeMaster(StockTakeMaster stockTakeMaster)
        {
            if (stockTakeMaster.Status == CodeMaster.StockTakeStatus.Create
                || stockTakeMaster.Status == CodeMaster.StockTakeStatus.Cancel
                || stockTakeMaster.Status == CodeMaster.StockTakeStatus.Close)
            {
                throw new BusinessException("盘点单{0}的状态为{1}，不能取消。",
                    stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }

            User user = SecurityContextHolder.Get();

            stockTakeMaster.Status = CodeMaster.StockTakeStatus.Cancel;
            stockTakeMaster.CancelUserId = user.Id;
            stockTakeMaster.CancelUserName = user.FullName;
            stockTakeMaster.CancelDate = DateTime.Now;

            this.genericMgr.Update(stockTakeMaster);
        }
        #endregion

        #region 记录盘点结果
        [Transaction(TransactionMode.Requires)]
        public void RecordStockTakeDetail(string stNo, IList<StockTakeDetail> stockTakeDetailList)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            RecordStockTakeDetail(stockTakeMaster, stockTakeDetailList);
        }

        private static object RecordStockTakeDetailLock = new object();
        [Transaction(TransactionMode.Requires)]
        public void RecordStockTakeDetail(StockTakeMaster stockTakeMaster, IList<StockTakeDetail> stockTakeDetailList)
        {
            lock (RecordStockTakeDetailLock)
            {
                #region 校查
                if (stockTakeDetailList == null || stockTakeDetailList.Count == 0)
                {
                    throw new BusinessException("盘点明细不能为空。");
                }
                IList<StockTakeDetail> noneZeroStockTakeDetailList = stockTakeDetailList.Where(d => d.Qty > 0).ToList();

                if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.InProcess)
                {
                    throw new BusinessException("盘点单{0}的状态为{1}，不能盘点。", stockTakeMaster.StNo,
                        this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
                }

                //检查库位是否正确
                BusinessException businessException = new BusinessException();

                IList<StockTakeLocation> stockTakeLocationList = this.genericMgr.FindAll<StockTakeLocation>
                    ("from StockTakeLocation as s where StNo = ? ", stockTakeMaster.StNo);
                var stockTakeLocationCodeList = stockTakeLocationList.Select(p => p.Location).ToList();
                foreach (string location in noneZeroStockTakeDetailList.Select(s => s.Location).Distinct())
                {
                    if (!stockTakeLocationCodeList.Contains(location))
                    {
                        businessException.AddMessage("库位{0}不在待盘点的库位列表中。", location);
                    }
                }
                var bins = stockTakeLocationList.Where(p => !string.IsNullOrWhiteSpace(p.Bins))
                   .SelectMany(p => p.Bins.Split(',')).ToList();
                if (bins.Count > 0)
                {
                    foreach (var bin in noneZeroStockTakeDetailList.Select(s => s.Bin).Distinct())
                    {
                        if (!bins.Contains(bin))
                        {
                            businessException.AddMessage("库格{0}不在待盘点的库格列表中。", bin);
                        }
                    }
                }

                //检查零件是否正确
                if (stockTakeMaster.Type == CodeMaster.StockTakeType.Part)
                {
                    IList<string> stockTakeItemList = this.genericMgr.FindAll<string>(SelectStockTakeItemStatement, stockTakeMaster.StNo);
                    foreach (string item in noneZeroStockTakeDetailList.Select(s => s.Item).Distinct())
                    {
                        if (!stockTakeItemList.Contains(item))
                        {
                            businessException.AddMessage("零件{0}不在待盘点的零件列表中。", item);
                        }
                    }
                }
                #endregion

                if (stockTakeMaster.IsScanHu)
                {
                    //删除重复项
                    var noDuplicateStockTakeDetailList = noneZeroStockTakeDetailList
                        .Where(p => !string.IsNullOrWhiteSpace(p.HuId))
                        .GroupBy(p => p.HuId)
                        .Select(p =>
                        {
                            return p.First();
                        })
                        .ToList();

                    //按条码盘点查询条码

                    var oldTakedDetailList = this.genericMgr.FindAll<StockTakeDetail>
                        (" from StockTakeDetail where StNo = ?", stockTakeMaster.StNo) ?? new List<StockTakeDetail>();

                    foreach (StockTakeDetail stockTakeDetail in noDuplicateStockTakeDetailList)
                    {
                        var oldTakedDetail = oldTakedDetailList.FirstOrDefault(p => p.HuId == stockTakeDetail.HuId);
                        if (oldTakedDetail == null)
                        {
                            //未扫描到的
                            stockTakeDetail.StNo = stockTakeMaster.StNo;
                            #region 计算基本单位和转换率
                            stockTakeDetail.BaseUom = itemMgr.GetCacheItem(stockTakeDetail.Item).Uom;
                            if (stockTakeDetail.BaseUom != stockTakeDetail.Uom)
                            {
                                stockTakeDetail.UnitQty = this.itemMgr.ConvertItemUomQty(stockTakeDetail.Item, stockTakeDetail.Uom, 1, stockTakeDetail.BaseUom);
                            }
                            else
                            {
                                stockTakeDetail.UnitQty = 1;
                            }
                            #endregion
                            this.genericMgr.Create(stockTakeDetail);
                        }
                        else if (oldTakedDetail.Bin != stockTakeDetail.Bin)
                        {
                            //库格不对的
                            oldTakedDetail.Bin = stockTakeDetail.Bin;
                            this.genericMgr.Update(oldTakedDetail);
                        }
                        else
                        {
                            //已扫到的,忽略不做
                        }
                    }
                    this.locationDetailMgr.InventoryPut(noDuplicateStockTakeDetailList);
                }
                else
                {
                    //按数量盘点查询零件号
                    IList<object[]> takedDetailList = this.genericMgr.FindAll<object[]>
                        ("select distinct Item, QualityType from StockTakeDetail where StNo = ?", stockTakeMaster.StNo);
                    if (takedDetailList != null && takedDetailList.Count > 0)
                    {
                        var duplicatedTakeDetailList = from takedDet in takedDetailList
                                                       join stockTakeDetail in noneZeroStockTakeDetailList
                                                       on new
                                                       {
                                                           Item = (string)takedDet[0],
                                                           QualityType = (CodeMaster.QualityType)takedDet[1]
                                                       } equals new { Item = stockTakeDetail.Item, QualityType = stockTakeDetail.QualityType }
                                                       select new
                                                       {
                                                           Item = stockTakeDetail.Item,
                                                           QualityType = stockTakeDetail.QualityType,
                                                       };

                        if (duplicatedTakeDetailList != null && duplicatedTakeDetailList.Count() > 0)
                        {
                            foreach (var duplicatedTakeDetail in duplicatedTakeDetailList)
                            {
                                businessException.AddMessage("质量状态为{1}的零件{0}重复盘点。", duplicatedTakeDetail.Item,
                                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.QualityType, ((int)duplicatedTakeDetail.QualityType).ToString()));
                            }
                        }
                    }

                    #region 循环更新盘点明细
                    foreach (StockTakeDetail stockTakeDetail in stockTakeDetailList)
                    {
                        stockTakeDetail.StNo = stockTakeMaster.StNo;
                        #region 计算基本单位和转换率
                        stockTakeDetail.BaseUom = itemMgr.GetCacheItem(stockTakeDetail.Item).Uom;
                        if (stockTakeDetail.BaseUom != stockTakeDetail.Uom)
                        {
                            stockTakeDetail.UnitQty = this.itemMgr.ConvertItemUomQty(stockTakeDetail.Item, stockTakeDetail.Uom, 1, stockTakeDetail.BaseUom);
                        }
                        else
                        {
                            stockTakeDetail.UnitQty = 1;
                        }
                        #endregion

                        this.genericMgr.Create(stockTakeDetail);
                    }
                    #endregion
                }

                if (businessException.HasMessage)
                {
                    throw businessException;
                }

                //this.genericMgr.Update(stockTakeMaster);
            }
        }
        #endregion

        #region 显示盘点结果
        public IList<StockTakeResultSummary> ListStockTakeResult(string stNo, bool listShortage, bool listProfit, bool listMatch, IList<string> locationList, IList<string> binList, IList<string> itemList, DateTime? BaseInventoryDate)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            return ListStockTakeResult(stockTakeMaster, listShortage, listProfit, listMatch, locationList, binList, itemList, BaseInventoryDate);
        }

        public IList<StockTakeResultSummary> ListStockTakeResult(StockTakeMaster stockTakeMaster, bool listShortage, bool listProfit, bool listMatch, IList<string> locationList, IList<string> binList, IList<string> itemList, DateTime? BaseInventoryDate)
        {
            if (stockTakeMaster.Status == CodeMaster.StockTakeStatus.Create
                || stockTakeMaster.Status == CodeMaster.StockTakeStatus.Cancel
                || stockTakeMaster.Status == CodeMaster.StockTakeStatus.Submit)
            {
                throw new BusinessException("盘点单{0}的状态为{1}，不能显示盘点结果。", stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }
            if (stockTakeMaster.Type == CodeMaster.StockTakeType.Part && (itemList == null || itemList.Count() == 0))
            {
                if (stockTakeMaster.StockTakeItems == null)
                {
                    stockTakeMaster.StockTakeItems = this.genericMgr.FindAll<StockTakeItem>("from StockTakeItem where StNo = ? ", stockTakeMaster.StNo);
                    if (itemList == null || itemList.Count() == 0)
                    {
                        itemList = stockTakeMaster.StockTakeItems.Select(p => p.Item).Distinct().ToList();
                    }
                }
            }
            if (stockTakeMaster.StockTakeLocations == null)
            {
                stockTakeMaster.StockTakeLocations = this.genericMgr.FindAll<StockTakeLocation>("from StockTakeLocation where StNo = ? ", stockTakeMaster.StNo);
                if (locationList == null || locationList.Count() == 0)
                {
                    locationList = stockTakeMaster.StockTakeLocations.Select(p => p.Location).Distinct().ToList();
                }
            }
            IList<StockTakeResultSummary> stockTakeResultSummaryList = new List<StockTakeResultSummary>();
            if (stockTakeMaster.Status == CodeMaster.StockTakeStatus.InProcess)
            {
                #region 执行中返回库存和盘点结果比较值
                IList<StockTakeResult> stockTakeResultList = CalStockTakeResult(stockTakeMaster, listShortage, listProfit, listMatch, locationList, binList, itemList, BaseInventoryDate);
                if (stockTakeMaster.IsScanHu)
                {
                    stockTakeResultSummaryList = (from tak in stockTakeResultList
                                                  group tak by new
                                                  {
                                                      Item = tak.Item,
                                                      //ItemDescription = tak.ItemDescription,
                                                      Uom = tak.Uom,
                                                      Location = tak.Location,
                                                      //LotNo = tak.LotNo,
                                                      Bin = tak.Bin,
                                                      StNo = tak.StNo,
                                                  } into result
                                                  select new StockTakeResultSummary
                                                  {
                                                      StNo = result.Key.StNo,
                                                      Bin = result.Key.Bin,
                                                      Item = result.Key.Item,
                                                      ItemDescription = result.First().ItemDescription,
                                                      Uom = result.Key.Uom,
                                                      Location = result.Key.Location,
                                                      //LotNo = result.Key.LotNo,
                                                      MatchQty = result.Sum(t => t.DifferenceQty == 0 ? t.InventoryQty : 0),
                                                      ShortageQty = result.Sum(t => t.DifferenceQty < 0 ? t.InventoryQty : 0),
                                                      ProfitQty = result.Sum(t => t.DifferenceQty > 0 ? t.StockTakeQty : 0),
                                                  }).ToList();
                }
                else
                {
                    stockTakeResultSummaryList = (from tak in stockTakeResultList
                                                  select new StockTakeResultSummary
                                                  {
                                                      Item = tak.Item,
                                                      ItemDescription = tak.ItemDescription,
                                                      Uom = tak.Uom,
                                                      Location = tak.Location,
                                                      QualityType = tak.QualityType,
                                                      InventoryQty = tak.InventoryQty,
                                                      StockTakeQty = tak.StockTakeQty,
                                                      DifferenceQty = tak.DifferenceQty,
                                                  }).ToList();
                }
                #endregion
            }
            else
            {
                #region 完成后
                if (stockTakeMaster.IsScanHu)
                {
                    #region 按条码

                    #region 盘亏
                    if (listShortage)
                    {
                        #region 查询盘亏结果
                        string selectShortageStatement = @"select Item, ItemDescription, QualityType, Uom, Location, Bin, LotNo, DifferenceQty as ShortageQty ,StNo,Id,IsAdjust
                                                        from StockTakeResult where StNo = ? and DifferenceQty < 0  ";
                        IList<object> selectShortageParas = new List<object>();
                        selectShortageParas.Add(stockTakeMaster.StNo);
                        selectShortageStatement += GetWhereStatement(selectShortageParas, itemList, locationList, binList);
                        IList<object[]> shorageList = this.genericMgr.FindAll<object[]>(selectShortageStatement, selectShortageParas.ToArray());
                        #endregion

                        #region 转换为StockTakeResultSummary
                        ((List<StockTakeResultSummary>)stockTakeResultSummaryList).AddRange
                            ((from shorage in shorageList
                              select new StockTakeResultSummary
                              {
                                  Item = (string)shorage[0],
                                  ItemDescription = (string)shorage[1],
                                  QualityType = (CodeMaster.QualityType)shorage[2],
                                  Uom = (string)shorage[3],
                                  Location = (string)shorage[4],
                                  Bin = (string)shorage[5],
                                  LotNo = (string)shorage[6],
                                  MatchQty = 0,
                                  ShortageQty = (decimal)shorage[7],
                                  StNo = (string)shorage[8],
                                  Id = (int)shorage[9],
                                  IsAdjust = (Boolean)shorage[10],
                                  ProfitQty = 0
                              }).ToList());
                        #endregion
                    }
                    #endregion

                    #region 盘盈
                    if (listProfit)
                    {
                        #region 查询盘盈结果
                        string selectProfitStatement = @"select Item, ItemDescription, QualityType, Uom, Location, Bin, LotNo, DifferenceQty as ProfitQty ,StNo,Id,IsAdjust
                                                        from StockTakeResult where StNo = ? and DifferenceQty > 0 ";
                        IList<object> selectProfitParas = new List<object>();
                        selectProfitParas.Add(stockTakeMaster.StNo);
                        selectProfitStatement += GetWhereStatement(selectProfitParas, itemList, locationList, binList);
                        IList<object[]> profitList = this.genericMgr.FindAll<object[]>(selectProfitStatement, selectProfitParas.ToArray());
                        #endregion

                        #region 转换为StockTakeResultSummary
                        ((List<StockTakeResultSummary>)stockTakeResultSummaryList).AddRange
                            ((from profit in profitList
                              select new StockTakeResultSummary
                              {
                                  Item = (string)profit[0],
                                  ItemDescription = (string)profit[1],
                                  QualityType = (CodeMaster.QualityType)profit[2],
                                  Uom = (string)profit[3],
                                  Location = (string)profit[4],
                                  Bin = (string)profit[5],
                                  LotNo = (string)profit[6],
                                  MatchQty = 0,
                                  ShortageQty = 0,
                                  ProfitQty = (decimal)profit[7],
                                  StNo = (string)profit[8],
                                  Id = (int)profit[9],
                                  IsAdjust = (Boolean)profit[10],
                              }).ToList());
                        #endregion
                    }
                    #endregion

                    #region 账实相符
                    if (listMatch)
                    {
                        #region 查询盘盈结果
                        string selectMatchStatement = @"select Item, ItemDescription, QualityType, Uom, Location, Bin, LotNo, InventoryQty as MatchQty  ,StNo,Id,IsAdjust
                                                        from StockTakeResult where StNo = ? and DifferenceQty = 0 ";
                        IList<object> selectMatchParas = new List<object>();
                        selectMatchParas.Add(stockTakeMaster.StNo);
                        selectMatchStatement += GetWhereStatement(selectMatchParas, itemList, locationList, binList);
                        IList<object[]> matchList = this.genericMgr.FindAll<object[]>(selectMatchStatement, selectMatchParas.ToArray());
                        #endregion

                        #region 转换为StockTakeResultSummary
                        ((List<StockTakeResultSummary>)stockTakeResultSummaryList).AddRange
                            ((from match in matchList
                              select new StockTakeResultSummary
                              {
                                  Item = (string)match[0],
                                  ItemDescription = (string)match[1],
                                  QualityType = (CodeMaster.QualityType)match[2],
                                  Uom = (string)match[3],
                                  Location = (string)match[4],
                                  Bin = (string)match[5],
                                  LotNo = (string)match[6],
                                  MatchQty = (decimal)match[7],
                                  StNo = (string)match[8],
                                  Id = (int)match[9],
                                  IsAdjust = (Boolean)match[10],
                                  ShortageQty = 0,
                                  ProfitQty = 0,
                              }).ToList());
                        #endregion
                    }
                    #endregion

                    #region 汇总
                    stockTakeResultSummaryList = (from sum in stockTakeResultSummaryList
                                                  group sum by new
                                                  {
                                                      Item = sum.Item,
                                                      ItemDescription = sum.ItemDescription,
                                                      QualityType = sum.QualityType,
                                                      Uom = sum.Uom,
                                                      Location = sum.Location,
                                                      Bin = sum.Bin,
                                                      //LotNo = sum.LotNo,
                                                      StNo = sum.StNo
                                                  } into result
                                                  select new StockTakeResultSummary
                                                  {
                                                      StNo = result.Key.StNo,
                                                      Item = result.Key.Item,
                                                      ItemDescription = result.Key.ItemDescription,
                                                      Uom = result.Key.Uom,
                                                      QualityType = result.Key.QualityType,
                                                      Location = result.Key.Location,
                                                      Bin = result.Key.Bin,
                                                      ProfitQty = result.Sum(sum => sum.ProfitQty),
                                                      ShortageQty = 0 - result.Sum(sum => sum.ShortageQty),
                                                      MatchQty = result.Sum(sum => sum.MatchQty),
                                                  }
                                                  ).OrderBy(c => c.Item).ThenBy(c => c.LotNo).ToList();
                    #endregion
                    #endregion
                }
                else
                {
                    #region 按数量
                    string selectStockTakeResultStatement = "from StockTakeResult where StNo = ?";
                    IList<object> selectStockTakeResultParas = new List<object>();
                    selectStockTakeResultParas.Add(stockTakeMaster.StNo);
                    if (!(listShortage && listProfit && listMatch))
                    {
                        if (listShortage)
                        {
                            if (listProfit)
                            {
                                selectStockTakeResultStatement += " DifferenceQty <> 0";
                            }
                            else if (listMatch)
                            {
                                selectStockTakeResultStatement += " DifferenceQty <= 0";
                            }
                            else
                            {
                                selectStockTakeResultStatement += " DifferenceQty < 0";
                            }
                        }
                        else if (listProfit)
                        {
                            if (listMatch)
                            {
                                selectStockTakeResultStatement += " DifferenceQty >= 0";
                            }
                            else
                            {
                                selectStockTakeResultStatement += " DifferenceQty > 0";
                            }
                        }
                        else
                        {
                            selectStockTakeResultStatement += " DifferenceQty = 0";
                        }
                    }
                    selectStockTakeResultStatement += GetWhereStatement(selectStockTakeResultParas, itemList, locationList, binList);
                    IList<StockTakeResult> stockTakeResultList = this.genericMgr.FindAll<StockTakeResult>(selectStockTakeResultStatement, selectStockTakeResultParas.ToArray());

                    #region 汇总
                    stockTakeResultSummaryList = (from rst in stockTakeResultList
                                                  select new StockTakeResultSummary
                                                  {
                                                      IsAdjust = rst.IsAdjust,
                                                      Id = rst.Id,
                                                      Item = rst.Item,
                                                      ItemDescription = rst.ItemDescription,
                                                      QualityType = rst.QualityType,
                                                      Uom = rst.Uom,
                                                      Location = rst.Location,
                                                      InventoryQty = rst.InventoryQty,
                                                      StockTakeQty = rst.StockTakeQty,
                                                      DifferenceQty = rst.DifferenceQty,
                                                  }
                                                  ).OrderBy(c => c.Item).ThenBy(c => c.LotNo).ToList();
                    #endregion
                    #endregion
                }
                #endregion
            }

            return stockTakeResultSummaryList;
        }

        public IList<StockTakeResult> ListStockTakeResultDetail(string stNo, bool listShortage, bool listProfit, bool listMatch, IList<string> locationList, IList<string> binList, IList<string> itemList, DateTime? BaseInventoryDate)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            return ListStockTakeResultDetail(stockTakeMaster, listShortage, listProfit, listMatch, locationList, binList, itemList, BaseInventoryDate);
        }

        public IList<StockTakeResult> ListStockTakeResultDetail(StockTakeMaster stockTakeMaster, bool listShortage, bool listProfit, bool listMatch, IList<string> locationList, IList<string> binList, IList<string> itemList, DateTime? BaseInventoryDate)
        {
            if (stockTakeMaster.Status == CodeMaster.StockTakeStatus.Create
                || stockTakeMaster.Status == CodeMaster.StockTakeStatus.Cancel
                || stockTakeMaster.Status == CodeMaster.StockTakeStatus.Submit)
            {
                throw new BusinessException("盘点单{0}的状态为{1}，不能显示盘点结果。", stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }

            IList<StockTakeResult> stockTakeResultList = new List<StockTakeResult>();
            if (stockTakeMaster.Status == CodeMaster.StockTakeStatus.InProcess)
            {
                #region 执行中返回库存和盘点结果比较值
                stockTakeResultList = CalStockTakeResult(stockTakeMaster, listShortage, listProfit, listMatch, locationList, binList, itemList, BaseInventoryDate);
                #endregion
            }
            else
            {
                #region 查询点明细结果，只有条码才有
                string selectStatement = @"from StockTakeResult where StNo = ? ";
                IList<object> selectParas = new List<object>();
                selectParas.Add(stockTakeMaster.StNo);
                if (listShortage)
                {
                    selectStatement += " and DifferenceQty<0";
                }
                else if (listProfit)
                {
                    selectStatement += " and DifferenceQty>0";
                }
                else
                {
                    selectStatement += " and DifferenceQty=0";
                }
                selectStatement += GetWhereStatement(selectParas, itemList, locationList, binList);
                stockTakeResultList = this.genericMgr.FindAll<StockTakeResult>(selectStatement, selectParas.ToArray());
                #endregion
            }
            return stockTakeResultList;
        }

        private string GetWhereStatement(IList<object> selectParas, IList<string> itemList, IList<string> locationList, IList<string> binList)
        {
            string whereStatement = string.Empty;
            if (itemList != null && itemList.Count > 0)
            {
                foreach (string item in itemList)
                {
                    if (itemList.First() == item)
                    {
                        whereStatement += " and Item in (?";
                    }
                    else
                    {
                        whereStatement += ", ?";
                    }
                    selectParas.Add(item);
                }
                whereStatement += ")";
            }

            if (locationList != null && locationList.Count > 0)
            {
                foreach (string location in locationList)
                {
                    if (locationList.First() == location)
                    {
                        whereStatement += " and Location in (?";
                    }
                    else
                    {
                        whereStatement += ", ?";
                    }
                    selectParas.Add(location);
                }
                whereStatement += ")";
            }

            if (binList != null && binList.Count > 0)
            {
                foreach (string bin in binList)
                {
                    if (binList.First() == bin)
                    {
                        whereStatement += " and Bin in (?";
                    }
                    else
                    {
                        whereStatement += ", ?";
                    }
                    selectParas.Add(bin);
                }
                whereStatement += ")";
            }

            return whereStatement;
        }

        private IList<StockTakeResult> CalStockTakeResult(StockTakeMaster stockTakeMaster, bool listShortage, bool listProfit, bool listMatch, IList<string> locationList, IList<string> binList, IList<string> itemList, DateTime? baseInventoryDate)
        {
            var stockTakeLocationList = this.genericMgr.FindAll<StockTakeLocation>
                    ("from StockTakeLocation as s where StNo = ?", stockTakeMaster.StNo);
            if (locationList == null || locationList.Count() == 0)
            {
                locationList = stockTakeLocationList.Select(p => p.Location).ToList();
            }

            if (stockTakeMaster.Type == CodeMaster.StockTakeType.Part && (itemList == null || itemList.Count() == 0))
            {
                itemList = this.genericMgr.FindAll<string>(SelectStockTakeItemStatement, stockTakeMaster.StNo);
            }

            var stockTakeResultList = new List<StockTakeResult>();
            baseInventoryDate = baseInventoryDate.HasValue ? baseInventoryDate.Value : stockTakeMaster.BaseInventoryDate;
            if (baseInventoryDate == null)
            {
                //默认取当天
                baseInventoryDate = DateTime.Now;
            }
            DateTime dateTimeNow = DateTime.Now;
            if (stockTakeMaster.IsScanHu)
            {
                #region 按条码盘点

                #region 查找库存
                var bins = stockTakeLocationList.Where(p => !string.IsNullOrWhiteSpace(p.Bins))
                    .SelectMany(p => p.Bins.Split(',')).ToList();

                IList<LocationLotDetail> locationLotDetailList = null;
                if (baseInventoryDate.HasValue && bins.Count == 0)
                {
                    if (binList != null && binList.Count > 0)
                    {
                        //throw new BusinessException("不支持按库格查看条码历史库存。");
                    }

                    string selectStockTakeInvStatement = "from StockTakeInv where StNo=? ";
                    IList<object> selectStockTakeInvParm = new List<object> { stockTakeMaster.StNo };
                    selectStockTakeInvStatement += GetWhereStatement(selectStockTakeInvParm, itemList, locationList, binList);
                    var stockTakeInvList = this.genericMgr.FindAll<StockTakeInv>(selectStockTakeInvStatement, selectStockTakeInvParm.ToArray());

                    locationLotDetailList = (from inv in stockTakeInvList
                                             where inv.QualityType == CodeMaster.QualityType.Qualified
                                             select new LocationLotDetail
                                             {
                                                 Location = inv.Location,
                                                 Item = inv.Item,
                                                 HuId = inv.HuId,
                                                 Bin = inv.Bin,
                                                 LotNo = inv.LotNo,
                                                 Qty = inv.Qty,
                                                 QualityType = inv.QualityType
                                             }).ToList();
                }
                else
                {
                    string selectHuLocationLotDetailStatement = "from LocationLotDetail where HuId is not null ";
                    IList<object> selectHuLocationLotDetailParm = new List<object>();
                    selectHuLocationLotDetailStatement += GetWhereStatement(selectHuLocationLotDetailParm, itemList, locationList, bins);
                    locationLotDetailList = this.genericMgr.FindAll<LocationLotDetail>(selectHuLocationLotDetailStatement, selectHuLocationLotDetailParm.ToArray());
                }
                #endregion

                #region 期间库存事务
                //将来改成存储过程 todo
                string locationSql = string.Empty;
                foreach (var location in locationList)
                {
                    if (locationSql == string.Empty)
                    {
                        locationSql = "('" + location;
                    }
                    else
                    {
                        locationSql += "','" + location;
                    }
                }
                locationSql += "')";
                //string sql = string.Format(@"select distinct(huId) from VIEW_LocTrans where  CreateDate>=? and HuId is not null 
                //            and ((LocFrom in {0} and IOType =1) or (LocTo in {1} and IOType =0)) ", locationSql, locationSql);
                //var activeHuIds = this.genericMgr.FindAllWithNativeSql<string>(sql, baseInventoryDate.Value).ToArray();
                string sql = string.Format(@"select distinct(huId) from VIEW_LocTrans where  CreateDate>=? and HuId is not null 
                         and LocTo in {0} and IOType =0 ", locationSql);
                var inHuIds = this.genericMgr.FindAllWithNativeSql<string>(sql, baseInventoryDate.Value).ToArray();

                sql = string.Format(@"select distinct(huId) from VIEW_LocTrans where  CreateDate>=? and HuId is not null 
                         and LocFrom in {0} and IOType =1 ", locationSql);
                var outHuIds = this.genericMgr.FindAllWithNativeSql<string>(sql, baseInventoryDate.Value).ToArray();
                //var activeHuIds = inHuIds.Where(p => !outHuIds.Contains(p)).ToArray();
                var allHuIds = inHuIds.Union(outHuIds).Distinct().ToArray();
                #endregion

                #region 查找盘点结果
                IList<object> selectStockTakeDetailParm = new List<object>();
                selectStockTakeDetailParm.Add(stockTakeMaster.StNo);
                string thisSelectStockTakeDetailStatement = SelectStockTakeDetailStatement + GetWhereStatement(selectStockTakeDetailParm, itemList, locationList, binList);
                IList<StockTakeDetail> stockTakeDetailList = this.genericMgr.FindAll<StockTakeDetail>(thisSelectStockTakeDetailStatement, selectStockTakeDetailParm.ToArray());
                //去掉已出库的
                stockTakeDetailList = stockTakeDetailList.Where(p => !outHuIds.Contains(p.HuId)).ToList();
                #endregion

                #region 盘亏\盘赢\账实相符
                #region 盘亏
                if (listShortage)
                {
                    //有出入库事务就不能算盘亏
                    if (baseInventoryDate.HasValue && bins.Count() == 0)
                    {
                        #region 比较历史库存，不看Bin
                        stockTakeResultList.AddRange((from loc in locationLotDetailList
                                                      join tak in stockTakeDetailList
                                                      on
                                                      new
                                                      {
                                                          Location = loc.Location,
                                                          Item = loc.Item,
                                                          HuId = loc.HuId
                                                      }
                                                      equals
                                                      new
                                                      {
                                                          Location = tak.Location,
                                                          Item = tak.Item,
                                                          HuId = tak.HuId
                                                      }
                                                      into gj
                                                      from result in gj.DefaultIfEmpty()
                                                      where result == null && !allHuIds.Contains(loc.HuId)
                                                      select new StockTakeResult
                                                      {
                                                          Item = loc.Item,
                                                          HuId = loc.HuId,
                                                          LotNo = loc.LotNo,
                                                          StockTakeQty = 0,
                                                          InventoryQty = loc.Qty,
                                                          DifferenceQty = 0 - loc.Qty,
                                                          Location = loc.Location,
                                                          //Bin = string.Empty,
                                                          BaseInventoryDate = baseInventoryDate.Value,
                                                          QualityType = loc.QualityType,
                                                          IsCS = loc.IsConsignment
                                                      }).ToList());
                        #endregion
                    }
                    else
                    {
                        #region 比较当前库存，看Bin
                        stockTakeResultList.AddRange((from loc in locationLotDetailList
                                                      join tak in stockTakeDetailList
                                                      on
                                                      new
                                                      {
                                                          Location = loc.Location,
                                                          Bin = loc.Bin,
                                                          Item = loc.Item,
                                                          HuId = loc.HuId
                                                      }
                                                      equals
                                                       new
                                                       {
                                                           Location = tak.Location,
                                                           Bin = tak.Bin,
                                                           Item = tak.Item,
                                                           HuId = tak.HuId
                                                       }
                                                      into gj
                                                      from result in gj.DefaultIfEmpty()
                                                      where result == null && !allHuIds.Contains(loc.HuId)
                                                      select new StockTakeResult
                                                      {
                                                          Item = loc.Item,
                                                          HuId = loc.HuId,
                                                          LotNo = loc.LotNo,
                                                          StockTakeQty = 0,
                                                          InventoryQty = loc.Qty,
                                                          DifferenceQty = 0 - loc.Qty,
                                                          Location = loc.Location,
                                                          Bin = loc.Bin,
                                                          BaseInventoryDate = dateTimeNow,
                                                          QualityType = loc.QualityType,
                                                          IsCS = loc.IsConsignment
                                                      }).ToList());
                        #endregion
                    }
                }
                #endregion

                #region 盘盈
                if (listProfit)
                {
                    //有出入库事务就不能算盘盈
                    if (baseInventoryDate.HasValue && bins.Count() == 0)
                    {
                        #region 比较历史库存，不看Bin
                        stockTakeResultList.AddRange((from tak in stockTakeDetailList
                                                      join loc in locationLotDetailList
                                                      on
                                                      new
                                                      {
                                                          Location = tak.Location,
                                                          Item = tak.Item,
                                                          HuId = tak.HuId
                                                      }
                                                      equals
                                                      new
                                                      {
                                                          Location = loc.Location,
                                                          Item = loc.Item,
                                                          HuId = loc.HuId
                                                      }
                                                      into gj
                                                      from result in gj.DefaultIfEmpty()
                                                      where result == null && !allHuIds.Contains(tak.HuId)
                                                      select new StockTakeResult
                                                      {
                                                          Item = tak.Item,
                                                          ItemDescription = tak.ItemDescription,
                                                          Uom = tak.BaseUom,
                                                          HuId = tak.HuId,
                                                          LotNo = tak.LotNo,
                                                          StockTakeQty = tak.Qty * tak.UnitQty,
                                                          InventoryQty = 0,
                                                          DifferenceQty = tak.Qty * tak.UnitQty,
                                                          Location = tak.Location,
                                                          //Bin = string.Empty,
                                                          BaseInventoryDate = baseInventoryDate.Value,
                                                          QualityType = tak.QualityType,
                                                          IsCS = false
                                                      }).ToList());
                        #endregion
                    }
                    else
                    {
                        #region 比较当前库存，看Bin
                        stockTakeResultList.AddRange((from tak in stockTakeDetailList
                                                      join loc in locationLotDetailList
                                                      on
                                                      new
                                                      {
                                                          Location = tak.Location,
                                                          Bin = tak.Bin,
                                                          Item = tak.Item,
                                                          HuId = tak.HuId
                                                      }
                                                      equals
                                                      new
                                                      {
                                                          Location = loc.Location,
                                                          Bin = loc.Bin,
                                                          Item = loc.Item,
                                                          HuId = loc.HuId
                                                      }
                                                      into gj
                                                      from result in gj.DefaultIfEmpty()
                                                      where result == null && !allHuIds.Contains(tak.HuId)
                                                      select new StockTakeResult
                                                      {
                                                          Item = tak.Item,
                                                          ItemDescription = tak.ItemDescription,
                                                          Uom = tak.BaseUom,
                                                          HuId = tak.HuId,
                                                          LotNo = tak.LotNo,
                                                          StockTakeQty = tak.Qty * tak.UnitQty,
                                                          InventoryQty = 0,
                                                          DifferenceQty = tak.Qty * tak.UnitQty,
                                                          Location = tak.Location,
                                                          Bin = tak.Bin,
                                                          BaseInventoryDate = dateTimeNow,
                                                          QualityType = tak.QualityType,
                                                          IsCS = false
                                                      }).ToList());
                        #endregion
                    }
                }
                #endregion

                #region 账实相符
                if (listMatch)
                {
                    if (baseInventoryDate.HasValue && bins.Count() == 0)
                    {
                        #region 比较历史库存，不看Bin
                        stockTakeResultList.AddRange((from loc in locationLotDetailList
                                                      join tak in stockTakeDetailList
                                                      on new
                                                      {
                                                          Location = loc.Location,
                                                          Item = loc.Item,
                                                          HuId = loc.HuId
                                                      }
                                                      equals new
                                                      {
                                                          Location = tak.Location,
                                                          Item = tak.Item,
                                                          HuId = tak.HuId
                                                      }
                                                      select new StockTakeResult
                                                      {
                                                          Item = loc.Item,
                                                          ItemDescription = tak.ItemDescription,
                                                          Uom = tak.BaseUom,
                                                          HuId = loc.HuId,
                                                          LotNo = loc.LotNo,
                                                          StockTakeQty = loc.Qty,
                                                          InventoryQty = loc.Qty,
                                                          DifferenceQty = 0,
                                                          Location = loc.Location,
                                                          //Bin = string.Empty,
                                                          BaseInventoryDate = baseInventoryDate.Value,
                                                          QualityType = loc.QualityType,
                                                      }).ToList());
                        #endregion
                    }
                    else
                    {
                        #region 比较当前库存，看Bin
                        stockTakeResultList.AddRange((from loc in locationLotDetailList
                                                      join tak in stockTakeDetailList
                                                      on new
                                                      {
                                                          Location = loc.Location,
                                                          Bin = loc.Bin,
                                                          Item = loc.Item,
                                                          HuId = loc.HuId
                                                      }
                                                      equals new
                                                      {
                                                          Location = tak.Location,
                                                          Bin = tak.Bin,
                                                          Item = tak.Item,
                                                          HuId = tak.HuId
                                                      }
                                                      select new StockTakeResult
                                                      {
                                                          Item = loc.Item,
                                                          ItemDescription = tak.ItemDescription,
                                                          Uom = tak.BaseUom,
                                                          HuId = loc.HuId,
                                                          LotNo = loc.LotNo,
                                                          StockTakeQty = loc.Qty,
                                                          InventoryQty = loc.Qty,
                                                          DifferenceQty = 0,
                                                          Location = loc.Location,
                                                          Bin = loc.Bin,
                                                          BaseInventoryDate = dateTimeNow,
                                                          QualityType = loc.QualityType,
                                                      }).ToList());
                        #endregion
                    }

                    stockTakeResultList.AddRange(locationLotDetailList
                        .Where(p => allHuIds.Contains(p.HuId) && !stockTakeDetailList.Select(q => q.HuId).Contains(p.HuId))
                        .Select(loc => new StockTakeResult
                        {
                            Item = loc.Item,
                            HuId = loc.HuId,
                            LotNo = loc.LotNo,
                            StockTakeQty = loc.Qty,
                            InventoryQty = loc.Qty,
                            DifferenceQty = 0,
                            Location = loc.Location,
                            Bin = baseInventoryDate.HasValue && bins.Count() == 0 ? null : loc.Bin,
                            BaseInventoryDate = dateTimeNow,
                            QualityType = loc.QualityType,
                            IsCS = loc.IsConsignment
                        }));

                    stockTakeResultList.AddRange(stockTakeDetailList
                       .Where(p => allHuIds.Contains(p.HuId) && !locationLotDetailList.Select(q => q.HuId).Contains(p.HuId))
                       .Select(tak => new StockTakeResult
                       {
                           Item = tak.Item,
                           HuId = tak.HuId,
                           LotNo = tak.LotNo,
                           StockTakeQty = tak.Qty * tak.UnitQty,
                           InventoryQty = tak.Qty * tak.UnitQty,
                           DifferenceQty = 0,
                           Location = tak.Location,
                           Bin = baseInventoryDate.HasValue && bins.Count() == 0 ? null : tak.Bin,
                           BaseInventoryDate = dateTimeNow,
                           QualityType = tak.QualityType,
                           IsCS = false
                       }));
                }
                #endregion

                #endregion
                if (bins.Count > 0)
                {
                    stockTakeResultList = stockTakeResultList.Where(p => bins.Contains(p.Bin)).ToList();
                }
                #endregion
            }
            else
            {
                #region 按数量盘点

                #region 查找盘点结果
                IList<object> selectStockTakeDetailParm = new List<object>();
                selectStockTakeDetailParm.Add(stockTakeMaster.StNo);
                string thisSelectStockTakeDetailStatement = SelectStockTakeDetailStatement + GetWhereStatement(selectStockTakeDetailParm, itemList, locationList, binList);
                IList<StockTakeDetail> stockTakeDetailList = this.genericMgr.FindAll<StockTakeDetail>(thisSelectStockTakeDetailStatement, selectStockTakeDetailParm.ToArray());
                #endregion

                var locationLotDetailList = new List<LocationLotDetail>();
                #region 查找库存
                if (baseInventoryDate.HasValue)
                {
                    string selectStockTakeInvStatement = "from StockTakeInv where StNo=? ";
                    IList<object> selectStockTakeInvParm = new List<object> { stockTakeMaster.StNo };
                    selectStockTakeInvStatement += GetWhereStatement(selectStockTakeInvParm, itemList, locationList, binList);
                    var stockTakeInvList = this.genericMgr.FindAll<StockTakeInv>(selectStockTakeInvStatement, selectStockTakeInvParm.ToArray());

                    locationLotDetailList = (from p in stockTakeInvList
                                             select new LocationLotDetail
                                             {
                                                 Location = p.Location,
                                                 Item = p.Item,
                                                 Qty = p.Qty,
                                                 QualityType = p.QualityType,
                                             }).ToList();
                }
                else
                {
                    string selectLocationLotDetailStatement = "from LocationLotDetail where HuId is null";
                    IList<object> selectLocationLotDetailParm = new List<object>();
                    selectLocationLotDetailStatement += GetWhereStatement(selectLocationLotDetailParm, itemList, locationList, null);
                    var locationLotDetails = this.genericMgr.FindAll<LocationLotDetail>(selectLocationLotDetailStatement, selectLocationLotDetailParm.ToArray());
                    locationLotDetailList = (from inv in locationLotDetails
                                             group inv by new { inv.Item, inv.Location, inv.QualityType } into g
                                             select new LocationLotDetail
                                            {
                                                Location = g.Key.Location,
                                                Item = g.Key.Item,
                                                QualityType = g.Key.QualityType,
                                                Qty = g.Sum(p => p.Qty)
                                            }).ToList();
                }

                #endregion

                #region 计算盘点差异
                stockTakeResultList.AddRange(from tak in stockTakeDetailList
                                             join inv in locationLotDetailList
                                             on new { Location = tak.Location, Item = tak.Item, QualityType = tak.QualityType }
                                             equals new { Location = inv.Location, Item = inv.Item, QualityType = inv.QualityType }
                                             into gj
                                             from result in gj.DefaultIfEmpty()
                                             select new StockTakeResult
                                             {
                                                 Location = tak.Location,
                                                 Item = tak.Item,
                                                 ItemDescription = tak.ItemDescription,
                                                 Uom = tak.BaseUom,
                                                 StockTakeQty = tak.Qty * tak.UnitQty,
                                                 InventoryQty = result != null ? result.Qty : 0,
                                                 DifferenceQty = tak.Qty * tak.UnitQty - (result != null ? result.Qty : 0),
                                                 BaseInventoryDate = baseInventoryDate.Value,
                                                 QualityType = tak.QualityType,
                                             });

                stockTakeResultList.AddRange(from inv in locationLotDetailList
                                             join tak in stockTakeDetailList
                                             on new { Location = inv.Location, Item = inv.Item, QualityType = inv.QualityType }
                                             equals new { Location = tak.Location, Item = tak.Item, QualityType = tak.QualityType }
                                             into gj
                                             from result2 in gj.DefaultIfEmpty()
                                             where result2 == null
                                             select new StockTakeResult
                                             {
                                                 Location = inv.Location,
                                                 Item = inv.Item,
                                                 StockTakeQty = 0,
                                                 InventoryQty = inv.Qty,
                                                 DifferenceQty = 0 - inv.Qty,
                                                 BaseInventoryDate = baseInventoryDate.Value,
                                                 QualityType = inv.QualityType,
                                             });
                #endregion

                #region 根据查询条件过滤
                if (!(listShortage && listProfit && listMatch))
                {
                    if (listShortage)
                    {
                        if (listProfit)
                        {
                            stockTakeResultList = stockTakeResultList.Where(c => c.DifferenceQty != 0).ToList();
                        }
                        else if (listMatch)
                        {
                            stockTakeResultList = stockTakeResultList.Where(c => c.DifferenceQty <= 0).ToList();
                        }
                        else
                        {
                            stockTakeResultList = stockTakeResultList.Where(c => c.DifferenceQty < 0).ToList();
                        }
                    }
                    else if (listProfit)
                    {
                        if (listMatch)
                        {
                            stockTakeResultList = stockTakeResultList.Where(c => c.DifferenceQty >= 0).ToList();
                        }
                        else
                        {
                            stockTakeResultList = stockTakeResultList.Where(c => c.DifferenceQty > 0).ToList();
                        }
                    }
                    else
                    {
                        stockTakeResultList = stockTakeResultList.Where(c => c.DifferenceQty == 0).ToList();
                    }
                }
                #endregion
                #endregion
            }

            #region 查询零件描述和基本单位
            foreach (StockTakeResult stockTakeResult in stockTakeResultList.Where(s => string.IsNullOrWhiteSpace(s.Uom)))
            {
                stockTakeResult.ItemDescription = itemMgr.GetCacheItem(stockTakeResult.Item).Description;
                stockTakeResult.Uom = itemMgr.GetCacheItem(stockTakeResult.Item).Uom;
            }
            #endregion

            stockTakeResultList = stockTakeResultList.OrderBy(c => c.Item).ThenBy(c => c.DifferenceQty).ToList();

            return stockTakeResultList;
        }
        #endregion

        #region 盘点完成
        [Transaction(TransactionMode.Requires)]
        public void CompleteStockTakeMaster(string stNo, DateTime? baseInventoryDate)
        {
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);
            CompleteStockTakeMaster(stockTakeMaster, baseInventoryDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void CompleteStockTakeMaster(StockTakeMaster stockTakeMaster, DateTime? baseInventoryDate)
        {
            if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.InProcess)
            {
                throw new BusinessException("盘点单{0}的状态为{1}，不能完工。", stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }

            IList<string> stockTakeLocationList = this.genericMgr.FindAll<string>(SelectStockTakeLocationStatement, stockTakeMaster.StNo);

            IList<StockTakeResult> resultList = CalStockTakeResult(stockTakeMaster, true, true, true, stockTakeLocationList.ToList(), null, null, baseInventoryDate);
            if (resultList != null && resultList.Count > 0)
            {
                foreach (StockTakeResult result in resultList)
                {
                    result.StNo = stockTakeMaster.StNo;
                    result.EffectiveDate = stockTakeMaster.EffectiveDate;
                    this.genericMgr.Create(result);
                }
            }

            stockTakeMaster.CompleteDate = DateTime.Now;
            User user = SecurityContextHolder.Get();
            stockTakeMaster.CompleteUserId = user.Id;
            stockTakeMaster.Status = CodeMaster.StockTakeStatus.Complete;
            stockTakeMaster.CompleteUserName = user.FullName;

            this.genericMgr.Update(stockTakeMaster);

            TryCloseStockTakeMaster(stockTakeMaster);
        }
        #endregion

        #region 盘点调整
        [Transaction(TransactionMode.Requires)]
        public void AdjustStockTakeResult(string stNo, DateTime? effectiveDate)
        {
            IList<StockTakeResult> stockTakeResultList = this.genericMgr.FindAll<StockTakeResult>
                ("from StockTakeResult where StNo = ? and IsAdjust = ? and QualityType=? ",
                new object[] { stNo, false, CodeMaster.QualityType.Qualified });
            AdjustStockTakeResult(stockTakeResultList, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void AdjustStockTakeResult(IList<int> stockTakeResultIdList, DateTime? effectiveDate)
        {
            if (stockTakeResultIdList != null && stockTakeResultIdList.Count > 0)
            {
                var stocktakeResultList = this.genericMgr.FindAllIn<StockTakeResult>
                    ("from StockTakeResult where IsAdjust = ? and QualityType=? and Id in(?",
                    stockTakeResultIdList.Select(p => (object)p), new object[] { false, CodeMaster.QualityType.Qualified });
                AdjustStockTakeResult(stocktakeResultList, effectiveDate);
            }
            else
            {
                throw new BusinessException("盘点调整结果不能为空。");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void AdjustStockTakeResult(IList<StockTakeResult> stockTakeResultList, DateTime? effectiveDate)
        {
            #region 检查
            IList<StockTakeResult> notAjustStockTakeResultList = stockTakeResultList
                .Where(s => !s.IsAdjust).ToList();

            if (notAjustStockTakeResultList == null || notAjustStockTakeResultList.Count == 0)
            {
                throw new BusinessException("盘点调整结果不能为空。");
            }

            if (notAjustStockTakeResultList.Where(s => s.QualityType != CodeMaster.QualityType.Qualified).Count() > 0)
            {
                throw new BusinessException("不能调整质量状态为{0}的盘点结果。",
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.QualityType, ((int)CodeMaster.QualityType.Inspect).ToString()));
            }
            string stNo = notAjustStockTakeResultList.Select(s => s.StNo).Distinct().Single();
            StockTakeMaster stockTakeMaster = this.genericMgr.FindById<StockTakeMaster>(stNo);

            if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.Complete)
            {
                throw new BusinessException("盘点单{0}的状态为{1}，不能调整盘点结果。", stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }
            #endregion

            #region 更新盘点结果
            foreach (StockTakeResult stockTakeResult in notAjustStockTakeResultList)
            {
                stockTakeResult.IsAdjust = true;
                this.genericMgr.Update(stockTakeResult);
            }
            #endregion

            #region 尝试关闭订单
            TryCloseStockTakeMaster(stockTakeMaster);
            #endregion

            //过滤待验库存和不合格品库存差异,不通过盘点调整.
            var tobeAjustStockTakeResultList = notAjustStockTakeResultList.Where(p => p.DifferenceQty != 0
                && p.QualityType == CodeMaster.QualityType.Qualified).ToList();

            #region 更新库存
            if (effectiveDate.HasValue)
            {
                this.locationDetailMgr.StockTakeAdjust(stockTakeMaster, tobeAjustStockTakeResultList, effectiveDate.Value);
            }
            else
            {
                this.locationDetailMgr.StockTakeAdjust(stockTakeMaster, tobeAjustStockTakeResultList);
            }
            #endregion
        }
        #endregion

        #region 关闭盘点单
        [Transaction(TransactionMode.Requires)]
        public void ManualCloseStockTakeMaster(string stNo)
        {
            this.ManualCloseStockTakeMaster(this.genericMgr.FindById<StockTakeMaster>(stNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void ManualCloseStockTakeMaster(StockTakeMaster stockTakeMaster)
        {
            if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.Complete)
            {
                throw new BusinessException("盘点单{0}的状态为{1}，不能关闭。", stockTakeMaster.StNo,
                    this.systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }

            DoCloseStockTakeMaster(stockTakeMaster);
        }

        private void TryCloseStockTakeMaster(StockTakeMaster stockTakeMaster)
        {
            if (stockTakeMaster.Status == CodeMaster.StockTakeStatus.Complete)
            {
                this.genericMgr.FlushSession();
                long counter = this.genericMgr.FindAll<long>("select count(*) as counter from StockTakeResult where StNo = ? and IsAdjust = ? and QualityType=? ",
                    new Object[] { stockTakeMaster.StNo, false, (int)CodeMaster.QualityType.Qualified })[0];

                if (counter == 0)
                {
                    DoCloseStockTakeMaster(stockTakeMaster);
                }
            }
        }

        private void DoCloseStockTakeMaster(StockTakeMaster stockTakeMaster)
        {
            stockTakeMaster.CloseDate = DateTime.Now;
            stockTakeMaster.Status = CodeMaster.StockTakeStatus.Close;
            User user = SecurityContextHolder.Get();
            stockTakeMaster.CloseUserId = user.Id;
            stockTakeMaster.CloseUserName = user.FullName;

            this.genericMgr.Update(stockTakeMaster);
        }
        #endregion

        #region 添加盘点明细
        public void BatchUpdateStockTakeDetails(string stNo,
           IList<StockTakeDetail> addStockDetailList, IList<StockTakeDetail> updateStockDetailList, IList<StockTakeDetail> deleteStockDetailList)
        {
            BatchUpdateStockTakeDetails(this.genericMgr.FindById<StockTakeMaster>(stNo), addStockDetailList, updateStockDetailList, deleteStockDetailList);
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateStockTakeDetails(StockTakeMaster stockTakeMaster,
            IList<StockTakeDetail> addStockDetailList, IList<StockTakeDetail> updateStockDetailList, IList<StockTakeDetail> deleteStockDetailList)
        {
            if (stockTakeMaster.Status != CodeMaster.StockTakeStatus.InProcess)
            {
                throw new BusinessException("盘点单{0}的状态为{1}不能修改明细。",
                      stockTakeMaster.StNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.StockTakeStatus, ((int)stockTakeMaster.Status).ToString()));
            }
            if (stockTakeMaster.Type == CodeMaster.StockTakeType.Part)
            {
                var items = this.genericMgr.FindAll<string>(SelectStockTakeItemStatement, stockTakeMaster.StNo);
                if (addStockDetailList != null)
                {
                    foreach (StockTakeDetail stockTakeDetail in addStockDetailList)
                    {
                        if (!items.Contains(stockTakeDetail.Item))
                        {
                            throw new BusinessException("盘点单{0}的物料{1}不在抽盘明细中。",
                                  stockTakeMaster.StNo, stockTakeDetail.Item);
                        }
                    }
                }
                if (updateStockDetailList != null)
                {
                    foreach (StockTakeDetail stockTakeDetail in updateStockDetailList)
                    {
                        if (!items.Contains(stockTakeDetail.Item))
                        {
                            throw new BusinessException("盘点单{0}的物料{1}不在抽盘明细中。",
                                  stockTakeMaster.StNo, stockTakeDetail.Item);
                        }
                    }
                }
            }

            IList<string> stockTakeLocationList = this.genericMgr.FindAll<string>(SelectStockTakeLocationStatement, stockTakeMaster.StNo);
            if (addStockDetailList != null)
            {
                foreach (StockTakeDetail stockTakeDetail in addStockDetailList)
                {
                    if (!stockTakeLocationList.Contains(stockTakeDetail.Location))
                    {
                        throw new BusinessException("盘点单{0}的库位{1}不在抽盘明细中。", stockTakeMaster.StNo, stockTakeDetail.Location);
                    }
                }
            }
            if (updateStockDetailList != null)
            {
                foreach (StockTakeDetail stockTakeDetail in updateStockDetailList)
                {
                    if (!stockTakeLocationList.Contains(stockTakeDetail.Location))
                    {
                        throw new BusinessException("盘点单{0}的库位{1}不在抽盘明细中。", stockTakeMaster.StNo, stockTakeDetail.Location);
                    }
                }
            }
            #region 新增盘点明细
            if (addStockDetailList != null && addStockDetailList.Count > 0)
            {
                #region 数量处理
                foreach (StockTakeDetail stockTakeDetail in addStockDetailList)
                {
                    Item item = this.genericMgr.FindById<Item>(stockTakeDetail.Item);

                    stockTakeDetail.StNo = stockTakeMaster.StNo;
                    stockTakeDetail.ItemDescription = item.Description;
                    if (stockTakeDetail.Uom == null)
                    {
                        stockTakeDetail.Uom = item.Uom;
                    }
                    stockTakeDetail.BaseUom = item.Uom;
                    if (stockTakeDetail.Uom != stockTakeDetail.BaseUom)
                    {
                        stockTakeDetail.UnitQty = this.itemMgr.ConvertItemUomQty(stockTakeDetail.Item, stockTakeDetail.Uom, 1, stockTakeDetail.BaseUom);
                    }
                    else
                    {
                        stockTakeDetail.UnitQty = 1;
                    }

                    this.genericMgr.Create(stockTakeDetail);

                    if (stockTakeMaster.StockTakeDetails == null)
                    {
                        stockTakeMaster.StockTakeDetails = new List<StockTakeDetail>();
                    }
                    stockTakeMaster.StockTakeDetails.Add(stockTakeDetail);
                }
                #endregion
            }
            #endregion

            #region 修改盘点明细
            if (updateStockDetailList != null && updateStockDetailList.Count > 0)
            {
                foreach (StockTakeDetail stockTakeDetail in updateStockDetailList)
                {
                    if (stockTakeDetail.Uom != stockTakeDetail.BaseUom)
                    {
                        stockTakeDetail.UnitQty = this.itemMgr.ConvertItemUomQty(stockTakeDetail.Item, stockTakeDetail.BaseUom, 1, stockTakeDetail.Uom);
                    }
                    else
                    {
                        stockTakeDetail.UnitQty = 1;
                    }
                    this.genericMgr.Update(stockTakeDetail);
                }
            }
            #endregion

            #region 删除盘点明细
            if (deleteStockDetailList != null && deleteStockDetailList.Count > 0)
            {
                #region 数量处理
                foreach (StockTakeDetail stockTakeDetail in deleteStockDetailList)
                {
                    this.genericMgr.Delete(stockTakeDetail);
                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region 导入盘点明细
        [Transaction(TransactionMode.Requires)]
        public void ImportStockTakeDetailFromXls(Stream inputStream, string stNo)
        {
            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            #region 清空明细
            string hql = @"from StockTakeDetail as s where s.StNo = ?";
            genericMgr.Delete(hql, new object[] { stNo }, new IType[] { NHibernateUtil.String });
            genericMgr.FlushSession();
            genericMgr.CleanSession();
            #endregion

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            var cell = sheet.GetRow(2).Cells[4];
            var cellStNo = ImportHelper.GetCellStringValue(cell);
            if (cellStNo != stNo)
            {
                throw new BusinessException("导入的模板不正确,盘点单号不一致");
            }

            ImportHelper.JumpRows(rows, 11);

            #region 列定义
            int colItem = 1;//物料代码
            int colUom = 7;//单位
            int colLocation = 3;// 库位
            int colQty = 6;//数量
            int colHu = 8;//条码
            int colBin = 9;//库格
            #endregion

            DateTime dateTimeNow = DateTime.Now;
            BusinessException errorMessage = new BusinessException();
            IList<StockTakeDetail> stockTakeDetailList = new List<StockTakeDetail>();
            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 1, 6))
                {
                    break;//边界
                }

                if (true)
                {
                    string itemCode = string.Empty;
                    decimal qty = 0;
                    string uomCode = string.Empty;
                    string locationCode = string.Empty;

                    #region 读取数据
                    #region 读取物料代码
                    itemCode = ImportHelper.GetCellStringValue(row.GetCell(colItem));
                    if (itemCode == null || itemCode.Trim() == string.Empty)
                    {
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error,
                         "读取物料失败在{0}行{1}列", (row.RowNum + 1).ToString(), (colItem+1).ToString()));
                        continue;
                    }

                    #endregion
                    #region 读取单位
                    uomCode = ImportHelper.GetCellStringValue(row.GetCell(colUom));
                    if (uomCode == null || uomCode.Trim() == string.Empty)
                    {
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error,
                         "读取单位失败在{0}行{1}列", (row.RowNum + 1).ToString(), (colUom+1).ToString()));
                        continue;
                    }
                    #endregion
                    #region 读取单位和库存单位不一致则抛错
                    if (uomCode != itemMgr.GetCacheItem(itemCode).Uom)
                    {
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error,
                         "读取单位失败在{0}行{1}列,单位和库存单位不一致", (row.RowNum + 1).ToString(), (colUom+1).ToString()));
                        continue;
                    }
                    #endregion
                    #endregion

                    #region 读取库位
                    locationCode = ImportHelper.GetCellStringValue(row.GetCell(colLocation));
                    if (locationCode == null || locationCode.Trim() == string.Empty)
                    {
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error,
                         "读取库位失败在{0}行{1}列", (row.RowNum + 1).ToString(), (colLocation+1).ToString()));
                        continue;
                    }

                    #region 读取数量
                    try
                    {
                        qty = Convert.ToDecimal(ImportHelper.GetCellStringValue(row.GetCell(colQty)));
                    }
                    catch
                    {
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error,
                         "读取数量失败在{0}行{1}列", (row.RowNum + 1).ToString(), (colQty+1).ToString()));
                        continue;
                    }
                    #endregion

                    #endregion
                    #region 填充数据
                    Item item = null;
                    try
                    {
                        item = genericMgr.FindById<Item>(itemCode);
                    }
                    catch (Exception)
                    {
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, "物料号{0}不存在", itemCode));
                        continue;
                    }
                    try
                    {
                        Uom uom = genericMgr.FindById<Uom>(uomCode);
                    }
                    catch (Exception)
                    {
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, "单位{0}不存在", uomCode));
                        continue;
                    }
                    try
                    {
                        Location location = genericMgr.FindById<Location>(locationCode);
                    }
                    catch (Exception)
                    {
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, "库位{0}不存在", locationCode));
                        continue;
                    }
                    StockTakeDetail stockTakeDetail = new StockTakeDetail();
                    stockTakeDetail.StNo = stNo;
                    stockTakeDetail.Item = itemCode;
                    stockTakeDetail.Uom = uomCode;
                    stockTakeDetail.Location = locationCode;
                    stockTakeDetail.BaseUom = item.Uom;
                    stockTakeDetail.Qty = qty;
                    stockTakeDetailList.Add(stockTakeDetail);
                    #endregion
                }
                else
                {
                    string huId = string.Empty;
                    string binCode = string.Empty;
                    string locationCode = string.Empty;

                    #region 读取数据
                    #region 读取条码
                    huId = row.GetCell(colHu) != null ? row.GetCell(colHu).StringCellValue : string.Empty;
                    if (string.IsNullOrEmpty(huId))
                    {
                        throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colHu.ToString());
                    }
                    var i = (
                        from c in stockTakeDetailList
                        where c.HuId != null && c.HuId.Trim().ToUpper() == huId.Trim().ToUpper()
                        select c).Count();

                    if (i > 0)
                    {
                        throw new BusinessException("Import.Business.Error.Duplicate", huId, (row.RowNum + 1).ToString(), colHu.ToString());
                    }
                    #endregion

                    #region 读取库位
                    locationCode = row.GetCell(colLocation) != null ? row.GetCell(colLocation).StringCellValue : string.Empty;
                    if (locationCode == null || locationCode.Trim() == string.Empty)
                    {
                        throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colUom.ToString());
                    }

                    #region 读取库格
                    binCode = row.GetCell(colBin) != null ? row.GetCell(colBin).StringCellValue : null;
                    #endregion
                    #endregion

                    #endregion

                    #region 填充数据
                    Hu hu = genericMgr.FindById<Hu>(huId);

                    Location location = genericMgr.FindById<Location>(locationCode);

                    LocationBin bin = null;
                    if (binCode != null && binCode.Trim() != string.Empty)
                    {
                        bin = genericMgr.FindById<LocationBin>(binCode);
                    }

                    StockTakeDetail stockTakeDetail = new StockTakeDetail();
                    stockTakeDetail.StNo = stNo;
                    stockTakeDetail.Item = hu.Item;
                    stockTakeDetail.Qty = hu.Qty;
                    stockTakeDetail.Uom = hu.Uom;
                    stockTakeDetail.BaseUom = hu.BaseUom;
                    stockTakeDetail.HuId = hu.HuId;
                    stockTakeDetail.LotNo = hu.LotNo;
                    stockTakeDetail.Location = location.Code;
                    stockTakeDetail.Bin = bin.Code;
                    stockTakeDetailList.Add(stockTakeDetail);
                    #endregion
                }
            }
            if (errorMessage.HasMessage)
            {
                throw errorMessage;
            }
            if (stockTakeDetailList.Count == 0)
            {
                throw new BusinessException("Import.Result.Error.ImportNothing");
            }

            #region 校验物料,库位
            var stockTakeDetailGroup = stockTakeDetailList.GroupBy(p => new { p.Item, p.Location }, (k, g) => new { k, Count = g.Count() })
                .Where(p => p.Count > 1).Select(p => new { p.k.Item, p.k.Location });
            foreach (var stockTakeDetail in stockTakeDetailGroup)
            {
                errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error,
                    "存在重复的明细:库位{0}物料{1}", stockTakeDetail.Location, stockTakeDetail.Item));
            }
            #endregion
            if (errorMessage.HasMessage)
            {
                throw errorMessage;
            }

            BatchUpdateStockTakeDetails(stNo, stockTakeDetailList, null, null);
        }
        #endregion




    }
}

