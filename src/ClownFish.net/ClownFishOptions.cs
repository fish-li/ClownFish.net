using ClownFish.Log.Writers;

namespace ClownFish;

internal static class ClownFishOptions
{
#if NETCOREAPP
    public static readonly int MemoryStreamPool_BlockSize = LocalSettings.GetUInt("ClownFish_MemoryStreamPool_DefaultBlockSize", 32 * 1024);
    public static readonly int MemoryStreamPool_LargeBufferMultiple = LocalSettings.GetUInt("ClownFish_MemoryStreamPool_DefaultLargeBufferMultiple", 256 * 1024);
    public static readonly int MemoryStreamPool_MaximumBufferSize = LocalSettings.GetUInt("ClownFish_MemoryStreamPool_DefaultMaximumBufferSize", 1 * 1024 * 1024);

    public static readonly int StringBuilderPool_InitialCapacity = LocalSettings.GetUInt("ClownFish_StringBuilderPool_InitialCapacity", 32 * 1024);
    public static readonly int StringBuilderPool_MaximumRetainedCapacity = LocalSettings.GetUInt("ClownFish_StringBuilderPool_MaximumRetainedCapacity", 512 * 1024);
    public static readonly int StringBuilderPool_MaximumRetained = LocalSettings.GetUInt("ClownFish_StringBuilderPool_MaximumRetained", Environment.ProcessorCount * 6);

    public static readonly int AsyncBackgroundTask_WaitSeconds1 = LocalSettings.GetUInt("ClownFish_AsyncBackgroundTask_WaitSeconds1", 60);
    public static readonly int AsyncBackgroundTask_WaitSeconds2 = LocalSettings.GetUInt("ClownFish_AsyncBackgroundTask_WaitSeconds2", 66);

#endif

    public static readonly bool ShowBadHttpRequestException = LocalSettings.GetBool("ClownFish_ShowBadHttpRequestException", 0);

    public static readonly int MinMessageLength = LocalSettings.GetUInt("ClownFish_MQ_MessageLength_Min", 5);

    public static bool JsonSerializer_CreateDefault = LocalSettings.GetBool("ClownFish_JsonSerializer_CreateDefault");
    public static bool JsonSerializer_CamelCase = LocalSettings.GetBool("ClownFish_JsonSerializer_CamelCase");

    public static string IndexNameTimeFormat => ElasticsearchWriter.IndexNameTimeFormat;

    public static readonly int GCCollectPeriodSec = LocalSettings.GetInt("ClownFish_GCCollect_PeriodSec", 60);

    // HttpClientHandler 的压缩设计的非常SB， （.net framework没有这个问题）
    // 如果你设置为 DecompressionMethods.All ，那么就会出现3个Accept-Encoding请求头，
    // 实际上，目前几乎都是使用 gzip，所以为了节约网络流量，所以就只支持 gzip
    public static readonly int HttpClient_DecompressionMethods = LocalSettings.GetUInt("ClownFish_HttpClient_DecompressionMethods", 1);  // GZip
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