using ClownFish.Base.Json;

namespace ClownFish.Base;

/// <summary>
/// JSON序列化的工具类
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// 当前JSON序列化的默认配置参数是否使用 CamelCase 风格
    /// </summary>
    public static bool DefaultCamelCase => ClownFishOptions.JsonSerializer_CamelCase;

    /// <summary>
    /// 将 JsonStyle 转成 JsonSerializerSettings
    /// </summary>
    /// <param name="style"></param>
    /// <returns></returns>
    public static JsonSerializerSettings ToSettings(this JsonStyle style)
    {
        return JsonSerializerSettingsUtils.Get(style);
    }


    internal static JsonSerializer CreateJsonSerializer(this JsonSerializerSettings settings)
    {
        JsonSerializerSettings settings2 = settings ?? JsonSerializerSettingsUtils.Get(JsonStyle.None);

        // 这里不使用 CreateDefault 方法，因为实际项目中没法预料将 Newtonsoft.Json.JsonConvert.DefaultSettings 设置成什么样子，
        // 它可能会导致框架不能预期工作~~~

        JsonSerializer jsonSerializer = ClownFishOptions.JsonSerializer_CreateDefault
                                        ? JsonSerializer.CreateDefault(settings2) // 默认不启用
                                        : JsonSerializer.Create(settings2);

        return jsonSerializer;
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
        return ToJson(obj, settings);
    }


    /// <summary>
    /// 将一个对象序列化为JSON字符串。
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <param name="settings">JsonSerializerSettings instance</param>
    /// <returns>序列化得到的JSON字符串</returns>
    public static string ToJson(this object obj, JsonSerializerSettings settings = null)
    {
        if( obj == null )
            return null;


        JsonSerializer jsonSerializer = settings.CreateJsonSerializer();

        StringBuilder sb = StringBuilderPool.Get();
        try {
            using( StringWriter stringWriter = new StringWriter(sb) ) {
                using( JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter) ) {
                    jsonTextWriter.Formatting = jsonSerializer.Formatting;
                    jsonSerializer.Serialize(jsonTextWriter, obj);
                    jsonTextWriter.Flush();
                }
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }

        // 说明：不使用下面代码的原因是它在内部每次会创建一个 new StringBuilder(256)，性能不理想！
        //return JsonConvert.SerializeObject(obj, settings);
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
        if( string.IsNullOrEmpty(json) )
            return default(object);

        JsonSerializerSettings settings2 = settings ?? JsonSerializerSettingsUtils.Get(JsonStyle.None);
        return JsonConvert.DeserializeObject(json, destType, settings2);
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
