namespace ClownFish.Base;

/// <summary>
/// 从Ndjson数据流中读取对象的工具类
/// </summary>
public sealed class NdJsonReader : IDisposable
{
    private readonly StreamReader _streamReader;
    private bool _disposed = false;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="httpStream">请求流或者响应流</param>
    /// <param name="contentEncoding">压缩算法名称，可参考HTTP头 Content-Encoding 的取值</param>
    /// <param name="encoding">字符编码</param>
    public NdJsonReader(Stream httpStream, string contentEncoding, Encoding encoding = null)
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
    }

    /// <summary>
    /// 从TextReader对象中逐行读取文本并转成对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<T> ReadLines<T>()
    {
        JsonSerializer jsonSerializer = ((JsonSerializerSettings)null).CreateJsonSerializer();

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
