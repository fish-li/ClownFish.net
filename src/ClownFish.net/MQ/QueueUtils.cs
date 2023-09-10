namespace ClownFish.MQ;
#if NET6_0_OR_GREATER

/// <summary>
/// Queue相关工具类
/// </summary>
public static class QueueUtils
{
    /// <summary>
    /// 消息的最小长度，小于这个长度的消息会认为是无效的消息，它将被丢弃
    /// </summary>
    public static readonly int MinMessageLength = LocalSettings.GetUInt("ClownFish_MQ_MessageLength_Min", 5);


    /// <summary>
    /// 获取指定数据类型的默认队列名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetQueueName(this Type type)
    {
        if( type.IsGenericType )      // 不支持泛型主要是得到的名称会包含很多特殊字符
            throw new NotSupportedException("消息对象类型不支持泛型。");

        if( type.IsArray )
            throw new NotSupportedException("消息对象类型不支持数组。");

        if( type.IsPrimitive || type == typeof(string) )
            throw new NotSupportedException("DOTNET内置类型不应该做为消息对象。");

        return type.FullName;
    }

}
#endif
