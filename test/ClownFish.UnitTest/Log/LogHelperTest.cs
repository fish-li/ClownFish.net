using ClownFish.Log.Writers;
using ClownFish.UnitTest.Log.Writers;

namespace ClownFish.UnitTest.Log;

[TestClass]
public class LogHelperTest
{
    private static readonly ValueCounter s_errorCount = new ValueCounter("ErrorCount");

    private static void OnErrorEvent(object sender, ExceptionEventArgs e)
    {
        s_errorCount.Increment();
    }
   
    [TestMethod]
    public void Test_RaiseErrorEvent()
    {
        long failureCount1 = ClownFishCounters.Logging.WriterErrorCount.Get();
        long errorCount1 = s_errorCount.Get();

        LogHelper.RaiseErrorEvent(ExceptionHelper.CreateException("9b35fa1b7c4a4413a7e37e724e4652b1"));

        long failureCount2 = ClownFishCounters.Logging.WriterErrorCount.Get();
        long errorCount2 = s_errorCount.Get();

        Assert.AreEqual(1, failureCount2 - failureCount1);
        Assert.AreEqual(0, errorCount1);
        Assert.AreEqual(errorCount2, errorCount1);

        LogHelper.OnError += OnErrorEvent;
        LogHelper.RaiseErrorEvent(ExceptionHelper.CreateException("71e6bd09a78c4f34ba7ade0bca3b867c"));
        LogHelper.OnError -= OnErrorEvent;

        long failureCount3 = ClownFishCounters.Logging.WriterErrorCount.Get();
        long errorCount3 = s_errorCount.Get();

        Assert.AreEqual(1, errorCount3 - errorCount2);
        Assert.AreEqual(1, failureCount3 - failureCount2);
    }

    public static bool Filter(object data)
    {
        if( data is OprLog oprlog )
            return oprlog.Status != 147258;

        if( data is InvokeLog invokeLog )
            return invokeLog.ActionType != 147258;

        if( data is XMessage xMessage )
            return xMessage.Message.ToString().StartsWithIgnoreCase("xx") == false;

        return true;
    }


    [TestMethod]
    public void Test_Filter()
    {
        Assert.IsFalse(LogHelper.InitCheck(new OprLog { Status = 147258}));
        Assert.IsFalse(LogHelper.InitCheck(new InvokeLog { ActionType = 147258  }));
        Assert.IsFalse(LogHelper.InitCheck(new XMessage("xxAAAAAAAAAA")));

        long count1 = ClownFishCounters.Logging.WriteCount.Get();
        LogHelper.Write(new OprLog { Status = 147258 });
        LogHelper.Write(new InvokeLog { ActionType = 147258 });
        LogHelper.Write(new XMessage("xxAAAAAAAAAA"));
        long count2 = ClownFishCounters.Logging.WriteCount.Get();

        Assert.AreEqual(count1, count2);
    }


    [TestMethod]
    public void Test_InitCheck()
    {
        InvokeLog log = FileWriterTest.CreateTempInvokeLog("be46a0ee60fc4e13809cfd1d5040df4d");
        XMessage msg = new XMessage("8a452f14373a494c8c04030a5469bdfa");

        Assert.IsTrue(LogHelper.InitCheck(log));
        Assert.IsTrue(LogHelper.InitCheck(msg));

        // 强制修改开关
        LogConfig.Instance.Enable = false;

        Assert.IsFalse(LogHelper.InitCheck(log));
        Assert.IsFalse(LogHelper.InitCheck(msg));

        // 恢复开关
        LogConfig.Instance.Enable = true;
    }


    [TestMethod]
    public void Test_InitCheck2()
    {
        XMessage msg = new XMessage("8a452f14373a494c8c04030a5469bdfa");

        Assert.IsTrue(LogHelper.InitCheck(msg));

        // 强制修改开关
        typeof(LogConfig).SetFieldValue("s_inited", false);

        Exception ex = MyAssert.IsError<InvalidOperationException>(() => {
            _ = LogHelper.InitCheck(msg);
        });

        // 恢复开关
        typeof(LogConfig).SetFieldValue("s_inited", true);

        Assert.IsTrue(LogHelper.InitCheck(msg));
    }


    [TestMethod]
    public void Test_AsyncWrite()
    {
        MemoryWriter memoryWriter = (MemoryWriter)WriterFactory.GetWriters(typeof(XMessage)).First(x => x is MemoryWriter);

        memoryWriter.PullALL();

        long count1 = ClownFishCounters.Logging.QueueFlushCount.Get();
        long count2 = ClownFishCounters.Logging.WriteCount.Get();

        LogHelper.Write((XMessage)null);
        LogHelper.Write(new XMessage("67130faa9ad54dca98039c3c7307681e"));
        LogHelper.Write(new XMessage("a443b55f10424ef4b279b8edd4970bda"));
        Thread.Sleep(10);

        while( count1 == ClownFishCounters.Logging.QueueFlushCount.Get() 
            || count2 == ClownFishCounters.Logging.WriteCount.Get() )
            Thread.Sleep(200);

        Thread.Sleep(200);
        List<XMessage> list = memoryWriter.PullALL().Cast<XMessage>().ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual("67130faa9ad54dca98039c3c7307681e", list[0].Message.ToString());
        Assert.AreEqual("a443b55f10424ef4b279b8edd4970bda", list[1].Message.ToString());
    }

    [TestMethod]
    public void Test_AsyncWrite_Exception()
    {
        long count1 = ClownFishCounters.Logging.QueueFlushCount.Get();
        long count2 = ClownFishCounters.Logging.XmlWriteCount.Get();
        
        LogHelper.Write((Exception)null);
        LogHelper.Write(ExceptionHelper.CreateException("33333333"));
        LogHelper.Write(ExceptionHelper.CreateException("444444444"));

        // 用死循环的方式等待后台线程执行成功
        while( count1 == ClownFishCounters.Logging.QueueFlushCount.Get()
            || count2 == ClownFishCounters.Logging.XmlWriteCount.Get() ) {
            Thread.Sleep(200);
        }

        Thread.Sleep(200);
        long count3 = ClownFishCounters.Logging.XmlWriteCount.Get();
        Assert.AreEqual(4, count3 - count2);  // 2 Oprlog, 2 InvokeLog
    }

    [TestMethod]
    public void Test_NotSupportedException()
    {
        MyAssert.IsError<NotSupportedException>(()=> {
            LogHelper.Write(new XMessage2("aaaaaaa"));
        });
    }

    [TestMethod]
    public void Test_RegisterFilter_ERROR()
    {
        MyAssert.IsError<ArgumentNullException>(()=> {
            LogHelper.RegisterFilter(null);
        });

        Exception ex1 = MyAssert.IsError<InvalidOperationException>(() => {
            LogHelper.RegisterFilter(Filter);
        });
        Assert.AreEqual("日志组件已初始化完成，不允许再调用当前方法。", ex1.Message);


        typeof(LogConfig).SetFieldValue("s_inited", false);


        Exception ex2 = MyAssert.IsError<InvalidOperationException>(() => {
            LogHelper.RegisterFilter(Filter);
        });
        Assert.AreEqual("不允许多次调用 RegisterFilter 方法。", ex2.Message);

        typeof(LogConfig).SetFieldValue("s_inited", true);
    }

    [TestMethod]
    public void Test_Writer_OnError()
    {
        ErrorMessage data = new ErrorMessage {
            Message = "1111111111"
        };

        long failureCount1 = ClownFishCounters.Logging.WriterErrorCount.Get();
        long count = ClownFishCounters.Logging.QueueFlushCount.Get();

        LogHelper.Write(data);

        while( count == ClownFishCounters.Logging.QueueFlushCount.Get() )
            Thread.Sleep(50);

        long failureCount2 = ClownFishCounters.Logging.WriterErrorCount.Get();

        Assert.AreEqual(1, failureCount2 - failureCount1);
    }

}
