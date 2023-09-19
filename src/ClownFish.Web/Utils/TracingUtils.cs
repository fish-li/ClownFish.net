﻿namespace ClownFish.Web.Utils;

internal static class TracingUtils
{
    public static void Init()
    {
        if( LoggingOptions.TracingEnabled == false )
            return;


        ClownFishInit.InitBase();

        // 对于这个项目功能而言，日志是必需的，所以这里尝试执行日志的初始化，如果在此之前有调用，这里就不起作用。
        string writesMap = Settings.GetSetting("ClownFish_Log_WritersMap");
        if( writesMap.IsNullOrEmpty() ) {
            Console2.Info("force set: ClownFish_Log_WritersMap => OprLog=http");
            MemoryConfig.AddSetting("ClownFish_Log_WritersMap", "OprLog=http");
        }
        ClownFishInit.InitLogAsDefault();


        DbLogger.Init();
        EFLogger.Init();
        HttpClientLogger2.Init();

        ReflectionUtils.CallStaticMethod("ClownFish.Email.MailLogger, ClownFish.Email", "Init");
        ReflectionUtils.CallStaticMethod("ClownFish.NRedis.RedisLogger, ClownFish.Redis", "Init");
        ReflectionUtils.CallStaticMethod("ClownFish.Rabbit.RabbitLogger, ClownFish.Rabbit", "Init");
    }
}
