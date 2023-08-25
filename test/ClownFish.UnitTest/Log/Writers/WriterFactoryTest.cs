using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Log.Logging;
using ClownFish.Log.Writers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log.Writers
{
    [TestClass]
    public class WriterFactoryTest
    {
        [TestMethod]
        public void Test_IsSupport()
        {
            Assert.IsTrue(WriterFactory.IsSupport(typeof(OprLog)));
            Assert.IsTrue(WriterFactory.IsSupport(typeof(InvokeLog)));
            Assert.IsTrue(WriterFactory.IsSupport(typeof(XMessage)));

            Assert.IsFalse(WriterFactory.IsSupport(typeof(string)));
            Assert.IsFalse(WriterFactory.IsSupport(typeof(NameValue)));
        }


        [TestMethod]
        public void Test_GetWriters()
        {
            Assert.AreEqual(2, WriterFactory.GetWriters(typeof(OprLog)).Length);
            Assert.AreEqual(4, WriterFactory.GetWriters(typeof(InvokeLog)).Length);
            Assert.AreEqual(2, WriterFactory.GetWriters(typeof(XMessage)).Length);


            Assert.IsNull(WriterFactory.GetWriters(typeof(string)));
            Assert.IsNull(WriterFactory.GetWriters(typeof(NameValue)));
        }
    }
}
