using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log
{
    [TestClass]
    public class XxxxTest
    {
        [TestMethod]
        public void Test_LoggingOptions()
        {
            Assert.AreEqual(10, LoggingOptions.MaxCacheQueueLength);
        }

        [TestMethod]
        public void Test_xxxx()   // 一些无意义的用例，只为了代码覆盖
        {
            typeof(LoggingOptions).TestStaticGetSet();
            typeof(LoggingOptions.Http).TestStaticGetSet();
            typeof(LoggingOptions.HttpClient).TestStaticGetSet();

            typeof(LoggingLimit).TestStaticGetSet();
            typeof(LoggingLimit.OprLog).TestStaticGetSet();
            typeof(LoggingLimit.SQL).TestStaticGetSet();
        }


        

    }
}
