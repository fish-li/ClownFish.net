using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MQ.MMQ;
using ClownFish.MQ.Pipeline;
using ClownFish.Tasks;

namespace AotTestConsoleApp1.TestCase;
internal class TestMMQ
{
    internal static readonly MemoryMesssageQueue<Product3> MMQ = new MemoryMesssageQueue<Product3>(MmqWorkMode.Async);
    public static volatile bool IsEnd = false;

    public static async Task Run()
    {
        MmqSubscriber.StartAsync<Product3, MessageHandler1>(new MmqSubscriberArgs<Product3> {
            Queue = MMQ,
        });

        List<Type> tasks = BackgroundTaskManager.SearchBackgroundTaskTypes();
        BackgroundTaskManager.StartAll(tasks.ToArray());

        await Task.Delay(5000);
        IsEnd = true;
    }
}


internal sealed class MessageHandler1 : AsyncBaseMessageHandler<Product3>
{
    private static readonly ValueCounter s_count = new ValueCounter();

    public override bool EnableLog => false;

    public override async Task ProcessMessage(PipelineContext<Product3> context)
    {
        await Task.CompletedTask;

        s_count.Increment();
        Console2.Info($"MessageHandler1.ProcessMessage: [{s_count.Get()}] {context.MessageData.ToJson()}");
    }
}


public sealed class BackgroundTask1 : AsyncBackgroundTask
{
    public override int? SleepSeconds => 1;

    public override bool FirstRun => true;

    public override bool EnableLog => false;

    public override async Task ExecuteAsync()
    {
        if( TestMMQ.IsEnd ) {
            this.ExitTask();
            return;
        }

        await TestMMQ.MMQ.WriteAsync(Product3.CreateByRandomData());
    }
}