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
        if( writesMap.IsNullOrEmpty() )
            MemoryConfig.AddSetting("ClownFish_Log_WritersMap", "OprLog=http");

        ClownFishInit.InitLogAsDefault();

        DbLogger.Init();
        EFLogger.Init();

        HttpClientLogger2.Init();
        RedisLogger.Init();
        
        AspnetcoreLogger.Init();
    }


    


}
