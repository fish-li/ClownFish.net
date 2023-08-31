namespace ClownFish.Base.Reflection;

/// <summary>
/// 反射相关工具类
/// </summary>
public static class ReflectionUtils
{
    /// <summary>
    /// 用反射方式读取一个对象的属性值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static T Get<T>(this object data, string propName)
    {
        PropertyInfo p = data.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
        if( p == null )
            throw new ArgumentOutOfRangeException(nameof(propName));

        return (T)p.FastGetValue(data);
    }
}

