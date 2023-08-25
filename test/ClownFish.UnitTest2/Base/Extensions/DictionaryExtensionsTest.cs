using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class DictionaryExtensionsTest
    {
        [TestMethod]
        public void Test_Dictionary_Add_SameKey()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.AddValue("abc", 1);



            string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
                dict.AddValue("abc", 1);
            });

            string expected = "Key=abc";
            Assert.IsTrue(error.IndexOf(expected) > 0);




            Exception exception = null;

            try {
                dict.AddValue("abc", 1);
            }
            catch( Exception ex ) {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));

        }



        [TestMethod]
        public void Test_Hashtable_Add_SameKey()
        {
            Hashtable dict = new Hashtable();
            dict.AddValue("abc", 1);



            string error = ExceptionHelper.ExecuteActionReturnErrorMessage(() => {
                dict.AddValue("abc", 1);
            });

            string expected = "Key=abc";
            Assert.IsTrue(error.IndexOf(expected) > 0);




            Exception exception = null;

            try {
                dict.AddValue("abc", 1);
            }
            catch( Exception ex ) {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));

        }


        [TestMethod]
        public void Test_Dictionary_Add_Get()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.AddValue("key1", "abcd");
            Assert.AreEqual("abcd", dict.TryGet("key1"));
            Assert.IsNull(dict.TryGet("xxxxxxxxxxx"));


            Exception lastException = null;
            try {
                dict.AddValue("key1", "1111");
            }
            catch( ArgumentException ex ) {
                lastException = ex;
            }

            Assert.IsNotNull(lastException);
            Assert.IsTrue(lastException.Message.IndexOf("当前Key=key1") > 0);
        }


        [TestMethod]
        public void Test_ConcurrentDictionary_Add_Get()
        {
            ConcurrentDictionary<string, string> dict = new ConcurrentDictionary<string, string>();
            dict.AddValue("key1", "abcd");
            Assert.AreEqual("abcd", dict.TryGet("key1"));
            Assert.IsNull(dict.TryGet("xxxxxxxxxxx"));


            Exception lastException = null;
            try {
                dict.AddValue("key1", "1111");
            }
            catch( ArgumentException ex ) {
                lastException = ex;
            }

            Assert.IsNotNull(lastException);
            Assert.IsTrue(lastException.Message.IndexOf("当前Key=key1") > 0);
        }




        [TestMethod]
        public void Test_Hashtable_Add_Get()
        {
            Hashtable dict = new Hashtable();
            dict.AddValue("key1", "abcd");
            Assert.AreEqual("abcd", dict["key1"]);
            Assert.IsNull(dict["xxxxxxxxxxx"]);


            Exception lastException = null;
            try {
                dict.AddValue("key1", "1111");
            }
            catch( ArgumentException ex ) {
                lastException = ex;
            }

            Assert.IsNotNull(lastException);
            Assert.IsTrue(lastException.Message.IndexOf("当前Key=key1") > 0);
        }

        [TestMethod]
        public void Test_ICollection_IsNullOrEmptyt()
        {
            int[] intArray1 = null;
            int[] intArray2 = new int[0];
            Assert.IsTrue(intArray1.IsNullOrEmpty());
            Assert.IsTrue(intArray2.IsNullOrEmpty());

            Dictionary<string, int> dict1 = null;
            Dictionary<string, int> dict2 = new Dictionary<string, int>();
            Assert.IsTrue(dict1.IsNullOrEmpty());
            Assert.IsTrue(dict2.IsNullOrEmpty());

            List<int> list1 = null;
            List<int> list2 = new List<int>();
            Assert.IsTrue(list1.IsNullOrEmpty());
            Assert.IsTrue(list2.IsNullOrEmpty());

        }



        [TestMethod]
        public void Test_Object_To_Dictionary()
        {
            Product p = new Product {
                CategoryID = 2,
                ProductID = 123,
                ProductName = Guid.NewGuid().ToString(),
                Quantity = 11,
                Unit = "GE",
                UnitPrice = 12.365m,
                Remark = "xxxxxxx"
            };

            var dict = p.ToDictionary();

            Product p2 = (Product)dict.ToObject(typeof(Product));

            string json1 = p.ToJson();
            string json2 = p2.ToJson();

            Assert.AreEqual(json1, json2);

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = DictionaryExtensions.ToDictionary(null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = DictionaryExtensions.ToObject(null, typeof(Product));
            });
        }

        [TestMethod]
        public void Test_Clone()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            dict["key1"] = "abc";
            dict["key2"] = "xyz";

            Dictionary<string, string> dict2 = dict.Clone();
            Assert.AreEqual(2, dict2.Count);
            Assert.AreEqual("abc", dict2["KEY1"]);

            string[] keys = dict.ToKeys();
            Assert.IsTrue(keys.Contains("key1"));
            Assert.IsTrue(keys.Contains("key2"));
        }


        [TestMethod]
        public void Test_ToStringDictionary()
        {
            Product p = new Product {
                CategoryID = 2,
                ProductID = 123,
                ProductName = Guid.NewGuid().ToString(),
                Quantity = 11,
                Unit = "GE",
                UnitPrice = 12.365m,
                Remark = "xxxxxxx",
                LongText = "33333333333"
            };

            var dict = p.ToStringDictionary();

            Assert.AreEqual(8, dict.Count);
            Assert.AreEqual("123", dict["ProductID"]);
            Assert.AreEqual("11", dict["Quantity"]);
            Assert.AreEqual("xxxxxxx", dict["Remark"]);
            Assert.AreEqual("33333333333", dict["LongText"]);

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = DictionaryExtensions.ToStringDictionary(null);
            });
        }
    }
}
