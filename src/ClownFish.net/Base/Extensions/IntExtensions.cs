namespace ClownFish.Base;

/// <summary>
/// int 相关扩展工具类
/// </summary>
public static class IntExtensions
{
    /// <summary>
    /// 如果当前值小于指定的最小值，就返回指定的最小值。
    /// </summary>
    /// <param name="number"></param>
    /// <param name="min"></param>
    /// <returns></returns>
    public static int Min(this int number, int min)
    {
        return number < min ? min : number;
    }

    /// <summary>
    /// 如果当前值小于指定的最小值，就返回指定的最小值。
    /// </summary>
    /// <param name="number"></param>
    /// <param name="min"></param>
    /// <returns></returns>
    public static long Min(this long number, long min)
    {
        return number < min ? min : number;
    }


    /// <summary>
    /// 如果当前值大于指定的最大值，就返回指定的最大值。
    /// </summary>
    /// <param name="number"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Max(this int number, int max)
    {
        return number > max ? max : number;
    }

    /// <summary>
    /// 如果当前值大于指定的最大值，就返回指定的最大值。
    /// </summary>
    /// <param name="number"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static long Max(this long number, long max)
    {
        return number > max ? max : number;
    }


    /// <summary>
    /// 如果number值小于或者等于0，就返回指定的newValue。
    /// </summary>
    /// <param name="number"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public static int If0Set(this int number, int newValue)
    {
        return number <= 0 ? newValue : number;
    }

    /// <summary>
    /// 如果number值小于或者等于0，就返回指定的newValue。
    /// </summary>
    /// <param name="number"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public static long If0Set(this long number, long newValue)
    {
        return number <= 0 ? newValue : number;
    }


    /// <summary>
    /// 判断一个值是否在某个范围内
    /// </summary>
    /// <param name="number"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static bool IsBetween(this int number, int min, int max)
    {
        return (number >= min && number <= max);
    }

    /// <summary>
    /// 判断一个值是否在某个范围内
    /// </summary>
    /// <param name="number"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static bool IsBetween(this long number, long min, long max)
    {
        return (number >= min && number <= max);
    }

    /// <summary>
    /// 将一个数字转成 KB, MB 这样的显示文本
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string MemSizeToGMK(this long size)
    {
        if( size <= 0 )
            return size.ToString();

        if( size < 1024 )
            return size.ToString() + "B";

        if( size < 1024 * 1024 )
            return ((1d * size) / 1024).ToString("N2") + "K";

        if( size < 1024L * 1024 * 1024 )
            return ((1d * size) / 1024 / 1024).ToString("N2") + "M";

        if( size < 1024L * 1024 * 1024 * 1024 )
            return ((1d * size) / 1024 / 1024 / 1024).ToString("N2") + "G";

        return ((1d * size) / 1024 / 1024 / 1024 / 1024).ToString("N2") + "T";
    }
}
