using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class VoidTest
    {
        [TestMethod]
        public void Test()
        {
            var object1 = ClownFish.Base.Void.Value;
            var object2 = ClownFish.Base.Void.Value;

            Assert.IsTrue(object.ReferenceEquals(object2, object1));
        }
    }
}
