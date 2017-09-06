using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LeanEngine;
using LeanEngine.Entity;
using LeanEngine.Utility;

namespace Testing
{
    [TestFixture]
    public class KBTest
    {
        private string itemCode;
        private string locFrom;
        private string locTo;

        private IEngine engine;
        private EngineContainer container;
        private Flow flow;
        private Item item;
        private ItemFlow itemFlow;
        private List<ItemFlow> ItemFlows;
        private InvBalance invBalance;
        private List<InvBalance> invBalances;
        private Plans plan;
        private List<Plans> plans;

        [SetUp]
        public void SetUp()
        {
            itemCode = "RM_A";
            locFrom = null;
            locTo = "RM";

            engine = new Engine();
            container = new EngineContainer();
            flow = new Flow();
            item = new Item();
            itemFlow = new ItemFlow();
            ItemFlows = new List<ItemFlow>();
            invBalance = new InvBalance();
            invBalances = new List<InvBalance>();
            plans = new List<Plans>();

            flow.FlowStrategy = new FlowStrategy();
            flow.FlowStrategy.Strategy = Enumerators.Strategy.KB;
            itemFlow.Flow = flow;
            item.ItemCode = itemCode;
            itemFlow.Item = item;
            itemFlow.LocFrom = locFrom;
            itemFlow.LocTo = locTo;
            itemFlow.MaxInv = 400;
            itemFlow.SafeInv = 200;
            itemFlow.UC = 50;
            ItemFlows.Add(itemFlow);

            string[] winTimes = new string[] { "11:00" };
            flow.FlowStrategy.MonWinTime = winTimes;
            flow.FlowStrategy.TueWinTime = winTimes;
            flow.FlowStrategy.WedWinTime = winTimes;
            flow.FlowStrategy.ThuWinTime = winTimes;
            flow.FlowStrategy.FriWinTime = winTimes;
            flow.FlowStrategy.SatWinTime = winTimes;
            flow.FlowStrategy.SunWinTime = winTimes;
            flow.WindowTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " 11:00");
            flow.NextOrderTime = null;

            invBalance.Item = item;
            invBalance.Loc = locTo;
            invBalance.Qty = 300;
            invBalances.Add(invBalance);

            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = DateTime.Now.AddDays(-1);
            plan.OrderedQty = 100;
            plan.FinishedQty = 80;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.RCT;
            plans.Add(plan);

            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = DateTime.Now;
            plan.OrderedQty = 50;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.RCT;
            plans.Add(plan);

            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = DateTime.Now.AddDays(2);
            plan.OrderedQty = 500;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.RCT;
            plans.Add(plan);

            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = DateTime.Now;
            plan.OrderedQty = 500;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Plans;
            plan.IRType = Enumerators.IRType.RCT;
            plans.Add(plan);

            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = DateTime.Now;
            plan.OrderedQty = 450;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.ISS;
            plans.Add(plan);

            container.ItemFlows = ItemFlows;
            container.InvBalances = invBalances;
            container.Plans = plans;
        }

        [Test]
        public void TellMeDemands1()
        {
            engine.TellMeDemands(container);
            Assert.AreEqual(30, itemFlow.ReqQty);
        }

        [Test]
        public void TellMeDemands2()
        {
            flow.OrderTime = DateTime.Now.AddHours(2);
            engine.TellMeDemands(container);
            Assert.AreEqual(0, itemFlow.ReqQty);
        }

        [Test]
        public void TellMeDemands3()
        {
            invBalance.Qty = 100;

            engine.TellMeDemands(container);
            Assert.AreEqual(230, itemFlow.ReqQty);
            Assert.AreEqual(true, itemFlow.IsEmergency);
        }

        #region RoundUp
        [Test]
        public void TellMeDemands_RoundUp1()
        {
            invBalance.Qty = 297;
            itemFlow.RoundUp = Enumerators.RoundUp.Ceiling;

            engine.TellMeDemands(container);
            var result = engine.DemandToOrders(container.ItemFlows);
            Assert.AreEqual(50, result[0].ItemFlows[0].OrderQty);
        }

        [Test]
        public void TellMeDemands_RoundUp2()
        {
            invBalance.Qty = 297;
            itemFlow.RoundUp = Enumerators.RoundUp.Floor;

            engine.TellMeDemands(container);
            var result = engine.DemandToOrders(container.ItemFlows);
            Assert.AreEqual(0, result[0].ItemFlows.Count);
        }

        [Test]
        public void TellMeDemands_RoundUp3()
        {
            invBalance.Qty = 297;

            engine.TellMeDemands(container);
            var result = engine.DemandToOrders(container.ItemFlows);
            Assert.AreEqual(33, result[0].ItemFlows[0].OrderQty);
        }
        #endregion

        [Test]
        public void Test_oldWindowTime()
        {
            flow.WindowTime = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 11:00");

            engine.TellMeDemands(container);
            var orders = engine.DemandToOrders(container.ItemFlows);
            Assert.AreEqual(0, orders[0].ItemFlows.Count);
            Assert.AreEqual(true, orders[0].Flow.IsUpdateWindowTime);
        }

        [Test]
        public void Test_IsUpdateWindowTime()
        {
            ItemFlow itemFlow1 = new ItemFlow();
            itemFlow1.Flow = flow;
            itemFlow1.Item = item;
            itemFlow1.LocFrom = locFrom;
            itemFlow1.LocTo = locTo;
            itemFlow1.MaxInv = 400;
            itemFlow1.SafeInv = 200;
            itemFlow1.UC = 50;
            ItemFlows.Add(itemFlow1);

            engine.TellMeDemands(container);
            var result = container.ItemFlows;
            Assert.AreEqual(true, result[0].Flow.IsUpdateWindowTime);
            Assert.AreEqual(true, result[1].Flow.IsUpdateWindowTime);
        }

        [Test]
        public void Test_LessThanOrderTime()
        {
            flow.OrderTime = DateTime.Now.AddHours(2);
            engine.TellMeDemands(container);
            var orders = engine.DemandToOrders(container.ItemFlows);
            Assert.AreEqual(0, orders.Count);
        }

        //BUG修正测试:2010-8-5,同一物料在到达发单时间时，在紧急单和正常单中都会产生需求
        [Test]
        public void Test_UrgentOrder()
        {
            flow.OrderTime = DateTime.Now;
            invBalance.Qty -= 200;//低于安全库存
            engine.TellMeDemands(container);
            var orders = engine.DemandToOrders(container.ItemFlows);
            Assert.AreEqual(2, orders.Count);
            Assert.AreEqual(1, orders[0].ItemFlows.Count);//紧急单覆盖需求
            Assert.AreEqual(0, orders[1].ItemFlows.Count);//正常单只更新窗口时间
            Assert.AreEqual(true, orders[1].Flow.IsUpdateWindowTime);
            Assert.AreEqual(true, orders[0].IsEmergency);
        }
    }
}
