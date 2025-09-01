namespace ClownFish.Http.Utils;

/// <summary>
/// 从HTTP流中读取数据。当数据流是压缩格式时自动解压缩。
/// </summary>
public struct HttpStreamReader
{
    private readonly Stream _httpStream;
    private readonly string _contentEncoding;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="httpStream">Request.InputStream or HttpResponseMessage.Content</param>
    /// <param name="contentEncoding">Content-Encoding 请求头/响应头</param>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpStreamReader(Stream httpStream, string contentEncoding = null)
    {
        if( httpStream == null )
            throw new ArgumentNullException(nameof(httpStream));

        if( CheckContentEncoding(contentEncoding) == false )
            throw new NotSupportedException("当前.NET版本不支持此压缩算法: " + contentEncoding);


        _httpStream = httpStream;
        _contentEncoding = contentEncoding;
    }

    private void EnsureCanRead()
    {
        if( _httpStream.CanRead == false )
            throw new InvalidOperationException("流不可读！");

        if( _httpStream.CanSeek )
            _httpStream.Position = 0;
    }

    internal static bool CheckContentEncoding(string contentEncoding)
    {
        if( contentEncoding.IsNullOrEmpty() )
            return true;

        return contentEncoding switch {
            "gzip" => true,
            "deflate" => true,
#if NETCOREAPP
            "br" => true,
#endif
            _ => false   // 不支持的压缩算法
        };
    }



    /// <summary>
    /// 按文本方式读取流数据
    /// </summary>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public string ReadAllText(Encoding encoding = null)
    {
        EnsureCanRead();

        Encoding encoding2 = encoding ?? Encoding.UTF8;

        if( _contentEncoding.IsNullOrEmpty() ) {
            return ReadAllText0(_httpStream, encoding2);
        }

        // 创建一个解压缩流的包装
        Stream zipStream = _httpStream.CreateCompressionStream(_contentEncoding, CompressionMode.Decompress);

        using( zipStream ) {
            return ReadAllText0(zipStream, encoding2);
        }
    }

    /// <summary>
    /// 按文本方式读取流数据
    /// </summary>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public async Task<string> ReadAllTextAsync(Encoding encoding = null)
    {
        EnsureCanRead();

        Encoding encoding2 = encoding ?? Encoding.UTF8;

        if( _contentEncoding.IsNullOrEmpty() ) {
            return await ReadAllText0Async(_httpStream, encoding2);
        }

        // 创建一个解压缩流的包装
        Stream zipStream = _httpStream.CreateCompressionStream(_contentEncoding, CompressionMode.Decompress);

        using( zipStream ) {
            return await ReadAllText0Async(zipStream, encoding2);
        }
    }



    private static string ReadAllText0(Stream stream, Encoding encoding)
    {
        using StreamReader reader = new StreamReader(stream, encoding, true, 1024, true);
        return reader.ReadToEnd();
    }

    private static async Task<string> ReadAllText0Async(Stream stream, Encoding encoding)
    {
        using StreamReader reader = new StreamReader(stream, encoding, true, 1024, true);
        return await reader.ReadToEndAsync();
    }





    #region  下面 2 个方法没有实际使用场景，所以先注释起来

    //public byte[] ReadAllBytes()
    //{
    //    EnsureCanRead();

    //    if( _contentEncoding.IsNullOrEmpty() )
    //        return _httpStream.ToArray();

    //    // 创建一个解压缩流的包装
    //    Stream zipStream = CreateCompressionStream(_httpStream, _contentEncoding, CompressionMode.Decompress);

    //    using( zipStream ) {
    //        return zipStream.ToArray();
    //    }
    //}


    //public async Task<byte[]> ReadAllBytesAsync()
    //{
    //    EnsureCanRead();

    //    if( _contentEncoding.IsNullOrEmpty() )
    //        return await _httpStream.ToArrayAsync();

    //    // 创建一个解压缩流的包装
    //    Stream zipStream = CreateCompressionStream(_httpStream, _contentEncoding, CompressionMode.Decompress);

    //    using( zipStream ) {
    //        return await zipStream.ToArrayAsync();
    //    }
    //}

    #endregion




}
