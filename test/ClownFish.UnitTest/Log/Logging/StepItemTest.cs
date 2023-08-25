using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log.Logging
{
    [TestClass]
    public class StepItemTest
    {
        [TestMethod]
        public void Test1()
        {
            StepItem s1 = new StepItem {
                StepName = "Exec_92b4074d92994569a083b50afad5d9ff"
            };

            Assert.AreEqual("Exec_92b4074d92994569a083b50afad5d9ff", s1.ToString());

            string text = (string)s1.InvokeMethod("GetCmdxText");
            Assert.AreEqual(string.Empty, text);
        }
    }
}
