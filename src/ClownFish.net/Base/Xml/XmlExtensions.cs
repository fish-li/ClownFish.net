namespace ClownFish.Base;

/// <summary>
/// XML序列化与反序列化的扩展方法类
/// </summary>
public static class XmlExtensions
{

    /// <summary>
    /// 将对象执行XML序列化（使用UTF-8编码）
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <returns>XML序列化的结果</returns>
    public static string ToXml(this object obj)
    {
        return XmlHelper.XmlSerialize(obj, Encoding.UTF8);
    }


    /// <summary>
    /// 将对象执行XML序列化（使用UTF-8编码），此方法尽量产生精简的XML
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <returns>XML序列化的结果</returns>
    public static string ToXml2(this object obj)
    {
        return XmlHelper.XmlSerializerObject(obj);
    }


    /// <summary>
    /// 从XML字符串中反序列化对象（使用UTF-8编码）
    /// </summary>
    /// <typeparam name="T">反序列化的结果类型</typeparam>
    /// <param name="xml">XML字符串</param>
    /// <returns>反序列化的结果</returns>
    public static T FromXml<T>(this string xml)
    {
        return XmlHelper.XmlDeserialize<T>(xml);
    }


    /// <summary>
    ///  从XML字符串中反序列化对象（使用UTF-8编码）
    /// </summary>
    /// <param name="s"></param>
    /// <param name="type">反序列化的结果类型</param>
    /// <returns></returns>
    public static object FromXml(this string s, Type type)
    {
        return XmlHelper.XmlDeserialize(s, type, Encoding.UTF8);
    }


}
