using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INP;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.MSG;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.SYS;
using com.Sconit.Entity.VIEW;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Utility;
using NHibernate;
using NHibernate.Type;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using System.Data.SqlClient;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class OrderMgrImpl : BaseMgr, IOrderMgr
    {
        #region SAP连接变量
        private static String SAPServiceUserName { get; set; }
        private static String SAPServicePassword { get; set; }
        private static Int32? SAPServiceTimeOut { get; set; }

        public ICredentials Credentials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SAPServiceUserName))
                {
                    SAPServiceUserName = systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.SAPSERVICEUSERNAME);
                }

                if (string.IsNullOrWhiteSpace(SAPServicePassword))
                {
                    SAPServicePassword = systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.SAPSERVICEPASSWORD);
                }

                return new NetworkCredential(SAPServiceUserName, SAPServicePassword);
            }
        }

        public Int32 TimeOut
        {
            get
            {
                if (!SAPServiceTimeOut.HasValue)
                {
                    SAPServiceTimeOut = int.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.SAPSERVICETIMEOUT));
                }

                return SAPServiceTimeOut.Value;
            }
        }
        #endregion

        #region 变量
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.OrderMaster");
        //private IPublishing proxy;
        //public IPubSubMgr pubSubMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public IItemMgr itemMgr { get; set; }
        public IBomMgr bomMgr { get; set; }
        public IRoutingMgr routingMgr { get; set; }
        public IFlowMgr flowMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IIpMgr ipMgr { get; set; }
        public IReceiptMgr receiptMgr { get; set; }
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public IProductionLineMgr productionLineMgr { get; set; }
        public IPickListMgr pickListMgr { get; set; }
        public IQueryMgr queryMgr { get; set; }
        public IHuMgr huMgr { get; set; }
        public IEmailMgr emailMgr { get; set; }
        public ISequenceMgr sequenceMgr { get; set; }
        public IWorkingCalendarMgr workingCalendarMgr { get; set; }
        public ITransportMgr transportMgr { get; set; }
        public IShipPlanMgr shipPlanMgr { get; set; }
        #endregion

        #region Delegate
        public delegate void OrderReleasedHandler(OrderMaster orderMaster);
        #endregion

        #region Event
        public event OrderReleasedHandler OrderReleased;
        #endregion

        #region public methods

        #region 路线转订单

        public OrderMaster TransferFlow2Order(FlowMaster flowMaster, bool isTransferDetail)
        {
            return TransferFlow2Order(flowMaster, null, DateTime.Now, isTransferDetail);
        }

        public OrderMaster TransferFlow2Order(FlowMaster flowMaster, IList<string> itemCodeList)
        {
            return TransferFlow2Order(flowMaster, itemCodeList, DateTime.Now);
        }

        public OrderMaster TransferFlow2Order(FlowMaster flowMaster, IList<string> itemCodeList, DateTime effectivedate)
        {
            return TransferFlow2Order(flowMaster, itemCodeList, effectivedate, true);
        }

        public OrderMaster TransferFlow2Order(FlowMaster flowMaster, IList<string> itemCodeList, DateTime effectivedate, bool isTransferDetail)
        {
            OrderMaster orderMaster = Mapper.Map<FlowMaster, OrderMaster>(flowMaster);
            orderMaster.IsPrintReceipt = flowMaster.IsPrintRceipt;

            if (isTransferDetail && (flowMaster.FlowDetails == null || flowMaster.FlowDetails.Count == 0))
            {
                TryLoadFlowDetails(flowMaster, itemCodeList);
            }

            #region 查找OrderMaster相关对象
            #region 查找Party
            string selectPartyStatement = "from Party where Code in (?";
            IList<object> selectPartyPara = new List<object>();
            selectPartyPara.Add(flowMaster.PartyFrom);
            if (flowMaster.PartyFrom != flowMaster.PartyTo)
            {
                selectPartyStatement += ", ?";
                selectPartyPara.Add(flowMaster.PartyTo);
            }
            selectPartyStatement += ")";
            IList<Party> partyList = this.genericMgr.FindAll<Party>(selectPartyStatement, selectPartyPara.ToArray());
            #endregion

            #region 查找Address
            #region 收集所有地址代码
            IList<string> addressCodeList = new List<string>();

            if (!string.IsNullOrEmpty(flowMaster.BillAddress))
            {
                addressCodeList.Add(flowMaster.BillAddress);
            }

            if (!string.IsNullOrEmpty(flowMaster.ShipFrom))
            {
                addressCodeList.Add(flowMaster.ShipFrom);
            }

            if (!string.IsNullOrEmpty(flowMaster.ShipTo))
            {
                addressCodeList.Add(flowMaster.ShipTo);
            }

            if (flowMaster.FlowDetails != null)
            {
                foreach (string billAddress in flowMaster.FlowDetails.Where(d => !string.IsNullOrEmpty(d.BillAddress)).Select(d => d.BillAddress).Distinct())
                {
                    addressCodeList.Add(billAddress);
                }
            }
            #endregion

            #region 查找Address
            IList<Address> addressList = null;
            if (addressCodeList.Count > 0)
            {
                string selectAddressStatement = string.Empty;
                IList<object> selectAddressPara = new List<object>();
                foreach (string addressCode in addressCodeList.Distinct())
                {
                    if (selectAddressStatement == string.Empty)
                    {
                        selectAddressStatement = "from Address where Code in (?";
                    }
                    else
                    {
                        selectAddressStatement += ",?";
                    }
                    selectAddressPara.Add(addressCode);
                }
                selectAddressStatement += ")";
                addressList = this.genericMgr.FindAll<Address>(selectAddressStatement, selectAddressPara.ToArray());
            }
            #endregion
            #endregion

            #region 查找Location
            #region 收集所有Location代码
            IList<string> locationCodeList = new List<string>();

            if (!string.IsNullOrEmpty(flowMaster.LocationFrom))
            {
                locationCodeList.Add(flowMaster.LocationFrom);
            }

            if (!string.IsNullOrEmpty(flowMaster.LocationTo))
            {
                locationCodeList.Add(flowMaster.LocationTo);
            }

            if (flowMaster.FlowDetails != null)
            {
                foreach (string locationFromCode in flowMaster.FlowDetails.Where(d => !string.IsNullOrEmpty(d.LocationFrom)).Select(d => d.LocationFrom).Distinct())
                {
                    locationCodeList.Add(locationFromCode);
                }

                foreach (string locationToCode in flowMaster.FlowDetails.Where(d => !string.IsNullOrEmpty(d.LocationTo)).Select(d => d.LocationTo).Distinct())
                {
                    locationCodeList.Add(locationToCode);
                }
            }
            #endregion

            #region 查找Location
            IList<Location> locationList = null;
            if (locationCodeList.Count > 0)
            {
                string selectLocationStatement = string.Empty;
                IList<object> selectLocationPara = new List<object>();
                foreach (string locationCode in locationCodeList.Distinct())
                {
                    if (selectLocationStatement == string.Empty)
                    {
                        selectLocationStatement = "from Location where Code in (?";
                    }
                    else
                    {
                        selectLocationStatement += ",?";
                    }
                    selectLocationPara.Add(locationCode);
                }
                selectLocationStatement += ")";
                locationList = this.genericMgr.FindAll<Location>(selectLocationStatement, selectLocationPara.ToArray());
            }
            #endregion
            #endregion

            #region 查找PriceList
            #region 收集所有PriceList代码
            IList<string> priceListCodeList = new List<string>();

            if (!string.IsNullOrEmpty(flowMaster.PriceList))
            {
                priceListCodeList.Add(flowMaster.PriceList);
            }

            if (flowMaster.FlowDetails != null)
            {
                foreach (string priceListCode in flowMaster.FlowDetails.Where(d => !string.IsNullOrEmpty(d.PriceList)).Select(d => d.PriceList).Distinct())
                {
                    priceListCodeList.Add(priceListCode);
                }
            }
            #endregion

            #region 查找PriceList
            IList<PriceListMaster> priceListList = null;
            if (priceListCodeList.Count > 0)
            {
                string selectPriceListStatement = string.Empty;
                IList<object> selectPriceListPara = new List<object>();
                foreach (string priceListCode in priceListCodeList.Distinct())
                {
                    if (selectPriceListStatement == string.Empty)
                    {
                        selectPriceListStatement = "from PriceListMaster where Code in (?";
                    }
                    else
                    {
                        selectPriceListStatement += ",?";
                    }
                    selectPriceListPara.Add(priceListCode);
                }
                selectPriceListStatement += ")";
                priceListList = this.genericMgr.FindAll<PriceListMaster>(selectPriceListStatement, selectPriceListPara.ToArray());
            }
            #endregion
            #endregion
            #endregion

            #region OrderMaster赋默认值

            #region PartyFrom和PartyTo赋值
            orderMaster.PartyFromName = partyList.Where(p => p.Code == orderMaster.PartyFrom).Single().Name;
            orderMaster.PartyToName = partyList.Where(p => p.Code == orderMaster.PartyTo).Single().Name;
            #endregion

            #region BillAddress赋值
            if (!string.IsNullOrEmpty(flowMaster.BillAddress))
            {
                orderMaster.BillAddressDescription = addressList.Where(a => a.Code == flowMaster.BillAddress).Single().AddressContent;
            }
            #endregion

            #region ShipFrom赋值
            if (!string.IsNullOrEmpty(flowMaster.ShipFrom))
            {
                Address shipFrom = addressList.Where(a => a.Code == flowMaster.ShipFrom).Single();
                orderMaster.ShipFromAddress = shipFrom.AddressContent;
                orderMaster.ShipFromCell = shipFrom.MobilePhone;
                orderMaster.ShipFromTel = shipFrom.TelPhone;
                orderMaster.ShipFromFax = shipFrom.Fax;
                orderMaster.ShipFromContact = shipFrom.ContactPersonName;
            }
            #endregion

            #region ShipTo赋值
            if (!string.IsNullOrEmpty(flowMaster.ShipTo))
            {
                Address shipTo = addressList.Where(a => a.Code == flowMaster.ShipTo).Single();
                orderMaster.ShipToAddress = shipTo.AddressContent;
                orderMaster.ShipToCell = shipTo.MobilePhone;
                orderMaster.ShipToTel = shipTo.TelPhone;
                orderMaster.ShipToFax = shipTo.Fax;
                orderMaster.ShipToContact = shipTo.ContactPersonName;
            }
            #endregion

            #region LocationFrom赋值
            if (!string.IsNullOrWhiteSpace(flowMaster.LocationFrom))
            {
                //   orderMaster.LocationFromName = locationList.Where(a => a.Code == flowMaster.LocationFrom).Single().Name;
                orderMaster.LocationFromName = genericMgr.FindById<Location>(flowMaster.LocationFrom).Name;
            }
            #endregion

            #region LocationTo赋值
            if (!string.IsNullOrWhiteSpace(flowMaster.LocationTo))
            {
                // orderMaster.LocationToName = locationList.Where(a => a.Code == flowMaster.LocationTo).Single().Name;
                orderMaster.LocationToName = genericMgr.FindById<Location>(flowMaster.LocationTo).Name;
            }
            #endregion

            #region PriceList赋值
            if (!string.IsNullOrWhiteSpace(flowMaster.PriceList))
            {
                orderMaster.Currency = priceListList.Where(a => a.Code == flowMaster.PriceList).Single().Currency;
            }
            #endregion

            orderMaster.EffectiveDate = effectivedate;
            orderMaster.SubType = com.Sconit.CodeMaster.OrderSubType.Normal;
            orderMaster.QualityType = com.Sconit.CodeMaster.QualityType.Qualified;
            orderMaster.Priority = com.Sconit.CodeMaster.OrderPriority.Normal;
            orderMaster.Status = com.Sconit.CodeMaster.OrderStatus.Create;
            orderMaster.OrderStrategy = flowMaster.FlowStrategy;
            #endregion

            #region OrderDetail

            if (isTransferDetail && flowMaster.FlowDetails != null && flowMaster.FlowDetails.Count > 0)
            {
                IList<Item> itemList = this.itemMgr.GetItems(flowMaster.FlowDetails.Select(det => det.Item).Distinct().ToList());

                int seq = 1;
                foreach (FlowDetail flowDetail in flowMaster.FlowDetails.OrderBy(d => d.Sequence))
                {
                    OrderDetail orderDetail = Mapper.Map<FlowDetail, OrderDetail>(flowDetail);
                    Item item = itemList.Where(a => a.Code == flowDetail.Item).Single();

                    if (flowDetail.ExternalSequence == 0)
                    {
                        orderDetail.Sequence = seq++; //重新记录顺序
                    }
                    else
                    {
                        orderDetail.Sequence = flowDetail.ExternalSequence;
                    }

                    //物料描述
                    orderDetail.ItemDescription = item.Description;

                    if (flowDetail.BindDemand != null)
                    {
                        //订单绑定取被绑定订单相关字段
                        orderDetail.ManufactureParty = flowDetail.BindDemand.ManufactureParty;
                        orderDetail.QualityType = flowDetail.BindDemand.QualityType;
                    }
                    else
                    {
                        orderDetail.QualityType = CodeMaster.QualityType.Qualified;
                    }
                    //orderDetail.UnitQty =  创建订单时会计算

                    if (!string.IsNullOrWhiteSpace(flowDetail.LocationFrom))
                    {
                        orderDetail.LocationFromName = locationList.Where(a => a.Code == flowDetail.LocationFrom).Single().Name;
                        //orderDetail.LocationFromName = genericMgr.FindById<Location>(flowDetail.LocationFrom).Name;
                    }
                    else
                    {
                        orderDetail.LocationFrom = orderMaster.LocationFrom;
                        orderDetail.LocationFromName = orderMaster.LocationFromName;
                    }

                    if (!string.IsNullOrWhiteSpace(flowDetail.LocationTo))
                    {
                        orderDetail.LocationToName = locationList.Where(a => a.Code == flowDetail.LocationTo).Single().Name;
                        //orderDetail.LocationToName = genericMgr.FindById<Location>(flowDetail.LocationTo).Name;
                    }
                    else
                    {
                        orderDetail.LocationTo = orderMaster.LocationTo;
                        orderDetail.LocationToName = orderMaster.LocationToName;
                    }

                    if (!string.IsNullOrWhiteSpace(flowDetail.BillAddress))
                    {
                        orderDetail.BillAddressDescription = addressList.Where(a => a.Code == flowDetail.BillAddress).Single().AddressContent;
                    }
                    orderDetail.ReferenceItemCode = string.IsNullOrWhiteSpace(orderDetail.ReferenceItemCode) ? item.ReferenceCode : orderDetail.ReferenceItemCode;
                    //if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Procurement
                    //   || orderMaster.Type == com.Sconit.CodeMaster.OrderType.ScheduleLine
                    //   || orderMaster.Type == com.Sconit.CodeMaster.OrderType.Distribution
                    //   || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                    //{
                    //    //计算价格
                    //    this.CalculateOrderDetailPrice(orderDetail, orderMaster, effectivedate);
                    //}

                    orderDetail.RequiredQty = flowDetail.OrderQty;
                    orderDetail.OrderedQty = flowDetail.OrderQty;

                    orderMaster.AddOrderDetail(orderDetail);
                }
            }

            #endregion

            #region OrderBinding
            TryLoadFlowBindings(flowMaster);
            if (flowMaster.FlowBindings != null && flowMaster.FlowBindings.Count > 0)
            {
                orderMaster.OrderBindings = (from b in flowMaster.FlowBindings
                                             select new OrderBinding
                                             {
                                                 BindFlow = b.BindedFlow.Code,
                                                 BindFlowStrategy = b.BindedFlow.FlowStrategy,
                                                 BindType = b.BindType,
                                             }).ToList();
            }
            #endregion

            return orderMaster;
        }

        public IList<OrderDetail> TransformFlowMster2OrderDetailList(FlowMaster flow, CodeMaster.OrderSubType orderSubType)
        {
            IList<FlowDetail> flowDetailList = flowMgr.GetFlowDetailList(flow, false, true);
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();
            foreach (FlowDetail flowDetail in flowDetailList)
            {
                OrderDetail orderDetail = Mapper.Map<FlowDetail, OrderDetail>(flowDetail);
                if (orderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    orderDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flow.LocationTo : flowDetail.LocationTo;
                    orderDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flow.LocationFrom : flowDetail.LocationFrom;
                }
                else
                {
                    orderDetail.LocationFrom = string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flow.LocationFrom : flowDetail.LocationFrom;
                    orderDetail.LocationTo = string.IsNullOrWhiteSpace(flowDetail.LocationTo) ? flow.LocationTo : flowDetail.LocationTo;
                }
                if (flow.Type == CodeMaster.OrderType.Production)
                {
                    orderDetail.Bom = string.IsNullOrWhiteSpace(orderDetail.Bom) ? orderDetail.Item : orderDetail.Bom;
                }
                //orderDetail.Id = flowDetail.Id;
                var item = itemMgr.GetCacheItem(flowDetail.Item);
                orderDetail.ItemDescription = item.Description;
                if (string.IsNullOrWhiteSpace(orderDetail.ReferenceItemCode))
                {
                    orderDetail.ReferenceItemCode = item.ReferenceCode;
                }
                orderDetailList.Add(orderDetail);
            }
            return orderDetailList;
        }
        #endregion

        #region 订单新增/修改操作
        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(OrderMaster orderMaster)
        {
            CreateOrder(orderMaster, true);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(OrderMaster orderMaster, bool expandOrderBomDetail)
        {
            #region 检查
            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    throw new BusinessException("没有此订单的操作权限。");
            //}

            if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count() > 0)
            {
                //按seq排序
                orderMaster.OrderDetails = orderMaster.OrderDetails.OrderBy(det => det.Sequence).ToList();
                int seq = 0; //新的序号
                IList<OrderDetail> activeOrderDetails = new List<OrderDetail>();
                IList<Item> itemList = this.itemMgr.GetItems(orderMaster.OrderDetails.Select(det => det.Item).Distinct().ToList());

                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    orderDetail.CurrentItem = itemList.Where(a => a.Code == orderDetail.Item).Single();
                    ((List<OrderDetail>)activeOrderDetails).AddRange(ProcessNewOrderDetail(orderDetail, orderMaster, ref seq));
                }

                orderMaster.OrderDetails = activeOrderDetails;
            }

            #region 生产线暂停不允许创建生产单
            if ((orderMaster.Type == CodeMaster.OrderType.Production
                || orderMaster.Type == CodeMaster.OrderType.SubContract) && !string.IsNullOrWhiteSpace(orderMaster.Flow))
            {
                FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);
                if (flowMaster.IsPause)
                {
                    throw new BusinessException("生产线{0}已经暂停，不能创建生产单。", orderMaster.Flow);
                }
            }
            #endregion

            //if (orderMaster.OrderDetails == null || orderMaster.OrderDetails.Count() == 0)
            //{
            //    throw new BusinessErrorException(Resources.ORD.OrderMaster.Errors_OrderDetailEmpty);
            //}
            #endregion

            #region 计算价格

            if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Procurement
                || orderMaster.Type == com.Sconit.CodeMaster.OrderType.ScheduleLine
                || orderMaster.Type == com.Sconit.CodeMaster.OrderType.Distribution
                || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
            {
                bool isAllowCreateOrderWithNoPrice = true;
                if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Distribution)
                {
                    isAllowCreateOrderWithNoPrice = bool.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.IsAllowCreateSalesOrderWithNoPrice));
                }
                else
                {
                    isAllowCreateOrderWithNoPrice = bool.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.IsAllowCreatePurchaseOrderWithNoPrice));
                }
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    //没有指定价格才需要计价
                    if (!orderDetail.UnitPrice.HasValue)
                    {
                        CalculateOrderDetailPrice(orderDetail, orderMaster, orderMaster.EffectiveDate);
                        if (!isAllowCreateOrderWithNoPrice)
                        {
                            if (!orderDetail.UnitPrice.HasValue || orderDetail.UnitPrice == 0M)
                            {
                                throw new BusinessException("在价格单{0}中没有找到物料{1}的价格,不能创建订单", orderMaster.PriceList, orderDetail.Item);
                            }
                        }
                    }
                }
            }
            #endregion

            #region 循环OrderDetail
            if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0)
            {
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    #region OrderDetail来源库位设定  //订单释放的时候在设置
                    //if (string.IsNullOrEmpty(orderDetail.LocationFrom)
                    //    && orderMaster.Type != com.Sconit.CodeMaster.OrderType.Procurement)
                    //{
                    //    orderDetail.LocationFrom = orderMaster.LocationFrom;
                    //    orderDetail.LocationFromName = orderMaster.LocationFromName;
                    //}
                    #endregion

                    #region OrderDetail目的库位设定 //订单释放的时候在设置
                    //if (string.IsNullOrEmpty(orderDetail.LocationTo)
                    //    && orderMaster.Type != com.Sconit.CodeMaster.OrderType.Distribution)
                    //{
                    //    orderDetail.LocationTo = orderMaster.LocationTo;
                    //    orderDetail.LocationToName = orderMaster.LocationToName;
                    //}
                    #endregion

                    #region 生成OrderOperation和OrderBomDetail
                    if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Production
                        || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                    {
                        if (orderDetail.OrderOperations == null || orderDetail.OrderOperations.Count == 0)
                        {
                            GenerateOrderOperation(orderDetail, orderMaster);
                        }

                        if (expandOrderBomDetail)
                        {
                            if (orderDetail.OrderBomDetails == null || orderDetail.OrderBomDetails.Count == 0)
                            {
                                GenerateOrderBomDetail(orderDetail, orderMaster);
                            }
                        }
                    }
                    #endregion

                    orderDetail.Direction = string.IsNullOrWhiteSpace(orderDetail.Direction) ? string.Empty : orderDetail.Direction;
                }
            }
            #endregion

            #region 生成OrderBinding
            if (!string.IsNullOrWhiteSpace(orderMaster.Flow))
            {
                IList<FlowBinding> flowBindingList = flowMgr.GetFlowBinding(orderMaster.Flow);

                if (flowBindingList != null && flowBindingList.Count() > 0)
                {
                    IList<OrderBinding> orderBindingList = (from binding in flowBindingList
                                                            select new OrderBinding
                                                            {
                                                                BindFlow = binding.BindedFlow.Code,
                                                                BindFlowStrategy = binding.BindedFlow.FlowStrategy,
                                                                BindType = binding.BindType,
                                                            }).ToList();

                    orderMaster.OrderBindings = orderBindingList;
                }
            }
            #endregion

            #region 创建OrderHead
            if (string.IsNullOrWhiteSpace(orderMaster.OrderNo))
            {
                orderMaster.OrderNo = numberControlMgr.GetOrderNo(orderMaster);
            }
            if (orderMaster.IsQuick)
            {
                if (orderMaster.WindowTime == DateTime.MinValue)
                {
                    orderMaster.WindowTime = DateTime.Now;
                }
                if (orderMaster.StartTime == DateTime.MinValue)
                {
                    orderMaster.StartTime = DateTime.Now;
                }
            }
            //orderMaster.OrderNo = numberControlMgr.GenerateOrderNo(orderMaster);
            orderMaster.Status = com.Sconit.CodeMaster.OrderStatus.Create;
            genericMgr.Create(orderMaster);

            #endregion

            #region 创建OrderBinding
            if (orderMaster.SubType == CodeMaster.OrderSubType.Normal  //退货没有绑定
                && orderMaster.OrderBindings != null && orderMaster.OrderBindings.Count() > 0)
            {
                foreach (OrderBinding orderBinding in orderMaster.OrderBindings)
                {
                    orderBinding.OrderNo = orderMaster.OrderNo;
                    genericMgr.Create(orderBinding);
                }
            }
            #endregion

            #region 创建OrderDetail
            if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0)
            {
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (orderDetail.OrderedQty < 0)
                    {
                        //throw new BusinessException(Resources.SYS.ErrorMessage.Errors_NegativeExceptiond);
                    }
                    orderDetail.OrderNo = orderMaster.OrderNo;
                    orderDetail.QualityType = orderMaster.QualityType;
                    genericMgr.Create(orderDetail);
                    if (orderDetail.OrderTracerList != null)
                    {
                        foreach (var orderTracer in orderDetail.OrderTracerList)
                        {
                            orderTracer.OrderDetailId = orderDetail.Id;
                            this.genericMgr.Create(orderTracer);
                        }
                    }
                    ProcessAddOrderOperations(orderDetail, orderDetail.OrderOperations);
                    ProcessAddOrderBomDetails(orderDetail, orderDetail.OrderBomDetails);
                }
            }
            else
            {
                throw new BusinessException("订单明细不能为空");
            }
            #endregion

            #region 自动Release
            if (orderMaster.IsAutoRelease || orderMaster.IsQuick)
            {
                ReleaseOrder(orderMaster);
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateOrder(OrderMaster orderMaster)
        {
            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //}

            if (orderMaster.OrderNo == null)
            {
                throw new TechnicalException("OrderNo not specified for OrderMaster.");
            }

            if (orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create)
            {
                this.genericMgr.Update(orderMaster);
                var orderDetails = this.genericMgr.FindAll<OrderDetail>
                    (" from OrderDetail where OrderNo =? ", orderMaster.OrderNo);
                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.QualityType = orderMaster.QualityType;
                    this.genericMgr.Update(orderDetail);
                }
            }
            else
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenModify, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrder(string orderNo)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);

            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    //throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //}

            if (orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create)
            {
                if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Production
                    || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                {
                    IList<OrderBomDetail> orderBomDetailList = TryLoadOrderBomDetails(orderMaster);
                    if (orderBomDetailList != null && orderBomDetailList.Count > 0)
                    {
                        this.genericMgr.Delete<OrderBomDetail>(orderBomDetailList);
                    }

                    IList<OrderOperation> orderOperationList = TryLoadOrderOperations(orderMaster);
                    if (orderOperationList != null && orderOperationList.Count > 0)
                    {
                        this.genericMgr.Delete<OrderOperation>(orderOperationList);
                    }
                }

                IList<OrderBinding> orderBindingList = TryLoadOrderBindings(orderMaster);
                if (orderBindingList != null && orderBindingList.Count > 0)
                {
                    this.genericMgr.Delete<OrderBinding>(orderBindingList);
                }

                IList<OrderDetail> orderDetailList = TryLoadOrderDetails(orderMaster);
                if (orderDetailList != null && orderDetailList.Count > 0)
                {
                    this.genericMgr.Delete<OrderDetail>(orderDetailList);
                }

                this.genericMgr.Delete(orderMaster);
            }
            else
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenDelete, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }
        #endregion

        #region 订单明细操作
        #region 批量更新订单明细
        [Transaction(TransactionMode.Requires)]
        public IList<OrderDetail> BatchUpdateOrderDetails(string orderNo, IList<OrderDetail> addOrderDetailList, IList<OrderDetail> updateOrderDetailList, IList<OrderDetail> deleteOrderDetailList)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new ArgumentNullException("OrderNo not specified.");
            }

            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            BeforeBatchUpdateOrderDetails(orderMaster);
            TryLoadOrderDetails(orderMaster);
            IList<OrderDetail> resultOrderDetailList = new List<OrderDetail>();
            if (addOrderDetailList != null && addOrderDetailList.Count > 0)
            {
                ((List<OrderDetail>)resultOrderDetailList).AddRange(ProcessAddOrderDetails(orderMaster, addOrderDetailList));
            }

            if (updateOrderDetailList != null && updateOrderDetailList.Count > 0)
            {
                ((List<OrderDetail>)resultOrderDetailList).AddRange(ProcessUpdateOrderDetails(orderMaster, updateOrderDetailList));
            }

            if (deleteOrderDetailList != null && deleteOrderDetailList.Count > 0)
            {
                IList<int> orderBindingIds = (from orderDetail in deleteOrderDetailList
                                              select orderDetail.Id).ToList();
                ProcessDeleteOrderDetails(orderMaster, orderBindingIds);
            }

            foreach (var orderDetail in resultOrderDetailList)
            {
                orderDetail.QualityType = orderMaster.QualityType;
            }

            return resultOrderDetailList;
        }

        private void BeforeBatchUpdateOrderDetails(OrderMaster orderMaster)
        {
            BeforeUpdateOrderDetails(orderMaster);
        }
        #endregion

        #region 添加订单明细
        [Transaction(TransactionMode.Requires)]
        public IList<OrderDetail> AddOrderDetails(string orderNo, IList<OrderDetail> orderDetailList)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new ArgumentNullException("OrderNo not specified.");
            }

            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);

            BeforeAddOrderDetails(orderMaster);
            return ProcessAddOrderDetails(orderMaster, orderDetailList);
        }

        private void BeforeAddOrderDetails(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenAddOrderDetail, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private IList<OrderDetail> ProcessAddOrderDetails(OrderMaster orderMaster, IList<OrderDetail> orderDetailList)
        {
            #region 获取最大订单明细序号
            string hql = "select max(Sequence) as seq from OrderDetail where OrderNo = ?";
            IList<int?> maxSeqList = genericMgr.FindAll<int?>(hql, orderMaster.OrderNo);
            int maxSeq = maxSeqList[0] != null ? maxSeqList[0].Value : 0;
            #endregion

            IList<OrderDetail> returnOrderDetailList = new List<OrderDetail>();
            IList<Item> itemList = new List<Item>();
            if ((orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0) || (orderDetailList != null && orderDetailList.Count > 0))
            {
                //新增的订单明细的Item要加进来
                itemList = this.itemMgr.GetItems(orderMaster.OrderDetails.Union(orderDetailList).Select(det => det.Item).Distinct().ToList());

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    #region 处理订单明细
                    //新增的话物料不在itemList中，重新查一把吧
                    var item = itemList.Where(a => a.Code == orderDetail.Item).FirstOrDefault();
                    orderDetail.CurrentItem = itemList.Where(a => a.Code == orderDetail.Item).Single();
                    IList<OrderDetail> newOrderDetailList = ProcessNewOrderDetail(orderDetail, orderMaster, ref maxSeq);
                    #endregion

                    #region 计算价格
                    //if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Procurement
                    //    || orderMaster.Type == com.Sconit.CodeMaster.OrderType.ScheduleLine
                    //    || orderMaster.Type == com.Sconit.CodeMaster.OrderType.Distribution
                    //    || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                    //{
                    //    foreach (OrderDetail newOrderDetail in newOrderDetailList)
                    //    {
                    //        //没有指定价格才需要计价
                    //        if (!newOrderDetail.UnitPrice.HasValue || string.IsNullOrWhiteSpace(newOrderDetail.Currency))
                    //        {
                    //            CalculateOrderDetailPrice(newOrderDetail, orderMaster, orderMaster.EffectiveDate);
                    //        }
                    //    }
                    //}
                    #endregion

                    #region 生成OrderOperation和OrderBomDetail
                    if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Production
                        || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                    {
                        foreach (OrderDetail newOrderDetail in newOrderDetailList)
                        {
                            GenerateOrderOperation(newOrderDetail, orderMaster);
                            GenerateOrderBomDetail(newOrderDetail, orderMaster);
                        }
                    }
                    #endregion

                    #region 创建OrderDetail
                    foreach (OrderDetail newOrderDetail in newOrderDetailList)
                    {
                        newOrderDetail.OrderNo = orderMaster.OrderNo;
                        newOrderDetail.QualityType = orderMaster.QualityType;
                        genericMgr.Create(newOrderDetail);
                        ProcessAddOrderOperations(orderDetail, orderDetail.OrderOperations);
                        ProcessAddOrderBomDetails(newOrderDetail, newOrderDetail.OrderBomDetails);
                    }
                    #endregion

                    ((List<OrderDetail>)returnOrderDetailList).AddRange(newOrderDetailList);
                }
            }

            return returnOrderDetailList;
        }
        #endregion

        #region 修改订单明细
        [Transaction(TransactionMode.Requires)]
        public IList<OrderDetail> UpdateOrderDetails(IList<OrderDetail> orderDetailList)
        {
            if (orderDetailList[0].OrderNo == null)
            {
                throw new TechnicalException("OrderNo not specified for OrderDetail.");
            }

            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetailList[0].OrderNo);
            BeforeUpdateOrderDetails(orderMaster);
            return ProcessUpdateOrderDetails(orderMaster, orderDetailList);
        }

        #region 轮胎分装发货更新订单明细
        [Transaction(TransactionMode.Requires)]
        public IList<OrderDetail> UpdateTireOrderDetails(string ordreNo, IList<OrderDetail> orderDetailList)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(ordreNo);
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Submit)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenModifyOrderDetail, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, int.Parse(((int)orderMaster.Status).ToString())));
            }
            return ProcessUpdateOrderDetails(orderMaster, orderDetailList);
        }
        #endregion

        private void BeforeUpdateOrderDetails(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenModifyOrderDetail, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, int.Parse(((int)orderMaster.Status).ToString())));
            }
        }

        private IList<OrderDetail> ProcessUpdateOrderDetails(OrderMaster orderMaster, IList<OrderDetail> orderDetailList)
        {
            IList<OrderDetail> returnOrderDetailList = new List<OrderDetail>();
            IList<OrderDetail> destinationOrderDetailList = this.LoadOrderDetails(orderDetailList.Select(det => det.Id).ToArray());
            IList<Item> itemList = this.itemMgr.GetItems(destinationOrderDetailList.Select(det => det.Item).Distinct().ToList());

            foreach (OrderDetail orderDetail in orderDetailList)
            {
                OrderDetail destinationOrderDetail = destinationOrderDetailList.Where(det => det.Id == orderDetail.Id).Single();

                //最好不要支持修改零件，如果改为Kit就不好办了
                string oldDetItem = destinationOrderDetail.Item;
                string oldDetUom = destinationOrderDetail.Uom;
                string oldDetRouting = destinationOrderDetail.Routing;
                string oldDetBom = destinationOrderDetail.Bom;
                //string oldProductionScan = destinationOrderDetail.ProductionScan;
                decimal oldDetOrderedQty = destinationOrderDetail.OrderedQty;

                Mapper.Map<OrderDetail, OrderDetail>(orderDetail, destinationOrderDetail);

                #region 整包校验
                CheckOrderedQtyFulfillment(orderMaster, destinationOrderDetail);
                #endregion

                #region 单位改变或者零件改变，重新生成UnitQty
                if (string.Compare(oldDetUom, destinationOrderDetail.Uom) != 0 || string.Compare(oldDetItem, destinationOrderDetail.Item) != 0)
                {
                    Item item = itemList.Where(a => a.Code == orderDetail.Item).Single();
                    if (destinationOrderDetail.Uom != item.Uom)
                    {
                        destinationOrderDetail.UnitQty = itemMgr.ConvertItemUomQty(destinationOrderDetail.Item, item.Uom, 1, destinationOrderDetail.Uom);
                    }
                    else
                    {
                        destinationOrderDetail.UnitQty = 1;
                    }
                }
                #endregion

                #region 判断是否需要重新生成OrderOperation和OrderBomDetail
                if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Production
                    || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                {
                    //明细的Routing改变了，或者零件改变了
                    //重新计算OrderOperation
                    if (string.Compare(oldDetItem, destinationOrderDetail.Item) != 0
                        || string.Compare(oldDetRouting, destinationOrderDetail.Routing) != 0)
                    {
                        ExpandOrderOperation(orderMaster, destinationOrderDetail);
                    }

                    //明细的Bom改变了，或者订单单位改变了，或者明细的Routing改变了，，或者零件改变了, //或者明细的防错扫描改变了
                    //重新计算OrderBomDetail
                    if (string.Compare(oldDetItem, destinationOrderDetail.Item) != 0
                        || string.Compare(oldDetBom, destinationOrderDetail.Bom) != 0
                        || string.Compare(oldDetUom, destinationOrderDetail.Uom) != 0
                        || string.Compare(oldDetRouting, destinationOrderDetail.Routing) != 0
                        //|| destinationOrderDetail.ProductionScan != oldProductionScan
                        )
                    {
                        ExpandOrderBomDetail(orderMaster, destinationOrderDetail);
                    }
                    else if (destinationOrderDetail.OrderedQty != oldDetOrderedQty)
                    {
                        //如果订单量发生变化，需要重新计算Bom用量
                        TryLoadOrderBomDetails(destinationOrderDetail);
                        foreach (OrderBomDetail orderBomDetail in destinationOrderDetail.OrderBomDetails)
                        {
                            orderBomDetail.OrderedQty = destinationOrderDetail.OrderedQty * orderBomDetail.BomUnitQty;
                            genericMgr.Update(orderBomDetail);
                        }
                    }
                }
                #endregion

                this.genericMgr.Update(destinationOrderDetail);

                returnOrderDetailList.Add(destinationOrderDetail);
            }

            return returnOrderDetailList;
        }
        #endregion

        #region 删除订单明细
        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderDetails(IList<int> orderDetailIds)
        {
            if (orderDetailIds == null && orderDetailIds.Count == 0)
            {
                throw new TechnicalException("OrderDetailIds is null.");
            }

            OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(orderDetailIds[0]);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);

            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    //throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //}

            BeforeDeleteOrderDetails(orderMaster);
            ProcessDeleteOrderDetails(orderMaster, orderDetailIds);
        }

        private void BeforeDeleteOrderDetails(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenDeleteOrderDetail,
                    orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void ProcessDeleteOrderDetails(OrderMaster orderMaster, IList<int> orderDetailIds)
        {
            string selectOrderDetailHql = "from OrderDetail where Id in (";
            object[] para = new object[orderDetailIds.Count];
            IType[] type = new IType[orderDetailIds.Count];
            for (int i = 0; i < orderDetailIds.Count; i++)
            {
                if (i == 0)
                {
                    selectOrderDetailHql += "?";
                }
                else
                {
                    selectOrderDetailHql += ", ?";
                }
                para[i] = orderDetailIds[i];
                // type[i] = NHibernateUtil.UInt32;
            }
            selectOrderDetailHql += ")";

            IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>(selectOrderDetailHql, para, type);

            foreach (OrderDetail orderDetail in orderDetailList)
            {
                IList<OrderBomDetail> orderBomDetailList = TryLoadOrderBomDetails(orderDetail);
                if (orderBomDetailList != null && orderBomDetailList.Count > 0)
                {
                    this.genericMgr.Delete<OrderBomDetail>(orderBomDetailList);
                }

                IList<OrderOperation> orderOperationList = TryLoadOrderOperations(orderDetail);
                if (orderOperationList != null && orderOperationList.Count > 0)
                {
                    this.genericMgr.Delete<OrderOperation>(orderOperationList);
                }
            }

            this.genericMgr.Delete<OrderDetail>(orderDetailList);
        }
        #endregion
        #endregion

        #region 订单绑定操作
        #region 批量更新订单绑定
        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateOrderBindings(string orderNo, IList<OrderBinding> addOrderBindingList, IList<OrderBinding> deleteOrderBindingList)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new ArgumentNullException("OrderNo not specified.");
            }

            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            BeforeBatchUpdateOrderBinding(orderMaster);

            if (addOrderBindingList != null && addOrderBindingList.Count > 0)
            {
                ProcessAddOrderBindings(orderMaster, addOrderBindingList);
            }

            if (deleteOrderBindingList != null && deleteOrderBindingList.Count > 0)
            {
                IList<int> deleteOrderBindingIds = (from orderBinding in deleteOrderBindingList
                                                    select orderBinding.Id).ToList();
                ProcessDeleteOrderBindings(deleteOrderBindingIds);
            }
        }

        private void BeforeBatchUpdateOrderBinding(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenUpdateOrderBinding, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }
        #endregion

        #region 添加订单绑定
        [Transaction(TransactionMode.Requires)]
        public void AddOrderBindings(string orderNo, IList<OrderBinding> orderBindingList)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new ArgumentNullException("OrderNo not specified.");
            }

            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            BeforeAddOrderBindings(orderMaster);
            ProcessAddOrderBindings(orderMaster, orderBindingList);
        }

        private void BeforeAddOrderBindings(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenAddOrderBinding, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void ProcessAddOrderBindings(OrderMaster orderMaster, IList<OrderBinding> orderBindingList)
        {
            foreach (OrderBinding orderBinding in orderBindingList)
            {
                orderBinding.OrderNo = orderMaster.OrderNo;
                this.genericMgr.Create(orderBinding);
            }
        }
        #endregion

        #region 删除订单绑定
        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderBindings(IList<int> orderBindingIds)
        {
            if (orderBindingIds == null && orderBindingIds.Count == 0)
            {
                throw new TechnicalException("OrderBindingIds is null.");
            }

            OrderBinding orderBinding = genericMgr.FindById<OrderBinding>(orderBindingIds[0]);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderBinding.OrderNo);

            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    //throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //}
            BeforeDeleteOrderBindings(orderMaster);
            ProcessDeleteOrderBindings(orderBindingIds);
        }

        private void BeforeDeleteOrderBindings(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenDeleteOrderBinding, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void ProcessDeleteOrderBindings(IList<int> orderBindingIds)
        {
            string hql = "from OrderBinding where Id in (";
            object[] para = new object[orderBindingIds.Count];
            IType[] type = new IType[orderBindingIds.Count];

            for (int i = 0; i < orderBindingIds.Count; i++)
            {
                if (i == 0)
                {
                    hql += "?";
                }
                else
                {
                    hql += ", ?";
                }
                para[i] = orderBindingIds[i];
                type[i] = NHibernateUtil.UInt32;
            }

            hql += ")";

            genericMgr.Delete(genericMgr.FindAll<OrderBinding>(hql, para, type));
        }
        #endregion

        #region 创建绑定订单
        ////[Transaction(TransactionMode.Requires)]
        //private void AsyncCreateBindOrder(OrderMaster orderMaster, CodeMaster.BindType bindType)
        //{
        //    AsyncCreateBindOrderDelegate asyncDelegate = new AsyncCreateBindOrderDelegate(this.CreateBindOrder);
        //    asyncDelegate.BeginInvoke(orderMaster, bindType, SecurityContextHolder.Get(), null, null);
        //}

        //private delegate void AsyncCreateBindOrderDelegate(OrderMaster orderMaster, CodeMaster.BindType bindType, User user);

        //private void CreateBindOrder(OrderMaster orderMaster, CodeMaster.BindType bindType, User user)
        private void CreateBindOrder(OrderMaster orderMaster, CodeMaster.BindType bindType)
        {
            //SecurityContextHolder.Set(user);
            if (orderMaster.SubType == CodeMaster.OrderSubType.Normal)  //只有普通订单才支持绑定
            {
                TryLoadOrderBindings(orderMaster);

                #region 根据绑定类型
                IList<OrderBinding> orderBindingList = new List<OrderBinding>();
                if (orderMaster.OrderBindings != null && orderMaster.OrderBindings.Count > 0)
                {
                    foreach (OrderBinding orderBinding in orderMaster.OrderBindings)
                    {
                        //过滤掉已经创建订单的绑定
                        if (orderBinding.BindType == bindType && string.IsNullOrWhiteSpace(orderBinding.BindOrderNo))
                        {
                            orderBindingList.Add(orderBinding);
                        }
                    }
                }
                #endregion

                #region 创建绑定订单
                DoCreateBindingOrder(orderMaster, orderBindingList);
                #endregion
            }
        }

        private void DoCreateBindingOrder(OrderMaster orderMaster, IList<OrderBinding> orderBindingList)
        {
            if (orderBindingList != null && orderBindingList.Count > 0)
            {
                if (orderMaster.OrderDetails == null)
                {
                    TryLoadOrderDetails(orderMaster);
                }
                if (orderMaster.Type == CodeMaster.OrderType.Production || orderMaster.Type == CodeMaster.OrderType.SubContract)
                {
                    TryLoadOrderBomDetails(orderMaster);
                    TryLoadOrderOperations(orderMaster);
                }

                #region 汇总被拉动需求
                IList<BindDemand> bindRequireList = new List<BindDemand>();
                if (orderMaster.Type == CodeMaster.OrderType.Production || orderMaster.Type == CodeMaster.OrderType.SubContract)
                {
                    #region 生产单拉动
                    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                    {
                        ((List<BindDemand>)bindRequireList).AddRange(from bomDet in orderDetail.OrderBomDetails
                                                                     select new BindDemand
                                                                     {
                                                                         OrderNo = orderDetail.OrderNo,
                                                                         OrderDetailId = orderDetail.Id,
                                                                         OrderBomDetailId = bomDet.Id,
                                                                         Item = bomDet.Item,
                                                                         Uom = bomDet.Uom,
                                                                         BaseUom = bomDet.BaseUom,
                                                                         UnitQty = bomDet.UnitQty,
                                                                         ManufactureParty = bomDet.ManufactureParty,
                                                                         QualityType = CodeMaster.QualityType.Qualified,
                                                                         Location = bomDet.Location,
                                                                         Qty = bomDet.BomUnitQty * orderDetail.OrderedQty,
                                                                         //考虑生产单明细的其它需求源作为生产单Bom的其它需求源
                                                                         ExtraDemandSource = !string.IsNullOrWhiteSpace(orderDetail.ExtraDemandSource) ? orderDetail.ExtraDemandSource : orderMaster.ExtraDemandSource,
                                                                         WindowTime = orderMaster.StartTime//bomDet.EstimateConsumeTime,
                                                                     });
                    }
                    #endregion
                }
                else
                {
                    #region 物流单拉动
                    ((List<BindDemand>)bindRequireList).AddRange(from det in orderMaster.OrderDetails
                                                                 select new BindDemand
                                                                 {
                                                                     OrderNo = det.OrderNo,
                                                                     OrderDetailId = det.Id,
                                                                     Item = det.Item,
                                                                     Uom = det.Uom,
                                                                     BaseUom = det.BaseUom,
                                                                     UnitQty = det.UnitQty,
                                                                     ManufactureParty = det.ManufactureParty,
                                                                     QualityType = det.QualityType,
                                                                     UnitCount = det.UnitCount,
                                                                     Location = det.LocationFrom,
                                                                     Qty = det.OrderedQty,
                                                                     ExtraDemandSource = !string.IsNullOrWhiteSpace(det.ExtraDemandSource) ? det.ExtraDemandSource : orderMaster.ExtraDemandSource,
                                                                     WindowTime = orderMaster.StartTime
                                                                 });
                    #endregion
                }
                #endregion

                IList<FlowMaster> flowMasterList = LoadFlowMaster(orderBindingList, orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value : DateTime.Now);
                foreach (OrderBinding orderBinding in orderBindingList)
                {
                    orderBinding.CurrentBindFlowMaster = flowMasterList.Where(f => f.Code == orderBinding.BindFlow).Single();

                    IList<FlowDetail> bindFlowDetailList = new List<FlowDetail>();
                    #region 匹配需求
                    foreach (BindDemand bindDemand in bindRequireList)
                    {
                        //先匹配零件号和库位
                        //考虑了其它需求源，即需求的其它需求源和FlowDetail的目的库位相同
                        var matchedFlowDetailList = orderBinding.CurrentBindFlowMaster.FlowDetails.Where(d => d.Item == bindDemand.Item
                              && (d.LocationTo == bindDemand.Location //FlowDetail的目的库位等于需求库位
                              || (!string.IsNullOrEmpty(bindDemand.ExtraDemandSource) && bindDemand.ExtraDemandSource.IndexOf(d.LocationTo) != -1) //FlowDetail的目的库位等于其它需求源
                              || (string.IsNullOrWhiteSpace(d.LocationTo) && orderBinding.CurrentBindFlowMaster.LocationTo == bindDemand.Location) //FlowDetail的目的库位为空 and FlowMaster的目的库位等于需求库位
                              || (string.IsNullOrWhiteSpace(d.LocationTo) && !string.IsNullOrEmpty(bindDemand.ExtraDemandSource) && bindDemand.ExtraDemandSource.IndexOf(orderBinding.CurrentBindFlowMaster.LocationTo) != -1)));  //FlowDetail的目的库位为空 and FlowMaster的目的库位等于其它需求源

                        //1. 一个需求只能匹配一条FlowDetail，找出最佳匹配项
                        //2. 优先顺序为库位/其它需求源、单位、单包装
                        if (matchedFlowDetailList != null && matchedFlowDetailList.Count() > 0)
                        {
                            var matcheLocationList = matchedFlowDetailList.Where(d => d.LocationTo == bindDemand.Location);

                            if (matcheLocationList != null && matcheLocationList.Count() > 0)
                            {
                                matchedFlowDetailList = matcheLocationList;
                            }

                            var matcheUomList = matchedFlowDetailList.Where(d => d.Uom == bindDemand.Uom);

                            if (matcheUomList != null && matcheUomList.Count() > 0)
                            {
                                matchedFlowDetailList = matcheUomList;
                            }

                            var matcheUCList = matchedFlowDetailList.Where(d => d.UnitCount == bindDemand.UnitCount);

                            if (matcheUCList != null && matcheUCList.Count() > 0)
                            {
                                matchedFlowDetailList = matcheUCList;
                            }

                            FlowDetail flowDetail = Mapper.Map<FlowDetail, FlowDetail>(matchedFlowDetailList.OrderBy(d => d.Sequence).First());
                            if (flowDetail.Uom != bindDemand.Uom)
                            {
                                flowDetail.OrderQty = bindDemand.Qty * bindDemand.UnitQty;//先转为基本单位

                                if (flowDetail.Uom != flowDetail.BaseUom)
                                {
                                    //在转为FlowDetail单位
                                    flowDetail.OrderQty = this.itemMgr.ConvertItemUomQty(flowDetail.Item, flowDetail.BaseUom, flowDetail.OrderQty, flowDetail.Uom);
                                }
                            }
                            else
                            {
                                flowDetail.OrderQty = bindDemand.Qty;
                            }
                            flowDetail.BindDemand = bindDemand;
                            bindFlowDetailList.Add(flowDetail);
                        }
                    }
                    #endregion


                    #region 创建绑定订单
                    if (bindFlowDetailList.Count > 0)
                    {
                        #region 创建绑定订单
                        FlowMaster flowMaster = Mapper.Map<FlowMaster, FlowMaster>(orderBinding.CurrentBindFlowMaster);
                        FlowStrategy flowStrategry = this.genericMgr.FindById<FlowStrategy>(flowMaster.Code);
                        flowMaster.FlowDetails = bindFlowDetailList;

                        OrderMaster bindOrderMaster = TransferFlow2Order(flowMaster, null, orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value : DateTime.Now);
                        bindOrderMaster.ExternalOrderNo = orderMaster.ExternalOrderNo;
                        bindOrderMaster.ReferenceOrderNo = orderMaster.OrderNo;
                        bindOrderMaster.TraceCode = orderMaster.TraceCode;
                        bindOrderMaster.WindowTime = flowMaster.FlowDetails.Min(d => d.BindDemand.WindowTime);  //取绑定明细最小的窗口时间

                        IList<WorkingCalendarView> workingCalendarViewList = this.workingCalendarMgr.GetWorkingCalendarViewList(bindOrderMaster.PartyFrom, bindOrderMaster.Flow, bindOrderMaster.WindowTime.Add(TimeSpan.FromDays(-7)), bindOrderMaster.WindowTime);
                        DateTime startTime = this.workingCalendarMgr.GetStartTimeAtWorkingDate(bindOrderMaster.WindowTime, (double)flowStrategry.LeadTime, CodeMaster.TimeUnit.Hour, bindOrderMaster.PartyFrom, bindOrderMaster.Flow, workingCalendarViewList);
                        if (startTime < DateTime.Now)
                        {
                            DateTime emStartTime = this.workingCalendarMgr.GetStartTimeAtWorkingDate(bindOrderMaster.WindowTime, (double)flowStrategry.EmergencyLeadTime, CodeMaster.TimeUnit.Hour, bindOrderMaster.PartyFrom, bindOrderMaster.Flow, workingCalendarViewList);// bindOrderMaster.WindowTime.AddHours(-(double)flowStrategry.EmergencyLeadTime);
                            if (emStartTime < DateTime.Now)
                            {
                                bindOrderMaster.StartTime = emStartTime;
                                bindOrderMaster.Priority = CodeMaster.OrderPriority.Urgent;
                            }
                            else
                            {
                                bindOrderMaster.StartTime = startTime;
                                bindOrderMaster.Priority = CodeMaster.OrderPriority.Normal;
                            }
                        }
                        else
                        {
                            bindOrderMaster.StartTime = startTime;
                            bindOrderMaster.Priority = CodeMaster.OrderPriority.Normal;
                        }
                        bindOrderMaster.QualityType = orderMaster.QualityType;
                        bindOrderMaster.Sequence = orderMaster.Sequence;
                        bindOrderMaster.EffectiveDate = orderMaster.EffectiveDate;
                        bindOrderMaster.OrderStrategy = flowStrategry.Strategy;

                        bindOrderMaster.OrderDetails = bindOrderMaster.OrderDetails
                            .GroupBy(p => new
                            {
                                Item = p.Item,
                                UnitCount = p.UnitCount,
                                Uom = p.Uom,
                                LocationFrom = p.LocationFrom,
                                LocationTo = p.LocationTo
                            }, (k, g) => new
                            {
                                OrderDetail = g.First(),
                                RequiredQty = g.Sum(q => q.OrderedQty),
                                OrderedQty = GetRoundOrderQty(g.First(), g.Sum(q => q.OrderedQty)),
                                OrderTracerList = g.Select(q => new OrderTracer
                                {
                                    Code = q.BindDemand.OrderNo,
                                    Item = q.BindDemand.Item,
                                    OrderedQty = q.BindDemand.Qty,
                                    Qty = q.BindDemand.Qty,
                                    RefId = q.BindDemand.OrderDetailId,
                                    ReqTime = q.BindDemand.WindowTime
                                }).ToList()
                            })
                            .Select(p =>
                            {
                                p.OrderDetail.OrderedQty = p.OrderedQty;
                                p.OrderDetail.RequiredQty = p.RequiredQty;
                                p.OrderDetail.OrderTracerList = p.OrderTracerList;
                                return p.OrderDetail;
                            }).ToList();

                        this.CreateOrder(bindOrderMaster);
                        #region 创建OrderBindingDetail
                        UpdateOrderBinding(orderBinding, bindOrderMaster);
                        #endregion

                        //if (flowMaster.Type == CodeMaster.OrderType.Procurement)
                        //{
                        //    #region 采购要调用SAP创建交货计划行服务
                        //    CreateSapProcOrder(orderMaster, SecurityContextHolder.Get());
                        //    #endregion
                        //}
                        //else
                        //{
                        //    this.CreateOrder(bindOrderMaster);
                        //    #region 创建OrderBindingDetail
                        //    UpdateOrderBinding(orderBinding, bindOrderMaster);
                        //    #endregion
                        //}
                        #endregion
                    }
                    #endregion
                }
            }
        }

        public decimal GetRoundOrderQty(OrderDetail orderDetail, decimal orderQty)
        {
            if (orderDetail.MinLotSize > 0 && orderQty < orderDetail.MinLotSize)
            {
                orderQty = orderDetail.MinLotSize;
            }
            if (orderDetail.UnitCount > 0)
            {
                if (orderDetail.RoundUpOption == CodeMaster.RoundUpOption.ToUp)
                {
                    orderQty = Math.Ceiling(orderQty / orderDetail.UnitCount) * orderDetail.UnitCount;
                }
                else if (orderDetail.RoundUpOption == CodeMaster.RoundUpOption.ToDown)
                {
                    orderQty = Math.Floor(orderQty / orderDetail.UnitCount) * orderDetail.UnitCount;
                }
            }
            int decimalLength = int.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DecimalLength));
            return Math.Round(orderQty, decimalLength);
        }

        public decimal GetRoundOrderQty(FlowDetail flowDetail, decimal orderQty)
        {
            if (flowDetail.MinLotSize > 0 && orderQty < flowDetail.MinLotSize)
            {
                orderQty = flowDetail.MinLotSize;
            }
            if (flowDetail.UnitCount > 0)
            {
                if (flowDetail.RoundUpOption == CodeMaster.RoundUpOption.ToUp)
                {
                    orderQty = Math.Ceiling(orderQty / flowDetail.UnitCount) * flowDetail.UnitCount;
                }
                else if (flowDetail.RoundUpOption == CodeMaster.RoundUpOption.ToDown)
                {
                    orderQty = Math.Floor(orderQty / flowDetail.UnitCount) * flowDetail.UnitCount;
                }
            }
            int decimalLength = int.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DecimalLength));
            return Math.Round(orderQty, decimalLength);
        }

        #region 重新绑定路线
        [Transaction(TransactionMode.Requires)]
        public void ReCreateBindOrder(OrderBinding orderBinding)
        {
            if (!string.IsNullOrEmpty(orderBinding.BindOrderNo))
            {
                var orders = this.genericMgr.FindAll<OrderMaster>(" from OrderMaster where OrderNo=? ", orderBinding.BindOrderNo);
                if (orders != null && orders.Count > 0)
                {
                    OrderMaster bindOrder = orders.First();

                    if (bindOrder.Type == CodeMaster.OrderType.Production)
                    {
                        throw new BusinessException("被绑定订单{0}为生产单，不能重新创建绑定。", bindOrder.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)bindOrder.Status).ToString()));
                    }

                    if (bindOrder.Status == CodeMaster.OrderStatus.Create)
                    {
                        this.DeleteOrder(bindOrder.OrderNo);
                    }
                    else if (bindOrder.Status == CodeMaster.OrderStatus.Submit)
                    {
                        this.CancelOrder(bindOrder);
                    }
                    else
                    {
                        throw new BusinessException("被绑定订单{0}的状态为{1}不能重新创建绑定。", bindOrder.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)bindOrder.Status).ToString()));
                    }
                }
            }

            OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderBinding.OrderNo);
            IList<OrderBinding> orderBindingList = new List<OrderBinding>();
            orderBindingList.Add(orderBinding);
            DoCreateBindingOrder(orderMaster, orderBindingList);
        }
        #endregion

        #region 创建OrderBindingDetail
        private void UpdateOrderBinding(OrderBinding orderBinding, OrderMaster orderMaster)
        {
            #region 更新OrderBinding
            orderBinding.BindOrderNo = orderMaster.OrderNo;
            orderBinding.CurrentBindOrderMaster = orderMaster;
            this.genericMgr.Update(orderBinding);
            #endregion

            #region 创建OrderBindingDetail//用OrderTracer代替
            //foreach (OrderDetail bindOrderDetail in orderMaster.OrderDetails)
            //{
            //    OrderBindingDetail orderBindingDetail = new OrderBindingDetail();
            //    orderBindingDetail.OrderBindingId = orderBinding.Id;
            //    orderBindingDetail.OrderNo = orderMaster.OrderNo;
            //    orderBindingDetail.OrderDetailId = bindOrderDetail.BindDemand.OrderDetailId;
            //    orderBindingDetail.OrderBomDetailId = bindOrderDetail.BindDemand.OrderBomDetailId;
            //    orderBindingDetail.BindOrderNo = orderMaster.OrderNo;
            //    orderBindingDetail.BindOrderDetailId = bindOrderDetail.Id;

            //    this.genericMgr.Create(bindOrderDetail);
            //}
            #endregion
        }
        #endregion

        private void CreateSapProcOrder(OrderMaster orderMaster, User user)
        {
            //IList<ErrorMessage> errorMessageList = new List<ErrorMessage>();
            //SecurityContextHolder.Set(user);
            //try
            //{
            //    MI_CRSL_LESService service = new MI_CRSL_LESService();
            //    service.Credentials = this.Credentials;
            //    service.Timeout = this.TimeOut;
            //    service.Url = "http://10.86.128.63:8000/XISOAPAdapter/MessageServlet?channel=:LES01:CC_SOAP7&version=3.0&Sender.Service=LES01&Interface=http%3A%2F%2Fwww.sih.cq.cn%2Fsap%2Fmm%2F07%5EMI_CRSL_LES";

            //    #region 循环创建计划协议明细
            //    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
            //    {
            //        //根据来源目的库位+零件号查找
            //        //来源目的库位明细上没有取头上的
            //        log.Debug(string.Format("开始连接Web服务创建计划协议，路线代码{0}，零件{1}，数量{2}，开始时间{3}，窗口时间{4}",
            //            orderMaster.Flow, orderDetail.Item, orderDetail.OrderedQty, orderMaster.StartTime, orderMaster.WindowTime));
            //        ZLSCHE_IN ZLSCHE_IN = new ZLSCHE_IN();

            //        ZLSCHE_IN.EBELN = orderDetail.EBELN;
            //        ZLSCHE_IN.EBELP = orderDetail.EBELP;
            //        ZLSCHE_IN.MENGE = orderDetail.OrderedQty;       //订单单位的数量
            //        ZLSCHE_IN.MENGESpecified = true;
            //        ZLSCHE_IN.EINDT = orderMaster.WindowTime.ToString("yyyyMMdd");

            //        ZLSCHE_OUT ZLSCHE_OUT = service.MI_CRSL_LES(ZLSCHE_IN);
            //        log.Debug("连接Web服务创建计划协议完成。");

            //        if (ZLSCHE_OUT.STATUS == "S")
            //        {
            //            orderMaster.ExternalOrderNo = ZLSCHE_OUT.EBELN;    //计划协议号
            //            orderDetail.ExternalSequence = ZLSCHE_OUT.EBELP;   //计划协议行号
            //            orderDetail.ExternalOrderNo = orderMaster.ExternalOrderNo;

            //            this.CreateOrder(orderMaster);
            //        }
            //        else
            //        {
            //            string logMessage = "创建计划协议失败。错误信息：" + ZLSCHE_OUT.MESSAGE;
            //            log.Error(logMessage);

            //            string mailTo = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ExceptionMailTo);
            //            string emailBody = @"订单号：" + orderMaster.ExternalOrderNo + "，计划协议号：" + ZLSCHE_OUT.EBELN + "，计划协议行号：" + ZLSCHE_OUT.EBELP;
            //            EmailService.EmailService emailService = new EmailService.EmailService();
            //            emailService.AsyncSend(logMessage, emailBody, mailTo, EmailService.MailPriority.High);
            //        }
            //    }
            //    #endregion
            //}
            //catch (Exception ex)
            //{
            //    string logMessage = "连接Web服务创建计划协议失败。";
            //    log.Error(logMessage, ex);

            //    string mailTo = this.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.ExceptionMailTo);
            //    string emailBody = @"订单号：" + orderMaster.ExternalOrderNo;
            //    EmailService.EmailService emailService = new EmailService.EmailService();
            //    emailService.AsyncSend(logMessage, emailBody, mailTo, EmailService.MailPriority.High);
            //}
        }

        private void AsyncCreateSapProcOrder(OrderMaster orderMaster)
        {
            AsyncCreateSapProcOrderDelegate asyncDelegate = new AsyncCreateSapProcOrderDelegate(this.CreateSapProcOrder);
            asyncDelegate.BeginInvoke(orderMaster, SecurityContextHolder.Get(), null, null);
        }

        private delegate void AsyncCreateSapProcOrderDelegate(OrderMaster orderMaster, User user);
        #endregion
        #endregion

        #region 订单工艺流程/BOM操作

        #region 工艺流程操作
        #region 批量更新工序
        [Transaction(TransactionMode.Requires)]
        public void BatchUpdateOrderOperations(int orderDetailId, IList<OrderOperation> addOrderOperationList, IList<OrderOperation> updateOrderOperationList, IList<OrderOperation> deleteOrderOperationList)
        {
            OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(orderDetailId);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);
            BeforeBatchUpdateOrderOperations(orderMaster);

            if (addOrderOperationList != null && addOrderOperationList.Count > 0)
            {
                ProcessAddOrderOperations(orderDetail, addOrderOperationList);
            }

            if (updateOrderOperationList != null && updateOrderOperationList.Count > 0)
            {
                ProcessUpdateOrderOperations(updateOrderOperationList);
            }

            if (deleteOrderOperationList != null && deleteOrderOperationList.Count > 0)
            {
                IList<int> deleteOrderOperationIds = (from orderOp in deleteOrderOperationList
                                                      select orderOp.Id).ToList();
                ProcessDeleteOrderOperations(deleteOrderOperationIds);
            }
        }

        private void BeforeBatchUpdateOrderOperations(OrderMaster orderMaster)
        {
            BeforeUpdateOrderOperations(orderMaster);
        }
        #endregion

        #region 添加工序
        [Transaction(TransactionMode.Requires)]
        public void AddOrderOperations(int orderDetailId, IList<OrderOperation> orderOperationList)
        {
            OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(orderDetailId);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);
            BeforeAddOrderOperations(orderMaster);
            ProcessAddOrderOperations(orderDetail, orderOperationList);
        }

        private void BeforeAddOrderOperations(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenAddOrderOperation, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void ProcessAddOrderOperations(OrderDetail orderDetail, IList<OrderOperation> orderOperationList)
        {
            if (orderOperationList != null && orderOperationList.Count > 0)
            {
                foreach (OrderOperation orderOperation in orderOperationList)
                {
                    orderOperation.OrderNo = orderDetail.OrderNo;
                    orderOperation.OrderDetId = orderDetail.Id;
                    this.genericMgr.Create(orderOperation);
                }
            }
        }
        #endregion

        #region 更新工序
        [Transaction(TransactionMode.Requires)]
        public void UpdateOrderOperations(IList<OrderOperation> orderOperations)
        {
            if (orderOperations == null && orderOperations.Count == 0)
            {
                throw new TechnicalException("OrderOperations is null.");
            }

            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderOperations[0].OrderNo);
            BeforeUpdateOrderOperations(orderMaster);
            ProcessUpdateOrderOperations(orderOperations);
        }

        private void BeforeUpdateOrderOperations(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenModifyOrderOperation,
                    orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void ProcessUpdateOrderOperations(IList<OrderOperation> orderOperations)
        {
            foreach (OrderOperation orderOperation in orderOperations)
            {
                this.genericMgr.Update(orderOperation);
            }
        }
        #endregion

        #region 删除工序
        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderOperations(IList<int> orderOperationIds)
        {
            if (orderOperationIds == null && orderOperationIds.Count == 0)
            {
                throw new TechnicalException("OrderOperationIds is null.");
            }

            OrderOperation orderOperation = genericMgr.FindById<OrderOperation>(orderOperationIds[0]);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderOperation.OrderNo);

            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    //throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //}


            BeforeDeleteOrderOperations(orderMaster);
            ProcessDeleteOrderOperations(orderOperationIds);
        }

        private void BeforeDeleteOrderOperations(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenDeleteOrderOperation,
                    orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void ProcessDeleteOrderOperations(IList<int> orderOperationIds)
        {
            string hql = "from OrderOperation where Id in (";
            object[] para = new object[orderOperationIds.Count];
            IType[] type = new IType[orderOperationIds.Count];

            for (int i = 0; i < orderOperationIds.Count; i++)
            {
                if (i == 0)
                {
                    hql += "?";
                }
                else
                {
                    hql += ", ?";
                }
                para[i] = orderOperationIds[i];
                type[i] = NHibernateUtil.UInt32;
            }

            hql += ")";

            genericMgr.Delete(genericMgr.FindAll<OrderOperation>(hql, para, type));
        }
        #endregion

        #region 展开工序
        [Transaction(TransactionMode.Requires)]
        public IList<OrderOperation> ExpandOrderOperation(int orderDetailId)
        {
            OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(orderDetailId);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);

            return ExpandOrderOperation(orderMaster, orderDetail);
        }

        private IList<OrderOperation> ExpandOrderOperation(OrderMaster orderMaster, OrderDetail orderDetail)
        {
            BeforeExpandOrderOperation(orderMaster);
            return ProcessExpandOrderOperation(orderMaster, orderDetail);
        }

        private void BeforeExpandOrderOperation(OrderMaster orderMaster)
        {
            if (!(orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Complete
                //&& orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Close
                && orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Cancel))
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenExpandOrderOperation,
                    orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private IList<OrderOperation> ProcessExpandOrderOperation(OrderMaster orderMaster, OrderDetail orderDetail)
        {
            string hql = "from OrderOperation where OrderDetailId = ?";
            genericMgr.Delete(genericMgr.FindAll<OrderOperation>(hql, orderDetail.Id));
            GenerateOrderOperation(orderDetail, orderMaster);
            ProcessAddOrderOperations(orderDetail, orderDetail.OrderOperations);

            return orderDetail.OrderOperations;
        }
        #endregion
        #endregion

        #region BOM操作
        #region 批量更新BOM
        public void BatchUpdateOrderBomDetails(OrderDetail orderDetail, IList<OrderBomDetail> addOrderBomDetailList, IList<OrderBomDetail> updateOrderBomDetailList, IList<OrderBomDetail> deleteOrderBomDetailList)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);
            BeforeBatchUpdateOrderBomDetails(orderMaster);

            if (addOrderBomDetailList != null && addOrderBomDetailList.Count > 0)
            {
                ProcessAddOrderBomDetails(orderDetail, addOrderBomDetailList);
            }

            if (updateOrderBomDetailList != null && updateOrderBomDetailList.Count > 0)
            {
                ProcessUpdateOrderBomDetails(updateOrderBomDetailList);
            }

            if (deleteOrderBomDetailList != null && deleteOrderBomDetailList.Count > 0)
            {
                IList<int> deleteOrderBomDetailIds = (from orderBomDet in deleteOrderBomDetailList
                                                      select orderBomDet.Id).ToList();
                ProcessDeleteOrderBomDetails(deleteOrderBomDetailIds);
            }
        }

        private void BeforeBatchUpdateOrderBomDetails(OrderMaster orderMaster)
        {
            BeforeUpdateOrderBomDetails(orderMaster);
        }
        #endregion

        #region 添加BOM
        [Transaction(TransactionMode.Requires)]
        public void AddOrderBomDetails(int orderDetailId, IList<OrderBomDetail> orderBomDetailList)
        {
            OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(orderDetailId);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);
            BeforeAddOrderBomDetails(orderMaster);
            ProcessAddOrderBomDetails(orderDetail, orderBomDetailList);
        }

        private void BeforeAddOrderBomDetails(OrderMaster orderMaster)
        {
            if (!(orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Complete   //释放和执行中都能修改BOM Detail
                //&& orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Close
                   && orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Cancel))
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenAddOrderOperation, orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void ProcessAddOrderBomDetails(OrderDetail orderDetail, IList<OrderBomDetail> orderBomDetailList)
        {
            if (orderBomDetailList != null && orderBomDetailList.Count > 0)
            {
                #region 查找最大序号的OrderBomDetail
                string hql = "select max(Sequence) as MaxSeq from OrderBomDetail where OrderDetailId = ?";

                IList<int?> maxSeqList = this.genericMgr.FindAll<int?>(hql, orderDetail.Id);
                int maxSeq = 0;
                if (maxSeqList != null && maxSeqList.Count > 0 && maxSeqList[0] != null)
                {
                    maxSeq = maxSeqList[0].Value;
                }
                #endregion

                foreach (OrderBomDetail orderBomDetail in orderBomDetailList.OrderBy(o => o.Operation).ThenBy(o => o.OpReference))
                {
                    orderBomDetail.Sequence = ++maxSeq;
                    orderBomDetail.OrderNo = orderDetail.OrderNo;
                    orderBomDetail.OrderType = orderDetail.OrderType;
                    orderBomDetail.OrderSubType = orderDetail.OrderSubType;
                    orderBomDetail.OrderDetailId = orderDetail.Id;
                    orderBomDetail.OrderDetailSequence = orderDetail.Sequence;
                    this.genericMgr.Create(orderBomDetail);
                }
            }
        }
        #endregion

        #region 更新BOM
        [Transaction(TransactionMode.Requires)]
        public void UpdateOrderBomDetails(IList<OrderBomDetail> orderBomDetails)
        {
            if (orderBomDetails == null && orderBomDetails.Count == 0)
            {
                throw new TechnicalException("OrderBomDetails is null.");
            }

            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderBomDetails[0].OrderNo);
            BeforeUpdateOrderBomDetails(orderMaster);
            ProcessUpdateOrderBomDetails(orderBomDetails);
        }

        private void BeforeUpdateOrderBomDetails(OrderMaster orderMaster)
        {
            if (!(orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Complete   //释放和执行中都能修改BOM Detail
                //&& orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Close
                && orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Cancel))
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenModifyOrderOperation,
                    orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void ProcessUpdateOrderBomDetails(IList<OrderBomDetail> orderBomDetails)
        {
            foreach (OrderBomDetail orderBomDetail in orderBomDetails)
            {
                this.genericMgr.Update(orderBomDetail);
            }
        }
        #endregion

        #region 删除BOM
        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderBomDetails(IList<int> orderBomDetailIds)
        {
            orderBomDetailIds = orderBomDetailIds.Where(i => i != 0).ToList<int>();
            if (orderBomDetailIds == null && orderBomDetailIds.Count == 0)
            {
                throw new TechnicalException("OrderBomDetailIds is null.");
            }
            OrderBomDetail orderBomDetail = genericMgr.FindById<OrderBomDetail>(orderBomDetailIds[0]);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderBomDetail.OrderNo);

            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    //throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //}

            BeforeDeleteOrderBomDetails(orderMaster);
            ProcessDeleteOrderBomDetails(orderBomDetailIds);
        }

        private void BeforeDeleteOrderBomDetails(OrderMaster orderMaster)
        {
            if (!(orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Complete   //释放和执行中都能修改BOM Detail
                //&& orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Close
                && orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Cancel))
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenModifyOrderOperation,
                    orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void ProcessDeleteOrderBomDetails(IList<int> orderBomDetailIds)
        {
            string hql = "from OrderBomDetail where Id in (";
            object[] para = new object[orderBomDetailIds.Count];

            for (int i = 0; i < orderBomDetailIds.Count; i++)
            {
                if (i == 0)
                {
                    hql += "?";
                }
                else
                {
                    hql += ", ?";
                }
                para[i] = orderBomDetailIds[i];
            }

            hql += ")";

            genericMgr.Delete(genericMgr.FindAll<OrderBomDetail>(hql, para));
        }
        #endregion

        #region 展开BOM
        [Transaction(TransactionMode.Requires)]
        public IList<OrderBomDetail> ExpandOrderBomDetail(int orderDetailId)
        {
            OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(orderDetailId);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);

            return ExpandOrderBomDetail(orderMaster, orderDetail);
        }

        private IList<OrderBomDetail> ExpandOrderBomDetail(OrderMaster orderMaster, OrderDetail orderDetail)
        {
            BeforeExpandOrderBomDetail(orderMaster);
            return ProcessExpandOrderBomDetail(orderMaster, orderDetail);
        }

        private void BeforeExpandOrderBomDetail(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenExpandOrderBomDetail,
                    orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private IList<OrderBomDetail> ProcessExpandOrderBomDetail(OrderMaster orderMaster, OrderDetail orderDetail)
        {
            string hql = "from OrderBomDetail where OrderDetailId = ?";
            genericMgr.Delete(genericMgr.FindAll<OrderBomDetail>(hql, orderDetail.Id));
            GenerateOrderBomDetail(orderDetail, orderMaster);
            ProcessAddOrderBomDetails(orderDetail, orderDetail.OrderBomDetails);

            return orderDetail.OrderBomDetails;
        }
        #endregion

        #region 展开工艺流程和BOM
        [Transaction(TransactionMode.Requires)]
        public object[] ExpandOrderOperationAndBomDetail(int orderDetailId)
        {
            OrderDetail orderDetail = genericMgr.FindById<OrderDetail>(orderDetailId);
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderDetail.OrderNo);

            return ExpandOrderOperationAndBomDetail(orderMaster, orderDetail);
        }

        private object[] ExpandOrderOperationAndBomDetail(OrderMaster orderMaster, OrderDetail orderDetail)
        {
            BeforeExpandOrderOperationAndBomDetail(orderMaster);
            object[] returnList = new object[2];
            returnList[0] = this.ProcessExpandOrderOperation(orderMaster, orderDetail);
            returnList[1] = this.ProcessExpandOrderBomDetail(orderMaster, orderDetail);

            return returnList;
        }

        private void BeforeExpandOrderOperationAndBomDetail(OrderMaster orderMaster)
        {
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenExpandOrderOperationAndBomDetail,
                    orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }
        #endregion
        #endregion
        #endregion

        #region 订单释放
        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(string orderNo)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            this.ReleaseOrder(orderMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(OrderMaster orderMaster)
        {
            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //}

            if (orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create)
            {
                #region 验证OrderDetail不能为空
                TryLoadOrderDetails(orderMaster);
                if (orderMaster.OrderDetails == null || orderMaster.OrderDetails.Count == 0)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_OrderDetailIsEmpty);
                }
                #endregion

                #region 暂估价释放
                if (orderMaster.Type == CodeMaster.OrderType.Procurement && orderMaster.SubType == CodeMaster.OrderSubType.Normal)
                {
                    FlowMaster flowMaster = orderMaster.CurrentFlowMaster;
                    if (flowMaster == null)
                    {
                        flowMaster = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);
                        orderMaster.CurrentFlowMaster = flowMaster;
                    }
                    if (!flowMaster.IsAllowProvEstRec)
                    {
                        foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                        {
                            if (orderDetail.IsProvisionalEstimate || orderDetail.UnitPrice == 0M)
                            {
                                throw new BusinessException("此采购路线{0}不允许暂估价释放", orderMaster.Flow);
                            }
                        }
                    }
                }
                if (orderMaster.Type == CodeMaster.OrderType.Procurement || orderMaster.Type == CodeMaster.OrderType.SubContract
                    || orderMaster.Type == CodeMaster.OrderType.Distribution)
                {
                    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                    {
                        if (orderDetail.UnitPrice == 0M)
                        {
                            throw new BusinessException("物料{0}价格为0不能释放", orderDetail.Item);
                        }
                    }
                }
                #endregion

                #region 验证OrderBomDetail不能为空
                if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Production
                    || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                {
                    TryLoadOrderBomDetails(orderMaster);
                    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                    {
                        if (orderDetail.OrderBomDetails == null || orderDetail.OrderBomDetails.Count == 0)
                        {
                            //throw new BusinessException(Resources.ORD.OrderMaster.Errors_OrderBomDetailIsEmpty, orderDetail.Item);
                        }
                    }
                }
                #endregion

                #region 订单、生产线暂停检查
                if (orderMaster.IsPause)
                {
                    if (orderMaster.Type == CodeMaster.OrderType.Production)
                    {
                        throw new BusinessException("生产单{0}已经暂停，不能释放。", orderMaster.OrderNo);
                    }
                    else if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                    {
                        throw new BusinessException("KIT单{0}已经暂停，不能释放。", orderMaster.OrderNo);
                    }
                    else if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
                    {
                        throw new BusinessException("排序单{0}已经暂停，不能释放。", orderMaster.OrderNo);
                    }
                    else
                    {
                        throw new BusinessException("订单{0}已经暂停，不能释放。", orderMaster.OrderNo);
                    }
                }

                if ((orderMaster.Type == CodeMaster.OrderType.Production
                    || orderMaster.Type == CodeMaster.OrderType.SubContract
                    || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT
                    || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
                    && !string.IsNullOrWhiteSpace(orderMaster.Flow))
                {
                    FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);
                    if (flowMaster.IsPause)
                    {
                        throw new BusinessException("生产线{0}已经暂停，不能释放。", orderMaster.Flow);
                    }
                }
                #endregion

                #region 释放的时候要把明细上locfrom，locto，Routing，BillAddr，PriceList，BillTerm为空的保存为头上的默认值
                var locationDic = GetLocationDic(orderMaster);
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    bool needUpdateOrderDetail = false;

                    if (orderDetail.BillTerm == CodeMaster.OrderBillTerm.AfterInspection)
                    {
                        orderDetail.IsInspect = true;
                        needUpdateOrderDetail = true;
                    }

                    if (orderDetail.Direction == null)
                    {
                        orderDetail.Direction = string.Empty;
                        needUpdateOrderDetail = true;
                    }

                    if (string.IsNullOrWhiteSpace(orderDetail.LocationFrom)
                        && !string.IsNullOrWhiteSpace(orderMaster.LocationFrom))
                    {
                        orderDetail.LocationFrom = orderMaster.LocationFrom;
                        orderDetail.LocationFromName = orderMaster.LocationFromName;
                        needUpdateOrderDetail = true;
                    }

                    if (string.IsNullOrWhiteSpace(orderDetail.LocationTo)
                        && !string.IsNullOrWhiteSpace(orderMaster.LocationTo))
                    {
                        orderDetail.LocationTo = orderMaster.LocationTo;
                        orderDetail.LocationToName = orderMaster.LocationToName;

                        needUpdateOrderDetail = true;
                    }

                    if (string.IsNullOrWhiteSpace(orderDetail.Routing)
                        && !string.IsNullOrWhiteSpace(orderMaster.Routing))
                    {
                        orderDetail.Routing = orderMaster.Routing;
                        needUpdateOrderDetail = true;
                    }

                    if (string.IsNullOrWhiteSpace(orderDetail.BillAddress)
                        && !string.IsNullOrWhiteSpace(orderMaster.BillAddress))
                    {
                        orderDetail.BillAddress = orderMaster.BillAddress;
                        orderDetail.BillAddressDescription = orderMaster.BillAddressDescription;
                        needUpdateOrderDetail = true;
                    }

                    if (string.IsNullOrWhiteSpace(orderDetail.PriceList)
                        && !string.IsNullOrWhiteSpace(orderMaster.PriceList))
                    {
                        orderDetail.PriceList = orderMaster.PriceList;
                        needUpdateOrderDetail = true;
                    }

                    if (orderDetail.BillTerm == com.Sconit.CodeMaster.OrderBillTerm.NA
                        && orderMaster.BillTerm != com.Sconit.CodeMaster.OrderBillTerm.NA)
                    {
                        orderDetail.BillTerm = orderMaster.BillTerm;
                        needUpdateOrderDetail = true;
                    }

                    if ((orderMaster.Type == com.Sconit.CodeMaster.OrderType.Procurement
                        || orderMaster.Type == com.Sconit.CodeMaster.OrderType.ScheduleLine
                        || orderMaster.Type == com.Sconit.CodeMaster.OrderType.Distribution
                         || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                        && string.IsNullOrWhiteSpace(orderDetail.Currency))
                    {
                        if (string.IsNullOrWhiteSpace(orderMaster.Currency))
                        {
                            //throw new BusinessException("订单{0}没有指定币种", orderMaster.OrderNo);
                        }
                        orderDetail.Currency = orderMaster.Currency;
                        needUpdateOrderDetail = true;
                    }

                    if (string.IsNullOrWhiteSpace(orderDetail.PickStrategy))
                    {
                        orderDetail.PickStrategy = orderMaster.PickStrategy;
                        needUpdateOrderDetail = true;
                    }
                    if (!string.IsNullOrWhiteSpace(orderDetail.LocationFrom) && string.IsNullOrWhiteSpace(orderDetail.LocationFromName))
                    {
                        orderDetail.LocationFromName = locationDic[orderDetail.LocationFrom].Name;
                        needUpdateOrderDetail = true;
                    }
                    if (!string.IsNullOrWhiteSpace(orderDetail.LocationTo) && string.IsNullOrWhiteSpace(orderDetail.LocationToName))
                    {
                        orderDetail.LocationToName = locationDic[orderDetail.LocationTo].Name;
                        needUpdateOrderDetail = true;
                    }

                    if (string.IsNullOrWhiteSpace(orderDetail.ReferenceItemCode))
                    {
                        var item = itemMgr.GetCacheItem(orderDetail.Item);
                        if (!string.IsNullOrWhiteSpace(item.ReferenceCode))
                        {
                            orderDetail.ReferenceItemCode = item.ReferenceCode;
                            needUpdateOrderDetail = true;
                        }
                    }
                    if (needUpdateOrderDetail)
                    {
                        genericMgr.Update(orderDetail);
                    }
                }
                #endregion

                //todo:生产单ATP检查，可以强制释放              

                #region 更新订单
                //if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT
                //    || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
                //{
                //    //如果是KIT单号SEQ单，送货单只能一次性收货。
                //    orderMaster.IsAsnUniqueReceive = true;
                //}
                if (orderMaster.BillTerm == CodeMaster.OrderBillTerm.AfterInspection)
                {
                    orderMaster.IsInspect = true;
                }
                //if (orderMaster.IsShipScanHu)
                //{
                //    orderMaster.IsAutoShip = false;
                //}
                if (orderMaster.IsReceiveScanHu && !orderMaster.IsShipScanHu)
                {
                    orderMaster.IsAutoReceive = false;
                }
                orderMaster.Status = com.Sconit.CodeMaster.OrderStatus.Submit;
                orderMaster.ReleaseDate = DateTime.Now;
                User user = SecurityContextHolder.Get();
                orderMaster.ReleaseUserId = user.Id;
                orderMaster.ReleaseUserName = user.FullName;
                genericMgr.Update(orderMaster);
                #endregion

                #region 发送打印
                this.AsyncSendPrintData(orderMaster);
                #endregion

                #region 自动Start
                if (orderMaster.IsQuick || orderMaster.IsAutoStart)
                {
                    StartOrder(orderMaster);
                }
                #endregion

                #region 生成发货任务
                if (!(orderMaster.IsQuick || orderMaster.IsAutoStart)
                    && (orderMaster.Type == CodeMaster.OrderType.Transfer
                        || orderMaster.Type == CodeMaster.OrderType.Distribution))
                {
                    this.genericMgr.FlushSession();
                    this.shipPlanMgr.CreateShipPlan(orderMaster.OrderNo);
                }
                #endregion

                #region 触发订单释放事件
                if (OrderReleased != null)
                {
                    //Call the Event
                    OrderReleased(orderMaster);
                }
                #endregion

                #region 创建绑定的订单
                //AsyncCreateBindOrder(orderMaster, CodeMaster.BindType.Submit);
                CreateBindOrder(orderMaster, CodeMaster.BindType.Submit);
                #endregion
            }
            else
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenRelease,
                    orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private Dictionary<string, Location> GetLocationDic(OrderMaster orderMaster)
        {
            var locationCodes = new List<string>();
            if (!string.IsNullOrWhiteSpace(orderMaster.LocationFrom))
            {
                locationCodes.Add(orderMaster.LocationFrom);
            }
            if (!string.IsNullOrWhiteSpace(orderMaster.LocationTo))
            {
                locationCodes.Add(orderMaster.LocationTo);
            }
            if (orderMaster.OrderDetails != null)
            {
                locationCodes.AddRange(orderMaster.OrderDetails
                    .Where(p => !string.IsNullOrWhiteSpace(p.LocationFrom))
                    .Select(p => p.LocationFrom)
                    .Union(orderMaster.OrderDetails.Where(p => !string.IsNullOrWhiteSpace(p.LocationTo)).Select(p => p.LocationTo))
                    .Distinct());
            }
            var locationDic = this.genericMgr.FindAllIn<Location>
                ("from Location where Code in(?", locationCodes).ToDictionary(d => d.Code, d => d);
            return locationDic;
        }
        #endregion

        #region 订单上线
        [Transaction(TransactionMode.Requires)]
        public void StartOrder(string orderNo)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            this.StartOrder(orderMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartOrder(OrderMaster orderMaster)
        {
            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //}

            if (orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Submit)
            {
                TryLoadOrderDetails(orderMaster);

                #region 排序单不支持手工上线
                if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
                {
                    throw new BusinessException("排序单{0}不支持手工上线。", orderMaster.OrderNo);
                }
                #endregion

                #region 订单、生产线暂停检查
                if (orderMaster.IsPause)
                {
                    if (orderMaster.Type == CodeMaster.OrderType.Production)
                    {
                        throw new BusinessException("生产单{0}已经暂停，不能上线。", orderMaster.OrderNo);
                    }
                    else if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                    {
                        throw new BusinessException("KIT单{0}已经暂停，不能上线。", orderMaster.OrderNo);
                    }
                    else if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
                    {
                        throw new BusinessException("排序单{0}已经暂停，不能上线。", orderMaster.OrderNo);
                    }
                    else
                    {
                        throw new BusinessException("订单{0}已经暂停，不能上线。", orderMaster.OrderNo);
                    }
                }

                if ((orderMaster.Type == CodeMaster.OrderType.Production
                    || orderMaster.Type == CodeMaster.OrderType.SubContract
                    || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT
                    || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
                    && !string.IsNullOrWhiteSpace(orderMaster.Flow))
                {
                    FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);
                    if (flowMaster.IsPause)
                    {
                        throw new BusinessException("生产线{0}已经暂停，不能上线。", orderMaster.Flow);
                    }
                }
                #endregion

                #region 检查生产单最大上线数量
                //因为递延扣减不能限制最大上线数，最大上线数用作最大驾驶室出库数
                if (false && !string.IsNullOrWhiteSpace(orderMaster.Flow)
                    && (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Production || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                    && orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    #region 查找生产线最大上线数量
                    string maxOrderCountHql = "select a.MaxOrderCount as count from FlowMaster as a where a.Code = ?";
                    int maxOrderCount = this.genericMgr.FindAll<int>(maxOrderCountHql, orderMaster.Flow)[0];
                    #endregion

                    if (maxOrderCount > 0)
                    {
                        #region 查找已上线生产单数量
                        string startedOrderCountHql = "select count(*) as counter from ORD_OrderMstr_4 as a where a.Type = ? and a.Flow = ? and a.Status = ?";
                        long startedOrderCount = this.genericMgr.FindAllWithNativeSql<long>(startedOrderCountHql, new Object[] { orderMaster.Type, orderMaster.Flow, com.Sconit.CodeMaster.OrderStatus.InProcess })[0];
                        #endregion

                        if (startedOrderCount >= maxOrderCount)
                        {
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ExcceedMaxOnlineCount, orderMaster.Flow);
                        }
                    }

                    //todo处理ProductLine Facility
                }
                #endregion

                #region 更新订单
                UpdateOrderMasterStatus2InProcess(orderMaster);
                #endregion

                #region 自动捡货/发货/收货
                AutoShipAndReceive(orderMaster);
                #endregion

                #region 绑定同步
                //AsyncCreateBindOrder(orderMaster, CodeMaster.BindType.Start);
                CreateBindOrder(orderMaster, CodeMaster.BindType.Start);
                #endregion
            }
            else
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenStart,
                       orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void AutoShipAndReceive(OrderMaster orderMaster)
        {
            #region 自动捡货/发货/收货
            if (orderMaster.IsQuick ||
                (orderMaster.IsAutoShip && orderMaster.IsAutoReceive && orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Normal
                && !(orderMaster.IsCreatePickList && orderMaster.IsShipScanHu)))
            {
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (orderDetail.OrderDetailInputs == null || orderDetail.OrderDetailInputs.Count == 0)
                    {
                        OrderDetailInput orderDetailInput = new OrderDetailInput();
                        if (orderDetail.OrderType == CodeMaster.OrderType.Production)
                        {
                            orderDetailInput.ScrapQty = orderDetail.CurrentScrapQty;
                            orderDetailInput.ReceiveQty = orderDetail.OrderedQty - orderDetail.CurrentScrapQty;
                            if (orderDetail.OrderedQty > 0)
                            {
                                orderDetailInput.ReceiveQty = orderDetailInput.ReceiveQty > 0 ? orderDetailInput.ReceiveQty : 0;
                            }
                            else
                            {
                                orderDetailInput.ReceiveQty = orderDetailInput.ReceiveQty < 0 ? orderDetailInput.ReceiveQty : 0;
                            }
                        }
                        else
                        {
                            orderDetailInput.ReceiveQty = orderDetail.OrderedQty;
                        }
                        orderDetail.AddOrderDetailInput(orderDetailInput);
                    }
                }

                #region 加一段下架的逻辑
                foreach (OrderDetail od in orderMaster.OrderDetails)
                {
                    var hus = this.genericMgr.FindAllIn<Hu>(" from Hu where HuId in(?", od.OrderDetailInputs.Select(p => p.HuId));
                    foreach (Hu hu in hus)
                    {
                        var inventoryPickList = new List<Entity.INV.InventoryPick>();
                        var inventoryPick = new Entity.INV.InventoryPick();
                        inventoryPick.HuId = hu.HuId;
                        inventoryPickList.Add(inventoryPick);
                        this.locationDetailMgr.InventoryPick(inventoryPickList);

                    }
                }
                #endregion

                ReceiveOrder(orderMaster.OrderDetails, orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value : DateTime.Now);
            }
            else if (!orderMaster.IsQuick && orderMaster.Type != com.Sconit.CodeMaster.OrderType.Production          //自动生成捡货单
                && orderMaster.Type != com.Sconit.CodeMaster.OrderType.SubContract
                && orderMaster.IsCreatePickList && orderMaster.IsShipScanHu
                && orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Normal //过滤掉退货
                && orderMaster.QualityType == com.Sconit.CodeMaster.QualityType.Qualified)   //过滤掉不合格品和待验物料
            {
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    OrderDetailInput orderDetailInput = new OrderDetailInput();
                    orderDetailInput.PickQty = orderDetail.OrderedQty;
                    orderDetail.AddOrderDetailInput(orderDetailInput);
                }
                pickListMgr.CreatePickList(orderMaster.OrderDetails);
            }
            else if (!orderMaster.IsQuick && orderMaster.Type != com.Sconit.CodeMaster.OrderType.Production       //生产单和委外加工没有发货概念                
                && orderMaster.Type != com.Sconit.CodeMaster.OrderType.SubContract
                && orderMaster.IsAutoShip
                && orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Normal                         //过滤掉退货
                //&& orderMaster.QualityType == com.Sconit.CodeMaster.QualityType.Qualified                      //过滤掉不合格品和待验物料
                && !(orderMaster.IsCreatePickList && orderMaster.IsShipScanHu))  //自动捡货和自动发货/自动收货冲突，如果设置了自动捡货将不考虑自动发货/自动收货选项
            {
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (orderDetail.OrderDetailInputs == null || orderDetail.OrderDetailInputs.Count == 0)
                    {
                        OrderDetailInput orderDetailInput = new OrderDetailInput();
                        orderDetailInput.ShipQty = orderDetail.OrderedQty;
                        orderDetail.AddOrderDetailInput(orderDetailInput);
                    }
                }
                ShipOrder(orderMaster.OrderDetails, orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value : DateTime.Now);
            }
            //else if (orderMaster.IsQuick //&& !orderMaster.IsShipScanHu && !orderMaster.IsReceiveScanHu        //快速订单直接收货，跳过发货和捡货
            //    || (orderMaster.IsAutoReceive && !orderMaster.IsShipByOrder                                   //不是订单发货
            //     && orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Normal                         //过滤掉退货
            //    && orderMaster.QualityType == com.Sconit.CodeMaster.QualityType.Qualified                      //过滤掉不合格品和待验物料
            //    && !(orderMaster.IsCreatePickList && orderMaster.IsShipScanHu)))  //支持不发货直接收货
            //{
            //    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
            //    {
            //        if (orderDetail.OrderDetailInputs == null || orderDetail.OrderDetailInputs.Count == 0)
            //        {
            //            OrderDetailInput orderDetailInput = new OrderDetailInput();
            //            if (orderDetail.OrderType == CodeMaster.OrderType.Production)
            //            {
            //                orderDetailInput.ScrapQty = orderDetail.CurrentScrapQty;
            //                orderDetailInput.ReceiveQty = orderDetail.OrderedQty - orderDetail.CurrentScrapQty;
            //                if (orderDetail.OrderedQty > 0)
            //                {
            //                    orderDetailInput.ReceiveQty = orderDetailInput.ReceiveQty > 0 ? orderDetailInput.ReceiveQty : 0;
            //                }
            //                else
            //                {
            //                    orderDetailInput.ReceiveQty = orderDetailInput.ReceiveQty < 0 ? orderDetailInput.ReceiveQty : 0;
            //                }
            //            }
            //            else
            //            {
            //                orderDetailInput.ReceiveQty = orderDetail.OrderedQty;
            //            }
            //            orderDetail.AddOrderDetailInput(orderDetailInput);
            //        }
            //    }
            //    ReceiveOrder(orderMaster.OrderDetails, orderMaster.EffectiveDate.HasValue ? orderMaster.EffectiveDate.Value : DateTime.Now);
            //}
            #endregion
        }

        private void UpdateOrderMasterStatus2InProcess(OrderMaster orderMaster)
        {
            orderMaster.Status = com.Sconit.CodeMaster.OrderStatus.InProcess;
            orderMaster.StartDate = DateTime.Now;
            User user = SecurityContextHolder.Get();
            orderMaster.StartUserId = user.Id;
            orderMaster.StartUserName = user.FullName;
            genericMgr.Update(orderMaster);
        }
        #endregion

        #region 计划协议发货
        public IpMaster ShipScheduleLine(IList<ScheduleLineInput> scheduleLineInputList)
        {
            IList<OrderDetail> shipOrderDetailList = new List<OrderDetail>();
            foreach (ScheduleLineInput scheduleLineInput in scheduleLineInputList)
            {
                decimal remainShipQty = scheduleLineInput.ShipQty;

                IList<OrderDetail> orderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>("select * from ORD_OrderDet_8 where ExtNo = ? and ExtSeq like ? and ScheduleType = ? and OrderQty > ShipQty order by EndDate",
                    new object[] { scheduleLineInput.EBELN, scheduleLineInput.EBELP + "-%", CodeMaster.ScheduleType.Firm });

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    OrderDetailInput orderDetailInput = new OrderDetailInput();
                    orderDetail.AddOrderDetailInput(orderDetailInput);
                    shipOrderDetailList.Add(orderDetail);

                    if (remainShipQty > orderDetail.RemainShippedQty)
                    {
                        orderDetailInput.ShipQty = orderDetail.RemainShippedQty;
                        remainShipQty -= orderDetail.RemainShippedQty;
                    }
                    else
                    {
                        orderDetailInput.ShipQty = remainShipQty;
                        remainShipQty = 0;
                        break;
                    }
                }

                if (remainShipQty > 0)
                {
                    throw new BusinessException("计划协议号{0}行号{1}的需求数不足。", scheduleLineInput.EBELN, scheduleLineInput.EBELP);
                }
            }

            return ShipOrder(shipOrderDetailList);
        }
        #endregion

        #region 订单发货
        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipOrder(IList<OrderDetail> orderDetailList, bool isOpPallet = false)
        {
            return ShipOrder(orderDetailList, DateTime.Now,  isOpPallet);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipOrder(IList<OrderDetail> orderDetailList, DateTime effectiveDate, bool isOpPallet = false)
        {
            return ShipOrder(orderDetailList, true, effectiveDate,  isOpPallet);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipOrder(IList<OrderDetail> orderDetailList, bool isCheckKitTraceItem, DateTime effectiveDate, bool isOpPallet = false)
        {
            #region 判断是否全0发货
            if (orderDetailList == null || orderDetailList.Count == 0)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipDetailIsEmpty);
            }

            IList<OrderDetail> nonZeroOrderDetailList = orderDetailList.Where(o => o.ShipQtyInput != 0).ToList();

            if (nonZeroOrderDetailList.Count == 0)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipDetailIsEmpty);
            }
            #endregion

            #region 查询订单头对象
            IList<OrderMaster> orderMasterList = LoadOrderMasters(nonZeroOrderDetailList.Select(p => p.OrderNo).Distinct(), true);

            //User user = SecurityContextHolder.Get();
            //foreach (var orderMaster in orderMasterList)
            //{
            //    if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //    {
            //        throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //    }
            //}
            #endregion

            //#region 按订单明细汇总发货数，按条码发货一条订单明细会对应多条发货记录
            //var summaryOrderDet = from det in orderDetailList
            //                      group det by new { Id = det.Id, OrderNo = det.OrderNo } into g
            //                      select new
            //                      {
            //                          Id = g.Key.Id,
            //                          OrderNo = g.Key.OrderNo,
            //                          ShipQty = g.Sum(det => det.CurrentShipQty)
            //                      };
            //#endregion

            #region 循环订单头检查
            IList<com.Sconit.CodeMaster.OrderType> orderTypeList = (from orderMaster in orderMasterList
                                                                    group orderMaster by orderMaster.Type into result
                                                                    select result.Key).ToList();

            if (orderTypeList.Count > 1)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_CannotMixOrderTypeShip);
            }

            com.Sconit.CodeMaster.OrderType orderType = orderTypeList.First();

            #region 排序单/KIT单顺序发货校验
            //没有考虑多个路线的排序单/Kit单同时发货
            //var groupedSeqOrderMaster = from mstr in orderMasterList
            //                            where !string.IsNullOrWhiteSpace(mstr.Flow)
            //                                    && (mstr.OrderStrategy == CodeMaster.FlowStrategy.KIT
            //                                    || mstr.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
            //                            group mstr by new { Flow = mstr.Flow, OrderStrategy = mstr.OrderStrategy } into result
            //                            select new
            //                            {
            //                                Flow = result.Key.Flow,
            //                                OrderStrategy = result.Key.OrderStrategy,
            //                                List = result.ToList()
            //                            };

            //if (groupedSeqOrderMaster != null && groupedSeqOrderMaster.Count() > 0)
            //{
            //    BusinessException businessException = new BusinessException();
            //    foreach (var seqOrderMaster in groupedSeqOrderMaster)
            //    {
            //        OrderMaster biggestSeqOrderMaster = seqOrderMaster.List.OrderBy(o => o.Sequence).LastOrDefault();

            //        if (biggestSeqOrderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
            //        {
            //            if (this.GetSpecifiedStatusOrderCount(biggestSeqOrderMaster, CodeMaster.OrderStatus.Submit)   //小与等于最大序号的待发货Kit单的总数
            //                > seqOrderMaster.List.Count())                                                          //当前待发货排序单/Kit单的总数
            //            {
            //                businessException.AddMessage("KIT路线{0}没有按顺序进行发货。", biggestSeqOrderMaster.Flow);
            //            }
            //        }
            //        else
            //        {
            //            if (this.GetSpecifiedStatusOrderCount(biggestSeqOrderMaster, CodeMaster.OrderStatus.InProcess)       //小与等于最大序号的待发货排序单的总数，创建了排序装箱单之后，排序单的状态为InProcess
            //                > seqOrderMaster.List.Count())                                                                  //当前待发货排序单/Kit单的总数
            //            {
            //                businessException.AddMessage("KIT路线{0}没有按顺序进行发货。", biggestSeqOrderMaster.Flow);
            //            }
            //        }
            //    }

            //    if (businessException.HasMessage)
            //    {
            //        throw businessException;
            //    }
            //}
            #endregion

            foreach (OrderMaster orderMaster in orderMasterList)
            {
                orderMaster.OrderDetails = nonZeroOrderDetailList.Where(det => det.OrderNo == orderMaster.OrderNo).ToList();

                //生产单不支持发货
                //if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Production
                //    || orderMaster.Type == com.Sconit.CodeMaster.OrderType.SubContract)
                //{
                //    throw new TechnicalException("Production Order not support ship operation.");
                //}

                //如果非生产，把Submit状态改为InProcess
                if (orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Submit
                    && orderMaster.Type != com.Sconit.CodeMaster.OrderType.Production
                    && orderMaster.Type != com.Sconit.CodeMaster.OrderType.SubContract)
                {
                    UpdateOrderMasterStatus2InProcess(orderMaster);
                }

                //判断OrderHead状态
                if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.InProcess)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenShip,
                          orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
                }

                #region 订单、生产线暂停检查
                if (orderMaster.IsPause)
                {
                    if (orderMaster.Type == CodeMaster.OrderType.Production)
                    {
                        throw new BusinessException("生产单{0}已经暂停，不能发货。", orderMaster.OrderNo);
                    }
                    else if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                    {
                        throw new BusinessException("KIT单{0}已经暂停，不能发货。", orderMaster.OrderNo);
                    }
                    else if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
                    {
                        throw new BusinessException("排序单{0}已经暂停，不能发货。", orderMaster.OrderNo);
                    }
                    else
                    {
                        throw new BusinessException("订单{0}已经暂停，不能发货。", orderMaster.OrderNo);
                    }
                }

                //if ((orderMaster.Type == CodeMaster.OrderType.Production
                //    || orderMaster.Type == CodeMaster.OrderType.SubContract
                //    || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT
                //    || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
                //    && !string.IsNullOrWhiteSpace(orderMaster.Flow))
                //{
                //    FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);
                //    if (flowMaster.IsPause)
                //    {
                //        throw new BusinessException("生产线{0}已经暂停，不能发货。", orderMaster.Flow);
                //    }
                //}
                #endregion

                #region 整包装发货判断,快速的不要判断
                if (orderMaster.IsShipFulfillUC
                    && orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Normal
                    && !(orderMaster.IsAutoRelease && orderMaster.IsAutoStart)
                    && orderMaster.Type != CodeMaster.OrderType.ScheduleLine)   //计划协议不检查整包装发货选项。
                {
                    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                    {
                        foreach (OrderDetailInput orderDetailInput in orderDetail.OrderDetailInputs)
                        {
                            if (orderDetailInput.ShipQty % orderDetail.UnitCount != 0)
                            {
                                //不是整包装
                                throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipQtyNotFulfillUnitCount, orderDetail.Item, orderDetail.UnitCount.ToString("0.##"));
                            }
                        }
                    }
                }
                #endregion

                #region 是否过量发货判断
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (!orderMaster.IsOpenOrder)
                    {
                        if (Math.Abs(orderDetail.ShippedQty) >= Math.Abs(orderDetail.OrderedQty))
                        {
                            //订单的发货数已经大于等于订单数
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipQtyExcceedOrderQty, orderDetail.OrderNo, orderDetail.Item);
                        }
                        else if (!orderMaster.IsShipExceed && Math.Abs(orderDetail.ShippedQty + orderDetail.ShipQtyInput) > Math.Abs(orderDetail.OrderedQty))   //不允许过量发货
                        {
                            //订单的发货数 + 本次发货数大于订单数
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipQtyExcceedOrderQty, orderDetail.OrderNo, orderDetail.Item);
                        }
                    }
                }
                #endregion

                #region KIT单发货判断
                if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                {
                    CheckKitOrderDetail(orderMaster, isCheckKitTraceItem, true);
                }
                #endregion

                #region 按数量发货检查指定供应商寄售库存出库
                //if (!orderMaster.IsShipScanHu)
                //{
                //    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                //    {
                //        if (!string.IsNullOrWhiteSpace(orderDetail.ManufactureParty))
                //        {
                //            foreach (OrderDetailInput orderDetailInput in orderDetail.OrderDetailInputs)
                //            {
                //                if (!string.IsNullOrWhiteSpace(orderDetailInput.ManufactureParty))
                //                {
                //                    if (orderDetail.ManufactureParty != orderDetailInput.ManufactureParty)
                //                    {
                //                        //发货零件的供应商和和订单明细的指定供应商不一致
                //                        throw new BusinessException("发货零件{0}的供应商{1}和和订单明细的指定供应商{2}不一致。", orderDetail.Item, orderDetailInput.ManufactureParty, orderDetail.ManufactureParty);
                //                    }
                //                }
                //                else
                //                {
                //                    //发货零件的供应商赋值为订单明细的供应商
                //                    orderDetailInput.ManufactureParty = orderDetail.ManufactureParty;
                //                }
                //            }
                //        }
                //    }
                //}
                #endregion
            }
            #endregion

            #region 循环更新订单明细
            foreach (OrderDetail orderDetail in nonZeroOrderDetailList)
            {
                orderDetail.ShippedQty += orderDetail.ShipQtyInput;
                genericMgr.Update(orderDetail);
            }
            #endregion

            #region 校验发货先进先出
            CheckShipFiFo(orderMasterList);
            #endregion

            #region 先加一段下架
            foreach (OrderMaster om in orderMasterList)
            {
                foreach (OrderDetail od in om.OrderDetails)
                {
                    var hus = this.genericMgr.FindAllIn<Hu>(" from Hu where HuId in(?", od.OrderDetailInputs.Select(p => p.HuId));
                    foreach (Hu hu in hus)
                    {

                        var inventoryPickList = new List<Entity.INV.InventoryPick>();
                        var inventoryPick = new Entity.INV.InventoryPick();
                        inventoryPick.HuId = hu.HuId;
                        inventoryPickList.Add(inventoryPick);
                        this.locationDetailMgr.InventoryPick(inventoryPickList);

                    }
                }

            }
            #endregion


            #region 退货自动解托盘,发货也解托盘
            foreach (OrderMaster om in orderMasterList)
            {
                if (!isOpPallet)
                {
                    foreach (OrderDetail od in om.OrderDetails)
                    {
                        #region 加一段托盘解绑的逻辑
                        var hus = this.genericMgr.FindAllIn<Hu>(" from Hu where HuId in(?", od.OrderDetailInputs.Select(p => p.HuId));
                        var palletHus = this.genericMgr.FindAllIn<PalletHu>(" from PalletHu where HuId in(?", od.OrderDetailInputs.Select(p => p.HuId));
                        foreach (Hu h in hus)
                        {
                            if (!string.IsNullOrEmpty(h.PalletCode))
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
                        #endregion
                    }
                }
            }
            #endregion

            #region 发货
            IpMaster ipMaster = ipMgr.TransferOrder2Ip(orderMasterList);
            ipMgr.CreateIp(ipMaster, effectiveDate);
            #endregion



            #region 自动收货
            AutoReceiveIp(ipMaster, effectiveDate);
            #endregion

            return ipMaster;
        }

        private void CheckShipFiFo(IList<OrderMaster> orderMasterList)
        {
            var orderMaster = orderMasterList.First();
            if (!orderMaster.IsShipScanHu || orderMaster.QualityType == CodeMaster.QualityType.Reject)
            {
                //如果发货不扫描条码,就不校验发货先进先出
                return;
            }

            var isShipFiFo = orderMasterList.Select(p => p.IsShipFifo).Distinct();
            if (isShipFiFo.Count() > 1)
            {
                throw new BusinessException("发货先进先出选项不同不能合并发货。");
            }

            if (!isShipFiFo.First())
            {
                //如果不校验发货先进先出
                return;
            }

            var locations = orderMasterList
                .SelectMany(p => p.OrderDetails)
                .Select(p => p.LocationFrom)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct();
            if (locations.Count() > 1)
            {
                throw new BusinessException("不同的发货库位不能合并发货");
            }

            var distinctItemCodes = orderMasterList
                .SelectMany(p => p.OrderDetails)
                .Select(p => p.Item)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct();

            #region 查找库存
            #region 拼SQL,因为现在一个零件会发多个客户，要按照条码上客户区分库存
            string statement = string.Empty;
            IList<object> detailPara = new List<object>();

            if (orderMaster.Type == CodeMaster.OrderType.Distribution)
            {

                foreach (var itemCode in distinctItemCodes)
                {
                    if (statement == string.Empty)
                    {
                        statement = @"select l.Item,l.HuId, l.LotNo, l.Direction
                        from VIEW_LocationLotDet l where l.HuId is not null
                        and l.Location = ? and l.QualityType = ? and l.OccupyType = ? and l.IsATP = ? and l.IsFreeze = ? and l.ManufactureParty = ?";
                        detailPara.Add(locations.First());
                        detailPara.Add(orderMaster.QualityType);
                        detailPara.Add(CodeMaster.OccupyType.None);
                        detailPara.Add(true);
                        detailPara.Add(false);
                        detailPara.Add(orderMaster.PartyTo);

                        statement += " and l.Item in (?";
                    }
                    else
                    {
                        statement += ", ?";
                    }
                    detailPara.Add(itemCode);
                }
                statement += ") order by l.LotNo Asc ";
            }
            else
            {
                foreach (var itemCode in distinctItemCodes)
                {
                    if (statement == string.Empty)
                    {
                        statement = @"select l.Item,l.HuId, l.LotNo, l.Direction
                        from VIEW_LocationLotDet l where l.HuId is not null
                        and l.Location = ? and l.QualityType = ? and l.OccupyType = ? and l.IsATP = ? and l.IsFreeze = ?";
                        detailPara.Add(locations.First());
                        detailPara.Add(orderMaster.QualityType);
                        detailPara.Add(CodeMaster.OccupyType.None);
                        detailPara.Add(true);
                        detailPara.Add(false);

                        statement += " and l.Item in (?";
                    }
                    else
                    {
                        statement += ", ?";
                    }
                    detailPara.Add(itemCode);
                }
                statement += ") order by l.LotNo Asc ";
            }
            #endregion

            var locationDetailsGroup = this.genericMgr.FindAllWithNativeSql<object[]>(statement, detailPara.ToArray())
                                      .Select(p => new
                                      {
                                          Item = (string)p[0],
                                          HuId = (string)p[1],
                                          LotNo = (string)p[2],
                                          Direction = p[3] == null ? string.Empty : (string)p[3]
                                      })
                                     .GroupBy(p => new { p.Item, p.Direction })
                                     .ToDictionary(d => d.Key, d => d.ToList());
            #endregion

            var orderDetailsGroup = orderMasterList.SelectMany(p => p.OrderDetails)
                .GroupBy(p => new { p.Item, p.Direction });

            var bussinessException = new BusinessException();
            foreach (var orderDetails in orderDetailsGroup)
            {
                var orderDetailInputs = orderDetails.SelectMany(p => p.OrderDetailInputs).ToList();
                var maxLotNo = orderDetailInputs.OrderBy(p => p.LotNo).Last().LotNo;
                var huIds = orderDetailInputs.Select(p => p.HuId).ToList();
                var locationDetails = locationDetailsGroup.ValueOrDefault(orderDetails.Key);
                if (locationDetails == null)
                {
                    throw new BusinessException("没有找到合适的库存");
                }
                var minLotNoLocationDetail = locationDetails.Where(p => !huIds.Contains(p.HuId)).OrderBy(p => p.LotNo).FirstOrDefault();

                string minLotNo = null;
                if (minLotNoLocationDetail != null)
                {
                    minLotNo = minLotNoLocationDetail.LotNo;
                }

                var directionDesc = string.Empty;
                if (!string.IsNullOrWhiteSpace(orderDetails.Key.Direction))
                {
                    var direction = this.genericMgr.FindById<HuTo>(orderDetails.Key.Direction);
                    directionDesc = string.Format("/方向:{0}", direction.CodeDescription);
                }

                var itemDesc = orderDetails.First().ItemDescription;
                if (minLotNo != null && string.Compare(maxLotNo, minLotNo) > 0)
                {
                    throw new BusinessException(string.Format("物料{0}[{1}]{2}违反了先进先出,不能发货",
                        orderDetails.Key.Item, orderDetails.First().ItemDescription, directionDesc));
                }
            }




            if (bussinessException.HasMessage)
            {
                throw bussinessException;
            }
        }


        private long GetSpecifiedStatusOrderCount(OrderMaster orderMaster, CodeMaster.OrderStatus orderStatus)
        {
            string hql = "select count(*) as counter from OrderMaster where Type = ? and Flow = ? and Status = ? and Sequence <= ? and IsPause = ?";
            return this.genericMgr.FindAll<long>(hql, new object[] { orderMaster.Type, orderMaster.Flow, orderStatus, orderMaster.Sequence, false })[0];
        }

        private void CheckKitOrderDetail(OrderMaster orderMaster, bool isCheckKitTraceItem, bool isShip)
        {
            BusinessException businessException = new BusinessException();

            #region 明细行是否收/发货判断
            IList<OrderDetail> unReceivedOrderDetailList = LoadExceptOrderDetail(orderMaster);
            if (unReceivedOrderDetailList != null && unReceivedOrderDetailList.Count > 0)
            {
                foreach (OrderDetail unReceivedOrderDetail in unReceivedOrderDetailList)
                {
                    if (isShip)
                    {
                        businessException.AddMessage("KIT单{0}行号{1}零件号{2}没有发货。", orderMaster.OrderNo, unReceivedOrderDetail.Sequence.ToString(), unReceivedOrderDetail.Item);
                    }
                    else
                    {
                        businessException.AddMessage("KIT单{0}行号{1}零件号{2}没有收货。", orderMaster.OrderNo, unReceivedOrderDetail.Sequence.ToString(), unReceivedOrderDetail.Item);
                    }
                }
            }
            #endregion

            #region 收/发货数是否等于订单数判断
            foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
            {
                if (isShip)
                {
                    if (orderDetail.OrderedQty != orderDetail.ShipQtyInput)
                    {
                        businessException.AddMessage("KIT单{0}行号{1}零件号{2}的发货数和订单数不一致。", orderMaster.OrderNo, orderDetail.Sequence.ToString(), orderDetail.Item);
                    }
                }
                else
                {
                    if (orderDetail.OrderedQty != orderDetail.ReceiveQtyInput)
                    {
                        businessException.AddMessage("KIT单{0}行号{1}零件号{2}的收货数和订单数不一致。", orderMaster.OrderNo, orderDetail.Sequence.ToString(), orderDetail.Item);
                    }
                }
            }
            #endregion

            #region KIT中的关键件是否扫描
            if (isCheckKitTraceItem)
            {
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails.Where(o => o.IsScanHu))
                {
                    if (orderDetail.OrderedQty != orderDetail.OrderDetailInputs.Where(o => !string.IsNullOrWhiteSpace(o.HuId)).Count())
                    {
                        businessException.AddMessage("KIT单{0}行号{1}的关键零件{1}没有扫描。", orderMaster.OrderNo, orderDetail.Sequence.ToString(), orderDetail.Item);
                    }
                }
            }
            #endregion

            if (businessException.HasMessage)
            {
                throw businessException;
            }
        }

        private void CheckKitIpDetail(IpMaster ipMaster, bool isCheckKitTraceItem)
        {
            BusinessException businessException = new BusinessException();

            #region 明细行是否收/发货判断
            IList<IpDetail> unReceivedIpDetailList = LoadExceptIpDetails(ipMaster.IpNo, ipMaster.IpDetails.Select(det => det.Id).ToArray());
            if (unReceivedIpDetailList != null && unReceivedIpDetailList.Count > 0)
            {
                foreach (IpDetail unReceivedIpDetail in unReceivedIpDetailList)
                {
                    businessException.AddMessage("KIT送货单{0}行号{1}零件号{2}没有收货。", ipMaster.IpNo, unReceivedIpDetail.Sequence.ToString(), unReceivedIpDetail.Item);
                }
            }
            #endregion

            #region 收/发货数是否等于订单数判断
            foreach (IpDetail ipDetail in ipMaster.IpDetails)
            {
                if (ipDetail.Qty != ipDetail.ReceiveQtyInput)
                {
                    businessException.AddMessage("KIT送货单{0}行号{1}零件号{2}的收货数和送货数不一致。", ipMaster.IpNo, ipDetail.Sequence.ToString(), ipDetail.Item);
                }
            }
            #endregion

            #region KIT中的关键件是否扫描
            if (isCheckKitTraceItem)
            {
                foreach (IpDetail ipDetail in ipMaster.IpDetails.Where(o => o.IsScanHu))
                {
                    if (ipDetail.Qty != ipDetail.IpDetailInputs.Where(o => !string.IsNullOrWhiteSpace(o.HuId)).Count())
                    {
                        businessException.AddMessage("KIT送货单{0}行号{1}的关键零件{1}没有扫描。", ipMaster.IpNo, ipDetail.Sequence.ToString(), ipDetail.Item);
                    }
                }
            }
            #endregion

            if (businessException.HasMessage)
            {
                throw businessException;
            }
        }
        #endregion

        #region 捡货单发货
        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipPickList(string pickListNo)
        {
            return ShipPickList(this.genericMgr.FindById<PickListMaster>(pickListNo), DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipPickList(string pickListNo, DateTime effectiveDate)
        {
            return ShipPickList(this.genericMgr.FindById<PickListMaster>(pickListNo), effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipPickList(PickListMaster pickListMaster)
        {
            return ShipPickList(pickListMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipPickList(IList<PickListDetail> pickListDetailList)
        {
            return ShipPickList(pickListDetailList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipPickList(IList<PickListDetail> pickListDetailList, DateTime effectiveDate)
        {
            var pickListNoList = pickListDetailList.Select(p => p.PickListNo).Distinct();
            if (pickListNoList.Count() > 1)
            {
                throw new BusinessException("多个拣货单不能同时发货");
            }
            string pickListNo = pickListNoList.First();
            PickListMaster pickListMaster = genericMgr.FindById<PickListMaster>(pickListNo);
            return ShipPickList(pickListMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipPickList(PickListMaster pickListMaster, DateTime effectiveDate)
        {
            TryLoadPickListResults(pickListMaster);
            if (pickListMaster.PickListResults == null || pickListMaster.PickListResults.Count == 0)
            {
                throw new BusinessException("捡货明细不能为空。");
            }

            #region 获取捡货单明细
            TryLoadPickListDetails(pickListMaster);
            #endregion

            #region 获取订单明细
            IList<OrderDetail> orderDetailList = TryLoadOrderDetails(pickListMaster);
            #endregion

            #region 获取订单头
            IList<OrderMaster> orderMasterList = LoadOrderMasters(orderDetailList.Select(det => det.OrderNo).Distinct().ToArray());

            //foreach (var orderMaster in orderMasterList)
            //{
            //    if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //    {
            //        throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //    }
            //}
            #endregion

            #region 更新捡货单明细
            foreach (PickListDetail pickListDetail in pickListMaster.PickListDetails)
            {
                pickListDetail.IsClose = true;
                this.genericMgr.Update(pickListDetail);
            }
            #endregion

            #region 更新订单头
            foreach (OrderMaster orderMaster in orderMasterList)
            {
                if (orderMaster.Status == CodeMaster.OrderStatus.Submit)
                {
                    UpdateOrderMasterStatus2InProcess(orderMaster);
                }
            }
            #endregion

            #region 更新订单明细的捡货数和发货数
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                orderDetail.PickedQty -= pickListMaster.PickListDetails.Where(p => p.OrderDetailId == orderDetail.Id).Sum(p => p.Qty);
                orderDetail.ShippedQty += pickListMaster.PickListResults.Where(p => p.OrderDetailId == orderDetail.Id).Sum(p => p.Qty);
                this.genericMgr.Update(orderDetail);
            }
            #endregion

            #region 发货
            IpMaster ipMaster = ipMgr.TransferPickList2Ip(pickListMaster);
            ipMgr.CreateIp(ipMaster, effectiveDate);
            #endregion

            #region 更新捡货单头
            pickListMaster.Status = CodeMaster.PickListStatus.Close;
            pickListMaster.CloseDate = DateTime.Now;
            pickListMaster.CloseUserId = SecurityContextHolder.Get().Id;
            pickListMaster.CloseUserName = SecurityContextHolder.Get().FullName;

            pickListMaster.IpNo = ipMaster.IpNo;
            this.genericMgr.Update(pickListMaster);
            #endregion

            #region 自动收货
            AutoReceiveIp(ipMaster, effectiveDate);
            #endregion

            return ipMaster;
        }
        #endregion

        #region 订单收货
        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster ReceiveOrder(IList<OrderDetail> orderDetailList)
        {
            return ReceiveOrder(orderDetailList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster ReceiveOrder(IList<OrderDetail> orderDetailList, DateTime effectiveDate)
        {
            return ReceiveOrder(orderDetailList, true, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster ReceiveOrder(IList<OrderDetail> orderDetailList, bool isCheckKitTraceItem, DateTime effectiveDate)
        {
            return ReceiveOrder(orderDetailList, true, null, effectiveDate, null);
        }

        private ReceiptMaster ReceiveOrder(IList<OrderDetail> orderDetailList, string ipNo)
        {
            return ReceiveOrder(orderDetailList, true, null, DateTime.Now, ipNo);
        }

        private ReceiptMaster ReceiveOrder(IList<OrderDetail> orderDetailList, bool isCheckKitTraceItem, ProductLineMap productLineMap, DateTime effectiveDate, string ipNo)
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
            #endregion

            #region 查询订单头对象
            IList<OrderMaster> orderMasterList = LoadOrderMasters(nonZeroOrderDetailList.Select(p => p.OrderNo).Distinct(), true);
            #endregion

            #region 获取收货订单类型
            IList<com.Sconit.CodeMaster.OrderType> orderTypeList = (from orderMaster in orderMasterList
                                                                    group orderMaster by orderMaster.Type into result
                                                                    select result.Key).ToList();

            if (orderTypeList.Count > 1)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_CannotMixOrderTypeReceive);
            }

            com.Sconit.CodeMaster.OrderType orderType = orderTypeList.First();
            #endregion

            #region 计划协议不能按收货校验
            if (orderType == CodeMaster.OrderType.ScheduleLine)
            {
                throw new BusinessException("计划协议不能按订单收货。");
            }
            #endregion

            #region 排序单不能按订单收货校验
            if (orderMasterList.Where(o => o.OrderStrategy == CodeMaster.FlowStrategy.SEQ).Count() > 0)
            {
                throw new BusinessException("排序单不能按订单收货。");
            }
            #endregion

            #region 循环订单头检查
            foreach (OrderMaster orderMaster in orderMasterList)
            {
                //if (!Utility.SecurityHelper.HasPermission(orderMaster))
                //{
                //    throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
                //}

                orderMaster.OrderDetails = nonZeroOrderDetailList.Where(det => det.OrderNo == orderMaster.OrderNo).ToList();

                //如果非生产，把Submit状态改为InProcess
                if (orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Submit
                    && orderMaster.Type != com.Sconit.CodeMaster.OrderType.Production
                    && orderMaster.Type != com.Sconit.CodeMaster.OrderType.SubContract)
                {
                    UpdateOrderMasterStatus2InProcess(orderMaster);
                }

                //判断OrderHead状态
                if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.InProcess)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_StatusErrorWhenReceive,
                            orderMaster.OrderNo, systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
                }

                #region 订单、生产线暂停检查
                if (orderMaster.IsPause)
                {
                    if (orderMaster.Type == CodeMaster.OrderType.Production)
                    {
                        throw new BusinessException("生产单{0}已经暂停，不能收货。", orderMaster.OrderNo);
                    }
                    else if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                    {
                        throw new BusinessException("KIT单{0}已经暂停，不能收货。", orderMaster.OrderNo);
                    }
                    else if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
                    {
                        throw new BusinessException("排序单{0}已经暂停，不能收货。", orderMaster.OrderNo);
                    }
                    else
                    {
                        throw new BusinessException("订单{0}已经暂停，不能收货。", orderMaster.OrderNo);
                    }
                }

                //if ((orderMaster.Type == CodeMaster.OrderType.Production
                //    || orderMaster.Type == CodeMaster.OrderType.SubContract
                //    //生产线暂停可以收货
                //    //|| orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT
                //    //|| orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ
                //    )
                //    && !string.IsNullOrWhiteSpace(orderMaster.Flow))
                //{
                //    FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(orderMaster.Flow);
                //    if (flowMaster.IsPause)
                //    {
                //        throw new BusinessException("生产线{0}已经暂停，不能收货。", orderMaster.Flow);
                //    }
                //}
                #endregion

                #region 整包装收货判断,快速的不要判断
                if (orderMaster.IsReceiveFulfillUC
                    && orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Normal
                    && !(orderMaster.IsAutoRelease && orderMaster.IsAutoStart))
                {
                    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                    {
                        //不合格品不作为收货数
                        //if (orderDetail.ReceiveQualifiedQtyInput % orderDetail.UnitCount != 0)
                        if (orderDetail.ReceiveQtyInput % orderDetail.UnitCount != 0)
                        {
                            //不是整包装
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ReceiveQtyNotFulfillUnitCount, orderDetail.Item);
                        }
                    }
                }
                #endregion

                #region 是否过量发货判断，未发货即收货也要判断是否过量发货
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (!orderMaster.IsOpenOrder
                        && orderMaster.Type != com.Sconit.CodeMaster.OrderType.Production   //生产和委外不需要判断
                        && orderMaster.Type != com.Sconit.CodeMaster.OrderType.SubContract)
                    {
                        if (Math.Abs(orderDetail.ShippedQty) > Math.Abs(orderDetail.OrderedQty))
                        {
                            //订单的发货数已经大于等于订单数
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipQtyExcceedOrderQty, orderDetail.OrderNo, orderDetail.Item);
                        }
                        else if (!orderMaster.IsShipExceed
                            && Math.Abs(orderDetail.ShippedQty + orderDetail.ReceiveQtyInput) > Math.Abs(orderDetail.OrderedQty))   //不允许过量收货
                        {
                            //订单的发货数 + 本次发货数大于订单数
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ShipQtyExcceedOrderQty, orderDetail.OrderNo, orderDetail.Item);
                        }
                    }
                }
                #endregion

                #region 是否过量收货判断
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (!orderMaster.IsOpenOrder)
                    {
                        var orderDeviationQty = decimal.MaxValue;
                        if (!orderMaster.IsReceiveExceed)
                        {
                            orderDeviationQty = Math.Abs(orderDetail.OrderedQty);
                        }
                        else
                        {
                            if (orderMaster.CurrentFlowMaster.OrderDeviation > 0)
                            {
                                orderDeviationQty = Math.Abs(orderDetail.OrderedQty * (((decimal)orderMaster.CurrentFlowMaster.OrderDeviation / 100) + 1));
                            }
                        }
                        //订单的收货数已经大于等于订单数
                        if (Math.Abs(orderDetail.ReceivedQty) >= Math.Abs(orderDetail.OrderedQty))
                        {
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ReceiveQtyExcceedOrderQty, orderDetail.OrderNo, orderDetail.Item);
                        }
                        else if (Math.Abs(orderDetail.ReceivedQty + orderDetail.RejectedQty + orderDetail.ScrapQty + orderDetail.ReceiveQtyInput + orderDetail.ScrapQtyInput) > orderDeviationQty)   //不允许过量收货
                        {
                            //订单的收货数 + 本次收货数大于订单数
                            throw new BusinessException(Resources.ORD.OrderMaster.Errors_ReceiveQtyExcceedOrderQty, orderDetail.OrderNo, orderDetail.Item);
                        }
                    }
                }
                #endregion

                #region KIT单收货判断
                if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                {
                    CheckKitOrderDetail(orderMaster, isCheckKitTraceItem, false);
                }
                #endregion
            }
            #endregion

            #region 校验发货先进先出
            CheckShipFiFo(orderMasterList);
            #endregion

            #region 循环更新订单明细
            foreach (OrderDetail orderDetail in nonZeroOrderDetailList)
            {
                //未发货直接收货需要累加已发货数量
                if (orderDetail.OrderType != com.Sconit.CodeMaster.OrderType.Production
                    && orderDetail.OrderType != com.Sconit.CodeMaster.OrderType.SubContract)
                {
                    orderDetail.ShippedQty += orderDetail.ReceiveQtyInput;
                }

                if (orderDetail.OrderType != com.Sconit.CodeMaster.OrderType.Production
                    && orderDetail.OrderType != com.Sconit.CodeMaster.OrderType.SubContract)
                {
                    //orderDetail.ReceivedQty += orderDetail.ReceiveQualifiedQtyInput;
                    orderDetail.ReceivedQty += orderDetail.ReceiveQtyInput;
                }
                else
                {
                    //生产收货更新合格数和不合格数量
                    //orderDetail.ReceivedQty += orderDetail.ReceiveQualifiedQtyInput;
                    orderDetail.ReceivedQty += orderDetail.ReceiveQtyInput;
                    orderDetail.ScrapQty += orderDetail.ScrapQtyInput;
                }
                genericMgr.Update(orderDetail);
            }
            #endregion

            #region 发货
            #region OrderDetailInput赋发货数量
            foreach (OrderDetail orderDetail in nonZeroOrderDetailList)
            {
                foreach (OrderDetailInput orderDetailInput in orderDetail.OrderDetailInputs)
                {
                    orderDetailInput.ShipQty = orderDetailInput.ReceiveQty;
                }
            }
            #endregion

            IpMaster ipMaster = null;
            var subType = orderMasterList.First().SubType;
            if ((orderType == com.Sconit.CodeMaster.OrderType.Production || orderType == com.Sconit.CodeMaster.OrderType.SubContract)
                && subType != CodeMaster.OrderSubType.Return)
            {
                //生产和委外的非退货单 
            }
            else
            {
                //物流单 生产和委外的退货单也要创建Ip
                ipMaster = this.ipMgr.TransferOrder2Ip(orderMasterList);
                #region 循环发货
                foreach (IpDetail ipDetail in ipMaster.IpDetails)
                {
                    ipDetail.CurrentPartyFrom = ipMaster.PartyFrom;  //为了记录库存事务
                    ipDetail.CurrentPartyFromName = ipMaster.PartyFromName;  //为了记录库存事务
                    ipDetail.CurrentPartyTo = ipMaster.PartyTo;      //为了记录库存事务
                    ipDetail.CurrentPartyToName = ipMaster.PartyToName;      //为了记录库存事务
                    //ipDetail.CurrentOccupyType = com.Sconit.CodeMaster.OccupyType.None; //todo-默认出库未占用库存，除非捡货或检验的出库

                    IList<InventoryTransaction> inventoryTransactionList = this.locationDetailMgr.InventoryOut(ipDetail, effectiveDate);

                    #region 建立发货库明细和IpDetailInput的关系
                    if (inventoryTransactionList != null && inventoryTransactionList.Count > 0)
                    {
                        ipDetail.IpLocationDetails = (from trans in inventoryTransactionList
                                                      group trans by new
                                                      {
                                                          HuId = trans.HuId,
                                                          LotNo = trans.LotNo,
                                                          IsCreatePlanBill = trans.IsCreatePlanBill,
                                                          IsConsignment = trans.IsConsignment,
                                                          PlanBill = trans.PlanBill,
                                                          ActingBill = trans.ActingBill,
                                                          QualityType = trans.QualityType,
                                                          IsFreeze = trans.IsFreeze,
                                                          IsATP = trans.IsATP,
                                                          OccupyType = trans.OccupyType,
                                                          OccupyReferenceNo = trans.OccupyReferenceNo
                                                      } into g
                                                      select new IpLocationDetail
                                                      {
                                                          HuId = g.Key.HuId,
                                                          LotNo = g.Key.LotNo,
                                                          IsCreatePlanBill = g.Key.IsCreatePlanBill,
                                                          IsConsignment = g.Key.IsConsignment,
                                                          PlanBill = g.Key.PlanBill,
                                                          ActingBill = g.Key.ActingBill,
                                                          QualityType = g.Key.QualityType,
                                                          IsFreeze = g.Key.IsFreeze,
                                                          IsATP = g.Key.IsATP,
                                                          OccupyType = g.Key.OccupyType,
                                                          OccupyReferenceNo = g.Key.OccupyReferenceNo,
                                                          Qty = g.Sum(t => -t.Qty),                      //发货的库存事务为负数，转为收货数应该取负数
                                                          //PlanBillQty = g.Sum(t=>-t.PlanBillQty),
                                                          //ActingBillQty = g.Sum(t=>-t.ActingBillQty)
                                                      }).ToList();
                    }
                    #endregion
                }
                #endregion

                #region 生成收货的IpInput
                foreach (OrderDetail orderDetail in nonZeroOrderDetailList)
                {
                    OrderMaster orderMaster = orderMasterList.Where(o => o.OrderNo == orderDetail.OrderNo).Single();
                    //订单收货一定是一条订单明细对应一条发货单明细
                    IpDetail ipDetail = ipMaster.IpDetails.Where(det => det.OrderDetailId == orderDetail.Id).Single();
                    ipDetail.IpDetailInputs = null;  //清空Ip的发货数据，准备添加收货数据

                    foreach (OrderDetailInput orderDetailInput in orderDetail.OrderDetailInputs)
                    {
                        IpDetailInput ipDetailInput = new IpDetailInput();
                        ipDetailInput.ReceiveQty = orderDetailInput.ShipQty;
                        if (orderMaster.IsReceiveScanHu || orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                        {
                            ipDetailInput.HuId = orderDetailInput.HuId;
                            ipDetailInput.LotNo = orderDetailInput.LotNo;
                        }

                        ipDetail.AddIpDetailInput(ipDetailInput);
                    }
                }

                if (orderMasterList.Where(o => o.OrderStrategy == CodeMaster.FlowStrategy.KIT).Count() > 0)
                {
                    #region Kit单生成收货Input
                    foreach (IpDetail ipDetail in ipMaster.IpDetails)
                    {
                        foreach (IpDetailInput ipDetailInput in ipDetail.IpDetailInputs)
                        {
                            if (!string.IsNullOrEmpty(ipDetailInput.HuId))
                            {
                                #region 按条码匹配
                                IpLocationDetail matchedIpLocationDetail = ipDetail.IpLocationDetails.Where(locDet => locDet.HuId == ipDetailInput.HuId).SingleOrDefault();
                                matchedIpLocationDetail.ReceivedQty = matchedIpLocationDetail.Qty;
                                if (matchedIpLocationDetail != null)
                                {
                                    ipDetailInput.AddReceivedIpLocationDetail(matchedIpLocationDetail);
                                }
                                #endregion
                            }
                            else
                            {
                                #region 按数量匹配
                                IpDetail gapIpDetail = new IpDetail();
                                MatchIpDetailInput(ipDetail, ipDetailInput, ipDetail.IpLocationDetails, ref gapIpDetail);
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 其它订单生成收货Input
                    if (ipMaster.IsShipScanHu && ipMaster.IsReceiveScanHu)
                    {
                        #region 按条码匹配
                        foreach (IpDetail ipDetail in ipMaster.IpDetails)
                        {
                            foreach (IpDetailInput ipDetailInput in ipDetail.IpDetailInputs)
                            {
                                IpLocationDetail matchedIpLocationDetail = ipDetail.IpLocationDetails.Where(locDet => locDet.HuId == ipDetailInput.HuId).SingleOrDefault();
                                matchedIpLocationDetail.ReceivedQty = matchedIpLocationDetail.Qty;
                                if (matchedIpLocationDetail != null)
                                {
                                    ipDetailInput.AddReceivedIpLocationDetail(matchedIpLocationDetail);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 按数量匹配
                        foreach (IpDetail ipDetail in ipMaster.IpDetails)
                        {
                            CycleMatchIpDetailInput(ipDetail, ipDetail.IpDetailInputs, ipDetail.IpLocationDetails);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            #region 收货 生产和委外退货的逻辑在CreateReceipt中实现
            ReceiptMaster receiptMaster = null;
            if (subType != CodeMaster.OrderSubType.Return &&
               (orderType == com.Sconit.CodeMaster.OrderType.Production || orderType == com.Sconit.CodeMaster.OrderType.SubContract))
            {
                #region 生产和委外正常收货 消耗原材料,收成品
                if (orderMasterList.Count > 1 && orderType == com.Sconit.CodeMaster.OrderType.Production)
                {
                    throw new TechnicalException("生产单不能合并收货。");
                }

                foreach (OrderMaster orderMaster in orderMasterList)
                {
                    orderMaster.IpNo = ipNo;
                    receiptMaster = this.receiptMgr.TransferOrder2Receipt(orderMaster);
                    this.receiptMgr.CreateReceipt(receiptMaster);

                    #region 回冲物料
                    if (productLineMap != null)
                    {
                        #region 整车
                        BackflushVan(productLineMap, nonZeroOrderDetailList, receiptMaster, orderMaster);
                        #endregion
                    }
                    else
                    {
                        #region 非整车
                        foreach (OrderDetail orderDetail in nonZeroOrderDetailList)
                        {
                            //OrderDetailInput orderDetailInput = orderDetail.OrderDetailInputs[0];
                            foreach (var orderDetailInput in orderDetail.OrderDetailInputs)
                            {
                                orderDetailInput.ReceiptDetails = receiptMaster.ReceiptDetails.Where(r => r.OrderDetailId == orderDetail.Id).ToList();
                            }
                        }
                        this.productionLineMgr.BackflushProductOrder(nonZeroOrderDetailList, orderMaster, DateTime.Now);
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                #region 物流收货
                receiptMaster = this.receiptMgr.TransferIp2Receipt(ipMaster);
                if (orderMasterList.Where(o => o.OrderStrategy == CodeMaster.FlowStrategy.KIT).Count() > 0)
                {
                    //kit收货特殊处理
                    this.receiptMgr.CreateReceipt(receiptMaster, true, effectiveDate);
                }
                else
                {
                    this.receiptMgr.CreateReceipt(receiptMaster, false, effectiveDate);
                }
                #endregion
            }
            #endregion

            #region 尝试关闭订单
            foreach (OrderMaster orderMaster in orderMasterList)
            {
                if (orderMaster.Type == CodeMaster.OrderType.Production)
                {
                    #region 生产
                    if (productLineMap != null && productLineMap.ProductLine == orderMaster.Flow)
                    {
                        #region 总装生产单
                        CloseVanOrder(orderMaster, productLineMap);
                        #endregion
                    }
                    else if (productLineMap != null && (productLineMap.CabFlow == orderMaster.Flow || productLineMap.ChassisFlow == orderMaster.Flow))
                    {
                        #region  驾驶室和底盘
                        //驾驶室和底盘下线完工订单，总装下线后一起关闭
                        CompleteVanSubOrder(orderMaster);
                        #endregion
                    }
                    else
                    {
                        #region  非整车
                        TryCloseOrder(orderMaster);
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 物流
                    TryCloseOrder(orderMaster);
                    #endregion
                }
            }
            #endregion

            return receiptMaster;
        }

        private void BackflushVan(ProductLineMap productLineMap, IList<OrderDetail> nonZeroOrderDetailList, ReceiptMaster receiptMaster, OrderMaster orderMaster)
        {
            if (productLineMap.ProductLine == orderMaster.Flow)
            {
                #region 总装，回冲总装、驾驶室、底盘的物料
                #region 回冲驾驶室和底盘
                IList<string> subOrderMasterList = this.genericMgr.FindAll<string>("select OrderNo from OrderMaster where Type = ? and Flow in (?,?)", new object[] { CodeMaster.OrderType.Production, productLineMap.CabFlow, productLineMap.ChassisFlow, CodeMaster.OrderStatus.Close });

                if (subOrderMasterList != null && subOrderMasterList.Count > 0)
                {
                    string selectOrderDetHql = string.Empty;
                    string selectReceiptDetHql = string.Empty;
                    IList<object> parms = new List<object>();
                    foreach (string subOrderNo in subOrderMasterList)
                    {
                        if (selectOrderDetHql == string.Empty)
                        {
                            selectOrderDetHql = "from OrderDetail where OrderNo in (?";
                            selectReceiptDetHql = "from ReceiptDetail where OrderNo in (?";
                        }
                        else
                        {
                            selectOrderDetHql += ",?";
                            selectReceiptDetHql += ",?";
                        }

                        parms.Add(subOrderNo);
                    }
                    selectOrderDetHql += ")";
                    selectReceiptDetHql += ")";

                    IList<OrderDetail> subOrderDetailList = this.genericMgr.FindAll<OrderDetail>(selectOrderDetHql, parms.ToArray());
                    IList<ReceiptDetail> subReceiptDetailList = this.genericMgr.FindAll<ReceiptDetail>(selectReceiptDetHql, parms.ToArray());

                    foreach (OrderDetail subOrderDetail in subOrderDetailList)
                    {
                        OrderDetailInput subOrderDetailInput = new OrderDetailInput();
                        subOrderDetailInput.ReceiveQty = subOrderDetail.OrderedQty;
                        subOrderDetail.AddOrderDetailInput(subOrderDetailInput);
                        subOrderDetailInput.ReceiptDetails = subReceiptDetailList.Where(r => r.OrderDetailId == subOrderDetail.Id).ToList();

                        IList<OrderDetail> backFlushOrderDetailList = new List<OrderDetail>();
                        backFlushOrderDetailList.Add(subOrderDetail);
                        this.productionLineMgr.BackflushProductOrder(backFlushOrderDetailList, orderMaster, DateTime.Now);
                    }
                }
                #endregion

                #region 回冲总装
                foreach (OrderDetail orderDetail in nonZeroOrderDetailList)
                {
                    OrderDetailInput orderDetailInput = orderDetail.OrderDetailInputs[0];
                    orderDetailInput.ReceiptDetails = receiptMaster.ReceiptDetails.Where(r => r.OrderDetailId == orderDetail.Id).ToList();
                }
                this.productionLineMgr.BackflushProductOrder(nonZeroOrderDetailList, orderMaster, DateTime.Now);
                #endregion
                #endregion
            }
            else
            {
                #region 驾驶室、底盘，不回冲物料
                #endregion
            }
        }

        private long GetReleasedOrStartedOrderCount(OrderMaster orderMaster)
        {
            string hql = "select count(*) as counter from OrderMaster where Type = ? and Flow = ? and Status in (?, ?) and Sequence <= ? and IsPause = ?";
            return this.genericMgr.FindAll<long>(hql, new object[] { orderMaster.Type, orderMaster.Flow, CodeMaster.OrderStatus.Submit, CodeMaster.OrderStatus.InProcess, orderMaster.Sequence, false })[0];
        }
        #endregion

        #region 送货单收货
        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster ReceiveIp(IList<IpDetail> ipDetailList)
        {
            return ReceiveIp(ipDetailList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster ReceiveIp(IList<IpDetail> ipDetailList, DateTime effectiveDate)
        {
            return ReceiveIp(ipDetailList, true, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster ReceiveIp(IList<IpDetail> ipDetailList, bool isCheckKitTraceItem, DateTime effectiveDate)
        {
            #region 判断送货单是否合并收货
            if ((from det in ipDetailList select det.IpNo).Distinct().Count() > 1)
            {
                throw new TechnicalException("送货单不能合并收货。");
            }
            #endregion

            #region 判断是否全0发货
            if (ipDetailList == null || ipDetailList.Count == 0)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_ReceiveDetailIsEmpty);
            }

            IList<IpDetail> nonZeroIpDetailList = ipDetailList.Where(o => o.ReceiveQtyInput != 0).ToList();

            if (nonZeroIpDetailList.Count == 0)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_ReceiveDetailIsEmpty);
            }
            #endregion

            #region 查询送货单头对象
            string ipNo = (from det in nonZeroIpDetailList select det.IpNo).Distinct().Single();
            IpMaster ipMaster = this.genericMgr.FindById<IpMaster>(ipNo);
            ipMaster.IpDetails = nonZeroIpDetailList;
            #endregion

            #region 查询订单头对象
            IList<OrderMaster> orderMasterList = LoadOrderMasters((from det in nonZeroIpDetailList
                                                                   select det.OrderNo).Distinct().ToArray());

            //foreach (var orderMaster in orderMasterList)
            //{
            //    if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //    {
            //        throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //    }
            //}

            #endregion

            #region 获取收货订单类型
            IList<com.Sconit.CodeMaster.OrderType> orderTypeList = (from orderMaster in orderMasterList
                                                                    group orderMaster by orderMaster.Type into result
                                                                    select result.Key).ToList();

            if (orderTypeList.Count > 1)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_CannotMixOrderTypeReceive);
            }

            com.Sconit.CodeMaster.OrderType orderType = orderTypeList.First();
            #endregion

            #region 排序单/KIT收货顺序判断
            //没有考虑多个路线的排序单/Kit单同时发货
            //var groupedSeqOrderMaster = from mstr in orderMasterList
            //                            where !string.IsNullOrWhiteSpace(mstr.Flow)
            //                                    && (mstr.OrderStrategy == CodeMaster.FlowStrategy.KIT
            //                                    || mstr.OrderStrategy == CodeMaster.FlowStrategy.SEQ)
            //                            group mstr by new { Flow = mstr.Flow, OrderStrategy = mstr.OrderStrategy } into result
            //                            select new
            //                            {
            //                                Flow = result.Key.Flow,
            //                                OrderStrategy = result.Key.OrderStrategy,
            //                                List = result.ToList()
            //                            };

            //if (groupedSeqOrderMaster != null && groupedSeqOrderMaster.Count() > 0)
            //{
            //    BusinessException businessException = new BusinessException();
            //    foreach (var seqOrderMaster in groupedSeqOrderMaster)
            //    {
            //        OrderMaster biggestSeqOrderMaster = seqOrderMaster.List.OrderBy(o => o.Sequence).LastOrDefault();

            //        if (this.GetSpecifiedStatusOrderCount(biggestSeqOrderMaster, CodeMaster.OrderStatus.InProcess)           //小与等于最大序号的待收货排序单/Kit单的总数，状态为InProcess
            //            > seqOrderMaster.List.Count())                                                                       //当前待收货排序单/Kit单的总数
            //        {
            //            if (biggestSeqOrderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
            //            {
            //                businessException.AddMessage("KIT路线{0}没有按顺序进行收货。", biggestSeqOrderMaster.Flow);
            //            }
            //            else
            //            {
            //                businessException.AddMessage("排序路线{0}没有按顺序进行收货。", biggestSeqOrderMaster.Flow);
            //            }
            //        }
            //    }

            //    if (businessException.HasMessage)
            //    {
            //        throw businessException;
            //    }
            //}
            #endregion

            #region 查询订单明细对象
            IList<OrderDetail> orderDetailList = LoadOrderDetails((from det in nonZeroIpDetailList
                                                                   where det.OrderDetailId.HasValue
                                                                   select det.OrderDetailId.Value).Distinct().ToArray());
            #endregion

            #region 查询送货单库存对象
            IList<IpLocationDetail> ipLocationDetailList = LoadIpLocationDetails((from det in nonZeroIpDetailList
                                                                                  select det.Id).ToArray());
            #endregion

            //FlowMaster flowMaster = null;
            //if (ipMaster.OrderType == CodeMaster.OrderType.Procurement)
            //{
            //    flowMaster = this.genericMgr.FindById<FlowMaster>(ipMaster.Flow);
            //}

            #region 循环检查发货明细
            foreach (IpDetail ipDetail in nonZeroIpDetailList)
            {
                #region 整包装收货判断
                if (ipMaster.IsReceiveFulfillUC)
                {
                    //不合格品不作为收货数
                    if (ipDetail.ReceiveQtyInput % ipDetail.UnitCount != 0)
                    {
                        //不是整包装
                        throw new BusinessException(Resources.ORD.OrderMaster.Errors_ReceiveQtyNotFulfillUnitCount, ipDetail.Item, ipDetail.UnitCount.ToString());
                    }
                }
                #endregion

                #region 是否过量收货判断
                if (orderType != CodeMaster.OrderType.ScheduleLine)
                {
                    if (Math.Abs(ipDetail.ReceivedQty) >= Math.Abs(ipDetail.Qty))
                    {
                        //送货单的收货数已经大于等于发货数
                        throw new BusinessException(Resources.ORD.IpMaster.Errors_ReceiveQtyExcceedOrderQty, ipMaster.IpNo, ipDetail.Item);
                    }
                    else if (!ipMaster.IsReceiveExceed && Math.Abs(ipDetail.ReceivedQty + ipDetail.ReceiveQtyInput) > Math.Abs(ipDetail.Qty))
                    {
                        //送货单的收货数 + 本次收货数大于发货数
                        throw new BusinessException(Resources.ORD.IpMaster.Errors_ReceiveQtyExcceedOrderQty, ipMaster.IpNo, ipDetail.Item);
                    }
                }
                #endregion

                #region 发货明细是否已经关闭
                if (ipDetail.IsClose)
                {
                    throw new BusinessException("送货单{0}零件{1}已经关闭，不能进行收货。", ipMaster.IpNo, ipDetail.Item);
                }
                #endregion

                #region 采购收货是否有价格单判断
                //if (ipMaster.OrderType == CodeMaster.OrderType.Procurement)
                //{
                //    if (!flowMaster.IsAllowProvEstRec)
                //    {
                //        if (ipDetail.IsProvisionalEstimate || ipDetail.UnitPrice == 0M)
                //        {
                //            throw new BusinessException("此采购路线{0}不允许暂估价收货", ipMaster.Flow);
                //        }
                //    }
                //}

                //if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT
                //    && !bool.Parse(entityPreference.Value))
                //{
                //    if (orderDetail.UnitPrice == Decimal.Zero)
                //    {
                //        //重新查找一次价格
                //        PriceListDetail priceListDetail = priceListDetailMgrE.GetLastestPriceListDetail(
                //            orderDetail.DefaultPriceList,
                //            orderDetail.Item,
                //            orderHead.StartTime,
                //            orderHead.Currency,
                //            orderDetail.Uom);

                //        if (priceListDetail != null)
                //        {
                //            orderDetail.UnitPrice = priceListDetail.UnitPrice;
                //            orderDetail.IsProvisionalEstimate = priceListDetail.IsProvisionalEstimate;
                //            orderDetail.IsIncludeTax = priceListDetail.IsIncludeTax;
                //            orderDetail.TaxCode = priceListDetail.TaxCode;
                //        }
                //        else
                //        {
                //            throw new BusinessErrorException("Order.Error.NoPriceListReceipt", orderDetail.Item.Code);
                //        }
                //    }
                //}
                #endregion
            }
            #endregion

            #region KIT单收货判断
            //if (ipMaster.Type == CodeMaster.IpType.KIT)
            //{
            //    string anjiRegion = systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.WMSAnjiRegion);
            //    if (ipMaster.PartyFrom != anjiRegion)
            //    {
            //        CheckKitIpDetail(ipMaster, isCheckKitTraceItem);
            //    }
            //}
            #endregion

            #region 关闭排序装箱单
            if (ipMaster.Type == CodeMaster.IpType.SEQ &&
                !string.IsNullOrWhiteSpace(ipMaster.SequenceNo))
            {
                SequenceMaster sequenceMaster = this.genericMgr.FindById<SequenceMaster>(ipMaster.SequenceNo);
                sequenceMaster.Status = CodeMaster.SequenceStatus.Close;
                sequenceMaster.CloseDate = DateTime.Now;
                sequenceMaster.CloseUserId = SecurityContextHolder.Get().Id;
                sequenceMaster.CloseUserName = SecurityContextHolder.Get().FullName;

                this.genericMgr.Update(sequenceMaster);

                foreach (SequenceDetail sequenceDetail in TryLoadSequenceDetails(sequenceMaster).Where(s => !s.IsClose))
                {
                    sequenceDetail.IsClose = true;
                    this.genericMgr.Update(sequenceDetail);
                }
            }
            #endregion

            #region 循环更新订单明细
            if (orderType == CodeMaster.OrderType.Production || orderType == CodeMaster.OrderType.SubContract)
            {
                //nothing todo
            }
            else if (orderType != CodeMaster.OrderType.ScheduleLine)
            {
                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    IList<IpDetail> targetIpDetailList = (from det in nonZeroIpDetailList
                                                          where det.OrderDetailId == orderDetail.Id
                                                          select det).ToList();

                    //更新订单的收货数
                    orderDetail.ReceivedQty += targetIpDetailList.Sum(det => det.ReceiveQtyInput);
                    genericMgr.Update(orderDetail);
                }
            }
            else
            {
                foreach (IpDetail ipDetail in nonZeroIpDetailList)
                {
                    decimal remainReceiveQty = ipDetail.ReceiveQtyInput;

                    IList<OrderDetail> scheduleOrderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>("select * from ORD_OrderDet_8 where ExtNo = ? and ExtSeq like ? and ScheduleType = ? and ShipQty > RecQty order by EndDate",
                                                new object[] { ipDetail.ExternalOrderNo, ipDetail.ExternalSequence + "-%", CodeMaster.ScheduleType.Firm });

                    if (scheduleOrderDetailList != null && scheduleOrderDetailList.Count > 0)
                    {
                        foreach (OrderDetail scheduleOrderDetail in scheduleOrderDetailList)
                        {

                            if (remainReceiveQty > (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty))
                            {
                                remainReceiveQty -= (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty);
                                scheduleOrderDetail.ReceivedQty = scheduleOrderDetail.ShippedQty;
                            }
                            else
                            {
                                scheduleOrderDetail.ReceivedQty += remainReceiveQty;
                                remainReceiveQty = 0;
                                break;
                            }

                            this.genericMgr.Update(scheduleOrderDetail);
                        }
                    }

                    if (remainReceiveQty > 0)
                    {
                        throw new BusinessException(Resources.ORD.IpMaster.Errors_ReceiveQtyExcceedOrderQty, ipMaster.IpNo, ipDetail.Item);
                    }
                }
            }
            #endregion

            #region 循环IpDetail，处理超收差异
            IList<IpDetail> gapIpDetailList = new List<IpDetail>();
            foreach (IpDetail targetIpDetail in nonZeroIpDetailList)
            {
                #region 收货输入和送货单库存明细匹配
                IList<IpLocationDetail> targetIpLocationDetailList = (from ipLocDet in ipLocationDetailList
                                                                      where ipLocDet.IpDetailId == targetIpDetail.Id
                                                                      select ipLocDet).OrderByDescending(d => d.IsConsignment).ToList();  //排序为了先匹配寄售的

                bool isContainHu = targetIpLocationDetailList.Where(ipLocDet => !string.IsNullOrWhiteSpace(ipLocDet.HuId)).Count() > 0;

                if (ipMaster.IsReceiveScanHu)
                {
                    #region 收货扫描条码
                    if (isContainHu)
                    {
                        #region 条码匹配条码

                        #region 匹配到的条码
                        IList<IpLocationDetail> matchedIpLocationDetailList = new List<IpLocationDetail>();
                        foreach (IpDetailInput ipDetailInput in targetIpDetail.IpDetailInputs)
                        {
                            IpLocationDetail matchedIpLocationDetail = targetIpLocationDetailList.Where(locDet => locDet.HuId == ipDetailInput.HuId).SingleOrDefault();

                            if (matchedIpLocationDetail != null)
                            {
                                ipDetailInput.AddReceivedIpLocationDetail(matchedIpLocationDetail);
                                matchedIpLocationDetailList.Add(matchedIpLocationDetail);

                                #region 更新库存状态
                                matchedIpLocationDetail.ReceivedQty = matchedIpLocationDetail.Qty;
                                matchedIpLocationDetail.IsClose = true;

                                genericMgr.Update(matchedIpLocationDetail);
                                #endregion
                            }
                        }
                        #endregion

                        #region 未匹配到的条码，记录差异
                        var matchedHuIdList = matchedIpLocationDetailList.Select(locDet => locDet.HuId);
                        var gapIpDetailInputList = targetIpDetail.IpDetailInputs.Where(input => !matchedHuIdList.Contains(input.HuId));

                        #region 记录差异
                        if (gapIpDetailInputList != null && gapIpDetailInputList.Count() > 0)
                        {
                            IpDetail gapIpDetail = Mapper.Map<IpDetail, IpDetail>(targetIpDetail);
                            gapIpDetail.Type = com.Sconit.CodeMaster.IpDetailType.Gap;
                            gapIpDetail.GapReceiptNo = string.Empty;                            //todo 记录产生差异的收货单号
                            //gapIpDetail.Qty = gapIpDetailInputList.Sum(gap => -gap.ReceiveQty / targetIpDetail.UnitQty); //多收的条码，数量为负，转为订单单位
                            gapIpDetail.Qty = gapIpDetailInputList.Sum(gap => -gap.ReceiveQty); //多收的条码，数量为负
                            gapIpDetail.ReceivedQty = 0;
                            gapIpDetail.IsClose = false;
                            gapIpDetail.GapIpDetailId = targetIpDetail.Id;

                            gapIpDetail.IpLocationDetails = (from gap in gapIpDetailInputList
                                                             select new IpLocationDetail
                                                             {
                                                                 IpNo = ipMaster.IpNo,
                                                                 OrderType = targetIpDetail.OrderType,
                                                                 OrderDetailId = targetIpDetail.OrderDetailId,
                                                                 Item = targetIpDetail.Item,
                                                                 //HuId = gap.HuId,      //多收的条码按数量记录差异
                                                                 //LotNo = gap.LotNo,
                                                                 IsCreatePlanBill = false,
                                                                 PlanBill = null,
                                                                 ActingBill = null,
                                                                 IsFreeze = false,
                                                                 IsATP = false,          //差异不能进行MRP运算
                                                                 QualityType = targetIpDetail.QualityType,
                                                                 OccupyType = com.Sconit.CodeMaster.OccupyType.None,
                                                                 OccupyReferenceNo = null,
                                                                 Qty = -gap.ReceiveQty * gapIpDetail.UnitQty,    //多收，产生负数的差异
                                                                 ReceivedQty = 0,
                                                                 IsClose = false
                                                             }).ToList();

                            gapIpDetailList.Add(gapIpDetail);
                        }
                        #endregion
                        #endregion
                        #endregion
                    }
                    else
                    {
                        #region 数量匹配条码
                        IpDetail gapIpDetail = CycleMatchIpDetailInput(targetIpDetail, targetIpDetail.IpDetailInputs, targetIpLocationDetailList);
                        if (gapIpDetail != null)
                        {
                            gapIpDetailList.Add(gapIpDetail);
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 收货不扫描条码
                    if (isContainHu)
                    {
                        #region 条码匹配数量
                        IpDetail gapIpDetail = CycleMatchIpDetailInput(targetIpDetail, targetIpDetail.IpDetailInputs, targetIpLocationDetailList);
                        if (gapIpDetail != null)
                        {
                            gapIpDetailList.Add(gapIpDetail);
                        }
                        #endregion
                    }
                    else
                    {
                        //采购数量超收，用收货输入数补发货数
                        if (ipMaster.OrderType == CodeMaster.OrderType.Procurement)
                        {
                            var remainSum = targetIpLocationDetailList.Sum(t => t.RemainReceiveQty);
                            var receiveSum = targetIpDetail.IpDetailInputs.Sum(t => t.ReceiveQty);
                            if (remainSum < receiveSum)
                            {
                                var firstIpLocationDetail = targetIpLocationDetailList.FirstOrDefault();
                                firstIpLocationDetail.Qty = firstIpLocationDetail.Qty + (receiveSum - remainSum);
                                this.genericMgr.Update(firstIpLocationDetail);
                            }
                        }
                        #region 数量匹配数量
                        IpDetail gapIpDetail = CycleMatchIpDetailInput(targetIpDetail, targetIpDetail.IpDetailInputs, targetIpLocationDetailList);
                        if (gapIpDetail != null)
                        {
                            gapIpDetailList.Add(gapIpDetail);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region 更新IpDetail上的收货数
                targetIpDetail.ReceivedQty += targetIpDetail.ReceiveQtyInput;
                if (targetIpLocationDetailList.Where(i => !i.IsClose).Count() == 0)
                {
                    //只有所有的IpLocationDetail关闭才能关闭
                    targetIpDetail.IsClose = true;
                }
                genericMgr.Update(targetIpDetail);
                #endregion
            }
            #endregion

            #region 送货单一次性收货，未收货差异
            if (ipMaster.IsAsnUniqueReceive)
            {
                #region 查找未关闭送货单明细
                List<IpDetail> openIpDetailList = (from det in nonZeroIpDetailList where !det.IsClose select det).ToList();

                #region 查询剩余的未关闭送货单明细
                IList<IpDetail> exceptIpDetailList = LoadExceptIpDetails(ipMaster.IpNo, (nonZeroIpDetailList.Select(det => det.Id)).ToArray());
                #endregion

                #region 合并未关闭送货单明细
                if (exceptIpDetailList != null && exceptIpDetailList.Count > 0)
                {
                    openIpDetailList.AddRange(exceptIpDetailList);
                }
                #endregion
                #endregion

                #region 查找未关闭送货单库存对象
                List<IpLocationDetail> openIpLocationDetailList = (from det in ipLocationDetailList where !det.IsClose select det).ToList();

                #region 查询剩余未关闭送货单库存对象
                IList<IpLocationDetail> expectIpLocationDetailList = LoadExceptIpLocationDetails(ipMaster.IpNo, (nonZeroIpDetailList.Select(det => det.Id)).ToArray());
                #endregion

                #region 合并未关闭送货单库存对象
                if (expectIpLocationDetailList != null && expectIpLocationDetailList.Count > 0)
                {
                    openIpLocationDetailList.AddRange(expectIpLocationDetailList);
                }
                #endregion
                #endregion

                #region 生成未收货差异
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
                                                             IsConsignment = locDet.IsConsignment,
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

                #region 关闭未收货送货单明细和库存明细
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
            }
            else
            {
                ipMaster.Status = CodeMaster.IpStatus.InProcess;
            }
            #endregion

            #region 收货

            ReceiptMaster receiptMaster = null;
            if (orderType == CodeMaster.OrderType.Production
                || orderType == CodeMaster.OrderType.SubContract)
            {
                foreach (var orderDetail in orderDetailList)
                {
                    var _ipDetail = nonZeroIpDetailList.First(i => i.OrderDetailId == orderDetail.Id);
                    foreach (var ipDetailInput in _ipDetail.IpDetailInputs)
                    {
                        OrderDetailInput orderDetailInput = new OrderDetailInput();
                        orderDetailInput.Bin = ipDetailInput.Bin;
                        orderDetailInput.HuId = ipDetailInput.HuId;
                        orderDetailInput.LotNo = ipDetailInput.LotNo;
                        orderDetailInput.ManufactureParty = ipDetailInput.ManufactureParty;
                        orderDetailInput.OccupyReferenceNo = ipDetailInput.OccupyReferenceNo;
                        orderDetailInput.OccupyType = ipDetailInput.OccupyType;
                        orderDetailInput.ReceiveQty = ipDetailInput.ReceiveQty;
                        if (_ipDetail.Id != 0)
                        {
                            orderDetailInput.IpNo = _ipDetail.IpNo;
                            orderDetailInput.IpDetId = _ipDetail.Id;
                        }
                        orderDetail.AddOrderDetailInput(orderDetailInput);
                    }
                }
                receiptMaster = this.ReceiveOrder(orderDetailList, ipNo);
            }
            else
            {
                receiptMaster = this.receiptMgr.TransferIp2Receipt(ipMaster);
                this.receiptMgr.CreateReceipt(receiptMaster, effectiveDate);

                #region 尝试关闭订单
                foreach (OrderMaster orderMaster in orderMasterList)
                {
                    TryCloseOrder(orderMaster);
                }
                #endregion
            }
            #endregion

            #region 记录收货差异
            if (gapIpDetailList != null && gapIpDetailList.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(ipMaster.SequenceNo))
                {
                    throw new BusinessException("排序单装箱单{0}的收货数和发货数不一致。", ipMaster.SequenceNo);
                }

                foreach (IpDetail gapIpDetail in gapIpDetailList)
                {
                    gapIpDetail.GapReceiptNo = receiptMaster.ReceiptNo;
                    this.genericMgr.Create(gapIpDetail);

                    foreach (IpLocationDetail gapIpLocationDetail in gapIpDetail.IpLocationDetails)
                    {
                        gapIpLocationDetail.IpDetailId = gapIpDetail.Id;
                        this.genericMgr.Create(gapIpLocationDetail);
                    }
                }

                #region 调整发货方库存
                if (ipMaster.ReceiveGapTo == CodeMaster.ReceiveGapTo.AdjectLocFromInv)
                {
                    foreach (IpDetail gapIpDetail in gapIpDetailList)
                    {
                        gapIpDetail.IpDetailInputs = null;

                        foreach (IpLocationDetail gapIpLocationDetail in gapIpDetail.IpLocationDetails)
                        {
                            IpDetailInput input = new IpDetailInput();
                            input.ReceiveQty = gapIpLocationDetail.Qty / gapIpDetail.UnitQty; //转为订单单位
                            if (ipMaster.IsReceiveScanHu)
                            {
                                input.HuId = gapIpLocationDetail.HuId;
                                input.LotNo = gapIpLocationDetail.LotNo;
                            }
                            gapIpDetail.AddIpDetailInput(input);
                        }
                    }

                    this.AdjustIpGap(gapIpDetailList, CodeMaster.IpGapAdjustOption.GI);
                }
                #endregion
            }
            #endregion

            #region 更新送货单状态
            ipMaster.Status = CodeMaster.IpStatus.InProcess;
            this.genericMgr.Update(ipMaster);
            #endregion

            #region 尝试关闭送货单
            this.ipMgr.TryCloseIp(ipMaster);
            #endregion

            return receiptMaster;
        }

        private IpDetail CycleMatchIpDetailInput(IpDetail ipDetail, IList<IpDetailInput> ipDetailInputList, IList<IpLocationDetail> ipLocationDetailList)
        {
            IpDetail gapIpDetail = null;
            #region 循环匹配收货记录和送货单库存明细
            foreach (IpDetailInput ipDetailInput in ipDetailInputList)
            {
                MatchIpDetailInput(ipDetail, ipDetailInput, ipLocationDetailList, ref gapIpDetail);
            }
            #endregion

            return gapIpDetail;
        }

        private void MatchIpDetailInput(IpDetail ipDetail, IpDetailInput ipDetailInput, IList<IpLocationDetail> ipLocationDetailList, ref IpDetail gapIpDetail)
        {
            decimal remainQty = ipDetailInput.ReceiveQty * ipDetail.UnitQty;  //转为库存单位
            foreach (IpLocationDetail ipLocationDetail in ipLocationDetailList)
            {
                if (ipLocationDetail.IsClose)
                {
                    continue;
                }

                //iplocationdet只可能为正数
                if (ipLocationDetail.RemainReceiveQty >= remainQty)
                {
                    //收货明细匹配完
                    #region 添加收货记录和IpLocationDetail的映射关系
                    IpLocationDetail receivedIpLocationDetail = Mapper.Map<IpLocationDetail, IpLocationDetail>(ipLocationDetail);
                    receivedIpLocationDetail.ReceivedQty = remainQty;
                    ipDetailInput.AddReceivedIpLocationDetail(receivedIpLocationDetail);
                    #endregion

                    ipLocationDetail.ReceivedQty += remainQty;
                    remainQty = 0;
                    if (ipLocationDetail.Qty == ipLocationDetail.ReceivedQty)
                    {
                        ipLocationDetail.IsClose = true;
                    }
                }
                else
                {
                    //收货明细未匹配完
                    #region 添加收货记录和IpLocationDetail的映射关系
                    IpLocationDetail receivedIpLocationDetail = Mapper.Map<IpLocationDetail, IpLocationDetail>(ipLocationDetail);
                    receivedIpLocationDetail.ReceivedQty = ipLocationDetail.RemainReceiveQty;
                    ipDetailInput.AddReceivedIpLocationDetail(receivedIpLocationDetail);
                    #endregion

                    remainQty -= ipLocationDetail.RemainReceiveQty;
                    ipLocationDetail.ReceivedQty = ipLocationDetail.Qty;
                    ipLocationDetail.IsClose = true;
                }

                //更新
                if (ipLocationDetail.Id > 0)
                {
                    genericMgr.Update(ipLocationDetail);
                }

                if (remainQty == 0)
                {
                    //匹配完，跳出循环
                    break;
                }
            }

            //超收，还有未匹配完的数量
            if (remainQty > 0)
            {
                #region 记录差异
                if (gapIpDetail == null || !gapIpDetail.GapIpDetailId.HasValue)
                {
                    gapIpDetail = Mapper.Map<IpDetail, IpDetail>(ipDetail);
                    gapIpDetail.Type = com.Sconit.CodeMaster.IpDetailType.Gap;
                    gapIpDetail.GapReceiptNo = string.Empty;
                    gapIpDetail.ReceivedQty = 0;
                    gapIpDetail.IsClose = false;
                    gapIpDetail.GapIpDetailId = ipDetail.Id;
                }

                gapIpDetail.Qty += -(remainQty / ipDetail.UnitQty);          //多收，数量为负，转为订单单位

                IpLocationDetail gapIpLocationDetail = new IpLocationDetail();
                gapIpLocationDetail.IpNo = ipDetail.IpNo;
                gapIpLocationDetail.OrderType = ipDetail.OrderType;
                gapIpLocationDetail.OrderDetailId = ipDetail.OrderDetailId;
                gapIpLocationDetail.Item = ipDetail.Item;
                gapIpLocationDetail.HuId = null;  //收货未匹配产生的差异全部为数量（除了条码匹配条码）
                gapIpLocationDetail.LotNo = null;
                gapIpLocationDetail.IsCreatePlanBill = false;
                gapIpLocationDetail.PlanBill = null;
                gapIpLocationDetail.ActingBill = null;
                gapIpLocationDetail.IsFreeze = false;
                gapIpLocationDetail.IsATP = false;          //差异不能进行MRP运算
                gapIpLocationDetail.QualityType = ipDetail.QualityType;
                gapIpLocationDetail.OccupyType = com.Sconit.CodeMaster.OccupyType.None;
                gapIpLocationDetail.OccupyReferenceNo = null;
                gapIpLocationDetail.Qty = -remainQty;   //多收，产生负数的差异
                gapIpLocationDetail.ReceivedQty = 0;
                gapIpLocationDetail.IsClose = false;

                gapIpDetail.AddIpLocationDetail(gapIpLocationDetail);
                #endregion
            }
        }
        #endregion

        #region 送货单差异调整
        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster AdjustIpGap(IList<IpDetail> ipDetailList, CodeMaster.IpGapAdjustOption ipGapAdjustOption)
        {
            return AdjustIpGap(ipDetailList, ipGapAdjustOption, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster AdjustIpGap(IList<IpDetail> ipDetailList, CodeMaster.IpGapAdjustOption ipGapAdjustOption, DateTime effectiveDate)
        {
            #region 判断送货单明细是否差异类型是否合并收货
            if (ipDetailList.Where(det => det.Type == CodeMaster.IpDetailType.Normal).Count() > 0)
            {
                throw new TechnicalException("送货单差异明细类型不正确。");
            }
            #endregion

            #region 判断送货单是否合并调整
            if ((from det in ipDetailList select det.IpNo).Distinct().Count() > 1)
            {
                throw new TechnicalException("送货单不能合并调整差异。");
            }
            #endregion

            #region 判断是否全0发货
            if (ipDetailList == null || ipDetailList.Count == 0)
            {
                throw new BusinessException("收货差异调整明细不能为空。");
            }

            IList<IpDetail> nonZeroIpDetailList = ipDetailList.Where(o => o.ReceiveQtyInput != 0).ToList();

            if (nonZeroIpDetailList.Count == 0)
            {
                throw new BusinessException("收货差异调整明细不能为空。");
            }
            #endregion

            #region 查询送货单头对象
            string ipNo = (from det in ipDetailList select det.IpNo).Distinct().Single();
            IpMaster ipMaster = this.genericMgr.FindById<IpMaster>(ipNo);
            ipMaster.IpDetails = nonZeroIpDetailList;
            #endregion

            #region 查询订单头对象
            IList<OrderMaster> orderMasterList = LoadOrderMasters((from det in nonZeroIpDetailList
                                                                   select det.OrderNo).Distinct().ToArray());
            #endregion

            #region 获取收货订单类型
            IList<com.Sconit.CodeMaster.OrderType> orderTypeList = (from orderMaster in orderMasterList
                                                                    group orderMaster by orderMaster.Type into result
                                                                    select result.Key).ToList();

            if (orderTypeList.Count > 1)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_CannotMixOrderTypeReceive);
            }

            com.Sconit.CodeMaster.OrderType orderType = orderTypeList.First();
            #endregion

            #region 查询送货单库存对象
            IList<IpLocationDetail> ipLocationDetailList = LoadIpLocationDetails((from det in nonZeroIpDetailList
                                                                                  select det.Id).ToArray());
            #endregion

            #region 循环检查发货明细
            foreach (IpDetail ipDetail in nonZeroIpDetailList)
            {
                #region 是否过量收货判断
                if (Math.Abs(ipDetail.ReceivedQty) >= Math.Abs(ipDetail.Qty))
                {
                    //送货单的收货数已经大于等于发货数
                    throw new BusinessException("送货单{0}行号{1}零件{2}的收货数量超出了差异数量。", ipMaster.IpNo, ipDetail.Sequence.ToString(), ipDetail.Item);
                }
                else if (!ipMaster.IsReceiveExceed && Math.Abs(ipDetail.ReceivedQty + ipDetail.ReceiveQtyInput) > Math.Abs(ipDetail.Qty))
                {
                    //送货单的收货数 + 本次收货数大于发货数
                    throw new BusinessException("送货单{0}行号{1}零件{2}的收货数量超出了差异数量。", ipMaster.IpNo, ipDetail.Sequence.ToString(), ipDetail.Item);
                }
                #endregion

                #region 差异明细是否已经关闭
                if (ipDetail.IsClose)
                {
                    throw new BusinessException("送货单{0}行号{1}零件{2}已经关闭，不能进行差异调整。", ipMaster.IpNo, ipDetail.Sequence.ToString(), ipDetail.Item);
                }
                #endregion
            }
            #endregion

            #region 循环更新订单明细
            BusinessException businessException = new BusinessException();

            if (orderType != CodeMaster.OrderType.ScheduleLine)
            {
                #region 非计划协议
                #region 查询订单明细对象
                IList<OrderDetail> orderDetailList = LoadOrderDetails((from det in nonZeroIpDetailList
                                                                       where det.OrderDetailId.HasValue
                                                                       select det.OrderDetailId.Value).Distinct().ToArray());
                #endregion

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    IList<IpDetail> targetIpDetailList = (from det in nonZeroIpDetailList
                                                          where det.OrderDetailId == orderDetail.Id
                                                          select det).ToList();

                    #region 调整发货方库存
                    if (ipGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
                    {
                        //更新订单的发货数
                        orderDetail.ShippedQty -= targetIpDetailList.Sum(det => det.ReceiveQtyInput);
                        genericMgr.Update(orderDetail);
                    }
                    #endregion

                    #region 调整收货方库存
                    else
                    {
                        //更新订单的收货数
                        orderDetail.ReceivedQty += targetIpDetailList.Sum(det => det.ReceiveQtyInput);
                        genericMgr.Update(orderDetail);
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                #region 计划协议
                foreach (IpDetail ipDetail in nonZeroIpDetailList)
                {
                    decimal remainReceiveQty = ipDetail.ReceiveQtyInput;

                    if (ipGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
                    {
                        #region 调整发货方库存
                        //更新订单的发货数
                        IList<OrderDetail> scheduleOrderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>("select * from ORD_OrderDet_8 where ExtNo = ? and ExtSeq like ? and ScheduleType = ? and ShipQty > RecQty order by EndDate desc",
                                                new object[] { ipDetail.ExternalOrderNo, ipDetail.ExternalSequence + "-%", CodeMaster.ScheduleType.Firm });

                        if (scheduleOrderDetailList != null && scheduleOrderDetailList.Count > 0)
                        {
                            foreach (OrderDetail scheduleOrderDetail in scheduleOrderDetailList)
                            {
                                if (remainReceiveQty > (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty))
                                {
                                    remainReceiveQty -= (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty);
                                    scheduleOrderDetail.ShippedQty = scheduleOrderDetail.ReceivedQty;
                                }
                                else
                                {
                                    scheduleOrderDetail.ShippedQty -= remainReceiveQty;
                                    remainReceiveQty = 0;
                                    break;
                                }

                                this.genericMgr.Update(scheduleOrderDetail);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 调整收货方库存
                        //更新订单的收货数
                        IList<OrderDetail> scheduleOrderDetailList = this.genericMgr.FindEntityWithNativeSql<OrderDetail>("select * from ORD_OrderDet_8 where ExtNo = ? and ExtSeq like ? and ScheduleType = ? and ShipQty > RecQty order by EndDate",
                                                new object[] { ipDetail.ExternalOrderNo, ipDetail.ExternalSequence + "-%", CodeMaster.ScheduleType.Firm });

                        if (scheduleOrderDetailList != null && scheduleOrderDetailList.Count > 0)
                        {
                            foreach (OrderDetail scheduleOrderDetail in scheduleOrderDetailList)
                            {
                                if (remainReceiveQty > (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty))
                                {
                                    remainReceiveQty -= (scheduleOrderDetail.ShippedQty - scheduleOrderDetail.ReceivedQty);
                                    scheduleOrderDetail.ReceivedQty = scheduleOrderDetail.ShippedQty;
                                }
                                else
                                {
                                    scheduleOrderDetail.ReceivedQty += remainReceiveQty;
                                    remainReceiveQty = 0;
                                    break;
                                }

                                this.genericMgr.Update(scheduleOrderDetail);
                            }
                        }
                        #endregion
                    }

                    if (remainReceiveQty > 0)
                    {
                        businessException.AddMessage(Resources.ORD.IpMaster.Errors_ReceiveQtyExcceedOrderQty, ipMaster.IpNo, ipDetail.Item);
                    }
                }
                #endregion
            }

            #region 循环IpDetail，更新收货数
            foreach (IpDetail targetIpDetail in nonZeroIpDetailList)
            {
                #region 收货输入和送货单库存明细匹配
                IList<IpLocationDetail> targetIpLocationDetailList = (from ipLocDet in ipLocationDetailList
                                                                      where ipLocDet.IpDetailId == targetIpDetail.Id
                                                                      select ipLocDet).OrderByDescending(d => d.IsConsignment).ToList();  //排序为了先匹配寄售的

                bool isContainHu = targetIpLocationDetailList.Where(ipLocDet => !string.IsNullOrWhiteSpace(ipLocDet.HuId)).Count() > 0;

                if (ipMaster.IsReceiveScanHu)
                {
                    #region 收货扫描条码
                    if (isContainHu)
                    {
                        #region 条码匹配条码
                        foreach (IpDetailInput targetIpDetailInput in targetIpDetail.IpDetailInputs)
                        {
                            IpLocationDetail matchedIpLocationDetail = targetIpLocationDetailList.Where(ipLocDet => ipLocDet.HuId == targetIpDetailInput.HuId).SingleOrDefault();
                            if (matchedIpLocationDetail != null)
                            {
                                #region 更新库存状态
                                matchedIpLocationDetail.ReceivedQty = matchedIpLocationDetail.Qty;
                                matchedIpLocationDetail.IsClose = true;
                                targetIpDetailInput.AddReceivedIpLocationDetail(matchedIpLocationDetail);

                                genericMgr.Update(matchedIpLocationDetail);
                                #endregion
                            }
                            else
                            {
                                #region 未匹配到的条码，报错
                                businessException.AddMessage("条码{0}不在送货单差异调整明细中。", targetIpDetailInput.HuId);
                                #endregion
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 数量匹配条码
                        IpDetail gapIpDetail = CycleMatchIpDetailInput(targetIpDetail, targetIpDetail.IpDetailInputs, targetIpLocationDetailList);
                        if (gapIpDetail != null)
                        {
                            businessException.AddMessage("送货单{0}行号{1}零件{2}差异调整不正确。", ipMaster.IpNo, targetIpDetail.Sequence.ToString(), targetIpDetail.Item);
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 收货不扫描条码
                    if (isContainHu)
                    {
                        #region 条码匹配数量
                        IpDetail gapIpDetail = CycleMatchIpDetailInput(targetIpDetail, targetIpDetail.IpDetailInputs, targetIpLocationDetailList);
                        if (gapIpDetail != null)
                        {
                            businessException.AddMessage("送货单{0}行号{1}零件{2}差异调整不正确。", ipMaster.IpNo, targetIpDetail.Sequence.ToString(), targetIpDetail.Item);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 数量匹配数量
                        IpDetail gapIpDetail = CycleMatchIpDetailInput(targetIpDetail, targetIpDetail.IpDetailInputs, targetIpLocationDetailList);
                        if (gapIpDetail != null)
                        {
                            businessException.AddMessage("送货单{0}行号{1}零件{2}差异调整不正确。", ipMaster.IpNo, targetIpDetail.Sequence.ToString(), targetIpDetail.Item);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region 更新IpDetail上的收货数
                targetIpDetail.ReceivedQty += targetIpDetail.ReceiveQtyInput;
                if (targetIpLocationDetailList.Where(i => !i.IsClose).Count() == 0)
                {
                    //只有所有的IpLocationDetail关闭才能关闭
                    targetIpDetail.IsClose = true;
                }
                genericMgr.Update(targetIpDetail);
                #endregion
            }
            #endregion

            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            #region 差异调整
            ReceiptMaster receiptMaster = null;
            receiptMaster = this.receiptMgr.TransferIpGap2Receipt(ipMaster, ipGapAdjustOption);
            this.receiptMgr.CreateReceipt(receiptMaster, effectiveDate);
            #endregion

            #region 尝试关闭送货单
            this.ipMgr.TryCloseIp(ipMaster);
            #endregion

            #region 尝试关闭订单
            //处理超收的不能关闭订单
            //foreach (OrderMaster orderMaster in orderMasterList)
            //{
            //    TryCloseOrder(orderMaster);
            //}
            #endregion

            return receiptMaster;
        }

        #endregion

        #region 取消订单
        [Transaction(TransactionMode.Requires)]
        public void CancelOrder(string orderNo)
        {
            CancelOrder(this.genericMgr.FindById<OrderMaster>(orderNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelOrder(OrderMaster orderMaster)
        {
            //if (!Utility.SecurityHelper.HasPermission(orderMaster))
            //{
            //    throw new BusinessException("没有此订单{0}的操作权限。", orderMaster.OrderNo);
            //}
            if (orderMaster.Status != CodeMaster.OrderStatus.Submit && orderMaster.Status != CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException("不能取消状态为{1}的订单{0}。", orderMaster.OrderNo,
                       systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
            else
            {
                orderMaster.Status = CodeMaster.OrderStatus.Cancel;
                orderMaster.CancelDate = DateTime.Now;
                User user = SecurityContextHolder.Get();
                orderMaster.CancelUserId = user.Id;
                orderMaster.CancelUserName = user.FullName;
                this.genericMgr.Update(orderMaster);
            }

            //string loc = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.WMSAnjiRegion);
            //if (orderMaster.PartyFrom.Equals(loc, StringComparison.OrdinalIgnoreCase))
            //{
            //    //this.genericMgr.FlushSession();
            //    //AsyncRecourdMessageQueue(MethodNameType.CancelOrder, orderMaster.OrderNo);
            //    this.CreateMessageQueue("CancelOrder", orderMaster.OrderNo);
            //}
        }
        #endregion

        #region 关闭订单
        [Transaction(TransactionMode.Requires)]
        public void ManualCloseOrder(string orderNo)
        {
            this.ManualCloseOrder(this.genericMgr.FindById<OrderMaster>(orderNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void ManualCloseOrder(OrderMaster orderMaster)
        {
            CloseOrder(orderMaster, true, true);
        }

        [Transaction(TransactionMode.Requires)]
        public void AutoCloseOrder()
        {
            IList<OrderMaster> orderMasterList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where Status = ?", CodeMaster.OrderStatus.Complete);

            if (orderMasterList != null && orderMasterList.Count > 0)
            {
                foreach (OrderMaster orderMaster in orderMasterList)
                {
                    try
                    {
                        TryCloseOrder(orderMaster);
                    }
                    catch (BusinessException ex)
                    {
                        log.Debug(string.Format("Close Order:{0} fail, Message {2}", orderMaster.OrderNo, ex.Message));
                    }
                    catch (Exception ex)
                    {
                        this.genericMgr.CleanSession();
                        log.Error(string.Format("Close Order:{0} error, Message {2}", orderMaster.OrderNo, ex.Message));
                    }
                }
            }
        }

        private void TryCloseOrder(string orderNo)
        {
            this.TryCloseOrder(this.genericMgr.FindById<OrderMaster>(orderNo));
        }

        private void TryCloseOrder(OrderMaster orderMaster)
        {
            if (!orderMaster.IsOpenOrder)
            {
                CloseOrder(orderMaster, false, false);
            }
        }

        private void CloseOrder(OrderMaster orderMaster, bool isForce, bool isThrowException = true)
        {
            if (orderMaster.Status == CodeMaster.OrderStatus.InProcess
                || orderMaster.Status == CodeMaster.OrderStatus.Complete)
            {
                DateTime dateTimeNow = DateTime.Now;
                User user = SecurityContextHolder.Get();

                this.genericMgr.FlushSession();
                BusinessException businessException = new BusinessException();

                #region 强制关闭生产单，先把状态改为Complete
                if (isForce && orderMaster.Status == CodeMaster.OrderStatus.InProcess && orderMaster.Type == CodeMaster.OrderType.Production)
                {
                    //生产先做订单完工
                    orderMaster.Status = CodeMaster.OrderStatus.Complete;
                    orderMaster.CompleteDate = dateTimeNow;
                    orderMaster.CompleteUserId = user.Id;
                    orderMaster.CompleteUserName = user.FullName;
                    this.genericMgr.Update(orderMaster);
                }
                #endregion

                #region 条件1所有订单明细收货数大于等于订单数 //强制关闭不用校验这条
                if (!isForce)
                {
                    string hql = "select count(*) as counter from OrderDetail where OrderNo = ? and (ReceivedQty+ScrapQty) < OrderedQty";
                    long counter = this.genericMgr.FindAll<long>(hql, new Object[] { orderMaster.OrderNo })[0];
                    if (counter > 0)
                    {
                        return;
                    }
                }
                #endregion

                #region 条件2所有送货单明细全部关闭
                if (orderMaster.Type != CodeMaster.OrderType.Production)
                {
                    string hql = "select count(*) as counter from IpDetail where OrderNo = ? and IsClose = ?";
                    long counter = this.genericMgr.FindAll<long>(hql, new Object[] { orderMaster.OrderNo, false })[0];
                    if (counter > 0)
                    {
                        if (!isForce)
                        {
                            //非强制关闭直接返回
                            return;
                        }
                        businessException.AddMessage("和订单相关的送货单明细没有全部关闭，不能关闭订单{0}。", orderMaster.OrderNo);
                    }
                }
                #endregion

                #region 条件3所有的拣货单全部关闭
                if (orderMaster.Type == CodeMaster.OrderType.Transfer
                   || orderMaster.Type == CodeMaster.OrderType.SubContractTransfer
                    || orderMaster.Type == CodeMaster.OrderType.Distribution)
                {
                    string hql = "select count(*) as counter from PickListDetail where OrderNo = ? and IsClose = ?";
                    long counter = this.genericMgr.FindAll<long>(hql, new Object[] { orderMaster.OrderNo, false })[0];
                    if (counter > 0)
                    {
                        if (!isForce)
                        {
                            //非强制关闭直接返回
                            return;
                        }
                        businessException.AddMessage("和订单相关的捡货单明细没有全部关闭，不能关闭订单{0}。", orderMaster.OrderNo);
                    }
                }
                #endregion

                if (businessException.HasMessage)
                {
                    if (isThrowException)
                    {
                        throw businessException;
                    }
                }
                else
                {
                    orderMaster.Status = CodeMaster.OrderStatus.Close;
                    orderMaster.CloseDate = dateTimeNow;
                    orderMaster.CloseUserId = user.Id;
                    orderMaster.CloseUserName = user.FullName;
                    this.genericMgr.Update(orderMaster);
                }
            }
            else if (!isForce)
            {
                throw new BusinessException("不能关闭状态为{1}的订单{0}。", orderMaster.OrderNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
            }
        }

        private void CompleteVanSubOrder(OrderMaster vanSubOrder)
        {
            DateTime dateTimeNow = DateTime.Now;
            User user = SecurityContextHolder.Get();

            vanSubOrder.Status = CodeMaster.OrderStatus.Complete;
            vanSubOrder.CompleteDate = dateTimeNow;
            vanSubOrder.CompleteUserId = user.Id;
            vanSubOrder.CompleteUserName = user.FullName;
            this.genericMgr.Update(vanSubOrder);
        }

        [Transaction(TransactionMode.Requires)]
        public void CleanOrder(List<string> flowCodeList)
        {
            if (flowCodeList == null || flowCodeList.Count == 0)
            {
                return;
            }

            var orderMasterList = this.genericMgr.FindAllIn<OrderMaster>
                ("from OrderMaster where  Status in(?,?,?) and SubType <>? and Windowtime<? and Flow in(?",
              flowCodeList,
              new object[] {
                  CodeMaster.OrderStatus.Create,
                  CodeMaster.OrderStatus.Submit,
                  CodeMaster.OrderStatus.InProcess,
                  CodeMaster.OrderSubType.Return, 
                  DateTime.Now.Date
              });

            foreach (OrderMaster orderMaster in orderMasterList)
            {
                try
                {
                    if (orderMaster.Status == CodeMaster.OrderStatus.Create)
                    {
                        this.DeleteOrder(orderMaster.OrderNo);
                    }
                    else
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        User user = SecurityContextHolder.Get();
                        orderMaster.Status = CodeMaster.OrderStatus.Complete;
                        orderMaster.CompleteDate = dateTimeNow;
                        orderMaster.CompleteUserId = user.Id;
                        orderMaster.CompleteUserName = user.FullName;
                        this.genericMgr.Update(orderMaster);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("CleanOrder{0}失败", orderMaster.OrderNo), ex);
                }
            }
        }

        private void CloseVanOrder(OrderMaster vanOrder, ProductLineMap productLineMap)
        {
            DateTime dateTimeNow = DateTime.Now;
            User user = SecurityContextHolder.Get();

            #region 总装生产单关闭时，关闭所有子生产单
            IList<OrderMaster> subOrderMasterList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where Type = ? and Flow in (?,?) and Status <> ?", new object[] { CodeMaster.OrderType.Production, productLineMap.CabFlow, productLineMap.ChassisFlow, CodeMaster.OrderStatus.Close });

            if (subOrderMasterList != null && subOrderMasterList.Count > 0)
            {
                foreach (OrderMaster subOrderMaster in subOrderMasterList)
                {
                    if (subOrderMaster.Status != CodeMaster.OrderStatus.Complete)
                    {
                        throw new BusinessException("子生产单{0}的状态为{1}，整车生产单{2}不能关闭。", subOrderMaster.OrderNo,
                            systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)subOrderMaster.Status).ToString()),
                            vanOrder.OrderNo);
                    }
                    else
                    {
                        subOrderMaster.Status = CodeMaster.OrderStatus.Close;
                        subOrderMaster.CloseDate = dateTimeNow;
                        subOrderMaster.CloseUserId = user.Id;
                        subOrderMaster.CloseUserName = user.FullName;
                        this.genericMgr.Update(subOrderMaster);
                    }
                }
            }
            #endregion

            #region 关闭总装生产单
            vanOrder.Status = CodeMaster.OrderStatus.Close;
            vanOrder.CloseDate = dateTimeNow;
            vanOrder.CloseUserId = user.Id;
            vanOrder.CloseUserName = user.FullName;
            this.genericMgr.Update(vanOrder);
            #endregion
        }

        private BusinessException VerifyVanOrderClose(OrderMaster orderMaster, ProductLineMap productLineMap)
        {
            BusinessException businessException = new BusinessException();
            #region 生产单关闭校验
            #region 条件5生产单的PlanBackflush全部关闭
            if (orderMaster.Type == CodeMaster.OrderType.Production
                || orderMaster.Type == CodeMaster.OrderType.SubContract)
            {

                #region 总装生产要校验总装、驾驶室和底盘的PlanBackflush
                string hql = "select count(*) as counter from PlanBackflush where OrderNo in (?, ?, ?) and IsClose = ?";
                long counter = this.genericMgr.FindAll<long>(hql, new Object[] { productLineMap.ProductLine, productLineMap.CabFlow, productLineMap.ChassisFlow, false })[0];
                if (counter > 0)
                {
                    businessException.AddMessage("加权平均扣料的零件还没有进行回冲，不能关闭订单{0}。", orderMaster.OrderNo);
                }
                #endregion
            }
            #endregion

            #region 条件6生产线上没有订单的投料
            if (orderMaster.Type == CodeMaster.OrderType.Production
                || orderMaster.Type == CodeMaster.OrderType.SubContract)
            {
                #region 总装生产要校验总装、驾驶室和底盘的订单投料
                string hql = "select count(*) as counter from ProductLineLocationDetail where OrderNo in (?, ?, ?) and IsClose = ?";
                long counter = this.genericMgr.FindAll<long>(hql, new Object[] { productLineMap.ProductLine, productLineMap.CabFlow, productLineMap.ChassisFlow, false })[0];
                if (counter > 0)
                {
                    businessException.AddMessage("生产线上还有投料的零件没有回冲，不能关闭订单{0}。", orderMaster.OrderNo);
                }
                #endregion
            }
            #endregion

            #region 条件7和TraceCode所有的失效模式关闭
            if ((orderMaster.Type == CodeMaster.OrderType.Production
               || orderMaster.Type == CodeMaster.OrderType.SubContract)
               && !string.IsNullOrWhiteSpace(orderMaster.TraceCode))
            {
                #region 只有总装的生产单关闭才需要校验，驾驶室和底盘的不需要
                string hql = "select count(*) as counter from IssueMaster where BackYards = ? and Status in (?,?,?)";
                long issueCounter = this.genericMgr.FindAll<long>(hql, new Object[] { orderMaster.TraceCode, CodeMaster.IssueStatus.Create, CodeMaster.IssueStatus.Submit, CodeMaster.IssueStatus.InProcess })[0];
                if (issueCounter > 0)
                {
                    businessException.AddMessage("失效模式没有全部关闭，不能关闭订单{0}。", orderMaster.OrderNo);
                }
                #endregion
            }
            #endregion

            #region 条件8生产单上的所有关键件全部投料，考虑到搭错的关键件Bom，不需要校验这条，可以给出警告信息
            if ((orderMaster.Type == CodeMaster.OrderType.Production
               || orderMaster.Type == CodeMaster.OrderType.SubContract))
            {
                //throw new NotImplementedException();
            }
            #endregion
            #endregion

            return businessException;
        }
        #endregion

        #region 生产单暂停
        [Transaction(TransactionMode.Requires)]
        public void PauseProductOrder(string orderNo, int? pauseOperation)
        {
            PauseProductOrder(this.genericMgr.FindById<OrderMaster>(orderNo), pauseOperation);
        }

        [Transaction(TransactionMode.Requires)]
        public void PauseProductOrder(OrderMaster orderMaster, int? pauseOperation)  //在指定工序之后暂停
        {
            log.Debug("Start pause product order, orderNo[" + orderMaster.OrderNo + "], operation[" + pauseOperation + "].");
            try
            {
                #region 检查
                if (orderMaster.Status != CodeMaster.OrderStatus.Submit && orderMaster.Status != CodeMaster.OrderStatus.InProcess)
                {
                    throw new BusinessException("生产单{0}的状态为{1}，不能暂停。", orderMaster.OrderNo,
                        systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)orderMaster.Status).ToString()));
                }

                if (orderMaster.IsPause || orderMaster.IsPlanPause)
                {
                    throw new BusinessException("生产单{0}已经暂停。", orderMaster.OrderNo);
                }

                if (orderMaster.IsProductLinePause)
                {
                    throw new BusinessException("生产线{0}已经暂停。", orderMaster.Flow);
                }

                if (orderMaster.Status == CodeMaster.OrderStatus.InProcess)
                {
                    if (!pauseOperation.HasValue)
                    {
                        throw new BusinessException("没有指定暂停的工序。");
                    }
                    else
                    {
                        #region 检查工序合法性
                        IList<OrderOperation> orderOperationList = this.genericMgr.FindAll<OrderOperation>("from OrderOperation where OrderNo = ? and Operation = ?", new object[] { orderMaster.OrderNo, pauseOperation.Value });

                        if (orderOperationList == null || orderOperationList.Count == 0)
                        {
                            throw new BusinessException("生产单{0}没有工序{1}。", orderMaster.OrderNo, pauseOperation.Value.ToString());
                        }

                        //if (orderOperationList.Where(o => o.IsBackflush).Count() > 0)
                        //{
                        //    throw new BusinessException("生产单{0}的工序{1}已经回冲物料，不能选择该工序暂停。", orderMaster.OrderNo, pauseOperation.Value.ToString());
                        //}
                        #endregion
                    }
                }
                #endregion

                #region 更新生产单
                if (orderMaster.Status == CodeMaster.OrderStatus.Submit)
                {
                    orderMaster.IsPause = true;
                    orderMaster.PauseTime = DateTime.Now;
                }
                else
                {
                    orderMaster.IsPlanPause = true;
                    orderMaster.PauseSequence = pauseOperation.Value;
                }
                this.genericMgr.Update(orderMaster);

                log.Debug("Success pause main product order, orderNo[" + orderMaster.OrderNo + "]");
                #endregion

                #region 暂停排序单和KIT
                PauseSequenceAndKitOrder(orderMaster, pauseOperation);
                #endregion
            }
            catch (Exception ex)
            {
                log.Error("Fail pause product order, orderNo[" + orderMaster.OrderNo + "], operation[" + pauseOperation + "].", ex);
                throw ex;
            }
            log.Debug("Success pause product order, orderNo[" + orderMaster.OrderNo + "], operation[" + pauseOperation + "].");
        }

        private void PauseSequenceAndKitOrder(OrderMaster productOrder, int? pauseOperation)
        {
            //todo：发送暂停通知邮件
            DateTime dateTimeNow = DateTime.Now;

            #region 查找需要暂停的工位
            string selectPauseLocationStatement = "select distinct Location from OrderBomDetail where OrderNo = ?";
            IList<object> selectPauseLocationParm = new List<object>();
            selectPauseLocationParm.Add(productOrder.OrderNo);
            if (pauseOperation.HasValue)
            {
                selectPauseLocationStatement += " and Operation > ?";  //在指定工序之后暂停，大于工序后的排序单/Kit单全部暂停
                selectPauseLocationParm.Add(pauseOperation.Value);
            }
            IList<string> pauseLocationList = this.genericMgr.FindAll<string>(selectPauseLocationStatement, selectPauseLocationParm.ToArray());

            ProductLineMap productLineMap = genericMgr.FindAll<ProductLineMap>(@"from ProductLineMap as p 
                                                                                    where (p.ProductLine = ? or p.CabFlow = ? or p.ChassisFlow = ?) 
                                                                                    and p.ProductLine is not null 
                                                                                    and p.CabFlow is not null 
                                                                                    and p.ChassisFlow is not null", new object[] { productOrder.Flow, productOrder.Flow, productOrder.Flow }).Single();

            #region 添加生产线的虚拟库位
            if (productLineMap.ProductLine == productOrder.Flow)
            {
                pauseLocationList.Add(productLineMap.VanLocation);
            }
            else if (productLineMap.CabFlow == productOrder.Flow)
            {
                pauseLocationList.Add(productLineMap.CabLocation);
            }
            else if (productLineMap.ChassisFlow == productOrder.Flow)
            {
                pauseLocationList.Add(productLineMap.ChassisLocation);
            }
            #endregion
            #endregion

            #region 循环查找OrderBinding
            IList<OrderBinding> orderBindingList = NestGetOrderBinding(productOrder.OrderNo);

            if (orderBindingList == null || orderBindingList.Count == 0)
            {
                log.Debug("No order binding.");
                return;
            }
            #endregion

            #region 查询Kit单和排序单
            string selectKitOrderStatement = string.Empty;
            string selectSeqOrderStatement = string.Empty;

            IList<object> selectKitOrderParm = new List<object>();
            IList<object> selectSeqOrderParm = new List<object>();

            foreach (OrderBinding orderBinding in orderBindingList)
            {
                if (selectKitOrderStatement == string.Empty)
                {
                    selectKitOrderStatement = "from OrderMaster where OrderStrategy = ? and Status = ? and IsPause = ? and OrderNo in (?";
                    selectSeqOrderStatement = "from OrderMaster where OrderStrategy = ? and Status in (?, ?) and IsPause = ? and OrderNo in (?";

                    selectKitOrderParm.Add(CodeMaster.FlowStrategy.KIT);
                    selectKitOrderParm.Add(CodeMaster.OrderStatus.Submit);
                    selectKitOrderParm.Add(false);

                    selectSeqOrderParm.Add(CodeMaster.FlowStrategy.SEQ);
                    selectSeqOrderParm.Add(CodeMaster.OrderStatus.Submit);
                    selectSeqOrderParm.Add(CodeMaster.OrderStatus.InProcess);
                    selectSeqOrderParm.Add(false);
                }
                else
                {
                    selectKitOrderStatement += ", ?";
                    selectSeqOrderStatement += ", ?";
                }

                selectKitOrderParm.Add(orderBinding.BindOrderNo);
                selectSeqOrderParm.Add(orderBinding.BindOrderNo);
            }

            selectKitOrderStatement += ")";
            selectSeqOrderStatement += ")";

            IList<OrderMaster> kitOrderList = this.genericMgr.FindAll<OrderMaster>(selectKitOrderStatement, selectKitOrderParm.ToArray());
            IList<OrderMaster> seqOrderList = this.genericMgr.FindAll<OrderMaster>(selectSeqOrderStatement, selectSeqOrderParm.ToArray());

            if ((kitOrderList == null || kitOrderList.Count == 0) && (seqOrderList == null || seqOrderList.Count == 0))
            {
                log.Debug("No sequence and kit order to pause.");
                return;
            }
            #endregion

            #region 查询排序装箱单
            #region 查询排序装箱单头
            IList<SequenceMaster> sequenceMasterList = null;
            if (seqOrderList != null && seqOrderList.Count > 0)
            {
                string selectSequenceMasterStatement = string.Empty;
                IList<object> selectSequenceMasterParas = new List<object>();

                foreach (OrderMaster seqOrder in seqOrderList)
                {
                    if (selectSequenceMasterStatement == string.Empty)
                    {
                        selectSequenceMasterStatement = "from SequenceMaster where Status in (?, ?) and SequenceNo in (select distinct SequenceNo from SequenceDetail where IsClose = ? and OrderNo in (?";
                        selectSequenceMasterParas.Add(CodeMaster.SequenceStatus.Submit);
                        selectSequenceMasterParas.Add(CodeMaster.SequenceStatus.Pack);
                        selectSequenceMasterParas.Add(false);
                    }
                    else
                    {
                        selectSequenceMasterStatement += ", ?";
                    }
                    selectSequenceMasterParas.Add(seqOrder.OrderNo);
                }
                selectSequenceMasterStatement += "))";

                sequenceMasterList = this.genericMgr.FindAll<SequenceMaster>(selectSequenceMasterStatement, selectSequenceMasterParas.ToArray());
            }
            #endregion

            #region 查询排序装箱单明细
            IList<SequenceDetail> sequenceDetailList = null;
            if (sequenceMasterList != null && sequenceMasterList.Count > 0)
            {
                string selectSequenceDetailStatement = string.Empty;
                IList<object> selectSequenceDetailParas = new List<object>();

                foreach (SequenceMaster sequenceMaster in sequenceMasterList)
                {
                    if (selectSequenceDetailStatement == string.Empty)
                    {
                        selectSequenceDetailStatement = "from SequenceDetail where IsClose = ? and SequenceNo in (?";
                        selectSequenceDetailParas.Add(false);
                    }
                    else
                    {
                        selectSequenceDetailStatement += ", ?";
                    }
                    selectSequenceDetailParas.Add(sequenceMaster.SequenceNo);
                }
                selectSequenceDetailStatement += "))";

                sequenceDetailList = this.genericMgr.FindAll<SequenceDetail>(selectSequenceDetailStatement, selectSequenceDetailParas.ToArray());
            }
            #endregion
            #endregion

            #region 直接绑定的Kit/SEQ单处理
            IList<string> pausedOrderNoList = new List<string>();
            #region KIT单处理
            if (kitOrderList != null && kitOrderList.Count > 0)
            {
                //没有指定暂停的工位则全部暂停
                //kit单的目的库位等于需要暂停的工位
                foreach (OrderMaster kitOrder in kitOrderList.Where(k => pauseLocationList.Contains(k.LocationTo)))
                {
                    DoPauseOrder(kitOrder, null, null, pausedOrderNoList, dateTimeNow);
                }
            }
            #endregion

            #region SEQ单处理
            if (seqOrderList != null && seqOrderList.Count > 0)
            {
                //已创建排序装箱单，并且被生产单直接绑定的排序单列表
                IList<OrderMaster> startedInLocationSeqOrderList = seqOrderList.Where(s => pauseLocationList.Contains(s.LocationTo)).ToList();

                if (startedInLocationSeqOrderList != null && startedInLocationSeqOrderList.Count > 0)
                {
                    foreach (OrderMaster startedInLocationSeqOrder in startedInLocationSeqOrderList)
                    {
                        DoPauseOrder(startedInLocationSeqOrder, sequenceMasterList, sequenceDetailList, pausedOrderNoList, dateTimeNow);
                    }
                }
            }
            #endregion
            #endregion

            #region 不是直接绑定的Kit/SEQ单处理
            IList<OrderMaster> unDirectBindOrderList = new List<OrderMaster>();

            //不是直接绑定的Kit列表
            IList<OrderMaster> unDirectBindKitOrderList = kitOrderList != null && kitOrderList.Count > 0 ?
                kitOrderList.Where(k => !pauseLocationList.Contains(k.LocationTo)).ToList() : null;
            if (unDirectBindKitOrderList != null)
            {
                ((List<OrderMaster>)unDirectBindOrderList).AddRange(unDirectBindKitOrderList);
            }

            //不是直接绑定的Seq列表
            IList<OrderMaster> unDirectBindReleasedSeqOrderList = seqOrderList != null && seqOrderList.Count > 0 ?
                seqOrderList.Where(s => !pauseLocationList.Contains(s.LocationTo)).ToList() : null;
            if (unDirectBindReleasedSeqOrderList != null)
            {
                ((List<OrderMaster>)unDirectBindOrderList).AddRange(unDirectBindReleasedSeqOrderList);
            }
            #region 递归处理没有被直接绑定的订单绑定
            //一层层查找没有被直接绑定的订单是否被已经暂停的订单绑定
            while (pausedOrderNoList != null && pausedOrderNoList.Count > 0)
            {
                pausedOrderNoList = NestPauseUnDirectBindOrder(unDirectBindOrderList.Where(o => !o.IsPause).ToList(), pausedOrderNoList, orderBindingList, sequenceMasterList, sequenceDetailList, dateTimeNow);
            }
            #endregion
            #endregion

            #region 判断排序装箱单是否要关闭
            if (sequenceMasterList != null && sequenceMasterList.Count > 0)
            {
                foreach (SequenceMaster sequenceMaster in sequenceMasterList)
                {
                    if (sequenceDetailList.Where(s => s.SequenceNo == sequenceMaster.SequenceNo && !s.IsClose).Count() == 0)
                    {
                        //排序装箱单的排序明细全部关闭
                        if (sequenceMaster.Status == CodeMaster.SequenceStatus.Pack)
                        {
                            //自动取消装箱
                            UnPackSequenceOrder(sequenceMaster);
                        }

                        sequenceMaster.Status = CodeMaster.SequenceStatus.Cancel;
                        sequenceMaster.CancelDate = dateTimeNow;
                        sequenceMaster.CancelUserId = SecurityContextHolder.Get().Id;
                        sequenceMaster.CancelUserName = SecurityContextHolder.Get().FullName;

                        this.genericMgr.Update(sequenceMaster);
                    }
                }
            }
            #endregion
        }

        private IList<OrderBinding> NestGetOrderBinding(string orderNo)
        {
            IList<OrderBinding> orderBindingList = this.genericMgr.FindAll<OrderBinding>(
                @"select ob from OrderBinding as ob where ob.OrderNo = ? and ob.BindOrderNo is not null", new object[] { orderNo });

            if (orderBindingList != null && orderBindingList.Count > 0)
            {
                IList<OrderBinding> filterOrderBindingList = new List<OrderBinding>();

                //要过滤掉三张生产单中的其余两张，即订单不是生产单
                IList<OrderMaster> orderMasterList = this.LoadOrderMasters(orderBindingList.Select(ob => ob.BindOrderNo).ToArray());

                foreach (OrderMaster orderMaster in orderMasterList)
                {
                    OrderBinding filterOrderBinding = orderBindingList.Where(f => f.BindOrderNo == orderMaster.OrderNo && orderMaster.Type != CodeMaster.OrderType.Production && orderMaster.Type != CodeMaster.OrderType.SubContract).SingleOrDefault();

                    if (filterOrderBinding != null)
                    {
                        filterOrderBindingList.Add(filterOrderBinding);
                    }
                }

                if (filterOrderBindingList.Count > 0)
                {
                    List<OrderBinding> subOrderBindingList = new List<OrderBinding>();
                    foreach (OrderBinding orderBinding in filterOrderBindingList)
                    {
                        subOrderBindingList.AddRange(NestGetOrderBinding(orderBinding.BindOrderNo));
                    }

                    ((List<OrderBinding>)filterOrderBindingList).AddRange(subOrderBindingList);
                }

                return filterOrderBindingList;
            }

            return new List<OrderBinding>();
        }

        private IList<string> NestPauseUnDirectBindOrder(IList<OrderMaster> unDirectBindOrderList, IList<string> lastPausedOrderNoList, IList<OrderBinding> orderBindingList, IList<SequenceMaster> sequenceMasterList, IList<SequenceDetail> sequenceDetailList, DateTime dateTimeNow)
        {
            IList<string> thisPausedOrderNoList = new List<string>();

            #region 递归处理没有被直接绑定的订单绑定
            if (unDirectBindOrderList != null && unDirectBindOrderList.Count > 0)
            {
                foreach (OrderMaster unDirectBindOrder in unDirectBindOrderList)
                {
                    //已经暂停的订单绑定的订单，即下层订单
                    if (orderBindingList.Where(o => lastPausedOrderNoList.Contains(o.OrderNo)
                        && o.BindOrderNo == unDirectBindOrder.OrderNo).Count() > 0)
                    {
                        DoPauseOrder(unDirectBindOrder, sequenceMasterList, sequenceDetailList, thisPausedOrderNoList, dateTimeNow);
                    }
                }
            }
            #endregion

            return thisPausedOrderNoList;
        }

        private void DoPauseOrder(OrderMaster orderMaster, IList<SequenceMaster> sequenceMasterList, IList<SequenceDetail> sequenceDetailList, IList<string> pausedOrderNoList, DateTime dateTimeNow)
        {
            if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.KIT)
            {
                orderMaster.IsPause = true;
                orderMaster.PauseTime = dateTimeNow;
                this.genericMgr.Update(orderMaster);
                pausedOrderNoList.Add(orderMaster.OrderNo);

                log.Debug("Success pause kit order, orderNo[" + orderMaster.OrderNo + "].");
            }
            else if (orderMaster.OrderStrategy == CodeMaster.FlowStrategy.SEQ
                && sequenceDetailList != null && sequenceDetailList.Count > 0)
            {
                //如果排序装箱单已经收货，sequenceDetail为空。
                SequenceDetail sequenceDetail = sequenceDetailList.Where(s => s.OrderNo == orderMaster.OrderNo).SingleOrDefault();

                if (sequenceDetail != null)
                {
                    SequenceMaster sequenceMaster = sequenceMasterList.Where(s => s.SequenceNo == sequenceDetail.SequenceNo).Single();

                    if (sequenceMaster.Status == CodeMaster.SequenceStatus.Submit
                        || sequenceMaster.Status == CodeMaster.SequenceStatus.Pack)  //没有发货前都可以取消
                    {
                        //先取消排序装箱单明细 
                        sequenceDetail.IsClose = true;
                        this.genericMgr.Update(sequenceDetail);

                        //再暂停排序单
                        //orderMaster.Status = CodeMaster.OrderStatus.Submit;
                        //可能一个零件有多个排序件，即多张排序装箱单对应一张排序单
                        if (!orderMaster.IsPause)
                        {
                            orderMaster.IsPause = true;
                            orderMaster.PauseTime = dateTimeNow;
                            this.genericMgr.Update(orderMaster);
                            pausedOrderNoList.Add(orderMaster.OrderNo);

                            log.Debug("Success pause sequence order, orderNo[" + orderMaster.OrderNo + "].");
                        }
                        else
                        {
                            log.Debug("Sequence order already paused, orderNo[" + orderMaster.OrderNo + "].");
                        }

                        //string loc = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.WMSAnjiRegion);
                        //if (sequenceMaster.PartyFrom.Equals(loc, StringComparison.OrdinalIgnoreCase))
                        //{
                        //    //this.genericMgr.FlushSession();
                        //    //AsyncRecourdMessageQueue(MethodNameType.CancelSequenceDetail, sequenceDetail.Id.ToString());
                        //    this.CreateMessageQueue("CancelSequenceDetail", sequenceDetail.Id.ToString());
                        //}
                    }
                }
                else
                {
                    orderMaster.IsPause = true;
                    orderMaster.PauseTime = dateTimeNow;
                    this.genericMgr.Update(orderMaster);

                    log.Debug("Success pause sequence order, orderNo[" + orderMaster.OrderNo + "].");
                }
            }
        }
        #endregion

        #region 生产单暂停恢复
        [Transaction(TransactionMode.Requires)]
        public void ReStartProductOrder(string orderNo, Int64 orderSequence)
        {
            ReStartProductOrder(this.genericMgr.FindById<OrderMaster>(orderNo), orderSequence, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReStartProductOrder(string orderNo, Int64 orderSequence, bool isForce)
        {
            ReStartProductOrder(this.genericMgr.FindById<OrderMaster>(orderNo), orderSequence, isForce);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReStartProductOrder(OrderMaster productOrder, Int64 orderSequence)
        {
            ReStartProductOrder(productOrder, orderSequence, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReStartProductOrder(OrderMaster productOrder, Int64 orderSequence, bool isForce)  //暂不支持恢复至另外一条生产线
        {
            //todo：发送恢复通知邮件
            #region 检查
            if (!productOrder.IsPause)
            {
                throw new BusinessException("生产单{0}没有暂停。", productOrder.OrderNo);
            }

            if (productOrder.Sequence > orderSequence)
            {
                throw new BusinessException("生产单{0}恢复之后的顺序号{1}小于原顺序号。", productOrder.OrderNo, orderSequence.ToString());
            }

            #region 查询插入点之前的生产单
            IList<OrderMaster> beforeProductOrderList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where Type = ? and Flow = ? and Sequence < ? and IsPause = ?  order by Sequence",
                                                            new object[] { productOrder.Type, productOrder.Flow, orderSequence, false }, 0, 1);

            OrderMaster beforeProductOrder = beforeProductOrderList[0];
            #endregion

            if (beforeProductOrder.Status == CodeMaster.OrderStatus.Create)
            {
                throw new BusinessException("小于顺序号{0}的生产单{1}未释放。", orderSequence.ToString(), beforeProductOrder.OrderNo);
            }

            #region 查询插入点之后的生产单
            IList<OrderMaster> afterProductOrderList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where Type = ? and Flow = ? and Sequence >= ? and IsPause = ? and Status <> ? order by Sequence",
                                                            new object[] { productOrder.Type, productOrder.Flow, orderSequence, false, CodeMaster.OrderStatus.Cancel }, 0, 1);

            OrderMaster afterProductOrder = null;
            if (afterProductOrderList != null && afterProductOrderList.Count > 0)
            {
                afterProductOrder = afterProductOrderList[0];
            }
            #endregion

            if (afterProductOrder != null)
            {
                if (afterProductOrder.Status == CodeMaster.OrderStatus.InProcess
                    && productOrder.Status == CodeMaster.OrderStatus.Submit)
                {
                    throw new BusinessException("生产单{0}未上线，而大于顺序号{1}的生产单{2}已经上线。", productOrder.OrderNo, orderSequence.ToString(), afterProductOrder.OrderNo);
                }
                else if (afterProductOrder.Status == CodeMaster.OrderStatus.Complete)
                {
                    throw new BusinessException("大于顺序号{0}的生产单{1}已经完工。", orderSequence.ToString(), afterProductOrder.OrderNo);
                }
                else if (afterProductOrder.Status == CodeMaster.OrderStatus.Close)
                {
                    throw new BusinessException("大于顺序号{0}的生产单{1}已经关闭。", orderSequence.ToString(), afterProductOrder.OrderNo);
                }
            }
            #endregion

            #region 检查生产单恢复后是否能够及时配送
            #region 查找工艺流程
            OrderDetail orderDetail = TryLoadOrderDetails(productOrder)[0];
            string routingCode = !string.IsNullOrWhiteSpace(orderDetail.Routing) ? orderDetail.Routing : productOrder.Routing;
            if (string.IsNullOrEmpty(routingCode))
            {
                throw new BusinessException("物料{0}没有找到对应的工艺流程。", orderDetail.Item);
            }
            RoutingMaster routingMaster = this.genericMgr.FindById<RoutingMaster>(routingCode);
            #endregion

            //#region 查询待恢复的Kit单和排序单
            //IList<OrderMaster> bindOrderList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where TraceCode = ? and OrderStrategy in {?, ?} and Status = ? and IsPause = ?",
            //                                        new object[] { productOrder.TraceCode, CodeMaster.FlowStrategy.KIT, CodeMaster.FlowStrategy.SEQ, CodeMaster.OrderStatus.Submit, true });
            //#endregion
            #region 查询待恢复订单（Kit单、排序单和底盘/驾驶室生产单）
            IList<OrderMaster> bindOrderList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where Type = ? and TraceCode = ? and IsPause = ? and OrderNo <> ?",
                                                    new object[] { productOrder.Type, productOrder.TraceCode, true, productOrder.OrderNo });
            #endregion

            IList<OrderMaster> relativeBindOrderList = null;
            if (bindOrderList != null && bindOrderList.Count > 0)
            {
                #region 查询大于等于或小于顺序号的绑定订单
                string hql = string.Empty;
                IList<object> paras = new List<object>();
                foreach (OrderMaster bindOrder in bindOrderList)
                {
                    if (hql == string.Empty)
                    {
                        hql = "from OrderMaster where Type = ? and Sequence = ? and Flow in (?";
                        if (afterProductOrder != null && afterProductOrder.Status != CodeMaster.OrderStatus.Create)
                        {
                            paras.Add(afterProductOrder.Type);
                            paras.Add(afterProductOrder.Sequence);
                        }
                        else
                        {
                            paras.Add(beforeProductOrder.Type);
                            paras.Add(beforeProductOrder.Sequence);
                        }
                    }
                    else
                    {
                        hql += ",?";
                    }
                    paras.Add(bindOrder.Flow);
                }
                hql += ")";

                relativeBindOrderList = this.genericMgr.FindAll<OrderMaster>(hql, paras);
                #endregion

                #region 循环校验KIT件和排序件能否及时配送
                foreach (OrderMaster bindOrder in bindOrderList)
                {
                    OrderMaster relativeBindOrder = relativeBindOrderList.Where(r => r.Flow == bindOrder.Flow).Single();  //一定能够找到匹配的KIT件或排序件

                    if (afterProductOrder != null && afterProductOrder.Status != CodeMaster.OrderStatus.Create)
                    {
                        #region 大于顺序号的排序件已经生成排序装箱单
                        if (relativeBindOrder.OrderStrategy == CodeMaster.FlowStrategy.SEQ
                            && relativeBindOrder.Status != CodeMaster.OrderStatus.Submit)
                        {
                            throw new BusinessException("大于顺序号{0}的排序单{1}已经在排序路线{2}生成排序装配单。", orderSequence.ToString(), relativeBindOrder.OrderNo, relativeBindOrder.Flow);
                        }
                        #endregion

                        #region 开始时间检查
                        //因为生产单恢复后将会替代后面一张生产单的位置，所有后面一张生产单排序件的开始时间就是本生产单恢复后的开始时间
                        if (!isForce && relativeBindOrder.StartTime < DateTime.Now)
                        {
                            if (relativeBindOrder.OrderStrategy == CodeMaster.FlowStrategy.KIT)
                            {
                                throw new BusinessException("生产单{0}调整至顺序号{1}后KIT路线{2}不能及时配送。", productOrder.OrderNo, orderSequence.ToString(), relativeBindOrder.Flow);
                            }
                            else
                            {
                                throw new BusinessException("生产单{0}调整至顺序号{1}后排序路线{2}不能及时配送。", productOrder.OrderNo, orderSequence.ToString(), relativeBindOrder.Flow);
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }
            #endregion

            #region 恢复生产单及KIT单和排序单顺序
            productOrder.IsPause = false;
            productOrder.PauseTime = null;
            productOrder.Sequence = orderSequence;

            #region 重新计算生产单开始时间和窗口时间和当前工位
            if (afterProductOrder != null && afterProductOrder.Status != CodeMaster.OrderStatus.Create)
            {
                //暂时不考虑把后续订单的窗口时间和当前工位往后递减
                productOrder.StartTime = afterProductOrder.StartTime;
                productOrder.WindowTime = afterProductOrder.WindowTime;
                productOrder.CurrentOperation = afterProductOrder.CurrentOperation;
            }
            else
            {
                //加节拍时间
                switch (routingMaster.TaktTimeUnit)
                {
                    case CodeMaster.TimeUnit.Second:
                        productOrder.StartTime = beforeProductOrder.StartTime.AddSeconds(routingMaster.TaktTime);
                        productOrder.WindowTime = beforeProductOrder.WindowTime.AddSeconds(routingMaster.TaktTime);
                        break;
                    case CodeMaster.TimeUnit.Minute:
                        productOrder.StartTime = beforeProductOrder.StartTime.AddMinutes(routingMaster.TaktTime);
                        productOrder.WindowTime = beforeProductOrder.WindowTime.AddMinutes(routingMaster.TaktTime);
                        break;
                    case CodeMaster.TimeUnit.Hour:
                        productOrder.StartTime = beforeProductOrder.StartTime.AddHours(routingMaster.TaktTime);
                        productOrder.WindowTime = beforeProductOrder.WindowTime.AddHours(routingMaster.TaktTime);
                        break;
                    case CodeMaster.TimeUnit.Day:
                        productOrder.StartTime = beforeProductOrder.StartTime.AddDays(routingMaster.TaktTime);
                        productOrder.WindowTime = beforeProductOrder.WindowTime.AddDays(routingMaster.TaktTime);
                        break;
                };

                if (beforeProductOrder.Status == CodeMaster.OrderStatus.Submit)
                {
                    productOrder.CurrentOperation = null;
                }
                else
                {
                    //查询已经回冲的最大工位
                    IList maxOpList = this.genericMgr.FindAll("select Max(Operation) as maxOp from OrderOperation where IsBackflush = ? and OrderNo = ?",
                                            new object[] { true, beforeProductOrder.OrderNo });

                    if (maxOpList != null && maxOpList.Count > 0 && maxOpList[0] != null)
                    {
                        productOrder.CurrentOperation = (int)maxOpList[0];
                    }
                }
            }
            #endregion

            this.genericMgr.Update(productOrder);

            foreach (OrderMaster bindOrder in bindOrderList)
            {
                bindOrder.IsPause = false;
                bindOrder.PauseTime = null;
                bindOrder.Sequence = orderSequence;

                OrderMaster relativeBindOrder = relativeBindOrderList.Where(r => r.Flow == bindOrder.Flow).Single();  //一定能够找到匹配的KIT件或排序件

                if (afterProductOrder != null && afterProductOrder.Status != CodeMaster.OrderStatus.Create)
                {
                    #region 根据大于等于指定顺序号的生产单恢复
                    bindOrder.StartTime = relativeBindOrder.StartTime;
                    bindOrder.WindowTime = relativeBindOrder.WindowTime;
                    #endregion
                }
                else
                {
                    #region 根据小于指定顺序号的生产单恢复
                    //加节拍时间
                    switch (routingMaster.TaktTimeUnit)
                    {
                        case CodeMaster.TimeUnit.Second:
                            bindOrder.StartTime = relativeBindOrder.StartTime.AddSeconds(routingMaster.TaktTime);
                            bindOrder.WindowTime = relativeBindOrder.WindowTime.AddSeconds(routingMaster.TaktTime);
                            break;
                        case CodeMaster.TimeUnit.Minute:
                            bindOrder.StartTime = relativeBindOrder.StartTime.AddMinutes(routingMaster.TaktTime);
                            bindOrder.WindowTime = relativeBindOrder.WindowTime.AddMinutes(routingMaster.TaktTime);
                            break;
                        case CodeMaster.TimeUnit.Hour:
                            bindOrder.StartTime = relativeBindOrder.StartTime.AddHours(routingMaster.TaktTime);
                            bindOrder.WindowTime = relativeBindOrder.WindowTime.AddHours(routingMaster.TaktTime);
                            break;
                        case CodeMaster.TimeUnit.Day:
                            bindOrder.StartTime = relativeBindOrder.StartTime.AddDays(routingMaster.TaktTime);
                            bindOrder.WindowTime = relativeBindOrder.WindowTime.AddDays(routingMaster.TaktTime);
                            break;
                    };
                    #endregion
                }

                this.genericMgr.Update(bindOrder);
            }
            #endregion

            #region 更新后续生产单的顺序
            if (afterProductOrder != null && afterProductOrder.Sequence == productOrder.Sequence)
            {
                this.genericMgr.FlushSession();
                User user = SecurityContextHolder.Get();
                this.genericMgr.FindAllWithNamedQuery("USP_Busi_UpdateSeq4RestoreVanOrder",
                    new object[] { productOrder.TraceCode, productOrder.OrderNo, orderSequence, user.Id, user.FullName });
                this.genericMgr.FlushSession();
            }
            #endregion

            throw new BusinessException();
        }
        #endregion

        #region 排序单创建
        private static Object CreatSequenceOrderLock = new Object();
        public IList<SequenceMaster> CreatSequenceOrder()
        {
            lock (CreatSequenceOrderLock)
            {
                log.Debug("Start generate sequence order.");
                #region 查询排序路线
                IList<FlowStrategy> flowStrategyList = this.queryMgr.FindAll<FlowStrategy>("from FlowStrategy where Strategy = ?", CodeMaster.FlowStrategy.SEQ);
                #endregion

                #region 查询待排序订单
                IList<object[]> orderDetailAryList = this.queryMgr.FindAllWithNamedQuery<object[]>("USP_Busi_GetWaitSeqDet");
                #endregion

                IList<SequenceMaster> sequenceMasterList = new List<SequenceMaster>();
                if (orderDetailAryList != null && orderDetailAryList.Count > 0)
                {
                    #region 循环创建排序单
                    foreach (FlowStrategy flowStrategy in flowStrategyList)
                    {
                        IList<object[]> thisFlowOrderDetailAryList = orderDetailAryList.Where(o => (string)o[1] == flowStrategy.Flow).OrderBy(o => (long)o[34]).ToList();
                        IList<SequenceMaster> thisSequenceMasterList = this.sequenceMgr.CreateSequenceOrderByFlow(flowStrategy, thisFlowOrderDetailAryList);
                        if (thisSequenceMasterList != null && thisSequenceMasterList.Count > 0)
                        {
                            ((List<SequenceMaster>)sequenceMasterList).AddRange(thisSequenceMasterList);
                        }
                    }
                    #endregion
                }

                log.Debug("Success generate sequence order.");

                //string loc = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.WMSAnjiRegion);

                //var sequenceOrderNos = sequenceMasterList.Where(s => s.PartyFrom.Equals(loc, StringComparison.OrdinalIgnoreCase))
                //    .Select(s => s.SequenceNo).ToList();

                //if (sequenceOrderNos.Count > 0)
                //{
                //    //AsyncRecourdMessageQueue(MethodNameType.CreateSeqOrder, sequenceOrderNos);
                //    foreach (var sequenceOrderNo in sequenceOrderNos)
                //    {
                //        this.CreateMessageQueue("CreateSeqOrder", sequenceOrderNo);
                //    }
                //}

                return sequenceMasterList;
            }
        }
        #endregion

        #region 排序单装箱
        [Transaction(TransactionMode.Requires)]
        public void PackSequenceOrder(string sequenceNo, IList<string> huIdList)
        {
            SequenceMaster sequenceMaster = this.genericMgr.FindById<SequenceMaster>(sequenceNo);
            PackSequenceOrder(sequenceMaster, huIdList);
        }

        [Transaction(TransactionMode.Requires)]
        public void PackSequenceOrder(SequenceMaster sequenceMaster, IList<string> huIdList)
        {
            #region 检查
            if (sequenceMaster.Status != CodeMaster.SequenceStatus.Submit)
            {
                throw new BusinessException("状态为{1}的排序装箱单{0}不能装箱。", sequenceMaster.SequenceNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.SequenceStatus, ((int)sequenceMaster.Status).ToString()));
            }

            if (huIdList == null || huIdList.Count == 0)
            {
                throw new BusinessException("排序装箱单{0}的装箱条码为空。", sequenceMaster.SequenceNo);
            }
            #endregion

            #region 库存占用
            IList<InventoryOccupy> inventoryOccupyList = (from huId in huIdList
                                                          select new InventoryOccupy
                                                          {
                                                              HuId = huId,
                                                              Location = sequenceMaster.LocationFrom,
                                                              QualityType = CodeMaster.QualityType.Qualified,
                                                              OccupyType = CodeMaster.OccupyType.Sequence,
                                                              OccupyReferenceNo = sequenceMaster.SequenceNo
                                                          }).ToList();

            IList<LocationLotDetail> locationLotDetailList = this.locationDetailMgr.InventoryOccupy(inventoryOccupyList);
            #endregion

            #region 排序单数量和扫描条码数量匹配
            BusinessException businessException = new BusinessException();
            TryLoadSequenceDetails(sequenceMaster);
            if (sequenceMaster.SequenceDetails.Where(d => !d.IsClose).Count() != huIdList.Count)  //过滤掉已经Close的排序件，因为已经暂停
            {
                businessException.AddMessage("扫描条码的数量和排序装箱单明细的数量不匹配。");
            }
            #endregion

            #region 条码匹配检查，按顺序检查排序件条码
            int i = 0;
            foreach (SequenceDetail sequenceDetail in sequenceMaster.SequenceDetails.Where(d => !d.IsClose).OrderBy(d => d.Sequence))
            {
                LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == huIdList[i]).Single();
                i++;
                if (sequenceDetail.Item != locationLotDetail.Item)
                {
                    businessException.AddMessage("排序装箱单序号{0}所需的零件为{1}，扫描的零件为{2}。", sequenceDetail.Sequence.ToString(), sequenceDetail.Item, locationLotDetail.Item);
                }
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            #region 更新排序装箱单头
            sequenceMaster.Status = CodeMaster.SequenceStatus.Pack;
            sequenceMaster.PackUserId = SecurityContextHolder.Get().Id;
            sequenceMaster.PackUserName = SecurityContextHolder.Get().FullName;
            sequenceMaster.PackDate = DateTime.Now;
            this.genericMgr.Update(sequenceMaster);
            #endregion

            #region 装箱操作
            i = 0;
            foreach (SequenceDetail sequenceDetail in sequenceMaster.SequenceDetails.Where(d => !d.IsClose))
            {
                LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == huIdList[i]).Single();
                i++;
                sequenceDetail.HuId = locationLotDetail.HuId;
                sequenceDetail.LotNo = locationLotDetail.LotNo;

                this.genericMgr.Update(sequenceDetail);
            }
            #endregion
        }
        #endregion

        #region 排序单装箱取消
        [Transaction(TransactionMode.Requires)]
        public void UnPackSequenceOrder(string sequenceNo)
        {
            UnPackSequenceOrder(this.genericMgr.FindById<SequenceMaster>(sequenceNo));
        }

        [Transaction(TransactionMode.Requires)]
        public void UnPackSequenceOrder(SequenceMaster sequenceMaster)
        {
            #region 检查
            if (sequenceMaster.Status != CodeMaster.SequenceStatus.Pack)
            {
                throw new BusinessException("状态为{1}的排序装箱单{0}不能取消装箱", sequenceMaster.SequenceNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.SequenceStatus, ((int)sequenceMaster.Status).ToString()));
            }
            #endregion

            #region 更新排序装箱单头
            sequenceMaster.Status = CodeMaster.SequenceStatus.Submit;
            this.genericMgr.Update(sequenceMaster);
            #endregion

            #region 更新排序装箱单明细
            TryLoadSequenceDetails(sequenceMaster);
            foreach (SequenceDetail sequenceDetail in sequenceMaster.SequenceDetails)
            {
                sequenceDetail.HuId = null;
                sequenceDetail.LotNo = null;

                this.genericMgr.Update(sequenceDetail);
            }
            #endregion

            #region 更新排序单
            string hql = string.Empty;
            IList<object> paras = new List<object>();
            foreach (SequenceDetail sequenceDetail in sequenceMaster.SequenceDetails)
            {
                if (hql == string.Empty)
                {
                    hql = "from OrderMaster where OrderNo in (?";
                }
                else
                {
                    hql += ", ?";
                }
                paras.Add(sequenceDetail.OrderNo);
            }
            hql += ")";

            foreach (OrderMaster orderMaster in this.genericMgr.FindAll<OrderMaster>(hql, paras.ToArray()))
            {
                orderMaster.Status = CodeMaster.OrderStatus.Submit;
                this.genericMgr.Update(orderMaster);
            }
            #endregion

            //释放库存暂用
            this.locationDetailMgr.CancelInventoryOccupy(CodeMaster.OccupyType.Sequence, sequenceMaster.SequenceNo);
        }
        #endregion

        #region 排序单发货
        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipSequenceOrder(string sequenceNo)
        {
            return ShipSequenceOrder(this.genericMgr.FindById<SequenceMaster>(sequenceNo), DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipSequenceOrder(string sequenceNo, DateTime effectiveDate)
        {
            return ShipSequenceOrder(this.genericMgr.FindById<SequenceMaster>(sequenceNo), effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipSequenceOrder(SequenceMaster sequenceMaster)
        {
            return ShipSequenceOrder(sequenceMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipSequenceOrder(SequenceMaster sequenceMaster, DateTime effectiveDate)
        {
            if (sequenceMaster.Status != CodeMaster.SequenceStatus.Pack)
            {
                throw new BusinessException("状态为{1}的排序装箱单{0}不能发货", sequenceMaster.SequenceNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.SequenceStatus, ((int)sequenceMaster.Status).ToString()));
            }

            #region 获取排序单明细
            TryLoadSequenceDetails(sequenceMaster);
            #endregion

            #region 获取订单头
            IList<OrderMaster> orderMasterList = TryLoadOrderMasters(sequenceMaster);
            #endregion

            #region 获取订单明细
            IList<OrderDetail> orderDetailList = TryLoadOrderDetails(sequenceMaster);
            #endregion

            #region 更新订单头状态
            foreach (OrderMaster orderMaster in orderMasterList)
            {
                if (orderMaster.Status == CodeMaster.OrderStatus.Submit)
                {
                    this.UpdateOrderMasterStatus2InProcess(orderMaster);
                }
            }
            #endregion

            #region 更新订单明细的发货数
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                orderDetail.ShippedQty += sequenceMaster.SequenceDetails.Where(p => p.OrderDetailId == orderDetail.Id).Sum(p => p.Qty);
                this.genericMgr.Update(orderDetail);
            }
            #endregion

            #region 更新排序单头
            sequenceMaster.Status = CodeMaster.SequenceStatus.Ship;
            sequenceMaster.ShipDate = DateTime.Now;
            sequenceMaster.ShipUserId = SecurityContextHolder.Get().Id;
            sequenceMaster.ShipUserName = SecurityContextHolder.Get().FullName;

            this.genericMgr.Update(sequenceMaster);
            #endregion

            #region 发货
            IpMaster ipMaster = ipMgr.TransferSequenceMaster2Ip(sequenceMaster);
            ipMgr.CreateIp(ipMaster, effectiveDate);
            #endregion

            #region 自动收货
            AutoReceiveIp(ipMaster, effectiveDate);
            #endregion

            return ipMaster;
        }
        #endregion

        #region 供应商排序发货
        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipSequenceOrderBySupplier(string sequenceNo)
        {
            return ShipSequenceOrderBySupplier(sequenceNo, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IpMaster ShipSequenceOrderBySupplier(string sequenceNo, DateTime effectiveDate)
        {
            SequenceMaster sequenceMaster = genericMgr.FindById<com.Sconit.Entity.ORD.SequenceMaster>(sequenceNo);

            bool IsShipScanHu = this.genericMgr.FindAll<bool>("select IsShipScanHu from FlowMaster where Code = ?", sequenceMaster.Flow).SingleOrDefault();

            if (IsShipScanHu)
            {
                throw new BusinessException("排序单{0}设置为发货扫描条码。", sequenceMaster.SequenceNo);
            }

            TryLoadSequenceDetails(sequenceMaster);

            sequenceMaster.Status = CodeMaster.SequenceStatus.Pack;
            sequenceMaster.PackUserId = SecurityContextHolder.Get().Id;
            sequenceMaster.PackUserName = SecurityContextHolder.Get().FullName;
            sequenceMaster.PackDate = effectiveDate;

            this.genericMgr.Update(sequenceMaster);

            return this.ShipSequenceOrder(sequenceMaster, effectiveDate);
        }
        #endregion

        #region 加载订单
        public OrderMaster LoadOrderMaster(string orderNo, bool includeDetail, bool includeOperation, bool includeBomDetail)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);

            if (includeDetail || includeOperation || includeBomDetail)
            {
                orderMaster.OrderDetails = this.genericMgr.FindAll<OrderDetail>("from OrderDetail o where o.OrderNo=?", orderNo);
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (includeBomDetail)
                    {
                        orderDetail.OrderBomDetails = this.genericMgr.FindAll<OrderBomDetail>("from OrderBomDetail o where o.OrderDetailId=?", orderDetail.Id);
                    }
                    if (includeOperation)
                    {
                        orderDetail.OrderOperations = this.genericMgr.FindAll<OrderOperation>("from OrderOperation o where o.OrderDetailId=?", orderDetail.Id);
                    }
                }
            }
            return orderMaster;
        }
        #endregion


        #region 根据订单号或者外部订单号加载订单
        public OrderMaster GetOrderMasterByOrderNoAndExtNo(string orderNo, bool includeDetail, bool includeOperation, bool includeBomDetail)
        {
            IList<OrderMaster> orderMasterList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster m where m.OrderNo = ? or m.ExternalOrderNo = ?", new string[] { orderNo, orderNo });

            OrderMaster orderMaster = orderMasterList.FirstOrDefault();
            if (orderMaster != null)
            {
                if (includeDetail || includeOperation || includeBomDetail)
                {
                    orderMaster.OrderDetails = this.genericMgr.FindAll<OrderDetail>("from OrderDetail o where o.OrderNo=?", orderMaster.OrderNo);
                    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                    {
                        if (includeBomDetail)
                        {
                            orderDetail.OrderBomDetails = this.genericMgr.FindAll<OrderBomDetail>("from OrderBomDetail o where o.OrderDetailId=?", orderDetail.Id);
                        }
                        if (includeOperation)
                        {
                            orderDetail.OrderOperations = this.genericMgr.FindAll<OrderOperation>("from OrderOperation o where o.OrderDetailId=?", orderDetail.Id);
                        }
                    }
                }
            }
            return orderMaster;
        }
        #endregion

        #region 根据投料的条码查找投料的工位
        public IList<OrderOperation> FindFeedOrderOperation(string orderNo, string huId)
        {
            throw null;
        }
        #endregion

        #region 路线可用性检查
        [Transaction(TransactionMode.Requires)]
        public void CheckOrder(string orderNo)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            this.CheckOrder(orderMaster);
        }

        [Transaction(TransactionMode.Requires)]
        public void CheckOrder(OrderMaster orderMaster)
        {
            BusinessException ex = new BusinessException();
            if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.CustomerGoods
                || orderMaster.Type == com.Sconit.CodeMaster.OrderType.Procurement)
            {
                ex.AddMessage("订单" + orderMaster.OrderNo + "无需检查库存");
            }
            else
            {
                var paramList = new List<object>();
                string sql = string.Empty;
                paramList.Add(orderMaster.LocationFrom);

                var itemQtyDic = new Dictionary<string, decimal>();
                if (orderMaster.Type == CodeMaster.OrderType.Production)
                {
                    TryLoadOrderBomDetails(orderMaster);
                    if (orderMaster.OrderDetails != null)
                    {
                        itemQtyDic = orderMaster.OrderDetails
                            .SelectMany(p => p.OrderBomDetails)
                            .GroupBy(p => p.Item).ToDictionary(d => d.Key, d => d.Sum(q => q.OrderedQty));
                    }

                    string hql = @"select distinct(isnull(d.LocFrom,f.LocFrom)) from Scm_FlowDet as d 
                               join SCM_FlowMstr f on d.Flow = f.Code
                               where f.IsActive = ? and (d.LocTo =? or (d.LocTo is null and f.LocTo = ? ))
                               and d.Item in(?  ";
                    var locations = genericMgr.FindAllWithNativeSqlIn<string>
                        (hql, itemQtyDic.Select(p => p.Key), new object[] { true, orderMaster.LocationFrom, orderMaster.LocationFrom });

                    foreach (var location in locations)
                    {
                        if (sql == string.Empty)
                        {
                            sql = @" select l.* from VIEW_LocationDet as l with(nolock) where l.ATPQty>0 and l.Location in(?,? ";
                        }
                        else
                        {
                            sql += ",?";
                        }
                    }
                    paramList.AddRange(locations);
                }
                else
                {
                    TryLoadOrderDetails(orderMaster);
                    if (orderMaster.OrderDetails != null)
                    {
                        itemQtyDic = orderMaster.OrderDetails
                            .GroupBy(p => p.Item).ToDictionary(d => d.Key, d => d.Sum(q => q.OrderedQty));
                    }
                    sql = @" select l.* from VIEW_LocationDet as l with(nolock) where l.ATPQty>0 and (l.Location =? ";
                }
                sql += @" ) and l.Item in(? ";

                IList<LocationDetailView> locationDetailViewList = this.genericMgr.FindEntityWithNativeSqlIn<LocationDetailView>
                    (sql, itemQtyDic.Select(p => p.Key), paramList);

                var invDic = (from p in locationDetailViewList
                              group p by p.Item into g
                              select new
                              {
                                  Item = g.Key,
                                  Qty = g.Sum(q => q.ATPQty)
                              }).ToDictionary(d => d.Item, d => d.Qty);
                foreach (var itemQty in itemQtyDic)
                {
                    var diffQty = itemQty.Value - invDic.ValueOrDefault(itemQty.Key);
                    if (diffQty > 0)
                    {
                        ex.AddMessage(string.Format("物料:{0}[{1}]没有足够的库存,缺口:{2}",
                            itemQty.Key, itemMgr.GetCacheItem(itemQty.Key).FullDescription, diffQty.ToString("0.##")));
                    }
                }
            }
            if (ex.GetMessages() != null && ex.GetMessages().Count > 0)
            {
                throw ex;
            }
        }

        private FlowMaster GetSourceFlow(string item, string location, IList<string> flowLocations)
        {
            FlowMaster sourceFlow = null;

            IList<FlowMaster> flowList = genericMgr.FindAll<FlowMaster>("from FlowMaster f where (f.Type = ? and f.IsManualCreateDetail = ?) or exists (select 1 from FlowDetail d where f.Code = d.Flow and d.Item = ? and (d.LocationTo = ? or (d.LocationTo is null and f.LocationTo = ?)) )", new object[] { (int)com.Sconit.CodeMaster.OrderType.Transfer, true, item, location, location });
            if (flowList != null && flowList.Count > 0)
            {
                //找到客供，采购，生产，委外都可以
                var q = flowList.Where(f => f.Type == com.Sconit.CodeMaster.OrderType.Procurement || f.Type == com.Sconit.CodeMaster.OrderType.CustomerGoods || f.Type == com.Sconit.CodeMaster.OrderType.Production || f.Type == com.Sconit.CodeMaster.OrderType.SubContract);
                if (q.ToList() != null && q.ToList().Count > 0)
                {
                    sourceFlow = q.ToList().First();
                }
                else
                {
                    flowLocations.Add(location);
                    foreach (FlowMaster flow in flowList)
                    {
                        if (!flowLocations.Contains(flow.LocationFrom))
                        {
                            sourceFlow = GetSourceFlow(item, flow.LocationFrom, flowLocations);
                        }
                        if (sourceFlow != null)
                        {
                            break;
                        }
                    }
                }
            }

            return sourceFlow;
        }
        #endregion

        #region 创建不合格品退货单
        [Transaction(TransactionMode.Requires)]
        public OrderMaster CreateReturnOrder(FlowMaster flowMaster, IList<RejectDetail> rejectDetailList)
        {
            //if (!Utility.SecurityHelper.HasPermission(flowMaster))
            //{
            //    throw new BusinessException("没有此路线{0}的操作权限。", flowMaster.Code);
            //}

            if (rejectDetailList == null || rejectDetailList.Count == 0)
            {
                throw new BusinessException("退货明细不能为空.");
            }

            #region 根据路线生成退货单
            FlowMaster returnFlow = flowMgr.GetReverseFlow(flowMaster, rejectDetailList.Select(r => r.Item).Distinct().ToList());
            OrderMaster orderMaster = TransferFlow2Order(returnFlow, null);
            orderMaster.IsQuick = true;
            orderMaster.SubType = CodeMaster.OrderSubType.Return;
            orderMaster.WindowTime = DateTime.Now;
            orderMaster.StartTime = DateTime.Now;
            orderMaster.EffectiveDate = DateTime.Now;
            orderMaster.QualityType = CodeMaster.QualityType.Reject;
            orderMaster.ReferenceOrderNo = rejectDetailList[0].RejectNo;
            #endregion

            #region 不合格品单明细变成退货单明细
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();
            foreach (RejectDetail rejectDetail in rejectDetailList)
            {
                if (rejectDetail.HandleQty < rejectDetail.HandledQty + rejectDetail.CurrentHandleQty)
                {
                    throw new BusinessException("不合格品单物料{0}本次处理数加已处理数超过总处理数.", rejectDetail.Item);
                }

                if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0)
                {
                    var q = orderMaster.OrderDetails.Where(o => o.Item == rejectDetail.Item).ToList();
                    if (q.ToList() == null || q.ToList().Count == 0)
                    {
                        throw new BusinessException("不合格品单物料{0}没找到对应的路线明细.", rejectDetail.Item);
                    }
                    OrderDetail orderDetail = Mapper.Map<OrderDetail, OrderDetail>(q.ToList().First());
                    orderDetail.QualityType = com.Sconit.CodeMaster.QualityType.Reject;
                    orderDetail.Uom = rejectDetail.Uom;
                    if (string.IsNullOrWhiteSpace(rejectDetailList.First().HuId))
                    {
                        orderDetail.OrderedQty = rejectDetail.CurrentHandleQty;
                    }
                    orderDetail.LocationFrom = orderMaster.LocationFrom;
                    //orderDetail.LocationFromName = genericMgr.FindById<Location>(rejectDetail.LocationFrom).Name;
                    orderDetail.ManufactureParty = rejectDetail.ManufactureParty;
                    orderDetailList.Add(orderDetail);
                }
                else
                {
                    throw new BusinessException("不合格品单物料{0}没找到对应的路线明细.", rejectDetail.Item);
                }
            }
            if (!string.IsNullOrWhiteSpace(rejectDetailList.First().HuId))
            {
                AddHuToOrderDetailInput(orderDetailList, rejectDetailList.Select(p => p.HuId).Distinct().ToList());
            }
            #endregion

            #region 创建退货单
            orderMaster.OrderDetails = orderDetailList;
            CreateOrder(orderMaster);
            #endregion

            #region 更新明细的已处理不合格品处理单状态
            if (orderMaster.Type == CodeMaster.OrderType.CustomerGoods ||
                orderMaster.Type == CodeMaster.OrderType.SubContract ||
                orderMaster.Type == CodeMaster.OrderType.Procurement)
            {
                foreach (RejectDetail rejectDetail in rejectDetailList)
                {
                    rejectDetail.HandledQty += rejectDetail.CurrentHandleQty;
                    genericMgr.Update(rejectDetail);
                }
                string hql = "from RejectDetail as r where r.RejectNo = ?";
                IList<RejectDetail> remainRejectDetailList = genericMgr.FindAll<RejectDetail>(hql, rejectDetailList[0].RejectNo);
                var m = remainRejectDetailList.Where(r => (r.HandledQty < r.HandleQty)).ToList();
                if (m == null || m.Count == 0)
                {
                    RejectMaster rejectMaster = genericMgr.FindById<RejectMaster>(rejectDetailList[0].RejectNo);
                    rejectMaster.Status = CodeMaster.RejectStatus.Close;
                    genericMgr.Update(rejectMaster);
                }
            }
            #endregion

            return orderMaster;
        }

        [Transaction(TransactionMode.Requires)]
        public OrderMaster CreateReturnOrder(FlowMaster flowMaster, IList<InspectResult> inspectResultList)
        {
            //if (!Utility.SecurityHelper.HasPermission(flowMaster))
            //{
            //    throw new BusinessException("没有此路线{0}的操作权限。", flowMaster.Code);
            //}
            if (inspectResultList == null || inspectResultList.Count == 0)
            {
                throw new BusinessException("退货明细不能为空.");
            }

            #region 根据路线生成退货单
            FlowMaster returnFlow = flowMgr.GetReverseFlow(flowMaster, inspectResultList.Select(r => r.Item).Distinct().ToList());
            OrderMaster orderMaster = TransferFlow2Order(returnFlow, null);
            orderMaster.IsQuick = true;
            orderMaster.SubType = CodeMaster.OrderSubType.Return;
            orderMaster.WindowTime = DateTime.Now;
            orderMaster.StartTime = DateTime.Now;
            orderMaster.EffectiveDate = DateTime.Now;
            orderMaster.QualityType = CodeMaster.QualityType.Reject;
            orderMaster.ReferenceOrderNo = inspectResultList[0].InspectNo;
            #endregion

            #region 不合格品单明细变成退货单明细
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();
            foreach (var inspectResult in inspectResultList)
            {
                if (inspectResult.JudgeQty < inspectResult.HandleQty + inspectResult.CurrentHandleQty)
                {
                    throw new BusinessException("不合格品物料{0}本次处理数加已处理数超过总处理数.", inspectResult.Item);
                }

                if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0)
                {
                    var q = orderMaster.OrderDetails.Where(o => o.Item == inspectResult.Item).ToList();
                    if (q.ToList() == null || q.ToList().Count == 0)
                    {
                        throw new BusinessException("不合格品物料{0}没找到对应的路线明细.", inspectResult.Item);
                    }
                    OrderDetail orderDetail = Mapper.Map<OrderDetail, OrderDetail>(q.ToList().First());
                    orderDetail.QualityType = com.Sconit.CodeMaster.QualityType.Reject;
                    orderDetail.Uom = inspectResult.Uom;
                    if (string.IsNullOrWhiteSpace(inspectResultList.First().HuId))
                    {
                        orderDetail.OrderedQty = inspectResult.CurrentHandleQty;
                    }
                    orderDetail.LocationFrom = orderMaster.LocationFrom;
                    //orderDetail.LocationFromName = genericMgr.FindById<Location>(inspectResult.CurrentLocation).Name;
                    orderDetail.ManufactureParty = inspectResult.ManufactureParty;
                    orderDetailList.Add(orderDetail);
                }
                else
                {
                    throw new BusinessException("不合格品物料{0}没找到对应的路线明细.", inspectResult.Item);
                }
            }
            if (!string.IsNullOrWhiteSpace(inspectResultList.First().HuId))
            {
                AddHuToOrderDetailInput(orderDetailList, inspectResultList.Select(p => p.HuId).Distinct().ToList());
            }

            #endregion

            #region 创建退货单
            orderMaster.OrderDetails = orderDetailList;
            CreateOrder(orderMaster);
            #endregion

            #region 更新明细状态
            if (orderMaster.Type == CodeMaster.OrderType.CustomerGoods ||
                orderMaster.Type == CodeMaster.OrderType.SubContract ||
                orderMaster.Type == CodeMaster.OrderType.Procurement)
            {
                foreach (var inspectResult in inspectResultList)
                {
                    inspectResult.HandleQty += inspectResult.CurrentHandleQty;
                    if (inspectResult.HandleQty == inspectResult.JudgeQty)
                    {
                        inspectResult.IsHandle = true;
                    }
                    genericMgr.Update(inspectResult);
                }
            }
            #endregion
            return orderMaster;
        }

        private void AddHuToOrderDetailInput(IList<OrderDetail> orderDetailList, IList<string> huIdList)
        {
            IList<HuStatus> huStatusList = huMgr.GetHuStatus(huIdList);
            IList<HuStatus> notInLocHu = huStatusList.Where(h => h.Status != CodeMaster.HuStatus.Location).ToList();
            if (notInLocHu != null && notInLocHu.Count > 0)
            {
                string strExeception = string.Empty;
                foreach (HuStatus hs in notInLocHu)
                {
                    if (string.IsNullOrEmpty(strExeception))
                        strExeception = "条码" + hs.HuId;
                    else
                        strExeception += "," + hs.HuId;

                }
                strExeception += "不在库存中";
                throw new BusinessException(strExeception);
            }

            foreach (HuStatus huStatus in huStatusList)
            {
                var h = orderDetailList.Where(o => o.Item == huStatus.Item && o.Uom == huStatus.Uom && o.LocationFrom == huStatus.Location).ToList();
                if (h == null || h.Count == 0)
                {
                    OrderDetail od = new OrderDetail();
                    od.Item = huStatus.Item;
                    od.Uom = huStatus.Uom;
                    od.UnitCount = huStatus.UnitCount;
                    od.ItemDescription = huStatus.ItemDescription;
                    od.ReferenceItemCode = huStatus.ReferenceItemCode;
                    od.LocationFrom = huStatus.Location;
                    od.OrderedQty = huStatus.Qty;

                    IList<OrderDetailInput> orderDetailInputList = new List<OrderDetailInput>();
                    OrderDetailInput orderDetailInput = new OrderDetailInput();
                    orderDetailInput.HuId = huStatus.HuId;
                    orderDetailInput.HuQty = huStatus.Qty;
                    orderDetailInput.LotNo = huStatus.LotNo;
                    orderDetailInput.ReceiveQty = huStatus.Qty;
                    orderDetailInputList.Add(orderDetailInput);

                    od.OrderDetailInputs = orderDetailInputList;
                    orderDetailList.Add(od);
                }
                else
                {
                    OrderDetail od = h[0];
                    od.OrderedQty += huStatus.Qty;

                    OrderDetailInput orderDetailInput = new OrderDetailInput();
                    orderDetailInput.HuId = huStatus.HuId;
                    orderDetailInput.HuQty = huStatus.Qty;
                    orderDetailInput.LotNo = huStatus.LotNo;
                    orderDetailInput.ReceiveQty = huStatus.Qty;
                    od.OrderDetailInputs.Add(orderDetailInput);
                }
            }
        }
        #endregion

        #region 创建不合格品移库单
        [Transaction(TransactionMode.Requires)]
        public void CreateRejectTransfer(Location location, IList<RejectDetail> rejectDetailList)
        {
            var orderMaster = new Entity.ORD.OrderMaster();
            var rejectNoList = (from rej in rejectDetailList select rej.RejectNo).Distinct().ToList();
            if (rejectNoList.Count() > 1)
            {
                throw new BusinessException("多个不合格品处理单不能合并移库。");
            }

            RejectMaster rejectMaster = genericMgr.FindById<RejectMaster>(rejectNoList[0]);

            var locationFrom = this.genericMgr.FindById<Entity.MD.Location>(rejectDetailList[0].LocationFrom);
            var partyFrom = this.genericMgr.FindById<Entity.MD.Party>(locationFrom.Region);
            var partyTo = this.genericMgr.FindById<Entity.MD.Party>(location.Region);

            orderMaster.LocationFrom = locationFrom.Code;
            orderMaster.IsShipScanHu = rejectMaster.InspectType == com.Sconit.CodeMaster.InspectType.Barcode;
            orderMaster.IsReceiveScanHu = rejectMaster.InspectType == com.Sconit.CodeMaster.InspectType.Barcode;
            orderMaster.LocationFromName = locationFrom.Name;
            orderMaster.LocationTo = location.Code;
            orderMaster.LocationToName = location.Name;
            orderMaster.PartyFrom = partyFrom.Code;
            orderMaster.PartyFromName = partyFrom.Name;
            orderMaster.PartyTo = partyTo.Code;
            orderMaster.PartyToName = partyTo.Name;
            orderMaster.Type = CodeMaster.OrderType.Transfer;
            orderMaster.StartTime = DateTime.Now;
            orderMaster.WindowTime = DateTime.Now;
            orderMaster.EffectiveDate = DateTime.Now;
            orderMaster.QualityType = com.Sconit.CodeMaster.QualityType.Reject;

            orderMaster.IsQuick = true;
            orderMaster.OrderDetails = new List<OrderDetail>();
            int seq = 1;

            var groupRejectDetailList = from r in rejectDetailList group r by new { r.Item, r.CurrentLocation } into result select result;


            foreach (var rejectDetail in groupRejectDetailList)
            {
                var currentRejectDetailList = rejectDetailList.Where(p => p.Item == rejectDetail.Key.Item && p.CurrentLocation == rejectDetail.Key.CurrentLocation).ToList();

                var orderDetail = new OrderDetail();
                var orderDetailInputList = new List<OrderDetailInput>();
                Mapper.Map(currentRejectDetailList[0], orderDetail);
                orderDetail.OrderType = com.Sconit.CodeMaster.OrderType.Transfer;
                orderDetail.QualityType = com.Sconit.CodeMaster.QualityType.Inspect;
                orderDetail.LocationFrom = rejectDetail.Key.CurrentLocation;
                orderDetail.LocationFromName = genericMgr.FindById<Location>(rejectDetail.Key.CurrentLocation).Name;
                orderDetail.LocationTo = location.Code;
                orderDetail.LocationToName = location.Name;
                orderDetail.Sequence = seq++;

                foreach (RejectDetail rej in currentRejectDetailList)
                {
                    var orderDetailInput = new OrderDetailInput();
                    if (rejectMaster.InspectType == com.Sconit.CodeMaster.InspectType.Barcode)
                    {
                        orderDetailInput.HuId = rej.HuId;
                        orderDetailInput.LotNo = rej.LotNo;
                    }

                    orderDetailInput.QualityType = com.Sconit.CodeMaster.QualityType.Reject;
                    orderDetailInput.ReceiveQty = rej.TobeHandleQty;

                    orderDetail.RequiredQty += rej.TobeHandleQty;
                    orderDetail.OrderedQty += rej.TobeHandleQty;
                    orderDetailInputList.Add(orderDetailInput);

                }
                orderDetail.OrderDetailInputs = orderDetailInputList;
                orderMaster.OrderDetails.Add(orderDetail);
            }

            CreateOrder(orderMaster);

            #region 更新检验明细
            foreach (RejectDetail rej in rejectDetailList)
            {
                rej.CurrentLocation = location.Code;
                genericMgr.Update(rej);
            }
            #endregion
        }
        #endregion

        #region 创建待验品移库单
        [Transaction(TransactionMode.Requires)]
        public void CreateInspectTransfer(Location location, IList<InspectDetail> inspectDetailList)
        {
            var orderMaster = new Entity.ORD.OrderMaster();
            var inspectNoList = (from inp in inspectDetailList select inp.InspectNo).Distinct().ToList();
            if (inspectNoList.Count() > 1)
            {
                throw new BusinessException("多个报验单待验明细不能合并移库。");
            }

            InspectMaster inspectMaster = genericMgr.FindById<InspectMaster>(inspectNoList[0]);

            var locationFrom = this.genericMgr.FindById<Entity.MD.Location>(inspectDetailList[0].LocationFrom);
            var partyFrom = this.genericMgr.FindById<Entity.MD.Party>(locationFrom.Region);
            var partyTo = this.genericMgr.FindById<Entity.MD.Party>(location.Region);

            orderMaster.LocationFrom = locationFrom.Code;
            orderMaster.IsShipScanHu = inspectMaster.Type == com.Sconit.CodeMaster.InspectType.Barcode;
            orderMaster.IsReceiveScanHu = inspectMaster.Type == com.Sconit.CodeMaster.InspectType.Barcode;
            orderMaster.LocationFromName = locationFrom.Name;
            orderMaster.LocationTo = location.Code;
            orderMaster.LocationToName = location.Name;
            orderMaster.PartyFrom = partyFrom.Code;
            orderMaster.PartyFromName = partyFrom.Name;
            orderMaster.PartyTo = partyTo.Code;
            orderMaster.PartyToName = partyTo.Name;
            orderMaster.Type = CodeMaster.OrderType.Transfer;
            orderMaster.StartTime = DateTime.Now;
            orderMaster.WindowTime = DateTime.Now;
            orderMaster.EffectiveDate = DateTime.Now;
            orderMaster.QualityType = com.Sconit.CodeMaster.QualityType.Inspect;

            orderMaster.IsQuick = true;
            orderMaster.OrderDetails = new List<OrderDetail>();
            int seq = 1;

            var groupInspectDetailList = from d in inspectDetailList group d by new { d.Item, d.CurrentLocation } into result select result;


            foreach (var inspectDetail in groupInspectDetailList)
            {
                var currentInspectDetailList = inspectDetailList.Where(p => p.Item == inspectDetail.Key.Item && p.CurrentLocation == inspectDetail.Key.CurrentLocation).ToList();

                var orderDetail = new OrderDetail();
                var orderDetailInputList = new List<OrderDetailInput>();
                Mapper.Map(currentInspectDetailList[0], orderDetail);
                orderDetail.OrderType = com.Sconit.CodeMaster.OrderType.Transfer;
                orderDetail.QualityType = com.Sconit.CodeMaster.QualityType.Inspect;
                orderDetail.LocationFrom = inspectDetail.Key.CurrentLocation;
                orderDetail.LocationFromName = genericMgr.FindById<Location>(inspectDetail.Key.CurrentLocation).Name;
                orderDetail.LocationTo = location.Code;
                orderDetail.LocationToName = location.Name;
                orderDetail.Sequence = seq++;

                foreach (InspectDetail insp in currentInspectDetailList)
                {
                    var orderDetailInput = new OrderDetailInput();
                    if (inspectMaster.Type == com.Sconit.CodeMaster.InspectType.Barcode)
                    {
                        orderDetailInput.HuId = insp.HuId;
                        orderDetailInput.LotNo = insp.LotNo;
                    }

                    orderDetailInput.QualityType = com.Sconit.CodeMaster.QualityType.Inspect;
                    orderDetailInput.OccupyType = com.Sconit.CodeMaster.OccupyType.Inspect;
                    orderDetailInput.OccupyReferenceNo = inspectMaster.InspectNo;
                    orderDetailInput.ReceiveQty = insp.CurrentTransferQty;

                    orderDetail.RequiredQty += insp.CurrentTransferQty;
                    orderDetail.OrderedQty += insp.CurrentTransferQty;
                    orderDetailInputList.Add(orderDetailInput);

                }
                orderDetail.OrderDetailInputs = orderDetailInputList;
                orderMaster.OrderDetails.Add(orderDetail);
            }


            CreateOrder(orderMaster);

            #region 更新检验明细
            foreach (InspectDetail insp in inspectDetailList)
            {
                insp.CurrentLocation = location.Code;
                genericMgr.Update(insp);
            }
            #endregion
        }
        #endregion

        #region 生产单手工拉料
        [Transaction(TransactionMode.Requires)]
        public string[] CreateRequisitionList(string orderNo)
        {
            return this.CreateRequisitionList(this.genericMgr.FindById<OrderMaster>(orderNo));
        }

        [Transaction(TransactionMode.Requires)]
        //返回2个列表，1是生成的订单，2是没拉出来的物料
        public string[] CreateRequisitionList(OrderMaster orderMaster)
        {
            string orderString = string.Empty;
            string itemString = string.Empty;

            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Submit && orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.InProcess)
            {
                throw new BusinessException("状态为{1}的试制生产单{0}不能产生拉料单。", orderMaster.OrderNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, (int)orderMaster.Status));
            }

            #region 去掉KB件，保存到表里，供查询
            string kbCountSql = "select count(*) as count from ORD_OrderBomdet as b inner join ORD_KBOrderBomDet as k on b.Id = k.OrderBomDetId where b.OrderNo = ?";
            IList<object> kbBomDetailCount = genericMgr.FindAllWithNativeSql<object>(kbCountSql, new object[] { orderMaster.OrderNo });
            if ((int)kbBomDetailCount[0] == 0)
            {
                string kbSql = "select d.Id,A.Flow from ORD_OrderBomdet as d inner join (select f.Item,f.Flow,case when f.LocTo is null then m.LocTo else f.LocTo end as LocTo from  SCM_FlowDet as f  inner join SCM_FlowMstr as m on f.Flow = m.Code where  m.FlowStrategy = ? and f.StartDate < ? and (f.EndDate is null or f.EndDate > ?))A on d.Item = A.Item and d.Location = A.LocTo where d.OrderNo = ?";
                IList<object[]> kbBomDetailList = genericMgr.FindAllWithNativeSql<object[]>(kbSql, new object[] { (int)com.Sconit.CodeMaster.FlowStrategy.KB, DateTime.Now, DateTime.Now, orderMaster.OrderNo });
                if (kbBomDetailList.Count > 0)
                {
                    foreach (object[] ob in kbBomDetailList)
                    {
                        KBOrderBomDetail kbOrderBomDetail = new KBOrderBomDetail();
                        kbOrderBomDetail.OrderBomDetId = (int)ob[0];
                        kbOrderBomDetail.Flow = (string)ob[1];
                        genericMgr.Create(kbOrderBomDetail);
                    }
                }
            }

            #endregion

            #region 得出去掉KB件的其他件
            string bomDetailSql = "select d.Item,d.Location,d.Uom,d.OrderQty from ORD_OrderBomdet as d where d.OrderNo = ? and not exists (select 1 from  SCM_FlowDet as f  inner join SCM_FlowMstr as m on f.Flow = m.Code where d.Item = f.Item and  ((f.LocTo is not null and d.Location = f.LocTo) or (f.LocTo is null and d.Location = m.LocTo)) and m.FlowStrategy = ?)";
            IList<object[]> orderBomDetailList = genericMgr.FindAllWithNativeSql<object[]>(bomDetailSql, new object[] { orderMaster.OrderNo, (int)com.Sconit.CodeMaster.FlowStrategy.KB });
            // IList<OrderBomDetail> orderBomDetailList = TryLoadOrderBomDetails(orderMaster);
            #endregion

            #region 过滤掉负的
            var groupOrderBomDetailList = (from det in orderBomDetailList
                                           where (decimal)det[3] > 0
                                           group det by new { Item = (string)det[0], Location = (string)det[1], Uom = (string)det[2] } into result
                                           select new OrderBomDetail
                                           {
                                               Item = result.Key.Item,
                                               Location = result.Key.Location,
                                               Uom = result.Key.Uom,
                                               OrderedQty = result.Sum(t => (decimal)t[3])
                                           }).ToList();
            #endregion

            #region 已经拉过料的
            IList<OrderDetail> orderDetailList = genericMgr.FindAll<OrderDetail>("from OrderDetail as d where exists (select 1 from OrderMaster as m where m.OrderNo = d.OrderNo and m.Type = ? and m.ReferenceOrderNo = ?)", new object[] { (int)com.Sconit.CodeMaster.OrderType.Transfer, orderMaster.OrderNo });

            var groupOrderDetailList = (from det in orderDetailList
                                        group det by new { Item = det.Item, Location = det.LocationTo, Uom = det.Uom } into result
                                        select new OrderBomDetail
                                        {
                                            Item = result.Key.Item,
                                            Location = result.Key.Location,
                                            OrderedQty = result.Sum(t => (t.OrderedQty * t.UnitQty))
                                        }).ToList();
            #endregion

            #region 求出实际需求
            var exactOrderBomDetailList = (from b in groupOrderBomDetailList
                                           join d in groupOrderDetailList
                                           on new
                                           {
                                               Location = b.Location,
                                               Item = b.Item
                                           }
                                               equals
                                               new
                                               {
                                                   Location = d.Location,
                                                   Item = d.Item
                                               }
                                           into bd
                                           from result in bd.DefaultIfEmpty()
                                           select new OrderBomDetail
                                           {
                                               Location = b.Location,
                                               Item = b.Item,
                                               Uom = b.Uom,
                                               OrderedQty = b.OrderedQty - (result != null ? result.OrderedQty * itemMgr.ConvertItemUomQty(b.Item, genericMgr.FindById<Item>(result.Item).Uom, 1, b.Uom) : 0)
                                           }).ToList().Where(p => p.OrderedQty > 0);

            if (exactOrderBomDetailList == null || exactOrderBomDetailList.Count() == 0)
            {
                throw new BusinessException("试制车生产单{0}物料清单为空。", orderMaster.OrderNo);
            }
            #endregion

            #region 把需求按库位分一下，应该会少一些
            IList<OrderMaster> orderMasterList = new List<OrderMaster>();
            var locationList = exactOrderBomDetailList.Select(b => b.Location).Distinct().ToList();
            foreach (string location in locationList)
            {
                FlowMaster transferFlow = null;
                FlowDetail transferFlowDetail = null;
                bool isShipScanHu = false;
                bool isReceiveScanHu = false;

                string hql = "from FlowDetail as d where exists (select 1 from FlowMaster as f where f.Code = d.Flow and f.Type = ? and f.IsActive = ? and (d.LocationTo = ? or (d.LocationTo is null and f.LocationTo = ?)))";
                IList<FlowDetail> transferFlowDetailList = genericMgr.FindAll<FlowDetail>(hql, new object[] { com.Sconit.CodeMaster.OrderType.Transfer, true, location, location });

                var lob = exactOrderBomDetailList.Where(p => p.Location == location).ToList(); //发到此库位的bom明细
                var nlob = new List<OrderBomDetail>();                                        //没找到路线明细的orderbomdet
                #region 找到有路线明细的
                foreach (OrderBomDetail orderBomDetail in lob)
                {
                    transferFlowDetail = transferFlowDetailList.Where(f => f.Item == orderBomDetail.Item).ToList().FirstOrDefault();
                    if (transferFlowDetail != null)
                    {
                        transferFlow = genericMgr.FindById<FlowMaster>(transferFlowDetail.Flow);
                        isShipScanHu = transferFlow.IsShipScanHu;
                        isReceiveScanHu = transferFlow.IsReceiveScanHu;

                        if (transferFlow.IsAutoCreate && transferFlowDetail.IsAutoCreate)
                        {
                            #region 自动拉料的不需要拉
                            continue;
                            #endregion
                        }

                        #region 建订单
                        OrderMaster transferOrderMaster = orderMasterList.Where(o => o.Flow == transferFlow.Code && o.IsShipScanHu == isShipScanHu && o.IsReceiveScanHu == isReceiveScanHu).ToList().SingleOrDefault<OrderMaster>();
                        if (transferOrderMaster == null)
                        {
                            OrderMaster productionOrder = genericMgr.FindById<OrderMaster>(orderMaster.OrderNo);
                            transferOrderMaster = TransferFlow2Order(transferFlow, null);
                            transferOrderMaster.ReferenceOrderNo = orderMaster.OrderNo;
                            transferOrderMaster.StartTime = DateTime.Now;
                            transferOrderMaster.WindowTime = DateTime.Now;
                            transferOrderMaster.TraceCode = productionOrder.TraceCode;
                            transferOrderMaster.IsShipScanHu = isShipScanHu;
                            transferOrderMaster.IsReceiveScanHu = isReceiveScanHu;
                            transferOrderMaster.IsOrderFulfillUC = false;
                            transferOrderMaster.IsShipFulfillUC = false;
                            transferOrderMaster.IsReceiveFulfillUC = false;
                            if (transferOrderMaster.OrderDetails == null)
                            {
                                transferOrderMaster.OrderDetails = new List<OrderDetail>();
                            }

                            orderMasterList.Add(transferOrderMaster);
                        }

                        OrderDetail orderDetail = Mapper.Map<OrderDetail, OrderDetail>(transferOrderMaster.OrderDetails.Where(d => d.Item == orderBomDetail.Item && (d.LocationTo == orderBomDetail.Location || (d.LocationTo == null && transferOrderMaster.LocationTo == orderBomDetail.Location))).First());
                        if (orderDetail.Uom != orderBomDetail.Uom)
                        {
                            orderDetail.UnitQty = this.itemMgr.ConvertItemUomQty(orderBomDetail.Item, orderDetail.Uom, 1, orderBomDetail.Uom);
                            orderDetail.OrderedQty = orderBomDetail.OrderedQty / orderDetail.UnitQty;
                        }
                        else
                        {
                            orderDetail.UnitQty = 1;
                            orderDetail.OrderedQty = orderBomDetail.OrderedQty;
                        }
                        transferOrderMaster.OrderDetails.Add(orderDetail);
                        #endregion
                    }
                    else
                    {
                        nlob.Add(orderBomDetail);
                    }
                }
                #endregion

                #region 没有路线明细的
                if (nlob.Count > 0)
                {
                    #region
                    //根据采购路线查找生产单BOM物料的采购入库地点，作为来源库位，取生产单BOM的库位为目的库位，生成要货单。
                    //因为一条路线上可能包含关键件和非关键件,收货入库可能条码也可能数量
                    //发货是否扫描条码要跟据采购路线的收货扫描条码选项
                    //收货是否扫描条码要根据是否关键件（itemtrace）
                    foreach (OrderBomDetail orderBomDetail in nlob)
                    {
                        //采购的以头上的为准
                        string procuremenSql = "select case when d.LocTo is null then m.LocTo else d.LocTo end as LocTo,m.IsRecScanHu from SCM_FlowDet as d inner join SCM_FlowMstr as m on d.Flow = m.Code  where m.Type = ? and m.IsActive = ? and d.Item = ?";
                        IList<object[]> procurementFlowList = genericMgr.FindAllWithNativeSql<object[]>(procuremenSql, new object[] { com.Sconit.CodeMaster.OrderType.Procurement, true, orderBomDetail.Item });
                        if (procurementFlowList == null || procurementFlowList.Count == 0)
                        {
                            // throw new BusinessException("找不到物料{0}对应的采购路线", orderBomDetail.Item);
                            itemString += string.IsNullOrEmpty(itemString) ? orderBomDetail.Item : "," + orderBomDetail.Item;
                            continue;
                        }
                        object[] procurementFlow = procurementFlowList[0];
                        hql = "from FlowMaster as f where f.Type = ? and  f.LocationFrom = ? and f.LocationTo = ? and f.IsActive = ? and f.IsAutoCreate = ?";
                        IList<FlowMaster> transferFlowList = genericMgr.FindAll<FlowMaster>(hql, new object[] { com.Sconit.CodeMaster.OrderType.Transfer, (string)procurementFlow[0], orderBomDetail.Location, true, false });
                        if (transferFlowList == null || transferFlowList.Count == 0)
                        {
                            // throw new BusinessException("找不到物料{0}对应的来源库位{1},目的库位{2}的移库路线", orderBomDetail.Item);
                            itemString += string.IsNullOrEmpty(itemString) ? (string)procurementFlow[0] + ":" + orderBomDetail.Item : "," + (string)procurementFlow[0] + ":" + orderBomDetail.Item;
                            continue;
                        }

                        transferFlow = transferFlowList[0];
                        isShipScanHu = (bool)procurementFlow[1];
                        //IList<ItemTrace> itemTraceList = genericMgr.FindAll<ItemTrace>("from ItemTrace as i where i.Item = ?", orderBomDetail.Item);
                        //isReceiveScanHu = (itemTraceList == null || itemTraceList.Count() == 0) ? false : true;

                        #region 建订单
                        OrderMaster transferOrderMaster = orderMasterList.Where(o => o.Flow == transferFlow.Code && o.IsShipScanHu == isShipScanHu && o.IsReceiveScanHu == isReceiveScanHu).ToList().SingleOrDefault<OrderMaster>();
                        if (transferOrderMaster == null)
                        {
                            OrderMaster productionOrder = genericMgr.FindById<OrderMaster>(orderMaster.OrderNo);
                            transferOrderMaster = TransferFlow2Order(transferFlow, null);
                            transferOrderMaster.ReferenceOrderNo = orderMaster.OrderNo;
                            transferOrderMaster.StartTime = DateTime.Now;
                            transferOrderMaster.WindowTime = DateTime.Now;
                            transferOrderMaster.TraceCode = productionOrder.TraceCode;
                            transferOrderMaster.IsShipScanHu = isShipScanHu;
                            transferOrderMaster.IsReceiveScanHu = isReceiveScanHu;
                            transferOrderMaster.IsOrderFulfillUC = false;
                            transferOrderMaster.IsShipFulfillUC = false;
                            transferOrderMaster.IsReceiveFulfillUC = false;
                            if (transferOrderMaster.OrderDetails == null)
                            {
                                transferOrderMaster.OrderDetails = new List<OrderDetail>();
                            }

                            orderMasterList.Add(transferOrderMaster);
                        }

                        OrderDetail orderDetail = new OrderDetail();

                        Mapper.Map(orderBomDetail, orderDetail);
                        Item item = genericMgr.FindById<Item>(orderBomDetail.Item);
                        orderDetail.ItemDescription = item.Description;
                        orderDetail.UnitCount = item.UnitCount;
                        orderDetail.BaseUom = item.Uom;
                        orderDetail.LocationFrom = transferOrderMaster.LocationFrom;
                        orderDetail.LocationTo = transferOrderMaster.LocationTo;
                        orderDetail.LocationFromName = transferOrderMaster.LocationFromName;
                        orderDetail.LocationToName = transferOrderMaster.LocationToName;
                        transferOrderMaster.OrderDetails.Add(orderDetail);

                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region 老代码
                //foreach (OrderBomDetail orderBomDetail in exactOrderBomDetailList)
                //{
                //    //如果根据bom中的子物料以及库位能够对应到路线明细，则以该路线明细来生成要货单
                //    //如果根据以上无法得到路线明细，则根据采购路线查找生产单BOM物料的采购入库地点，作为来源库位，取生产单BOM的库位为目的库位，生成要货单。

                //    FlowMaster transferFlow = null;
                //    FlowDetail transferFlowDetail = null;

                //    string hql = "from FlowDetail as d where d.Item = ? and exists (select 1 from FlowMaster as f where f.Code = d.Flow and f.Type = ? and f.IsActive = ? and (d.LocationTo = ? or (d.LocationTo is null and f.LocationTo = ?))) order by d.IsAutoCreate desc";
                //    IList<FlowDetail> transferFlowDetailList = genericMgr.FindAll<FlowDetail>(hql, new object[] { orderBomDetail.Item, com.Sconit.CodeMaster.OrderType.Transfer, true, orderBomDetail.Location, orderBomDetail.Location });

                //    bool isShipScanHu = false;
                //    bool isReceiveScanHu = false;
                //    if (transferFlowDetailList != null && transferFlowDetailList.Count > 0)
                //    {
                //        transferFlow = genericMgr.FindById<FlowMaster>(transferFlowDetailList[0].Flow);
                //        isShipScanHu = transferFlow.IsShipScanHu;
                //        isReceiveScanHu = transferFlow.IsReceiveScanHu;
                //        transferFlowDetail = transferFlowDetailList[0];
                //        if (transferFlow.IsAutoCreate && transferFlowDetail.IsAutoCreate)
                //        {
                //            #region 自动拉料的不需要拉
                //            continue;
                //            #endregion
                //        }
                //    }
                //    else
                //    {
                //        #region 则根据采购路线查找生产单BOM物料的采购入库地点，作为来源库位，取生产单BOM的库位为目的库位，生成要货单。
                //        //因为一条路线上可能包含关键件和非关键件,收货入库可能条码也可能数量
                //        //发货是否扫描条码要跟据采购路线的收货扫描条码选项
                //        //收货是否扫描条码要根据是否关键件（itemtrace）
                //        FlowMaster procurementFlow = GetSourceFlow(orderBomDetail.Item, orderBomDetail.Location, new List<string>());
                //        if (procurementFlow == null)
                //        {
                //            // throw new BusinessException("找不到物料{0}对应的采购路线", orderBomDetail.Item);
                //            itemString += string.IsNullOrEmpty(itemString) ? orderBomDetail.Item : "," + orderBomDetail.Item;
                //            continue;
                //        }
                //        hql = "from FlowMaster as f where f.Type = ? and  f.LocationFrom = ? and f.LocationTo = ? and f.IsActive = ?";
                //        IList<FlowMaster> transferFlowList = genericMgr.FindAll<FlowMaster>(hql, new object[] { com.Sconit.CodeMaster.OrderType.Transfer, procurementFlow.LocationTo, orderBomDetail.Location, true });
                //        if (transferFlowList == null || transferFlowList.Count == 0)
                //        {
                //            // throw new BusinessException("找不到物料{0}对应的来源库位{1},目的库位{2}的移库路线", orderBomDetail.Item);
                //            itemString += string.IsNullOrEmpty(itemString) ? orderBomDetail.Item : "," + orderBomDetail.Item;
                //            continue;
                //        }
                //        #endregion

                //        transferFlow = transferFlowList[0];
                //        isShipScanHu = procurementFlow.IsReceiveScanHu;
                //        IList<ItemTrace> itemTraceList = genericMgr.FindAll<ItemTrace>("from ItemTrace as i where i.Item = ?", orderBomDetail.Item);
                //        isReceiveScanHu = (itemTraceList == null || itemTraceList.Count() == 0) ? false : true;

                //    }

                #endregion
            }

            #endregion

            foreach (OrderMaster om in orderMasterList)
            {
                CreateOrder(om);
                orderString += string.IsNullOrEmpty(orderString) ? om.OrderNo : "," + om.OrderNo;
            }
            return new string[2] { orderString, itemString };

        }
        #endregion

        #region 导入生成紧急拉料单
        [Transaction(TransactionMode.Requires)]
        public string[] CreateEmTransferOrderFromXls(Stream inputStream)
        {
            string orderStr = string.Empty;
            string itemStr = string.Empty;
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
            int colLocTo = 4;// 目的库位
            int colQty = 5;//数量
            int colWindowTime = 6;//窗口时间
            #endregion

            DateTime dateTimeNow = DateTime.Now;
            IList<OrderDetail> exactOrderDetailList = new List<OrderDetail>();
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
                string locationCode = string.Empty;
                DateTime windowTime = DateTime.Now;

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

                #endregion

                #region 读取库位
                locationCode = row.GetCell(colLocTo) != null ? row.GetCell(colLocTo).StringCellValue : string.Empty;
                if (locationCode == null || locationCode.Trim() == string.Empty)
                {
                    throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colLocTo.ToString());
                }
                #endregion

                #region 读取窗口时间
                try
                {
                    windowTime = row.GetCell(colWindowTime).DateCellValue;
                }
                catch
                {
                    ImportHelper.ThrowCommonError(row.RowNum, colWindowTime, row.GetCell(colWindowTime));
                }
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

                #region 填充数据

                OrderDetail od = new OrderDetail();
                od.LocationTo = locationCode;
                od.Item = itemCode;
                od.Uom = uomCode;
                od.OrderedQty = qty;
                od.WindowTime = windowTime;
                exactOrderDetailList.Add(od);
                #endregion
            }

            #region 创建要货单
            //IList<WorkingCalendarView> workView=genericMgr.FindAll <WorkingCalendarView>("select v from WorkingCalendarView as v");
            IList<OrderMaster> orderMasterList = new List<OrderMaster>();
            var locationList = exactOrderDetailList.Select(b => b.LocationTo).Distinct().ToList();
            foreach (string location in locationList)
            {
                FlowMaster transferFlow = null;
                FlowDetail transferFlowDetail = null;
                FlowStrategy transferFlowStrategy = null;
                string hql = "from FlowDetail as d where exists (select 1 from FlowMaster as f where f.Code = d.Flow and f.Type = ? and f.IsActive = ? and f.FlowStrategy not in (?,?) and (d.LocationTo = ? or (d.LocationTo is null and f.LocationTo = ?)))";
                IList<FlowDetail> transferFlowDetailList = genericMgr.FindAll<FlowDetail>(hql, new object[] { com.Sconit.CodeMaster.OrderType.Transfer, true, com.Sconit.CodeMaster.FlowStrategy.KIT, com.Sconit.CodeMaster.FlowStrategy.SEQ, location, location });

                var lod = exactOrderDetailList.Where(p => p.LocationTo == location).ToList(); //发到此库位的order明细
                var nod = new List<OrderDetail>();                                        //没找到路线明细的orderdet
                #region 找到有路线明细的
                foreach (OrderDetail od in lod)
                {
                    transferFlowDetail = transferFlowDetailList.Where(f => f.Item == od.Item).ToList().FirstOrDefault();
                    if (transferFlowDetail != null)
                    {
                        transferFlow = genericMgr.FindById<FlowMaster>(transferFlowDetail.Flow);
                        transferFlowStrategy = genericMgr.FindById<FlowStrategy>(transferFlow.Code);
                        #region 建订单
                        OrderMaster transferOrderMaster = orderMasterList.Where(o => o.Flow == transferFlow.Code && o.WindowTime == od.WindowTime).ToList().SingleOrDefault<OrderMaster>();
                        if (transferOrderMaster == null)
                        {
                            transferOrderMaster = TransferFlow2Order(transferFlow, null);
                            transferOrderMaster.WindowTime = od.WindowTime == null ? DateTime.Now : od.WindowTime.Value;
                            IList<WorkingCalendarView> workingCalendarViewList = this.workingCalendarMgr.GetWorkingCalendarViewList(transferOrderMaster.PartyFrom, transferOrderMaster.Flow, transferOrderMaster.WindowTime.Add(TimeSpan.FromDays(-7)), transferOrderMaster.WindowTime);
                            transferOrderMaster.StartTime = this.workingCalendarMgr.GetStartTimeAtWorkingDate(transferOrderMaster.WindowTime, (double)transferFlowStrategy.EmergencyLeadTime, CodeMaster.TimeUnit.Hour, transferFlow.PartyFrom, transferFlow.Code, workingCalendarViewList);
                            transferOrderMaster.IsAutoRelease = true;
                            if (transferOrderMaster.OrderDetails == null)
                            {
                                transferOrderMaster.OrderDetails = new List<OrderDetail>();
                            }

                            orderMasterList.Add(transferOrderMaster);
                            //orderStr += string.IsNullOrEmpty(orderStr) ? transferOrderMaster.OrderNo : "," + transferOrderMaster.OrderNo;
                        }

                        OrderDetail orderDetail = Mapper.Map<OrderDetail, OrderDetail>(transferOrderMaster.OrderDetails.Where(d => d.Item == od.Item && (d.LocationTo == od.LocationTo || (d.LocationTo == null && transferOrderMaster.LocationTo == od.LocationTo))).First());
                        if (orderDetail.Uom != od.Uom)
                        {
                            orderDetail.UnitQty = this.itemMgr.ConvertItemUomQty(od.Item, orderDetail.Uom, 1, od.Uom);
                            orderDetail.OrderedQty = od.OrderedQty / orderDetail.UnitQty;
                        }
                        else
                        {
                            orderDetail.UnitQty = 1;
                            orderDetail.OrderedQty = od.OrderedQty;
                        }
                        transferOrderMaster.OrderDetails.Add(orderDetail);
                        #endregion
                    }
                    else
                    {
                        itemStr += string.IsNullOrEmpty(itemStr) ? od.Item : "," + od.Item;
                    }
                }
                #endregion

            }
            foreach (OrderMaster om in orderMasterList)
            {
                CreateOrder(om);
                orderStr += string.IsNullOrEmpty(orderStr) ? om.OrderNo : "," + om.OrderNo;
            }
            #endregion

            return new string[] { orderStr, itemStr };
        }
        #endregion

        #region 自由移库
        [Transaction(TransactionMode.Requires)]
        public string CreateTransferOrderFromXls(Stream inputStream, string regionFromCode, string regionToCode, DateTime effectiveDate)
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
            int colLocTo = 5;// 目的库位
            int colQty = 6;//数量
            #endregion

            DateTime dateTimeNow = DateTime.Now;
            if (string.IsNullOrEmpty(regionToCode))
            {
                regionToCode = regionFromCode;
            }

            IList<OrderDetail> exactOrderDetailList = new List<OrderDetail>();
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

                #endregion

                #region 读取来源库位
                locationFromCode = row.GetCell(colLocFrom) != null ? row.GetCell(colLocFrom).StringCellValue : string.Empty;
                if (string.IsNullOrEmpty(locationFromCode))
                {
                    throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colLocFrom.ToString());
                }

                IList<Location> locationFromList = genericMgr.FindAll<Location>("select l from Location as l where l.Code=?", locationFromCode);
                if (locationFromList != null && locationFromList.Count > 0)
                {

                    if (locationFromList[0].Region != regionFromCode)
                    {
                        throw new BusinessException("指定区域不存在此库位" + locationFromCode, (row.RowNum + 1).ToString(), colLocFrom.ToString());
                    }
                }
                else
                {
                    throw new BusinessException("指定区域不存在此库位" + regionToCode, (row.RowNum + 1).ToString(), colLocFrom.ToString());
                }
                //  Location locationFrom = genericMgr.FindById<Location>(locationFromCode);


                #endregion

                #region 读取目的库位
                locationToCode = row.GetCell(colLocTo) != null ? row.GetCell(colLocTo).StringCellValue : string.Empty;
                if (string.IsNullOrEmpty(locationFromCode))
                {
                    throw new BusinessException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colLocTo.ToString());
                }

                IList<Location> locationToList = genericMgr.FindAll<Location>("select l from Location as l where l.Code=?", locationToCode);
                if (locationToList != null && locationToList.Count > 0)
                {
                    if (locationToList[0].Region != regionToCode)
                    {
                        throw new BusinessException("指定区域不存在此库位" + regionToCode, (row.RowNum + 1).ToString(), colLocFrom.ToString());
                    }
                }
                else
                {
                    throw new BusinessException("指定区域不存在此库位" + regionToCode, (row.RowNum + 1).ToString(), colLocFrom.ToString());
                }

                //Location locationTo = genericMgr.FindById<Location>(locationToCode);
                //if (locationTo.Region != regionToCode)
                //{
                //    throw new BusinessException("指定区域不存在此库位" + locationTo, (row.RowNum + 1).ToString(), colLocFrom.ToString());
                //}
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

                #region 填充数据
                OrderDetail od = new OrderDetail();
                od.LocationFrom = locationFromCode;
                od.LocationTo = locationToCode;
                od.Item = itemCode;
                od.Uom = uomCode;
                od.OrderedQty = qty;


                exactOrderDetailList.Add(od);
                #endregion
            }

            #endregion

            return CreateFreeTransferOrderMaster(regionFromCode, regionToCode, exactOrderDetailList, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public string CreateFreeTransferOrderMaster(string regionFromCode, string regionToCode, IList<OrderDetail> orderDetailList, DateTime effectiveDate)
        {
            if (orderDetailList == null || orderDetailList.Count == 0)
            {
                throw new BusinessException("移库明细不能为空");
            }
            var orderMaster = new OrderMaster();

            var regionFrom = genericMgr.FindById<Region>(regionFromCode);
            var regionTo = genericMgr.FindById<Region>(regionToCode);

            Location locFrom = genericMgr.FindById<Location>(orderDetailList[0].LocationFrom);
            Location locTo = genericMgr.FindById<Location>(orderDetailList[0].LocationTo);
            orderMaster.LocationFrom = locFrom.Code;
            orderMaster.IsShipScanHu = false;
            orderMaster.IsReceiveScanHu = false;
            orderMaster.LocationFromName = locFrom.Name;
            orderMaster.LocationTo = locTo.Code;
            orderMaster.LocationToName = locTo.Name;
            orderMaster.PartyFrom = regionFrom.Code;
            orderMaster.PartyFromName = regionFrom.Name;
            orderMaster.PartyTo = regionTo.Code;
            orderMaster.PartyToName = regionTo.Name;
            orderMaster.Type = CodeMaster.OrderType.Transfer;
            orderMaster.StartTime = DateTime.Now;
            orderMaster.WindowTime = DateTime.Now;
            orderMaster.EffectiveDate = effectiveDate;

            orderMaster.IsQuick = true;
            foreach (OrderDetail od in orderDetailList)
            {
                Item item = genericMgr.FindById<Item>(od.Item);
                Location dLocFrom = genericMgr.FindById<Location>(od.LocationFrom);
                Location dLocTo = genericMgr.FindById<Location>(od.LocationTo);
                //Location dLocFrom = genericMgr.FindById<Location>(od.LocationFrom);
                od.Uom = item.Uom;
                od.ItemDescription = item.Description;
                od.ReferenceItemCode = item.ReferenceCode;
                od.LocationFromName = dLocFrom.Name;
                od.LocationToName = dLocTo.Name;
                od.UnitCount = item.UnitCount;
            }

            orderMaster.OrderDetails = orderDetailList;
            this.CreateOrder(orderMaster);
            return orderMaster.OrderNo;
        }

        #endregion

        #region 导入要货单
        [Transaction(TransactionMode.Requires)]
        public string CreateProcurementOrderFromXls(Stream inputStream,
            string flowCode, string extOrderNo, string refOrderNo,
            DateTime startTime, DateTime windowTime, CodeMaster.OrderPriority priority)
        {
            #region 导入数据
            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 1);

            #region 列定义
            int colSeqNo = 0;//序号
            int colItem = 1;//物料代码
            //int colItemDescription = 2;// 物料描述
            int colQty = 3;//数量
            int colUom = 4;//单位
            int colUnitCount = 5;//单包装
            int colLocationTo = 6;//来源库位
            #endregion

            IList<OrderDetail> exactOrderDetailList = new List<OrderDetail>();

            var flowMaster = this.genericMgr.FindById<FlowMaster>(flowCode);

            var flowDetailList = this.flowMgr.GetFlowDetailList(flowCode, false, true);

            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 1, 9))
                {
                    break;//边界
                }
                int seqNo = 0;
                string itemCode = string.Empty;
                decimal qty = 0;
                string uom = string.Empty;
                decimal unitCount = 0;
                Location locationTo = null;

                #region 读取数据

                #region 读取序号
                try
                {
                    seqNo = Convert.ToInt32(row.GetCell(colSeqNo).NumericCellValue);
                }
                catch
                {
                    seqNo = 0;
                }
                #endregion

                #region 读取物料代码
                itemCode = ImportHelper.GetCellStringValue(row.GetCell(colItem));
                if (string.IsNullOrWhiteSpace(itemCode))
                {
                    ImportHelper.ThrowCommonError(row.RowNum, colItem, row.GetCell(colItem));
                }
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

                #region 读取单位
                uom = ImportHelper.GetCellStringValue(row.GetCell(colUom));
                #endregion

                #region 读取单包装
                try
                {
                    unitCount = Convert.ToDecimal(row.GetCell(colUnitCount).NumericCellValue);
                }
                catch
                {
                    unitCount = 0;
                }
                #endregion

                #region 读取目的库位
                string locationToCode = ImportHelper.GetCellStringValue(row.GetCell(colLocationTo));
                if (!string.IsNullOrWhiteSpace(locationToCode))
                {
                    locationTo = this.genericMgr.FindById<Location>(locationToCode);
                    if (locationTo.Region != flowMaster.PartyTo)
                    {
                        throw new BusinessException("库位{0}不在区域{1}下", locationTo.Code, flowMaster.PartyTo);
                    }
                }
                #endregion

                #endregion

                #region 填充数据
                var flowDetail = flowDetailList.Where(f => f.Item == itemCode && f.LocationTo == locationToCode).FirstOrDefault();
                if (flowDetail == null)
                {
                    if (flowMaster.IsManualCreateDetail)
                    {
                        flowDetail = new FlowDetail();

                        var item = this.genericMgr.FindById<Entity.MD.Item>(itemCode);
                        flowDetail.OrderQty = qty;
                        flowDetail.BaseUom = item.Uom;
                        flowDetail.Item = itemCode;
                        if (seqNo > 0)
                        {
                            flowDetail.ExternalSequence = seqNo;
                        }
                        if (!string.IsNullOrWhiteSpace(uom))
                        {
                            flowDetail.Uom = uom;
                        }
                        else
                        {
                            flowDetail.Uom = item.Uom;
                        }
                        if (unitCount > 0)
                        {
                            flowDetail.UnitCount = unitCount;
                        }
                        else
                        {
                            flowDetail.UnitCount = item.UnitCount;
                        }
                        if (locationToCode == null)
                        {
                            flowDetail.LocationTo = flowMaster.LocationTo;
                        }
                        else
                        {
                            flowDetail.LocationTo = locationTo.Code;
                        }
                    }
                    else
                    {
                        throw new BusinessException("没有找到匹配的物流路线明细", itemCode, uom, unitCount.ToString());
                    }
                }
                else
                {
                    if (seqNo > 0)
                    {
                        flowDetail.ExternalSequence = seqNo;
                    }
                    flowDetail.OrderQty = qty;
                }
                flowMaster.AddFlowDetail(flowDetail);

                #endregion
            }

            #endregion

            #region 创建要货单
            OrderMaster orderMaster = TransferFlow2Order(flowMaster, true);

            orderMaster.ReferenceOrderNo = refOrderNo;
            orderMaster.ExternalOrderNo = extOrderNo;
            orderMaster.StartTime = startTime;
            orderMaster.WindowTime = windowTime;
            orderMaster.Priority = priority;
            this.CreateOrder(orderMaster);

            #endregion

            return orderMaster.OrderNo;
        }
        #endregion

        #region 页面条码移库
        [Transaction(TransactionMode.Requires)]
        public string CreateHuTransferOrder(string flowCode, IList<string> huIdList, DateTime effectiveDate)
        {
            FlowMaster flow = genericMgr.FindById<FlowMaster>(flowCode);
            OrderMaster order = TransferFlow2Order(flow, false);
            IList<OrderDetail> orderDetailList = new List<OrderDetail>();
            AddHuToOrderDetailInput(orderDetailList, huIdList);
            order.OrderDetails = orderDetailList;
            order.EffectiveDate = effectiveDate;
            order.WindowTime = DateTime.Now;
            order.StartDate = DateTime.Now;
            order.IsQuick = true;
            CreateOrder(order);
            return order.OrderNo;

        }
        #endregion

        #region 获得有权限的订单
        public OrderMaster GetAuthenticOrder(string orderNo)
        {
            var orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);
            if (Utility.SecurityHelper.HasPermission(orderMaster))
            {
                return orderMaster;
            }
            return null;
        }
        #endregion

        #region 高级仓库发货
        [Transaction(TransactionMode.Requires)]
        public void ProcessShipPlanResult4Hu(string transportOrderNo, IList<string> huIdList, DateTime? effDate)
        {
            DataSet ds = null;
            #region 处理发运计划
            User user = SecurityContextHolder.Get();
            SqlParameter[] paras = new SqlParameter[3];
            DataTable shipResultTable = new DataTable();
            shipResultTable.Columns.Add("HuId", typeof(string));
            foreach (var hu in huIdList)
            {
                DataRow row = shipResultTable.NewRow();
                row[0] = hu;
                shipResultTable.Rows.Add(row);
            }
            paras[0] = new SqlParameter("@ShipResultTable", SqlDbType.Structured);
            paras[0].Value = shipResultTable;
            paras[1] = new SqlParameter("@CreateUserId", SqlDbType.Int);
            paras[1].Value = user.Id;
            paras[2] = new SqlParameter("@CreateUserNm", SqlDbType.VarChar);
            paras[2].Value = user.FullName;

            try
            {
                ds = this.genericMgr.GetDatasetByStoredProcedure("USP_WMS_ProcessShipResult4Hu", paras);

                if (ds != null && ds.Tables != null && ds.Tables[0] != null
                    && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow msg in ds.Tables[0].Rows)
                    {
                        if (msg[0].ToString() == "0")
                        {
                            MessageHolder.AddInfoMessage((string)msg[1]);
                        }
                        else if (msg[0].ToString() == "1")
                        {
                            MessageHolder.AddWarningMessage((string)msg[1]);
                        }
                        else
                        {
                            MessageHolder.AddErrorMessage((string)msg[1]);
                        }
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        MessageHolder.AddErrorMessage(ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        MessageHolder.AddErrorMessage(ex.InnerException.Message);
                    }
                }
                else
                {
                    MessageHolder.AddErrorMessage(ex.Message);
                }

                return;
            }
            #endregion

            #region 创建ASN并添加到运输单中
            if (ds != null && ds.Tables != null && ds.Tables[1] != null
                   && ds.Tables[1].Rows != null && ds.Tables[1].Rows.Count > 0)
            {
                IList<object> orderDetailIdList = new List<object>();
                foreach (DataRow hu in ds.Tables[1].Rows)
                {
                    if (!orderDetailIdList.Contains(hu[3]))
                    {
                        orderDetailIdList.Add(hu[3]);
                    }
                }

                IList<OrderDetail> orderDetailList = genericMgr.FindAllIn<OrderDetail>("from OrderDetail where Id in(?", orderDetailIdList);
                IDictionary<string, IList<OrderDetail>> flowDic = new Dictionary<string, IList<OrderDetail>>();
                foreach (DataRow hu in ds.Tables[1].Rows)
                {
                    OrderDetail orderDetail = orderDetailList.Where(od => od.Id == (int)hu[3]).Single();
                    OrderDetailInput orderDetailInput = new OrderDetailInput();
                    orderDetailInput.HuId = (string)hu[0];
                    orderDetailInput.LotNo = (string)hu[1];
                    orderDetailInput.ShipQty = (decimal)hu[2];
                    orderDetailInput.OccupyType = CodeMaster.OccupyType.Pick;
                    orderDetail.AddOrderDetailInput(orderDetailInput);

                    if (!flowDic.ContainsKey((string)hu[4]))
                    {
                        IList<OrderDetail> flowOrderDetailList = new List<OrderDetail>();
                        flowOrderDetailList.Add(orderDetail);
                        flowDic.Add((string)hu[4], flowOrderDetailList);
                    }
                    else
                    {
                        IList<OrderDetail> flowOrderDetailList = flowDic[(string)hu[4]];
                        if (!flowOrderDetailList.Contains(orderDetail))
                        {
                            flowOrderDetailList.Add(orderDetail);
                        }
                    }
                }

                if (!effDate.HasValue)
                {
                    effDate = DateTime.Now;
                }

                IList<string> ipNoList = new List<string>();
                foreach (var flow in flowDic)
                {
                    IpMaster ipMaster = ShipOrder(flow.Value, effDate.Value);
                    ipNoList.Add(ipMaster.IpNo);
                }

                if (!string.IsNullOrWhiteSpace(transportOrderNo))
                {
                    transportMgr.AddTransportOrderDetail(transportOrderNo, ipNoList);
                }
            }
            else
            {
                throw new TechnicalException("返回的条码信息为空。");
            }
            #endregion
        }
        #endregion

        #endregion

        #region private methods
        private void GenerateOrderOperation(OrderDetail orderDetail, OrderMaster orderMaster)
        {
            string routingCode = !string.IsNullOrWhiteSpace(orderDetail.Routing) ? orderDetail.Routing : orderMaster.Routing;
            if (!string.IsNullOrWhiteSpace(routingCode))
            {
                RoutingMaster routing = this.genericMgr.FindById<RoutingMaster>(routingCode);
                IList<RoutingDetail> routingDetailList = routingMgr.GetRoutingDetails(routingCode, orderMaster.StartTime);

                if (routingDetailList != null && routingDetailList.Count() > 0)
                {
                    IList<OrderOperation> orderOperationList = (from det in routingDetailList
                                                                select new OrderOperation
                                                                {
                                                                    Op = det.Operation,
                                                                    OpReference = det.OpReference,
                                                                    Location = det.Location
                                                                }).OrderBy(det => det.Op).ThenBy(det => det.OpReference).ToList();

                    orderDetail.OrderOperations = orderOperationList;
                }
                else
                {
                    throw new BusinessException(Resources.PRD.Routing.Errors_RoutingDetailNotFound, routingCode);
                }
            }
        }

        private void GenerateOrderBomDetail(OrderDetail orderDetail, OrderMaster orderMaster)
        {
            if (orderDetail.ScheduleType == CodeMaster.ScheduleType.MES21 || orderDetail.ScheduleType == CodeMaster.ScheduleType.MES22
                || orderDetail.ScheduleType == CodeMaster.ScheduleType.MES23)
            {
                //只记录废品数,无材料消耗
                return;
            }
            if (orderMaster.ProductLineFacility == "EXV")
            {

                //不消耗材料

                return;
            }
            //if (orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Return)
            //{
            //    //直接退成品
            //}

            #region 查找成品单位和Bom单位的转换关系
            //把OrderDetail的收货单位和单位用量转换为BOM单位和单位用量
            //fgUom，fgUnityQty代表接收一个orderDetail.Uom单位(等于订单的收货单位)的FG，等于单位(fgUom)有多少(fgUnityQty)值
            string fgUom = orderDetail.Uom;
            //如果和Bom上的单位不一致，转化为Bom上的单位，不然会导致物料回冲不正确。  

            //查找Bom
            BomMaster bomMaster = FindOrderDetailBom(orderDetail);
            decimal fgUnityQty = 1;

            #region 判断Bom是否有效
            if (!bomMaster.IsActive)
            {
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_BomInActive, orderDetail.Bom);
            }
            #endregion

            //订单单位和Bom单位不一致，需要做单位转换
            if (string.Compare(orderDetail.Uom, bomMaster.Uom) != 0)
            {
                fgUom = bomMaster.Uom;
                fgUnityQty = itemMgr.ConvertItemUomQty(orderDetail.Item, orderDetail.Uom, fgUnityQty, fgUom);
            }
            #endregion

            #region 创建OrderBomDetail
            //Item fgItem = genericMgr.FindById<Item>(orderDetail.Item);

            #region 查询Bom明细
            IList<BomDetail> bomDetailList = bomMgr.GetFlatBomDetail(bomMaster, orderMaster.StartTime);
            #endregion

            var itemCodes = bomDetailList.Select(b => b.Item).Distinct();
            #region 查询Bom Item
            IList<Item> bomItemList = this.genericMgr.FindAllIn<Item>("from Item where Code in(?", itemCodes);
            #endregion

            #region 查询工艺流程明细
            //IList<RoutingDetail> routingDetailList = null;
            //if (!string.IsNullOrEmpty(orderDetail.Routing))
            //{
            //    RoutingMaster routing = this.genericMgr.FindById<RoutingMaster>(orderDetail.Routing);
            //    if (!routing.IsActive)
            //    {
            //        throw new BusinessErrorException(Resources.ORD.OrderMaster.Errors_RoutingInActive, orderDetail.Routing);
            //    }
            //    routingDetailList = routingMgr.GetRoutingDetails(orderDetail.Routing, orderMaster.StartTime);
            //}
            #endregion

            #region 查询生产防错明细 SIH客户化是从零件追溯表中取需要扫描的零件。
            IList<string> itemTraceList = this.genericMgr.FindAllIn<string>
                ("select it.Item From ItemTrace as it where it.Item in (?", itemCodes);
            #endregion

            foreach (BomDetail bomDetail in bomDetailList)
            {
                #region 查找物料的来源库位和提前期
                string bomLocFrom = string.Empty;
                //来源库位查找逻辑RoutingDetail-->OrderDetail-->Order-->BomDetail 
                //工序的优先级最大，因为同一个OrderMaster可以有不同的工艺流程，其次OrderMaster，最后BomDetail
                TryLoadOrderOperations(orderDetail);
                if (orderDetail.OrderOperations != null && orderDetail.OrderOperations.Count > 0)
                {
                    //取RoutingDetail上的
                    OrderOperation orderOperation = orderDetail.OrderOperations.Where(
                            p => p.Op == bomDetail.Operation
                            && p.OpReference == bomDetail.OpReference).SingleOrDefault();

                    if (orderOperation != null)
                    {
                        bomLocFrom = orderOperation.Location;
                    }
                }

                if (string.IsNullOrEmpty(bomLocFrom))
                {
                    //在取OrderDetail上，然后是OrderHead上取
                    //取默认库位FlowDetail-->Flow
                    if (orderDetail.OrderSubType == CodeMaster.OrderSubType.Normal)
                    {
                        if (!string.IsNullOrEmpty(orderDetail.LocationFrom))
                        {
                            bomLocFrom = orderDetail.LocationFrom;
                        }
                        else
                        {
                            bomLocFrom = orderMaster.LocationFrom;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(orderDetail.LocationTo))
                        {
                            bomLocFrom = orderDetail.LocationTo;
                        }
                        else
                        {
                            bomLocFrom = orderMaster.LocationTo;
                        }
                    }
                }

                if (string.IsNullOrEmpty(bomLocFrom))
                {
                    //最后取BomDetail上的Location
                    bomLocFrom = bomDetail.Location;
                }
                #endregion

                #region 创建生产单物料明细
                OrderBomDetail orderBomDetail = new OrderBomDetail();
                Item bomItem = bomItemList.Where(i => i.Code == bomDetail.Item).Single();

                orderBomDetail.Bom = bomDetail.Bom;
                orderBomDetail.Item = bomDetail.Item;
                orderBomDetail.ItemDescription = bomItem.Description;
                orderBomDetail.Uom = bomDetail.Uom;   //Bom单位
                //todo 检查Bom Operation和Routing Operation 不匹配的情况       
                orderBomDetail.Operation = bomDetail.Operation;
                orderBomDetail.OpReference = bomDetail.OpReference;
                if (orderMaster.IsListPrice)
                {
                    orderBomDetail.BomUnitQty = bomDetail.UnitBomQty * fgUnityQty; //单位成品（订单单位），需要消耗物料数量（Bom单位）。
                }
                else
                {
                    orderBomDetail.BomUnitQty = bomDetail.CalculatedQty * fgUnityQty; //单位成品（订单单位），需要消耗物料数量（Bom单位）。
                }
                orderBomDetail.OrderedQty = orderDetail.OrderedQty * orderBomDetail.BomUnitQty;
                orderBomDetail.Location = bomLocFrom;
                orderBomDetail.IsPrint = bomDetail.IsPrint;
                orderBomDetail.IsScanHu = itemTraceList.Contains(orderBomDetail.Item);   //生产防错标记
                orderBomDetail.BackFlushMethod = bomDetail.BackFlushMethod;
                orderBomDetail.FeedMethod = bomDetail.FeedMethod;
                orderBomDetail.IsAutoFeed = bomDetail.IsAutoFeed;
                //orderBomDetail.BackFlushInShortHandle = bomDetail.BackFlushInShortHandle;
                orderBomDetail.EstimateConsumeTime = orderMaster.StartTime;

                //BomDetail的基本单位
                orderBomDetail.BaseUom = bomItem.Uom;
                if (orderBomDetail.BaseUom != orderBomDetail.Uom)
                {
                    orderBomDetail.UnitQty = this.itemMgr.ConvertItemUomQty(orderBomDetail.Item, orderBomDetail.Uom, 1, orderBomDetail.BaseUom);
                }
                else
                {
                    orderBomDetail.UnitQty = 1;
                }

                orderDetail.AddOrderBomDetail(orderBomDetail);
                #endregion

                #region 查找零件消耗提前期，累加所有工序小于等于当前工序的提前期
                if (orderDetail.OrderOperations != null)
                {
                    IList<OrderOperation> orderOperationList = orderDetail.OrderOperations.Where(
                               p => p.Op < bomDetail.Operation
                        //每道工序对应一个工位，不考虑一道工序多工位的情况
                        //|| (p.Operation == bomDetail.Operation              //同道工序多工位的情况
                        //&& string.Compare(p.OpReference, bomDetail.OpReference) <= 0)
                               ).ToList();

                    if (orderOperationList != null && orderOperationList.Count > 0)
                    {
                        foreach (OrderOperation orderOperation in orderOperationList)
                        {
                            //switch (orderOperation.TimeUnit)
                            //{
                            //    case com.Sconit.CodeMaster.TimeUnit.Day:
                            //        orderBomDetail.EstimateConsumeTime = orderBomDetail.EstimateConsumeTime.Add(TimeSpan.FromDays(orderOperation.LeadTime));
                            //        break;
                            //    case com.Sconit.CodeMaster.TimeUnit.Hour:
                            //        orderBomDetail.EstimateConsumeTime = orderBomDetail.EstimateConsumeTime.Add(TimeSpan.FromHours(orderOperation.LeadTime));
                            //        break;
                            //    case com.Sconit.CodeMaster.TimeUnit.Minute:
                            //        orderBomDetail.EstimateConsumeTime = orderBomDetail.EstimateConsumeTime.Add(TimeSpan.FromMinutes(orderOperation.LeadTime));
                            //        break;
                            //    case com.Sconit.CodeMaster.TimeUnit.Second:
                            //        orderBomDetail.EstimateConsumeTime = orderBomDetail.EstimateConsumeTime.Add(TimeSpan.FromSeconds(orderOperation.LeadTime));
                            //        break;
                            //};
                        }
                    }
                }
                #endregion

                #region 更新生产防错标记
                //if (productionScanDetailList != null && productionScanDetailList.Count > 0)
                //{
                //    ProductionScanDetail productionScanDetail = productionScanDetailList.Where(
                //           p => p.Operation == orderBomDetail.Operation
                //               && p.OpReference == orderBomDetail.OpReference
                //               && p.Item == orderBomDetail.Item).SingleOrDefault();

                //    if (productionScanDetail != null)
                //    {
                //        orderBomDetail.IsScanHu = true;
                //    }
                //    else
                //    {
                //        orderBomDetail.IsScanHu = false;
                //    }
                //}
                #endregion
            }

            #region 委外退货
            if (orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Return)
            {
                //更改原材料库位,材料增加              

                //OrderBomDetail orderBomDetail = new OrderBomDetail();
                //orderBomDetail.Item = orderDetail.Item;
                //orderBomDetail.ItemDescription = orderDetail.ItemDescription;
                //orderBomDetail.Uom = orderDetail.Uom;
                //#region 取工序，先取RoutingDetail上最小工序，如果没有取BomDetail的最小工序
                //if (orderDetail.OrderOperations != null && orderDetail.OrderOperations.Count() > 0)
                //{
                //    //先根据工序排序，在根据工位排序
                //    OrderOperation orderOperation = orderDetail.OrderOperations
                //        .OrderBy(op => op.Operation)
                //        .ThenBy(op => op.OpReference)
                //        .First();
                //    orderBomDetail.Operation = orderOperation.Operation;
                //    orderBomDetail.OpReference = orderOperation.OpReference;
                //}
                //else
                //{
                //    BomDetail bomDetail = bomDetailList.OrderBy(det => det.Operation).ThenBy(det => det.OpReference).First();
                //    orderBomDetail.Operation = bomDetail.Operation;
                //    orderBomDetail.OpReference = bomDetail.OpReference;
                //}
                //#endregion
                //orderBomDetail.BaseUom = orderDetail.BaseUom;
                //orderBomDetail.BomUnitQty = 1;
                //orderBomDetail.OrderedQty = orderDetail.OrderedQty;
                //orderBomDetail.Location = !string.IsNullOrWhiteSpace(orderDetail.LocationTo) ? orderDetail.LocationTo : orderMaster.LocationTo;
                //orderBomDetail.IsPrint = false;
                //orderBomDetail.IsScanHu = orderMaster.IsReceiveScanHu || orderMaster.CreateHuOption == com.Sconit.CodeMaster.CreateHuOption.Receive;  //收获扫描Hu或者收货时创建条码，返工时要扫描成品条码
                //orderBomDetail.BackFlushMethod = com.Sconit.CodeMaster.BackFlushMethod.GoodsReceive;
                //orderBomDetail.FeedMethod = com.Sconit.CodeMaster.FeedMethod.None;
                //orderBomDetail.IsAutoFeed = false;
                ////orderBomDetail.BackFlushInShortHandle = BomDetail.BackFlushInShortHandleEnum.Nothing;
                //orderBomDetail.EstimateConsumeTime = orderMaster.StartTime;   //预计消耗时间等于开始时间
                //orderDetail.AddOrderBomDetail(orderBomDetail);
            }
            #endregion
            #endregion
        }

        private BomMaster FindOrderDetailBom(OrderDetail orderDetail)
        {
            //Bom的选取顺序orderDetail.Bom(Copy from 路线明细) --> orderDetail.Item.Bom--> 用orderDetail.Item.Code作为BomCode
            if (string.IsNullOrWhiteSpace(orderDetail.Bom))
            {
                orderDetail.Bom = bomMgr.FindItemBom(orderDetail.Item);
            }

            try
            {
                BomMaster bom = genericMgr.FindById<BomMaster>(orderDetail.Bom);
                orderDetail.Bom = bom.Code;
                return bom;
            }
            catch (ObjectNotFoundException)
            {
                throw new BusinessException(Resources.PRD.Bom.Errors_ItemBomNotFound, orderDetail.Item);
            }
        }

        private void CheckOrderedQtyFulfillment(OrderMaster orderMaster, OrderDetail orderDetail)
        {
            if (orderMaster.IsOrderFulfillUC
                && !(orderMaster.IsAutoRelease && orderMaster.IsAutoStart) //快速的不考虑
                && orderMaster.SubType == com.Sconit.CodeMaster.OrderSubType.Normal)  //只考虑正常订单，退货/返工等不考虑
            {
                if (orderDetail.OrderedQty % orderDetail.UnitCount != 0)
                {
                    throw new BusinessException(Resources.ORD.OrderMaster.Errors_OrderQtyNotFulfillUnitCount, orderDetail.Item, orderDetail.UnitCount.ToString());
                }
            }
        }

        private IList<OrderDetail> ProcessNewOrderDetail(OrderDetail orderDetail, OrderMaster orderMaster, ref int seq)
        {
            IList<OrderDetail> activeOrderDetails = new List<OrderDetail>();

            if (orderDetail.OrderedQty != 0) //过滤数量为0的明细
            {
                #region 整包校验
                CheckOrderedQtyFulfillment(orderMaster, orderDetail);
                #endregion

                Item item = orderDetail.CurrentItem != null ? orderDetail.CurrentItem : genericMgr.FindById<Item>(orderDetail.Item);

                if (item.IsKit && false)  //暂时不支持套件
                {
                    #region 分解套件
                    //没有考虑套件下面还是套件的情况
                    IList<ItemKit> itemKitList = itemMgr.GetKitItemChildren(item.Code);

                    if (itemKitList != null && itemKitList.Count() > 0)
                    {
                        foreach (ItemKit kit in itemKitList)
                        {
                            //检查订单明细的零件类型
                            CheckOrderDetailItemType(kit.ChildItem, (com.Sconit.CodeMaster.OrderType)orderMaster.Type);

                            OrderDetail activeOrderDetail = new OrderDetail();
                            activeOrderDetail.OrderType = orderMaster.Type;
                            activeOrderDetail.OrderSubType = orderMaster.SubType;
                            activeOrderDetail.Sequence = ++seq;
                            activeOrderDetail.Item = kit.ChildItem.Code;
                            activeOrderDetail.ItemDescription = kit.ChildItem.Description;
                            activeOrderDetail.Uom = orderDetail.Uom;
                            activeOrderDetail.BaseUom = kit.ChildItem.Uom;
                            activeOrderDetail.UnitCount = orderDetail.UnitCount;
                            activeOrderDetail.RequiredQty = orderDetail.RequiredQty * kit.Qty;
                            activeOrderDetail.OrderedQty = orderDetail.OrderedQty * kit.Qty;
                            if (activeOrderDetail.Uom != kit.ChildItem.Uom)
                            {
                                activeOrderDetail.UnitQty = kit.Qty;
                            }
                            else
                            {
                                activeOrderDetail.UnitQty = itemMgr.ConvertItemUomQty(kit.ChildItem.Code, kit.ChildItem.Uom, kit.Qty, activeOrderDetail.Uom);
                            }
                            activeOrderDetail.ReceiveLotSize = orderDetail.ReceiveLotSize * kit.Qty;
                            activeOrderDetail.LocationFrom = orderDetail.LocationFrom;
                            activeOrderDetail.LocationFromName = orderDetail.LocationFromName;
                            activeOrderDetail.LocationTo = orderDetail.LocationTo;
                            activeOrderDetail.LocationToName = orderDetail.LocationToName;
                            activeOrderDetail.IsInspect = orderDetail.IsInspect;
                            //activeOrderDetail.InspectLocation = orderDetail.InspectLocation;
                            //activeOrderDetail.InspectLocationName = orderDetail.InspectLocationName;
                            //activeOrderDetail.RejectLocation = orderDetail.RejectLocation;
                            //activeOrderDetail.RejectLocationName = orderDetail.RejectLocationName;
                            activeOrderDetail.BillAddress = orderDetail.BillAddress;
                            activeOrderDetail.BillAddressDescription = orderDetail.BillAddressDescription;
                            activeOrderDetail.PriceList = orderDetail.PriceList;
                            activeOrderDetail.Routing = activeOrderDetail.Routing;
                            //activeOrderDetail.HuLotSize = activeOrderDetail.HuLotSize * kit.Qty;
                            activeOrderDetail.BillTerm = activeOrderDetail.BillTerm;
                            //activeOrderDetail.OldOption = CodeMaster.HuOption.

                            activeOrderDetails.Add(activeOrderDetail);
                        }
                    }
                    else
                    {
                        throw new BusinessException(Resources.MD.Item.Errors_ItemKit_ChildrenItemNotFound, orderDetail.Item);
                    }
                    #endregion
                }
                else
                {
                    orderDetail.Sequence = ++seq;
                    orderDetail.OrderType = orderMaster.Type;
                    orderDetail.OrderSubType = orderMaster.SubType;
                    orderDetail.BaseUom = item.Uom;
                    orderDetail.ItemDescription = item.Description;

                    #region 零件类型校验
                    CheckOrderDetailItemType(item, (com.Sconit.CodeMaster.OrderType)orderMaster.Type);
                    activeOrderDetails.Add(orderDetail);
                    #endregion

                    #region 设置和库存单位的转换
                    if (string.Compare(orderDetail.Uom, item.Uom) != 0)
                    {
                        orderDetail.UnitQty = itemMgr.ConvertItemUomQty(orderDetail.Item, orderDetail.Uom, 1, item.Uom);
                    }
                    else
                    {
                        orderDetail.UnitQty = 1;
                    }
                    #endregion
                }
            }

            return activeOrderDetails;
        }

        private void CheckOrderDetailItemType(Item item, com.Sconit.CodeMaster.OrderType orderType)
        {
            #region 零件类型校验
            if (!item.IsActive)
            {
                //零件不启用
                throw new BusinessException(Resources.ORD.OrderMaster.Errors_ItemInActive, item.Code);
            }
            //暂时不启用零件校验 
            else if (true)
            {
            }
            //else if (item.IsVirtual)
            //{
            //    //虚零件
            //    throw new BusinessException(Resources.ORD.OrderMaster.Errors_ItemIsVirtual, item.Code);
            //}
            //else if (orderType == com.Sconit.CodeMaster.OrderType.CustomerGoods && !item.IsCustomerGoods)
            //{
            //    throw new BusinessException(Resources.ORD.OrderMaster.Errors_ItemNotCustomerGoods, item.Code);
            //}
            //else if (orderType == com.Sconit.CodeMaster.OrderType.Production && !item.IsManufacture)
            //{
            //    throw new BusinessException(Resources.ORD.OrderMaster.Errors_ItemNotManufacture, item.Code);
            //}
            //else if ((orderType == com.Sconit.CodeMaster.OrderType.Procurement
            //    || orderType == com.Sconit.CodeMaster.OrderType.ScheduleLine) && !item.IsPurchase)
            //{
            //    throw new BusinessException(Resources.ORD.OrderMaster.Errors_ItemNotPurchase, item.Code);
            //}
            //else if (orderType == com.Sconit.CodeMaster.OrderType.Distribution && !item.IsSales)
            //{
            //    throw new BusinessException(Resources.ORD.OrderMaster.Errors_ItemNotSales, item.Code);
            //}
            //else if (orderType == com.Sconit.CodeMaster.OrderType.SubContract && !item.IsSubContract)
            //{
            //    throw new BusinessException(Resources.ORD.OrderMaster.Errors_ItemNotSubContract, item.Code);
            //}
            #endregion
        }

        private void CalculateOrderDetailPrice(OrderDetail orderDetail, OrderMaster orderMaster, DateTime? effectiveDate)
        {
            string priceList = !string.IsNullOrWhiteSpace(orderDetail.PriceList) ? orderDetail.PriceList : orderMaster.PriceList;

            if (string.IsNullOrWhiteSpace(priceList))
            {
                bool isAllowCreateOrderWithNoPrice = true;
                if (orderMaster.Type == com.Sconit.CodeMaster.OrderType.Distribution)
                {
                    isAllowCreateOrderWithNoPrice = bool.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.IsAllowCreateSalesOrderWithNoPrice));
                }
                else
                {
                    isAllowCreateOrderWithNoPrice = bool.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.IsAllowCreatePurchaseOrderWithNoPrice));
                }

                if (isAllowCreateOrderWithNoPrice)
                {
                    return;
                }
                else
                {
                    throw new BusinessException("没有指定价格单。");
                }
            }

            #region 币种
            PriceListMaster priceListMaster = orderDetail.CurrentPriceListMaster != null ? orderDetail.CurrentPriceListMaster : orderMaster.CurrentPriceListMaster;
            if (priceListMaster == null)
            {
                if (!string.IsNullOrWhiteSpace(orderDetail.PriceList))
                {
                    orderDetail.CurrentPriceListMaster = this.genericMgr.FindById<PriceListMaster>(orderDetail.PriceList);
                    priceListMaster = orderDetail.CurrentPriceListMaster;
                }
                else if (!string.IsNullOrWhiteSpace(orderMaster.PriceList))
                {
                    orderMaster.CurrentPriceListMaster = this.genericMgr.FindById<PriceListMaster>(orderMaster.PriceList);
                    priceListMaster = orderMaster.CurrentPriceListMaster;
                }
            }

            orderDetail.Currency = priceListMaster.Currency;
            #endregion

            #region 价格
            PriceListDetail priceListDetail = itemMgr.GetItemPrice(orderDetail.Item, orderDetail.Uom, priceList, orderMaster.Currency, effectiveDate);
            if (priceListDetail != null)
            {
                orderDetail.UnitPrice = priceListDetail.UnitPrice;
                orderDetail.IsProvisionalEstimate = priceListDetail.IsProvisionalEstimate;
                orderDetail.Tax = priceListDetail.PriceList.Tax;
                orderDetail.IsIncludeTax = priceListDetail.PriceList.IsIncludeTax;
            }
            #endregion
        }

        //private void CreateOrderOperation(OrderDetail orderDetail)
        //{
        //    if (orderDetail.OrderOperations != null && orderDetail.OrderOperations.Count() > 0)
        //    {
        //        foreach (OrderOperation orderOperation in orderDetail.OrderOperations)
        //        {
        //            orderOperation.OrderDetailId = orderDetail.Id;
        //            orderOperation.OrderNo = orderDetail.OrderNo;

        //            genericMgr.Create(orderOperation);
        //        }
        //    }
        //}

        //private void CreateOrderBomDetail(OrderDetail orderDetail)
        //{
        //    if (orderDetail.OrderBomDetails != null && orderDetail.OrderBomDetails.Count() > 0)
        //    {
        //        foreach (OrderBomDetail orderBomDetail in orderDetail.OrderBomDetails)
        //        {
        //            orderBomDetail.OrderNo = orderDetail.OrderNo;
        //            orderBomDetail.OrderType = orderDetail.OrderType;
        //            orderBomDetail.OrderSubType = orderDetail.OrderSubType;
        //            orderBomDetail.OrderDetailId = orderDetail.Id;
        //            orderBomDetail.OrderDetailSequence = orderDetail.Sequence;

        //            genericMgr.Create(orderBomDetail);
        //        }
        //    }
        //}

        private void AutoReceiveIp(IpMaster ipMaster, DateTime effectiveDate)
        {
            if (ipMaster.IsAutoReceive)
            {
                foreach (IpDetail ipDetail in ipMaster.IpDetails)
                {
                    if (ipDetail.IpDetailInputs != null && ipDetail.IpDetailInputs.Count > 0)
                    {
                        foreach (IpDetailInput ipDetailInput in ipDetail.IpDetailInputs)
                        {
                            ipDetailInput.ReceiveQty = ipDetailInput.ShipQty;
                        }
                    }
                    else
                    {
                        IpDetailInput ipDetailInput = new IpDetailInput();
                        ipDetailInput.ReceiveQty = ipDetail.Qty;
                        ipDetail.AddIpDetailInput(ipDetailInput);
                    }
                }

                this.genericMgr.FlushSession();
                ReceiveIp(ipMaster.IpDetails, effectiveDate);
            }
        }

        private IList<FlowDetail> TryLoadFlowDetails(FlowMaster flowMaster)
        {
            if (!string.IsNullOrWhiteSpace(flowMaster.Code))
            {
                if (flowMaster.FlowDetails == null)
                {
                    string hql = "from FlowDetail where Flow = ? order by Sequence";

                    flowMaster.FlowDetails = this.genericMgr.FindAll<FlowDetail>(hql, flowMaster.Code);
                }

                return flowMaster.FlowDetails;
            }
            else
            {
                return null;
            }
        }

        private IList<FlowDetail> TryLoadFlowDetails(FlowMaster flowMaster, IList<string> itemCodeList)
        {
            if (!string.IsNullOrWhiteSpace(flowMaster.Code))
            {
                if (flowMaster.FlowDetails == null)
                {
                    string hql = "from FlowDetail where Flow = ?";
                    IList<object> parm = new List<object>();
                    parm.Add(flowMaster.Code);

                    if (itemCodeList != null && itemCodeList.Count > 0 && itemCodeList.Count < 2000)
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

                    flowMaster.FlowDetails = this.genericMgr.FindAll<FlowDetail>(hql, parm.ToArray());
                    if (itemCodeList != null && itemCodeList.Count >= 2000)
                    {
                        flowMaster.FlowDetails = flowMaster.FlowDetails.Where(f => itemCodeList.Contains(f.Item)).ToList();
                    }
                }

                return flowMaster.FlowDetails;
            }
            else
            {
                return null;
            }
        }

        private IList<FlowBinding> TryLoadFlowBindings(FlowMaster flowMaster)
        {
            if (!string.IsNullOrWhiteSpace(flowMaster.Code))
            {
                if (flowMaster.FlowBindings == null)
                {
                    string hql = "from FlowBinding where MasterFlow.Code = ?";

                    flowMaster.FlowBindings = this.genericMgr.FindAll<FlowBinding>(hql, flowMaster.Code);
                }

                return flowMaster.FlowBindings;
            }
            else
            {
                return null;
            }
        }

        private IList<OrderDetail> TryLoadOrderDetails(OrderMaster orderMaster)
        {
            if (!string.IsNullOrWhiteSpace(orderMaster.OrderNo))
            {
                if (orderMaster.OrderDetails == null)
                {
                    string hql = "from OrderDetail where OrderNo = ? order by Sequence";

                    orderMaster.OrderDetails = this.genericMgr.FindAll<OrderDetail>(hql, orderMaster.OrderNo);
                }

                return orderMaster.OrderDetails;
            }
            else
            {
                return null;
            }
        }

        private IList<OrderBinding> TryLoadOrderBindings(OrderMaster orderMaster)
        {
            if (!string.IsNullOrWhiteSpace(orderMaster.OrderNo))
            {
                if (orderMaster.OrderBindings == null)
                {
                    string hql = "from OrderBinding where OrderNo = ?";

                    orderMaster.OrderBindings = this.genericMgr.FindAll<OrderBinding>(hql, orderMaster.OrderNo);
                }

                return orderMaster.OrderBindings;
            }
            else
            {
                return null;
            }
        }

        private IList<OrderOperation> TryLoadOrderOperations(OrderDetail orderDetail)
        {
            if (orderDetail.Id != 0)
            {
                if (orderDetail.OrderOperations == null)
                {
                    string hql = "from OrderOperation where OrderDetailId = ? order by Operation, OpReference";

                    orderDetail.OrderOperations = this.genericMgr.FindAll<OrderOperation>(hql, orderDetail.Id);
                }

                return orderDetail.OrderOperations;
            }
            else
            {
                return null;
            }
        }

        private IList<OrderOperation> TryLoadOrderOperations(OrderMaster orderMaster)
        {
            if (orderMaster.OrderNo != null)
            {
                TryLoadOrderDetails(orderMaster);

                IList<OrderOperation> orderOperationList = new List<OrderOperation>();

                string hql = string.Empty;
                IList<object> para = new List<object>();
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (orderDetail.OrderOperations != null && orderDetail.OrderOperations.Count > 0)
                    {
                        ((List<OrderOperation>)orderOperationList).AddRange(orderDetail.OrderOperations);
                    }
                    else
                    {
                        if (hql == string.Empty)
                        {
                            hql = "from OrderOperation where OrderDetailId in (?";
                        }
                        else
                        {
                            hql += ",?";
                        }
                        para.Add(orderDetail.Id);
                    }
                }

                if (hql != string.Empty)
                {
                    hql += ") order by OrderDetailId, Operation, OpReference";

                    ((List<OrderOperation>)orderOperationList).AddRange(this.genericMgr.FindAll<OrderOperation>(hql, para.ToArray()));
                }

                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (orderDetail.OrderOperations == null || orderDetail.OrderOperations.Count == 0)
                    {
                        orderDetail.OrderOperations = orderOperationList.Where(o => o.OrderDetId == orderDetail.Id).ToList();
                    }
                }

                return orderOperationList;
            }
            else
            {
                return null;
            }
        }

        private IList<OrderBomDetail> TryLoadOrderBomDetails(OrderMaster orderMaster)
        {
            if (orderMaster.OrderNo != null)
            {
                TryLoadOrderDetails(orderMaster);

                IList<OrderBomDetail> orderBomDetailList = new List<OrderBomDetail>();

                string hql = string.Empty;
                IList<object> para = new List<object>();
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (orderDetail.OrderBomDetails != null && orderDetail.OrderBomDetails.Count > 0)
                    {
                        ((List<OrderBomDetail>)orderBomDetailList).AddRange(orderDetail.OrderBomDetails);
                    }
                    else
                    {
                        if (hql == string.Empty)
                        {
                            hql = "from OrderBomDetail where OrderDetailId in (?";
                        }
                        else
                        {
                            hql += ",?";
                        }
                        para.Add(orderDetail.Id);
                    }
                }

                if (hql != string.Empty)
                {
                    hql += ") order by OrderDetailId, Operation, OpReference";

                    ((List<OrderBomDetail>)orderBomDetailList).AddRange(this.genericMgr.FindAll<OrderBomDetail>(hql, para.ToArray()));
                }

                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (orderDetail.OrderBomDetails == null || orderDetail.OrderBomDetails.Count == 0)
                    {
                        orderDetail.OrderBomDetails = orderBomDetailList.Where(o => o.OrderDetailId == orderDetail.Id).ToList();
                    }
                }

                return orderBomDetailList;
            }
            else
            {
                return null;
            }
        }

        private IList<OrderBomDetail> TryLoadOrderBomDetails(OrderDetail orderDetail)
        {
            if (orderDetail.Id != 0)
            {
                if (orderDetail.OrderBomDetails == null)
                {
                    string hql = "from OrderBomDetail where OrderDetailId = ? order by Operation, OpReference";

                    orderDetail.OrderBomDetails = this.genericMgr.FindAll<OrderBomDetail>(hql, orderDetail.Id);
                }

                return orderDetail.OrderBomDetails;
            }
            else
            {
                return null;
            }
        }

        private IList<OrderMaster> LoadOrderMasters(IEnumerable<string> orderNoList, bool isLoadCurrentFlowMaster = false)
        {
            var orderMasterList = this.genericMgr.FindAllIn<OrderMaster>
                (" from OrderMaster where OrderNo in (? ", orderNoList);
            if (isLoadCurrentFlowMaster)
            {
                var flowMasterList = this.genericMgr.FindAllIn<FlowMaster>
                    (" from FlowMaster where Code in (? ", orderMasterList.Where(p => !string.IsNullOrWhiteSpace(p.Flow)).Select(p => p.Flow).Distinct());
                foreach (var orderMaster in orderMasterList)
                {
                    if (!string.IsNullOrWhiteSpace(orderMaster.Flow))
                    {
                        orderMaster.CurrentFlowMaster = flowMasterList.Single(p => p.Code == orderMaster.Flow);
                    }
                }
            }
            return orderMasterList;
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

        private IList<OrderDetail> LoadExceptOrderDetail(OrderMaster orderMaster)
        {
            if (orderMaster.OrderDetails != null && orderMaster.OrderDetails.Count > 0)
            {
                string hql = string.Empty;
                IList<object> paras = new List<object>();
                foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                {
                    if (hql == string.Empty)
                    {
                        hql = "from OrderDetail where OrderNo = ? and Id not in (?";
                        paras.Add(orderMaster.OrderNo);
                    }
                    else
                    {
                        hql += ",?";
                    }
                    paras.Add(orderDetail.Id);
                }
                hql += ")";
                return this.genericMgr.FindAll<OrderDetail>(hql, paras.ToArray());
            }
            else
            {
                return this.TryLoadOrderDetails(orderMaster);
            }
        }

        private IList<IpDetail> LoadExceptIpDetails(string ipNo, int[] ipDetIdList)
        {
            IList<object> para = new List<object>();
            para.Add(ipNo);
            para.Add(false);   //未关闭的
            para.Add(com.Sconit.CodeMaster.IpDetailType.Normal);    //过滤掉差异明细，其实没有必要，因为一次性收货的送货单，在第一次收货时还没有生成差异明细
            string selectExceptIpDetailStatement = string.Empty;
            foreach (int id in ipDetIdList)
            {
                if (selectExceptIpDetailStatement == string.Empty)
                {
                    selectExceptIpDetailStatement = "from IpDetail where IpNo = ? and IsClose = ? and Type = ? and Id not in (?";
                }
                else
                {
                    selectExceptIpDetailStatement += ",?";
                }

                para.Add(id);
            }
            selectExceptIpDetailStatement += ")";

            return this.genericMgr.FindAll<IpDetail>(selectExceptIpDetailStatement, para.ToArray());
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

        private IList<IpLocationDetail> LoadExceptIpLocationDetails(string ipNo, int[] ipDetIdList)
        {
            IList<object> para = new List<object>();
            para.Add(ipNo);   //未关闭的
            para.Add(false);   //未关闭的
            string selectExceptIpLocationDetailStatement = string.Empty;
            foreach (int id in ipDetIdList)
            {
                if (selectExceptIpLocationDetailStatement == string.Empty)
                {
                    selectExceptIpLocationDetailStatement = "from IpLocationDetail where IpNo = ? and IsClose = ? and IpDetailId not in (?";
                }
                else
                {
                    selectExceptIpLocationDetailStatement += ",?";
                }

                para.Add(id);
            }
            selectExceptIpLocationDetailStatement += ")";

            return this.genericMgr.FindAll<IpLocationDetail>(selectExceptIpLocationDetailStatement, para.ToArray());
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
            }

            return pickListMaster.OrderDetails;
        }

        private IList<SequenceDetail> TryLoadSequenceDetails(SequenceMaster sequenceMaster)
        {
            if (sequenceMaster.SequenceDetails == null)
            {
                #region 获取排序单明细
                sequenceMaster.SequenceDetails = this.genericMgr.FindAll<SequenceDetail>("from SequenceDetail where SequenceNo = ? and IsClose = ? order by Sequence", new object[] { sequenceMaster.SequenceNo, false });
                #endregion
            }

            return sequenceMaster.SequenceDetails;
        }

        private IList<OrderDetail> TryLoadOrderDetails(SequenceMaster sequenceMaster)
        {
            if (sequenceMaster.OrderDetails == null)
            {
                TryLoadSequenceDetails(sequenceMaster);

                #region 获取订单明细
                string hql = string.Empty;
                IList<object> para = new List<object>();
                foreach (int orderDetailId in sequenceMaster.SequenceDetails.Select(p => p.OrderDetailId).Distinct())
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

                sequenceMaster.OrderDetails = this.genericMgr.FindAll<OrderDetail>(hql, para.ToArray());
                #endregion
            }

            return sequenceMaster.OrderDetails;
        }

        private IList<OrderMaster> TryLoadOrderMasters(SequenceMaster sequenceMaster)
        {
            if (sequenceMaster.OrderDetails == null)
            {
                TryLoadSequenceDetails(sequenceMaster);
            }

            string hql = string.Empty;
            IList<object> para = new List<object>();
            foreach (string orderNo in sequenceMaster.SequenceDetails.Select(p => p.OrderNo).Distinct())
            {
                if (hql == string.Empty)
                {
                    hql = "from OrderMaster where OrderNo in (?";
                }
                else
                {
                    hql += ",?";
                }

                para.Add(orderNo);
            }
            hql += ")";

            return this.genericMgr.FindAll<OrderMaster>(hql, para.ToArray());
        }

        private IList<FlowMaster> LoadFlowMaster(IList<OrderBinding> orderBindingList, DateTime effectiveDate)
        {
            if (orderBindingList != null && orderBindingList.Count > 0)
            {
                string selectFlowStatement = string.Empty;
                string selectFlowDetailStatement = string.Empty;
                IList<object> selectFlowPara = new List<object>();
                IList<object> selectFlowDetailPara = new List<object>();
                foreach (string flowCode in orderBindingList.Select(o => o.BindFlow).Distinct())
                {
                    if (selectFlowStatement == string.Empty)
                    {
                        selectFlowStatement = "from FlowMaster where Code in (?";
                        selectFlowDetailStatement = "from FlowDetail where (StartDate is null or StartDate <= ?) and (EndDate is null or EndDate >= ?) and Flow in (?";
                        selectFlowDetailPara.Add(effectiveDate);
                        selectFlowDetailPara.Add(effectiveDate);
                    }
                    else
                    {
                        selectFlowStatement += ",?";
                        selectFlowDetailStatement += ",?";
                    }

                    selectFlowPara.Add(flowCode);
                    selectFlowDetailPara.Add(flowCode);
                }

                selectFlowStatement += ")";
                selectFlowDetailStatement += ")";


                IList<FlowMaster> flowMasterList = this.genericMgr.FindAll<FlowMaster>(selectFlowStatement, selectFlowPara.ToArray());
                IList<FlowDetail> flowDetailList = this.genericMgr.FindAll<FlowDetail>(selectFlowDetailStatement, selectFlowDetailPara.ToArray());

                foreach (FlowMaster flowMaster in flowMasterList)
                {
                    flowMaster.FlowDetails = flowDetailList.Where(f => f.Flow == flowMaster.Code).ToList();
                }

                return flowMasterList;
            }

            return null;
        }
        #endregion

        #region 客户化功能
        #region 整车上线并投料（驾驶室上线投驾驶室，底盘上总装投底盘）
        [Transaction(TransactionMode.Requires)]
        public void StartVanOrder(string orderNo)
        {
            DoStartVanOrder(orderNo, null, null, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartVanOrder(string orderNo, string feedOrderNo)
        {
            DoStartVanOrder(orderNo, feedOrderNo, null, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartVanOrder(string orderNo, string feedOrderNo, bool isForce)
        {
            DoStartVanOrder(orderNo, feedOrderNo, null, isForce);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartVanOrder(string orderNo, IList<string> feedHuIdList)
        {
            DoStartVanOrder(orderNo, null, feedHuIdList, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartVanOrder(string orderNo, IList<string> feedHuIdList, bool isForce)
        {
            DoStartVanOrder(orderNo, null, feedHuIdList, isForce);
        }

        private void DoStartVanOrder(string orderNo, string feedOrderNo, IList<string> feedHuIdList, bool isForce)
        {
            #region 上线
            OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);

            #region 判断是否第一辆上线
            IList<long> beforeUnstartVanOrderCount = this.genericMgr.FindAll<long>("select count(*) as counter from OrderMaster where Type = ? and Flow = ? and Status = ? and IsPause = ? and Sequence < ?", new object[] { orderMaster.Type, orderMaster.Flow, CodeMaster.OrderStatus.Submit, false, orderMaster.Sequence });

            if (beforeUnstartVanOrderCount != null && beforeUnstartVanOrderCount.Count > 0 && beforeUnstartVanOrderCount[0] > 0)
            {
                throw new BusinessException("生产单{0}不是生产线{1}第一张待上线的生产单。", orderNo, orderMaster.Flow);
            }
            #endregion

            this.StartOrder(orderMaster);
            #endregion

            #region 子生产单投料
            if (!string.IsNullOrWhiteSpace(feedOrderNo))
            {
                this.productionLineMgr.FeedProductOrder(orderNo, feedOrderNo, isForce);
            }
            #endregion

            #region 物料投料
            if (feedHuIdList != null && feedHuIdList.Count > 0)
            {
                #region 查找投料条码
                IList<Hu> huList = this.huMgr.LoadHus(feedHuIdList);
                #endregion

                #region 查找投料工位
                string hql = string.Empty;
                IList<object> para = new List<object>();
                foreach (string item in huList.Select(h => h.Item).Distinct())
                {
                    if (hql == string.Empty)
                    {
                        hql = "from OrderBomDetail where OrderNo = ? and Item in (?";
                        para.Add(orderNo);
                    }
                    else
                    {
                        hql += ", ?";
                    }
                    para.Add(item);
                }
                hql += ")";

                IList<OrderBomDetail> orderBomDetailList = this.genericMgr.FindAll<OrderBomDetail>(hql, para.ToArray());

                #region 判断条码是否在OrderBomDetial中存在
                if (!isForce)
                {
                    BusinessException businessException = new BusinessException();
                    foreach (Hu hu in huList)
                    {
                        if (orderBomDetailList.Where(det => det.Item == hu.Item).Count() == 0)
                        {
                            businessException.AddMessage("投料的条码{0}在生产单的物料清单中不存在。", hu.HuId);
                        }
                    }

                    if (businessException.HasMessage)
                    {
                        throw businessException;
                    }
                }
                #endregion
                #endregion

                #region 投料
                foreach (string huId in feedHuIdList)
                {
                    Hu hu = huList.Where(h => h.HuId == huId).Single();
                    OrderBomDetail orderBomDetail = orderBomDetailList.Where(o => o.Item == hu.Item).OrderBy(o => o.Sequence).First();

                    FeedInput feedInput = new FeedInput();
                    feedInput.HuId = huId;
                    feedInput.Operation = orderBomDetail.Operation;
                    feedInput.OpReference = orderBomDetail.OpReference;

                    IList<FeedInput> feedInputList = new List<FeedInput>();
                    feedInputList.Add(feedInput);

                    this.productionLineMgr.FeedRawMaterial(orderNo, feedInputList, isForce);
                }
                #endregion
            }
            #endregion

            #region 释放驾驶室生产单
            //由后台Job自动释放
            #endregion

            #region 递延扣减
            //记录递延扣减需求，由后台Job自动扣减
            IList<DeferredFeedCounter> deferredFeedCounterList = this.genericMgr.FindAll<DeferredFeedCounter>("from DeferredFeedCounter where Flow = ?", orderMaster.Flow);
            if (deferredFeedCounterList != null && deferredFeedCounterList.Count > 0)
            {
                deferredFeedCounterList[0].Counter++;
                this.genericMgr.Update(deferredFeedCounterList[0]);
            }
            else
            {
                DeferredFeedCounter deferredFeedCounter = new DeferredFeedCounter();
                deferredFeedCounter.Flow = orderMaster.Flow;
                deferredFeedCounter.Counter = 1;
                this.genericMgr.Create(deferredFeedCounter);
            }
            #endregion
        }
        #endregion

        #region 空车上线
        [Transaction(TransactionMode.Requires)]
        public void StartEmptyVanOrder(string flow)
        {
            IList<DeferredFeedCounter> deferredFeedCounterList = this.genericMgr.FindAll<DeferredFeedCounter>("from DeferredFeedCounter where Flow = ?", flow);
            if (deferredFeedCounterList != null && deferredFeedCounterList.Count > 0)
            {
                deferredFeedCounterList[0].Counter++;
                this.genericMgr.Update(deferredFeedCounterList[0]);
            }
            else
            {
                DeferredFeedCounter deferredFeedCounter = new DeferredFeedCounter();
                deferredFeedCounter.Flow = flow;
                deferredFeedCounter.Counter = 1;
                this.genericMgr.Create(deferredFeedCounter);
            }
        }
        #endregion

        #region 整车/驾驶室/底盘下线
        [Transaction(TransactionMode.Requires)]
        public ReceiptMaster ReceiveVanOrder(string orderNo)
        {
            OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(orderNo);

            ProductLineMap productLineMap = null;
            IList<ProductLineMap> productLineMapList = genericMgr.FindAll<ProductLineMap>(@"from ProductLineMap as p 
                                                                                                  where (p.ProductLine = ? or p.CabFlow = ? or p.ChassisFlow = ?) 
                                                                                                  and p.ProductLine is not null 
                                                                                                  and p.CabFlow is not null 
                                                                                                  and p.ChassisFlow is not null", new object[] { orderMaster.Flow, orderMaster.Flow, orderMaster.Flow });

            if (productLineMapList != null && productLineMapList.Count > 0)
            {
                if (productLineMapList.Count > 1)
                {
                    throw new BusinessException("生产线{0}找到多条整车生产线映射。", orderMaster.Flow);
                }
                else
                {
                    productLineMap = productLineMapList.Single();
                }
            }
            else
            {
                throw new TechnicalException("收货的不是整车生产单。");
            }

            BusinessException businessException = VerifyVanOrderClose(orderMaster, productLineMap);

            #region 未扣减的工位扣减物料，只扣减自动投料至生产单的物料，不扣减收货回冲的物料
            IList<OrderBomDetail> orderBomDetailList = this.queryMgr.FindAllWithNativeSql<OrderBomDetail>(
                  NativeSqlStatement.SELECT_NOT_BACKFLUSH_ORDER_BOM_DETAIL_STATEMENT, new object[] { orderNo, false, CodeMaster.FeedMethod.ProductionOrder, true });

            BatchFeed(orderBomDetailList);

            if (orderBomDetailList.Count > 0)
            {
                if (orderMaster.CurrentOperation < orderBomDetailList.Max(det => det.Operation))
                {
                    orderMaster.CurrentOperation = orderBomDetailList.Max(det => det.Operation);
                    this.genericMgr.Update(orderMaster);
                }
            }
            #endregion

            IList<OrderDetail> orderDetailList = this.genericMgr.FindAll<OrderDetail>("from OrderDetail where OrderNo = ?", orderNo);
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                OrderDetailInput orderDetailInput = new OrderDetailInput();
                orderDetailInput.ReceiveQty = 1;
                orderDetail.AddOrderDetailInput(orderDetailInput);
            }

            ReceiptMaster receiptMaster = ReceiveOrder(orderDetailList, false, productLineMap, DateTime.Now, null);

            return receiptMaster;
        }
        #endregion

        #region 递延扣减
        private static Object DeferredFeedLock = new Object();
        [Transaction(TransactionMode.Requires)]
        public void DeferredFeed(string flow)
        {
            lock (DeferredFeedLock)
            {
                flow = flow.Trim();
                IList<DeferredFeedCounter> deferredFeedCounterList = this.genericMgr.FindAll<DeferredFeedCounter>("from DeferredFeedCounter where Flow = ? and Counter > 0", flow);

                if (deferredFeedCounterList != null && deferredFeedCounterList.Count > 0)
                {
                    try
                    {
                        log.Debug("Start deferred feed productline[" + flow + "].");

                        #region 获取待回冲生产单Bom
                        User user = SecurityContextHolder.Get();
                        IList<OrderBomDetail> orderBomDetailList =
                            this.genericMgr.FindAllWithNamedQuery<OrderBomDetail>("USP_Busi_GetDeferredFeedOrderBomDet", new object[] { flow, user.Id, user.FullName });
                        #endregion

                        #region 循环按工序投料
                        log.Debug("Deferred feed order bom detail count = " + (orderBomDetailList != null && orderBomDetailList.Count > 0 ? orderBomDetailList.Count : 0));
                        BatchFeed(orderBomDetailList);
                        #endregion

                        this.genericMgr.FlushSession();

                        #region 关闭计划暂停的生产单
                        this.genericMgr.FindAllWithNamedQuery<OrderBomDetail>("USP_Busi_PauseVanOrder");
                        this.genericMgr.Update("update from DeferredFeedCounter set Counter = Counter - 1 where Flow = ?", flow);
                        #endregion

                        log.Debug("Success deferred feed productline[" + flow + "].");
                    }
                    catch (Exception ex)
                    {
                        log.Error("Fail deferred feed.", ex);
                        this.genericMgr.CleanSession();
                        //todo: Email
                        throw ex;
                    }
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void BatchFeed(IList<OrderBomDetail> orderBomDetailList)
        {
            #region 循环按工序投料
            if (orderBomDetailList != null)
            {
                #region 根据工单，工位汇总
                var feedList = from bomDet in orderBomDetailList
                               group bomDet by new
                               {
                                   OrderNo = bomDet.OrderNo,
                                   Operation = bomDet.Operation,
                                   OpReference = bomDet.OpReference,
                                   ReserveNo = bomDet.ReserveNo,
                                   ReserveLine = bomDet.ReserveLine,
                                   AUFNR = bomDet.AUFNR,
                                   ICHARG = bomDet.ICHARG,
                                   BWART = bomDet.BWART,
                               } into g
                               select new
                               {
                                   OrderNo = g.Key.OrderNo,
                                   Operation = g.Key.Operation,
                                   OpReference = g.Key.OpReference,
                                   ReserveNo = g.Key.ReserveNo,
                                   ReserveLine = g.Key.ReserveLine,
                                   AUFNR = g.Key.AUFNR,
                                   ICHARG = g.Key.ICHARG,
                                   BWART = g.Key.BWART,
                                   List = g.ToList()
                               };
                #endregion

                #region 循环投料
                foreach (var feed in feedList)
                {
                    IList<FeedInput> feedInputList = (from l in feed.List
                                                      select new FeedInput
                                                      {
                                                          Item = l.Item,
                                                          Uom = l.Uom,
                                                          QualityType = CodeMaster.QualityType.Qualified,
                                                          Qty = l.OrderedQty,
                                                          LocationFrom = l.Location,
                                                          ReserveNo = l.ReserveNo,
                                                          ReserveLine = l.ReserveLine,
                                                          AUFNR = l.AUFNR,
                                                          ICHARG = l.ICHARG,
                                                          BWART = l.BWART,
                                                          Operation = feed.Operation,
                                                          OpReference = feed.OpReference,
                                                      }).ToList();

                    //递延扣减按数量投料，不要校验
                    this.productionLineMgr.FeedRawMaterial(feed.OrderNo, feedInputList);
                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region 分装生产单下线
        [Transaction(TransactionMode.Requires)]
        public void KitOrderOffline(IList<OrderDetail> orderDetailList, IList<string> feedKitOrderNoList)
        {
            this.KitOrderOffline(orderDetailList, feedKitOrderNoList, false, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void KitOrderOffline(IList<OrderDetail> orderDetailList, IList<string> feedKitOrderNoList, bool isForceFeed)
        {
            this.KitOrderOffline(orderDetailList, feedKitOrderNoList, isForceFeed, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public void KitOrderOffline(IList<OrderDetail> orderDetailList, IList<string> feedKitOrderNoList, DateTime effectiveDate)
        {
            this.KitOrderOffline(orderDetailList, feedKitOrderNoList, false, effectiveDate);
        }

        [Transaction(TransactionMode.Requires)]
        public void KitOrderOffline(IList<OrderDetail> orderDetailList, IList<string> feedKitOrderNoList, bool isForceFeed, DateTime effectiveDate)
        {
            #region 获取下线Kit单号
            if (orderDetailList.Select(det => det.OrderNo).Distinct().Count() > 1)
            {
                throw new BusinessException("分装生产单不能合并下线。");
            }
            string kitOrderNo = orderDetailList.Select(det => det.OrderNo).Distinct().Single();
            #endregion

            #region 查询Kit单和投料的Kit单
            List<string> orderNoList = new List<string>();
            orderNoList.Add(kitOrderNo);
            if (feedKitOrderNoList != null && feedKitOrderNoList.Count > 0)
            {
                orderNoList.AddRange(feedKitOrderNoList);
            }
            IList<OrderMaster> orderMasterList = this.LoadOrderMasters(orderNoList.ToArray());
            #endregion

            #region 检查
            OrderMaster kitOrder = orderMasterList.Where(o => o.OrderNo == kitOrderNo).Single();
            IList<OrderMaster> feedKitOrderList = orderMasterList.Where(o => o.OrderNo != kitOrderNo).ToList();

            if (kitOrder.OrderStrategy != CodeMaster.FlowStrategy.KIT)
            {
                throw new BusinessException("订单{0}的类型不是分装生产单。", kitOrder.OrderNo);
            }

            if (kitOrder.Status != CodeMaster.OrderStatus.Submit
                && kitOrder.Status != CodeMaster.OrderStatus.InProcess)
            {
                throw new BusinessException("分装生产单{0}的状态为{1}不能下线。", kitOrder.OrderNo,
                    systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, ((int)kitOrder.Status).ToString()));
            }

            if (feedKitOrderList != null && feedKitOrderList.Count > 0)
            {
                foreach (OrderMaster feedKitOrder in feedKitOrderList)
                {
                    if (feedKitOrder.OrderStrategy != CodeMaster.FlowStrategy.KIT)
                    {
                        throw new TechnicalException("Childe KitOrder strategy is not correct.");
                    }

                    if (feedKitOrder.Status != CodeMaster.OrderStatus.Close)
                    {
                        throw new BusinessException("子生产单{0}没有收货，不能投料。", feedKitOrder.OrderNo);
                    }

                    if (!isForceFeed && feedKitOrder.TraceCode != feedKitOrder.TraceCode)
                    {
                        throw new BusinessException("子生产单{0}的VAN号{1}和分装生产单的VAN号{2}不一致。", feedKitOrder.OrderNo, feedKitOrder.TraceCode, kitOrder.TraceCode);
                    }

                    if (this.genericMgr.FindAll<long>("select count(*) as counter from ProductFeed where FeedOrder = ?", feedKitOrder.OrderNo)[0] > 0)
                    {
                        throw new BusinessException("子生产单{0}已经投料。", feedKitOrder.OrderNo);
                    }
                }
            }
            #endregion

            #region 记录Kit单投料
            if (feedKitOrderList != null && feedKitOrderList.Count > 0)
            {
                foreach (OrderMaster feedKitOrder in feedKitOrderList)
                {
                    ProductFeed productFeed = new ProductFeed();
                    productFeed.TraceCode = feedKitOrder.TraceCode;
                    productFeed.FeedOrder = feedKitOrder.OrderNo;
                    productFeed.ProductOrder = kitOrderNo;

                    this.genericMgr.Create(productFeed);
                }
            }
            #endregion

            #region Kit收货
            this.ReceiveOrder(orderDetailList, effectiveDate);
            #endregion
        }
        #endregion

        #region 分装生产单下线并投料
        [Transaction(TransactionMode.Requires)]
        public void KitOrderOfflineAndFeed(string kitOrderNo)
        {
            IList<OrderDetail> orderDetailList = this.genericMgr.FindAll<OrderDetail>("from OrderDetail where OrderNo = ?", kitOrderNo);
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                if (orderDetail.IsScanHu)
                {
                    throw new BusinessException("分装生产单{0}含有关键件，不能下线并投料。", kitOrderNo);
                }
                else
                {
                    Entity.ORD.OrderDetailInput orderDetailInput = new Entity.ORD.OrderDetailInput();
                    orderDetailInput.ReceiveQty = orderDetail.OrderedQty;

                    orderDetail.AddOrderDetailInput(orderDetailInput);
                }
            }

            IList<string> orderNoList = this.genericMgr.FindAll<string>("select OrderNo from OrderBinding where BindOrderNo = ?", kitOrderNo);

            if (orderNoList != null && orderNoList.Count > 0)
            {
                if (orderNoList.Count > 1)
                {
                    throw new BusinessException("分装生产单{0}的母生产单数量大于1。", kitOrderNo);
                }
                else
                {
                    DateTime dateTimeNow = DateTime.Now;
                    KitOrderOffline(orderDetailList, null, false, dateTimeNow);
                    this.productionLineMgr.FeedKitOrder(orderNoList[0], kitOrderNo, false, dateTimeNow);
                }
            }
            else
            {
                throw new BusinessException("分装生产单{0}没有要投料的母生产单。", kitOrderNo);
            }
        }
        #endregion

        #region Kit单投Kit单(取消)
        //[Transaction(TransactionMode.Requires)]
        //public void FeedKitOrder(string parentKitOrderNo, string childKitOrderNo)
        //{
        //    FeedKitOrder(parentKitOrderNo, childKitOrderNo, false, DateTime.Now);
        //}

        //[Transaction(TransactionMode.Requires)]
        //public void FeedKitOrder(string parentKitOrderNo, string childKitOrderNo, bool isForceFeed)
        //{
        //    FeedKitOrder(parentKitOrderNo, childKitOrderNo, isForceFeed, DateTime.Now);
        //}

        //[Transaction(TransactionMode.Requires)]
        //public void FeedKitOrder(string parentKitOrderNo, string childKitOrderNo, DateTime effectiveDate)
        //{
        //    FeedKitOrder(parentKitOrderNo, childKitOrderNo, false, effectiveDate);
        //}

        //[Transaction(TransactionMode.Requires)]
        //public void FeedKitOrder(string parentKitOrderNo, string childKitOrderNo, bool isForceFeed, DateTime effectiveDate)
        //{
        //    #region 查询订单
        //    IList<OrderMaster> kitOrderMasterList = this.genericMgr.FindAll<OrderMaster>("from OrderMaster where OrderNo in (?, ?)",
        //        new object[] { parentKitOrderNo, childKitOrderNo });

        //    #endregion

        //    #region 检查
        //    OrderMaster parentKitOrder = kitOrderMasterList.Where(o => o.OrderNo == parentKitOrderNo).Single();
        //    OrderMaster childKitOrder = kitOrderMasterList.Where(o => o.OrderNo == childKitOrderNo).Single();

        //    if (parentKitOrder.OrderStrategy != CodeMaster.FlowStrategy.KIT)
        //    {
        //        throw new TechnicalException("Parent KitOrder strategy is not correct.");
        //    }

        //    if (childKitOrder.OrderStrategy != CodeMaster.FlowStrategy.KIT)
        //    {
        //        throw new TechnicalException("Childe KitOrder strategy is not correct.");
        //    }

        //    if (parentKitOrder.Status != CodeMaster.OrderStatus.Submit
        //        && parentKitOrder.Status != CodeMaster.OrderStatus.InProcess)
        //    {
        //        throw new BusinessException("父KIT单{0}的状态为{1}，不能投料。", parentKitOrder.OrderNo,
        //            systemMgr.GetCodeDetailDescription(com.Sconit.CodeMaster.CodeMaster.OrderStatus, parentKitOrder.Status.ToString()));
        //    }

        //    if (childKitOrder.Status != CodeMaster.OrderStatus.Close)
        //    {
        //        throw new BusinessException("子KIT单{0}没有收货，不能投料。", childKitOrder.OrderNo);
        //    }

        //    if (!isForceFeed && parentKitOrder.TraceCode != childKitOrder.TraceCode)
        //    {
        //        throw new BusinessException("子KIT单{0}的VAN号{1}和父KIT单的VAN号{2}不一致。", childKitOrder.OrderNo, childKitOrder.TraceCode, parentKitOrder.TraceCode);
        //    }

        //    #region 查询Kit单是否已经投料
        //    if (this.genericMgr.FindAll<long>("select count(*) as counter from ProductFeed where FeedOrder = ?", childKitOrder.OrderNo)[0] > 0)
        //    {
        //        throw new BusinessException("子KIT单{0}已经投料。", childKitOrder.OrderNo);
        //    }
        //    #endregion
        //    #endregion

        //    #region 记录Kit单投料
        //    ProductFeed productFeed = new ProductFeed();
        //    productFeed.TraceCode = parentKitOrder.TraceCode;
        //    productFeed.FeedOrder = childKitOrderNo;
        //    productFeed.ProductOrder = parentKitOrderNo;

        //    this.genericMgr.Create(productFeed);
        //    #endregion
        //}

        //private IList<string> NestGetChildKitOrderNo(string orderNo)
        //{
        //    IList<string> childKitOrderNoList = this.genericMgr.FindAll<string>("select FeedOrder from ProductFeed where ProductOrder = ?", orderNo);

        //    if (childKitOrderNoList != null && childKitOrderNoList.Count > 0)
        //    {
        //        foreach (string childKitOrderNo in childKitOrderNoList)
        //        {
        //            IList<string> nextChildKitOrderNoList = NestGetChildKitOrderNo(childKitOrderNo);
        //            if (nextChildKitOrderNoList != null && nextChildKitOrderNoList.Count > 0)
        //            {
        //                ((List<string>)childKitOrderNoList).AddRange(nextChildKitOrderNoList);
        //            }
        //        }
        //    }

        //    return childKitOrderNoList;
        //}

        //private void SetUom4FeedInput(IList<FeedInput> feedInputList)
        //{
        //    #region FeedInput的ITEM赋基本单位
        //    IList<string> itemCodeList = feedInputList.Where(f => !string.IsNullOrWhiteSpace(f.HuId)).Select(f => f.Item).Distinct().ToList();
        //    if (itemCodeList != null && itemCodeList.Count > 0)
        //    {
        //        string selectItemStatement = string.Empty;
        //        IList<object> selectItemParas = new List<object>();
        //        foreach (string itemCode in itemCodeList)
        //        {
        //            if (selectItemStatement == string.Empty)
        //            {
        //                selectItemStatement = "from Item where Code in (?";
        //            }
        //            else
        //            {
        //                selectItemStatement += ", ?";
        //            }
        //            selectItemParas.Add(itemCode);
        //        }
        //        selectItemStatement += ")";

        //        IList<Item> itemList = this.genericMgr.FindAll<Item>(selectItemStatement, selectItemParas.ToList());

        //        foreach (FeedInput feedInput in feedInputList.Where(f => !string.IsNullOrWhiteSpace(f.HuId)))
        //        {
        //            feedInput.Uom = itemList.Where(i => i.Code == feedInput.Item).Single().Uom; //基本单位
        //        }
        //    }
        //    #endregion
        //}
        #endregion
        #endregion

        #region 交货单过账
        [Transaction(TransactionMode.Requires)]
        public void DistributionReceiveOrder(OrderMaster orderMaster)
        {

            #region 检查交货单明细是否一起过账
            IList<OrderDetail> selectOrderDetailList = this.genericMgr.FindAll<OrderDetail>("select d from OrderDetail as d where d.OrderNo='" + orderMaster.OrderNo + "'");

            foreach (OrderDetail orderDetail in selectOrderDetailList)
            {
                bool b = false;
                foreach (OrderDetail item in orderMaster.OrderDetails)
                {
                    if (orderDetail.Id == item.Id)
                    {
                        b = true;
                        break;
                    }
                }

                if (b == false)
                {
                    string externalNo = this.genericMgr.FindById<OrderMaster>(orderDetail.OrderNo).ExternalOrderNo;
                    throw new BusinessException("交货单的明细必须一起过账。订单号为" + orderDetail.OrderNo);
                }
            }
            #endregion

            #region 把创建状态的OrderMaster释放
            this.genericMgr.CleanSession();
            if (orderMaster.Status == com.Sconit.CodeMaster.OrderStatus.Create)
            {
                this.ReleaseOrder(orderMaster);
            }

            #endregion

            #region 过账
            this.ReceiveOrder(orderMaster.OrderDetails);
            #endregion

            #region 把OrderMaster的状态关闭
            if (orderMaster.Status != com.Sconit.CodeMaster.OrderStatus.Close)
            {
                this.ManualCloseOrder(orderMaster);
            }
            #endregion

        }

        #endregion

        #region 异步打印
        //public void SendPrintData(OrderMaster orderMaster)

        private void AsyncSendPrintData(OrderMaster orderMaster)
        {
            if (orderMaster.SubType == CodeMaster.OrderSubType.Return)
            {
                return;
            }
            //AsyncSend asyncSend = new AsyncSend(this.SendPrintData);
            //asyncSend.BeginInvoke(masterData, isOrder, null, null);
            if (orderMaster.IsPrintOrder)
            {
                try
                {
                    var subPrintOrderList = this.genericMgr.FindAll<SubPrintOrder>();
                    var pubPrintOrders = subPrintOrderList.Where(p => (p.Flow == orderMaster.Flow || string.IsNullOrWhiteSpace(p.Flow))
                                && (p.UserId == orderMaster.ReleaseUserId || p.UserId == 0)
                                && (p.Region == orderMaster.PartyFrom || string.IsNullOrWhiteSpace(p.Region))
                                && (p.Location == orderMaster.LocationFrom || string.IsNullOrWhiteSpace(p.Location))
                                && p.ExcelTemplate == orderMaster.OrderTemplate)
                                .Select(p => new PubPrintOrder
                                {
                                    Client = p.Client,
                                    ExcelTemplate = p.ExcelTemplate,
                                    Code = orderMaster.OrderNo,
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

        public delegate void AsyncSend(EntityBase masterData, Boolean isOrder);
        #endregion

        [Transaction(TransactionMode.Requires)]
        public void DoItemTrace(string orderNo, List<string> huIdList)
        {
            if (huIdList != null)
            {
                var productionTraces = this.genericMgr.FindAllIn<ProductionTrace>("from ProductionTrace where OrderNo = ? and RmHu in( ? ",
                    huIdList, new object[] { orderNo });

                var flowFg = this.genericMgr.FindAllWithNativeSql<object[]>
                    ("select top 1 o.Flow,d.Item from ORD_OrderMstr_4 as o join ORD_OrderDet_4 as d on d.OrderNo = o.OrderNO where o.OrderNo =? ", orderNo);

                foreach (var huId in huIdList)
                {
                    var oldProductionTrace = productionTraces.FirstOrDefault(p => p.OrderNo == orderNo && p.RmHu == huId);
                    if (oldProductionTrace == null)
                    {
                        Hu hu = this.genericMgr.FindById<Hu>(huId);

                        ProductionTrace productionTrace = new ProductionTrace();
                        productionTrace.OrderNo = orderNo;
                        productionTrace.Flow = (string)flowFg[0][0];
                        productionTrace.Fg = (string)flowFg[0][1];
                        productionTrace.Item = hu.Item;
                        productionTrace.ItemDescription = hu.ItemDescription;
                        productionTrace.LotNo = hu.LotNo;

                        productionTrace.RmHu = huId;
                        this.genericMgr.Create(productionTrace);
                    }
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelItemTrace(string orderNo, List<string> huIdList)
        {
            var productionTraces = this.genericMgr.FindAllIn<ProductionTrace>("from ProductionTrace where OrderNo = ? and RmHu in( ? ",
                huIdList, new object[] { orderNo });

            if (huIdList != null)
            {
                foreach (var productionTrace in productionTraces)
                {
                    productionTrace.IsCancel = true;
                    this.genericMgr.Update(productionTrace);
                }
            }
        }

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


        [Transaction(TransactionMode.Requires)]
        public string PrintTraceCode()
        {
            ProdTraceCode prodTraceCode = new ProdTraceCode();
            prodTraceCode.TraceCode = numberControlMgr.GetTraceCode();
            this.genericMgr.Create(prodTraceCode);

            return prodTraceCode.TraceCode;
        }

        [Transaction(TransactionMode.Requires)]
        public string PrintTraceCode(string orderNo)
        {
            OrderDetail orderDetail = this.genericMgr.FindAll<OrderDetail>("from OrderDetail where OrderNo = ?", orderNo).Single();
            OrderOperation orderOperation = this.genericMgr.FindAll<OrderOperation>("from OrderOperation where OrderNo = ? order by Op", orderNo).First();

            ProdTraceCode prodTraceCode = new ProdTraceCode();
            prodTraceCode.TraceCode = numberControlMgr.GetTraceCode();
            prodTraceCode.OrderNo = orderDetail.OrderNo;
            prodTraceCode.OrderDetId = orderDetail.Id;
            prodTraceCode.Item = orderDetail.Item;
            prodTraceCode.ItemDesc = orderDetail.ItemDescription;
            prodTraceCode.OrderOp = orderOperation.Op;
            prodTraceCode.OrderOpId = orderOperation.Id;

            orderOperation.ReportQty++;

            this.genericMgr.Update(orderOperation);
            this.genericMgr.Create(prodTraceCode);

            return prodTraceCode.TraceCode;
        }


        [Transaction(TransactionMode.Requires)]
        public IList<Hu> ReceiveTraceCode(IList<OrderDetail> orderDetList, IList<String> traceCodes)
        {
            IList<Hu> huList = new List<Hu>();
            var recMaster = this.ReceiveOrder(orderDetList);
            var recDet = recMaster.ReceiptDetails.Single();
            recDet.UnitCount = recDet.ReceivedQty;
            var hu = this.huMgr.CreateHu(recMaster, recDet, DateTime.Now).Single();
            huList.Add(hu);
            var orderOp = this.genericMgr.FindAll<OrderOperation>("from OrderOperation where OrderDetId=? and Op=?", new object[] { recDet.OrderDetailId, 6 }).Single();
            orderOp.ReportQty = recDet.ReceivedQty;
            this.genericMgr.Update(orderOp);


            string updateSql = string.Empty;
            IList<object> param = new List<object>();
            param.Add(orderOp.Id);
            param.Add(hu.HuId);
            foreach (string tc in traceCodes)
            {
                if (string.IsNullOrEmpty(updateSql))
                {
                    updateSql += "update INP_ProdTraceCode set OrderOp=6, OrderOpId=?, HuId=?  where TraceCode in(?";
                    param.Add(tc);
                }
                else
                {
                    updateSql += ",?";
                    param.Add(tc);
                }
            }
            if (!string.IsNullOrEmpty(updateSql))
            {
                updateSql += ")";
            }
            this.genericMgr.FindAllWithNativeSql(updateSql, param.ToArray());



            return huList;

        }

        [Transaction(TransactionMode.Requires)]
        public void ReportOrderOp(int op)
        {
            OrderOperation orderOperation = this.genericMgr.FindAll<OrderOperation>("from OrderOperation where Op = ?", op).Single();
            ProdTraceCode prodTraceCode = this.genericMgr.FindAll<ProdTraceCode>("from ProdTraceCode where Op < ? order by Op, CreateDate", op).FirstOrDefault();

            prodTraceCode.OrderOp = orderOperation.Op;
            prodTraceCode.OrderOpId = orderOperation.Id;

            orderOperation.ReportQty++;

            this.genericMgr.Update(orderOperation);
            this.genericMgr.Update(prodTraceCode);
        }

        [Transaction(TransactionMode.Requires)]
        public string ReceiveTraceCode(IList<string> traceCodeList)
        {
            return string.Empty;
        }
    }



    [Transactional]
    public class SequenceMgrImpl : BaseMgr, ISequenceMgr
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.OrderMaster");
        //private IPublishing proxy;
        //public IPubSubMgr pubSubMgr { get; set; }

        public INumberControlMgr numberControlMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public IList<SequenceMaster> CreateSequenceOrderByFlow(FlowStrategy flowStrategy, IList<object[]> orderDetailAryList)
        {
            try
            {
                if (orderDetailAryList != null && orderDetailAryList.Count > 0)
                {
                    //待排序队列
                    IList<object[]> waitOrderDetailAryList = new List<object[]>();

                    #region 把排序单中数量大于1的排序件拆分为每个排序件数量为1
                    foreach (object[] orderDetailAry in orderDetailAryList)
                    {
                        //待排序零件的数量
                        decimal thisOrderDetailQty = (decimal)orderDetailAry[48];
                        //已收货排序件的数量
                        decimal thisShippedQty = (decimal)orderDetailAry[49];
                        //已经生成排序装箱单单的数量
                        int thisSequencedQty = (int)orderDetailAry[50];

                        for (int i = 0; i < thisOrderDetailQty; i++)
                        {
                            //复制一条排序信息
                            //object[] waitOrderDetailAry = Mapper.Map<object[], object[]>(orderDetailAry);
                            object[] waitOrderDetailAry = new object[orderDetailAry.Length];
                            for (int j = 0; j < orderDetailAry.Length; j++)
                            {
                                waitOrderDetailAry[j] = orderDetailAry[j];
                            }

                            //数量置为1
                            waitOrderDetailAry[48] = (decimal)1;

                            //先占用已收货数量，不够在占用已排序数量
                            //如果剩余已收货数量大于零，收货数置为1，代表不需要在配送
                            if (thisShippedQty > 0)
                            {
                                //已收货数量设为1
                                waitOrderDetailAry[49] = (decimal)1;
                                //已排序数量设为0
                                waitOrderDetailAry[50] = (decimal)0;
                                thisShippedQty--;
                            }
                            else
                            {
                                //已收货数量设为0
                                waitOrderDetailAry[49] = (decimal)0;

                                //如果剩余已排序数量大于零，排序数置为1，代表不需要在配送
                                if (thisSequencedQty > 0)
                                {
                                    //已排序数量设为1
                                    waitOrderDetailAry[50] = (decimal)1;
                                    thisSequencedQty--;
                                }
                                else
                                {
                                    //已排序数量设为0
                                    waitOrderDetailAry[50] = (decimal)0;
                                }
                            }

                            //添加排序信息至等待列表中
                            waitOrderDetailAryList.Add(waitOrderDetailAry);
                        }
                    }
                    #endregion

                    IList<SequenceMaster> sequenceMasterList = new List<SequenceMaster>();

                    #region 旧逻辑
                    //#region 符合等待批量创建
                    //int count = waitOrderDetailAryList.Count / flowStrategy.WaitBatch;

                    //for (int i = 0; i < count; i++)
                    //{
                    //    IList<object[]> batchWaitOrderDetailAryList = waitOrderDetailAryList.Skip(flowStrategy.WaitBatch * i).Take(flowStrategy.WaitBatch).ToList();
                    //    SequenceMaster sequenceMaster = CreateSequenceOrder(batchWaitOrderDetailAryList);
                    //    if (sequenceMaster != null)
                    //    {
                    //        sequenceMasterList.Add(sequenceMaster);
                    //    }
                    //}
                    //#endregion

                    //#region 符合等待时间创建
                    //if (flowStrategy.WaitBatch * count < waitOrderDetailAryList.Count())
                    //{
                    //    IList<object[]> batchWaitOrderDetailAryList = waitOrderDetailAryList.Skip(count * flowStrategy.WaitBatch).Take(waitOrderDetailAryList.Count() - count * flowStrategy.WaitBatch).ToList();
                    //    //第一条不符合等待批量的订单
                    //    object[] firstWaitOrderDetailAry = batchWaitOrderDetailAryList.First();

                    //    if (firstWaitOrderDetailAry != null && (DateTime)firstWaitOrderDetailAry[4] < DateTime.Now)  //没有等待时间的概念，只要第一张订单的开始时间小于当前时间
                    //    {
                    //        SequenceMaster sequenceMaster = CreateSequenceOrder(batchWaitOrderDetailAryList);
                    //        if (sequenceMaster != null)
                    //        {
                    //            sequenceMasterList.Add(sequenceMaster);
                    //        }
                    //    }
                    //}
                    //#endregion
                    #endregion

                    #region 新逻辑
                    while (waitOrderDetailAryList != null && waitOrderDetailAryList.Count > 0)
                    {
                        object[] firstWaitOrderDetailAry = waitOrderDetailAryList.First();
                        if (firstWaitOrderDetailAry != null && (DateTime)firstWaitOrderDetailAry[4] < DateTime.Now)  //没有等待时间的概念，只要第一张订单的开始时间小于当前时间
                        {
                            SequenceMaster sequenceMaster = CreateSequenceOrder(waitOrderDetailAryList.Take(waitOrderDetailAryList.Count > flowStrategy.WaitBatch ? flowStrategy.WaitBatch : waitOrderDetailAryList.Count).ToList());
                            if (sequenceMaster != null)
                            {
                                sequenceMasterList.Add(sequenceMaster);
                                waitOrderDetailAryList = waitOrderDetailAryList.Skip(sequenceMaster.SequenceDetails.Count).ToList();
                            }
                            else
                            {
                                waitOrderDetailAryList = null;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    #endregion

                    this.genericMgr.FlushSession();

                    return sequenceMasterList;
                }
            }
            catch (Exception ex)
            {
                log.Error("Fail generate sequence order.", ex);
                this.genericMgr.CleanSession();
            }

            return null;
        }

        private SequenceMaster CreateSequenceOrder(IList<object[]> orderDetailAryList)
        {
            if (orderDetailAryList == null || orderDetailAryList.Count() == 0)
            {
                return null;
            }

            string orderNos = string.Empty;
            foreach (string orderNo in orderDetailAryList.Select(o => (string)o[0]))
            {
                if (orderNos == string.Empty)
                {
                    orderNos = orderNo;
                }
                else
                {
                    orderNos += "," + orderNo;
                }
            }

            try
            {
                log.Debug("Start create sequence order, flow[" + orderDetailAryList[0][1] + "], orderNo in [" + orderNos + "].");

                SequenceMaster sequenceMaster = new SequenceMaster();

                sequenceMaster.SequenceNo = this.numberControlMgr.GetSequenceNo(sequenceMaster);
                sequenceMaster.Status = CodeMaster.SequenceStatus.Submit;
                sequenceMaster.Flow = (string)orderDetailAryList[0][1];
                sequenceMaster.OrderType = (CodeMaster.OrderType)((byte)orderDetailAryList[0][2]);
                sequenceMaster.QualityType = (CodeMaster.QualityType)((byte)orderDetailAryList[0][3]);
                sequenceMaster.StartTime = (DateTime)orderDetailAryList[0][4];
                sequenceMaster.WindowTime = (DateTime)orderDetailAryList[0][5];
                sequenceMaster.PartyFrom = (string)orderDetailAryList[0][6];
                sequenceMaster.PartyFromName = (string)orderDetailAryList[0][7];
                sequenceMaster.PartyTo = (string)orderDetailAryList[0][8];
                sequenceMaster.PartyToName = (string)orderDetailAryList[0][9];
                sequenceMaster.ShipFrom = (string)orderDetailAryList[0][10];
                sequenceMaster.ShipFromAddress = (string)orderDetailAryList[0][11];
                sequenceMaster.ShipFromTel = (string)orderDetailAryList[0][12];
                sequenceMaster.ShipFromCell = (string)orderDetailAryList[0][13];
                sequenceMaster.ShipFromFax = (string)orderDetailAryList[0][14];
                sequenceMaster.ShipFromContact = (string)orderDetailAryList[0][15];
                sequenceMaster.ShipTo = (string)orderDetailAryList[0][16];
                sequenceMaster.ShipToAddress = (string)orderDetailAryList[0][17];
                sequenceMaster.ShipToTel = (string)orderDetailAryList[0][18];
                sequenceMaster.ShipToCell = (string)orderDetailAryList[0][19];
                sequenceMaster.ShipToFax = (string)orderDetailAryList[0][20];
                sequenceMaster.ShipToContact = (string)orderDetailAryList[0][21];
                sequenceMaster.LocationFrom = (string)orderDetailAryList[0][22];
                sequenceMaster.LocationFromName = (string)orderDetailAryList[0][23];
                sequenceMaster.LocationTo = (string)orderDetailAryList[0][24];
                sequenceMaster.LocationToName = (string)orderDetailAryList[0][25];
                sequenceMaster.Dock = (string)orderDetailAryList[0][26];
                sequenceMaster.IsAutoReceive = (bool)orderDetailAryList[0][27];
                sequenceMaster.IsPrintAsn = (bool)orderDetailAryList[0][28];
                sequenceMaster.IsPrintReceipt = (bool)orderDetailAryList[0][29];
                sequenceMaster.IsCheckPartyFromAuthority = (bool)orderDetailAryList[0][30];
                sequenceMaster.IsCheckPartyToAuthority = (bool)orderDetailAryList[0][31];
                sequenceMaster.AsnTemplate = (string)orderDetailAryList[0][32];
                sequenceMaster.ReceiptTemplate = (string)orderDetailAryList[0][33];
                sequenceMaster.Container = (string)orderDetailAryList[0][46];
                sequenceMaster.ContainerDescription = (string)orderDetailAryList[0][47];

                this.genericMgr.Create(sequenceMaster);

                foreach (object[] orderDetailAry in orderDetailAryList)
                {
                    SequenceDetail sequenceDetail = new SequenceDetail();

                    sequenceDetail.SequenceNo = sequenceMaster.SequenceNo;
                    sequenceDetail.Sequence = (long)orderDetailAry[34];
                    sequenceDetail.OrderNo = (string)orderDetailAry[0];
                    sequenceDetail.TraceCode = (string)orderDetailAry[35];
                    sequenceDetail.OrderDetailId = (int)orderDetailAry[36];
                    sequenceDetail.OrderDetailSequence = (int)orderDetailAry[37];
                    sequenceDetail.StartTime = (DateTime)orderDetailAry[4];
                    sequenceDetail.WindowTime = (DateTime)orderDetailAry[5];
                    sequenceDetail.Item = (string)orderDetailAry[38];
                    sequenceDetail.ItemDescription = (string)orderDetailAry[39];
                    sequenceDetail.ReferenceItemCode = (string)orderDetailAry[40];
                    sequenceDetail.Uom = (string)orderDetailAry[41];
                    sequenceDetail.UnitQty = (decimal)orderDetailAry[42];
                    sequenceDetail.BaseUom = (string)orderDetailAry[43];
                    sequenceDetail.UnitCount = (decimal)orderDetailAry[44];
                    sequenceDetail.QualityType = (CodeMaster.QualityType)((byte)orderDetailAry[3]);
                    sequenceDetail.ManufactureParty = (string)orderDetailAry[45];
                    sequenceDetail.Qty = (decimal)orderDetailAry[48];
                    sequenceDetail.IsClose = (decimal)orderDetailAry[49] > 0 || (decimal)orderDetailAry[50] > 0; //已排序数量大于0或者收货数大于0，代表已经送过，要把位置空着

                    this.genericMgr.Create(sequenceDetail);
                    sequenceMaster.AddSequenceDetail(sequenceDetail);
                }

                //#region 发送WMS
                //string loc = systemMgr.GetEntityPreferenceValue(Entity.SYS.EntityPreference.CodeEnum.WMSAnjiRegion);
                //if (sequenceMaster.PartyFrom.Equals(loc, StringComparison.OrdinalIgnoreCase))
                //{
                //    //this.genericMgr.FlushSession();
                //    //this.AsyncRecourdMessageQueue(MethodNameType.CreateSeqOrder, sequenceMaster.SequenceNo);
                //    this.CreateMessageQueue("CreateSeqOrder", sequenceMaster.SequenceNo);
                //}
                //#endregion

                #region 发送打印
                this.AsyncSendPrintData(sequenceMaster, false);
                #endregion

                log.Debug("Success create sequence order, flow[" + orderDetailAryList[0][1] + "], orderNo in [" + orderNos + "]");

                return sequenceMaster;
            }
            catch (Exception ex)
            {
                log.Error("Fail create sequence order, flow[" + orderDetailAryList[0][1] + "], orderNo in [" + orderNos + "]", ex);

                //清空Session
                this.genericMgr.CleanSession();
            }

            return null;
        }

        private void UpdateOrderMasterStatus2InProcess(OrderMaster orderMaster)
        {
            orderMaster.Status = com.Sconit.CodeMaster.OrderStatus.InProcess;
            orderMaster.StartDate = DateTime.Now;
            User user = SecurityContextHolder.Get();
            orderMaster.StartUserId = user.Id;
            orderMaster.StartUserName = user.FullName;
            genericMgr.Update(orderMaster);
        }

        #region 异步打印
        //public void SendPrintData(OrderMaster orderMaster)
        public void SendPrintData(EntityBase masterData, Boolean isOrder)
        {

        }

        public void AsyncSendPrintData(EntityBase masterData, Boolean isOrder)
        {
            AsyncSend asyncSend = new AsyncSend(this.SendPrintData);
            asyncSend.BeginInvoke(masterData, isOrder, null, null);
        }

        public delegate void AsyncSend(EntityBase masterData, Boolean isOrder);
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
