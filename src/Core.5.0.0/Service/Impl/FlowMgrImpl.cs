namespace com.Sconit.Service.Impl
{
    using AutoMapper;
    using Castle.Services.Transaction;
    using com.Sconit.Entity;
    using com.Sconit.Entity.BIL;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SCM;
    using com.Sconit.Utility;
    using NHibernate.Criterion;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [Transactional]
    public class FlowMgrImpl : BaseMgr, IFlowMgr
    {
        #region 变量
        public IGenericMgr genericMgr { get; set; }
        #endregion

        public IList<FlowBinding> GetFlowBinding(string flow)
        {
            DetachedCriteria detachedCriteria = DetachedCriteria.For<FlowBinding>();
            detachedCriteria.Add(Expression.Eq("MasterFlow.Code", flow));

            return this.genericMgr.FindAll<FlowBinding>(detachedCriteria);
        }



        public FlowStrategy GetFlowStrategy(string flow)
        {
            try
            {
                return this.genericMgr.FindById<FlowStrategy>(flow);
            }
            catch (Exception)
            { }
            return null;
        }

        public IList<FlowDetail> GetFlowDetailList(FlowMaster flowMaster)
        {
            return GetFlowDetailList(flowMaster, false, true);
        }

        public IList<FlowDetail> GetFlowDetailList(string flow)
        {
            return GetFlowDetailList(flow, false, true);
        }

        public IList<FlowDetail> GetFlowDetailList(FlowMaster flowMaster, bool includeInactiveDetail)
        {
            IList<FlowDetail> flowDetails = new List<FlowDetail>();
            DateTime dateTimeNow = DateTime.Now;
            DetachedCriteria criteria = DetachedCriteria.For<FlowDetail>();
            criteria.Add(Expression.Eq("Flow", flowMaster.Code));
            if (!includeInactiveDetail)
            {
                criteria.Add(Expression.Or(Expression.IsNull("StartDate"), Expression.Le("StartDate", dateTimeNow)));
                criteria.Add(Expression.Or(Expression.IsNull("EndDate"), Expression.Ge("EndDate", dateTimeNow)));
            }
            criteria.AddOrder(Order.Asc("Sequence"));
            criteria.AddOrder(Order.Asc("Id"));
            flowDetails = this.genericMgr.FindAll<FlowDetail>(criteria);

            foreach (var flowDetail in flowDetails)
            {
                flowDetail.CurrentFlowMaster = flowMaster;
                flowDetail.DefaultLocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom)
                       ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                flowDetail.DefaultLocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo)
                    ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                flowDetail.DefaultExtraLocationFrom = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationFrom)
                    ? flowDetail.CurrentFlowMaster.ExtraLocationFrom : flowDetail.ExtraLocationFrom;
                flowDetail.DefaultExtraLocationTo = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationTo)
                    ? flowDetail.CurrentFlowMaster.ExtraLocationTo : flowDetail.ExtraLocationTo;
            }
            return flowDetails;
        }

        public IList<FlowDetail> GetFlowDetailList(string flow, bool includeInactiveDetail)
        {
            var flowMaster = this.genericMgr.FindById<Entity.SCM.FlowMaster>(flow);
            return GetFlowDetailList(flowMaster, includeInactiveDetail);
        }

        public IList<FlowDetail> GetFlowDetailList(FlowMaster flowMaster, bool includeInactiveDetail, bool includeReferenceFlow)
        {
            var flowDetails = this.GetFlowDetailList(flowMaster, includeInactiveDetail).ToList();

            if (includeReferenceFlow)
            {
                //int maxFlowDetaiId = flowDetails.Count() == 0 ? 0 : flowDetails.Max(det => det.Id);
                if (!string.IsNullOrWhiteSpace(flowMaster.ReferenceFlow))
                {
                    var refFlowDetails = this.GetFlowDetailList(flowMaster.ReferenceFlow, includeInactiveDetail);
                    var newFlowDetails = Mapper.Map<IList<FlowDetail>, IList<FlowDetail>>(refFlowDetails);
                    foreach (var flowDetail in newFlowDetails)
                    {
                        flowDetail.CurrentFlowMaster = flowMaster;
                        flowDetail.LocationFrom = null;   //来源库位制空，取头上的库位
                        flowDetail.LocationTo = null;    //目的库位制空，取头上的库位
                        flowDetail.DefaultLocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom)
                               ? flowDetail.CurrentFlowMaster.LocationFrom : flowDetail.LocationFrom;
                        flowDetail.DefaultLocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo)
                            ? flowDetail.CurrentFlowMaster.LocationTo : flowDetail.LocationTo;
                        flowDetail.DefaultExtraLocationFrom = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationFrom)
                            ? flowDetail.CurrentFlowMaster.ExtraLocationFrom : flowDetail.ExtraLocationFrom;
                        flowDetail.DefaultExtraLocationTo = string.IsNullOrWhiteSpace(flowDetail.ExtraLocationTo)
                            ? flowDetail.CurrentFlowMaster.ExtraLocationTo : flowDetail.ExtraLocationTo;
                        //if (maxFlowDetaiId > 0)
                        //{
                        //    flowDetail.Id = maxFlowDetaiId++; //为了区分不同的ShipPlan，要给flowdetailId赋不同的值。
                        //}
                    }
                    flowDetails.AddRange(newFlowDetails);
                }
            }
            return flowDetails;
        }

        /// <summary>
        /// 路线引用只能引用一次,避免循环引用
        /// </summary>
        public IList<FlowDetail> GetFlowDetailList(string flowCode, bool includeInactiveDetail, bool includeReferenceFlow)
        {
            var flowMaster = this.genericMgr.FindById<Entity.SCM.FlowMaster>(flowCode);
            return GetFlowDetailList(flowMaster, includeInactiveDetail, includeReferenceFlow);
        }

        public FlowMaster GetReverseFlow(FlowMaster flow, IList<string> itemCodeList)
        {
            FlowMaster reverseFlow = new FlowMaster();
            Mapper.Map(flow, reverseFlow);
            reverseFlow.PartyFrom = flow.PartyTo;
            reverseFlow.PartyTo = flow.PartyFrom;
            reverseFlow.IsCheckPartyFromAuthority = flow.IsCheckPartyToAuthority;
            reverseFlow.IsCheckPartyToAuthority = flow.IsCheckPartyFromAuthority;
            reverseFlow.ShipFrom = flow.ShipTo;
            reverseFlow.ShipTo = flow.ShipFrom;
            reverseFlow.LocationFrom = flow.LocationTo;
            reverseFlow.LocationTo = flow.LocationFrom;
            reverseFlow.IsShipScanHu = flow.IsReceiveScanHu;
            reverseFlow.IsReceiveScanHu = flow.IsShipScanHu;
            reverseFlow.IsShipFifo = flow.IsReceiveFifo;
            reverseFlow.IsReceiveFifo = flow.IsShipFifo;
            reverseFlow.IsShipExceed = flow.IsReceiveExceed;
            reverseFlow.IsReceiveExceed = flow.IsShipExceed;
            //reverseFlow.IsShipFulfillUC = flow.IsReceiveFulfillUC;
            //reverseFlow.IsReceiveFulfillUC = flow.IsShipFulfillUC;
            reverseFlow.IsInspect = flow.IsRejectInspect;
            reverseFlow.IsRejectInspect = flow.IsInspect;
            reverseFlow.IsCreatePickList = false;
            reverseFlow.IsShipByOrder = true;
            //以后有什么字段再加

            #region 路线明细
            if (flow.FlowDetails == null || flow.FlowDetails.Count == 0)
            {
                string hql = "from FlowDetail where Flow = ?";
                IList<object> parm = new List<object>();
                parm.Add(flow.Code);
                if (itemCodeList != null && itemCodeList.Count() > 0)
                {
                    string whereHql = string.Empty;
                    foreach (string itemCode in itemCodeList.Distinct())
                    {
                        if (whereHql == string.Empty)
                        {
                            whereHql = " and Item in (?";
                        }
                        else
                        {
                            whereHql += ",?";
                        }
                        parm.Add(itemCode);
                    }
                    whereHql += ")";
                    hql += whereHql;
                }
                hql += " order by Sequence";

                flow.FlowDetails = this.genericMgr.FindAll<FlowDetail>(hql, parm.ToArray());
            }

            if (flow.FlowDetails != null && flow.FlowDetails.Count > 0)
            {
                IList<FlowDetail> reverseFlowDetailList = new List<FlowDetail>();
                foreach (FlowDetail flowDetail in flow.FlowDetails)
                {
                    FlowDetail reverseFlowDetail = new FlowDetail();
                    Mapper.Map(flowDetail, reverseFlowDetail);
                    reverseFlowDetail.LocationFrom = flowDetail.LocationTo;
                    reverseFlowDetail.LocationTo = flowDetail.LocationFrom;
                    reverseFlowDetail.IsRejectInspect = flowDetail.IsInspect;
                    reverseFlowDetail.IsInspect = flowDetail.IsRejectInspect;
                    //PartyFrom?
                    reverseFlowDetailList.Add(reverseFlowDetail);
                }
                reverseFlow.FlowDetails = reverseFlowDetailList;
            }
            #endregion

            return reverseFlow;
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateFlow(FlowMaster flowMaster)
        {
            genericMgr.Create(flowMaster);

            #region flow strategy
            FlowStrategy flowStrategy = new FlowStrategy();
            flowStrategy.Flow = flowMaster.Code;
            flowStrategy.Strategy = flowMaster.FlowStrategy;
            flowStrategy.Description = flowMaster.Description;
            //默认取小时，以后再改
            flowStrategy.TimeUnit = CodeMaster.TimeUnit.Hour;
            genericMgr.Create(flowStrategy);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateFlowStrategy(FlowStrategy flowstrategy)
        {
            FlowStrategy oldFlowStrategy = genericMgr.FindById<FlowStrategy>(flowstrategy.Flow);
            if (oldFlowStrategy.Strategy != flowstrategy.Strategy)
            {
                #region 反向更新FlowMaster上的策略
                FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(flowstrategy.Flow);
                flowMaster.FlowStrategy = flowstrategy.Strategy;
                genericMgr.Update(flowMaster);
            }

            //默认取小时，以后再改
            Mapper.Map<FlowStrategy, FlowStrategy>(flowstrategy, oldFlowStrategy);
            //oldFlowStrategy.TimeUnit = CodeMaster.TimeUnit.Hour;
            genericMgr.Update(oldFlowStrategy);

                #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteFlow(string flow)
        {
            FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(flow);

            #region 删除绑定
            IList<FlowBinding> flowBindingList = GetFlowBinding(flow);
            if (flowBindingList != null && flowBindingList.Count > 0)
            {
                genericMgr.Delete<FlowBinding>(flowBindingList);
            }
            #endregion

            #region 删除策略
            FlowStrategy flowStrategy = GetFlowStrategy(flow);
            genericMgr.Delete(flowStrategy);
            #endregion

            #region 删除明细
            IList<FlowDetail> flowDetailList = GetFlowDetailList(flow, true);
            if (flowDetailList != null && flowDetailList.Count > 0)
            {
                genericMgr.Delete<FlowDetail>(flowDetailList);
            }
            #endregion

            #region 删除头
            genericMgr.Delete(flowMaster);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateFlowDetail(FlowDetail flowDetail)
        {
            IList<ItemPackage> itemPackageList = genericMgr.FindAll<ItemPackage>("from ItemPackage as i where i.Item = ? and i.UnitCount = ?", new object[] { flowDetail.Item, flowDetail.UnitCount });
            if (itemPackageList == null || itemPackageList.Count == 0)
            {
                ItemPackage itemPackage = new ItemPackage();
                itemPackage.Item = flowDetail.Item;
                itemPackage.UnitCount = flowDetail.UnitCount;
                itemPackage.Description = string.IsNullOrEmpty(flowDetail.UnitCountDescription) ? string.Empty : flowDetail.UnitCountDescription;
                genericMgr.CreateWithTrim(itemPackage);
            }
            genericMgr.CreateWithTrim(flowDetail);
        }


        [Transaction(TransactionMode.Requires)]
        public void UpdateFlowDetail(FlowDetail flowDetail)
        {
            IList<ItemPackage> itemPackageList = genericMgr.FindAll<ItemPackage>("from ItemPackage as i where i.Item = ? and i.UnitCount = ?", new object[] { flowDetail.Item, flowDetail.UnitCount });
            if (itemPackageList == null || itemPackageList.Count == 0)
            {
                ItemPackage itemPackage = new ItemPackage();
                itemPackage.Item = flowDetail.Item;
                itemPackage.UnitCount = flowDetail.UnitCount;
                itemPackage.Description = string.IsNullOrEmpty(flowDetail.UnitCountDescription) ? string.Empty : flowDetail.UnitCountDescription;
                genericMgr.CreateWithTrim(itemPackage);
            }
            genericMgr.UpdateWithTrim(flowDetail);
        }

        public FlowMaster GetAuthenticFlow(string flowCode)
        {
            var flowMaster = this.genericMgr.FindById<FlowMaster>(flowCode);
            if (Utility.SecurityHelper.HasPermission(flowMaster))
            {
                return flowMaster;
            }
            return null;
        }

        private IList<FlowDetail> GetFlowDetailList(string refFlowCode, bool includeInactive, bool includeReferenceFlow, FlowMaster baseFlow)
        {
            if (refFlowCode != baseFlow.Code)
            {
                var flowDetails = this.GetFlowDetailList(refFlowCode, includeInactive).ToList();

                if (includeReferenceFlow)
                {
                    var refflowMaster = this.genericMgr.FindById<Entity.SCM.FlowMaster>(refFlowCode);
                    if (!string.IsNullOrWhiteSpace(refflowMaster.ReferenceFlow) && refflowMaster.IsActive)
                    {
                        var refFlowDetails = this.GetFlowDetailList(refflowMaster.ReferenceFlow, false, true, baseFlow);
                        var newFlowDetails = Mapper.Map<IList<FlowDetail>, IList<FlowDetail>>(refFlowDetails);
                        foreach (var detail in newFlowDetails)
                        {
                            detail.CurrentFlowMaster = baseFlow;
                        }
                        flowDetails.AddRange(newFlowDetails);
                    }
                }
                return flowDetails;
            }
            return new List<FlowDetail>();
        }


        [Transaction(TransactionMode.Requires)]
        public void ImportFlow(Stream inputStream, CodeMaster.OrderType flowType)
        {
            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 10);

            #region 列定义
            // FlowMaster
            int colType = 1;//路线类型
            int colCode = 2; // 路线代码
            int colDesc = 3; // 路线描述


            // FlowDet
            int colItem = 4; // 物料
            int colUom = 5; // 单位
            int colUnitCount = 6; // 单包装
            int colUcDesc = 7; // 单包装
            int colMinStock = 8;//
            int colMaxStock = 9;//

            // FlowStrategy
            int colLeadTime = 10; // 提前期
            #endregion

            var errorMessage = new BusinessException();
            int colCount = 10;
            List<List<string>> rowDataList = new List<List<string>>();
            var items = this.genericMgr.FindAll<Item>().ToDictionary(d => d.Code, d => d);
            #region 读取数据
            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 1, 8))
                {
                    break;//边界
                }
                colCount++;

                var rowData = new List<string>();

                #region FlowMaster
                rowData.Add("0");
                rowData.Add(((int)flowType).ToString());
                rowData.Add(ImportHelper.GetCellStringValue(row.GetCell(colCode)));
                if (string.IsNullOrWhiteSpace(rowData[2]))
                {
                    rowData[0] = "1";
                    errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, Resources.EXT.ServiceLan.FlowCodeShouldNotEmpty, colCount.ToString()));
                }
                rowData.Add(ImportHelper.GetCellStringValue(row.GetCell(colDesc)));
                if (string.IsNullOrWhiteSpace(rowData[3]))
                {
                    rowData[0] = "1";
                    errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, Resources.EXT.ServiceLan.FlowDescShouldNotEmpty, colCount.ToString()));
                }
                #endregion

                #region FlowDetail
                Item item = null;
                rowData.Add(ImportHelper.GetCellStringValue(row.GetCell(colItem)));
                if (!string.IsNullOrWhiteSpace(rowData[4]))
                {
                    item = items.ValueOrDefault(rowData[4]);
                }
                if (item == null)
                {
                    rowData[0] = "1";
                    errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, Resources.EXT.ServiceLan.TheSpecialLineItemNotFound,
                        colCount.ToString(), rowData[4]));
                    continue;
                }

                rowData.Add(ImportHelper.GetCellStringValue(row.GetCell(colUom)));
                if (string.IsNullOrWhiteSpace(rowData[5]))
                {
                    rowData[5] = item.Uom;
                }
                else
                {
                    try
                    {
                        Uom uom = this.genericMgr.FindById<Uom>(rowData[5].ToUpper());
                        rowData[5] = uom.Code;
                    }
                    catch (Exception)
                    {
                        rowData[0] = "1";
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, Resources.EXT.ServiceLan.TheSpecialLineUomNotFound,
                            colCount.ToString(), rowData[5]));
                    }
                }
                rowData.Add(ImportHelper.GetCellStringValue(row.GetCell(colUnitCount)));
                if (string.IsNullOrWhiteSpace(rowData[6]))
                {
                    rowData[6] = item.UnitCount.ToString();
                }
                else
                {
                    decimal unitCount = item.UnitCount;
                    if (!decimal.TryParse(rowData[6], out unitCount))
                    {
                        rowData[0] = "1";
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, Resources.EXT.ServiceLan.TheSpecialLineUCIsNotNum, colCount.ToString()));
                    }
                    rowData[6] = unitCount == 0 ? item.UnitCount.ToString() : unitCount.ToString();
                }
                //7
                rowData.Add(ImportHelper.GetCellStringValue(row.GetCell(colUcDesc)));

                rowData.Add(ImportHelper.GetCellStringValue(row.GetCell(colMinStock)));
                if (string.IsNullOrWhiteSpace(rowData[8]))
                {
                    rowData[8] = "0";
                }
                else
                {
                    decimal decVal = 0m;
                    if (!decimal.TryParse(rowData[8], out decVal))
                    {
                        rowData[0] = "1";
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, Resources.EXT.ServiceLan.TheSpecialLineSafeInvIsNotNum, colCount.ToString()));
                    }
                    rowData[8] = decVal.ToString();
                }

                rowData.Add(ImportHelper.GetCellStringValue(row.GetCell(colMaxStock)));
                if (string.IsNullOrWhiteSpace(rowData[9]))
                {
                    rowData[9] = "0";
                }
                else
                {
                    decimal decVal = 0m;
                    if (!decimal.TryParse(rowData[9], out decVal))
                    {
                        rowData[0] = "1";
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, Resources.EXT.ServiceLan.TheSpecialLineMaxInvIsNotNum, colCount.ToString()));
                    }
                    rowData[9] = decVal.ToString();
                }
                #endregion

                #region FlowStrategy
                rowData.Add(ImportHelper.GetCellStringValue(row.GetCell(colLeadTime)));
                if (string.IsNullOrWhiteSpace(rowData[10]))
                {
                    rowData[10] = "0";
                }
                else
                {
                    decimal leadTimeVal = 0m;
                    if (!decimal.TryParse(rowData[10], out leadTimeVal))
                    {
                        rowData[0] = "1";
                        errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error, Resources.EXT.ServiceLan.TheSpecialLineLeantimeIsNotNum, colCount.ToString()));
                    }
                    rowData[10] = leadTimeVal.ToString();
                }
                #endregion

                rowData.Add(item.Description);//11
                rowData.Add(item.Uom);//12
                rowData.Add(item.ReferenceCode);//13

                rowDataList.Add(rowData);
            }
            #endregion

            #region 验证

            //Excle中重复性验证
            var flowItems = rowDataList.Where(p => p[0] == "0")
                            .Select(p => new
                           {
                               Type = (CodeMaster.OrderType)(int.Parse(p[1])),
                               Flow = p[2],
                               FlowDescription = p[3],
                               Item = p[4],
                               Uom = p[5],
                               UnitCount = decimal.Parse(p[6]),
                               UcDesc = p[7],
                               MinStock = decimal.Parse(p[8]),
                               MaxStock = decimal.Parse(p[9]),
                               LeadTime = decimal.Parse(p[10]),
                               ItemDescription = p[11],
                               BaseUom = p[12],
                               ReferenceCode = p[13]
                           });

            if (flowItems == null || flowItems.Count() == 0)
            {
                errorMessage.AddMessage(Resources.EXT.ServiceLan.TheImportIsNotEffect);
                throw errorMessage;
            }
            var flowItemGroup = flowItems.GroupBy(p => new { p.Flow, p.Item, p.Uom, p.UnitCount }, (k, g) => new { k, Count = g.Count() })
                .Where(p => p.Count > 1).Select(p => new { p.k.Flow, p.k.Item });
            foreach (var flowItem in flowItemGroup)
            {
                errorMessage.AddMessage(new Message(com.Sconit.CodeMaster.MessageType.Error,
                    Resources.EXT.ServiceLan.DuplicateFlowDetail, flowItem.Flow, flowItem.Item));
            }

            var distinctFlowItems = flowItems.GroupBy(p => new { p.Flow, p.Item }, (k, g) => new { k, j = g.First() })
                .Select(p => p.j);

            var excelFlowMasterList = distinctFlowItems.GroupBy(p => p.Flow, (k, g) => new
                                    {
                                        Code = k,
                                        List = g
                                    });
            var flowMasterDic = this.genericMgr.FindAllIn<FlowMaster>("from FlowMaster where Code in(?",
                excelFlowMasterList.Select(p => p.Code)).ToDictionary(d => d.Code, d => d);

            var flowStrategyDic = this.genericMgr.FindAllIn<FlowStrategy>("from FlowStrategy where Flow in(?",
                excelFlowMasterList.Select(p => p.Code)).ToDictionary(d => d.Flow, d => d);

            var allParty = this.genericMgr.FindAll<Party>().ToDictionary(d => d.Code, d => d.Code);
            var allPriceList = this.genericMgr.FindAll<PriceListMaster>().ToDictionary(d => d.Code, d => d.Code);
            var allAddress = this.genericMgr.FindAll<Address>().ToDictionary(d => d.Code, d => d.Code);
            var allLocation = this.genericMgr.FindAll<Location>().ToDictionary(d => d.Code, d => d.Code);

            foreach (var excelFlowMaster in excelFlowMasterList)
            {
                var firstFlowItem = excelFlowMaster.List.First();
                var flowMaster = flowMasterDic.ValueOrDefault(excelFlowMaster.Code);
                var flowStrategy = flowStrategyDic.ValueOrDefault(excelFlowMaster.Code);

                if (flowType != firstFlowItem.Type)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.FlowTypeIsNotCorrect, firstFlowItem.Type.ToString());
                    continue;
                }
                #region flowMaster
                if (flowMaster == null)
                {
                    #region FlowMaster赋值
                    var flowCodeSplits = excelFlowMaster.Code.Split('-');
                    var flowDescriptionSplits = firstFlowItem.FlowDescription.Split('-');
                    var newFlowMaster = new FlowMaster();
                    newFlowMaster.Code = excelFlowMaster.Code;
                    newFlowMaster.Type = firstFlowItem.Type;
                    newFlowMaster.Description = firstFlowItem.FlowDescription;

                    newFlowMaster.IsListDet = true;
                    newFlowMaster.IsPrintOrder = true;
                    newFlowMaster.IsPrintAsn = true;
                    newFlowMaster.IsPrintRceipt = true;
                    newFlowMaster.IsShipExceed = true;
                    newFlowMaster.IsShipFifo = true;
                    newFlowMaster.IsAllowProvEstRec = true;
                    newFlowMaster.ReceiveGapTo = CodeMaster.ReceiveGapTo.AdjectLocFromInv;
                    //Code	Desc1	IsActive	Type	RefFlow	PartyFrom	PartyTo	ShipFrom	ShipTo	LocFrom	LocTo	BillAddr	
                    //PriceList	Routing	ReturnRouting	Dock	IsAutoCreate	IsAutoRelease	IsAutoStart	IsAutoShip	IsAutoReceive
                    //IsAutoBill	IsListDet	IsManualCreateDet	IsListPrice	IsPrintOrder	IsPrintAsn	IsPrintRcpt	IsShipExceed	
                    //IsRecExceed	IsOrderFulfillUC	IsShipFulfillUC	IsRecFulfillUC	IsShipScanHu	IsRecScanHu	IsCreatePL	
                    //IsInspect	IsShipFifo	IsRejInspect	IsRecFifo	IsShipByOrder	IsAsnUniqueRec	IsMRP	IsCheckPartyFromAuth	
                    //IsCheckPartyToAuth	RecGapTo	RecTemplate	OrderTemplate	AsnTemplate	HuTemplate	BillTerm	CreateHuOpt	
                    //MaxOrderCount	MRPOpt	IsPause	PauseTime	FlowStrategy	ExtraDmdSource	PickStrategy	CreateUser	CreateUserNm
                    //CreateDate	LastModifyUser	LastModifyUserNm	LastModifyDate	DAUAT	LastRefreshDate	ResourceGroup	
                    //IsAllowProvEstRec	UcDeviation	OrderDeviation


                    if (firstFlowItem.Type == CodeMaster.OrderType.Procurement
                        || firstFlowItem.Type == CodeMaster.OrderType.CustomerGoods
                        || firstFlowItem.Type == CodeMaster.OrderType.SubContract)
                    {
                        newFlowMaster.PartyFrom = flowCodeSplits[0];
                        var location = this.genericMgr.FindById<Location>(flowCodeSplits[1]);
                        newFlowMaster.PartyTo = location.Region;
                        if (firstFlowItem.Type == CodeMaster.OrderType.SubContract)
                        {
                            newFlowMaster.LocationFrom = newFlowMaster.PartyFrom;
                        }
                        newFlowMaster.LocationTo = location.Code;
                        newFlowMaster.BillAddress = newFlowMaster.PartyFrom;
                        newFlowMaster.PriceList = newFlowMaster.PartyFrom;
                        newFlowMaster.IsReceiveScanHu = true;
                        newFlowMaster.OrderTemplate = "ORD_Purchase.xls";
                        newFlowMaster.AsnTemplate = "ASN_Purchase.xls";
                        newFlowMaster.ReceiptTemplate = "REC_Receipt.xls";
                        newFlowMaster.HuTemplate = "BarCodePurchase2D.xls";
                        newFlowMaster.BillTerm = CodeMaster.OrderBillTerm.ReceivingSettlement;
                        newFlowMaster.CreateHuOption = CodeMaster.CreateHuOption.None;
                        newFlowMaster.IsListPrice = true;
                        newFlowMaster.IsAsnUniqueReceive = true;
                        newFlowMaster.IsAllowProvEstRec = true;
                    }
                    else if (firstFlowItem.Type == CodeMaster.OrderType.Distribution
                        || firstFlowItem.Type == CodeMaster.OrderType.SubContractTransfer)
                    {
                        var location = this.genericMgr.FindById<Location>(flowCodeSplits[0]);
                        newFlowMaster.LocationFrom = location.Code;
                        newFlowMaster.PartyFrom = location.Region;
                        newFlowMaster.PartyTo = flowCodeSplits[1];
                        if (firstFlowItem.Type == CodeMaster.OrderType.SubContractTransfer)
                        {
                            newFlowMaster.LocationTo = newFlowMaster.PartyTo;
                            newFlowMaster.OrderTemplate = "ASN_Transfer.xls";
                            newFlowMaster.AsnTemplate = "ASN_Transfer.xls";
                            newFlowMaster.ReceiptTemplate = "REC_InvOut.xls";
                            newFlowMaster.HuTemplate = "BarCodeMI2D.xls";
                        }
                        else
                        {
                            newFlowMaster.BillAddress = newFlowMaster.PartyTo;
                            newFlowMaster.PriceList = newFlowMaster.PartyTo;
                            newFlowMaster.OrderTemplate = "ORD_Sale.xls";
                            newFlowMaster.AsnTemplate = "ASN_Sale.xls";
                            newFlowMaster.ReceiptTemplate = "REC_Sale.xls";
                            newFlowMaster.HuTemplate = "BarCodeFI2D.xls";
                            newFlowMaster.BillTerm = CodeMaster.OrderBillTerm.ReceivingSettlement;
                            newFlowMaster.CreateHuOption = CodeMaster.CreateHuOption.None;
                        }
                        newFlowMaster.IsRejectInspect = true;
                        newFlowMaster.IsShipScanHu = true;
                        newFlowMaster.IsCreatePickList = true;

                    }
                    else if (firstFlowItem.Type == CodeMaster.OrderType.Production)
                    {
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheProdLineNotFound, excelFlowMaster.Code);
                    }
                    else if (firstFlowItem.Type == CodeMaster.OrderType.Transfer)
                    {
                        var locationFrom = this.genericMgr.FindById<Location>(flowCodeSplits[0]);
                        var locationTo = this.genericMgr.FindById<Location>(flowCodeSplits[1]);
                        newFlowMaster.PartyFrom = locationFrom.Region;
                        newFlowMaster.PartyTo = locationTo.Region;
                        newFlowMaster.LocationFrom = locationFrom.Code;
                        newFlowMaster.LocationTo = locationTo.Code;
                        newFlowMaster.IsShipScanHu = true;
                        newFlowMaster.IsReceiveScanHu = true;
                        newFlowMaster.IsCreatePickList = true;
                        newFlowMaster.OrderTemplate = "ASN_Transfer.xls";
                        newFlowMaster.AsnTemplate = "ASN_Transfer.xls";
                        newFlowMaster.ReceiptTemplate = "REC_InvOut.xls";
                        newFlowMaster.HuTemplate = "BarCodeEX2D.xls";
                        newFlowMaster.CreateHuOption = CodeMaster.CreateHuOption.None;
                    }
                    newFlowMaster.ShipFrom = newFlowMaster.PartyFrom;
                    newFlowMaster.ShipTo = newFlowMaster.PartyTo;
                    #endregion

                    #region 校验
                    bool isMark = false;
                    if (!allParty.ContainsKey(newFlowMaster.PartyFrom))
                    {
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.ThePartyFromNotFound, newFlowMaster.PartyFrom);
                        isMark = true;
                    }
                    if (!allParty.ContainsKey(newFlowMaster.PartyTo))
                    {
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.ThePartyToNotFound, newFlowMaster.PartyTo);
                        isMark = true;
                    }
                    if (!string.IsNullOrWhiteSpace(newFlowMaster.LocationFrom) && !allLocation.ContainsKey(newFlowMaster.LocationFrom))
                    {
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheLocFromNotFound, newFlowMaster.LocationFrom);
                        isMark = true;
                    }
                    if (!string.IsNullOrWhiteSpace(newFlowMaster.LocationTo) && !allLocation.ContainsKey(newFlowMaster.LocationTo))
                    {
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheLocToNotFound, newFlowMaster.LocationTo);
                        isMark = true;
                    }
                    if (!string.IsNullOrWhiteSpace(newFlowMaster.PriceList) && !allPriceList.ContainsKey(newFlowMaster.PriceList))
                    {
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.ThePriceListNotFound, newFlowMaster.PriceList);
                        isMark = true;
                    }
                    if (!string.IsNullOrWhiteSpace(newFlowMaster.BillAddress) && !allAddress.ContainsKey(newFlowMaster.BillAddress))
                    {
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheBillAddrNotFound, newFlowMaster.BillAddress);
                        isMark = true;
                    }
                    if (!allAddress.ContainsKey(newFlowMaster.ShipFrom))
                    {
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheShipAddrNotFound, newFlowMaster.ShipFrom);
                        isMark = true;
                    }
                    if (!allAddress.ContainsKey(newFlowMaster.ShipTo))
                    {
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheShipToNotFound, newFlowMaster.ShipTo);
                        isMark = true;
                    }
                    #endregion

                    if (!isMark && !errorMessage.HasMessage)
                    {
                        try
                        {
                            this.genericMgr.Create(newFlowMaster);
                        }
                        catch (Exception ex)
                        {
                            errorMessage.AddMessage(ex.Message);
                        }
                    }
                }
                else
                {
                    if (flowMaster.Description != firstFlowItem.FlowDescription && !errorMessage.HasMessage)
                    {
                        flowMaster.Description = firstFlowItem.FlowDescription;
                        this.genericMgr.Update(flowMaster);
                    }
                }
                #endregion

                #region FlowStrategy
                if (!errorMessage.HasMessage)
                {
                    if (flowStrategy == null)
                    {
                        //Flow	Strategy	Desc1	LeadTime	EmLeadTime	TimeUnit	
                        var newFlowStrategy = new FlowStrategy();
                        newFlowStrategy.Flow = excelFlowMaster.Code;
                        newFlowStrategy.Description = firstFlowItem.FlowDescription;
                        newFlowStrategy.LeadTime = firstFlowItem.LeadTime;
                        newFlowStrategy.TimeUnit = CodeMaster.TimeUnit.Hour;
                        this.genericMgr.Create(newFlowStrategy);
                    }
                    else
                    {
                        flowStrategy.LeadTime = firstFlowItem.LeadTime;
                        this.genericMgr.Update(flowStrategy);
                    }
                }
                #endregion

                #region FlowDetail
                if (!errorMessage.HasMessage)
                {
                    var flowDetailDic = this.genericMgr.FindAllIn<FlowDetail>
                        ("from FlowDetail where Flow = ? and Item in(? ",
                        excelFlowMaster.List.Select(p => p.Item), new object[] { excelFlowMaster.Code })
                        .GroupBy(p => p.Item, (k, g) => new { Item = k, FlowDetails = g })
                        .ToDictionary(d => d.Item, d => d.FlowDetails);
                    var maxSeq = 10;
                    var maxItemSeq = this.genericMgr.FindAllIn<FlowDetail>
                        ("from FlowDetail where Flow = ? ", new object[] { excelFlowMaster.Code }).Select(p => p.Sequence).Max();
                    if (flowDetailDic.Values != null && flowDetailDic.Values.Count > 0)
                    {
                        maxSeq = flowDetailDic.Values.Max(p => p.Max(q => q.Sequence));
                    }
                    foreach (var flowItem in excelFlowMaster.List)
                    {
                        //Id	Flow	Seq	Item	RefItemCode	Uom	BaseUom	UC	UCDesc	MinUC	StartDate	EndDate	Bom	LocFrom	LocTo	
                        //BillAddr	PriceList	Routing	ReturnRouting	Strategy	ProdScan	Container	ContainerDesc	IsAutoCreate	
                        //IsInspect	IsRejInspect	SafeStock	MaxStock	MinLotSize	OrderLotSize	RecLotSize	BatchSize	RoundUpOpt	
                        //BillTerm	MrpWeight	MrpTotal	MrpTotalAdj	ExtraDmdSource	PickStrategy	EBELN	EBELP	CreateUser	
                        //CreateUserNm	CreateDate	LastModifyUser	LastModifyUserNm	LastModifyDate	BinTo	IsChangeUC	Machine	Uom1
                        //var item = this.genericMgr.FindById<Item>(flowItem.Item);
                        try
                        {
                            //todo check Details
                            var flowDetail = (flowDetailDic.ValueOrDefault(flowItem.Item) ?? new List<FlowDetail>())
                                .FirstOrDefault(p => p.UnitCount == flowItem.UnitCount);
                            if (flowDetail == null)
                            {
                                var newFlowDetail = new FlowDetail();
                                newFlowDetail.Flow = flowItem.Flow;
                                newFlowDetail.Item = flowItem.Item;
                                newFlowDetail.BaseUom = flowItem.BaseUom;
                                newFlowDetail.Uom = flowItem.Uom;
                                //newFlowDetail.ReferenceItemCode = flowItem.ReferenceCode;
                                newFlowDetail.UnitCount = flowItem.UnitCount;
                                newFlowDetail.MinUnitCount = flowItem.UnitCount;
                                newFlowDetail.UnitCountDescription = flowItem.UcDesc;
                                newFlowDetail.Sequence = maxSeq+maxItemSeq;
                                newFlowDetail.SafeStock = flowItem.MinStock;
                                newFlowDetail.MaxStock = flowItem.MaxStock;
                                newFlowDetail.MrpWeight = 1;
                                newFlowDetail.RoundUpOption = Sconit.CodeMaster.RoundUpOption.ToUp;
                                maxSeq += 10;
                                this.genericMgr.Create(newFlowDetail);
                            }
                            else
                            {
                                flowDetail.BaseUom = flowItem.BaseUom;
                                flowDetail.Uom = flowItem.Uom;
                                flowDetail.UnitCount = flowItem.UnitCount;
                                flowDetail.MinUnitCount = flowItem.UnitCount;
                                flowDetail.UnitCountDescription = flowItem.UcDesc;
                                flowDetail.SafeStock = flowItem.MinStock;
                                flowDetail.MaxStock = flowItem.MaxStock;
                                this.genericMgr.Update(flowDetail);
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessage.AddMessage(ex.Message);
                        }
                    }
                }
                #endregion
            }
            #endregion

            if (errorMessage.HasMessage)
            {
                throw errorMessage;
            }
        }
        //public FlowDetail



         public void UpdateFlowShiftDetails(string flow, IList<FlowShiftDetail> addFlowShiftDet,
         IList<FlowShiftDetail> updateFlowShiftDetail, IList<FlowShiftDetail> deleteFlowShiftDet)
        {
            if (addFlowShiftDet != null)
            {
                foreach (var detail in addFlowShiftDet)
                {
                    detail.Flow = flow.ToString();
                    this.genericMgr.Create(detail);
                }
            }
            if (updateFlowShiftDetail != null)
            {
                foreach (var detail in updateFlowShiftDetail)
                {
                    this.genericMgr.Update(detail);
                }
            }
            if (deleteFlowShiftDet != null)
            {
                foreach (var detail in deleteFlowShiftDet)
                {
                    this.genericMgr.Delete(detail);
                }
            }

        }
    }
}
