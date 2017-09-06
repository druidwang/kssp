using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LeanEngine.Entity;
using LeanEngine.OAE;
using LeanEngine.Utility;

namespace Testing
{
    /// <summary>
    /// TestCase for ProcessTime
    /// </summary>
    [TestFixture]
    public class WindowTimeTest
    {
        private IOAE kb;
        private Flow flow1;
        private Flow flow2;

        [SetUp]
        public void SetUp()
        {
            kb = new KB(null, null, null);

            flow1 = new Flow();
            flow1.FlowStrategy = new FlowStrategy();

            flow2 = new Flow();
            flow2.FlowStrategy = new FlowStrategy();
            string[] winTimes = new string[] { "06:00", "08:00", "10:00", "12:00", "14:00", "16:00", "18:00" };
            flow2.FlowStrategy.MonWinTime = winTimes;
            flow2.FlowStrategy.TueWinTime = winTimes;
            flow2.FlowStrategy.WedWinTime = winTimes;
            flow2.FlowStrategy.ThuWinTime = winTimes;
            flow2.FlowStrategy.FriWinTime = winTimes;
            flow2.FlowStrategy.SatWinTime = null;
            flow2.FlowStrategy.SunWinTime = null;
            flow2.FlowStrategy.LeadTime = 1;
        }

        [Test]
        [ExpectedException(typeof(BusinessException))]
        public void Test_Exception()
        {
            kb.ProcessTime(null);
            kb.ProcessTime(new Flow());
            Assert.Fail();
        }

        [Test]
        public void Flow1_Test1()
        {
            flow1.FlowStrategy.LeadTime = 3;
            flow1.WindowTime = null;
            kb.ProcessTime(flow1);

            DateTime expected = DateTime.Now.AddHours(3);
            Assert.AreEqual(expected, flow1.WindowTime);
            Assert.AreEqual(null, flow1.NextWindowTime);
        }

        [Test]
        public void Flow1_Test2()
        {
            //WeekInterval
            flow1.FlowStrategy.WeekInterval = 1;
            flow1.FlowStrategy.LeadTime = 1;
            flow1.FlowStrategy.TueWinTime = new string[] { "10:00" };
            flow1.WindowTime = new DateTime(2020, 7, 2, 10, 0, 0);
            kb.ProcessTime(flow1);

            DateTime expected1 = new DateTime(2020, 7, 14, 9, 0, 0);
            DateTime expected2 = new DateTime(2020, 7, 14, 10, 0, 0);
            Assert.AreEqual(expected1, flow1.NextOrderTime.Value);
            Assert.AreEqual(expected2, flow1.NextWindowTime.Value);
        }

        [Test]
        public void Flow2_Test1()
        {
            //Normal
            flow2.FlowStrategy.WeekInterval = 0;
            flow2.WindowTime = new DateTime(2020, 7, 2, 14, 0, 0);
            kb.ProcessTime(flow2);

            DateTime expected1 = new DateTime(2020, 7, 2, 15, 0, 0);
            DateTime expected2 = new DateTime(2020, 7, 2, 16, 0, 0);
            Assert.AreEqual(expected1, flow2.NextOrderTime.Value);
            Assert.AreEqual(expected2, flow2.NextWindowTime.Value);
        }

        [Test]
        public void Flow2_Test2()
        {
            //Day last
            flow2.FlowStrategy.WeekInterval = 0;
            flow2.WindowTime = new DateTime(2020, 7, 1, 18, 0, 0);
            kb.ProcessTime(flow2);

            DateTime expected1 = new DateTime(2020, 7, 2, 5, 0, 0);
            DateTime expected2 = new DateTime(2020, 7, 2, 6, 0, 0);
            Assert.AreEqual(expected1, flow2.NextOrderTime.Value);
            Assert.AreEqual(expected2, flow2.NextWindowTime.Value);
        }

        [Test]
        public void Flow2_Test3()
        {
            //Weekend
            flow2.FlowStrategy.WeekInterval = 0;
            flow2.WindowTime = new DateTime(2020, 7, 3, 18, 0, 0);
            kb.ProcessTime(flow2);

            DateTime expected1 = new DateTime(2020, 7, 6, 5, 0, 0);
            DateTime expected2 = new DateTime(2020, 7, 6, 6, 0, 0);
            Assert.AreEqual(expected1, flow2.NextOrderTime.Value);
            Assert.AreEqual(expected2, flow2.NextWindowTime.Value);
        }

        [Test]
        public void Flow2_Test4()
        {
            //Old time
            flow2.FlowStrategy.WeekInterval = 0;
            string[] winTimes = new string[] { "00:00", "02:00", "04:00", "06:00", "08:00", "10:00", "12:00", "14:00", "16:00", "18:00", "20:00", "22:00" };
            flow2.FlowStrategy.MonWinTime = winTimes;
            flow2.FlowStrategy.TueWinTime = winTimes;
            flow2.FlowStrategy.WedWinTime = winTimes;
            flow2.FlowStrategy.ThuWinTime = winTimes;
            flow2.FlowStrategy.FriWinTime = winTimes;
            flow2.FlowStrategy.SatWinTime = winTimes;
            flow2.FlowStrategy.SunWinTime = winTimes;
            flow2.WindowTime = DateTime.Today.AddDays(-7).AddHours(9);
            kb.ProcessTime(flow2);

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int hour = DateTime.Now.Hour;
            hour = hour - hour % 2 + 2;
            DateTime expected2 = new DateTime(year, month, day, hour, 0, 0);
            DateTime expected1 = expected2.AddHours(-1);
            DateTime expected3 = new DateTime(year, month, day, hour, 0, 0);
            if (expected1 <= DateTime.Now)
            {
                expected1 = expected1.AddHours(2);
                expected2 = expected2.AddHours(2);
            }
            bool expected4 = flow2.IsUpdateWindowTime;
            Assert.AreEqual(expected1, flow2.NextOrderTime.Value);
            Assert.AreEqual(expected2, flow2.NextWindowTime.Value);
            Assert.AreEqual(expected3, flow2.WindowTime.Value);
            Assert.AreEqual(expected4, true);
        }
    }
}
