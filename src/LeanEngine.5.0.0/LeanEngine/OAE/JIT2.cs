using System;
using System.Collections.Generic;
using System.Linq;
using LeanEngine.Entity;
using LeanEngine.Utility;
using System.Threading.Tasks;

namespace LeanEngine.OAE
{
    /// <summary>
    /// Just In Time
    /// </summary>
    public class JIT2 : JIT
    {
        public JIT2(List<Plans> Plans, List<InvBalance> InvBalances, List<DemandChain> DemandChains, List<RestTime> RestTimes)
            : base(Plans, InvBalances, DemandChains, RestTimes)
        {
            return;
        }

        protected override decimal GetReqQty(ItemFlow itemFlow)
        {
            DateTime? orderTime = itemFlow.Flow.OrderTime;
            DateTime? winTime = itemFlow.Flow.WindowTime;
            DateTime? nextWinTime = itemFlow.Flow.NextWindowTime;

            #region Demand
            OrderTracer demand = this.GetDemand_OrderTracer(itemFlow);
            itemFlow.AddOrderTracer(demand);
            #endregion

            foreach (var loc in itemFlow.DemandSources)
            {
                #region Demand
                var demands = this.GetOrderIss(loc, itemFlow.Item, winTime, nextWinTime, Enumerators.TracerType.Demand);
                itemFlow.AddOrderTracer(demands);
                #endregion

                #region OnhandInv
                OrderTracer onhandInv = this.GetOnhandInv_OrderTracer(loc, itemFlow.Item);
                itemFlow.AddOrderTracer(onhandInv);
                #endregion

                #region InspectInv
                OrderTracer inspectInv = this.GetInspectInv_OrderTracer(loc, itemFlow.Item);
                itemFlow.AddOrderTracer(inspectInv);
                #endregion

                #region OrderRct
                var orderRcts = this.GetOrderRct(loc, itemFlow.Item, null, winTime);
                itemFlow.AddOrderTracer(orderRcts);
                #endregion

                #region OrderIss
                DateTime? startTime = null;
                if (true)//todo,config
                {
                    startTime = orderTime;
                }
                //var orderIsss = this.GetOrderIss(loc, itemFlow.Item, startTime, winTime);
                //考虑过期的待发需求
                var orderIsss = this.GetOrderIss(loc, itemFlow.Item, null, winTime);
                itemFlow.AddOrderTracer(orderIsss);
                #endregion
            }

            decimal reqQty = this.GetReqQty(itemFlow.LocTo, itemFlow.OrderTracers);

            return reqQty;
        }

        protected decimal GetReqQty(string loc, List<OrderTracer> list)
        {
            decimal reqQty = 0;
            if (list != null && list.Count > 0)
            {
                var groupedOTList = from l in list
                                    group l by l.Location into gj
                                    select new
                                    {
                                        Location = gj.Key,
                                        List = gj.ToList()
                                    };

                #region 找出Buff库位的剩余库存
                decimal bufQty = 0;   //Buff的剩余库存，待收+库存+待验-待发-需求
                var bufOT = groupedOTList.Where(ot => ot.Location == loc).SingleOrDefault();

                if (bufOT != null)
                {
                    decimal totalQty = 0;
                    decimal issQty = 0;
                    foreach (var l in bufOT.List)
                    {
                        totalQty += l.Qty;
                        if (l.TracerType == Enumerators.TracerType.Demand ||
                            l.TracerType == Enumerators.TracerType.OrderIss)
                        {
                            issQty += l.Qty;
                        }
                    }

                    decimal rctQty = totalQty - issQty; //待收+库存
                    bufQty = rctQty - issQty;
                }
                #endregion

                #region 循环各个其它需求源的需求
                foreach (var groupedOT in groupedOTList.Where(ot => ot.Location != loc))
                {
                    decimal totalQty = 0;
                    decimal issQty = 0;
                    foreach (var l in groupedOT.List)
                    {
                        totalQty += l.Qty;
                        if (l.TracerType == Enumerators.TracerType.Demand ||
                            l.TracerType == Enumerators.TracerType.OrderIss)
                        {
                            issQty += l.Qty;
                        }
                    }

                    decimal rctQty = totalQty - issQty;//待收+库存

                    #region 如果单个需求源的待收+库存 > 待发，多余的库存不纳入考虑范围
                    if (issQty > rctQty)
                    {
                        reqQty += issQty - rctQty;
                    }
                    #endregion
                }
                #endregion

                #region 如果其它需求源的需求小于Buf的库存，不用产生拉动
                if (reqQty - bufQty < 0)
                {
                    reqQty = 0;
                }
                else
                {
                    reqQty -= bufQty;
                }
                #endregion
            }

            return reqQty;
        }
    }
}