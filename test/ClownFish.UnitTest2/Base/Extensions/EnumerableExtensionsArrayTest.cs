//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using ClownFish.Base;

//namespace ClownFish.UnitTest.Base.Extensions
//{
//    [TestClass]
//    public class EnumerableExtensionsArrayTest
//    {
//        [TestMethod]
//        public void Test_SplitList_0()
//        {
//            int[] list = new int[0];

//            int[][] list2 = list.SplitArray(5, 6);

//            Assert.AreEqual(1, list2.Length);
//            Assert.AreEqual(0, list2[0].Length);
//        }

//        [TestMethod]
//        public void Test_SplitList_1()
//        {
//            int[] list = EnumerableExtensionsListTest.CreateList(3).ToArray();

//            int[][] list2 = list.SplitArray(5, 6);

//            Assert.AreEqual(1, list2.Length);
//            Assert.AreEqual(3, list2[0].Length);
//        }

//        [TestMethod]
//        public void Test_SplitList_2()
//        {
//            int[] list = EnumerableExtensionsListTest.CreateList(10).ToArray();

//            int[][] list2 = list.SplitArray(5, 6);

//            Assert.AreEqual(2, list2.Length);
//            Assert.AreEqual(6, list2[0].Length);
//            Assert.AreEqual(4, list2[1].Length);
//        }

//        [TestMethod]
//        public void Test_SplitList_3()
//        {
//            int[] list = EnumerableExtensionsListTest.CreateList(20).ToArray();

//            int[][] list2 = list.SplitArray(5, 6);

//            Assert.AreEqual(4, list2.Length);
//            Assert.AreEqual(6, list2[0].Length);
//            Assert.AreEqual(6, list2[1].Length);
//            Assert.AreEqual(6, list2[2].Length);
//            Assert.AreEqual(2, list2[3].Length);
//        }


//        [TestMethod]
//        public void Test_SplitList_4()
//        {
//            int[] list = EnumerableExtensionsListTest.CreateList(33).ToArray();

//            int[][] list2 = list.SplitArray(5, 6);

//            Assert.AreEqual(5, list2.Length);
//            Assert.AreEqual(7, list2[0].Length);
//            Assert.AreEqual(7, list2[1].Length);
//            Assert.AreEqual(7, list2[2].Length);
//            Assert.AreEqual(7, list2[3].Length);
//            Assert.AreEqual(5, list2[4].Length);
//        }


//        [TestMethod]
//        public void Test_SplitList_5()
//        {
//            int[] list = EnumerableExtensionsListTest.CreateList(53).ToArray();

//            int[][] list2 = list.SplitArray(5, 6);

//            Assert.AreEqual(5, list2.Length);
//            Assert.AreEqual(11, list2[0].Length);
//            Assert.AreEqual(11, list2[1].Length);
//            Assert.AreEqual(11, list2[2].Length);
//            Assert.AreEqual(11, list2[3].Length);
//            Assert.AreEqual(9, list2[4].Length);
//        }

//        [TestMethod]
//        public void Test_SplitList_6()
//        {
//            int[] list = EnumerableExtensionsListTest.CreateList(6).ToArray();

//            int[][] list2 = list.SplitArray(5, 6);

//            Assert.AreEqual(1, list2.Length);
//            Assert.AreEqual(6, list2[0].Length);
//        }

//        [TestMethod]
//        public void Test_SplitList_7()
//        {
//            int[] list = EnumerableExtensionsListTest.CreateList(7).ToArray();

//            int[][] list2 = list.SplitArray(5, 6);

//            Assert.AreEqual(2, list2.Length);
//            Assert.AreEqual(6, list2[0].Length);
//            Assert.AreEqual(1, list2[1].Length);
//        }


//        [TestMethod]
//        public void Test_SplitList_Error()
//        {
//            int[] list = EnumerableExtensionsListTest.CreateList(7).ToArray();

//            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
//                var x = list.SplitArray(0, 6);
//            });

//            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
//                var x = list.SplitArray(-1, 6);
//            });


//            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
//                var x = list.SplitArray(5, 0);
//            });
//            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
//                var x = list.SplitArray(5, -1);
//            });
//        }
//    }
//}
