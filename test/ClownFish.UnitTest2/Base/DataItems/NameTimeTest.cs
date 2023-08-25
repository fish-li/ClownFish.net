using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.DataItems
{
    [TestClass]
    public class NameTimeTest
    {
        [TestMethod]
        public void Test1()
        {
            DateTime now = DateTime.Now;

            NameTime x1 = new NameTime { Name = "key1", Time = now };
            Assert.AreEqual("key1=" + now.ToTime27String(), x1.ToString());

            NameTime x2 = new NameTime("key2", now);
            Assert.AreEqual("key2=" + now.ToTime27String(), x2.ToString());

            NameTime x3 = new NameTime("key3");
            Assert.AreEqual("key3", x3.Name);
            Assert.IsTrue((now - x3.Time).TotalSeconds < 2);
        }
    }
}
