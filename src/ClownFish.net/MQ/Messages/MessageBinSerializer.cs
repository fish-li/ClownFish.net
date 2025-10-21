using Void = ClownFish.Base.Void;

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

        if( targetType == typeof(Void) ) {
            return (T)(object)Void.Value;   // 其实就是不需要做反序列化，MessageHandler可以直接访问原始数据流
        }

        if( targetType == typeof(ReadOnlyMemory<byte>) || targetType == typeof(byte[]) ) {
            throw new NotSupportedException();
        }

        if( targetType == typeof(NHttpRequest) || targetType == typeof(HttpRequestAlone) ) {
            return (T)(object)HttpRequestAlone.Create(body);
        }

        if( typeof(IBinarySerializer).IsAssignableFrom(targetType) ) {
            return CreateObjectFromBinary<T>(body);
        }

        if( targetType.IsSuitableDeserialize() ) {   // 如果 “返回值类型” 适合做反序列化，就直接做JSON反序列化
            //using( Stream stream = body.AsStream() ) {  //using CommunityToolkit.HighPerformance;

            using( Stream stream = body.AsReadonlyStream() ) {
                using StreamReader reader = new StreamReader(stream);
                return (T)reader.FromJson(targetType);
            }
        }

        string text = Encoding.UTF8.GetString(body.Span);

        if( targetType == typeof(string) ) {
            return (T)(object)text;
        }

        return text.FromJson<T>();
    }

#if NETCOREAPP
    [UnconditionalSuppressMessage("TrimAnalyzer", "IL2087: Activator.CreateInstance")]
#endif
    internal static T CreateObjectFromBinary<T>(ReadOnlyMemory<byte> body)
    {
        IBinarySerializer obj = (IBinarySerializer)Activator.CreateInstance(typeof(T));
        obj.LoadData(body);
        return (T)obj;
    }


}

#endif
