using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Init;
[TestClass]
public class InitAttributeTest
{
    [TestMethod]
    public void Test1()
    {
        long count1 = InitAttributeTest1.Counter.Get();

        InitAttribute.ExecuteAll();

        long count2 = InitAttributeTest1.Counter.Get();
        Assert.AreEqual(count1 + 1, count2);
    }
}


[Init(MethodName = nameof(StaticInit))]
public static class InitAttributeTest1
{
    public static readonly ValueCounter Counter = new ValueCounter();

    public static void StaticInit()
    {
        Counter.Increment();
    }
}
