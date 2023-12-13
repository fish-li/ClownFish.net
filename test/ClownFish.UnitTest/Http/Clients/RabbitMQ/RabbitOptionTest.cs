using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Clients.RabbitMQ;

namespace ClownFish.UnitTest.Http.Clients.RabbitMQ;
[TestClass]
public class RabbitOptionTest
{
    [TestMethod]
    public void Test1()
    {
        Assert.AreEqual("ClownFish_Log_Rabbit", LoggingOptions.RabbitSettingName);

        RabbitOption opt = LocalSettings.GetSetting<RabbitOption>("rabbit_config", true);
        string text = opt.ToString();
        Assert.IsTrue(text.StartsWith0("Server="));
    }

    [TestMethod]
    public void Test_Validate()
    {
        RabbitOption opt = new RabbitOption();
        opt.VHost = "";

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            opt.Validate();
        });
               

        opt.Server = "s1";
        MyAssert.IsError<ConfigurationErrorsException>(() => {
            opt.Validate();
        });

        opt.Username = "username";
        opt.Validate();

        Assert.AreEqual("/", opt.VHost);
    }

    [TestMethod]
    public void Test_GetHttpOption()
    {
        RabbitOption opt = new RabbitOption {
            Server = "localpc",
            Username = "user1",
            Password = "xxxx"
        };

        HttpOption httpOption1 = opt.GetHttpOption("/test");
        Assert.IsTrue(httpOption1.Url.StartsWith0("http://localpc:15672"));

        opt.HttpPort = 12345;
        HttpOption httpOption2 = opt.GetHttpOption("/test");
        Assert.IsTrue(httpOption2.Url.StartsWith0("http://localpc:12345"));

    }
}
