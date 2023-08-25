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
    public class EnumerableExtensionsListTest
    {
        internal static List<int> CreateList(int count)
        {
            List<int> list = new List<int>();
            for( int i = 0; i < count; i++ )
                list.Add(i + 1);

            return list;
        }



        [TestMethod]
        public void Test_SplitList_空集合()
        {
            List<int> list = new List<int>();

            List<List<int>> list2 = list.SplitList(5, 6);
            
            Assert.AreEqual(1, list2.Count);
            Assert.AreEqual(0, list2[0].Count);
        }

        [TestMethod]
        public void Test_SplitList_集合数量小于batchSize_产生1个分组()
        {
            List<int> list = CreateList(3);

            List<List<int>> list2 = list.SplitList(5, 6);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(1, list2.Count);
            Assert.AreEqual(3, list2[0].Count);
        }

        [TestMethod]
        public void Test_SplitList_集合数量大于batchSize_产生2个分组()
        {
            List<int> list = CreateList(10);

            List<List<int>> list2 = list.SplitList(5, 6);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(2, list2.Count);
            Assert.AreEqual(6, list2[0].Count);
            Assert.AreEqual(4, list2[1].Count);
        }

        [TestMethod]
        public void Test_SplitList_集合数量大于batchSize_产生的分组数量小于maxBatch()
        {
            List<int> list = CreateList(20);

            List<List<int>> list2 = list.SplitList(5, 6);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(4, list2.Count);
            Assert.AreEqual(6, list2[0].Count);
            Assert.AreEqual(6, list2[1].Count);
            Assert.AreEqual(6, list2[2].Count);
            Assert.AreEqual(2, list2[3].Count);
        }


        [TestMethod]
        public void Test_SplitList_集合数量等于batchSize()
        {
            List<int> list = CreateList(6);

            List<List<int>> list2 = list.SplitList(5, 6);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(1, list2.Count);
            Assert.AreEqual(6, list2[0].Count);
        }

        [TestMethod]
        public void Test_SplitList_集合数量等于batchSize加1()
        {
            List<int> list = CreateList(7);

            List<List<int>> list2 = list.SplitList(5, 6);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(2, list2.Count);
            Assert.AreEqual(6, list2[0].Count);
            Assert.AreEqual(1, list2[1].Count);
        }


        [TestMethod]
        public void Test_SplitList_集合数量超出maxBatch和batchSize相乘_场景1()
        {
            List<int> list = CreateList(33);

            List<List<int>> list2 = list.SplitList(5, 6);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(5, list2.Count);
            Assert.AreEqual(7, list2[0].Count);
            Assert.AreEqual(7, list2[1].Count);
            Assert.AreEqual(7, list2[2].Count);
            Assert.AreEqual(7, list2[3].Count);
            Assert.AreEqual(5, list2[4].Count);
        }


        [TestMethod]
        public void Test_SplitList_集合数量超出maxBatch和batchSize相乘_场景1_增加集合数量且拆分参数不变()
        {
            List<int> list = CreateList(53);

            List<List<int>> list2 = list.SplitList(5, 6);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(5, list2.Count);
            Assert.AreEqual(11, list2[0].Count);
            Assert.AreEqual(11, list2[1].Count);
            Assert.AreEqual(11, list2[2].Count);
            Assert.AreEqual(11, list2[3].Count);
            Assert.AreEqual(9, list2[4].Count);
        }


        [TestMethod]
        public void Test_SplitList_集合数量超出maxBatch和batchSize相乘_场景2()
        {
            List<int> list = CreateList(103);

            List<List<int>> list2 = list.SplitList(5, 20);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(5, list2.Count);
            Assert.AreEqual(21, list2[0].Count);
            Assert.AreEqual(21, list2[1].Count);
            Assert.AreEqual(21, list2[2].Count);
            Assert.AreEqual(21, list2[3].Count);
            Assert.AreEqual(19, list2[4].Count);
        }

        [TestMethod]
        public void Test_SplitList_集合数量超出maxBatch和batchSize相乘_场景2_集合数量不变但减少batchSize()
        {
            List<int> list = CreateList(103);

            List<List<int>> list2 = list.SplitList(5, 3);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(5, list2.Count);
            Assert.AreEqual(21, list2[0].Count);
            Assert.AreEqual(21, list2[1].Count);
            Assert.AreEqual(21, list2[2].Count);
            Assert.AreEqual(21, list2[3].Count);
            Assert.AreEqual(19, list2[4].Count);
        }


        [TestMethod]
        public void Test_SplitList_maxBatch取最大值_不够1批()
        {
            List<int> list = CreateList(3);

            List<List<int>> list2 = list.SplitList(int.MaxValue, 5);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(1, list2.Count);
            Assert.AreEqual(3, list2[0].Count);
        }

        [TestMethod]
        public void Test_SplitList_maxBatch取最大值_刚好1批()
        {
            List<int> list = CreateList(5);

            List<List<int>> list2 = list.SplitList(int.MaxValue, 5);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(1, list2.Count);
            Assert.AreEqual(5, list2[0].Count);
        }

        [TestMethod]
        public void Test_SplitList_maxBatch取最大值_多批()
        {
            List<int> list = CreateList(13);

            List<List<int>> list2 = list.SplitList(int.MaxValue, 5);
            Console.WriteLine(list2.ToJson());

            Assert.AreEqual(3, list2.Count);
            Assert.AreEqual(5, list2[0].Count);
            Assert.AreEqual(5, list2[1].Count);
            Assert.AreEqual(3, list2[2].Count);
        }


        [TestMethod]
        public void Test_SplitList_maxBatch取最大值_多批2()
        {
            List<int> list = CreateList(1_3000);

            List<List<int>> list2 = list.SplitList(int.MaxValue, 10);
            Console.WriteLine(list2.ToJson());

        }

        [TestMethod]
        public void Test_SplitList_Error()
        {
            List<int> list = new List<int>();

            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                list.SplitList(1, 5);
            });

            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                list.SplitList(5, 1);
            });
        }
    }
}
