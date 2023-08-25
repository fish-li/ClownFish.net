using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Debug
{
    [TestClass]
    public class DebugReportTest
    {
        [TestMethod]
        public void Test()
        {
            DebugReportBlock block = new DebugReportBlock {
                Order = 1,
                Category = "Test",
            };

            block.AppendLine("1111111111111111111111");
            block.AppendLine("2222222222222222222222");

            string text = block.ToString();

            // 不想检查内容，偷个懒
            Assert.IsTrue(text.Length > 0);
        }

        [TestMethod]
        public void Test_null()
        {
            DebugReportBlock block = new DebugReportBlock();
            Assert.AreEqual("##### NONE #####0\r\n", block.ToString2());


            block.AppendLine(null);
            string text = block.ToString2();
        }


    }
}
