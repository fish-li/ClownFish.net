namespace ClownFish.Base;

/// <summary>
/// Encoding相关工具类
/// </summary>
public static class EncodingUtils
{
    /// <summary>
    /// UTF8 without BOM header
    /// </summary>
    public static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);

    /// <summary>
    /// 根据名称返回对应的Encoding实例，如果失败返回null
    /// </summary>
    /// <param name="encodingName"></param>
    /// <returns></returns>
    public static Encoding GetEncoding(string encodingName)
    {
        if( string.IsNullOrEmpty(encodingName) )
            return null;

        try {
            return Encoding.GetEncoding(encodingName);
        }
        catch {
            /* 忽略无效的 charset 值 */
            return null;
        }
    }
}
