﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ClownFish.UnitTest.Base.Extensions;

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
        Product2 p = new Product2 {
            CategoryID = 2,
            ProductID = 123,
            ProductName = Guid.NewGuid().ToString(),
            Quantity = 11,
            Unit = "GE",
            UnitPrice = 12.365m,
            Remark = "xxxxxxx"
        };

        var dict = p.ToDictionary();

        Product2 p2 = (Product2)dict.ToObject(typeof(Product2));

        string json1 = p.ToJson();
        string json2 = p2.ToJson();

        Assert.AreEqual(json1, json2);

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DictionaryExtensions.ToDictionary(null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DictionaryExtensions.ToObject(null, typeof(Product2));
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
        Product2 p = new Product2 {
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


    [TestMethod]
    public void Test_ToDictionary2()
    {
        List<NameValue> list = new List<NameValue>();
        list.Add(new NameValue("aaa", "111"));
        list.Add(new NameValue("bbb", "222"));

        Dictionary<string, string> dict1 = list.ToDictionary2(list.Count * 2, x => x.Name, x => x.Value);
        Assert.AreEqual(2, dict1.Count);
        Assert.IsTrue(dict1.ContainsKey("aaa"));
        Assert.IsFalse(dict1.ContainsKey("AAA"));
        Assert.IsTrue(dict1.ContainsKey("bbb"));
        Assert.IsFalse(dict1.ContainsKey("Bbb"));

        Dictionary<string, string> dict2 = list.ToDictionary2(list.Count * 2, x => x.Name, x => x.Value, StringComparer.OrdinalIgnoreCase);
        Assert.AreEqual(2, dict2.Count);
        Assert.IsTrue(dict2.ContainsKey("aaa"));
        Assert.IsTrue(dict2.ContainsKey("AAA"));
        Assert.IsTrue(dict2.ContainsKey("bbb"));
        Assert.IsTrue(dict2.ContainsKey("Bbb"));


        Dictionary<string, NameValue> dict3 = list.ToDictionary2(list.Count * 2, x => x.Name);
        Assert.AreEqual(2, dict3.Count);
        Assert.IsTrue(dict3.ContainsKey("aaa"));
        Assert.IsFalse(dict3.ContainsKey("AAA"));
        Assert.IsTrue(dict3.ContainsKey("bbb"));
        Assert.IsFalse(dict3.ContainsKey("Bbb"));


        Dictionary<string, NameValue> dict4 = list.ToDictionary2(list.Count * 2, x => x.Name, StringComparer.OrdinalIgnoreCase);
        Assert.AreEqual(2, dict4.Count);
        Assert.IsTrue(dict4.ContainsKey("aaa"));
        Assert.IsTrue(dict4.ContainsKey("AAA"));
        Assert.IsTrue(dict4.ContainsKey("bbb"));
        Assert.IsTrue(dict4.ContainsKey("Bbb"));


        MyAssert.IsError<ArgumentNullException>(() => {
            List<NameValue> list = null;
            Func<NameValue, string> keySelector = x => x.Name;
            _ = DictionaryExtensions.ToDictionary2<NameValue, string>(list, 128, keySelector);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            List<NameValue> list = new List<NameValue>();
            Func<NameValue, string> keySelector = null; //    x => x.Name;
            _ = DictionaryExtensions.ToDictionary2<NameValue, string>(list, 128, keySelector);
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            List<NameValue> list = null;
            Func<NameValue, string> keySelector = x => x.Name;
            Func<NameValue, string> elementSelector = x => x.Value;
            _ = DictionaryExtensions.ToDictionary2<NameValue, string, string>(list, 128, keySelector, elementSelector);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            List<NameValue> list = new List<NameValue>();
            Func<NameValue, string> keySelector = null; //    x => x.Name;
            Func<NameValue, string> elementSelector = x => x.Value;
            _ = DictionaryExtensions.ToDictionary2<NameValue, string, string>(list, 128, keySelector, elementSelector);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            List<NameValue> list = new List<NameValue>();
            Func<NameValue, string> keySelector = x => x.Name;
            Func<NameValue, string> elementSelector = null; //    x => x.Value;
            _ = DictionaryExtensions.ToDictionary2<NameValue, string, string>(list, 128, keySelector, elementSelector);
        });
    }
}


