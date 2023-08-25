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
    public class FileConfigTest
    {
        [TestMethod]
        public void Test()
        {
            FileConfig config = new FileConfig();
            config.MaxCount = -1;

            config.CheckOrSetDefault();

            Assert.AreEqual("Logs", config.RootPath);
            Assert.AreEqual("500M", config.MaxLength);
            Assert.AreEqual(0, config.MaxCount);
        }

        [TestMethod]
        public void Test2()
        {
            FileConfig config = new FileConfig();
            config.RootPath = "/abc";
            config.MaxLength = "1G";
            config.MaxCount = 10;

            config.CheckOrSetDefault();

            Assert.AreEqual("/abc", config.RootPath);
            Assert.AreEqual("1G", config.MaxLength);
            Assert.AreEqual(10, config.MaxCount);
        }
    }
}
