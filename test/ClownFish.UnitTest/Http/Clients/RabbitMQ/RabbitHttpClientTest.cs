using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Clients.RabbitMQ;

namespace ClownFish.UnitTest.Http.Clients.RabbitMQ;
#if NETCOREAPP

[TestClass]
public class RabbitHttpClientTest
{
    [TestCleanup]
    public void TestCleanup()
    {
        HttpClientMockResults.Clear();
    }

    [TestMethod]
    public void Test_ctor()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = new RabbitHttpClient(null);
        });
    }

    [TestMethod]
    public void Test_CreateQueueBind()
    {
        RabbitOption opt = LocalSettings.GetSetting<RabbitOption>("rabbit_config", true);
        RabbitHttpClient rabbitclient = new RabbitHttpClient(opt);

        MyAssert.IsError<ArgumentNullException>(() => {
            rabbitclient.CreateQueueBind("");
        });

        MyAssert.IsError<RabbitHttpException>(() => {
            rabbitclient.CreateQueueBind("???");
        });
    }

    [TestMethod]
    public async Task Test_SendMessage()
    {
        RabbitOption opt = LocalSettings.GetSetting<RabbitOption>("rabbit_config", true);
        RabbitHttpClient rabbitclient = new RabbitHttpClient(opt);

        string queue = "test1";

        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_QueueDeclare", ClownFish.Base.Void.Value);
        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_QueueBind", ClownFish.Base.Void.Value);
        rabbitclient.CreateQueueBind(queue);

        MyAssert.IsError<ArgumentNullException>(() => {
            rabbitclient.SendMessage(null);
        });

        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_SendMessage", ClownFish.Base.Void.Value);
        string message = Guid.NewGuid().ToString("N");
        rabbitclient.SendMessage(message, null, queue);

        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_GetMessage", message + "_这个API返回的是一个JSON__");
        string result1 = await rabbitclient.GetMessageAsync(queue, true);
        Assert.IsTrue(result1.Contains(message));



        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await rabbitclient.SendMessageAsync(null);
        });

        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_SendMessage", ClownFish.Base.Void.Value);
        string message2 = Guid.NewGuid().ToString("N");
        await rabbitclient.SendMessageAsync(message2, null, queue);

        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_GetMessage", message2 + "_这个API返回的是一个JSON__");
        string result2 = await rabbitclient.GetMessageAsync(queue, false);
        Assert.IsTrue(result2.Contains(message2));


        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_AckLast", ClownFish.Base.Void.Value);
        await rabbitclient.AckLast(queue);
        Thread.Sleep(1000);


        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_MessageCount", "{\"messages\":0}");
        long count = rabbitclient.MessageCount(queue);
        Assert.AreEqual(0, count);
    }
}
#endif