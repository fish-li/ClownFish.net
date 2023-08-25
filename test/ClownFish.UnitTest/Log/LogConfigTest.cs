using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Log;
using ClownFish.Log.Configuration;
using ClownFish.UnitTest.Log.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log
{
    [TestClass]
    public class LogConfigTest
    {
        [TestMethod]
        public void Test_LogConfiguration()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                LogConfiguration.LoadFromXml("");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                LogConfiguration.LoadFromFile("");
            });

            MyAssert.IsError<FileNotFoundException>(() => {
                LogConfiguration.LoadFromFile("xxxxxxxxxx.config", true);
            });


            string filePath = ConfigHelper.GetFileAbsolutePath("ClownFish.Log.config");
            string xml = File.ReadAllText(filePath, Encoding.UTF8);

            LogConfiguration cfg1 = LogConfiguration.LoadFromXml(xml);
            LogConfiguration cfg2 = LogConfiguration.LoadFromFile(filePath, true);
        }

        [TestMethod]
        public void Test_GetDebugReportBlock()
        {
            DebugReportBlock block = LogConfig.GetDebugReportBlock();
            Console.WriteLine(block.ToString2());
            // 这个用例不关心结果，只保证 GetDebugReportBlock 这个方法能成功调用就可以了。
        }


    }
}
