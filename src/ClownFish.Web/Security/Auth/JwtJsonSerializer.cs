namespace ClownFish.Web.Security.Auth;

/// <summary>
/// 用于JWT的JSON序列化工具类
/// </summary>
internal sealed class JwtJsonSerializer
{
    private readonly JsonSerializerSettings _jsonSettings;

    public JwtJsonSerializer(bool useShortTypeName)
    {
        _jsonSettings = GetJwtSerializerSettings(useShortTypeName);
    }

    internal static JsonSerializerSettings GetJwtSerializerSettings(bool useShortTypeName)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();

        // 保存类型全名，用于反序列化
        settings.TypeNameHandling = TypeNameHandling.Auto;

        // 忽略NULL值成员
        settings.NullValueHandling = NullValueHandling.Ignore;

        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;

        if( useShortTypeName )
            settings.SerializationBinder = JwtJsonUserTypesBinder.Instance;
        else
            settings.SerializationBinder = JwtJsonUserTypesBinder2.Instance;

        return settings;
    }

    public string Serialize(object data)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        return data.ToJson(_jsonSettings);
    }

    public T Deserialize<T>(string json)
    {
        if( json.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(json));

        return json.FromJson<T>(_jsonSettings);
    }
}
