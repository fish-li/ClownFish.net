namespace ClownFish.Base;

/// <summary>
/// ExpandoObject扩展方法类
/// </summary>
public static class ExpandoObjectExtensions
{
    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="data"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public static void Set(this ExpandoObject data, string name, object value)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        ((IDictionary<string, object>)data)[name] = value;
    }


    /// <summary>
    /// 获取指定名称对应的值
    /// </summary>
    /// <param name="data"></param>
    /// <param name="name"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static object Get(this ExpandoObject data, string name, bool checkExist)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        object value;
        if( ((IDictionary<string, object>)data).TryGetValue(name, out value) )
            return value;

        if( checkExist )
            throw new ArgumentOutOfRangeException(nameof(name));

        return null;
    }
}
