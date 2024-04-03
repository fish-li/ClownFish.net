using ClownFish.MQ;
using ClownFish.Tasks;

namespace ClownFish;


internal static class ClownFishOptions
{
    // 说明：这个类不直接定义 static readonly 的字段变量，
    // 因为如果在程序初始化早期被访问，那么这些变量的值就【提前】固定了，导致“远程App.config”定义的参数不起作用

#if NETCOREAPP
    public static int MemoryStreamPool_BlockSize => MemoryStreamPool.BlockSize;
    public static int MemoryStreamPool_LargeBufferMultiple => MemoryStreamPool.LargeBufferMultiple;
    public static int MemoryStreamPool_MaximumBufferSize => MemoryStreamPool.MaximumBufferSize;

    public static int StringBuilderPool_InitialCapacity => StringBuilderPool.Options.InitialCapacity;
    public static int StringBuilderPool_MaximumRetainedCapacity => StringBuilderPool.Options.MaximumRetainedCapacity;
    public static int StringBuilderPool_MaximumRetained => StringBuilderPool.Options.MaximumRetained;

    public static int AsyncBackgroundTask_WaitSeconds1 => AsyncBackgroundTask.WaitSecond60;
    public static int AsyncBackgroundTask_WaitSeconds2 => AsyncBackgroundTask.WaitSecond66;
#endif

    public static bool ShowBadHttpRequestException => NHttpRequest.ShowBadHttpRequestException;

    public static int MinMessageLength => QueueUtils.MinMessageLength;

    public static bool JsonSerializer_CreateDefault => JsonExtensions.Options.CreateDefault;
    public static bool JsonSerializer_CamelCase => JsonExtensions.Options.CamelCase;

}

