namespace ClownFish.Base.Json;

/// <summary>
/// 支持XmlCdata的Json序列化包装类
/// </summary>
internal sealed class XmlCdataJsonConverter : JsonConverter
{
    internal static readonly XmlCdataJsonConverter Instance = new XmlCdataJsonConverter();

    /// <summary>
    /// 重写JSON.NET的方法
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(XmlCdata);
    }


    /// <summary>
    /// 重写JSON.NET的方法
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="objectType"></param>
    /// <param name="existingValue"></param>
    /// <param name="serializer"></param>
    /// <returns></returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if( objectType == typeof(XmlCdata) ) {
            if( reader.Value != null ) {
                return new XmlCdata(reader.Value.ToString());
            }
        }
        return null;
    }



    /// <summary>
    /// 重写JSON.NET的方法
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="serializer"></param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if( value is XmlCdata data ) {
            writer.WriteValue(data.Value);
        }
    }

}
