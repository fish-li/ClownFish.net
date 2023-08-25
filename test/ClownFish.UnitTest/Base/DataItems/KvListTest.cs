using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.DataItems
{
    [TestClass]
    public class KvListTest
    {
        [TestMethod]
        public void Test_basic()
        {
            KvList<int, string> dict = new KvList<int, string>(8);
            dict[1] = "aaa";
            dict[2] = "bbb";

            string value;
            bool flag = dict.TryGetValue(1, out value);

            Assert.AreEqual(true, flag);
            Assert.AreEqual("aaa", value);


            Assert.AreEqual(2, dict.Count);           
            Assert.IsTrue(dict.ContainsKey(1));

            Assert.IsFalse(dict.IsReadOnly);
            Assert.AreEqual("1,2", string.Join(",", dict.Keys.ToArray()));
            Assert.AreEqual("aaa,bbb", string.Join(",", dict.Values.ToArray()));

            Assert.AreEqual(2, ((ICollection)dict).Count);
            Assert.IsFalse(((ICollection)dict).IsSynchronized);
            Assert.IsNotNull(((ICollection)dict).SyncRoot);

            dict.Remove(1);
            Assert.IsFalse(dict.ContainsKey(1));
            Assert.AreEqual(1, dict.Count);
        }

        [TestMethod]
        public void Test_2()
        {
            KvList<string, int> dict = new KvList<string, int>(8, StringComparer.OrdinalIgnoreCase);
            dict["key1"] = 11;
            dict["key2"] = 22;
            dict["key3"] = 33;

            MyAssert.IsError<NotImplementedException>(() => {
                dict.Add(new KeyValuePair<string, int>("key4", 44));
            });

            var dict2 = dict.Clone();
            Assert.AreEqual(3, dict2.Count);
            Assert.IsTrue(dict2.ContainsKey("KEY1"));
            dict2.Clear();
            Assert.AreEqual(0, dict2.Count);


            Assert.IsFalse(dict.ContainsKey(null));
            Assert.IsTrue(dict.ContainsKey("KEY1"));

            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                _ = dict["key-xx"];
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                dict.Add(null, 2);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                dict.SetValue(null, 2);
            });
            

            Assert.AreEqual(3, dict.Count);
            Assert.IsFalse(dict.Remove(null));
            Assert.AreEqual(3, dict.Count);

            Assert.IsTrue(dict.Remove("key3"));
            Assert.IsFalse(dict.Remove("key3"));
            Assert.AreEqual(2, dict.Count);

            Assert.IsFalse(dict.TryGetValue(null, out int xx));
            Assert.IsFalse(dict.TryGetValue("xx", out int xx2));

            Assert.AreEqual(11, dict["key1"]);
            dict.SetValue("KEY1", 111);
            Assert.AreEqual(111, dict["key1"]);
        }

        [TestMethod]
        public void Test_NotImplementedException()
        {
            KvList<string, int> dict = new KvList<string, int>(8, StringComparer.OrdinalIgnoreCase);

            MyAssert.IsError<NotImplementedException>(() => {
                dict.Contains(new KeyValuePair<string, int>("xx", 33));
            });

            MyAssert.IsError<NotImplementedException>(() => {
                dict.Remove(new KeyValuePair<string, int>("xx", 33));
            });

            MyAssert.IsError<NotImplementedException>(() => {
                dict.CopyTo(null, 0);
            });

            MyAssert.IsError<NotImplementedException>(() => {
                _= ((IEnumerable)dict).GetEnumerator();
            });

            MyAssert.IsError<NotImplementedException>(() => {
                ((ICollection)dict).CopyTo(null, 0);
            });
        }

        [TestMethod]
        public void Test_StringKey()
        {
            KvList<string, int> dict = new KvList<string, int>(16, StringComparer.OrdinalIgnoreCase);

            dict["aa"] = 11;

            int value;
            bool flag = dict.TryGetValue("aa", out value);

            Assert.AreEqual(true, flag);
            Assert.AreEqual(11, value);



            flag = dict.TryGetValue("AA", out value);

            Assert.AreEqual(true, flag);
            Assert.AreEqual(11, value);
        }


        [TestMethod]
        public void Test_order()
        {
            KvList<string, int> dict = new KvList<string, int>();

            dict["aa"] = 11;
            dict["cc"] = 33;
            dict["bb"] = 22;

            StringBuilder sb = new StringBuilder();

            foreach( var x in dict ) {
                sb.Append($"{x.Key}={x.Value};");
            }

            Assert.AreEqual("aa=11;cc=33;bb=22;", sb.ToString());
        }


        [TestMethod]
        public void Test_json()
        {
            KvList<string, int> dict = new KvList<string, int>();
            dict["aa"] = 11;
            dict["cc"] = 33;
            dict["bb"] = 22;
            string json1 = dict.ToJson();
            Console.WriteLine(json1);


            KvList<string, int> dict2 = json1.FromJson<KvList<string, int>>();

            Assert.AreEqual(3, dict2.Count);
            Assert.AreEqual(11, dict["aa"]);
            Assert.AreEqual(22, dict["bb"]);
            Assert.AreEqual(33, dict["cc"]);

            string json2 = dict2.ToJson();
            Assert.AreEqual(json1, json2);



            Dictionary<string, int> dict3 = new Dictionary<string, int>();
            dict3["aa"] = 11;
            dict3["cc"] = 33;
            dict3["bb"] = 22;
            string json3 = dict3.ToJson();
            Console.WriteLine(json3);

        }


        [TestMethod]
        public void Test_performance()
        {
            int count = 100_0000;

            Stopwatch watch = Stopwatch.StartNew();
            KvListRW(count);
            watch.Stop();
            Console.WriteLine(watch.Elapsed);


            watch = Stopwatch.StartNew();
            DictionaryRW(count);
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }


        private void KvListRW(int count)
        {
            for( int i = 0; i < count; i++ ) {
                IDictionary<string, long> dict = new KvList<string, long>();
                TestGetSet(dict);
            }
        }

        private void DictionaryRW(int count)
        {
            for( int i = 0; i < count; i++ ) {
                IDictionary<string, long> dict = new Dictionary<string, long>();
                TestGetSet(dict);
            }
        }

        private void TestGetSet(IDictionary<string, long> dict)
        {
            dict["集群服务数量"] = 11;
            dict["集群节点数量"] = 33;
            dict["MySQL数据规模GB"] = 22;
            dict["监控规则数量"] = 33;
            dict["监控资源数量"] = 33;

            long value;
            bool flag = dict.TryGetValue("集群服务数量", out value);
            Assert.AreEqual(true, flag);

            flag = dict.TryGetValue("集群节点数量", out value);
            Assert.AreEqual(true, flag);

            flag = dict.TryGetValue("MySQL数据规模GB", out value);
            Assert.AreEqual(true, flag);

            flag = dict.TryGetValue("监控规则数量", out value);
            Assert.AreEqual(true, flag);

            flag = dict.TryGetValue("监控资源数量", out value);
            Assert.AreEqual(true, flag);
        }


    }
}
