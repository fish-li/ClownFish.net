using ClownFish.Log.Writers;

namespace ClownFish;

internal static class ClownFishOptions
{
#if NETCOREAPP
    public static readonly int MemoryStreamPool_BlockSize = LocalSettings.GetUInt("ClownFish_MemoryStreamPool_DefaultBlockSize", 64 * 1024);
    public static readonly int MemoryStreamPool_LargeBufferMultiple = LocalSettings.GetUInt("ClownFish_MemoryStreamPool_DefaultLargeBufferMultiple", 512 * 1024);
    public static readonly int MemoryStreamPool_MaximumBufferSize = LocalSettings.GetUInt("ClownFish_MemoryStreamPool_DefaultMaximumBufferSize", 6 * 1024 * 1024);

    public static readonly int StringBuilderPool_InitialCapacity = LocalSettings.GetUInt("ClownFish_StringBuilderPool_InitialCapacity", 32 * 1024);
    public static readonly int StringBuilderPool_MaximumRetainedCapacity = LocalSettings.GetUInt("ClownFish_StringBuilderPool_MaximumRetainedCapacity", 512 * 1024);
    public static readonly int StringBuilderPool_MaximumRetained = LocalSettings.GetUInt("ClownFish_StringBuilderPool_MaximumRetained", Environment.ProcessorCount * 6);

#endif

    public static readonly int AsyncBackgroundTask_WaitSeconds1 = LocalSettings.GetUInt("ClownFish_AsyncBackgroundTask_WaitSeconds1", 60);
    public static readonly int AsyncBackgroundTask_WaitSeconds2 = LocalSettings.GetUInt("ClownFish_AsyncBackgroundTask_WaitSeconds2", 66);

    public static readonly bool ShowBadHttpRequestException = LocalSettings.GetBool("ClownFish_ShowBadHttpRequestException", 0);

    public static readonly int MinMessageLength = LocalSettings.GetUInt("ClownFish_MQ_MessageLength_Min", 5);

    public static bool JsonSerializer_CreateDefault = LocalSettings.GetBool("ClownFish_JsonSerializer_CreateDefault");
    public static bool JsonSerializer_CamelCase = LocalSettings.GetBool("ClownFish_JsonSerializer_CamelCase");

    public static string IndexNameTimeFormat => ElasticsearchWriter.IndexNameTimeFormat;

    public static readonly int GCCollectPeriodSec = LocalSettings.GetInt("ClownFish_GCCollect_PeriodSec", 60);

    public static readonly int HttpClient_GzipThreshold = LocalSettings.GetUInt("ClownFish_HttpClient_GzipThreshold", 1024);

    public static readonly bool AutoEnableCors = LocalSettings.GetBool("ClownFish_NHttpApplication_AutoEnableCors", 1);

    public static readonly bool ShowOneoffHttpMessageHandlerWarnning = LocalSettings.GetBool("ClownFish_ShowOneoffHttpMessageHandlerWarnning", 0);

    /// <summary>
    /// ThreadUtils在处理未捕获异常时是否显示到控制台窗口
    /// </summary>
    public static readonly bool ThreadUtilsShowErrorToConsole = LocalSettings.GetBool("ClownFish_ThreadUtils_ShowErrorToConsole", 1);

    /// <summary>
    /// 查找 XmlCommand 时【优先】支持特定的数据库种类，默认值：false (不损害性能)
    /// </summary>
    public static bool XmlCommandSupportMulitDbType = LocalSettings.GetBool("ClownFish_XmlCommand_SupportMulitDbType", 0);

    /// <summary>
    /// SimpleEsClient写日志时多久检查一次响应体来判断是否写入成功，默认值：0，表示永远不检查响应体。 如果设置为 100 表示 每执行 100 次写入动作，检查 1 次是否写入成功！
    /// </summary>
    public static readonly int SimpleEsClient_CheckResponseFrequency = LocalSettings.GetInt("SimpleEsClient_CheckResponseFrequency", 0);
}


/// <summary>
/// 一些控制参数
/// </summary>
public static class ClownFishPubOptions
{
    /// <summary>
    /// RabbitMQ 的默认队列类型，默认值："classic"  ，如果希望使用 quorum queue，可设置为 "quorum"
    /// </summary>
    public static readonly string RabbitmqDefaultQueueType = Settings.GetSetting("ClownFish_RabbitMQ_DefaultQueueType", "classic");
}