using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log.Configuration
{
    [TestClass]
    public class TypeItemConfigTest
    {
        [TestMethod]
        public void Test()
        {
            TypeItemConfig config = new TypeItemConfig {
                DataType = "abc",
                Writers = "xml,json"
            };

            Assert.AreEqual("abc => xml,json", config.ToString());
        }
    }
}
