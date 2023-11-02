namespace ClownFish.MQ.Messages;

#if NETCOREAPP

/// <summary>
/// 用于保存Http请求的数据类型
/// </summary>
public sealed class RequestData : IBinarySerializer, ITextSerializer
{
    /// <summary>
    /// 请求行
    /// </summary>
    public string RequestLine { get; private set; }

    /// <summary>
    /// 请求头，
    /// 多个头之间用换行符分隔。
    /// </summary>
    public string Headers { get; private set; }

    /// <summary>
    /// 请求体
    /// </summary>
    public byte[] Body { get; private set; }

    /// <summary>
    /// 构造方法
    /// </summary>
    public RequestData() { }


    internal RequestData(string requestLine, string headers, byte[] body)
    {
        RequestLine = requestLine;
        Headers = headers;
        Body = body;
    }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.RequestLine ?? "NULL";
    }


    /// <summary>
    /// 根据HttpRequest实例创建RequestData
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static RequestData Create(NHttpRequest request)
    {
        if( request == null )
            throw new ArgumentNullException(nameof(request));

        RequestData message = new RequestData();

        message.RequestLine = $"{request.HttpMethod} {request.FullUrl} HTTP/1.1";

        // 请求头有可能没有
        StringBuilder sb = StringBuilderPool.Get();
        try {
            request.AccessHeaders((k, v) => sb.Append(k).Append(": ").Append(v).Append('\n'));
            message.Headers = sb.ToString().TrimEnd();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
        // 请求体有可能为空
        message.Body = request.ReadBodyAsBytes();

        return message;
    }

    /// <summary>
    /// 根据HttpRequest实例创建RequestData
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<RequestData> CreateAsync(NHttpRequest request)
    {
        if( request == null )
            throw new ArgumentNullException(nameof(request));

        RequestData message = new RequestData();

        message.RequestLine = $"{request.HttpMethod} {request.FullUrl} HTTP/1.1";

        // 请求头有可能没有
        StringBuilder sb = StringBuilderPool.Get();
        try {
            request.AccessHeaders((k, v) => sb.Append(k).Append(": ").Append(v).Append('\n'));
            message.Headers = sb.ToString().TrimEnd();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
        // 请求体有可能为空
        message.Body = await request.ReadBodyAsBytesAsync();

        return message;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static RequestData FromRawText(string text)
    {
        if( text.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(text));

        RequestData data = new RequestData();

        using( StringReader reader = new StringReader(text.Trim()) ) {

            data.RequestLine = reader.ReadLine();

            string line = null;
            StringBuilder sb = StringBuilderPool.Get();
            try {
                while( (line = reader.ReadLine()) != null ) {
                    if( line.Length == 0 )
                        break;
                    else
                        sb.Append(line).Append('\n');
                }
                data.Headers = sb.ToString().TrimEnd();
            }
            finally {
                StringBuilderPool.Return(sb);
            }

            // 读取请求体数据
            string postText = reader.ReadToEnd();
            if( string.IsNullOrEmpty(postText) == false )
                data.Body = Encoding.UTF8.GetBytes(postText);
            else
                data.Body = Array.Empty<byte>();
        }

        return data;
    }

    byte[] IBinarySerializer.ToBytes()
    {
        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {

            // 在将数据转成二进制时，为了方便后面反向读取，将会写入2个长度到数据流中

            // 写请求行，先写长度，再写内容
            byte[] b1 = Encoding.UTF8.GetBytes(this.RequestLine);
            byte[] lenBytes = BitConverter.GetBytes(b1.Length);  // 长度固定为 4
            ms.Write(lenBytes, 0, lenBytes.Length);
            ms.WriteByte((byte)'\n');  // 在文本情况下方便阅读
            ms.Write(b1, 0, b1.Length);


            // 写请求头
            byte[] b2 = Encoding.UTF8.GetBytes(this.Headers);
            lenBytes = BitConverter.GetBytes(b2.Length);  // 长度固定为 4
            ms.Write(lenBytes, 0, lenBytes.Length);
            if( b2.Length > 0 ) { // 请求头有可能没有
                ms.WriteByte((byte)'\n');  // 在文本情况下方便阅读
                ms.Write(b2, 0, b2.Length);
            }

            // 写请求体
            if( this.Body != null && this.Body.Length > 0 ) {
                lenBytes = BitConverter.GetBytes(this.Body.Length);
                ms.Write(lenBytes, 0, lenBytes.Length);
                ms.WriteByte((byte)'\n');  // 在文本情况下方便阅读
                ms.Write(this.Body, 0, this.Body.Length);
            }
            else {
                // 没有请求体也写入一个“零”标记
                lenBytes = BitConverter.GetBytes(0);
                ms.Write(lenBytes, 0, lenBytes.Length);
            }

            return ms.ToArray();
        }
    }

    void IBinarySerializer.LoadData(ReadOnlyMemory<byte> body)
    {
        if( body.Length == 0 ) {
            this.RequestLine = string.Empty;
            this.Headers = string.Empty;
            this.Body = Array.Empty<byte>();
            return;
        }


        int start = 0;
        ReadOnlySpan<byte> span = body.Span;
        

        // 读取“请求行”的长度
        int len = BitConverter.ToInt32(span.Slice(start, 4));
        start += 4;
        start++; // 跳过 \n 符号

        // 读取“请求行” 二进制数据
        ReadOnlySpan<byte> data = span.Slice(start, len);
        start += len;

        this.RequestLine = Encoding.UTF8.GetString(data);

        // -------------------------------------------------------

        // 读取“请求头”的长度
        len = BitConverter.ToInt32(span.Slice(start, 4));
        start += 4;

        if( len > 0 ) {
            // 读取“请求头” 二进制数据
            start++; // 跳过 \n 符号
            data = span.Slice(start, len);
            start += len;

            this.Headers = Encoding.UTF8.GetString(data);
        }
        else {
            this.Headers = string.Empty;
        }

        // -------------------------------------------------------
        len = BitConverter.ToInt32(span.Slice(start, 4));
        start += 4;

        if(len > 0 ) {
            start++; // 跳过 \n 符号
            data = span.Slice(start, len);
            this.Body = data.ToArray();
        }
        else {
            this.Body = Array.Empty<byte>();
        }
    }

    string ITextSerializer.ToText()
    {
        return this.RequestLine + "\n"
                + (this.Headers.IsNullOrEmpty() ? "" :  this.Headers + "\n")
                + "\n"
                + this.Body.ToBase64();
    }

    void ITextSerializer.LoadData(string text)
    {
        if( text.IsNullOrEmpty() ) {
            this.RequestLine = string.Empty;
            this.Headers = string.Empty;
            this.Body = Array.Empty<byte>();
            return;
        }

        using( StringReader reader = new StringReader(text) ) {

            // 第一行，固定是请求行
            this.RequestLine = reader.ReadLine();

            // 中间是请求头
            string line = null;
            StringBuilder sb = StringBuilderPool.Get();
            try {
                while( (line = reader.ReadLine()) != null ) {

                    // 中间一个空行用于隔开请求体
                    if( line.Length == 0 )
                        break;
                    else
                        sb.Append(line).Append('\n');
                }
                this.Headers = sb.ToString().TrimEnd();
            }
            finally {
                StringBuilderPool.Return(sb);
            }

            // 最后尝试读取一行，可能是NULL
            line = reader.ReadLine();

            // 如果不NULL，就是请求体的BASE64
            if( line.IsNullOrEmpty() == false ) {
                this.Body = Convert.FromBase64String(line);
            }
            else {
                this.Body = Array.Empty<byte>();
            }
        }
    }


}
#endif
