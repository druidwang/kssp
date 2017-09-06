namespace com.Sconit.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Castle.Services.Transaction;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.CUST;
    using com.Sconit.Entity.ORD;
    using AutoMapper;
    using com.Sconit.PrintModel.ORD;
    using com.Sconit.Utility.Report;
    using com.Sconit.PrintModel.INV;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Entity;
    using com.Sconit.Utility;
    using com.Sconit.Entity.MRP.MD;
    using com.Sconit.Entity.MRP.TRANS;
    using com.Sconit.Entity.Report;
    using System.Data.SqlClient;
    using Resources.Report;

    [Transactional]
    public class CustomizationMgrImpl : BaseMgr, ICustomizationMgr
    {
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public IHuMgr huMgr { get; set; }
        public IReportGen reportGen { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IItemMgr itemMgr { get; set; }
        public IBomMgr bomMgr { get; set; }

        //protected static DateTime LastPubDateTime;

        /// <summary>
        /// 老化开始
        /// </summary>
        /// <param name="hu"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Requires)]
        public Hu AgingHu(Hu hu)
        {
            var huStatus = this.huMgr.GetHuStatus(hu.HuId);
            if (huStatus.Status != CodeMaster.HuStatus.Location)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.CouldNotBurnInNotInInventory);
            }
            if (huStatus.IsFreeze)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.CouldNotBurnInForFreeze);
            }
            if (hu.HuOption == CodeMaster.HuOption.Aged)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.CouldNotBurnInHasBurnIn);
            }
            else if (hu.HuOption == CodeMaster.HuOption.UnAging || hu.HuOption == CodeMaster.HuOption.NoNeed)
            {
                hu.AgingStartTime = DateTime.Now;
            }
            else
            {
                throw new BusinessException(Resources.EXT.ServiceLan.CouldNotBurnInOther);
            }
            this.genericMgr.Update(hu);
            return hu;
        }

        /// <summary>
        /// 老化
        /// </summary>
        [Transaction(TransactionMode.Requires)]
        public Hu AgedHu(Hu hu, DateTime effectiveDate)
        {
            if (hu.HuOption == CodeMaster.HuOption.UnAging || hu.HuOption == CodeMaster.HuOption.NoNeed)
            {
                if (!hu.AgingStartTime.HasValue)
                {
                    throw new BusinessException(Resources.EXT.ServiceLan.CouldNotBurnInNotStart);
                }

                var huLocationLotDetail = locationDetailMgr.GetHuLocationLotDetail(hu.HuId);
                if (huLocationLotDetail == null)
                {
                    throw new BusinessException(Resources.EXT.ServiceLan.CouldNotBurnInNotInInventory);
                }
                Hu newHu = huMgr.CloneHu(hu, hu.Qty);
                newHu.HuOption = CodeMaster.HuOption.Aged;
                newHu.AgingEndTime = DateTime.Now;
                IList<ItemExchange> itemExchangeList = new List<ItemExchange>();

                ItemExchange itemExchange = new ItemExchange();
                itemExchange.BaseUom = hu.BaseUom;
                itemExchange.EffectiveDate = effectiveDate;
                itemExchange.OldHu = hu.HuId;
                itemExchange.IsVoid = false;
                itemExchange.ItemFrom = hu.Item;
                itemExchange.ItemTo = hu.Item;
                itemExchange.LocationFrom = huLocationLotDetail.Location;
                itemExchange.LocationTo = huLocationLotDetail.Location;
                itemExchange.ItemExchangeType = CodeMaster.ItemExchangeType.Aging;
                itemExchange.NewBaseUom = hu.BaseUom;
                itemExchange.NewHu = newHu.HuId;
                itemExchange.NewQty = newHu.Qty;
                itemExchange.NewUnitQty = newHu.UnitQty;
                itemExchange.NewUom = newHu.Uom;
                itemExchange.Qty = hu.Qty;
                itemExchange.QualityType = huLocationLotDetail.QualityType;
                string region = this.genericMgr.FindById<Location>(huLocationLotDetail.Location).Region;
                itemExchange.RegionFrom = region;
                itemExchange.RegionTo = region;
                itemExchange.UnitQty = hu.UnitQty;
                itemExchange.Uom = hu.Uom;
                itemExchange.LotNo = hu.LotNo;
                this.genericMgr.Create(itemExchange);

                itemExchangeList.Add(itemExchange);

                locationDetailMgr.InventoryExchange(itemExchangeList);

                newHu.RefId = itemExchange.Id;
                //newHu.HuTemplate = "BarCodeEX.xls";
                this.genericMgr.Update(newHu);

                return newHu;
            }
            else
            {
                throw new BusinessException(Resources.EXT.ServiceLan.CouldNotBurnIn);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelAgingHu(Hu hu, DateTime effectiveDate)
        {
            if (hu.HuOption != CodeMaster.HuOption.Aged)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.CouldNotVoidIsNotBurnIn);
            }

            ItemExchange itemExchange = this.genericMgr.FindById<ItemExchange>(hu.RefId);

            CancelItemExchangeHu(itemExchange, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public Hu FilterHu(Hu hu, decimal newQty, DateTime effectiveDate)
        {
            var huLocationLotDetail = locationDetailMgr.GetHuLocationLotDetail(hu.HuId);
            if (huLocationLotDetail == null)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.CouldNotFilterNotInInventory);
            }

            Hu newHu = huMgr.CloneHu(hu, newQty);
            newHu.HuOption = CodeMaster.HuOption.Filtered;

            IList<ItemExchange> itemExchangeList = new List<ItemExchange>();
            ItemExchange itemExchange = new ItemExchange();
            itemExchange.BaseUom = hu.BaseUom;
            itemExchange.EffectiveDate = effectiveDate;
            itemExchange.OldHu = hu.HuId;
            itemExchange.IsVoid = false;
            itemExchange.ItemFrom = hu.Item;
            itemExchange.ItemTo = hu.Item;
            itemExchange.LocationFrom = huLocationLotDetail.Location;
            itemExchange.LocationTo = huLocationLotDetail.Location;
            itemExchange.ItemExchangeType = CodeMaster.ItemExchangeType.Filter;
            itemExchange.NewBaseUom = hu.BaseUom;
            itemExchange.NewHu = newHu.HuId;
            itemExchange.NewQty = newHu.Qty;
            itemExchange.NewUnitQty = newHu.UnitQty;
            itemExchange.NewUom = newHu.Uom;
            itemExchange.Qty = hu.Qty;
            itemExchange.QualityType = huLocationLotDetail.QualityType;
            string region = this.genericMgr.FindById<Location>(huLocationLotDetail.Location).Region;
            itemExchange.RegionFrom = region;
            itemExchange.RegionTo = region;
            itemExchange.UnitQty = hu.UnitQty;
            itemExchange.Uom = hu.Uom;
            itemExchange.LotNo = hu.LotNo;
            this.genericMgr.Create(itemExchange);

            itemExchangeList.Add(itemExchange);

            locationDetailMgr.InventoryExchange(itemExchangeList);

            newHu.RefId = itemExchange.Id;
            this.genericMgr.Update(newHu);

            return newHu;
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelFilterHu(Hu hu, decimal outQty, DateTime effectiveDate)
        {
            if (hu.HuOption != CodeMaster.HuOption.Filtered)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.CouldNotFilterVoidHasNotFiltered);
            }

            ItemExchange itemExchange = this.genericMgr.FindById<ItemExchange>(hu.RefId);

            CancelItemExchangeHu(itemExchange, effectiveDate);
        }


        [Transaction(TransactionMode.Requires)]
        public void CancelItemExchangeHu(int itemExchangeId, DateTime effectiveDate)
        {
            ItemExchange itemExchange = this.genericMgr.FindById<ItemExchange>(itemExchangeId);
            CancelItemExchangeHu(itemExchange, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelItemExchangeHu(ItemExchange itemExchange, DateTime effectiveDate)
        {
            if (itemExchange.IsVoid)
            {
                throw new BusinessException(Resources.EXT.ServiceLan.HuHasVoided);
            }

            IList<ItemExchange> itemExchangeList = new List<ItemExchange>();
            itemExchangeList.Add(itemExchange);
            locationDetailMgr.CancelInventoryExchange(itemExchangeList);
        }


        private static object GetPubPrintOrderListLock = new object();
        [Transaction(TransactionMode.Requires)]
        public IList<PubPrintOrder> GetPubPrintOrderList(string clientCode)
        {
            lock (GetPubPrintOrderListLock)
            {
                var excelTemplateDic = systemMgr.GetCodeDetailDictionary()
                    .SelectMany(p => p.Value.Where(q => q.Value.EndsWith(".xls")))
                    .GroupBy(p => p.Value, (k, g) => new { k, Code = g.First().Code })
                    .ToDictionary(d => d.k, d => d.Code);

                DateTime lastPubDateTime = DateTime.Now.AddMinutes(-30);
                //lastPubDateTime = lastPubDateTime > LastPubDateTime ? lastPubDateTime : LastPubDateTime;
                //LastPubDateTime = DateTime.Now.AddMinutes(-1);

                var pubPrintOrderList = genericMgr.FindAll<PubPrintOrder>
                    (" from PubPrintOrder where IsPrinted = ? and CreateDate>=? and Client=? ",
                    new object[] { false, lastPubDateTime, clientCode });
                foreach (var pubPrintOrder in pubPrintOrderList)
                {
                    var templateType = excelTemplateDic.ValueOrDefault(pubPrintOrder.ExcelTemplate);
                    if (!string.IsNullOrWhiteSpace(templateType))
                    {
                        string printUrl = null;
                        try
                        {
                            switch (templateType)
                            {
                                case "OrderTemplate":
                                    printUrl = PrintOrder(pubPrintOrder.Code);
                                    break;
                                case "AsnTemplate":
                                    printUrl = PrintASN(pubPrintOrder.Code);
                                    break;
                                case "ReceiptTemplate":
                                    printUrl = PrintReceipt(pubPrintOrder.Code);
                                    break;
                                case "PickListTemplate":
                                    printUrl = PrintPickList(pubPrintOrder.Code);
                                    break;
                                case "HuTemplate":
                                    printUrl = PrintHu(pubPrintOrder.Code, pubPrintOrder.CreateUserName);
                                    break;
                                default:
                                    pubSubLog.Error(string.Format("没有定义此模板{0}类型.Id{1}", pubPrintOrder.ExcelTemplate, pubPrintOrder.Id));
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            pubSubLog.Error("获取打印订阅失败:", ex);
                        }
                        pubPrintOrder.PrintUrl = printUrl;
                        pubPrintOrder.IsPrinted = true;
                        genericMgr.Update(pubPrintOrder);
                    }
                    else
                    {
                        pubSubLog.Error(string.Format("模板字段不为空,Id:{0}", pubPrintOrder.Id));
                    }
                }
                return pubPrintOrderList;
            }
        }

        public string PrintOrder(string orderNo)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            IList<OrderDetail> orderDetails = genericMgr.FindAll<OrderDetail>
                ("select od from OrderDetail as od where od.OrderNo=?", orderNo);
            orderMaster.OrderDetails = orderDetails;
            PrintOrderMaster printOrderMstr = Mapper.Map<OrderMaster, PrintOrderMaster>(orderMaster);

            IList<object> data = new List<object>();
            data.Add(printOrderMstr);
            data.Add(printOrderMstr.OrderDetails);
            return reportGen.WriteToFile(orderMaster.OrderTemplate, data);
        }

        public string PrintASN(string ipNo)
        {
            IpMaster ipMaster = genericMgr.FindById<IpMaster>(ipNo);
            IList<IpDetail> ipDetails = genericMgr.FindAll<IpDetail>
                ("select id from IpDetail as id where id.IpNo=?", ipNo);
            ipMaster.IpDetails = ipDetails.Where(i => string.IsNullOrEmpty(i.GapReceiptNo)).ToList();

            foreach (var IpDetail in ipMaster.IpDetails)
            {
                var ipDetailGapData = ipDetails.Where(o => o.GapIpDetailId == IpDetail.Id).FirstOrDefault() ?? new IpDetail();
                IpDetail.OrderQty = genericMgr.FindById<OrderDetail>(IpDetail.OrderDetailId).OrderedQty;
                IpDetail.GapQty = ipDetailGapData.Qty;
                var iplocationCount = genericMgr.FindAll<IpLocationDetail>
                    ("from IpLocationDetail as o where o.IpDetailId = ?", IpDetail.Id)
                    .Where(o => !string.IsNullOrWhiteSpace(o.HuId))
                    .Count();

                if (iplocationCount == 0)
                {
                    if (!IpDetail.IsChangeUnitCount)
                    {
                        IpDetail.BoxQty = (int)Math.Ceiling(IpDetail.Qty / IpDetail.UnitCount);
                    }
                }
                else
                {
                    IpDetail.BoxQty = iplocationCount;
                }
            }

            PrintIpMaster printIpMaster = Mapper.Map<IpMaster, PrintIpMaster>(ipMaster);
            IList<object> data = new List<object>();
            data.Add(printIpMaster);
            data.Add(printIpMaster.IpDetails);

            return reportGen.WriteToFile(ipMaster.AsnTemplate, data);
        }

        public string PrintReceipt(string receiptNo)
        {
            ReceiptMaster receiptMaster = genericMgr.FindById<ReceiptMaster>(receiptNo);
            IList<ReceiptDetail> receiptDetail = genericMgr.FindAll<ReceiptDetail>
                ("select rd from ReceiptDetail as rd where rd.ReceiptNo=?", receiptNo);
            foreach (var receiptDet in receiptDetail)
            {
                var receiptlocationCount = genericMgr.FindAll<ReceiptLocationDetail>
                    ("from ReceiptLocationDetail as o where o.ReceiptDetailId = ?", receiptDet.Id)
                    .Where(o => !string.IsNullOrWhiteSpace(o.HuId))
                    .Count();

                if (receiptlocationCount == 0)
                {
                    receiptDet.BoxQty = (int)Math.Ceiling(receiptDet.ReceivedQty / (receiptDet.UnitCount > 0 ? receiptDet.UnitCount : 1));
                }
                else
                {
                    receiptDet.BoxQty = receiptlocationCount;
                }
            }
            receiptMaster.ReceiptDetails = receiptDetail;
            PrintReceiptMaster printReceiptMaster = Mapper.Map<ReceiptMaster, PrintReceiptMaster>(receiptMaster);
            IList<object> data = new List<object>();
            data.Add(printReceiptMaster);
            data.Add(printReceiptMaster.ReceiptDetails);
            return reportGen.WriteToFile(printReceiptMaster.ReceiptTemplate, data);
        }

        public string PrintPickList(string pickListNo)
        {
            PickListMaster pickListMaster = genericMgr.FindById<PickListMaster>(pickListNo);
            IList<PickListDetail> pickListDetails = genericMgr.FindAll<PickListDetail>
                ("select pl from PickListDetail as pl where pl.PickListNo=?", pickListNo);
            pickListMaster.PickListDetails = pickListDetails;
            PrintPickListMaster printPickListMaster = Mapper.Map<PickListMaster, PrintPickListMaster>(pickListMaster);
            IList<object> data = new List<object>();
            data.Add(printPickListMaster);
            data.Add(printPickListMaster.PickListDetails);
            return reportGen.WriteToFile("PickList.xls", data);
        }

        public string PrintHu(string huId, string userFullName)
        {
            Hu hu = genericMgr.FindById<Hu>(huId);
            string huTemplate = hu.HuTemplate;
            if (string.IsNullOrWhiteSpace(huTemplate))
            {
                huTemplate = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.DefaultBarCodeTemplate);
            }
            IList<PrintHu> huList = new List<PrintHu>();
            PrintHu printHu = Mapper.Map<Hu, PrintHu>(hu);
            if (!string.IsNullOrWhiteSpace(hu.ManufactureParty))
            {
                printHu.ManufacturePartyDescription = genericMgr.FindById<Party>(hu.ManufactureParty).Name;
            }
            if (!string.IsNullOrWhiteSpace(hu.Direction))
            {
                printHu.Direction = this.genericMgr.FindById<HuTo>(hu.Direction).CodeDescription;
            }
            huList.Add(printHu);
            IList<object> data = new List<object>();
            data.Add(huList);
            data.Add(userFullName);
            return reportGen.WriteToFile(huTemplate, data);
        }

        public void SetHuTo(IList<HuToMapping> huToMappingList, MrpMiPlan mrpMiPlan)
        {
            foreach (var huToMapping in huToMappingList)
            {
                if (huToMapping.FgList == null)
                {
                    if (!string.IsNullOrWhiteSpace(huToMapping.Fg))
                    {
                        var item = this.itemMgr.GetCacheItem(huToMapping.Fg);
                        //支持断面和车型
                        if (item.ItemCategory == "MODEL" || item.ItemCategory == "ZHDM")
                        {
                            huToMapping.FgList = this.bomMgr.GetFlatBomDetail(huToMapping.Fg, DateTime.Now).Select(p => p.Item).ToList();
                        }
                        else
                        {
                            huToMapping.FgList = new List<string>();
                            huToMapping.FgList.Add(huToMapping.Fg);
                        }
                    }
                    else
                    {
                        huToMapping.FgList = new List<string>();
                    }
                }
            }

            var sourcePartyHuToMappings = huToMappingList.Where(p => p.Party == mrpMiPlan.SourceParty);
            //匹配胶料
            var itemHuToMappings = sourcePartyHuToMappings.Where(p => p.Item == mrpMiPlan.Item);
            if (itemHuToMappings != null && itemHuToMappings.Count() > 0)
            {
                //匹配来源物料
                var parentItemHuToMappings = itemHuToMappings.Where(p => p.FgList.Contains(mrpMiPlan.ParentItem));
                if (parentItemHuToMappings != null && parentItemHuToMappings.Count() > 0)
                {
                    //匹配来源路线
                    var sourceFlowHuToMapping = parentItemHuToMappings.Where(p => p.Flow == mrpMiPlan.SourceFlow).FirstOrDefault();
                    if (sourceFlowHuToMapping != null)
                    {
                        //最严格的匹配:匹配了来源路线
                        mrpMiPlan.HuTo = sourceFlowHuToMapping.HuTo;
                    }
                    else
                    {
                        //没有匹配上来源路线,找来源路线为空的
                        var flowHuToMapping = parentItemHuToMappings.Where(p => string.IsNullOrWhiteSpace(p.Flow)).FirstOrDefault();
                        if (flowHuToMapping != null)
                        {
                            //匹配上了来源路线为空的
                            mrpMiPlan.HuTo = flowHuToMapping.HuTo;
                        }
                        else
                        {
                            //没有匹配上,找上一级的匹配:来源物料为空的
                            var parentItemHuToMapping = itemHuToMappings.Where(p => p.FgList.Count() == 0).FirstOrDefault();
                            if (parentItemHuToMapping != null)
                            {
                                //找到为空的物料 就返回
                                mrpMiPlan.HuTo = parentItemHuToMapping.HuTo;
                            }
                            else
                            {
                                //没有找到取默认的
                                MatchHutoMapping(sourcePartyHuToMappings, mrpMiPlan);
                            }
                        }
                    }
                }
                else
                {
                    //匹配来源物料为空
                    var parentItemHuToMapping = itemHuToMappings.Where(p => p.FgList.Count() == 0).FirstOrDefault();
                    if (parentItemHuToMapping != null)
                    {
                        //找到为空的物料 就返回
                        mrpMiPlan.HuTo = parentItemHuToMapping.HuTo;
                    }
                    else
                    {
                        //没有找到取默认的
                        MatchHutoMapping(sourcePartyHuToMappings, mrpMiPlan);
                    }
                }
            }
            else
            {
                //没有找到取默认的
                MatchHutoMapping(sourcePartyHuToMappings, mrpMiPlan);
            }
        }

        private void MatchHutoMapping(IEnumerable<HuToMapping> huToMappingList, MrpMiPlan mrpMiPlan)
        {
            //胶料没有匹配上,只匹配胶料为空的来源组织
            var itemHuToMappings = huToMappingList.Where(p => string.IsNullOrWhiteSpace(p.Item));
            //匹配路线
            var sourceFlowHuToMapping = itemHuToMappings.Where(p => p.Flow == mrpMiPlan.SourceFlow).FirstOrDefault();
            if (sourceFlowHuToMapping != null)
            {
                //找到匹配路线返回
                mrpMiPlan.HuTo = sourceFlowHuToMapping.HuTo;
            }
            else
            {
                //没有匹配上路线,就匹配路线路线为空的
                sourceFlowHuToMapping = itemHuToMappings.Where(p => string.IsNullOrWhiteSpace(p.Flow)).FirstOrDefault();
                if (sourceFlowHuToMapping != null)
                {
                    //找到匹配路线为空的返回
                    mrpMiPlan.HuTo = sourceFlowHuToMapping.HuTo;
                }
                else
                {
                    //没有找到,返回默认
                }
            }
        }

        public string GetHuTo(IList<HuToMapping> huToMappingList, string flow, string item)
        {
            var huToMapping = huToMappingList.Where(p => p.Flow == flow && p.Item == item).FirstOrDefault();
            if (huToMapping == null)
            {
                huToMapping = huToMappingList.Where(p => string.IsNullOrWhiteSpace(p.Flow) && p.Item == item).FirstOrDefault();
            }
            if (huToMapping != null)
            {
                return huToMapping.HuTo;
            }
            return null;
        }
        [Transaction]
        public void AddNewCustReport(string code)
        {
            CustReportMaster custReportMaster = genericMgr.FindById<CustReportMaster>(code);
            Permission accPermission = new Permission();
            accPermission.Code = code;
            accPermission.Description = "报表管理-信息-"+custReportMaster.Name;
            accPermission.PermissionCategory = "Menu_CustReport";
            accPermission.Sequence = custReportMaster.Seq;
            genericMgr.Create(accPermission);
        }
        [Transaction]
        public void DeleteCustReport(string code)
        {
            //delete top (1) from ACC_Permission output deleted.Code, deleted.Desc1, deleted.Category, deleted.Sequence into #jjjj  where Sequence =-1
            //select top 1000 * from ACC_Permission where  Id =9584
            //select top 1000 * from ACC_RolePermission where PermissionId =9584 RolePermission
            //select top 1000 * from ACC_UserPermission where PermissionId =9584 UserPermission
            //select top 1000 * from ACC_PermissionGroupPermission where PermissionId =9584 PermissionGroupPermission
            Permission accPermission = genericMgr.FindAll<Permission>("from Permission where Code = ?", code).FirstOrDefault();
            if (accPermission != null)
            {
                this.genericMgr.Delete(accPermission);
                SqlParameter[] sqlParams = new SqlParameter[1];
                sqlParams[0] = new SqlParameter("@Id", accPermission.Id);
                this.genericMgr.ExecuteSql(@"delete from ACC_RolePermission where PermissionId = @Id;
                                        delete from ACC_UserPermission where PermissionId =@Id;
                                        delete from ACC_PermissionGroupPermission where PermissionId = @Id", sqlParams);
            }
        }
        [Transaction]
        public void UpdateCustReport(CustReportMaster custReport)
        {
            //delete top (1) from ACC_Permission output deleted.Code, deleted.Desc1, deleted.Category, deleted.Sequence into #jjjj  where Sequence =-1
            //select top 1000 * from ACC_Permission where  Id =9584
            //select top 1000 * from ACC_RolePermission where PermissionId =9584 RolePermission
            //select top 1000 * from ACC_UserPermission where PermissionId =9584 UserPermission
            //select top 1000 * from ACC_PermissionGroupPermission where PermissionId =9584 PermissionGroupPermission
            Permission accPermission = genericMgr.FindAll<Permission>("from Permission where Code = ?", custReport.Code).FirstOrDefault();
            accPermission.Description = "报表管理-信息-" + custReport.Name;
            if (custReport.IsActive && accPermission != null)
            {
                accPermission.Sequence = custReport.Seq;
            }
            else
            {
                accPermission.Sequence = -1;
            }
            genericMgr.UpdateWithTrim(accPermission);

        }
    }
}
