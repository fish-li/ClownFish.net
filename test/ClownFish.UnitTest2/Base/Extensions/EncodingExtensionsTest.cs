using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class EncodingExtensionsTest
    {
        [TestMethod]
        public void Test()
        {
            Encoding encoding = null;
            Assert.AreEqual(Encoding.UTF8, encoding.GetOrDefault());

            encoding = Encoding.ASCII;
            Assert.AreEqual(Encoding.ASCII, encoding.GetOrDefault());
        }
    }
}
