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
    public class WriterConfigTest
    {
        [TestMethod]
        public void Test()
        {
            WriterConfig config = new WriterConfig();
            Assert.IsNull(config.GetOptionValue("key1"));

            config.Options = new WriterOption[0];
            Assert.IsNull(config.GetOptionValue("key1"));

            MyAssert.IsError<ArgumentNullException>(()=> {
                var x = config.GetOptionValue("");
            });
        }

        [TestMethod]
        public void Test2()
        {
            WriterConfig config = new WriterConfig();
            config.Name = "writer1";
            config.Type = "writer1_type";
            config.Options = new WriterOption[] { new WriterOption{
                Key = "key1", Value = "123"
            } };
            Assert.AreEqual("123", config.GetOptionValue("key1"));
            Assert.AreEqual("123", config.GetOptionValue("KEY1"));
            Assert.IsNull(config.GetOptionValue("key2"));

            Assert.AreEqual("writer1 = writer1_type", config.ToString());
        }
    }
}
