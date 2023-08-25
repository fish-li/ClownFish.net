using ClownFish.Base.Json;

namespace ClownFish.Base;

/// <summary>
/// JSON序列化的工具类
/// </summary>
public static class JsonExtensions
{
    internal static string Serialize0(object obj, JsonSerializerSettings settings)
    {
        if( obj == null )
            throw new ArgumentNullException(nameof(obj));


        JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);

        StringBuilder sb = StringBuilderPool.Get();
        try {
            using( StringWriter stringWriter = new StringWriter(sb) ) {
                using( JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter) ) {
                    jsonTextWriter.Formatting = jsonSerializer.Formatting;
                    jsonSerializer.Serialize(jsonTextWriter, obj);
                }
                return stringWriter.ToString();
            }
        }
        finally {
            StringBuilderPool.Return(sb);
        }

        // 说明：不使用下面代码的原因是它在内部每次会创建一个 new StringBuilder(256)，性能不理想！
        //return JsonConvert.SerializeObject(obj, settings);
    }



    /// <summary>
    /// 将一个对象序列化为JSON字符串。
    /// </summary>
    /// <param name="list">要序列化的对象</param>
    /// <param name="style"></param>
    /// <returns>序列化得到的JSON字符串</returns>
    public static string ToMultiLineJson<T>(this IEnumerable<T> list, JsonStyle style = JsonStyle.None)
    {
        JsonSerializerSettings settings = JsonSerializerSettingsUtils.Get(style);

        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( var x in list ) {
                sb.Append(Serialize0(x, settings));
                sb.Append('\n');

                // 最后以“换行符”结束，这里参考了 elasticsearch 的要求
                // The final line of data must end with a newline character \n
                // https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-bulk.html#docs-bulk-api-desc
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    /// 将一个对象序列化为JSON字符串。
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <param name="style">JSON序列化格式</param>
    /// <returns>序列化得到的JSON字符串</returns>
    public static string ToJson(this object obj, JsonStyle style)
    {
        JsonSerializerSettings settings = JsonSerializerSettingsUtils.Get(style);
        return Serialize0(obj, settings);
    }


    /// <summary>
    /// 将一个对象序列化为JSON字符串。
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <param name="settings">JsonSerializerSettings instance</param>
    /// <returns>序列化得到的JSON字符串</returns>
    public static string ToJson(this object obj, JsonSerializerSettings settings = null)
    {
        JsonSerializerSettings settings2 = settings ?? JsonSerializerSettingsUtils.Get(JsonStyle.None);
        return Serialize0(obj, settings2);
    }


    /// <summary>
    /// 将一个多行JSON字符串反序列化为列表对象
    /// </summary>
    /// <typeparam name="T">反序列的对象类型参数</typeparam>
    /// <param name="multiLineJson">以换行符为分隔的多行JSON字符串</param>
    /// <param name="capacity">返回列表的初始容量</param>
    /// <param name="settings"></param>
    /// <returns>反序列化得到的结果</returns>
    public static List<T> FromMultiLineJson<T>(this string multiLineJson, int capacity = 32, JsonSerializerSettings settings = null)
    {
        if( multiLineJson == null )
            return default(List<T>);

        if( multiLineJson.Length == 0 )
            return new List<T>();


        JsonSerializerSettings settings2 = settings ?? JsonSerializerSettingsUtils.Get(JsonStyle.None);
        List<T> list = new List<T>(capacity);

        using( StringReader reader = new StringReader(multiLineJson) ) {
            while( true ) {
                string line = reader.ReadLine();
                if( line.IsNullOrEmpty() )
                    break;

                T log = JsonConvert.DeserializeObject<T>(line, settings2);
                list.Add(log);
            }
        }

        return list;
    }


    /// <summary>
    /// 将一个JSON字符串反序列化为对象
    /// </summary>
    /// <typeparam name="T">反序列的对象类型参数</typeparam>
    /// <param name="json">JSON字符串</param>
    /// <param name="settings"></param>
    /// <returns>反序列化得到的结果</returns>
    public static T FromJson<T>(this string json, JsonSerializerSettings settings = null)
    {
        if( string.IsNullOrEmpty(json) )
            return default(T);

        JsonSerializerSettings settings2 = settings ?? JsonSerializerSettingsUtils.Get(JsonStyle.None);
        return JsonConvert.DeserializeObject<T>(json, settings2);
    }


    /// <summary>
    /// 将一个JSON字符串反序列化为对象
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <param name="destType">反序列的对象类型参数</param>
    /// <param name="settings"></param>
    /// <returns>反序列化得到的结果</returns>
    public static object FromJson(this string json, Type destType, JsonSerializerSettings settings = null)
    {
        JsonSerializerSettings settings2 = settings ?? JsonSerializerSettingsUtils.Get(JsonStyle.None);
        return JsonConvert.DeserializeObject(json, destType, settings2);
    }


    /// <summary>
    /// 根据指定的JsonStyle创建一个JsonSerializerSettings实例
    /// </summary>
    /// <param name="style"></param>
    /// <returns></returns>
    public static JsonSerializerSettings ToJsonSerializerSettings(this JsonStyle style)
    {
        return JsonSerializerSettingsUtils.Get(style);
    }



    /// <summary>
    /// 采用JSON序列化反序列化的方式克隆对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T JsonCloneObject<T>(this T obj)
    {
        return obj.ToJson().FromJson<T>();
    }
}
