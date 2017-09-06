using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Entity.CUST;
using com.Sconit.Utility;
using com.Sconit.Entity.MSG;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class IpMgrImpl : BaseMgr, IIpMgr
    {
        #region 变量
        //private IPublishing proxy;
        //public IPubSubMgr pubSubMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IVehicleInFactoryMgrImpl vehicleInFactoryMgr { get; set; }
        public IHuMgr huMgr { get; set; }
        #endregion

        public IpMaster TransferOrder2Ip(IList<OrderMaster> orderMasterList)
        {
            #region 发货单头
            IpMaster ipMaster = MergeOrderMaster2IpMaster(orderMasterList);
            string WMSNo = string.Empty;
            foreach (OrderMaster orderMaster in orderMasterList)
            {
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (orderDetail.OrderDetailInputs.Select(i => i.WMSIpNo).Distinct().Count() > 1)
                    {
                        throw new TechnicalException("WMS发货单号不一致。");
                    }

                    if (string.IsNullOrWhiteSpace(WMSNo))
                    {
                        WMSNo = orderDetail.OrderDetailInputs.First().WMSIpNo;
                    }
                    else if (WMSNo != orderDetail.OrderDetailInputs.First().WMSIpNo)
                    {
                        throw new TechnicalException("WMS发货单号不一致。");
                    }
                }
            }
            ipMaster.WMSNo = WMSNo;
            #endregion

            #region 发货单明细
            foreach (OrderMaster orderMaster in orderMasterList)
            {
                if (orderMaster.Type != CodeMaster.OrderType.ScheduleLine)
                {
                    #region 非计划协议
                    if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0)
                    {
                        foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                        {
                            IpDetail ipDetail = Mapper.Map<OrderDetail, IpDetail>(orderDetail);
                            ipDetail.Flow = orderMaster.Flow;

                            #region 采购送货单明细需要合同号。
                            if (orderMaster.Type == CodeMaster.OrderType.Procurement
                                || orderMaster.Type == CodeMaster.OrderType.ScheduleLine)
                            {
                                ipDetail.ExternalOrderNo = orderMaster.ExternalOrderNo;
                            }
                            #endregion

                            //ipDetail.EffectiveDate = orderMaster.EffectiveDate;
                            ipDetail.IsInspect = orderMaster.IsInspect && ipDetail.IsInspect; //头和明细都选择报验才报验
                            if (orderDetail.StartDate.HasValue)
                            {
                                ipDetail.StartTime = orderDetail.StartDate;
                            }
                            else
                            {
                                ipDetail.StartTime = orderMaster.StartTime;
                            }
                            if (orderDetail.EndDate.HasValue)
                            {
                                ipDetail.WindowTime = orderDetail.EndDate;
                            }
                            else
                            {
                                ipDetail.WindowTime = orderMaster.WindowTime;
                            }
                            foreach (OrderDetailInput orderDetailInput in orderDetail.OrderDetailInputs)
                            {
                                IpDetailInput ipDetailInput = new IpDetailInput();
                                ipDetailInput.ShipQty = orderDetailInput.ShipQty;
                                if (orderMaster.IsShipScanHu
                                    || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                                {
                                    ipDetailInput.HuId = orderDetailInput.HuId;
                                    ipDetailInput.LotNo = orderDetailInput.LotNo;
                                }
                                ipDetailInput.WMSIpSeq = orderDetailInput.WMSIpSeq;
                                ipDetailInput.ManufactureParty = orderDetailInput.ManufactureParty;
                                ipDetailInput.OccupyType = orderDetailInput.OccupyType;
                                ipDetailInput.OccupyReferenceNo = orderDetailInput.OccupyReferenceNo;

                                ipDetail.AddIpDetailInput(ipDetailInput);
                            }

                            ipMaster.AddIpDetail(ipDetail);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 计划协议
                    if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0)
                    {
                        var groupedOrderDetailList = from det in orderMaster.OrderDetails
                                                     group det by
                                                     new
                                                     {
                                                         Item = det.Item,
                                                         EBELN = det.ExternalOrderNo,
                                                         EBELP = det.ExternalSequence.Split('-')[0],
                                                     } into gj
                                                     select new
                                                     {
                                                         Item = gj.Key.Item,
                                                         EBELN = gj.Key.EBELN,
                                                         EBELP = gj.Key.EBELP,
                                                         List = gj.ToList(),
                                                     };

                        foreach (var groupdOrderDetail in groupedOrderDetailList)
                        {
                            OrderDetail orderDetail = groupdOrderDetail.List.First();

                            IpDetail ipDetail = Mapper.Map<OrderDetail, IpDetail>(orderDetail);
                            ipDetail.Flow = orderMaster.Flow;
                            ipDetail.ExternalOrderNo = groupdOrderDetail.EBELN;
                            ipDetail.ExternalSequence = groupdOrderDetail.EBELP;

                            //ipDetail.EffectiveDate = orderMaster.EffectiveDate;
                            ipDetail.IsInspect = orderMaster.IsInspect && ipDetail.IsInspect; //头和明细都选择报验才报验
                            if (orderDetail.StartDate.HasValue)
                            {
                                ipDetail.StartTime = orderDetail.StartDate;
                            }
                            else
                            {
                                ipDetail.StartTime = orderMaster.StartTime;
                            }
                            if (orderDetail.EndDate.HasValue)
                            {
                                ipDetail.WindowTime = orderDetail.EndDate;
                            }
                            else
                            {
                                ipDetail.WindowTime = orderMaster.WindowTime;
                            }

                            foreach (OrderDetail eachOrderDetail in groupdOrderDetail.List)
                            {
                                foreach (OrderDetailInput orderDetailInput in eachOrderDetail.OrderDetailInputs)
                                {
                                    IpDetailInput ipDetailInput = new IpDetailInput();
                                    ipDetailInput.ShipQty = orderDetailInput.ShipQty;
                                    if (orderMaster.IsShipScanHu
                                        || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                                    {
                                        ipDetailInput.HuId = orderDetailInput.HuId;
                                        ipDetailInput.LotNo = orderDetailInput.LotNo;
                                    }
                                    ipDetailInput.WMSIpSeq = orderDetailInput.WMSIpSeq;
                                    ipDetailInput.ManufactureParty = orderDetailInput.ManufactureParty;
                                    ipDetailInput.OccupyType = orderDetailInput.OccupyType;
                                    ipDetailInput.OccupyReferenceNo = orderDetailInput.OccupyReferenceNo;

                                    ipDetail.AddIpDetailInput(ipDetailInput);
                                }
                            }

                            //计划协议发货检验整包装
                            if (orderMaster.IsReceiveFulfillUC && ipDetail.ShipQtyInput % ipDetail.UnitCount != 0)
                            {
                                //不是整包装
                                throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipQtyNotFulfillUnitCount, orderDetail.Item, orderDetail.UnitCount.ToString("0.##"));
                            }

                            ipMaster.AddIpDetail(ipDetail);
                        }
                    }
                    #endregion
                }
            }
            #endregion

            return ipMaster;
        }

        public IpMaster MergeOrderMaster2IpMaster(IList<OrderMaster> orderMasterList)
        {
            IpMaster ipMaster = new IpMaster();

            #region 路线代码
            //var flow = from om in orderMasterList select om.Flow;
            //if (flow.Distinct().Count() > 1)
            //{
            //    throw new BusinessException("路线代码不同不能合并发货。");
            //}
            //ipMaster.Flow = flow.Distinct().Single();
            ipMaster.Flow = (orderMasterList.OrderBy(om => om.Flow).Select(om => om.Flow)).First();
            #endregion

            #region 发货单类型
            if (orderMasterList.Where(mstr => mstr.OrderStrategy == CodeMaster.FlowStrategy.KIT).Count() > 0
               && orderMasterList.Where(mstr => mstr.OrderStrategy != CodeMaster.FlowStrategy.KIT).Count() > 0)
            {
                //KIT单和非KIT单不能合并发货。
                throw new BusinessException("KIT单不能和其它订单合并发货。");
            }

            if (orderMasterList.Where(mstr => mstr.OrderStrategy == CodeMaster.FlowStrategy.SEQ).Count() > 0
               && orderMasterList.Where(mstr => mstr.OrderStrategy != CodeMaster.FlowStrategy.SEQ).Count() > 0)
            {
                //排序单和非排序单不能合并发货。
                throw new BusinessException("排序单不能和其它订单合并发货。");
            }

            if (orderMasterList.Where(mstr => mstr.OrderStrategy == CodeMaster.FlowStrategy.KIT).Count() > 0)
            {
                ipMaster.Type = com.Sconit.CodeMaster.IpType.KIT;
            }
            else if (orderMasterList.Where(mstr => mstr.OrderStrategy == CodeMaster.FlowStrategy.KIT).Count() > 0)
            {
                //排序单能够直接发货？
                ipMaster.Type = com.Sconit.CodeMaster.IpType.SEQ;
            }
            else
            {
                ipMaster.Type = com.Sconit.CodeMaster.IpType.Normal;
            }
            #endregion

            #region 订单类型
            var orderType = from om in orderMasterList select om.Type;
            if (orderType.Distinct().Count() > 1)
            {
                throw new BusinessException("订单类型不同不能合并发货。");
            }
            ipMaster.OrderType = orderType.Distinct().Single();
            #endregion

            #region 订单类型
            var orderSubType = from om in orderMasterList select om.SubType;
            if (orderSubType.Distinct().Count() > 1)
            {
                throw new BusinessException("订单子类型不同不能合并发货。");
            }
            ipMaster.OrderSubType = orderSubType.Distinct().Single();
            #endregion

            #region 订单质量类型
            var qualityType = from om in orderMasterList select om.QualityType;
            if (qualityType.Distinct().Count() > 1)
            {
                throw new BusinessException("订单质量状态不同不能合并发货。");
            }
            ipMaster.QualityType = qualityType.Distinct().Single();
            #endregion

            #region 状态
            ipMaster.Status = com.Sconit.CodeMaster.IpStatus.Submit;
            #endregion

            #region 发出时间
            ipMaster.DepartTime = (from om in orderMasterList select om.StartTime).Min();
            #endregion

            #region 到达时间
            ipMaster.ArriveTime = (from om in orderMasterList select om.WindowTime).Min();
            #endregion

            #region PartyFrom
            var partyFrom = from om in orderMasterList select om.PartyFrom;
            if (partyFrom.Distinct().Count() > 1)
            {
                throw new BusinessException("来源组织不同不能合并发货。");
            }
            ipMaster.PartyFrom = partyFrom.Distinct().Single();
            #endregion

            #region PartyFromName
            ipMaster.PartyFromName = (from om in orderMasterList select om.PartyFromName).First();
            #endregion

            #region PartyTo
            var partyTo = from om in orderMasterList select om.PartyTo;
            if (partyTo.Distinct().Count() > 1)
            {
                throw new BusinessException("目的组织不同不能合并发货。");
            }
            ipMaster.PartyTo = partyTo.Distinct().Single();
            #endregion

            #region PartyToName
            ipMaster.PartyToName = (from om in orderMasterList select om.PartyToName).First();
            #endregion

            #region ShipFrom
            var shipFrom = from om in orderMasterList select om.ShipFrom;
            if (shipFrom.Distinct().Count() > 1)
            {
                throw new BusinessException("发货地址不同不能合并发货。");
            }
            ipMaster.ShipFrom = shipFrom.Distinct().Single();
            #endregion

            #region ShipFromAddr
            ipMaster.ShipFromAddress = (from om in orderMasterList select om.ShipFromAddress).First();
            #endregion

            #region ShipFromTel
            ipMaster.ShipFromTel = (from om in orderMasterList select om.ShipFromTel).First();
            #endregion

            #region ShipFromCell
            ipMaster.ShipFromCell = (from om in orderMasterList select om.ShipFromCell).First();
            #endregion

            #region ShipFromFax
            ipMaster.ShipFromFax = (from om in orderMasterList select om.ShipFromFax).First();
            #endregion

            #region ShipFromContact
            ipMaster.ShipFromContact = (from om in orderMasterList select om.ShipFromContact).First();
            #endregion

            #region ShipTo
            var shipTo = from om in orderMasterList select om.ShipTo;
            if (shipTo.Distinct().Count() > 1)
            {
                throw new BusinessException("收货地址不同不能合并发货。");
            }
            ipMaster.ShipTo = shipTo.Distinct().Single();
            #endregion

            #region ShipToAddr
            ipMaster.ShipToAddress = (from om in orderMasterList select om.ShipToAddress).First();
            #endregion

            #region ShipToTel
            ipMaster.ShipToTel = (from om in orderMasterList select om.ShipToTel).First();
            #endregion

            #region ShipToCell
            ipMaster.ShipToCell = (from om in orderMasterList select om.ShipToCell).First();
            #endregion

            #region ShipToFax
            ipMaster.ShipToFax = (from om in orderMasterList select om.ShipToFax).First();
            #endregion

            #region ShipToContact
            ipMaster.ShipToContact = (from om in orderMasterList select om.ShipToContact).First();
            #endregion

            #region Dock
            var dock = from om in orderMasterList select om.Dock;
            if (dock.Distinct().Count() > 1)
            {
                throw new BusinessException("道口不同不能合并发货。");
            }
            ipMaster.Dock = dock.Distinct().Single();
            #endregion

            #region IsAutoReceive
            var isAutoReceive = from om in orderMasterList select om.IsAutoReceive;
            if (isAutoReceive.Distinct().Count() > 1)
            {
                throw new BusinessException("自动收货选项不同不能合并发货。");
            }
            ipMaster.IsAutoReceive = isAutoReceive.Distinct().Single();
            #endregion

            #region IsShipScanHu
            var isShipScanHu = from om in orderMasterList select om.IsShipScanHu;
            if (isShipScanHu.Distinct().Count() > 1)
            {
                throw new BusinessException("发货扫描条码选项不同不能合并发货。");
            }
            ipMaster.IsShipScanHu = isShipScanHu.Distinct().Single();
            #endregion

            #region IsRecScanHu
            var isRecScanHu = from om in orderMasterList select om.IsReceiveScanHu;
            if (isRecScanHu.Distinct().Count() > 1)
            {
                throw new BusinessException("收货扫描条码选项不同不能合并发货。");
            }
            ipMaster.IsReceiveScanHu = isRecScanHu.Distinct().Single();
            #endregion

            #region IsPrintAsn
            ipMaster.IsPrintAsn = orderMasterList.Where(om => om.IsPrintAsn == true) != null;
            #endregion

            #region IsAsnPrinted
            ipMaster.IsAsnPrinted = false;
            #endregion

            #region IsPrintRec
            ipMaster.IsPrintReceipt = orderMasterList.Where(om => om.IsPrintReceipt == true) != null;
            #endregion

            #region IsRecExceed
            var isRecExceed = from om in orderMasterList select om.IsReceiveExceed;
            if (isRecExceed.Distinct().Count() > 1)
            {
                throw new BusinessException("允许超收选项不同不能合并发货。");
            }
            ipMaster.IsReceiveExceed = isRecExceed.Distinct().Single();
            #endregion

            #region IsRecFulfillUC
            var isRecFulfillUC = from om in orderMasterList select om.IsReceiveFulfillUC;
            if (isRecFulfillUC.Distinct().Count() > 1)
            {
                throw new BusinessException("收货满足包装选项不同不能合并发货。");
            }
            ipMaster.IsReceiveFulfillUC = isRecFulfillUC.Distinct().Single();
            #endregion

            #region IsRecFifo
            var isRecFifo = from om in orderMasterList select om.IsReceiveFifo;
            if (isRecFifo.Distinct().Count() > 1)
            {
                throw new BusinessException("收货先进先出选项不同不能合并发货。");
            }
            ipMaster.IsReceiveFifo = isRecFifo.Distinct().Single();
            #endregion

            #region IsAsnUniqueRec
            var isAsnUniqueRec = from om in orderMasterList select om.IsAsnUniqueReceive;
            if (isAsnUniqueRec.Distinct().Count() > 1)
            {
                throw new BusinessException("ASN一次性收货选项不同不能合并发货。");
            }
            ipMaster.IsAsnUniqueReceive = isAsnUniqueRec.Distinct().Single();
            #endregion

            #region IsRecCreateHu
            var createHuOption = (from om in orderMasterList
                                  select om.CreateHuOption).Distinct();
            if (createHuOption != null && createHuOption.Count() > 1)
            {
                throw new BusinessException("创建条码选项不同不能合并发货。");
            }
            ipMaster.CreateHuOption = createHuOption.Single();
            #endregion

            #region IsCheckPartyFromAuth
            ipMaster.IsCheckPartyFromAuthority = orderMasterList.Where(om => om.IsCheckPartyFromAuthority == true).Count() > 0;
            #endregion

            #region IsCheckPartyToAuth
            ipMaster.IsCheckPartyToAuthority = orderMasterList.Where(om => om.IsCheckPartyToAuthority == true).Count() > 0;
            #endregion

            #region RecGapTo
            var recGapTo = from om in orderMasterList select om.ReceiveGapTo;
            if (recGapTo.Distinct().Count() > 1)
            {
                throw new BusinessException("收货差异调整选项不同不能合并发货。");
            }
            ipMaster.ReceiveGapTo = recGapTo.Distinct().Single();
            #endregion

            #region AsnTemplate
            var asnTemplate = orderMasterList.Select(om => om.AsnTemplate).First();
            ipMaster.AsnTemplate = asnTemplate;
            #endregion

            #region RecTemplate
            var recTemplate = orderMasterList.Select(om => om.ReceiptTemplate).First();
            ipMaster.ReceiptTemplate = recTemplate;
            #endregion

            #region HuTemplate
            var huTemplate = orderMasterList.Select(om => om.HuTemplate).First();
            ipMaster.HuTemplate = huTemplate;
            #endregion

            #region 外部订单号
            var externalOrderNos = orderMasterList.Select(p => p.ExternalOrderNo).Distinct();
            if (externalOrderNos.Count() > 1)
            {
                throw new BusinessException("外部订单号不同不能合并发货。");
            }
            if (string.IsNullOrWhiteSpace(ipMaster.ExternalIpNo))
            {
                ipMaster.ExternalIpNo = externalOrderNos.Single();
            }
            #endregion
            return ipMaster;
        }

        public IpMaster TransferPickList2Ip(PickListMaster pickListMaster)
        {
            #region 发货单头
            IpMaster ipMaster = Mapper.Map<PickListMaster, IpMaster>(pickListMaster);
            ipMaster.IsShipScanHu = true;
            ipMaster.Status = CodeMaster.IpStatus.Submit;
            #endregion

            #region 发货单明细
            IList<OrderMaster> orderMasterList = this.LoadOrderMasters(pickListMaster.OrderDetails.Select(det => det.OrderNo).ToArray());
            foreach (OrderDetail orderDetail in pickListMaster.OrderDetails)
            {
                IList<PickListResult> pickListResultList = pickListMaster.PickListResults.Where(p => p.OrderDetailId == orderDetail.Id).ToList();
                if (pickListResultList.Count > 0)
                {
                    IpDetail ipDetail = Mapper.Map<OrderDetail, IpDetail>(orderDetail);
                    ipDetail.Flow = orderMasterList.Where(mstr => mstr.OrderNo == orderDetail.OrderNo).Single().Flow;

                    //ipDetail.EffectiveDate = orderMaster.EffectiveDate;
                    PickListDetail pickListDetail = pickListMaster.PickListDetails.Where(p => p.OrderDetailId == orderDetail.Id).First();
                    ipDetail.IsInspect = pickListDetail.IsInspect;
                    ipDetail.StartTime = pickListDetail.StartTime;
                    ipDetail.WindowTime = pickListDetail.WindowTime;

                    foreach (PickListResult pickListResult in pickListResultList)
                    {
                        IpDetailInput ipDetailInput = new IpDetailInput();
                        ipDetailInput.ShipQty = pickListResult.Qty;
                        ipDetailInput.HuId = pickListResult.HuId;
                        ipDetailInput.LotNo = pickListResult.LotNo;
                        ipDetailInput.OccupyType = CodeMaster.OccupyType.Pick;
                        ipDetailInput.OccupyReferenceNo = pickListMaster.PickListNo;

                        ipDetail.AddIpDetailInput(ipDetailInput);
                    }

                    ipMaster.AddIpDetail(ipDetail);
                }
            }
            #endregion

            return ipMaster;
        }

        public IpMaster TransferSequenceMaster2Ip(SequenceMaster sequenceMaster)
        {
            #region 发货单头
            IpMaster ipMaster = Mapper.Map<SequenceMaster, IpMaster>(sequenceMaster);
            ipMaster.IsShipScanHu = true;
            ipMaster.IsReceiveScanHu = true;
            ipMaster.IsReceiveExceed = false;
            ipMaster.IsReceiveFulfillUC = true;
            ipMaster.IsReceiveFifo = false;
            ipMaster.IsAsnUniqueReceive = true;
            ipMaster.CreateHuOption = CodeMaster.CreateHuOption.None;
            ipMaster.ReceiveGapTo = CodeMaster.ReceiveGapTo.RecordIpGap;
            ipMaster.Status = CodeMaster.IpStatus.Submit;
            ipMaster.WMSNo = sequenceMaster.WMSIpNo;
            #endregion

            #region 发货单明细
            foreach (SequenceDetail sequenceDetail in sequenceMaster.SequenceDetails.OrderBy(s => s.Sequence))
            {
                OrderDetail orderDetail = sequenceMaster.OrderDetails.Where(o => o.Id == sequenceDetail.OrderDetailId).Single();
                IpDetail ipDetail = Mapper.Map<OrderDetail, IpDetail>(orderDetail);
                ipDetail.Flow = sequenceMaster.Flow;
                ipDetail.IsInspect = false;
                ipDetail.StartTime = sequenceDetail.StartTime;
                ipDetail.WindowTime = sequenceDetail.WindowTime;

                IpDetailInput ipDetailInput = new IpDetailInput();
                ipDetailInput.ShipQty = sequenceDetail.Qty / sequenceDetail.UnitQty;
                ipDetailInput.HuId = sequenceDetail.HuId;
                ipDetailInput.LotNo = sequenceDetail.LotNo;
                if (!string.IsNullOrWhiteSpace(ipDetailInput.HuId))
                {
                    ipDetailInput.OccupyType = CodeMaster.OccupyType.Sequence;
                    ipDetailInput.OccupyReferenceNo = sequenceMaster.SequenceNo;
                }
                ipDetailInput.SequenceNo = sequenceDetail.SequenceNo;
                ipDetailInput.WMSIpSeq = sequenceDetail.WMSIpSeq;

                ipDetail.AddIpDetailInput(ipDetailInput);
                ipMaster.AddIpDetail(ipDetail);
            }
            #endregion

            return ipMaster;
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateIp(IpMaster ipMaster)
        {
            CreateIp(ipMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateIp(IpMaster ipMaster, DateTime effectiveDate)
        {

            #region 发货明细不能为空
            if (ipMaster.IpDetails == null || ipMaster.IpDetails.Count == 0)
            {
                throw new BusinessException(Resources.ORD.IpMaster.Errors_IpDetailIsEmpty);
            }
            #endregion

            #region 保存发货单头
            ipMaster.IpNo = numberControlMgr.GetIpNo(ipMaster);
            ipMaster.EffectiveDate = effectiveDate;
            this.genericMgr.Create(ipMaster);
            #endregion

            //#region 按订单明细汇总发货数，按条码发货一条订单明细会对应多条发货记录
            //var summaryIpDet = from det in ipMaster.IpDetails
            //                   group det by det.OrderDetailId into g           
            //                   select new
            //                   {
            //                       OrderDetailId = g.Key,
            //                       List = g.ToList()
            //                   };
            //#endregion

            #region 保存发货单明细
            int seqCount = 1;
            foreach (IpDetail ipDetail in ipMaster.IpDetails.OrderBy(i => i.OrderNo).ThenBy(i => i.OrderDetailSequence))
            {
                if (ipDetail.ShipQtyInput < 0)
                {
                    throw new BusinessException(Resources.SYS.ErrorMessage.Errors_NegativeExceptiond);
                }
                ipDetail.Qty = ipDetail.ShipQtyInput;
                ipDetail.Sequence = seqCount++;
                ipDetail.IpNo = ipMaster.IpNo;

                this.genericMgr.Create(ipDetail);
            }
            #endregion

            #region 发货创建条码
            if ((ipMaster.OrderType == CodeMaster.OrderType.Procurement
                || ipMaster.OrderType == CodeMaster.OrderType.CustomerGoods)
                && ipMaster.CreateHuOption == CodeMaster.CreateHuOption.Ship)
            {
                var flowMaster = ipMaster.CurrentFlowMaster;
                if (flowMaster == null && !string.IsNullOrWhiteSpace(ipMaster.Flow))
                {
                    flowMaster = this.genericMgr.FindById<FlowMaster>(ipMaster.Flow);
                    ipMaster.CurrentFlowMaster = flowMaster;
                }
                foreach (IpDetail ipDetail in ipMaster.IpDetails)
                {
                    IList<IpDetail> ipDetailList = new List<IpDetail>();
                    ipDetail.ManufactureParty = ipMaster.PartyFrom;
                    ipDetail.HuQty = ipDetail.ShipQtyInput;
                    ipDetail.LotNo = LotNoHelper.GenerateLotNo();
                    ipDetailList.Add(ipDetail);

                    if (flowMaster != null)
                    {
                        if (flowMaster.UcDeviation >= 0)
                        {
                            ipDetail.MaxUc = ipDetail.UnitCount * Convert.ToDecimal(((flowMaster.UcDeviation / 100) + 1));
                            ipDetail.MinUc = ipDetail.UnitCount * Convert.ToDecimal((1 - (flowMaster.UcDeviation / 100)));
                        }
                        else
                        {
                            ipDetail.MaxUc = ipDetail.ReceivedQty;
                            ipDetail.MinUc = ipDetail.ReceivedQty;
                        }
                    }
                    else
                    {
                        ipDetail.MaxUc = ipDetail.UnitCount;
                        ipDetail.MinUc = ipDetail.UnitCount;
                    }

                    IList<Hu> huList = huMgr.CreateHu(ipMaster, ipDetailList);

                    ipDetail.IpDetailInputs = (from hu in huList
                                               select new IpDetailInput
                                               {
                                                   ShipQty = hu.Qty,    //订单单位
                                                   HuId = hu.HuId,
                                                   LotNo = hu.LotNo,
                                                   IsCreatePlanBill = false,
                                                   IsConsignment = false,
                                                   PlanBill = null,
                                                   ActingBill = null,
                                                   IsFreeze = false,
                                                   IsATP = true,
                                                   OccupyType = com.Sconit.CodeMaster.OccupyType.None,
                                                   OccupyReferenceNo = null
                                               }).ToList();
                }
            }
            #endregion

            #region 出库
            //条码上不带状态库位等信息，状态全部通过查找库存明细来获得。
            //暂不支持发货创建条码
            foreach (IpDetail ipDetail in ipMaster.IpDetails.OrderByDescending(det => det.ManufactureParty))
            {
                ipDetail.CurrentPartyFrom = ipMaster.PartyFrom;  //为了记录库存事务
                ipDetail.CurrentPartyFromName = ipMaster.PartyFromName;  //为了记录库存事务
                ipDetail.CurrentPartyTo = ipMaster.PartyTo;      //为了记录库存事务
                ipDetail.CurrentPartyToName = ipMaster.PartyToName;      //为了记录库存事务
                //inputIpDetail.CurrentIsATP = inputIpDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                //inputIpDetail.CurrentIsFreeze = false;              //默认只出库未冻结库存
                //ipDetail.CurrentOccupyType = com.Sconit.CodeMaster.OccupyType.None; //todo-默认出库未占用库存，除非拣货或检验的出库

                IList<InventoryTransaction> inventoryTransactionList = this.locationDetailMgr.InventoryOut(ipDetail);

                if (inventoryTransactionList != null && inventoryTransactionList.Count > 0)
                {
                    IList<IpLocationDetail> ipLocationDetailList = (from trans in inventoryTransactionList
                                                                    group trans by new
                                                                    {
                                                                        HuId = trans.HuId,
                                                                        LotNo = trans.LotNo,
                                                                        IsCreatePlanBill = trans.IsCreatePlanBill,
                                                                        IsConsignment = trans.IsConsignment,
                                                                        PlanBill = trans.PlanBill,
                                                                        ActingBill = trans.ActingBill,
                                                                        IsFreeze = trans.IsFreeze,
                                                                        IsATP = trans.IsATP,
                                                                        OccupyType = trans.OccupyType,
                                                                        OccupyReferenceNo = trans.OccupyReferenceNo,
                                                                        WMSSeq = trans.WMSIpSeq,
                                                                    } into g
                                                                    select new IpLocationDetail
                                                                    {
                                                                        Item = ipDetail.Item,
                                                                        HuId = g.Key.HuId,
                                                                        LotNo = g.Key.LotNo,
                                                                        IsCreatePlanBill = g.Key.IsCreatePlanBill,
                                                                        IsConsignment = g.Key.IsConsignment,
                                                                        PlanBill = g.Key.PlanBill,
                                                                        ActingBill = g.Key.ActingBill,
                                                                        QualityType = ipDetail.QualityType,
                                                                        IsFreeze = g.Key.IsFreeze,
                                                                        IsATP = g.Key.IsATP,
                                                                        OccupyType = g.Key.OccupyType == CodeMaster.OccupyType.Inspect ? g.Key.OccupyType : CodeMaster.OccupyType.None, //只有检验才保留占用状态
                                                                        OccupyReferenceNo = g.Key.OccupyType == CodeMaster.OccupyType.Inspect ? g.Key.OccupyReferenceNo : null,
                                                                        Qty = g.Sum(t => -t.Qty),       //出库的inventoryTrans为负数，转为ipLocationDetail需要为正数
                                                                        WMSSeq = g.Key.WMSSeq,
                                                                    }).ToList();

                    ipDetail.AddIpLocationDetail(ipLocationDetailList);
                }
            }
            #endregion

            #region 保存发货单库存明细
            foreach (IpDetail ipDetail in ipMaster.IpDetails)
            {
                //if (!string.IsNullOrWhiteSpace(ipDetail.LocationFrom))
                //{
                if (ipDetail.IpLocationDetails == null || ipDetail.IpLocationDetails.Count == 0)
                {
                    throw new TechnicalException("IpLocationDetails is empty.");
                }

                foreach (IpLocationDetail ipLocationDetail in ipDetail.IpLocationDetails)
                {
                    ipLocationDetail.IpNo = ipMaster.IpNo;
                    ipLocationDetail.IpDetailId = ipDetail.Id;
                    ipLocationDetail.OrderType = ipDetail.OrderType;
                    ipLocationDetail.OrderDetailId = ipDetail.OrderDetailId;
                    genericMgr.Create(ipLocationDetail);
                }
                //}
            }
            #endregion

            this.AsyncSendPrintData(ipMaster);

            ////基于性能考虑,在此过滤不需要的ASN
            //string loc = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.WMSAnjiRegion);
            //if (ipMaster.PartyTo.Equals(loc, StringComparison.OrdinalIgnoreCase))
            //{
            //    //this.genericMgr.FlushSession();
            //    //AsyncRecourdMessageQueue(MethodNameType.CreateIp, ipMaster.IpNo);
            //    this.CreateMessageQueue("CreateIp", ipMaster.IpNo);
            //}
        }

        [Transaction(TransactionMode.Requires)]
        public void ManualCloseIp(string IpNo)
        {
            ManualCloseIp(this.genericMgr.FindById<IpMaster>(IpNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void ManualCloseIp(IpMaster ipMaster)
        {
            if (!Utility.SecurityHelper.HasPermission(ipMaster))
            {
                //throw new BusinessException("没有此送货单{0}的操作权限。", ipMaster.IpNo);
            }

            #region 查找未关闭IpDetail
            IList<IpDetail> openIpDetailList = this.genericMgr.FindAll<IpDetail>(
                "from IpDetail where IpNo = ? and Type = ? and IsClose = ?",
                new Object[] { ipMaster.IpNo, CodeMaster.IpDetailType.Normal, false });
            #endregion

            #region 记录未收货差异
            if (openIpDetailList != null && openIpDetailList.Count > 0)
            {
                #region 查找未关闭ASN库存对象
                string hql = "from IpLocationDetail where IsClose = ? and IpDetailId in (?";
                IList<IpLocationDetail> openIpLocationDetailList = this.genericMgr.FindAllIn<IpLocationDetail>
                    (hql, openIpDetailList.Select(p => (object)p.Id), new object[] { false });
                #endregion

                #region 生成未收货差异
                IList<IpDetail> gapIpDetailList = new List<IpDetail>();
                if (openIpDetailList != null && openIpDetailList.Count > 0)
                {
                    foreach (IpDetail openIpDetail in openIpDetailList)
                    {
                        var targetOpenIpLocationDetailList = openIpLocationDetailList.Where(o => o.IpDetailId == openIpDetail.Id);

                        IpDetail gapIpDetail = Mapper.Map<IpDetail, IpDetail>(openIpDetail);
                        gapIpDetail.Type = com.Sconit.CodeMaster.IpDetailType.Gap;
                        gapIpDetail.GapReceiptNo = string.Empty;                            //todo 记录产生差异的收货单号
                        gapIpDetail.Qty = targetOpenIpLocationDetailList.Sum(o => o.RemainReceiveQty / openIpDetail.UnitQty);
                        gapIpDetail.ReceivedQty = 0;
                        gapIpDetail.IsClose = false;
                        gapIpDetail.GapIpDetailId = openIpDetail.Id;

                        gapIpDetail.IpLocationDetails = (from locDet in targetOpenIpLocationDetailList
                                                         select new IpLocationDetail
                                                         {
                                                             IpNo = locDet.IpNo,
                                                             OrderType = locDet.OrderType,
                                                             OrderDetailId = locDet.OrderDetailId,
                                                             Item = locDet.Item,
                                                             HuId = locDet.HuId,
                                                             LotNo = locDet.LotNo,
                                                             IsCreatePlanBill = locDet.IsCreatePlanBill,
                                                             PlanBill = locDet.PlanBill,
                                                             ActingBill = locDet.ActingBill,
                                                             IsFreeze = locDet.IsFreeze,
                                                             //IsATP = locDet.IsATP,
                                                             IsATP = false,
                                                             QualityType = locDet.QualityType,
                                                             OccupyType = locDet.OccupyType,
                                                             OccupyReferenceNo = locDet.OccupyReferenceNo,
                                                             Qty = locDet.RemainReceiveQty,
                                                             ReceivedQty = 0,
                                                             IsClose = false
                                                         }).ToList();

                        gapIpDetailList.Add(gapIpDetail);
                    }
                }
                #endregion

                #region 关闭未收货ASN明细和库存明细
                if (openIpDetailList != null && openIpDetailList.Count > 0)
                {
                    foreach (IpDetail openIpDetail in openIpDetailList)
                    {
                        openIpDetail.IsClose = true;
                        this.genericMgr.Update(openIpDetail);
                    }
                }

                if (openIpLocationDetailList != null && openIpLocationDetailList.Count > 0)
                {
                    foreach (IpLocationDetail openIpLocationDetail in openIpLocationDetailList)
                    {
                        openIpLocationDetail.IsClose = true;
                        this.genericMgr.Update(openIpLocationDetail);
                    }
                }
                //string batchupdateipdetailstatement = "update from ipdetail set isclose = true where ipno = ? and isclose = false";
                //genericmgr.update(batchupdateipdetailstatement, ipmaster.ipno);

                //string batchUpdateIpLocationDetailStatement = "update from IpLocationDetail set IsClose = True where IpNo = ? and IsClose = False";
                //genericMgr.Update(batchUpdateIpLocationDetailStatement, ipMaster.IpNo);                
                #endregion

                #region 记录收货差异
                if (gapIpDetailList != null && gapIpDetailList.Count > 0)
                {
                    foreach (IpDetail gapIpDetail in gapIpDetailList)
                    {
                        // gapIpDetail.GapReceiptNo = receiptMaster.ReceiptNo;
                        this.genericMgr.Create(gapIpDetail);

                        foreach (IpLocationDetail gapIpLocationDetail in gapIpDetail.IpLocationDetails)
                        {
                            gapIpLocationDetail.IpDetailId = gapIpDetail.Id;
                            this.genericMgr.Create(gapIpLocationDetail);
                        }
                    }
                }
                #endregion
            }
            else
            {
                //DoCloseIpMaster(ipMaster);
                throw new BusinessException("所有明细已经全部关闭,无需再次关闭");
            }
            ipMaster.Status = com.Sconit.CodeMaster.IpStatus.InProcess;
            this.genericMgr.Update(ipMaster);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCloseIp(IpMaster ipMaster)
        {
            if (ipMaster.Status == com.Sconit.CodeMaster.IpStatus.Submit
                || ipMaster.Status == com.Sconit.CodeMaster.IpStatus.InProcess)
            {
                this.genericMgr.FlushSession();
                string hql = "select count(*) as counter from IpDetail where IpNo = ? and IsClose = ?";
                long counter = this.genericMgr.FindAll<long>(hql, new Object[] { ipMaster.IpNo, false })[0];
                if (counter == 0)
                {
                    DoCloseIpMaster(ipMaster);
                }

                #region 关入厂证
                IList<VehicleInFactoryDetail> vehicleInFactoryDetailList = genericMgr.FindAll<VehicleInFactoryDetail>(" from VehicleInFactoryDetail as v where v.IpNo = ? and v.IsClose = ?", new object[] { ipMaster.IpNo, false });
                if (vehicleInFactoryDetailList != null && vehicleInFactoryDetailList.Count > 0)
                {
                    //理论上应该没有多个的，有也不能影响收货，多个就先关多个吧
                    foreach (VehicleInFactoryDetail vehicleInFactoryDetail in vehicleInFactoryDetailList)
                    {
                        vehicleInFactoryMgr.CloseVehicleInFactoryDetail(vehicleInFactoryDetail);
                    }
                }
                #endregion
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelIp(string ipNo)
        {
            IpMaster ipMaster = this.genericMgr.FindById<IpMaster>(ipNo);
            CancelIp(ipMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelIp(string ipNo, DateTime effectiveDate)
        {
            IpMaster ipMaster = this.genericMgr.FindById<IpMaster>(ipNo);
            CancelIp(ipMaster, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelIp(IpMaster ipMaster)
        {
            CancelIp(ipMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelIp(IpMaster ipMaster, DateTime effectiveDate)
        {
            if (!Utility.SecurityHelper.HasPermission(ipMaster))
            {
                //throw new BusinessException("没有此送货单{0}的操作权限。", ipMaster.IpNo);
            }

            #region 获取送货单库存明细
            string selectIpLocationDetailStatement = "from IpLocationDetail where IpNo = ? and IsClose = ?";
            IList<IpLocationDetail> ipLocationDetailList = this.genericMgr.FindAll<IpLocationDetail>(selectIpLocationDetailStatement, new object[] { ipMaster.IpNo, false });
            #endregion

            #region 获取送货单明细
            if (ipMaster.IpDetails == null)
            {
                string selectIpDetailStatement = "from IpDetail where IpNo = ? and IsClose = ?";
                ipMaster.IpDetails = this.genericMgr.FindAll<IpDetail>(selectIpDetailStatement, new object[] { ipMaster.IpNo, false });
            }
            #endregion

            #region 一点货都没有收的才能冲销
            var recIpDetails = ipMaster.IpDetails.Count(i => i.ReceivedQty > 0);
            if (recIpDetails > 0)
            {
                throw new BusinessException("已收过货的送货单不能冲销。", ipMaster.IpNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.IpStatus, ((int)ipMaster.Status).ToString()));
            }
            #endregion

            #region 关闭送货单库存明细
            foreach (IpLocationDetail ipLocationDetail in ipLocationDetailList)
            {
                ipLocationDetail.IsClose = true;
                this.genericMgr.Update(ipLocationDetail);
            }
            #endregion

            #region 关闭送货单明细
            foreach (IpDetail ipDetail in ipMaster.IpDetails)
            {
                ipDetail.IsClose = true;
                this.genericMgr.Update(ipDetail);
            }
            #endregion

            #region 更新发货单状态
            ipMaster.Status = com.Sconit.CodeMaster.IpStatus.Cancel;
            this.genericMgr.Update(ipMaster);
            #endregion

            #region 更新订单明细
            if (ipMaster.OrderType != CodeMaster.OrderType.ScheduleLine)
            {
                #region 非计划协议
                #region 获取订单明细
                string selectOrderDetailDetailStatement = "from OrderDetail where Id in (select OrderDetailId from IpDetail where IpNo = ? and IsClose = ?)";
                IList<OrderDetail> orderDetailList = this.genericMgr.FindAll<OrderDetail>(selectOrderDetailDetailStatement, new object[] { ipMaster.IpNo, false });
                #endregion

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    #region 更新订单数量
                    orderDetail.ShippedQty -= ipMaster.IpDetails.Where(det => det.OrderDetailId == orderDetail.Id).Sum(det => det.Qty);
                    this.genericMgr.Update(orderDetail);
                    #endregion
                }
                #endregion
            }
            else
            {
                #region 计划协议
                BusinessException businessException = new BusinessException();
                foreach (IpDetail ipDetail in ipMaster.IpDetails)
                {
                    decimal remainQty = ipDetail.Qty;

                    IList<OrderDetail> scheduleOrderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>("select * from ORD_OrderDet_8 where ExtNo = ? and ExtSeq like ? and ScheduleType = ? and ShipQty > RecQty order by EndDate desc",
                                                new object[] { ipDetail.ExternalOrderNo, ipDetail.ExternalSequence + "-%", CodeMaster.ScheduleType.Firm });

                    if (scheduleOrderDetailList != null && scheduleOrderDetailList.Count > 0)
                    {
                        foreach (OrderDetail scheduleOrderDetail in scheduleOrderDetailList)
                        {
                            //更新订单的发货数
                            if (remainQty > (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty))
                            {
                                remainQty -= (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty);
                                scheduleOrderDetail.ShippedQty = scheduleOrderDetail.ReceivedQty;
                            }
                            else
                            {
                                scheduleOrderDetail.ShippedQty -= remainQty;
                                remainQty = 0;
                                break;
                            }

                            this.genericMgr.Update(scheduleOrderDetail);
                        }
                    }

                    if (remainQty > 0)
                    {
                        businessException.AddMessage(Resources.ORD.IpMaster.Errors_ReceiveQtyExcceedOrderQty, ipMaster.IpNo, ipDetail.Item);
                    }
                }
                #endregion
            }
            #endregion

            #region 退回排序单状态
            if (!string.IsNullOrEmpty(ipMaster.SequenceNo))
            {
                #region 更新排序单头
                SequenceMaster sequenceMaster = this.genericMgr.FindById<SequenceMaster>(ipMaster.SequenceNo);

                sequenceMaster.Status = CodeMaster.SequenceStatus.Pack;
                sequenceMaster.ShipDate = null;
                sequenceMaster.ShipUserId = 0;
                sequenceMaster.ShipUserName = null;

                this.genericMgr.Update(sequenceMaster);
                #endregion
            }
            #endregion

            this.genericMgr.FlushSession();

            #region 更新库存 委外/生产退回不更新库存
            if (ipMaster.OrderType != CodeMaster.OrderType.SubContract && ipMaster.OrderType != CodeMaster.OrderType.Production)
            {
                foreach (IpDetail ipDetail in ipMaster.IpDetails)
                {
                    ipDetail.IsVoid = true;
                    //ipDetail.OrderSubType = com.Sconit.CodeMaster.OrderSubType.Return;
                    var targetIpLocationDetail = from locDet in ipLocationDetailList
                                                 where locDet.IpDetailId == ipDetail.Id
                                                 select locDet;

                    ipDetail.CurrentPartyFrom = ipMaster.PartyFrom;  //为了记录库存事务
                    ipDetail.CurrentPartyFromName = ipMaster.PartyFromName;  //为了记录库存事务
                    ipDetail.CurrentPartyTo = ipMaster.PartyTo;      //为了记录库存事务
                    ipDetail.CurrentPartyToName = ipMaster.PartyToName;      //为了记录库存事务

                    foreach (IpLocationDetail ipLocationDetail in targetIpLocationDetail)
                    {
                        IpDetailInput ipDetailInput = new IpDetailInput();
                        ipDetailInput.HuId = ipLocationDetail.HuId;
                        ipDetailInput.ShipQty = -ipLocationDetail.Qty / ipDetail.UnitQty;  //转为订单单位
                        ipDetailInput.LotNo = ipLocationDetail.LotNo;
                        ipDetailInput.IsCreatePlanBill = ipLocationDetail.IsCreatePlanBill;
                        ipDetailInput.IsConsignment = ipLocationDetail.IsConsignment;
                        ipDetailInput.PlanBill = ipLocationDetail.PlanBill;
                        ipDetailInput.ActingBill = ipLocationDetail.ActingBill;
                        ipDetailInput.IsATP = ipLocationDetail.IsATP;
                        ipDetailInput.IsFreeze = ipLocationDetail.IsFreeze;
                        ipDetailInput.OccupyType = ipLocationDetail.OccupyType;
                        ipDetailInput.OccupyReferenceNo = ipLocationDetail.OccupyReferenceNo;

                        ipDetail.AddIpDetailInput(ipDetailInput);
                    }
                    #region 更新库存、记库存事务
                    this.locationDetailMgr.InventoryOut(ipDetail, effectiveDate);
                    #endregion
                }
            }
            #endregion

            //string loc = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.WMSAnjiRegion);
            //if (ipMaster.PartyTo.Equals(loc, StringComparison.OrdinalIgnoreCase))
            //{
            //    //this.genericMgr.FlushSession();
            //    //AsyncRecourdMessageQueue(MethodNameType.CancelIp, ipMaster.IpNo);
            //    this.CreateMessageQueue("CancelIp", ipMaster.IpNo);
            //}
        }

        #region private methods

        private void DoCloseIpMaster(IpMaster ipMaster)
        {
            ipMaster.Status = com.Sconit.CodeMaster.IpStatus.Close;
            ipMaster.CloseDate = DateTime.Now;
            ipMaster.CloseUserId = SecurityContextHolder.Get().Id;
            ipMaster.CloseUserName = SecurityContextHolder.Get().FullName;

            this.genericMgr.Update(ipMaster);
        }

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
        #endregion

        #region 异步打印

        public void AsyncSendPrintData(IpMaster ipMaster)
        {
            //AsyncSend asyncSend = new AsyncSend(this.SendPrintData);
            //asyncSend.BeginInvoke(ipMaster, null, null);
            if (ipMaster.IsPrintAsn)
            {
                try
                {
                    string location = (ipMaster.IpDetails != null && ipMaster.IpDetails.Count() > 0) ? ipMaster.IpDetails[0].LocationFrom : null;
                    var subPrintOrderList = this.genericMgr.FindAll<SubPrintOrder>();
                    var pubPrintOrders = subPrintOrderList.Where(p => (p.Flow == ipMaster.Flow || string.IsNullOrWhiteSpace(p.Flow))
                                && (p.UserId == ipMaster.CreateUserId || p.UserId == 0)
                                && (p.Region == ipMaster.PartyFrom || string.IsNullOrWhiteSpace(p.Region))
                                && (location == null || p.Location == location || string.IsNullOrWhiteSpace(p.Location))
                                && p.ExcelTemplate == ipMaster.AsnTemplate)
                                .Select(p => new PubPrintOrder
                                {
                                    Client = p.Client,
                                    ExcelTemplate = p.ExcelTemplate,
                                    Code = ipMaster.IpNo,
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
        }


        public delegate void AsyncSend(IpMaster ipMaster);

        #endregion

        private void CreateMessageQueue(string methodName, string paramValue)
        {
            MessageQueue messageQueue = new MessageQueue();
            messageQueue.MethodName = methodName;
            messageQueue.ParamValue = paramValue;
            messageQueue.Status = CodeMaster.MQStatusEnum.Pending;
            messageQueue.LastModifyDate = DateTime.Now;
            messageQueue.CreateTime = DateTime.Now;
            this.genericMgr.Create(messageQueue);
        }
    }
}
