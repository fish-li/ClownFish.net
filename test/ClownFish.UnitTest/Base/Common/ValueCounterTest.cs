using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class ValueCounterTest
    {
        [TestMethod]
        public void Test()
        {
            ValueCounter counter = new ValueCounter("abc");
            Assert.AreEqual("abc", counter.Label);
            Assert.AreEqual(0L, counter.Get());

            counter.Increment();
            counter.Increment();
            Assert.AreEqual(2L, counter.Get());

            Assert.AreEqual("2", counter.AsString());
            Assert.AreEqual("abc=2", counter.ToString());

            counter.Add(3);
            Assert.AreEqual(5L, counter.Get());

            Assert.AreEqual("5", counter.AsString());
            Assert.AreEqual("abc=5", counter.ToString());

            counter.Set(7L);
            Assert.AreEqual(7L, counter.Get());

            long value = counter;
            Assert.AreEqual(7L, value);

            counter.Reset();
            Assert.AreEqual(0L, counter.Get());


            DateTime now = DateTime.Now;
            counter.Set(now);
            DateTime time2 = counter.GetAsDateTime();
            Assert.AreEqual(now, time2);

        }
    }
}
