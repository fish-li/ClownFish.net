#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MQ.MMQ;

namespace ClownFish.UnitTest.MQ.MMQ;

[TestClass]
public class MemoryMesssageQueueTest
{
    [TestMethod]
    public async Task Test_sync()
    {
        MemoryMesssageQueue<string> mmq = new MemoryMesssageQueue<string>(MmqWorkMode.Sync, 100);
        mmq.Write("aaaaa");
        mmq.Write("bbbbb");       

        Assert.AreEqual(2,mmq.Count);

        string s1 = mmq.Read(null);
        string s2 = mmq.Read(null);

        Assert.AreEqual("aaaaa", s1);
        Assert.AreEqual("bbbbb", s2);
        Assert.AreEqual(0, mmq.Count);


        MyAssert.IsError<ArgumentNullException>(() => {
            mmq.Write(null);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await mmq.WriteAsync(null);
        });

        await MyAssert.IsErrorAsync<NotSupportedException>(async () => {
            _ = await mmq.ReadAsync(null);
        });
    }


    [TestMethod]
    public async Task Test_async()
    {
        MemoryMesssageQueue<string> mmq = new MemoryMesssageQueue<string>(MmqWorkMode.Async, 100);
        await mmq.WriteAsync("aaaaa");
        await mmq.WriteAsync("bbbbb");        

        Assert.AreEqual(2, mmq.Count);

        string s1 = await mmq.ReadAsync(null);
        string s2 = await mmq.ReadAsync(CancellationToken.None);

        Assert.AreEqual("aaaaa", s1);
        Assert.AreEqual("bbbbb", s2);
        Assert.AreEqual(0, mmq.Count);

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await mmq.WriteAsync(null);
        });

        MyAssert.IsError<NotSupportedException>(() => {
            mmq.Write("ccccccc");
        });

        MyAssert.IsError<NotSupportedException>(() => {
           _ =  mmq.Read(null);
        });
    }
}
#endif
