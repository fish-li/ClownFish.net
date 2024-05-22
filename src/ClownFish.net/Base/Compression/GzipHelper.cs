namespace ClownFish.Base;

/// <summary>
/// GZIP压缩相关的工具方法
/// </summary>
public static class GzipHelper
{
    /// <summary>
    /// 用GZIP压缩一个字符串，并以BASE64字符串的形式返回压缩后的结果
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Compress(string input)
    {
        if( string.IsNullOrEmpty(input) )
            return input;

        byte[] bb = Encoding.UTF8.GetBytes(input);
        byte[] gzipBB = ToGzip(bb);
        return Convert.ToBase64String(gzipBB);
    }

    /// <summary>
    /// 用GZIP解压缩一个BASE64字符串
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static string Decompress(string base64)
    {
        if( string.IsNullOrEmpty(base64) )
            return base64;

        byte[] bb = Convert.FromBase64String(base64);
        byte[] gzipBB = UnGzip(bb);
        return Encoding.UTF8.GetString(gzipBB);
    }


    /// <summary>
    /// 将一个文本转指定编码后做Gzip压缩
    /// </summary>
    /// <param name="text"></param>
    /// <param name="encoding">默认UTF8</param>
    /// <returns></returns>
    public static byte[] ToGzip(this string text, Encoding encoding = null)
    {
        if( text == null )
            return null;

        if( text.Length == 0 )
            return Empty.Array<byte>();

        if( encoding == null )
            encoding = Encoding.UTF8;

        byte[] buffer = encoding.GetBytes(text);

        return ToGzip(buffer);
    }



    /// <summary>
    /// 用Gzip压缩格式压缩一个二进制数组
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Usage", "CA2202")]
    public static byte[] ToGzip(this byte[] input)
    {
        if( input == null )
            return null;

        if( input.Length == 0 )
            return Empty.Array<byte>();

        using( MemoryStream resultStream = MemoryStreamPool.GetStream() ) {
            using( GZipStream gZipStream = new GZipStream(resultStream, CompressionMode.Compress, true) ) {

                gZipStream.Write(input, 0, input.Length);
                gZipStream.Close();

                resultStream.Position = 0;
                return resultStream.ToArray();

            }
        }
    }

    /// <summary>
    /// 解压缩一个二进制数组
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Usage", "CA2202")]
    public static byte[] UnGzip(this byte[] input)
    {
        if( input == null )
            return null;

        if( input.Length == 0 )
            return Empty.Array<byte>();

        using( MemoryStream sourceStream = new MemoryStream(input, false) ) {
            using( GZipStream gZipStream = new GZipStream(sourceStream, CompressionMode.Decompress, true) ) {

                return gZipStream.ToArray();
            }
        }
    }


    /// <summary>
    /// 压缩一个二进制数组
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Usage", "CA2202")]
    public static byte[] Compress(byte[] input)
    {
        return ToGzip(input);
    }


    /// <summary>
    /// 解压缩一个二进制数组
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Usage", "CA2202")]
    public static byte[] Decompress(byte[] input)
    {
        return UnGzip(input);
    }
}
