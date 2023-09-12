namespace ClownFish.MQ.Messages;

#if NETCOREAPP

/// <summary>
/// 定义一些与ComplexMessage相关的常量
/// </summary>
public static class ComplexMessage
{
    internal static readonly string DividingLine = "\n--e6805900f0e74e589295fb6d0966f873--\n";

    /// <summary>
    /// MessageId
    /// </summary>
    public static readonly string MessageId = "MessageId";

    /// <summary>
    /// CreateTime
    /// </summary>
    public static readonly string CreateTime = "CreateTime";

    /// <summary>
    /// TenantId
    /// </summary>
    public static readonly string TenantId = "TenantId";        
}

/// <summary>
/// 描述一种复合消息体结构，此结构包含消息头和消息体。
/// </summary>
/// <typeparam name="T">消息体的数据类型</typeparam>
public sealed class ComplexMessage<T> : ITextSerializer, IBinarySerializer, IMsgObject
    where T : class
{

    /// <summary>
    /// 消息头
    /// </summary>
    public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

    /// <summary>
    /// 消息体
    /// </summary>
    public T Body { get; private set; }

    DateTime IMsgObject.GetTime() => DateTime.Parse(this.Headers[ComplexMessage.CreateTime]);

    string IMsgObject.GetId() => this.Headers[ComplexMessage.MessageId] ?? string.Empty;

    /// <summary>
    /// 构造方法，给反序列化使用。
    /// </summary>
    public ComplexMessage()
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="body"></param>
    public ComplexMessage(T body)
    {
        if( body == null )
            throw new ArgumentNullException(nameof(body));

        this.Body = body;

        this.Headers[ComplexMessage.CreateTime] = DateTime.Now.ToTimeString();
        this.Headers[ComplexMessage.MessageId] = Guid.NewGuid().ToString("N");
    }


    private void Validate()
    {
        if( this.Headers.Count == 0 )
            throw new InvalidDataException("没有指定消息头。如果确实不需要消息头，请不要使用这个类型。");

        if( this.Body == null )
            throw new InvalidDataException("消息体为空，不能执行序列化。");

        if( typeof(T) == typeof(string) ) {
            string value = this.Body.ToString();
            if( value.Length == 0 )
                throw new InvalidDataException("消息体为空，不能执行序列化。");
        }
    }

    private string GetBodyAsString()
    {
        if( this.Body == null )
            return null;


        if( typeof(T) == typeof(string) ) {
            return this.Body.ToString();
        }
        else if( typeof(T) == typeof(byte[]) ) {
            byte[] bb = (byte[])(object)this.Body;
            return Convert.ToBase64String(bb);
        }
        else {
            return this.Body.ToJson();
        }
    }

    private T StringToBodyObject(string text)
    {
        if( text.IsNullOrEmpty() )
            return null;


        if( typeof(T) == typeof(string) ) {
            return (T)(object)text;
        }
        else if( typeof(T) == typeof(byte[]) ) {
            byte[] bb = Convert.FromBase64String(text);
            return (T)(object)bb;
        }
        else {
            return text.FromJson<T>();
        }
    }

    string ITextSerializer.ToText()
    {
        Validate();

        string header = this.Headers.ToJson(JsonStyle.Indented);
        string body = GetBodyAsString();

        return header + ComplexMessage.DividingLine + body;
    }

    void ITextSerializer.LoadData(string text)
    {
        if( text.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(text));

        int p = text.IndexOf(ComplexMessage.DividingLine, StringComparison.Ordinal);
        if( p < 0 )
            throw new InvalidDataException("消息内容格式不正确，不能执行反序列化。");

        string header = text.Substring(0, p);
        string body = text.Substring(p + ComplexMessage.DividingLine.Length);

        this.Headers = header.FromJson<Dictionary<string, string>>();
        this.Body = StringToBodyObject(body);
    }

    byte[] IBinarySerializer.ToBytes()
    {
        Validate();

        string header = this.Headers.ToJson();

        if( typeof(T) == typeof(byte[]) ) {
            byte[] bodyBytes = (byte[])(object)this.Body;     // 直接使用二进制数据
            return ToBytes0(header, bodyBytes);
        }
        else {
            string body = GetBodyAsString();
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);  // 先转成string（有可能会做JSON序列化），再转byte[]
            return ToBytes0(header, bodyBytes);
        }
    }


    private static byte[] ToBytes0(string header, byte[] bodyBytes)
    {
        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {

            // 写消息头，先写长度，再写内容
            byte[] b1 = Encoding.UTF8.GetBytes(header);
            byte[] lenBytes = BitConverter.GetBytes(b1.Length);  // 长度固定为 4
            ms.Write(lenBytes, 0, lenBytes.Length);
            ms.WriteByte((byte)'\n');  // 在文本情况下方便阅读
            ms.Write(b1, 0, b1.Length);


            // 写消息体
            byte[] b2 = bodyBytes;
            lenBytes = BitConverter.GetBytes(b2.Length);  // 长度固定为 4
            ms.Write(lenBytes, 0, lenBytes.Length);
            ms.WriteByte((byte)'\n');  // 在文本情况下方便阅读
            ms.Write(b2, 0, b2.Length);

            return ms.ToArray();
        }
    }


    void IBinarySerializer.LoadData(ReadOnlyMemory<byte> body)
    {
        if( body.Length == 0 )
            throw new ArgumentNullException(nameof(body));


        int start = 0;
        ReadOnlySpan<byte> span = body.Span;


        // 读取“消息头”的长度
        int len = BitConverter.ToInt32(span.Slice(start, 4));
        start += 4;
        start++; // 跳过 \n 符号

        // 读取“消息头” 二进制数据
        ReadOnlySpan<byte> data = span.Slice(start, len);
        start += len;

        string header = Encoding.UTF8.GetString(data);
        this.Headers = header.FromJson<Dictionary<string, string>>();

        // -------------------------------------------------------

        // 读取“消息体”的长度
        len = BitConverter.ToInt32(span.Slice(start, 4));
        start += 4;

        if( len > 0 ) {
            // 跳过 \n 符号
            start++;
            // 读取“消息体” 二进制数据
            data = span.Slice(start, len);

            if( typeof(T) == typeof(byte[]) ) {
                byte[] bb = data.ToArray();
                this.Body = (T)(object)bb;
            }
            else {
                string text = Encoding.UTF8.GetString(data);
                this.Body = StringToBodyObject(text);
            }
        }
    }
}
#endif
