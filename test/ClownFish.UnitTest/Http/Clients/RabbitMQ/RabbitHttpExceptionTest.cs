using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Clients.RabbitMQ;

namespace ClownFish.UnitTest.Http.Clients.RabbitMQ;
#if NETCOREAPP

[TestClass]
public class RabbitHttpExceptionTest
{
    [TestMethod]
    public void Test()
    {
        RabbitOption opt = LocalSettings.GetSetting<RabbitOption>("rabbit_config", true);
        RabbitHttpClient rabbitHttp = new RabbitHttpClient(opt);

        RabbitHttpException lastException = null;

        try {
            rabbitHttp.MessageCount("xx");
        }
        catch( RabbitHttpException ex ) {
            lastException = ex;
        }

        Assert.IsNotNull( lastException );
        Console.WriteLine( lastException.Message );

        Assert.IsTrue(lastException.Message.StartsWith0("执行 MessageCount 失败"));
    }
}
#endif
