namespace ClownFish.Base;

/// <summary>
/// 包含DataTime类型相关的扩展方法
/// </summary>
public static class DateTimeExtensions
{

    /// <summary>
    /// 将DateTime转换成TimeSpan，仅保留 hh:mm:ss（不包含日期和毫秒）
    /// </summary>
    /// <param name="datetime"></param>
    /// <returns></returns>
    public static TimeSpan ToTimeSpan(this DateTime datetime)
    {
        return new TimeSpan(datetime.Hour, datetime.Minute, datetime.Second);
    }


    /// <summary>
    /// 将TimeSpan转换成 hh:mm:ss 格式的字符串
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string ToTime8String(this TimeSpan time)
    {
        return time.ToString(@"hh\:mm\:ss");
    }

    /// <summary>
	/// 返回包含日期时间格式的字符串（"yyyy-MM-dd HH:mm:ss.fffffff"）
	/// </summary>
	/// <param name="datetime"></param>
	/// <returns></returns>
	public static string ToTime27String(this DateTime datetime)
    {
        return datetime.ToString(DateTimeStyle.Time27);
    }

    /// <summary>
	/// 返回包含日期时间格式的字符串（"yyyy-MM-dd HH:mm:ss.fff"）
	/// </summary>
	/// <param name="datetime"></param>
	/// <returns></returns>
	public static string ToTime23String(this DateTime datetime)
    {
        return datetime.ToString(DateTimeStyle.Time23);
    }


    /// <summary>
    /// 返回包含日期时间格式的字符串（"yyyy-MM-dd HH:mm:ss"）
    /// </summary>
    /// <param name="datetime"></param>
    /// <returns></returns>
    public static string ToTimeString(this DateTime datetime)
    {
        return datetime.ToString(DateTimeStyle.Time19);
    }


    /// <summary>
    /// 返回仅仅包含日期格式的字符串（"yyyy-MM-dd"）
    /// </summary>
    /// <param name="datetime"></param>
    /// <returns></returns>
    public static string ToDateString(this DateTime datetime)
    {
        return datetime.ToString(DateTimeStyle.Date10);
    }

    /// <summary>
    /// 返回包含日期时间格式的字符串（"yyyyMMddHHmmss"）
    /// </summary>
    /// <param name="datetime"></param>
    /// <returns></returns>
    public static string ToTime14(this DateTime datetime)
    {
        return datetime.ToString(DateTimeStyle.Time14);
    }


    /// <summary>
    /// 日期转字符串
    /// </summary>
    /// <param name="datetime"></param>
    /// <returns></returns>
    public static string ToDate8(this DateTime datetime)
    {
        return datetime.ToString(DateTimeStyle.Date8);
    }


    /// <summary>
    /// 将一个字符串转成DateTime类型，可支持的格式：
    /// yyyyMMdd
    /// yyyy-MM-dd
    /// yyyy-MM-dd HH:mm:ss
    /// yyyyMMddHHmmss
    /// long ticks
    /// </summary>
    /// <param name="timeString"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(this string timeString)
    {
        if( string.IsNullOrEmpty(timeString) )
            throw new ArgumentNullException(nameof(timeString));

        if( timeString.Length == 8 ) {     // yyyyMMdd
            return ParseTime8(timeString);
        }

        if( timeString.Length < 10 )
            throw new ArgumentException("不能解析的时间字符串：" + timeString);


        if( timeString.Length == 14 ) {     // yyyyMMddHHmmss
            return ParseTime14(timeString);
        }

        if( timeString[4] == '-' && timeString[7] == '-' ) {    // yyyy-MM-dd or yyyy-MM-dd HH:mm:ss
            return ParseTime10or19(timeString);
        }

        // 最后按 ticks 整数 模式来解析， 
        // for example: 636533096875234446, length == 18
        return ParseTimeAsTicks(timeString);
    }


    /// <summary>
    /// 尝试将一个字符串转成DateTime类型
    /// </summary>
    /// <param name="timeString"></param>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static bool TryToDateTime(this string timeString, out DateTime dateTime)
    {
        try {
            dateTime = ToDateTime(timeString);
            return true;
        }
        catch {
            dateTime = DateTime.MinValue;
            return false;
        }
    }


    /// <summary>
    /// 尝试将一个字符串转成DateTime类型
    /// </summary>
    /// <param name="timeString"></param>
    /// <param name="defaultTime"></param>
    /// <returns></returns>
    public static DateTime TryToDateTime(this string timeString, DateTime? defaultTime = null)
    {
        try {
            return ToDateTime(timeString);
        }
        catch {
            if( defaultTime.HasValue )
                return defaultTime.Value;
            else
                return DateTime.MinValue;
        }
    }

    private static DateTime ParseTime8(string timeString)
    {
        try {
#if NETFRAMEWORK
            int year = int.Parse(timeString.Substring(0, 4));
            int month = int.Parse(timeString.Substring(4, 2));
            int day = int.Parse(timeString.Substring(6, 2));
            return new DateTime(year, month, day);
#else
            ReadOnlySpan<char> chars = timeString.ToCharArray();
            int year = int.Parse(chars.Slice(0, 4));
            int month = int.Parse(chars.Slice(4, 2));
            int day = int.Parse(chars.Slice(6, 2));
            return new DateTime(year, month, day);
#endif
        }
        catch( Exception ex ) {
            throw new ArgumentException("不能解析的时间字符串：" + timeString, ex);
        }
    }

    private static DateTime ParseTime14(string timeString)
    {
        try {
#if NETFRAMEWORK
            int year = int.Parse(timeString.Substring(0, 4));
            int month = int.Parse(timeString.Substring(4, 2));
            int day = int.Parse(timeString.Substring(6, 2));
            int hour = int.Parse(timeString.Substring(8, 2));
            int minute = int.Parse(timeString.Substring(10, 2));
            int second = int.Parse(timeString.Substring(12, 2));
            return new DateTime(year, month, day, hour, minute, second);
#else
            ReadOnlySpan<char> chars = timeString.ToCharArray();
            int year = int.Parse(chars.Slice(0, 4));
            int month = int.Parse(chars.Slice(4, 2));
            int day = int.Parse(chars.Slice(6, 2));
            int hour = int.Parse(chars.Slice(8, 2));
            int minute = int.Parse(chars.Slice(10, 2));
            int second = int.Parse(chars.Slice(12, 2));
            return new DateTime(year, month, day, hour, minute, second);
#endif
        }
        catch( Exception ex ) {
            throw new ArgumentException("不能解析的时间字符串：" + timeString, ex);
        }
    }


    private static DateTime ParseTime10or19(string timeString)
    {
        if( timeString.Length == 10 || timeString.Length >= 19 ) {     // yyyy-MM-dd or yyyy-MM-dd HH:mm:ss
            try {
                return DateTime.Parse(timeString);
            }
            catch( Exception ex ) {
                throw new ArgumentException("不能解析的时间字符串：" + timeString, ex);
            }
        }
        else {
            throw new ArgumentException("不能解析的时间字符串：" + timeString);
        }
    }


    private static DateTime ParseTimeAsTicks(string timeString)
    {
        try {
            long ticks = long.Parse(timeString);        // for example: 636533096875234446, length == 18
            return new DateTime(ticks);
        }
        catch( Exception ex ) {
            throw new ArgumentException("不能解析的时间字符串：" + timeString, ex);
        }
    }


    /// <summary>
    /// 将一个时间值的毫秒部分清零
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static DateTime SetMillisecondToZero(this DateTime time)
    {
        return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
    }

    private static readonly long s_maxTimeValue = DateTime.MaxValue.ToNumber();    // 99991231235959
    private static readonly long s_minTimeValue = DateTime.MinValue.ToNumber();    //    10101000000

    /// <summary>
    /// 将一个数字转成DateTime
    /// </summary>
    /// <param name="ticks"></param>
    /// <returns></returns>
    public static DateTime AsDateTime(this long ticks)
    {
        return new DateTime(ticks);
    }


    /// <summary>
    /// 将一个由 datetime.ToNumber() 产生的数字转成DateTime
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(this long number)
    {
        if( number > s_maxTimeValue )
            throw new ArgumentOutOfRangeException(nameof(number));

        if( number < s_minTimeValue )    // 这个判断用于简化项目代码的处理过程，不要删除！
            return DateTime.MinValue;

        int year = (int)(number / 100_00_00_00_00);
        long value = number - year * 100_00_00_00_00;

        int month = (int)(value / 100_00_00_00);
        value -= month * 100_00_00_00;

        int day = (int)(value / 100_00_00);
        value -= day * 100_00_00;

        int hour = (int)(value / 100_00);
        value -= hour * 100_00;

        int minute = (int)(value / 100);
        int second = (int)(number % 100);

        return new DateTime(year, month, day, hour, minute, second);
    }


    /// <summary>
    /// 将一个日期时间值转成【可读】数字，例如："2022-02-06 14:12:59" 转成  20220206141259
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static long ToNumber(this DateTime time)
    {
        return time.Second
             + time.Minute * 100
             + time.Hour * 100_00
             + time.Day * 100_00_00
             + time.Month * 100_00_00_00
             + time.Year * 100_00_00_00_00;
    }
}
