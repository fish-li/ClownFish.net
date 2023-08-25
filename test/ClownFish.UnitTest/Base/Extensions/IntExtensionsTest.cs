using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class IntExtensionsTest
    {
        [TestMethod]
        public void IntMin()
        {
            int a = 0.Min(2);
            Assert.AreEqual(2, a);

            int b = 3.Min(2);
            Assert.AreEqual(3, b);

            int c = 2.Min(2);
            Assert.AreEqual(2, c);
        }

        [TestMethod]
        public void IntMax()
        {
            int a = 0.Max(2);
            Assert.AreEqual(0, a);

            int b = 3.Max(2);
            Assert.AreEqual(2, b);

            int c = 2.Max(2);
            Assert.AreEqual(2, c);
        }


        [TestMethod]
        public void LongMin()
        {
            long a = 0L.Min(2L);
            Assert.AreEqual(2L, a);

            long b = 3L.Min(2L);
            Assert.AreEqual(3L, b);

            long c = 2L.Min(2L);
            Assert.AreEqual(2L, c);
        }

        [TestMethod]
        public void LongMax()
        {
            long a = 0L.Max(2L);
            Assert.AreEqual(0L, a);

            long b = 3L.Max(2L);
            Assert.AreEqual(2L, b);

            long c = 2L.Max(2L);
            Assert.AreEqual(2L, c);
        }


        [TestMethod]
        public void IntIf0Set()
        {
            int a = 0.If0Set(2);
            Assert.AreEqual(2, a);

            int b = (-3).If0Set(2);
            Assert.AreEqual(2, b);

            int c = 2.If0Set(12);
            Assert.AreEqual(2, c);
        }

        [TestMethod]
        public void LongIf0Set()
        {
            long a = 0L.If0Set(2L);
            Assert.AreEqual(2L, a);

            long b = (-3L).If0Set(2L);
            Assert.AreEqual(2L, b);

            long c = 2L.If0Set(12L);
            Assert.AreEqual(2L, c);
        }

        [TestMethod]
        public void IntIsBetween()
        {
            Assert.IsTrue(5.IsBetween(1, 5));
            Assert.IsTrue(5.IsBetween(1, 6));
            Assert.IsTrue(5.IsBetween(5, 5));
            Assert.IsTrue(5.IsBetween(5, 8));

            Assert.IsFalse(5.IsBetween(1, 4));
            Assert.IsFalse(5.IsBetween(6, 8));

            Assert.IsFalse(5.IsBetween(5, 3));
            Assert.IsFalse(5.IsBetween(8, 5));
        }

        [TestMethod]
        public void LongIsBetween()
        {
            Assert.IsTrue(5L.IsBetween(1, 5));
            Assert.IsTrue(5L.IsBetween(1, 6));
            Assert.IsTrue(5L.IsBetween(5, 5));
            Assert.IsTrue(5L.IsBetween(5, 8));

            Assert.IsFalse(5L.IsBetween(1, 4));
            Assert.IsFalse(5L.IsBetween(6, 8));

            Assert.IsFalse(5L.IsBetween(5, 3));
            Assert.IsFalse(5L.IsBetween(8, 5));
        }

    }
}
