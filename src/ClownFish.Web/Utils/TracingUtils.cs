namespace ClownFish.Web.Utils;

internal static class TracingUtils
{
    public static void Init()
    {
        if( LoggingOptions.TracingEnabled == false )
            return;


        DbLogger.Init();
        EFLogger.Init();
        HttpClientLogger2.Init();

        ReflectionUtils.CallStaticMethod("ClownFish.Email.MailLogger, ClownFish.Email", "Init");
        ReflectionUtils.CallStaticMethod("ClownFish.NRedis.RedisLogger, ClownFish.Redis", "Init");
        ReflectionUtils.CallStaticMethod("ClownFish.Rabbit.RabbitLogger, ClownFish.Rabbit", "Init");
    }


    public static void CheckLogConfig()
    {
        string writesMap = Settings.GetSetting("ClownFish_Log_WritersMap");
        if( writesMap.IsNullOrEmpty() ) {
            Console2.Info("force set: ClownFish_Log_WritersMap => OprLog=http");
            MemoryConfig.AddSetting("ClownFish_Log_WritersMap", "OprLog=http");
        }
    }
}
