using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Extensions;
[TestClass]
public class EnumerableExtensionsTest
{
    [TestMethod]
    public void Test_IsNullOrEmpty()
    {
        List<int> list1 = null;
        List<int> list2 = new List<int>();

        Assert.IsTrue(list1.IsNullOrEmpty());
        Assert.IsTrue(list2.IsNullOrEmpty());
    }

    [TestMethod]
    public void Test_HasValue()
    {
        List<int> list1 = null;
        List<int> list2 = new List<int>();

        Assert.IsFalse(list1.HasValue());
        Assert.IsFalse(list2.HasValue());
    }


    [TestMethod]
    public void Test_AddRange2()
    {
        List<int> list1 = null;
        List<int> list2 = new List<int>();

        int[] list3 = new int[] {1, 3, 5 };

        MyAssert.IsError<ArgumentNullException>(() => {
            list1.AddRange2(list2);
        });

        list2.AddRange2(list3);
        Assert.AreEqual(3, list2.Count);
    }







}
