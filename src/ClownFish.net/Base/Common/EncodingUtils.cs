namespace ClownFish.Base;
internal static class EncodingUtils
{
    public static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);

    public static Encoding GetEncodingFromString(string encodingName)
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
