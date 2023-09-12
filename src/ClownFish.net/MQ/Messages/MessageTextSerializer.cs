namespace ClownFish.MQ.Messages;

#if NETCOREAPP

/// <summary>
/// 消息的文本序列化处理类
/// </summary>
public sealed class MessageTextSerializer
{
    /// <summary>
    /// 单例引用
    /// </summary>
    public static readonly MessageTextSerializer Instance = new MessageTextSerializer();


    /// <summary>
    /// 将一个数据对象转换成符合队列要求的消息格式。
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string Serialize(object data)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));


        if( data is string s ) {
            return s;
        }

        if( data is byte[] bb ) {
            return Convert.ToBase64String(bb);
        }

        if( data is NHttpRequest request ) {
            RequestData requestData = RequestData.Create(request);
            return (requestData as ITextSerializer).ToText();
        }

        if( data is ITextSerializer data2 ) {
            return data2.ToText();
        }

        return data.ToJson();
    }


    /// <summary>
    /// 将文本消息转换指定的对象实例。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="body"></param>
    /// <returns></returns>
    public T Deserialize<T>(string body)
    {
        if( body == null )
            throw new ArgumentNullException(nameof(body));

        Type targetType = typeof(T);

        if( targetType == typeof(string) ) {
            return (T)(object)body;
        }

        if( targetType == typeof(byte[]) ) {
            return (T)(object)Convert.FromBase64String(body);
        }

        if( targetType == typeof(NHttpRequest) || targetType == typeof(HttpRequestAlone) ) {
            RequestData data = new RequestData();
            (data as ITextSerializer).LoadData(body);

            HttpRequestAlone request = new HttpRequestAlone(data);
            return (T)(object)request;
        }

        if( typeof(ITextSerializer).IsAssignableFrom(targetType) ) {
            ITextSerializer obj = (ITextSerializer)Activator.CreateInstance(typeof(T));
            obj.LoadData(body);
            return (T)obj;
        }

        return body.FromJson<T>();
    }



}
#endif
