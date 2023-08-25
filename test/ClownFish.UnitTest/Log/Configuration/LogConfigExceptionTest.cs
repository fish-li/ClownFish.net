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
    public class LogConfigExceptionTest
    {
        [TestMethod]
        public void Test()
        {
            // 没什么具体意义，只是为了覆盖代码
            LogConfigException ex1 = new LogConfigException("aa");
            LogConfigException ex2 = new LogConfigException("bb", new Exception("xx"));
        }
    }
}
