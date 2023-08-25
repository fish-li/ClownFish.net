using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Xml
{
    [TestClass]
    public class XmlCommandManagerTest
    {
        [TestMethod]
        public void Test_LoadFromText()
        {
            XmlCommandManager m1 = new XmlCommandManager();

            m1.LoadFromText("");  // 忽略空操作

            var x1 = m1.GetCommand("DeleteCustomer");
            Assert.IsNull(x1);

            string xml = File.ReadAllText("App_Data/XmlCommand/Test1.config", Encoding.UTF8);
            m1.LoadFromText(xml);

            var x2 = m1.GetCommand("DeleteCustomer");
            Assert.IsNotNull(x2);            
        }

        [TestMethod]
        public void Test_LoadFromDirectory()
        {
            XmlCommandManager m1 = new XmlCommandManager();

            MyAssert.IsError<ArgumentNullException>(() => {
                m1.LoadFromDirectory("");
            });

            MyAssert.IsError<DirectoryNotFoundException>(() => {
                m1.LoadFromDirectory("xxxxxxxxxxxxxxxxxx");
            });


            var x1 = m1.GetCommand("DeleteCustomer");
            Assert.IsNull(x1);

            m1.LoadFromDirectory("App_Data/XmlCommand");

            var x2 = m1.GetCommand("DeleteCustomer");
            Assert.IsNotNull(x2);
        }
    }
}
