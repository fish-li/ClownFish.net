using ClownFish.Log.Writers;

namespace ClownFish.Log;

/// <summary>
/// 日志组件配置类
/// </summary>
public static class LogConfig
{
    internal static readonly string ConfigFileName = "ClownFish.Log.config";

    /// <summary>
    /// 配置对象的静态引用
    /// </summary>
    internal static LogConfiguration Instance { get; private set; }


    private static bool s_inited = false;
    private static readonly object s_lock = new object();


    internal static bool IsInited => s_inited;



    /// <summary>
    /// 初始化日志组件
    /// </summary>
    /// <param name="config">LogConfiguration实例。可以从配置文件ClownFish.Log.config中加载</param>
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
    }



    internal static DebugReportBlock GetDebugReportBlock()
    {
        if( s_inited == false )
            return null;

        return Instance.GetDebugReportBlock();
    }

}
