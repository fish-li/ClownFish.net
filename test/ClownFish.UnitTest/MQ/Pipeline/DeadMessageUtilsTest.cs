using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.MQ.Pipeline;

#if NETCOREAPP
[TestClass]
public class DeadMessageUtilsTest
{
    private static readonly ValueCounter s_counter = new ValueCounter();

    [TestMethod]
    public void Test1()
    {
        DeadMessageUtils.OnDeadMessage += DeadMessageUtils_OnDeadMessage;

        DeadMessageUtils.HandlerDeadMessage("xxxxxxxx");

        Assert.AreEqual(1, s_counter.Get());
    }

    private void DeadMessageUtils_OnDeadMessage(object sender, DeadMessageArgs e)
    {
        Console.WriteLine(e.Message);
        s_counter.Increment();
    }
}
#endif
