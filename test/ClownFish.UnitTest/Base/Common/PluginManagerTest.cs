namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class PluginManagerTest
{
    public class BaseTestPlugin { }

    public class Plugin1 : BaseTestPlugin { }

    public class Plugin2 : BaseTestPlugin { }

    public class Plugin3 : BaseTestPlugin {
        private Plugin3() { }
    }

    public class MyTest { }


    [TestMethod]
    public void Test1()
    {
        PluginManager<MyTest, BaseTestPlugin>.RegisterPlugin<Plugin1>();
        PluginManager<MyTest, BaseTestPlugin>.RegisterPlugin<Plugin2>();

        var plugins = PluginManager<MyTest, BaseTestPlugin>.CreatePlugins();

        Assert.AreEqual(2, plugins.Count);

        Assert.AreEqual(typeof(Plugin1), plugins[0].GetType());
        Assert.AreEqual(typeof(Plugin2), plugins[1].GetType());
    }


    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_ArgumentNullException()
    {
        PluginManager<MyTest, BaseTestPlugin>.RegisterPlugin((Type)null);
    }

    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    [TestMethod]
    public void Test_ArgumentOutOfRangeException()
    {
        PluginManager<MyTest, BaseTestPlugin>.RegisterPlugin<PluginManagerTest>();
    }


    [ExpectedException(typeof(ArgumentException))]
    [TestMethod]
    public void Test_ArgumentException()
    {
        PluginManager<MyTest, BaseTestPlugin>.RegisterPlugin<Plugin3>();
    }


    [ExpectedException(typeof(InvalidOperationException))]
    [TestMethod]
    public void Test_InvalidOperationException()
    {
        try {
            PluginManager<MyTest, BaseTestPlugin>.RegisterPlugin<Plugin1>();
        }
        catch { }

        var plugins = PluginManager<MyTest, BaseTestPlugin>.CreatePlugins();

        PluginManager<MyTest, BaseTestPlugin>.RegisterPlugin<Plugin2>();
    }


}
