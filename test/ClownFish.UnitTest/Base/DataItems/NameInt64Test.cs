using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.DataItems;
[TestClass]
public class NameInt64Test
{
    [TestMethod]
    public void Test1()
    {
        NameInt64 value1 = new NameInt64("aa", 123);
        Assert.AreEqual("aa=123", value1.ToString());

        value1.Name = "bb";
        value1.Value = 123456;
        Assert.AreEqual("bb=12'3456", value1.ToString());
    }
}
