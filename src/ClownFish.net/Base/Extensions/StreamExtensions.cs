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


    /// <summary>
    /// 按字符串的方式读取流对象中的所有内容，默认使用UTF-8编码
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string ReadAsString(this Stream stream, Encoding encoding = null)
    {
        CheckStreamRead(stream);

        encoding = encoding ?? Encoding.UTF8;
        using( StreamReader reader = new StreamReader(stream, encoding, true, 4096, true) ) {
            return reader.ReadToEnd();
        }
    }


    /// <summary>
    /// 在2个流对象之间复制数据
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="length"></param>
    /// <returns>实际复制的数据长度</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static long CopyToWithLen(this Stream source, Stream destination, long length)
    {
        if( source == null )
            throw new ArgumentNullException(nameof(source));
        if( destination == null )
            throw new ArgumentNullException(nameof(destination));
        if( length < 0 )
            throw new ArgumentOutOfRangeException(nameof(length));

        if( length == 0 )
            return 0;

        int bufferSize = 1024;
        long sum = 0;

        using( ByteBuffer byteBuffer = new ByteBuffer(bufferSize) ) {
            byte[] buffer = byteBuffer.Buffer;

            while( true ) {
                long remaining = length - sum;
                long readSize = remaining < bufferSize ? remaining : bufferSize;

                int len = source.Read(buffer, 0, (int)readSize);
                sum += len;

                if( len > 0 ) {
                    destination.Write(buffer, 0, len);
                }
                else {
                    return sum;
                }

                if( sum >= length )
                    return sum;
            }
        }
    }



    /// <summary>
    /// 将一个数据流包装成 压缩流/解压缩流
    /// </summary>
    /// <param name="httpStream">请求流或者响应流</param>
    /// <param name="contentEncoding">压缩算法名称，可参考HTTP头 Content-Encoding 的取值</param>
    /// <param name="mode">压缩还是解压缩</param>
    /// <param name="leaveOpen"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static Stream CreateCompressionStream(this Stream httpStream, string contentEncoding, CompressionMode mode, bool leaveOpen = true)
    {
        return contentEncoding switch {
            null or "" => throw new ArgumentNullException(nameof(contentEncoding)),
            "gzip" => new GZipStream(httpStream, mode, leaveOpen),
            "deflate" => new DeflateStream(httpStream, mode, leaveOpen),
#if NETCOREAPP
            "br" => new BrotliStream(httpStream, mode, leaveOpen),
#endif
            _ => throw new NotSupportedException("当前.NET版本不支持此压缩算法: " + contentEncoding)
        };
    }


    /// <summary>
    /// 为数据流创建StreamWriter，并可以添加压缩包装
    /// </summary>
    /// <param name="httpStream">请求流或者响应流</param>
    /// <param name="bufferSize">缓冲区大小</param>
    /// <returns></returns>
    public static StreamWriter CreateGzipWriter(this Stream httpStream, int bufferSize = 4096)
    {
        GZipStream zipStream = new GZipStream(httpStream, CompressionMode.Compress, true);
        return new StreamWriter(zipStream, EncodingUtils.UTF8NoBOM, bufferSize, false);
    }

    /// <summary>
    /// 为数据流创建StreamWriter
    /// </summary>
    /// <param name="httpStream"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public static StreamWriter CreateWriter(this Stream httpStream, int bufferSize = 4096)
    {
        return new StreamWriter(httpStream, EncodingUtils.UTF8NoBOM, bufferSize, false);
    }

    /// <summary>
    /// 为数据流创建StreamReader，并可以添加解压缩包装
    /// </summary>
    /// <param name="httpStream">请求流或者响应流</param>
    /// <param name="bufferSize">缓冲区大小</param>
    /// <returns></returns>
    public static StreamReader CreateGzipReader(this Stream httpStream, int bufferSize = 4096)
    {
        GZipStream zipStream = new GZipStream(httpStream, CompressionMode.Decompress, true);
        return new StreamReader(zipStream, Encoding.UTF8, true, bufferSize, false);
    }


}
