// **************************************************************************************
// This is a solution for SCM(Supply Chain Management). It contains
// SCM(Supply Chain Model)/OAE(Order Automation Engine)/MPS(Master Production Schedule)/
// MRP(Material Requirements Planning)/APS(Advanced Planning and Scheduling) components.
// Author: Deng Xuyao.  Date: 2010-07.
// **************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LeanEngine.Business;
using LeanEngine.Entity;
using LeanEngine.OAE;
using LeanEngine.SCM;
using LeanEngine.Utility;

namespace LeanEngine
{
    /// <summary>
    /// Lean Engine
    /// </summary>
    public class Engine : IEngine, ISupplyChainMgr
    {
        private KB kb;
        private KB2 kb2;
        private JIT jit;
        private JIT2 jit2;
        private ODP odp;
        private ISupplyChainMgr TheSupplyChainMgr = new SupplyChainMgr();
        private List<RestTime> RestTimes;

        public List<SupplyChain> BuildSupplyChain(string itemCode, List<ItemFlow> ItemFlows)
        {
            return TheSupplyChainMgr.BuildSupplyChain(itemCode, ItemFlows);
        }

        public IOAE GetProcessor(Enumerators.Strategy strategy)
        {
            Dictionary<Enumerators.Strategy, IOAE> dic = new Dictionary<Enumerators.Strategy, IOAE>();
            dic.Add(Enumerators.Strategy.KB, kb);
            dic.Add(Enumerators.Strategy.KB2, kb2);
            dic.Add(Enumerators.Strategy.JIT, jit);
            dic.Add(Enumerators.Strategy.JIT2, jit2);
            dic.Add(Enumerators.Strategy.ODP, odp);

            if (dic.ContainsKey(strategy))
            {
                return dic[strategy];
            }
            return null;
        }

        public void TellMeDemands(EngineContainer container)
        {
            if (container == null || container.ItemFlows == null || container.ItemFlows.Count == 0)
                return;

            #region Container param
            List<ItemFlow> ItemFlows = container.ItemFlows;
            List<Plans> Plans = container.Plans;
            List<InvBalance> InvBalances = container.InvBalances;
            List<DemandChain> DemandChains = container.DemandChains;
            List<RestTime> RestTimes = container.RestTimes;
            #endregion

            #region Initialize
            kb = new KB(Plans, InvBalances, DemandChains, RestTimes);
            kb2 = new KB2(Plans, InvBalances, DemandChains, RestTimes);
            jit = new JIT(Plans, InvBalances, DemandChains, RestTimes);
            jit2 = new JIT2(Plans, InvBalances, DemandChains, RestTimes);
            odp = new ODP(Plans, InvBalances, DemandChains, RestTimes);

            this.RestTimes = container.RestTimes;
            #endregion

            #region Process time
            List<Flow> flows = ItemFlows.Select(s => s.Flow).Distinct().ToList<Flow>();
            this.ProcessTime(flows);
            #endregion

            #region Process ReqQty
            Parallel.ForEach(ItemFlows, (itemFlow) =>
            //foreach (var itemFlow in ItemFlows)
            {
                this.DataValidCheck(itemFlow);
                this.SetFlowProperty(itemFlow, flows);

                this.ProcessReqQty(itemFlow);
            }
            );
            #endregion
        }

        public List<Orders> DemandToOrders(List<ItemFlow> itemFlows)
        {
            #region ProcessOrderQty
            var query = from i in itemFlows
                        where i.ReqQty > 0
                        group i by new { i.LocTo, i.Item } into g
                        select new { g.Key, list = g.ToList(), Count = g.Count() };

            if (query != null && query.Count() > 0)
            {
                BidItemFlow bidItemFlow = new BidItemFlow();
                foreach (var g in query)
                {
                    if (g.Count == 1)
                    {
                        ItemFlow itemFlow = itemFlows.Single(i => i.Equals(g.list[0]));
                        this.ProcessOrderQty(itemFlow);
                    }
                    else
                    {
                        //bidding
                        ItemFlow winBidItemFlow = bidItemFlow.getWinBidItemFlow(g.list);
                        ItemFlow itemFlow = itemFlows.SingleOrDefault(i => i.Equals(winBidItemFlow));
                        this.ProcessOrderQty(itemFlow);
                    }
                }
            }
            #endregion

            List<Orders> result = new List<Orders>();
            #region Emergency Orders
            List<Orders> emOrders = this.BuildOrders(itemFlows, true);
            if (emOrders != null)
            {
                result = result.Concat(emOrders).ToList();
            }
            #endregion

            #region Normal Orders
            List<Orders> nmlOrders = this.BuildOrders(itemFlows, false);
            if (nmlOrders != null)
            {
                result = result.Concat(nmlOrders).ToList();
            }
            #endregion

            #region Filter
            result = result.Where(r => (r.IsEmergency && r.ItemFlows.Count > 0)
                || (!r.IsEmergency && (r.Flow.IsUpdateWindowTime || r.ItemFlows.Count > 0))).ToList();
            #endregion

            return result;
        }

        #region Data check
        private void DataValidCheck(ItemFlow itemFlow)
        {
            if (itemFlow.Flow == null)
            {
                throw new BusinessException("Flow is key infomation, it can't be empty!");
            }
        }
        private void DataValidCheck(Flow flow)
        {
            if (flow.FlowStrategy == null)
            {
                throw new BusinessException("FlowStrategy is key infomation, just tell me the strategy!");
            }
        }
        #endregion

        #region Setting properties
        private void SetFlowProperty(ItemFlow itemFlow, List<Flow> flows)
        {
            Flow flow = flows.Single(f => f.Equals(itemFlow.Flow));
            itemFlow.Flow.WindowTime = flow.WindowTime;
            itemFlow.Flow.NextOrderTime = flow.NextOrderTime;
            itemFlow.Flow.NextWindowTime = flow.NextWindowTime;
            itemFlow.Flow.IsUpdateWindowTime = flow.IsUpdateWindowTime;
        }
        #endregion

        #region Goto Processor
        private void ProcessTime(List<Flow> flows)
        {
            if (flows != null && flows.Count > 0)
            {
                foreach (var flow in flows)
                //Parallel.ForEach(flows, (flow) =>
                {
                    Thread.Sleep(10);
                    this.DataValidCheck(flow);
                    IOAE processor = this.GetProcessor(flow.FlowStrategy.Strategy);
                    if (processor != null)
                        processor.ProcessTime(flow);
                }
                //);
            }
        }

        private void ProcessReqQty(ItemFlow itemFlow)
        {
            IOAE processor = this.GetProcessor(itemFlow.Flow.FlowStrategy.Strategy);
            if (processor != null)
                processor.ProcessReqQty(itemFlow);
        }

        private void ProcessOrderQty(ItemFlow itemFlow)
        {
            if (itemFlow == null || itemFlow.ReqQty <= 0)
                return;

            decimal orderQty = itemFlow.ReqQty;
            decimal minLotSize = itemFlow.MinLotSize;//Min qty to order
            decimal UC = itemFlow.UC;//Unit container
            Enumerators.RoundUp roundUp = itemFlow.RoundUp;//Round up option
            //decimal orderLotSize = itemFlow.OrderLotSize;//Order lot size, one to many

            //Min lot size to order
            if (minLotSize > 0 && orderQty < minLotSize)
            {
                orderQty = minLotSize;
            }

            //round up
            if (UC > 0)
            {
                if (roundUp == Enumerators.RoundUp.Ceiling)
                {
                    orderQty = Math.Ceiling(orderQty / UC) * UC;
                }
                else if (roundUp == Enumerators.RoundUp.Floor)
                {
                    orderQty = Math.Floor(orderQty / UC) * UC;
                }
            }
            itemFlow.OrderQty = orderQty;

            //Order lot size, only production support
            //if (itemFlow.Flow.FlowType == Enumerators.FlowType.Production && orderLotSize > 0)
            //{
            //    itemFlow.OrderQtyList = this.SplitOrderByLotSize(orderQty, orderLotSize);
            //}
        }

        private List<decimal> SplitOrderByLotSize(decimal orderQty, decimal orderLotSize)
        {
            if (orderQty <= 0 || orderLotSize <= 0)
                return null;

            List<decimal> orderQtyList = new List<decimal>();
            if (orderLotSize > 0)
            {
                int count = (int)Math.Floor(orderQty / orderLotSize);
                for (int i = 0; i < count; i++)
                {
                    orderQtyList.Add(orderLotSize);
                }

                decimal oddQty = orderQty % orderLotSize;
                if (oddQty > 0)
                {
                    orderQtyList.Add(oddQty);
                }
            }

            return orderQtyList;
        }
        #endregion

        #region Build Orders
        private List<Orders> BuildOrders(List<ItemFlow> itemFlows, bool isEmergency)
        {
            if (itemFlows == null || itemFlows.Count == 0)
                return null;

            #region 非JIT2的ItemFlow转订单
            List<Orders> ordersList =
                    (from i in itemFlows
                     where i.Flow.FlowStrategy.Strategy != Enumerators.Strategy.JIT2
                            && (i.IsEmergency == isEmergency || (!isEmergency && i.Flow.IsUpdateWindowTime))
                     group i by i.Flow into g
                     select new Orders
                     {
                         Flow = g.Key,
                         IsEmergency = isEmergency,
                         ItemFlows = g.Where(i => i.OrderQty > 0 && i.IsEmergency == isEmergency).ToList(),
                         WindowTime = this.GetWindowTime(g.Key, isEmergency),
                         StartTime = this.GetStartTime(g.Key, isEmergency)
                     }).ToList();
            #endregion

            #region JIT2的ItemFlow转订单
            var groupedItemFlows = from i in itemFlows
                                   where i.Flow.FlowStrategy.Strategy == Enumerators.Strategy.JIT2
                                          && (i.IsEmergency == isEmergency || (!isEmergency && i.Flow.IsUpdateWindowTime))
                                   group i by i.Flow into g
                                   select new
                                   {
                                       Flow = g.Key,
                                       IsEmergency = isEmergency,
                                       WindowTime = this.GetWindowTime(g.Key, isEmergency),
                                       StartTime = this.GetStartTime(g.Key, isEmergency),
                                       List = g.ToList(),
                                   };

            foreach (var groupedItemFlow in groupedItemFlows)
            {
                Orders orders = new Orders();
                orders.Flow = groupedItemFlow.Flow;
                orders.IsEmergency = groupedItemFlow.IsEmergency;
                orders.WindowTime = groupedItemFlow.WindowTime;
                orders.StartTime = groupedItemFlow.StartTime;
                orders.ItemFlows = new List<ItemFlow>();

                ordersList.Add(orders);

                foreach (ItemFlow itemFlow in groupedItemFlow.List.Where(i => i.OrderQty > 0))
                {
                    var groupedOTList = from l in itemFlow.OrderTracers
                                        group l by l.Location into gj
                                        select new
                                        {
                                            Location = gj.Key,
                                            List = gj.ToList()
                                        };

                    #region 找出Buff库位的剩余库存
                    decimal bufQty = 0;   //Buff的剩余库存，待收+库存+待验-待发-需求
                    var bufOT = groupedOTList.Where(ot => ot.Location == itemFlow.LocTo).SingleOrDefault();

                    if (bufOT != null)
                    {
                        decimal totalQty = bufOT.List.Sum(l => l.Qty);
                        decimal issQty = bufOT.List.Where(l =>
                            l.TracerType == Enumerators.TracerType.Demand ||
                            l.TracerType == Enumerators.TracerType.OrderIss)
                            .Sum(l => l.Qty);

                        decimal rctQty = totalQty - issQty; //待收+库存
                        bufQty = rctQty - issQty;
                    }

                    #region Buff库位本身需要产生拉动
                    if (bufQty < 0)
                    {
                        ItemFlow bufItemFlow = Mapper.Map<ItemFlow, ItemFlow>(itemFlow);
                        bufItemFlow.OrderTracers = bufOT.List;
                        bufItemFlow.ReqQty = -bufQty;
                        this.ProcessOrderQty(bufItemFlow);

                        orders.ItemFlows.Add(bufItemFlow);
                    }
                    #endregion
                    #endregion

                    #region 根据需求时间、库位、类型的先后顺序扣减Buf区库存
                    if (bufQty > 0)
                    {
                        #region 线边先按库位扣减收发
                        foreach (var groupedOT in groupedOTList.Where(ot => ot.Location != itemFlow.LocTo))
                        {
                            foreach (var issOT in groupedOT.List.Where(ot => ot.Qty > 0
                                        && (ot.TracerType == Enumerators.TracerType.Demand ||
                                            ot.TracerType == Enumerators.TracerType.OrderIss))
                                            .OrderBy(ot => ot.ReqTime))
                            {
                                foreach (var rctOT in groupedOT.List.Where(ot => ot.Qty > 0
                                                                        && (ot.TracerType == Enumerators.TracerType.OnhandInv ||
                                                                        ot.TracerType == Enumerators.TracerType.InspectInv ||
                                                                        ot.TracerType == Enumerators.TracerType.OrderRct))
                                                                            .OrderBy(ot => ot.ReqTime))
                                {
                                    if (issOT.Qty > rctOT.Qty)
                                    {
                                        issOT.Qty -= rctOT.Qty;
                                        rctOT.Qty = 0;
                                    }
                                    else
                                    {
                                        rctOT.Qty -= issOT.Qty;
                                        issOT.Qty = 0;
                                    }

                                    if (issOT.Qty == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 在扣减Buf的库存
                        foreach (var issOT in itemFlow.OrderTracers.Where(
                            ot => ot.Location != itemFlow.LocTo && ot.Qty > 0
                            && (ot.TracerType == Enumerators.TracerType.Demand ||
                                ot.TracerType == Enumerators.TracerType.OrderIss))
                                .OrderBy(ot => ot.ReqTime).ThenBy(ot => ot.Location).ThenBy(ot => ot.TracerType))
                        {
                            if (issOT.Qty > bufQty)
                            {
                                issOT.Qty -= bufQty;
                                bufQty = 0;
                            }
                            else
                            {
                                bufQty -= issOT.Qty;
                                issOT.Qty = 0;
                            }

                            if (bufQty == 0)
                            {
                                break;
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region 循环各个其它需求源的需求
                    foreach (var groupedOT in groupedOTList.Where(ot => ot.Location != itemFlow.LocTo))
                    {
                        decimal thisReqQty = 0;
                        decimal totalQty = groupedOT.List.Sum(l => l.Qty);
                        decimal issQty = groupedOT.List.Where(l =>
                            l.TracerType == Enumerators.TracerType.Demand ||
                            l.TracerType == Enumerators.TracerType.OrderIss)
                            .Sum(l => l.Qty);

                        decimal rctQty = totalQty - issQty;//待收+库存

                        #region 创建需求，如果单个需求源的待收+库存 > 待发，多余的库存不纳入考虑范围
                        if (issQty > rctQty)
                        {
                            thisReqQty = issQty - rctQty;

                            ItemFlow lineItemFlow = Mapper.Map<ItemFlow, ItemFlow>(itemFlow);
                            lineItemFlow.OrderTracers = groupedOT.List;
                            lineItemFlow.ReqQty = thisReqQty;
                            this.ProcessOrderQty(lineItemFlow);

                            orders.ItemFlows.Add(lineItemFlow);
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            #endregion

            return ordersList;
        }

        private DateTime GetWindowTime(Flow flow, bool isEmergency)
        {
            if (isEmergency)
                return DateTime.Now.AddHours(flow.FlowStrategy.EmLeadTime);
            else
                return flow.WindowTime.HasValue ? flow.WindowTime.Value.AddHours(-flow.FlowStrategy.WinTimeDiff) : DateTime.Now.AddHours(flow.FlowStrategy.LeadTime);
        }

        private DateTime GetStartTime(Flow flow, bool isEmergency)
        {
            if (isEmergency)
            {
                return DateTime.Now;
            }
            else
            {
                DateTime windowTime = this.GetWindowTime(flow, isEmergency);
                return Utilities.GetStartTime(windowTime, flow.FlowStrategy.LeadTime, this.RestTimes);
            }
        }
        #endregion

        private DateTime NestCalNextStartTime(DateTime baseTime, DateTime startTime)
        {
            if (this.RestTimes != null && this.RestTimes.Count > 0)
            {
                List<RestTime> restTimes = this.RestTimes.Where(rt => rt.EndTime > baseTime
                    && rt.StartTime < startTime).ToList();

                if (restTimes != null && restTimes.Count > 0)
                {
                    DateTime nextWindowTime = new DateTime(startTime.Ticks);
                    foreach (RestTime rt in restTimes)
                    {
                        nextWindowTime = nextWindowTime.Add((rt.EndTime > startTime ? startTime : rt.EndTime).Subtract((rt.StartTime > baseTime ? rt.StartTime : baseTime)));
                    }

                    startTime = NestCalNextStartTime(startTime, nextWindowTime);
                }
            }

            return startTime;
        }

        #region Rest Time
        private void ProcessRestTime(Flow flow, List<RestTime> restTimes)
        {
            if (!flow.WindowTime.HasValue)
                return;

            DateTime winTime = flow.WindowTime.Value;
            var query = restTimes.Where(r => r.StartTime < winTime && r.EndTime > winTime).OrderBy(r => r.StartTime).ToList();

            //party/region rest time
            var q1 = query.Where(r => r.Party.Equals(flow.PartyTo)).ToList();
            if (q1.Count > 0)
            {
                flow.WindowTime = q1[0].EndTime;
                return;
            }

            //global
            var q2 = query.Where(r => r.Party == null).ToList();
            if (q2.Count > 0)
            {
                flow.WindowTime = q2[0].EndTime;
                return;
            }
        }
        #endregion
    }
}
