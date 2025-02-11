using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Extensions;

[TestClass]
public class CharExtensionsTest
{
    [TestMethod]
    public void Test()
    {
        Assert.IsTrue('0'.IsDigit09());
        Assert.IsTrue('1'.IsDigit09());
        Assert.IsTrue('2'.IsDigit09());
        Assert.IsTrue('3'.IsDigit09());
        Assert.IsTrue('4'.IsDigit09());
        Assert.IsTrue('5'.IsDigit09());
        Assert.IsTrue('6'.IsDigit09());
        Assert.IsTrue('7'.IsDigit09());
        Assert.IsTrue('8'.IsDigit09());
        Assert.IsTrue('9'.IsDigit09());


        Assert.IsFalse('a'.IsDigit09());
        Assert.IsFalse('x'.IsDigit09());
        Assert.IsFalse('-'.IsDigit09());
        Assert.IsFalse('.'.IsDigit09());

        Assert.IsTrue(Char.IsDigit('٤'));
        Assert.IsFalse('٤'.IsDigit09());

        Assert.IsTrue(Char.IsDigit('٣'));
        Assert.IsTrue(Char.IsNumber('٣'));
        Assert.IsFalse('٣'.IsDigit09());

    }



}
