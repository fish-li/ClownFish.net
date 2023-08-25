using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class ExpandoObjectExtensionsTest
    {
        [TestMethod]
        public void Test()
        {
            ExpandoObject data = new ExpandoObject();
            ExpandoObjectExtensions.Set(data, "key1", "abc");
            Assert.AreEqual("abc", ExpandoObjectExtensions.Get(data, "key1", true));
        }


        [TestMethod]
        public void Test_Error()
        {
            ExpandoObject data = new ExpandoObject();

            MyAssert.IsError<ArgumentNullException>(() => {
                ExpandoObjectExtensions.Set(null, "key1", "xx");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                ExpandoObjectExtensions.Set(data, string.Empty, "xx");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = ExpandoObjectExtensions.Get(null, "key1", true);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = ExpandoObjectExtensions.Get(data, string.Empty, true);
            });

            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                _ = ExpandoObjectExtensions.Get(data, "xx", true);
            });

            object value = ExpandoObjectExtensions.Get(data, "xx", false);
            Assert.IsNull(value);
        }
    }
}
