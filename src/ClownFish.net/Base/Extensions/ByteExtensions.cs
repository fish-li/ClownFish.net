using ClownFish.Jwt;

namespace ClownFish.Base;

/// <summary>
/// 二进制数据操作的工具类
/// </summary>
public static class ByteExtensions
{
    /// <summary>
    /// 比较二个字节数组是不是相等
    /// </summary>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    /// <returns></returns>
    public static bool IsEqual(this byte[] b1, byte[] b2)
    {
        if( b1 == null && b2 == null )
            return true;

        if( b1 == null || b2 == null )
            return false;

        if( b1.Length != b2.Length )
            return false;

        for( int i = 0; i < b1.Length; i++ ) {
            if( b1[i] != b2[i] )
                return false;
        }

        return true;
    }



    /// <summary>
	/// 将byte[]做BASE64编码，Convert.ToBase64String(bytes);
	/// </summary>
	/// <param name="bytes"></param>
	/// <returns></returns>
	public static string ToBase64(this byte[] bytes)
    {
        if( bytes == null || bytes.Length == 0 )
            return string.Empty;

        return Convert.ToBase64String(bytes);
    }


    /// <summary>
	/// 将byte[]按十六进制转换成字符串，BitConverter.ToString(bytes).Replace("-", "");
	/// </summary>
	/// <param name="bytes"></param>
	/// <returns></returns>
	public static string ToHexString(this byte[] bytes)
    {
        if( bytes == null || bytes.Length == 0 )
            return string.Empty;

#if NET6_0_OR_GREATER
        return Convert.ToHexString(bytes);
#else
        return BitConverter.ToString(bytes).Replace("-", "");
#endif
    }


    /// <summary>
    /// 将二进制数据按UTF8编码方式转成对应的string
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToUtf8String(this byte[] bytes)
    {
        if( bytes == null || bytes.Length == 0 )
            return string.Empty;

        return Encoding.UTF8.GetString(bytes);
    }


    /// <summary>
    /// 将字符串按UTF8编码转成对应的二进制数据
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static byte[] ToUtf8Bytes(this string text)
    {
        if( text.IsNullOrEmpty() )
            return Empty.Array<byte>();

        return Encoding.UTF8.GetBytes(text);
    }

    /// <summary>
    /// 将二进制数据做 Base64 + UrlEncode 编码
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToUrlBase64(this byte[] bytes)
    {
        return NbJwtBase64UrlEncoder.Encode(bytes);
    }

    /// <summary>
    /// 从一个  Base64 + UrlEncode 编码的字符串中还原 二进制数据
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static byte[] FromUrlBase64(this string text)
    {
        return NbJwtBase64UrlEncoder.Decode(text);
    }
}
