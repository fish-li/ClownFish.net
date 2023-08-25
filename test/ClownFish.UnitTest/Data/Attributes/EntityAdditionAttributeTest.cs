using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.Internals;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Attributes
{
    [TestClass]
    public class EntityAdditionAttributeTest
    {
        [TestMethod]
        public void Test()
        {
            EntityAdditionAttribute a = new EntityAdditionAttribute {
                ProxyType = typeof(Customer),
            };

            Assert.AreEqual(typeof(Customer), a.ProxyType);
        }
    }
}
