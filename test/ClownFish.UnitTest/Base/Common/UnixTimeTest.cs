using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class UnixTimeTest
    {
        [TestMethod]
        public void Test()
        {
            DateTime now = DateTime.Now;
            long stamp1 = UnixTime.GetUtcNanoTime();
            long stamp2 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000000;

            Console.WriteLine("TimeStamp1: " + stamp1);
            Console.WriteLine("TimeStamp2: " + stamp2);
            Assert.AreEqual(stamp1.ToString().Length, stamp2.ToString().Length);

            DateTime time1 = UnixTime.ToDateTime(stamp1);
            DateTime time2 = DateTimeOffset.FromUnixTimeMilliseconds(stamp2 / 1000000).DateTime.ToLocalTime();

            Console.WriteLine("Now  : " + now.ToTimeString());
            Console.WriteLine("Time1: " + time1.ToTimeString());
            Console.WriteLine("Time2: " + time2.ToTimeString());

            Assert.IsTrue((time1 - now).TotalMilliseconds < 10);
            Assert.IsTrue((time2 - now).TotalMilliseconds < 10);

            Console.WriteLine("---------------------------");
            // 测试 InfluxDB 返回的时间
            Console.WriteLine(UnixTime.ToDateTime(1561444155586543600).ToTimeString());  // 2019-06-25T06:30:45.5756104Z
            Console.WriteLine(UnixTime.ToDateTime(1561444155577250200).ToTimeString());  // 2019-06-25T06:30:45.5745689Z
            Console.WriteLine("---------------------------");
        }

        [TestMethod]
        public void Test2()
        {
            DateTime now = DateTime.Now;

            long time1 = UnixTime.GetNanoTime(now);
            long time2 = UnixTime.GetUtcNanoTime(now.ToUniversalTime());

            Assert.AreEqual(time1, time2);
            Assert.AreEqual(now, UnixTime.ToDateTime(time1));
        }
    }
}
