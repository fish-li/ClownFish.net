namespace ClownFish.Base;

internal static class EncodingExtensions
{
    /// <summary>
    /// 如果参数为NULL，则返回默认值，否则返回本身
    /// </summary>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static Encoding GetOrDefault (this Encoding encoding)
    {
        return encoding ?? Encoding.UTF8;
    }
}
