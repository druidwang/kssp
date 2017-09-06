using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LeanEngine.Entity;
using LeanEngine.OAE;
using LeanEngine;
using LeanEngine.Utility;

namespace Testing
{
    /// <summary>
    /// TestCase for Order Automation Engine
    /// </summary>
    [TestFixture]
    public class OAETest
    {
        private IEngine engine;
        private Flow flow;
        private ItemFlow itemFlow;
        private List<ItemFlow> ItemFlows;
        private EngineContainer container;

        [SetUp]
        public void SetUp()
        {
            engine = new Engine();

            flow = new Flow();
            flow.FlowStrategy = new FlowStrategy();

            itemFlow = new ItemFlow();
            itemFlow.Item = new Item();
            itemFlow.Flow = flow;
            itemFlow.Item.ItemCode = "Item";
            itemFlow.LocFrom = "LocFrom";
            itemFlow.LocTo = "LocTo";

            ItemFlows = new List<ItemFlow>();
            ItemFlows.Add(itemFlow);

            container = new EngineContainer();
            container.ItemFlows = ItemFlows;
        }

        [Test]
        public void TellMeDemands()
        {
            try
            {
                engine.TellMeDemands(container);
            }
            catch (BusinessException ex)
            {
                string message = ex.Message;
            }
        }

    }
}
