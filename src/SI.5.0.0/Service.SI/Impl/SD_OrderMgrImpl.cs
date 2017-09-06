namespace com.Sconit.Service.SI.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Castle.Services.Transaction;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.SI.SD_ORD;
    using com.Sconit.Entity.SCM;
    using com.Sconit.Entity.INV;
    using com.Sconit.Utility;
    using com.Sconit.Entity.VIEW;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.CUST;
    using com.Sconit.Entity.PRD;


    [Transactional]
    public class SD_OrderMgrImpl : BaseMgr, ISD_OrderMgr
    {
        public Entity.SI.SD_ORD.OrderMaster GetOrderKeyParts(string orderNo)
        {
            Entity.ORD.OrderMaster orderMaster = orderMgr.LoadOrderMaster(orderNo, true, false, false);
            orderMaster.OrderDetails = orderMaster.OrderDetails.Where(o => o.IsScanHu == true).ToList();
            var sdOrderMaster = Mapper.Map<Entity.ORD.OrderMaster, Entity.SI.SD_ORD.OrderMaster>(orderMaster);

            if (sdOrderMaster.OrderDetails != null)
            {
                sdOrderMaster.OrderDetails = Mapper.Map<IList<Entity.ORD.OrderDetail>, List<Entity.SI.SD_ORD.OrderDetail>>(orderMaster.OrderDetails).OrderBy(s => s.Sequence).ToList();
            }
            return sdOrderMaster;
        }

        public Entity.SI.SD_ORD.OrderMaster GetOrder(string orderNo, bool includeDetail)
        {
            Entity.ORD.OrderMaster orderMaster = orderMgr.LoadOrderMaster(orderNo, includeDetail, false, false);
            var sdOrderMaster = Mapper.Map<Entity.ORD.OrderMaster, Entity.SI.SD_ORD.OrderMaster>(orderMaster);

            if (sdOrderMaster.OrderDetails != null)
            {
                sdOrderMaster.OrderDetails = Mapper.Map<IList<Entity.ORD.OrderDetail>, List<Entity.SI.SD_ORD.OrderDetail>>(orderMaster.OrderDetails).OrderBy(s => s.Sequence).ToList();
            }
            return sdOrderMaster;
        }

        public Entity.SI.SD_ORD.OrderMaster GetOrderByOrderNoAndExtNo(string orderNo, bool includeDetail)
        {
            Entity.ORD.OrderMaster orderMaster = orderMgr.GetOrderMasterByOrderNoAndExtNo(orderNo, includeDetail, false, false);
            var sdOrderMaster = Mapper.Map<Entity.ORD.OrderMaster, Entity.SI.SD_ORD.OrderMaster>(orderMaster);

            if (sdOrderMaster != null && sdOrderMaster.OrderDetails != null && sdOrderMaster.OrderDetails.Count > 0)
            {
                sdOrderMaster.OrderDetails = Mapper.Map<IList<Entity.ORD.OrderDetail>, List<Entity.SI.SD_ORD.OrderDetail>>(orderMaster.OrderDetails).OrderBy(s => s.Sequence).ToList();
            }
            return sdOrderMaster;
        }

        public Entity.SI.SD_INV.Hu GetHuByOrderNo(string orderNo)
        {
            Entity.ORD.OrderMaster orderMaster = orderMgr.LoadOrderMaster(orderNo, true, false, false);
            foreach (var orderDetail in orderMaster.OrderDetails)
            {
                IList<string> huIds = genericMgr.FindAll<string>("select rld.HuId from ReceiptLocationDetail as rld where rld.OrderDetailId=?", orderDetail.Id);
                if (huIds != null && huIds.Count > 0)
                {
                    Entity.INV.Hu hu = genericMgr.FindById<Entity.INV.Hu>(huIds.SingleOrDefault().ToString());
                    var sdHu = Mapper.Map<Entity.INV.Hu, Entity.SI.SD_INV.Hu>(hu);
                    sdHu.OrderNo = orderNo;
                    return sdHu;
                }
            }
            return null;
        }

        public Entity.SI.SD_ORD.SequenceMaster GetSeq(string seqNo, bool includeDetail)
        {
            Entity.ORD.SequenceMaster sequenceMaster = genericMgr.FindById<Entity.ORD.SequenceMaster>(seqNo);

            if (includeDetail)
            {
                sequenceMaster.SequenceDetails = genericMgr.FindAll<Entity.ORD.SequenceDetail>("from SequenceDetail as sd where sd.SequenceNo=?", sequenceMaster.SequenceNo);
            }
            var sdSequenceMaster = Mapper.Map<Entity.ORD.SequenceMaster, Entity.SI.SD_ORD.SequenceMaster>(sequenceMaster);
            sdSequenceMaster.SequenceDetails = sdSequenceMaster.SequenceDetails.OrderBy(s => s.Sequence).ToList();
            return sdSequenceMaster;
        }

        public Entity.SI.SD_ORD.MiscOrderMaster GetMis(string MisNo)
        {
            Entity.ORD.MiscOrderMaster miscOrderMaster = genericMgr.FindById<Entity.ORD.MiscOrderMaster>(MisNo);
            var sdMiscOrderMaster = Mapper.Map<Entity.ORD.MiscOrderMaster, Entity.SI.SD_ORD.MiscOrderMaster>(miscOrderMaster);
            return sdMiscOrderMaster;
        }

        public List<string> GetItemTraces(string orderNo)
        {
            return genericMgr.FindAll<string>("select o.Item From OrderBomDetail as o where o.IsScanHu = ? and o.OrderNo = ?",
                new object[] { true, orderNo }).Distinct().ToList();
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateMiscOrderDetails(string miscOrderNo,
            IList<string> addHuIdList)
        {
            this.miscOrderMgr.BatchUpdateMiscOrderDetails(miscOrderNo, addHuIdList, null);
        }

        [Transaction(TransactionMode.Requires)]
        public void ConfirmMiscOrder(string miscOrderNo, IList<string> addHuIdList)
        {
            this.miscOrderMgr.BatchUpdateMiscOrderDetails(miscOrderNo, addHuIdList, null);
            this.genericMgr.FlushSession();
            this.miscOrderMgr.CloseMiscOrder(miscOrderNo);
        }

        [Transaction(TransactionMode.Requires)]
        public void QuickCreateMiscOrder(IList<string> addHuIdList, string locationCode, string binCode, int type)
        {
            this.miscOrderMgr.QuickCreateMiscOrder(addHuIdList, locationCode, binCode, type);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartVanOrder(string orderNo, string location, IList<com.Sconit.Entity.SI.SD_INV.Hu> feedHuList)
        {
            bool isForce = false;
            foreach (var hu in feedHuList)
            {
                if (hu.IsEffective == false)
                {
                    //#region 不需要校验驾驶室的投料库位和Bom是否一致
                    //IList<string> orderBomLoationList = this.genericMgr.FindAll<string>("select distinct Location from OrderBomDetail where OrderNo = ? and Item = ? and IsScanHu = ?",
                    //    new object[] { orderNo, hu.Item, true });

                    //if (orderBomLoationList == null || orderBomLoationList.Count == 0)
                    //{
                    //    throw new BusinessException("生产单{0}上没有关键件{1}。", orderNo, hu.Item);
                    //}
                    //if (orderBomLoationList.Count > 1 || !orderBomLoationList.Contains(location))
                    //{
                    //    throw new BusinessException("关键件{0}不在库位{1}上投料。", hu.Item, location);
                    //}
                    //#endregion

                    TryPackBarcode(hu.HuId, location);
                    isForce = true;
                }
            }
            this.orderMgr.StartVanOrder(orderNo, feedHuList.Select(h => h.HuId).ToList(), isForce);
        }

        [Transaction(TransactionMode.Requires)]
        public void PackSequenceOrder(string sequenceNo, List<string> huIdList)
        {
            this.orderMgr.PackSequenceOrder(sequenceNo, huIdList);
        }

        [Transaction(TransactionMode.Requires)]
        public void UnPackSequenceOrder(string sequenceNo)
        {
            this.orderMgr.UnPackSequenceOrder(sequenceNo);
        }

        [Transaction(TransactionMode.Requires)]
        public void ShipSequenceOrder(string sequenceNo)
        {
            this.orderMgr.ShipSequenceOrder(sequenceNo);
        }

        [Transaction(TransactionMode.Requires)]
        public void ShipSequenceOrderBySupplier(string sequenceNo)
        {
            this.orderMgr.ShipSequenceOrderBySupplier(sequenceNo);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartVanOrder(string orderNo, string feedOrderNo)
        {
            this.orderMgr.StartVanOrder(orderNo, feedOrderNo);
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_ORD.IpMaster GetIp(string ipNo, bool includeDetail)
        {
            try
            {
                Entity.ORD.IpMaster ipMaster = genericMgr.FindById<Entity.ORD.IpMaster>(ipNo);
                var sdIpMaster = Mapper.Map<Entity.ORD.IpMaster, Entity.SI.SD_ORD.IpMaster>(ipMaster);
                if (includeDetail)
                {
                    var ipDetails = this.genericMgr.FindAll<Entity.ORD.IpDetail>("from IpDetail i where i.IpNo=? and i.Type=? and i.IsClose=? order by Sequence asc",
                            new object[] { ipNo, (int)CodeMaster.IpDetailType.Normal, false });
                    sdIpMaster.IpDetails = Mapper.Map<IList<Entity.ORD.IpDetail>, List<Entity.SI.SD_ORD.IpDetail>>(ipDetails);
                    sdIpMaster.IpDetails = sdIpMaster.IpDetails.OrderBy(i => i.Id).ToList();
                    var ipLocationDetails = this.genericMgr.FindAll<Entity.ORD.IpLocationDetail>
                            ("from IpLocationDetail i where i.IpNo = ? and i.IsClose = ? ", new object[] { ipNo, false });

                    var IpDetailInputs = new List<Entity.SI.SD_ORD.IpDetailInput>();

                    if (ipLocationDetails != null)
                    {
                        foreach (var ipDetail in sdIpMaster.IpDetails)
                        {
                            //ipDetail.IpDetailInputs = new List<Entity.SI.SD_ORD.IpDetailInput>();
                            foreach (var ipLocationDetail in ipLocationDetails)
                            {
                                if (ipLocationDetail.IpDetailId == ipDetail.Id
                                    && !string.IsNullOrWhiteSpace(ipLocationDetail.HuId))
                                {
                                    var ipdi = new Entity.SI.SD_ORD.IpDetailInput();
                                    ipdi.Id = ipDetail.Id;
                                    ipdi.Qty = ipLocationDetail.Qty;
                                    ipdi.ReceiveQty = ipLocationDetail.ReceivedQty;
                                    ipdi.HuId = ipLocationDetail.HuId;
                                    ipdi.IsOriginal = true;
                                    IpDetailInputs.Add(ipdi);
                                    //ipDetail.IpDetailInputs.Add(ipdi);
                                }
                            }
                        }
                    }
                    sdIpMaster.IpDetailInputs = IpDetailInputs;
                }
                return sdIpMaster;
            }
            catch
            {
                throw new BusinessException(string.Format("送货单号{0}没有找到", ipNo));
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_ORD.IpMaster GetIpByWmsIpNo(string wmsIpNo, bool includeDetail)
        {
            try
            {
                Entity.ORD.IpMaster ipMaster = genericMgr.FindAll<Entity.ORD.IpMaster>("from IpMaster im where im.WMSNo=?", wmsIpNo).FirstOrDefault();
                var ipNo = ipMaster.IpNo;
                var sdIpMaster = Mapper.Map<Entity.ORD.IpMaster, Entity.SI.SD_ORD.IpMaster>(ipMaster);
                if (includeDetail)
                {
                    var ipDetails = this.genericMgr.FindAll<Entity.ORD.IpDetail>("from IpDetail i where i.IpNo=? and i.Type=? and i.IsClose=? order by Sequence asc",
                            new object[] { ipNo, (int)CodeMaster.IpDetailType.Normal, false });
                    sdIpMaster.IpDetails = Mapper.Map<IList<Entity.ORD.IpDetail>, List<Entity.SI.SD_ORD.IpDetail>>(ipDetails);
                    sdIpMaster.IpDetails = sdIpMaster.IpDetails.OrderBy(i => i.Id).ToList();
                    var ipLocationDetails = this.genericMgr.FindAll<Entity.ORD.IpLocationDetail>
                            ("from IpLocationDetail i where i.IpNo = ? and i.IsClose = ? ", new object[] { ipNo, false });

                    var IpDetailInputs = new List<Entity.SI.SD_ORD.IpDetailInput>();

                    if (ipLocationDetails != null)
                    {
                        foreach (var ipDetail in sdIpMaster.IpDetails)
                        {
                            //ipDetail.IpDetailInputs = new List<Entity.SI.SD_ORD.IpDetailInput>();
                            foreach (var ipLocationDetail in ipLocationDetails)
                            {
                                if (ipLocationDetail.IpDetailId == ipDetail.Id
                                    && !string.IsNullOrWhiteSpace(ipLocationDetail.HuId))
                                {
                                    var ipdi = new Entity.SI.SD_ORD.IpDetailInput();
                                    ipdi.Id = ipDetail.Id;
                                    ipdi.Qty = ipLocationDetail.Qty;
                                    ipdi.ReceiveQty = ipLocationDetail.ReceivedQty;
                                    ipdi.HuId = ipLocationDetail.HuId;
                                    ipdi.IsOriginal = true;
                                    IpDetailInputs.Add(ipdi);
                                    //ipDetail.IpDetailInputs.Add(ipdi);
                                }
                            }
                        }
                    }
                    sdIpMaster.IpDetailInputs = IpDetailInputs;
                }
                return sdIpMaster;
            }
            catch
            {
                throw new BusinessException(string.Format("送货单号{0}没有找到", wmsIpNo));
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_ORD.PickListMaster GetPickList(string pickListNo, bool includeDetail)
        {
            var basePickListMaster = genericMgr.FindById<Entity.ORD.PickListMaster>(pickListNo);
            var pickListMaster = Mapper.Map<Entity.ORD.PickListMaster, Entity.SI.SD_ORD.PickListMaster>(basePickListMaster);
            if (includeDetail)
            {
                var pickListDetails = this.genericMgr.FindAll<Entity.ORD.PickListDetail>
                    ("from PickListDetail i where i.PickListNo=? and i.IsClose =? and i.IsInventory = ?",
                    new object[] { pickListNo, false, true });
                pickListMaster.PickListDetails = Mapper.Map<IList<Entity.ORD.PickListDetail>, List<Entity.SI.SD_ORD.PickListDetail>>(pickListDetails);

                foreach (var pickListDetail in pickListMaster.PickListDetails)
                {
                    pickListDetail.ItemDisconList = this.itemMgr.GetItemDiscontinues(pickListDetail.Item, DateTime.Now)
                        .Select(p => p.DiscontinueItem).ToList();
                }
            }
            return pickListMaster;
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_ORD.InspectMaster GetInspect(string inspectNo, bool includeDetail)
        {
            var baseInspectMaster = genericMgr.FindById<Entity.INP.InspectMaster>(inspectNo);
            var inspectMaster = Mapper.Map<Entity.INP.InspectMaster, Entity.SI.SD_ORD.InspectMaster>(baseInspectMaster);
            if (includeDetail)
            {
                var baseInspectDetails = this.genericMgr.FindAll<Entity.INP.InspectDetail>("from InspectDetail i where i.IsJudge = ? and  i.InspectNo= ?  ", new object[] { false, inspectNo });
                inspectMaster.InspectDetails = Mapper.Map<IList<Entity.INP.InspectDetail>, List<Entity.SI.SD_ORD.InspectDetail>>(baseInspectDetails);
            }
            return inspectMaster;
        }

        /// <summary>
        /// 发货
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public string DoShipOrder(List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList, DateTime? effDate, bool isOpPallet)
        {
            if (orderDetailInputList == null || orderDetailInputList.Count == 0)
            {
                throw new com.Sconit.Entity.Exception.BusinessException("没有要发货的明细");
            }
            IList<Entity.ORD.OrderDetail> baseOrderDetailList = new List<Entity.ORD.OrderDetail>();
            var ids = orderDetailInputList.Select(o => o.Id).Distinct();

            foreach (var id in ids)
            {
                var baseOrderDatail = genericMgr.FindById<Entity.ORD.OrderDetail>(id);
                var selectedOrderDetailInputList = orderDetailInputList.Where(o => o.Id == id);
                if (selectedOrderDetailInputList != null)
                {
                    baseOrderDatail.OrderDetailInputs = new List<Entity.ORD.OrderDetailInput>();
                    foreach (var orderDetailInput in selectedOrderDetailInputList)
                    {
                        Entity.ORD.OrderDetailInput baseOrderDetailInput = new Entity.ORD.OrderDetailInput();
                        baseOrderDetailInput.HuId = orderDetailInput.HuId;
                        baseOrderDetailInput.ShipQty = orderDetailInput.ShipQty;
                        baseOrderDetailInput.LotNo = orderDetailInput.LotNo;

                        baseOrderDatail.OrderDetailInputs.Add(baseOrderDetailInput);
                    }
                }
                baseOrderDetailList.Add(baseOrderDatail);
            }

            if (effDate.HasValue)
            {
                return this.orderMgr.ShipOrder(baseOrderDetailList, effDate.Value, isOpPallet).IpNo;
            }
            else
            {
                return this.orderMgr.ShipOrder(baseOrderDetailList, isOpPallet).IpNo;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DoRepackAndShipOrder(List<Entity.SI.SD_INV.Hu> huList, DateTime? effDate)
        {
            List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList = new List<OrderDetailInput>();
            foreach (var hu in huList)
            {
                var orderDetailInput = new OrderDetailInput();
                orderDetailInput.HuId = hu.HuId;
                orderDetailInput.LotNo = hu.LotNo;
                orderDetailInput.ShipQty = hu.Qty;
                orderDetailInput.Id = hu.OrderDetId;
                orderDetailInputList.Add(orderDetailInput);
            }

            IList<Entity.ORD.OrderDetail> baseOrderDetailList = new List<Entity.ORD.OrderDetail>();
            var ids = orderDetailInputList.Select(o => o.Id).Distinct();

            foreach (var id in ids)
            {
                var baseOrderDatail = genericMgr.FindById<Entity.ORD.OrderDetail>(id);
                var selectedOrderDetailInputList = orderDetailInputList.Where(o => o.Id == id);
                if (selectedOrderDetailInputList != null)
                {
                    baseOrderDatail.OrderDetailInputs = new List<Entity.ORD.OrderDetailInput>();
                    foreach (var orderDetailInput in selectedOrderDetailInputList)
                    {
                        Entity.ORD.OrderDetailInput baseOrderDetailInput = new Entity.ORD.OrderDetailInput();
                        baseOrderDetailInput.HuId = orderDetailInput.HuId;
                        //翻包
                        HuMapping huMapping = this.genericMgr.FindAll<HuMapping>("select h from HuMapping as h where HuId = ?", orderDetailInput.HuId).SingleOrDefault();
                        if (huMapping.IsEffective == false)
                        {
                            IList<HuMapping> huMappingList = this.genericMgr.FindAll<HuMapping>("select h from HuMapping as h where OldHus = ?", huMapping.OldHus);
                            var inventoryPackList = new List<Entity.INV.InventoryRePack>();
                            foreach (var huId in huMappingList.Select(h => h.HuId))
                            {
                                var inventoryUnPack = new Entity.INV.InventoryRePack();
                                inventoryUnPack.HuId = huId;
                                inventoryUnPack.Type = CodeMaster.RePackType.In;
                                inventoryPackList.Add(inventoryUnPack);
                            }
                            foreach (var huId in huMappingList.FirstOrDefault().OldHus.Split(new char[] { ';' }))
                            {
                                if (!string.IsNullOrEmpty(huId))
                                {
                                    var inventoryUnPack = new Entity.INV.InventoryRePack();
                                    inventoryUnPack.HuId = huId;
                                    inventoryUnPack.Type = CodeMaster.RePackType.Out;
                                    inventoryPackList.Add(inventoryUnPack);
                                }
                            }
                            foreach (var hum in huMappingList)
                            {
                                hum.IsEffective = true;
                                this.genericMgr.Update(hum);
                            }
                            locationDetailMgr.InventoryRePack(inventoryPackList, false, effDate.HasValue ? effDate.Value : DateTime.Now);

                            //else
                            //{
                            //    var inventoryPackList = new List<Entity.INV.InventoryRePack>();

                            //    var inventoryPacked = new Entity.INV.InventoryRePack();
                            //    inventoryPacked.HuId = huMapping.HuId;
                            //    inventoryPacked.Type = CodeMaster.RePackType.Out;
                            //    inventoryPackList.Add(inventoryPacked);

                            //    var inventoryUnPack = new Entity.INV.InventoryRePack();
                            //    inventoryUnPack.HuId = huMapping.OldHus;
                            //    inventoryUnPack.Type = CodeMaster.RePackType.In;
                            //    inventoryPackList.Add(inventoryUnPack);

                            //    locationDetailMgr.InventoryRePack(inventoryPackList);
                            //}
                        }

                        baseOrderDetailInput.ShipQty = orderDetailInput.ShipQty;
                        baseOrderDetailInput.LotNo = orderDetailInput.LotNo;

                        baseOrderDatail.OrderDetailInputs.Add(baseOrderDetailInput);
                    }
                }
                baseOrderDetailList.Add(baseOrderDatail);
            }
            if (effDate.HasValue)
            {
                this.orderMgr.ShipOrder(baseOrderDetailList, effDate.Value);
            }
            else
            {
                this.orderMgr.ShipOrder(baseOrderDetailList);
            }
        }

        /// <summary>
        /// 收货
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public string DoReceiveOrder(List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList, DateTime? effDate)
        {
            if (orderDetailInputList == null || orderDetailInputList.Count == 0)
            {
                throw new com.Sconit.Entity.Exception.BusinessException("没有要收货的明细");
            }
            IList<Entity.ORD.OrderDetail> orderDetailList = new List<Entity.ORD.OrderDetail>();
            var ids = orderDetailInputList.Select(o => o.Id).Distinct();

            foreach (var id in ids)
            {
                var baseOrderDatail = genericMgr.FindById<Entity.ORD.OrderDetail>(id);
                var selectedrderDetailInputList = orderDetailInputList.Where(o => o.Id == id);
                if (selectedrderDetailInputList != null)
                {
                    baseOrderDatail.OrderDetailInputs = new List<Entity.ORD.OrderDetailInput>();
                    foreach (var orderDetailInput in selectedrderDetailInputList)
                    {
                        Entity.ORD.OrderDetailInput baseOrderDetailInput = new Entity.ORD.OrderDetailInput();
                        baseOrderDetailInput.HuId = orderDetailInput.HuId;
                        baseOrderDetailInput.ReceiveQty = orderDetailInput.ReceiveQty;
                        baseOrderDetailInput.LotNo = orderDetailInput.LotNo;
                        baseOrderDetailInput.Bin = orderDetailInput.Bin;

                        baseOrderDatail.OrderDetailInputs.Add(baseOrderDetailInput);
                    }
                }
                orderDetailList.Add(baseOrderDatail);
            }

            if (effDate.HasValue)
            {
                return this.orderMgr.ReceiveOrder(orderDetailList, effDate.Value).ReceiptNo;
            }
            else
            {
                return this.orderMgr.ReceiveOrder(orderDetailList).ReceiptNo;
            }
        }

        /// <summary>
        /// 收货
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public string DoReceiveIp(List<Entity.SI.SD_ORD.IpDetailInput> ipDetailInputList, DateTime? effDate)
        {
            if (ipDetailInputList == null || ipDetailInputList.Count() == 0)
            {
                throw new com.Sconit.Entity.Exception.BusinessException("没有要收货的明细");
            }
            IList<Entity.ORD.IpDetail> baseIpDetailList = new List<Entity.ORD.IpDetail>();

            var detIds = ipDetailInputList.Select(i => i.Id).Distinct();

            foreach (var id in detIds)
            {
                var ipDatail = genericMgr.FindById<Entity.ORD.IpDetail>(id);
                var q_1 = ipDetailInputList.Where(o => o.Id == id);
                if (q_1 != null)
                {
                    ipDatail.IpDetailInputs = new List<Entity.ORD.IpDetailInput>();
                    foreach (var odi in q_1)
                    {
                        var baseIpDetailInput = new Entity.ORD.IpDetailInput();
                        baseIpDetailInput.HuId = odi.HuId;
                        baseIpDetailInput.ReceiveQty = odi.ReceiveQty;
                        baseIpDetailInput.ShipQty = odi.ShipQty;
                        baseIpDetailInput.LotNo = odi.LotNo;
                        baseIpDetailInput.Bin = odi.Bin;

                        ipDatail.IpDetailInputs.Add(baseIpDetailInput);
                    }
                }
                baseIpDetailList.Add(ipDatail);
            }
            string recerptNo = string.Empty;
            if (effDate.HasValue)
            {
                recerptNo = this.orderMgr.ReceiveIp(baseIpDetailList, effDate.Value).ReceiptNo;
            }
            else
            {
                recerptNo = this.orderMgr.ReceiveIp(baseIpDetailList).ReceiptNo;
            }
            return recerptNo;
        }

        [Transaction(TransactionMode.Requires)]
        public void DoReceiveKit(string kitNo, DateTime? effDate)
        {
            var baseIpDetails = this.genericMgr.FindAll<Entity.ORD.IpDetail>(" from IpDetail i where i.OrderNo = ? ", kitNo);
            foreach (var baseIpDetail in baseIpDetails)
            {
                baseIpDetail.IpLocationDetails =
                    this.genericMgr.FindAll<Entity.ORD.IpLocationDetail>(" from IpLocationDetail i where i.IpDetailI d= ? ", baseIpDetail.Id);

                foreach (var ipLocationDetail in baseIpDetail.IpLocationDetails)
                {
                    var baseIpDetailInput = new Entity.ORD.IpDetailInput();
                    baseIpDetailInput.HuId = ipLocationDetail.HuId;
                    baseIpDetailInput.ReceiveQty = ipLocationDetail.Qty / baseIpDetail.UnitQty;
                    baseIpDetailInput.LotNo = ipLocationDetail.LotNo;

                    baseIpDetail.IpDetailInputs.Add(baseIpDetailInput);
                }
            }

            effDate = effDate.HasValue ? effDate.Value : DateTime.Now;

            this.orderMgr.ReceiveIp(baseIpDetails, effDate.Value);
        }

        /// <summary>
        /// 移库
        /// </summary>List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList
        [Transaction(TransactionMode.Requires)]
        public void DoTransfer(Entity.SI.SD_SCM.FlowMaster flowMaster, List<Entity.SI.SD_SCM.FlowDetailInput> flowDetailInputList,bool isFifo = true,bool isOpPallet = false)
        {
            #region 按库位分多个订单
            var locCodes = flowDetailInputList.Select(p => p.LocFrom).Distinct();
            #endregion

            foreach (string locFrom in locCodes)
            {
                List<Entity.SI.SD_SCM.FlowDetailInput> matchedFlowDetailInputList = flowDetailInputList.Where(p => p.LocFrom == locFrom).ToList();
                if (matchedFlowDetailInputList == null || matchedFlowDetailInputList.Count == 0)
                {
                    throw new BusinessException("没有可以移库的明细");
                }
                if (matchedFlowDetailInputList.GroupBy(p => p.QualityType).Count() > 1)
                {
                    throw new BusinessException("不同质量状态的条码不能合并成一张订单移库");
                }

                var orderMaster = new Entity.ORD.OrderMaster();
                var locationFrom = this.genericMgr.FindById<Entity.MD.Location>(locFrom);
                var locationTo = this.genericMgr.FindById<Entity.MD.Location>(flowMaster.LocationTo);
                var partyFrom = this.genericMgr.FindById<Entity.MD.Party>(flowMaster.PartyFrom);
                var partyTo = this.genericMgr.FindById<Entity.MD.Party>(flowMaster.PartyTo);

                orderMaster.LocationFrom = locationFrom.Code;
                orderMaster.IsShipScanHu = true;
                orderMaster.IsReceiveScanHu = true;
                orderMaster.LocationFromName = locationFrom.Name;
                orderMaster.LocationTo = locationTo.Code;
                orderMaster.LocationToName = locationTo.Name;
                orderMaster.PartyFrom = partyFrom.Code;
                orderMaster.PartyFromName = partyFrom.Name;
                orderMaster.PartyTo = partyTo.Code;
                orderMaster.PartyToName = partyTo.Name;
                orderMaster.Type = !locationTo.Region.StartsWith("S", StringComparison.OrdinalIgnoreCase) ? CodeMaster.OrderType.Transfer : CodeMaster.OrderType.SubContractTransfer;
                orderMaster.StartTime = DateTime.Now;
                orderMaster.WindowTime = DateTime.Now;
                orderMaster.EffectiveDate = flowMaster.EffectiveDate;
                orderMaster.Flow = flowMaster.Code;
                orderMaster.IsShipFulfillUC = false;
                orderMaster.IsQuick = true;
                orderMaster.IsPrintReceipt = true;
                orderMaster.QualityType = matchedFlowDetailInputList.First().QualityType;
                orderMaster.OrderTemplate = "ORD_Transfer.xls";
                orderMaster.AsnTemplate = "ASN_Transfer.xls";
                orderMaster.ReceiptTemplate = "REC_InvIn.xls";
                orderMaster.IsAsnUniqueReceive = true;

                if (!string.IsNullOrWhiteSpace(flowMaster.Code))
                {
                    var baseFlowMaster = this.genericMgr.FindById<FlowMaster>(flowMaster.Code);

                    orderMaster.IsQuick = false;

                    orderMaster.IsShipScanHu = baseFlowMaster.IsShipScanHu;
                    orderMaster.IsReceiveScanHu = baseFlowMaster.IsReceiveScanHu;
                    orderMaster.IsAutoReceive = baseFlowMaster.IsAutoReceive;
                    orderMaster.IsAutoRelease = true;//baseFlowMaster.IsAutoRelease;
                    orderMaster.IsAutoStart = true;//baseFlowMaster.IsAutoStart;
                    orderMaster.IsAutoShip = true;//baseFlowMaster.IsAutoShip;
                    orderMaster.IsInspect = baseFlowMaster.IsInspect;
                    orderMaster.IsPrintAsn = baseFlowMaster.IsPrintAsn;
                    orderMaster.IsPrintOrder = baseFlowMaster.IsPrintOrder;
                    orderMaster.IsPrintReceipt = baseFlowMaster.IsPrintRceipt;
                    orderMaster.IsShipByOrder = baseFlowMaster.IsShipByOrder;
                    orderMaster.OrderTemplate = baseFlowMaster.OrderTemplate;
                    orderMaster.AsnTemplate = baseFlowMaster.AsnTemplate;
                    orderMaster.ReceiptTemplate = baseFlowMaster.ReceiptTemplate;
                    orderMaster.IsShipFifo = baseFlowMaster.IsShipFifo;
                    orderMaster.IsAsnUniqueReceive = baseFlowMaster.IsAsnUniqueReceive;

                    if (!string.IsNullOrWhiteSpace(baseFlowMaster.ShipFrom))
                    {
                        var shipFrom = this.genericMgr.FindById<Address>(baseFlowMaster.ShipFrom);
                        orderMaster.ShipFrom = shipFrom.Code;
                        orderMaster.ShipFromAddress = shipFrom.AddressContent;
                        orderMaster.ShipFromCell = shipFrom.MobilePhone;
                        orderMaster.ShipFromTel = shipFrom.TelPhone;
                        orderMaster.ShipFromFax = shipFrom.Fax;
                        orderMaster.ShipFromContact = shipFrom.ContactPersonName;
                    }
                    if (!string.IsNullOrWhiteSpace(baseFlowMaster.ShipTo))
                    {
                        var shipTo = this.genericMgr.FindById<Address>(baseFlowMaster.ShipTo);
                        orderMaster.ShipTo = shipTo.Code;
                        orderMaster.ShipToAddress = shipTo.AddressContent;
                        orderMaster.ShipToCell = shipTo.MobilePhone;
                        orderMaster.ShipToTel = shipTo.TelPhone;
                        orderMaster.ShipToFax = shipTo.Fax;
                        orderMaster.ShipToContact = shipTo.ContactPersonName;
                    }
                }
                else
                {
                    var shipFrom = (this.genericMgr.FindAll<Address>(
                        " select a from PartyAddress p join p.Address as a where p.Party = ? and p.Type =?",
                        new object[] { orderMaster.PartyFrom, (int)CodeMaster.AddressType.ShipAddress }, 0, 1) ?? new List<Address>()).FirstOrDefault();
                    if (shipFrom != null)
                    {
                        orderMaster.ShipFrom = shipFrom.Code;
                        orderMaster.ShipFromAddress = shipFrom.AddressContent;
                        orderMaster.ShipFromCell = shipFrom.MobilePhone;
                        orderMaster.ShipFromTel = shipFrom.TelPhone;
                        orderMaster.ShipFromFax = shipFrom.Fax;
                        orderMaster.ShipFromContact = shipFrom.ContactPersonName;
                    }

                    var shipTo = (this.genericMgr.FindAll<Address>(
                        " select a from PartyAddress p join p.Address as a where p.Party = ? and p.Type =? ",
                        new object[] { orderMaster.PartyTo, (int)CodeMaster.AddressType.ShipAddress }, 0, 1) ?? new List<Address>()).FirstOrDefault();
                    if (shipTo != null)
                    {
                        orderMaster.ShipTo = shipTo.Code;
                        orderMaster.ShipToAddress = shipTo.AddressContent;
                        orderMaster.ShipToCell = shipTo.MobilePhone;
                        orderMaster.ShipToTel = shipTo.TelPhone;
                        orderMaster.ShipToFax = shipTo.Fax;
                        orderMaster.ShipToContact = shipTo.ContactPersonName;
                    }
                }

                #region 加一段强制先进先出
                orderMaster.IsShipFifo = isFifo;

                //bool isForceFifo = bool.Parse(systemMgr.GetEntityPreferenceValue(com.Sconit.Entity.SYS.EntityPreference.CodeEnum.IsForceFIFO));
                //if (string.IsNullOrEmpty(flowMaster.Bin) && isForceFifo)
                //{
                //    orderMaster.IsShipFifo = true;
                //}
                #endregion

                orderMaster.OrderDetails = new List<Entity.ORD.OrderDetail>();
                int seq = 1;
                var groupHus = this.genericMgr.FindAllIn<Hu>(" from Hu where HuId in(?", matchedFlowDetailInputList.Select(p => p.HuId))
                    .GroupBy(r => new { r.Item, r.Uom, r.Direction, r.UnitCount, r.BaseUom});


                foreach (var groupHu in groupHus)
                {
                    var baseOrderDetail = new Entity.ORD.OrderDetail();
                    baseOrderDetail.BaseUom = groupHu.Key.BaseUom;
                    baseOrderDetail.Item = groupHu.Key.Item;
                    baseOrderDetail.UnitCount = groupHu.Key.UnitCount;
                    baseOrderDetail.Uom = groupHu.Key.Uom;


                    baseOrderDetail.ItemDescription = groupHu.First().ItemDescription;
                    baseOrderDetail.OrderType = orderMaster.Type;
                    baseOrderDetail.QualityType = orderMaster.QualityType;
                    baseOrderDetail.Sequence = seq++;



                    baseOrderDetail.OrderDetailInputs = new List<Entity.ORD.OrderDetailInput>();
                    foreach (var hu in groupHu)
                    {
                        var baseOrderDetailInput = new Entity.ORD.OrderDetailInput();
                        baseOrderDetailInput.HuId = hu.HuId;
                        baseOrderDetailInput.ReceiveQty = hu.Qty;
                        baseOrderDetailInput.Bin = flowMaster.Bin;
                        baseOrderDetailInput.LotNo = hu.LotNo;
                        baseOrderDetailInput.ShipQty = hu.Qty;
                        baseOrderDetail.OrderDetailInputs.Add(baseOrderDetailInput);

                        baseOrderDetail.RequiredQty += baseOrderDetailInput.ShipQty;
                        baseOrderDetail.OrderedQty += baseOrderDetailInput.ShipQty;
                    }
                    orderMaster.OrderDetails.Add(baseOrderDetail);
                }
                this.orderMgr.CreateOrder(orderMaster);

                #region 加一段托盘解绑的逻辑
                if (!isOpPallet)
                {
                    var hus = this.genericMgr.FindAllIn<Hu>(" from Hu where HuId in(?", matchedFlowDetailInputList.Select(p => p.HuId));
                    var palletHus = this.genericMgr.FindAllIn<PalletHu>(" from PalletHu where HuId in(?", matchedFlowDetailInputList.Select(p => p.HuId));
                    foreach (Hu h in hus)
                    {
                        if (!string.IsNullOrEmpty(h.PalletCode) && !h.IsExternal)
                        {
                            h.PalletCode = string.Empty;
                            genericMgr.Update(h);

                            var palletHu = palletHus.Where(p => p.HuId == h.HuId).FirstOrDefault();
                            if (palletHu != null)
                            {
                                genericMgr.Delete(palletHu);
                            }
                        }
                    }
                }
                #endregion

                if (!string.IsNullOrWhiteSpace(flowMaster.Bin) && orderMaster.Status == CodeMaster.OrderStatus.Close)
                {
                    var inventoryPutList = matchedFlowDetailInputList.Where(p => !string.IsNullOrWhiteSpace(p.HuId))
                        .Select(p => new InventoryPut { HuId = p.HuId, Bin = flowMaster.Bin }).ToList();
                    locationDetailMgr.InventoryPut(inventoryPutList);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DoAnDon(List<AnDonInput> anDonInputList)
        {
            List<string> flowCodeList = anDonInputList.Select(a => a.Flow).Distinct().ToList();
            foreach (var flowCode in flowCodeList)
            {
                //string flowCode = anDonInputList.Select(a => a.Flow).Distinct().Single();

                Entity.SCM.FlowMaster flowMaster = this.genericMgr.FindById<Entity.SCM.FlowMaster>(flowCode);
                Entity.ORD.OrderMaster orderMaster = orderMgr.TransferFlow2Order(flowMaster, anDonInputList.Select(a => a.Item).Distinct().ToList());
                orderMaster.StartTime = DateTime.Now;

                FlowStrategy flowStrategy = genericMgr.FindById<FlowStrategy>(flowCode);
                orderMaster.WindowTime = orderMaster.StartTime.AddHours((double)flowStrategy.LeadTime);
                orderMaster.IsAutoRelease = true;

                foreach (AnDonInput anDonInput in anDonInputList)
                {
                    Entity.ORD.OrderDetail orderDetail = orderMaster.OrderDetails != null ?
                        orderMaster.OrderDetails.Where(f => (f.LocationTo == anDonInput.LocationTo || (string.IsNullOrWhiteSpace(f.LocationTo) && flowMaster.LocationTo == anDonInput.LocationTo))
                        && f.Item == anDonInput.Item && f.UnitCount == anDonInput.UnitCount
                        && f.Uom == anDonInput.Uom && f.ManufactureParty == anDonInput.ManufactureParty).OrderBy(f => f.Sequence).FirstOrDefault() : null;

                    if (orderDetail != null)
                    {
                        orderDetail.RequiredQty += anDonInput.UnitCount;
                        orderDetail.OrderedQty += anDonInput.UnitCount;
                    }
                    else
                    {
                        orderDetail = new Entity.ORD.OrderDetail();
                        orderDetail.OrderType = orderMaster.Type;
                        orderDetail.OrderSubType = orderMaster.SubType;
                        //orderDetail.Sequence  = ;
                        Entity.MD.Item item = this.genericMgr.FindById<Entity.MD.Item>(anDonInput.Item);
                        orderDetail.Item = item.Code;
                        orderDetail.ItemDescription = item.Description;
                        orderDetail.ReferenceItemCode = item.ReferenceCode;
                        orderDetail.BaseUom = item.Uom;
                        orderDetail.Uom = anDonInput.Uom;
                        //orderDetail.PartyFrom=
                        orderDetail.UnitCount = anDonInput.UnitCount;
                        //orderDetail.UnitCountDescription=
                        //orderDetail.MinUnitCount=
                        orderDetail.QualityType = CodeMaster.QualityType.Qualified;
                        orderDetail.ManufactureParty = anDonInput.ManufactureParty;
                        orderDetail.RequiredQty += anDonInput.UnitCount;
                        orderDetail.OrderedQty += anDonInput.UnitCount;
                        //orderDetail.ShippedQty = ;
                        //orderDetail.ReceivedQty = ;
                        //orderDetail.RejectedQty = ;
                        //orderDetail.ScrapQty = ;
                        //orderDetail.PickedQty = ;
                        if (orderDetail.BaseUom != orderDetail.Uom)
                        {
                            orderDetail.UnitQty = this.itemMgr.ConvertItemUomQty(orderDetail.Item, orderDetail.BaseUom, 1, orderDetail.Uom);
                        }
                        else
                        {
                            orderDetail.UnitQty = 1;
                        }
                        //public string LocationFrom { get; set; }
                        //public string LocationFromName { get; set; }
                        Entity.MD.Location locationTo = this.genericMgr.FindById<Entity.MD.Location>(anDonInput.LocationTo);
                        orderDetail.LocationTo = locationTo.Code;
                        orderDetail.LocationToName = locationTo.Name;
                        orderDetail.IsInspect = false;
                        //public string Container { get; set; }
                        //public string ContainerDescription { get; set; }
                        //public Boolean IsScanHu { get; set; }
                        orderDetail.BinTo = anDonInput.Note;

                        orderMaster.AddOrderDetail(orderDetail);
                    }
                }

                this.orderMgr.CreateOrder(orderMaster);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public AnDonInput GetKanBanCard(string cardNo)
        {
            AnDonInput anDonInput = new AnDonInput();
            Entity.INV.KanBanCardInfo kanBanCardInfo = this.genericMgr.FindById<Entity.INV.KanBanCardInfo>(cardNo);
            Entity.INV.KanBanCard kanBanCard = this.genericMgr.FindById<Entity.INV.KanBanCard>(kanBanCardInfo.KBICode);
            anDonInput = Mapper.Map<Entity.INV.KanBanCard, AnDonInput>(kanBanCard);
            anDonInput.CardNo = kanBanCardInfo.CardNo;
            return anDonInput;
        }

        [Transaction(TransactionMode.Requires)]
        public void StartPickList(string pickListNo)
        {
            this.pickListMgr.StartPickList(pickListNo);
            //var basePickListMaster = genericMgr.FindById<Entity.ORD.PickListMaster>(pickListNo);
            //var pickListMaster = Mapper.Map<Entity.ORD.PickListMaster, Entity.SI.SD_ORD.PickListMaster>(basePickListMaster);

            //return pickListMaster;
        }

        [Transaction(TransactionMode.Requires)]
        public string ShipPickList(string pickListNo)
        {
            return this.orderMgr.ShipPickList(pickListNo).IpNo;
        }

        /// <summary>
        /// 拣货
        /// </summary>
        /// <param name="orderDetails">由多个<PickListDetaiId,HuId>组成的数组</param>
        /// <param name="userCode"></param>
        [Transaction(TransactionMode.Requires)]
        public void DoPickList(List<Entity.SI.SD_ORD.PickListDetailInput> pickListDetailInputList)
        {
            IList<Entity.ORD.PickListDetail> basePickListDetailList = new List<Entity.ORD.PickListDetail>();
            //todo
            var ids = pickListDetailInputList.Select(p => p.Id).Distinct();

            foreach (var id in ids)
            {
                var basePickListDatail = this.genericMgr.FindById<Entity.ORD.PickListDetail>(id);
                var huIdList = pickListDetailInputList.Where(o => o.Id == id).Select(p => p.HuId).Distinct();
                if (huIdList != null)
                {
                    basePickListDatail.PickListDetailInputs = new List<Entity.ORD.PickListDetailInput>();
                    foreach (var huId in huIdList)
                    {
                        Entity.ORD.PickListDetailInput basePickListDetailInput = new Entity.ORD.PickListDetailInput();
                        basePickListDetailInput.HuId = huId;

                        basePickListDatail.PickListDetailInputs.Add(basePickListDetailInput);
                    }
                }
                basePickListDetailList.Add(basePickListDatail);
            }
            this.pickListMgr.DoPick(basePickListDetailList);
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_ORD.OrderBomDetail DoMaterialIn(string orderNo, Entity.SI.SD_INV.Hu hu)
        {
            //todo
            throw new Exception();
        }

        [Transaction(TransactionMode.Requires)]
        public void DoInspect(List<string> huIdList, DateTime? effDate)
        {
            if (huIdList == null || huIdList.Count == 0)
            {
                throw new Entity.Exception.BusinessException("没有要检验的明细");
            }

            var inspectMaster = new Entity.INP.InspectMaster();
            inspectMaster.Status = CodeMaster.InspectStatus.Submit;
            inspectMaster.Type = CodeMaster.InspectType.Barcode;
            var inspectDetailList = new List<Entity.INP.InspectDetail>();
            foreach (var huId in huIdList)
            {
                var inspectDetail = new Entity.INP.InspectDetail();
                inspectDetail.HuId = huId;
                inspectDetailList.Add(inspectDetail);
            }
            inspectMaster.InspectDetails = inspectDetailList;
            inspectMgr.CreateInspectMaster(inspectMaster, effDate.HasValue ? effDate.Value : DateTime.Now);

            //try
            //{
            //    var subPrintOrderList = this.genericMgr.FindAll<SubPrintOrder>();
            //    var pubPrintOrders = subPrintOrderList.Where(p => (p.UserId == inspectMaster.CreateUserId || p.UserId == 0)
            //              && (p.Region == inspectMaster.Region || string.IsNullOrWhiteSpace(p.Region))
            //              && (p.Location == inspectMaster.InspectDetails[0].LocationFrom || string.IsNullOrWhiteSpace(p.Location)
            //              && p.ExcelTemplate == "InspectOrder.xls")
            //              ).Select(p => new PubPrintOrder
            //              {
            //                  Client = p.Client,
            //                  ExcelTemplate = p.ExcelTemplate,
            //                  Code = inspectMaster.InspectNo,
            //                  Printer = p.Printer
            //              });
            //    foreach (var pubPrintOrder in pubPrintOrders)
            //    {
            //        this.genericMgr.Create(pubPrintOrder);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //log.Error("Send data to print sevrer error:", ex);
            //}
        }

        [Transaction(TransactionMode.Requires)]
        public void DoWorkersWaste(List<string> huIdList, DateTime? effDate)
        {
            if (huIdList == null || huIdList.Count == 0)
            {
                throw new Entity.Exception.BusinessException("没有要检验的明细");
            }

            var inspectMaster = new Entity.INP.InspectMaster();
            inspectMaster.Status = CodeMaster.InspectStatus.Submit;
            inspectMaster.Type = CodeMaster.InspectType.Barcode;
            var inspectDetailList = new List<Entity.INP.InspectDetail>();
            foreach (var huId in huIdList)
            {
                var inspectDetail = new Entity.INP.InspectDetail();
                inspectDetail.HuId = huId;
                inspectDetailList.Add(inspectDetail);
            }
            inspectMaster.InspectDetails = inspectDetailList;
            inspectMgr.CreateWorkersWaste(inspectMaster, effDate.HasValue ? effDate.Value : DateTime.Now);
        }


        [Transaction(TransactionMode.Requires)]
        public void DoJudgeInspect(Entity.SI.SD_ORD.InspectMaster inspectMaster, List<string> HuIdList, DateTime? effDate)
        {
            var baseInspectMaster = this.GetInspectMaster(inspectMaster.InspectNo, true, false);
            if (baseInspectMaster.InspectDetails == null || baseInspectMaster.InspectDetails.Count == 0)
            {
                throw new Entity.Exception.BusinessException("没有检验明细");
            }
            if (HuIdList == null || HuIdList.Count == 0)
            {
                throw new Entity.Exception.BusinessException("没有判定明细");
            }
            foreach (var baseInspectDetail in baseInspectMaster.InspectDetails)
            {
                foreach (var huId in HuIdList)
                {
                    if (huId.Equals(baseInspectDetail.HuId, StringComparison.OrdinalIgnoreCase))
                    {
                        //baseInspectDetail.CurrentInspectQty = baseInspectDetail.InspectQty;
                        baseInspectDetail.JudgeFailCode = inspectMaster.FailCode;
                        baseInspectDetail.CurrentQty = baseInspectDetail.InspectQty;
                    }
                }
            }
            effDate = effDate.HasValue ? effDate.Value : DateTime.Now;
            if (effDate.HasValue)
            {
                this.inspectMgr.JudgeInspectDetail(baseInspectMaster.InspectDetails, effDate.Value);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_INV.Hu StartAging(string huId)
        {
            var hu = this.genericMgr.FindById<Hu>(huId);
            var newHu = customizationMgr.AgingHu(hu);
            return Mapper.Map<Hu, Entity.SI.SD_INV.Hu>(newHu);
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_INV.Hu DoAging(string huId)
        {
            var hu = this.genericMgr.FindById<Hu>(huId);
            var newHu = customizationMgr.AgedHu(hu, DateTime.Now);
            return Mapper.Map<Hu, Entity.SI.SD_INV.Hu>(newHu);
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_INV.Hu DoFilter(string huId, decimal outQty)
        {
            var hu = this.genericMgr.FindById<Hu>(huId);
            var newHu = customizationMgr.FilterHu(hu, outQty, DateTime.Now);
            return Mapper.Map<Hu, Entity.SI.SD_INV.Hu>(newHu);
        }

        [Transaction(TransactionMode.Requires)]
        public List<string> GetProdLineStation(string orderNo, string huId)
        {
            return null;
        }

        #region verify
        [Transaction(TransactionMode.Requires)]
        public Boolean VerifyOrderCompareToHu(string orderNo, string huId)
        {
            Entity.INV.Hu hu = genericMgr.FindById<Entity.INV.Hu>(huId);

            long counter = this.genericMgr.FindAll<long>("select count(*) as counter from OrderBomDetail where OrderNo = ? and Item = ?", new object[] { orderNo, hu.Item }).Single();

            if (counter > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 投料
        //投料到生产线
        [Transaction(TransactionMode.Requires)]
        public void FeedProdLineRawMaterial(string productLine, string productLineFacility, string location, List<com.Sconit.Entity.SI.SD_INV.Hu> hus, bool isForceFeed, DateTime? effectiveDate)
        {
            if (hus == null || hus.Count == 0)
            {
                throw new Entity.Exception.BusinessException("投料的条码明细不能为空");
            }

            var feedInputs = new List<Entity.PRD.FeedInput>();
            foreach (var hu in hus)
            {
                if (hu.IsEffective == false)
                {
                    TryPackBarcode(hu.HuId, location);
                }
                var feedInput = new Entity.PRD.FeedInput();

                feedInput.HuId = hu.HuId;
                feedInput.LotNo = hu.LotNo;
                feedInput.Qty = hu.Qty;
                feedInputs.Add(feedInput);
            }
            effectiveDate = effectiveDate.HasValue ? effectiveDate.Value : DateTime.Now;

            this.genericMgr.FlushSession();
            this.productionLineMgr.FeedRawMaterial(productLine, productLineFacility, feedInputs, isForceFeed, effectiveDate.Value);
        }

        #region 关键件投料到生产单
        [Transaction(TransactionMode.Requires)]
        public void FeedOrderRawMaterial(string orderNo, string location, List<com.Sconit.Entity.SI.SD_INV.Hu> hus, bool isForceFeed, DateTime? effectiveDate)
        {
            if (hus == null || hus.Count == 0)
            {
                throw new Entity.Exception.BusinessException("投料的条码明细不能为空");
            }

            var feedInputs = new List<Entity.PRD.FeedInput>();
            foreach (var hu in hus)
            {
                if (hu.IsEffective == false)
                {
                    #region 校验投料的库位是否Bom物料的来源库位
                    if (!isForceFeed)
                    {
                        IList<string> orderBomLoationList = this.genericMgr.FindAll<string>("select distinct Location from OrderBomDetail where OrderNo = ? and Item = ? and IsScanHu = ?",
                            new object[] { orderNo, hu.Item, true });
                        if (orderBomLoationList == null || orderBomLoationList.Count == 0 || !orderBomLoationList.Contains(location))
                        {
                            string bomLocations = string.Empty;
                            foreach (string bomLocation in orderBomLoationList)
                            {
                                if (orderBomLoationList.IndexOf(bomLocation) == 0)
                                {
                                    bomLocations += bomLocation;
                                }
                                else
                                {
                                    bomLocations += ", " + bomLocation;
                                }
                            }
                            if (bomLocations != string.Empty)
                            {
                                throw new BusinessException("关键件{0}不在库位{1}上投料，可投料的库位有{2}", hu.Item, location, bomLocations);
                            }
                            else
                            {
                                throw new BusinessException("物料{1}不是生产单{0}的关键件。", orderNo, hu.Item);
                            }
                        }
                    }
                    #endregion

                    TryPackBarcode(hu.HuId, location);
                }
                var feedInput = new Entity.PRD.FeedInput();
                feedInput.HuId = hu.HuId;
                feedInput.LotNo = hu.LotNo;
                feedInput.Qty = hu.Qty;
                feedInputs.Add(feedInput);
            }
            effectiveDate = effectiveDate.HasValue ? effectiveDate.Value : DateTime.Now;
            this.genericMgr.FlushSession();
            this.productionLineMgr.FeedRawMaterial(orderNo, feedInputs, isForceFeed, effectiveDate.Value);
        }
        #endregion

        #region 生产单投料，投Kit单料
        [Transaction(TransactionMode.Requires)]
        public void FeedKitOrder(string orderNo, string kitOrderNo, bool isForceFeed, DateTime? effectiveDate)
        {
            effectiveDate = effectiveDate.HasValue ? effectiveDate.Value : DateTime.Now;
            this.productionLineMgr.FeedKitOrder(orderNo, kitOrderNo, isForceFeed, effectiveDate.Value);
        }
        #endregion

        #region 生产单投料，投工单
        [Transaction(TransactionMode.Requires)]
        public void FeedProductOrder(string orderNo, string productOrderNo, bool isForceFeed, DateTime? effectiveDate)
        {
            effectiveDate = effectiveDate.HasValue ? effectiveDate.Value : DateTime.Now;
            this.productionLineMgr.FeedProductOrder(orderNo, productOrderNo, isForceFeed, effectiveDate.Value);
        }
        #endregion

        #endregion

        [Transaction(TransactionMode.Requires)]
        public void ReturnOrderRawMaterial(string orderNo, string traceCode, int? operation, string opReference, string[][] huDetails, DateTime? effectiveDate)
        {
            if (huDetails == null)
            {
                throw new Entity.Exception.BusinessException("投料的条码明细不能为空");
            }

            var returnInputs = new List<Entity.PRD.ReturnInput>();
            foreach (var huDetail in huDetails)
            {
                var returnInput = new Entity.PRD.ReturnInput();
                returnInput.HuId = huDetail[0];
                returnInput.Qty = decimal.Parse(huDetail[1]);
                returnInputs.Add(returnInput);
            }
            effectiveDate = effectiveDate.HasValue ? effectiveDate.Value : DateTime.Now;
            this.productionLineMgr.ReturnRawMaterial(orderNo, traceCode, operation, opReference, returnInputs, effectiveDate.Value);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReturnProdLineRawMaterial(string productLine, string productLineFacility, string[][] huDetails, DateTime? effectiveDate)
        {
            if (huDetails == null)
            {
                throw new Entity.Exception.BusinessException("投料的条码明细不能为空");
            }

            var returnInputs = new List<Entity.PRD.ReturnInput>();
            foreach (var huDetail in huDetails)
            {
                var returnInput = new Entity.PRD.ReturnInput();
                returnInput.HuId = huDetail[0];
                returnInput.Qty = decimal.Parse(huDetail[1]);
                returnInputs.Add(returnInput);
            }
            effectiveDate = effectiveDate.HasValue ? effectiveDate.Value : DateTime.Now;
            this.productionLineMgr.ReturnRawMaterial(productLine, productLineFacility, returnInputs, effectiveDate.Value);
        }

        [Transaction(TransactionMode.Requires)]
        public void DoKitOrderScanKeyPart(string[][] huDetails, string orderNo)
        {
            var receiptDetails = this.genericMgr.FindAll<com.Sconit.Entity.ORD.ReceiptDetail>("select rd from ReceiptDetail rd where rd.OrderNo=?", orderNo);

            var inventoryPackList = new List<Entity.INV.InventoryPack>();
            foreach (var hu in huDetails)
            {
                if (receiptDetails.All(rd => rd.Item != hu[1]))
                {
                    throw new BusinessException("分装生产单{0}上的物料{1}没有收货.", receiptDetails[0].ReceiptNo, hu[1]);
                }
                var receiptDetail = receiptDetails.FirstOrDefault(rd => rd.Item == hu[1]);
                var receiptLocationDet = this.genericMgr.FindAll<com.Sconit.Entity.ORD.ReceiptLocationDetail>("select rld from ReceiptLocationDetail rld where rld.ReceiptDetailId=?", receiptDetail.Id).FirstOrDefault(r => r.ReceiptDetailId == receiptDetail.Id);
                if (!string.IsNullOrEmpty(receiptLocationDet.HuId))
                {
                    throw new BusinessException("分装生产单{0}上的物料{1}已扫描.", receiptDetails[0].ReceiptNo, hu[1]);
                }
                receiptLocationDet.HuId = hu[0];
                this.genericMgr.Update(receiptLocationDet);
                var inventoryPack = new Entity.INV.InventoryPack();
                inventoryPack.HuId = hu[0];
                inventoryPack.Location = receiptDetails[0].LocationTo;
                inventoryPackList.Add(inventoryPack);
            }
            locationDetailMgr.InventoryPack(inventoryPackList);
        }

        #region 快速退货
        //生产线退库，把数量变为条码移库
        [Transaction(TransactionMode.Requires)]
        public void DoReturnOrder(string flowCode, List<string> huIdList, DateTime? effectiveDate)
        {
            if (huIdList == null || huIdList.Count == 0)
            {
                throw new BusinessException("退库条码不能为空。");
            }
            IList<com.Sconit.Entity.VIEW.HuStatus> huStatusList = this.huMgr.GetHuStatus(huIdList);

            FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(flowCode);
            FlowMaster returnflowMaster = this.flowMgr.GetReverseFlow(flowMaster, huStatusList.Select(h => h.Item).Distinct().ToList());
            com.Sconit.Entity.ORD.OrderMaster orderMaster = this.orderMgr.TransferFlow2Order(returnflowMaster, null);

            orderMaster.StartTime = DateTime.Now;
            orderMaster.WindowTime = DateTime.Now;
            orderMaster.EffectiveDate = effectiveDate.HasValue ? effectiveDate : DateTime.Now;
            orderMaster.IsQuick = true;
            //orderMaster.IsAutoRelease = true;
            //orderMaster.IsAutoShip = false;
            //orderMaster.IsAutoReceive = true;
            orderMaster.IsShipScanHu = true;
            orderMaster.IsReceiveScanHu = true;
            orderMaster.IsPrintReceipt = true;
            orderMaster.SubType = CodeMaster.OrderSubType.Return;

            IList<InventoryPack> inventoryPackList = new List<InventoryPack>();
            BusinessException businessException = new BusinessException();
            foreach (com.Sconit.Entity.VIEW.HuStatus huStatus in huStatusList)
            {
                if (huStatus.Status == CodeMaster.HuStatus.Ip)
                {
                    businessException.AddMessage("条码{0}为库位{1}至库位{2}的在途库存，不能退库。", huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                }
                else if (huStatus.Status == CodeMaster.HuStatus.Location)
                {
                    businessException.AddMessage("条码{0}已经在库位{1}中，不能装箱。", huStatus.HuId, huStatus.Location);
                }
                else
                {
                    InventoryPack inventoryPack = new InventoryPack();
                    inventoryPack.Location = returnflowMaster.LocationFrom;
                    inventoryPack.HuId = huStatus.HuId;
                    inventoryPack.OccupyType = CodeMaster.OccupyType.None;
                    inventoryPack.OccupyReferenceNo = null;
                    inventoryPackList.Add(inventoryPack);
                }
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }

            //先装箱
            this.locationDetailMgr.InventoryPack(inventoryPackList);

            var groupedHuList = from hu in huStatusList
                                group hu by new
                                {
                                    Item = hu.Item,
                                    ItemDescription = hu.ItemDescription,
                                    ReferenceItemCode = hu.ReferenceItemCode,
                                    Uom = hu.Uom,
                                    BaseUom = hu.BaseUom,
                                    UnitQty = hu.UnitQty,
                                    UnitCount = hu.UnitCount
                                } into gj
                                select new
                                {
                                    Item = gj.Key.Item,
                                    ItemDescription = gj.Key.ItemDescription,
                                    ReferenceItemCode = gj.Key.ReferenceItemCode,
                                    Uom = gj.Key.Uom,
                                    BaseUom = gj.Key.BaseUom,
                                    UnitQty = gj.Key.UnitQty,
                                    UnitCount = gj.Key.UnitCount,
                                    Qty = gj.Sum(hu => hu.Qty),
                                    List = gj.ToList()
                                };

            foreach (var groupedHu in groupedHuList)
            {
                Entity.ORD.OrderDetail orderDetail = new Entity.ORD.OrderDetail();
                orderDetail.OrderNo = orderMaster.OrderNo;
                orderDetail.OrderType = orderMaster.Type;
                orderDetail.OrderSubType = orderMaster.SubType;
                orderDetail.Item = groupedHu.Item;
                orderDetail.ItemDescription = groupedHu.ItemDescription;
                orderDetail.ReferenceItemCode = groupedHu.ReferenceItemCode;
                orderDetail.Uom = groupedHu.Uom;
                orderDetail.BaseUom = groupedHu.BaseUom;
                orderDetail.UnitQty = groupedHu.UnitQty;
                orderDetail.UnitCount = groupedHu.UnitCount;
                orderDetail.QualityType = CodeMaster.QualityType.Qualified;
                orderDetail.RequiredQty = groupedHu.Qty;
                orderDetail.OrderedQty = groupedHu.Qty;
                orderMaster.AddOrderDetail(orderDetail);

                foreach (com.Sconit.Entity.VIEW.HuStatus huStatus in groupedHu.List)
                {
                    Entity.ORD.OrderDetailInput orderDetailInput = new Entity.ORD.OrderDetailInput();
                    orderDetailInput.HuId = huStatus.HuId;
                    orderDetailInput.ReceiveQty = huStatus.Qty;
                    orderDetailInput.LotNo = huStatus.LotNo;
                    orderDetail.AddOrderDetailInput(orderDetailInput);
                }
            }
            this.orderMgr.CreateOrder(orderMaster);
        }
        #endregion

        #region 分装生产单下线
        public void KitOrderOffline(string kitOrderNo, List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList, IList<string> feedKitOrderNoList, DateTime? effectiveDate)
        {
            //if (orderDetailInputList == null || orderDetailInputList.Count == 0)
            //{
            //    
            //}

            IList<Entity.ORD.OrderDetail> orderDetailList = this.genericMgr.FindAll<Entity.ORD.OrderDetail>("from OrderDetail where OrderNo = ?", kitOrderNo);
            var ids = orderDetailInputList.Select(o => o.Id).Distinct();

            foreach (Entity.ORD.OrderDetail orderDetail in orderDetailList)
            {
                if (orderDetail.IsScanHu)
                {
                    IList<Entity.SI.SD_ORD.OrderDetailInput> matchedOrderDetailInputList = orderDetailInputList.Where(input => input.Id == orderDetail.Id).ToList();

                    if (matchedOrderDetailInputList != null && matchedOrderDetailInputList.Count > 0)
                    {
                        foreach (Entity.SI.SD_ORD.OrderDetailInput matchedOrderDetailInput in matchedOrderDetailInputList)
                        {
                            if (!matchedOrderDetailInput.IsHuInLocation)
                            {
                                TryPackBarcode(matchedOrderDetailInput.HuId, orderDetail.LocationFrom);
                            }
                            Entity.ORD.OrderDetailInput orderDetailInput = new Entity.ORD.OrderDetailInput();
                            orderDetailInput.HuId = matchedOrderDetailInput.HuId;
                            orderDetailInput.LotNo = matchedOrderDetailInput.LotNo;
                            orderDetailInput.ReceiveQty = matchedOrderDetailInput.ReceiveQty;

                            orderDetail.AddOrderDetailInput(orderDetailInput);
                        }
                    }
                    else
                    {
                        throw new com.Sconit.Entity.Exception.BusinessException("没有扫描关键件{0}。", orderDetail.Item);
                    }
                }
                else
                {
                    Entity.ORD.OrderDetailInput orderDetailInput = new Entity.ORD.OrderDetailInput();
                    orderDetailInput.ReceiveQty = orderDetail.OrderedQty;

                    orderDetail.AddOrderDetailInput(orderDetailInput);
                }
            }

            this.genericMgr.FlushSession();
            this.orderMgr.KitOrderOffline(orderDetailList, feedKitOrderNoList, effectiveDate.HasValue ? effectiveDate.Value : DateTime.Now);
        }

        public List<com.Sconit.Entity.SI.SD_ORD.OrderMaster> GetKitBindingOrders(string orderNo)
        {
            var BindOrders = new List<com.Sconit.Entity.SI.SD_ORD.OrderMaster>();
            IList<com.Sconit.Entity.ORD.OrderBinding> orderBindingList = this.genericMgr.FindAll<com.Sconit.Entity.ORD.OrderBinding>("from OrderBinding where BindOrderNo is not null And OrderNo=?", orderNo);
            foreach (var orderBinding in orderBindingList)
            {
                if (!string.IsNullOrEmpty(orderBinding.BindFlow))
                {
                    com.Sconit.Entity.SCM.FlowMaster flowMaster = this.genericMgr.FindById<com.Sconit.Entity.SCM.FlowMaster>(orderBinding.BindFlow);
                    if (flowMaster.FlowStrategy == CodeMaster.FlowStrategy.KIT)
                    {
                        var orderMaster = this.genericMgr.FindById<com.Sconit.Entity.ORD.OrderMaster>(orderBinding.BindOrderNo);
                        BindOrders.Add(Mapper.Map<com.Sconit.Entity.ORD.OrderMaster, com.Sconit.Entity.SI.SD_ORD.OrderMaster>(orderMaster));
                    }
                }
            }

            return BindOrders;
        }
        #endregion

        [Transaction(TransactionMode.Requires)]
        public void DoItemTrace(string orderNo, List<string> huIdList)
        {
            orderMgr.DoItemTrace(orderNo, huIdList);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelItemTrace(string orderNo, List<string> huIdList)
        {
            orderMgr.CancelItemTrace(orderNo, huIdList);
        }

        private com.Sconit.Entity.INP.InspectMaster GetInspectMaster(string inspectNo, bool includeDetail, bool includeJudge)
        {
            var inspectMaster = genericMgr.FindById<com.Sconit.Entity.INP.InspectMaster>(inspectNo);
            if (includeDetail)
            {
                inspectMaster.InspectDetails = this.genericMgr.FindAll<com.Sconit.Entity.INP.InspectDetail>("from InspectDetail i where i.IsJudge = ? and  i.InspectNo= ?  ", new object[] { includeJudge, inspectNo });
            }
            return inspectMaster;
        }

        private void TryPackBarcode(string huId, string location)
        {
            HuStatus huStatus = this.huMgr.GetHuStatus(huId);

            if (string.IsNullOrWhiteSpace(huStatus.Location) && string.IsNullOrWhiteSpace(huStatus.LocationTo))
            {
                #region 装箱
                InventoryPack inventoryPack = new InventoryPack();

                inventoryPack.Location = location;
                inventoryPack.HuId = huId;
                inventoryPack.OccupyType = CodeMaster.OccupyType.None;
                inventoryPack.OccupyReferenceNo = null;

                IList<InventoryPack> inventoryPackList = new List<InventoryPack>();
                inventoryPackList.Add(inventoryPack);
                this.locationDetailMgr.InventoryPack(inventoryPackList);
                #endregion

                this.genericMgr.FlushSession();
            }
        }

        private bool IsQualityBarcode(string barcode)
        {
            if (barcode == null && barcode.Length != 17)
            {
                return false;
            }

            string supplierShortCode = barcode.Substring(0, 4);
            string itemShortCode = barcode.Substring(4, 5);
            string lotNo = barcode.Substring(9, 4);

            return LotNoHelper.IsValidateLotNo(lotNo);
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_INV.Hu DoReceiveProdOrder(string huId)
        {
            HuStatus huStatus = huMgr.GetHuStatus(huId);
            if (huStatus.Status == CodeMaster.HuStatus.Location || huStatus.Status == CodeMaster.HuStatus.Ip)
            {
                throw new BusinessException("此条码已在库存中");
            }
            if (string.IsNullOrWhiteSpace(huStatus.OrderNo))
            {
                throw new BusinessException("此条码没有带生产单号,不能收货");
            }
            var orderMasters = this.genericMgr.FindEntityWithNativeSql<Entity.ORD.OrderMaster>
                ("select * from ORD_OrderMstr_4 where OrderNo = ? ", huStatus.OrderNo);
            var order = orderMasters.First();
            if (orderMasters == null || orderMasters.Count == 0)
            {
                throw new BusinessException("没有找到此生产单");
            }
            else if (orderMasters.First().Status != CodeMaster.OrderStatus.InProcess)
            {
                throw new BusinessException("状态不是执行中的生产单不能收货");
            }
            else if (!Utility.SecurityHelper.HasPermission(order.Type, order.IsCheckPartyFromAuthority, order.IsCheckPartyToAuthority, order.PartyFrom, order.PartyTo, false, false))
            {
                throw new BusinessException("没有此生产单的权限");
            }
            var orderDetails = this.genericMgr.FindAllWithNativeSql<Entity.ORD.OrderDetail>
                ("select * from ORD_OrderDet_4 where OrderNo = ? ", huStatus.OrderNo);
            Entity.ORD.OrderDetail orderDetail = null;
            if (orderDetails != null && orderDetails.Count() > 0)
            {
                var orderDetail_1 = orderDetails.FirstOrDefault(o => o.Item == huStatus.Item && o.Uom == huStatus.Uom
                     && o.UnitCount == huStatus.UnitCount);
                if (orderDetail == null)
                {
                    orderDetail = orderDetails.FirstOrDefault(o => o.Item == huStatus.Item && o.Uom == huStatus.Uom);
                }
                else
                {
                    orderDetail = orderDetail_1;
                }
            }

            if (orderDetail == null)
            {
                throw new BusinessException("没有找到对应的生产单明细");
            }
            else
            {
                if (orderDetail.RemainReceivedQty == 0 && !orderMasters.First().IsReceiveExceed)
                {
                    throw new BusinessException("此生产单不允许超收");
                }
            }

            var orderDetailInput = new Entity.ORD.OrderDetailInput();
            orderDetailInput.HuId = huStatus.HuId;
            orderDetailInput.ReceiveQty = huStatus.Qty;
            orderDetailInput.LotNo = huStatus.LotNo;
            orderDetail.AddOrderDetailInput(orderDetailInput);
            var receiptMaster = this.orderMgr.ReceiveOrder(new List<Entity.ORD.OrderDetail>() { orderDetail });

            Entity.INV.Hu baseHu = this.genericMgr.FindById<Entity.INV.Hu>(huStatus.HuId);
            baseHu.ReceiptNo = receiptMaster.ReceiptNo;
            this.genericMgr.Update(baseHu);

            var hu = Mapper.Map<Entity.INV.Hu, Entity.SI.SD_INV.Hu>(baseHu);
            return hu;
        }

        [Transaction(TransactionMode.Requires)]
        public void DoReceiveProdOrder(List<string> huIdList)
        {
            var huStatusList = huMgr.GetHuStatus(huIdList);
            foreach (var huStatus in huStatusList)
            {
                if (huStatus.Status == CodeMaster.HuStatus.Location || huStatus.Status == CodeMaster.HuStatus.Ip)
                {
                    throw new BusinessException("此条码已在库存中");
                }
                if (string.IsNullOrWhiteSpace(huStatus.OrderNo))
                {
                    throw new BusinessException("此条码没有带生产单号,不能收货");
                }
            }
            var orderMasters = this.genericMgr.FindEntityWithNativeSqlIn<Entity.ORD.OrderMaster>
                ("select * from ORD_OrderMstr_4 where OrderNo in(? ", huStatusList.Select(p => p.OrderNo).Distinct());

            if (orderMasters == null || orderMasters.Count() == 0)
            {
                throw new BusinessException("没有找到生产单");
            }
            foreach (var orderMaster in orderMasters)
            {
                if (orderMaster.Status != CodeMaster.OrderStatus.InProcess)
                {
                    throw new BusinessException("状态不是执行中的生产单{0}不能收货", orderMaster.OrderNo);
                }
                else if (!Utility.SecurityHelper.HasPermission(orderMaster.Type, orderMaster.IsCheckPartyFromAuthority, orderMaster.IsCheckPartyToAuthority, orderMaster.PartyFrom, orderMaster.PartyTo, false, false))
                {
                    throw new BusinessException("没有此生产单{0}的权限", orderMaster.OrderNo);
                }
            }

            foreach (var orderMaster in orderMasters)
            {
                var huStatusList_OrderNo = huStatusList.Where(p => p.OrderNo == orderMaster.OrderNo);
                var orderDetails = this.genericMgr.FindEntityWithNativeSqlIn<Entity.ORD.OrderDetail>
                    ("select * from ORD_OrderDet_4 where OrderNo=? and Item in(? ",
                   huStatusList_OrderNo.Select(p => p.Item).Distinct(),
                   new object[] { orderMaster.OrderNo });
                foreach (var huStatus in huStatusList_OrderNo)
                {
                    var orderDetail = orderDetails.FirstOrDefault(o => o.Item == huStatus.Item && o.Uom == huStatus.Uom
                            && o.UnitCount == huStatus.UnitCount);
                    if (orderDetail == null)
                    {
                        orderDetail = orderDetails.FirstOrDefault(o => o.Item == huStatus.Item && o.Uom == huStatus.Uom);
                    }
                    if (orderDetail == null)
                    {
                        throw new BusinessException("没有找到条码{0}对应的生产单明细", huStatus.HuId);
                    }
                    else
                    {
                        if (orderDetail.RemainReceivedQty == 0 && !orderMaster.IsReceiveExceed)
                        {
                            throw new BusinessException("此生产单{0}不允许超收", orderMaster.OrderNo);
                        }
                    }
                    var orderDetailInput = new Entity.ORD.OrderDetailInput();
                    orderDetailInput.HuId = huStatus.HuId;
                    orderDetailInput.ReceiveQty = huStatus.Qty;
                    orderDetailInput.LotNo = huStatus.LotNo;
                    orderDetail.AddOrderDetailInput(orderDetailInput);
                }
                var receiptMaster = this.orderMgr.ReceiveOrder(orderDetails);
                var huList = this.genericMgr.FindAllIn<Entity.INV.Hu>
                    (" from Hu where HuId in(? ", huStatusList_OrderNo.Select(p => p.HuId));
                foreach (var hu in huList)
                {
                    hu.ReceiptNo = receiptMaster.ReceiptNo;
                    this.genericMgr.Update(hu);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_INV.Hu CancelReceiveProdOrder(string huId)
        {
            HuStatus huStatus = huMgr.GetHuStatus(huId);
            if (huStatus.Status != CodeMaster.HuStatus.Location)
            {
                throw new BusinessException("此条码不在库存中,不能冲销");
            }
            if (string.IsNullOrWhiteSpace(huStatus.ReceiptNo))
            {
                throw new BusinessException("此条码没有带收货单号,不能冲销");
            }

            var receiptMasters = this.genericMgr.FindEntityWithNativeSql<Entity.ORD.ReceiptMaster>
                        ("select * from ORD_RecMstr_4  where RecNo = ? ", huStatus.ReceiptNo);

            if (receiptMasters == null || receiptMasters.Count == 0)
            {
                throw new BusinessException("没有找到对应的生产收货单进行冲销");
            }
            if (receiptMasters.First().Status == CodeMaster.ReceiptStatus.Cancel)
            {
                throw new BusinessException("此收货单已经冲销:" + huStatus.ReceiptNo);
            }
            if (!Utility.SecurityHelper.HasPermission(receiptMasters.First()))
            {
                throw new BusinessException("没有此生产单的权限");
            }

            var receiptDetails = this.genericMgr.FindEntityWithNativeSql<Entity.ORD.ReceiptDetail>
                        ("select * from ORD_RecDet_4  where RecNo = ? ", huStatus.ReceiptNo);

            if (receiptDetails == null || receiptDetails.Count == 0)
            {
                throw new BusinessException("没有找到对应的生产收货单明细进行冲销");
            }
            if (huStatus.Location != receiptDetails.First().LocationTo)
            {
                throw new BusinessException("条码的库位不在:" + receiptDetails.First().LocationTo + ",不能冲销");
            }

            receiptMgr.CreateReceipt(receiptMasters.First());

            Entity.INV.Hu baseHu = this.genericMgr.FindById<Entity.INV.Hu>(huStatus.HuId);
            baseHu.ReceiptNo = null;
            this.genericMgr.Update(baseHu);

            var hu = Mapper.Map<Entity.INV.Hu, Entity.SI.SD_INV.Hu>(baseHu);
            hu.Memo = baseHu.ItemVersion + " " + baseHu.Remark;
            return hu;
        }
        public void RecSmallChkSparePart(string huId, string spareItem, string userCode)
        {
            try
            {
                SmallSparePartChk smallSparePartChk = new SmallSparePartChk();
                Hu hu = genericMgr.FindById<Hu>(huId);
                Item spareItem1 = itemMgr.GetCacheItem(spareItem);
                smallSparePartChk.Huid = hu.HuId;
                smallSparePartChk.HuItem = hu.Item;
                smallSparePartChk.HuItemDesc = hu.ItemDescription;
                smallSparePartChk.HuQty = hu.Qty;
                smallSparePartChk.SpareItem = spareItem1.Code;
                smallSparePartChk.SpareItemDesc = spareItem1.Description;
                genericMgr.Create(smallSparePartChk);
            }
            catch
            {
                throw new BusinessException(@Resources.PRD.SmallSparePartChk.SmallSparePartChk_FailToSave);
            }
        }
    }
}
