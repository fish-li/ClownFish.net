namespace ClownFish.Base;

internal static class CacheOption
{
    /// <summary>
    /// AppCache类型的默认缓存时间，单位：秒
    /// 默认值：180 秒
    /// </summary>
    public static readonly int AppCacheSeconds = LocalSettings.GetUInt("ClownFish_AppCache_CacheSeconds", 180).Min(10);

    /// <summary>
    /// CacheDictionary&lt;T&gt;类型的主动过期清理周期，单位：秒
    /// 默认值：1800 秒
    /// </summary>
    public static readonly int ExpirationScanFrequency = LocalSettings.GetUInt("ClownFish_CacheDictionary_ExpirationScanFrequency", 1800).Min(20); // 默认半小时执行一次主动过期扫描

}
