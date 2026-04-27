using ClownFish.Log.Writers;

namespace ClownFish.Log;

/// <summary>
/// 日志组件配置类
/// </summary>
public static class LogConfig
{
    /// <summary>
    /// 配置对象的静态引用
    /// </summary>
    internal static LogConfiguration Instance { get; private set; }


    private static bool s_inited = false;
#if NET9_0_OR_GREATER
    private static readonly Lock s_lock = new Lock();
#else
    private static readonly object s_lock = new object();
#endif

    /// <summary>
    /// 指示日志组件是否已初始化结束
    /// </summary>
    public static bool IsInited => s_inited;


    /// <summary>
    /// 初始化日志组件
    /// </summary>
    /// <param name="config">LogConfiguration实例</param>
#if NETCOREAPP    // 下面几个类型不参与裁剪，它们全是内部类型只能反射使用
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ILogWriter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ElasticsearchWriter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FileWriter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HttpJsonWriter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Json2Writer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ClownFish.Log.Writers.JsonWriter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NullWriter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(OprlogEsWriter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(RabbitHttpWriter))]
    //[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(XmlWriter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TxtWriter))]

    // 下面几个类型不参与裁剪，保留无参构造函数，确保可序列化
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(LogConfiguration))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PerformanceConfig))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FileConfig))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WriterConfig))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TypeItemConfig))]

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(OprLog))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(InvokeLog))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(OprLogScope))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CodeSnippetContext))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DbLogger))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HttpClientLogger2))]
#endif
    public static void Init(LogConfiguration config)
    {
        if( config == null )
            throw new ArgumentNullException(nameof(config));


        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {

                    Init0(config);

                    // 标记初始化已成功
                    s_inited = true;
                }
            }
        }
    }

    private static void Init0(LogConfiguration config)
    {
        // 注意：这里不克隆 config，而是直接引用它

        if( config.Enable == false ) {
            LogConfig.Instance = config;
            return;
        }

        // 解析配置参数
        ConfigLoader loader = new ConfigLoader();
        var list = loader.Load(config);

        WriterFactory.Init(list);

        LogConfig.Instance = config;

        // 创建后台写入线程
        CacheQueueManager.Start(list);

        int count = (from d in config.Types
                     let ws = WriterFactory.GetWriters(d.TypeObject)
                     where ws.Length > 0 && ws.Any(x => (x is NullWriter) == false)
                     select d).Count();

        if( count == 0 ) {
            Console2.Info("### 所有日志数据类型没有配置写入器，日志组件将不会执行写入动作！可尝试配置ClownFish_Log_WritersMap参数。");
            config.Enable = false;
        }
    }




    internal static DebugReportBlock GetDebugReportBlock()
    {
        if( s_inited == false )
            return null;

        return Instance.GetDebugReportBlock();
    }


    /// <summary>
    /// 从文件中加载LogConfiguration对象
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static LogConfiguration LoadFromFile(string filePath, bool checkExist = true)
    {
        if( filePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filePath));


        if( System.IO.File.Exists(filePath) == false ) {
            if( checkExist )
                throw new FileNotFoundException("配置文件没有找到，filePath: " + filePath);
            else
                return null;
        }

        string text = System.IO.File.ReadAllText(filePath, Encoding.UTF8);

        if( filePath.EndsWith1(".ini") )
            return LoadFromIni(text);

        if( EnvArgs0.IsAot == false ) {
            if( filePath.EndsWith1(".config") )
                return LoadFromXml(text);
        }

        throw new NotSupportedException("不支持的配置文件格式，filePath: " + filePath);
    }



    /// <summary>
    /// 从INI文本中加载LogConfiguration对象
    /// </summary>
    /// <param name="ini"></param>
    /// <returns></returns>
    public static LogConfiguration LoadFromIni(string ini)
    {
        return LogConfigIni.LoadIni(ini);
    }


    /// <summary>
    /// 【不建议使用】从XML文本中加载LogConfiguration对象
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    //[Obsolete("不建议使用XML格式的配置文件，NativeAOT模式下不支持XML格式的配置文件，请使用INI格式的配置文件")]
    internal static LogConfiguration LoadFromXml(string xml)
    {
        if( xml.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(xml));

        return XmlHelper.XmlDeserialize<LogConfiguration>(xml);
    }


}
