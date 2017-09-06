using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LeanEngine.Business;
using LeanEngine.Entity;

namespace Testing.Business
{
    [TestFixture]
    public class BidItemFlowTest
    {
        private BidItemFlow bidItemFlow;
        private List<ItemFlow> itemFlows;

        [SetUp]
        public void SetUp()
        {
            bidItemFlow = new BidItemFlow();
            itemFlows = new List<ItemFlow>();

            ItemFlow itemFlow = new ItemFlow();
            //测试时间过滤
            itemFlow.Flow = new Flow();
            itemFlow.ID = 1;
            itemFlow.Flow.OrderTime = DateTime.Now.AddHours(2);//未到订单时间，过滤
            itemFlows.Add(itemFlow);

            //测试0需求过滤
            itemFlow = new ItemFlow();
            itemFlow.Flow = new Flow();
            itemFlow.ID = 2;
            itemFlow.Flow.OrderTime = DateTime.Now.AddHours(-2);
            itemFlow.ReqQty = 0;//需求为0，过滤
            itemFlows.Add(itemFlow);

            //测试无竞标规则时，默认返回第一行
            itemFlow = new ItemFlow();
            itemFlow.Flow = new Flow();
            itemFlow.ID = 3;
            itemFlow.Flow.OrderTime = DateTime.Now.AddHours(-2);
            itemFlow.ReqQty = 20;
            itemFlows.Add(itemFlow);

            itemFlow = new ItemFlow();
            itemFlow.Flow = new Flow();
            itemFlow.ID = 4;
            itemFlow.Flow.OrderTime = DateTime.Now.AddHours(-2);
            itemFlow.ReqQty = 20;
            itemFlows.Add(itemFlow);

            //其它新增ID从5开始，在各自方法内执行
        }

        /// <summary>
        /// 测试无竞标规则时，默认返回结果
        /// </summary>
        [Test]
        public void getWinBidItemFlowTest1()
        {
            ItemFlow winBidItemFlow = bidItemFlow.getWinBidItemFlow(itemFlows);

            //3、4行并列，3行在前，返回3
            Assert.AreEqual(3, winBidItemFlow.ID);
        }

        /// <summary>
        /// 测试配额竞标策略：1)首次执行时
        /// </summary>
        [Test]
        public void getWinBidItemFlowTest2()
        {
            this.addNewBidItemFlows();
            itemFlows[4].AcmlOrderedQty = 0;
            itemFlows[5].AcmlOrderedQty = 0;
            itemFlows[6].AcmlOrderedQty = 0;

            ItemFlow winBidItemFlow = bidItemFlow.getWinBidItemFlow(itemFlows);

            //首次执行时，返回配额比例最大的ID=5
            Assert.AreEqual(5, winBidItemFlow.ID);
        }

        /// <summary>
        /// 测试配额竞标策略：2)主算法，返回最能拉近平衡点
        /// </summary>
        [Test]
        public void getWinBidItemFlowTest3()
        {
            this.addNewBidItemFlows();
            itemFlows[4].AcmlOrderedQty = 60;
            itemFlows[5].AcmlOrderedQty = 30;
            itemFlows[6].AcmlOrderedQty = 8;//10为平衡点，故中标

            ItemFlow winBidItemFlow = bidItemFlow.getWinBidItemFlow(itemFlows);

            //主算法，返回最能拉近平衡点
            Assert.AreEqual(7, winBidItemFlow.ID);
        }

        private void addNewBidItemFlows()
        {
            ItemFlow itemFlow = new ItemFlow();
            itemFlow.Flow = new Flow();
            itemFlow.ID = 5;
            itemFlow.Flow.OrderTime = DateTime.Now.AddHours(-2);
            itemFlow.ReqQty = 20;
            itemFlow.SupplyingRate = 60;//60%
            itemFlows.Add(itemFlow);

            itemFlow = new ItemFlow();
            itemFlow.Flow = new Flow();
            itemFlow.ID = 6;
            itemFlow.Flow.OrderTime = DateTime.Now.AddHours(-2);
            itemFlow.ReqQty = 20;
            itemFlow.SupplyingRate = 30;//30%
            itemFlows.Add(itemFlow);

            itemFlow = new ItemFlow();
            itemFlow.Flow = new Flow();
            itemFlow.ID = 7;
            itemFlow.Flow.OrderTime = DateTime.Now.AddHours(-2);
            itemFlow.ReqQty = 20;
            itemFlow.SupplyingRate = 10;//10%
            itemFlows.Add(itemFlow);
        }
    }
}
