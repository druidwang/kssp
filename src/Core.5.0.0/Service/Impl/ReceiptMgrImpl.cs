using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INP;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SYS;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Entity.MSG;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.CUST;
using com.Sconit.Utility;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class ReceiptMgrImpl : BaseMgr, IReceiptMgr
    {
        #region 变量
        //private IPublishing proxy;
        //public IPubSubMgr pubSubMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public IInspectMgr inspectMgr { get; set; }
        public IHuMgr huMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        #endregion

        //public ReceiptMaster TransferOrder2Receipt(IList<OrderMaster> orderMasterList)
        //{
        //    #region 发货单头
        //    ReceiptMaster receiptMaster = new ReceiptMaster();

        //    #region 发货单类型
        //    //receiptMaster.Type = ReceiptMaster.TypeEnum.Normal;
        //    #endregion

        //    #region 订单类型
        //    var orderType = from om in orderMasterList select om.Type;
        //    if (orderType.Distinct().Count() > 1)
        //    {
        //        throw new BusinessErrorException("订单类型不同不能合并收货。");
        //    }
        //    receiptMaster.OrderType = orderType.Single();
        //    #endregion

        //    #region 订单质量类型
        //    var qualityType = from om in orderMasterList select om.QualityType;
        //    if (qualityType.Distinct().Count() > 1)
        //    {
        //        throw new BusinessErrorException("订单质量状态不同不能合并收货。");
        //    }
        //    receiptMaster.QualityType = qualityType.Single();
        //    #endregion

        //    #region PartyFrom
        //    var partyFrom = from om in orderMasterList select om.PartyFrom;
        //    if (partyFrom.Distinct().Count() > 1)
        //    {
        //        throw new BusinessErrorException("来源组织不同不能合并收货。");
        //    }
        //    receiptMaster.PartyFrom = partyFrom.Single();
        //    #endregion

        //    #region PartyFromName
        //    receiptMaster.PartyFromName = (from om in orderMasterList select om.PartyFromName).First();
        //    #endregion

        //    #region PartyTo
        //    var partyTo = from om in orderMasterList select om.PartyTo;
        //    if (partyTo.Distinct().Count() > 1)
        //    {
        //        throw new BusinessErrorException("目的组织不同不能合并收货。");
        //    }
        //    receiptMaster.PartyTo = partyTo.Single();
        //    #endregion

        //    #region PartyToName
        //    receiptMaster.PartyToName = (from om in orderMasterList select om.PartyToName).First();
        //    #endregion

        //    #region ShipFrom
        //    var shipFrom = from om in orderMasterList select om.ShipFrom;
        //    if (shipFrom.Distinct().Count() > 1)
        //    {
        //        throw new BusinessErrorException("发货地址不同不能合并收货。");
        //    }
        //    receiptMaster.ShipFrom = shipFrom.Single();
        //    #endregion

        //    #region ShipFromAddr
        //    receiptMaster.ShipFromAddress = (from om in orderMasterList select om.ShipFromAddress).First();
        //    #endregion

        //    #region ShipFromTel
        //    receiptMaster.ShipFromTel = (from om in orderMasterList select om.ShipFromTel).First();
        //    #endregion

        //    #region ShipFromCell
        //    receiptMaster.ShipFromCell = (from om in orderMasterList select om.ShipFromCell).First();
        //    #endregion

        //    #region ShipFromFax
        //    receiptMaster.ShipFromFax = (from om in orderMasterList select om.ShipFromFax).First();
        //    #endregion

        //    #region ShipFromContact
        //    receiptMaster.ShipFromContact = (from om in orderMasterList select om.ShipFromContact).First();
        //    #endregion

        //    #region ShipTo
        //    var shipTo = from om in orderMasterList select om.ShipTo;
        //    if (shipTo.Distinct().Count() > 1)
        //    {
        //        throw new BusinessErrorException("收货地址不同不能合并收货。");
        //    }
        //    receiptMaster.ShipTo = shipTo.Single();
        //    #endregion

        //    #region ShipToAddr
        //    receiptMaster.ShipToAddress = (from om in orderMasterList select om.ShipToAddress).First();
        //    #endregion

        //    #region ShipToTel
        //    receiptMaster.ShipToTel = (from om in orderMasterList select om.ShipToTel).First();
        //    #endregion

        //    #region ShipToCell
        //    receiptMaster.ShipToCell = (from om in orderMasterList select om.ShipToCell).First();
        //    #endregion

        //    #region ShipToFax
        //    receiptMaster.ShipToFax = (from om in orderMasterList select om.ShipToFax).First();
        //    #endregion

        //    #region ShipToContact
        //    receiptMaster.ShipToContact = (from om in orderMasterList select om.ShipToContact).First();
        //    #endregion

        //    #region Dock
        //    var dock = from om in orderMasterList select om.Dock;
        //    if (dock.Distinct().Count() > 1)
        //    {
        //        throw new BusinessErrorException("道口不同不能合并收货。");
        //    }
        //    receiptMaster.Dock = dock.Single();
        //    #endregion

        //    #region IsPrintRec
        //    receiptMaster.IsPrintReceipt = orderMasterList.Where(om => om.IsPrintReceipt == true) != null;
        //    #endregion

        //    #region IsCheckPartyFromAuth
        //    receiptMaster.IsCheckPartyFromAuthority = orderMasterList.Where(om => om.IsCheckPartyFromAuthority == true) != null;
        //    #endregion

        //    #region IsCheckPartyToAuth
        //    receiptMaster.IsCheckPartyToAuthority = orderMasterList.Where(om => om.IsCheckPartyToAuthority == true) != null;
        //    #endregion

        //    #region RecTemplate
        //    var recTemplate = orderMasterList.Select(om => om.ReceiptTemplate).First();
        //    receiptMaster.ReceiptTemplate = recTemplate;
        //    #endregion
        //    #endregion

        //    #region 发货单明细
        //    foreach (OrderMaster orderMaster in orderMasterList)
        //    {
        //        if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0)
        //        {
        //            foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
        //            {
        //                ReceiptDetail receiptDetail = new ReceiptDetail();
        //                Mapper.Map<OrderDetail, ReceiptDetail>(orderDetail, receiptDetail);
        //                //receiptDetail.EffectiveDate = orderMaster.EffectiveDate;
        //                foreach (OrderDetailInput orderDetailInput in orderDetail.OrderDetailInputs)
        //                {
        //                    ReceiptDetailInput receiptDetailInput = new ReceiptDetailInput();
        //                    receiptDetailInput.ReceiveQty = orderDetailInput.ReceiveQty;
        //                    //receiptDetailInput.RejectQty = orderDetailInput.RejectQty;
        //                    receiptDetailInput.HuId = orderDetailInput.HuId;
        //                    receiptDetailInput.LotNo = orderDetailInput.LotNo;

        //                    receiptDetail.AddReceiptDetailInput(receiptDetailInput);
        //                }

        //                receiptMaster.AddReceiptDetail(receiptDetail);
        //            }
        //        }
        //    }
        //    #endregion

        //    return receiptMaster;
        //}

        public ReceiptMaster TransferOrder2Receipt(OrderMaster orderMaster)
        {
            #region 发货单头
            ReceiptMaster receiptMaster = Mapper.Map<OrderMaster, ReceiptMaster>(orderMaster);
            receiptMaster.Type = com.Sconit.CodeMaster.IpDetailType.Normal;
            receiptMaster.CreateHuOption = orderMaster.CreateHuOption;
            #endregion

            #region 发货单明细
            if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0)
            {
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    ReceiptDetail receiptDetail = Mapper.Map<OrderDetail, ReceiptDetail>(orderDetail);
                    receiptDetail.Flow = orderMaster.Flow;

                    receiptDetail.IsInspect = orderMaster.IsInspect && orderDetail.IsInspect; //头和明细都选择报验才报验
                    //receiptDetail.EffectiveDate = orderMaster.EffectiveDate;
                    foreach (OrderDetailInput orderDetailInput in orderDetail.OrderDetailInputs)
                    {
                        ReceiptDetailInput receiptDetailInput = new ReceiptDetailInput();
                        receiptDetailInput.ReceiveQty = orderDetailInput.ReceiveQty;
                        receiptDetailInput.ScrapQty = orderDetailInput.ScrapQty;
                        //receiptDetailInput.RejectQty = orderDetailInput.RejectQty;
                        if (orderMaster.IsReceiveScanHu)
                        {
                            receiptDetailInput.HuId = orderDetailInput.HuId;
                            receiptDetailInput.LotNo = orderDetailInput.LotNo;
                        }
                        receiptDetailInput.OccupyType = orderDetailInput.OccupyType;
                        receiptDetailInput.OccupyReferenceNo = orderDetailInput.OccupyReferenceNo;
                        if (orderDetailInput.IpDetId != 0)
                        {
                            receiptDetail.IpNo = orderDetailInput.IpNo;
                            receiptDetail.IpDetailId = orderDetailInput.IpDetId;
                        }
                        receiptDetail.AddReceiptDetailInput(receiptDetailInput);
                    }

                    receiptMaster.AddReceiptDetail(receiptDetail);
                }
            }
            #endregion

            return receiptMaster;
        }

        public ReceiptMaster TransferIp2Receipt(IpMaster ipMaster)
        {
            ReceiptMaster receiptMaster = Mapper.Map<IpMaster, ReceiptMaster>(ipMaster);
            receiptMaster.Type = com.Sconit.CodeMaster.IpDetailType.Normal;
            //记录外部订单号
            if (string.IsNullOrWhiteSpace(receiptMaster.ExternalReceiptNo))
            {
                receiptMaster.ExternalReceiptNo = ipMaster.ExternalIpNo;
            }
            //记录WMSNo
            string WMSNo = string.Empty;
            foreach (IpDetail ipDetail in ipMaster.IpDetails)
            {
                if (ipDetail.IpDetailInputs.Select(i => i.WMSRecNo).Distinct().Count() > 1)
                {
                    throw new TechnicalException("WMS收货单号不一致。");
                }

                if (string.IsNullOrWhiteSpace(WMSNo))
                {
                    WMSNo = ipDetail.IpDetailInputs.First().WMSRecNo;
                }
                else if (WMSNo != ipDetail.IpDetailInputs.First().WMSRecNo)
                {
                    throw new TechnicalException("WMS收货单号不一致。");
                }
            }
            receiptMaster.WMSNo = WMSNo;

            foreach (IpDetail ipDetail in ipMaster.IpDetails)
            {
                ReceiptDetail receiptDetail = Mapper.Map<IpDetail, ReceiptDetail>(ipDetail);

                if (ipDetail.Id != 0)
                {
                    receiptDetail.IpDetailId = ipDetail.Id;
                    receiptDetail.IpNo = ipDetail.IpNo;
                    receiptDetail.IpDetailSequence = ipDetail.Sequence;
                    receiptDetail.IpDetailType = CodeMaster.IpDetailType.Normal;
                    receiptDetail.ExternalOrderNo = receiptMaster.ExternalReceiptNo;
                }

                foreach (IpDetailInput ipDetailInput in ipDetail.IpDetailInputs)
                {
                    ReceiptDetailInput receiptDetailInput = new ReceiptDetailInput();
                    receiptDetailInput.ReceiveQty = ipDetailInput.ReceiveQty;
                    receiptDetailInput.QualityType = ipDetail.QualityType;
                    //receiptDetailInput.RejectQty = ipDetailInput.RejectQty;
                    receiptDetailInput.HuId = ipDetailInput.HuId;
                    receiptDetailInput.LotNo = ipDetailInput.LotNo;
                    receiptDetailInput.IsCreatePlanBill = ipDetailInput.IsCreatePlanBill;
                    receiptDetailInput.IsConsignment = ipDetailInput.IsConsignment;
                    receiptDetailInput.PlanBill = ipDetailInput.PlanBill;
                    receiptDetailInput.ActingBill = ipDetailInput.ActingBill;
                    receiptDetailInput.IsFreeze = ipDetailInput.IsFreeze;
                    receiptDetailInput.IsATP = ipDetailInput.IsATP;
                    receiptDetailInput.OccupyType = ipDetailInput.OccupyType;
                    receiptDetailInput.OccupyReferenceNo = ipDetailInput.OccupyReferenceNo;
                    receiptDetailInput.SequenceNo = ipMaster.SequenceNo;
                    receiptDetailInput.WMSRecSeq = ipDetailInput.WMSRecSeq;
                    receiptDetailInput.ReceivedIpLocationDetailList = ipDetailInput.ReceivedIpLocationDetailList;

                    receiptDetail.AddReceiptDetailInput(receiptDetailInput);
                }

                receiptMaster.AddReceiptDetail(receiptDetail);
            }
            return receiptMaster;
        }

        public ReceiptMaster TransferIpGap2Receipt(IpMaster ipMaster, CodeMaster.IpGapAdjustOption ipGapAdjustOption)
        {
            ReceiptMaster receiptMaster = Mapper.Map<IpMaster, ReceiptMaster>(ipMaster);
            receiptMaster.Type = CodeMaster.IpDetailType.Gap;

            if (ipGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
            {
                receiptMaster.PartyFrom = ipMaster.PartyTo;
                receiptMaster.PartyFromName = ipMaster.PartyToName;
                receiptMaster.PartyTo = ipMaster.PartyFrom;
                receiptMaster.PartyToName = ipMaster.PartyFromName;
                receiptMaster.ShipFrom = ipMaster.ShipTo;
                receiptMaster.ShipFromAddress = ipMaster.ShipToAddress;
                receiptMaster.ShipFromTel = ipMaster.ShipToTel;
                receiptMaster.ShipFromCell = ipMaster.ShipToCell;
                receiptMaster.ShipFromFax = ipMaster.ShipToFax;
                receiptMaster.ShipFromContact = ipMaster.ShipToContact;
                receiptMaster.ShipTo = ipMaster.ShipFrom;
                receiptMaster.ShipToAddress = ipMaster.ShipFromAddress;
                receiptMaster.ShipToTel = ipMaster.ShipFromTel;
                receiptMaster.ShipToCell = ipMaster.ShipFromCell;
                receiptMaster.ShipToFax = ipMaster.ShipFromFax;
                receiptMaster.ShipToContact = ipMaster.ShipFromContact;
                receiptMaster.Dock = string.Empty;
            }

            foreach (IpDetail ipDetail in ipMaster.IpDetails)
            {
                ReceiptDetail receiptDetail = Mapper.Map<IpDetail, ReceiptDetail>(ipDetail);

                if (ipGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
                {
                    receiptDetail.LocationFrom = ipDetail.LocationTo;
                    receiptDetail.LocationFromName = ipDetail.LocationToName;
                    receiptDetail.LocationTo = ipDetail.LocationFrom;
                    receiptDetail.LocationToName = ipDetail.LocationFromName;
                    receiptDetail.IsInspect = false;
                }

                receiptDetail.IpDetailId = ipDetail.Id;
                receiptDetail.IpNo = ipDetail.IpNo;
                receiptDetail.IpDetailSequence = ipDetail.Sequence;
                receiptDetail.IpDetailType = ipDetail.Type;
                receiptDetail.IpGapAdjustOption = ipGapAdjustOption;

                foreach (IpDetailInput ipDetailInput in ipDetail.IpDetailInputs)
                {
                    ReceiptDetailInput receiptDetailInput = new ReceiptDetailInput();
                    receiptDetailInput.ReceiveQty = ipDetailInput.ReceiveQty;
                    receiptDetailInput.QualityType = ipDetail.QualityType;
                    //receiptDetailInput.RejectQty = ipDetailInput.RejectQty;
                    receiptDetailInput.HuId = ipDetailInput.HuId;
                    receiptDetailInput.LotNo = ipDetailInput.LotNo;
                    receiptDetailInput.IsCreatePlanBill = ipDetailInput.IsCreatePlanBill;
                    receiptDetailInput.IsConsignment = ipDetailInput.IsConsignment;
                    receiptDetailInput.PlanBill = ipDetailInput.PlanBill;
                    receiptDetailInput.ActingBill = ipDetailInput.ActingBill;
                    receiptDetailInput.IsFreeze = ipDetailInput.IsFreeze;
                    receiptDetailInput.IsATP = ipDetailInput.IsATP;
                    receiptDetailInput.OccupyType = ipDetailInput.OccupyType;
                    receiptDetailInput.OccupyReferenceNo = ipDetailInput.OccupyReferenceNo;
                    receiptDetailInput.SequenceNo = ipMaster.SequenceNo;
                    receiptDetailInput.ReceivedIpLocationDetailList = ipDetailInput.ReceivedIpLocationDetailList;

                    receiptDetail.AddReceiptDetailInput(receiptDetailInput);
                }

                receiptMaster.AddReceiptDetail(receiptDetail);
            }

            return receiptMaster;
        }

        public IpMaster TransferReceipt2Ip(ReceiptMaster receiptMaster)
        {
            IpMaster ipMaster = Mapper.Map<ReceiptMaster, IpMaster>(receiptMaster);

            foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
            {
                IpDetail ipDetail = Mapper.Map<ReceiptDetail, IpDetail>(receiptDetail);

                foreach (ReceiptLocationDetail receiptLocationDetail in receiptDetail.ReceiptLocationDetails)
                {
                    IpDetailInput ipDetailInput = new IpDetailInput();
                    ipDetailInput.ShipQty = receiptLocationDetail.Qty / receiptDetail.UnitQty;
                    ipDetailInput.HuId = receiptLocationDetail.HuId;
                    ipDetailInput.LotNo = receiptLocationDetail.LotNo;

                    ipDetail.AddIpDetailInput(ipDetailInput);
                }

                ipMaster.AddIpDetail(ipDetail);
            }
            return ipMaster;
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateReceipt(ReceiptMaster receiptMaster)
        {
            CreateReceipt(receiptMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateReceipt(ReceiptMaster receiptMaster, DateTime effectiveDate)
        {
            CreateReceipt(receiptMaster, false, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateReceipt(ReceiptMaster receiptMaster, bool isKit, DateTime effectiveDate)
        {
            if (!Utility.SecurityHelper.HasPermission(receiptMaster))
            {
                //throw new BusinessException("没有此收货单{0}的操作权限。", receiptMaster.ReceiptNo);
            }

            #region 收货明细不能为空
            if (receiptMaster.ReceiptDetails == null || receiptMaster.ReceiptDetails.Count == 0)
            {
                throw new BusinessException(Resources.ORD.ReceiptMaster.Errors_ReceiptDetailIsEmpty);
            }
            #endregion

            #region 保存收货单头
            receiptMaster.ReceiptNo = numberControlMgr.GetReceiptNo(receiptMaster);
            receiptMaster.EffectiveDate = effectiveDate;
            receiptMaster.Status = CodeMaster.ReceiptStatus.Close;
            this.genericMgr.Create(receiptMaster);
            #endregion

            #region 保存收货单明细
            int seqCount = 1;
            foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails.OrderBy(i => i.IpDetailSequence).ThenBy(i => i.OrderNo).ThenBy(i => i.OrderDetailSequence))
            {
                if (receiptDetail.ReceiveQtyInput < 0)
                {
                    //throw new BusinessException(Resources.SYS.ErrorMessage.Errors_NegativeExceptiond);
                }
                receiptDetail.ReceivedQty = receiptDetail.ReceiveQtyInput;
                receiptDetail.ScrapQty = receiptDetail.ScrapQtyInput;
                //receiptDetail.RejectedQty = receiptDetail.RejectQtyInput;
                receiptDetail.Sequence = seqCount++;
                receiptDetail.ReceiptNo = receiptMaster.ReceiptNo;

                this.genericMgr.Create(receiptDetail);
            }
            #endregion

            #region 收货创建条码
            //只有生产才支持收货创建条码，因为物流收货IpLocaitonLotDetail上会有寄售信息，也有可能一条收货记录对应多条IpLocaitonLotDetail，没有办法赋值HuId
            if ((receiptMaster.OrderType == CodeMaster.OrderType.Production)
                && receiptMaster.CreateHuOption == CodeMaster.CreateHuOption.Receive
                && receiptMaster.OrderSubType == CodeMaster.OrderSubType.Normal)
            {
                var flowMaster = receiptMaster.CurrentFlowMaster;
                if (flowMaster == null && !string.IsNullOrWhiteSpace(receiptMaster.Flow))
                {
                    flowMaster = this.genericMgr.FindById<FlowMaster>(receiptMaster.Flow);
                    receiptMaster.CurrentFlowMaster = flowMaster;
                }
                foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
                {
                    if (receiptDetail.ReceivedQty > 0)
                    {
                        if (flowMaster != null)
                        {
                            if (flowMaster.UcDeviation >= 0)
                            {
                                receiptDetail.MaxUc = receiptDetail.UnitCount * Convert.ToDecimal(((flowMaster.UcDeviation / 100) + 1));
                                receiptDetail.MinUc = receiptDetail.UnitCount * Convert.ToDecimal((1 - (flowMaster.UcDeviation / 100)));
                            }
                            else
                            {
                                receiptDetail.MaxUc = receiptDetail.ReceivedQty;
                                receiptDetail.MinUc = receiptDetail.ReceivedQty;
                            }
                        }
                        else
                        {
                            receiptDetail.MaxUc = receiptDetail.UnitCount;
                            receiptDetail.MinUc = receiptDetail.UnitCount;
                        }

                        IList<Hu> huList = huMgr.CreateHu(receiptMaster, receiptDetail, receiptMaster.StartTime);

                        receiptDetail.ReceiptDetailInputs = (from hu in huList
                                                             select new ReceiptDetailInput
                                                             {
                                                                 ReceiveQty = hu.Qty,   //转为订单单位
                                                                 QualityType = receiptDetail.QualityType,
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
            }
            #endregion

            #region 循环收货
            foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
            {
                if (receiptDetail.ReceivedQty != 0)
                {
                    receiptDetail.CurrentPartyFrom = receiptMaster.PartyFrom;  //为了记录库存事务
                    receiptDetail.CurrentPartyFromName = receiptMaster.PartyFromName;
                    receiptDetail.CurrentPartyTo = receiptMaster.PartyTo;
                    receiptDetail.CurrentPartyToName = receiptMaster.PartyToName;
                    receiptDetail.CurrentExternalReceiptNo = receiptMaster.ExternalReceiptNo;
                    receiptDetail.CurrentIsReceiveScanHu = receiptMaster.IsReceiveScanHu || isKit;
                    //inputIpDetail.CurrentIsATP = inputIpDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    //inputIpDetail.CurrentIsFreeze = false;              //默认只出库未冻结库存

                    IList<InventoryTransaction> inventoryTransactionList = this.locationDetailMgr.InventoryIn(receiptDetail, effectiveDate);

                    if (inventoryTransactionList != null && inventoryTransactionList.Count > 0)
                    {
                        IList<ReceiptLocationDetail> receiptLocationDetailList = (from trans in inventoryTransactionList
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
                                                                                      WMSSeq = trans.WMSRecSeq,
                                                                                  } into g
                                                                                  select new ReceiptLocationDetail
                                                                                  {
                                                                                      Item = receiptDetail.Item,
                                                                                      HuId = g.Key.HuId,
                                                                                      LotNo = g.Key.LotNo,
                                                                                      IsCreatePlanBill = g.Key.IsCreatePlanBill,
                                                                                      IsConsignment = g.Key.IsConsignment,
                                                                                      PlanBill = g.Key.PlanBill,
                                                                                      ActingBill = g.Key.ActingBill,
                                                                                      QualityType = receiptDetail.QualityType,
                                                                                      IsFreeze = g.Key.IsFreeze,
                                                                                      IsATP = g.Key.IsATP,
                                                                                      OccupyType = receiptDetail.CurrentOccupyType,
                                                                                      OccupyReferenceNo = receiptDetail.CurrentOccupyReferenceNo,
                                                                                      Qty = g.Sum(t => t.Qty),
                                                                                      WMSSeq = g.Key.WMSSeq
                                                                                  }).ToList();

                        receiptDetail.AddReceiptLocationDetail(receiptLocationDetailList);
                    }
                }
            }
            #endregion

            #region 保存发货单库存明细
            foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
            {
                if (receiptDetail.ReceiptLocationDetails != null && receiptDetail.ReceiptLocationDetails.Count > 0)
                {
                    foreach (ReceiptLocationDetail receiptLocationDetail in receiptDetail.ReceiptLocationDetails)
                    {
                        receiptLocationDetail.ReceiptNo = receiptMaster.ReceiptNo;
                        receiptLocationDetail.ReceiptDetailId = receiptDetail.Id;
                        receiptLocationDetail.OrderType = receiptDetail.OrderType;
                        receiptLocationDetail.OrderDetailId = receiptDetail.OrderDetailId;
                        genericMgr.Create(receiptLocationDetail);
                    }
                }
            }
            #endregion

            #region 检验
            InspectMaster inspectMaster = inspectMgr.TransferReceipt2Inspect(receiptMaster);
            if (inspectMaster != null && inspectMaster.InspectDetails != null && inspectMaster.InspectDetails.Count > 0)
            {
                inspectMgr.CreateInspectMaster(inspectMaster, effectiveDate);
            }
            #endregion

            #region 生产原材料回用,委外退货 增加原材料库存
            if ((receiptMaster.OrderType == CodeMaster.OrderType.Production || receiptMaster.OrderType == CodeMaster.OrderType.SubContract)
                && receiptMaster.OrderSubType == CodeMaster.OrderSubType.Return)
            {
                var backflushInputList = ReceiptMaster2BackflushInputList(receiptMaster);
                locationDetailMgr.BackflushProductMaterial(backflushInputList, effectiveDate);
                var orderBackflushDetailList = BackflushInputList2OrderBackflushDetailList(backflushInputList);
                DateTime dateTimeNow = DateTime.Now;
                User currentUser = SecurityContextHolder.Get();
                foreach (OrderBackflushDetail orderBackflushDetail in orderBackflushDetailList)
                {
                    orderBackflushDetail.EffectiveDate = effectiveDate;
                    orderBackflushDetail.CreateUserId = currentUser.Id;
                    orderBackflushDetail.CreateUserName = currentUser.FullName;
                    orderBackflushDetail.CreateDate = dateTimeNow;
                    orderBackflushDetail.IsVoid = true;
                    this.genericMgr.Create(orderBackflushDetail);
                }
            }
            #endregion

            this.AsyncSendPrintData(receiptMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelReceipt(string receiptNo)
        {
            ReceiptMaster receiptMaster = this.genericMgr.FindById<ReceiptMaster>(receiptNo);
            CancelReceipt(receiptMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelReceipt(string receiptNo, DateTime effectiveDate)
        {
            ReceiptMaster receiptMaster = this.genericMgr.FindById<ReceiptMaster>(receiptNo);
            CancelReceipt(receiptMaster, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelReceipt(ReceiptMaster receiptMaster)
        {
            CancelReceipt(receiptMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelReceipt(ReceiptMaster receiptMaster, DateTime effectiveDate)
        {
            if (!Utility.SecurityHelper.HasPermission(receiptMaster))
            {
                //throw new BusinessException("没有此收货单{0}的操作权限。", receiptMaster.ReceiptNo);
            }

            #region 判断收货单状态，只有Close才能冲销
            if (receiptMaster.Status == CodeMaster.ReceiptStatus.Cancel)
            {
                throw new BusinessException("收货单{0}已经冲销。", receiptMaster.ReceiptNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.ReceiptStatus, ((int)receiptMaster.Status).ToString()));
            }
            #endregion

            #region 加载收货单明细及收货单库存明细
            //  TryLoadReceiptDetails(receiptMaster);
            IList<ReceiptLocationDetail> receiptLocationDetailList = TryLoadReceiptLocationDetails(receiptMaster);
            #endregion

            #region 加载订单头和明细
            //GAP收货取消不用调整订单数据
            IList<OrderMaster> orderMasterList = null;
            IList<OrderDetail> orderDetialList = null;
            //if (receiptMaster.Type == CodeMaster.IpDetailType.Normal)
            //{
            #region 获取订单头
            orderMasterList = LoadOrderMasters(receiptMaster.ReceiptDetails.Select(det => det.OrderNo).Distinct().ToArray());
            #endregion

            #region 获取订单明细
            orderDetialList = LoadOrderDetails(receiptMaster.ReceiptDetails.Where(det => det.OrderDetailId.HasValue).Select(det => det.OrderDetailId.Value).Distinct().ToArray());
            #endregion
            //}
            #endregion

            //小数保留位数
            int decimalLength = int.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DecimalLength));

            #region 回滚送货单
            if (!string.IsNullOrWhiteSpace(receiptMaster.IpNo))
            {
                IpMaster ipMaster = this.genericMgr.FindById<IpMaster>(receiptMaster.IpNo);

                #region 查找送货单明细
                string selectIpDetailStatement = string.Empty;
                IList<object> selectIpDetailPram = new List<object>();
                foreach (int ipDetailId in receiptMaster.ReceiptDetails.Select(recDet => recDet.IpDetailId).Distinct())
                {
                    if (selectIpDetailStatement == string.Empty)
                    {
                        selectIpDetailStatement = "from IpDetail where Id in (?";
                    }
                    else
                    {
                        selectIpDetailStatement += ",?";
                    }

                    selectIpDetailPram.Add(ipDetailId);
                }
                selectIpDetailStatement += ")";
                IList<IpDetail> ipDetailList = this.genericMgr.FindAll<IpDetail>(selectIpDetailStatement, selectIpDetailPram.ToArray());
                #endregion

                #region 查找送货单库存明细
                IList<IpLocationDetail> ipLocationDetailList = LoadIpLocationDetails(ipDetailList.Select(ipDet => ipDet.Id).ToArray());
                #endregion

                #region 查找差异送货单明细
                IList<IpDetail> gapIpDetailList = this.genericMgr.FindAll<IpDetail>("from IpDetail where GapReceiptNo = ?", receiptMaster.ReceiptNo);
                #endregion

                #region 差异全部关闭
                if (gapIpDetailList != null && gapIpDetailList.Count > 0)
                {
                    #region 查找差异送货单库存明细
                    IList<IpLocationDetail> gapIpLocationDetailList = LoadIpLocationDetails(gapIpDetailList.Select(ipDet => ipDet.Id).ToArray());
                    #endregion

                    foreach (IpDetail gapIpDetail in gapIpDetailList)
                    {
                        if (gapIpDetail.ReceivedQty != 0)
                        {
                            throw new BusinessException("收货单{0}的收货差异已经调整，不能冲销。", receiptMaster.ReceiptNo);
                        }

                        gapIpDetail.IsClose = true;
                        this.genericMgr.Update(gapIpDetail);
                    }

                    foreach (IpLocationDetail gapIpLocationDetail in gapIpLocationDetailList)
                    {
                        gapIpLocationDetail.IsClose = true;
                        this.genericMgr.Update(gapIpLocationDetail);
                    }
                }
                #endregion

                #region 打开未收货的发货明细
                //只有正常收货才能打开未收货的发货明细。
                int isGapRec = receiptMaster.ReceiptDetails.Where(recDet => recDet.IpDetailType == CodeMaster.IpDetailType.Gap).Count();
                if (isGapRec == 0)
                {
                    IList<IpDetail> unReceivedIpDetailList = this.genericMgr.FindAll<IpDetail>("from IpDetail where IpNo = ? and Type = ?",
                        new object[] { ipMaster.IpNo, CodeMaster.IpDetailType.Normal });

                    if (unReceivedIpDetailList != null && unReceivedIpDetailList.Count > 0)
                    {
                        #region 查找差异送货单库存明细
                        IList<IpLocationDetail> unReceivedIpLocationDetailList = LoadIpLocationDetails(unReceivedIpDetailList.Select(ipDet => ipDet.Id).ToArray());
                        #endregion

                        foreach (IpDetail unReceivedIpDetail in unReceivedIpDetailList)
                        {
                            if (unReceivedIpDetail.IsClose && receiptMaster.ReceiptDetails.Select(p => p.IpDetailId).Contains(unReceivedIpDetail.Id))
                            {
                                unReceivedIpDetail.IsClose = false;
                                this.genericMgr.Update(unReceivedIpDetail);
                            }
                        }

                        //foreach (IpLocationDetail unReceivedIpLocationDetail in unReceivedIpLocationDetailList)
                        //{
                        //    if (unReceivedIpLocationDetail.IsClose && receiptMaster.ReceiptDetails.Select(p => p.IpDetailId).Contains(unReceivedIpLocationDetail.IpDetailId))
                        //    {
                        //        unReceivedIpLocationDetail.IsClose = false;
                        //        this.genericMgr.Update(unReceivedIpLocationDetail);
                        //    }
                        //}
                    }
                }
                #endregion

                #region 收货库存明细和送货库存明细匹配
                foreach (ReceiptLocationDetail receiptLocationDetail in receiptLocationDetailList)
                {
                    ReceiptDetail receiptDetail = receiptMaster.ReceiptDetails.Where(recDet => recDet.Id == receiptLocationDetail.ReceiptDetailId).Single();

                    decimal remainBaseQty = receiptLocationDetail.Qty;  //基本单位
                    decimal remainQty = receiptLocationDetail.Qty / receiptDetail.UnitQty; //转为订单单位

                    if (!string.IsNullOrWhiteSpace(receiptLocationDetail.HuId)
                        && ipLocationDetailList.Where(ipLocDet => !string.IsNullOrWhiteSpace(ipLocDet.HuId)).Count() > 0)
                    {
                        #region 条码和条码匹配
                        IpLocationDetail ipLocationDetail = ipLocationDetailList.Where(ipLocDet => ipLocDet.HuId == receiptLocationDetail.HuId).SingleOrDefault();
                        IpDetail ipDetail = ipDetailList.Where(ipDet => ipDet.Id == receiptDetail.IpDetailId).Single();

                        if (ipDetail.Id != receiptDetail.IpDetailId)
                        {
                            throw new TechnicalException("收货单明细和发货单明细ID不匹配。");
                        }

                        if (ipDetail.Type != receiptDetail.IpDetailType)
                        {
                            throw new TechnicalException("收货单明细和发货单明细类型不匹配。");
                        }

                        #region 扣减发货单的收货数
                        if (ipLocationDetail != null)
                        {
                            ipLocationDetail.ReceivedQty -= remainBaseQty;
                            ipLocationDetail.IsClose = false;
                        }
                        ipDetail.ReceivedQty -= remainQty; //转为订单单位
                        ipDetail.IsClose = false;

                        remainBaseQty = 0;
                        remainQty = 0;
                        #endregion

                        if (ipLocationDetail != null)
                        {
                            this.genericMgr.Update(ipLocationDetail);
                        }
                        this.genericMgr.Update(ipDetail);
                        #endregion
                    }
                    else
                    {
                        #region 按数量匹配
                        IList<IpDetail> thisIpDetailList = ipDetailList.Where(ipDet => ipDet.ReceivedQty != 0  //过滤掉已经扣完的明细
                                                                    && ipDet.Id == receiptDetail.IpDetailId).ToList();

                        if (thisIpDetailList != null && thisIpDetailList.Count > 0)
                        {
                            IList<IpLocationDetail> thisIpLocationDetailList = null;
                            foreach (IpDetail thisIpDetail in thisIpDetailList)
                            {
                                if (thisIpDetail.ReceivedQty > 0)
                                {
                                    thisIpLocationDetailList = ipLocationDetailList.Where(
                                        ipLocDet => ipLocDet.ReceivedQty > 0 && ipLocDet.IpDetailId == thisIpDetail.Id).ToList();
                                }
                                else if (thisIpDetail.ReceivedQty < 0)
                                {
                                    thisIpLocationDetailList = ipLocationDetailList.Where(
                                        ipLocDet => ipLocDet.ReceivedQty < 0 && ipLocDet.IpDetailId == thisIpDetail.Id).ToList();
                                }

                                if (thisIpLocationDetailList != null && thisIpLocationDetailList.Count > 0)
                                {
                                    foreach (IpLocationDetail thisIpLocationDetail in thisIpLocationDetailList)
                                    {
                                        if (thisIpLocationDetail.ReceivedQty > remainBaseQty)
                                        {
                                            //如果剩余冲销数量为0则不打开送货单
                                            if (remainBaseQty != 0)
                                            {
                                                thisIpDetail.IsClose = false;
                                                thisIpLocationDetail.IsClose = false;
                                            }
                                            thisIpDetail.ReceivedQty -= remainQty;
                                            remainQty = 0;

                                            thisIpLocationDetail.ReceivedQty -= remainBaseQty;
                                            remainBaseQty = 0;


                                        }
                                        else
                                        {
                                            decimal thisBackflushQty = thisIpLocationDetail.ReceivedQty / thisIpDetail.UnitQty; //转为订单单位
                                            remainQty -= thisBackflushQty;
                                            thisIpDetail.ReceivedQty -= thisBackflushQty;

                                            remainBaseQty -= thisIpLocationDetail.ReceivedQty;
                                            thisIpLocationDetail.ReceivedQty = 0;

                                            thisIpDetail.IsClose = false;
                                            thisIpLocationDetail.IsClose = false;
                                        }

                                        this.genericMgr.Update(thisIpLocationDetail);
                                        this.genericMgr.Update(thisIpDetail);
                                    }
                                }
                                else
                                {
                                    //throw new TechnicalException("差异送货单明细和送货单库存明细不匹配。");
                                }
                            }
                        }
                        #endregion
                    }

                    if (remainBaseQty != 0)
                    {
                        throw new TechnicalException("收货单的收货数没有回冲完。");
                    }
                }
                #endregion

                #region 更新订单明细
                if (receiptMaster.OrderType != CodeMaster.OrderType.ScheduleLine)
                {
                    #region 非计划协议
                    foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
                    {
                        OrderDetail matchedOrderDetial = orderDetialList.Where(det => det.Id == receiptDetail.OrderDetailId.Value).Single();

                        if (receiptDetail.IpDetailType == CodeMaster.IpDetailType.Normal)
                        {
                            matchedOrderDetial.ReceivedQty -= receiptDetail.ReceivedQty;
                            if (matchedOrderDetial.ReceivedQty < 0)
                            {
                                throw new TechnicalException("订单收货数小于0。");
                            }
                        }
                        else
                        {
                            //差异收货冲销
                            #region 调整发货方库存
                            if (receiptMaster.PartyFrom == ipMaster.PartyTo)   //发货方等于收货方
                            {
                                //更新订单的发货数
                                matchedOrderDetial.ShippedQty += receiptDetail.ReceivedQty;
                            }
                            #endregion

                            #region 调整收货方库存
                            else
                            {
                                //更新订单的收货数
                                matchedOrderDetial.ReceivedQty -= receiptDetail.ReceivedQty;
                            }
                            #endregion
                        }
                        genericMgr.Update(matchedOrderDetial);
                    }
                    #endregion
                }
                else
                {
                    foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
                    {
                        decimal receivedQty = receiptDetail.ReceivedQty;

                        if (receiptDetail.IpDetailType == CodeMaster.IpDetailType.Normal)
                        {
                            #region 调整收货方库存
                            IList<OrderDetail> scheduleOrderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>("select * from ORD_OrderDet_8 where ExtNo = ? and ExtSeq like ? and ScheduleType = ? and RecQty > 0 order by EndDate desc",
                                              new object[] { receiptDetail.ExternalOrderNo, receiptDetail.ExternalSequence + "-%", CodeMaster.ScheduleType.Firm });

                            foreach (OrderDetail scheduleOrderDetail in scheduleOrderDetailList)
                            {
                                if (receivedQty > (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty))
                                {
                                    receivedQty -= scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty;
                                    scheduleOrderDetail.ReceivedQty -= receiptDetail.ReceivedQty;
                                }
                                else
                                {
                                    scheduleOrderDetail.ReceivedQty -= receivedQty;
                                    receivedQty = 0;
                                }

                                genericMgr.Update(scheduleOrderDetail);
                            }
                            #endregion
                        }
                        else
                        {
                            //差异收货冲销
                            #region 调整发货方库存
                            if (receiptMaster.PartyFrom == ipMaster.PartyTo)   //发货方等于收货方
                            {
                                IList<OrderDetail> scheduleOrderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>("select * from ORD_OrderDet_8 where ExtNo = ? and ExtSeq like ? and ScheduleType = ? and OrderQty > ShipQty order by EndDate",
                                              new object[] { receiptDetail.ExternalOrderNo, receiptDetail.ExternalSequence + "-%", CodeMaster.ScheduleType.Firm });

                                foreach (OrderDetail scheduleOrderDetail in scheduleOrderDetailList)
                                {
                                    if (receivedQty > (scheduleOrderDetail.OrderedQty - scheduleOrderDetail.ShippedQty))
                                    {
                                        receivedQty -= scheduleOrderDetail.OrderedQty - scheduleOrderDetail.ShippedQty;
                                        scheduleOrderDetail.ShippedQty = scheduleOrderDetail.OrderedQty;
                                    }
                                    else
                                    {
                                        scheduleOrderDetail.ShippedQty += receivedQty;
                                        receivedQty = 0;
                                    }

                                    genericMgr.Update(scheduleOrderDetail);
                                }
                            }
                            #endregion

                            #region 调整收货方库存
                            else
                            {
                                IList<OrderDetail> scheduleOrderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>("select * from ORD_OrderDet_8 where ExtNo = ? and ExtSeq like ? and ScheduleType = ? and RecQty > 0 order by EndDate desc",
                                              new object[] { receiptDetail.ExternalOrderNo, receiptDetail.ExternalSequence + "-%", CodeMaster.ScheduleType.Firm });

                                foreach (OrderDetail scheduleOrderDetail in scheduleOrderDetailList)
                                {
                                    if (receivedQty > (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty))
                                    {
                                        receivedQty -= scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty;
                                        scheduleOrderDetail.ReceivedQty -= receiptDetail.ReceivedQty;
                                    }
                                    else
                                    {
                                        scheduleOrderDetail.ReceivedQty -= receivedQty;
                                        receivedQty = 0;
                                    }

                                    genericMgr.Update(scheduleOrderDetail);
                                }
                            }
                            #endregion
                        }
                    }
                }
                #endregion

                #region 回滚送货单状态
                //1. 普通状态的送货明细没有收过货
                //2. 差异状态的送货明细全部关闭
                if (ipDetailList.Where(ipDet => ipDet.Type == CodeMaster.IpDetailType.Normal && ipDet.ReceivedQty != 0).Count() == 0
                    && ipDetailList.Where(ipDet => ipDet.Type == CodeMaster.IpDetailType.Gap && !ipDet.IsClose).Count() == 0)
                {
                    ipMaster.Status = CodeMaster.IpStatus.Submit;
                    this.genericMgr.Update(ipMaster);

                    #region 回滚排序单状态
                    if (!string.IsNullOrWhiteSpace(ipMaster.SequenceNo))
                    {
                        SequenceMaster sequenceMaster = this.genericMgr.FindById<SequenceMaster>(ipMaster.SequenceNo);
                        sequenceMaster.Status = CodeMaster.SequenceStatus.Ship;
                        this.genericMgr.Update(sequenceMaster);
                    }
                    #endregion
                }
                else if (ipMaster.Status != CodeMaster.IpStatus.InProcess)
                {
                    ipMaster.Status = CodeMaster.IpStatus.InProcess;
                    this.genericMgr.Update(ipMaster);
                }
                #endregion

                #region 退回排序单状态
                if (!string.IsNullOrEmpty(ipMaster.SequenceNo))
                {
                    #region 更新排序单头
                    SequenceMaster sequenceMaster = this.genericMgr.FindById<SequenceMaster>(ipMaster.SequenceNo);

                    sequenceMaster.Status = CodeMaster.SequenceStatus.Ship;
                    sequenceMaster.CloseDate = null;
                    sequenceMaster.CloseUserId = 0;
                    sequenceMaster.CloseUserName = null;

                    this.genericMgr.Update(sequenceMaster);
                    #endregion

                    foreach (SequenceDetail sequenceDetail in TryLoadSequenceDetails(sequenceMaster))
                    {
                        //对于调整计划关闭的排序明细也有可能打开了，会有Bug
                        if (orderDetialList.Select(i => i.Id).Contains(sequenceDetail.OrderDetailId))
                        {
                            sequenceDetail.IsClose = false;
                            this.genericMgr.Update(sequenceDetail);
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region 更新订单明细
                foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
                {
                    if (receiptDetail != null)
                    {
                        OrderDetail matchedOrderDetial = orderDetialList.Where(det => det.Id == receiptDetail.OrderDetailId.Value).Single();
                        matchedOrderDetial.ReceivedQty -= receiptDetail.ReceivedQty;
                        matchedOrderDetial.ScrapQty -= receiptDetail.ScrapQty;
                        if (matchedOrderDetial.OrderType != CodeMaster.OrderType.Production
                           && matchedOrderDetial.OrderType != CodeMaster.OrderType.SubContract)
                        {
                            matchedOrderDetial.ShippedQty -= receiptDetail.ReceivedQty;
                            if (matchedOrderDetial.ShippedQty < 0)
                            {
                                throw new TechnicalException("订单发货数小于0。");
                            }
                        }

                        if (matchedOrderDetial.ReceivedQty < 0)
                        {
                            throw new TechnicalException("订单收货数小于0。");
                        }

                        this.genericMgr.Update(matchedOrderDetial);
                    }
                }
                #endregion
            }
            #endregion

            #region 更新订单
            foreach (OrderMaster orderMaster in orderMasterList)
            {
                if (orderMaster.Status != CodeMaster.OrderStatus.InProcess)
                {
                    orderMaster.Status = CodeMaster.OrderStatus.InProcess;
                    orderMaster.CloseDate = null;
                    orderMaster.CloseUserId = null;
                    orderMaster.CloseUserName = null;
                    this.genericMgr.Update(orderMaster);
                }
            }
            #endregion

            #region 更新收货单
            receiptMaster.Status = CodeMaster.ReceiptStatus.Cancel;
            this.genericMgr.Update(receiptMaster);
            #endregion

            #region 冲销收货记录
            List<IpDetail> cancelIpDetailList = new List<IpDetail>();
            foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
            {
                if (receiptDetail.ReceivedQty != 0)
                {
                    receiptDetail.CurrentPartyFrom = receiptMaster.PartyFrom;  //为了记录库存事务
                    receiptDetail.CurrentPartyFromName = receiptMaster.PartyFromName;
                    receiptDetail.CurrentPartyTo = receiptMaster.PartyTo;
                    receiptDetail.CurrentPartyToName = receiptMaster.PartyToName;
                    receiptDetail.CurrentExternalReceiptNo = receiptMaster.ExternalReceiptNo;
                    receiptDetail.CurrentIsReceiveScanHu = receiptMaster.IsReceiveScanHu || orderMasterList.Where(o => o.OrderStrategy == CodeMaster.FlowStrategy.KIT).Count() > 0;
                    receiptDetail.IsVoid = true;

                    foreach (ReceiptLocationDetail receiptLocationDetail in receiptDetail.ReceiptLocationDetails)
                    {
                        ReceiptDetailInput receiptDetailInput = new ReceiptDetailInput();
                        receiptDetailInput.HuId = receiptLocationDetail.HuId;
                        receiptDetailInput.ReceiveQty = -receiptLocationDetail.Qty / receiptDetail.UnitQty; //转为订单单位
                        receiptDetailInput.LotNo = receiptLocationDetail.LotNo;
                        receiptDetailInput.IsCreatePlanBill = receiptLocationDetail.IsCreatePlanBill;
                        receiptDetailInput.IsConsignment = receiptLocationDetail.IsConsignment;
                        receiptDetailInput.PlanBill = receiptLocationDetail.PlanBill;
                        receiptDetailInput.ActingBill = receiptLocationDetail.ActingBill;
                        receiptDetailInput.IsATP = receiptLocationDetail.IsATP;
                        receiptDetailInput.IsFreeze = receiptLocationDetail.IsFreeze;
                        receiptDetailInput.OccupyType = receiptLocationDetail.OccupyType;
                        receiptDetailInput.OccupyReferenceNo = receiptLocationDetail.OccupyReferenceNo;
                        receiptDetailInput.QualityType = receiptLocationDetail.QualityType;

                        receiptDetail.AddReceiptDetailInput(receiptDetailInput);
                    }

                    #region 更新库存、记库存事务
                    IList<InventoryTransaction> rctInventoryTransactionList = this.locationDetailMgr.InventoryIn(receiptDetail, effectiveDate);
                    #endregion

                    #region 订单直接收货创建发货明细对象
                    if (!string.IsNullOrWhiteSpace(receiptMaster.IpNo))
                    {
                        //nothing todo 
                    }
                    else if (receiptMaster.Type == CodeMaster.IpDetailType.Gap)
                    {
                        //nothing todo 
                    }
                    else if (receiptMaster.OrderType == CodeMaster.OrderType.Production)
                    {
                        //nothing todo 
                    }
                    else if (receiptMaster.OrderType == CodeMaster.OrderType.SubContract && receiptMaster.OrderSubType == CodeMaster.OrderSubType.Normal)
                    {
                        //nothing todo 
                    }
                    else
                    //if (string.IsNullOrWhiteSpace(receiptMaster.IpNo)
                    //    && receiptMaster.Type != CodeMaster.IpDetailType.Gap
                    //    && receiptMaster.OrderType != CodeMaster.OrderType.Production
                    //    && (receiptMaster.OrderType != CodeMaster.OrderType.SubContract))
                    {
                        IpDetail ipdetail = new IpDetail();
                        ipdetail.OrderNo = receiptDetail.OrderNo;
                        ipdetail.OrderType = receiptDetail.OrderType;
                        ipdetail.OrderSubType = receiptDetail.OrderSubType;
                        ipdetail.OrderDetailId = receiptDetail.OrderDetailId;
                        ipdetail.OrderDetailSequence = receiptDetail.OrderDetailSequence;
                        ipdetail.Item = receiptDetail.Item;
                        ipdetail.ItemDescription = receiptDetail.ItemDescription;
                        ipdetail.ReferenceItemCode = receiptDetail.ReferenceItemCode;
                        ipdetail.BaseUom = receiptDetail.BaseUom;
                        ipdetail.Uom = receiptDetail.Uom;
                        ipdetail.UnitCount = receiptDetail.UnitCount;
                        //ipdetail.UnitCountDescription = receiptDetail.UnitCountDescription;
                        //ipdetail.Container = receiptDetail.Container;
                        //ipdetail.ContainerDescription = receiptDetail.ContainerDescription;
                        ipdetail.QualityType = receiptDetail.QualityType;
                        //ipdetail.ManufactureParty = receiptDetail.ManufactureParty;
                        if ((ipdetail.OrderType == CodeMaster.OrderType.Procurement || ipdetail.OrderType == CodeMaster.OrderType.SubContract) && ipdetail.OrderSubType == CodeMaster.OrderSubType.Return)
                        {
                            //退货的订单需要入库y
                            ipdetail.Qty = receiptDetail.ReceivedQty;
                        }
                        else
                        {
                            ipdetail.Qty = -receiptDetail.ReceivedQty;
                        }
                        //ipdetail.ReceivedQty = 
                        ipdetail.UnitQty = receiptDetail.UnitQty;
                        ipdetail.LocationFrom = receiptDetail.LocationFrom;
                        ipdetail.LocationFromName = receiptDetail.LocationFromName;
                        ipdetail.LocationTo = receiptDetail.LocationTo;
                        ipdetail.LocationToName = receiptDetail.LocationToName;
                        ipdetail.IsInspect = false;
                        //ipdetail.BillTerm = receiptDetail.BillTerm;
                        ipdetail.IsVoid = true;

                        cancelIpDetailList.Add(ipdetail);

                        foreach (InventoryTransaction inventoryTransaction in rctInventoryTransactionList)
                        {
                            IpDetailInput ipDetailInput = new IpDetailInput();

                            ipDetailInput.HuId = inventoryTransaction.HuId;
                            if ((ipdetail.OrderType == CodeMaster.OrderType.Procurement || ipdetail.OrderType == CodeMaster.OrderType.SubContract) && ipdetail.OrderSubType == CodeMaster.OrderSubType.Return)
                            {
                                ipDetailInput.ShipQty = -inventoryTransaction.Qty / ipdetail.UnitQty;
                            }
                            else
                            {
                                ipDetailInput.ShipQty = inventoryTransaction.Qty / ipdetail.UnitQty;
                            }
                            ipDetailInput.LotNo = inventoryTransaction.LotNo;
                            ipDetailInput.IsCreatePlanBill = inventoryTransaction.IsCreatePlanBill;
                            if (inventoryTransaction.ActingBill.HasValue)
                            {
                                int planBill = this.genericMgr.FindAllWithNativeSql<Int32>("select PlanBill from BIL_ActBill where Id = ?", inventoryTransaction.ActingBill.Value).Single();
                                ipDetailInput.IsConsignment = true;
                                ipDetailInput.PlanBill = planBill;
                                ipDetailInput.ActingBill = null;
                            }
                            else
                            {
                                ipDetailInput.IsConsignment = inventoryTransaction.IsConsignment;
                                ipDetailInput.PlanBill = inventoryTransaction.PlanBill;
                                ipDetailInput.ActingBill = inventoryTransaction.ActingBill;
                            }
                            ipDetailInput.OccupyType = inventoryTransaction.OccupyType;
                            ipDetailInput.OccupyReferenceNo = inventoryTransaction.OccupyReferenceNo;

                            ipdetail.AddIpDetailInput(ipDetailInput);
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region 订单直接收货，冲销发货记录
            if (cancelIpDetailList != null && cancelIpDetailList.Count > 0)
            {
                foreach (IpDetail cancelIpDetail in cancelIpDetailList)
                {
                    cancelIpDetail.CurrentPartyFrom = receiptMaster.PartyFrom;  //为了记录库存事务
                    cancelIpDetail.CurrentPartyFromName = receiptMaster.PartyFromName;  //为了记录库存事务
                    cancelIpDetail.CurrentPartyTo = receiptMaster.PartyTo;      //为了记录库存事务
                    cancelIpDetail.CurrentPartyToName = receiptMaster.PartyToName;      //为了记录库存事务

                    this.locationDetailMgr.InventoryOut(cancelIpDetail);
                }
            }
            #endregion

            #region 生产单,委外冲销退回原材料
            if (receiptMaster.OrderType == CodeMaster.OrderType.Production ||
                receiptMaster.OrderType == CodeMaster.OrderType.SubContract)
            {
                var backflushInputList = ReceiptMaster2BackflushInputList(receiptMaster);
                locationDetailMgr.CancelBackflushProductMaterial(backflushInputList, effectiveDate);
                var orderBackflushDetailList = BackflushInputList2OrderBackflushDetailList(backflushInputList);
                DateTime dateTimeNow = DateTime.Now;
                User currentUser = SecurityContextHolder.Get();
                foreach (OrderBackflushDetail orderBackflushDetail in orderBackflushDetailList)
                {
                    orderBackflushDetail.EffectiveDate = effectiveDate;
                    orderBackflushDetail.CreateUserId = currentUser.Id;
                    orderBackflushDetail.CreateUserName = currentUser.FullName;
                    orderBackflushDetail.CreateDate = dateTimeNow;
                    orderBackflushDetail.IsVoid = true;
                    this.genericMgr.Create(orderBackflushDetail);
                }
            }
            #endregion
        }

        private List<BackflushInput> ReceiptMaster2BackflushInputList(ReceiptMaster receiptMaster)
        {
            List<BackflushInput> backflushInputList = new List<BackflushInput>();
            if (receiptMaster.CurrentFlowMaster == null && !string.IsNullOrWhiteSpace(receiptMaster.Flow))
            {
                receiptMaster.CurrentFlowMaster = this.genericMgr.FindById<FlowMaster>(receiptMaster.Flow);
            }

            IList<ReceiptDetail> receiptDetailList = receiptMaster.ReceiptDetails;

            var allOrderBackFlushDetail = this.genericMgr.FindAllIn<OrderBackflushDetail>(
                "from OrderBackflushDetail where ReceiptDetailId in(?", receiptDetailList.Select(p => (object)p.Id));
            var allOrderBackFlushDetailDic = allOrderBackFlushDetail.GroupBy(p => p.ReceiptDetailId, (k, g) => new { k, g })
                .ToDictionary(d => d.k, d => d.g.ToList());
            var allOrderBomDetailDic = this.genericMgr.FindAllIn<OrderBomDetail>
             (" from OrderBomDetail where OrderDetailId in(? ", receiptDetailList.Select(p => (object)p.OrderDetailId))
             .GroupBy(p => p.OrderDetailId, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.ToList());
            //委外退货要根据Bom来计算消耗用量
            if (!(receiptMaster.OrderType == Sconit.CodeMaster.OrderType.SubContract && receiptMaster.OrderSubType == Sconit.CodeMaster.OrderSubType.Return))
            {
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    var orderBackFlushDetailList = allOrderBackFlushDetailDic.ValueOrDefault(receiptDetail.Id);
                    if (orderBackFlushDetailList != null)
                    {
                        var backflushInputs = from bfd in orderBackFlushDetailList
                                              where bfd.BackflushedQty != 0
                                              select new BackflushInput
                                              {
                                                  OrderNo = receiptDetail.OrderNo,
                                                  OrderType = receiptDetail.OrderType,
                                                  OrderSubType = receiptDetail.OrderSubType,
                                                  OrderDetailSequence = bfd.OrderDetailSequence,
                                                  OrderDetailId = bfd.OrderDetailId,
                                                  OrderBomDetail = new OrderBomDetail()
                                                  {
                                                      Id = bfd.OrderBomDetailId.Value,
                                                      Sequence = bfd.OrderBomDetailSequence.Value,
                                                      ManufactureParty = bfd.ManufactureParty,
                                                      Location = bfd.LocationFrom,
                                                      Bom = bfd.Bom,
                                                      Item = bfd.Item,
                                                      ReferenceItemCode = bfd.ReferenceItemCode,
                                                      ItemDescription = bfd.ItemDescription,
                                                      Uom = bfd.Uom,
                                                      BaseUom = bfd.BaseUom,
                                                      Operation = bfd.Operation.HasValue ? bfd.Operation.Value : 0,
                                                      OpReference = bfd.OpReference,
                                                      //OrderedQty = bfd.OrderedQty,
                                                      UnitQty = bfd.UnitQty,
                                                      //BomUnitQty = bfd.BomUnitQty,
                                                      IsPrint = false,
                                                      BackFlushMethod = CodeMaster.BackFlushMethod.GoodsReceive,
                                                      FeedMethod = CodeMaster.FeedMethod.None,
                                                      //IsScanHu = bfd.IsScanHu,
                                                      IsAutoFeed = false,
                                                      //EstimateConsumeTime = bfd.EstimateConsumeTime,
                                                      ReserveNo = bfd.ReserveNo,
                                                      ReserveLine = bfd.ReserveLine,
                                                      //ZOPWZ = bfd.ZOPWZ,
                                                      //ZOPID = bfd.ZOPID,
                                                      //ZOPDS = bfd.ZOPDS,
                                                      ICHARG = bfd.ICHARG,
                                                      BWART = bfd.BWART,

                                                  },
                                                  ReceiptNo = receiptDetail.ReceiptNo,
                                                  ReceiptDetailId = receiptDetail.Id,
                                                  ReceiptDetailSequence = receiptDetail.Sequence,
                                                  //TraceCode = receiptDetail.TraceCode,
                                                  Item = bfd.Item,
                                                  ItemDescription = bfd.ItemDescription,
                                                  ReferenceItemCode = bfd.ReferenceItemCode,
                                                  Uom = bfd.Uom,
                                                  BaseUom = bfd.BaseUom,
                                                  Qty = -(bfd.BackflushedQty + bfd.BackflushedRejectQty),
                                                  UnitQty = bfd.UnitQty,
                                                  Location = string.IsNullOrWhiteSpace(bfd.LocationFrom) ? receiptDetail.LocationFrom : bfd.LocationFrom,
                                                  CurrentProductLine = receiptMaster.CurrentFlowMaster,
                                                  ProductLine = receiptMaster.CurrentFlowMaster.Code,
                                                  FGItem = receiptDetail.Item,
                                                  FGQualityType = receiptDetail.QualityType,
                                                  Operation = bfd.Operation,
                                                  OpReference = bfd.OpReference,
                                                  //HuId = receiptDetail.ReceiptDetailInputs.First().HuId
                                              };
                        backflushInputList.AddRange(backflushInputs);
                    }
                }
            }
            else
            {
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    var orderBomDetailList = allOrderBomDetailDic.ValueOrDefault(receiptDetail.OrderDetailId.Value);
                    if (orderBomDetailList != null)
                    {
                        var backflushInputs = from bom in orderBomDetailList
                                              where bom.OrderedQty != 0
                                              select new BackflushInput
                                              {
                                                  OrderNo = receiptDetail.OrderNo,
                                                  OrderType = receiptDetail.OrderType,
                                                  OrderSubType = receiptDetail.OrderSubType,
                                                  OrderDetailSequence = bom.OrderDetailSequence,
                                                  OrderDetailId = bom.OrderDetailId,
                                                  OrderBomDetail = bom,
                                                  ReceiptNo = receiptDetail.ReceiptNo,
                                                  ReceiptDetailId = receiptDetail.Id,
                                                  ReceiptDetailSequence = receiptDetail.Sequence,
                                                  //TraceCode = receiptDetail.TraceCode,
                                                  Item = bom.Item,
                                                  ItemDescription = bom.ItemDescription,
                                                  ReferenceItemCode = bom.ReferenceItemCode,
                                                  Uom = bom.Uom,
                                                  BaseUom = bom.BaseUom,
                                                  Qty = bom.BomUnitQty * (receiptDetail.ReceivedQty + receiptDetail.ScrapQty),
                                                  UnitQty = bom.UnitQty,
                                                  Location = string.IsNullOrWhiteSpace(bom.Location) ? receiptDetail.LocationFrom : bom.Location,
                                                  CurrentProductLine = receiptMaster.CurrentFlowMaster,
                                                  ProductLine = receiptMaster.CurrentFlowMaster.Code,
                                                  FGItem = receiptDetail.Item,
                                                  FGQualityType = receiptDetail.QualityType,
                                                  Operation = bom.Operation,
                                                  OpReference = bom.OpReference,
                                                  //HuId = receiptDetail.ReceiptDetailInputs.First().HuId
                                              };
                        backflushInputList.AddRange(backflushInputs);
                    }
                }
            }
            //var allOrderBomDetailDic = this.genericMgr.FindAllIn<OrderBomDetail>
            // (" from OrderBomDetail where OrderDetailId in(? ", receiptDetailList.Select(p => (object)p.OrderDetailId))
            // .GroupBy(p => p.OrderDetailId, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g.ToList());

            //foreach (ReceiptDetail receiptDetail in receiptDetailList)
            //{
            //    var orderBomDetailList = allOrderBomDetailDic.ValueOrDefault(receiptDetail.OrderDetailId.Value);
            //    if (orderBomDetailList != null)
            //    {
            //        var backflushInputs = from bom in orderBomDetailList
            //                              where bom.OrderedQty != 0
            //                              select new BackflushInput
            //                              {
            //                                  OrderNo = receiptDetail.OrderNo,
            //                                  OrderType = receiptDetail.OrderType,
            //                                  OrderSubType = receiptDetail.OrderSubType,
            //                                  OrderDetailSequence = bom.OrderDetailSequence,
            //                                  OrderDetailId = bom.OrderDetailId,
            //                                  OrderBomDetail = bom,
            //                                  ReceiptNo = receiptDetail.ReceiptNo,
            //                                  ReceiptDetailId = receiptDetail.Id,
            //                                  ReceiptDetailSequence = receiptDetail.Sequence,
            //                                  //TraceCode = receiptDetail.TraceCode,
            //                                  Item = bom.Item,
            //                                  ItemDescription = bom.ItemDescription,
            //                                  ReferenceItemCode = bom.ReferenceItemCode,
            //                                  Uom = bom.Uom,
            //                                  BaseUom = bom.BaseUom,
            //                                  Qty = bom.BomUnitQty * (receiptDetail.ReceivedQty + receiptDetail.ScrapQty),
            //                                  UnitQty = bom.UnitQty,
            //                                  Location = string.IsNullOrWhiteSpace(bom.Location) ? receiptDetail.LocationFrom : bom.Location,
            //                                  CurrentProductLine = receiptMaster.CurrentFlowMaster,
            //                                  ProductLine = receiptMaster.CurrentFlowMaster.Code,
            //                                  FGItem = receiptDetail.Item,
            //                                  FGQualityType = receiptDetail.QualityType,
            //                                  Operation = bom.Operation,
            //                                  OpReference = bom.OpReference,
            //                                  //HuId = receiptDetail.ReceiptDetailInputs.First().HuId
            //                              };
            //        backflushInputList.AddRange(backflushInputs);
            //    }
            //}
            return backflushInputList;
        }

        private List<OrderBackflushDetail> BackflushInputList2OrderBackflushDetailList(IList<BackflushInput> backflushInputList)
        {
            List<OrderBackflushDetail> orderBackflushDetailList = new List<OrderBackflushDetail>();
            foreach (var backflushInput in backflushInputList)
            {
                var orderBackflushDetails = from trans in backflushInput.InventoryTransactionList
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
                                                Bom = backflushInput.OrderBomDetail.Bom,
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
                                                //LotNo = result.Key.LotNo,
                                                Operation = backflushInput.Operation,
                                                OpReference = backflushInput.OpReference,
                                                BackflushedQty = backflushInput.FGQualityType == CodeMaster.QualityType.Qualified ? g.Sum(trans => trans.Qty) / backflushInput.UnitQty : 0,   //根据收货成品的质量状态记录至不同的回冲数量中
                                                BackflushedRejectQty = backflushInput.FGQualityType == CodeMaster.QualityType.Reject ? g.Sum(trans => trans.Qty) / backflushInput.UnitQty : 0,
                                                //BackflushedScrapQty = input.BackflushedQty,
                                                LocationFrom = backflushInput.OrderBomDetail.Location,
                                                ProductLine = backflushInput.ProductLine,
                                                ProductLineFacility = backflushInput.ProductLineFacility,
                                                PlanBill = g.Key,
                                                //EffectiveDate = effectiveDate,
                                                //CreateUserId = currentUser.Id,
                                                //CreateUserName = currentUser.FullName,
                                                //CreateDate = dateTimeNow,
                                                //IsVoid = isVoid,
                                            };
                orderBackflushDetailList.AddRange(orderBackflushDetails);
            }
            return orderBackflushDetailList;
        }

        private IList<ReceiptDetail> TryLoadReceiptDetails(ReceiptMaster receiptMaster)
        {
            if (receiptMaster.ReceiptDetails == null)
            {
                string hql = "from ReceiptDetail where ReceiptNo = ?";
                receiptMaster.ReceiptDetails = this.genericMgr.FindAll<ReceiptDetail>(hql, receiptMaster.ReceiptNo);
            }

            return receiptMaster.ReceiptDetails;
        }

        private IList<ReceiptLocationDetail> TryLoadReceiptLocationDetails(ReceiptMaster receiptMaster)
        {
            if (receiptMaster.ReceiptDetails == null)
            {
                if (receiptMaster.ReceiptDetails == null)
                {
                    TryLoadReceiptDetails(receiptMaster);
                }

                string hql = "from ReceiptLocationDetail where ReceiptNo = ?";
                IList<ReceiptLocationDetail> receiptLocationDetailList = this.genericMgr.FindAll<ReceiptLocationDetail>(hql, receiptMaster.ReceiptNo);

                foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
                {
                    receiptDetail.ReceiptLocationDetails = receiptLocationDetailList.Where(r => r.ReceiptDetailId == receiptDetail.Id).ToList();
                }

                return receiptLocationDetailList;
            }
            else
            {
                IList<ReceiptLocationDetail> receiptLocationDetailList = new List<ReceiptLocationDetail>();

                foreach (ReceiptDetail receiptDetail in receiptMaster.ReceiptDetails)
                {
                    ((List<ReceiptLocationDetail>)receiptLocationDetailList).AddRange(receiptDetail.ReceiptLocationDetails);
                }

                return receiptLocationDetailList;
            }
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

        private IList<IpLocationDetail> LoadIpLocationDetails(int[] ipDetIdList)
        {
            IList<object> para = new List<object>();
            string selectIpLocationDetailStatement = string.Empty;
            foreach (int id in ipDetIdList)
            {
                if (selectIpLocationDetailStatement == string.Empty)
                {
                    selectIpLocationDetailStatement = "from IpLocationDetail where IpDetailId in (?";
                }
                else
                {
                    selectIpLocationDetailStatement += ",?";
                }
                para.Add(id);
            }
            selectIpLocationDetailStatement += ")";

            return this.genericMgr.FindAll<IpLocationDetail>(selectIpLocationDetailStatement, para.ToArray());
        }

        private IList<SequenceDetail> TryLoadSequenceDetails(SequenceMaster sequenceMaster)
        {
            if (sequenceMaster.SequenceDetails == null)
            {
                #region 获取排序单明细
                sequenceMaster.SequenceDetails = this.genericMgr.FindAll<SequenceDetail>("from SequenceDetail where SequenceNo = ? order by Sequence", new object[] { sequenceMaster.SequenceNo });
                #endregion
            }

            return sequenceMaster.SequenceDetails;
        }

        #region 异步打印

        public void AsyncSendPrintData(ReceiptMaster receiptMaster)
        {
            if (receiptMaster.Type == CodeMaster.IpDetailType.Gap)
            {
                return;
            }
            //AsyncSend asyncSend = new AsyncSend(this.SendPrintData);
            //asyncSend.BeginInvoke(receiptMaster, null, null);
            if (receiptMaster.IsPrintReceipt)
            {
                try
                {
                    var subPrintOrderList = this.genericMgr.FindAll<SubPrintOrder>();
                    string location = (receiptMaster.ReceiptDetails != null && receiptMaster.ReceiptDetails.Count > 0) ? receiptMaster.ReceiptDetails[0].LocationTo : null;
                    var pubPrintOrders = subPrintOrderList.Where(p => (p.Flow == receiptMaster.Flow || string.IsNullOrWhiteSpace(p.Flow))
                                && (p.UserId == receiptMaster.CreateUserId || p.UserId == 0)
                                && (p.Region == receiptMaster.PartyTo || string.IsNullOrWhiteSpace(p.Region))
                                && (location == null || p.Location == location || string.IsNullOrWhiteSpace(p.Location))
                                && p.ExcelTemplate == receiptMaster.ReceiptTemplate)
                                .Select(p => new PubPrintOrder
                                {
                                    Client = p.Client,
                                    ExcelTemplate = p.ExcelTemplate,
                                    Code = receiptMaster.ReceiptNo,
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

        public delegate void AsyncSend(ReceiptMaster receiptMaster);
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
