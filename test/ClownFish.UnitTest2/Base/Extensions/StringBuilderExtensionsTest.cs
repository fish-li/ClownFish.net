using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Extensions;
[TestClass]
public class StringBuilderExtensionsTest
{
    [TestMethod]
    public void Test()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLineRN("abc");
        Assert.AreEqual(5, sb.Length);

        sb.WriteLine("123");
        Assert.AreEqual(10, sb.Length);

        sb.AppendLineRN(null);
        Assert.AreEqual(12, sb.Length);
    }

    [TestMethod]
    public void Test2()
    {
        StringBuilder sb = null;

        MyAssert.IsError<ArgumentNullException>(() => {
            sb.AppendLineRN("abc");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            sb.WriteLine("123");
        });




    }
}
