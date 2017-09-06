using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.INV;
using System.Collections;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using NHibernate;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.CUST;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class PickListMgrImpl : BaseMgr, IPickListMgr
    {
        #region 变量
        //private IPublishing proxy;
        //public IPubSubMgr pubSubMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public IIpMgr ipMgr { get; set; }
        #endregion

        #region public methods
        [Transaction(TransactionMode.Requires)]
        public PickListMaster CreatePickList(IList<OrderDetail> orderDetailList)
        {
            #region 判断是否全0收货
            if (orderDetailList == null || orderDetailList.Count == 0)
            {
                throw new BusinessException("订单明细不能为空。");
            }

            IList<OrderDetail> nonZeroOrderDetailList = orderDetailList.Where(o => o.PickQtyInput > 0).ToList();

            if (nonZeroOrderDetailList.Count == 0)
            {
                throw new BusinessException("订单明细不能为空。");
            }
            #endregion

            #region 查询订单头对象
            IList<OrderMaster> orderMasterList = LoadOrderMasters((from det in nonZeroOrderDetailList
                                                                   select det.OrderNo).Distinct().ToArray());
            #endregion

            #region 循环订单头检查
            IList<com.Sconit.CodeMaster.OrderType> orderTypeList = (from orderMaster in orderMasterList
                                                                    group orderMaster by orderMaster.Type into result
                                                                    select result.Key).ToList();

            if (orderTypeList.Count > 1)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_CannotMixOrderTypePick);
            }

            com.Sconit.CodeMaster.OrderType orderType = orderTypeList.Single();

            #region 判断是否超发
            foreach (OrderMaster orderMaster in orderMasterList)
            {
                orderMaster.OrderDetails = nonZeroOrderDetailList.Where(det => det.OrderNo == orderMaster.OrderNo).ToList();

                foreach (OrderDetail orderDetail in nonZeroOrderDetailList)
                {
                    if (!orderMaster.IsOpenOrder)
                    {
                        if (Math.Abs(orderDetail.ShippedQty + orderDetail.PickedQty) >= Math.Abs(orderDetail.OrderedQty))
                        {
                            //订单的发货数已经大于等于订单数
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipQtyExcceedOrderQty, orderDetail.OrderNo, orderDetail.Item);
                        }
                        else if (!orderMaster.IsShipExceed && Math.Abs(orderDetail.ShippedQty + orderDetail.PickedQty + orderDetail.PickQtyInput) > Math.Abs(orderDetail.OrderedQty))   //不允许过量发货
                        {
                            //订单的发货数 + 本次发货数大于订单数
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipQtyExcceedOrderQty, orderDetail.OrderNo, orderDetail.Item);
                        }
                    }
                }
            }
            #endregion
            #endregion

            #region 生成拣货单头
            PickListMaster pickListMaster = new PickListMaster();

            #region 订单质量类型
            //var flow = from om in orderMasterList select om.Flow;
            //if (flow.Distinct().Count() > 1)
            //{
            //    throw new BusinessException("路线代码不同不能合并拣货。");
            //}
            //pickListMaster.Flow = flow.Distinct().Single();
            pickListMaster.Flow = (orderMasterList.OrderBy(om => om.Flow).Select(om => om.Flow)).First();
            #endregion

            #region 订单类型
            pickListMaster.OrderType = orderType;
            #endregion

            #region 订单质量类型
            var qualityType = from om in orderMasterList select om.QualityType;
            if (qualityType.Distinct().Count() > 1)
            {
                throw new BusinessException("订单质量状态不同不能合并拣货。");
            }
            pickListMaster.QualityType = qualityType.Distinct().Single();
            #endregion

            #region 状态
            //pickListMaster.Status = com.Sconit.CodeMaster.PickListStatus.Create;
            pickListMaster.Status = com.Sconit.CodeMaster.PickListStatus.Submit;
            #endregion

            #region 发出时间
            pickListMaster.StartTime = (from om in orderMasterList select om.StartTime).Min();
            #endregion

            #region 到达时间
            pickListMaster.WindowTime = (from om in orderMasterList select om.WindowTime).Min();
            #endregion

            #region PartyFrom
            var partyFrom = from om in orderMasterList select om.PartyFrom;
            if (partyFrom.Distinct().Count() > 1)
            {
                throw new BusinessException("来源组织不同不能合并拣货。");
            }
            pickListMaster.PartyFrom = partyFrom.Distinct().Single();
            #endregion

            #region PartyFromName
            pickListMaster.PartyFromName = (from om in orderMasterList select om.PartyFromName).First();
            #endregion

            #region PartyTo
            var partyTo = from om in orderMasterList select om.PartyTo;
            if (partyTo.Distinct().Count() > 1)
            {
                throw new BusinessException("目的组织不同不能合并拣货。");
            }
            pickListMaster.PartyTo = partyTo.Distinct().Single();
            #endregion

            #region PartyToName
            pickListMaster.PartyToName = (from om in orderMasterList select om.PartyToName).First();
            #endregion

            #region ShipFrom
            var shipFrom = from om in orderMasterList select om.ShipFrom;
            if (shipFrom.Distinct().Count() > 1)
            {
                throw new BusinessException("发货地址不同不能合并拣货。");
            }
            pickListMaster.ShipFrom = shipFrom.Distinct().Single();
            #endregion

            #region ShipFromAddr
            pickListMaster.ShipFromAddress = (from om in orderMasterList select om.ShipFromAddress).First();
            #endregion

            #region ShipFromTel
            pickListMaster.ShipFromTel = (from om in orderMasterList select om.ShipFromTel).First();
            #endregion

            #region ShipFromCell
            pickListMaster.ShipFromCell = (from om in orderMasterList select om.ShipFromCell).First();
            #endregion

            #region ShipFromFax
            pickListMaster.ShipFromFax = (from om in orderMasterList select om.ShipFromFax).First();
            #endregion

            #region ShipFromContact
            pickListMaster.ShipFromContact = (from om in orderMasterList select om.ShipFromContact).First();
            #endregion

            #region ShipTo
            var shipTo = from om in orderMasterList select om.ShipTo;
            if (shipTo.Distinct().Count() > 1)
            {
                throw new BusinessException("收货地址不同不能合并拣货。");
            }
            pickListMaster.ShipTo = shipTo.Distinct().Single();
            #endregion

            #region ShipToAddr
            pickListMaster.ShipToAddress = (from om in orderMasterList select om.ShipToAddress).First();
            #endregion

            #region ShipToTel
            pickListMaster.ShipToTel = (from om in orderMasterList select om.ShipToTel).First();
            #endregion

            #region ShipToCell
            pickListMaster.ShipToCell = (from om in orderMasterList select om.ShipToCell).First();
            #endregion

            #region ShipToFax
            pickListMaster.ShipToFax = (from om in orderMasterList select om.ShipToFax).First();
            #endregion

            #region ShipToContact
            pickListMaster.ShipToContact = (from om in orderMasterList select om.ShipToContact).First();
            #endregion

            #region Dock
            var dock = from om in orderMasterList select om.Dock;
            if (dock.Distinct().Count() > 1)
            {
                throw new BusinessException("道口不同不能合并拣货。");
            }
            pickListMaster.Dock = dock.Distinct().Single();
            #endregion

            #region IsAutoReceive
            var isAutoReceive = from om in orderMasterList select om.IsAutoReceive;
            if (isAutoReceive.Distinct().Count() > 1)
            {
                throw new BusinessException("自动收货选项不同不能合并拣货。");
            }
            pickListMaster.IsAutoReceive = isAutoReceive.Distinct().Single();
            #endregion

            #region IsRecScanHu
            var isRecScanHu = from om in orderMasterList select om.IsReceiveScanHu;
            if (isRecScanHu.Distinct().Count() > 1)
            {
                throw new BusinessException("收货扫描条码选项不同不能合并拣货。");
            }
            pickListMaster.IsReceiveScanHu = isRecScanHu.Distinct().Single();
            #endregion

            #region IsPrintAsn
            pickListMaster.IsPrintAsn = orderMasterList.Where(om => om.IsPrintAsn == true) != null;
            #endregion

            #region IsPrintRec
            pickListMaster.IsPrintReceipt = orderMasterList.Where(om => om.IsPrintReceipt == true) != null;
            #endregion

            #region IsRecExceed
            var isRecExceed = from om in orderMasterList select om.IsReceiveExceed;
            if (isRecExceed.Distinct().Count() > 1)
            {
                throw new BusinessException("允许超收选项不同不能合并拣货。");
            }
            pickListMaster.IsReceiveExceed = isRecExceed.Distinct().Single();
            #endregion

            #region IsRecFulfillUC
            var isRecFulfillUC = from om in orderMasterList select om.IsReceiveFulfillUC;
            if (isRecFulfillUC.Distinct().Count() > 1)
            {
                throw new BusinessException("收货满足包装选项不同不能合并拣货。");
            }
            pickListMaster.IsReceiveFulfillUC = isRecFulfillUC.Distinct().Single();
            #endregion

            #region IsRecFifo
            var isRecFifo = from om in orderMasterList select om.IsReceiveFifo;
            if (isRecFifo.Distinct().Count() > 1)
            {
                throw new BusinessException("收货先进先出选项不同不能合并拣货。");
            }
            pickListMaster.IsReceiveFifo = isRecFifo.Distinct().Single();
            #endregion

            #region IsAsnUniqueRec
            var isAsnUniqueRec = from om in orderMasterList select om.IsAsnUniqueReceive;
            if (isAsnUniqueRec.Distinct().Count() > 1)
            {
                throw new BusinessException("ASN一次性收货选项不同不能合并拣货。");
            }
            pickListMaster.IsAsnUniqueReceive = isAsnUniqueRec.Distinct().Single();
            #endregion

            #region IsRecCreateHu
            //var createHuOption = from om in orderMasterList
            //                     where om.CreateHuOption == com.Sconit.CodeMaster.CreateHuOption.Receive
            //                     select om.CreateHuOption;
            //if (createHuOption != null && createHuOption.Count() > 0 && createHuOption.Count() != orderMasterList.Count())
            //{
            //    throw new BusinessException("收货创建条码选项不同不能合并拣货。");
            //}
            pickListMaster.CreateHuOption = CodeMaster.CreateHuOption.None;
            #endregion

            #region IsCheckPartyFromAuth
            pickListMaster.IsCheckPartyFromAuthority = orderMasterList.Where(om => om.IsCheckPartyFromAuthority == true).Count() > 0;
            #endregion

            #region IsCheckPartyToAuth
            pickListMaster.IsCheckPartyToAuthority = orderMasterList.Where(om => om.IsCheckPartyToAuthority == true).Count() > 0;
            #endregion

            #region RecGapTo
            IList<CodeMaster.ReceiveGapTo> recGapTo = (from om in orderMasterList select om.ReceiveGapTo).ToList();
            if (recGapTo.Distinct().Count() > 1)
            {
                throw new BusinessException("收货差异调整选项不同不能合并拣货。");
            }
            pickListMaster.ReceiveGapTo = recGapTo.Distinct().Single();
            #endregion

            #region AsnTemplate
            var asnTemplate = orderMasterList.Select(om => om.AsnTemplate).First();
            pickListMaster.AsnTemplate = asnTemplate;
            #endregion

            #region RecTemplate
            var recTemplate = orderMasterList.Select(om => om.ReceiptTemplate).First();
            pickListMaster.ReceiptTemplate = recTemplate;
            #endregion

            #region HuTemplate
            var huTemplate = orderMasterList.Select(om => om.HuTemplate).First();
            pickListMaster.HuTemplate = huTemplate;
            #endregion

            #region EffectiveDate
            pickListMaster.EffectiveDate = DateTime.Now;
            #endregion
            #endregion

            #region 生成拣货单明细
            #region 根据拣货的库位和策略分组待拣货明细
            var groupedOrderDetailList = from det in nonZeroOrderDetailList
                                         group det by new
                                         {
                                             LocationFrom = det.LocationFrom,
                                             LocationTo = det.LocationTo,
                                             PickStrategy = det.PickStrategy
                                         } into result
                                         select new
                                         {
                                             LocationFrom = result.Key.LocationFrom,
                                             LocationTo = result.Key.LocationTo,
                                             PickStrategy = result.Key.PickStrategy,
                                             List = result.OrderByDescending(o => o.ManufactureParty).ThenBy(o => o.Item).ThenBy(p => p.UnitCount).ToList()
                                         };
            #endregion

            IList<PickLocationDetail> pickLocationLotDetailList = new List<PickLocationDetail>();
            foreach (var groupedOrderDetail in groupedOrderDetailList)
            {
                #region 查找拣货策略
                string pickStrategyCode = groupedOrderDetail.PickStrategy;
                if (string.IsNullOrWhiteSpace(pickStrategyCode))
                {
                    //如果没有拣货策略，从企业选项中取
                    pickStrategyCode = this.systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultPickStrategy);

                    if (string.IsNullOrWhiteSpace(pickStrategyCode))
                    {
                        throw new BusinessException("没有找到拣货策略，请在订单或企业选项中维护拣货策略。");
                    }
                }

                PickStrategy pickStrategy = this.genericMgr.FindById<PickStrategy>(pickStrategyCode);
                #endregion

                if (pickStrategy.IsSimple)
                {
                    #region 简单模式
                    int seqS = 1;
                    foreach (OrderDetail orderDetail in groupedOrderDetail.List)
                    {
                        PickListDetail pickListDetail = new PickListDetail();
                        pickListDetail.Sequence = seqS++;
                        pickListDetail.OrderNo = orderDetail.OrderNo;
                        pickListDetail.OrderType = orderDetail.OrderType;
                        pickListDetail.OrderSubType = orderDetail.OrderSubType;
                        pickListDetail.OrderDetailId = orderDetail.Id;
                        pickListDetail.OrderDetailSequence = orderDetail.Sequence;
                        pickListDetail.StartTime = orderDetail.StartDate.HasValue ? orderDetail.StartDate : orderMasterList.Where(mstr => mstr.OrderNo == orderDetail.OrderNo).Single().StartTime;
                        pickListDetail.WindowTime = orderDetail.EndDate.HasValue ? orderDetail.EndDate : orderMasterList.Where(mstr => mstr.OrderNo == orderDetail.OrderNo).Single().WindowTime;
                        pickListDetail.Item = orderDetail.Item;
                        pickListDetail.ItemDescription = orderDetail.ItemDescription;
                        pickListDetail.ReferenceItemCode = orderDetail.ReferenceItemCode;
                        pickListDetail.Uom = orderDetail.Uom;
                        pickListDetail.BaseUom = orderDetail.BaseUom;
                        pickListDetail.UnitQty = orderDetail.UnitQty;
                        pickListDetail.UnitCount = orderDetail.UnitCount;
                        pickListDetail.QualityType = orderDetail.QualityType;
                        pickListDetail.ManufactureParty = orderDetail.ManufactureParty;
                        pickListDetail.LocationFrom = orderDetail.LocationFrom;
                        pickListDetail.LocationFromName = orderDetail.LocationFromName;
                        //pickListDetail.Area = g.Key.Area;
                        //pickListDetail.Bin = g.Key.Bin;
                        pickListDetail.LocationTo = orderDetail.LocationTo;
                        pickListDetail.LocationToName = orderDetail.LocationToName;
                        pickListDetail.Qty = orderDetail.PickQtyInput;
                        pickListDetail.PickedQty = 0;
                        //pickListDetail.LotNo = g.Key.LotNo;
                        pickListDetail.IsInspect = orderDetail.IsInspect && orderMasterList.Where(o => o.OrderNo == orderDetail.OrderNo).Single().IsInspect;
                        //pickListDetail.PickStrategy = g.Key.PickStrategy;
                        pickListDetail.IsClose = false;
                        //pickListDetail.IsOdd = g.Key.IsOdd;
                        //pickListDetail.IsDevan = g.Key.IsDevan;
                        pickListDetail.IsInventory = true;

                        pickListMaster.AddPickListDetail(pickListDetail);
                    }
                    #endregion
                }
                else
                {
                    var distinctItemCodes = groupedOrderDetail.List.Select(p => p.Item).Distinct();
                    #region 查找库存
                    #region 拼SQL
                    string selectLocationLotDetailStatement = string.Empty;
                    IList<object> selectLocationLotDetailPara = new List<object>();
                    foreach (var itemCode in distinctItemCodes)
                    {
                        if (selectLocationLotDetailStatement == string.Empty)
                        {
                            selectLocationLotDetailStatement =
                                @"select l.Item, l.UC, l.HuUom, l.ManufactureParty, l.LotNo, l.Bin, l.HuQty, l.IsOdd, l.Area,l.Direction,l.BinSeq
                                from VIEW_LocationLotDet l join INV_Hu h on l.HuId = h.HuId join MD_Item i on i.Code = l.Item
                                where l.HuId is not null and (i.ItemOption!=1 or h.HuOption =2 or h.HuOption=0 or i.Location is null )
                                and l.Location = ? and l.QualityType = ? 
                                and l.OccupyType = ? and l.IsATP = ? and l.IsFreeze = ?";
                            //ItemOption.NeedAging = 1,  HuOption.Aged = 2,或者目的库位一致
                            selectLocationLotDetailPara.Add(groupedOrderDetail.LocationFrom);
                            selectLocationLotDetailPara.Add(pickListMaster.QualityType);
                            selectLocationLotDetailPara.Add(CodeMaster.OccupyType.None);
                            selectLocationLotDetailPara.Add(true);
                            selectLocationLotDetailPara.Add(false);

                            if (pickStrategy.IsPickFromBin)
                            {
                                selectLocationLotDetailStatement += " and l.Bin is not null";
                            }
                            selectLocationLotDetailStatement += " and l.Item in (?";
                        }
                        else
                        {
                            selectLocationLotDetailStatement += ", ?";
                        }
                        selectLocationLotDetailPara.Add(itemCode);
                    }

                    selectLocationLotDetailStatement += ") order by ";
                    if (pickStrategy.ShipStrategy == CodeMaster.ShipStrategy.LIFO)
                    {
                        selectLocationLotDetailStatement += "l.ManufactureDate Desc";
                    }
                    else
                    {
                        selectLocationLotDetailStatement += "l.ManufactureDate Asc";
                    }
                    selectLocationLotDetailStatement += ",l.HuQty Asc,case when l.BinSeq is null then 1 else 0 end Asc,l.BinSeq Asc,l.Bin Asc ";
                    #endregion
                    var sumLocationLotDetailList =
                        (from locDet in this.genericMgr.FindAllWithNativeSql<object[]>
                             (selectLocationLotDetailStatement, selectLocationLotDetailPara.ToArray())
                         select new PickLocationDetail
                         {
                             Item = (string)locDet[0],
                             UnitCount = (decimal)locDet[1],
                             Uom = (string)locDet[2],
                             ManufactureParty = (string.IsNullOrWhiteSpace((string)locDet[3]) ? (string)null : (string)locDet[3]),
                             LotNo = (string)locDet[4],
                             Bin = (string)locDet[5],
                             Qty = (decimal)locDet[6],
                             IsOdd = (bool)locDet[7],
                             Area = (string)locDet[8],
                             Direction = string.IsNullOrWhiteSpace((string)locDet[9]) ? (string)null : (string)locDet[9],
                             BinSeq = locDet[10] == null ? int.MaxValue : (int)(locDet[10])
                         }).ToList();
                    #endregion

                    #region 查找未关闭的拣货单明细
                    var unPickListDetailList = this.genericMgr.FindAllIn<PickListDetail>
                         (@" from PickListDetail where LocationFrom = ? and QualityType = ? and IsInventory = ? and IsClose = ? 
                            and Item in (? ", distinctItemCodes,
                         new object[] { groupedOrderDetail.LocationFrom, pickListMaster.QualityType, true, false }
                         );
                    #endregion

                    #region 过滤已经被拣货单占用的库存
                    if (unPickListDetailList != null && unPickListDetailList.Count > 0)
                    {
                        foreach (var unPickListDetail in unPickListDetailList)
                        {
                            decimal unPickedQty = (unPickListDetail.Qty - unPickListDetail.PickedQty);// *unPickListDetail.UnitQty;
                            if (unPickedQty == 0)
                            {
                                continue;
                            }
                            decimal ucDeviation = (decimal)pickStrategy.UcDeviation / 100;
                            var machedLocationLotDetailList = (sumLocationLotDetailList.Where(l =>
                                l.Item == unPickListDetail.Item   //Item
                                    //&& l.UnitCount == unPickListDetail.UnitCount      //UC
                                && (!pickStrategy.IsFulfillUC || l.Qty == unPickListDetail.UnitCount)  //匹配包装
                                && l.Uom == unPickListDetail.Uom     //Uom
                                    //&& string.Compare(l.ManufactureParty, unPickListDetail.ManufactureParty) == 0  //ManufactureParty
                                && l.LotNo == unPickListDetail.LotNo       //LotNo
                                && l.Bin == unPickListDetail.Bin       //Bin
                                && l.Qty > 0
                                && l.IsOdd == unPickListDetail.IsOdd      //IsOdd
                                && (!unPickListDetail.IsMatchDirection || string.IsNullOrWhiteSpace(unPickListDetail.Direction) || l.Direction == unPickListDetail.Direction)//Direction
                                )).ToList();

                            PickMatch(ref unPickedQty, unPickListDetail.UnitCount, pickStrategy, machedLocationLotDetailList);
                        }
                    }
                    #endregion

                    #region 循环匹配拣货项
                    foreach (OrderDetail orderDetail in groupedOrderDetail.List)  //把指定供应商的待拣货项放前面先匹配
                    {
                        decimal ucDeviation = (decimal)pickStrategy.UcDeviation / 100;
                        decimal pickQty = orderDetail.OrderDetailInputs[0].PickQty;
                        #region 按匹配的选项过滤
                        var matchedLocationLotDetailList =
                            sumLocationLotDetailList.Where(l => l.Qty > 0
                            && l.Item == orderDetail.Item
                            && l.Uom == orderDetail.Uom
                                //&& (string.IsNullOrWhiteSpace(orderDetail.ManufactureParty) || l.ManufactureParty == orderDetail.ManufactureParty)  //指定供应商
                            && (!pickStrategy.IsFulfillUC || l.Qty == orderDetail.UnitCount)  //匹配包装
                            && (!l.IsOdd || pickStrategy.OddOption == CodeMaster.PickOddOption.OddFirst)   //零头先发
                            && (!pickStrategy.IsMatchDirection || string.IsNullOrWhiteSpace(orderDetail.Direction) || l.Direction == orderDetail.Direction)
                            ).ToList();
                        #endregion

                        var newPickLocationDetailList = PickMatch(ref pickQty, orderDetail.UnitCount, pickStrategy, matchedLocationLotDetailList);
                        foreach (var newPickLocationDetail in newPickLocationDetailList)
                        {
                            newPickLocationDetail.IsMatchDirection = pickStrategy.IsMatchDirection;
                            newPickLocationDetail.Direction = orderDetail.Direction;
                            newPickLocationDetail.OrderDetail = orderDetail;
                            newPickLocationDetail.UcDeviation = (decimal)pickStrategy.UcDeviation;
                            pickLocationLotDetailList.Add(newPickLocationDetail);
                        }

                        #region 未满足的发货数
                        if (pickQty > 0)
                        {
                            PickLocationDetail pickLocationDetail = new PickLocationDetail();
                            pickLocationDetail.OrderDetail = orderDetail;
                            pickLocationDetail.PickStrategy = pickStrategy.Code;
                            pickLocationDetail.Item = orderDetail.Item;
                            pickLocationDetail.UnitCount = orderDetail.UnitCount;
                            pickLocationDetail.Uom = orderDetail.Uom;
                            pickLocationDetail.ManufactureParty = orderDetail.ManufactureParty;
                            pickLocationDetail.Qty = pickQty;
                            pickLocationDetail.IsOdd = false;
                            pickLocationDetail.OrderDetail = orderDetail;
                            pickLocationDetail.IsInventory = false;
                            pickLocationDetail.Direction = orderDetail.Direction;
                            pickLocationDetail.IsMatchDirection = pickStrategy.IsMatchDirection;
                            pickLocationDetail.BinSeq = int.MaxValue;
                            pickLocationLotDetailList.Add(pickLocationDetail);
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            #endregion

            #region 创建拣货单头
            pickListMaster.PickListNo = this.numberControlMgr.GetPickListNo(pickListMaster);
            this.genericMgr.Create(pickListMaster);
            #endregion

            #region 创建拣货明细
            IList<PickListDetail> pickListDetailList = (from det in pickLocationLotDetailList
                                                        group det by new
                                                        {
                                                            OrderDetail = det.OrderDetail,
                                                            PickStrategy = det.PickStrategy,
                                                            LotNo = det.LotNo,
                                                            UnitCount = det.UnitCount,
                                                            //ManufactureParty = det.ManufactureParty,
                                                            Area = det.Area,
                                                            Bin = det.Bin,
                                                            //BinSeq = det.BinSeq,
                                                            IsOdd = det.IsOdd,
                                                            IsDevan = det.IsDevan,
                                                            IsInventory = det.IsInventory,
                                                            IsMatchDirection = det.IsMatchDirection,
                                                            Direction = det.Direction,
                                                            UcDeviation = det.UcDeviation
                                                        } into g
                                                        select new PickListDetail
                                                        {
                                                            //PickListNo = pickListMaster.PickListNo,
                                                            OrderNo = g.Key.OrderDetail.OrderNo,
                                                            OrderType = g.Key.OrderDetail.OrderType,
                                                            OrderSubType = g.Key.OrderDetail.OrderSubType,
                                                            OrderDetailId = g.Key.OrderDetail.Id,
                                                            OrderDetailSequence = g.Key.OrderDetail.Sequence,
                                                            StartTime = g.Key.OrderDetail.StartDate.HasValue ? g.Key.OrderDetail.StartDate : orderMasterList.Where(mstr => mstr.OrderNo == g.Key.OrderDetail.OrderNo).Single().StartTime,
                                                            WindowTime = g.Key.OrderDetail.EndDate.HasValue ? g.Key.OrderDetail.EndDate : orderMasterList.Where(mstr => mstr.OrderNo == g.Key.OrderDetail.OrderNo).Single().WindowTime,
                                                            Item = g.Key.OrderDetail.Item,
                                                            ItemDescription = g.Key.OrderDetail.ItemDescription,
                                                            ReferenceItemCode = g.Key.OrderDetail.ReferenceItemCode,
                                                            Uom = g.Key.OrderDetail.Uom,
                                                            BaseUom = g.Key.OrderDetail.BaseUom,
                                                            UnitQty = g.Key.OrderDetail.UnitQty,
                                                            UnitCount = g.Key.OrderDetail.UnitCount,
                                                            QualityType = g.Key.OrderDetail.QualityType,
                                                            //ManufactureParty = g.Key.ManufactureParty,
                                                            LocationFrom = g.Key.OrderDetail.LocationFrom,
                                                            LocationFromName = g.Key.OrderDetail.LocationFromName,
                                                            Area = g.Key.Area,
                                                            Bin = g.Key.Bin,
                                                            BinSeq = g.First().BinSeq,
                                                            LocationTo = g.Key.OrderDetail.LocationTo,
                                                            LocationToName = g.Key.OrderDetail.LocationToName,
                                                            Qty = g.Sum(det => det.Qty),
                                                            PickedQty = 0,
                                                            LotNo = g.Key.LotNo,
                                                            IsInspect = g.Key.OrderDetail.IsInspect && orderMasterList.Where(o => o.OrderNo == g.Key.OrderDetail.OrderNo).Single().IsInspect,
                                                            PickStrategy = g.Key.PickStrategy,
                                                            IsClose = false,
                                                            IsOdd = g.Key.IsOdd,
                                                            IsDevan = g.Key.IsDevan,
                                                            IsInventory = g.Key.IsInventory,
                                                            IsMatchDirection = g.Key.IsMatchDirection,
                                                            Direction = g.Key.Direction,
                                                            UcDeviation = g.Key.UcDeviation
                                                        }).ToList();
            pickListDetailList = pickListDetailList
                .OrderBy(d => d.BinSeq)//库格序号排序
                .ThenBy(d => d.Bin)//库格排序
                .ThenBy(p => p.Item)//然后物料排序
                .ThenByDescending(d => d.IsInventory)//最后按是否有库存排序
                .ToList();

            int seq = 1;
            foreach (PickListDetail pickListDetail in pickListDetailList)
            {
                pickListDetail.Sequence = seq++;
                pickListDetail.PickListNo = pickListMaster.PickListNo;
                this.genericMgr.Create(pickListDetail);
                pickListMaster.AddPickListDetail(pickListDetail);
            }

            #endregion

            #region 更新订单明细拣货数
            foreach (var det in (from det in pickListMaster.PickListDetails
                                 group det by new
                                 {
                                     OrderDetailId = det.OrderDetailId,
                                 } into g
                                 select new
                                 {
                                     OrderDetailId = g.Key.OrderDetailId,
                                     Qty = g.Sum(det => det.Qty),
                                 }).ToList())
            {
                OrderDetail orderDetail = orderDetailList.Where(orderDet => orderDet.Id == det.OrderDetailId).Single();
                orderDetail.PickedQty += det.Qty;

                this.genericMgr.Update(orderDetail);
            }
            #endregion

            this.AsyncSendPrintData(pickListMaster);
            return pickListMaster;
        }

        private IList<PickLocationDetail> PickMatch(ref decimal pickQty, decimal unitCount,
            PickStrategy pickStrategy, IList<PickLocationDetail> matchedLocationLotDetailList)
        {
            IList<PickLocationDetail> newPickLocationDetailList = new List<PickLocationDetail>();
            #region 零头不占用发货数
            if (!pickStrategy.IsOddOccupy)
            {
                //零头全部发走
                var oddLocationLotDetails = matchedLocationLotDetailList.Where(l => l.IsOdd && l.UnitCount < unitCount);
                foreach (PickLocationDetail matchedLocationLotDetail in oddLocationLotDetails)
                {
                    PickLocationDetail pickLocationDetail = Mapper.Map<PickLocationDetail, PickLocationDetail>(matchedLocationLotDetail);
                    pickLocationDetail.PickStrategy = pickStrategy.Code;
                    pickLocationDetail.IsInventory = true;
                    newPickLocationDetailList.Add(pickLocationDetail);
                    matchedLocationLotDetail.Qty = 0; //库存明细数量置零
                }
            }
            #endregion

            //优先匹配不要拆箱的.
            var qty = pickQty;
            var matchUcDetail = matchedLocationLotDetailList.Where(l => l.Qty > 0)
                .Where(p => qty == p.Qty).FirstOrDefault();
            if (matchUcDetail != null && matchedLocationLotDetailList.Where(l => l.Qty > 0).First().LotNo == matchUcDetail.LotNo)
            {
                PickLocationDetail pickLocationDetail = Mapper.Map<PickLocationDetail, PickLocationDetail>(matchUcDetail);
                pickLocationDetail.Qty = pickQty;
                pickLocationDetail.PickStrategy = pickStrategy.Code;
                pickLocationDetail.IsInventory = true;
                matchUcDetail.Qty = 0;
                pickQty = 0;
                newPickLocationDetailList.Add(pickLocationDetail);
            }
            else
            {
                #region 循环拣货
                var matchDetails = matchedLocationLotDetailList.Where(l => l.Qty > 0).ToList();
                foreach (PickLocationDetail matchedLocationLotDetail in matchDetails)
                {
                    if (pickQty <= 0)
                    {
                        break;
                    }
                    PickLocationDetail pickLocationDetail = Mapper.Map<PickLocationDetail, PickLocationDetail>(matchedLocationLotDetail);
                    pickLocationDetail.PickStrategy = pickStrategy.Code;
                    pickLocationDetail.IsInventory = true;

                    if (pickQty >= matchedLocationLotDetail.Qty)
                    {
                        pickLocationDetail.Qty = matchedLocationLotDetail.Qty;
                        pickQty -= matchedLocationLotDetail.Qty;
                        matchedLocationLotDetail.Qty = 0; //库存明细数量置零
                    }
                    else
                    {
                        var deviation = unitCount * ((decimal)pickStrategy.UcDeviation / 100);
                        if (pickQty < deviation)
                        {
                            pickQty = 0;
                            break;
                            //小于容差值,可以不拣
                        }
                        else if (pickStrategy.IsDevan)
                        {
                            #region 允许拆箱
                            pickLocationDetail.Qty = pickQty;
                            pickLocationDetail.IsDevan = true;
                            matchedLocationLotDetail.Qty -= pickQty;
                            pickQty = 0;
                            #endregion
                        }
                        else
                        {
                            #region 不允许拆箱,全部发走
                            pickLocationDetail.Qty = matchedLocationLotDetail.Qty;
                            matchedLocationLotDetail.Qty = 0; //库存明细数量置零
                            pickQty = 0;
                            #endregion
                        }
                    }
                    newPickLocationDetailList.Add(pickLocationDetail);
                }
                #endregion
            }
            return newPickLocationDetailList;
        }

        //[Transaction(TransactionMode.Requires)]
        //public void DeletePickList(string pickListNo)
        //{
        //    PickListMaster pickListMaster = genericMgr.FindById<PickListMaster>(pickListNo);
        //    DeletePickList(pickListMaster);
        //}

        //[Transaction(TransactionMode.Requires)]
        //public void DeletePickList(PickListMaster pickListMaster)
        //{
        //    if (pickListMaster.Status != CodeMaster.PickListStatus.Create)
        //    {
        //        throw new BusinessException("不能删除状态为{1}的拣货单{0}。",
        //            pickListMaster.PickListNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.PickListStatus, pickListMaster.Status.ToString()));
        //    }

        //    string hql = "from PickListDetail where PickListNo = ?";
        //    this.genericMgr.Delete(hql, pickListMaster.PickListNo, NHibernateUtil.String);

        //    hql = "from PickListMaster where PickListNo = ?";
        //    this.genericMgr.Delete(hql, pickListMaster.PickListNo, NHibernateUtil.String);
        //}

        //[Transaction(TransactionMode.Requires)]
        //public void ReleasePickList(string pickListNo)
        //{
        //    PickListMaster pickListMaster = genericMgr.FindById<PickListMaster>(pickListNo);
        //    this.ReleasePickList(pickListMaster);
        //}

        //[Transaction(TransactionMode.Requires)]
        //public void ReleasePickList(PickListMaster pickListMaster)
        //{
        //    if (pickListMaster.Status == com.Sconit.CodeMaster.PickListStatus.Create)
        //    {
        //        //验证OrderDetail不能为空
        //        TryLoadPickListDetails(pickListMaster);
        //        if (pickListMaster.PickListDetails == null || pickListMaster.PickListDetails.Count == 0)
        //        {
        //            throw new BusinessException("拣货列表不能为空。");
        //        }
        //    }
        //    else
        //    {
        //        throw new BusinessException("不能释放状态为{1}的拣货单{0}。",
        //           pickListMaster.PickListNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.PickListStatus, pickListMaster.Status.ToString()));
        //    }

        //    pickListMaster.Status = CodeMaster.PickListStatus.Submit;
        //    pickListMaster.ReleaseDate = DateTime.Now;
        //    pickListMaster.ReleaseUserId = SecurityContextHolder.Get().Id;
        //    pickListMaster.ReleaseUserName = SecurityContextHolder.Get().FullName;

        //    this.genericMgr.Update(pickListMaster);
        //}

        [Transaction(TransactionMode.Requires)]
        public void CancelPickList(string pickListNo)
        {
            PickListMaster pickListMaster = this.genericMgr.FindById<PickListMaster>(pickListNo);
            CancelPickList(pickListMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelPickList(PickListMaster pickListMaster)
        {
            if (pickListMaster.Status != com.Sconit.CodeMaster.PickListStatus.Submit
                && pickListMaster.Status != com.Sconit.CodeMaster.PickListStatus.InProcess)
            {
                throw new BusinessException("不能取消状态为{1}的拣货单{0}。",
                   pickListMaster.PickListNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.PickListStatus, ((int)pickListMaster.Status).ToString()));
            }

            VoidPickListDetail(pickListMaster);

            #region 更新拣货单头
            pickListMaster.Status = CodeMaster.PickListStatus.Cancel;
            pickListMaster.CancelDate = DateTime.Now;
            pickListMaster.CancelUserId = SecurityContextHolder.Get().Id;
            pickListMaster.CancelUserName = SecurityContextHolder.Get().FullName;

            this.genericMgr.Update(pickListMaster);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void StartPickList(string pickListNo)
        {
            PickListMaster pickListMaster = genericMgr.FindById<PickListMaster>(pickListNo);
            this.StartPickList(pickListMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartPickList(PickListMaster pickListMaster)
        {
            if (pickListMaster.Status != com.Sconit.CodeMaster.PickListStatus.Submit)
            {
                throw new BusinessException("不能开始状态为{1}的拣货单{0}。",
                   pickListMaster.PickListNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.PickListStatus, ((int)pickListMaster.Status).ToString()));
            }

            pickListMaster.Status = CodeMaster.PickListStatus.InProcess;
            pickListMaster.StartDate = DateTime.Now;
            pickListMaster.StartUserId = SecurityContextHolder.Get().Id;
            pickListMaster.StartUserName = SecurityContextHolder.Get().FullName;

            this.genericMgr.Update(pickListMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void DoPick(IList<PickListDetail> pickListDetailList)
        {
            if (pickListDetailList.Select(det => det.PickListNo).Distinct().Count() > 1)
            {
                throw new TechnicalException("不能跨拣货单拣货。");
            }

            string pickListNo = pickListDetailList.Select(det => det.PickListNo).Distinct().Single();

            #region 判断是否全0拣货
            if (pickListDetailList == null || pickListDetailList.Count == 0)
            {
                throw new BusinessException("拣货明细不能为空。");
            }

            IList<PickListDetail> nonZeroPickListDetailList = pickListDetailList.Where(o => o.PickListDetailInputs != null && o.PickListDetailInputs.Count > 0).ToList();

            if (nonZeroPickListDetailList.Count == 0)
            {
                throw new BusinessException("拣货明细不能为空。");
            }
            #endregion

            #region 库存占用
            IList<InventoryOccupy> inventoryOccupyList = new List<InventoryOccupy>();
            foreach (PickListDetail pickListDetail in nonZeroPickListDetailList)
            {
                foreach (PickListDetailInput pickListDetailInput in pickListDetail.PickListDetailInputs)
                {
                    InventoryOccupy inventoryOccupy = new InventoryOccupy();
                    inventoryOccupy.HuId = pickListDetailInput.HuId;
                    inventoryOccupy.Location = pickListDetail.LocationFrom;
                    inventoryOccupy.QualityType = CodeMaster.QualityType.Qualified;
                    inventoryOccupy.OccupyType = CodeMaster.OccupyType.Pick;
                    inventoryOccupy.OccupyReferenceNo = pickListDetail.PickListNo;
                    inventoryOccupyList.Add(inventoryOccupy);
                }
            }

            IList<LocationLotDetail> locationLotDetailList = this.locationDetailMgr.InventoryOccupy(inventoryOccupyList);
            #endregion

            #region 检查是否超拣
            BusinessException businessException = new BusinessException();
            foreach (PickListDetail pickListDetail in nonZeroPickListDetailList)
            {
                decimal pickedQty = locationLotDetailList.Where(l => pickListDetail.GetPickedHuList().Contains(l.HuId)).Sum(l => l.HuQty);
                pickListDetail.PickedQty += pickedQty;
                if (pickListDetail.UcDeviation >= 0)
                {
                    decimal ucDeviation = (pickListDetail.UcDeviation / 100) * pickListDetail.UnitCount;
                    if (pickListDetail.Qty + ucDeviation < pickListDetail.PickedQty)
                    {
                        businessException.AddMessage("拣货行{0}的拣货数已经超过待拣数。", pickListDetail.Sequence.ToString());
                    }
                }
            }
            #endregion

            if (businessException.HasMessage)
            {
                throw businessException;
            }

            #region 更新拣货单头
            PickListMaster pickListMaster = this.genericMgr.FindById<PickListMaster>(pickListNo);
            if (pickListMaster.Status == com.Sconit.CodeMaster.PickListStatus.Submit)
            {
                pickListMaster.Status = CodeMaster.PickListStatus.InProcess;
                pickListMaster.StartDate = DateTime.Now;
                pickListMaster.StartUserId = SecurityContextHolder.Get().Id;
                pickListMaster.StartUserName = SecurityContextHolder.Get().FullName;
            }
            else if (pickListMaster.Status == com.Sconit.CodeMaster.PickListStatus.InProcess)
            {
                //nothing todo
            }
            else
            {
                throw new BusinessException("不能对状态为{1}的拣货单{0}拣货。",
                pickListMaster.PickListNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.PickListStatus, ((int)pickListMaster.Status).ToString()));
            }
            pickListMaster.CompleteDate = DateTime.Now;
            pickListMaster.CompleteUserId = SecurityContextHolder.Get().Id;
            pickListMaster.CompleteUserName = SecurityContextHolder.Get().FullName;
            #endregion

            var pickListResultDic = this.genericMgr.FindAll<PickListResult>
                (" from PickListResult where PickListNo=? ", pickListNo).ToDictionary(d => d.HuId, d => d);

            #region 更新拣货明细
            foreach (PickListDetail pickListDetail in nonZeroPickListDetailList)
            {
                this.genericMgr.Update(pickListDetail);

                foreach (PickListDetailInput pickListDetailInput in pickListDetail.PickListDetailInputs)
                {
                    if (pickListResultDic.ContainsKey(pickListDetailInput.HuId))
                    {
                        continue;
                    }
                    LocationLotDetail huLocationLotDetail = locationLotDetailList.Where(l => l.HuId == pickListDetailInput.HuId).Single();
                    PickListResult pickListResult = new PickListResult();
                    pickListResult.PickListNo = pickListMaster.PickListNo;
                    pickListResult.PickListDetailId = pickListDetail.Id;
                    pickListResult.OrderDetailId = pickListDetail.OrderDetailId;
                    pickListResult.Item = pickListDetail.Item;
                    pickListResult.ItemDescription = pickListDetail.ItemDescription;
                    pickListResult.ReferenceItemCode = pickListDetail.ReferenceItemCode;
                    pickListResult.Uom = huLocationLotDetail.HuUom;
                    pickListResult.BaseUom = huLocationLotDetail.BaseUom;
                    pickListResult.UnitCount = huLocationLotDetail.UnitCount;
                    pickListResult.UnitQty = huLocationLotDetail.UnitQty;
                    pickListResult.HuId = huLocationLotDetail.HuId;
                    pickListResult.LotNo = huLocationLotDetail.LotNo;
                    pickListResult.IsConsignment = huLocationLotDetail.IsConsignment;
                    pickListResult.PlanBill = huLocationLotDetail.PlanBill;
                    pickListResult.QualityType = huLocationLotDetail.QualityType;
                    pickListResult.IsFreeze = huLocationLotDetail.IsFreeze;
                    pickListResult.IsATP = huLocationLotDetail.IsATP;
                    pickListResult.Qty = huLocationLotDetail.Qty / huLocationLotDetail.UnitQty;
                    this.genericMgr.Create(pickListResult);
                }
            }
            #endregion

            #region 新增拣货明细

            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void DeletePickListResult(IList<PickListResult> pickListResultList)
        {
            foreach (PickListResult pickListResult in pickListResultList)
            {
                this.genericMgr.DeleteById<PickListResult>(pickListResult.Id);
                if (!string.IsNullOrWhiteSpace(pickListResult.HuId))
                {
                    LocationLotDetail locationLotDetail = this.locationDetailMgr.GetHuLocationLotDetail(pickListResult.HuId);
                    locationLotDetail.OccupyType = CodeMaster.OccupyType.None;
                    locationLotDetail.OccupyReferenceNo = null;
                    genericMgr.Update(locationLotDetail);
                }
            }
            var pickListDetails = this.genericMgr.FindAllIn<PickListDetail>
                   (" from PickListDetail where Id in(? ", pickListResultList.Select(p => (object)p.PickListDetailId).Distinct());
            foreach (var pickListDetail in pickListDetails)
            {
                pickListDetail.PickedQty -= pickListResultList.Where(p => p.PickListDetailId == pickListDetail.Id).Sum(p => p.Qty);
                this.genericMgr.Update(pickListDetail);
            }

        }

        //[Transaction(TransactionMode.Requires)]
        //public void ManualClosePickList(string pickListNo)
        //{
        //    PickListMaster pickListMaster = genericMgr.FindById<PickListMaster>(pickListNo);
        //    ManualClosePickList(pickListMaster);
        //}

        //[Transaction(TransactionMode.Requires)]
        //public void ManualClosePickList(PickListMaster pickListMaster)
        //{
        //    if (pickListMaster.Status != CodeMaster.PickListStatus.InProcess)
        //    {
        //        throw new BusinessException("不能关闭状态为{1}的拣货单{0}。",
        //         pickListMaster.PickListNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.PickListStatus, pickListMaster.Status.ToString()));
        //    }

        //    VoidPickListDetail(pickListMaster);
        //    this.locationDetailMgr.VoidPickHu(pickListMaster.PickListNo);

        //    pickListMaster.Status = CodeMaster.PickListStatus.Close;
        //    pickListMaster.CloseDate = DateTime.Now;
        //    pickListMaster.CloseUserId = SecurityContextHolder.Get().Id;
        //    pickListMaster.CloseUserName = SecurityContextHolder.Get().FullName;

        //    this.genericMgr.Update(pickListMaster);
        //}        
        #endregion

        #region private methods
        private IList<OrderMaster> LoadOrderMasters(string[] orderNoList)
        {
            IList<object> para = new List<object>();
            string selectOrderMasterStatement = string.Empty;
            foreach (string orderNo in orderNoList)
            {
                if (selectOrderMasterStatement == string.Empty)
                {
                    selectOrderMasterStatement = "from OrderMaster where OrderNo in (?";
                }
                else
                {
                    selectOrderMasterStatement += ",?";
                }
                para.Add(orderNo);
            }
            selectOrderMasterStatement += ")";

            return this.genericMgr.FindAll<OrderMaster>(selectOrderMasterStatement, para.ToArray());
        }

        private IList<PickListDetail> TryLoadPickListDetails(PickListMaster pickListMaster)
        {
            if (!string.IsNullOrWhiteSpace(pickListMaster.PickListNo))
            {
                if (pickListMaster.PickListDetails == null)
                {
                    string hql = "from PickListDetail where PickListNo = ?";

                    pickListMaster.PickListDetails = this.genericMgr.FindAll<PickListDetail>(hql, pickListMaster.PickListNo);
                }

                return pickListMaster.PickListDetails;
            }
            else
            {
                return null;
            }
        }

        private IList<PickListResult> TryLoadPickListResults(PickListMaster pickListMaster)
        {
            if (!string.IsNullOrWhiteSpace(pickListMaster.PickListNo))
            {
                if (pickListMaster.PickListResults == null)
                {
                    string hql = "from PickListResult where PickListNo = ?";

                    pickListMaster.PickListResults = this.genericMgr.FindAll<PickListResult>(hql, pickListMaster.PickListNo);
                }

                return pickListMaster.PickListResults;
            }
            else
            {
                return null;
            }
        }

        private IList<OrderDetail> TryLoadOrderDetails(PickListMaster pickListMaster)
        {
            if (pickListMaster.OrderDetails == null)
            {
                if (pickListMaster.PickListDetails == null)
                {
                    TryLoadPickListDetails(pickListMaster);
                }

                #region 获取订单明细
                string hql = string.Empty;
                IList<object> para = new List<object>();
                foreach (int orderDetailId in pickListMaster.PickListDetails.Select(p => p.OrderDetailId).Distinct())
                {
                    if (hql == string.Empty)
                    {
                        hql = "from OrderDetail where Id in (?";
                    }
                    else
                    {
                        hql += ",?";
                    }

                    para.Add(orderDetailId);
                }
                hql += ")";

                pickListMaster.OrderDetails = this.genericMgr.FindAll<OrderDetail>(hql, para.ToArray());
                #endregion

                return pickListMaster.OrderDetails;
            }

            return null;
        }

        private void VoidPickListDetail(PickListMaster pickListMaster)
        {
            #region 获取拣货单明细
            TryLoadPickListDetails(pickListMaster);
            #endregion

            #region 获取订单明细
            IList<OrderDetail> orderDetailList = TryLoadOrderDetails(pickListMaster);
            #endregion

            #region 更新拣货单明细
            foreach (PickListDetail pickListDetail in pickListMaster.PickListDetails)
            {
                pickListDetail.IsClose = true;
                this.genericMgr.Update(pickListDetail);
            }
            #endregion

            #region 更新订单明细的拣货数
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                orderDetail.PickedQty -= pickListMaster.PickListDetails.Where(pd => pd.OrderDetailId == orderDetail.Id).Sum(p => p.Qty);
                this.genericMgr.Update(orderDetail);
            }
            #endregion

            #region 取消拣货单库存占用
            if (pickListMaster.Status == CodeMaster.PickListStatus.InProcess)
            {
                this.locationDetailMgr.CancelInventoryOccupy(CodeMaster.OccupyType.Pick, pickListMaster.PickListNo);
            }
            #endregion
        }
        #endregion

        #region 异步打印

        public void AsyncSendPrintData(PickListMaster pickListMaster)
        {
            //AsyncSend asyncSend = new AsyncSend(this.SendPrintData);
            //asyncSend.BeginInvoke(pickListMaster, null, null); 
            try
            {
                string location = (pickListMaster.PickListDetails != null && pickListMaster.PickListDetails.Count() > 0) ? pickListMaster.PickListDetails[0].LocationFrom : null;
                var subPrintOrderList = this.genericMgr.FindAll<SubPrintOrder>();
                var pubPrintOrders = subPrintOrderList.Where(p => (p.Flow == pickListMaster.Flow || string.IsNullOrWhiteSpace(p.Flow))
                            && (p.UserId == pickListMaster.CreateUserId || p.UserId == 0)
                            && (p.Region == pickListMaster.PartyFrom || string.IsNullOrWhiteSpace(p.Region))
                            && (location == null || p.Location == location || string.IsNullOrWhiteSpace(p.Location))
                            && p.ExcelTemplate == "PickList.xls"
                            ).Select(p => new PubPrintOrder
                            {
                                Client = p.Client,
                                ExcelTemplate = p.ExcelTemplate,
                                Code = pickListMaster.PickListNo,
                                Printer = p.Printer
                            });
                foreach (var pubPrintOrder in pubPrintOrders)
                {
                    this.genericMgr.Create(pubPrintOrder);
                }
            }
            catch (Exception ex)
            {
                pubSubLog.Error("Send data to print sevrer error:", ex);
            }
        }

        public delegate void AsyncSend(PickListMaster pickListMaster);
        #endregion
    }
}
