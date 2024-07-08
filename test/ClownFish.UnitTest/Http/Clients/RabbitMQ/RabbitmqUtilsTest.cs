using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Clients.RabbitMQ;

namespace ClownFish.UnitTest.Http.Clients.RabbitMQ;
[TestClass]
public class RabbitmqUtilsTest
{

    [TestMethod]
    public void Test_1()
    {
        IDictionary<string, object> arguments = null;

        arguments = RabbitmqUtils.SetQueueType(arguments, "");

        Assert.IsNull(arguments);
    }

    [TestMethod]
    public void Test_2()
    {
        IDictionary<string, object> arguments = null;

        arguments = RabbitmqUtils.SetQueueType(arguments, "quorum");

        Assert.IsNotNull(arguments);
        Assert.AreEqual(1, arguments.Count);
        Assert.AreEqual("quorum", arguments["x-queue-type"].ToString());
    }

    [TestMethod]
    public void Test_3()
    {
        IDictionary<string, object> arguments = new Dictionary<string, object>();

        arguments = RabbitmqUtils.SetQueueType(arguments, "quorum");

        Assert.IsNotNull(arguments);
        Assert.AreEqual(1, arguments.Count);
        Assert.AreEqual("quorum", arguments["x-queue-type"].ToString());
    }

    [TestMethod]
    public void Test_4()
    {
        IDictionary<string, object> arguments = new Dictionary<string, object>();
        arguments["x-queue-type"] = "classic";

        arguments = RabbitmqUtils.SetQueueType(arguments, "quorum");

        Assert.IsNotNull(arguments);
        Assert.AreEqual(1, arguments.Count);
        Assert.AreEqual("classic", arguments["x-queue-type"].ToString());
    }
}
