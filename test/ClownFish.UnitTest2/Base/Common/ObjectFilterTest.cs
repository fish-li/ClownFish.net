using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class ObjectFilterTest
    {
        [TestMethod]
        public void Test1()
        {
            ObjectFilter<Exception> filter = new ObjectFilter<Exception>(e => e.ToString(), 3);

            Exception ex1 = ExceptionHelper.CreateException();
            Assert.IsFalse(filter.IsExist(ex1));

            Exception ex2 = ExceptionHelper.CreateException();
            Assert.IsTrue(filter.IsExist(ex2));
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Test2()
        {
            ObjectFilter<Exception> filter = new ObjectFilter<Exception>(e => e.ToString(), 3);

            Exception ex1 = null;
            Assert.IsFalse(filter.IsExist(ex1));
        }


        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Test3()
        {
            ObjectFilter<Exception> filter = new ObjectFilter<Exception>(null, 3);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void Test4()
        {
            ObjectFilter<Exception> filter = new ObjectFilter<Exception>(e => e.ToString(), -3);
        }
    }
}
