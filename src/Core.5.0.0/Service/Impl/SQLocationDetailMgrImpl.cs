using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INV;
using com.Sconit.Utility;

namespace com.Sconit.Service
{
    public class SQLocationDetailMgrImpl : BaseMgr, ISQLocationDetailMgr
    {
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public IHuMgr huMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public IList<Hu> MatchNewHuForRepack(IList<OrderDetail> orderDetailList, Boolean isJIT, BusinessException businessException)
        {
            #region 变量
            // BusinessException businessException = new BusinessException();
            List<Hu> huList = new List<Hu>();
            List<InventoryOccupy> inventoryOccupyList = new List<InventoryOccupy>();
            decimal totalQty = 0m;
            string oldHus = string.Empty;
            #endregion

            #region 检查发货单是否有占用的条码，有的话需要用当前数量减去已匹配数量
            string hql = string.Empty;
            List<object> paras = new List<object>();
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                if (string.IsNullOrEmpty(hql))
                {
                    hql = "from HuMapping where IsEffective = 0 and OrderDetId in(?";
                }
                else
                {
                    hql += ", ?";
                }
                paras.Add(orderDetail.Id);
            }
            hql += ")";
            IList<HuMapping> huMappingList = this.genericMgr.FindAll<HuMapping>(hql, paras.ToArray());

            if (huMappingList.Count > 0)
            {
                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    orderDetail.OrderedQty = orderDetail.OrderedQty - huMappingList.Where(o => o.OrderDetId == orderDetail.Id).Sum(o => o.Qty);
                }
            }
            #endregion

            #region 翻箱
            orderDetailList = orderDetailList.Where(o => o.RemainShippedQty > 0).ToList();
            if (orderDetailList.Count == 0)
            {
                businessException.AddMessage("没有需要打印翻箱标签得要货明细。");
                return huList;
            }
            IList<string> itemList = orderDetailList.Select(o => o.Item).Distinct().ToList();

            foreach (var item in itemList)
            {
                #region 查看库存是否满足要货需求
                totalQty = 0m; oldHus = string.Empty;
                var groupOrderDetailList = orderDetailList.Where(o => o.Item == item).ToList();

                var locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetAotuPickInventory", new Object[] { groupOrderDetailList[0].LocationFrom, groupOrderDetailList[0].Item, groupOrderDetailList[0].QualityType, 0, false, true }).OrderBy(l => l.Qty).ThenBy(l => l.LotNo).ToList();
                var matchLocationLotDetailList = new List<LocationLotDetail>();
                if (locationLotDetailList.Count == 0)
                {
                    businessException.AddMessage("零件{0}在库位{1}中没有库存。", groupOrderDetailList[0].Item, groupOrderDetailList[0].LocationFrom);
                    continue;
                }
                if (locationLotDetailList.Where(l => l.OccupyType == CodeMaster.OccupyType.None).Sum(l => l.Qty) < groupOrderDetailList.Sum(g => g.RemainShippedQty))
                {
                    businessException.AddMessage("物料{0}的库存数量{1}不满足本次要货数{2}。", item, locationLotDetailList.Where(l => l.OccupyType == CodeMaster.OccupyType.None).Sum(l => l.Qty).ToString("0.######"), groupOrderDetailList.Sum(g => g.RemainShippedQty).ToString("0.######"));
                }
                #endregion

                #region JIT件每条明细一张条码
                if (isJIT == true)
                {
                    try
                    {
                        //如果有数量能直接匹配上零头箱的，那么就直接将这个匹配
                        var oddMatchedlocationLotDetailList = (from l in locationLotDetailList
                                                              from g in groupOrderDetailList
                                                              where g.Item == l.Item
                                                               && g.RemainShippedQty == l.Qty
                                                              select new LocationLotDetail
                                                              {
                                                                  Area = l.Area,
                                                                  BaseUom = l.BaseUom,
                                                                  Bin = l.Bin,
                                                                  BinSequence = l.BinSequence,
                                                                  ConsigementParty = l.ConsigementParty,
                                                                  CreateDate = l.CreateDate,
                                                                  CreateUserId = l.CreateUserId,
                                                                  CreateUserName = l.CreateUserName,
                                                                  FirstInventoryDate = l.FirstInventoryDate,
                                                                  HuId = l.HuId,
                                                                  HuQty = l.HuQty,
                                                                  HuUom = l.HuUom,
                                                                  Id = l.Id,
                                                                  IsATP = l.IsATP,
                                                                  IsConsignment = l.IsConsignment,
                                                                  IsFreeze = l.IsFreeze,
                                                                  IsOdd = l.IsOdd,
                                                                  Item = l.Item,
                                                                  ItemDescription = l.ItemDescription,
                                                                  LastModifyDate = l.LastModifyDate,
                                                                  LastModifyUserId = l.LastModifyUserId,
                                                                  LastModifyUserName = l.LastModifyUserName,
                                                                  Location = l.Location,
                                                                  LotNo = l.LotNo,
                                                                  ManufactureDate = l.ManufactureDate,
                                                                  ManufactureParty = l.ManufactureParty,
                                                                  OccupyReferenceNo = l.OccupyReferenceNo,
                                                                  OccupyType = l.OccupyType,
                                                                  PlanBill = l.PlanBill,
                                                                  Qty = l.Qty,
                                                                  QualityType = l.QualityType,
                                                                  ReferenceItemCode = l.ReferenceItemCode,
                                                                  UnitCount = l.UnitCount,
                                                                  UnitQty = l.UnitQty,
                                                                  Version = l.Version
                                                              });
                        if (oddMatchedlocationLotDetailList.Count() > 0)
                        {
                            var oddMatchedlocationLotDetail = oddMatchedlocationLotDetailList.FirstOrDefault();
                            var oddMatchedOrderDetail = groupOrderDetailList.FirstOrDefault(o => o.RemainShippedQty == oddMatchedlocationLotDetail.Qty && o.Item == oddMatchedlocationLotDetail.Item);
                            huList.AddRange(this.huMgr.CreateHu(oddMatchedOrderDetail, false, oddMatchedlocationLotDetail.ManufactureParty, oddMatchedlocationLotDetail.LotNo, oddMatchedlocationLotDetail.Qty, oddMatchedlocationLotDetail.Qty, oddMatchedlocationLotDetail.Qty, oddMatchedlocationLotDetail.HuId, oddMatchedOrderDetail.BinTo, true));
                            this.genericMgr.Update(oddMatchedlocationLotDetail);
                            locationLotDetailList.Remove(oddMatchedlocationLotDetail);
                            groupOrderDetailList.Remove(oddMatchedOrderDetail);
                        }

                        //将剩下的全部翻箱
                        var groupItemQty = groupOrderDetailList.Sum(g => g.RemainShippedQty);
                        foreach (var locationLotDetail in locationLotDetailList)
                        {
                            totalQty += locationLotDetail.Qty;
                            oldHus += locationLotDetail.HuId + ";";
                            InventoryOccupy inventoryOccupy = new InventoryOccupy();
                            inventoryOccupy.HuId = locationLotDetail.HuId;
                            inventoryOccupy.Location = locationLotDetail.Location;
                            inventoryOccupy.QualityType = locationLotDetail.QualityType;
                            inventoryOccupy.OccupyType = CodeMaster.OccupyType.Pick;
                            inventoryOccupy.OccupyReferenceNo = string.Empty;
                            inventoryOccupyList.Add(inventoryOccupy);
                            //locationLotDetail.OccupyType = CodeMaster.OccupyType.AutoPick;
                            //this.genericMgr.Update(locationLotDetail);
                            matchLocationLotDetailList.Add(locationLotDetail);

                            if (totalQty >= groupItemQty)
                            {
                                break;
                            }
                        }

                        foreach (var orderDetail in groupOrderDetailList)
                        {
                            if (totalQty - orderDetail.RemainShippedQty >= 0)
                            {
                                totalQty = totalQty - orderDetail.RemainShippedQty;
                                huList.AddRange(this.huMgr.CreateHu(orderDetail, true, matchLocationLotDetailList[0].ManufactureParty, LotNoHelper.GenerateLotNo(DateTime.Now), orderDetail.RemainShippedQty, orderDetail.RemainShippedQty, orderDetail.RemainShippedQty, oldHus, orderDetail.BinTo, true));
                            }
                            else
                            {
                                if (totalQty == 0m)
                                {
                                    break;
                                }
                                else
                                {
                                    huList.AddRange(this.huMgr.CreateHu(orderDetail, true, matchLocationLotDetailList[0].ManufactureParty, LotNoHelper.GenerateLotNo(DateTime.Now), totalQty, totalQty, totalQty, oldHus, orderDetail.BinTo, true));
                                    totalQty = 0m;
                                }
                            }

                        }
                        if (totalQty > 0m)
                        {
                            huList.AddRange(this.huMgr.CreateHu(groupOrderDetailList[0], true, matchLocationLotDetailList[0].ManufactureParty, LotNoHelper.GenerateLotNo(DateTime.Now), totalQty, totalQty, totalQty, oldHus, string.Empty, false));
                        }
                    }
                    catch (BusinessException be)
                    {
                        businessException.AddMessage(be.GetMessages()[0].GetMessageString());
                    }
                }
                #endregion
                #region 看板件需求必须是UC的整数倍，零头不管
                else
                {
                    foreach (var orderDetail in groupOrderDetailList)
                    {
                        if (locationLotDetailList.Where(l => l.IsOdd == false && l.Qty == orderDetail.UnitCount).Count() <= 0)
                        {
                            businessException.AddMessage("零件{0}的包装数不满足要货的包装数。", orderDetail.Item );
                        }
                        if (orderDetail.RemainShippedQty % orderDetail.UnitCount != 0)
                        {
                            businessException.AddMessage("要货单{0}中的看板件{1}的要货需求有零头数{2}，系统无法自动翻箱。", orderDetail.OrderNo, orderDetail.Item, (orderDetail.RemainShippedQty % orderDetail.UnitCount).ToString());
                        }

                        totalQty = 0m; oldHus = string.Empty;
                        for (int i = 0; i < orderDetail.RemainShippedQty / orderDetail.UnitCount; ++i)
                        {
                            if (locationLotDetailList.Where(l => l.IsOdd == false && l.Qty == orderDetail.UnitCount).Count() > 0)
                            {
                                var oneIntPack = locationLotDetailList.FirstOrDefault(l => l.IsOdd == false && l.Qty == orderDetail.UnitCount);
                                huList.AddRange(this.huMgr.CreateHu(orderDetail, false, oneIntPack.ManufactureParty, oneIntPack.LotNo, orderDetail.UnitCount, orderDetail.UnitCount, orderDetail.UnitCount, oneIntPack.HuId, orderDetail.BinTo, true));

                                InventoryOccupy inventoryOccupy = new InventoryOccupy();
                                inventoryOccupy.HuId = oneIntPack.HuId;
                                inventoryOccupy.Location = oneIntPack.Location;
                                inventoryOccupy.QualityType = oneIntPack.QualityType;
                                inventoryOccupy.OccupyType = CodeMaster.OccupyType.Pick;
                                inventoryOccupy.OccupyReferenceNo = string.Empty;
                                inventoryOccupyList.Add(inventoryOccupy);
                                locationLotDetailList.Remove(oneIntPack);
                                //oneIntPack.OccupyType = CodeMaster.OccupyType.AutoPick;
                                //this.genericMgr.Update(oneIntPack);
                            }
                        }
                    }
                }
                #endregion

                #region 库存占用
                try
                {
                    if (inventoryOccupyList.Count > 0)
                    {
                        this.locationDetailMgr.InventoryOccupy(inventoryOccupyList);
                    }
                }
                catch (BusinessException be)
                {
                    businessException.AddMessage(be.GetMessages()[0].GetMessageString());
                }

                #endregion
            }
            return huList;
            #endregion
        }


        [Transaction(TransactionMode.Requires)]
        public void DistributionLabelCancel(string HuId)
        {
            HuMapping hm = this.genericMgr.FindAll<HuMapping>("select h from HuMapping as h where HuId=?", HuId)[0];
            if (hm.IsEffective)
            {
                throw new BusinessException("条码{0}已经配送,不能取消。", HuId);
            }
            this.genericMgr.Delete(hm);
            string[] oldHuIdArray = hm.OldHus.Split(';');
            string hql = string.Empty;
            IList<object> parm = new List<object>();
            foreach (string oldHuId in oldHuIdArray)
            {
                if (!string.IsNullOrEmpty(oldHuId))
                {
                    if (string.IsNullOrEmpty(hql))
                    {
                        hql = "select l from LocationLotDetail as l where HuId in ( ?";
                    }
                    else
                    {
                        hql += ",?";
                    }
                    parm.Add(oldHuId);
                }
            }
            hql += ")";
            IList<LocationLotDetail> locLotDetList = this.genericMgr.FindAll<LocationLotDetail>(hql, parm.ToArray());
            foreach (LocationLotDetail item in locLotDetList)
            {
                item.OccupyType = 0;
                this.genericMgr.Update(item);
            }
        }
    }
}
