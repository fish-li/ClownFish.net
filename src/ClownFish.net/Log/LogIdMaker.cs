using ClownFish.Jwt;

namespace ClownFish.Log;

/// <summary>
/// 日志ID生成器
/// </summary>
public static class LogIdMaker
{
    /// <summary>
    /// 生成一个新的日志ID
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetNewId() => GetNewId(DateTime.Now);

    /// <summary>
    /// 根据时间生成一个新的日志ID
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string GetNewId(DateTime time)
    {
#if NETCOREAPP
        return LogIdMakerV2.Instance.GetNewId2(time);
#else
        return LogIdMakerV2.Instance.GetNewId(time);
#endif
    }

    /// <summary>
    /// 从日志ID中提取时间
    /// </summary>
    /// <param name="logId"></param>
    /// <returns></returns>
    public static DateTime? ExtractTime(string logId)
    {
        if( logId.IsNullOrEmpty() )
            return null;

        return logId.Length switch {
            24 => LogIdMakerV2.Instance.ExtractTime(logId),
            49 => LogIdMakerV1.Instance.ExtractTime(logId),
            _ => null
        };
    }
}


/// <summary>
/// 日志ID生成器接口
/// </summary>
internal interface ILogIdMaker
{
    /// <summary>
    /// 根据时间生成一个新的日志ID
    /// </summary>
    /// <param name="time"></param>
    string GetNewId(DateTime time);

    /// <summary>
    /// 从日志ID中提取时间
    /// </summary>
    /// <param name="logId"></param>
    /// <returns></returns>
    DateTime? ExtractTime(string logId);
}

internal class LogIdMakerV2 : ILogIdMaker
{
    internal static readonly LogIdMakerV2 Instance = new LogIdMakerV2();

    private static readonly RandomNumberGenerator s_randomGenerator = RandomNumberGenerator.Create();

    public string GetNewId(DateTime time)
    {
        // 这里参考了 GuidHelper 的实现方式

        byte[] randomBytes = new byte[12];
        s_randomGenerator.GetBytes(randomBytes);

        long timestamp = time.Ticks / 10000L;
        byte[] timestampBytes = BitConverter.GetBytes(timestamp);

        if( BitConverter.IsLittleEndian ) {
            Array.Reverse(timestampBytes);
        }

        byte[] resultBytes = new byte[18];  // 18位byte 可以确保生成的字符串结果长度是 固定长度 24
        Buffer.BlockCopy(timestampBytes, 2, resultBytes, 0, 6); // 丢弃开头2位
        Buffer.BlockCopy(randomBytes, 0, resultBytes, 6, 12);

        return resultBytes.ToUrlBase64();
    }

#if NETCOREAPP
    public string GetNewId2(DateTime time)
    {
        long timestamp = time.Ticks / 10000L;
        byte[] timestampBytes = BitConverter.GetBytes(timestamp);

        if( BitConverter.IsLittleEndian ) {
            Array.Reverse(timestampBytes);
        }

        Span<byte> resultBytes = stackalloc byte[18];  // 18位byte 可以确保生成的字符串结果长度是 固定长度 24

        Span<byte> timeSpan = new Span<byte>(timestampBytes, 2, 6); // 丢弃开头2位
        timeSpan.CopyTo(resultBytes.Slice(0, 6));

        s_randomGenerator.GetBytes(resultBytes.Slice(6));

        return ((ReadOnlySpan<byte>)resultBytes).ToUrlBase64();
    }
#endif

    public DateTime? ExtractTime(string logId)
    {
        if( logId.IsNullOrEmpty() || logId.Length != 24 )
            return null;

        try {
            byte[] bytes = logId.FromUrlBase64();

            byte[] timestampBytes = new byte[8];
            Buffer.BlockCopy(bytes, 0, timestampBytes, 2, 6);

            if( BitConverter.IsLittleEndian ) {
                Array.Reverse(timestampBytes);
            }

            long timestamp = BitConverter.ToInt64(timestampBytes, 0) * 10000L;
            return new DateTime(timestamp);
        }
        catch {
            // 忽略错误
            return null;
        }
    }
}

internal class LogIdMakerV1 : ILogIdMaker
{
    internal static readonly LogIdMakerV1 Instance = new LogIdMakerV1();

    public string GetNewId(DateTime time)
    {
        return time.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N");
    }

    public DateTime? ExtractTime(string logId)
    {
        if( logId.IsNullOrEmpty() || logId.Length != 49 ) 
            return null;        

        string timeValue = logId.Substring(0, 14);  // 忽略毫秒数
        return timeValue.ToDateTime();
    }
}

