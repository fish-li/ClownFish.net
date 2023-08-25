namespace ClownFish.Base;

/// <summary>
/// UNIX时间相关的工具类
/// </summary>
public static class UnixTime
{
    /// <summary>
    /// 所谓的UNIX新纪元开始时间 UTC 1970-01-01 00:00:00
    /// </summary>
    public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// 所谓的UNIX新纪元开始时间 本地时间 1970-01-01 00:00:00
    /// </summary>
    public static readonly DateTime EpochLocalTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);


    /// <summary>
    /// 获取当前时刻的 UNIX nanosecond 时间戳。
    /// 可用于InfluxDB插入数据时指定当前时间（InfluxDB uses the server’s local nanosecond timestamp in UTC）。
    /// </summary>
    /// <returns></returns>
    public static long GetUtcNanoTime()
    {
        return (DateTime.UtcNow - EpochTime).Ticks * 100;
    }

    /// <summary>
    /// 将一个UTC时间转成 UNIX nanosecond 时间戳。
    /// </summary>
    /// <param name="utcTime"></param>
    /// <returns></returns>
    public static long GetUtcNanoTime(this DateTime utcTime)
    {
        return (utcTime - EpochTime).Ticks * 100;
    }

    /// <summary>
    /// 将一个本地时间转成 UNIX nanosecond 时间戳。
    /// </summary>
    /// <param name="localTime"></param>
    /// <returns></returns>
    public static long GetNanoTime(this DateTime localTime)
    {
        return (localTime.ToUniversalTime() - EpochTime).Ticks * 100;
    }


    ///// <summary>
    ///// 将 UNIX nanosecond 时间戳转换成 .NET DateTime
    ///// </summary>
    ///// <param name="nanoTimestamp"></param>
    ///// <returns></returns>
    //public static DateTime ToUtcDateTime(long nanoTimestamp)
    //{
    //    long ticks = (long)(nanoTimestamp / 100) + EpochTime.Ticks;
    //    return new DateTime(ticks, DateTimeKind.Utc);
    //}


    /// <summary>
    /// 将 UNIX nanosecond 时间戳转换成 .NET Local DateTime
    /// </summary>
    /// <param name="nanoTimestamp"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(long nanoTimestamp)
    {
        long ticks = (long)(nanoTimestamp / 100) + EpochTime.Ticks;
        DateTime time = new DateTime(ticks, DateTimeKind.Utc);
        return time.ToLocalTime();
    }




}
