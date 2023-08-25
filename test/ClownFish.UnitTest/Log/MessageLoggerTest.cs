using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log
{
    [TestClass]
    public class MessageLoggerTest
    {
        [TestMethod]
        public void Test_Write()
        {
            string filePath = Path.Combine(FileUtils.RootPath, "MessageLoggerTest1.txt");
            MessageLogger logger = new MessageLogger(filePath, true);    // supportConcurrent = true
            logger.Write("68ade4c8e2d948a7bae98ed8b8b08602");

            string body = RetryFile.ReadAllText(filePath);
            Console.WriteLine(body);
            Assert.IsTrue(body.Contains("68ade4c8e2d948a7bae98ed8b8b08602"));

            RetryFile.Delete(filePath);
        }


        [TestMethod]
        public void Test_Write2()
        {
            string filePath = Path.Combine(FileUtils.RootPath, "MessageLoggerTest1.txt");
            MessageLogger logger = new MessageLogger(filePath, false);    // supportConcurrent = false

            logger.Write("");
            Assert.IsFalse(File.Exists(filePath));

            logger.Write("68ade4c8e2d948a7bae98ed8b8b08602");

            string body = RetryFile.ReadAllText(filePath);
            Console.WriteLine(body);
            Assert.IsTrue(body.Contains("68ade4c8e2d948a7bae98ed8b8b08602"));

            RetryFile.Delete(filePath);
        }

        [TestMethod]
        public void Test_Write3()
        {
            MessageLogger logger = new MessageLogger("xx:\\");
            logger.Write("aaaaaaaaaa");
            // 不出现异常就算通过
        }

        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                MessageLogger logger = new MessageLogger("");
            });
        }
    }
}
