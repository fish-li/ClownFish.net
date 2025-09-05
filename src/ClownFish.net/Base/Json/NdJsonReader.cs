namespace ClownFish.Base;

/// <summary>
/// 从Ndjson数据流中读取对象的工具类
/// </summary>
public sealed class NdJsonReader : IDisposable
{
    private readonly StreamReader _streamReader;
    private bool _disposed = false;

    private readonly Stream _autoCloseStream;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="httpStream">请求流或者响应流</param>
    /// <param name="contentEncoding">压缩算法名称，可参考HTTP头 Content-Encoding 的取值</param>
    /// <param name="encoding">字符编码</param>
    /// <param name="autoCloseStream"></param>
    public NdJsonReader(Stream httpStream, string contentEncoding = null, Encoding encoding = null, bool autoCloseStream = false)
    {
        if( httpStream == null )
            throw new ArgumentNullException(nameof(httpStream));

        Encoding encoding2 = encoding ?? Encoding.UTF8;

        if( contentEncoding.IsNullOrEmpty() ) {
            _streamReader = new StreamReader(httpStream, encoding2, true, 1024, true);
        }
        else {
            Stream zipStream = httpStream.CreateCompressionStream(contentEncoding, CompressionMode.Decompress);
            _streamReader = new StreamReader(zipStream, encoding2, true, 1024, false);
        }

        if( autoCloseStream ) {
            _autoCloseStream = httpStream;
        }
    }


    /// <summary>
    /// 从NHttpRequest实例中构造NdJsonReader
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static NdJsonReader Create(NHttpRequest request)
    {
        if( request == null )
            throw new ArgumentNullException(nameof(request));

        HttpUtils.ParseContentType(request.ContentType, out string mediaType, out Encoding encoding);
        if( mediaType != RequestContentType.Ndjson )
            throw new InvalidOperationException($"请求体数据类型不是预期的ndjson格式，当前Content-Type={request.ContentType}");

        if( request.CheckBodyCanReadAndSetReadFlag() == false )
            throw new InvalidOperationException("请求体不可读或者已被读取过了！");

        string contentEncoding = request.Header(HttpHeaders.Request.ContentEncoding);

        return new NdJsonReader(request.InputStream, contentEncoding, encoding, false);
    }


    /// <summary>
    /// 从HttpResult&lt;Stream&gt;实例中构造NdJsonReader
    /// </summary>
    /// <param name="httpResult"></param>
    /// <param name="autoCloseStream"></param>
    /// <returns></returns>
    public static NdJsonReader Create(HttpResult<Stream> httpResult, bool autoCloseStream = true)
    {
        if( httpResult == null )
            throw new ArgumentNullException(nameof(httpResult));
        if( httpResult.Result == null )
            throw new ArgumentNullException(nameof(httpResult.Result));

        string contentType = httpResult.GetHeader(HttpHeaders.Request.ContentType);
        HttpUtils.ParseContentType(contentType, out string mediaType, out Encoding encoding);

        if( mediaType != RequestContentType.Ndjson )
            throw new InvalidOperationException($"响应体数据类型不是预期的ndjson格式，当前Content-Type={contentType}");


        string contentEncoding = httpResult.GetHeader(HttpHeaders.Request.ContentEncoding);
        return new NdJsonReader(httpResult.Result, contentEncoding, encoding, autoCloseStream);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if( _disposed )
            return;

        _disposed = true;

        if( _streamReader != null ) {
            _streamReader.Dispose();
        }

        if( _autoCloseStream != null ) {
            _autoCloseStream.Dispose();
        }
    }

    /// <summary>
    /// 从TextReader对象中逐行读取文本并转成对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<T> ReadLines<T>()
    {
        JsonSerializer jsonSerializer = JsonExtensions.CreateJsonSerializer(null);

        Type destType = typeof(T);

        while( true ) {
            string line = _streamReader.ReadLine();
            if( line == null )
                break;

            if( line.Length > 0 ) {

                TextReader reader2 = new StringReader(line);
                T item = (T)jsonSerializer.Deserialize(reader2, destType);
                yield return item;
            }
        }
    }

}
