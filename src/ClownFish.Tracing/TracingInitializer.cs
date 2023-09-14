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

        string writesMap = Settings.GetSetting("ClownFish_Log_WritersMap");
        if( writesMap.IsNullOrEmpty() ) {
            Console2.Info("ClownFish.Tracing force set: ClownFish_Log_WritersMap => OprLog=http");
            MemoryConfig.AddSetting("ClownFish_Log_WritersMap", "OprLog=http");
        }

        ClownFishInit.InitLogAsDefault();

        DbLogger.Init();
        EFLogger.Init();
        HttpClientLogger2.Init();

        InitXLog("ClownFish.NRedis.RedisLogger, ClownFish.Redis", "Init");
        InitXLog("ClownFish.Rabbit.RabbitLogger, ClownFish.Rabbit", "Init");

        AspnetcoreLogger.Init();
    }

    private static void InitXLog(string typeName, string methodName)
    {
        Type type = Type.GetType(typeName, false, true);
        if( type == null )
            return;

        MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if( method == null )
            return;

        method.Invoke(null, null);
    }
}
