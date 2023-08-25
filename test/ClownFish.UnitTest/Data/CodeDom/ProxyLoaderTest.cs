using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data.CodeDom;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.CodeDom
{
    [TestClass]
    public class ProxyLoaderTest
    {
        [TestMethod]
        public void Test()
        {
            var list = ProxyLoader.SearchExistEntityCompileResult();
            Type[] types = list.Select(x => x.EntityType).ToArray();

            Assert.IsTrue(types.Contains(typeof(Customer)));
            Assert.IsTrue(types.Contains(typeof(Product)));

            Assert.IsTrue(types.Contains(typeof(OrderDetailX2)));
            Assert.IsTrue(types.Contains(typeof(OrderDetailX3)));
            Assert.IsTrue(types.Contains(typeof(OrderDetailX4)));

            Assert.IsFalse(types.Contains(typeof(OrderDetailX1)));
            Assert.IsFalse(types.Contains(typeof(OrderDetailX5)));
            Assert.IsFalse(types.Contains(typeof(OrderDetailX6)));


            string block = ProxyLoader.EntityProxyAssemblyListReportBlock.ToString2();
            Console.WriteLine(block);

            Assert.IsTrue(block.Contains("##### EntityProxy Assembly List #####"));
            Assert.IsTrue(block.Contains("ClownFish.UnitTest.EntityProxy.dll"));

        }
    }
}
