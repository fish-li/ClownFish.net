namespace ClownFish.Base;

#if NETCOREAPP
/// <summary>
/// Brotli算法的扩展工具类
/// </summary>
public static class BrotliHelper
{
    /// <summary>
    /// 压缩一个字符串，并以BASE64字符串的形式返回压缩后的结果
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Compress(string input)
    {
        if( string.IsNullOrEmpty(input) )
            return input;

        byte[] bb = Encoding.UTF8.GetBytes(input);
        byte[] b2 = ToBrotli(bb);
        return Convert.ToBase64String(b2);
    }

    /// <summary>
    /// 解压缩一个BASE64字符串
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static string Decompress(string base64)
    {
        if( string.IsNullOrEmpty(base64) )
            return base64;

        byte[] bb = Convert.FromBase64String(base64);
        byte[] b2 = UnBrotli(bb);
        return Encoding.UTF8.GetString(b2);
    }


    /// <summary>
    /// 将一个文本转指定编码后做Brotli压缩
    /// </summary>
    /// <param name="text"></param>
    /// <param name="encoding">默认UTF8</param>
    /// <returns></returns>
    public static byte[] ToBrotli(this string text, Encoding encoding = null)
    {
        if( text == null )
            return null;

        if( text.Length == 0 )
            return Empty.Array<byte>();

        if( encoding == null )
            encoding = Encoding.UTF8;

        byte[] buffer = encoding.GetBytes(text);

        return ToBrotli(buffer);
    }



    /// <summary>
    /// 用Brotli压缩格式压缩一个二进制数组
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Usage", "CA2202")]
    public static byte[] ToBrotli(this byte[] input)
    {
        if( input == null )
            return null;

        if( input.Length == 0 )
            return Empty.Array<byte>();

        using( MemoryStream resultStream = MemoryStreamPool.GetStream() ) {
            using( BrotliStream zipStream = new BrotliStream(resultStream, CompressionLevel.SmallestSize, true) ) {

                zipStream.Write(input, 0, input.Length);
                zipStream.Close();

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
    public static byte[] UnBrotli(this byte[] input)
    {
        if( input == null )
            return null;

        if( input.Length == 0 )
            return Empty.Array<byte>();

        using( MemoryStream sourceStream = new MemoryStream(input, false) ) {
            using( BrotliStream zipStream = new BrotliStream(sourceStream, CompressionMode.Decompress, true) ) {

                return zipStream.ToArray();
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
        return ToBrotli(input);
    }


    /// <summary>
    /// 解压缩一个二进制数组
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Usage", "CA2202")]
    public static byte[] Decompress(byte[] input)
    {
        return UnBrotli(input);
    }
}
#endif
