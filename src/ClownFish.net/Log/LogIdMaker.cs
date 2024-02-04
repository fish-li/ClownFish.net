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
    public static string GetNewId() => LogIdMakerV2.Instance.GetNewId();

    /// <summary>
    /// 根据时间生成一个新的日志ID
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string GetNewId(DateTime time) => LogIdMakerV2.Instance.GetNewId(time);

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
    /// 生成一个新的日志ID
    /// </summary>
    /// <returns></returns>
    string GetNewId();

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

    public string GetNewId()
    {
        return GetNewId(DateTime.Now);
    }

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

        byte[] resultBytes = new byte[18];
        Buffer.BlockCopy(timestampBytes, 2, resultBytes, 0, 6);
        Buffer.BlockCopy(randomBytes, 0, resultBytes, 6, 12);

        return resultBytes.ToUrlBase64();
    }

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

    public string GetNewId()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N");
    }

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

