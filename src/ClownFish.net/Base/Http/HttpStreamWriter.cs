namespace ClownFish.Base;


internal struct HttpStreamWriter
{
    private readonly Stream _responseStream;
    private readonly string _contentEncoding;

    public HttpStreamWriter(Stream responseStream, string contentEncoding = null)
    {
        if( responseStream == null )
            throw new ArgumentNullException(nameof(responseStream));

        if( HttpStreamReader.CheckContentEncoding(contentEncoding) == false )
            throw new NotSupportedException("当前.NET版本不支持此压缩算法: " + contentEncoding);

        _responseStream = responseStream;
        _contentEncoding = contentEncoding;
    }

    private void EnsureCanWrite()
    {
        if( _responseStream.CanWrite == false )
            throw new InvalidOperationException("流不可写！");
    }

    public async Task WriteAsync(byte[] data)
    {
        if( data.IsNullOrEmpty() )
            return;

        EnsureCanWrite();

        if( _contentEncoding.IsNullOrEmpty() ) {
            await _responseStream.WriteAsync(data, 0, data.Length);
            return;
        }

        Stream zipStream = HttpStreamReader.WrapperCompressionStream(_responseStream, _contentEncoding, CompressionMode.Compress);

        if( zipStream != null ) {
            using( zipStream ) {
                await zipStream.WriteAsync(data, 0, data.Length);
                zipStream.Flush();
                return;
            }
        }

        await _responseStream.WriteAsync(data, 0, data.Length);
    }

    public async Task<int> WriteAsync(string text, Encoding encoding = null)
    {
        if( text.IsNullOrEmpty() )
            return 0;

        Encoding encoding2 = encoding ?? Encoding.UTF8;
        byte[] data = encoding2.GetBytes(text);

        await WriteAsync(data);
        return data.Length;
    }


}
