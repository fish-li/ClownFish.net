using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Init;
[TestClass]
public class ApplicationInitializerTest
{
    [TestMethod]
    public void Test1()
    {
        long count1 = AppInitializer.Counter.Get();

        ApplicationInitializer.Execute();

        long count2 = AppInitializer.Counter.Get();
        Assert.AreEqual(count1 + 1, count2);
    }
}



public static class AppInitializer
{
    public static readonly ValueCounter Counter = new ValueCounter();

    public static void Init()
    {
        Counter.Increment();
    }
}
