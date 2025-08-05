#if NETCOREAPP
using Microsoft.IO;
#endif

namespace ClownFish.Base;

/// <summary>
/// MemoryStreamPool
/// </summary>
public static class MemoryStreamPool
{
#if NETCOREAPP

    private static readonly RecyclableMemoryStreamManager s_msPool = new RecyclableMemoryStreamManager(
                               new RecyclableMemoryStreamManager.Options {
                                   BlockSize = ClownFishOptions.MemoryStreamPool_BlockSize,
                                   LargeBufferMultiple = ClownFishOptions.MemoryStreamPool_LargeBufferMultiple,
                                   MaximumBufferSize = ClownFishOptions.MemoryStreamPool_MaximumBufferSize,
                                   MaximumSmallPoolFreeBytes = ClownFishOptions.MemoryStreamPool_MaximumSmallPoolFreeBytes,
                                   MaximumLargePoolFreeBytes = ClownFishOptions.MemoryStreamPool_MaximumLargePoolFreeBytes,
                                   UseExponentialLargeBuffer = ClownFishOptions.MemoryStreamPool_UseExponentialLargeBuffer
                               });


    /// <summary>
    /// GetStream
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="requiredSize"></param>
    /// <returns></returns>
    public static MemoryStream GetStream(string tag, int requiredSize)
    {
        if( ClownFishOptions.MemoryStreamPool_Enable )
            return s_msPool.GetStream(tag, requiredSize);
        else
            return new MemoryStream(requiredSize);
    }


    /// <summary>
    /// GetStream
    /// </summary>
    /// <returns></returns>
    public static MemoryStream GetStream()
    {
        if( ClownFishOptions.MemoryStreamPool_Enable )
            return s_msPool.GetStream();
        else
            return new MemoryStream(1024);
    }

    internal static DebugReportBlock GetStatus()
    {
        DebugReportBlock block = new DebugReportBlock { Category = "MemoryStreamPool Status" };
        block.AppendLine("SmallPoolFreeSize: " + s_msPool.SmallPoolFreeSize.ToKString());
        block.AppendLine("SmallPoolInUseSize: " + s_msPool.SmallPoolInUseSize.ToKString());
        block.AppendLine("SmallBlocksFree: " + s_msPool.SmallBlocksFree.ToString());

        block.AppendLine("LargePoolFreeSize: " + s_msPool.LargePoolFreeSize.ToKString());
        block.AppendLine("LargePoolInUseSize: " + s_msPool.LargePoolInUseSize.ToKString());
        block.AppendLine("LargeBuffersFree: " + s_msPool.LargeBuffersFree.ToString());

        return block;
    }

#else
    /// <summary>
    /// GetStream
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="requiredSize"></param>
    /// <returns></returns>
    public static MemoryStream GetStream(string tag, int requiredSize)
    {
        return new MemoryStream(requiredSize);
    }

    /// <summary>
    /// GetStream
    /// </summary>
    /// <returns></returns>
    public static MemoryStream GetStream()
    {
        return new MemoryStream();
    }
#endif


}
