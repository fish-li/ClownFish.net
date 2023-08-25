using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class EmptyTest
    {
        [TestMethod]
        public void Test_Array()
        {
            Assert.AreEqual(0, Empty.Array<int>().Length);

            Assert.AreEqual(0, Empty.Array<string>().Length);
        }

        [TestMethod]
        public void Test_List()
        {
            List<int> list = Empty.List<int>();
            Assert.AreEqual(0, list.Count);

            list.Add(2);

            MyAssert.IsError<InvalidCodeException>(() => {
                Assert.AreEqual(0, Empty.List<int>().Count);
            });


            list.Clear();
            Assert.AreEqual(0, Empty.List<int>().Count);
        }
    }
}
