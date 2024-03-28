﻿#if NETCOREAPP
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
                    ClownFishOptions.MemoryStreamPool_BlockSize,
                    ClownFishOptions.MemoryStreamPool_LargeBufferMultiple,
                    ClownFishOptions.MemoryStreamPool_MaximumBufferSize
    );

    /// <summary>
    /// GetStream
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="requiredSize"></param>
    /// <returns></returns>
    public static MemoryStream GetStream(string tag, int requiredSize)
    {
        return s_msPool.GetStream(tag, requiredSize);
    }


    /// <summary>
    /// GetStream
    /// </summary>
    /// <returns></returns>
    public static MemoryStream GetStream()
    {
        return s_msPool.GetStream();
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
