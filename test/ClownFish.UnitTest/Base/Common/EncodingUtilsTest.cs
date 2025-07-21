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
        Assert.IsNull(EncodingUtils.GetEncoding(null));
        Assert.IsNull(EncodingUtils.GetEncoding("xxx"));

        Assert.AreEqual(Encoding.UTF8, EncodingUtils.GetEncoding("utf-8"));
        Assert.AreEqual(Encoding.Unicode, EncodingUtils.GetEncoding("utf-16"));
        Assert.AreEqual(Encoding.GetEncoding("GB2312"), EncodingUtils.GetEncoding("GB2312"));
    }

}
