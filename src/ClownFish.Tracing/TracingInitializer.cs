using ClownFish.Base.Xml;
using ClownFish.Log.Configuration;

namespace ClownFish.Tracing;

public static class TracingInitializer
{
    private static bool s_inited = false;
    private static readonly object s_lock = new object();

    internal static readonly DateTime StartTime = DateTime.Now;

    public static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {
                    
                    Init0();
                    s_inited = true;
                }
            }
        }
    }


    private static void Init0()
    {
        ClownFishInit.InitBase();
        InitLog();

        DbLogger.Init();
        EFLogger.Init();

        HttpClientLogger2.Init();
        RedisLogger.Init();
        
        AspnetcoreLogger.Init();
    }


    private static void InitLog()
    {
        if( ClownFish.Log.LogConfig.IsInited )
            return;

        // 从程序集中加载默认配置文件
        string xml = typeof(TracingInitializer).Assembly.ReadResAsText("ClownFish.Tracing.ClownFish.Log.config");
        LogConfiguration config = XmlHelper.XmlDeserialize<LogConfiguration>(xml);

        ClownFishInit.InitLog(config);
    }


}
