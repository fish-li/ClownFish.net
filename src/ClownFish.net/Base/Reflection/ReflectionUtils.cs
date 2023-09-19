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


    /// <summary>
    /// 用反射的方式查找一个类型，然后调用它的一个【静态无参方法】
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static int CallStaticMethod(string typeFullName, string methodName)
    {
        Type type = Type.GetType(typeFullName, false, true);
        if( type == null )
            return -1;

        MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if( method == null )
            return -2;

        method.Invoke(null, null);
        return 1;
    }
}

