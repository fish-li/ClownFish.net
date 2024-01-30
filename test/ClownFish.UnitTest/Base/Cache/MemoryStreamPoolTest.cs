using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Cache;
[TestClass]
public class MemoryStreamPoolTest
{
    [TestMethod]
    public void Test1()
    {
        using Stream s1 = MemoryStreamPool.GetStream();
        Assert.IsNotNull(s1);

        using MemoryStream s2 = MemoryStreamPool.GetStream("aaaaa", 2048);
        Assert.IsNotNull(s2);
    }
}
