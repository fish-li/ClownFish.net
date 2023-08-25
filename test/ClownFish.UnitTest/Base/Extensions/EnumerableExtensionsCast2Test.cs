using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class EnumerableExtensionsCast2Test
    {
        [TestMethod]
        public void Test_direct_return()
        {
            IList values = new int[] { 1, 2, 3 };
            int[] v2 = values.Cast2<int>().ToArray();
            Assert.AreEqual(3, v2.Length);
            Assert.AreEqual("1,2,3", string.Join(",", v2));
        }


        [TestMethod]
        public void Test_args_null()
        {
            long[] values = null;
            long[] v2 = values.Cast2<long>().ToArray();
            Assert.IsNotNull(v2);
            Assert.AreEqual(0, v2.Length);
        }

        [TestMethod]
        public void Test_int_to_long()
        {
            IList values = new int[] { 1, 2, 3 };

            MyAssert.IsError<InvalidCastException>(() => {
                long[] v2 = values.Cast<long>().ToArray();
            });

            long[] v3 = values.Cast2<long>().ToArray();
            Assert.AreEqual(3, v3.Length);
            Assert.AreEqual("1,2,3", string.Join(",", v3));
        }


        [TestMethod]
        public void Test_int_to_string()
        {
            IList values = new int[] { 1, 2, 3 };

            MyAssert.IsError<InvalidCastException>(() => {
                string[] v2 = values.Cast<string>().ToArray();
            });

            string[] v3 = values.Cast2<string>().ToArray();
            Assert.AreEqual(3, v3.Length);
            Assert.AreEqual("1,2,3", string.Join(",", v3));
        }


        [TestMethod]
        public void Test_string_to_int()
        {
            IList values = new string[] { "1", "2", "3" };

            MyAssert.IsError<InvalidCastException>(() => {
                int[] v2 = values.Cast<int>().ToArray();
            });

            int[] v3 = values.Cast2<int>().ToArray();
            Assert.AreEqual(3, v3.Length);
            Assert.AreEqual("1,2,3", string.Join(",", v3));
        }


        [TestMethod]
        public void Test_string_to_string()
        {
            IList values = new object[] { "1", "2", "3" };

            string[] v3 = values.Cast2<string>().ToArray();
            Assert.AreEqual(3, v3.Length);
            Assert.AreEqual("1,2,3", string.Join(",", v3));
        }

        [TestMethod]
        public void Test_int_to_int()
        {
            IList values = new object[] { 1, 2, 3 };

            int[] v3 = values.Cast2<int>().ToArray();
            Assert.AreEqual(3, v3.Length);
            Assert.AreEqual("1,2,3", string.Join(",", v3));
        }
    }
}
