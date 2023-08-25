using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class MyTimeZoneTest
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual("Asia/Shanghai", MyTimeZone.CurrentTZ);

            TimeZoneInfo z1 = MyTimeZone.Get("Asia/Shanghai");
            TimeZoneInfo z2 = MyTimeZone.Get("China Standard Time");

            Assert.AreEqual(z1.Id, z2.Id);
            Assert.AreEqual(z1.StandardName, z2.StandardName);
            Assert.AreEqual(z1.BaseUtcOffset.Ticks, z2.BaseUtcOffset.Ticks);
        }


    }
}
