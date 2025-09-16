namespace ClownFish.WebClient;

/// <summary>
/// 表示一个HTTP的调用结果，包含响应头和响应内容
/// </summary>
/// <typeparam name="T">响应内容的类型参数</typeparam>
public sealed partial class HttpResult<T> : IToAllText
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int StatusCode { get; private set; }

    /// <summary>
    /// 从服务端返回响应头集合
    /// </summary>
    public NameValueCollection Headers { get; private set; }

    /// <summary>
    /// 响应体中的结果
    /// </summary>
    public T Result { get; private set; }

    /// <summary>
    /// 响应头中的 Content-Type
    /// </summary>
    public string ContentType {
        get => this.Headers[HttpHeaders.Response.ContentType];
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="headers"></param>
    /// <param name="result"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpResult(int statusCode, NameValueCollection headers, T result)
    {
        this.StatusCode = statusCode;
        this.Headers = headers ?? new NameValueCollection();
        this.Result = result;
    }


    /// <summary>
    /// 将一个对象的所有信息全部转成文本形式输出
    /// </summary>
    /// <returns></returns>
    public string ToAllText()
    {
        return ToAllText(true);
    }

    /// <summary>
    /// 将HttpResult&lt;string&gt;实例转成可读文本
    /// </summary>
    /// <param name="includeBody">是否包含请求体部分</param>
    /// <returns></returns>
    public string ToAllText(bool includeBody)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {

            string status = ((HttpStatusCode)this.StatusCode).ToString();
            sb.Append("HTTP/1.1 ").Append(this.StatusCode.ToString()).Append(' ').AppendLineRN(status);

            foreach( string name in this.Headers.Keys ) {
                string[] values = this.Headers.GetValues(name);
                foreach( string value in values )
                    sb.Append(name).Append(": ").AppendLineRN(value);
            }

            sb.AppendLineRN();

            if( includeBody ) {
                sb.Append(GetResultAsText());
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal string GetResultAsText()
    {
        if( this.Result == null ) {
            return string.Empty;     // response body = null 没有意义
        }

        if( this.Result is string text ) {
            return text;
        }
        else if( this.Result is byte[] bytes ) {
            return bytes.ToBase64();
        }
        else {
            return this.Result.ToJson();
        }
    }

    /// <summary>
    /// 获取响应头
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetHeader(string name)
    {
        return this.Headers[name];
    }

}



#if NETCOREAPP
public sealed partial class HttpResult<T> : ITextSerializer, IBinarySerializer
{
    private void CheckStreamNotSupportSerialize()
    {
        if( typeof(T).IsCompatible(typeof(Stream)) )
            throw new NotSupportedException("Stream对象不支持序列化！");
    }

    string ITextSerializer.ToText()
    {
        CheckStreamNotSupportSerialize();

        return this.ToAllText(true);
    }

    void ITextSerializer.LoadData(string text)
    {
        CheckStreamNotSupportSerialize();

        this.StatusCode = -1;
        this.Headers = new NameValueCollection();
        this.Result = default(T);

        if( text.IsNullOrEmpty() )
            return;


        using( StringReader reader = new StringReader(text) ) {

            // 第一行，固定是开始行
            string responseLine = reader.ReadLine();
            string[] items = responseLine.Split(' ');   // responseLine示例值  "HTTP/1.1 200 OK"
            if( items.Length != 3 )
                throw new InvalidDataException($"responseLine is error, items.Length={items.Length}");

            this.StatusCode = int.Parse(items[1]);

            // 解析 响应头
            string line = null;
            while( (line = reader.ReadLine()) != null ) {

                // 中间一个空行用于隔开请求体
                if( line.Length == 0 ) {
                    break;
                }
                else {
                    int p = line.IndexOf(':');  // line示例  "name: value"
                    if( p > 0 && p < line.Length - 2 ) {
                        string name = line.Substring(0, p);
                        string value = line.Substring(p + 2);
                        this.Headers.Add(name, value);
                    }
                }
            }

            // 最后读取响应体，可能是NULL
            string body = reader.ReadToEnd();
            if( body.HasValue() ) {
                if( typeof(T) == typeof(string) ) {
                    this.Result = (T)(object)body;
                }
                else if( typeof(T) == typeof(byte[]) ) {
                    this.Result = (T)(object)Convert.FromBase64String(body);
                }
                else {
                    this.Result = body.FromJson<T>();
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal string GetHeadersAsText()
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( string name in this.Headers.Keys ) {
                string[] values = this.Headers.GetValues(name);
                foreach( string value in values )
                    sb.Append(name).Append('\n').Append(value).Append('\n');   // name, value 各占一行，简化后续解析过程
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    private static readonly char[] s_headersSplitChars = new char[] { '\n' };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void FillNameValueCollection(string text, NameValueCollection collection)
    {
        string[] items = text.Split(s_headersSplitChars, StringSplitOptions.RemoveEmptyEntries);
        if( items.Length % 2 != 0 )
            throw new InvalidDataException($"headers is error, items.Length={items.Length}");

        for( int i = 0; i < items.Length; i = i + 2 ) {
            string name = items[i];
            string value = items[i + 1];
            collection.Add(name, value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal byte[] GetResultAsBytes()
    {
        if( this.Result == null )
            return Empty.Array<byte>();

        if( this.Result is string text ) {
            return text.GetBytes();
        }
        else if( this.Result is byte[] bb ) {
            return bb;
        }
        else {
            return this.Result.ToJson().GetBytes();
        }
    }

    /// <summary>
    /// 将当前对象序列化为 BytesList 实例
    /// </summary>
    /// <returns></returns>
    public BytesList ToBytesList()
    {
        CheckStreamNotSupportSerialize();

        string headers = GetHeadersAsText();

        byte[] body = GetResultAsBytes();

        BytesList buffer = new BytesList();

        // 写入 StatusCode
        buffer.Write(this.StatusCode);
        buffer.WriteLn();  // 在文本情况下方便阅读


        // 写响应头，先写长度，再写内容
        byte[] b1 = Encoding.UTF8.GetBytes(headers);
        buffer.Write(b1.Length);

        buffer.WriteLn();  // 在文本情况下方便阅读
        buffer.Write(b1);


        // 写消息体
        byte[] b2 = body;

        buffer.Write(b2.Length);

        buffer.WriteLn();  // 在文本情况下方便阅读
        buffer.Write(b2);

        return buffer;
    }

    byte[] IBinarySerializer.ToBytes()
    {
        BytesList buffer = ToBytesList();

        return buffer.ToArray();
    }

    void IBinarySerializer.LoadData(ReadOnlyMemory<byte> body)
    {
        CheckStreamNotSupportSerialize();

        this.StatusCode = -1;
        this.Headers = new NameValueCollection();
        this.Result = default(T);

        if( body.Length == 0 )
            return;

        int start = 0;
        ReadOnlySpan<byte> span = body.Span;

        this.StatusCode = BitConverter.ToInt32(span.Slice(start, 4));
        start += 5;  // 5 = 4 + 1

        // 读取“响应头”的长度
        int len = BitConverter.ToInt32(span.Slice(start, 4));
        start += 5;  // 5 = 4 + 1

        // 读取“响应头” 二进制数据
        ReadOnlySpan<byte> data = span.Slice(start, len);
        start += len;

        string header = Encoding.UTF8.GetString(data);
        FillNameValueCollection(header, this.Headers);

        // -------------------------------------------------------

        // 读取“响应体”的长度
        len = BitConverter.ToInt32(span.Slice(start, 4));
        start += 5;  // 5 = 4 + 1

        if( len > 0 ) {
            // 读取“响应体” 二进制数据
            data = span.Slice(start, len);

            if( typeof(T) == typeof(byte[]) ) {
                byte[] bb = data.ToArray();
                this.Result = (T)(object)bb;
            }
            else {
                string text = Encoding.UTF8.GetString(data);
                if( typeof(T) == typeof(string) ) {
                    this.Result = (T)(object)text;
                }
                else {
                    this.Result = text.FromJson<T>();
                }
            }
        }
    }
}
#endif
