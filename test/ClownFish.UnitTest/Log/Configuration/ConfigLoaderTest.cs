using ClownFish.Log.Writers;
using ClownFish.UnitTest.Log.Writers;

namespace ClownFish.UnitTest.Log.Configuration;

[TestClass]
public class ConfigLoaderTest
{
    internal static LogConfiguration GetLogConfiguration()
    {
        string filePath = ConfigHelper.GetFileAbsolutePath("ClownFish.Log.config");
        return LogConfiguration.LoadFromFile(filePath, true);
    }


    [TestMethod]
    public void Test1()
    {
        LogConfiguration config = GetLogConfiguration();

        ConfigLoader loader = new ConfigLoader();
        List<DataTypeWriterMap> list = loader.Load(config);

        Assert.AreEqual(4, list.Count);

        Assert.AreEqual(typeof(OprLog), list[0].DataType);
        Assert.AreEqual(2, list[0].WriteTypes.Length);
        Assert.AreEqual(typeof(XmlWriter), list[0].WriteTypes[0]);
        Assert.AreEqual(typeof(ClownFish.Log.Writers.JsonWriter), list[0].WriteTypes[1]);

        Assert.AreEqual(typeof(InvokeLog), list[1].DataType);
        Assert.AreEqual(4, list[1].WriteTypes.Length);
        Assert.AreEqual(typeof(XmlWriter), list[1].WriteTypes[0]);
        Assert.AreEqual(typeof(ClownFish.Log.Writers.JsonWriter), list[1].WriteTypes[1]);
        Assert.AreEqual(typeof(Json2Writer), list[1].WriteTypes[2]);
        Assert.AreEqual(typeof(HttpJsonWriter), list[1].WriteTypes[3]);

        Assert.AreEqual(typeof(XMessage), list[2].DataType);
        Assert.AreEqual(2, list[2].WriteTypes.Length);
        Assert.AreEqual(typeof(MemoryWriter), list[2].WriteTypes[0]);
        Assert.AreEqual(typeof(NullWriter), list[2].WriteTypes[1]);
    }

    [TestMethod]
    public void Test2()
    {
        LogConfiguration config = GetLogConfiguration();
        config.Performance = null;
        config.File = null;

        ConfigLoader loader = new ConfigLoader();
        _ = loader.Load(config);

        Assert.IsNotNull(config.Performance);
        Assert.IsNotNull(config.File);
    }

    [TestMethod]
    public void Test_ChceckDataTypeConfig_ERROR()
    {
        Exception ex1 = MyAssert.IsError<LogConfigException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Types = null;    // 引发异常

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
        Assert.AreEqual("日志配置文件中没有配置Types节点。", ex1.Message);


        Exception ex2 = MyAssert.IsError<LogConfigException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Types[0].DataType = string.Empty;    // 引发异常

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
        Assert.AreEqual("日志配置文件中Types/Type/DataType属性不允许为空。", ex2.Message);


        Exception ex3 = MyAssert.IsError<LogConfigException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Types[0].Writers = string.Empty;    // 引发异常

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
        Assert.AreEqual("日志配置文件中Types/Type/Writers属性不允许为空。", ex3.Message);

        Exception ex4 = MyAssert.IsError<LogConfigException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Types[0].Writers = ",,,";    // 引发异常

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
        Assert.AreEqual("日志配置文件中Types/Type/Writers属性值无效（没有实际内容）。", ex4.Message);


        Exception ex5 = MyAssert.IsError<TypeLoadException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Types[0].DataType = "xxxxxxxxxx";    // 引发异常

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
    }


    [TestMethod]
    public void Test_LoadWritersType_ERROR()
    {
        Exception ex1 = MyAssert.IsError<LogConfigException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Writers = null;    // 引发异常

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
        Assert.AreEqual("日志配置文件中没有配置Writers节点。", ex1.Message);


        Exception ex2 = MyAssert.IsError<LogConfigException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Writers[0].Name = null;    // 引发异常

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
        Assert.AreEqual("日志配置文件中Writers/Writer/Name属性不允许为空。", ex2.Message);


        Exception ex3 = MyAssert.IsError<LogConfigException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Writers[0].Type = null;    // 引发异常

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
        Assert.AreEqual("日志配置文件中Writers/Writer/Type属性不允许为空。", ex3.Message);


        Exception ex4 = MyAssert.IsError<LogConfigException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Writers[0].Type = "ClownFish.Log.LogHelper, ClownFish.net";    // 引发异常

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
        Assert.AreEqual("日志配置文件中Writers/Writer/Type属性值 [ClownFish.Log.LogHelper, ClownFish.net] 没有实现接口ILogWriter。", ex4.Message);
    }


    [TestMethod]
    public void Test_SetDataTypeCache_ERROR()
    {
        Exception ex1 = MyAssert.IsError<LogConfigException>(() => {
            LogConfiguration config = GetLogConfiguration();
            config.Types[0].Writers += ",xx";

            ConfigLoader loader = new ConfigLoader();
            _ = loader.Load(config);
        });
        Assert.AreEqual("日志配置文件中Types/Type/Writers属性值 [xx] 无效（不是有效的写入器名称）。", ex1.Message);
    }


    [TestMethod]
    public void Test_InitWrites()
    {
        WriterConfig wc = new WriterConfig {
            Name = "test1",
            Type = "ClownFish.UnitTest.Log.Configuration.ConfigLoaderTest+TestWriterInit, ClownFish.UnitTest"
        };

        LogConfiguration config = GetLogConfiguration();
        config.Writers = config.Writers.Add(wc);
        //config.Types[0].Writers += ",test1";   // 就是由于这行代码被注释了！

        ConfigLoader loader = new ConfigLoader();
        _ = loader.Load(config);

        Assert.AreEqual(0, TestWriterInit.InitCount);  // 虽然有定义写入器，但没有使用它，所以最终没用调用 Init 方法
    }

    [TestMethod]
    public void Test_InitWrites2()
    {
        WriterConfig wc = new WriterConfig {
            Name = "test1",
            Type = "ClownFish.UnitTest.Log.Configuration.ConfigLoaderTest+TestWriterInit, ClownFish.UnitTest"
        };

        LogConfiguration config = GetLogConfiguration();
        config.Writers = config.Writers.Add(wc);
        config.Types[0].Writers += ",test1";

        ConfigLoader loader = new ConfigLoader();
        _ = loader.Load(config);

        Assert.AreEqual(1, TestWriterInit.InitCount);
    }


    internal class TestWriterInit : ILogWriter
    {
        public static int InitCount { get; private set; }
        public void Init(LogConfiguration config, WriterConfig section)
        {
            InitCount++;
        }

        public void WriteOne<T>(T info)
        {
            throw new NotImplementedException();
        }

        void ILogWriter.WriteList<T>(List<T> list)
        {
            throw new NotImplementedException();
        }
    }
}
