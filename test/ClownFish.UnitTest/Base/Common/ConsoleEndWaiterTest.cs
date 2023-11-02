using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#if NET6_0_OR_GREATER
namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class ConsoleEndWaiterTest
{
    [TestMethod]
    public void Test1()
    {
        int flag = 0;
        ConsoleEndWaiter waiter = new ConsoleEndWaiter();

        Task.Run(() => {
            waiter.Wait();
            flag = 1;
        });


        Thread.Sleep(100);
        Assert.AreEqual(0, flag);


        PosixSignalContext posixSignal = new PosixSignalContext(PosixSignal.SIGQUIT);
        waiter.HandlePosixSignal(posixSignal);
        Thread.Sleep(10);

        Assert.AreEqual(1, flag);
        waiter.Dispose();
    }
}
#endif
