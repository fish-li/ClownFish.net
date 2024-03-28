namespace ClownFish;

internal static class ClownFishOptions
{
    public static readonly int MemoryStreamPool_BlockSize = LocalSettings.GetUInt("ClownFish_MemoryStreamPool_DefaultBlockSize", 32 * 1024);
    public static readonly int MemoryStreamPool_LargeBufferMultiple = LocalSettings.GetUInt("ClownFish_MemoryStreamPool_DefaultLargeBufferMultiple", 256 * 1024);
    public static readonly int MemoryStreamPool_MaximumBufferSize = LocalSettings.GetUInt("ClownFish_MemoryStreamPool_DefaultMaximumBufferSize", 2 * 1024 * 1024);

    public static readonly int StringBuilderPool_InitialCapacity = LocalSettings.GetUInt("ClownFish_StringBuilderPool_InitialCapacity", 32 * 1024);
    public static readonly int StringBuilderPool_MaximumRetainedCapacity = LocalSettings.GetUInt("ClownFish_StringBuilderPool_MaximumRetainedCapacity", 1024 * 512);
    public static readonly int StringBuilderPool_MaximumRetained = LocalSettings.GetUInt("ClownFish_StringBuilderPool_MaximumRetained", Environment.ProcessorCount * 6);

    public static readonly int AsyncBackgroundTask_WaitSeconds1 = LocalSettings.GetUInt("ClownFish_AsyncBackgroundTask_WaitSeconds1", 60);
    public static readonly int AsyncBackgroundTask_WaitSeconds2 = LocalSettings.GetUInt("ClownFish_AsyncBackgroundTask_WaitSeconds2", 66);

    public static readonly bool ShowBadHttpRequestException = LocalSettings.GetBool("ClownFish_ShowBadHttpRequestException", 0);

    public static readonly int MinMessageLength = LocalSettings.GetUInt("ClownFish_MQ_MessageLength_Min", 5);


}
