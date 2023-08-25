using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base
{
    [TestClass]
    public class ClownFishBehaviorTest
    {
        [TestMethod]
        public void Test()
        {
            string appName = ClownFishBehavior.Instance.GetApplicationName();
            string hostName = ClownFishBehavior.Instance.GetHostName();
            string envName = ClownFishBehavior.Instance.GetEnvName();
            string tempPath = ClownFishBehavior.Instance.GetTempPath();

            Assert.AreEqual("ClownFish.UnitTest", appName);
            Assert.AreEqual("FishDev", envName);

            // 下面2个结果没有写断言
            Console.WriteLine(hostName);
            Console.WriteLine(tempPath);
        }


        [TestMethod]
        public void Test_EnvUtils()
        {
            string appName = EnvUtils.GetApplicationName();
            string hostName = EnvUtils.GetHostName();
            string envName = EnvUtils.GetEnvName();
            string tempPath = EnvUtils.GetTempPath();

            Assert.IsTrue(EnvUtils.IsDevEnv);

            Assert.AreEqual("ClownFish.UnitTest", appName);
            Assert.AreEqual("FishDev", envName);

            // 下面2个结果没有写断言
            Console.WriteLine(hostName);
            Console.WriteLine(tempPath);
        }

        [TestMethod]
        public void Test_EvnKind()
        {
            Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind(""));
            Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("Prod"));
            Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("Product"));
            Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("production"));

            Assert.AreEqual(EvnKind.Test, EnvUtils.GetEvnKind("Test"));
            Assert.AreEqual(EvnKind.Test, EnvUtils.GetEvnKind("Test2"));

            Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("DEV"));
            Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("Development"));
            Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("FishDev"));
            Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("xxxxxxxx"));
        }

    }
}
