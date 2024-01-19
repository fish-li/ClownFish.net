namespace ClownFish.Base;

/// <summary>
/// Stream扩展方法工具类
/// </summary>
public static class StreamExtensions
{
    private static void CheckStreamRead(Stream stream)
    {
        if( stream == null )
            throw new ArgumentNullException(nameof(stream));

        if( stream.CanRead == false )
            throw new InvalidOperationException("当前流不可读。");


        if( stream.CanSeek )
            stream.Position = 0;
    }


    /// <summary>
    /// 将一个Stream的内容【复制到】MemoryStream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="ms"></param>
    public static void CopyToMemoryStream(this Stream stream, MemoryStream ms)
    {
        CheckStreamRead(stream);

        stream.CopyTo(ms);

        ms.Position = 0;
    }



    /// <summary>
    /// 将一个Stream的内容【复制到】MemoryStream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="ms"></param>
    public static async Task CopyToMemoryStreamAsync(this Stream stream, MemoryStream ms)
    {
        CheckStreamRead(stream);

        await stream.CopyToAsync(ms);

        ms.Position = 0;
    }


    private static byte[] TryGetBytes(Stream stream)
    {
        if( stream == null )
            throw new ArgumentNullException(nameof(stream));

        if( stream.CanRead == false )
            throw new InvalidOperationException("当前流不可读。");

        if( stream is MemoryStream ms ) {
            return ms.ToArray();
        }

        // 对于可以【定位】的流，几乎都是本地流，就一次性读取到 byte[]，避免用MemoryStream缓冲而浪费性能
        if( stream.CanSeek ) {

            // 强制定位开流的开头，尽量读取全部内容
            stream.Position = 0;

            byte[] buffer = new byte[stream.Length];
            int len = stream.Read(buffer, 0, buffer.Length);

            if( len == buffer.Length ) {
                return buffer;
            }
            else {
                // 这个分支应该永远不会进来的，除非流的开发作者设计有问题！
                // 如果流在设计上真有问题，这里就放弃一次性读取的方式
                stream.Position = 0;
                return null;
            }
        }
        else {
            // 对于不能定位的流（例如：网络流），就采用MemoryStream的缓冲方式读取
            return null;
        }
    }

    /// <summary>
    /// 获取流对象中的所有字节
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static byte[] ToArray(this Stream stream)
    {
        byte[] buffer = TryGetBytes(stream);
        if( buffer != null )
            return buffer;

        using( MemoryStream ms2 = MemoryStreamPool.GetStream() ) {
            stream.CopyTo(ms2);
            return ms2.ToArray();
        }
    }


    /// <summary>
    /// 获取流对象中的所有字节
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<byte[]> ToArrayAsync(this Stream stream)
    {
        byte[] buffer = TryGetBytes(stream);
        if( buffer != null )
            return buffer;

        using( MemoryStream ms2 = MemoryStreamPool.GetStream() ) {
            await stream.CopyToAsync(ms2);
            return ms2.ToArray();
        }
    }

}
