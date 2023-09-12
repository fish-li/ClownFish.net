namespace ClownFish.MQ.Messages;

#if NETCOREAPP

/// <summary>
/// 消息的二进制序列化处理类
/// </summary>
public sealed class MessageBinSerializer
{
    /// <summary>
    /// 单例引用
    /// </summary>
    public static readonly MessageBinSerializer Instance = new MessageBinSerializer();


    /// <summary>
    /// 将一个数据对象转换成符合队列要求的消息格式。
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public byte[] Serialize(object data)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( data is string s ) {
            return Encoding.UTF8.GetBytes(s);
        }

        if( data is ReadOnlyMemory<byte> mem ) {
            return mem.ToArray();
        }

        if( data is byte[] bytes ) {
            return bytes;
        }

        if( data is NHttpRequest request ) {
            RequestData requestData = RequestData.Create(request);
            return (requestData as IBinarySerializer).ToBytes();
        }

        if( data is IBinarySerializer data2 ) {
            return data2.ToBytes();
        }

        string json = data.ToJson();
        return Encoding.UTF8.GetBytes(json);
    }


    /// <summary>
    /// 将二进制消息转换指定的对象实例。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="body"></param>
    /// <returns></returns>
    public T Deserialize<T>(ReadOnlyMemory<byte> body)
    {
        if( body.IsEmpty )
            return default(T);

        Type targetType = typeof(T);

        if( targetType == typeof(ReadOnlyMemory<byte>) ) {
            return (T)(object)body;
        }

        if( targetType == typeof(byte[]) ) {
            return (T)(object)body.ToArray();
        }

        if( targetType == typeof(NHttpRequest) || targetType == typeof(HttpRequestAlone) ) {
            RequestData data = new RequestData();
            (data as IBinarySerializer).LoadData(body);

            HttpRequestAlone request = new HttpRequestAlone(data);
            return (T)(object)request;
        }

        if( typeof(IBinarySerializer).IsAssignableFrom(targetType) ) {
            IBinarySerializer obj = (IBinarySerializer)Activator.CreateInstance(typeof(T));
            obj.LoadData(body);
            return (T)obj;
        }

        string text = Encoding.UTF8.GetString(body.Span);

        if( targetType == typeof(string) ) {
            return (T)(object)text;
        }

        return text.FromJson<T>();
    }

}

#endif
