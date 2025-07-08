using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class EncodingUtilsTest
{
    [TestMethod]
    public void Test_GetEncodingFromString()
    {
        Assert.IsNull(EncodingUtils.GetEncodingFromString(null));
        Assert.IsNull(EncodingUtils.GetEncodingFromString("xxx"));

        Assert.AreEqual(Encoding.UTF8, EncodingUtils.GetEncodingFromString("utf-8"));
        Assert.AreEqual(Encoding.Unicode, EncodingUtils.GetEncodingFromString("utf-16"));
        Assert.AreEqual(Encoding.GetEncoding("GB2312"), EncodingUtils.GetEncodingFromString("GB2312"));
    }

}
