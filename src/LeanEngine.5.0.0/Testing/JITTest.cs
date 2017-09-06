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
    public class JITTest
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
        private DateTime time;
        private List<DemandChain> demandChains;

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
            demandChains = new List<DemandChain>();

            flow.Code = "Flow01";
            item.ItemCode = itemCode;
            flow.FlowStrategy = new FlowStrategy();
            flow.FlowStrategy.Strategy = Enumerators.Strategy.JIT;
            itemFlow.Flow = flow;
            itemFlow.Item = item;
            itemFlow.LocFrom = locFrom;
            itemFlow.LocTo = locTo;
            itemFlow.MaxInv = 0;
            itemFlow.SafeInv = 50;
            ItemFlows.Add(itemFlow);

            time = DateTime.Parse(DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH") + ":00");
            string[] winTimes = new string[] { "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00",
                "10:00","11:00", "12:00","13:00", "14:00","15:00", "16:00","17:00", "18:00","19:00", "20:00","21:00", "22:00","23:00" };
            flow.FlowStrategy.MonWinTime = winTimes;
            flow.FlowStrategy.TueWinTime = winTimes;
            flow.FlowStrategy.WedWinTime = winTimes;
            flow.FlowStrategy.ThuWinTime = winTimes;
            flow.FlowStrategy.FriWinTime = winTimes;
            flow.FlowStrategy.SatWinTime = winTimes;
            flow.FlowStrategy.SunWinTime = winTimes;
            flow.WindowTime = time;
            flow.OrderTime = DateTime.Now;
            flow.NextOrderTime = null;
            flow.FlowStrategy.LeadTime = 0.5;

            invBalance.Item = item;
            invBalance.Loc = locTo;
            invBalance.Qty = 100;
            invBalances.Add(invBalance);

            //RCT:20
            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = DateTime.Now.AddDays(-1);
            plan.OrderedQty = 100;
            plan.FinishedQty = 80;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.RCT;
            plans.Add(plan);

            //RCT:50
            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = DateTime.Now;
            plan.OrderedQty = 50;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.RCT;
            plans.Add(plan);

            //RCT:30(Ignore)
            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = time;
            plan.OrderedQty = 30;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.RCT;
            plans.Add(plan);

            //ISS:120
            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = DateTime.Now;
            plan.OrderedQty = 120;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.ISS;
            plans.Add(plan);

            //ISS:500(Ignore)
            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = time.AddHours(1);
            plan.OrderedQty = 500;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.ISS;
            plans.Add(plan);

            //ISS:450(Demand)
            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.ReqTime = time;
            plan.OrderedQty = 450;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.ISS;
            plans.Add(plan);

            container.ItemFlows = ItemFlows;
            container.InvBalances = invBalances;
            container.Plans = plans;
            container.DemandChains = demandChains;
        }

        [Test]
        public void TellMeDemands1()
        {
            engine.TellMeDemands(container);
            Assert.AreEqual(420, itemFlow.ReqQty);
        }

        [Test]
        public void TellMeDemands2()
        {
            flow.OrderTime = DateTime.Now.AddHours(2);
            engine.TellMeDemands(container);
            Assert.AreEqual(0, itemFlow.ReqQty);
        }

        //[Test]
        //public void TellMeDemands3()
        //{
        //    //ISS:500(Demand Plan)
        //    plan = new Plans();
        //    plan.Loc = locTo;
        //    plan.Item = item;
        //    plan.ReqTime = time.AddHours(0.5);
        //    plan.OrderedQty = 500;
        //    plan.FinishedQty = 0;
        //    plan.PlanType = Enumerators.PlanType.Plans;
        //    plan.IRType = Enumerators.IRType.ISS;
        //    plans.Add(plan);

        //    engine.TellMeDemands(container);
        //    Assert.AreEqual(470, itemFlow.ReqQty);
        //}

        //[Test]
        //public void TellMeDemands4()
        //{
        //    string code = "DmdFlow01";
        //    DemandChain demandChain1 = new DemandChain(locTo, null, flow.Code);
        //    demandChain1.ID = 1;
        //    demandChain1.Code = code;
        //    demandChains.Add(demandChain1);
        //    DemandChain demandChain2 = new DemandChain(locTo, null, code);
        //    demandChain2.ID = 2;
        //    demandChain2.Code = code;
        //    demandChains.Add(demandChain2);

        //    //ISS:300(Demand)
        //    plan = new Plans();
        //    plan.Loc = locTo;
        //    plan.Item = item;
        //    plan.FlowCode = code;
        //    plan.ReqTime = time;
        //    plan.OrderedQty = 300;
        //    plan.FinishedQty = 0;
        //    plan.PlanType = Enumerators.PlanType.Orders;
        //    plan.IRType = Enumerators.IRType.ISS;
        //    plans.Add(plan);

        //    engine.TellMeDemands(container);
        //    Assert.AreEqual(270, itemFlow.ReqQty);
        //}

        [Test]
        public void Test_oldWindowTime()
        {
            invBalance.Qty = 600;
            flow.OrderTime = time.AddHours(-2.5);
            flow.WindowTime = time.AddHours(-2);

            engine.TellMeDemands(container);
            var orders = engine.DemandToOrders(container.ItemFlows);
            Assert.AreEqual(0, orders[0].ItemFlows.Count);
            Assert.AreEqual(true, orders[0].Flow.IsUpdateWindowTime);
        }

        //Test freevalue
        [Test]
        public void Test_FreeValue1()
        {
            //ISS:450(Demand),FreeValue1
            plan = new Plans();
            plan.Loc = locTo;
            plan.Item = item;
            plan.Item.FreeValue1 = "freevalue1";//FreeValue1
            plan.ReqTime = time;
            plan.OrderedQty = 50;
            plan.FinishedQty = 0;
            plan.PlanType = Enumerators.PlanType.Orders;
            plan.IRType = Enumerators.IRType.ISS;
            container.Plans.Add(plan);

            engine.TellMeDemands(container);
            //original 420, add freevalue1 50, then 470
            Assert.AreEqual(470, itemFlow.ReqQty);
        }
    }
}
